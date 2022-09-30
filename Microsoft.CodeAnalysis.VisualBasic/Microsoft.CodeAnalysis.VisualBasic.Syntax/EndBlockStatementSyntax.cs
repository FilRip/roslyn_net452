using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EndBlockStatementSyntax : DeclarationStatementSyntax
	{
		public SyntaxToken EndKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)base.Green)._endKeyword, base.Position, 0);

		public SyntaxToken BlockKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)base.Green)._blockKeyword, GetChildPosition(1), GetChildIndex(1));

		internal EndBlockStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EndBlockStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax(kind, errors, annotations, endKeyword, blockKeyword), null, 0)
		{
		}

		public EndBlockStatementSyntax WithEndKeyword(SyntaxToken endKeyword)
		{
			return Update(Kind(), endKeyword, BlockKeyword);
		}

		public EndBlockStatementSyntax WithBlockKeyword(SyntaxToken blockKeyword)
		{
			return Update(Kind(), EndKeyword, blockKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEndBlockStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEndBlockStatement(this);
		}

		public EndBlockStatementSyntax Update(SyntaxKind kind, SyntaxToken endKeyword, SyntaxToken blockKeyword)
		{
			if (kind != Kind() || endKeyword != EndKeyword || blockKeyword != BlockKeyword)
			{
				EndBlockStatementSyntax endBlockStatementSyntax = SyntaxFactory.EndBlockStatement(kind, endKeyword, blockKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(endBlockStatementSyntax, annotations);
				}
				return endBlockStatementSyntax;
			}
			return this;
		}
	}
}
