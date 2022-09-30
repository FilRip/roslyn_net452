using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TryCastExpressionSyntax : CastExpressionSyntax
	{
		public new SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax)base.Green)._keyword, base.Position, 0);

		public new SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public new ExpressionSyntax Expression => GetRed(ref _expression, 2);

		public new SyntaxToken CommaToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax)base.Green)._commaToken, GetChildPosition(3), GetChildIndex(3));

		public new TypeSyntax Type => GetRed(ref _type, 4);

		public new SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(5), GetChildIndex(5));

		internal TryCastExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TryCastExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax(kind, errors, annotations, keyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, commaToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green, closeParenToken), null, 0)
		{
		}

		internal override SyntaxToken GetKeywordCore()
		{
			return Keyword;
		}

		internal override CastExpressionSyntax WithKeywordCore(SyntaxToken keyword)
		{
			return WithKeyword(keyword);
		}

		public new TryCastExpressionSyntax WithKeyword(SyntaxToken keyword)
		{
			return Update(keyword, OpenParenToken, Expression, CommaToken, Type, CloseParenToken);
		}

		internal override SyntaxToken GetOpenParenTokenCore()
		{
			return OpenParenToken;
		}

		internal override CastExpressionSyntax WithOpenParenTokenCore(SyntaxToken openParenToken)
		{
			return WithOpenParenToken(openParenToken);
		}

		public new TryCastExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(Keyword, openParenToken, Expression, CommaToken, Type, CloseParenToken);
		}

		internal override ExpressionSyntax GetExpressionCore()
		{
			return Expression;
		}

		internal override CastExpressionSyntax WithExpressionCore(ExpressionSyntax expression)
		{
			return WithExpression(expression);
		}

		public new TryCastExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Keyword, OpenParenToken, expression, CommaToken, Type, CloseParenToken);
		}

		internal override SyntaxToken GetCommaTokenCore()
		{
			return CommaToken;
		}

		internal override CastExpressionSyntax WithCommaTokenCore(SyntaxToken commaToken)
		{
			return WithCommaToken(commaToken);
		}

		public new TryCastExpressionSyntax WithCommaToken(SyntaxToken commaToken)
		{
			return Update(Keyword, OpenParenToken, Expression, commaToken, Type, CloseParenToken);
		}

		internal override TypeSyntax GetTypeCore()
		{
			return Type;
		}

		internal override CastExpressionSyntax WithTypeCore(TypeSyntax type)
		{
			return WithType(type);
		}

		public new TryCastExpressionSyntax WithType(TypeSyntax type)
		{
			return Update(Keyword, OpenParenToken, Expression, CommaToken, type, CloseParenToken);
		}

		internal override SyntaxToken GetCloseParenTokenCore()
		{
			return CloseParenToken;
		}

		internal override CastExpressionSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken)
		{
			return WithCloseParenToken(closeParenToken);
		}

		public new TryCastExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(Keyword, OpenParenToken, Expression, CommaToken, Type, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _expression, 
				4 => _type, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => Expression, 
				4 => Type, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTryCastExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTryCastExpression(this);
		}

		public TryCastExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken commaToken, TypeSyntax type, SyntaxToken closeParenToken)
		{
			if (keyword != Keyword || openParenToken != OpenParenToken || expression != Expression || commaToken != CommaToken || type != Type || closeParenToken != CloseParenToken)
			{
				TryCastExpressionSyntax tryCastExpressionSyntax = SyntaxFactory.TryCastExpression(keyword, openParenToken, expression, commaToken, type, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(tryCastExpressionSyntax, annotations);
				}
				return tryCastExpressionSyntax;
			}
			return this;
		}
	}
}
