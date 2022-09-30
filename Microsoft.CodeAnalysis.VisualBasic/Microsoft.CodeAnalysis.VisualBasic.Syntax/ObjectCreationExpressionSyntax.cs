using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ObjectCreationExpressionSyntax : NewExpressionSyntax
	{
		internal TypeSyntax _type;

		internal ArgumentListSyntax _argumentList;

		internal ObjectCreationInitializerSyntax _initializer;

		public new SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax)base.Green)._newKeyword, base.Position, 0);

		public new SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributeLists, 1);
				return new SyntaxList<AttributeListSyntax>(red);
			}
		}

		public TypeSyntax Type => GetRed(ref _type, 2);

		public ArgumentListSyntax ArgumentList => GetRed(ref _argumentList, 3);

		public ObjectCreationInitializerSyntax Initializer => GetRed(ref _initializer, 4);

		internal ObjectCreationExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ObjectCreationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax newKeyword, SyntaxNode attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax(kind, errors, annotations, newKeyword, attributeLists?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green, (argumentList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green) : null, (initializer != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax)initializer.Green) : null), null, 0)
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

		public new ObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
		{
			return Update(newKeyword, AttributeLists, Type, ArgumentList, Initializer);
		}

		internal override SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			return AttributeLists;
		}

		internal override NewExpressionSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeLists(attributeLists);
		}

		public new ObjectCreationExpressionSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(NewKeyword, attributeLists, Type, ArgumentList, Initializer);
		}

		public new ObjectCreationExpressionSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override NewExpressionSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
		{
			return AddAttributeLists(items);
		}

		public ObjectCreationExpressionSyntax WithType(TypeSyntax type)
		{
			return Update(NewKeyword, AttributeLists, type, ArgumentList, Initializer);
		}

		public ObjectCreationExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList)
		{
			return Update(NewKeyword, AttributeLists, Type, argumentList, Initializer);
		}

		public ObjectCreationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArgumentList != null) ? ArgumentList : SyntaxFactory.ArgumentList());
			return WithArgumentList(argumentListSyntax.AddArguments(items));
		}

		public ObjectCreationExpressionSyntax WithInitializer(ObjectCreationInitializerSyntax initializer)
		{
			return Update(NewKeyword, AttributeLists, Type, ArgumentList, initializer);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _attributeLists, 
				2 => _type, 
				3 => _argumentList, 
				4 => _initializer, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => GetRed(ref _attributeLists, 1), 
				2 => Type, 
				3 => ArgumentList, 
				4 => Initializer, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitObjectCreationExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitObjectCreationExpression(this);
		}

		public ObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxList<AttributeListSyntax> attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
		{
			if (newKeyword != NewKeyword || attributeLists != AttributeLists || type != Type || argumentList != ArgumentList || initializer != Initializer)
			{
				ObjectCreationExpressionSyntax objectCreationExpressionSyntax = SyntaxFactory.ObjectCreationExpression(newKeyword, attributeLists, type, argumentList, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(objectCreationExpressionSyntax, annotations);
				}
				return objectCreationExpressionSyntax;
			}
			return this;
		}
	}
}
