using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class DeclarationContext : BlockContext
	{
		internal override bool IsSingleLine => false;

		internal DeclarationContext(SyntaxKind kind, StatementSyntax statement, BlockContext context)
			: base(kind, statement, context)
		{
		}

		internal override StatementSyntax Parse()
		{
			return base.Parser.ParseDeclarationStatement();
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind contextKind;
			MethodBaseSyntax methodBaseSyntax;
			switch (node.Kind)
			{
			case SyntaxKind.NamespaceStatement:
			{
				bool flag = true;
				DiagnosticInfo[] diagnostics = node.GetDiagnostics();
				if (diagnostics != null)
				{
					DiagnosticInfo[] array = diagnostics;
					for (int i = 0; i < array.Length; i = checked(i + 1))
					{
						int code = array[i].Code;
						if (code == 30289 || code == 30618)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_NamespaceNotAtNamespace);
				}
				BlockContext prevBlock = base.PrevBlock;
				BlockContextExtensions.RecoverFromMissingEnd(this, prevBlock);
				return prevBlock.ProcessSyntax(node);
			}
			case SyntaxKind.ModuleStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ModuleNotAtNamespace);
				return new TypeBlockContext(SyntaxKind.ModuleBlock, (StatementSyntax)node, this);
			case SyntaxKind.EnumStatement:
				return new EnumDeclarationBlockContext((StatementSyntax)node, this);
			case SyntaxKind.ClassStatement:
				return new TypeBlockContext(SyntaxKind.ClassBlock, (StatementSyntax)node, this);
			case SyntaxKind.StructureStatement:
				return new TypeBlockContext(SyntaxKind.StructureBlock, (StatementSyntax)node, this);
			case SyntaxKind.InterfaceStatement:
				return new InterfaceDeclarationBlockContext((StatementSyntax)node, this);
			case SyntaxKind.SubStatement:
				contextKind = SyntaxKind.SubBlock;
				goto IL_0286;
			case SyntaxKind.SubNewStatement:
				contextKind = SyntaxKind.ConstructorBlock;
				goto IL_0286;
			case SyntaxKind.FunctionStatement:
				contextKind = SyntaxKind.FunctionBlock;
				goto IL_0286;
			case SyntaxKind.OperatorStatement:
				if (!base.Parser.IsFirstStatementOnLine(node.GetFirstToken()))
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_MethodMustBeFirstStatementOnLine);
				}
				if (base.BlockKind == SyntaxKind.ModuleBlock)
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_OperatorDeclaredInModule);
				}
				return new MethodBlockContext(SyntaxKind.OperatorBlock, (StatementSyntax)node, this);
			case SyntaxKind.PropertyStatement:
			{
				PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)node;
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<KeywordSyntax> modifiers = propertyStatementSyntax.Modifiers;
				bool isPropertyBlock = false;
				if (modifiers.Any())
				{
					if (modifiers.Any(505))
					{
						node = PropertyBlockContext.ReportErrorIfHasInitializer((PropertyStatementSyntax)node);
						Add(node);
						break;
					}
					isPropertyBlock = ParserExtensions.Any(modifiers, SyntaxKind.DefaultKeyword, SyntaxKind.IteratorKeyword);
				}
				return new PropertyBlockContext(propertyStatementSyntax, this, isPropertyBlock);
			}
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ExpectedDeclaration);
				Add(node);
				break;
			case SyntaxKind.EventStatement:
			{
				EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)node;
				if (eventStatementSyntax.CustomKeyword != null)
				{
					if (!eventStatementSyntax.AsClause.AsKeyword.IsMissing)
					{
						return new EventBlockContext(eventStatementSyntax, this);
					}
					node = Parser.ReportSyntaxError(eventStatementSyntax, ERRID.ERR_CustomEventRequiresAs);
				}
				Add(node);
				break;
			}
			case SyntaxKind.AttributesStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_AttributeStmtWrongOrder);
				Add(node);
				break;
			case SyntaxKind.OptionStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_OptionStmtWrongOrder);
				Add(node);
				break;
			case SyntaxKind.ImportsStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ImportsMustBeFirst);
				Add(node);
				break;
			case SyntaxKind.InheritsStatement:
			{
				StatementSyntax beginStatement = base.BeginStatement;
				node = ((beginStatement == null || beginStatement.Kind != SyntaxKind.InterfaceStatement) ? Parser.ReportSyntaxError(node, ERRID.ERR_InheritsStmtWrongOrder) : Parser.ReportSyntaxError(node, ERRID.ERR_BadInterfaceOrderOnInherits));
				Add(node);
				break;
			}
			case SyntaxKind.ImplementsStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ImplementsStmtWrongOrder);
				Add(node);
				break;
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
			case SyntaxKind.PropertyBlock:
			case SyntaxKind.EventBlock:
				Add(node);
				break;
			case SyntaxKind.EmptyStatement:
			case SyntaxKind.EnumMemberDeclaration:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			case SyntaxKind.IncompleteMember:
			case SyntaxKind.FieldDeclaration:
				Add(node);
				break;
			case SyntaxKind.LabelStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_InvOutsideProc);
				Add(node);
				break;
			case SyntaxKind.StopStatement:
			case SyntaxKind.EndStatement:
				Add(node);
				break;
			default:
				{
					if (!SyntaxFacts.IsEndBlockLoopOrNextStatement(node.Kind))
					{
						node = Parser.ReportSyntaxError(node, ERRID.ERR_ExecutableAsDeclaration);
					}
					Add(node);
					break;
				}
				IL_0286:
				if (!base.Parser.IsFirstStatementOnLine(node.GetFirstToken()))
				{
					node = Parser.ReportSyntaxError(node, ERRID.ERR_MethodMustBeFirstStatementOnLine);
				}
				methodBaseSyntax = (MethodBaseSyntax)node;
				if (!methodBaseSyntax.Modifiers.Any(505))
				{
					return new MethodBlockContext(contextKind, methodBaseSyntax, this);
				}
				Add(node);
				break;
			}
			return this;
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
			case SyntaxKind.OptionStatement:
			case SyntaxKind.ImportsStatement:
			case SyntaxKind.NamespaceStatement:
			case SyntaxKind.InheritsStatement:
			case SyntaxKind.ImplementsStatement:
			case SyntaxKind.ModuleStatement:
			case SyntaxKind.StructureStatement:
			case SyntaxKind.InterfaceStatement:
			case SyntaxKind.ClassStatement:
			case SyntaxKind.EnumStatement:
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
			case SyntaxKind.SubNewStatement:
			case SyntaxKind.EventStatement:
			case SyntaxKind.OperatorStatement:
			case SyntaxKind.PropertyStatement:
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
			case SyntaxKind.AttributesStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.EnumMemberDeclaration:
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			case SyntaxKind.FieldDeclaration:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
				return UseSyntax(node, ref newContext, ((TypeBlockSyntax)node).EndBlockStatement.IsMissing);
			case SyntaxKind.EnumBlock:
				return UseSyntax(node, ref newContext, ((EnumBlockSyntax)node).EndEnumStatement.IsMissing);
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
				return UseSyntax(node, ref newContext, ((MethodBlockBaseSyntax)node).End.IsMissing);
			case SyntaxKind.OperatorBlock:
				if (base.BlockKind == SyntaxKind.ModuleBlock)
				{
					newContext = this;
					return LinkResult.Crumble;
				}
				return UseSyntax(node, ref newContext, ((OperatorBlockSyntax)node).End.IsMissing);
			case SyntaxKind.EventBlock:
				return UseSyntax(node, ref newContext, ((EventBlockSyntax)node).EndEventStatement.IsMissing);
			case SyntaxKind.PropertyBlock:
				return UseSyntax(node, ref newContext, ((PropertyBlockSyntax)node).EndPropertyStatement.IsMissing);
			case SyntaxKind.NamespaceBlock:
			case SyntaxKind.ModuleBlock:
				newContext = this;
				return LinkResult.Crumble;
			case SyntaxKind.SingleLineIfStatement:
				newContext = this;
				return LinkResult.Crumble;
			case SyntaxKind.LocalDeclarationStatement:
				newContext = this;
				return LinkResult.Crumble;
			case SyntaxKind.IfStatement:
				node = Parser.ReportSyntaxError(node, ERRID.ERR_ExecutableAsDeclaration);
				return TryUseStatement(node, ref newContext);
			default:
				newContext = this;
				return LinkResult.NotUsed;
			}
		}

		internal override BlockContext RecoverFromMismatchedEnd(StatementSyntax statement)
		{
			ERRID eRRID = ERRID.ERR_Syntax;
			switch (statement.Kind)
			{
			case SyntaxKind.EndIfStatement:
				eRRID = ERRID.ERR_EndIfNoMatchingIf;
				break;
			case SyntaxKind.EndWithStatement:
				eRRID = ERRID.ERR_EndWithWithoutWith;
				break;
			case SyntaxKind.EndSelectStatement:
				eRRID = ERRID.ERR_EndSelectNoSelect;
				break;
			case SyntaxKind.EndWhileStatement:
				eRRID = ERRID.ERR_EndWhileNoWhile;
				break;
			case SyntaxKind.SimpleLoopStatement:
			case SyntaxKind.LoopWhileStatement:
			case SyntaxKind.LoopUntilStatement:
				eRRID = ERRID.ERR_LoopNoMatchingDo;
				break;
			case SyntaxKind.NextStatement:
				eRRID = ERRID.ERR_NextNoMatchingFor;
				break;
			case SyntaxKind.EndSubStatement:
				eRRID = ERRID.ERR_InvalidEndSub;
				break;
			case SyntaxKind.EndFunctionStatement:
				eRRID = ERRID.ERR_InvalidEndFunction;
				break;
			case SyntaxKind.EndOperatorStatement:
				eRRID = ERRID.ERR_InvalidEndOperator;
				break;
			case SyntaxKind.EndPropertyStatement:
				eRRID = ERRID.ERR_InvalidEndProperty;
				break;
			case SyntaxKind.EndGetStatement:
				eRRID = ERRID.ERR_InvalidEndGet;
				break;
			case SyntaxKind.EndSetStatement:
				eRRID = ERRID.ERR_InvalidEndSet;
				break;
			case SyntaxKind.EndEventStatement:
				eRRID = ERRID.ERR_InvalidEndEvent;
				break;
			case SyntaxKind.EndAddHandlerStatement:
				eRRID = ERRID.ERR_InvalidEndAddHandler;
				break;
			case SyntaxKind.EndRemoveHandlerStatement:
				eRRID = ERRID.ERR_InvalidEndRemoveHandler;
				break;
			case SyntaxKind.EndRaiseEventStatement:
				eRRID = ERRID.ERR_InvalidEndRaiseEvent;
				break;
			case SyntaxKind.EndStructureStatement:
				eRRID = ERRID.ERR_EndStructureNoStructure;
				break;
			case SyntaxKind.EndEnumStatement:
				eRRID = ERRID.ERR_InvalidEndEnum;
				break;
			case SyntaxKind.EndInterfaceStatement:
				eRRID = ERRID.ERR_InvalidEndInterface;
				break;
			case SyntaxKind.EndTryStatement:
				eRRID = ERRID.ERR_EndTryNoTry;
				break;
			case SyntaxKind.EndClassStatement:
				eRRID = ERRID.ERR_EndClassNoClass;
				break;
			case SyntaxKind.EndModuleStatement:
				eRRID = ERRID.ERR_EndModuleNoModule;
				break;
			case SyntaxKind.EndNamespaceStatement:
				eRRID = ERRID.ERR_EndNamespaceNoNamespace;
				break;
			case SyntaxKind.EndUsingStatement:
				eRRID = ERRID.ERR_EndUsingWithoutUsing;
				break;
			case SyntaxKind.EndSyncLockStatement:
				eRRID = ERRID.ERR_EndSyncLockNoSyncLock;
				break;
			case SyntaxKind.EmptyStatement:
			case SyntaxKind.IncompleteMember:
				eRRID = ERRID.ERR_UnrecognizedEnd;
				break;
			default:
				throw new ArgumentException("Statement must be an end block statement");
			}
			statement = Parser.ReportSyntaxError(statement, eRRID);
			return ProcessSyntax(statement);
		}

		internal override BlockContext EndBlock(StatementSyntax endStmt)
		{
			VisualBasicSyntaxNode syntax = CreateBlockSyntax(endStmt);
			return base.PrevBlock.ProcessSyntax(syntax);
		}

		internal override BlockContext ProcessStatementTerminator(BlockContext lambdaContext)
		{
			base.Parser.ConsumeStatementTerminator(colonAsSeparator: false);
			return this;
		}
	}
}
