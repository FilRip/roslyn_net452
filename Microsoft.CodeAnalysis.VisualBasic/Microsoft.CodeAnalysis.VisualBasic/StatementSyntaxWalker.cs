using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class StatementSyntaxWalker : VisualBasicSyntaxVisitor
	{
		public virtual void VisitList(IEnumerable<VisualBasicSyntaxNode> list)
		{
			foreach (VisualBasicSyntaxNode item in list)
			{
				Visit(item);
			}
		}

		public override void VisitCompilationUnit(CompilationUnitSyntax node)
		{
			VisitList(node.Options);
			VisitList(node.Imports);
			VisitList(node.Attributes);
			VisitList(node.Members);
		}

		public override void VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			Visit(node.NamespaceStatement);
			VisitList(node.Members);
			Visit(node.EndNamespaceStatement);
		}

		public override void VisitModuleBlock(ModuleBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Members);
			Visit(node.EndBlockStatement);
		}

		public override void VisitClassBlock(ClassBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Inherits);
			VisitList(node.Implements);
			VisitList(node.Members);
			Visit(node.EndBlockStatement);
		}

		public override void VisitStructureBlock(StructureBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Inherits);
			VisitList(node.Implements);
			VisitList(node.Members);
			Visit(node.EndBlockStatement);
		}

		public override void VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Inherits);
			VisitList(node.Members);
			Visit(node.EndBlockStatement);
		}

		public override void VisitEnumBlock(EnumBlockSyntax node)
		{
			Visit(node.EnumStatement);
			VisitList(node.Members);
			Visit(node.EndEnumStatement);
		}

		public override void VisitMethodBlock(MethodBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Statements);
			Visit(node.EndBlockStatement);
		}

		public override void VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Statements);
			Visit(node.EndBlockStatement);
		}

		public override void VisitOperatorBlock(OperatorBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Statements);
			Visit(node.EndBlockStatement);
		}

		public override void VisitAccessorBlock(AccessorBlockSyntax node)
		{
			Visit(node.BlockStatement);
			VisitList(node.Statements);
			Visit(node.EndBlockStatement);
		}

		public override void VisitPropertyBlock(PropertyBlockSyntax node)
		{
			Visit(node.PropertyStatement);
			VisitList(node.Accessors);
			Visit(node.EndPropertyStatement);
		}

		public override void VisitEventBlock(EventBlockSyntax node)
		{
			Visit(node.EventStatement);
			VisitList(node.Accessors);
			Visit(node.EndEventStatement);
		}

		public override void VisitWhileBlock(WhileBlockSyntax node)
		{
			Visit(node.WhileStatement);
			VisitList(node.Statements);
			Visit(node.EndWhileStatement);
		}

		public override void VisitUsingBlock(UsingBlockSyntax node)
		{
			Visit(node.UsingStatement);
			VisitList(node.Statements);
			Visit(node.EndUsingStatement);
		}

		public override void VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			Visit(node.SyncLockStatement);
			VisitList(node.Statements);
			Visit(node.EndSyncLockStatement);
		}

		public override void VisitWithBlock(WithBlockSyntax node)
		{
			Visit(node.WithStatement);
			VisitList(node.Statements);
			Visit(node.EndWithStatement);
		}

		public override void VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			VisitList(node.Statements);
			Visit(node.ElseClause);
		}

		public override void VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			VisitList(node.Statements);
		}

		public override void VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			Visit(node.IfStatement);
			VisitList(node.Statements);
			VisitList(node.ElseIfBlocks);
			Visit(node.ElseBlock);
			Visit(node.EndIfStatement);
		}

		public override void VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			Visit(node.ElseIfStatement);
			VisitList(node.Statements);
		}

		public override void VisitElseBlock(ElseBlockSyntax node)
		{
			Visit(node.ElseStatement);
			VisitList(node.Statements);
		}

		public override void VisitTryBlock(TryBlockSyntax node)
		{
			Visit(node.TryStatement);
			VisitList(node.Statements);
			VisitList(node.CatchBlocks);
			Visit(node.FinallyBlock);
			Visit(node.EndTryStatement);
		}

		public override void VisitCatchBlock(CatchBlockSyntax node)
		{
			Visit(node.CatchStatement);
			VisitList(node.Statements);
		}

		public override void VisitFinallyBlock(FinallyBlockSyntax node)
		{
			Visit(node.FinallyStatement);
			VisitList(node.Statements);
		}

		public override void VisitSelectBlock(SelectBlockSyntax node)
		{
			Visit(node.SelectStatement);
			VisitList(node.CaseBlocks);
			Visit(node.EndSelectStatement);
		}

		public override void VisitCaseBlock(CaseBlockSyntax node)
		{
			Visit(node.CaseStatement);
			VisitList(node.Statements);
		}

		public override void VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			Visit(node.DoStatement);
			VisitList(node.Statements);
			Visit(node.LoopStatement);
		}

		public override void VisitForBlock(ForBlockSyntax node)
		{
			Visit(node.ForStatement);
			VisitList(node.Statements);
		}

		public override void VisitForEachBlock(ForEachBlockSyntax node)
		{
			Visit(node.ForEachStatement);
			VisitList(node.Statements);
		}
	}
}
