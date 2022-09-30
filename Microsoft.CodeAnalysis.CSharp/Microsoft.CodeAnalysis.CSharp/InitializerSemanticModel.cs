using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class InitializerSemanticModel : MemberSemanticModel
    {
        private InitializerSemanticModel(CSharpSyntaxNode syntax, Symbol symbol, Binder rootBinder, SyntaxTreeSemanticModel containingSemanticModelOpt = null, SyntaxTreeSemanticModel parentSemanticModelOpt = null, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt = null, int speculatedPosition = 0)
            : base(syntax, symbol, rootBinder, containingSemanticModelOpt, parentSemanticModelOpt, null, parentRemappedSymbolsOpt, speculatedPosition)
        {
        }

        internal static InitializerSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, CSharpSyntaxNode syntax, FieldSymbol fieldSymbol, Binder rootBinder)
        {
            return new InitializerSemanticModel(syntax, fieldSymbol, rootBinder, containingSemanticModel);
        }

        internal static InitializerSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, CSharpSyntaxNode syntax, PropertySymbol propertySymbol, Binder rootBinder)
        {
            return new InitializerSemanticModel(syntax, propertySymbol, rootBinder, containingSemanticModel);
        }

        internal static InitializerSemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, ParameterSyntax syntax, ParameterSymbol parameterSymbol, Binder rootBinder, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt)
        {
            return new InitializerSemanticModel(syntax, parameterSymbol, rootBinder, containingSemanticModel, null, parentRemappedSymbolsOpt);
        }

        internal static InitializerSemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, Symbol owner, CSharpSyntaxNode syntax, Binder rootBinder, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt, int position)
        {
            return new InitializerSemanticModel(syntax, owner, rootBinder, null, parentSemanticModel, parentRemappedSymbolsOpt, position);
        }

        protected internal override CSharpSyntaxNode GetBindableSyntaxNode(CSharpSyntaxNode node)
        {
            if (!IsBindableInitializer(node))
            {
                return base.GetBindableSyntaxNode(node);
            }
            return node;
        }

        internal override BoundNode GetBoundRoot()
        {
            CSharpSyntaxNode root = Root;
            return GetUpperBoundNode(GetBindableSyntaxNode(root.Kind() switch
            {
                SyntaxKind.VariableDeclarator => ((VariableDeclaratorSyntax)root).Initializer,
                SyntaxKind.Parameter => ((ParameterSyntax)root).Default,
                SyntaxKind.EqualsValueClause => (EqualsValueClauseSyntax)root,
                SyntaxKind.EnumMemberDeclaration => ((EnumMemberDeclarationSyntax)root).EqualsValue,
                SyntaxKind.PropertyDeclaration => ((PropertyDeclarationSyntax)root).Initializer,
                _ => throw ExceptionUtilities.UnexpectedValue(root.Kind()),
            }));
        }

        internal override BoundNode Bind(Binder binder, CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            EqualsValueClauseSyntax equalsValueClauseSyntax = null;
            switch (node.Kind())
            {
                case SyntaxKind.EqualsValueClause:
                    equalsValueClauseSyntax = (EqualsValueClauseSyntax)node;
                    break;
                case SyntaxKind.VariableDeclarator:
                    equalsValueClauseSyntax = ((VariableDeclaratorSyntax)node).Initializer;
                    break;
                case SyntaxKind.PropertyDeclaration:
                    equalsValueClauseSyntax = ((PropertyDeclarationSyntax)node).Initializer;
                    break;
                case SyntaxKind.Parameter:
                    equalsValueClauseSyntax = ((ParameterSyntax)node).Default;
                    break;
                case SyntaxKind.EnumMemberDeclaration:
                    equalsValueClauseSyntax = ((EnumMemberDeclarationSyntax)node).EqualsValue;
                    break;
            }
            if (equalsValueClauseSyntax != null)
            {
                return BindEqualsValue(binder, equalsValueClauseSyntax, diagnostics);
            }
            return base.Bind(binder, node, diagnostics);
        }

        private BoundEqualsValue BindEqualsValue(Binder binder, EqualsValueClauseSyntax equalsValue, BindingDiagnosticBag diagnostics)
        {
            switch (base.MemberSymbol.Kind)
            {
                case SymbolKind.Field:
                    {
                        FieldSymbol fieldSymbol = (FieldSymbol)base.MemberSymbol;
                        if (fieldSymbol is SourceEnumConstantSymbol symbol)
                        {
                            return binder.BindEnumConstantInitializer(symbol, equalsValue, diagnostics);
                        }
                        return binder.BindFieldInitializer(fieldSymbol, equalsValue, diagnostics);
                    }
                case SymbolKind.Property:
                    {
                        SourcePropertySymbol sourcePropertySymbol = (SourcePropertySymbol)base.MemberSymbol;
                        BoundFieldEqualsValue boundFieldEqualsValue = binder.BindFieldInitializer(sourcePropertySymbol.BackingField, equalsValue, diagnostics);
                        return new BoundPropertyEqualsValue(boundFieldEqualsValue.Syntax, sourcePropertySymbol, boundFieldEqualsValue.Locals, boundFieldEqualsValue.Value);
                    }
                case SymbolKind.Parameter:
                    {
                        ParameterSymbol parameter = (ParameterSymbol)base.MemberSymbol;
                        return binder.BindParameterDefaultValue(equalsValue, parameter, diagnostics, out BoundExpression valueBeforeConversion);
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(base.MemberSymbol.Kind);
            }
        }

        private bool IsBindableInitializer(CSharpSyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.EqualsValueClause)
            {
                if (Root != node)
                {
                    return Root == node.Parent;
                }
                return true;
            }
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel)
        {
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder == null)
            {
                speculativeModel = null;
                return false;
            }
            enclosingBinder = new ExecutableCodeBinder(initializer, enclosingBinder.ContainingMemberOrLambda, enclosingBinder);
            speculativeModel = CreateSpeculative(parentModel, base.MemberSymbol, initializer, enclosingBinder, GetRemappedSymbols(), position);
            return true;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        protected override BoundNode RewriteNullableBoundNodesWithSnapshots(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots, out NullableWalker.SnapshotManager snapshotManager, ref ImmutableDictionary<Symbol, Symbol> remappedSymbols)
        {
            return NullableWalker.AnalyzeAndRewrite(Compilation, base.MemberSymbol, boundRoot, binder, null, diagnostics, createSnapshots, out snapshotManager, ref remappedSymbols);
        }

        protected override void AnalyzeBoundNodeNullability(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots)
        {
            NullableWalker.AnalyzeWithoutRewrite(Compilation, base.MemberSymbol, boundRoot, binder, diagnostics, createSnapshots);
        }

        protected override bool IsNullableAnalysisEnabled()
        {
            switch (base.MemberSymbol.Kind)
            {
                case SymbolKind.Field:
                case SymbolKind.Property:
                    if (base.MemberSymbol.ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
                    {
                        return sourceMemberContainerTypeSymbol.IsNullableEnabledForConstructorsAndInitializers(base.MemberSymbol.IsStatic);
                    }
                    return false;
                case SymbolKind.Parameter:
                    {
                        SyntaxNode defaultValueSyntaxForIsNullableAnalysisEnabled = SourceComplexParameterSymbol.GetDefaultValueSyntaxForIsNullableAnalysisEnabled(Root as ParameterSyntax);
                        if (defaultValueSyntaxForIsNullableAnalysisEnabled != null)
                        {
                            return Compilation.IsNullableAnalysisEnabledIn(defaultValueSyntaxForIsNullableAnalysisEnabled);
                        }
                        return false;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(base.MemberSymbol.Kind);
            }
        }
    }
}
