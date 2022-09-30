using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LabelSyntax : ExpressionSyntax
	{
		public SyntaxToken LabelToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)base.Green)._labelToken, base.Position, 0);

		internal LabelSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LabelSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken labelToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax(kind, errors, annotations, labelToken), null, 0)
		{
		}

		public LabelSyntax WithLabelToken(SyntaxToken labelToken)
		{
			return Update(Kind(), labelToken);
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
			return visitor.VisitLabel(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLabel(this);
		}

		public LabelSyntax Update(SyntaxKind kind, SyntaxToken labelToken)
		{
			if (kind != Kind() || labelToken != LabelToken)
			{
				LabelSyntax labelSyntax = SyntaxFactory.Label(kind, labelToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(labelSyntax, annotations);
				}
				return labelSyntax;
			}
			return this;
		}
	}
}
