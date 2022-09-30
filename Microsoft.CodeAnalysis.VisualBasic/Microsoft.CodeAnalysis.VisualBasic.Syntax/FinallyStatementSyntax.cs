using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class FinallyStatementSyntax : StatementSyntax
	{
		public SyntaxToken FinallyKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)base.Green)._finallyKeyword, base.Position, 0);

		internal FinallyStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal FinallyStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax finallyKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax(kind, errors, annotations, finallyKeyword), null, 0)
		{
		}

		public FinallyStatementSyntax WithFinallyKeyword(SyntaxToken finallyKeyword)
		{
			return Update(finallyKeyword);
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
			return visitor.VisitFinallyStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitFinallyStatement(this);
		}

		public FinallyStatementSyntax Update(SyntaxToken finallyKeyword)
		{
			if (finallyKeyword != FinallyKeyword)
			{
				FinallyStatementSyntax finallyStatementSyntax = SyntaxFactory.FinallyStatement(finallyKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(finallyStatementSyntax, annotations);
				}
				return finallyStatementSyntax;
			}
			return this;
		}
	}
}
