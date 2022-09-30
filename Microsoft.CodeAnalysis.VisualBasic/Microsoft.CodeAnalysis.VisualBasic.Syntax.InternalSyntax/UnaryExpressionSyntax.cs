using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class UnaryExpressionSyntax : ExpressionSyntax
	{
		internal readonly SyntaxToken _operatorToken;

		internal readonly ExpressionSyntax _operand;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyntaxToken OperatorToken => _operatorToken;

		internal ExpressionSyntax Operand => _operand;

		internal UnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(operand);
			_operand = operand;
		}

		internal UnaryExpressionSyntax(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(operand);
			_operand = operand;
		}

		internal UnaryExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxToken operatorToken, ExpressionSyntax operand)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(operand);
			_operand = operand;
		}

		internal UnaryExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_operatorToken = syntaxToken;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_operand = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_operand);
		}

		static UnaryExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new UnaryExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(UnaryExpressionSyntax), (ObjectReader r) => new UnaryExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.UnaryExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _operatorToken, 
				1 => _operand, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new UnaryExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _operatorToken, _operand);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new UnaryExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _operatorToken, _operand);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitUnaryExpression(this);
		}
	}
}
