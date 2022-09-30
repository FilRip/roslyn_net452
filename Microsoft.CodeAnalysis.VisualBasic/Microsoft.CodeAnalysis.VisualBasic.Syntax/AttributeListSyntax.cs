using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AttributeListSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _attributes;

		public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)base.Green)._lessThanToken, base.Position, 0);

		public SeparatedSyntaxList<AttributeSyntax> Attributes
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributes, 1);
				return (red == null) ? default(SeparatedSyntaxList<AttributeSyntax>) : new SeparatedSyntaxList<AttributeSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)base.Green)._greaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal AttributeListSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AttributeListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, SyntaxNode attributes, PunctuationSyntax greaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax(kind, errors, annotations, lessThanToken, attributes?.Green, greaterThanToken), null, 0)
		{
		}

		public AttributeListSyntax WithLessThanToken(SyntaxToken lessThanToken)
		{
			return Update(lessThanToken, Attributes, GreaterThanToken);
		}

		public AttributeListSyntax WithAttributes(SeparatedSyntaxList<AttributeSyntax> attributes)
		{
			return Update(LessThanToken, attributes, GreaterThanToken);
		}

		public AttributeListSyntax AddAttributes(params AttributeSyntax[] items)
		{
			return WithAttributes(Attributes.AddRange(items));
		}

		public AttributeListSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
		{
			return Update(LessThanToken, Attributes, greaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _attributes;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _attributes, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAttributeList(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAttributeList(this);
		}

		public AttributeListSyntax Update(SyntaxToken lessThanToken, SeparatedSyntaxList<AttributeSyntax> attributes, SyntaxToken greaterThanToken)
		{
			if (lessThanToken != LessThanToken || attributes != Attributes || greaterThanToken != GreaterThanToken)
			{
				AttributeListSyntax attributeListSyntax = SyntaxFactory.AttributeList(lessThanToken, attributes, greaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(attributeListSyntax, annotations);
				}
				return attributeListSyntax;
			}
			return this;
		}
	}
}
