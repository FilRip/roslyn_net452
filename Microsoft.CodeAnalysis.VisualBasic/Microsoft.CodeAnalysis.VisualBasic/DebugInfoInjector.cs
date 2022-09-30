using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DebugInfoInjector : CompoundInstrumenter
	{
		public static readonly DebugInfoInjector Singleton = new DebugInfoInjector(Instrumenter.NoOp);

		public DebugInfoInjector(Instrumenter previous)
			: base(previous)
		{
		}

		private static BoundStatement MarkStatementWithSequencePoint(BoundStatement original, BoundStatement rewritten)
		{
			return new BoundSequencePoint(original.Syntax, rewritten);
		}

		public override BoundStatement InstrumentExpressionStatement(BoundExpressionStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentExpressionStatement(original, rewritten));
		}

		public override BoundStatement InstrumentStopStatement(BoundStopStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentStopStatement(original, rewritten));
		}

		public override BoundStatement InstrumentEndStatement(BoundEndStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentEndStatement(original, rewritten));
		}

		public override BoundStatement InstrumentContinueStatement(BoundContinueStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentContinueStatement(original, rewritten));
		}

		public override BoundStatement InstrumentExitStatement(BoundExitStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentExitStatement(original, rewritten));
		}

		public override BoundStatement InstrumentGotoStatement(BoundGotoStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentGotoStatement(original, rewritten));
		}

		public override BoundStatement InstrumentLabelStatement(BoundLabelStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentLabelStatement(original, rewritten));
		}

		public override BoundStatement InstrumentRaiseEventStatement(BoundRaiseEventStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentRaiseEventStatement(original, rewritten));
		}

		public override BoundStatement InstrumentReturnStatement(BoundReturnStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentReturnStatement(original, rewritten));
		}

		public override BoundStatement InstrumentThrowStatement(BoundThrowStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentThrowStatement(original, rewritten));
		}

		public override BoundStatement InstrumentOnErrorStatement(BoundOnErrorStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentOnErrorStatement(original, rewritten));
		}

		public override BoundStatement InstrumentResumeStatement(BoundResumeStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentResumeStatement(original, rewritten));
		}

		public override BoundStatement InstrumentAddHandlerStatement(BoundAddHandlerStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentAddHandlerStatement(original, rewritten));
		}

		public override BoundStatement InstrumentRemoveHandlerStatement(BoundRemoveHandlerStatement original, BoundStatement rewritten)
		{
			return MarkStatementWithSequencePoint(original, base.InstrumentRemoveHandlerStatement(original, rewritten));
		}

		public override BoundStatement CreateBlockPrologue(BoundBlock trueOriginal, BoundBlock original, ref LocalSymbol synthesizedLocal)
		{
			return CreateBlockPrologue(original, base.CreateBlockPrologue(trueOriginal, original, ref synthesizedLocal));
		}

		public override BoundExpression InstrumentTopLevelExpressionInQuery(BoundExpression original, BoundExpression rewritten)
		{
			rewritten = base.InstrumentTopLevelExpressionInQuery(original, rewritten);
			return new BoundSequencePointExpression(original.Syntax, rewritten, rewritten.Type);
		}

		public override BoundStatement InstrumentQueryLambdaBody(BoundQueryLambda original, BoundStatement rewritten)
		{
			rewritten = base.InstrumentQueryLambdaBody(original, rewritten);
			SyntaxNode syntaxNode = null;
			TextSpan span = default(TextSpan);
			switch (original.LambdaSymbol.SynthesizedKind)
			{
			case SynthesizedLambdaKind.AggregateQueryLambda:
			{
				AggregateClauseSyntax aggregateClauseSyntax = (AggregateClauseSyntax)original.Syntax.Parent!.Parent;
				if (aggregateClauseSyntax.AggregationVariables.Count == 1)
				{
					syntaxNode = aggregateClauseSyntax;
					span = aggregateClauseSyntax.Span;
				}
				else
				{
					syntaxNode = aggregateClauseSyntax;
					span = ((aggregateClauseSyntax.AdditionalQueryOperators.Count != 0) ? TextSpan.FromBounds(aggregateClauseSyntax.SpanStart, aggregateClauseSyntax.AdditionalQueryOperators.Last().Span.End) : TextSpan.FromBounds(aggregateClauseSyntax.SpanStart, aggregateClauseSyntax.Variables.Last().Span.End));
				}
				break;
			}
			case SynthesizedLambdaKind.LetVariableQueryLambda:
				syntaxNode = original.Syntax;
				span = TextSpan.FromBounds(original.Syntax.SpanStart, original.Syntax.Span.End);
				break;
			}
			if (syntaxNode != null)
			{
				rewritten = new BoundSequencePointWithSpan(syntaxNode, rewritten, span);
			}
			return rewritten;
		}

		public override BoundStatement InstrumentDoLoopEpilogue(BoundDoLoopStatement original, BoundStatement epilogueOpt)
		{
			return new BoundSequencePoint(((DoLoopBlockSyntax)original.Syntax).LoopStatement, base.InstrumentDoLoopEpilogue(original, epilogueOpt));
		}

		public override BoundStatement CreateSyncLockStatementPrologue(BoundSyncLockStatement original)
		{
			return new BoundSequencePoint(((SyncLockBlockSyntax)original.Syntax).SyncLockStatement, base.CreateSyncLockStatementPrologue(original));
		}

		public override BoundStatement InstrumentSyncLockObjectCapture(BoundSyncLockStatement original, BoundStatement rewritten)
		{
			return new BoundSequencePoint(original.LockExpression.Syntax, base.InstrumentSyncLockObjectCapture(original, rewritten));
		}

		public override BoundStatement CreateSyncLockExitDueToExceptionEpilogue(BoundSyncLockStatement original)
		{
			return new BoundSequencePoint(((SyncLockBlockSyntax)original.Syntax).EndSyncLockStatement, base.CreateSyncLockExitDueToExceptionEpilogue(original));
		}

		public override BoundStatement CreateSyncLockExitNormallyEpilogue(BoundSyncLockStatement original)
		{
			return new BoundSequencePoint(((SyncLockBlockSyntax)original.Syntax).EndSyncLockStatement, base.CreateSyncLockExitNormallyEpilogue(original));
		}

		public override BoundStatement InstrumentWhileEpilogue(BoundWhileStatement original, BoundStatement epilogueOpt)
		{
			return new BoundSequencePoint(((WhileBlockSyntax)original.Syntax).EndWhileStatement, base.InstrumentWhileEpilogue(original, epilogueOpt));
		}

		public override BoundStatement InstrumentWhileStatementConditionalGotoStart(BoundWhileStatement original, BoundStatement ifConditionGotoStart)
		{
			return new BoundSequencePoint(((WhileBlockSyntax)original.Syntax).WhileStatement, base.InstrumentWhileStatementConditionalGotoStart(original, ifConditionGotoStart));
		}

		public override BoundStatement InstrumentDoLoopStatementEntryOrConditionalGotoStart(BoundDoLoopStatement original, BoundStatement ifConditionGotoStartOpt)
		{
			return new BoundSequencePoint(((DoLoopBlockSyntax)original.Syntax).DoStatement, base.InstrumentDoLoopStatementEntryOrConditionalGotoStart(original, ifConditionGotoStartOpt));
		}

		public override BoundStatement InstrumentForEachStatementConditionalGotoStart(BoundForEachStatement original, BoundStatement ifConditionGotoStart)
		{
			return new BoundSequencePoint(null, base.InstrumentForEachStatementConditionalGotoStart(original, ifConditionGotoStart));
		}

		public override BoundStatement InstrumentIfStatementConditionalGoto(BoundIfStatement original, BoundStatement condGoto)
		{
			condGoto = base.InstrumentIfStatementConditionalGoto(original, condGoto);
			switch (VisualBasicExtensions.Kind(original.Syntax))
			{
			case SyntaxKind.MultiLineIfBlock:
				condGoto = new BoundSequencePoint(((MultiLineIfBlockSyntax)original.Syntax).IfStatement, condGoto);
				break;
			case SyntaxKind.ElseIfBlock:
				condGoto = new BoundSequencePoint(((ElseIfBlockSyntax)original.Syntax).ElseIfStatement, condGoto);
				break;
			case SyntaxKind.SingleLineIfStatement:
			{
				SingleLineIfStatementSyntax singleLineIfStatementSyntax = (SingleLineIfStatementSyntax)original.Syntax;
				condGoto = new BoundSequencePointWithSpan(singleLineIfStatementSyntax, condGoto, TextSpan.FromBounds(singleLineIfStatementSyntax.IfKeyword.SpanStart, singleLineIfStatementSyntax.ThenKeyword.EndPosition - 1));
				break;
			}
			}
			return condGoto;
		}

		public override BoundStatement InstrumentIfStatementAfterIfStatement(BoundIfStatement original, BoundStatement afterIfStatement)
		{
			return new BoundSequencePoint(((MultiLineIfBlockSyntax)original.Syntax).EndIfStatement, base.InstrumentIfStatementAfterIfStatement(original, afterIfStatement));
		}

		public override BoundStatement InstrumentIfStatementConsequenceEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			epilogueOpt = base.InstrumentIfStatementConsequenceEpilogue(original, epilogueOpt);
			VisualBasicSyntaxNode syntax = null;
			switch (VisualBasicExtensions.Kind(original.Syntax))
			{
			case SyntaxKind.MultiLineIfBlock:
				syntax = ((MultiLineIfBlockSyntax)original.Syntax).EndIfStatement;
				break;
			case SyntaxKind.ElseIfBlock:
				syntax = ((MultiLineIfBlockSyntax)original.Syntax.Parent).EndIfStatement;
				break;
			case SyntaxKind.SingleLineIfStatement:
				return epilogueOpt;
			}
			return new BoundSequencePoint(syntax, epilogueOpt);
		}

		public override BoundStatement InstrumentIfStatementAlternativeEpilogue(BoundIfStatement original, BoundStatement epilogueOpt)
		{
			return new BoundSequencePoint(((MultiLineIfBlockSyntax)original.AlternativeOpt.Syntax.Parent).EndIfStatement, base.InstrumentIfStatementAlternativeEpilogue(original, epilogueOpt));
		}

		public override BoundStatement CreateIfStatementAlternativePrologue(BoundIfStatement original)
		{
			BoundStatement boundStatement = base.CreateIfStatementAlternativePrologue(original);
			switch (VisualBasicExtensions.Kind(original.AlternativeOpt.Syntax))
			{
			case SyntaxKind.ElseBlock:
				boundStatement = new BoundSequencePoint(((ElseBlockSyntax)original.AlternativeOpt.Syntax).ElseStatement, boundStatement);
				break;
			case SyntaxKind.SingleLineElseClause:
				boundStatement = new BoundSequencePointWithSpan(original.AlternativeOpt.Syntax, boundStatement, ((SingleLineElseClauseSyntax)original.AlternativeOpt.Syntax).ElseKeyword.Span);
				break;
			}
			return boundStatement;
		}

		public override BoundExpression InstrumentDoLoopStatementCondition(BoundDoLoopStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentDoLoopStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentWhileStatementCondition(BoundWhileStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentWhileStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentForEachStatementCondition(BoundForEachStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentForEachStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentObjectForLoopInitCondition(BoundForToStatement original, BoundExpression rewrittenInitCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentObjectForLoopInitCondition(original, rewrittenInitCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentObjectForLoopCondition(BoundForToStatement original, BoundExpression rewrittenLoopCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentObjectForLoopCondition(original, rewrittenLoopCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentIfStatementCondition(BoundIfStatement original, BoundExpression rewrittenCondition, MethodSymbol currentMethodOrLambda)
		{
			return AddConditionSequencePoint(base.InstrumentIfStatementCondition(original, rewrittenCondition, currentMethodOrLambda), original, currentMethodOrLambda);
		}

		public override BoundExpression InstrumentCatchBlockFilter(BoundCatchBlock original, BoundExpression rewrittenFilter, MethodSymbol currentMethodOrLambda)
		{
			rewrittenFilter = base.InstrumentCatchBlockFilter(original, rewrittenFilter, currentMethodOrLambda);
			rewrittenFilter = new BoundSequencePointExpression(((CatchBlockSyntax)original.Syntax).CatchStatement, rewrittenFilter, rewrittenFilter.Type);
			return AddConditionSequencePoint(rewrittenFilter, original, currentMethodOrLambda);
		}

		public override BoundStatement CreateSelectStatementPrologue(BoundSelectStatement original)
		{
			return new BoundSequencePoint(original.ExpressionStatement.Syntax, base.CreateSelectStatementPrologue(original));
		}

		public override BoundExpression InstrumentSelectStatementCaseCondition(BoundSelectStatement original, BoundExpression rewrittenCaseCondition, MethodSymbol currentMethodOrLambda, ref LocalSymbol lazyConditionalBranchLocal)
		{
			return AddConditionSequencePoint(base.InstrumentSelectStatementCaseCondition(original, rewrittenCaseCondition, currentMethodOrLambda, ref lazyConditionalBranchLocal), original, currentMethodOrLambda, ref lazyConditionalBranchLocal);
		}

		public override BoundStatement InstrumentCaseBlockConditionalGoto(BoundCaseBlock original, BoundStatement condGoto)
		{
			return new BoundSequencePoint(original.CaseStatement.Syntax, base.InstrumentCaseBlockConditionalGoto(original, condGoto));
		}

		public override BoundStatement InstrumentCaseElseBlock(BoundCaseBlock original, BoundBlock rewritten)
		{
			return new BoundSequencePoint(original.CaseStatement.Syntax, base.InstrumentCaseElseBlock(original, rewritten));
		}

		public override BoundStatement InstrumentSelectStatementEpilogue(BoundSelectStatement original, BoundStatement epilogueOpt)
		{
			return new BoundSequencePoint(((SelectBlockSyntax)original.Syntax).EndSelectStatement, base.InstrumentSelectStatementEpilogue(original, epilogueOpt));
		}

		public override BoundStatement CreateCatchBlockPrologue(BoundCatchBlock original)
		{
			return new BoundSequencePoint(((CatchBlockSyntax)original.Syntax).CatchStatement, base.CreateCatchBlockPrologue(original));
		}

		public override BoundStatement CreateFinallyBlockPrologue(BoundTryStatement original)
		{
			return new BoundSequencePoint(((FinallyBlockSyntax)original.FinallyBlockOpt.Syntax).FinallyStatement, base.CreateFinallyBlockPrologue(original));
		}

		public override BoundStatement CreateTryBlockPrologue(BoundTryStatement original)
		{
			return new BoundSequencePoint(((TryBlockSyntax)original.Syntax).TryStatement, base.CreateTryBlockPrologue(original));
		}

		public override BoundStatement InstrumentTryStatement(BoundTryStatement original, BoundStatement rewritten)
		{
			return new BoundStatementList(original.Syntax, ImmutableArray.Create(base.InstrumentTryStatement(original, rewritten), new BoundSequencePoint(((TryBlockSyntax)original.Syntax).EndTryStatement, null)));
		}

		public override BoundStatement InstrumentFieldOrPropertyInitializer(BoundFieldOrPropertyInitializer original, BoundStatement rewritten, int symbolIndex, bool createTemporary)
		{
			rewritten = base.InstrumentFieldOrPropertyInitializer(original, rewritten, symbolIndex, createTemporary);
			if (createTemporary)
			{
				rewritten = MarkInitializerSequencePoint(rewritten, original.Syntax, symbolIndex);
			}
			return rewritten;
		}

		public override BoundStatement InstrumentForEachLoopInitialization(BoundForEachStatement original, BoundStatement initialization)
		{
			return new BoundSequencePoint(((ForEachBlockSyntax)original.Syntax).ForEachStatement, base.InstrumentForEachLoopInitialization(original, initialization));
		}

		public override BoundStatement InstrumentForEachLoopEpilogue(BoundForEachStatement original, BoundStatement epilogueOpt)
		{
			epilogueOpt = base.InstrumentForEachLoopEpilogue(original, epilogueOpt);
			if (((ForEachBlockSyntax)original.Syntax).NextStatement != null)
			{
				epilogueOpt = new BoundSequencePoint(((ForEachBlockSyntax)original.Syntax).NextStatement, epilogueOpt);
			}
			return epilogueOpt;
		}

		public override BoundStatement InstrumentForLoopInitialization(BoundForToStatement original, BoundStatement initialization)
		{
			return new BoundSequencePoint(((ForBlockSyntax)original.Syntax).ForStatement, base.InstrumentForLoopInitialization(original, initialization));
		}

		public override BoundStatement InstrumentForLoopIncrement(BoundForToStatement original, BoundStatement increment)
		{
			increment = base.InstrumentForLoopIncrement(original, increment);
			if (((ForBlockSyntax)original.Syntax).NextStatement != null)
			{
				increment = new BoundSequencePoint(((ForBlockSyntax)original.Syntax).NextStatement, increment);
			}
			return increment;
		}

		public override BoundStatement InstrumentLocalInitialization(BoundLocalDeclaration original, BoundStatement rewritten)
		{
			return MarkInitializerSequencePoint(base.InstrumentLocalInitialization(original, rewritten), original.Syntax);
		}

		public override BoundStatement CreateUsingStatementPrologue(BoundUsingStatement original)
		{
			return new BoundSequencePoint(original.UsingInfo.UsingStatementSyntax.UsingStatement, base.CreateUsingStatementPrologue(original));
		}

		public override BoundStatement InstrumentUsingStatementResourceCapture(BoundUsingStatement original, int resourceIndex, BoundStatement rewritten)
		{
			rewritten = base.InstrumentUsingStatementResourceCapture(original, resourceIndex, rewritten);
			if (!original.ResourceList.IsDefault && original.ResourceList.Length > 1)
			{
				BoundLocalDeclarationBase boundLocalDeclarationBase = original.ResourceList[resourceIndex];
				SyntaxNode syntax = ((boundLocalDeclarationBase.Kind != BoundKind.LocalDeclaration) ? boundLocalDeclarationBase.Syntax : boundLocalDeclarationBase.Syntax.Parent);
				rewritten = new BoundSequencePoint(syntax, rewritten);
			}
			return rewritten;
		}

		public override BoundStatement CreateUsingStatementDisposePrologue(BoundUsingStatement original)
		{
			return new BoundSequencePoint(((UsingBlockSyntax)original.Syntax).EndUsingStatement, base.CreateUsingStatementDisposePrologue(original));
		}

		public override BoundStatement CreateWithStatementPrologue(BoundWithStatement original)
		{
			return new BoundSequencePoint(((WithBlockSyntax)original.Syntax).WithStatement, base.CreateWithStatementPrologue(original));
		}

		public override BoundStatement CreateWithStatementEpilogue(BoundWithStatement original)
		{
			return new BoundSequencePoint(((WithBlockSyntax)original.Syntax).EndWithStatement, base.CreateWithStatementEpilogue(original));
		}

		internal static BoundExpression AddConditionSequencePoint(BoundExpression condition, BoundCatchBlock containingCatchWithFilter, MethodSymbol currentMethodOrLambda)
		{
			LocalSymbol lazyConditionalBranchLocal = null;
			return AddConditionSequencePoint(condition, containingCatchWithFilter.ExceptionFilterOpt.Syntax.Parent, currentMethodOrLambda, ref lazyConditionalBranchLocal, shareLocal: false);
		}

		internal static BoundExpression AddConditionSequencePoint(BoundExpression condition, BoundStatement containingStatement, MethodSymbol currentMethodOrLambda)
		{
			LocalSymbol lazyConditionalBranchLocal = null;
			return AddConditionSequencePoint(condition, containingStatement.Syntax, currentMethodOrLambda, ref lazyConditionalBranchLocal, shareLocal: false);
		}

		internal static BoundExpression AddConditionSequencePoint(BoundExpression condition, BoundStatement containingStatement, MethodSymbol currentMethodOrLambda, ref LocalSymbol lazyConditionalBranchLocal)
		{
			return AddConditionSequencePoint(condition, containingStatement.Syntax, currentMethodOrLambda, ref lazyConditionalBranchLocal, shareLocal: true);
		}

		private static BoundExpression AddConditionSequencePoint(BoundExpression condition, SyntaxNode synthesizedVariableSyntax, MethodSymbol currentMethodOrLambda, ref LocalSymbol lazyConditionalBranchLocal, bool shareLocal)
		{
			if (!currentMethodOrLambda.DeclaringCompilation.Options.EnableEditAndContinue)
			{
				return condition;
			}
			SyntaxNode syntax = condition.Syntax;
			if ((object)lazyConditionalBranchLocal == null)
			{
				lazyConditionalBranchLocal = new SynthesizedLocal(currentMethodOrLambda, condition.Type, SynthesizedLocalKind.ConditionalBranchDiscriminator, synthesizedVariableSyntax);
			}
			BoundExpression valueOpt = (((object)condition.ConstantValueOpt == null) ? new BoundSequencePointExpression(null, MakeLocalRead(syntax, lazyConditionalBranchLocal), condition.Type) : condition);
			return new BoundSequence(syntax, shareLocal ? ImmutableArray<LocalSymbol>.Empty : ImmutableArray.Create(lazyConditionalBranchLocal), ImmutableArray.Create(MakeAssignmentExpression(syntax, MakeLocalWrite(syntax, lazyConditionalBranchLocal), condition)), valueOpt, condition.Type);
		}

		private static BoundLocal MakeLocalRead(SyntaxNode syntax, LocalSymbol localSym)
		{
			BoundLocal boundLocal = new BoundLocal(syntax, localSym, isLValue: false, localSym.Type);
			boundLocal.SetWasCompilerGenerated();
			return boundLocal;
		}

		private static BoundLocal MakeLocalWrite(SyntaxNode syntax, LocalSymbol localSym)
		{
			BoundLocal boundLocal = new BoundLocal(syntax, localSym, isLValue: true, localSym.Type);
			boundLocal.SetWasCompilerGenerated();
			return boundLocal;
		}

		private static BoundExpression MakeAssignmentExpression(SyntaxNode syntax, BoundExpression left, BoundExpression right)
		{
			BoundAssignmentOperator boundAssignmentOperator = new BoundAssignmentOperator(syntax, left, right, suppressObjectClone: true);
			boundAssignmentOperator.SetWasCompilerGenerated();
			return boundAssignmentOperator;
		}

		public static BoundStatement CreateBlockPrologue(BoundBlock node, BoundStatement previousPrologue)
		{
			if (node.Syntax is MethodBlockBaseSyntax methodBlockBaseSyntax)
			{
				MethodBaseSyntax blockStatement = methodBlockBaseSyntax.BlockStatement;
				TextSpan span = TextSpan.FromBounds(((blockStatement.Modifiers.Count <= 0) ? blockStatement.DeclarationKeyword : blockStatement.Modifiers[0]).SpanStart, blockStatement.Span.End);
				previousPrologue = new BoundSequencePointWithSpan(blockStatement, previousPrologue, span);
			}
			else if (node.Syntax is LambdaExpressionSyntax lambdaExpressionSyntax)
			{
				previousPrologue = new BoundSequencePoint(lambdaExpressionSyntax.SubOrFunctionHeader, previousPrologue);
			}
			return previousPrologue;
		}

		private static BoundStatement MarkInitializerSequencePoint(BoundStatement rewrittenStatement, SyntaxNode syntax, int nameIndex)
		{
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax.Parent, SyntaxKind.PropertyStatement))
			{
				PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)syntax.Parent;
				TextSpan span = TextSpan.FromBounds(propertyStatementSyntax.Identifier.SpanStart, (propertyStatementSyntax.Initializer == null) ? propertyStatementSyntax.AsClause.Span.End : propertyStatementSyntax.Initializer.Span.End);
				return new BoundSequencePointWithSpan(syntax, rewrittenStatement, span);
			}
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.AsNewClause))
			{
				VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)syntax.Parent;
				if (variableDeclaratorSyntax.Names.Count > 1)
				{
					return new BoundSequencePoint(variableDeclaratorSyntax.Names[nameIndex], rewrittenStatement);
				}
				return new BoundSequencePoint(syntax.Parent, rewrittenStatement);
			}
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.ModifiedIdentifier))
			{
				return new BoundSequencePoint(syntax, rewrittenStatement);
			}
			return new BoundSequencePoint(syntax.Parent, rewrittenStatement);
		}

		private static BoundStatement MarkInitializerSequencePoint(BoundStatement rewrittenStatement, SyntaxNode syntax)
		{
			if (((ModifiedIdentifierSyntax)syntax).ArrayBounds != null)
			{
				return new BoundSequencePoint(syntax, rewrittenStatement);
			}
			VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)syntax.Parent;
			if (variableDeclaratorSyntax.Names.Count > 1)
			{
				return new BoundSequencePoint(syntax, rewrittenStatement);
			}
			return new BoundSequencePoint(variableDeclaratorSyntax, rewrittenStatement);
		}
	}
}
