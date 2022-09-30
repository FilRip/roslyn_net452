using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
	{
		internal readonly PunctuationSyntax _openParenToken;

		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _closeParenToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax OpenParenToken => _openParenToken;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax CloseParenToken => _closeParenToken;

		internal ParenthesizedExpressionSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ParenthesizedExpressionSyntax(SyntaxKind kind, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ParenthesizedExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(openParenToken);
			_openParenToken = openParenToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(closeParenToken);
			_closeParenToken = closeParenToken;
		}

		internal ParenthesizedExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
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
			writer.WriteValue(_openParenToken);
			writer.WriteValue(_expression);
			writer.WriteValue(_closeParenToken);
		}

		static ParenthesizedExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new ParenthesizedExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ParenthesizedExpressionSyntax), (ObjectReader r) => new ParenthesizedExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ParenthesizedExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _openParenToken, 
				1 => _expression, 
				2 => _closeParenToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ParenthesizedExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _openParenToken, _expression, _closeParenToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ParenthesizedExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _openParenToken, _expression, _closeParenToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitParenthesizedExpression(this);
		}
	}
}
