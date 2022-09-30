using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NullableTypeSyntax : TypeSyntax
	{
		internal TypeSyntax _elementType;

		public TypeSyntax ElementType => GetRedAtZero(ref _elementType);

		public SyntaxToken QuestionMarkToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax)base.Green)._questionMarkToken, GetChildPosition(1), GetChildIndex(1));

		internal NullableTypeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NullableTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax elementType, PunctuationSyntax questionMarkToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)elementType.Green, questionMarkToken), null, 0)
		{
		}

		public NullableTypeSyntax WithElementType(TypeSyntax elementType)
		{
			return Update(elementType, QuestionMarkToken);
		}

		public NullableTypeSyntax WithQuestionMarkToken(SyntaxToken questionMarkToken)
		{
			return Update(ElementType, questionMarkToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _elementType;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return ElementType;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNullableType(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNullableType(this);
		}

		public NullableTypeSyntax Update(TypeSyntax elementType, SyntaxToken questionMarkToken)
		{
			if (elementType != ElementType || questionMarkToken != QuestionMarkToken)
			{
				NullableTypeSyntax nullableTypeSyntax = SyntaxFactory.NullableType(elementType, questionMarkToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(nullableTypeSyntax, annotations);
				}
				return nullableTypeSyntax;
			}
			return this;
		}
	}
}
