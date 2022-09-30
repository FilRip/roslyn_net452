using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class TernaryConditionalExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _ifKeyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _condition;

		internal readonly PunctuationSyntax _firstCommaToken;

		internal readonly ExpressionSyntax _whenTrue;

		internal readonly PunctuationSyntax _secondCommaToken;

		internal readonly ExpressionSyntax _whenFalse;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax IfKeyword => _ifKeyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Condition => _condition;

		internal PunctuationSyntax FirstCommaToken => _firstCommaToken;

		internal ExpressionSyntax WhenTrue => _whenTrue;

		internal PunctuationSyntax SecondCommaToken => _secondCommaToken;

		internal ExpressionSyntax WhenFalse => _whenFalse;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal TernaryConditionalExpressionSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(whenTrue);
			_whenTrue = whenTrue;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(whenFalse);
			_whenFalse = whenFalse;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TernaryConditionalExpressionSyntax(SyntaxKind kind, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 8;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(whenTrue);
			_whenTrue = whenTrue;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(whenFalse);
			_whenFalse = whenFalse;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TernaryConditionalExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 8;
			AdjustFlagsAndWidth(ifKeyword);
			_ifKeyword = ifKeyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(condition);
			_condition = condition;
			AdjustFlagsAndWidth(firstCommaToken);
			_firstCommaToken = firstCommaToken;
			AdjustFlagsAndWidth(whenTrue);
			_whenTrue = whenTrue;
			AdjustFlagsAndWidth(secondCommaToken);
			_secondCommaToken = secondCommaToken;
			AdjustFlagsAndWidth(whenFalse);
			_whenFalse = whenFalse;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal TernaryConditionalExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 8;
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
				_condition = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_firstCommaToken = punctuationSyntax2;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_whenTrue = expressionSyntax2;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_secondCommaToken = punctuationSyntax3;
			}
			ExpressionSyntax expressionSyntax3 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax3 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax3);
				_whenFalse = expressionSyntax3;
			}
			PunctuationSyntax punctuationSyntax4 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax4 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax4);
				_closeParenToken = punctuationSyntax4;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_ifKeyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_condition);
			writer.WriteValue(_firstCommaToken);
			writer.WriteValue(_whenTrue);
			writer.WriteValue(_secondCommaToken);
			writer.WriteValue(_whenFalse);
			writer.WriteValue(_closeParenToken);
		}

		static TernaryConditionalExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new TernaryConditionalExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(TernaryConditionalExpressionSyntax), (ObjectReader r) => new TernaryConditionalExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.TernaryConditionalExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _ifKeyword, 
				1 => _openParenToken, 
				2 => _condition, 
				3 => _firstCommaToken, 
				4 => _whenTrue, 
				5 => _secondCommaToken, 
				6 => _whenFalse, 
				7 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new TernaryConditionalExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _ifKeyword, _openParenToken, _condition, _firstCommaToken, _whenTrue, _secondCommaToken, _whenFalse, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new TernaryConditionalExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _ifKeyword, _openParenToken, _condition, _firstCommaToken, _whenTrue, _secondCommaToken, _whenFalse, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitTernaryConditionalExpression(this);
		}
	}
}
