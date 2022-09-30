using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterpolationAlignmentClauseSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _value;

		public SyntaxToken CommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax)base.Green)._commaToken, base.Position, 0);

		public ExpressionSyntax Value => GetRed(ref _value, 1);

		internal InterpolationAlignmentClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterpolationAlignmentClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax commaToken, ExpressionSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax(kind, errors, annotations, commaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)value.Green), null, 0)
		{
		}

		public InterpolationAlignmentClauseSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return Update(commaToken, Value);
		}

		public InterpolationAlignmentClauseSyntax WithValue(ExpressionSyntax value)
		{
			return Update(CommaToken, value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _value;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Value;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInterpolationAlignmentClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterpolationAlignmentClause(this);
		}

		public InterpolationAlignmentClauseSyntax Update(SyntaxToken commaToken, ExpressionSyntax value)
		{
			if (commaToken != CommaToken || value != Value)
			{
				InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = SyntaxFactory.InterpolationAlignmentClause(commaToken, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interpolationAlignmentClauseSyntax, annotations);
				}
				return interpolationAlignmentClauseSyntax;
			}
			return this;
		}
	}
}
