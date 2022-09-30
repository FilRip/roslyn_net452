using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlEmbeddedExpressionSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanPercentEqualsToken;

		internal readonly ExpressionSyntax _expression;

		internal readonly PunctuationSyntax _percentGreaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanPercentEqualsToken => _lessThanPercentEqualsToken;

		internal ExpressionSyntax Expression => _expression;

		internal PunctuationSyntax PercentGreaterThanToken => _percentGreaterThanToken;

		internal XmlEmbeddedExpressionSyntax(SyntaxKind kind, PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanPercentEqualsToken);
			_lessThanPercentEqualsToken = lessThanPercentEqualsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(percentGreaterThanToken);
			_percentGreaterThanToken = percentGreaterThanToken;
		}

		internal XmlEmbeddedExpressionSyntax(SyntaxKind kind, PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanPercentEqualsToken);
			_lessThanPercentEqualsToken = lessThanPercentEqualsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(percentGreaterThanToken);
			_percentGreaterThanToken = percentGreaterThanToken;
		}

		internal XmlEmbeddedExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(lessThanPercentEqualsToken);
			_lessThanPercentEqualsToken = lessThanPercentEqualsToken;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(percentGreaterThanToken);
			_percentGreaterThanToken = percentGreaterThanToken;
		}

		internal XmlEmbeddedExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanPercentEqualsToken = punctuationSyntax;
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
				_percentGreaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanPercentEqualsToken);
			writer.WriteValue(_expression);
			writer.WriteValue(_percentGreaterThanToken);
		}

		static XmlEmbeddedExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlEmbeddedExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlEmbeddedExpressionSyntax), (ObjectReader r) => new XmlEmbeddedExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlEmbeddedExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanPercentEqualsToken, 
				1 => _expression, 
				2 => _percentGreaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlEmbeddedExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanPercentEqualsToken, _expression, _percentGreaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlEmbeddedExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanPercentEqualsToken, _expression, _percentGreaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlEmbeddedExpression(this);
		}
	}
}
