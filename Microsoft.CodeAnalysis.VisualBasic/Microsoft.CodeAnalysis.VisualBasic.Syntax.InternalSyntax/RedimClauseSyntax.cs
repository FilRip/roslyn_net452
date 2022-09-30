using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class RedimClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly ExpressionSyntax _expression;

		internal readonly ArgumentListSyntax _arrayBounds;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Expression => _expression;

		internal ArgumentListSyntax ArrayBounds => _arrayBounds;

		internal RedimClauseSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(arrayBounds);
			_arrayBounds = arrayBounds;
		}

		internal RedimClauseSyntax(SyntaxKind kind, ExpressionSyntax expression, ArgumentListSyntax arrayBounds, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(arrayBounds);
			_arrayBounds = arrayBounds;
		}

		internal RedimClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(expression);
			_expression = expression;
			AdjustFlagsAndWidth(arrayBounds);
			_arrayBounds = arrayBounds;
		}

		internal RedimClauseSyntax(ObjectReader reader)
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
				_arrayBounds = argumentListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_expression);
			writer.WriteValue(_arrayBounds);
		}

		static RedimClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new RedimClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(RedimClauseSyntax), (ObjectReader r) => new RedimClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _arrayBounds, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new RedimClauseSyntax(base.Kind, newErrors, GetAnnotations(), _expression, _arrayBounds);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new RedimClauseSyntax(base.Kind, GetDiagnostics(), annotations, _expression, _arrayBounds);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitRedimClause(this);
		}
	}
}
