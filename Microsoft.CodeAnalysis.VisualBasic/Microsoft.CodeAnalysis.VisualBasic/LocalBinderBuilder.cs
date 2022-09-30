using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class LocalBinderBuilder : VisualBasicSyntaxVisitor
	{
		private ImmutableDictionary<SyntaxNode, BlockBaseBinder> _nodeMap;

		private ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> _listMap;

		private readonly MethodSymbol _enclosingMethod;

		private Binder _containingBinder;

		public ImmutableDictionary<SyntaxNode, BlockBaseBinder> NodeToBinderMap => _nodeMap;

		public ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> StmtListToBinderMap => _listMap;

		public LocalBinderBuilder(MethodSymbol enclosingMethod)
		{
			_enclosingMethod = enclosingMethod;
			_nodeMap = ImmutableDictionary.Create<SyntaxNode, BlockBaseBinder>();
			_listMap = ImmutableDictionary.Create<SyntaxList<StatementSyntax>, BlockBaseBinder>();
		}

		public LocalBinderBuilder(MethodSymbol enclosingMethod, ImmutableDictionary<SyntaxNode, BlockBaseBinder> nodeMap, ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> listMap)
		{
			_enclosingMethod = enclosingMethod;
			_nodeMap = nodeMap;
			_listMap = listMap;
		}

		public void MakeBinder(SyntaxNode node, Binder containingBinder)
		{
			Binder containingBinder2 = _containingBinder;
			_containingBinder = containingBinder;
			base.Visit(node);
			_containingBinder = containingBinder2;
		}

		private void VisitStatementsInList(IEnumerable<StatementSyntax> list, BlockBaseBinder currentBinder)
		{
			foreach (StatementSyntax item in list)
			{
				MakeBinder(item, currentBinder);
			}
		}

		private void CreateBinderFromStatementList(SyntaxList<StatementSyntax> list, Binder outerBinder)
		{
			StatementListBinder statementListBinder = new StatementListBinder(outerBinder, list);
			_listMap = _listMap.SetItem(list, statementListBinder);
			VisitStatementsInList(list, statementListBinder);
		}

		private void RememberBinder(VisualBasicSyntaxNode node, Binder binder)
		{
			_nodeMap = _nodeMap.SetItem(node, (BlockBaseBinder)binder);
		}

		public override void VisitCompilationUnit(CompilationUnitSyntax node)
		{
			SyntaxList<StatementSyntax>.Enumerator enumerator = node.Members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				MakeBinder(current, _containingBinder);
			}
		}

		public override void VisitMethodBlock(MethodBlockSyntax node)
		{
			VisitMethodBlockBase(node);
		}

		public override void VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			VisitMethodBlockBase(node);
		}

		public override void VisitOperatorBlock(OperatorBlockSyntax node)
		{
			VisitMethodBlockBase(node);
		}

		public override void VisitAccessorBlock(AccessorBlockSyntax node)
		{
			VisitMethodBlockBase(node);
		}

		private void VisitMethodBlockBase(MethodBlockBaseSyntax methodBlock)
		{
			SyntaxKind exitKind;
			switch (methodBlock.BlockStatement.Kind())
			{
			case SyntaxKind.SubStatement:
			case SyntaxKind.SubNewStatement:
				exitKind = SyntaxKind.ExitSubStatement;
				break;
			case SyntaxKind.FunctionStatement:
				exitKind = SyntaxKind.ExitFunctionStatement;
				break;
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
				exitKind = SyntaxKind.ExitPropertyStatement;
				break;
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				exitKind = SyntaxKind.EventStatement;
				break;
			case SyntaxKind.OperatorStatement:
				exitKind = SyntaxKind.OperatorStatement;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(methodBlock.BlockStatement.Kind());
			}
			_containingBinder = new ExitableStatementBinder(_containingBinder, SyntaxKind.None, exitKind);
			RememberBinder(methodBlock, _containingBinder);
			CreateBinderFromStatementList(methodBlock.Statements, _containingBinder);
		}

		public override void VisitSingleLineLambdaExpression(SingleLineLambdaExpressionSyntax node)
		{
			if (node == _enclosingMethod.Syntax)
			{
				_containingBinder = new ExitableStatementBinder(exitKind: node.Kind() switch
				{
					SyntaxKind.SingleLineSubLambdaExpression => SyntaxKind.ExitSubStatement, 
					SyntaxKind.SingleLineFunctionLambdaExpression => SyntaxKind.ExitFunctionStatement, 
					_ => throw ExceptionUtilities.UnexpectedValue(node.Kind()), 
				}, enclosing: _containingBinder, continueKind: SyntaxKind.None);
				RememberBinder(node, _containingBinder);
				if (node.Kind() == SyntaxKind.SingleLineSubLambdaExpression)
				{
					CreateBinderFromStatementList(node.Statements, _containingBinder);
				}
				MakeBinder(node.Body, _containingBinder);
			}
			else
			{
				base.VisitSingleLineLambdaExpression(node);
			}
		}

		public override void VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			if (node == _enclosingMethod.Syntax)
			{
				_containingBinder = new ExitableStatementBinder(exitKind: node.Kind() switch
				{
					SyntaxKind.MultiLineSubLambdaExpression => SyntaxKind.ExitSubStatement, 
					SyntaxKind.MultiLineFunctionLambdaExpression => SyntaxKind.ExitFunctionStatement, 
					_ => throw ExceptionUtilities.UnexpectedValue(node.Kind()), 
				}, enclosing: _containingBinder, continueKind: SyntaxKind.None);
				RememberBinder(node, _containingBinder);
				CreateBinderFromStatementList(node.Statements, _containingBinder);
			}
			else
			{
				base.VisitMultiLineLambdaExpression(node);
			}
		}

		public override void VisitWhileBlock(WhileBlockSyntax node)
		{
			_containingBinder = new ExitableStatementBinder(_containingBinder, SyntaxKind.ContinueWhileStatement, SyntaxKind.ExitWhileStatement);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitUsingBlock(UsingBlockSyntax node)
		{
			_containingBinder = new UsingBlockBinder(_containingBinder, node);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitWithBlock(WithBlockSyntax node)
		{
			_containingBinder = new WithBlockBinder(_containingBinder, node);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
			MakeBinder(node.ElseClause, _containingBinder);
		}

		public override void VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
			SyntaxList<ElseIfBlockSyntax>.Enumerator enumerator = node.ElseIfBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ElseIfBlockSyntax current = enumerator.Current;
				MakeBinder(current, _containingBinder);
			}
			MakeBinder(node.ElseBlock, _containingBinder);
		}

		public override void VisitElseBlock(ElseBlockSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitTryBlock(TryBlockSyntax node)
		{
			_containingBinder = new ExitableStatementBinder(_containingBinder, SyntaxKind.None, SyntaxKind.ExitTryStatement);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
			SyntaxList<CatchBlockSyntax>.Enumerator enumerator = node.CatchBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CatchBlockSyntax current = enumerator.Current;
				MakeBinder(current, _containingBinder);
			}
			MakeBinder(node.FinallyBlock, _containingBinder);
		}

		public override void VisitCatchBlock(CatchBlockSyntax node)
		{
			_containingBinder = new CatchBlockBinder(_containingBinder, node);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitFinallyBlock(FinallyBlockSyntax node)
		{
			_containingBinder = new FinallyBlockBinder(_containingBinder);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitSelectBlock(SelectBlockSyntax node)
		{
			_containingBinder = new ExitableStatementBinder(_containingBinder, SyntaxKind.None, SyntaxKind.ExitSelectStatement);
			RememberBinder(node, _containingBinder);
			SyntaxList<CaseBlockSyntax>.Enumerator enumerator = node.CaseBlocks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CaseBlockSyntax current = enumerator.Current;
				MakeBinder(current, _containingBinder);
			}
		}

		public override void VisitCaseBlock(CaseBlockSyntax node)
		{
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			_containingBinder = new ExitableStatementBinder(_containingBinder, SyntaxKind.ContinueDoStatement, SyntaxKind.ExitDoStatement);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitForBlock(ForBlockSyntax node)
		{
			_containingBinder = new ForOrForEachBlockBinder(_containingBinder, node);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}

		public override void VisitForEachBlock(ForEachBlockSyntax node)
		{
			_containingBinder = new ForOrForEachBlockBinder(_containingBinder, node);
			RememberBinder(node, _containingBinder);
			CreateBinderFromStatementList(node.Statements, _containingBinder);
		}
	}
}
