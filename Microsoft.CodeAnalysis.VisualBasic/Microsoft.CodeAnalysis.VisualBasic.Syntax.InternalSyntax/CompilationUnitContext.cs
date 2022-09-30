using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CompilationUnitContext : NamespaceBlockContext
	{
		private class DiagnosticRewriter : VisualBasicSyntaxRewriter
		{
			private HashSet<IfDirectiveTriviaSyntax> _notClosedIfDirectives;

			private HashSet<RegionDirectiveTriviaSyntax> _notClosedRegionDirectives;

			private ExternalSourceDirectiveTriviaSyntax _notClosedExternalSourceDirective;

			private bool _regionsAreAllowedEverywhere;

			private Parser _parser;

			private ArrayBuilder<VisualBasicSyntaxNode> _declarationBlocksBeingVisited;

			private ArrayBuilder<VisualBasicSyntaxNode> _parentsOfRegionDirectivesAwaitingClosure;

			private SyntaxToken _tokenWithDirectivesBeingVisited;

			private DiagnosticRewriter()
			{
				_notClosedIfDirectives = null;
				_notClosedRegionDirectives = null;
				_notClosedExternalSourceDirective = null;
			}

			public static CompilationUnitSyntax Rewrite(CompilationUnitSyntax compilationUnit, ArrayBuilder<IfDirectiveTriviaSyntax> notClosedIfDirectives, ArrayBuilder<RegionDirectiveTriviaSyntax> notClosedRegionDirectives, bool regionsAreAllowedEverywhere, ExternalSourceDirectiveTriviaSyntax notClosedExternalSourceDirective, Parser parser)
			{
				DiagnosticRewriter diagnosticRewriter = new DiagnosticRewriter();
				if (notClosedIfDirectives != null)
				{
					diagnosticRewriter._notClosedIfDirectives = new HashSet<IfDirectiveTriviaSyntax>(ReferenceEqualityComparer.Instance);
					ArrayBuilder<IfDirectiveTriviaSyntax>.Enumerator enumerator = notClosedIfDirectives.GetEnumerator();
					while (enumerator.MoveNext())
					{
						IfDirectiveTriviaSyntax current = enumerator.Current;
						diagnosticRewriter._notClosedIfDirectives.Add(current);
					}
				}
				if (notClosedRegionDirectives != null)
				{
					diagnosticRewriter._notClosedRegionDirectives = new HashSet<RegionDirectiveTriviaSyntax>(ReferenceEqualityComparer.Instance);
					ArrayBuilder<RegionDirectiveTriviaSyntax>.Enumerator enumerator2 = notClosedRegionDirectives.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						RegionDirectiveTriviaSyntax current2 = enumerator2.Current;
						diagnosticRewriter._notClosedRegionDirectives.Add(current2);
					}
				}
				diagnosticRewriter._parser = parser;
				diagnosticRewriter._regionsAreAllowedEverywhere = regionsAreAllowedEverywhere;
				if (!regionsAreAllowedEverywhere)
				{
					diagnosticRewriter._declarationBlocksBeingVisited = ArrayBuilder<VisualBasicSyntaxNode>.GetInstance();
					diagnosticRewriter._parentsOfRegionDirectivesAwaitingClosure = ArrayBuilder<VisualBasicSyntaxNode>.GetInstance();
				}
				diagnosticRewriter._notClosedExternalSourceDirective = notClosedExternalSourceDirective;
				CompilationUnitSyntax result = (CompilationUnitSyntax)diagnosticRewriter.Visit(compilationUnit);
				if (!regionsAreAllowedEverywhere)
				{
					diagnosticRewriter._declarationBlocksBeingVisited.Free();
					diagnosticRewriter._parentsOfRegionDirectivesAwaitingClosure.Free();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitCompilationUnit(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitMethodBlock(MethodBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitMethodBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitConstructorBlock(ConstructorBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitConstructorBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitOperatorBlock(OperatorBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitOperatorBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitAccessorBlock(AccessorBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitAccessorBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitNamespaceBlock(NamespaceBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitNamespaceBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitClassBlock(ClassBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitClassBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitStructureBlock(StructureBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitStructureBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitModuleBlock(ModuleBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitModuleBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitInterfaceBlock(InterfaceBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitInterfaceBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitEnumBlock(EnumBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitEnumBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitPropertyBlock(PropertyBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitPropertyBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitEventBlock(EventBlockSyntax node)
			{
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Push(node);
				}
				VisualBasicSyntaxNode result = base.VisitEventBlock(node);
				if (_declarationBlocksBeingVisited != null)
				{
					_declarationBlocksBeingVisited.Pop();
				}
				return result;
			}

			public override VisualBasicSyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = base.VisitIfDirectiveTrivia(node);
				if (_notClosedIfDirectives != null && _notClosedIfDirectives.Contains(node))
				{
					visualBasicSyntaxNode = Parser.ReportSyntaxError(visualBasicSyntaxNode, ERRID.ERR_LbExpectedEndIf);
				}
				return visualBasicSyntaxNode;
			}

			public override VisualBasicSyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = base.VisitRegionDirectiveTrivia(node);
				if (_notClosedRegionDirectives != null && _notClosedRegionDirectives.Contains(node))
				{
					visualBasicSyntaxNode = Parser.ReportSyntaxError(visualBasicSyntaxNode, ERRID.ERR_ExpectedEndRegion);
				}
				else if (!_regionsAreAllowedEverywhere)
				{
					visualBasicSyntaxNode = VerifyRegionPlacement(node, visualBasicSyntaxNode);
				}
				return visualBasicSyntaxNode;
			}

			private VisualBasicSyntaxNode VerifyRegionPlacement(VisualBasicSyntaxNode original, VisualBasicSyntaxNode rewritten)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = _declarationBlocksBeingVisited.Peek();
				if (_declarationBlocksBeingVisited.Count > 1)
				{
					if (_tokenWithDirectivesBeingVisited == visualBasicSyntaxNode.GetFirstToken())
					{
						GreenNode leadingTrivia = _tokenWithDirectivesBeingVisited.GetLeadingTrivia();
						if (leadingTrivia != null && new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(leadingTrivia).Nodes.Contains(original))
						{
							visualBasicSyntaxNode = _declarationBlocksBeingVisited[_declarationBlocksBeingVisited.Count - 2];
						}
					}
					else if (_tokenWithDirectivesBeingVisited == visualBasicSyntaxNode.GetLastToken())
					{
						GreenNode trailingTrivia = _tokenWithDirectivesBeingVisited.GetTrailingTrivia();
						if (trailingTrivia != null && new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(trailingTrivia).Nodes.Contains(original))
						{
							visualBasicSyntaxNode = _declarationBlocksBeingVisited[_declarationBlocksBeingVisited.Count - 2];
						}
					}
				}
				bool flag = !IsValidContainingBlockForRegionInVB12(visualBasicSyntaxNode);
				if (original.Kind == SyntaxKind.RegionDirectiveTrivia)
				{
					_parentsOfRegionDirectivesAwaitingClosure.Push(visualBasicSyntaxNode);
				}
				else if (_parentsOfRegionDirectivesAwaitingClosure.Count > 0)
				{
					VisualBasicSyntaxNode visualBasicSyntaxNode2 = _parentsOfRegionDirectivesAwaitingClosure.Pop();
					if (visualBasicSyntaxNode2 != visualBasicSyntaxNode && IsValidContainingBlockForRegionInVB12(visualBasicSyntaxNode2))
					{
						flag = true;
					}
				}
				if (flag)
				{
					rewritten = _parser.ReportFeatureUnavailable(Feature.RegionsEverywhere, rewritten);
				}
				return rewritten;
			}

			private static bool IsValidContainingBlockForRegionInVB12(VisualBasicSyntaxNode containingBlock)
			{
				SyntaxKind kind = containingBlock.Kind;
				if (kind - 79 <= SyntaxKind.EndSelectStatement)
				{
					return false;
				}
				return true;
			}

			public override VisualBasicSyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = base.VisitEndRegionDirectiveTrivia(node);
				if (!_regionsAreAllowedEverywhere)
				{
					visualBasicSyntaxNode = VerifyRegionPlacement(node, visualBasicSyntaxNode);
				}
				return visualBasicSyntaxNode;
			}

			public override VisualBasicSyntaxNode VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = base.VisitExternalSourceDirectiveTrivia(node);
				if (_notClosedExternalSourceDirective == node)
				{
					visualBasicSyntaxNode = Parser.ReportSyntaxError(visualBasicSyntaxNode, ERRID.ERR_ExpectedEndExternalSource);
				}
				return visualBasicSyntaxNode;
			}

			public override VisualBasicSyntaxNode Visit(VisualBasicSyntaxNode node)
			{
				if (node == null || !node.ContainsDirectives)
				{
					return node;
				}
				return node.Accept(this);
			}

			public override SyntaxToken VisitSyntaxToken(SyntaxToken token)
			{
				if (token == null || !token.ContainsDirectives)
				{
					return token;
				}
				_tokenWithDirectivesBeingVisited = token;
				GreenNode leadingTrivia = token.GetLeadingTrivia();
				GreenNode trailingTrivia = token.GetTrailingTrivia();
				if (leadingTrivia != null)
				{
					GreenNode node = VisitList(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(leadingTrivia)).Node;
					if (leadingTrivia != node)
					{
						token = (SyntaxToken)token.WithLeadingTrivia(node);
					}
				}
				if (trailingTrivia != null)
				{
					GreenNode node2 = VisitList(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(trailingTrivia)).Node;
					if (trailingTrivia != node2)
					{
						token = (SyntaxToken)token.WithTrailingTrivia(node2);
					}
				}
				_tokenWithDirectivesBeingVisited = null;
				return token;
			}
		}

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax> _optionStmts;

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax> _importsStmts;

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax> _attributeStmts;

		private SyntaxKind _state;

		internal override bool IsWithinAsyncMethodOrLambda => base.Parser.IsScript;

		internal CompilationUnitContext(Parser parser)
			: base(SyntaxKind.CompilationUnit, null, null)
		{
			base.Parser = parser;
			_statements = _parser._pool.Allocate<StatementSyntax>();
			_state = SyntaxKind.OptionStatement;
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			while (true)
			{
				switch (_state)
				{
				case SyntaxKind.OptionStatement:
					if (node.Kind == SyntaxKind.OptionStatement)
					{
						Add(node);
						return this;
					}
					_optionStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax>(Body().Node);
					_state = SyntaxKind.ImportsStatement;
					continue;
				case SyntaxKind.ImportsStatement:
					if (node.Kind == SyntaxKind.ImportsStatement)
					{
						Add(node);
						return this;
					}
					_importsStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax>(Body().Node);
					_state = SyntaxKind.AttributesStatement;
					continue;
				case SyntaxKind.AttributesStatement:
					if (node.Kind == SyntaxKind.AttributesStatement)
					{
						Add(node);
						return this;
					}
					_attributeStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax>(Body().Node);
					_state = SyntaxKind.None;
					continue;
				}
				if (_parser.IsScript)
				{
					BlockContext blockContext = TryProcessExecutableStatement(node);
					if (blockContext != null)
					{
						return blockContext;
					}
				}
				return base.ProcessSyntax(node);
			}
		}

		internal override LinkResult TryLinkSyntax(VisualBasicSyntaxNode node, ref BlockContext newContext)
		{
			if (_parser.IsScript)
			{
				return TryLinkStatement(node, ref newContext);
			}
			return base.TryLinkSyntax(node, ref newContext);
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal CompilationUnitSyntax CreateCompilationUnit(PunctuationSyntax optionalTerminator, ArrayBuilder<IfDirectiveTriviaSyntax> notClosedIfDirectives, ArrayBuilder<RegionDirectiveTriviaSyntax> notClosedRegionDirectives, bool haveRegionDirectives, ExternalSourceDirectiveTriviaSyntax notClosedExternalSourceDirective)
		{
			if (_state != 0)
			{
				switch (_state)
				{
				case SyntaxKind.OptionStatement:
					_optionStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax>(Body().Node);
					break;
				case SyntaxKind.ImportsStatement:
					_importsStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax>(Body().Node);
					break;
				case SyntaxKind.AttributesStatement:
					_attributeStmts = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax>(Body().Node);
					break;
				}
				_state = SyntaxKind.None;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> syntaxList = Body();
			CompilationUnitSyntax compilationUnitSyntax = base.SyntaxFactory.CompilationUnit(_optionStmts, _importsStmts, _attributeStmts, syntaxList, optionalTerminator);
			bool flag = !haveRegionDirectives || base.Parser.CheckFeatureAvailability(Feature.RegionsEverywhere);
			if (notClosedIfDirectives != null || notClosedRegionDirectives != null || notClosedExternalSourceDirective != null || !flag)
			{
				compilationUnitSyntax = DiagnosticRewriter.Rewrite(compilationUnitSyntax, notClosedIfDirectives, notClosedRegionDirectives, flag, notClosedExternalSourceDirective, base.Parser);
				notClosedIfDirectives?.Free();
				notClosedRegionDirectives?.Free();
			}
			FreeStatements();
			return compilationUnitSyntax;
		}
	}
}
