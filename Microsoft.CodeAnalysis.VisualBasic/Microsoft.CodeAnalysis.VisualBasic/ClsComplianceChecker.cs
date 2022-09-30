using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ClsComplianceChecker : VisualBasicSymbolVisitor
	{
		private enum Compliance
		{
			DeclaredTrue,
			DeclaredFalse,
			InheritedTrue,
			InheritedFalse,
			ImpliedFalse
		}

		private readonly VisualBasicCompilation _compilation;

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

		private ClsComplianceChecker(VisualBasicCompilation compilation, SyntaxTree filterTree, TextSpan? filterSpanWithinTree, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
		{
			_compilation = compilation;
			_filterTree = filterTree;
			_filterSpanWithinTree = filterSpanWithinTree;
			_diagnostics = diagnostics;
			_cancellationToken = cancellationToken;
			_declaredOrInheritedCompliance = new ConcurrentDictionary<Symbol, Compliance>();
			if (ConcurrentAnalysis)
			{
				_compilerTasks = new ConcurrentStack<Task>();
			}
		}

		public static void CheckCompliance(VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken, SyntaxTree filterTree = null, TextSpan? filterSpanWithinTree = null)
		{
			BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(diagnostics.DiagnosticBag, new ConcurrentSet<AssemblySymbol>());
			ClsComplianceChecker clsComplianceChecker = new ClsComplianceChecker(compilation, filterTree, filterSpanWithinTree, bindingDiagnosticBag, cancellationToken);
			clsComplianceChecker.Visit(compilation.Assembly);
			clsComplianceChecker.WaitForWorkers();
			diagnostics.AddDependencies(bindingDiagnosticBag.DependenciesBag);
		}

		private void WaitForWorkers()
		{
			ConcurrentStack<Task> compilerTasks = _compilerTasks;
			if (compilerTasks != null)
			{
				Task result = null;
				while (compilerTasks.TryPop(out result))
				{
					result.GetAwaiter().GetResult();
				}
			}
		}

		public override void VisitAssembly(AssemblySymbol symbol)
		{
			CancellationToken cancellationToken = _cancellationToken;
			cancellationToken.ThrowIfCancellationRequested();
			if (symbol.Modules.Length > 1 && ConcurrentAnalysis)
			{
				VisitAssemblyMembersAsTasks(symbol);
			}
			else
			{
				VisitAssemblyMembers(symbol);
			}
		}

		private void VisitAssemblyMembersAsTasks(AssemblySymbol symbol)
		{
			ImmutableArray<ModuleSymbol>.Enumerator enumerator = symbol.Modules.GetEnumerator();
			_Closure_0024__13_002D0 closure_0024__13_002D = default(_Closure_0024__13_002D0);
			while (enumerator.MoveNext())
			{
				closure_0024__13_002D = new _Closure_0024__13_002D0(closure_0024__13_002D);
				closure_0024__13_002D._0024VB_0024Me = this;
				closure_0024__13_002D._0024VB_0024Local_m = enumerator.Current;
				_compilerTasks.Push(Task.Run(UICultureUtilities.WithCurrentUICulture(closure_0024__13_002D._Lambda_0024__0), _cancellationToken));
			}
		}

		private void VisitAssemblyMembers(AssemblySymbol symbol)
		{
			ImmutableArray<ModuleSymbol>.Enumerator enumerator = symbol.Modules.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ModuleSymbol current = enumerator.Current;
				VisitModule(current);
			}
		}

		public override void VisitModule(ModuleSymbol symbol)
		{
			Visit(symbol.GlobalNamespace);
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
			_Closure_0024__17_002D0 closure_0024__17_002D = default(_Closure_0024__17_002D0);
			while (enumerator.MoveNext())
			{
				closure_0024__17_002D = new _Closure_0024__17_002D0(closure_0024__17_002D);
				closure_0024__17_002D._0024VB_0024Me = this;
				closure_0024__17_002D._0024VB_0024Local_m = enumerator.Current;
				_compilerTasks.Push(Task.Run(UICultureUtilities.WithCurrentUICulture(closure_0024__17_002D._Lambda_0024__0), _cancellationToken));
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
				CheckMemberDistinctness(symbol);
				if (symbol.TypeKind == TypeKind.Delegate)
				{
					CheckParameterCompliance(symbol.DelegateInvokeMethod.Parameters, symbol);
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
				ImmutableArray<TypeSymbol>.Enumerator enumerator2 = GetParameterTypes(current).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeSymbol current2 = enumerator2.Current;
					if (current2.TypeKind == TypeKind.Array || TypedConstant.GetTypedConstantKind(current2, _compilation) == TypedConstantKind.Error)
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
			bool flag = VisitTypeOrMember(symbol, declaredOrInheritedCompliance);
			bool flag2 = SymbolExtensions.IsAccessor(symbol);
			if (!flag && !flag2)
			{
				return;
			}
			if (!flag2)
			{
				if (IsTrue(declaredOrInheritedCompliance))
				{
					CheckParameterCompliance(symbol.Parameters, symbol.ContainingType);
					CheckTypeParameterCompliance(symbol.TypeParameters, symbol.ContainingType);
				}
				return;
			}
			MethodKind methodKind = symbol.MethodKind;
			switch (methodKind)
			{
			case MethodKind.PropertyGet:
			case MethodKind.PropertySet:
			{
				ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = symbol.GetAttributes().GetEnumerator();
				while (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute))
					{
						Location location = null;
						if (TryGetAttributeWarningLocation(current, ref location))
						{
							AttributeUsageInfo attributeUsageInfo = current.AttributeClass.GetAttributeUsageInfo();
							AddDiagnostic(symbol, ERRID.WRN_CLSAttrInvalidOnGetSet, location, current.AttributeClass.Name, attributeUsageInfo.GetValidTargetsErrorArgument());
							break;
						}
					}
				}
				break;
			}
			case MethodKind.EventAdd:
			case MethodKind.EventRemove:
			{
				if (!flag)
				{
					break;
				}
				NamedTypeSymbol containingType = symbol.ContainingType;
				if (!IsTrue(GetDeclaredOrInheritedCompliance(containingType)))
				{
					Location attributeLocation = null;
					bool? declaredCompliance = GetDeclaredCompliance(symbol, out attributeLocation);
					declaredCompliance = declaredCompliance;
					if (declaredCompliance.GetValueOrDefault())
					{
						AddDiagnostic(symbol, ERRID.WRN_CLSEventMethodInNonCLSType3, attributeLocation, MethodKindExtensions.TryGetAccessorDisplayName(methodKind), symbol.AssociatedSymbol.Name, containingType);
					}
				}
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
				VisitTypeOrMember(symbol, declaredOrInheritedCompliance);
			}
		}

		private bool VisitTypeOrMember(Symbol symbol, Compliance compliance)
		{
			if (!IsAccessibleOutsideAssembly(symbol))
			{
				return false;
			}
			bool flag = SymbolExtensions.IsAccessor(symbol);
			if (IsTrue(compliance))
			{
				CheckName(symbol);
				if (!flag)
				{
					CheckForCompliantWithinNonCompliant(symbol);
				}
				if (symbol.Kind == SymbolKind.NamedType)
				{
					MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)symbol).DelegateInvokeMethod;
					if ((object)delegateInvokeMethod != null)
					{
						CheckReturnTypeCompliance(delegateInvokeMethod);
					}
				}
				else if (symbol.Kind == SymbolKind.Event)
				{
					CheckEventTypeCompliance((EventSymbol)symbol);
				}
				else if (!flag)
				{
					CheckReturnTypeCompliance(symbol);
				}
			}
			else if (!flag && IsTrue(GetInheritedCompliance(symbol)))
			{
				CheckForNonCompliantAbstractMember(symbol);
			}
			return true;
		}

		private void CheckForNonCompliantAbstractMember(Symbol symbol)
		{
			NamedTypeSymbol containingType = symbol.ContainingType;
			if ((object)containingType != null && containingType.IsInterface)
			{
				AddDiagnostic(symbol, ERRID.WRN_NonCLSMemberInCLSInterface1, symbol);
			}
			else if (symbol.IsMustOverride && symbol.Kind != SymbolKind.NamedType)
			{
				AddDiagnostic(symbol, ERRID.WRN_NonCLSMustOverrideInCLSType1, containingType);
			}
		}

		private void CheckBaseTypeCompliance(NamedTypeSymbol symbol)
		{
			if (symbol.IsInterface)
			{
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = symbol.InterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					if (ShouldReportNonCompliantType(current, symbol))
					{
						AddDiagnostic(symbol, ERRID.WRN_InheritedInterfaceNotCLSCompliant2, symbol, current);
					}
				}
			}
			else if (symbol.TypeKind == TypeKind.Enum)
			{
				NamedTypeSymbol enumUnderlyingType = symbol.EnumUnderlyingType;
				if (ShouldReportNonCompliantType(enumUnderlyingType, symbol))
				{
					AddDiagnostic(symbol, ERRID.WRN_EnumUnderlyingTypeNotCLS1, enumUnderlyingType);
				}
			}
			else
			{
				NamedTypeSymbol baseTypeNoUseSiteDiagnostics = symbol.BaseTypeNoUseSiteDiagnostics;
				if ((object)baseTypeNoUseSiteDiagnostics != null && ShouldReportNonCompliantType(baseTypeNoUseSiteDiagnostics, symbol))
				{
					AddDiagnostic(symbol, ERRID.WRN_BaseClassNotCLSCompliant2, symbol, baseTypeNoUseSiteDiagnostics);
				}
			}
		}

		private void CheckForCompliantWithinNonCompliant(Symbol symbol)
		{
			NamedTypeSymbol containingType = symbol.ContainingType;
			if ((object)containingType != null && !IsTrue(GetDeclaredOrInheritedCompliance(containingType)))
			{
				AddDiagnostic(symbol, ERRID.WRN_CLSMemberInNonCLSType3, SymbolExtensions.GetKindText(symbol), symbol, containingType);
			}
		}

		private void CheckTypeParameterCompliance(ImmutableArray<TypeParameterSymbol> typeParameters, NamedTypeSymbol context)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				ImmutableArray<TypeSymbol>.Enumerator enumerator2 = current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeSymbol current2 = enumerator2.Current;
					if (ShouldReportNonCompliantType(current2, context, current))
					{
						AddDiagnostic(current, ERRID.WRN_GenericConstraintNotCLSCompliant1, current2);
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
				if (ShouldReportNonCompliantType(current.Type, context, current))
				{
					AddDiagnostic(current, ERRID.WRN_ParamNotCLSCompliant1, current.Name);
				}
				else if (current.HasExplicitDefaultValue)
				{
					switch (current.ExplicitDefaultConstantValue.Discriminator)
					{
					case ConstantValueTypeDiscriminator.SByte:
					case ConstantValueTypeDiscriminator.UInt16:
					case ConstantValueTypeDiscriminator.UInt32:
					case ConstantValueTypeDiscriminator.UInt64:
						AddDiagnostic(current, ERRID.WRN_OptionalValueNotCLSCompliant1, current.Name);
						break;
					}
				}
			}
		}

		private bool TryGetAttributeWarningLocation(VisualBasicAttributeData attribute, ref Location location)
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

		private void CheckReturnTypeCompliance(Symbol symbol)
		{
			ERRID code;
			TypeSymbol type;
			switch (symbol.Kind)
			{
			case SymbolKind.Field:
				code = ERRID.WRN_FieldNotCLSCompliant1;
				type = ((FieldSymbol)symbol).Type;
				break;
			case SymbolKind.Property:
				code = ERRID.WRN_ProcTypeNotCLSCompliant1;
				type = ((PropertySymbol)symbol).Type;
				break;
			case SymbolKind.Method:
				code = ERRID.WRN_ProcTypeNotCLSCompliant1;
				type = ((MethodSymbol)symbol).ReturnType;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
			}
			if (ShouldReportNonCompliantType(type, symbol.ContainingType, symbol))
			{
				AddDiagnostic(symbol, code, symbol.Name);
			}
		}

		private void CheckEventTypeCompliance(EventSymbol symbol)
		{
			TypeSymbol type = symbol.Type;
			if (type.TypeKind == TypeKind.Delegate && type.IsImplicitlyDeclared && (object)(type as NamedTypeSymbol)?.AssociatedSymbol == symbol)
			{
				CheckParameterCompliance(symbol.DelegateParameters, symbol.ContainingType);
			}
			else if (ShouldReportNonCompliantType(type, symbol.ContainingType, symbol))
			{
				AddDiagnostic(symbol, ERRID.WRN_EventDelegateTypeNotCLSCompliant2, type, symbol.Name);
			}
		}

		private void CheckMemberDistinctness(NamespaceOrTypeSymbol symbol)
		{
			MultiDictionary<string, Symbol> multiDictionary = new MultiDictionary<string, Symbol>(CaseInsensitiveComparison.Comparer);
			if (symbol.Kind != SymbolKind.Namespace)
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
				foreach (NamedTypeSymbol key in namedTypeSymbol.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys)
				{
					if (!IsAccessibleOutsideAssembly(key))
					{
						continue;
					}
					ImmutableArray<Symbol>.Enumerator enumerator2 = key.GetMembersUnordered().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current2 = enumerator2.Current;
						if (IsAccessibleIfContainerIsAccessible(current2) && (!current2.IsOverrides || (current2.Kind != SymbolKind.Method && current2.Kind != SymbolKind.Property)))
						{
							multiDictionary.Add(current2.Name, current2);
						}
					}
				}
				NamedTypeSymbol baseTypeNoUseSiteDiagnostics = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
				while ((object)baseTypeNoUseSiteDiagnostics != null)
				{
					ImmutableArray<Symbol>.Enumerator enumerator3 = baseTypeNoUseSiteDiagnostics.GetMembersUnordered().GetEnumerator();
					while (enumerator3.MoveNext())
					{
						Symbol current3 = enumerator3.Current;
						if (IsAccessibleOutsideAssembly(current3) && IsTrue(GetDeclaredOrInheritedCompliance(current3)) && (!current3.IsOverrides || (current3.Kind != SymbolKind.Method && current3.Kind != SymbolKind.Property)))
						{
							multiDictionary.Add(current3.Name, current3);
						}
					}
					baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
				}
			}
			ImmutableArray<Symbol>.Enumerator enumerator4 = symbol.GetMembers().GetEnumerator();
			while (enumerator4.MoveNext())
			{
				Symbol current4 = enumerator4.Current;
				if (!DoNotVisit(current4) && IsAccessibleIfContainerIsAccessible(current4) && IsTrue(GetDeclaredOrInheritedCompliance(current4)) && !current4.IsOverrides)
				{
					string name = current4.Name;
					MultiDictionary<string, Symbol>.ValueSet sameNameSymbols = multiDictionary[name];
					if (sameNameSymbols.Count > 0)
					{
						CheckSymbolDistinctness(current4, sameNameSymbols);
					}
					multiDictionary.Add(name, current4);
				}
			}
		}

		private void CheckSymbolDistinctness(Symbol symbol, MultiDictionary<string, Symbol>.ValueSet sameNameSymbols)
		{
			if (symbol.Kind != SymbolKind.Method && symbol.Kind != SymbolKind.Property)
			{
				return;
			}
			foreach (Symbol item in sameNameSymbols)
			{
				if (symbol.Kind == item.Kind && !SymbolExtensions.IsAccessor(symbol) && !SymbolExtensions.IsAccessor(item) && SignaturesCollide(symbol, item))
				{
					AddDiagnostic(symbol, ERRID.WRN_ArrayOverloadsNonCLS2, symbol, item);
				}
			}
		}

		private void CheckName(Symbol symbol)
		{
			if (!symbol.CanBeReferencedByName)
			{
				return;
			}
			string name = symbol.Name;
			if (name.Length <= 0 || name[0] != '_')
			{
				return;
			}
			if (symbol.Kind == SymbolKind.Namespace)
			{
				NamespaceSymbol rootNamespace = _compilation.RootNamespace;
				if (symbol == rootNamespace && symbol.ContainingNamespace.IsGlobalNamespace)
				{
					AddDiagnostic(symbol, ERRID.WRN_RootNamespaceNotCLSCompliant1, rootNamespace);
					return;
				}
				NamespaceSymbol namespaceSymbol = rootNamespace;
				while ((object)namespaceSymbol != null)
				{
					if (symbol == namespaceSymbol)
					{
						AddDiagnostic(symbol, ERRID.WRN_RootNamespaceNotCLSCompliant2, symbol.Name, rootNamespace);
						return;
					}
					namespaceSymbol = namespaceSymbol.ContainingNamespace;
				}
			}
			AddDiagnostic(symbol, ERRID.WRN_NameNotCLSCompliant1, name);
		}

		private bool DoNotVisit(Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Namespace)
			{
				return false;
			}
			return symbol.DeclaringCompilation != _compilation || symbol.IsImplicitlyDeclared || IsSyntacticallyFilteredOut(symbol);
		}

		private bool IsSyntacticallyFilteredOut(Symbol symbol)
		{
			if (_filterTree != null)
			{
				return !symbol.IsDefinedInSourceTree(_filterTree, _filterSpanWithinTree, _cancellationToken);
			}
			return false;
		}

		private bool ShouldReportNonCompliantType(TypeSymbol type, NamedTypeSymbol context, Symbol diagnosticSymbol = null)
		{
			ReportNonCompliantTypeArguments(type, context, diagnosticSymbol ?? context);
			return !IsCompliantType(type, context);
		}

		private void ReportNonCompliantTypeArguments(TypeSymbol type, NamedTypeSymbol context, Symbol diagnosticSymbol)
		{
			switch (type.TypeKind)
			{
			case TypeKind.Array:
				ReportNonCompliantTypeArguments(((ArrayTypeSymbol)type).ElementType, context, diagnosticSymbol);
				break;
			case TypeKind.Class:
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Interface:
			case TypeKind.Module:
			case TypeKind.Struct:
			case TypeKind.Submission:
				ReportNonCompliantTypeArguments((NamedTypeSymbol)type, context, diagnosticSymbol);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
			case TypeKind.Error:
			case TypeKind.TypeParameter:
				break;
			}
		}

		private void ReportNonCompliantTypeArguments(NamedTypeSymbol type, NamedTypeSymbol context, Symbol diagnosticSymbol)
		{
			if (type.IsTupleType)
			{
				type = type.TupleUnderlyingType;
			}
			ImmutableArray<TypeSymbol>.Enumerator enumerator = type.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				if (!IsCompliantType(current, context))
				{
					AddDiagnostic(diagnosticSymbol, ERRID.WRN_TypeNotCLSCompliant1, current);
				}
				ReportNonCompliantTypeArguments(current, context, diagnosticSymbol);
			}
		}

		private bool IsCompliantType(TypeSymbol type, NamedTypeSymbol context)
		{
			switch (type.TypeKind)
			{
			case TypeKind.Array:
				return IsCompliantType(((ArrayTypeSymbol)type).ElementType, context);
			case TypeKind.Error:
			case TypeKind.TypeParameter:
				return true;
			case TypeKind.Class:
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Interface:
			case TypeKind.Module:
			case TypeKind.Struct:
			case TypeKind.Submission:
				return IsCompliantType((NamedTypeSymbol)type);
			default:
				throw ExceptionUtilities.UnexpectedValue(type.TypeKind);
			}
		}

		private bool IsCompliantType(NamedTypeSymbol type)
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
				if (type.TypeKind == TypeKind.Error)
				{
					return true;
				}
				if (!IsTrue(GetDeclaredOrInheritedCompliance(type.OriginalDefinition)))
				{
					return false;
				}
				if (type.IsTupleType)
				{
					return IsCompliantType(type.TupleUnderlyingType);
				}
				return true;
			}
		}

		private Compliance GetDeclaredOrInheritedCompliance(Symbol symbol)
		{
			if (symbol.Kind == SymbolKind.Namespace)
			{
				return GetDeclaredOrInheritedCompliance(GetContainingModuleOrAssembly(symbol));
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
			Location attributeLocation = null;
			bool? declaredCompliance = GetDeclaredCompliance(symbol, out attributeLocation);
			value = (declaredCompliance.HasValue ? ((!declaredCompliance.GetValueOrDefault()) ? Compliance.DeclaredFalse : Compliance.DeclaredTrue) : ((symbol.Kind != SymbolKind.Assembly && symbol.Kind != SymbolKind.NetModule) ? (IsTrue(GetInheritedCompliance(symbol)) ? Compliance.InheritedTrue : Compliance.InheritedFalse) : Compliance.ImpliedFalse));
			SymbolKind kind = symbol.Kind;
			if (kind == SymbolKind.Assembly || (uint)(kind - 10) <= 1u)
			{
				return _declaredOrInheritedCompliance.GetOrAdd(symbol, value);
			}
			return value;
		}

		private Compliance GetInheritedCompliance(Symbol symbol)
		{
			Symbol symbol2 = symbol.ContainingType ?? GetContainingModuleOrAssembly(symbol);
			return GetDeclaredOrInheritedCompliance(symbol2);
		}

		private bool? GetDeclaredCompliance(Symbol symbol, out Location attributeLocation)
		{
			if (symbol.IsFromCompilation(_compilation) || symbol.Kind != SymbolKind.NamedType)
			{
				bool isAttributeInherited = false;
				return GetDeclaredComplianceHelper(symbol, out attributeLocation, out isAttributeInherited);
			}
			NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
			while ((object)namedTypeSymbol != null)
			{
				bool isAttributeInherited2 = false;
				bool? declaredComplianceHelper = GetDeclaredComplianceHelper(namedTypeSymbol, out attributeLocation, out isAttributeInherited2);
				if (declaredComplianceHelper.HasValue)
				{
					return ((object)namedTypeSymbol == symbol || (isAttributeInherited2 && ((!declaredComplianceHelper) ?? declaredComplianceHelper).GetValueOrDefault())) ? declaredComplianceHelper : null;
				}
				namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			return null;
		}

		private bool? GetDeclaredComplianceHelper(Symbol symbol, out Location attributeLocation, out bool isAttributeInherited)
		{
			attributeLocation = null;
			isAttributeInherited = false;
			ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = symbol.GetAttributes().GetEnumerator();
			bool? result;
			while (true)
			{
				if (enumerator.MoveNext())
				{
					VisualBasicAttributeData current = enumerator.Current;
					if (!current.IsTargetAttribute(symbol, AttributeDescription.CLSCompliantAttribute))
					{
						continue;
					}
					NamedTypeSymbol attributeClass = current.AttributeClass;
					if ((object)attributeClass != null)
					{
						_diagnostics.ReportUseSite(attributeClass, symbol.Locations.IsEmpty ? NoLocation.Singleton : symbol.Locations[0]);
					}
					if (!current.HasErrors)
					{
						if (!TryGetAttributeWarningLocation(current, ref attributeLocation))
						{
							attributeLocation = null;
						}
						isAttributeInherited = current.AttributeClass.GetAttributeUsageInfo().Inherited;
						result = (bool)current.CommonConstructorArguments[0].ValueInternal;
						break;
					}
					continue;
				}
				result = null;
				break;
			}
			return result;
		}

		private Symbol GetContainingModuleOrAssembly(Symbol symbol)
		{
			AssemblySymbol containingAssembly = symbol.ContainingAssembly;
			if ((object)containingAssembly != _compilation.Assembly)
			{
				return containingAssembly;
			}
			return (_compilation.Options.OutputKind == OutputKind.NetModule) ? ((Symbol)symbol.ContainingModule) : ((Symbol)containingAssembly);
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
			case Accessibility.Public:
				return true;
			case Accessibility.Protected:
			case Accessibility.ProtectedOrInternal:
			{
				NamedTypeSymbol containingType = symbol.ContainingType;
				return (object)containingType == null || !containingType.IsNotInheritable;
			}
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

		private void AddDiagnostic(Symbol symbol, ERRID code, params object[] args)
		{
			Location location = (symbol.Locations.IsEmpty ? NoLocation.Singleton : symbol.Locations[0]);
			AddDiagnostic(symbol, code, location, args);
		}

		private void AddDiagnostic(Symbol symbol, ERRID code, Location location, params object[] args)
		{
			BadSymbolDiagnostic info = new BadSymbolDiagnostic(symbol, code, args);
			VBDiagnostic diag = new VBDiagnostic(info, location);
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

		private static bool SignaturesCollide(Symbol x, Symbol y)
		{
			ImmutableArray<TypeSymbol> parameterTypes = GetParameterTypes(x);
			ImmutableArray<TypeSymbol> parameterTypes2 = GetParameterTypes(y);
			GetParameterRefKinds(x);
			GetParameterRefKinds(y);
			int length = parameterTypes.Length;
			if (parameterTypes2.Length != length)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeSymbol = parameterTypes[i];
				TypeSymbol typeSymbol2 = parameterTypes2[i];
				TypeKind typeKind = typeSymbol.TypeKind;
				if (typeSymbol2.TypeKind != typeKind)
				{
					return false;
				}
				if (typeKind == TypeKind.Array)
				{
					ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)typeSymbol;
					ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)typeSymbol2;
					flag = flag || arrayTypeSymbol.Rank != arrayTypeSymbol2.Rank;
					bool flag3 = !TypeSymbol.Equals(arrayTypeSymbol.ElementType, arrayTypeSymbol2.ElementType, TypeCompareKind.ConsiderEverything);
					if (IsArrayOfArrays(arrayTypeSymbol) && IsArrayOfArrays(arrayTypeSymbol2))
					{
						flag2 = flag2 || flag3;
					}
					else if (flag3)
					{
						return false;
					}
				}
				else if (!TypeSymbol.Equals(typeSymbol, typeSymbol2, TypeCompareKind.ConsiderEverything))
				{
					return false;
				}
			}
			return flag2 || flag;
		}

		private static bool IsArrayOfArrays(ArrayTypeSymbol arrayType)
		{
			return arrayType.ElementType.Kind == SymbolKind.ArrayType;
		}

		private static ImmutableArray<TypeSymbol> GetParameterTypes(Symbol symbol)
		{
			ImmutableArray<ParameterSymbol> immutableArray = symbol.Kind switch
			{
				SymbolKind.Method => ((MethodSymbol)symbol).Parameters, 
				SymbolKind.Property => ((PropertySymbol)symbol).Parameters, 
				_ => throw ExceptionUtilities.UnexpectedValue(symbol.Kind), 
			};
			if (immutableArray.IsEmpty)
			{
				return ImmutableArray<TypeSymbol>.Empty;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(immutableArray.Length);
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				instance.Add(current.Type);
			}
			return instance.ToImmutableAndFree();
		}

		private static ImmutableArray<RefKind> GetParameterRefKinds(Symbol symbol)
		{
			ImmutableArray<ParameterSymbol> immutableArray = symbol.Kind switch
			{
				SymbolKind.Method => ((MethodSymbol)symbol).Parameters, 
				SymbolKind.Property => ((PropertySymbol)symbol).Parameters, 
				_ => throw ExceptionUtilities.UnexpectedValue(symbol.Kind), 
			};
			if (immutableArray.IsEmpty)
			{
				return ImmutableArray<RefKind>.Empty;
			}
			ArrayBuilder<RefKind> instance = ArrayBuilder<RefKind>.GetInstance(immutableArray.Length);
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				instance.Add(current.IsByRef ? RefKind.Ref : RefKind.None);
			}
			return instance.ToImmutableAndFree();
		}
	}
}
