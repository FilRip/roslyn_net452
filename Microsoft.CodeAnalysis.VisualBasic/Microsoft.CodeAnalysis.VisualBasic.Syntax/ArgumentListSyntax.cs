using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ArgumentListSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _arguments;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)base.Green)._openParenToken, base.Position, 0);

		public SeparatedSyntaxList<ArgumentSyntax> Arguments
		{
			get
			{
				SyntaxNode red = GetRed(ref _arguments, 1);
				return (red == null) ? default(SeparatedSyntaxList<ArgumentSyntax>) : new SeparatedSyntaxList<ArgumentSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal ArgumentListSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ArgumentListSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, SyntaxNode arguments, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax(kind, errors, annotations, openParenToken, arguments?.Green, closeParenToken), null, 0)
		{
		}

		public ArgumentListSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, Arguments, CloseParenToken);
		}

		public ArgumentListSyntax WithArguments(SeparatedSyntaxList<ArgumentSyntax> arguments)
		{
			return Update(OpenParenToken, arguments, CloseParenToken);
		}

		public ArgumentListSyntax AddArguments(params ArgumentSyntax[] items)
		{
			return WithArguments(Arguments.AddRange(items));
		}

		public ArgumentListSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, Arguments, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _arguments;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _arguments, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitArgumentList(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitArgumentList(this);
		}

		public ArgumentListSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || arguments != Arguments || closeParenToken != CloseParenToken)
			{
				ArgumentListSyntax argumentListSyntax = SyntaxFactory.ArgumentList(openParenToken, arguments, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(argumentListSyntax, annotations);
				}
				return argumentListSyntax;
			}
			return this;
		}
	}
}
