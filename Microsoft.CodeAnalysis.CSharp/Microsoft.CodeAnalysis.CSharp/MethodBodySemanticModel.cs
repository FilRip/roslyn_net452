using System.Collections.Immutable;
using System.Linq;

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

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class MethodBodySemanticModel : MemberSemanticModel
    {
        internal readonly struct InitialState
        {
            internal readonly CSharpSyntaxNode Syntax;

            internal readonly BoundNode? Body;

            internal readonly ExecutableCodeBinder? Binder;

            internal readonly NullableWalker.SnapshotManager? SnapshotManager;

            internal readonly ImmutableDictionary<Symbol, Symbol>? RemappedSymbols;

            internal InitialState(CSharpSyntaxNode syntax, BoundNode? bodyOpt = null, ExecutableCodeBinder? binder = null, NullableWalker.SnapshotManager? snapshotManager = null, ImmutableDictionary<Symbol, Symbol>? remappedSymbols = null)
            {
                Syntax = syntax;
                Body = bodyOpt;
                Binder = binder;
                SnapshotManager = snapshotManager;
                RemappedSymbols = remappedSymbols;
            }
        }

        private MethodBodySemanticModel(MethodSymbol owner, Binder rootBinder, CSharpSyntaxNode syntax, SyntaxTreeSemanticModel containingSemanticModelOpt = null, SyntaxTreeSemanticModel parentSemanticModelOpt = null, NullableWalker.SnapshotManager snapshotManagerOpt = null, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt = null, int speculatedPosition = 0)
            : base(syntax, owner, rootBinder, containingSemanticModelOpt, parentSemanticModelOpt, snapshotManagerOpt, parentRemappedSymbolsOpt, speculatedPosition)
        {
        }

        internal static MethodBodySemanticModel Create(SyntaxTreeSemanticModel containingSemanticModel, MethodSymbol owner, InitialState initialState)
        {
            MethodBodySemanticModel methodBodySemanticModel = new MethodBodySemanticModel(owner, initialState.Binder, initialState.Syntax, containingSemanticModel);
            if (initialState.Body != null)
            {
                methodBodySemanticModel.UnguardedAddBoundTreeForStandaloneSyntax(initialState.Syntax, initialState.Body, initialState.SnapshotManager, initialState.RemappedSymbols);
            }
            return methodBodySemanticModel;
        }

        internal override BoundNode Bind(Binder binder, CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ArrowExpressionClause:
                    return binder.BindExpressionBodyAsBlock((ArrowExpressionClauseSyntax)node, diagnostics);
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.ThisConstructorInitializer:
                    return binder.BindConstructorInitializer((ConstructorInitializerSyntax)node, diagnostics);
                case SyntaxKind.PrimaryConstructorBaseType:
                    return binder.BindConstructorInitializer((PrimaryConstructorBaseTypeSyntax)node, diagnostics);
                case SyntaxKind.CompilationUnit:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                case SyntaxKind.RecordDeclaration:
                    return binder.BindMethodBody(node, diagnostics);
                default:
                    return base.Bind(binder, node, diagnostics);
            }
        }

        internal static MethodBodySemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, MethodSymbol owner, StatementSyntax syntax, Binder rootBinder, NullableWalker.SnapshotManager snapshotManagerOpt, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt, int position)
        {
            return new MethodBodySemanticModel(owner, rootBinder, syntax, null, parentSemanticModel, snapshotManagerOpt, parentRemappedSymbolsOpt, position);
        }

        internal static MethodBodySemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, MethodSymbol owner, ArrowExpressionClauseSyntax syntax, Binder rootBinder, int position)
        {
            return new MethodBodySemanticModel(owner, rootBinder, syntax, null, parentSemanticModel, null, null, position);
        }

        internal static MethodBodySemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, MethodSymbol owner, ConstructorInitializerSyntax syntax, Binder rootBinder, int position)
        {
            return new MethodBodySemanticModel(owner, rootBinder, syntax, null, parentSemanticModel, null, null, position);
        }

        internal static MethodBodySemanticModel CreateSpeculative(SyntaxTreeSemanticModel parentSemanticModel, MethodSymbol owner, PrimaryConstructorBaseTypeSyntax syntax, Binder rootBinder, int position)
        {
            return new MethodBodySemanticModel(owner, rootBinder, syntax, null, parentSemanticModel, null, null, position);
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel)
        {
            return GetSpeculativeSemanticModelForMethodBody(parentModel, position, method.Body, out speculativeModel);
        }

        private bool GetSpeculativeSemanticModelForMethodBody(SyntaxTreeSemanticModel parentModel, int position, BlockSyntax body, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            MethodSymbol methodSymbol = (MethodSymbol)base.MemberSymbol;
            Binder binder = RootBinder;
            do
            {
                if (binder is ExecutableCodeBinder)
                {
                    binder = binder.Next;
                    break;
                }
                binder = binder.Next;
            }
            while (binder != null);
            Binder next = new WithNullableContextBinder(SyntaxTree, position, binder ?? RootBinder);
            next = new ExecutableCodeBinder(body, methodSymbol, next);
            Binder rootBinder = next.GetBinder(body)!.WithAdditionalFlags(GetSemanticModelBinderFlags());
            speculativeModel = CreateSpeculative(parentModel, methodSymbol, body, rootBinder, null, null, position);
            return true;
        }

        internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel)
        {
            return GetSpeculativeSemanticModelForMethodBody(parentModel, position, accessor.Body, out speculativeModel);
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder == null)
            {
                speculativeModel = null;
                return false;
            }
            MethodSymbol methodSymbol = (MethodSymbol)base.MemberSymbol;
            enclosingBinder = new WithNullableContextBinder(SyntaxTree, position, enclosingBinder);
            enclosingBinder = new ExecutableCodeBinder(statement, methodSymbol, enclosingBinder);
            speculativeModel = CreateSpeculative(parentModel, methodSymbol, statement, enclosingBinder, GetSnapshotManager(), GetRemappedSymbols(), position);
            return true;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel)
        {
            position = CheckAndAdjustPosition(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder == null)
            {
                speculativeModel = null;
                return false;
            }
            MethodSymbol methodSymbol = (MethodSymbol)base.MemberSymbol;
            enclosingBinder = new WithNullableContextBinder(SyntaxTree, position, enclosingBinder);
            enclosingBinder = new ExecutableCodeBinder(expressionBody, methodSymbol, enclosingBinder);
            speculativeModel = CreateSpeculative(parentModel, methodSymbol, expressionBody, enclosingBinder, position);
            return true;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            if (base.MemberSymbol is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Constructor && Root.FindToken(position).Parent?.AncestorsAndSelf().OfType<ConstructorInitializerSyntax>().FirstOrDefault()?.Parent == Root)
            {
                Binder enclosingBinder = GetEnclosingBinder(position);
                if (enclosingBinder != null)
                {
                    enclosingBinder = new WithNullableContextBinder(SyntaxTree, position, enclosingBinder);
                    enclosingBinder = new ExecutableCodeBinder(constructorInitializer, methodSymbol, enclosingBinder);
                    speculativeModel = CreateSpeculative(parentModel, methodSymbol, constructorInitializer, enclosingBinder, position);
                    return true;
                }
            }
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            if (base.MemberSymbol is SynthesizedRecordConstructor synthesizedRecordConstructor)
            {
                RecordDeclarationSyntax syntax = synthesizedRecordConstructor.GetSyntax();
                if (syntax != null && Root.FindToken(position).Parent?.AncestorsAndSelf().OfType<PrimaryConstructorBaseTypeSyntax>().FirstOrDefault() == syntax.PrimaryConstructorBaseTypeIfClass)
                {
                    Binder enclosingBinder = GetEnclosingBinder(position);
                    if (enclosingBinder != null)
                    {
                        enclosingBinder = new WithNullableContextBinder(SyntaxTree, position, enclosingBinder);
                        enclosingBinder = new ExecutableCodeBinder(constructorInitializer, synthesizedRecordConstructor, enclosingBinder);
                        speculativeModel = CreateSpeculative(parentModel, synthesizedRecordConstructor, constructorInitializer, enclosingBinder, position);
                        return true;
                    }
                }
            }
            speculativeModel = null;
            return false;
        }

        internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel)
        {
            speculativeModel = null;
            return false;
        }

        protected override BoundNode RewriteNullableBoundNodesWithSnapshots(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots, out NullableWalker.SnapshotManager snapshotManager, ref ImmutableDictionary<Symbol, Symbol> remappedSymbols)
        {
            NullableWalker.VariableState afterInitializersState = NullableWalker.GetAfterInitializersState(Compilation, base.MemberSymbol);
            return NullableWalker.AnalyzeAndRewrite(Compilation, base.MemberSymbol, boundRoot, binder, afterInitializersState, diagnostics, createSnapshots, out snapshotManager, ref remappedSymbols);
        }

        protected override void AnalyzeBoundNodeNullability(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots)
        {
            NullableWalker.AnalyzeWithoutRewrite(Compilation, base.MemberSymbol, boundRoot, binder, diagnostics, createSnapshots);
        }

        protected override bool IsNullableAnalysisEnabled()
        {
            return Compilation.IsNullableAnalysisEnabledIn((MethodSymbol)base.MemberSymbol);
        }
    }
}
