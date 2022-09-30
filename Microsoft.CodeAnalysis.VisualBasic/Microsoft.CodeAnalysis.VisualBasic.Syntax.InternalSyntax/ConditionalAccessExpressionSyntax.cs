using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ConditionalAccessExpressionSyntax : ExpressionSyntax
	{
		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _questionMarkToken;

		internal readonly ExpressionSyntax _whenNotNull;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax QuestionMarkToken => _questionMarkToken;

		internal ExpressionSyntax WhenNotNull => _whenNotNull;

		internal ConditionalAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull)
			: base(kind)
		{
			base._slotCount = 3;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
			AdjustFlagsAndWidth(whenNotNull);
			_whenNotNull = whenNotNull;
		}

		internal ConditionalAccessExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
			AdjustFlagsAndWidth(whenNotNull);
			_whenNotNull = whenNotNull;
		}

		internal ConditionalAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
			AdjustFlagsAndWidth(whenNotNull);
			_whenNotNull = whenNotNull;
		}

		internal ConditionalAccessExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_questionMarkToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_whenNotNull = expressionSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
			writer.WriteValue(_questionMarkToken);
			writer.WriteValue(_whenNotNull);
		}

		static ConditionalAccessExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new ConditionalAccessExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ConditionalAccessExpressionSyntax), (ObjectReader r) => new ConditionalAccessExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _questionMarkToken, 
				2 => _whenNotNull, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ConditionalAccessExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _expression, _questionMarkToken, _whenNotNull);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ConditionalAccessExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _expression, _questionMarkToken, _whenNotNull);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitConditionalAccessExpression(this);
		}
	}
}
