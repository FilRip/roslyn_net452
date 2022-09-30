using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ObjectCollectionInitializerSyntax : ObjectCreationInitializerSyntax
	{
		internal CollectionInitializerSyntax _initializer;

		public SyntaxToken FromKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax)base.Green)._fromKeyword, base.Position, 0);

		public CollectionInitializerSyntax Initializer => GetRed(ref _initializer, 1);

		internal ObjectCollectionInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ObjectCollectionInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax(kind, errors, annotations, fromKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)initializer.Green), null, 0)
		{
		}

		public ObjectCollectionInitializerSyntax WithFromKeyword(SyntaxToken fromKeyword)
		{
			return Update(fromKeyword, Initializer);
		}

		public ObjectCollectionInitializerSyntax WithInitializer(CollectionInitializerSyntax initializer)
		{
			return Update(FromKeyword, initializer);
		}

		public ObjectCollectionInitializerSyntax AddInitializerInitializers(params ExpressionSyntax[] items)
		{
			CollectionInitializerSyntax collectionInitializerSyntax = ((Initializer != null) ? Initializer : SyntaxFactory.CollectionInitializer());
			return WithInitializer(collectionInitializerSyntax.AddInitializers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _initializer;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Initializer;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitObjectCollectionInitializer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitObjectCollectionInitializer(this);
		}

		public ObjectCollectionInitializerSyntax Update(SyntaxToken fromKeyword, CollectionInitializerSyntax initializer)
		{
			if (fromKeyword != FromKeyword || initializer != Initializer)
			{
				ObjectCollectionInitializerSyntax objectCollectionInitializerSyntax = SyntaxFactory.ObjectCollectionInitializer(fromKeyword, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(objectCollectionInitializerSyntax, annotations);
				}
				return objectCollectionInitializerSyntax;
			}
			return this;
		}
	}
}
