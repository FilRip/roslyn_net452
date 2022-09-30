using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterpolationFormatClauseSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)base.Green)._colonToken, base.Position, 0);

		public SyntaxToken FormatStringToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax)base.Green)._formatStringToken, GetChildPosition(1), GetChildIndex(1));

		internal InterpolationFormatClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterpolationFormatClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax(kind, errors, annotations, colonToken, formatStringToken), null, 0)
		{
		}

		public InterpolationFormatClauseSyntax WithColonToken(SyntaxToken colonToken)
		{
			return Update(colonToken, FormatStringToken);
		}

		public InterpolationFormatClauseSyntax WithFormatStringToken(SyntaxToken formatStringToken)
		{
			return Update(ColonToken, formatStringToken);
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
			return visitor.VisitInterpolationFormatClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterpolationFormatClause(this);
		}

		public InterpolationFormatClauseSyntax Update(SyntaxToken colonToken, SyntaxToken formatStringToken)
		{
			if (colonToken != ColonToken || formatStringToken != FormatStringToken)
			{
				InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interpolationFormatClauseSyntax, annotations);
				}
				return interpolationFormatClauseSyntax;
			}
			return this;
		}
	}
}
