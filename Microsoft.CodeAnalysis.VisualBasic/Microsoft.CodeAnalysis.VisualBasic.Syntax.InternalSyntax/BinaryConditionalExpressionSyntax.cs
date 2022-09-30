using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class BinaryConditionalExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _ifKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _firstExpression;

		internal readonly PunctuationSyntax _commaToken;

		internal readonly ExpressionSyntax _secondExpression;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax IfKeyword => _ifKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax FirstExpression => _firstExpression;

		internal PunctuationSyntax CommaToken => _commaToken;

		internal ExpressionSyntax SecondExpression => _secondExpression;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal BinaryConditionalExpressionSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(firstExpression);
			_firstExpression = firstExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(secondExpression);
			_secondExpression = secondExpression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal BinaryConditionalExpressionSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 6;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(firstExpression);
			_firstExpression = firstExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(secondExpression);
			_secondExpression = secondExpression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal BinaryConditionalExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 6;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(firstExpression);
			_firstExpression = firstExpression;
			AdjustFlagsAndWidth(commaToken);
			_commaToken = commaToken;
			AdjustFlagsAndWidth(secondExpression);
			_secondExpression = secondExpression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal BinaryConditionalExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 6;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_ifKeyword = keywordSyntax;
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
				_firstExpression = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_commaToken = punctuationSyntax2;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_secondExpression = expressionSyntax2;
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
			writer.WriteValue(_ifKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_firstExpression);
			writer.WriteValue(_commaToken);
			writer.WriteValue(_secondExpression);
			writer.WriteValue(_closeParenToken);
		}

		static BinaryConditionalExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new BinaryConditionalExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(BinaryConditionalExpressionSyntax), (ObjectReader r) => new BinaryConditionalExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryConditionalExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _ifKeyword, 
				1 => _openParenToken, 
				2 => _firstExpression, 
				3 => _commaToken, 
				4 => _secondExpression, 
				5 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new BinaryConditionalExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _ifKeyword, _openParenToken, _firstExpression, _commaToken, _secondExpression, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new BinaryConditionalExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _ifKeyword, _openParenToken, _firstExpression, _commaToken, _secondExpression, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitBinaryConditionalExpression(this);
		}
	}
}
