// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// A base class for components that instrument various portions of executable code.
    /// It provides a set of APIs that are called by <see cref="LocalRewriter"/> to instrument
    /// specific portions of the code. These APIs have at least two parameters:
    ///     - original bound node produced by the <see cref="Binder"/> for the relevant portion of the code;
    ///     - rewritten bound node created by the <see cref="LocalRewriter"/> for the original node.
    /// The APIs are expected to return new state of the rewritten node, after they apply appropriate
    /// modifications, if any.
    /// 
    /// The base class provides default implementation for all APIs, which simply returns the rewritten node. 
    /// </summary>
    internal class Instrumenter
    {
        /// <summary>
        /// The singleton NoOp instrumenter, can be used to terminate the chain of <see cref="CompoundInstrumenter"/>s.
        /// </summary>
        public static readonly Instrumenter NoOp = new();

        public Instrumenter()
        {
        }

#pragma warning disable IDE0060
        private static BoundStatement InstrumentStatement(BoundStatement original, BoundStatement rewritten)
#pragma warning restore IDE0060
        {
            return rewritten;
        }

        public virtual BoundStatement InstrumentNoOpStatement(BoundNoOpStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentYieldBreakStatement(BoundYieldBreakStatement original, BoundStatement rewritten)
        {
            return rewritten;
        }

        public virtual BoundStatement InstrumentYieldReturnStatement(BoundYieldReturnStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        /// <summary>
        /// Return a node that is associated with open brace of the block. Ok to return null.
        /// </summary>
        public virtual BoundStatement? CreateBlockPrologue(BoundBlock original, out Symbols.LocalSymbol? synthesizedLocal)
        {
            synthesizedLocal = null;
            return null;
        }

        /// <summary>
        /// Return a node that is associated with close brace of the block. Ok to return null.
        /// </summary>
        public virtual BoundStatement? CreateBlockEpilogue(BoundBlock original)
        {
            return null;
        }

        public virtual BoundStatement InstrumentThrowStatement(BoundThrowStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentContinueStatement(BoundContinueStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentGotoStatement(BoundGotoStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentExpressionStatement(BoundExpressionStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentFieldOrPropertyInitializer(BoundStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentBreakStatement(BoundBreakStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundExpression InstrumentDoStatementCondition(BoundDoStatement original, BoundExpression rewrittenCondition, SyntheticBoundNodeFactory factory)
        {
            return rewrittenCondition;
        }

        public virtual BoundExpression InstrumentWhileStatementCondition(BoundWhileStatement original, BoundExpression rewrittenCondition, SyntheticBoundNodeFactory factory)
        {
            return rewrittenCondition;
        }

        public virtual BoundStatement InstrumentDoStatementConditionalGotoStart(BoundDoStatement original, BoundStatement ifConditionGotoStart)
        {
            return ifConditionGotoStart;
        }

        public virtual BoundStatement InstrumentWhileStatementConditionalGotoStartOrBreak(BoundWhileStatement original, BoundStatement ifConditionGotoStart)
        {
            return ifConditionGotoStart;
        }

        [return: NotNullIfNotNull(nameof(collectionVarDecl))]
        public virtual BoundStatement? InstrumentForEachStatementCollectionVarDeclaration(BoundForEachStatement original, BoundStatement? collectionVarDecl)
        {
            return collectionVarDecl;
        }

        public virtual BoundStatement InstrumentForEachStatement(BoundForEachStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentForEachStatementIterationVarDeclaration(BoundForEachStatement original, BoundStatement iterationVarDecl)
        {
            return iterationVarDecl;
        }

        public virtual BoundStatement InstrumentForEachStatementDeconstructionVariablesDeclaration(BoundForEachStatement original, BoundStatement iterationVarDecl)
        {
            return iterationVarDecl;
        }

        public virtual BoundStatement InstrumentForEachStatementConditionalGotoStart(BoundForEachStatement original, BoundStatement branchBack)
        {
            return branchBack;
        }

        public virtual BoundStatement InstrumentForStatementConditionalGotoStartOrBreak(BoundForStatement original, BoundStatement branchBack)
        {
            return branchBack;
        }

        public virtual BoundExpression InstrumentForStatementCondition(BoundForStatement original, BoundExpression rewrittenCondition, SyntheticBoundNodeFactory factory)
        {
            return rewrittenCondition;
        }

        public virtual BoundStatement InstrumentIfStatement(BoundIfStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundExpression InstrumentIfStatementCondition(BoundIfStatement original, BoundExpression rewrittenCondition, SyntheticBoundNodeFactory factory)
        {
            return rewrittenCondition;
        }

        public virtual BoundStatement InstrumentLabelStatement(BoundLabeledStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentLocalInitialization(BoundLocalDeclaration original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        public virtual BoundStatement InstrumentLockTargetCapture(BoundLockStatement original, BoundStatement lockTargetCapture)
        {
            return lockTargetCapture;
        }

        public virtual BoundStatement InstrumentReturnStatement(BoundReturnStatement original, BoundStatement rewritten)
        {
            return rewritten;
        }

        public virtual BoundStatement InstrumentSwitchStatement(BoundSwitchStatement original, BoundStatement rewritten)
        {
            return InstrumentStatement(original, rewritten);
        }

        /// <summary>
        /// Instrument a switch case when clause, which is translated to a conditional branch to the body of the case block.
        /// </summary>
        /// <param name="original">the bound expression of the when clause</param>
        /// <param name="ifConditionGotoBody">the lowered conditional branch into the case block</param>
        public virtual BoundStatement InstrumentSwitchWhenClauseConditionalGotoBody(BoundExpression original, BoundStatement ifConditionGotoBody)
        {
            return ifConditionGotoBody;
        }

        public virtual BoundStatement InstrumentUsingTargetCapture(BoundUsingStatement original, BoundStatement usingTargetCapture)
        {
            return usingTargetCapture;
        }

        public virtual BoundExpression InstrumentCatchClauseFilter(BoundCatchBlock original, BoundExpression rewrittenFilter, SyntheticBoundNodeFactory factory)
        {
            return rewrittenFilter;
        }

        public virtual BoundExpression InstrumentSwitchStatementExpression(BoundStatement original, BoundExpression rewrittenExpression, SyntheticBoundNodeFactory factory)
        {
            return rewrittenExpression;
        }

        /// <summary>
        /// Instrument the expression of a switch arm of a switch expression.
        /// </summary>
        public virtual BoundExpression InstrumentSwitchExpressionArmExpression(BoundExpression original, BoundExpression rewrittenExpression, SyntheticBoundNodeFactory factory)
        {
            return rewrittenExpression;
        }

        public virtual BoundStatement InstrumentSwitchBindCasePatternVariables(BoundStatement bindings)
        {
            return bindings;
        }
    }
}
