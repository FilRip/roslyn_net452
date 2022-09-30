using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterpolatedStringTextSyntax : InterpolatedStringContentSyntax
	{
		public SyntaxToken TextToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax)base.Green)._textToken, base.Position, 0);

		internal InterpolatedStringTextSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterpolatedStringTextSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, InterpolatedStringTextTokenSyntax textToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax(kind, errors, annotations, textToken), null, 0)
		{
		}

		public InterpolatedStringTextSyntax WithTextToken(SyntaxToken textToken)
		{
			return Update(textToken);
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
			return visitor.VisitInterpolatedStringText(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterpolatedStringText(this);
		}

		public InterpolatedStringTextSyntax Update(SyntaxToken textToken)
		{
			if (textToken != TextToken)
			{
				InterpolatedStringTextSyntax interpolatedStringTextSyntax = SyntaxFactory.InterpolatedStringText(textToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interpolatedStringTextSyntax, annotations);
				}
				return interpolatedStringTextSyntax;
			}
			return this;
		}
	}
}
