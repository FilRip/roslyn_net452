using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class BinaryConditionalExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _firstExpression;

		internal ExpressionSyntax _secondExpression;

		public SyntaxToken IfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)base.Green)._ifKeyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax FirstExpression => GetRed(ref _firstExpression, 2);

		public SyntaxToken CommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)base.Green)._commaToken, GetChildPosition(3), GetChildIndex(3));

		public ExpressionSyntax SecondExpression => GetRed(ref _secondExpression, 4);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(5), GetChildIndex(5));

		internal BinaryConditionalExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal BinaryConditionalExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax(kind, errors, annotations, ifKeyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)firstExpression.Green, commaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)secondExpression.Green, closeParenToken), null, 0)
		{
		}

		public BinaryConditionalExpressionSyntax WithIfKeyword(SyntaxToken ifKeyword)
		{
			return Update(ifKeyword, OpenParenToken, FirstExpression, CommaToken, SecondExpression, CloseParenToken);
		}

		public BinaryConditionalExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(IfKeyword, openParenToken, FirstExpression, CommaToken, SecondExpression, CloseParenToken);
		}

		public BinaryConditionalExpressionSyntax WithFirstExpression(ExpressionSyntax firstExpression)
		{
			return Update(IfKeyword, OpenParenToken, firstExpression, CommaToken, SecondExpression, CloseParenToken);
		}

		public BinaryConditionalExpressionSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return Update(IfKeyword, OpenParenToken, FirstExpression, commaToken, SecondExpression, CloseParenToken);
		}

		public BinaryConditionalExpressionSyntax WithSecondExpression(ExpressionSyntax secondExpression)
		{
			return Update(IfKeyword, OpenParenToken, FirstExpression, CommaToken, secondExpression, CloseParenToken);
		}

		public BinaryConditionalExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(IfKeyword, OpenParenToken, FirstExpression, CommaToken, SecondExpression, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _firstExpression, 
				4 => _secondExpression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => FirstExpression, 
				4 => SecondExpression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitBinaryConditionalExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitBinaryConditionalExpression(this);
		}

		public BinaryConditionalExpressionSyntax Update(SyntaxToken ifKeyword, SyntaxToken openParenToken, ExpressionSyntax firstExpression, SyntaxToken commaToken, ExpressionSyntax secondExpression, SyntaxToken closeParenToken)
		{
			if (ifKeyword != IfKeyword || openParenToken != OpenParenToken || firstExpression != FirstExpression || commaToken != CommaToken || secondExpression != SecondExpression || closeParenToken != CloseParenToken)
			{
				BinaryConditionalExpressionSyntax binaryConditionalExpressionSyntax = SyntaxFactory.BinaryConditionalExpression(ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(binaryConditionalExpressionSyntax, annotations);
				}
				return binaryConditionalExpressionSyntax;
			}
			return this;
		}
	}
}
