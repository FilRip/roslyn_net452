using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AnonymousObjectCreationExpressionSyntax : NewExpressionSyntax
	{
		internal ObjectMemberInitializerSyntax _initializer;

		public new SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)base.Green)._newKeyword, base.Position, 0);

		public new SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributeLists, 1);
				return new SyntaxList<AttributeListSyntax>(red);
			}
		}

		public ObjectMemberInitializerSyntax Initializer => GetRed(ref _initializer, 2);

		internal AnonymousObjectCreationExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AnonymousObjectCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, SyntaxNode attributeLists, ObjectMemberInitializerSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax(kind, errors, annotations, newKeyword, attributeLists?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)initializer.Green), null, 0)
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

		public new AnonymousObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
		{
			return Update(newKeyword, AttributeLists, Initializer);
		}

		internal override SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			return AttributeLists;
		}

		internal override NewExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeLists(attributeLists);
		}

		public new AnonymousObjectCreationExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(NewKeyword, attributeLists, Initializer);
		}

		public new AnonymousObjectCreationExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override NewExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
		{
			return AddAttributeLists(items);
		}

		public AnonymousObjectCreationExpressionSyntax WithInitializer(ObjectMemberInitializerSyntax initializer)
		{
			return Update(NewKeyword, AttributeLists, initializer);
		}

		public AnonymousObjectCreationExpressionSyntax AddInitializerInitializers(params FieldInitializerSyntax[] items)
		{
			ObjectMemberInitializerSyntax objectMemberInitializerSyntax = ((Initializer != null) ? Initializer : SyntaxFactory.ObjectMemberInitializer());
			return WithInitializer(objectMemberInitializerSyntax.AddInitializers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _attributeLists, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _attributeLists, 1), 
				2 => Initializer, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAnonymousObjectCreationExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAnonymousObjectCreationExpression(this);
		}

		public AnonymousObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxList<AttributeListSyntax> attributeLists, ObjectMemberInitializerSyntax initializer)
		{
			if (newKeyword != NewKeyword || attributeLists != AttributeLists || initializer != Initializer)
			{
				AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax = SyntaxFactory.AnonymousObjectCreationExpression(newKeyword, attributeLists, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(anonymousObjectCreationExpressionSyntax, annotations);
				}
				return anonymousObjectCreationExpressionSyntax;
			}
			return this;
		}
	}
}
