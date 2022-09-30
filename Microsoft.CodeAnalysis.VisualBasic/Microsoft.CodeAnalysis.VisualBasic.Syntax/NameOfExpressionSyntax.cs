using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NameOfExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _argument;

		public SyntaxToken NameOfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)base.Green)._nameOfKeyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Argument => GetRed(ref _argument, 2);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal NameOfExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NameOfExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax(kind, errors, annotations, nameOfKeyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)argument.Green, closeParenToken), null, 0)
		{
		}

		public NameOfExpressionSyntax WithNameOfKeyword(SyntaxToken nameOfKeyword)
		{
			return Update(nameOfKeyword, OpenParenToken, Argument, CloseParenToken);
		}

		public NameOfExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(NameOfKeyword, openParenToken, Argument, CloseParenToken);
		}

		public NameOfExpressionSyntax WithArgument(ExpressionSyntax argument)
		{
			return Update(NameOfKeyword, OpenParenToken, argument, CloseParenToken);
		}

		public NameOfExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(NameOfKeyword, OpenParenToken, Argument, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _argument;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Argument;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNameOfExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNameOfExpression(this);
		}

		public NameOfExpressionSyntax Update(SyntaxToken nameOfKeyword, SyntaxToken openParenToken, ExpressionSyntax argument, SyntaxToken closeParenToken)
		{
			if (nameOfKeyword != NameOfKeyword || openParenToken != OpenParenToken || argument != Argument || closeParenToken != CloseParenToken)
			{
				NameOfExpressionSyntax nameOfExpressionSyntax = SyntaxFactory.NameOfExpression(nameOfKeyword, openParenToken, argument, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(nameOfExpressionSyntax, annotations);
				}
				return nameOfExpressionSyntax;
			}
			return this;
		}
	}
}
