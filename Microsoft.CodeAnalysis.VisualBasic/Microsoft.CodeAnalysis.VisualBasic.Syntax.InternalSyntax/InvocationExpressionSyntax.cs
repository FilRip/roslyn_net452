using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class InvocationExpressionSyntax : ExpressionSyntax
	{
		internal readonly ExpressionSyntax _expression;

		internal readonly ArgumentListSyntax _argumentList;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal ArgumentListSyntax ArgumentList => _argumentList;

		internal InvocationExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax argumentList)
			: base(kind)
		{
			base._slotCount = 2;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal InvocationExpressionSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax argumentList, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal InvocationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, ArgumentListSyntax argumentList)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (expression != null)
			{
				AdjustFlagsAndWidth(expression);
				_expression = expression;
			}
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal InvocationExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)reader.ReadValue();
			if (argumentListSyntax != null)
			{
				AdjustFlagsAndWidth(argumentListSyntax);
				_argumentList = argumentListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
			writer.WriteValue(_argumentList);
		}

		static InvocationExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new InvocationExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(InvocationExpressionSyntax), (ObjectReader r) => new InvocationExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _argumentList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new InvocationExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _expression, _argumentList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new InvocationExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _expression, _argumentList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitInvocationExpression(this);
		}
	}
}
