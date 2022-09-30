using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class BlockContext : ISyntaxFactoryContext
	{
		[Flags]
		internal enum LinkResult
		{
			NotUsed = 0,
			Used = 1,
			SkipTerminator = 2,
			MissingTerminator = 4,
			TerminatorFlags = 6,
			Crumble = 8
		}

		private StatementSyntax _beginStatement;

		protected Parser _parser;

		protected SyntaxListBuilder<StatementSyntax> _statements;

		private readonly SyntaxKind _kind;

		private readonly SyntaxKind _endKind;

		private readonly BlockContext _prev;

		private readonly bool _isWithinMultiLineLambda;

		private readonly bool _isWithinSingleLineLambda;

		private readonly bool _isWithinAsyncMethodOrLambda;

		private readonly bool _isWithinIteratorMethodOrLambdaOrProperty;

		private readonly int _level;

		private readonly ContextAwareSyntaxFactory _syntaxFactory;

		internal StatementSyntax BeginStatement => _beginStatement;

		internal bool IsLineIf
		{
			get
			{
				if (_kind != SyntaxKind.SingleLineIfStatement)
				{
					return _kind == SyntaxKind.SingleLineElseClause;
				}
				return true;
			}
		}

		internal bool IsWithinLambda => _isWithinMultiLineLambda | _isWithinSingleLineLambda;

		internal bool IsWithinSingleLineLambda => _isWithinSingleLineLambda;

		internal virtual bool IsWithinAsyncMethodOrLambda => _isWithinAsyncMethodOrLambda;

		internal virtual bool IsWithinIteratorContext => _isWithinIteratorMethodOrLambdaOrProperty;

		internal bool IsWithinIteratorMethodOrLambdaOrProperty => _isWithinIteratorMethodOrLambdaOrProperty;

		internal Parser Parser
		{
			get
			{
				return _parser;
			}
			set
			{
				_parser = value;
			}
		}

		internal ContextAwareSyntaxFactory SyntaxFactory => _syntaxFactory;

		internal SyntaxKind BlockKind => _kind;

		internal BlockContext PrevBlock => _prev;

		internal int Level => _level;

		internal SyntaxListBuilder<StatementSyntax> Statements => _statements;

		internal abstract bool IsSingleLine { get; }

		internal virtual bool IsLambda => false;

		protected BlockContext(SyntaxKind kind, StatementSyntax statement, BlockContext prev)
		{
			_beginStatement = statement;
			_kind = kind;
			_prev = prev;
			_syntaxFactory = new ContextAwareSyntaxFactory(this);
			if (prev != null)
			{
				_isWithinSingleLineLambda = prev._isWithinSingleLineLambda;
				_isWithinMultiLineLambda = prev._isWithinMultiLineLambda;
			}
			if (!_isWithinSingleLineLambda)
			{
				_isWithinSingleLineLambda = SyntaxFacts.IsSingleLineLambdaExpression(_kind);
			}
			if (!_isWithinMultiLineLambda)
			{
				_isWithinMultiLineLambda = SyntaxFacts.IsMultiLineLambdaExpression(_kind);
			}
			switch (_kind)
			{
			case SyntaxKind.PropertyBlock:
				_isWithinIteratorMethodOrLambdaOrProperty = ((PropertyStatementSyntax)statement).Modifiers.Any(632);
				break;
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
				_isWithinIteratorMethodOrLambdaOrProperty = _prev.IsWithinIteratorMethodOrLambdaOrProperty;
				break;
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
				_isWithinAsyncMethodOrLambda = ((MethodStatementSyntax)statement).Modifiers.Any(630);
				_isWithinIteratorMethodOrLambdaOrProperty = ((MethodStatementSyntax)statement).Modifiers.Any(632);
				break;
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				_isWithinAsyncMethodOrLambda = ((LambdaHeaderSyntax)statement).Modifiers.Any(630);
				_isWithinIteratorMethodOrLambdaOrProperty = ((LambdaHeaderSyntax)statement).Modifiers.Any(632);
				break;
			default:
				if (_prev != null)
				{
					_isWithinAsyncMethodOrLambda = _prev.IsWithinAsyncMethodOrLambda;
					_isWithinIteratorMethodOrLambdaOrProperty = _prev.IsWithinIteratorMethodOrLambdaOrProperty;
				}
				break;
			}
			_endKind = GetEndKind(kind);
			_level = ((prev != null) ? (prev.Level + 1) : 0);
			if (prev != null)
			{
				_parser = prev.Parser;
				_statements = _parser._pool.Allocate<StatementSyntax>();
			}
		}

		internal void GetBeginEndStatements<T1, T2>(ref T1 beginStmt, ref T2 endStmt) where T1 : StatementSyntax where T2 : StatementSyntax
		{
			beginStmt = (T1)BeginStatement;
			if (endStmt == null)
			{
				ERRID errorId = default(ERRID);
				endStmt = (T2)CreateMissingEnd(ref errorId);
				if (errorId != 0)
				{
					beginStmt = Parser.ReportSyntaxError(beginStmt, errorId);
				}
			}
		}

		internal virtual bool KindEndsBlock(SyntaxKind kind)
		{
			return _endKind == kind;
		}

		internal void Add(VisualBasicSyntaxNode node)
		{
			_statements.Add((StatementSyntax)node);
		}

		internal void FreeStatements()
		{
			_parser._pool.Free(_statements);
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Body()
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> result = _statements.ToList();
			_statements.Clear();
			return result;
		}

		internal StatementSyntax SingleStatementOrDefault()
		{
			if (_statements.Count != 1)
			{
				return null;
			}
			return _statements[0];
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> OptionalBody()
		{
			StatementSyntax statementSyntax = SingleStatementOrDefault();
			if (statementSyntax != null && statementSyntax.Kind == SyntaxKind.EmptyStatement && statementSyntax.FullWidth == 0)
			{
				return default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>);
			}
			return Body();
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> Body<T>() where T : StatementSyntax
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> result = _statements.ToList<T>();
			_statements.Clear();
			return result;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> BodyWithWeakChildren()
		{
			if (IsLargeEnoughNonEmptyStatementList(_statements))
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> result = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(SyntaxList.List(((SyntaxListBuilder)_statements).ToArray()));
				_statements.Clear();
				return result;
			}
			return Body();
		}

		private static bool IsLargeEnoughNonEmptyStatementList(SyntaxListBuilder<StatementSyntax> statements)
		{
			if (statements.Count == 0)
			{
				return false;
			}
			if (statements.Count <= 2)
			{
				return statements[0]!.Width > 60;
			}
			return true;
		}

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> BaseDeclarations<T>() where T : InheritsOrImplementsStatementSyntax
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> result = _statements.ToList<T>();
			_statements.Clear();
			return result;
		}

		internal abstract StatementSyntax Parse();

		internal abstract BlockContext ProcessSyntax(VisualBasicSyntaxNode syntax);

		internal abstract VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax statement);

		internal abstract BlockContext EndBlock(StatementSyntax statement);

		internal abstract BlockContext RecoverFromMismatchedEnd(StatementSyntax statement);

		internal virtual BlockContext ResyncAndProcessStatementTerminator(StatementSyntax statement, BlockContext lambdaContext)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected = Parser.ResyncAt();
			HandleAnyUnexpectedTokens(statement, unexpected);
			return ProcessStatementTerminator(lambdaContext);
		}

		internal abstract BlockContext ProcessStatementTerminator(BlockContext lambdaContext);

		private void HandleAnyUnexpectedTokens(StatementSyntax currentStmt, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected)
		{
			if (unexpected.Node != null)
			{
				int num;
				StatementSyntax node;
				if (_statements.Count == 0)
				{
					num = -1;
					node = _beginStatement;
				}
				else
				{
					num = _statements.Count - 1;
					node = _statements[num];
				}
				node = ((currentStmt.ContainsDiagnostics || ParserExtensions.ContainsDiagnostics(unexpected)) ? SyntaxNodeExtensions.AddTrailingSyntax(node, unexpected) : SyntaxNodeExtensions.AddTrailingSyntax(node, unexpected, ERRID.ERR_ExpectedEOS));
				if (num == -1)
				{
					_beginStatement = node;
				}
				else
				{
					_statements[num] = node;
				}
			}
		}

		internal abstract LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext);

		internal BlockContext LinkSyntax(VisualBasicSyntaxNode node)
		{
			SyntaxKind kind = node.Kind;
			BlockContext blockContext = this;
			while (blockContext != null)
			{
				if (blockContext.KindEndsBlock(kind))
				{
					BlockContext blockContext2 = BlockContextExtensions.FindNearestLambdaOrSingleLineIf(this, blockContext);
					if (blockContext2 != null)
					{
						if (blockContext2.IsLambda)
						{
							break;
						}
						node = Parser.ReportSyntaxError(node, ERRID.ERR_BogusWithinLineIf);
						return ProcessSyntax(node);
					}
					if (blockContext != this)
					{
						BlockContextExtensions.RecoverFromMissingEnd(this, blockContext);
					}
					return blockContext.EndBlock((StatementSyntax)node);
				}
				if (SyntaxFacts.IsEndBlockLoopOrNextStatement(kind))
				{
					blockContext = blockContext.PrevBlock;
					continue;
				}
				return ProcessSyntax(node);
			}
			return RecoverFromMismatchedEnd((StatementSyntax)node);
		}

		internal LinkResult UseSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext, bool AddMissingTerminator = false)
		{
			Parser.GetNextSyntaxNode();
			newContext = LinkSyntax(node);
			if (AddMissingTerminator)
			{
				return LinkResult.Used | LinkResult.MissingTerminator;
			}
			return LinkResult.Used;
		}

		internal LinkResult TryUseStatement(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			if (node is StatementSyntax node2)
			{
				return UseSyntax(node2, ref newContext);
			}
			return LinkResult.NotUsed;
		}

		internal BlockContext TryProcessExecutableStatement(VisualBasicSyntaxNode node)
		{
			switch (node.Kind)
			{
			case SyntaxKind.SingleLineIfStatement:
				Add(node);
				break;
			case SyntaxKind.IfStatement:
			{
				IfStatementSyntax ifStatementSyntax = (IfStatementSyntax)node;
				if (ifStatementSyntax.ThenKeyword != null && !SyntaxFacts.IsTerminator(Parser.CurrentToken.Kind))
				{
					return new SingleLineIfBlockContext(ifStatementSyntax, this);
				}
				return new IfBlockContext(ifStatementSyntax, this);
			}
			case SyntaxKind.ElseStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_ElseNoMatchingIf));
				break;
			case SyntaxKind.ElseIfStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_ElseIfNoMatchingIf));
				break;
			case SyntaxKind.SimpleDoStatement:
			case SyntaxKind.DoWhileStatement:
			case SyntaxKind.DoUntilStatement:
				return new DoLoopBlockContext((StatementSyntax)node, this);
			case SyntaxKind.ForStatement:
			case SyntaxKind.ForEachStatement:
				return new ForBlockContext((StatementSyntax)node, this);
			case SyntaxKind.SelectStatement:
				return new SelectBlockContext((StatementSyntax)node, this);
			case SyntaxKind.CaseStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_CaseNoSelect));
				break;
			case SyntaxKind.CaseElseStatement:
				Add(Parser.ReportSyntaxError(node, ERRID.ERR_CaseElseNoSelect));
				break;
			case SyntaxKind.WhileStatement:
				return new StatementBlockContext(SyntaxKind.WhileBlock, (StatementSyntax)node, this);
			case SyntaxKind.WithStatement:
				return new StatementBlockContext(SyntaxKind.WithBlock, (StatementSyntax)node, this);
			case SyntaxKind.SyncLockStatement:
				return new StatementBlockContext(SyntaxKind.SyncLockBlock, (StatementSyntax)node, this);
			case SyntaxKind.UsingStatement:
				return new StatementBlockContext(SyntaxKind.UsingBlock, (StatementSyntax)node, this);
			case SyntaxKind.TryStatement:
				return new TryBlockContext((StatementSyntax)node, this);
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
			{
				BlockContext blockContext = BlockContextExtensions.FindNearestInSameMethodScope(this, SyntaxKind.TryBlock, SyntaxKind.CatchBlock, SyntaxKind.FinallyBlock);
				if (blockContext != null)
				{
					BlockContextExtensions.RecoverFromMissingEnd(this, blockContext);
					return blockContext.ProcessSyntax((StatementSyntax)node);
				}
				Add(Parser.ReportSyntaxError(node, (node.Kind == SyntaxKind.CatchStatement) ? ERRID.ERR_CatchNoMatchingTry : ERRID.ERR_FinallyNoMatchingTry));
				break;
			}
			case SyntaxKind.WhileBlock:
			case SyntaxKind.UsingBlock:
			case SyntaxKind.SyncLockBlock:
			case SyntaxKind.WithBlock:
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.TryBlock:
			case SyntaxKind.SelectBlock:
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				Add(node);
				break;
			default:
				if (!(node is ExecutableStatementSyntax))
				{
					return null;
				}
				Add(node);
				break;
			}
			return this;
		}

		internal LinkResult TryLinkStatement(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			newContext = null;
			switch (node.Kind)
			{
			case SyntaxKind.SelectBlock:
				return UseSyntax(node, ref newContext, ((SelectBlockSyntax)node).EndSelectStatement.IsMissing);
			case SyntaxKind.WhileBlock:
				return UseSyntax(node, ref newContext, ((WhileBlockSyntax)node).EndWhileStatement.IsMissing);
			case SyntaxKind.WithBlock:
				return UseSyntax(node, ref newContext, ((WithBlockSyntax)node).EndWithStatement.IsMissing);
			case SyntaxKind.SyncLockBlock:
				return UseSyntax(node, ref newContext, ((SyncLockBlockSyntax)node).EndSyncLockStatement.IsMissing);
			case SyntaxKind.UsingBlock:
				return UseSyntax(node, ref newContext, ((UsingBlockSyntax)node).EndUsingStatement.IsMissing);
			case SyntaxKind.TryBlock:
				return UseSyntax(node, ref newContext, ((TryBlockSyntax)node).EndTryStatement.IsMissing);
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				return UseSyntax(node, ref newContext, ((DoLoopBlockSyntax)node).LoopStatement.IsMissing);
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
				newContext = this;
				return LinkResult.Crumble;
			case SyntaxKind.SingleLineIfStatement:
				return UseSyntax(node, ref newContext);
			case SyntaxKind.MultiLineIfBlock:
				return UseSyntax(node, ref newContext, ((MultiLineIfBlockSyntax)node).EndIfStatement.IsMissing);
			case SyntaxKind.NextStatement:
				newContext = this;
				return LinkResult.NotUsed;
			default:
				return TryUseStatement(node, ref newContext);
			}
		}

		private StatementSyntax CreateMissingEnd(ref ERRID errorId)
		{
			return CreateMissingEnd(BlockKind, ref errorId);
		}

		private StatementSyntax CreateMissingEnd(SyntaxKind kind, ref ERRID errorId)
		{
			KeywordSyntax endKeyword = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EndKeyword);
			StatementSyntax result;
			switch (kind)
			{
			case SyntaxKind.NamespaceBlock:
				result = SyntaxFactory.EndNamespaceStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.NamespaceKeyword));
				errorId = ERRID.ERR_ExpectedEndNamespace;
				break;
			case SyntaxKind.ModuleBlock:
				result = SyntaxFactory.EndModuleStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ModuleKeyword));
				errorId = ERRID.ERR_ExpectedEndModule;
				break;
			case SyntaxKind.ClassBlock:
				result = SyntaxFactory.EndClassStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.ClassKeyword));
				errorId = ERRID.ERR_ExpectedEndClass;
				break;
			case SyntaxKind.StructureBlock:
				result = SyntaxFactory.EndStructureStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.StructureKeyword));
				errorId = ERRID.ERR_ExpectedEndStructure;
				break;
			case SyntaxKind.InterfaceBlock:
				result = SyntaxFactory.EndInterfaceStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.InterfaceKeyword));
				errorId = ERRID.ERR_MissingEndInterface;
				break;
			case SyntaxKind.EnumBlock:
				result = SyntaxFactory.EndEnumStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EnumKeyword));
				errorId = ERRID.ERR_MissingEndEnum;
				break;
			case SyntaxKind.SubBlock:
			case SyntaxKind.ConstructorBlock:
				result = SyntaxFactory.EndSubStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword));
				errorId = ERRID.ERR_EndSubExpected;
				break;
			case SyntaxKind.MultiLineSubLambdaExpression:
				result = SyntaxFactory.EndSubStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SubKeyword));
				errorId = ERRID.ERR_MultilineLambdaMissingSub;
				break;
			case SyntaxKind.FunctionBlock:
				result = SyntaxFactory.EndFunctionStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.FunctionKeyword));
				errorId = ERRID.ERR_EndFunctionExpected;
				break;
			case SyntaxKind.MultiLineFunctionLambdaExpression:
				result = SyntaxFactory.EndFunctionStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.FunctionKeyword));
				errorId = ERRID.ERR_MultilineLambdaMissingFunction;
				break;
			case SyntaxKind.OperatorBlock:
				result = SyntaxFactory.EndOperatorStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.OperatorKeyword));
				errorId = ERRID.ERR_EndOperatorExpected;
				break;
			case SyntaxKind.PropertyBlock:
				result = SyntaxFactory.EndPropertyStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.PropertyKeyword));
				errorId = ERRID.ERR_EndProp;
				break;
			case SyntaxKind.GetAccessorBlock:
				result = SyntaxFactory.EndGetStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.GetKeyword));
				errorId = ERRID.ERR_MissingEndGet;
				break;
			case SyntaxKind.SetAccessorBlock:
				result = SyntaxFactory.EndSetStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SetKeyword));
				errorId = ERRID.ERR_MissingEndSet;
				break;
			case SyntaxKind.EventBlock:
				result = SyntaxFactory.EndEventStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.EventKeyword));
				errorId = ERRID.ERR_MissingEndEvent;
				break;
			case SyntaxKind.AddHandlerAccessorBlock:
				result = SyntaxFactory.EndAddHandlerStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.AddHandlerKeyword));
				errorId = ERRID.ERR_MissingEndAddHandler;
				break;
			case SyntaxKind.RemoveHandlerAccessorBlock:
				result = SyntaxFactory.EndRemoveHandlerStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.RemoveHandlerKeyword));
				errorId = ERRID.ERR_MissingEndRemoveHandler;
				break;
			case SyntaxKind.RaiseEventAccessorBlock:
				result = SyntaxFactory.EndRaiseEventStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.RaiseEventKeyword));
				errorId = ERRID.ERR_MissingEndRaiseEvent;
				break;
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
				result = SyntaxFactory.EndIfStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.IfKeyword));
				errorId = ERRID.ERR_ExpectedEndIf;
				break;
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
				result = SyntaxFactory.SimpleLoopStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.LoopKeyword), null);
				errorId = ERRID.ERR_ExpectedLoop;
				break;
			case SyntaxKind.WhileBlock:
				result = SyntaxFactory.EndWhileStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WhileKeyword));
				errorId = ERRID.ERR_ExpectedEndWhile;
				break;
			case SyntaxKind.WithBlock:
				result = SyntaxFactory.EndWithStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.WithKeyword));
				errorId = ERRID.ERR_ExpectedEndWith;
				break;
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
				result = SyntaxFactory.NextStatement(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.NextKeyword), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode>));
				errorId = ERRID.ERR_ExpectedNext;
				break;
			case SyntaxKind.SyncLockBlock:
				result = SyntaxFactory.EndSyncLockStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SyncLockKeyword));
				errorId = ERRID.ERR_ExpectedEndSyncLock;
				break;
			case SyntaxKind.SelectBlock:
				result = SyntaxFactory.EndSelectStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.SelectKeyword));
				errorId = ERRID.ERR_ExpectedEndSelect;
				break;
			case SyntaxKind.TryBlock:
				result = SyntaxFactory.EndTryStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.TryKeyword));
				errorId = ERRID.ERR_ExpectedEndTry;
				break;
			case SyntaxKind.UsingBlock:
				result = SyntaxFactory.EndUsingStatement(endKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.MissingKeyword(SyntaxKind.UsingKeyword));
				errorId = ERRID.ERR_ExpectedEndUsing;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			}
			return result;
		}

		private static SyntaxKind GetEndKind(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.CompilationUnit:
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
				return SyntaxKind.None;
			case SyntaxKind.NamespaceBlock:
				return SyntaxKind.EndNamespaceStatement;
			case SyntaxKind.ModuleBlock:
				return SyntaxKind.EndModuleStatement;
			case SyntaxKind.ClassBlock:
				return SyntaxKind.EndClassStatement;
			case SyntaxKind.StructureBlock:
				return SyntaxKind.EndStructureStatement;
			case SyntaxKind.InterfaceBlock:
				return SyntaxKind.EndInterfaceStatement;
			case SyntaxKind.EnumBlock:
				return SyntaxKind.EndEnumStatement;
			case SyntaxKind.SubBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.MultiLineSubLambdaExpression:
				return SyntaxKind.EndSubStatement;
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
				return SyntaxKind.EndFunctionStatement;
			case SyntaxKind.OperatorBlock:
				return SyntaxKind.EndOperatorStatement;
			case SyntaxKind.PropertyBlock:
				return SyntaxKind.EndPropertyStatement;
			case SyntaxKind.GetAccessorBlock:
				return SyntaxKind.EndGetStatement;
			case SyntaxKind.SetAccessorBlock:
				return SyntaxKind.EndSetStatement;
			case SyntaxKind.EventBlock:
				return SyntaxKind.EndEventStatement;
			case SyntaxKind.AddHandlerAccessorBlock:
				return SyntaxKind.EndAddHandlerStatement;
			case SyntaxKind.RemoveHandlerAccessorBlock:
				return SyntaxKind.EndRemoveHandlerStatement;
			case SyntaxKind.RaiseEventAccessorBlock:
				return SyntaxKind.EndRaiseEventStatement;
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
				return SyntaxKind.EndIfStatement;
			case SyntaxKind.SingleLineIfStatement:
			case SyntaxKind.SingleLineElseClause:
				return SyntaxKind.None;
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
				return SyntaxKind.SimpleLoopStatement;
			case SyntaxKind.WhileBlock:
				return SyntaxKind.EndWhileStatement;
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
				return SyntaxKind.NextStatement;
			case SyntaxKind.WithBlock:
				return SyntaxKind.EndWithStatement;
			case SyntaxKind.SyncLockBlock:
				return SyntaxKind.EndSyncLockStatement;
			case SyntaxKind.SelectBlock:
			case SyntaxKind.CaseBlock:
			case SyntaxKind.CaseElseBlock:
				return SyntaxKind.EndSelectStatement;
			case SyntaxKind.TryBlock:
			case SyntaxKind.CatchBlock:
			case SyntaxKind.FinallyBlock:
				return SyntaxKind.EndTryStatement;
			case SyntaxKind.UsingBlock:
				return SyntaxKind.EndUsingStatement;
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			}
		}
	}
}
