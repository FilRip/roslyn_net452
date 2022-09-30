using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public class SyntaxTreeSemanticModel : CSharpSemanticModel
    {
        private readonly CSharpCompilation _compilation;

        private readonly SyntaxTree _syntaxTree;

        private ImmutableDictionary<CSharpSyntaxNode, MemberSemanticModel> _memberModels = ImmutableDictionary<CSharpSyntaxNode, MemberSemanticModel>.Empty;

        private readonly BinderFactory _binderFactory;

        private readonly Func<CSharpSyntaxNode, MemberSemanticModel> _createMemberModelFunction;

        private readonly bool _ignoresAccessibility;

        private ScriptLocalScopeBinder.Labels _globalStatementLabels;

        private static readonly Func<CSharpSyntaxNode, bool> s_isMemberDeclarationFunction = IsMemberDeclaration;

        public override CSharpCompilation Compilation => _compilation;

        internal override CSharpSyntaxNode Root => (CSharpSyntaxNode)_syntaxTree.GetRoot();

        public override SyntaxTree SyntaxTree => _syntaxTree;

        public override bool IgnoresAccessibility => _ignoresAccessibility;

        public override bool IsSpeculativeSemanticModel => false;

        public override int OriginalPositionForSpeculation => 0;

        public override CSharpSemanticModel ParentModel => null;

        public override SemanticModel ContainingModelOrSelf => this;

        internal ImmutableDictionary<CSharpSyntaxNode, MemberSemanticModel> TestOnlyMemberModels => _memberModels;

        private bool IsRegularCSharp => SyntaxTree.Options.Kind == SourceCodeKind.Regular;

        internal SyntaxTreeSemanticModel(CSharpCompilation compilation, SyntaxTree syntaxTree, bool ignoreAccessibility = false)
        {
            _compilation = compilation;
            _syntaxTree = syntaxTree;
            _ignoresAccessibility = ignoreAccessibility;
            if (!Compilation.SyntaxTrees.Contains(syntaxTree))
            {
                throw new ArgumentOutOfRangeException("syntaxTree", CSharpResources.TreeNotPartOfCompilation);
            }
            _binderFactory = compilation.GetBinderFactory(SyntaxTree, ignoreAccessibility);
        }

        internal SyntaxTreeSemanticModel(CSharpCompilation parentCompilation, SyntaxTree parentSyntaxTree, SyntaxTree speculatedSyntaxTree)
        {
            _compilation = parentCompilation;
            _syntaxTree = speculatedSyntaxTree;
            _binderFactory = _compilation.GetBinderFactory(parentSyntaxTree);
        }

        private void VerifySpanForGetDiagnostics(TextSpan? span)
        {
            if (span.HasValue && !Root.FullSpan.Contains(span.Value))
            {
                throw new ArgumentException("span");
            }
        }

        public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifySpanForGetDiagnostics(span);
            return Compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Parse, SyntaxTree, span, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifySpanForGetDiagnostics(span);
            return Compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Declare, SyntaxTree, span, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifySpanForGetDiagnostics(span);
            return Compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, SyntaxTree, span, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            VerifySpanForGetDiagnostics(span);
            return Compilation.GetDiagnosticsForSyntaxTree(CompilationStage.Compile, SyntaxTree, span, includeEarlierStages: true, cancellationToken);
        }

        internal override Binder GetEnclosingBinderInternal(int position)
        {
            SyntaxToken syntaxToken = Root.FindTokenIncludingCrefAndNameAttributes(position);
            if (position == 0 && position != syntaxToken.SpanStart)
            {
                return _binderFactory.GetBinder(Root, position).WithAdditionalFlags(GetSemanticModelBinderFlags());
            }
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.GetEnclosingBinder(position);
            }
            return _binderFactory.GetBinder((CSharpSyntaxNode)syntaxToken.Parent, position).WithAdditionalFlags(GetSemanticModelBinderFlags());
        }

        internal override IOperation GetOperationWorker(CSharpSyntaxNode node, CancellationToken cancellationToken)
        {
            return ((node is ConstructorDeclarationSyntax constructorDeclarationSyntax) ? ((constructorDeclarationSyntax.HasAnyBody() || constructorDeclarationSyntax.Initializer != null) ? GetOrAddModel(node) : null) : ((node is BaseMethodDeclarationSyntax declaration) ? (declaration.HasAnyBody() ? GetOrAddModel(node) : null) : ((node is AccessorDeclarationSyntax accessorDeclarationSyntax) ? ((accessorDeclarationSyntax.Body != null || accessorDeclarationSyntax.ExpressionBody != null) ? GetOrAddModel(node) : null) : ((!(node is RecordDeclarationSyntax recordDeclarationSyntax) || recordDeclarationSyntax.ParameterList == null || recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass == null || (object)TryGetSynthesizedRecordConstructor(recordDeclarationSyntax) == null) ? GetMemberModel(node) : GetOrAddModel(recordDeclarationSyntax)))))?.GetOperationWorker(node, cancellationToken);
        }

        internal override SymbolInfo GetSymbolInfoWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            CSharpSemanticModel.ValidateSymbolInfoOptions(options);
            node = SyntaxFactory.GetStandaloneNode(node);
            MemberSemanticModel memberModel = GetMemberModel(node);
            SymbolInfo result;
            XmlNameAttributeSyntax syntax;
            if (memberModel != null)
            {
                result = memberModel.GetSymbolInfoWorker(node, options, cancellationToken);
                if (result.Symbol == null && result.CandidateReason == CandidateReason.None && node is ExpressionSyntax && SyntaxFacts.IsInNamespaceOrTypeContext((ExpressionSyntax)node))
                {
                    Binder enclosingBinder = GetEnclosingBinder(GetAdjustedNodePosition(node));
                    if (enclosingBinder != null)
                    {
                        enclosingBinder = new LocalScopeBinder(enclosingBinder);
                        BoundExpression boundExpression = enclosingBinder.BindExpression((ExpressionSyntax)node, BindingDiagnosticBag.Discarded);
                        SymbolInfo symbolInfoForNode = GetSymbolInfoForNode(options, boundExpression, boundExpression, null, null);
                        if (symbolInfoForNode.Symbol != null)
                        {
                            result = new SymbolInfo(null, ImmutableArray.Create(symbolInfoForNode.Symbol), CandidateReason.NotATypeOrNamespace);
                        }
                        else if (!symbolInfoForNode.CandidateSymbols.IsEmpty)
                        {
                            result = new SymbolInfo(null, symbolInfoForNode.CandidateSymbols, CandidateReason.NotATypeOrNamespace);
                        }
                    }
                }
            }
            else if (node.Parent!.Kind() == SyntaxKind.XmlNameAttribute && (syntax = (XmlNameAttributeSyntax)node.Parent).Identifier == node)
            {
                result = SymbolInfo.None;
                Binder enclosingBinder2 = GetEnclosingBinder(GetAdjustedNodePosition(node));
                if (enclosingBinder2 != null)
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    ImmutableArray<Symbol> symbols = enclosingBinder2.BindXmlNameAttribute(syntax, ref useSiteInfo);
                    result = symbols.Length switch
                    {
                        0 => SymbolInfo.None,
                        1 => SymbolInfoFactory.Create(symbols, LookupResultKind.Viable, isDynamic: false),
                        _ => SymbolInfoFactory.Create(symbols, LookupResultKind.Ambiguous, isDynamic: false),
                    };
                }
            }
            else if (node is CrefSyntax crefSyntax)
            {
                int adjustedNodePosition = GetAdjustedNodePosition(crefSyntax);
                result = GetCrefSymbolInfo(adjustedNodePosition, crefSyntax, options, CSharpSemanticModel.HasParameterList(crefSyntax));
            }
            else
            {
                Symbol semanticInfoSymbolInNonMemberContext = GetSemanticInfoSymbolInNonMemberContext(node, (options & SymbolInfoOptions.PreserveAliases) != 0);
                result = (((object)semanticInfoSymbolInNonMemberContext != null) ? CSharpSemanticModel.GetSymbolInfoForSymbol(semanticInfoSymbolInNonMemberContext, options) : SymbolInfo.None);
            }
            return result;
        }

        internal override SymbolInfo GetCollectionInitializerSymbolInfoWorker(InitializerExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetMemberModel(collectionInitializer)?.GetCollectionInitializerSymbolInfoWorker(collectionInitializer, node, cancellationToken) ?? SymbolInfo.None;
        }

        internal override CSharpTypeInfo GetTypeInfoWorker(CSharpSyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            node = SyntaxFactory.GetStandaloneNode(node);
            MemberSemanticModel memberModel = GetMemberModel(node);
            if (memberModel != null)
            {
                return memberModel.GetTypeInfoWorker(node, cancellationToken);
            }
            Symbol semanticInfoSymbolInNonMemberContext = GetSemanticInfoSymbolInNonMemberContext(node, bindVarAsAliasFirst: false);
            if ((object)semanticInfoSymbolInNonMemberContext == null)
            {
                return CSharpTypeInfo.None;
            }
            return CSharpSemanticModel.GetTypeInfoForSymbol(semanticInfoSymbolInNonMemberContext);
        }

        private Symbol GetSemanticInfoSymbolInNonMemberContext(CSharpSyntaxNode node, bool bindVarAsAliasFirst)
        {
            Binder enclosingBinder = GetEnclosingBinder(GetAdjustedNodePosition(node));
            if (enclosingBinder != null && node is TypeSyntax typeSyntax)
            {
                ConsList<TypeSymbol> basesBeingResolved = GetBasesBeingResolved(typeSyntax);
                if (SyntaxFacts.IsNamespaceAliasQualifier(typeSyntax))
                {
                    return enclosingBinder.BindNamespaceAliasSymbol(node as IdentifierNameSyntax, BindingDiagnosticBag.Discarded);
                }
                if (SyntaxFacts.IsInTypeOnlyContext(typeSyntax))
                {
                    if (!typeSyntax.IsVar)
                    {
                        return enclosingBinder.BindTypeOrAlias(typeSyntax, BindingDiagnosticBag.Discarded, basesBeingResolved).Symbol;
                    }
                    Symbol symbol = (bindVarAsAliasFirst ? enclosingBinder.BindTypeOrAlias(typeSyntax, BindingDiagnosticBag.Discarded, basesBeingResolved).Symbol : null);
                    if (((object)symbol == null || symbol.Kind == SymbolKind.ErrorType) && typeSyntax.Parent is VariableDeclarationSyntax variableDeclarationSyntax && variableDeclarationSyntax.Variables.Any())
                    {
                        FieldSymbol declaredFieldSymbol = GetDeclaredFieldSymbol(variableDeclarationSyntax.Variables.First());
                        if ((object)declaredFieldSymbol != null)
                        {
                            symbol = declaredFieldSymbol.Type;
                        }
                    }
                    return symbol ?? enclosingBinder.BindTypeOrAlias(typeSyntax, BindingDiagnosticBag.Discarded, basesBeingResolved).Symbol;
                }
                return enclosingBinder.BindNamespaceOrTypeOrAliasSymbol(typeSyntax, BindingDiagnosticBag.Discarded, basesBeingResolved, basesBeingResolved != null).Symbol;
            }
            return null;
        }

        internal override ImmutableArray<Symbol> GetMemberGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            node = SyntaxFactory.GetStandaloneNode(node);
            return GetMemberModel(node)?.GetMemberGroupWorker(node, options, cancellationToken) ?? ImmutableArray<Symbol>.Empty;
        }

        internal override ImmutableArray<IPropertySymbol> GetIndexerGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            node = SyntaxFactory.GetStandaloneNode(node);
            return GetMemberModel(node)?.GetIndexerGroupWorker(node, options, cancellationToken) ?? ImmutableArray<IPropertySymbol>.Empty;
        }

        internal override Optional<object> GetConstantValueWorker(CSharpSyntaxNode node, CancellationToken cancellationToken)
        {
            node = SyntaxFactory.GetStandaloneNode(node);
            return GetMemberModel(node)?.GetConstantValueWorker(node, cancellationToken) ?? default(Optional<object>);
        }

        public override QueryClauseInfo GetQueryClauseInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetQueryClauseInfo(node, cancellationToken) ?? default(QueryClauseInfo);
        }

        public override SymbolInfo GetSymbolInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetSymbolInfo(node, cancellationToken) ?? SymbolInfo.None;
        }

        public override TypeInfo GetTypeInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetTypeInfo(node, cancellationToken) ?? CSharpTypeInfo.None;
        }

        public override IPropertySymbol GetDeclaredSymbol(AnonymousObjectMemberDeclaratorSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            return GetMemberModel(declaratorSyntax)?.GetDeclaredSymbol(declaratorSyntax, cancellationToken);
        }

        public override INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            return GetMemberModel(declaratorSyntax)?.GetDeclaredSymbol(declaratorSyntax, cancellationToken);
        }

        public override INamedTypeSymbol GetDeclaredSymbol(TupleExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            return GetMemberModel(declaratorSyntax)?.GetDeclaredSymbol(declaratorSyntax, cancellationToken);
        }

        public override ISymbol GetDeclaredSymbol(ArgumentSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            return GetMemberModel(declaratorSyntax)?.GetDeclaredSymbol(declaratorSyntax, cancellationToken);
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetDeclaredSymbol(node, cancellationToken);
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(JoinIntoClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetDeclaredSymbol(node, cancellationToken);
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(QueryContinuationSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetDeclaredSymbol(node, cancellationToken);
        }

        public override SymbolInfo GetSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetMemberModel(node)?.GetSymbolInfo(node, cancellationToken) ?? SymbolInfo.None;
        }

        private ConsList<TypeSymbol> GetBasesBeingResolved(TypeSyntax expression)
        {
            while (expression != null && expression.Parent != null)
            {
                CSharpSyntaxNode parent = expression.Parent;
                if (parent is BaseTypeSyntax baseTypeSyntax && parent.Parent != null && parent.Parent!.Kind() == SyntaxKind.BaseList && baseTypeSyntax.Type == expression)
                {
                    BaseTypeDeclarationSyntax declarationSyntax = (BaseTypeDeclarationSyntax)parent.Parent!.Parent;
                    INamedTypeSymbol declaredSymbol = GetDeclaredSymbol(declarationSyntax);
                    return ConsList<TypeSymbol>.Empty.Prepend(declaredSymbol.GetSymbol()!.OriginalDefinition);
                }
                expression = expression.Parent as TypeSyntax;
            }
            return null;
        }

        public override Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination, bool isExplicitInSource = false)
        {
            TypeSymbol destination2 = destination.EnsureCSharpSymbolOrNull("destination");
            if (expression.Kind() == SyntaxKind.DeclarationExpression)
            {
                return Conversion.NoConversion;
            }
            if (isExplicitInSource)
            {
                return ClassifyConversionForCast(expression, destination2);
            }
            CheckSyntaxNode(expression);
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            return GetMemberModel(expression)?.ClassifyConversion(expression, destination) ?? Conversion.NoConversion;
        }

        internal override Conversion ClassifyConversionForCast(ExpressionSyntax expression, TypeSymbol destination)
        {
            CheckSyntaxNode(expression);
            if ((object)destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            return GetMemberModel(expression)?.ClassifyConversionForCast(expression, destination) ?? Conversion.NoConversion;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, type, bindingOption, out speculativeModel);
            }
            Binder speculativeBinder = GetSpeculativeBinder(position, type, bindingOption);
            if (speculativeBinder != null)
            {
                speculativeModel = SpeculativeSyntaxTreeSemanticModel.Create(parentModel, type, speculativeBinder, position, bindingOption);
                return true;
            }
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, CrefSyntax crefSyntax, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder != null && enclosingBinder.InCref)
            {
                speculativeModel = SpeculativeSyntaxTreeSemanticModel.Create(parentModel, crefSyntax, enclosingBinder, position);
                return true;
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, statement, out speculativeModel);
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelForMethodBodyCore(parentModel, position, method, out speculativeModel);
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelForMethodBodyCore(parentModel, position, accessor, out speculativeModel);
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, initializer, out speculativeModel);
            }
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MemberSemanticModel memberModel = GetMemberModel(position);
            if (memberModel != null)
            {
                return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, expressionBody, out speculativeModel);
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            if (Root.FindToken(position).Parent!.AncestorsAndSelf().OfType<ConstructorInitializerSyntax>().FirstOrDefault() != null)
            {
                MemberSemanticModel memberModel = GetMemberModel(position);
                if (memberModel != null)
                {
                    return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, constructorInitializer, out speculativeModel);
                }
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax = Root.FindToken(position).Parent!.AncestorsAndSelf().OfType<PrimaryConstructorBaseTypeSyntax>().FirstOrDefault();
            if (primaryConstructorBaseTypeSyntax != null)
            {
                MemberSemanticModel memberModel = GetMemberModel(primaryConstructorBaseTypeSyntax);
                if (memberModel != null)
                {
                    return memberModel.TryGetSpeculativeSemanticModelCore(parentModel, position, constructorInitializer, out speculativeModel);
                }
            }
            speculativeModel = null;
            return false;
        }

        internal override BoundExpression GetSpeculativelyBoundExpression(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (bindingOption == SpeculativeBindingOption.BindAsExpression)
            {
                position = CheckAndAdjustPosition(position);
                MemberSemanticModel memberModel = GetMemberModel(position);
                if (memberModel != null)
                {
                    return memberModel.GetSpeculativelyBoundExpression(position, expression, bindingOption, out binder, out crefSymbols);
                }
            }
            return GetSpeculativelyBoundExpressionWithoutNullability(position, expression, bindingOption, out binder, out crefSymbols);
        }

        internal AttributeSemanticModel CreateSpeculativeAttributeSemanticModel(int position, AttributeSyntax attribute, Binder binder, AliasSymbol aliasOpt, NamedTypeSymbol attributeType)
        {
            return AttributeSemanticModel.CreateSpeculative(this, attribute, attributeType, aliasOpt, binder, (IsNullableAnalysisEnabledAtSpeculativePosition(position, attribute) ? GetMemberModel(position) : null)?.GetRemappedSymbols(), position);
        }

        internal bool IsNullableAnalysisEnabledAtSpeculativePosition(int position, SyntaxNode speculativeSyntax)
        {
            return ((CSharpSyntaxTree)speculativeSyntax.SyntaxTree).IsNullableAnalysisEnabled(speculativeSyntax.Span) ?? Compilation.IsNullableAnalysisEnabledIn((CSharpSyntaxTree)SyntaxTree, new TextSpan(position, 0));
        }

        private MemberSemanticModel GetMemberModel(int position)
        {
            CSharpSyntaxNode node = (CSharpSyntaxNode)Root.FindTokenIncludingCrefAndNameAttributes(position).Parent;
            CSharpSyntaxNode memberDeclaration = GetMemberDeclaration(node);
            bool flag = false;
            if (memberDeclaration != null)
            {
                switch (memberDeclaration.Kind())
                {
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.InitAccessorDeclaration:
                        flag = !LookupPosition.IsInBody(position, (AccessorDeclarationSyntax)memberDeclaration);
                        break;
                    case SyntaxKind.ConstructorDeclaration:
                        {
                            ConstructorDeclarationSyntax constructorDeclarationSyntax = (ConstructorDeclarationSyntax)memberDeclaration;
                            flag = !LookupPosition.IsInConstructorParameterScope(position, constructorDeclarationSyntax) && !LookupPosition.IsInParameterList(position, constructorDeclarationSyntax);
                            break;
                        }
                    case SyntaxKind.RecordDeclaration:
                        {
                            RecordDeclarationSyntax recordDeclarationSyntax = (RecordDeclarationSyntax)memberDeclaration;
                            if (recordDeclarationSyntax.ParameterList == null)
                            {
                                flag = true;
                                break;
                            }
                            ArgumentListSyntax argumentListSyntax = recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass?.ArgumentList;
                            flag = argumentListSyntax == null || !LookupPosition.IsBetweenTokens(position, argumentListSyntax.OpenParenToken, argumentListSyntax.CloseParenToken);
                            break;
                        }
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                        {
                            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax = (BaseMethodDeclarationSyntax)memberDeclaration;
                            flag = !LookupPosition.IsInBody(position, baseMethodDeclarationSyntax) && !LookupPosition.IsInParameterList(position, baseMethodDeclarationSyntax);
                            break;
                        }
                }
            }
            if (!flag)
            {
                return GetMemberModel(node);
            }
            return null;
        }

        internal override MemberSemanticModel GetMemberModel(SyntaxNode node)
        {
            if (IsInDocumentationComment(node))
            {
                return null;
            }
            CSharpSyntaxNode cSharpSyntaxNode = GetMemberDeclaration(node) ?? (node as CompilationUnitSyntax);
            if (cSharpSyntaxNode != null)
            {
                TextSpan span = node.Span;
                switch (cSharpSyntaxNode.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                    case SyntaxKind.ConversionOperatorDeclaration:
                        {
                            BaseMethodDeclarationSyntax baseMethodDeclarationSyntax = (BaseMethodDeclarationSyntax)cSharpSyntaxNode;
                            ArrowExpressionClauseSyntax? expressionBodySyntax2 = baseMethodDeclarationSyntax.GetExpressionBodySyntax();
                            if (expressionBodySyntax2 == null || !expressionBodySyntax2!.FullSpan.Contains(span))
                            {
                                BlockSyntax? body2 = baseMethodDeclarationSyntax.Body;
                                if (body2 == null || !body2!.FullSpan.Contains(span))
                                {
                                    return null;
                                }
                            }
                            return GetOrAddModel(baseMethodDeclarationSyntax);
                        }
                    case SyntaxKind.ConstructorDeclaration:
                        {
                            ConstructorDeclarationSyntax constructorDeclarationSyntax = (ConstructorDeclarationSyntax)cSharpSyntaxNode;
                            ArrowExpressionClauseSyntax expressionBodySyntax3 = constructorDeclarationSyntax.GetExpressionBodySyntax();
                            ConstructorInitializerSyntax? initializer = constructorDeclarationSyntax.Initializer;
                            if ((initializer == null || !initializer!.FullSpan.Contains(span)) && (expressionBodySyntax3 == null || !expressionBodySyntax3.FullSpan.Contains(span)))
                            {
                                BlockSyntax? body3 = constructorDeclarationSyntax.Body;
                                if (body3 == null || !body3!.FullSpan.Contains(span))
                                {
                                    return null;
                                }
                            }
                            return GetOrAddModel(constructorDeclarationSyntax);
                        }
                    case SyntaxKind.RecordDeclaration:
                        {
                            RecordDeclarationSyntax recordDeclarationSyntax = (RecordDeclarationSyntax)cSharpSyntaxNode;
                            if (recordDeclarationSyntax.ParameterList != null)
                            {
                                PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeIfClass = recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass;
                                if (primaryConstructorBaseTypeIfClass != null && (node == primaryConstructorBaseTypeIfClass || primaryConstructorBaseTypeIfClass.ArgumentList.FullSpan.Contains(span)))
                                {
                                    return GetOrAddModel(cSharpSyntaxNode);
                                }
                            }
                            return null;
                        }
                    case SyntaxKind.DestructorDeclaration:
                        {
                            DestructorDeclarationSyntax destructorDeclarationSyntax = (DestructorDeclarationSyntax)cSharpSyntaxNode;
                            ArrowExpressionClauseSyntax? expressionBodySyntax = destructorDeclarationSyntax.GetExpressionBodySyntax();
                            if (expressionBodySyntax == null || !expressionBodySyntax!.FullSpan.Contains(span))
                            {
                                BlockSyntax? body = destructorDeclarationSyntax.Body;
                                if (body == null || !body!.FullSpan.Contains(span))
                                {
                                    return null;
                                }
                            }
                            return GetOrAddModel(destructorDeclarationSyntax);
                        }
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.InitAccessorDeclaration:
                        {
                            AccessorDeclarationSyntax accessorDeclarationSyntax = (AccessorDeclarationSyntax)cSharpSyntaxNode;
                            ArrowExpressionClauseSyntax? expressionBody = accessorDeclarationSyntax.ExpressionBody;
                            if (expressionBody == null || !expressionBody!.FullSpan.Contains(span))
                            {
                                BlockSyntax? body4 = accessorDeclarationSyntax.Body;
                                if (body4 == null || !body4!.FullSpan.Contains(span))
                                {
                                    return null;
                                }
                            }
                            return GetOrAddModel(accessorDeclarationSyntax);
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            IndexerDeclarationSyntax indexerDeclarationSyntax = (IndexerDeclarationSyntax)cSharpSyntaxNode;
                            return GetOrAddModelIfContains(indexerDeclarationSyntax.ExpressionBody, span);
                        }
                    case SyntaxKind.FieldDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                        {
                            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = ((BaseFieldDeclarationSyntax)cSharpSyntaxNode).Declaration.Variables.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                VariableDeclaratorSyntax current = enumerator.Current;
                                MemberSemanticModel orAddModelIfContains = GetOrAddModelIfContains(current.Initializer, span);
                                if (orAddModelIfContains != null)
                                {
                                    return orAddModelIfContains;
                                }
                            }
                            break;
                        }
                    case SyntaxKind.EnumMemberDeclaration:
                        {
                            EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = (EnumMemberDeclarationSyntax)cSharpSyntaxNode;
                            if (enumMemberDeclarationSyntax.EqualsValue == null)
                            {
                                return null;
                            }
                            return GetOrAddModelIfContains(enumMemberDeclarationSyntax.EqualsValue, span);
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)cSharpSyntaxNode;
                            return GetOrAddModelIfContains(propertyDeclarationSyntax.Initializer, span) ?? GetOrAddModelIfContains(propertyDeclarationSyntax.ExpressionBody, span);
                        }
                    case SyntaxKind.GlobalStatement:
                        if (SyntaxFacts.IsSimpleProgramTopLevelStatement((GlobalStatementSyntax)cSharpSyntaxNode))
                        {
                            return GetOrAddModel((CompilationUnitSyntax)cSharpSyntaxNode.Parent);
                        }
                        return GetOrAddModel(cSharpSyntaxNode);
                    case SyntaxKind.CompilationUnit:
                        if ((object)SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(Compilation, (CompilationUnitSyntax)cSharpSyntaxNode, fallbackToMainEntryPoint: false) != null)
                        {
                            return GetOrAddModel(cSharpSyntaxNode);
                        }
                        break;
                    case SyntaxKind.Attribute:
                        return GetOrAddModelForAttribute((AttributeSyntax)cSharpSyntaxNode);
                    case SyntaxKind.Parameter:
                        if (node != cSharpSyntaxNode)
                        {
                            return GetOrAddModelForParameter((ParameterSyntax)cSharpSyntaxNode, span);
                        }
                        return GetMemberModel(cSharpSyntaxNode.Parent);
                }
            }
            return null;
        }

        private MemberSemanticModel GetOrAddModelForAttribute(AttributeSyntax attribute)
        {
            MemberSemanticModel memberSemanticModel = ((attribute.Parent != null) ? GetMemberModel(attribute.Parent) : null);
            if (memberSemanticModel == null)
            {
                return GetOrAddModel(attribute);
            }
            return ImmutableInterlocked.GetOrAdd(ref _memberModels, attribute, (CSharpSyntaxNode node, (Binder binder, MemberSemanticModel model) binderAndModel) => CreateModelForAttribute(binderAndModel.binder, (AttributeSyntax)node, binderAndModel.model), (memberSemanticModel.GetEnclosingBinder(attribute.SpanStart), memberSemanticModel));
        }

        private static bool IsInDocumentationComment(SyntaxNode node)
        {
            for (SyntaxNode syntaxNode = node; syntaxNode != null; syntaxNode = syntaxNode.Parent)
            {
                if (SyntaxFacts.IsDocumentationCommentTrivia(syntaxNode.Kind()))
                {
                    return true;
                }
            }
            return false;
        }

        private MemberSemanticModel GetOrAddModelForParameter(ParameterSyntax paramDecl, TextSpan span)
        {
            EqualsValueClauseSyntax @default = paramDecl.Default;
            MemberSemanticModel memberSemanticModel = ((paramDecl.Parent != null) ? GetMemberModel(paramDecl.Parent) : null);
            if (memberSemanticModel == null)
            {
                return GetOrAddModelIfContains(@default, span);
            }
            if (@default != null && @default.FullSpan.Contains(span))
            {
                ParameterSymbol symbol = memberSemanticModel.GetDeclaredSymbol(paramDecl).GetSymbol<ParameterSymbol>();
                if ((object)symbol != null)
                {
                    return ImmutableInterlocked.GetOrAdd(ref _memberModels, @default, (CSharpSyntaxNode equalsValue, (CSharpCompilation compilation, ParameterSyntax paramDecl, ParameterSymbol parameterSymbol, MemberSemanticModel containing) tuple) => InitializerSemanticModel.Create(this, tuple.paramDecl, tuple.parameterSymbol, tuple.containing.GetEnclosingBinder(tuple.paramDecl.SpanStart).CreateBinderForParameterDefaultValue(tuple.parameterSymbol, (EqualsValueClauseSyntax)equalsValue), tuple.containing.GetRemappedSymbols()), (Compilation, paramDecl, symbol, memberSemanticModel));
                }
            }
            return memberSemanticModel;
        }

        private static CSharpSyntaxNode GetMemberDeclaration(SyntaxNode node)
        {
            return node.FirstAncestorOrSelf(s_isMemberDeclarationFunction);
        }

        private MemberSemanticModel GetOrAddModelIfContains(CSharpSyntaxNode node, TextSpan span)
        {
            if (node != null && node.FullSpan.Contains(span))
            {
                return GetOrAddModel(node);
            }
            return null;
        }

        private MemberSemanticModel GetOrAddModel(CSharpSyntaxNode node)
        {
            Func<CSharpSyntaxNode, MemberSemanticModel> createMemberModelFunction = CreateMemberModel;
            return GetOrAddModel(node, createMemberModelFunction);
        }

        internal MemberSemanticModel GetOrAddModel(CSharpSyntaxNode node, Func<CSharpSyntaxNode, MemberSemanticModel> createMemberModelFunction)
        {
            return ImmutableInterlocked.GetOrAdd(ref _memberModels, node, createMemberModelFunction);
        }

        private MemberSemanticModel CreateMemberModel(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    return createMethodBodySemanticModel(node, SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(Compilation, (CompilationUnitSyntax)node, fallbackToMainEntryPoint: false));
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                    {
                        MemberDeclarationSyntax memberDeclarationSyntax = (MemberDeclarationSyntax)node;
                        SourceMemberMethodSymbol symbol4 = GetDeclaredSymbol(memberDeclarationSyntax).GetSymbol<SourceMemberMethodSymbol>();
                        return createMethodBodySemanticModel(memberDeclarationSyntax, symbol4);
                    }
                case SyntaxKind.RecordDeclaration:
                    {
                        SynthesizedRecordConstructor synthesizedRecordConstructor = TryGetSynthesizedRecordConstructor((RecordDeclarationSyntax)node);
                        if ((object)synthesizedRecordConstructor == null)
                        {
                            return null;
                        }
                        return createMethodBodySemanticModel(node, synthesizedRecordConstructor);
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                    {
                        AccessorDeclarationSyntax accessorDeclarationSyntax = (AccessorDeclarationSyntax)node;
                        SourceMemberMethodSymbol symbol5 = GetDeclaredSymbol(accessorDeclarationSyntax).GetSymbol<SourceMemberMethodSymbol>();
                        return createMethodBodySemanticModel(accessorDeclarationSyntax, symbol5);
                    }
                case SyntaxKind.Block:
                    ExceptionUtilities.UnexpectedValue(node.Parent);
                    break;
                case SyntaxKind.EqualsValueClause:
                    switch (node.Parent!.Kind())
                    {
                        case SyntaxKind.VariableDeclarator:
                            {
                                VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)node.Parent;
                                FieldSymbol declaredFieldSymbol = GetDeclaredFieldSymbol(variableDeclaratorSyntax);
                                return InitializerSemanticModel.Create(this, variableDeclaratorSyntax, declaredFieldSymbol, GetFieldOrPropertyInitializerBinder(declaredFieldSymbol, defaultOuter(), variableDeclaratorSyntax.Initializer));
                            }
                        case SyntaxKind.PropertyDeclaration:
                            {
                                PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)node.Parent;
                                SourcePropertySymbol symbol3 = GetDeclaredSymbol(propertyDeclarationSyntax).GetSymbol<SourcePropertySymbol>();
                                return InitializerSemanticModel.Create(this, propertyDeclarationSyntax, symbol3, GetFieldOrPropertyInitializerBinder(symbol3.BackingField, defaultOuter(), propertyDeclarationSyntax.Initializer));
                            }
                        case SyntaxKind.Parameter:
                            {
                                ParameterSyntax parameterSyntax = (ParameterSyntax)node.Parent;
                                ParameterSymbol declaredNonLambdaParameterSymbol = GetDeclaredNonLambdaParameterSymbol(parameterSyntax);
                                if ((object)declaredNonLambdaParameterSymbol == null)
                                {
                                    return null;
                                }
                                return InitializerSemanticModel.Create(this, parameterSyntax, declaredNonLambdaParameterSymbol, defaultOuter().CreateBinderForParameterDefaultValue(declaredNonLambdaParameterSymbol, (EqualsValueClauseSyntax)node), null);
                            }
                        case SyntaxKind.EnumMemberDeclaration:
                            {
                                EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = (EnumMemberDeclarationSyntax)node.Parent;
                                FieldSymbol symbol2 = GetDeclaredSymbol(enumMemberDeclarationSyntax).GetSymbol<FieldSymbol>();
                                if ((object)symbol2 == null)
                                {
                                    return null;
                                }
                                return InitializerSemanticModel.Create(this, enumMemberDeclarationSyntax, symbol2, GetFieldOrPropertyInitializerBinder(symbol2, defaultOuter(), enumMemberDeclarationSyntax.EqualsValue));
                            }
                        default:
                            throw ExceptionUtilities.UnexpectedValue(node.Parent!.Kind());
                    }
                case SyntaxKind.ArrowExpressionClause:
                    {
                        SourceMemberMethodSymbol sourceMemberMethodSymbol = null;
                        ArrowExpressionClauseSyntax arrowExpressionClauseSyntax = (ArrowExpressionClauseSyntax)node;
                        if (node.Parent is BasePropertyDeclarationSyntax)
                        {
                            sourceMemberMethodSymbol = GetDeclaredSymbol(arrowExpressionClauseSyntax).GetSymbol<SourceMemberMethodSymbol>();
                        }
                        else
                        {
                            ExceptionUtilities.UnexpectedValue(node.Parent);
                        }
                        ExecutableCodeBinder executableCodeBinder = sourceMemberMethodSymbol?.TryGetBodyBinder(_binderFactory, IgnoresAccessibility);
                        if (executableCodeBinder == null)
                        {
                            return null;
                        }
                        return MethodBodySemanticModel.Create(this, sourceMemberMethodSymbol, new MethodBodySemanticModel.InitialState(arrowExpressionClauseSyntax, null, executableCodeBinder));
                    }
                case SyntaxKind.GlobalStatement:
                    {
                        CSharpSyntaxNode parent = node.Parent;
                        if (parent.Kind() == SyntaxKind.CompilationUnit && !IsRegularCSharp && (object)_compilation.ScriptClass != null)
                        {
                            SynthesizedInteractiveInitializerMethod scriptInitializer = _compilation.ScriptClass!.GetScriptInitializer();
                            if ((object)scriptInitializer == null)
                            {
                                return null;
                            }
                            if (_globalStatementLabels == null)
                            {
                                Interlocked.CompareExchange(ref _globalStatementLabels, new ScriptLocalScopeBinder.Labels(scriptInitializer, (CompilationUnitSyntax)parent), null);
                            }
                            return MethodBodySemanticModel.Create(this, scriptInitializer, new MethodBodySemanticModel.InitialState(node, null, new ExecutableCodeBinder(node, scriptInitializer, new ScriptLocalScopeBinder(_globalStatementLabels, defaultOuter()))));
                        }
                        break;
                    }
                case SyntaxKind.Attribute:
                    return CreateModelForAttribute(defaultOuter(), (AttributeSyntax)node, null);
            }
            return null;
            MemberSemanticModel createMethodBodySemanticModel(CSharpSyntaxNode memberDecl, SourceMemberMethodSymbol symbol)
            {
                ExecutableCodeBinder executableCodeBinder2 = symbol?.TryGetBodyBinder(_binderFactory, IgnoresAccessibility);
                if (executableCodeBinder2 == null)
                {
                    return null;
                }
                return MethodBodySemanticModel.Create(this, symbol, new MethodBodySemanticModel.InitialState(memberDecl, null, executableCodeBinder2));
            }
            Binder defaultOuter()
            {
                return _binderFactory.GetBinder(node).WithAdditionalFlags(IgnoresAccessibility ? BinderFlags.IgnoreAccessibility : BinderFlags.None);
            }
        }

        private SynthesizedRecordConstructor TryGetSynthesizedRecordConstructor(RecordDeclarationSyntax node)
        {
            SynthesizedRecordConstructor synthesizedRecordConstructor = GetDeclaredType(node).GetMembersUnordered().OfType<SynthesizedRecordConstructor>().SingleOrDefault();
            if (synthesizedRecordConstructor?.SyntaxRef.SyntaxTree != node.SyntaxTree || synthesizedRecordConstructor.GetSyntax() != node)
            {
                return null;
            }
            return synthesizedRecordConstructor;
        }

        private AttributeSemanticModel CreateModelForAttribute(Binder enclosingBinder, AttributeSyntax attribute, MemberSemanticModel containingModel)
        {
            NamedTypeSymbol attributeType = (NamedTypeSymbol)enclosingBinder.BindType(attribute.Name, BindingDiagnosticBag.Discarded, out AliasSymbol alias).Type;
            return AttributeSemanticModel.Create(this, attribute, attributeType, alias, enclosingBinder.WithAdditionalFlags(BinderFlags.AttributeArgument), containingModel?.GetRemappedSymbols());
        }

        private FieldSymbol GetDeclaredFieldSymbol(VariableDeclaratorSyntax variableDecl)
        {
            ISymbol declaredSymbol = GetDeclaredSymbol(variableDecl);
            if (declaredSymbol != null)
            {
                switch (variableDecl.Parent!.Parent!.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        return declaredSymbol.GetSymbol<FieldSymbol>();
                    case SyntaxKind.EventFieldDeclaration:
                        return declaredSymbol.GetSymbol<EventSymbol>()!.AssociatedField;
                }
            }
            return null;
        }

        private Binder GetFieldOrPropertyInitializerBinder(FieldSymbol symbol, Binder outer, EqualsValueClauseSyntax initializer)
        {
            outer = outer.GetFieldInitializerBinder(symbol, !IsRegularCSharp && symbol.ContainingType.IsScriptClass);
            if (initializer != null)
            {
                outer = new ExecutableCodeBinder(initializer, symbol, outer);
            }
            return outer;
        }

        private static bool IsMemberDeclaration(CSharpSyntaxNode node)
        {
            if (!(node is MemberDeclarationSyntax) && !(node is AccessorDeclarationSyntax) && node.Kind() != SyntaxKind.Attribute)
            {
                return node.Kind() == SyntaxKind.Parameter;
            }
            return true;
        }

        public override INamespaceSymbol GetDeclaredSymbol(NamespaceDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredNamespace(declarationSyntax).GetPublicSymbol();
        }

        private NamespaceSymbol GetDeclaredNamespace(NamespaceDeclarationSyntax declarationSyntax)
        {
            NamespaceOrTypeSymbol container = ((declarationSyntax.Parent!.Kind() != SyntaxKind.CompilationUnit) ? GetDeclaredNamespaceOrType(declarationSyntax.Parent) : _compilation.Assembly.GlobalNamespace);
            NamespaceSymbol namespaceSymbol = (NamespaceSymbol)GetDeclaredMember(container, declarationSyntax.Span, declarationSyntax.Name);
            return _compilation.GetCompilationNamespace(namespaceSymbol);
        }

        public override INamedTypeSymbol GetDeclaredSymbol(BaseTypeDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredType(declarationSyntax).GetPublicSymbol();
        }

        public override INamedTypeSymbol GetDeclaredSymbol(DelegateDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredType(declarationSyntax).GetPublicSymbol();
        }

        private NamedTypeSymbol GetDeclaredType(BaseTypeDeclarationSyntax declarationSyntax)
        {
            string valueText = declarationSyntax.Identifier.ValueText;
            return GetDeclaredNamedType(declarationSyntax, valueText);
        }

        private NamedTypeSymbol GetDeclaredType(DelegateDeclarationSyntax declarationSyntax)
        {
            string valueText = declarationSyntax.Identifier.ValueText;
            return GetDeclaredNamedType(declarationSyntax, valueText);
        }

        private NamedTypeSymbol GetDeclaredNamedType(CSharpSyntaxNode declarationSyntax, string name)
        {
            NamespaceOrTypeSymbol declaredTypeMemberContainer = GetDeclaredTypeMemberContainer(declarationSyntax);
            return GetDeclaredMember(declaredTypeMemberContainer, declarationSyntax.Span, name) as NamedTypeSymbol;
        }

        private NamespaceOrTypeSymbol GetDeclaredNamespaceOrType(CSharpSyntaxNode declarationSyntax)
        {
            if (declarationSyntax is NamespaceDeclarationSyntax declarationSyntax2)
            {
                return GetDeclaredNamespace(declarationSyntax2);
            }
            if (declarationSyntax is BaseTypeDeclarationSyntax declarationSyntax3)
            {
                return GetDeclaredType(declarationSyntax3);
            }
            if (declarationSyntax is DelegateDeclarationSyntax declarationSyntax4)
            {
                return GetDeclaredType(declarationSyntax4);
            }
            return null;
        }

        public override ISymbol GetDeclaredSymbol(MemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            switch (declarationSyntax.Kind())
            {
                case SyntaxKind.GlobalStatement:
                    return null;
                case SyntaxKind.IncompleteMember:
                    return null;
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return null;
                default:
                    return (GetDeclaredNamespaceOrType(declarationSyntax) ?? GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
            }
        }

        public override IMethodSymbol GetDeclaredSymbol(CompilationUnitSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(Compilation, declarationSyntax, fallbackToMainEntryPoint: false).GetPublicSymbol();
        }

        public override ISymbol GetDeclaredSymbol(LocalFunctionStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetMemberModel(declarationSyntax)?.GetDeclaredSymbol(declarationSyntax, cancellationToken);
        }

        public override IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((FieldSymbol)GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
        }

        public override IMethodSymbol GetDeclaredSymbol(BaseMethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((MethodSymbol)GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
        }

        public override ISymbol GetDeclaredSymbol(BasePropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDeclaredMemberSymbol(declarationSyntax).GetPublicSymbol();
        }

        public override IPropertySymbol GetDeclaredSymbol(PropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((PropertySymbol)GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
        }

        public override IPropertySymbol GetDeclaredSymbol(IndexerDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((PropertySymbol)GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
        }

        public override IEventSymbol GetDeclaredSymbol(EventDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ((EventSymbol)GetDeclaredMemberSymbol(declarationSyntax)).GetPublicSymbol();
        }

        public override IMethodSymbol GetDeclaredSymbol(AccessorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            if (declarationSyntax.Kind() == SyntaxKind.UnknownAccessorDeclaration)
            {
                return null;
            }
            CSharpSyntaxNode parent = declarationSyntax.Parent!.Parent;
            SyntaxKind syntaxKind = parent.Kind();
            if (syntaxKind == SyntaxKind.EventFieldDeclaration || syntaxKind - 8892 <= (SyntaxKind)2)
            {
                NamespaceOrTypeSymbol declaredTypeMemberContainer = GetDeclaredTypeMemberContainer(parent);
                return (GetDeclaredMember(declaredTypeMemberContainer, declarationSyntax.Span) as MethodSymbol).GetPublicSymbol();
            }
            throw ExceptionUtilities.UnexpectedValue(parent.Kind());
        }

        public override IMethodSymbol GetDeclaredSymbol(ArrowExpressionClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            CSharpSyntaxNode parent = declarationSyntax.Parent;
            SyntaxKind syntaxKind = parent.Kind();
            if (syntaxKind == SyntaxKind.PropertyDeclaration || syntaxKind == SyntaxKind.IndexerDeclaration)
            {
                NamespaceOrTypeSymbol declaredTypeMemberContainer = GetDeclaredTypeMemberContainer(parent);
                return (GetDeclaredMember(declaredTypeMemberContainer, declarationSyntax.Span) as MethodSymbol).GetPublicSymbol();
            }
            ExceptionUtilities.UnexpectedValue(parent.Kind());
            return null;
        }

        private string GetDeclarationName(CSharpSyntaxNode declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)declaration;
                        return GetDeclarationName(declaration, methodDeclarationSyntax.ExplicitInterfaceSpecifier, methodDeclarationSyntax.Identifier.ValueText);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)declaration;
                        return GetDeclarationName(declaration, propertyDeclarationSyntax.ExplicitInterfaceSpecifier, propertyDeclarationSyntax.Identifier.ValueText);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        IndexerDeclarationSyntax indexerDeclarationSyntax = (IndexerDeclarationSyntax)declaration;
                        return GetDeclarationName(declaration, indexerDeclarationSyntax.ExplicitInterfaceSpecifier, "this[]");
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        EventDeclarationSyntax eventDeclarationSyntax = (EventDeclarationSyntax)declaration;
                        return GetDeclarationName(declaration, eventDeclarationSyntax.ExplicitInterfaceSpecifier, eventDeclarationSyntax.Identifier.ValueText);
                    }
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)declaration).Identifier.ValueText;
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.RecordDeclaration:
                    return ((BaseTypeDeclarationSyntax)declaration).Identifier.ValueText;
                case SyntaxKind.VariableDeclarator:
                    return ((VariableDeclaratorSyntax)declaration).Identifier.ValueText;
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)declaration).Identifier.ValueText;
                case SyntaxKind.DestructorDeclaration:
                    return "Finalize";
                case SyntaxKind.ConstructorDeclaration:
                    if (((ConstructorDeclarationSyntax)declaration).Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return ".cctor";
                    }
                    return ".ctor";
                case SyntaxKind.OperatorDeclaration:
                    return OperatorFacts.OperatorNameFromDeclaration((OperatorDeclarationSyntax)declaration);
                case SyntaxKind.ConversionOperatorDeclaration:
                    if (((ConversionOperatorDeclarationSyntax)declaration).ImplicitOrExplicitKeyword.Kind() == SyntaxKind.ExplicitKeyword)
                    {
                        return "op_Explicit";
                    }
                    return "op_Implicit";
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    throw new ArgumentException(CSharpResources.InvalidGetDeclarationNameMultipleDeclarators);
                case SyntaxKind.IncompleteMember:
                    return null;
                default:
                    throw ExceptionUtilities.UnexpectedValue(declaration.Kind());
            }
        }

        private string GetDeclarationName(CSharpSyntaxNode declaration, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierOpt, string memberName)
        {
            if (explicitInterfaceSpecifierOpt == null)
            {
                return memberName;
            }
            return ExplicitInterfaceHelpers.GetMemberName(_binderFactory.GetBinder(declaration), explicitInterfaceSpecifierOpt, memberName);
        }

        private Symbol GetDeclaredMember(NamespaceOrTypeSymbol container, TextSpan declarationSpan, NameSyntax name)
        {
            switch (name.Kind())
            {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                    return GetDeclaredMember(container, declarationSpan, ((SimpleNameSyntax)name).Identifier.ValueText);
                case SyntaxKind.QualifiedName:
                    {
                        QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)name;
                        NamespaceOrTypeSymbol container2 = GetDeclaredMember(container, declarationSpan, qualifiedNameSyntax.Left) as NamespaceOrTypeSymbol;
                        return GetDeclaredMember(container2, declarationSpan, qualifiedNameSyntax.Right);
                    }
                case SyntaxKind.AliasQualifiedName:
                    {
                        AliasQualifiedNameSyntax aliasQualifiedNameSyntax = (AliasQualifiedNameSyntax)name;
                        return GetDeclaredMember(container, declarationSpan, aliasQualifiedNameSyntax.Name);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(name.Kind());
            }
        }

        private Symbol GetDeclaredMember(NamespaceOrTypeSymbol container, TextSpan declarationSpan, string name = null)
        {
            if ((object)container == null)
            {
                return null;
            }
            ImmutableArray<Symbol> immutableArray = ((name != null) ? container.GetMembers(name) : container.GetMembersUnordered());
            Symbol symbol = null;
            ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current is ImplicitNamedTypeSymbol implicitNamedTypeSymbol && implicitNamedTypeSymbol.IsImplicitClass)
                {
                    Symbol declaredMember = GetDeclaredMember(implicitNamedTypeSymbol, declarationSpan, name);
                    if ((object)declaredMember != null)
                    {
                        return declaredMember;
                    }
                }
                ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Location current2 = enumerator2.Current;
                    if (current2.IsInSource && current2.SourceTree == SyntaxTree && declarationSpan.Contains(current2.SourceSpan))
                    {
                        if (!current2.SourceSpan.IsEmpty || current2.SourceSpan.End != declarationSpan.Start)
                        {
                            return current;
                        }
                        symbol = current;
                    }
                }
                MethodSymbol methodSymbol = ((current.Kind == SymbolKind.Method) ? ((MethodSymbol)current).PartialImplementationPart : null);
                if ((object)methodSymbol != null)
                {
                    Location location = methodSymbol.Locations[0];
                    if (location.IsInSource && location.SourceTree == SyntaxTree && declarationSpan.Contains(location.SourceSpan))
                    {
                        return methodSymbol;
                    }
                }
            }
            Symbol symbol2 = symbol;
            if ((object)symbol2 == null)
            {
                if (name == null)
                {
                    return null;
                }
                symbol2 = GetDeclaredMember(container, declarationSpan);
            }
            return symbol2;
        }

        public override ISymbol GetDeclaredSymbol(VariableDeclaratorSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            BaseFieldDeclarationSyntax baseFieldDeclarationSyntax = ((declarationSyntax.Parent == null) ? null : (declarationSyntax.Parent!.Parent as BaseFieldDeclarationSyntax));
            if (baseFieldDeclarationSyntax != null)
            {
                NamespaceOrTypeSymbol declaredTypeMemberContainer = GetDeclaredTypeMemberContainer(baseFieldDeclarationSyntax);
                return GetDeclaredMember(declaredTypeMemberContainer, declarationSyntax.Span, declarationSyntax.Identifier.ValueText).GetPublicSymbol();
            }
            return GetMemberModel(declarationSyntax)?.GetDeclaredSymbol(declarationSyntax, cancellationToken);
        }

        public override ISymbol GetDeclaredSymbol(SingleVariableDesignationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = GetMemberModel(declarationSyntax)?.GetDeclaredSymbol(declarationSyntax, cancellationToken);
            if (symbol != null)
            {
                return symbol;
            }
            return GetEnclosingBinder(declarationSyntax.Position)?.LookupDeclaredField(declarationSyntax).GetPublicSymbol();
        }

        internal override LocalSymbol GetAdjustedLocalSymbol(SourceLocalSymbol originalSymbol)
        {
            int spanStart = originalSymbol.IdentifierToken.SpanStart;
            return GetMemberModel(spanStart)?.GetAdjustedLocalSymbol(originalSymbol) ?? originalSymbol;
        }

        public override ILabelSymbol GetDeclaredSymbol(LabeledStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetMemberModel(declarationSyntax)?.GetDeclaredSymbol(declarationSyntax, cancellationToken);
        }

        public override ILabelSymbol GetDeclaredSymbol(SwitchLabelSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetMemberModel(declarationSyntax)?.GetDeclaredSymbol(declarationSyntax, cancellationToken);
        }

        public override IAliasSymbol GetDeclaredSymbol(UsingDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            if (declarationSyntax.Alias == null)
            {
                return null;
            }
            for (Binder binder = _binderFactory.GetInNamespaceBinder(declarationSyntax.Parent); binder != null; binder = binder.Next)
            {
                ImmutableArray<AliasAndUsingDirective> usingAliases = binder.UsingAliases;
                if (!usingAliases.IsDefault)
                {
                    ImmutableArray<AliasAndUsingDirective>.Enumerator enumerator = usingAliases.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        AliasAndUsingDirective current = enumerator.Current;
                        if (current.Alias.Locations[0].SourceSpan == declarationSyntax.Alias!.Name.Span)
                        {
                            return current.Alias.GetPublicSymbol();
                        }
                    }
                    break;
                }
            }
            return null;
        }

        public override IAliasSymbol GetDeclaredSymbol(ExternAliasDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            for (Binder binder = _binderFactory.GetInNamespaceBinder(declarationSyntax.Parent); binder != null; binder = binder.Next)
            {
                ImmutableArray<AliasAndExternAliasDirective> externAliases = binder.ExternAliases;
                if (!externAliases.IsDefault)
                {
                    ImmutableArray<AliasAndExternAliasDirective>.Enumerator enumerator = externAliases.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        AliasAndExternAliasDirective current = enumerator.Current;
                        if (current.Alias.Locations[0].SourceSpan == declarationSyntax.Identifier.Span)
                        {
                            return current.Alias.GetPublicSymbol();
                        }
                    }
                    break;
                }
            }
            return null;
        }

        internal override ImmutableArray<ISymbol> GetDeclaredSymbols(BaseFieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            ArrayBuilder<ISymbol> arrayBuilder = new ArrayBuilder<ISymbol>();
            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = declarationSyntax.Declaration.Variables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VariableDeclaratorSyntax current = enumerator.Current;
                ISymbol declaredSymbol = GetDeclaredSymbol(current, cancellationToken);
                if (declaredSymbol != null)
                {
                    arrayBuilder.Add(declaredSymbol);
                }
            }
            return arrayBuilder.ToImmutableAndFree();
        }

        private ParameterSymbol GetMethodParameterSymbol(ParameterSyntax parameter, CancellationToken cancellationToken)
        {
            if (!(parameter.Parent is ParameterListSyntax parameterListSyntax))
            {
                return null;
            }
            if (!(parameterListSyntax.Parent is MemberDeclarationSyntax memberDeclarationSyntax))
            {
                return null;
            }
            MethodSymbol methodSymbol = ((!(memberDeclarationSyntax is RecordDeclarationSyntax recordDeclarationSyntax) || recordDeclarationSyntax.ParameterList != parameterListSyntax) ? (GetDeclaredSymbol(memberDeclarationSyntax, cancellationToken) as IMethodSymbol).GetSymbol() : TryGetSynthesizedRecordConstructor(recordDeclarationSyntax));
            if ((object)methodSymbol == null)
            {
                return null;
            }
            object obj = GetParameterSymbol(methodSymbol.Parameters, parameter, cancellationToken);
            if (obj == null)
            {
                if ((object)methodSymbol.PartialDefinitionPart != null)
                {
                    return GetParameterSymbol(methodSymbol.PartialDefinitionPart.Parameters, parameter, cancellationToken);
                }
                obj = null;
            }
            return (ParameterSymbol)obj;
        }

        private ParameterSymbol GetIndexerParameterSymbol(ParameterSyntax parameter, CancellationToken cancellationToken)
        {
            if (!(parameter.Parent is BracketedParameterListSyntax bracketedParameterListSyntax))
            {
                return null;
            }
            if (!(bracketedParameterListSyntax.Parent is MemberDeclarationSyntax declarationSyntax))
            {
                return null;
            }
            PropertySymbol symbol = (GetDeclaredSymbol(declarationSyntax, cancellationToken) as IPropertySymbol).GetSymbol();
            if ((object)symbol == null)
            {
                return null;
            }
            return GetParameterSymbol(symbol.Parameters, parameter, cancellationToken);
        }

        private ParameterSymbol GetDelegateParameterSymbol(ParameterSyntax parameter, CancellationToken cancellationToken)
        {
            if (!(parameter.Parent is ParameterListSyntax parameterListSyntax))
            {
                return null;
            }
            if (!(parameterListSyntax.Parent is DelegateDeclarationSyntax declarationSyntax))
            {
                return null;
            }
            NamedTypeSymbol symbol = GetDeclaredSymbol(declarationSyntax, cancellationToken).GetSymbol();
            if ((object)symbol == null)
            {
                return null;
            }
            MethodSymbol delegateInvokeMethod = symbol.DelegateInvokeMethod;
            if ((object)delegateInvokeMethod == null || delegateInvokeMethod.HasUseSiteError)
            {
                return null;
            }
            return GetParameterSymbol(delegateInvokeMethod.Parameters, parameter, cancellationToken);
        }

        public override IParameterSymbol GetDeclaredSymbol(ParameterSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            MemberSemanticModel memberModel = GetMemberModel(declarationSyntax);
            if (memberModel != null)
            {
                return memberModel.GetDeclaredSymbol(declarationSyntax, cancellationToken);
            }
            return GetDeclaredNonLambdaParameterSymbol(declarationSyntax, cancellationToken).GetPublicSymbol();
        }

        private ParameterSymbol GetDeclaredNonLambdaParameterSymbol(ParameterSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetMethodParameterSymbol(declarationSyntax, cancellationToken) ?? GetIndexerParameterSymbol(declarationSyntax, cancellationToken) ?? GetDelegateParameterSymbol(declarationSyntax, cancellationToken);
        }

        public override ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (typeParameter == null)
            {
                throw new ArgumentNullException("typeParameter");
            }
            if (!IsInTree(typeParameter))
            {
                throw new ArgumentException("typeParameter not within tree");
            }
            if (typeParameter.Parent is TypeParameterListSyntax typeParameterListSyntax)
            {
                ISymbol symbol = null;
                CSharpSyntaxNode parent = typeParameterListSyntax.Parent;
                if (!(parent is MemberDeclarationSyntax declarationSyntax))
                {
                    if (!(parent is LocalFunctionStatementSyntax declarationSyntax2))
                    {
                        throw ExceptionUtilities.UnexpectedValue(typeParameter.Parent!.Kind());
                    }
                    symbol = GetDeclaredSymbol(declarationSyntax2, cancellationToken);
                }
                else
                {
                    symbol = GetDeclaredSymbol(declarationSyntax, cancellationToken);
                }
                Symbol symbol2 = symbol.GetSymbol();
                if (symbol2 is NamedTypeSymbol namedTypeSymbol)
                {
                    return GetTypeParameterSymbol(namedTypeSymbol.TypeParameters, typeParameter).GetPublicSymbol();
                }
                if (symbol2 is MethodSymbol methodSymbol)
                {
                    return (GetTypeParameterSymbol(methodSymbol.TypeParameters, typeParameter) ?? (((object)methodSymbol.PartialDefinitionPart == null) ? null : GetTypeParameterSymbol(methodSymbol.PartialDefinitionPart.TypeParameters, typeParameter))).GetPublicSymbol();
                }
            }
            return null;
        }

        private TypeParameterSymbol GetTypeParameterSymbol(ImmutableArray<TypeParameterSymbol> parameters, TypeParameterSyntax parameter)
        {
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Location current2 = enumerator2.Current;
                    if (current2.SourceTree == SyntaxTree && parameter.Span.Contains(current2.SourceSpan))
                    {
                        return current;
                    }
                }
            }
            return null;
        }

        public override ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            ValidateStatementRange(firstStatement, lastStatement);
            return new CSharpControlFlowAnalysis(RegionAnalysisContext(firstStatement, lastStatement));
        }

        private void ValidateStatementRange(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            if (firstStatement == null)
            {
                throw new ArgumentNullException("firstStatement");
            }
            if (lastStatement == null)
            {
                throw new ArgumentNullException("lastStatement");
            }
            if (!IsInTree(firstStatement))
            {
                throw new ArgumentException("statements not within tree");
            }
            if (firstStatement.Parent == null || firstStatement.Parent != lastStatement.Parent)
            {
                throw new ArgumentException("statements not within the same statement list");
            }
            if (firstStatement.SpanStart > lastStatement.SpanStart)
            {
                throw new ArgumentException("first statement does not precede last statement");
            }
        }

        public override DataFlowAnalysis AnalyzeDataFlow(ExpressionSyntax expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!IsInTree(expression))
            {
                throw new ArgumentException("expression not within tree");
            }
            return new CSharpDataFlowAnalysis(RegionAnalysisContext(expression));
        }

        public override DataFlowAnalysis AnalyzeDataFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            ValidateStatementRange(firstStatement, lastStatement);
            return new CSharpDataFlowAnalysis(RegionAnalysisContext(firstStatement, lastStatement));
        }

        private static BoundNode GetBoundRoot(MemberSemanticModel memberModel, out Symbol member)
        {
            member = memberModel.MemberSymbol;
            return memberModel.GetBoundRoot();
        }

        private NamespaceOrTypeSymbol GetDeclaredTypeMemberContainer(CSharpSyntaxNode memberDeclaration)
        {
            if (memberDeclaration.Parent!.Kind() == SyntaxKind.CompilationUnit)
            {
                if (memberDeclaration.Kind() == SyntaxKind.NamespaceDeclaration)
                {
                    return _compilation.Assembly.GlobalNamespace;
                }
                if (SyntaxTree.Options.Kind != 0)
                {
                    return Compilation.ScriptClass;
                }
                if (SyntaxFacts.IsTypeDeclaration(memberDeclaration.Kind()))
                {
                    return _compilation.Assembly.GlobalNamespace;
                }
                return _compilation.Assembly.GlobalNamespace.ImplicitType;
            }
            NamespaceOrTypeSymbol declaredNamespaceOrType = GetDeclaredNamespaceOrType(memberDeclaration.Parent);
            if (!declaredNamespaceOrType.IsNamespace)
            {
                return declaredNamespaceOrType;
            }
            if (memberDeclaration.Kind() == SyntaxKind.NamespaceDeclaration || SyntaxFacts.IsTypeDeclaration(memberDeclaration.Kind()))
            {
                return declaredNamespaceOrType;
            }
            return ((NamespaceSymbol)declaredNamespaceOrType).ImplicitType;
        }

        private Symbol GetDeclaredMemberSymbol(CSharpSyntaxNode declarationSyntax)
        {
            CheckSyntaxNode(declarationSyntax);
            NamespaceOrTypeSymbol declaredTypeMemberContainer = GetDeclaredTypeMemberContainer(declarationSyntax);
            string declarationName = GetDeclarationName(declarationSyntax);
            return GetDeclaredMember(declaredTypeMemberContainer, declarationSyntax.Span, declarationName);
        }

        public override AwaitExpressionInfo GetAwaitExpressionInfo(AwaitExpressionSyntax node)
        {
            return GetMemberModel(node)?.GetAwaitExpressionInfo(node) ?? default(AwaitExpressionInfo);
        }

        public override ForEachStatementInfo GetForEachStatementInfo(ForEachStatementSyntax node)
        {
            return GetMemberModel(node)?.GetForEachStatementInfo(node) ?? default(ForEachStatementInfo);
        }

        public override ForEachStatementInfo GetForEachStatementInfo(CommonForEachStatementSyntax node)
        {
            return GetMemberModel(node)?.GetForEachStatementInfo(node) ?? default(ForEachStatementInfo);
        }

        public override DeconstructionInfo GetDeconstructionInfo(AssignmentExpressionSyntax node)
        {
            return GetMemberModel(node)?.GetDeconstructionInfo(node) ?? default(DeconstructionInfo);
        }

        public override DeconstructionInfo GetDeconstructionInfo(ForEachVariableStatementSyntax node)
        {
            return GetMemberModel(node)?.GetDeconstructionInfo(node) ?? default(DeconstructionInfo);
        }

        internal override Symbol RemapSymbolIfNecessaryCore(Symbol symbol)
        {
            if (symbol.Locations.IsDefaultOrEmpty)
            {
                return symbol;
            }
            Location location = symbol.Locations[0];
            if (location.SourceTree != SyntaxTree)
            {
                return symbol;
            }
            int position = CheckAndAdjustPosition(location.SourceSpan.Start);
            return GetMemberModel(position)?.RemapSymbolIfNecessaryCore(symbol) ?? symbol;
        }

        public override Func<SyntaxNode, bool> GetSyntaxNodesToAnalyzeFilter(SyntaxNode declaredNode, ISymbol declaredSymbol)
        {
            CompilationUnitSyntax compilationUnitSyntax = declaredNode as CompilationUnitSyntax;
            if (compilationUnitSyntax == null)
            {
                RecordDeclarationSyntax recordDeclarationSyntax = declaredNode as RecordDeclarationSyntax;
                if (recordDeclarationSyntax == null)
                {
                    PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax = declaredNode as PrimaryConstructorBaseTypeSyntax;
                    if (primaryConstructorBaseTypeSyntax != null)
                    {
                        CSharpSyntaxNode parent = primaryConstructorBaseTypeSyntax.Parent;
                        if (parent is BaseListSyntax && parent.Parent is RecordDeclarationSyntax recordDeclarationSyntax2 && recordDeclarationSyntax2.PrimaryConstructorBaseTypeIfClass == declaredNode)
                        {
                            SynthesizedRecordConstructor synthesizedRecordConstructor = TryGetSynthesizedRecordConstructor(recordDeclarationSyntax2);
                            if ((object)synthesizedRecordConstructor != null && (object)declaredSymbol.GetSymbol() == synthesizedRecordConstructor)
                            {
                                return (SyntaxNode node) => node != primaryConstructorBaseTypeSyntax.Type;
                            }
                        }
                    }
                    else if (declaredNode is ParameterSyntax parameterSyntax && declaredSymbol.Kind == SymbolKind.Property && parameterSyntax.Parent?.Parent is RecordDeclarationSyntax recordDeclarationSyntax3 && recordDeclarationSyntax3.ParameterList == parameterSyntax.Parent)
                    {
                        return (SyntaxNode node) => false;
                    }
                }
                else if ((object)TryGetSynthesizedRecordConstructor(recordDeclarationSyntax) != null)
                {
                    if (recordDeclarationSyntax.IsKind(SyntaxKind.RecordDeclaration))
                    {
                        switch (declaredSymbol.Kind)
                        {
                            case SymbolKind.Method:
                                return delegate (SyntaxNode node)
                                {
                                    if (node.Parent == recordDeclarationSyntax)
                                    {
                                        if (node != recordDeclarationSyntax.ParameterList)
                                        {
                                            return node == recordDeclarationSyntax.BaseList;
                                        }
                                        return true;
                                    }
                                    if (node.Parent is BaseListSyntax)
                                    {
                                        return node == recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass;
                                    }
                                    return !(node.Parent is PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax2) || primaryConstructorBaseTypeSyntax2 != recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass || node == primaryConstructorBaseTypeSyntax2.ArgumentList;
                                };
                            case SymbolKind.NamedType:
                                return (SyntaxNode node) => node != recordDeclarationSyntax.ParameterList && (node.Kind() != SyntaxKind.ArgumentList || node != recordDeclarationSyntax.PrimaryConstructorBaseTypeIfClass?.ArgumentList);
                        }
                        ExceptionUtilities.UnexpectedValue(declaredSymbol.Kind);
                    }
                    else
                    {
                        switch (declaredSymbol.Kind)
                        {
                            case SymbolKind.Method:
                                return (SyntaxNode node) => node.Parent != recordDeclarationSyntax || node == recordDeclarationSyntax.ParameterList;
                            case SymbolKind.NamedType:
                                return (SyntaxNode node) => node != recordDeclarationSyntax.ParameterList;
                        }
                        ExceptionUtilities.UnexpectedValue(declaredSymbol.Kind);
                    }
                }
            }
            else if ((object)SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(Compilation, compilationUnitSyntax, fallbackToMainEntryPoint: false) != null)
            {
                switch (declaredSymbol.Kind)
                {
                    case SymbolKind.Namespace:
                        return (SyntaxNode node) => node.Kind() != SyntaxKind.GlobalStatement || node.Parent != compilationUnitSyntax;
                    case SymbolKind.Method:
                        return (SyntaxNode node) => node.Parent != compilationUnitSyntax || node.Kind() == SyntaxKind.GlobalStatement;
                    case SymbolKind.NamedType:
                        return (SyntaxNode node) => false;
                }
                ExceptionUtilities.UnexpectedValue(declaredSymbol.Kind);
            }
            return null;
        }

        private RegionAnalysisContext RegionAnalysisContext(ExpressionSyntax expression)
        {
            while (expression.Kind() == SyntaxKind.ParenthesizedExpression)
            {
                expression = ((ParenthesizedExpressionSyntax)expression).Expression;
            }
            MemberSemanticModel memberModel = GetMemberModel(expression);
            if (memberModel == null)
            {
                BoundBadStatement boundBadStatement = new BoundBadStatement(expression, ImmutableArray<BoundNode>.Empty, hasErrors: true);
                return new RegionAnalysisContext(Compilation, null, boundBadStatement, boundBadStatement, boundBadStatement);
            }
            BoundNode boundRoot = GetBoundRoot(memberModel, out Symbol member);
            BoundNode upperBoundNode = memberModel.GetUpperBoundNode(expression, promoteToBindable: true);
            BoundNode lastInRegion = upperBoundNode;
            return new RegionAnalysisContext(Compilation, member, boundRoot, upperBoundNode, lastInRegion);
        }

        private RegionAnalysisContext RegionAnalysisContext(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            MemberSemanticModel memberModel = GetMemberModel(firstStatement);
            if (memberModel == null)
            {
                BoundBadStatement boundBadStatement = new BoundBadStatement(firstStatement, ImmutableArray<BoundNode>.Empty, hasErrors: true);
                return new RegionAnalysisContext(Compilation, null, boundBadStatement, boundBadStatement, boundBadStatement);
            }
            BoundNode boundRoot = GetBoundRoot(memberModel, out Symbol member);
            BoundNode upperBoundNode = memberModel.GetUpperBoundNode(firstStatement, promoteToBindable: true);
            BoundNode upperBoundNode2 = memberModel.GetUpperBoundNode(lastStatement, promoteToBindable: true);
            return new RegionAnalysisContext(Compilation, member, boundRoot, upperBoundNode, upperBoundNode2);
        }
    }
}
