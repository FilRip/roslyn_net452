namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ExecutableStatementContext : DeclarationContext
	{
		internal override bool IsSingleLine => base.PrevBlock.IsSingleLine;

		internal ExecutableStatementContext(SyntaxKind contextKind, StatementSyntax statement, BlockContext prevContext)
			: base(contextKind, statement, prevContext)
		{
		}

		internal sealed override StatementSyntax Parse()
		{
			return base.Parser.ParseStatementInMethodBody();
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			if (Parser.IsDeclarationStatement(node.Kind))
			{
				BlockContext blockContext = BlockContextExtensions.FindNearest(this, (SyntaxKind s) => SyntaxFacts.IsMethodBlock(s) || s == SyntaxKind.ConstructorBlock || s == SyntaxKind.OperatorBlock || SyntaxFacts.IsAccessorBlock(s));
				if (blockContext != null)
				{
					BlockContext prevBlock = blockContext.PrevBlock;
					BlockContextExtensions.RecoverFromMissingEnd(this, prevBlock);
					return prevBlock.ProcessSyntax(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEndsProc));
				}
				node = Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideBlock, SyntaxFacts.GetBlockName(base.BlockKind));
				return base.ProcessSyntax(node);
			}
			SyntaxKind kind = node.Kind;
			if (kind - 41 <= SyntaxKind.List || kind - 57 <= SyntaxKind.List)
			{
				node = ((BlockContextExtensions.FindNearest(this, (SyntaxKind s) => SyntaxFacts.IsMethodBlock(s) || s == SyntaxKind.ConstructorBlock || s == SyntaxKind.OperatorBlock || SyntaxFacts.IsAccessorBlock(s) || SyntaxFacts.IsMultiLineLambdaExpression(s) || SyntaxFacts.IsSingleLineLambdaExpression(s)) == null) ? Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideBlock, SyntaxFacts.GetBlockName(base.BlockKind)) : Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideProc));
				Add(node);
				return this;
			}
			return TryProcessExecutableStatement(node) ?? base.ProcessSyntax(node);
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			switch (node.Kind)
			{
			case SyntaxKind.OptionStatement:
			case SyntaxKind.ImportsStatement:
			case SyntaxKind.NamespaceStatement:
			case SyntaxKind.InheritsStatement:
			case SyntaxKind.ImplementsStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.StructureStatement:
			case SyntaxKind.InterfaceStatement:
			case SyntaxKind.ClassStatement:
				if (!((TypeStatementSyntax)node).Modifiers.Any())
				{
					return UseSyntax(node, ref newContext);
				}
				newContext = this;
				return LinkResult.NotUsed;
			case SyntaxKind.EnumStatement:
				if (!((EnumStatementSyntax)node).Modifiers.Any())
				{
					return UseSyntax(node, ref newContext);
				}
				newContext = this;
				return LinkResult.NotUsed;
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				if (!((MethodBaseSyntax)node).Modifiers.Any())
				{
					return UseSyntax(node, ref newContext);
				}
				newContext = this;
				return LinkResult.NotUsed;
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
				if (!ParserExtensions.Any(((MethodBaseSyntax)node).Modifiers, SyntaxKind.DimKeyword, SyntaxKind.ConstKeyword))
				{
					return UseSyntax(node, ref newContext);
				}
				newContext = this;
				return LinkResult.NotUsed;
			case SyntaxKind.NamespaceBlock:
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
			case SyntaxKind.EnumBlock:
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
			case SyntaxKind.PropertyBlock:
			case SyntaxKind.EventBlock:
			case SyntaxKind.FieldDeclaration:
			case SyntaxKind.AttributeList:
			case SyntaxKind.SingleLineElseClause:
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
			case SyntaxKind.CatchBlock:
			case SyntaxKind.FinallyBlock:
			case SyntaxKind.CaseBlock:
			case SyntaxKind.CaseElseBlock:
				newContext = this;
				return LinkResult.Crumble;
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				newContext = this;
				return LinkResult.NotUsed;
			default:
				return TryLinkStatement(node, ref newContext);
			}
		}

		internal override BlockContext ProcessStatementTerminator(BlockContext lambdaContext)
		{
			SyntaxKind kind = base.Parser.CurrentToken.Kind;
			bool isSingleLine = IsSingleLine;
			if (isSingleLine && kind - 677 <= SyntaxKind.List)
			{
				return EndBlock(null).ProcessStatementTerminator(lambdaContext);
			}
			bool allowLeadingMultilineTrivia = false;
			switch (kind)
			{
			case SyntaxKind.StatementTerminatorToken:
				allowLeadingMultilineTrivia = true;
				break;
			case SyntaxKind.ColonToken:
				allowLeadingMultilineTrivia = !IsSingleLine;
				break;
			}
			if (lambdaContext == null || base.Parser.IsNextStatementInsideLambda(this, lambdaContext, allowLeadingMultilineTrivia))
			{
				base.Parser.ConsumeStatementTerminator(isSingleLine);
				return this;
			}
			return BlockContextExtensions.EndLambda(this);
		}
	}
}
