using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EqualsValueSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _value;

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)base.Green)._equalsToken, base.Position, 0);

		public ExpressionSyntax Value => GetRed(ref _value, 1);

		internal EqualsValueSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EqualsValueSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax(kind, errors, annotations, equalsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)value.Green), null, 0)
		{
		}

		public EqualsValueSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(equalsToken, Value);
		}

		public EqualsValueSyntax WithValue(ExpressionSyntax value)
		{
			return Update(EqualsToken, value);
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
			return visitor.VisitEqualsValue(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEqualsValue(this);
		}

		public EqualsValueSyntax Update(SyntaxToken equalsToken, ExpressionSyntax value)
		{
			if (equalsToken != EqualsToken || value != Value)
			{
				EqualsValueSyntax equalsValueSyntax = SyntaxFactory.EqualsValue(equalsToken, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(equalsValueSyntax, annotations);
				}
				return equalsValueSyntax;
			}
			return this;
		}
	}
}
