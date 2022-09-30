namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InterfaceDeclarationBlockContext : TypeBlockContext
	{
		internal InterfaceDeclarationBlockContext(StatementSyntax statement, BlockContext prevContext)
			: base(SyntaxKind.InterfaceBlock, statement, prevContext)
		{
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			while (true)
			{
				switch (_state)
				{
				case SyntaxKind.None:
				{
					SyntaxKind kind2 = node.Kind;
					if (kind2 == SyntaxKind.InheritsStatement)
					{
						_state = SyntaxKind.InheritsStatement;
					}
					else
					{
						_state = SyntaxKind.InterfaceStatement;
					}
					continue;
				}
				case SyntaxKind.InheritsStatement:
				{
					SyntaxKind kind3 = node.Kind;
					if (kind3 == SyntaxKind.InheritsStatement)
					{
						Add(node);
						return this;
					}
					_inheritsDecls = BaseDeclarations<InheritsStatementSyntax>();
					_state = SyntaxKind.InterfaceStatement;
					continue;
				}
				}
				switch (kind)
				{
				case SyntaxKind.EmptyStatement:
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
				case SyntaxKind.DelegateSubStatement:
				case SyntaxKind.DelegateFunctionStatement:
					Add(node);
					break;
				case SyntaxKind.IncompleteMember:
					node = Parser.ReportSyntaxError(node, ERRID.ERR_InterfaceMemberSyntax);
					Add(node);
					break;
				case SyntaxKind.PropertyStatement:
					node = PropertyBlockContext.ReportErrorIfHasInitializer((PropertyStatementSyntax)node);
					Add(node);
					break;
				case SyntaxKind.SubNewStatement:
					Add(Parser.ReportSyntaxError(node, ERRID.ERR_NewInInterface));
					break;
				case SyntaxKind.EventStatement:
				{
					EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)node;
					if (eventStatementSyntax.CustomKeyword != null)
					{
						eventStatementSyntax = Parser.ReportSyntaxError(eventStatementSyntax, ERRID.ERR_CustomEventInvInInterface);
					}
					Add(eventStatementSyntax);
					break;
				}
				case SyntaxKind.EnumStatement:
					return new EnumDeclarationBlockContext((StatementSyntax)node, this);
				case SyntaxKind.ClassStatement:
					return new TypeBlockContext(SyntaxKind.ClassBlock, (StatementSyntax)node, this);
				case SyntaxKind.StructureStatement:
					return new TypeBlockContext(SyntaxKind.StructureBlock, (StatementSyntax)node, this);
				case SyntaxKind.InterfaceStatement:
					return new InterfaceDeclarationBlockContext((StatementSyntax)node, this);
				case SyntaxKind.FieldDeclaration:
					Add(Parser.ReportSyntaxError(node, ERRID.ERR_InterfaceMemberSyntax));
					break;
				case SyntaxKind.LabelStatement:
					node = Parser.ReportSyntaxError(node, ERRID.ERR_InvOutsideProc);
					Add(node);
					break;
				case SyntaxKind.StructureBlock:
				case SyntaxKind.InterfaceBlock:
				case SyntaxKind.ClassBlock:
				case SyntaxKind.EnumBlock:
					Add(node);
					break;
				case SyntaxKind.EndSubStatement:
				case SyntaxKind.EndFunctionStatement:
				case SyntaxKind.EndGetStatement:
				case SyntaxKind.EndSetStatement:
				case SyntaxKind.EndPropertyStatement:
				case SyntaxKind.EndOperatorStatement:
				case SyntaxKind.EndEventStatement:
				case SyntaxKind.EndAddHandlerStatement:
				case SyntaxKind.EndRemoveHandlerStatement:
				case SyntaxKind.EndRaiseEventStatement:
					Add(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideInterface));
					break;
				case SyntaxKind.NamespaceStatement:
					return EndBlock(null).ProcessSyntax(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideEndsInterface));
				default:
					Add(Parser.ReportSyntaxError(node, ERRID.ERR_InvInsideInterface));
					break;
				}
				return this;
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
			case SyntaxKind.EndSubStatement:
			case SyntaxKind.EndFunctionStatement:
			case SyntaxKind.EndGetStatement:
			case SyntaxKind.EndSetStatement:
			case SyntaxKind.EndPropertyStatement:
			case SyntaxKind.EndOperatorStatement:
			case SyntaxKind.EndEventStatement:
			case SyntaxKind.EndAddHandlerStatement:
			case SyntaxKind.EndRemoveHandlerStatement:
			case SyntaxKind.EndRaiseEventStatement:
			case SyntaxKind.InheritsStatement:
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.PropertyStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
				return UseSyntax(node, ref newContext, ((TypeBlockSyntax)node).EndBlockStatement.IsMissing);
			case SyntaxKind.EnumBlock:
				return UseSyntax(node, ref newContext, ((EnumBlockSyntax)node).EndEnumStatement.IsMissing);
			default:
				newContext = this;
				return LinkResult.Crumble;
			}
		}

		internal override BlockContext RecoverFromMismatchedEnd(StatementSyntax statement)
		{
			SyntaxKind kind = statement.Kind;
			if (kind - 15 <= SyntaxKind.EndStructureStatement)
			{
				return ProcessSyntax(statement);
			}
			return base.RecoverFromMismatchedEnd(statement);
		}
	}
}
