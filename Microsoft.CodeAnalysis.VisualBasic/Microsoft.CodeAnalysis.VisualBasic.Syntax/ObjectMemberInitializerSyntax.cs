using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ObjectMemberInitializerSyntax : ObjectCreationInitializerSyntax
	{
		internal SyntaxNode _initializers;

		public SyntaxToken WithKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)base.Green)._withKeyword, base.Position, 0);

		public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)base.Green)._openBraceToken, GetChildPosition(1), GetChildIndex(1));

		public SeparatedSyntaxList<FieldInitializerSyntax> Initializers
		{
			get
			{
				SyntaxNode red = GetRed(ref _initializers, 2);
				return (red == null) ? default(SeparatedSyntaxList<FieldInitializerSyntax>) : new SeparatedSyntaxList<FieldInitializerSyntax>(red, GetChildIndex(2));
			}
		}

		public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax)base.Green)._closeBraceToken, GetChildPosition(3), GetChildIndex(3));

		internal ObjectMemberInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ObjectMemberInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, SyntaxNode initializers, PunctuationSyntax closeBraceToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax(kind, errors, annotations, withKeyword, openBraceToken, initializers?.Green, closeBraceToken), null, 0)
		{
		}

		public ObjectMemberInitializerSyntax WithWithKeyword(SyntaxToken withKeyword)
		{
			return Update(withKeyword, OpenBraceToken, Initializers, CloseBraceToken);
		}

		public ObjectMemberInitializerSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
		{
			return Update(WithKeyword, openBraceToken, Initializers, CloseBraceToken);
		}

		public ObjectMemberInitializerSyntax WithInitializers(SeparatedSyntaxList<FieldInitializerSyntax> initializers)
		{
			return Update(WithKeyword, OpenBraceToken, initializers, CloseBraceToken);
		}

		public ObjectMemberInitializerSyntax AddInitializers(params FieldInitializerSyntax[] items)
		{
			return WithInitializers(Initializers.AddRange(items));
		}

		public ObjectMemberInitializerSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
		{
			return Update(WithKeyword, OpenBraceToken, Initializers, closeBraceToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _initializers;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _initializers, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitObjectMemberInitializer(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitObjectMemberInitializer(this);
		}

		public ObjectMemberInitializerSyntax Update(SyntaxToken withKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<FieldInitializerSyntax> initializers, SyntaxToken closeBraceToken)
		{
			if (withKeyword != WithKeyword || openBraceToken != OpenBraceToken || initializers != Initializers || closeBraceToken != CloseBraceToken)
			{
				ObjectMemberInitializerSyntax objectMemberInitializerSyntax = SyntaxFactory.ObjectMemberInitializer(withKeyword, openBraceToken, initializers, closeBraceToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(objectMemberInitializerSyntax, annotations);
				}
				return objectMemberInitializerSyntax;
			}
			return this;
		}
	}
}
