using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ExpressionRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal readonly VariableNameEqualsSyntax _nameEquals;

		internal readonly ExpressionSyntax _expression;

		internal static Func<ObjectReader, object> CreateInstance;

		internal VariableNameEqualsSyntax NameEquals => _nameEquals;

		internal ExpressionSyntax Expression => _expression;

		internal ExpressionRangeVariableSyntax(SyntaxKind kind, VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
			: base(kind)
		{
			base._slotCount = 2;
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionRangeVariableSyntax(SyntaxKind kind, VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			if (nameEquals != null)
			{
				AdjustFlagsAndWidth(nameEquals);
				_nameEquals = nameEquals;
			}
			AdjustFlagsAndWidth(expression);
			_expression = expression;
		}

		internal ExpressionRangeVariableSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			VariableNameEqualsSyntax variableNameEqualsSyntax = (VariableNameEqualsSyntax)reader.ReadValue();
			if (variableNameEqualsSyntax != null)
			{
				AdjustFlagsAndWidth(variableNameEqualsSyntax);
				_nameEquals = variableNameEqualsSyntax;
			}
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
			writer.WriteValue(_nameEquals);
			writer.WriteValue(_expression);
		}

		static ExpressionRangeVariableSyntax()
		{
			CreateInstance = (ObjectReader o) => new ExpressionRangeVariableSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ExpressionRangeVariableSyntax), (ObjectReader r) => new ExpressionRangeVariableSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _nameEquals, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ExpressionRangeVariableSyntax(base.Kind, newErrors, GetAnnotations(), _nameEquals, _expression);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ExpressionRangeVariableSyntax(base.Kind, GetDiagnostics(), annotations, _nameEquals, _expression);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitExpressionRangeVariable(this);
		}
	}
}
