using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class MemberSemanticModel : CSharpSemanticModel
    {
        internal class IncrementalBinder : Binder
        {
            private readonly MemberSemanticModel _semanticModel;

            internal IncrementalBinder(MemberSemanticModel semanticModel, Binder next)
                : base(next)
            {
                _semanticModel = semanticModel;
            }

            internal override Binder GetBinder(SyntaxNode node)
            {
                Binder binder = base.Next!.GetBinder(node);
                if (binder != null)
                {
                    return new IncrementalBinder(_semanticModel, binder.WithAdditionalFlags(BinderFlags.SemanticModel));
                }
                return null;
            }

            public override BoundStatement BindStatement(StatementSyntax node, BindingDiagnosticBag diagnostics)
            {
                if (node.SyntaxTree == _semanticModel.SyntaxTree)
                {
                    BoundStatement boundStatement = _semanticModel.GuardedGetSynthesizedStatementFromMap(node);
                    if (boundStatement != null)
                    {
                        return boundStatement;
                    }
                    BoundNode boundNode = TryGetBoundNodeFromMap(node);
                    if (boundNode != null)
                    {
                        return (BoundStatement)boundNode;
                    }
                }
                BoundStatement boundStatement2 = base.BindStatement(node, diagnostics);
                if (boundStatement2.WasCompilerGenerated && node.SyntaxTree == _semanticModel.SyntaxTree)
                {
                    _semanticModel.GuardedAddSynthesizedStatementToMap(node, boundStatement2);
                }
                return boundStatement2;
            }

            internal override BoundBlock BindEmbeddedBlock(BlockSyntax node, BindingDiagnosticBag diagnostics)
            {
                BoundBlock boundBlock = (BoundBlock)TryGetBoundNodeFromMap(node);
                if (boundBlock != null)
                {
                    return boundBlock;
                }
                return base.BindEmbeddedBlock(node, diagnostics);
            }

            private BoundNode TryGetBoundNodeFromMap(CSharpSyntaxNode node)
            {
                if (node.SyntaxTree == _semanticModel.SyntaxTree)
                {
                    ImmutableArray<BoundNode> immutableArray = _semanticModel.GuardedGetBoundNodesFromMap(node);
                    if (!immutableArray.IsDefaultOrEmpty)
                    {
                        return immutableArray[0];
                    }
                }
                return null;
            }

            public override BoundNode BindMethodBody(CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
            {
                BoundNode boundNode = TryGetBoundNodeFromMap(node);
                if (boundNode != null)
                {
                    return boundNode;
                }
                return base.BindMethodBody(node, diagnostics);
            }

            internal override BoundExpressionStatement BindConstructorInitializer(ConstructorInitializerSyntax node, BindingDiagnosticBag diagnostics)
            {
                return ((BoundExpressionStatement)TryGetBoundNodeFromMap(node)) ?? base.BindConstructorInitializer(node, diagnostics);
            }

            internal override BoundExpressionStatement BindConstructorInitializer(PrimaryConstructorBaseTypeSyntax node, BindingDiagnosticBag diagnostics)
            {
                return ((BoundExpressionStatement)TryGetBoundNodeFromMap(node)) ?? base.BindConstructorInitializer(node, diagnostics);
            }

            internal override BoundBlock BindExpressionBodyAsBlock(ArrowExpressionClauseSyntax node, BindingDiagnosticBag diagnostics)
            {
                BoundBlock boundBlock = (BoundBlock)TryGetBoundNodeFromMap(node);
                if (boundBlock != null)
                {
                    return boundBlock;
                }
                return base.BindExpressionBodyAsBlock(node, diagnostics);
            }
        }

        protected sealed class NodeMapBuilder : BoundTreeWalkerWithStackGuard
        {
            private readonly OrderPreservingMultiDictionary<SyntaxNode, BoundNode> _map;

            private readonly SyntaxTree _tree;

            private readonly SyntaxNode _thisSyntaxNodeOnly;

            private NodeMapBuilder(OrderPreservingMultiDictionary<SyntaxNode, BoundNode> map, SyntaxTree tree, SyntaxNode thisSyntaxNodeOnly)
            {
                _map = map;
                _tree = tree;
                _thisSyntaxNodeOnly = thisSyntaxNodeOnly;
            }

            public static void AddToMap(BoundNode root, Dictionary<SyntaxNode, ImmutableArray<BoundNode>> map, SyntaxTree tree, SyntaxNode node = null)
            {
                if (root == null || map.ContainsKey(root.Syntax))
                {
                    return;
                }
                OrderPreservingMultiDictionary<SyntaxNode, BoundNode> instance = OrderPreservingMultiDictionary<SyntaxNode, BoundNode>.GetInstance();
                new NodeMapBuilder(instance, tree, node).Visit(root);
                foreach (CSharpSyntaxNode key in instance.Keys)
                {
                    if (!map.ContainsKey(key))
                    {
                        map[key] = instance[key];
                    }
                }
                instance.Free();
            }

            public override BoundNode Visit(BoundNode node)
            {
                if (node == null || node.SyntaxTree != _tree)
                {
                    return null;
                }
                BoundNode boundNode = node;
                if (node.Kind == BoundKind.UnboundLambda)
                {
                    boundNode = ((UnboundLambda)node).BindForErrorRecovery();
                }
                if (ShouldAddNode(boundNode))
                {
                    _map.Add(boundNode.Syntax, boundNode);
                }
                if (boundNode is BoundBinaryOperator boundBinaryOperator)
                {
                    ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
                    instance.Push(boundBinaryOperator.Right);
                    boundNode = boundBinaryOperator.Left;
                    for (BoundBinaryOperator boundBinaryOperator2 = boundNode as BoundBinaryOperator; boundBinaryOperator2 != null; boundBinaryOperator2 = boundNode as BoundBinaryOperator)
                    {
                        if (ShouldAddNode(boundBinaryOperator2))
                        {
                            _map.Add(boundBinaryOperator2.Syntax, boundBinaryOperator2);
                        }
                        instance.Push(boundBinaryOperator2.Right);
                        boundNode = boundBinaryOperator2.Left;
                    }
                    Visit(boundNode);
                    while (instance.Count > 0)
                    {
                        Visit(instance.Pop());
                    }
                    instance.Free();
                }
                else
                {
                    base.Visit(boundNode);
                }
                return null;
            }

            private bool ShouldAddNode(BoundNode currentBoundNode)
            {
                if (currentBoundNode.WasCompilerGenerated)
                {
                    return false;
                }
                if (_thisSyntaxNodeOnly != null && currentBoundNode.Syntax != _thisSyntaxNodeOnly)
                {
                    return false;
                }
                return true;
            }

            public override BoundNode VisitQueryClause(BoundQueryClause node)
            {
                Visit(node.Value);
                VisitUnoptimizedForm(node);
                return null;
            }

            public override BoundNode VisitRangeVariable(BoundRangeVariable node)
            {
                return null;
            }

            public override BoundNode VisitAwaitableInfo(BoundAwaitableInfo node)
            {
                return null;
            }

            public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
            {
                throw ExceptionUtilities.Unreachable;
            }

            protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
            {
                return false;
            }
        }

        private sealed class SpeculativeMemberSemanticModel : MemberSemanticModel
        {
            public SpeculativeMemberSemanticModel(SyntaxTreeSemanticModel parentSemanticModel, Symbol owner, TypeSyntax root, Binder rootBinder, NullableWalker.SnapshotManager snapshotManagerOpt, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt, int position)
                : base(root, owner, rootBinder, null, parentSemanticModel, snapshotManagerOpt, parentRemappedSymbolsOpt, position)
            {
            }

            protected override NullableWalker.SnapshotManager GetSnapshotManager()
            {
                return _parentSnapshotManagerOpt;
            }

            protected override BoundNode RewriteNullableBoundNodesWithSnapshots(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots, out NullableWalker.SnapshotManager snapshotManager, ref ImmutableDictionary<Symbol, Symbol> remappedSymbols)
            {
                return NullableWalker.AnalyzeAndRewrite(Compilation, base.MemberSymbol as MethodSymbol, boundRoot, binder, null, diagnostics, createSnapshots: false, out snapshotManager, ref remappedSymbols);
            }

            protected override void AnalyzeBoundNodeNullability(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots)
            {
                NullableWalker.AnalyzeWithoutRewrite(Compilation, base.MemberSymbol as MethodSymbol, boundRoot, binder, diagnostics, createSnapshots);
            }

            protected override bool IsNullableAnalysisEnabled()
            {
                return _parentSemanticModelOpt.IsNullableAnalysisEnabledAtSpeculativePosition(OriginalPositionForSpeculation, Root);
            }

            internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel)
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private readonly Symbol _memberSymbol;

        private readonly CSharpSyntaxNode _root;

        private readonly ReaderWriterLockSlim _nodeMapLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly Dictionary<SyntaxNode, ImmutableArray<BoundNode>> _guardedBoundNodeMap = new Dictionary<SyntaxNode, ImmutableArray<BoundNode>>();

        private readonly Dictionary<SyntaxNode, IOperation> _guardedIOperationNodeMap = new Dictionary<SyntaxNode, IOperation>();

        private Dictionary<SyntaxNode, BoundStatement> _lazyGuardedSynthesizedStatementsMap;

        private NullableWalker.SnapshotManager _lazySnapshotManager;

        private ImmutableDictionary<Symbol, Symbol> _lazyRemappedSymbols;

        private readonly ImmutableDictionary<Symbol, Symbol> _parentRemappedSymbolsOpt;

        private readonly NullableWalker.SnapshotManager _parentSnapshotManagerOpt;

        internal readonly Binder RootBinder;

        private readonly SyntaxTreeSemanticModel _containingSemanticModelOpt;

        private readonly SyntaxTreeSemanticModel _parentSemanticModelOpt;

        private readonly int _speculatedPosition;

        private readonly Lazy<CSharpOperationFactory> _operationFactory;

        public override CSharpCompilation Compilation => (_containingSemanticModelOpt ?? _parentSemanticModelOpt).Compilation;

        internal override CSharpSyntaxNode Root => _root;

        internal Symbol MemberSymbol => _memberSymbol;

        public sealed override bool IsSpeculativeSemanticModel => _parentSemanticModelOpt != null;

        public sealed override int OriginalPositionForSpeculation => _speculatedPosition;

        public sealed override CSharpSemanticModel ParentModel => _parentSemanticModelOpt;

        public sealed override SemanticModel ContainingModelOrSelf => (SemanticModel)(_containingSemanticModelOpt ?? ((object)this));

        public override SyntaxTree SyntaxTree => _root.SyntaxTree;

        protected MemberSemanticModel(CSharpSyntaxNode root, Symbol memberSymbol, Binder rootBinder, SyntaxTreeSemanticModel containingSemanticModelOpt, SyntaxTreeSemanticModel parentSemanticModelOpt, NullableWalker.SnapshotManager snapshotManagerOpt, ImmutableDictionary<Symbol, Symbol> parentRemappedSymbolsOpt, int speculatedPosition)
        {
            _root = root;
            _memberSymbol = memberSymbol;
            RootBinder = rootBinder.WithAdditionalFlags(GetSemanticModelBinderFlags());
            _containingSemanticModelOpt = containingSemanticModelOpt;
            _parentSemanticModelOpt = parentSemanticModelOpt;
            _parentSnapshotManagerOpt = snapshotManagerOpt;
            _parentRemappedSymbolsOpt = parentRemappedSymbolsOpt;
            _speculatedPosition = speculatedPosition;
            _operationFactory = new Lazy<CSharpOperationFactory>(() => new CSharpOperationFactory(this));
        }

        internal override MemberSemanticModel GetMemberModel(SyntaxNode node)
        {
            if (!IsInTree(node))
            {
                return null;
            }
            return this;
        }

        protected virtual NullableWalker.SnapshotManager GetSnapshotManager()
        {
            EnsureNullabilityAnalysisPerformedIfNecessary();
            return _lazySnapshotManager;
        }

        internal ImmutableDictionary<Symbol, Symbol> GetRemappedSymbols()
        {
            EnsureNullabilityAnalysisPerformedIfNecessary();
            return _lazyRemappedSymbols;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel)
        {
            ExpressionSyntax standaloneExpression = SyntaxFactory.GetStandaloneExpression(type);
            Binder speculativeBinder = GetSpeculativeBinder(position, standaloneExpression, bindingOption);
            if (speculativeBinder != null)
            {
                speculativeModel = new SpeculativeMemberSemanticModel(parentModel, _memberSymbol, type, speculativeBinder, GetSnapshotManager(), GetRemappedSymbols(), position);
                return true;
            }
            speculativeModel = null;
            return false;
        }

        internal sealed override bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, CrefSyntax crefSyntax, out SemanticModel speculativeModel)
        {
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
                NullableWalker.SnapshotManager snapshotManager = GetSnapshotManager();
                if (snapshotManager != null)
                {
                    crefSymbols = default(ImmutableArray<Symbol>);
                    position = CheckAndAdjustPosition(position);
                    expression = SyntaxFactory.GetStandaloneExpression(expression);
                    binder = GetSpeculativeBinder(position, expression, bindingOption);
                    BoundExpression node = binder.BindExpression(expression, BindingDiagnosticBag.Discarded);
                    ImmutableDictionary<Symbol, Symbol> remappedSymbols = null;
                    return (BoundExpression)NullableWalker.AnalyzeAndRewriteSpeculation(position, node, binder, snapshotManager, out NullableWalker.SnapshotManager newSnapshots, ref remappedSymbols);
                }
            }
            return GetSpeculativelyBoundExpressionWithoutNullability(position, expression, bindingOption, out binder, out crefSymbols);
        }

        private Binder GetEnclosingBinderInternalWithinRoot(SyntaxNode node, int position)
        {
            return GetEnclosingBinderInternalWithinRoot(node, position, RootBinder, _root).WithAdditionalFlags(GetSemanticModelBinderFlags());
        }

        private static Binder GetEnclosingBinderInternalWithinRoot(SyntaxNode node, int position, Binder rootBinder, SyntaxNode root)
        {
            if (node == root)
            {
                return rootBinder.GetBinder(node) ?? rootBinder;
            }
            ExpressionSyntax expressionSyntax = null;
            LocalFunctionStatementSyntax localFunctionStatementSyntax = null;
            Binder binder = null;
            SyntaxNode syntaxNode = node;
            while (binder == null)
            {
                StatementSyntax statementSyntax = syntaxNode as StatementSyntax;
                SyntaxKind syntaxKind = syntaxNode.Kind();
                if (statementSyntax != null)
                {
                    if (LookupPosition.IsInStatementScope(position, statementSyntax))
                    {
                        binder = rootBinder.GetBinder(syntaxNode);
                        if (binder != null)
                        {
                            binder = AdjustBinderForPositionWithinStatement(position, binder, statementSyntax);
                        }
                        else if (syntaxKind == SyntaxKind.LocalFunctionStatement)
                        {
                            LocalFunctionStatementSyntax localFunctionStatementSyntax2 = (LocalFunctionStatementSyntax)statementSyntax;
                            if (localFunctionStatementSyntax2.TypeParameterList != null && !LookupPosition.IsBetweenTokens(position, localFunctionStatementSyntax2.Identifier, localFunctionStatementSyntax2.TypeParameterList!.LessThanToken))
                            {
                                localFunctionStatementSyntax = localFunctionStatementSyntax2;
                            }
                        }
                    }
                }
                else
                {
                    switch (syntaxKind)
                    {
                        case SyntaxKind.CatchClause:
                            if (LookupPosition.IsInCatchBlockScope(position, (CatchClauseSyntax)syntaxNode))
                            {
                                binder = rootBinder.GetBinder(syntaxNode);
                            }
                            break;
                        case SyntaxKind.CatchFilterClause:
                            if (LookupPosition.IsInCatchFilterScope(position, (CatchFilterClauseSyntax)syntaxNode))
                            {
                                binder = rootBinder.GetBinder(syntaxNode);
                            }
                            break;
                        default:
                            {
                                if (syntaxNode.IsAnonymousFunction())
                                {
                                    if (LookupPosition.IsInAnonymousFunctionOrQuery(position, syntaxNode))
                                    {
                                        binder = rootBinder.GetBinder(syntaxNode.AnonymousFunctionBody());
                                    }
                                    break;
                                }
                                TypeOfExpressionSyntax typeOfExpressionSyntax;
                                if (syntaxKind == SyntaxKind.TypeOfExpression && expressionSyntax == null && LookupPosition.IsBetweenTokens(position, (typeOfExpressionSyntax = (TypeOfExpressionSyntax)syntaxNode).OpenParenToken, typeOfExpressionSyntax.CloseParenToken))
                                {
                                    expressionSyntax = typeOfExpressionSyntax.Type;
                                    break;
                                }
                                switch (syntaxKind)
                                {
                                    case SyntaxKind.SwitchSection:
                                        if (LookupPosition.IsInSwitchSectionScope(position, (SwitchSectionSyntax)syntaxNode))
                                        {
                                            binder = rootBinder.GetBinder(syntaxNode);
                                        }
                                        break;
                                    case SyntaxKind.ArgumentList:
                                        {
                                            ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)syntaxNode;
                                            if (LookupPosition.IsBetweenTokens(position, argumentListSyntax.OpenParenToken, argumentListSyntax.CloseParenToken))
                                            {
                                                binder = rootBinder.GetBinder(syntaxNode);
                                            }
                                            break;
                                        }
                                    case SyntaxKind.EqualsValueClause:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.Attribute:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.ArrowExpressionClause:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.BaseConstructorInitializer:
                                    case SyntaxKind.ThisConstructorInitializer:
                                    case SyntaxKind.PrimaryConstructorBaseType:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.ConstructorDeclaration:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.SwitchExpression:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    case SyntaxKind.SwitchExpressionArm:
                                        binder = rootBinder.GetBinder(syntaxNode);
                                        break;
                                    default:
                                        if ((syntaxNode as ExpressionSyntax).IsValidScopeDesignator())
                                        {
                                            binder = rootBinder.GetBinder(syntaxNode);
                                        }
                                        break;
                                }
                                break;
                            }
                    }
                }
                if (syntaxNode == root)
                {
                    break;
                }
                syntaxNode = syntaxNode.ParentOrStructuredTriviaParent;
            }
            binder = binder ?? rootBinder.GetBinder(root) ?? rootBinder;
            if (localFunctionStatementSyntax != null)
            {
                LocalFunctionSymbol declaredLocalFunction = GetDeclaredLocalFunction(binder, localFunctionStatementSyntax.Identifier);
                if ((object)declaredLocalFunction != null)
                {
                    binder = declaredLocalFunction.SignatureBinder;
                }
            }
            if (expressionSyntax != null)
            {
                binder = new TypeofBinder(expressionSyntax, binder);
            }
            return binder;
        }

        private static Binder AdjustBinderForPositionWithinStatement(int position, Binder binder, StatementSyntax stmt)
        {
            switch (stmt.Kind())
            {
                case SyntaxKind.SwitchStatement:
                    {
                        SwitchStatementSyntax switchStatementSyntax = (SwitchStatementSyntax)stmt;
                        if (LookupPosition.IsBetweenTokens(position, switchStatementSyntax.SwitchKeyword, switchStatementSyntax.OpenBraceToken))
                        {
                            binder = binder.GetBinder(switchStatementSyntax.Expression);
                        }
                        break;
                    }
                case SyntaxKind.ForStatement:
                    {
                        ForStatementSyntax forStatementSyntax = (ForStatementSyntax)stmt;
                        if (LookupPosition.IsBetweenTokens(position, forStatementSyntax.SecondSemicolonToken, forStatementSyntax.CloseParenToken) && forStatementSyntax.Incrementors.Count > 0)
                        {
                            binder = binder.GetBinder(forStatementSyntax.Incrementors.First());
                        }
                        else if (LookupPosition.IsBetweenTokens(position, forStatementSyntax.FirstSemicolonToken, LookupPosition.GetFirstExcludedToken(forStatementSyntax)) && forStatementSyntax.Condition != null)
                        {
                            binder = binder.GetBinder(forStatementSyntax.Condition);
                        }
                        break;
                    }
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                    {
                        CommonForEachStatementSyntax commonForEachStatementSyntax = (CommonForEachStatementSyntax)stmt;
                        SyntaxToken firstIncluded = ((stmt.Kind() == SyntaxKind.ForEachVariableStatement) ? commonForEachStatementSyntax.InKeyword : commonForEachStatementSyntax.OpenParenToken);
                        if (LookupPosition.IsBetweenTokens(position, firstIncluded, commonForEachStatementSyntax.Statement.GetFirstToken()))
                        {
                            binder = binder.GetBinder(commonForEachStatementSyntax.Expression);
                        }
                        break;
                    }
            }
            return binder;
        }

        public override Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination, bool isExplicitInSource = false)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            TypeSymbol destination2 = destination.EnsureCSharpSymbolOrNull("destination");
            if (expression.Kind() == SyntaxKind.DeclarationExpression)
            {
                return Conversion.NoConversion;
            }
            if (expression.IsAnonymousFunction())
            {
                CheckSyntaxNode(expression);
                return ClassifyConversion(expression.SpanStart, expression, destination, isExplicitInSource);
            }
            if (isExplicitInSource)
            {
                return ClassifyConversionForCast(expression, destination2);
            }
            CheckSyntaxNode(expression);
            Binder enclosingBinderInternal = GetEnclosingBinderInternal(expression, GetAdjustedNodePosition(expression));
            CSharpSyntaxNode bindableSyntaxNode = GetBindableSyntaxNode(expression);
            BoundExpression boundExpression = GetLowerBoundNode(bindableSyntaxNode) as BoundExpression;
            if (enclosingBinderInternal == null || boundExpression == null)
            {
                return Conversion.NoConversion;
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return enclosingBinderInternal.Conversions.ClassifyConversionFromExpression(boundExpression, destination2, ref useSiteInfo);
        }

        internal override Conversion ClassifyConversionForCast(ExpressionSyntax expression, TypeSymbol destination)
        {
            CheckSyntaxNode(expression);
            if ((object)destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            Binder enclosingBinderInternal = GetEnclosingBinderInternal(expression, GetAdjustedNodePosition(expression));
            CSharpSyntaxNode bindableSyntaxNode = GetBindableSyntaxNode(expression);
            BoundExpression boundExpression = GetLowerBoundNode(bindableSyntaxNode) as BoundExpression;
            if (enclosingBinderInternal == null || boundExpression == null)
            {
                return Conversion.NoConversion;
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return enclosingBinderInternal.Conversions.ClassifyConversionFromExpression(boundExpression, destination, ref useSiteInfo, forCast: true);
        }

        internal virtual BoundNode GetBoundRoot()
        {
            return GetUpperBoundNode(GetBindableSyntaxNode(Root));
        }

        internal BoundNode GetUpperBoundNode(CSharpSyntaxNode node, bool promoteToBindable = false)
        {
            if (promoteToBindable)
            {
                node = GetBindableSyntaxNode(node);
            }
            ImmutableArray<BoundNode> boundNodes = GetBoundNodes(node);
            if (boundNodes.Length == 0)
            {
                return null;
            }
            return boundNodes[0];
        }

        internal BoundNode GetLowerBoundNode(CSharpSyntaxNode node)
        {
            ImmutableArray<BoundNode> boundNodes = GetBoundNodes(node);
            if (boundNodes.Length == 0)
            {
                return null;
            }
            return GetLowerBoundNode(boundNodes);
        }

        private static BoundNode GetLowerBoundNode(ImmutableArray<BoundNode> boundNodes)
        {
            return boundNodes[boundNodes.Length - 1];
        }

        public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException();
        }

        public override INamespaceSymbol GetDeclaredSymbol(NamespaceDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override INamedTypeSymbol GetDeclaredSymbol(BaseTypeDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override INamedTypeSymbol GetDeclaredSymbol(DelegateDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override ISymbol GetDeclaredSymbol(LocalFunctionStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredLocalFunction(declarationSyntax).GetPublicSymbol();
        }

        public override ISymbol GetDeclaredSymbol(MemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IMethodSymbol GetDeclaredSymbol(CompilationUnitSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IMethodSymbol GetDeclaredSymbol(BaseMethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override ISymbol GetDeclaredSymbol(BasePropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IPropertySymbol GetDeclaredSymbol(PropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IPropertySymbol GetDeclaredSymbol(IndexerDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IEventSymbol GetDeclaredSymbol(EventDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IMethodSymbol GetDeclaredSymbol(AccessorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IMethodSymbol GetDeclaredSymbol(ArrowExpressionClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override ISymbol GetDeclaredSymbol(VariableDeclaratorSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredLocal(declarationSyntax, declarationSyntax.Identifier).GetPublicSymbol();
        }

        public override ISymbol GetDeclaredSymbol(SingleVariableDesignationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetDeclaredLocal(declarationSyntax, declarationSyntax.Identifier).GetPublicSymbol();
        }

        private LocalSymbol GetDeclaredLocal(CSharpSyntaxNode declarationSyntax, SyntaxToken declaredIdentifier)
        {
            for (Binder binder = GetEnclosingBinder(GetAdjustedNodePosition(declarationSyntax)); binder != null; binder = binder.Next)
            {
                ImmutableArray<LocalSymbol>.Enumerator enumerator = binder.Locals.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalSymbol current = enumerator.Current;
                    if (current.IdentifierToken == declaredIdentifier)
                    {
                        return GetAdjustedLocalSymbol((SourceLocalSymbol)current);
                    }
                }
            }
            return null;
        }

        internal override LocalSymbol GetAdjustedLocalSymbol(SourceLocalSymbol local)
        {
            return GetRemappedSymbol((LocalSymbol)local);
        }

        private LocalFunctionSymbol GetDeclaredLocalFunction(LocalFunctionStatementSyntax declarationSyntax)
        {
            LocalFunctionSymbol declaredLocalFunction = GetDeclaredLocalFunction(GetEnclosingBinder(GetAdjustedNodePosition(declarationSyntax)), declarationSyntax.Identifier);
            return GetRemappedSymbol(declaredLocalFunction);
        }

        private T GetRemappedSymbol<T>(T originalSymbol) where T : Symbol
        {
            EnsureNullabilityAnalysisPerformedIfNecessary();
            if (_lazyRemappedSymbols == null)
            {
                return originalSymbol;
            }
            if (_lazyRemappedSymbols.TryGetValue(originalSymbol, out var value))
            {
                return (T)value;
            }
            return originalSymbol;
        }

        private static LocalFunctionSymbol GetDeclaredLocalFunction(Binder enclosingBinder, SyntaxToken declaredIdentifier)
        {
            for (Binder binder = enclosingBinder; binder != null; binder = binder.Next)
            {
                ImmutableArray<LocalFunctionSymbol>.Enumerator enumerator = binder.LocalFunctions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalFunctionSymbol current = enumerator.Current;
                    if (current.NameToken == declaredIdentifier)
                    {
                        return current;
                    }
                }
            }
            return null;
        }

        public override ILabelSymbol GetDeclaredSymbol(LabeledStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            Binder binder = GetEnclosingBinder(GetAdjustedNodePosition(declarationSyntax));
            while (binder != null && !binder.IsLabelsScopeBinder)
            {
                binder = binder.Next;
            }
            if (binder != null)
            {
                ImmutableArray<LabelSymbol>.Enumerator enumerator = binder.Labels.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LabelSymbol current = enumerator.Current;
                    if (current.IdentifierNodeOrToken.IsToken && current.IdentifierNodeOrToken.AsToken() == declarationSyntax.Identifier)
                    {
                        return current.GetPublicSymbol();
                    }
                }
            }
            return null;
        }

        public override ILabelSymbol GetDeclaredSymbol(SwitchLabelSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            Binder binder = GetEnclosingBinder(GetAdjustedNodePosition(declarationSyntax));
            while (binder != null && !(binder is SwitchBinder))
            {
                binder = binder.Next;
            }
            if (binder != null)
            {
                ImmutableArray<LabelSymbol>.Enumerator enumerator = binder.Labels.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LabelSymbol current = enumerator.Current;
                    if (current.IdentifierNodeOrToken.IsNode && current.IdentifierNodeOrToken.AsNode() == declarationSyntax)
                    {
                        return current.GetPublicSymbol();
                    }
                }
            }
            return null;
        }

        public override IAliasSymbol GetDeclaredSymbol(UsingDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IAliasSymbol GetDeclaredSymbol(ExternAliasDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IParameterSymbol GetDeclaredSymbol(ParameterSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            return GetLambdaOrLocalFunctionParameterSymbol(declarationSyntax, cancellationToken).GetPublicSymbol();
        }

        internal override ImmutableArray<ISymbol> GetDeclaredSymbols(BaseFieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ImmutableArray.Create<ISymbol>();
        }

        private ParameterSymbol GetLambdaOrLocalFunctionParameterSymbol(ParameterSyntax parameter, CancellationToken cancellationToken)
        {
            if (parameter.Parent is SimpleLambdaExpressionSyntax lambda)
            {
                return GetLambdaParameterSymbol(parameter, lambda, cancellationToken);
            }
            if (!(parameter.Parent is ParameterListSyntax parameterListSyntax) || parameterListSyntax.Parent == null)
            {
                return null;
            }
            if (parameterListSyntax.Parent.IsAnonymousFunction())
            {
                return GetLambdaParameterSymbol(parameter, (ExpressionSyntax)parameterListSyntax.Parent, cancellationToken);
            }
            if (parameterListSyntax.Parent!.Kind() == SyntaxKind.LocalFunctionStatement)
            {
                MethodSymbol symbol = GetDeclaredSymbol((LocalFunctionStatementSyntax)parameterListSyntax.Parent, cancellationToken).GetSymbol<MethodSymbol>();
                if ((object)symbol != null)
                {
                    return GetParameterSymbol(symbol.Parameters, parameter, cancellationToken);
                }
            }
            return null;
        }

        private ParameterSymbol GetLambdaParameterSymbol(ParameterSyntax parameter, ExpressionSyntax lambda, CancellationToken cancellationToken)
        {
            SymbolInfo symbolInfo = GetSymbolInfo(lambda, cancellationToken);
            LambdaSymbol symbol;
            if (symbolInfo.Symbol != null)
            {
                symbol = symbolInfo.Symbol.GetSymbol<LambdaSymbol>();
            }
            else
            {
                if (symbolInfo.CandidateSymbols.Length != 1)
                {
                    return null;
                }
                symbol = symbolInfo.CandidateSymbols.Single().GetSymbol<LambdaSymbol>();
            }
            return GetParameterSymbol(symbol.Parameters, parameter, cancellationToken);
        }

        public override ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(JoinIntoClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetBoundQueryClause(node)?.DefinedSymbol.GetPublicSymbol();
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(QueryClauseSyntax queryClause, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetBoundQueryClause(queryClause)?.DefinedSymbol.GetPublicSymbol();
        }

        public override IRangeVariableSymbol GetDeclaredSymbol(QueryContinuationSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetBoundQueryClause(node)?.DefinedSymbol.GetPublicSymbol();
        }

        public override AwaitExpressionInfo GetAwaitExpressionInfo(AwaitExpressionSyntax node)
        {
            if (node.Kind() != SyntaxKind.AwaitExpression)
            {
                throw new ArgumentException("node.Kind==" + node.Kind());
            }
            BoundNode upperBoundNode = GetUpperBoundNode(node);
            BoundAwaitableInfo boundAwaitableInfo = (((upperBoundNode as BoundExpressionStatement)?.Expression ?? upperBoundNode) as BoundAwaitExpression)?.AwaitableInfo;
            if (boundAwaitableInfo == null)
            {
                return default(AwaitExpressionInfo);
            }
            return new AwaitExpressionInfo((IMethodSymbol)(boundAwaitableInfo.GetAwaiter?.ExpressionSymbol.GetPublicSymbol()), boundAwaitableInfo.IsCompleted.GetPublicSymbol(), boundAwaitableInfo.GetResult.GetPublicSymbol(), boundAwaitableInfo.IsDynamic);
        }

        public override ForEachStatementInfo GetForEachStatementInfo(ForEachStatementSyntax node)
        {
            return GetForEachStatementInfo((CommonForEachStatementSyntax)node);
        }

        public override ForEachStatementInfo GetForEachStatementInfo(CommonForEachStatementSyntax node)
        {
            BoundForEachStatement boundForEachStatement = (BoundForEachStatement)GetUpperBoundNode(node);
            if (boundForEachStatement == null)
            {
                return default(ForEachStatementInfo);
            }
            ForEachEnumeratorInfo enumeratorInfoOpt = boundForEachStatement.EnumeratorInfoOpt;
            if (enumeratorInfoOpt == null)
            {
                return default(ForEachStatementInfo);
            }
            if (enumeratorInfoOpt.ElementType.IsPointerType())
            {
                return default(ForEachStatementInfo);
            }
            MethodSymbol symbol = null;
            if (enumeratorInfoOpt.NeedsDisposal)
            {
                MethodArgumentInfo patternDisposeInfo = enumeratorInfoOpt.PatternDisposeInfo;
                if ((object)patternDisposeInfo != null)
                {
                    MethodSymbol method = patternDisposeInfo.Method;
                    symbol = method;
                }
                else
                {
                    symbol = (enumeratorInfoOpt.IsAsync ? ((MethodSymbol)Compilation.GetWellKnownTypeMember(WellKnownMember.System_IAsyncDisposable__DisposeAsync)) : ((MethodSymbol)Compilation.GetSpecialTypeMember(SpecialMember.System_IDisposable__Dispose)));
                }
            }
            return new ForEachStatementInfo(enumeratorInfoOpt.IsAsync, enumeratorInfoOpt.GetEnumeratorInfo.Method.GetPublicSymbol(), enumeratorInfoOpt.MoveNextInfo.Method.GetPublicSymbol(), ((PropertySymbol)(enumeratorInfoOpt.CurrentPropertyGetter?.AssociatedSymbol)).GetPublicSymbol(), symbol.GetPublicSymbol(), enumeratorInfoOpt.ElementType.GetPublicSymbol(), boundForEachStatement.ElementConversion, enumeratorInfoOpt.CurrentConversion);
        }

        public override DeconstructionInfo GetDeconstructionInfo(AssignmentExpressionSyntax node)
        {
            if (!(GetUpperBoundNode(node) is BoundDeconstructionAssignmentOperator boundDeconstructionAssignmentOperator))
            {
                return default(DeconstructionInfo);
            }
            BoundConversion right = boundDeconstructionAssignmentOperator.Right;
            if (right == null)
            {
                return default(DeconstructionInfo);
            }
            return new DeconstructionInfo(right.Conversion);
        }

        public override DeconstructionInfo GetDeconstructionInfo(ForEachVariableStatementSyntax node)
        {
            BoundForEachStatement boundForEachStatement = (BoundForEachStatement)GetUpperBoundNode(node);
            if (boundForEachStatement == null)
            {
                return default(DeconstructionInfo);
            }
            BoundForEachDeconstructStep deconstructionOpt = boundForEachStatement.DeconstructionOpt;
            if (deconstructionOpt == null)
            {
                return default(DeconstructionInfo);
            }
            return new DeconstructionInfo(deconstructionOpt.DeconstructionAssignment.Right.Conversion);
        }

        private BoundQueryClause GetBoundQueryClause(CSharpSyntaxNode node)
        {
            CheckSyntaxNode(node);
            return GetLowerBoundNode(node) as BoundQueryClause;
        }

        private QueryClauseInfo GetQueryClauseInfo(BoundQueryClause bound)
        {
            if (bound == null)
            {
                return default(QueryClauseInfo);
            }
            SymbolInfo castInfo = ((bound.Cast == null) ? SymbolInfo.None : GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, bound.Cast, bound.Cast, null, null));
            SymbolInfo symbolInfoForQuery = GetSymbolInfoForQuery(bound);
            return new QueryClauseInfo(castInfo, symbolInfoForQuery);
        }

        private SymbolInfo GetSymbolInfoForQuery(BoundQueryClause bound)
        {
            if (!(bound?.Operation is BoundCall boundCall))
            {
                return SymbolInfo.None;
            }
            BoundExpression boundExpression = (boundCall.IsDelegateCall ? boundCall.ReceiverOpt : boundCall);
            return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, boundExpression, boundExpression, null, null);
        }

        private CSharpTypeInfo GetTypeInfoForQuery(BoundQueryClause bound)
        {
            if (bound != null)
            {
                return GetTypeInfoForNode(bound, bound, bound);
            }
            return CSharpTypeInfo.None;
        }

        public override QueryClauseInfo GetQueryClauseInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            BoundQueryClause boundQueryClause = GetBoundQueryClause(node);
            return GetQueryClauseInfo(boundQueryClause);
        }

        public override IPropertySymbol GetDeclaredSymbol(AnonymousObjectMemberDeclaratorSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax = (AnonymousObjectCreationExpressionSyntax)declaratorSyntax.Parent;
            if (anonymousObjectCreationExpressionSyntax == null)
            {
                return null;
            }
            if (!(GetLowerBoundNode(anonymousObjectCreationExpressionSyntax) is BoundAnonymousObjectCreationExpression boundAnonymousObjectCreationExpression))
            {
                return null;
            }
            if (!(boundAnonymousObjectCreationExpression.Type is NamedTypeSymbol type))
            {
                return null;
            }
            int index = anonymousObjectCreationExpressionSyntax.Initializers.IndexOf(declaratorSyntax);
            return AnonymousTypeManager.GetAnonymousTypeProperty(type, index).GetPublicSymbol();
        }

        public override INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            if (GetLowerBoundNode(declaratorSyntax) is BoundAnonymousObjectCreationExpression boundAnonymousObjectCreationExpression)
            {
                return (boundAnonymousObjectCreationExpression.Type as NamedTypeSymbol).GetPublicSymbol();
            }
            return null;
        }

        public override INamedTypeSymbol GetDeclaredSymbol(TupleExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            return GetTypeOfTupleLiteral(declaratorSyntax).GetPublicSymbol();
        }

        public override ISymbol GetDeclaredSymbol(ArgumentSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declaratorSyntax);
            if (!(declaratorSyntax?.Parent is TupleExpressionSyntax tupleExpressionSyntax))
            {
                return null;
            }
            NamedTypeSymbol typeOfTupleLiteral = GetTypeOfTupleLiteral(tupleExpressionSyntax);
            if ((object)typeOfTupleLiteral != null)
            {
                ImmutableArray<FieldSymbol> tupleElements = typeOfTupleLiteral.TupleElements;
                if (!tupleElements.IsDefault)
                {
                    int index = tupleExpressionSyntax.Arguments.IndexOf(declaratorSyntax);
                    return tupleElements[index].GetPublicSymbol();
                }
            }
            return null;
        }

        private NamedTypeSymbol GetTypeOfTupleLiteral(TupleExpressionSyntax declaratorSyntax)
        {
            return (GetLowerBoundNode(declaratorSyntax) as BoundTupleExpression)?.Type as NamedTypeSymbol;
        }

        internal override IOperation? GetOperationWorker(CSharpSyntaxNode node, CancellationToken cancellationToken)
        {
            CSharpSyntaxNode node2 = node;
            using (_nodeMapLock.DisposableRead())
            {
                if (_guardedIOperationNodeMap.Count != 0)
                {
                    return guardedGetIOperation();
                }
            }
            IOperation rootOperation = GetRootOperation();
            using (_nodeMapLock.DisposableWrite())
            {
                if (_guardedIOperationNodeMap.Count != 0)
                {
                    return guardedGetIOperation();
                }
                OperationMapBuilder.AddToMap(rootOperation, _guardedIOperationNodeMap);
                return guardedGetIOperation();
            }
            IOperation? guardedGetIOperation()
            {
                _nodeMapLock.AssertCanRead();
                if (!_guardedIOperationNodeMap.TryGetValue(node2, out var value))
                {
                    return null;
                }
                return value;
            }
        }

        private CSharpSyntaxNode GetBindingRootOrInitializer(CSharpSyntaxNode node)
        {
            CSharpSyntaxNode bindingRoot = GetBindingRoot(node);
            if (bindingRoot is ParameterSyntax parameterSyntax)
            {
                EqualsValueClauseSyntax? @default = parameterSyntax.Default;
                if (@default != null && @default!.FullSpan.Contains(node.Span))
                {
                    return parameterSyntax.Default;
                }
            }
            if (bindingRoot is VariableDeclaratorSyntax variableDeclaratorSyntax)
            {
                EqualsValueClauseSyntax? initializer = variableDeclaratorSyntax.Initializer;
                if (initializer != null && initializer!.FullSpan.Contains(node.Span))
                {
                    CSharpSyntaxNode? parent = variableDeclaratorSyntax.Parent;
                    if (parent == null || !parent!.Parent.IsKind(SyntaxKind.FieldDeclaration))
                    {
                        CSharpSyntaxNode? parent2 = variableDeclaratorSyntax.Parent;
                        if (parent2 == null || !parent2!.Parent.IsKind(SyntaxKind.EventFieldDeclaration))
                        {
                            goto IL_00ac;
                        }
                    }
                    return variableDeclaratorSyntax.Initializer;
                }
            }
            goto IL_00ac;
        IL_00ac:
            if (bindingRoot is EnumMemberDeclarationSyntax enumMemberDeclarationSyntax)
            {
                EqualsValueClauseSyntax? equalsValue = enumMemberDeclarationSyntax.EqualsValue;
                if (equalsValue != null && equalsValue!.FullSpan.Contains(node.Span))
                {
                    return enumMemberDeclarationSyntax.EqualsValue;
                }
            }
            if (bindingRoot is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                EqualsValueClauseSyntax? initializer2 = propertyDeclarationSyntax.Initializer;
                if (initializer2 != null && initializer2!.FullSpan.Contains(node.Span))
                {
                    return propertyDeclarationSyntax.Initializer;
                }
            }
            return bindingRoot;
        }

        private IOperation GetRootOperation()
        {
            BoundNode boundNode = GetBoundRoot();
            if (boundNode is BoundGlobalStatementInitializer boundGlobalStatementInitializer)
            {
                BoundStatement statement = boundGlobalStatementInitializer.Statement;
                boundNode = statement;
            }
            IOperation? operation = _operationFactory.Value.Create(boundNode);
            Operation.SetParentOperation(operation, null);
            return operation;
        }

        internal override SymbolInfo GetSymbolInfoWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            CSharpSemanticModel.ValidateSymbolInfoOptions(options);
            GetBoundNodes(node, out var _, out var lowestBoundNode, out var highestBoundNode, out var boundParent);
            return GetSymbolInfoForNode(options, lowestBoundNode, highestBoundNode, boundParent, null);
        }

        internal override CSharpTypeInfo GetTypeInfoWorker(CSharpSyntaxNode node, CancellationToken cancellationToken = default(CancellationToken))
        {
            GetBoundNodes(node, out var _, out var lowestBoundNode, out var highestBoundNode, out var boundParent);
            return GetTypeInfoForNode(lowestBoundNode, highestBoundNode, boundParent);
        }

        internal override ImmutableArray<Symbol> GetMemberGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            GetBoundNodes(node, out var _, out var lowestBoundNode, out var _, out var boundParent);
            return GetMemberGroupForNode(options, lowestBoundNode, boundParent, null);
        }

        internal override ImmutableArray<IPropertySymbol> GetIndexerGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            GetBoundNodes(node, out var _, out var lowestBoundNode, out var _, out var _);
            return GetIndexerGroupForNode(lowestBoundNode, null);
        }

        internal override Optional<object> GetConstantValueWorker(CSharpSyntaxNode node, CancellationToken cancellationToken)
        {
            CSharpSyntaxNode bindableSyntaxNode = GetBindableSyntaxNode(node);
            if (!(GetLowerBoundNode(bindableSyntaxNode) is BoundExpression boundExpression))
            {
                return default(Optional<object>);
            }
            ConstantValue constantValue = boundExpression.ConstantValue;
            if (!(constantValue == null) && !constantValue.IsBad)
            {
                return new Optional<object>(constantValue.Value);
            }
            return default(Optional<object>);
        }

        internal override SymbolInfo GetCollectionInitializerSymbolInfoWorker(InitializerExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (GetLowerBoundNode(collectionInitializer) is BoundCollectionInitializerExpression boundCollectionInitializerExpression)
            {
                BoundExpression boundExpression = boundCollectionInitializerExpression.Initializers[collectionInitializer.Expressions.IndexOf(node)];
                return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, boundExpression, boundExpression, null, null);
            }
            return SymbolInfo.None;
        }

        public override SymbolInfo GetSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            BoundQueryClause boundQueryClause = GetBoundQueryClause(node);
            return GetSymbolInfoForQuery(boundQueryClause);
        }

        public override SymbolInfo GetSymbolInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            BoundQueryClause boundQueryClause = GetBoundQueryClause(node);
            return GetSymbolInfoForQuery(boundQueryClause);
        }

        public override TypeInfo GetTypeInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            BoundQueryClause boundQueryClause = GetBoundQueryClause(node);
            return GetTypeInfoForQuery(boundQueryClause);
        }

        private void GetBoundNodes(CSharpSyntaxNode node, out CSharpSyntaxNode bindableNode, out BoundNode lowestBoundNode, out BoundNode highestBoundNode, out BoundNode boundParent)
        {
            bindableNode = GetBindableSyntaxNode(node);
            CSharpSyntaxNode bindableParentNode = GetBindableParentNode(bindableNode);
            if (bindableParentNode != null && bindableParentNode.Kind() == SyntaxKind.SimpleMemberAccessExpression && ((MemberAccessExpressionSyntax)bindableParentNode).Expression == bindableNode)
            {
                bindableParentNode = GetBindableParentNode(bindableParentNode);
            }
            boundParent = ((bindableParentNode == null) ? null : GetLowerBoundNode(bindableParentNode));
            lowestBoundNode = GetLowerBoundNode(bindableNode);
            highestBoundNode = GetUpperBoundNode(bindableNode);
        }

        private CSharpSyntaxNode GetInnermostLambdaOrQuery(CSharpSyntaxNode node, int position, bool allowStarting = false)
        {
            for (CSharpSyntaxNode cSharpSyntaxNode = node; cSharpSyntaxNode != Root; cSharpSyntaxNode = cSharpSyntaxNode.ParentOrStructuredTriviaParent)
            {
                if ((cSharpSyntaxNode.IsAnonymousFunction() || cSharpSyntaxNode.IsQuery()) && LookupPosition.IsInAnonymousFunctionOrQuery(position, cSharpSyntaxNode) && (allowStarting || cSharpSyntaxNode != node))
                {
                    return cSharpSyntaxNode;
                }
            }
            return null;
        }

        private void GuardedAddSynthesizedStatementToMap(StatementSyntax node, BoundStatement statement)
        {
            if (_lazyGuardedSynthesizedStatementsMap == null)
            {
                _lazyGuardedSynthesizedStatementsMap = new Dictionary<SyntaxNode, BoundStatement>();
            }
            _lazyGuardedSynthesizedStatementsMap.Add(node, statement);
        }

        private BoundStatement GuardedGetSynthesizedStatementFromMap(StatementSyntax node)
        {
            if (_lazyGuardedSynthesizedStatementsMap != null && _lazyGuardedSynthesizedStatementsMap.TryGetValue(node, out var value))
            {
                return value;
            }
            return null;
        }

        private ImmutableArray<BoundNode> GuardedGetBoundNodesFromMap(CSharpSyntaxNode node)
        {
            if (!_guardedBoundNodeMap.TryGetValue(node, out var value))
            {
                return default(ImmutableArray<BoundNode>);
            }
            return value;
        }

        internal ImmutableArray<BoundNode> TestOnlyTryGetBoundNodesFromMap(CSharpSyntaxNode node)
        {
            if (!_guardedBoundNodeMap.TryGetValue(node, out var value))
            {
                return default(ImmutableArray<BoundNode>);
            }
            return value;
        }

        private ImmutableArray<BoundNode> GuardedAddBoundTreeAndGetBoundNodeFromMap(CSharpSyntaxNode syntax, BoundNode bound)
        {
            bool flag = false;
            if (bound != null)
            {
                flag = _guardedBoundNodeMap.ContainsKey(bound.Syntax);
            }
            if (!flag)
            {
                NodeMapBuilder.AddToMap(bound, _guardedBoundNodeMap, SyntaxTree);
            }
            if (!_guardedBoundNodeMap.TryGetValue(syntax, out var value))
            {
                return default(ImmutableArray<BoundNode>);
            }
            return value;
        }

        protected void UnguardedAddBoundTreeForStandaloneSyntax(SyntaxNode syntax, BoundNode bound, NullableWalker.SnapshotManager manager = null, ImmutableDictionary<Symbol, Symbol> remappedSymbols = null)
        {
            using (_nodeMapLock.DisposableWrite())
            {
                GuardedAddBoundTreeForStandaloneSyntax(syntax, bound, manager, remappedSymbols);
            }
        }

        protected void GuardedAddBoundTreeForStandaloneSyntax(SyntaxNode syntax, BoundNode bound, NullableWalker.SnapshotManager manager = null, ImmutableDictionary<Symbol, Symbol> remappedSymbols = null)
        {
            bool flag = false;
            if (bound != null)
            {
                flag = _guardedBoundNodeMap.ContainsKey(bound.Syntax);
            }
            if (!flag)
            {
                if (syntax == _root || syntax is StatementSyntax)
                {
                    NodeMapBuilder.AddToMap(bound, _guardedBoundNodeMap, SyntaxTree);
                }
                else
                {
                    NodeMapBuilder.AddToMap(bound, _guardedBoundNodeMap, SyntaxTree, syntax);
                }
                if (manager != null)
                {
                    _lazySnapshotManager = manager;
                    _lazyRemappedSymbols = remappedSymbols;
                }
            }
        }

        private CSharpSyntaxNode GetBindingRoot(CSharpSyntaxNode node)
        {
            for (CSharpSyntaxNode cSharpSyntaxNode = node; cSharpSyntaxNode != Root; cSharpSyntaxNode = cSharpSyntaxNode.ParentOrStructuredTriviaParent)
            {
                if (cSharpSyntaxNode is StatementSyntax)
                {
                    return cSharpSyntaxNode;
                }
                switch (cSharpSyntaxNode.Kind())
                {
                    case SyntaxKind.BaseConstructorInitializer:
                    case SyntaxKind.ThisConstructorInitializer:
                    case SyntaxKind.PrimaryConstructorBaseType:
                        return cSharpSyntaxNode;
                    case SyntaxKind.ArrowExpressionClause:
                        if (cSharpSyntaxNode.Parent == null || cSharpSyntaxNode.Parent!.Kind() != SyntaxKind.LocalFunctionStatement)
                        {
                            return cSharpSyntaxNode;
                        }
                        break;
                }
            }
            return Root;
        }

        internal override Binder GetEnclosingBinderInternal(int position)
        {
            if (!Root.FullSpan.Contains(position))
            {
                return RootBinder;
            }
            CSharpSyntaxNode node = (CSharpSyntaxNode)Root.FindToken(position).Parent;
            return GetEnclosingBinderInternal(node, position);
        }

        private Binder GetEnclosingBinderInternal(CSharpSyntaxNode node, int position)
        {
            CSharpSyntaxNode innermostLambdaOrQuery = GetInnermostLambdaOrQuery(node, position, allowStarting: true);
            if (innermostLambdaOrQuery == null)
            {
                return GetEnclosingBinderInternalWithinRoot(node, position);
            }
            BoundNode boundInnerLambdaOrQuery = GetBoundLambdaOrQuery(innermostLambdaOrQuery);
            return GetEnclosingBinderInLambdaOrQuery(position, node, innermostLambdaOrQuery, ref boundInnerLambdaOrQuery);
        }

        private BoundNode GetBoundLambdaOrQuery(CSharpSyntaxNode lambdaOrQuery)
        {
            EnsureNullabilityAnalysisPerformedIfNecessary();
            ImmutableArray<BoundNode> boundNodes;
            using (_nodeMapLock.DisposableRead())
            {
                boundNodes = GuardedGetBoundNodesFromMap(lambdaOrQuery);
            }
            if (!boundNodes.IsDefaultOrEmpty)
            {
                return GetLowerBoundNode(boundNodes);
            }
            CSharpSyntaxNode bindingRoot = GetBindingRoot(lambdaOrQuery);
            CSharpSyntaxNode innermostLambdaOrQuery = GetInnermostLambdaOrQuery(lambdaOrQuery, lambdaOrQuery.SpanStart);
            BoundNode boundInnerLambdaOrQuery = null;
            CSharpSyntaxNode node;
            Binder next;
            if (innermostLambdaOrQuery == null)
            {
                node = bindingRoot;
                next = GetEnclosingBinderInternalWithinRoot(node, GetAdjustedNodePosition(node));
            }
            else
            {
                node = ((innermostLambdaOrQuery != bindingRoot && innermostLambdaOrQuery.Contains(bindingRoot)) ? bindingRoot : lambdaOrQuery);
                boundInnerLambdaOrQuery = GetBoundLambdaOrQuery(innermostLambdaOrQuery);
                using (_nodeMapLock.DisposableRead())
                {
                    boundNodes = GuardedGetBoundNodesFromMap(lambdaOrQuery);
                }
                if (!boundNodes.IsDefaultOrEmpty)
                {
                    return GetLowerBoundNode(boundNodes);
                }
                next = GetEnclosingBinderInLambdaOrQuery(GetAdjustedNodePosition(node), node, innermostLambdaOrQuery, ref boundInnerLambdaOrQuery);
            }
            Binder binder = new IncrementalBinder(this, next);
            using (_nodeMapLock.DisposableWrite())
            {
                BoundNode bound = Bind(binder, node, BindingDiagnosticBag.Discarded);
                boundNodes = GuardedAddBoundTreeAndGetBoundNodeFromMap(lambdaOrQuery, bound);
            }
            if (!boundNodes.IsDefaultOrEmpty)
            {
                return GetLowerBoundNode(boundNodes);
            }
            next = ((innermostLambdaOrQuery != null) ? GetEnclosingBinderInLambdaOrQuery(GetAdjustedNodePosition(lambdaOrQuery), lambdaOrQuery, innermostLambdaOrQuery, ref boundInnerLambdaOrQuery) : GetEnclosingBinderInternalWithinRoot(lambdaOrQuery, GetAdjustedNodePosition(lambdaOrQuery)));
            binder = new IncrementalBinder(this, next);
            using (_nodeMapLock.DisposableWrite())
            {
                BoundNode boundNode = Bind(binder, lambdaOrQuery, BindingDiagnosticBag.Discarded);
                if (!IsNullableAnalysisEnabled() && Compilation.IsNullableAnalysisEnabledAlways)
                {
                    AnalyzeBoundNodeNullability(boundNode, binder, new DiagnosticBag(), createSnapshots: false);
                }
                boundNodes = GuardedAddBoundTreeAndGetBoundNodeFromMap(lambdaOrQuery, boundNode);
            }
            return GetLowerBoundNode(boundNodes);
        }

        private Binder GetEnclosingBinderInLambdaOrQuery(int position, CSharpSyntaxNode node, CSharpSyntaxNode innerLambdaOrQuery, ref BoundNode boundInnerLambdaOrQuery)
        {
            Binder binder;
            switch (boundInnerLambdaOrQuery.Kind)
            {
                case BoundKind.UnboundLambda:
                    boundInnerLambdaOrQuery = ((UnboundLambda)boundInnerLambdaOrQuery).BindForErrorRecovery();
                    goto case BoundKind.Lambda;
                case BoundKind.Lambda:
                    binder = GetLambdaEnclosingBinder(position, node, innerLambdaOrQuery, ((BoundLambda)boundInnerLambdaOrQuery).Binder);
                    break;
                case BoundKind.QueryClause:
                    binder = GetQueryEnclosingBinder(position, node, (BoundQueryClause)boundInnerLambdaOrQuery);
                    break;
                default:
                    return GetEnclosingBinderInternalWithinRoot(node, position);
            }
            return binder.WithAdditionalFlags(GetSemanticModelBinderFlags());
        }

        private static Binder GetQueryEnclosingBinder(int position, CSharpSyntaxNode startingNode, BoundQueryClause queryClause)
        {
            BoundExpression boundExpression = queryClause;
            do
            {
                switch (boundExpression.Kind)
                {
                    case BoundKind.QueryClause:
                        queryClause = (BoundQueryClause)boundExpression;
                        boundExpression = GetQueryClauseValue(queryClause);
                        continue;
                    case BoundKind.Call:
                        {
                            BoundCall boundCall = (BoundCall)boundExpression;
                            boundExpression = GetContainingArgument(boundCall.Arguments, position);
                            if (boundExpression != null)
                            {
                                continue;
                            }
                            BoundExpression receiverOpt = boundCall.ReceiverOpt;
                            while (receiverOpt != null && receiverOpt.Kind == BoundKind.MethodGroup)
                            {
                                receiverOpt = ((BoundMethodGroup)receiverOpt).ReceiverOpt;
                            }
                            if (receiverOpt != null)
                            {
                                boundExpression = GetContainingExprOrQueryClause(receiverOpt, position);
                                if (boundExpression != null)
                                {
                                    continue;
                                }
                            }
                            boundExpression = boundCall.Arguments.LastOrDefault();
                            continue;
                        }
                    case BoundKind.Conversion:
                        boundExpression = ((BoundConversion)boundExpression).Operand;
                        continue;
                    case BoundKind.UnboundLambda:
                        {
                            UnboundLambda unboundLambda = (UnboundLambda)boundExpression;
                            return GetEnclosingBinderInternalWithinRoot(AdjustStartingNodeAccordingToNewRoot(startingNode, unboundLambda.Syntax), position, unboundLambda.BindForErrorRecovery().Binder, unboundLambda.Syntax);
                        }
                    case BoundKind.Lambda:
                        {
                            BoundLambda boundLambda = (BoundLambda)boundExpression;
                            return GetEnclosingBinderInternalWithinRoot(AdjustStartingNodeAccordingToNewRoot(startingNode, boundLambda.Body.Syntax), position, boundLambda.Binder, boundLambda.Body.Syntax);
                        }
                }
                break;
            }
            while (boundExpression != null);
            return GetEnclosingBinderInternalWithinRoot(AdjustStartingNodeAccordingToNewRoot(startingNode, queryClause.Syntax), position, queryClause.Binder, queryClause.Syntax);
        }

        private static BoundExpression GetContainingArgument(ImmutableArray<BoundExpression> arguments, int position)
        {
            BoundExpression boundExpression = null;
            TextSpan textSpan = default(TextSpan);
            ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression containingExprOrQueryClause = GetContainingExprOrQueryClause(enumerator.Current, position);
                if (containingExprOrQueryClause != null)
                {
                    TextSpan fullSpan = containingExprOrQueryClause.Syntax.FullSpan;
                    if (boundExpression == null || textSpan.Contains(fullSpan))
                    {
                        boundExpression = containingExprOrQueryClause;
                        textSpan = fullSpan;
                    }
                }
            }
            return boundExpression;
        }

        private static BoundExpression GetContainingExprOrQueryClause(BoundExpression expr, int position)
        {
            if (expr.Kind == BoundKind.QueryClause)
            {
                BoundExpression queryClauseValue = GetQueryClauseValue((BoundQueryClause)expr);
                if (queryClauseValue.Syntax.FullSpan.Contains(position))
                {
                    return queryClauseValue;
                }
            }
            if (expr.Syntax.FullSpan.Contains(position))
            {
                return expr;
            }
            return null;
        }

        private static BoundExpression GetQueryClauseValue(BoundQueryClause queryClause)
        {
            return queryClause.UnoptimizedForm ?? queryClause.Value;
        }

        private static SyntaxNode AdjustStartingNodeAccordingToNewRoot(SyntaxNode startingNode, SyntaxNode root)
        {
            SyntaxNode syntaxNode = (startingNode.Contains(root) ? root : startingNode);
            if (syntaxNode != root && !root.Contains(syntaxNode))
            {
                syntaxNode = root;
            }
            return syntaxNode;
        }

        private static Binder GetLambdaEnclosingBinder(int position, CSharpSyntaxNode startingNode, CSharpSyntaxNode containingLambda, Binder lambdaBinder)
        {
            return GetEnclosingBinderInternalWithinRoot(startingNode, position, lambdaBinder, containingLambda);
        }

        protected void EnsureNullabilityAnalysisPerformedIfNecessary()
        {
            bool flag = IsNullableAnalysisEnabled();
            if ((!flag && !Compilation.IsNullableAnalysisEnabledAlways) || _lazySnapshotManager != null)
            {
                return;
            }
            CSharpSyntaxNode bindableRoot = GetBindableSyntaxNode(Root);
            ImmutableDictionary<Symbol, Symbol> remappedSymbols;
            BoundNode boundRoot;
            Binder binder2;
            using (ReaderWriterLockSlimExtensions.UpgradeableReadLockExiter upgradeableReadLockExiter = _nodeMapLock.DisposableUpgradeableRead())
            {
                if (_guardedBoundNodeMap.Count > 0)
                {
                    return;
                }
                upgradeableReadLockExiter.EnterWrite();
                remappedSymbols = _parentRemappedSymbolsOpt;
                boundRoot = bind(bindableRoot, out binder2);
                if (IsSpeculativeSemanticModel)
                {
                    if (_parentSnapshotManagerOpt == null || !flag)
                    {
                        rewriteAndCache();
                        return;
                    }
                    boundRoot = NullableWalker.AnalyzeAndRewriteSpeculation(_speculatedPosition, boundRoot, binder2, _parentSnapshotManagerOpt, out var newSnapshots, ref remappedSymbols);
                    GuardedAddBoundTreeForStandaloneSyntax(bindableRoot, boundRoot, newSnapshots, remappedSymbols);
                }
                else
                {
                    rewriteAndCache();
                }
            }
            BoundNode bind(CSharpSyntaxNode root, out Binder binder)
            {
                if (root is CompilationUnitSyntax)
                {
                    binder = RootBinder.GetBinder(root);
                }
                else
                {
                    binder = GetEnclosingBinder(GetAdjustedNodePosition(root));
                }
                return Bind(binder, root, BindingDiagnosticBag.Discarded);
            }
            void rewriteAndCache()
            {
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                boundRoot = RewriteNullableBoundNodesWithSnapshots(boundRoot, binder2, instance, createSnapshots: true, out NullableWalker.SnapshotManager snapshotManager, ref remappedSymbols);
                instance.Free();
                GuardedAddBoundTreeForStandaloneSyntax(bindableRoot, boundRoot, snapshotManager, remappedSymbols);
            }
        }

        protected abstract BoundNode RewriteNullableBoundNodesWithSnapshots(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots, out NullableWalker.SnapshotManager? snapshotManager, ref ImmutableDictionary<Symbol, Symbol>? remappedSymbols);

        protected abstract void AnalyzeBoundNodeNullability(BoundNode boundRoot, Binder binder, DiagnosticBag diagnostics, bool createSnapshots);

        protected abstract bool IsNullableAnalysisEnabled();

        internal ImmutableArray<BoundNode> GetBoundNodes(CSharpSyntaxNode node)
        {
            if (node == null)
            {
                node = GetBindableSyntaxNode(Root);
            }
            EnsureNullabilityAnalysisPerformedIfNecessary();
            ImmutableArray<BoundNode> result;
            using (_nodeMapLock.DisposableRead())
            {
                result = GuardedGetBoundNodesFromMap(node);
            }
            if (!result.IsDefaultOrEmpty)
            {
                return result;
            }
            CSharpSyntaxNode bindingRoot = GetBindingRoot(node);
            Binder enclosingBinder = GetEnclosingBinder(GetAdjustedNodePosition(bindingRoot));
            Binder binder = new IncrementalBinder(this, enclosingBinder);
            using (_nodeMapLock.DisposableWrite())
            {
                BoundNode bound = Bind(binder, bindingRoot, BindingDiagnosticBag.Discarded);
                result = GuardedAddBoundTreeAndGetBoundNodeFromMap(node, bound);
            }
            if (!result.IsDefaultOrEmpty)
            {
                return result;
            }
            Binder enclosingBinder2 = GetEnclosingBinder(GetAdjustedNodePosition(node));
            binder = new IncrementalBinder(this, enclosingBinder2);
            using (_nodeMapLock.DisposableRead())
            {
                result = GuardedGetBoundNodesFromMap(node);
            }
            if (result.IsDefaultOrEmpty)
            {
                using (_nodeMapLock.DisposableWrite())
                {
                    BoundNode bound2 = Bind(binder, node, BindingDiagnosticBag.Discarded);
                    GuardedAddBoundTreeForStandaloneSyntax(node, bound2);
                    result = GuardedGetBoundNodesFromMap(node);
                }
                if (!result.IsDefaultOrEmpty)
                {
                    return result;
                }
                return ImmutableArray<BoundNode>.Empty;
            }
            return result;
        }

        protected internal virtual CSharpSyntaxNode GetBindableSyntaxNode(CSharpSyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.GlobalStatement:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.Subpattern:
                case SyntaxKind.InitAccessorDeclaration:
                    return node;
                case SyntaxKind.PositionalPatternClause:
                    return node.Parent;
            }
            while (true)
            {
                if (!(node is ParenthesizedExpressionSyntax parenthesizedExpressionSyntax))
                {
                    if (!(node is CheckedExpressionSyntax checkedExpressionSyntax))
                    {
                        if (node is PostfixUnaryExpressionSyntax postfixUnaryExpressionSyntax)
                        {
                            if (node.RawKind != 9054)
                            {
                                break;
                            }
                            node = postfixUnaryExpressionSyntax.Operand;
                        }
                        else if (!(node is UnsafeStatementSyntax unsafeStatementSyntax))
                        {
                            if (!(node is CheckedStatementSyntax checkedStatementSyntax))
                            {
                                break;
                            }
                            node = checkedStatementSyntax.Block;
                        }
                        else
                        {
                            node = unsafeStatementSyntax.Block;
                        }
                    }
                    else
                    {
                        node = checkedExpressionSyntax.Expression;
                    }
                }
                else
                {
                    node = parenthesizedExpressionSyntax.Expression;
                }
            }
            CSharpSyntaxNode parent = node.Parent;
            if (parent != null && node != Root)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.GenericName:
                        {
                            CSharpSyntaxNode standaloneNode = SyntaxFactory.GetStandaloneNode(node);
                            if (standaloneNode != node)
                            {
                                return GetBindableSyntaxNode(standaloneNode);
                            }
                            break;
                        }
                    case SyntaxKind.AnonymousObjectMemberDeclarator:
                        return GetBindableSyntaxNode(parent);
                    case SyntaxKind.VariableDeclarator:
                        {
                            CSharpSyntaxNode parent2 = parent.Parent;
                            if (parent2 != null && parent2.Kind() == SyntaxKind.LocalDeclarationStatement && ((VariableDeclarationSyntax)parent).Variables.Count == 1)
                            {
                                return GetBindableSyntaxNode(parent);
                            }
                            break;
                        }
                    default:
                        if ((node is QueryExpressionSyntax && parent is QueryContinuationSyntax) || (!(node is ExpressionSyntax) && !(node is StatementSyntax) && !(node is SelectOrGroupClauseSyntax) && !(node is QueryClauseSyntax) && !(node is OrderingSyntax) && !(node is JoinIntoClauseSyntax) && !(node is QueryContinuationSyntax) && !(node is ConstructorInitializerSyntax) && !(node is PrimaryConstructorBaseTypeSyntax) && !(node is ArrowExpressionClauseSyntax) && !(node is PatternSyntax)))
                        {
                            return GetBindableSyntaxNode(parent);
                        }
                        break;
                }
            }
            return node;
        }

        protected CSharpSyntaxNode GetBindableParentNode(CSharpSyntaxNode node)
        {
            if (!(node is ExpressionSyntax))
            {
                return null;
            }
            CSharpSyntaxNode cSharpSyntaxNode = node.Parent;
            if (cSharpSyntaxNode == null)
            {
                if (IsSpeculativeSemanticModel && Root == node)
                {
                    return null;
                }
                throw new ArgumentException("The parent of node must not be null unless this is a speculative semantic model.", "node");
            }
            while (true)
            {
                SyntaxKind syntaxKind = cSharpSyntaxNode.Kind();
                if (syntaxKind != SyntaxKind.ParenthesizedExpression && syntaxKind - 9050 > SyntaxKind.List)
                {
                    break;
                }
                CSharpSyntaxNode parent = cSharpSyntaxNode.Parent;
                if (parent != null)
                {
                    cSharpSyntaxNode = parent;
                }
            }
            CSharpSyntaxNode cSharpSyntaxNode2 = GetBindableSyntaxNode(cSharpSyntaxNode);
            if (cSharpSyntaxNode2.Kind() == SyntaxKind.SimpleMemberAccessExpression && cSharpSyntaxNode2.Parent!.Kind() == SyntaxKind.InvocationExpression)
            {
                cSharpSyntaxNode2 = cSharpSyntaxNode2.Parent;
            }
            else if (cSharpSyntaxNode2.Kind() == SyntaxKind.ArrayType)
            {
                cSharpSyntaxNode2 = SyntaxFactory.GetStandaloneExpression((ArrayTypeSyntax)cSharpSyntaxNode2);
            }
            return cSharpSyntaxNode2;
        }

        internal override Symbol RemapSymbolIfNecessaryCore(Symbol symbol)
        {
            EnsureNullabilityAnalysisPerformedIfNecessary();
            if (_lazyRemappedSymbols == null)
            {
                return symbol;
            }
            if (_lazyRemappedSymbols.TryGetValue(symbol, out var value))
            {
                return value;
            }
            return symbol;
        }

        public sealed override Func<SyntaxNode, bool> GetSyntaxNodesToAnalyzeFilter(SyntaxNode declaredNode, ISymbol declaredSymbol)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
