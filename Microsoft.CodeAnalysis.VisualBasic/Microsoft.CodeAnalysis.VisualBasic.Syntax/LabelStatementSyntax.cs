using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LabelStatementSyntax : ExecutableStatementSyntax
	{
		public SyntaxToken LabelToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)base.Green)._labelToken, base.Position, 0);

		public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax)base.Green)._colonToken, GetChildPosition(1), GetChildIndex(1));

		internal LabelStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LabelStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken labelToken, PunctuationSyntax colonToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax(kind, errors, annotations, labelToken, colonToken), null, 0)
		{
		}

		public LabelStatementSyntax WithLabelToken(SyntaxToken labelToken)
		{
			return Update(labelToken, ColonToken);
		}

		public LabelStatementSyntax WithColonToken(SyntaxToken colonToken)
		{
			return Update(LabelToken, colonToken);
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
			return visitor.VisitLabelStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLabelStatement(this);
		}

		public LabelStatementSyntax Update(SyntaxToken labelToken, SyntaxToken colonToken)
		{
			if (labelToken != LabelToken || colonToken != ColonToken)
			{
				LabelStatementSyntax labelStatementSyntax = SyntaxFactory.LabelStatement(labelToken, colonToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(labelStatementSyntax, annotations);
				}
				return labelStatementSyntax;
			}
			return this;
		}
	}
}
