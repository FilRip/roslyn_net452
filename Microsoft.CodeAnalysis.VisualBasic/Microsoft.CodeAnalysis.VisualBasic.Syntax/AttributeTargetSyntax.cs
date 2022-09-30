using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AttributeTargetSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken AttributeModifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)base.Green)._attributeModifier, base.Position, 0);

		public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax)base.Green)._colonToken, GetChildPosition(1), GetChildIndex(1));

		internal AttributeTargetSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AttributeTargetSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax attributeModifier, PunctuationSyntax colonToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax(kind, errors, annotations, attributeModifier, colonToken), null, 0)
		{
		}

		public AttributeTargetSyntax WithAttributeModifier(SyntaxToken attributeModifier)
		{
			return Update(attributeModifier, ColonToken);
		}

		public AttributeTargetSyntax WithColonToken(SyntaxToken colonToken)
		{
			return Update(AttributeModifier, colonToken);
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
			return visitor.VisitAttributeTarget(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAttributeTarget(this);
		}

		public AttributeTargetSyntax Update(SyntaxToken attributeModifier, SyntaxToken colonToken)
		{
			if (attributeModifier != AttributeModifier || colonToken != ColonToken)
			{
				AttributeTargetSyntax attributeTargetSyntax = SyntaxFactory.AttributeTarget(attributeModifier, colonToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(attributeTargetSyntax, annotations);
				}
				return attributeTargetSyntax;
			}
			return this;
		}
	}
}
