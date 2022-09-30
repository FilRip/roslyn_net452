using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CollectionInitializerSyntax : ExpressionSyntax
	{
		internal SyntaxNode _initializers;

		public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)base.Green)._openBraceToken, base.Position, 0);

		public SeparatedSyntaxList<ExpressionSyntax> Initializers
		{
			get
			{
				SyntaxNode red = GetRed(ref _initializers, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionSyntax>) : new SeparatedSyntaxList<ExpressionSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax)base.Green)._closeBraceToken, GetChildPosition(2), GetChildIndex(2));

		internal CollectionInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CollectionInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openBraceToken, SyntaxNode initializers, PunctuationSyntax closeBraceToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax(kind, errors, annotations, openBraceToken, initializers?.Green, closeBraceToken), null, 0)
		{
		}

		public CollectionInitializerSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
		{
			return Update(openBraceToken, Initializers, CloseBraceToken);
		}

		public CollectionInitializerSyntax WithInitializers(SeparatedSyntaxList<ExpressionSyntax> initializers)
		{
			return Update(OpenBraceToken, initializers, CloseBraceToken);
		}

		public CollectionInitializerSyntax AddInitializers(params ExpressionSyntax[] items)
		{
			return WithInitializers(Initializers.AddRange(items));
		}

		public CollectionInitializerSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
		{
			return Update(OpenBraceToken, Initializers, closeBraceToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _initializers;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _initializers, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCollectionInitializer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCollectionInitializer(this);
		}

		public CollectionInitializerSyntax Update(SyntaxToken openBraceToken, SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken closeBraceToken)
		{
			if (openBraceToken != OpenBraceToken || initializers != Initializers || closeBraceToken != CloseBraceToken)
			{
				CollectionInitializerSyntax collectionInitializerSyntax = SyntaxFactory.CollectionInitializer(openBraceToken, initializers, closeBraceToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(collectionInitializerSyntax, annotations);
				}
				return collectionInitializerSyntax;
			}
			return this;
		}
	}
}
