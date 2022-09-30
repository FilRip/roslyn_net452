using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeArgumentListSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _arguments;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)base.Green)._openParenToken, base.Position, 0);

		public SyntaxToken OfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)base.Green)._ofKeyword, GetChildPosition(1), GetChildIndex(1));

		public SeparatedSyntaxList<TypeSyntax> Arguments
		{
			get
			{
				SyntaxNode red = GetRed(ref _arguments, 2);
				return (red == null) ? default(SeparatedSyntaxList<TypeSyntax>) : new SeparatedSyntaxList<TypeSyntax>(red, GetChildIndex(2));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal TypeArgumentListSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeArgumentListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, SyntaxNode arguments, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax(kind, errors, annotations, openParenToken, ofKeyword, arguments?.Green, closeParenToken), null, 0)
		{
		}

		public TypeArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, OfKeyword, Arguments, CloseParenToken);
		}

		public TypeArgumentListSyntax WithOfKeyword(SyntaxToken ofKeyword)
		{
			return Update(OpenParenToken, ofKeyword, Arguments, CloseParenToken);
		}

		public TypeArgumentListSyntax WithArguments(SeparatedSyntaxList<TypeSyntax> arguments)
		{
			return Update(OpenParenToken, OfKeyword, arguments, CloseParenToken);
		}

		public TypeArgumentListSyntax AddArguments(params TypeSyntax[] items)
		{
			return WithArguments(Arguments.AddRange(items));
		}

		public TypeArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, OfKeyword, Arguments, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _arguments;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _arguments, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeArgumentList(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeArgumentList(this);
		}

		public TypeArgumentListSyntax Update(SyntaxToken openParenToken, SyntaxToken ofKeyword, SeparatedSyntaxList<TypeSyntax> arguments, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || ofKeyword != OfKeyword || arguments != Arguments || closeParenToken != CloseParenToken)
			{
				TypeArgumentListSyntax typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(openParenToken, ofKeyword, arguments, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeArgumentListSyntax, annotations);
				}
				return typeArgumentListSyntax;
			}
			return this;
		}
	}
}
