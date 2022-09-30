using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class ClsComplianceChecker : CSharpSymbolVisitor
    {
        private enum Compliance
        {
            DeclaredTrue,
            DeclaredFalse,
            InheritedTrue,
            InheritedFalse,
            ImpliedFalse
        }

        private readonly CSharpCompilation _compilation;

        private readonly SyntaxTree _filterTree;

        private readonly TextSpan? _filterSpanWithinTree;

        private readonly BindingDiagnosticBag _diagnostics;

        private readonly CancellationToken _cancellationToken;

        private readonly ConcurrentDictionary<Symbol, Compliance> _declaredOrInheritedCompliance;

        private readonly ConcurrentStack<Task> _compilerTasks;

        private bool ConcurrentAnalysis
        {
            get
            {
                if (_filterTree == null)
                {
                    return _compilation.Options.ConcurrentBuild;
                }
                return false;
            }
        }

        private ClsComplianceChecker(CSharpCompilation compilation, SyntaxTree filterTree, TextSpan? filterSpanWithinTree, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _filterTree = filterTree;
            _filterSpanWithinTree = filterSpanWithinTree;
            _diagnostics = diagnostics;
            _cancellationToken = cancellationToken;
            _declaredOrInheritedCompliance = new ConcurrentDictionary<Symbol, Compliance>(Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything);
            if (ConcurrentAnalysis)
            {
                _compilerTasks = new ConcurrentStack<Task>();
            }
        }

        public static void CheckCompliance(CSharpCompilation compilation, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken, SyntaxTree filterTree = null, TextSpan? filterSpanWithinTree = null)
        {
            BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(diagnostics.DiagnosticBag, diagnostics.AccumulatesDependencies ? new ConcurrentSet<AssemblySymbol>() : null);
            ClsComplianceChecker clsComplianceChecker = new ClsComplianceChecker(compilation, filterTree, filterSpanWithinTree, bindingDiagnosticBag, cancellationToken);
            clsComplianceChecker.Visit(compilation.Assembly);
            clsComplianceChecker.WaitForWorkers();
            diagnostics.AddDependencies(bindingDiagnosticBag);
        }

        public override void VisitAssembly(AssemblySymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
            if (declaredOrInheritedCompliance == Compliance.DeclaredFalse)
            {
                return;
            }
            bool flag = IsTrue(declaredOrInheritedCompliance);
            for (int i = 0; i < symbol.Modules.Length; i++)
            {
                ModuleSymbol moduleSymbol = symbol.Modules[i];
                bool? declaredCompliance = GetDeclaredCompliance(moduleSymbol, out Location attributeLocation);
                Location location = ((i == 0) ? attributeLocation : moduleSymbol.Locations[0]);
                if (declaredCompliance.HasValue)
                {
                    if (location != null)
                    {
                        if (!IsDeclared(declaredOrInheritedCompliance))
                        {
                            AddDiagnostic(ErrorCode.WRN_CLS_NotOnModules, location);
                        }
                        else if (flag != declaredCompliance.GetValueOrDefault())
                        {
                            AddDiagnostic(ErrorCode.WRN_CLS_NotOnModules2, location);
                        }
                    }
                }
                else
                {
                    if (!flag || i <= 0)
                    {
                        continue;
                    }
                    bool flag2 = false;
                    PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)moduleSymbol;
                    ImmutableArray<CSharpAttributeData>.Enumerator enumerator = pEModuleSymbol.GetAssemblyAttributes().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.IsTargetAttribute(pEModuleSymbol, AttributeDescription.CLSCompliantAttribute))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_ModuleMissingCLS, location);
                    }
                }
            }
            if (flag)
            {
                CheckForAttributeWithArrayArgument(symbol);
            }
            ModuleSymbol symbol2 = symbol.Modules[0];
            if (IsTrue(GetDeclaredOrInheritedCompliance(symbol2)))
            {
                CheckForAttributeWithArrayArgument(symbol2);
            }
            Visit(symbol.GlobalNamespace);
        }

        private void WaitForWorkers()
        {
            ConcurrentStack<Task> compilerTasks = _compilerTasks;
            if (compilerTasks != null)
            {
                while (compilerTasks.TryPop(out Task result))
                {
                    result.GetAwaiter().GetResult();
                }
            }
        }

        public override void VisitNamespace(NamespaceSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (!DoNotVisit(symbol))
            {
                if (IsTrue(GetDeclaredOrInheritedCompliance(symbol)))
                {
                    CheckName(symbol);
                    CheckMemberDistinctness(symbol);
                }
                if (ConcurrentAnalysis)
                {
                    VisitNamespaceMembersAsTasks(symbol);
                }
                else
                {
                    VisitNamespaceMembers(symbol);
                }
            }
        }

        private void VisitNamespaceMembersAsTasks(NamespaceSymbol symbol)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol i = enumerator.Current;
                _compilerTasks.Push(Task.Run(UICultureUtilities.WithCurrentUICulture(delegate
                {
                    try
                    {
                        Visit(i);
                    }
                    catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception))
                    {
                        throw ExceptionUtilities.Unreachable;
                    }
                }), _cancellationToken));
            }
        }

        private void VisitNamespaceMembers(NamespaceSymbol symbol)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                Visit(current);
            }
        }

        public override void VisitNamedType(NamedTypeSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (DoNotVisit(symbol))
            {
                return;
            }
            Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
            if (VisitTypeOrMember(symbol, declaredOrInheritedCompliance) && IsTrue(declaredOrInheritedCompliance))
            {
                CheckBaseTypeCompliance(symbol);
                CheckTypeParameterCompliance(symbol.TypeParameters, symbol);
                if (symbol.TypeKind == TypeKind.Delegate)
                {
                    CheckParameterCompliance(symbol.DelegateInvokeMethod.Parameters, symbol);
                }
                else if (_compilation.IsAttributeType(symbol) && !HasAcceptableAttributeConstructor(symbol))
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadAttributeType, symbol.Locations[0], symbol);
                }
            }
            ImmutableArray<Symbol>.Enumerator enumerator = symbol.GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                Visit(current);
            }
        }

        private bool HasAcceptableAttributeConstructor(NamedTypeSymbol attributeType)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = attributeType.InstanceConstructors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (!IsTrue(GetDeclaredOrInheritedCompliance(current)) || !IsAccessibleIfContainerIsAccessible(current))
                {
                    continue;
                }
                bool flag = false;
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator2 = current.ParameterTypesWithAnnotations.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeWithAnnotations current2 = enumerator2.Current;
                    if (current2.TypeKind == TypeKind.Array || current2.Type.GetAttributeParameterTypedConstantKind(_compilation) == TypedConstantKind.Error)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return true;
                }
            }
            return false;
        }

        public override void VisitMethod(MethodSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (DoNotVisit(symbol))
            {
                return;
            }
            Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
            if (symbol.IsAccessor())
            {
                CheckForAttributeOnAccessor(symbol);
                CheckForMeaninglessOnParameter(symbol.Parameters);
                CheckForMeaninglessOnReturn(symbol);
                if (IsTrue(declaredOrInheritedCompliance))
                {
                    CheckForAttributeWithArrayArgument(symbol);
                }
            }
            else if (VisitTypeOrMember(symbol, declaredOrInheritedCompliance) && IsTrue(declaredOrInheritedCompliance))
            {
                CheckParameterCompliance(symbol.Parameters, symbol.ContainingType);
                CheckTypeParameterCompliance(symbol.TypeParameters, symbol.ContainingType);
                if (symbol.IsVararg)
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_NoVarArgs, symbol.Locations[0]);
                }
            }
        }

        private void CheckForAttributeOnAccessor(MethodSymbol symbol)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = symbol.GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                if (current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute) && TryGetAttributeWarningLocation(current, out var location))
                {
                    AttributeUsageInfo attributeUsageInfo = current.AttributeClass!.GetAttributeUsageInfo();
                    AddDiagnostic(ErrorCode.ERR_AttributeNotOnAccessor, location, current.AttributeClass!.Name, attributeUsageInfo.GetValidTargetsErrorArgument());
                    break;
                }
            }
        }

        public override void VisitProperty(PropertySymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (!DoNotVisit(symbol))
            {
                Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
                if (VisitTypeOrMember(symbol, declaredOrInheritedCompliance) && IsTrue(declaredOrInheritedCompliance))
                {
                    CheckParameterCompliance(symbol.Parameters, symbol.ContainingType);
                }
            }
        }

        public override void VisitEvent(EventSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (!DoNotVisit(symbol))
            {
                Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
                VisitTypeOrMember(symbol, declaredOrInheritedCompliance);
            }
        }

        public override void VisitField(FieldSymbol symbol)
        {
            CancellationToken cancellationToken = _cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (!DoNotVisit(symbol))
            {
                Compliance declaredOrInheritedCompliance = GetDeclaredOrInheritedCompliance(symbol);
                if (VisitTypeOrMember(symbol, declaredOrInheritedCompliance) && IsTrue(declaredOrInheritedCompliance) && symbol.IsVolatile)
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_VolatileField, symbol.Locations[0], symbol);
                }
            }
        }

        private bool VisitTypeOrMember(Symbol symbol, Compliance compliance)
        {
            SymbolKind kind = symbol.Kind;
            if (!CheckForDeclarationWithoutAssemblyDeclaration(symbol, compliance))
            {
                return false;
            }
            bool flag = IsTrue(compliance);
            bool flag2 = IsAccessibleOutsideAssembly(symbol);
            if (flag2)
            {
                if (flag)
                {
                    CheckName(symbol);
                    CheckForCompliantWithinNonCompliant(symbol);
                    CheckReturnTypeCompliance(symbol);
                    if (symbol.Kind == SymbolKind.NamedType)
                    {
                        CheckMemberDistinctness((NamedTypeSymbol)symbol);
                    }
                }
                else if (GetDeclaredOrInheritedCompliance(symbol.ContainingAssembly) == Compliance.DeclaredTrue && IsTrue(GetInheritedCompliance(symbol)))
                {
                    CheckForNonCompliantAbstractMember(symbol);
                }
            }
            else if (IsDeclared(compliance))
            {
                AddDiagnostic(ErrorCode.WRN_CLS_MeaninglessOnPrivateType, symbol.Locations[0], symbol);
                return false;
            }
            if (flag)
            {
                CheckForAttributeWithArrayArgument(symbol);
            }
            switch (kind)
            {
                case SymbolKind.NamedType:
                    {
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
                        if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
                        {
                            MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
                            CheckForMeaninglessOnParameter(delegateInvokeMethod.Parameters);
                            CheckForMeaninglessOnReturn(delegateInvokeMethod);
                        }
                        break;
                    }
                case SymbolKind.Method:
                    {
                        MethodSymbol methodSymbol = (MethodSymbol)symbol;
                        CheckForMeaninglessOnParameter(methodSymbol.Parameters);
                        CheckForMeaninglessOnReturn(methodSymbol);
                        break;
                    }
                case SymbolKind.Property:
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)symbol;
                        CheckForMeaninglessOnParameter(propertySymbol.Parameters);
                        break;
                    }
            }
            return flag2;
        }

        private void CheckForNonCompliantAbstractMember(Symbol symbol)
        {
            NamedTypeSymbol containingType = symbol.ContainingType;
            if ((object)containingType != null && containingType.IsInterface)
            {
                AddDiagnostic(ErrorCode.WRN_CLS_BadInterfaceMember, symbol.Locations[0], symbol);
            }
            else if (symbol.IsAbstract && symbol.Kind != SymbolKind.NamedType)
            {
                AddDiagnostic(ErrorCode.WRN_CLS_NoAbstractMembers, symbol.Locations[0], symbol);
            }
        }

        private void CheckBaseTypeCompliance(NamedTypeSymbol symbol)
        {
            if (symbol.IsInterface)
            {
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = symbol.InterfacesNoUseSiteDiagnostics().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamedTypeSymbol current = enumerator.Current;
                    if (!IsCompliantType(current, symbol))
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_BadInterface, symbol.Locations[0], symbol, current);
                    }
                }
            }
            else
            {
                NamedTypeSymbol namedTypeSymbol = symbol.EnumUnderlyingType ?? symbol.BaseTypeNoUseSiteDiagnostics;
                if ((object)namedTypeSymbol != null && !IsCompliantType(namedTypeSymbol, symbol))
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadBase, symbol.Locations[0], symbol, namedTypeSymbol);
                }
            }
        }

        private void CheckForCompliantWithinNonCompliant(Symbol symbol)
        {
            NamedTypeSymbol containingType = symbol.ContainingType;
            if ((object)containingType != null && !IsTrue(GetDeclaredOrInheritedCompliance(containingType)))
            {
                AddDiagnostic(ErrorCode.WRN_CLS_IllegalTrueInFalse, symbol.Locations[0], symbol, containingType);
            }
        }

        private void CheckTypeParameterCompliance(ImmutableArray<TypeParameterSymbol> typeParameters, NamedTypeSymbol context)
        {
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator2 = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TypeWithAnnotations current2 = enumerator2.Current;
                    if (!IsCompliantType(current2.Type, context))
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_BadTypeVar, current.Locations[0], current2.Type);
                    }
                }
            }
        }

        private void CheckParameterCompliance(ImmutableArray<ParameterSymbol> parameters, NamedTypeSymbol context)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (!IsCompliantType(current.Type, context))
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadArgType, current.Locations[0], current.Type);
                }
            }
        }

        private void CheckForAttributeWithArrayArgument(Symbol symbol)
        {
            CheckForAttributeWithArrayArgumentInternal(symbol.GetAttributes());
            if (symbol.Kind == SymbolKind.Method)
            {
                CheckForAttributeWithArrayArgumentInternal(((MethodSymbol)symbol).GetReturnTypeAttributes());
            }
        }

        private void CheckForAttributeWithArrayArgumentInternal(ImmutableArray<CSharpAttributeData> attributes)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = attributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                foreach (TypedConstant constructorArgument in current.ConstructorArguments)
                {
                    if (constructorArgument.TypeInternal!.TypeKind == TypeKind.Array && TryGetAttributeWarningLocation(current, out var location))
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_ArrayArgumentToAttribute, location);
                        return;
                    }
                }
                foreach (KeyValuePair<string, TypedConstant> namedArgument in current.NamedArguments)
                {
                    if (namedArgument.Value.TypeInternal!.TypeKind == TypeKind.Array && TryGetAttributeWarningLocation(current, out var location2))
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_ArrayArgumentToAttribute, location2);
                        return;
                    }
                }
                if ((object)current.AttributeConstructor == null)
                {
                    continue;
                }
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator4 = current.AttributeConstructor!.ParameterTypesWithAnnotations.GetEnumerator();
                while (enumerator4.MoveNext())
                {
                    if (enumerator4.Current.TypeKind == TypeKind.Array && TryGetAttributeWarningLocation(current, out var location3))
                    {
                        AddDiagnostic(ErrorCode.WRN_CLS_ArrayArgumentToAttribute, location3);
                        return;
                    }
                }
            }
        }

        private bool TryGetAttributeWarningLocation(CSharpAttributeData attribute, out Location location)
        {
            SyntaxReference applicationSyntaxReference = attribute.ApplicationSyntaxReference;
            if (applicationSyntaxReference == null && _filterTree == null)
            {
                location = NoLocation.Singleton;
                return true;
            }
            if (_filterTree == null || (applicationSyntaxReference != null && applicationSyntaxReference.SyntaxTree == _filterTree))
            {
                location = new SourceLocation(applicationSyntaxReference);
                return true;
            }
            location = null;
            return false;
        }

        private void CheckForMeaninglessOnParameter(ImmutableArray<ParameterSymbol> parameters)
        {
            if (parameters.IsEmpty)
            {
                return;
            }
            int num = 0;
            Symbol containingSymbol = parameters[0].ContainingSymbol;
            if (containingSymbol.Kind == SymbolKind.Method)
            {
                Symbol associatedSymbol = ((MethodSymbol)containingSymbol).AssociatedSymbol;
                if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Property)
                {
                    num = ((PropertySymbol)associatedSymbol).ParameterCount;
                }
            }
            for (int i = num; i < parameters.Length; i++)
            {
                if (TryGetClsComplianceAttributeLocation(parameters[i].GetAttributes(), parameters[i], out var attributeLocation))
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_MeaninglessOnParam, attributeLocation);
                }
            }
        }

        private void CheckForMeaninglessOnReturn(MethodSymbol method)
        {
            if (TryGetClsComplianceAttributeLocation(method.GetReturnTypeAttributes(), method, out var attributeLocation))
            {
                AddDiagnostic(ErrorCode.WRN_CLS_MeaninglessOnReturn, attributeLocation);
            }
        }

        private void CheckReturnTypeCompliance(Symbol symbol)
        {
            ErrorCode code;
            TypeSymbol type;
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    code = ErrorCode.WRN_CLS_BadFieldPropType;
                    type = ((FieldSymbol)symbol).Type;
                    break;
                case SymbolKind.Property:
                    code = ErrorCode.WRN_CLS_BadFieldPropType;
                    type = ((PropertySymbol)symbol).Type;
                    break;
                case SymbolKind.Event:
                    code = ErrorCode.WRN_CLS_BadFieldPropType;
                    type = ((EventSymbol)symbol).Type;
                    break;
                case SymbolKind.Method:
                    {
                        code = ErrorCode.WRN_CLS_BadReturnType;
                        MethodSymbol methodSymbol = (MethodSymbol)symbol;
                        type = methodSymbol.ReturnType;
                        if (methodSymbol.MethodKind == MethodKind.DelegateInvoke)
                        {
                            symbol = methodSymbol.ContainingType;
                        }
                        break;
                    }
                case SymbolKind.NamedType:
                    symbol = ((NamedTypeSymbol)symbol).DelegateInvokeMethod;
                    if ((object)symbol == null)
                    {
                        return;
                    }
                    goto case SymbolKind.Method;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
            }
            if (!IsCompliantType(type, symbol.ContainingType))
            {
                AddDiagnostic(code, symbol.Locations[0], symbol);
            }
        }

        private bool TryGetClsComplianceAttributeLocation(ImmutableArray<CSharpAttributeData> attributes, Symbol targetSymbol, out Location attributeLocation)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = attributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                if (current.IsTargetAttribute(targetSymbol, AttributeDescription.CLSCompliantAttribute) && TryGetAttributeWarningLocation(current, out attributeLocation))
                {
                    return true;
                }
            }
            attributeLocation = null;
            return false;
        }

        private bool CheckForDeclarationWithoutAssemblyDeclaration(Symbol symbol, Compliance compliance)
        {
            if (IsDeclared(compliance) && !IsDeclared(GetDeclaredOrInheritedCompliance(symbol.ContainingAssembly)))
            {
                ErrorCode code = (IsTrue(compliance) ? ErrorCode.WRN_CLS_AssemblyNotCLS : ErrorCode.WRN_CLS_AssemblyNotCLS2);
                AddDiagnostic(code, symbol.Locations[0], symbol);
                return false;
            }
            return true;
        }

        private void CheckMemberDistinctness(NamespaceOrTypeSymbol symbol)
        {
            MultiDictionary<string, Symbol> multiDictionary = new MultiDictionary<string, Symbol>(CaseInsensitiveComparison.Comparer);
            ImmutableArray<Symbol>.Enumerator enumerator2;
            if (symbol.Kind != SymbolKind.Namespace)
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
                foreach (NamedTypeSymbol key in namedTypeSymbol.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys)
                {
                    if (!IsAccessibleOutsideAssembly(key))
                    {
                        continue;
                    }
                    enumerator2 = key.GetMembersUnordered().GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Symbol current2 = enumerator2.Current;
                        if (IsAccessibleIfContainerIsAccessible(current2) && (!current2.IsOverride || (current2.Kind != SymbolKind.Method && current2.Kind != SymbolKind.Property)))
                        {
                            multiDictionary.Add(current2.Name, current2);
                        }
                    }
                }
                NamedTypeSymbol baseTypeNoUseSiteDiagnostics = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
                while ((object)baseTypeNoUseSiteDiagnostics != null)
                {
                    enumerator2 = baseTypeNoUseSiteDiagnostics.GetMembersUnordered().GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Symbol current3 = enumerator2.Current;
                        if (IsAccessibleOutsideAssembly(current3) && IsTrue(GetDeclaredOrInheritedCompliance(current3)) && (!current3.IsOverride || (current3.Kind != SymbolKind.Method && current3.Kind != SymbolKind.Property)))
                        {
                            multiDictionary.Add(current3.Name, current3);
                        }
                    }
                    baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
                }
            }
            enumerator2 = symbol.GetMembers().GetEnumerator();
            while (enumerator2.MoveNext())
            {
                Symbol current4 = enumerator2.Current;
                if (!DoNotVisit(current4) && IsAccessibleIfContainerIsAccessible(current4) && IsTrue(GetDeclaredOrInheritedCompliance(current4)) && !current4.IsOverride)
                {
                    string name = current4.Name;
                    MultiDictionary<string, Symbol>.ValueSet sameNameSymbols = multiDictionary[name];
                    if (sameNameSymbols.Count > 0)
                    {
                        CheckSymbolDistinctness(current4, name, sameNameSymbols);
                    }
                    multiDictionary.Add(name, current4);
                }
            }
        }

        private void CheckSymbolDistinctness(Symbol symbol, string symbolName, MultiDictionary<string, Symbol>.ValueSet sameNameSymbols)
        {
            bool flag = symbol.Kind == SymbolKind.Method || symbol.Kind == SymbolKind.Property;
            foreach (Symbol item in sameNameSymbols)
            {
                if (item.Name != symbolName && (!flag || item.Kind != symbol.Kind))
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadIdentifierCase, symbol.Locations[0], symbol);
                    return;
                }
            }
            if (!flag)
            {
                return;
            }
            foreach (Symbol item2 in sameNameSymbols)
            {
                if (symbol.Kind == item2.Kind && !symbol.IsAccessor() && !item2.IsAccessor() && TryGetCollisionErrorCode(symbol, item2, out var code))
                {
                    AddDiagnostic(code, symbol.Locations[0], symbol);
                    break;
                }
                if (item2.Name != symbolName)
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadIdentifierCase, symbol.Locations[0], symbol);
                    break;
                }
            }
        }

        private void CheckName(Symbol symbol)
        {
            if (symbol.CanBeReferencedByName && !symbol.IsOverride)
            {
                string name = symbol.Name;
                if (name.Length > 0 && name[0] == '_')
                {
                    AddDiagnostic(ErrorCode.WRN_CLS_BadIdentifier, symbol.Locations[0], name);
                }
            }
        }

        private bool DoNotVisit(Symbol symbol)
        {
            if (symbol.Kind == SymbolKind.Namespace)
            {
                return false;
            }
            if (symbol.DeclaringCompilation == _compilation && !symbol.IsImplicitlyDeclared)
            {
                return IsSyntacticallyFilteredOut(symbol);
            }
            return true;
        }

        private bool IsSyntacticallyFilteredOut(Symbol symbol)
        {
            if (_filterTree != null)
            {
                return !symbol.IsDefinedInSourceTree(_filterTree, _filterSpanWithinTree);
            }
            return false;
        }

        private bool IsCompliantType(TypeSymbol type, NamedTypeSymbol context)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Array:
                    return IsCompliantType(((ArrayTypeSymbol)type).ElementType, context);
                case TypeKind.Dynamic:
                    return true;
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                    return false;
                case TypeKind.Error:
                case TypeKind.TypeParameter:
                    return true;
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Struct:
                case TypeKind.Submission:
                    return IsCompliantType((NamedTypeSymbol)type, context);
                default:
                    throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
            }
        }

        private bool IsCompliantType(NamedTypeSymbol type, NamedTypeSymbol context)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_UIntPtr:
                case SpecialType.System_TypedReference:
                    return false;
                case SpecialType.System_SByte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    return false;
                default:
                    {
                        if (type.TypeKind == TypeKind.Error)
                        {
                            return true;
                        }
                        if (!IsTrue(GetDeclaredOrInheritedCompliance(type.OriginalDefinition)))
                        {
                            return false;
                        }
                        ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (!IsCompliantType(enumerator.Current.Type, context))
                            {
                                return false;
                            }
                        }
                        return !IsInaccessibleBecauseOfConstruction(type, context);
                    }
            }
        }

        private static bool IsInaccessibleBecauseOfConstruction(NamedTypeSymbol type, NamedTypeSymbol context)
        {
            bool flag = type.DeclaredAccessibility.HasProtected();
            bool flag2 = false;
            Dictionary<NamedTypeSymbol, NamedTypeSymbol> dictionary = null;
            NamedTypeSymbol containingType = type.ContainingType;
            while ((object)containingType != null)
            {
                if (dictionary == null)
                {
                    dictionary = new Dictionary<NamedTypeSymbol, NamedTypeSymbol>();
                }
                flag = flag || containingType.DeclaredAccessibility.HasProtected();
                flag2 = flag2 || containingType.Arity > 0;
                dictionary.Add(containingType.OriginalDefinition, containingType);
                containingType = containingType.ContainingType;
            }
            if (!flag || !flag2 || dictionary == null)
            {
                return false;
            }
            while ((object)context != null)
            {
                NamedTypeSymbol namedTypeSymbol = context;
                while ((object)namedTypeSymbol != null)
                {
                    if (dictionary.TryGetValue(namedTypeSymbol.OriginalDefinition, out var value))
                    {
                        return !TypeSymbol.Equals(value, namedTypeSymbol, TypeCompareKind.ConsiderEverything);
                    }
                    namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
                }
                context = context.ContainingType;
            }
            return false;
        }

        private Compliance GetDeclaredOrInheritedCompliance(Symbol symbol)
        {
            if (symbol.Kind == SymbolKind.Namespace)
            {
                return GetDeclaredOrInheritedCompliance(symbol.ContainingAssembly);
            }
            if (symbol.Kind == SymbolKind.Method)
            {
                Symbol associatedSymbol = ((MethodSymbol)symbol).AssociatedSymbol;
                if ((object)associatedSymbol != null)
                {
                    return GetDeclaredOrInheritedCompliance(associatedSymbol);
                }
            }
            if (_declaredOrInheritedCompliance.TryGetValue(symbol, out var value))
            {
                return value;
            }
            bool? declaredCompliance = GetDeclaredCompliance(symbol, out Location attributeLocation);
            value = (declaredCompliance.HasValue ? ((!declaredCompliance.GetValueOrDefault()) ? Compliance.DeclaredFalse : Compliance.DeclaredTrue) : ((symbol.Kind != SymbolKind.Assembly) ? (IsTrue(GetInheritedCompliance(symbol)) ? Compliance.InheritedTrue : Compliance.InheritedFalse) : Compliance.ImpliedFalse));
            if (symbol.Kind != SymbolKind.Assembly && symbol.Kind != SymbolKind.NamedType)
            {
                return value;
            }
            return _declaredOrInheritedCompliance.GetOrAdd(symbol, value);
        }

        private Compliance GetInheritedCompliance(Symbol symbol)
        {
            Symbol symbol2 = (Symbol)(symbol.ContainingType ?? ((object)symbol.ContainingAssembly));
            return GetDeclaredOrInheritedCompliance(symbol2);
        }

        private bool? GetDeclaredCompliance(Symbol symbol, out Location attributeLocation)
        {
            attributeLocation = null;
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = symbol.GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                if (!current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute))
                {
                    continue;
                }
                NamedTypeSymbol attributeClass = current.AttributeClass;
                if (((object)attributeClass == null || !_diagnostics.ReportUseSite(attributeClass, symbol.Locations.IsEmpty ? NoLocation.Singleton : symbol.Locations[0])) && !current.HasErrors)
                {
                    if (!TryGetAttributeWarningLocation(current, out attributeLocation))
                    {
                        attributeLocation = null;
                    }
                    return (bool)current.CommonConstructorArguments[0].ValueInternal;
                }
            }
            return null;
        }

        private static bool IsAccessibleOutsideAssembly(Symbol symbol)
        {
            while ((object)symbol != null && !IsImplicitClass(symbol))
            {
                if (!IsAccessibleIfContainerIsAccessible(symbol))
                {
                    return false;
                }
                symbol = symbol.ContainingType;
            }
            return true;
        }

        private static bool IsAccessibleIfContainerIsAccessible(Symbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Protected:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Public:
                    return true;
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                case Accessibility.Internal:
                    return false;
                case Accessibility.NotApplicable:
                    return false;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
            }
        }

        private void AddDiagnostic(ErrorCode code, Location location)
        {
            CSDiagnostic diag = new CSDiagnostic(new CSDiagnosticInfo(code), location);
            _diagnostics.Add(diag);
        }

        private void AddDiagnostic(ErrorCode code, Location location, params object[] args)
        {
            CSDiagnostic diag = new CSDiagnostic(new CSDiagnosticInfo(code, args), location);
            _diagnostics.Add(diag);
        }

        private static bool IsImplicitClass(Symbol symbol)
        {
            if (symbol.Kind == SymbolKind.NamedType)
            {
                return ((NamedTypeSymbol)symbol).IsImplicitClass;
            }
            return false;
        }

        private static bool IsTrue(Compliance compliance)
        {
            switch (compliance)
            {
                case Compliance.DeclaredTrue:
                case Compliance.InheritedTrue:
                    return true;
                case Compliance.DeclaredFalse:
                case Compliance.InheritedFalse:
                case Compliance.ImpliedFalse:
                    return false;
                default:
                    throw ExceptionUtilities.UnexpectedValue(compliance);
            }
        }

        private static bool IsDeclared(Compliance compliance)
        {
            switch (compliance)
            {
                case Compliance.DeclaredTrue:
                case Compliance.DeclaredFalse:
                    return true;
                case Compliance.InheritedTrue:
                case Compliance.InheritedFalse:
                case Compliance.ImpliedFalse:
                    return false;
                default:
                    throw ExceptionUtilities.UnexpectedValue(compliance);
            }
        }

        private static bool TryGetCollisionErrorCode(Symbol x, Symbol y, out ErrorCode code)
        {
            code = ErrorCode.Void;
            ImmutableArray<TypeWithAnnotations> parameterTypesWithAnnotations;
            ImmutableArray<RefKind> parameterRefKinds;
            ImmutableArray<TypeWithAnnotations> parameterTypesWithAnnotations2;
            ImmutableArray<RefKind> parameterRefKinds2;
            switch (x.Kind)
            {
                case SymbolKind.Method:
                    {
                        MethodSymbol obj3 = (MethodSymbol)x;
                        parameterTypesWithAnnotations = obj3.ParameterTypesWithAnnotations;
                        parameterRefKinds = obj3.ParameterRefKinds;
                        MethodSymbol obj4 = (MethodSymbol)y;
                        parameterTypesWithAnnotations2 = obj4.ParameterTypesWithAnnotations;
                        parameterRefKinds2 = obj4.ParameterRefKinds;
                        break;
                    }
                case SymbolKind.Property:
                    {
                        PropertySymbol obj = (PropertySymbol)x;
                        parameterTypesWithAnnotations = obj.ParameterTypesWithAnnotations;
                        parameterRefKinds = obj.ParameterRefKinds;
                        PropertySymbol obj2 = (PropertySymbol)y;
                        parameterTypesWithAnnotations2 = obj2.ParameterTypesWithAnnotations;
                        parameterRefKinds2 = obj2.ParameterRefKinds;
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(x.Kind);
            }
            int length = parameterTypesWithAnnotations.Length;
            if (parameterTypesWithAnnotations2.Length != length)
            {
                return false;
            }
            bool flag = parameterRefKinds.IsDefault != parameterRefKinds2.IsDefault;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < length; i++)
            {
                TypeSymbol type = parameterTypesWithAnnotations[i].Type;
                TypeSymbol type2 = parameterTypesWithAnnotations2[i].Type;
                TypeKind typeKind = type.TypeKind;
                if (type2.TypeKind != typeKind)
                {
                    return false;
                }
                if (typeKind == TypeKind.Array)
                {
                    ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
                    ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)type2;
                    flag2 = flag2 || arrayTypeSymbol.Rank != arrayTypeSymbol2.Rank;
                    bool flag4 = !TypeSymbol.Equals(arrayTypeSymbol.ElementType, arrayTypeSymbol2.ElementType, TypeCompareKind.ConsiderEverything);
                    if (IsArrayOfArrays(arrayTypeSymbol) || IsArrayOfArrays(arrayTypeSymbol2))
                    {
                        flag3 = flag3 || flag4;
                    }
                    else if (flag4)
                    {
                        return false;
                    }
                }
                else if (!TypeSymbol.Equals(type, type2, TypeCompareKind.ConsiderEverything))
                {
                    return false;
                }
                if (!parameterRefKinds.IsDefault)
                {
                    flag = flag || parameterRefKinds[i] != parameterRefKinds2[i];
                }
            }
            code = (flag3 ? ErrorCode.WRN_CLS_OverloadUnnamed : (flag2 ? ErrorCode.WRN_CLS_OverloadRefOut : (flag ? ErrorCode.WRN_CLS_OverloadRefOut : ErrorCode.Void)));
            return code != ErrorCode.Void;
        }

        private static bool IsArrayOfArrays(ArrayTypeSymbol arrayType)
        {
            return arrayType.ElementType.Kind == SymbolKind.ArrayType;
        }
    }
}
