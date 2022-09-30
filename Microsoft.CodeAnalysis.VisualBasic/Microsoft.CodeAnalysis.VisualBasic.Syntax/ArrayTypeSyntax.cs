using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ArrayTypeSyntax : TypeSyntax
	{
		internal TypeSyntax _elementType;

		internal SyntaxNode _rankSpecifiers;

		public TypeSyntax ElementType => GetRedAtZero(ref _elementType);

		public SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers
		{
			get
			{
				SyntaxNode red = GetRed(ref _rankSpecifiers, 1);
				return new SyntaxList<ArrayRankSpecifierSyntax>(red);
			}
		}

		internal ArrayTypeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ArrayTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax elementType, SyntaxNode rankSpecifiers)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)elementType.Green, rankSpecifiers?.Green), null, 0)
		{
		}

		public ArrayTypeSyntax WithElementType(TypeSyntax elementType)
		{
			return Update(elementType, RankSpecifiers);
		}

		public ArrayTypeSyntax WithRankSpecifiers(SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
		{
			return Update(ElementType, rankSpecifiers);
		}

		public ArrayTypeSyntax AddRankSpecifiers(params ArrayRankSpecifierSyntax[] items)
		{
			return WithRankSpecifiers(RankSpecifiers.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _elementType, 
				1 => _rankSpecifiers, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ElementType, 
				1 => GetRed(ref _rankSpecifiers, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitArrayType(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitArrayType(this);
		}

		public ArrayTypeSyntax Update(TypeSyntax elementType, SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
		{
			if (elementType != ElementType || rankSpecifiers != RankSpecifiers)
			{
				ArrayTypeSyntax arrayTypeSyntax = SyntaxFactory.ArrayType(elementType, rankSpecifiers);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(arrayTypeSyntax, annotations);
				}
				return arrayTypeSyntax;
			}
			return this;
		}
	}
}
