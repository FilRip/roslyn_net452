using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PropertyBlockContext : DeclarationContext
	{
		private readonly bool _isPropertyBlock;

		private bool IsPropertyBlock
		{
			get
			{
				if (!_isPropertyBlock)
				{
					return base.Statements.Count > 0;
				}
				return true;
			}
		}

		internal PropertyBlockContext(StatementSyntax statement, BlockContext prevContext, bool isPropertyBlock)
			: base(SyntaxKind.PropertyBlock, statement, prevContext)
		{
			_isPropertyBlock = isPropertyBlock;
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			PropertyStatementSyntax beginStmt = null;
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorBlockSyntax> syntaxList = _statements.ToList<AccessorBlockSyntax>();
			FreeStatements();
			if (syntaxList.Any())
			{
				beginStmt = ReportErrorIfHasInitializer(beginStmt);
			}
			return base.SyntaxFactory.PropertyBlock(beginStmt, syntaxList, endStmt2);
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.GetAccessorStatement:
				return new MethodBlockContext(SyntaxKind.GetAccessorBlock, (StatementSyntax)node, this);
			case SyntaxKind.SetAccessorStatement:
				return new MethodBlockContext(SyntaxKind.SetAccessorBlock, (StatementSyntax)node, this);
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
				Add(node);
				return this;
			default:
			{
				BlockContext blockContext = EndBlock(null);
				if (IsPropertyBlock)
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEndsProperty);
				}
				return blockContext.ProcessSyntax(node);
			}
			}
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			if (KindEndsBlock(node.Kind))
			{
				return UseSyntax(node, ref newContext);
			}
			switch (node.Kind)
			{
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
				return UseSyntax(node, ref newContext, ((AccessorBlockSyntax)node).End.IsMissing);
			default:
				newContext = this;
				return LinkResult.Crumble;
			}
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			if (IsPropertyBlock || endStmt != null)
			{
				return base.EndBlock(endStmt);
			}
			PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)base.BeginStatement;
			if (propertyStatementSyntax.ParameterList != null && propertyStatementSyntax.ParameterList.Parameters.Count > 0)
			{
				propertyStatementSyntax = new PropertyStatementSyntax(propertyStatementSyntax.Kind, propertyStatementSyntax.AttributeLists.Node, propertyStatementSyntax.Modifiers.Node, propertyStatementSyntax.PropertyKeyword, propertyStatementSyntax.Identifier, Parser.ReportSyntaxError(propertyStatementSyntax.ParameterList, ERRID.ERR_AutoPropertyCantHaveParams), propertyStatementSyntax.AsClause, propertyStatementSyntax.Initializer, propertyStatementSyntax.ImplementsClause);
			}
			BlockContext prevBlock = base.PrevBlock;
			prevBlock.Add(propertyStatementSyntax);
			return prevBlock;
		}

		internal static PropertyStatementSyntax ReportErrorIfHasInitializer(PropertyStatementSyntax propertyStatement)
		{
			if (propertyStatement.Initializer != null || (propertyStatement.AsClause != null && propertyStatement.AsClause is AsNewClauseSyntax))
			{
				propertyStatement = Parser.ReportSyntaxError(propertyStatement, ERRID.ERR_InitializedExpandedProperty);
			}
			return propertyStatement;
		}
	}
}
