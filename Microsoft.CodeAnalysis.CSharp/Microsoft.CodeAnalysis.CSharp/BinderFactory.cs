using System;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class BinderFactory
    {
        private sealed class BinderFactoryVisitor : CSharpSyntaxVisitor<Binder>
        {
            private int _position;

            private CSharpSyntaxNode _memberDeclarationOpt;

            private Symbol _memberOpt;

            private readonly BinderFactory _factory;

            private CSharpCompilation compilation => _factory._compilation;

            private SyntaxTree _syntaxTree => _factory._syntaxTree;

            private BuckStopsHereBinder buckStopsHereBinder => _factory._buckStopsHereBinder;

            private ConcurrentCache<BinderCacheKey, Binder> binderCache => _factory._binderCache;

            private bool InScript => _factory.InScript;

            internal BinderFactoryVisitor(BinderFactory factory)
            {
                _factory = factory;
            }

            internal void Initialize(int position, CSharpSyntaxNode memberDeclarationOpt, Symbol memberOpt)
            {
                _position = position;
                _memberDeclarationOpt = memberDeclarationOpt;
                _memberOpt = memberOpt;
            }

            public override Binder DefaultVisit(SyntaxNode parent)
            {
                return VisitCore(parent.Parent);
            }

            public override Binder Visit(SyntaxNode node)
            {
                return VisitCore(node);
            }

            private Binder VisitCore(SyntaxNode node)
            {
                return ((CSharpSyntaxNode)node).Accept(this);
            }

            public override Binder VisitGlobalStatement(GlobalStatementSyntax node)
            {
                if (SyntaxFacts.IsSimpleProgramTopLevelStatement(node))
                {
                    CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)node.Parent;
                    if (compilationUnitSyntax != _syntaxTree.GetRoot())
                    {
                        throw new ArgumentOutOfRangeException("node", "node not part of tree");
                    }
                    BinderCacheKey key = CreateBinderCacheKey(compilationUnitSyntax, NodeUsage.MethodBody);
                    if (!binderCache.TryGetValue(key, out var value))
                    {
                        value = SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(compilation, (CompilationUnitSyntax)node.Parent, fallbackToMainEntryPoint: false)!.GetBodyBinder(_factory._ignoreAccessibility).GetBinder(compilationUnitSyntax);
                        binderCache.TryAdd(key, value);
                    }
                    return value;
                }
                return base.VisitGlobalStatement(node);
            }

            public override Binder VisitMethodDeclaration(MethodDeclarationSyntax methodDecl)
            {
                if (!LookupPosition.IsInMethodDeclaration(_position, methodDecl))
                {
                    return VisitCore(methodDecl.Parent);
                }
                NodeUsage nodeUsage = (LookupPosition.IsInBody(_position, methodDecl) ? NodeUsage.MethodBody : (LookupPosition.IsInMethodTypeParameterScope(_position, methodDecl) ? NodeUsage.MethodTypeParameters : NodeUsage.Normal));
                BinderCacheKey key = CreateBinderCacheKey(methodDecl, nodeUsage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = ((!(methodDecl.Parent is TypeDeclarationSyntax parent)) ? VisitCore(methodDecl.Parent) : VisitTypeDeclarationCore(parent, NodeUsage.MethodBody));
                    SourceMemberMethodSymbol sourceMemberMethodSymbol = null;
                    if (nodeUsage != 0 && methodDecl.TypeParameterList != null)
                    {
                        sourceMemberMethodSymbol = GetMethodSymbol(methodDecl, value);
                        value = new WithMethodTypeParametersBinder(sourceMemberMethodSymbol, value);
                    }
                    if (nodeUsage == NodeUsage.MethodBody)
                    {
                        sourceMemberMethodSymbol = sourceMemberMethodSymbol ?? GetMethodSymbol(methodDecl, value);
                        value = new InMethodBinder(sourceMemberMethodSymbol, value);
                    }
                    value = value.WithUnsafeRegionIfNecessary(methodDecl.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitConstructorDeclaration(ConstructorDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInMethodDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                bool flag = LookupPosition.IsInConstructorParameterScope(_position, parent);
                NodeUsage usage = (flag ? NodeUsage.MethodTypeParameters : NodeUsage.Normal);
                BinderCacheKey key = CreateBinderCacheKey(parent, usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent);
                    if (flag)
                    {
                        SourceMemberMethodSymbol methodSymbol = GetMethodSymbol(parent, value);
                        if ((object)methodSymbol != null)
                        {
                            value = new InMethodBinder(methodSymbol, value);
                        }
                    }
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitDestructorDeclaration(DestructorDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInMethodDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                BinderCacheKey key = CreateBinderCacheKey(parent, NodeUsage.Normal);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent);
                    value = new InMethodBinder(GetMethodSymbol(parent, value), value);
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitAccessorDeclaration(AccessorDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInMethodDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                bool flag = LookupPosition.IsInBody(_position, parent);
                NodeUsage usage = (flag ? NodeUsage.MethodTypeParameters : NodeUsage.Normal);
                BinderCacheKey key = CreateBinderCacheKey(parent, usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent);
                    if (flag)
                    {
                        CSharpSyntaxNode parent2 = parent.Parent!.Parent;
                        MethodSymbol methodSymbol = null;
                        switch (parent2.Kind())
                        {
                            case SyntaxKind.PropertyDeclaration:
                            case SyntaxKind.IndexerDeclaration:
                                {
                                    SourcePropertySymbol propertySymbol = GetPropertySymbol((BasePropertyDeclarationSyntax)parent2, value);
                                    if ((object)propertySymbol != null)
                                    {
                                        methodSymbol = ((parent.Kind() == SyntaxKind.GetAccessorDeclaration) ? propertySymbol.GetMethod : propertySymbol.SetMethod);
                                    }
                                    break;
                                }
                            case SyntaxKind.EventFieldDeclaration:
                            case SyntaxKind.EventDeclaration:
                                {
                                    SourceEventSymbol eventSymbol = GetEventSymbol((EventDeclarationSyntax)parent2, value);
                                    if ((object)eventSymbol != null)
                                    {
                                        methodSymbol = ((parent.Kind() == SyntaxKind.AddAccessorDeclaration) ? eventSymbol.AddMethod : eventSymbol.RemoveMethod);
                                    }
                                    break;
                                }
                            default:
                                throw ExceptionUtilities.UnexpectedValue(parent2.Kind());
                        }
                        if ((object)methodSymbol != null)
                        {
                            value = new InMethodBinder(methodSymbol, value);
                        }
                    }
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private Binder VisitOperatorOrConversionDeclaration(BaseMethodDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInMethodDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                bool flag = LookupPosition.IsInBody(_position, parent);
                NodeUsage usage = (flag ? NodeUsage.MethodTypeParameters : NodeUsage.Normal);
                BinderCacheKey key = CreateBinderCacheKey(parent, usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent);
                    MethodSymbol methodSymbol = GetMethodSymbol(parent, value);
                    if ((object)methodSymbol != null && flag)
                    {
                        value = new InMethodBinder(methodSymbol, value);
                    }
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitOperatorDeclaration(OperatorDeclarationSyntax parent)
            {
                return VisitOperatorOrConversionDeclaration(parent);
            }

            public override Binder VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax parent)
            {
                return VisitOperatorOrConversionDeclaration(parent);
            }

            public override Binder VisitFieldDeclaration(FieldDeclarationSyntax parent)
            {
                return VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
            }

            public override Binder VisitEventDeclaration(EventDeclarationSyntax parent)
            {
                return VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
            }

            public override Binder VisitEventFieldDeclaration(EventFieldDeclarationSyntax parent)
            {
                return VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
            }

            public override Binder VisitPropertyDeclaration(PropertyDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInBody(_position, parent))
                {
                    return VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
                }
                return VisitPropertyOrIndexerExpressionBody(parent);
            }

            public override Binder VisitIndexerDeclaration(IndexerDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInBody(_position, parent))
                {
                    return VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
                }
                return VisitPropertyOrIndexerExpressionBody(parent);
            }

            private Binder VisitPropertyOrIndexerExpressionBody(BasePropertyDeclarationSyntax parent)
            {
                BinderCacheKey key = CreateBinderCacheKey(parent, NodeUsage.MethodTypeParameters);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent).WithUnsafeRegionIfNecessary(parent.Modifiers);
                    MethodSymbol getMethod = GetPropertySymbol(parent, value).GetMethod;
                    if ((object)getMethod != null)
                    {
                        value = new InMethodBinder(getMethod, value);
                    }
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private NamedTypeSymbol GetContainerType(Binder binder, CSharpSyntaxNode node)
            {
                Symbol containingMemberOrLambda = binder.ContainingMemberOrLambda;
                NamedTypeSymbol namedTypeSymbol = containingMemberOrLambda as NamedTypeSymbol;
                if ((object)namedTypeSymbol == null)
                {
                    namedTypeSymbol = ((node.Parent!.Kind() != SyntaxKind.CompilationUnit || _syntaxTree.Options.Kind == SourceCodeKind.Regular) ? ((NamespaceSymbol)containingMemberOrLambda).ImplicitType : compilation.ScriptClass);
                }
                return namedTypeSymbol;
            }

            private static string GetMethodName(BaseMethodDeclarationSyntax baseMethodDeclarationSyntax, Binder outerBinder)
            {
                switch (baseMethodDeclarationSyntax.Kind())
                {
                    case SyntaxKind.ConstructorDeclaration:
                        if (!baseMethodDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                        {
                            return ".ctor";
                        }
                        return ".cctor";
                    case SyntaxKind.DestructorDeclaration:
                        return "Finalize";
                    case SyntaxKind.OperatorDeclaration:
                        return OperatorFacts.OperatorNameFromDeclaration((OperatorDeclarationSyntax)baseMethodDeclarationSyntax);
                    case SyntaxKind.ConversionOperatorDeclaration:
                        if (((ConversionOperatorDeclarationSyntax)baseMethodDeclarationSyntax).ImplicitOrExplicitKeyword.Kind() != SyntaxKind.ImplicitKeyword)
                        {
                            return "op_Explicit";
                        }
                        return "op_Implicit";
                    case SyntaxKind.MethodDeclaration:
                        {
                            MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)baseMethodDeclarationSyntax;
                            return ExplicitInterfaceHelpers.GetMemberName(outerBinder, methodDeclarationSyntax.ExplicitInterfaceSpecifier, methodDeclarationSyntax.Identifier.ValueText);
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(baseMethodDeclarationSyntax.Kind());
                }
            }

            private static string GetPropertyOrEventName(BasePropertyDeclarationSyntax basePropertyDeclarationSyntax, Binder outerBinder)
            {
                ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = basePropertyDeclarationSyntax.ExplicitInterfaceSpecifier;
                switch (basePropertyDeclarationSyntax.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                        {
                            PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)basePropertyDeclarationSyntax;
                            return ExplicitInterfaceHelpers.GetMemberName(outerBinder, explicitInterfaceSpecifier, propertyDeclarationSyntax.Identifier.ValueText);
                        }
                    case SyntaxKind.IndexerDeclaration:
                        return ExplicitInterfaceHelpers.GetMemberName(outerBinder, explicitInterfaceSpecifier, "this[]");
                    case SyntaxKind.EventFieldDeclaration:
                    case SyntaxKind.EventDeclaration:
                        {
                            EventDeclarationSyntax eventDeclarationSyntax = (EventDeclarationSyntax)basePropertyDeclarationSyntax;
                            return ExplicitInterfaceHelpers.GetMemberName(outerBinder, explicitInterfaceSpecifier, eventDeclarationSyntax.Identifier.ValueText);
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(basePropertyDeclarationSyntax.Kind());
                }
            }

            private SourceMemberMethodSymbol GetMethodSymbol(BaseMethodDeclarationSyntax baseMethodDeclarationSyntax, Binder outerBinder)
            {
                if (baseMethodDeclarationSyntax == _memberDeclarationOpt)
                {
                    return (SourceMemberMethodSymbol)_memberOpt;
                }
                NamedTypeSymbol containerType = GetContainerType(outerBinder, baseMethodDeclarationSyntax);
                if ((object)containerType == null)
                {
                    return null;
                }
                string methodName = GetMethodName(baseMethodDeclarationSyntax, outerBinder);
                return (SourceMemberMethodSymbol)GetMemberSymbol(methodName, baseMethodDeclarationSyntax.FullSpan, containerType, SymbolKind.Method);
            }

            private SourcePropertySymbol GetPropertySymbol(BasePropertyDeclarationSyntax basePropertyDeclarationSyntax, Binder outerBinder)
            {
                if (basePropertyDeclarationSyntax == _memberDeclarationOpt)
                {
                    return (SourcePropertySymbol)_memberOpt;
                }
                NamedTypeSymbol containerType = GetContainerType(outerBinder, basePropertyDeclarationSyntax);
                if ((object)containerType == null)
                {
                    return null;
                }
                string propertyOrEventName = GetPropertyOrEventName(basePropertyDeclarationSyntax, outerBinder);
                return (SourcePropertySymbol)GetMemberSymbol(propertyOrEventName, basePropertyDeclarationSyntax.Span, containerType, SymbolKind.Property);
            }

            private SourceEventSymbol GetEventSymbol(EventDeclarationSyntax eventDeclarationSyntax, Binder outerBinder)
            {
                if (eventDeclarationSyntax == _memberDeclarationOpt)
                {
                    return (SourceEventSymbol)_memberOpt;
                }
                NamedTypeSymbol containerType = GetContainerType(outerBinder, eventDeclarationSyntax);
                if ((object)containerType == null)
                {
                    return null;
                }
                string propertyOrEventName = GetPropertyOrEventName(eventDeclarationSyntax, outerBinder);
                return (SourceEventSymbol)GetMemberSymbol(propertyOrEventName, eventDeclarationSyntax.Span, containerType, SymbolKind.Event);
            }

            private Symbol GetMemberSymbol(string memberName, TextSpan memberSpan, NamedTypeSymbol container, SymbolKind kind)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers(memberName).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind != kind)
                    {
                        continue;
                    }
                    if (current.Kind == SymbolKind.Method)
                    {
                        if (InSpan(current.Locations[0], _syntaxTree, memberSpan))
                        {
                            return current;
                        }
                        MethodSymbol partialImplementationPart = ((MethodSymbol)current).PartialImplementationPart;
                        if ((object)partialImplementationPart != null && InSpan(partialImplementationPart.Locations[0], _syntaxTree, memberSpan))
                        {
                            return partialImplementationPart;
                        }
                    }
                    else if (InSpan(current.Locations, _syntaxTree, memberSpan))
                    {
                        return current;
                    }
                }
                return null;
            }

            private static bool InSpan(Location location, SyntaxTree syntaxTree, TextSpan span)
            {
                if (location.SourceTree == syntaxTree)
                {
                    return span.Contains(location.SourceSpan);
                }
                return false;
            }

            private static bool InSpan(ImmutableArray<Location> locations, SyntaxTree syntaxTree, TextSpan span)
            {
                ImmutableArray<Location>.Enumerator enumerator = locations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (InSpan(enumerator.Current, syntaxTree, span))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override Binder VisitDelegateDeclaration(DelegateDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInDelegateDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                BinderCacheKey key = CreateBinderCacheKey(parent, NodeUsage.Normal);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    Binder binder = VisitCore(parent.Parent);
                    SourceNamedTypeSymbol sourceTypeMember = ((NamespaceOrTypeSymbol)binder.ContainingMemberOrLambda).GetSourceTypeMember(parent);
                    value = new InContainerBinder(sourceTypeMember, binder);
                    if (parent.TypeParameterList != null)
                    {
                        value = new WithClassTypeParametersBinder(sourceTypeMember, value);
                    }
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitEnumDeclaration(EnumDeclarationSyntax parent)
            {
                if (!LookupPosition.IsBetweenTokens(_position, parent.OpenBraceToken, parent.CloseBraceToken) && !LookupPosition.IsInAttributeSpecification(_position, parent.AttributeLists))
                {
                    return VisitCore(parent.Parent);
                }
                BinderCacheKey key = CreateBinderCacheKey(parent, NodeUsage.Normal);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    Binder binder = VisitCore(parent.Parent);
                    value = new InContainerBinder(((NamespaceOrTypeSymbol)binder.ContainingMemberOrLambda).GetSourceTypeMember(parent.Identifier.ValueText, 0, SyntaxKind.EnumDeclaration, parent), binder);
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private Binder VisitTypeDeclarationCore(TypeDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInTypeDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                NodeUsage extraInfo = NodeUsage.Normal;
                if (parent.OpenBraceToken != default(SyntaxToken) && parent.CloseBraceToken != default(SyntaxToken) && (LookupPosition.IsBetweenTokens(_position, parent.OpenBraceToken, parent.CloseBraceToken) || LookupPosition.IsInAttributeSpecification(_position, parent.AttributeLists)))
                {
                    extraInfo = NodeUsage.MethodBody;
                }
                else if (LookupPosition.IsInTypeParameterList(_position, parent))
                {
                    extraInfo = NodeUsage.MethodBody;
                }
                else if (LookupPosition.IsBetweenTokens(_position, parent.Keyword, parent.OpenBraceToken))
                {
                    extraInfo = NodeUsage.NamedTypeBaseListOrParameterList;
                }
                return VisitTypeDeclarationCore(parent, extraInfo);
            }

            internal Binder VisitTypeDeclarationCore(TypeDeclarationSyntax parent, NodeUsage extraInfo)
            {
                BinderCacheKey key = CreateBinderCacheKey(parent, extraInfo);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = VisitCore(parent.Parent);
                    if (extraInfo != 0)
                    {
                        SourceNamedTypeSymbol sourceTypeMember = ((NamespaceOrTypeSymbol)value.ContainingMemberOrLambda).GetSourceTypeMember(parent);
                        if (extraInfo == NodeUsage.NamedTypeBaseListOrParameterList)
                        {
                            value = new WithClassTypeParametersBinder(sourceTypeMember, value);
                        }
                        else
                        {
                            value = new InContainerBinder(sourceTypeMember, value);
                            if (parent.TypeParameterList != null)
                            {
                                value = new WithClassTypeParametersBinder(sourceTypeMember, value);
                            }
                        }
                    }
                    value = value.WithUnsafeRegionIfNecessary(parent.Modifiers);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                return VisitTypeDeclarationCore(node);
            }

            public override Binder VisitStructDeclaration(StructDeclarationSyntax node)
            {
                return VisitTypeDeclarationCore(node);
            }

            public override Binder VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                return VisitTypeDeclarationCore(node);
            }

            public override Binder VisitRecordDeclaration(RecordDeclarationSyntax node)
            {
                return VisitTypeDeclarationCore(node);
            }

            public override Binder VisitNamespaceDeclaration(NamespaceDeclarationSyntax parent)
            {
                if (!LookupPosition.IsInNamespaceDeclaration(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                bool inBody = LookupPosition.IsBetweenTokens(_position, parent.OpenBraceToken, parent.CloseBraceToken);
                bool inUsing = IsInUsing(parent);
                return VisitNamespaceDeclaration(parent, _position, inBody, inUsing);
            }

            internal Binder VisitNamespaceDeclaration(NamespaceDeclarationSyntax parent, int position, bool inBody, bool inUsing)
            {
                NodeUsage usage = (inUsing ? NodeUsage.MethodBody : (inBody ? NodeUsage.MethodTypeParameters : NodeUsage.Normal));
                BinderCacheKey key = CreateBinderCacheKey(parent, usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    CSharpSyntaxNode parent2 = parent.Parent;
                    Binder binder = ((!InScript || parent2.Kind() != SyntaxKind.CompilationUnit) ? _factory.GetBinder(parent.Parent, position) : VisitCompilationUnit((CompilationUnitSyntax)parent2, inUsing: false, inScript: false));
                    value = (inBody ? MakeNamespaceBinder(parent, parent.Name, binder, inUsing) : binder);
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private static Binder MakeNamespaceBinder(CSharpSyntaxNode node, NameSyntax name, Binder outer, bool inUsing)
            {
                if (name is QualifiedNameSyntax qualifiedNameSyntax)
                {
                    outer = MakeNamespaceBinder(qualifiedNameSyntax.Left, qualifiedNameSyntax.Left, outer, inUsing: false);
                    name = qualifiedNameSyntax.Right;
                }
                NamespaceOrTypeSymbol namespaceOrTypeSymbol = ((!(outer is InContainerBinder inContainerBinder)) ? outer.Compilation.GlobalNamespace : inContainerBinder.Container);
                NamespaceSymbol nestedNamespace = ((NamespaceSymbol)namespaceOrTypeSymbol).GetNestedNamespace(name);
                if ((object)nestedNamespace == null)
                {
                    return outer;
                }
                if (node is NamespaceDeclarationSyntax declarationSyntax)
                {
                    outer = AddInImportsBinders((SourceNamespaceSymbol)outer.Compilation.SourceModule.GetModuleNamespace(nestedNamespace), declarationSyntax, outer, inUsing);
                }
                return new InContainerBinder(nestedNamespace, outer);
            }

            public override Binder VisitCompilationUnit(CompilationUnitSyntax parent)
            {
                return VisitCompilationUnit(parent, IsInUsing(parent), InScript);
            }

            internal Binder VisitCompilationUnit(CompilationUnitSyntax compilationUnit, bool inUsing, bool inScript)
            {
                if (compilationUnit != _syntaxTree.GetRoot())
                {
                    throw new ArgumentOutOfRangeException("compilationUnit", "node not part of tree");
                }
                NodeUsage usage = ((!inUsing) ? (inScript ? NodeUsage.MethodBody : NodeUsage.Normal) : ((!inScript) ? NodeUsage.MethodTypeParameters : NodeUsage.NamedTypeBaseListOrParameterList));
                BinderCacheKey key = CreateBinderCacheKey(compilationUnit, usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = buckStopsHereBinder;
                    if (inScript)
                    {
                        bool flag = compilation.IsSubmissionSyntaxTree(compilationUnit.SyntaxTree);
                        NamedTypeSymbol scriptClass = compilation.ScriptClass;
                        bool isSubmissionClass = scriptClass.IsSubmissionClass;
                        if (!inUsing)
                        {
                            value = WithUsingNamespacesAndTypesBinder.Create(compilation.GlobalImports, value, withImportChainEntry: true);
                            if (isSubmissionClass)
                            {
                                value = WithUsingNamespacesAndTypesBinder.Create((SourceNamespaceSymbol)compilation.SourceModule.GlobalNamespace, compilationUnit, value, compilation.PreviousSubmission != null && flag, withImportChainEntry: true);
                            }
                        }
                        value = new InContainerBinder(compilation.GlobalNamespace, value);
                        if (compilation.HostObjectType != null)
                        {
                            value = new HostObjectModelBinder(value);
                        }
                        if (isSubmissionClass)
                        {
                            value = new InSubmissionClassBinder(scriptClass, value, compilationUnit, inUsing);
                        }
                        else
                        {
                            value = AddInImportsBinders((SourceNamespaceSymbol)compilation.SourceModule.GlobalNamespace, compilationUnit, value, inUsing);
                            value = new InContainerBinder(scriptClass, value);
                        }
                    }
                    else
                    {
                        NamespaceSymbol globalNamespace = compilation.GlobalNamespace;
                        value = AddInImportsBinders((SourceNamespaceSymbol)compilation.SourceModule.GlobalNamespace, compilationUnit, value, inUsing);
                        value = new InContainerBinder(globalNamespace, value);
                        if (!inUsing)
                        {
                            SynthesizedSimpleProgramEntryPointSymbol simpleProgramEntryPoint = SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(compilation, compilationUnit, fallbackToMainEntryPoint: true);
                            if ((object)simpleProgramEntryPoint != null)
                            {
                                ExecutableCodeBinder bodyBinder = simpleProgramEntryPoint.GetBodyBinder(_factory._ignoreAccessibility);
                                value = new SimpleProgramUnitBinder(value, (SimpleProgramBinder)bodyBinder.GetBinder(simpleProgramEntryPoint.SyntaxNode));
                            }
                        }
                    }
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private static Binder AddInImportsBinders(SourceNamespaceSymbol declaringSymbol, CSharpSyntaxNode declarationSyntax, Binder next, bool inUsing)
            {
                if (inUsing)
                {
                    return WithExternAliasesBinder.Create(declaringSymbol, declarationSyntax, next);
                }
                return WithExternAndUsingAliasesBinder.Create(declaringSymbol, declarationSyntax, WithUsingNamespacesAndTypesBinder.Create(declaringSymbol, declarationSyntax, next));
            }

            internal static BinderCacheKey CreateBinderCacheKey(CSharpSyntaxNode node, NodeUsage usage)
            {
                return new BinderCacheKey(node, usage);
            }

            private bool IsInUsing(CSharpSyntaxNode containingNode)
            {
                TextSpan span = containingNode.Span;
                SyntaxToken syntaxToken;
                if (containingNode.Kind() != SyntaxKind.CompilationUnit && _position == span.End)
                {
                    syntaxToken = containingNode.GetLastToken();
                }
                else
                {
                    if (_position < span.Start || _position > span.End)
                    {
                        return false;
                    }
                    syntaxToken = containingNode.FindToken(_position);
                }
                SyntaxNode parent = syntaxToken.Parent;
                while (parent != null && parent != containingNode)
                {
                    if (parent.IsKind(SyntaxKind.UsingDirective) && parent.Parent == containingNode)
                    {
                        return true;
                    }
                    parent = parent.Parent;
                }
                return false;
            }

            public override Binder VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax parent)
            {
                return VisitCore(parent.ParentTrivia.Token.Parent);
            }

            public override Binder VisitCrefParameter(CrefParameterSyntax parent)
            {
                XmlCrefAttributeSyntax parent2 = parent.FirstAncestorOrSelf<XmlCrefAttributeSyntax>(null, ascendOutOfTrivia: false);
                return VisitXmlCrefAttributeInternal(parent2, NodeUsage.MethodTypeParameters);
            }

            public override Binder VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax parent)
            {
                if (parent.Type.Span.Contains(_position))
                {
                    XmlCrefAttributeSyntax parent2 = parent.FirstAncestorOrSelf<XmlCrefAttributeSyntax>(null, ascendOutOfTrivia: false);
                    return VisitXmlCrefAttributeInternal(parent2, NodeUsage.MethodTypeParameters);
                }
                return base.VisitConversionOperatorMemberCref(parent);
            }

            public override Binder VisitXmlCrefAttribute(XmlCrefAttributeSyntax parent)
            {
                if (!LookupPosition.IsInXmlAttributeValue(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                NodeUsage extraInfo = NodeUsage.Normal;
                return VisitXmlCrefAttributeInternal(parent, extraInfo);
            }

            private Binder VisitXmlCrefAttributeInternal(XmlCrefAttributeSyntax parent, NodeUsage extraInfo)
            {
                BinderCacheKey key = CreateBinderCacheKey(parent, extraInfo);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    CrefSyntax cref = parent.Cref;
                    MemberDeclarationSyntax associatedMemberForXmlSyntax = GetAssociatedMemberForXmlSyntax(parent);
                    bool inParameterOrReturnType = extraInfo == NodeUsage.MethodTypeParameters;
                    value = ((associatedMemberForXmlSyntax == null) ? MakeCrefBinderInternal(cref, VisitCore(parent.Parent), inParameterOrReturnType) : MakeCrefBinder(cref, associatedMemberForXmlSyntax, _factory, inParameterOrReturnType));
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            public override Binder VisitXmlNameAttribute(XmlNameAttributeSyntax parent)
            {
                if (!LookupPosition.IsInXmlAttributeValue(_position, parent))
                {
                    return VisitCore(parent.Parent);
                }
                XmlNameAttributeElementKind elementKind = parent.GetElementKind();
                NodeUsage usage;
                switch (elementKind)
                {
                    case XmlNameAttributeElementKind.Parameter:
                    case XmlNameAttributeElementKind.ParameterReference:
                        usage = NodeUsage.MethodTypeParameters;
                        break;
                    case XmlNameAttributeElementKind.TypeParameter:
                        usage = NodeUsage.MethodBody;
                        break;
                    case XmlNameAttributeElementKind.TypeParameterReference:
                        usage = NodeUsage.NamedTypeBaseListOrParameterList;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(elementKind);
                }
                BinderCacheKey key = CreateBinderCacheKey(GetEnclosingDocumentationComment(parent), usage);
                if (!binderCache.TryGetValue(key, out var value))
                {
                    value = buckStopsHereBinder;
                    Binder binder = VisitCore(GetEnclosingDocumentationComment(parent));
                    if (binder != null)
                    {
                        value = value.WithContainingMemberOrLambda(binder.ContainingMemberOrLambda);
                    }
                    MemberDeclarationSyntax associatedMemberForXmlSyntax = GetAssociatedMemberForXmlSyntax(parent);
                    if (associatedMemberForXmlSyntax != null)
                    {
                        switch (elementKind)
                        {
                            case XmlNameAttributeElementKind.Parameter:
                            case XmlNameAttributeElementKind.ParameterReference:
                                value = GetParameterNameAttributeValueBinder(associatedMemberForXmlSyntax, value);
                                break;
                            case XmlNameAttributeElementKind.TypeParameter:
                                value = GetTypeParameterNameAttributeValueBinder(associatedMemberForXmlSyntax, includeContainingSymbols: false, value);
                                break;
                            case XmlNameAttributeElementKind.TypeParameterReference:
                                value = GetTypeParameterNameAttributeValueBinder(associatedMemberForXmlSyntax, includeContainingSymbols: true, value);
                                break;
                        }
                    }
                    binderCache.TryAdd(key, value);
                }
                return value;
            }

            private Binder GetParameterNameAttributeValueBinder(MemberDeclarationSyntax memberSyntax, Binder nextBinder)
            {
                if (memberSyntax is BaseMethodDeclarationSyntax baseMethodDeclarationSyntax)
                {
                    ParameterListSyntax parameterList = baseMethodDeclarationSyntax.ParameterList;
                    if (parameterList != null && parameterList.ParameterCount > 0)
                    {
                        Binder outerBinder = VisitCore(memberSyntax.Parent);
                        return new WithParametersBinder(GetMethodSymbol(baseMethodDeclarationSyntax, outerBinder).Parameters, nextBinder);
                    }
                }
                if (memberSyntax is RecordDeclarationSyntax recordDeclarationSyntax)
                {
                    ParameterListSyntax parameterList = recordDeclarationSyntax.ParameterList;
                    if (parameterList != null && parameterList.ParameterCount > 0)
                    {
                        SynthesizedRecordConstructor synthesizedRecordConstructor = ((NamespaceOrTypeSymbol)VisitCore(memberSyntax).ContainingMemberOrLambda).GetSourceTypeMember((TypeDeclarationSyntax)memberSyntax)!.GetMembersUnordered().OfType<SynthesizedRecordConstructor>().SingleOrDefault();
                        if (synthesizedRecordConstructor.SyntaxRef.SyntaxTree == memberSyntax.SyntaxTree && synthesizedRecordConstructor.GetSyntax() == memberSyntax)
                        {
                            return new WithParametersBinder(synthesizedRecordConstructor.Parameters, nextBinder);
                        }
                    }
                }
                switch (memberSyntax.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                        {
                            Binder outerBinder2 = VisitCore(memberSyntax.Parent);
                            BasePropertyDeclarationSyntax basePropertyDeclarationSyntax = (BasePropertyDeclarationSyntax)memberSyntax;
                            PropertySymbol propertySymbol = GetPropertySymbol(basePropertyDeclarationSyntax, outerBinder2);
                            ImmutableArray<ParameterSymbol> immutableArray = propertySymbol.Parameters;
                            if ((object)propertySymbol.SetMethod != null)
                            {
                                immutableArray = immutableArray.Add(propertySymbol.SetMethod.Parameters.Last());
                            }
                            if (immutableArray.Any())
                            {
                                return new WithParametersBinder(immutableArray, nextBinder);
                            }
                            break;
                        }
                    case SyntaxKind.DelegateDeclaration:
                        {
                            ImmutableArray<ParameterSymbol> parameters = ((NamespaceOrTypeSymbol)VisitCore(memberSyntax.Parent).ContainingMemberOrLambda).GetSourceTypeMember((DelegateDeclarationSyntax)memberSyntax)!.DelegateInvokeMethod.Parameters;
                            if (parameters.Any())
                            {
                                return new WithParametersBinder(parameters, nextBinder);
                            }
                            break;
                        }
                }
                return nextBinder;
            }

            private Binder GetTypeParameterNameAttributeValueBinder(MemberDeclarationSyntax memberSyntax, bool includeContainingSymbols, Binder nextBinder)
            {
                if (includeContainingSymbols)
                {
                    NamedTypeSymbol containingType = VisitCore(memberSyntax.Parent).ContainingType;
                    while ((object)containingType != null)
                    {
                        if (containingType.Arity > 0)
                        {
                            nextBinder = new WithClassTypeParametersBinder(containingType, nextBinder);
                        }
                        containingType = containingType.ContainingType;
                    }
                }
                if (memberSyntax is TypeDeclarationSyntax typeDeclarationSyntax && typeDeclarationSyntax.Arity > 0)
                {
                    return new WithClassTypeParametersBinder(((NamespaceOrTypeSymbol)VisitCore(memberSyntax.Parent).ContainingMemberOrLambda).GetSourceTypeMember(typeDeclarationSyntax), nextBinder);
                }
                if (memberSyntax.Kind() == SyntaxKind.MethodDeclaration)
                {
                    MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)memberSyntax;
                    if (methodDeclarationSyntax.Arity > 0)
                    {
                        Binder outerBinder = VisitCore(memberSyntax.Parent);
                        return new WithMethodTypeParametersBinder(GetMethodSymbol(methodDeclarationSyntax, outerBinder), nextBinder);
                    }
                }
                else if (memberSyntax.Kind() == SyntaxKind.DelegateDeclaration)
                {
                    SourceNamedTypeSymbol sourceTypeMember = ((NamespaceOrTypeSymbol)VisitCore(memberSyntax.Parent).ContainingMemberOrLambda).GetSourceTypeMember((DelegateDeclarationSyntax)memberSyntax);
                    if (sourceTypeMember.TypeParameters.Any())
                    {
                        return new WithClassTypeParametersBinder(sourceTypeMember, nextBinder);
                    }
                }
                return nextBinder;
            }
        }

        private struct BinderCacheKey : IEquatable<BinderCacheKey>
        {
            public readonly CSharpSyntaxNode syntaxNode;

            public readonly NodeUsage usage;

            public BinderCacheKey(CSharpSyntaxNode syntaxNode, NodeUsage usage)
            {
                this.syntaxNode = syntaxNode;
                this.usage = usage;
            }

            bool IEquatable<BinderCacheKey>.Equals(BinderCacheKey other)
            {
                if (syntaxNode == other.syntaxNode)
                {
                    return usage == other.usage;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(syntaxNode.GetHashCode(), (int)usage);
            }

            public override bool Equals(object obj)
            {
                throw new NotSupportedException();
            }
        }

        internal enum NodeUsage : byte
        {
            Normal = 0,
            MethodTypeParameters = 1,
            MethodBody = 2,
            ConstructorBodyOrInitializer = 1,
            AccessorBody = 1,
            OperatorBody = 1,
            NamedTypeBodyOrTypeParameters = 2,
            NamedTypeBaseListOrParameterList = 4,
            NamespaceBody = 1,
            NamespaceUsings = 2,
            CompilationUnitUsings = 1,
            CompilationUnitScript = 2,
            CompilationUnitScriptUsings = 4,
            DocumentationCommentParameter = 1,
            DocumentationCommentTypeParameter = 2,
            DocumentationCommentTypeParameterReference = 4,
            CrefParameterOrReturnType = 1
        }

        private readonly ConcurrentCache<BinderCacheKey, Binder> _binderCache;

        private readonly CSharpCompilation _compilation;

        private readonly SyntaxTree _syntaxTree;

        private readonly BuckStopsHereBinder _buckStopsHereBinder;

        private readonly bool _ignoreAccessibility;

        private readonly ObjectPool<BinderFactoryVisitor> _binderFactoryVisitorPool;

        internal SyntaxTree SyntaxTree => _syntaxTree;

        private bool InScript => _syntaxTree.Options.Kind == SourceCodeKind.Script;

        internal static Binder MakeCrefBinder(CrefSyntax crefSyntax, MemberDeclarationSyntax memberSyntax, BinderFactory factory, bool inParameterOrReturnType = false)
        {
            Binder binder = ((!(memberSyntax is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)) ? factory.GetBinder(memberSyntax) : factory.GetBinder(memberSyntax, baseTypeDeclarationSyntax.OpenBraceToken.SpanStart));
            return MakeCrefBinderInternal(crefSyntax, binder, inParameterOrReturnType);
        }

        private static Binder MakeCrefBinderInternal(CrefSyntax crefSyntax, Binder binder, bool inParameterOrReturnType)
        {
            BinderFlags binderFlags = BinderFlags.SuppressConstraintChecks | BinderFlags.Cref | BinderFlags.UnsafeRegion;
            if (inParameterOrReturnType)
            {
                binderFlags |= BinderFlags.CrefParameterOrReturnType;
            }
            binder = binder.WithAdditionalFlags(binderFlags);
            binder = new WithCrefTypeParametersBinder(crefSyntax, binder);
            return binder;
        }

        internal static MemberDeclarationSyntax GetAssociatedMemberForXmlSyntax(CSharpSyntaxNode xmlSyntax)
        {
            for (CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)GetEnclosingDocumentationComment(xmlSyntax).ParentTrivia.Token.Parent; cSharpSyntaxNode != null; cSharpSyntaxNode = cSharpSyntaxNode.Parent)
            {
                if (cSharpSyntaxNode is MemberDeclarationSyntax result)
                {
                    return result;
                }
            }
            return null;
        }

        private static DocumentationCommentTriviaSyntax GetEnclosingDocumentationComment(CSharpSyntaxNode xmlSyntax)
        {
            CSharpSyntaxNode cSharpSyntaxNode = xmlSyntax;
            while (!SyntaxFacts.IsDocumentationCommentTrivia(cSharpSyntaxNode.Kind()))
            {
                cSharpSyntaxNode = cSharpSyntaxNode.Parent;
            }
            return (DocumentationCommentTriviaSyntax)cSharpSyntaxNode;
        }

        internal BinderFactory(CSharpCompilation compilation, SyntaxTree syntaxTree, bool ignoreAccessibility)
        {
            _compilation = compilation;
            _syntaxTree = syntaxTree;
            _ignoreAccessibility = ignoreAccessibility;
            _binderFactoryVisitorPool = new ObjectPool<BinderFactoryVisitor>(() => new BinderFactoryVisitor(this), 64);
            _binderCache = new ConcurrentCache<BinderCacheKey, Binder>(50);
            _buckStopsHereBinder = new BuckStopsHereBinder(compilation);
        }

        internal Binder GetBinder(SyntaxNode node, CSharpSyntaxNode memberDeclarationOpt = null, Symbol memberOpt = null)
        {
            int spanStart = node.SpanStart;
            if ((!InScript || node.Kind() != SyntaxKind.CompilationUnit) && node.Parent != null)
            {
                node = node.Parent;
            }
            return GetBinder(node, spanStart, memberDeclarationOpt, memberOpt);
        }

        internal Binder GetBinder(SyntaxNode node, int position, CSharpSyntaxNode memberDeclarationOpt = null, Symbol memberOpt = null)
        {
            BinderFactoryVisitor binderFactoryVisitor = _binderFactoryVisitorPool.Allocate();
            binderFactoryVisitor.Initialize(position, memberDeclarationOpt, memberOpt);
            Binder result = binderFactoryVisitor.Visit(node);
            _binderFactoryVisitorPool.Free(binderFactoryVisitor);
            return result;
        }

        internal InMethodBinder GetRecordConstructorInMethodBinder(SynthesizedRecordConstructor constructor)
        {
            RecordDeclarationSyntax syntax = constructor.GetSyntax();
            NodeUsage usage = NodeUsage.MethodTypeParameters;
            BinderCacheKey key = BinderFactoryVisitor.CreateBinderCacheKey(syntax, usage);
            if (!_binderCache.TryGetValue(key, out var value))
            {
                value = new InMethodBinder(constructor, GetInRecordBodyBinder(syntax));
                _binderCache.TryAdd(key, value);
            }
            return (InMethodBinder)value;
        }

        internal Binder GetInRecordBodyBinder(RecordDeclarationSyntax typeDecl)
        {
            BinderFactoryVisitor binderFactoryVisitor = _binderFactoryVisitorPool.Allocate();
            binderFactoryVisitor.Initialize(typeDecl.SpanStart, null, null);
            Binder result = binderFactoryVisitor.VisitTypeDeclarationCore(typeDecl, NodeUsage.MethodBody);
            _binderFactoryVisitorPool.Free(binderFactoryVisitor);
            return result;
        }

        internal Binder GetInNamespaceBinder(CSharpSyntaxNode unit)
        {
            switch (unit.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        BinderFactoryVisitor binderFactoryVisitor2 = _binderFactoryVisitorPool.Allocate();
                        binderFactoryVisitor2.Initialize(0, null, null);
                        Binder result2 = binderFactoryVisitor2.VisitNamespaceDeclaration((NamespaceDeclarationSyntax)unit, unit.SpanStart, inBody: true, inUsing: false);
                        _binderFactoryVisitorPool.Free(binderFactoryVisitor2);
                        return result2;
                    }
                case SyntaxKind.CompilationUnit:
                    {
                        BinderFactoryVisitor binderFactoryVisitor = _binderFactoryVisitorPool.Allocate();
                        binderFactoryVisitor.Initialize(0, null, null);
                        Binder result = binderFactoryVisitor.VisitCompilationUnit((CompilationUnitSyntax)unit, inUsing: false, InScript);
                        _binderFactoryVisitorPool.Free(binderFactoryVisitor);
                        return result;
                    }
                default:
                    return null;
            }
        }
    }
}
