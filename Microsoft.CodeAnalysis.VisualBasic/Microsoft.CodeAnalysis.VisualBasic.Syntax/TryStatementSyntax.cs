using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TryStatementSyntax : StatementSyntax
	{
		public SyntaxToken TryKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)base.Green)._tryKeyword, base.Position, 0);

		internal TryStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TryStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax tryKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax(kind, errors, annotations, tryKeyword), null, 0)
		{
		}

		public TryStatementSyntax WithTryKeyword(SyntaxToken tryKeyword)
		{
			return Update(tryKeyword);
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
			return visitor.VisitTryStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTryStatement(this);
		}

		public TryStatementSyntax Update(SyntaxToken tryKeyword)
		{
			if (tryKeyword != TryKeyword)
			{
				TryStatementSyntax tryStatementSyntax = SyntaxFactory.TryStatement(tryKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(tryStatementSyntax, annotations);
				}
				return tryStatementSyntax;
			}
			return this;
		}
	}
}
