using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class CastExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _keyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _commaToken;

		internal readonly TypeSyntax _type;

		internal readonly PunctuationSyntax _closeParenToken;

		internal KeywordSyntax Keyword => _keyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax CommaToken => _commaToken;

		internal TypeSyntax Type => _type;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal CastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CastExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(type);
			_type = type;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal CastExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_keyword = keywordSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_openParenToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_commaToken = punctuationSyntax2;
			}
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_type = typeSyntax;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_closeParenToken = punctuationSyntax3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_keyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_expression);
			writer.WriteValue(_commaToken);
			writer.WriteValue(_type);
			writer.WriteValue(_closeParenToken);
		}
	}
}
