using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ExpressionStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal ExpressionStatementSyntax(SyntaxKind kind, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionStatementSyntax(SyntaxKind kind, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_expression = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
		}

		static ExpressionStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new ExpressionStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ExpressionStatementSyntax), (ObjectReader r) => new ExpressionStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _expression;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ExpressionStatementSyntax(base.Kind, newErrors, GetAnnotations(), _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ExpressionStatementSyntax(base.Kind, GetDiagnostics(), annotations, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitExpressionStatement(this);
		}
	}
}
