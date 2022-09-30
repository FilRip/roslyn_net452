using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class PredefinedCastExpressionSyntax : ExpressionSyntax
	{
		internal readonly KeywordSyntax _keyword;

		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax Keyword => _keyword;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal PredefinedCastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal PredefinedCastExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal PredefinedCastExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(keyword);
			_keyword = keyword;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal PredefinedCastExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
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
				_closeParenToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_keyword);
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_expression);
			writer.WriteValue(_closeParenToken);
		}

		static PredefinedCastExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new PredefinedCastExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PredefinedCastExpressionSyntax), (ObjectReader r) => new PredefinedCastExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.PredefinedCastExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _keyword, 
				1 => _openParenToken, 
				2 => _expression, 
				3 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PredefinedCastExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword, _openParenToken, _expression, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PredefinedCastExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword, _openParenToken, _expression, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitPredefinedCastExpression(this);
		}
	}
}
