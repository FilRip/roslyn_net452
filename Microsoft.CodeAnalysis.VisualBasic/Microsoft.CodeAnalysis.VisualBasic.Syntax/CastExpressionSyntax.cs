using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class CastExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		internal TypeSyntax _type;

		public SyntaxToken Keyword => GetKeywordCore();

		public SyntaxToken OpenParenToken => GetOpenParenTokenCore();

		public ExpressionSyntax Expression => GetExpressionCore();

		public SyntaxToken CommaToken => GetCommaTokenCore();

		public TypeSyntax Type => GetTypeCore();

		public SyntaxToken CloseParenToken => GetCloseParenTokenCore();

		internal CastExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)base.Green)._keyword, base.Position, 0);
		}

		public CastExpressionSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithKeywordCore(keyword);
		}

		internal abstract CastExpressionSyntax WithKeywordCore(SyntaxToken keyword);

		internal virtual SyntaxToken GetOpenParenTokenCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));
		}

		public CastExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return WithOpenParenTokenCore(openParenToken);
		}

		internal abstract CastExpressionSyntax WithOpenParenTokenCore(SyntaxToken openParenToken);

		internal virtual ExpressionSyntax GetExpressionCore()
		{
			return GetRed(ref _expression, 2);
		}

		public CastExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return WithExpressionCore(expression);
		}

		internal abstract CastExpressionSyntax WithExpressionCore(ExpressionSyntax expression);

		internal virtual SyntaxToken GetCommaTokenCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)base.Green)._commaToken, GetChildPosition(3), GetChildIndex(3));
		}

		public CastExpressionSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return WithCommaTokenCore(commaToken);
		}

		internal abstract CastExpressionSyntax WithCommaTokenCore(SyntaxToken commaToken);

		internal virtual TypeSyntax GetTypeCore()
		{
			return GetRed(ref _type, 4);
		}

		public CastExpressionSyntax WithType(TypeSyntax type)
		{
			return WithTypeCore(type);
		}

		internal abstract CastExpressionSyntax WithTypeCore(TypeSyntax type);

		internal virtual SyntaxToken GetCloseParenTokenCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(5), GetChildIndex(5));
		}

		public CastExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return WithCloseParenTokenCore(closeParenToken);
		}

		internal abstract CastExpressionSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken);
	}
}
