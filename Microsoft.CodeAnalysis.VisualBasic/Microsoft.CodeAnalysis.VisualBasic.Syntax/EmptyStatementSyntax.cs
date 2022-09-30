using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EmptyStatementSyntax : StatementSyntax
	{
		public SyntaxToken Empty => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax)base.Green)._empty, base.Position, 0);

		internal EmptyStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EmptyStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax empty)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax(kind, errors, annotations, empty), null, 0)
		{
		}

		public EmptyStatementSyntax WithEmpty(SyntaxToken empty)
		{
			return Update(empty);
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
			return visitor.VisitEmptyStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEmptyStatement(this);
		}

		public EmptyStatementSyntax Update(SyntaxToken empty)
		{
			if (empty != Empty)
			{
				EmptyStatementSyntax emptyStatementSyntax = SyntaxFactory.EmptyStatement(empty);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(emptyStatementSyntax, annotations);
				}
				return emptyStatementSyntax;
			}
			return this;
		}
	}
}
