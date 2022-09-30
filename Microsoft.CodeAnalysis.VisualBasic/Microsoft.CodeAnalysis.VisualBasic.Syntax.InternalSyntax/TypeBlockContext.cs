using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class TypeBlockContext : DeclarationContext
	{
		protected Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> _inheritsDecls;

		private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> _implementsDecls;

		protected SyntaxKind _state;

		internal TypeBlockContext(SyntaxKind contextKind, StatementSyntax statement, BlockContext prevContext)
			: base(contextKind, statement, prevContext)
		{
			_state = SyntaxKind.None;
		}

		internal override BlockContext ProcessSyntax(VisualBasicSyntaxNode node)
		{
			while (true)
			{
				switch (_state)
				{
				case SyntaxKind.None:
					switch (node.Kind)
					{
					case SyntaxKind.InheritsStatement:
						_state = SyntaxKind.InheritsStatement;
						break;
					case SyntaxKind.ImplementsStatement:
						_state = SyntaxKind.ImplementsStatement;
						break;
					default:
						_state = SyntaxKind.ClassStatement;
						break;
					}
					continue;
				case SyntaxKind.InheritsStatement:
				{
					SyntaxKind kind2 = node.Kind;
					if (kind2 == SyntaxKind.InheritsStatement)
					{
						Add(node);
						break;
					}
					_inheritsDecls = BaseDeclarations<InheritsStatementSyntax>();
					_state = SyntaxKind.ImplementsStatement;
					continue;
				}
				case SyntaxKind.ImplementsStatement:
				{
					SyntaxKind kind = node.Kind;
					if (kind == SyntaxKind.ImplementsStatement)
					{
						Add(node);
						break;
					}
					_implementsDecls = BaseDeclarations<ImplementsStatementSyntax>();
					_state = SyntaxKind.ClassStatement;
					continue;
				}
				default:
					return base.ProcessSyntax(node);
				}
				break;
			}
			return this;
		}

		internal override VisualBasicSyntaxNode CreateBlockSyntax(StatementSyntax endStmt)
		{
			TypeStatementSyntax beginStmt = null;
			EndBlockStatementSyntax endStmt2 = (EndBlockStatementSyntax)endStmt;
			GetBeginEndStatements(ref beginStmt, ref endStmt2);
			if (_state != SyntaxKind.ClassStatement)
			{
				switch (_state)
				{
				case SyntaxKind.InheritsStatement:
					_inheritsDecls = BaseDeclarations<InheritsStatementSyntax>();
					break;
				case SyntaxKind.ImplementsStatement:
					_implementsDecls = BaseDeclarations<ImplementsStatementSyntax>();
					break;
				}
				_state = SyntaxKind.ClassStatement;
			}
			TypeBlockSyntax result = Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxFactory.TypeBlock(base.BlockKind, beginStmt, _inheritsDecls, _implementsDecls, Body(), endStmt2);
			FreeStatements();
			return result;
		}
	}
}
