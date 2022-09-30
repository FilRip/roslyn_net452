using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class OrderingSyntax : VisualBasicSyntaxNode
	{
		internal readonly ExpressionSyntax _expression;

		internal readonly KeywordSyntax _ascendingOrDescendingKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal KeywordSyntax AscendingOrDescendingKeyword => _ascendingOrDescendingKeyword;

		internal OrderingSyntax(SyntaxKind kind, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (ascendingOrDescendingKeyword != null)
			{
				AdjustFlagsAndWidth(ascendingOrDescendingKeyword);
				_ascendingOrDescendingKeyword = ascendingOrDescendingKeyword;
			}
		}

		internal OrderingSyntax(SyntaxKind kind, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (ascendingOrDescendingKeyword != null)
			{
				AdjustFlagsAndWidth(ascendingOrDescendingKeyword);
				_ascendingOrDescendingKeyword = ascendingOrDescendingKeyword;
			}
		}

		internal OrderingSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			if (ascendingOrDescendingKeyword != null)
			{
				AdjustFlagsAndWidth(ascendingOrDescendingKeyword);
				_ascendingOrDescendingKeyword = ascendingOrDescendingKeyword;
			}
		}

		internal OrderingSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_ascendingOrDescendingKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
			writer.WriteValue(_ascendingOrDescendingKeyword);
		}

		static OrderingSyntax()
		{
			CreateInstance = (ObjectReader o) => new OrderingSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(OrderingSyntax), (ObjectReader r) => new OrderingSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderingSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _ascendingOrDescendingKeyword, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new OrderingSyntax(base.Kind, newErrors, GetAnnotations(), _expression, _ascendingOrDescendingKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new OrderingSyntax(base.Kind, GetDiagnostics(), annotations, _expression, _ascendingOrDescendingKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitOrdering(this);
		}
	}
}
