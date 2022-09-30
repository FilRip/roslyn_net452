using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
	{
		internal SyntaxNode _contents;

		public SyntaxToken DollarSignDoubleQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)base.Green)._dollarSignDoubleQuoteToken, base.Position, 0);

		public SyntaxList<InterpolatedStringContentSyntax> Contents
		{
			get
			{
				SyntaxNode red = GetRed(ref _contents, 1);
				return new SyntaxList<InterpolatedStringContentSyntax>(red);
			}
		}

		public SyntaxToken DoubleQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax)base.Green)._doubleQuoteToken, GetChildPosition(2), GetChildIndex(2));

		internal InterpolatedStringExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InterpolatedStringExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax dollarSignDoubleQuoteToken, SyntaxNode contents, PunctuationSyntax doubleQuoteToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax(kind, errors, annotations, dollarSignDoubleQuoteToken, contents?.Green, doubleQuoteToken), null, 0)
		{
		}

		public InterpolatedStringExpressionSyntax WithDollarSignDoubleQuoteToken(SyntaxToken dollarSignDoubleQuoteToken)
		{
			return Update(dollarSignDoubleQuoteToken, Contents, DoubleQuoteToken);
		}

		public InterpolatedStringExpressionSyntax WithContents(SyntaxList<InterpolatedStringContentSyntax> contents)
		{
			return Update(DollarSignDoubleQuoteToken, contents, DoubleQuoteToken);
		}

		public InterpolatedStringExpressionSyntax AddContents(params InterpolatedStringContentSyntax[] items)
		{
			return WithContents(Contents.AddRange(items));
		}

		public InterpolatedStringExpressionSyntax WithDoubleQuoteToken(SyntaxToken doubleQuoteToken)
		{
			return Update(DollarSignDoubleQuoteToken, Contents, doubleQuoteToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _contents;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _contents, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInterpolatedStringExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInterpolatedStringExpression(this);
		}

		public InterpolatedStringExpressionSyntax Update(SyntaxToken dollarSignDoubleQuoteToken, SyntaxList<InterpolatedStringContentSyntax> contents, SyntaxToken doubleQuoteToken)
		{
			if (dollarSignDoubleQuoteToken != DollarSignDoubleQuoteToken || contents != Contents || doubleQuoteToken != DoubleQuoteToken)
			{
				InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = SyntaxFactory.InterpolatedStringExpression(dollarSignDoubleQuoteToken, contents, doubleQuoteToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(interpolatedStringExpressionSyntax, annotations);
				}
				return interpolatedStringExpressionSyntax;
			}
			return this;
		}
	}
}
