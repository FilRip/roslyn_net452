using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ArrayCreationExpressionSyntax : NewExpressionSyntax
	{
		internal TypeSyntax _type;

		internal ArgumentListSyntax _arrayBounds;

		internal SyntaxNode _rankSpecifiers;

		internal CollectionInitializerSyntax _initializer;

		public new SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax)base.Green)._newKeyword, base.Position, 0);

		public new SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributeLists, 1);
				return new SyntaxList<AttributeListSyntax>(red);
			}
		}

		public TypeSyntax Type => GetRed(ref _type, 2);

		public ArgumentListSyntax ArrayBounds => GetRed(ref _arrayBounds, 3);

		public SyntaxList<ArrayRankSpecifierSyntax> RankSpecifiers
		{
			get
			{
				SyntaxNode red = GetRed(ref _rankSpecifiers, 4);
				return new SyntaxList<ArrayRankSpecifierSyntax>(red);
			}
		}

		public CollectionInitializerSyntax Initializer => GetRed(ref _initializer, 5);

		internal ArrayCreationExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ArrayCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, SyntaxNode attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, SyntaxNode rankSpecifiers, CollectionInitializerSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax(kind, errors, annotations, newKeyword, attributeLists?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green, (arrayBounds != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)arrayBounds.Green) : null, rankSpecifiers?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)initializer.Green), null, 0)
		{
		}

		internal override SyntaxToken GetNewKeywordCore()
		{
			return NewKeyword;
		}

		internal override NewExpressionSyntax WithNewKeywordCore(SyntaxToken newKeyword)
		{
			return WithNewKeyword(newKeyword);
		}

		public new ArrayCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
		{
			return Update(newKeyword, AttributeLists, Type, ArrayBounds, RankSpecifiers, Initializer);
		}

		internal override SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			return AttributeLists;
		}

		internal override NewExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeLists(attributeLists);
		}

		public new ArrayCreationExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(NewKeyword, attributeLists, Type, ArrayBounds, RankSpecifiers, Initializer);
		}

		public new ArrayCreationExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override NewExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
		{
			return AddAttributeLists(items);
		}

		public ArrayCreationExpressionSyntax WithType(TypeSyntax type)
		{
			return Update(NewKeyword, AttributeLists, type, ArrayBounds, RankSpecifiers, Initializer);
		}

		public ArrayCreationExpressionSyntax WithArrayBounds(ArgumentListSyntax arrayBounds)
		{
			return Update(NewKeyword, AttributeLists, Type, arrayBounds, RankSpecifiers, Initializer);
		}

		public ArrayCreationExpressionSyntax AddArrayBoundsArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArrayBounds != null) ? ArrayBounds : SyntaxFactory.ArgumentList());
			return WithArrayBounds(argumentListSyntax.AddArguments(items));
		}

		public ArrayCreationExpressionSyntax WithRankSpecifiers(SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers)
		{
			return Update(NewKeyword, AttributeLists, Type, ArrayBounds, rankSpecifiers, Initializer);
		}

		public ArrayCreationExpressionSyntax AddRankSpecifiers(params ArrayRankSpecifierSyntax[] items)
		{
			return WithRankSpecifiers(RankSpecifiers.AddRange(items));
		}

		public ArrayCreationExpressionSyntax WithInitializer(CollectionInitializerSyntax initializer)
		{
			return Update(NewKeyword, AttributeLists, Type, ArrayBounds, RankSpecifiers, initializer);
		}

		public ArrayCreationExpressionSyntax AddInitializerInitializers(params ExpressionSyntax[] items)
		{
			CollectionInitializerSyntax collectionInitializerSyntax = ((Initializer != null) ? Initializer : SyntaxFactory.CollectionInitializer());
			return WithInitializer(collectionInitializerSyntax.AddInitializers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _attributeLists, 
				2 => _type, 
				3 => _arrayBounds, 
				4 => _rankSpecifiers, 
				5 => _initializer, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _attributeLists, 1), 
				2 => Type, 
				3 => ArrayBounds, 
				4 => GetRed(ref _rankSpecifiers, 4), 
				5 => Initializer, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitArrayCreationExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitArrayCreationExpression(this);
		}

		public ArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxList<AttributeListSyntax> attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers, CollectionInitializerSyntax initializer)
		{
			if (newKeyword != NewKeyword || attributeLists != AttributeLists || type != Type || arrayBounds != ArrayBounds || rankSpecifiers != RankSpecifiers || initializer != Initializer)
			{
				ArrayCreationExpressionSyntax arrayCreationExpressionSyntax = SyntaxFactory.ArrayCreationExpression(newKeyword, attributeLists, type, arrayBounds, rankSpecifiers, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(arrayCreationExpressionSyntax, annotations);
				}
				return arrayCreationExpressionSyntax;
			}
			return this;
		}
	}
}
