using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class AttributeSemanticModel : MemberSemanticModel
    {
        private readonly AliasSymbol _aliasOpt;

        private NamedTypeSymbol AttributeType => (NamedTypeSymbol)base.MemberSymbol;

        private AttributeSemanticModel(AttributeSyntax syntax, NamedTypeSymbol attributeType, AliasSymbol aliasOpt, Binder rootBinder, SyntaxTreeSemanticModel? containingSemanticModelOpt = null, SyntaxTreeSemanticModel? parentSemanticModelOpt = null, ImmutableDictionary<Symbol, Symbol>? parentRemappedSymbolsOpt = null, int speculatedPosition = 0)
            : base(syntax, attributeType, new ExecutableCodeBinder(syntax, rootBinder.ContainingMember(), rootBinder), containingSemanticModelOpt, parentSemanticModelOpt, null, parentRemappedSymbolsOpt, speculatedPosition)
        {
            _aliasOpt = aliasOpt;
        }

        public static AttributeSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, AttributeSyntax syntax, NamedTypeSymbol attributeType, AliasSymbol aliasOpt, Binder rootBinder, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt)
        {
            return new AttributeSemanticModel(syntax, attributeType, aliasOpt, rootBinder, containingSemanticModel, null, parentRemappedSymbolsOpt);
        }

        public static AttributeSemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, AttributeSyntax syntax, NamedTypeSymbol attributeType, AliasSymbol aliasOpt, Binder rootBinder, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt, int position)
        {
            return new AttributeSemanticModel(syntax, attributeType, aliasOpt, rootBinder, null, parentSemanticModel, parentRemappedSymbolsOpt, position);
        }

        protected internal override CSharpSyntaxNode GetBindableSyntaxNode(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.Attribute:
                    return node;
                case SyntaxKind.AttributeArgument:
                    {
                        CSharpSyntaxNode parent = node.Parent;
                        if (parent != null)
                        {
                            parent = parent.Parent;
                            if (parent != null)
                            {
                                return parent;
                            }
                        }
                        break;
                    }
            }
            return base.GetBindableSyntaxNode(node);
        }

        internal override BoundNode Bind(Binder binder, CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            if (node.Kind() == SyntaxKind.Attribute)
            {
                AttributeSyntax node2 = (AttributeSyntax)node;
                return binder.BindAttribute(node2, AttributeType, diagnostics);
            }
            if (SyntaxFacts.IsAttributeName(node))
            {
                return new BoundTypeExpression((NameSyntax)node, _aliasOpt, AttributeType);
            }
            return base.Bind(binder, node, diagnostics);
        }

        protected override BoundNode RewriteNullableBoundNodesWithSnapshots(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots, out NullableWalker.SnapshotManager? snapshotManager, ref ImmutableDictionary<Symbol, Symbol>? remappedSymbols)
        {
            return NullableWalker.AnalyzeAndRewrite(Compilation, null, boundRoot, binder, null, diagnostics, createSnapshots, out snapshotManager, ref remappedSymbols);
        }

        protected override void AnalyzeBoundNodeNullability(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots)
        {
            NullableWalker.AnalyzeWithoutRewrite(Compilation, null, boundRoot, binder, diagnostics, createSnapshots);
        }

        protected override bool IsNullableAnalysisEnabled()
        {
            return IsNullableAnalysisEnabledIn(Compilation, (AttributeSyntax)Root);
        }

        internal static bool IsNullableAnalysisEnabledIn(CSharpCompilation compilation, AttributeSyntax syntax)
        {
            return compilation.IsNullableAnalysisEnabledIn(syntax);
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel? speculativeModel)
        {
            speculativeModel = null;
            return false;
        }
    }
}
