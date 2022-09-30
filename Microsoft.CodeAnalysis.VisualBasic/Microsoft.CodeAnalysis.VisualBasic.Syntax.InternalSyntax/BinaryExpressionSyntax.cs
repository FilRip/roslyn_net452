using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class BinaryExpressionSyntax : ExpressionSyntax
	{
		internal readonly ExpressionSyntax _left;

		internal readonly SyntaxToken _operatorToken;

		internal readonly ExpressionSyntax _right;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ExpressionSyntax Left => _left;

		internal SyntaxToken OperatorToken => _operatorToken;

		internal ExpressionSyntax Right => _right;

		internal BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal BinaryExpressionSyntax(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal BinaryExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(left);
			_left = left;
			AdjustFlagsAndWidth(operatorToken);
			_operatorToken = operatorToken;
			AdjustFlagsAndWidth(right);
			_right = right;
		}

		internal BinaryExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_left = expressionSyntax;
			}
			SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
			if (syntaxToken != null)
			{
				AdjustFlagsAndWidth(syntaxToken);
				_operatorToken = syntaxToken;
			}
			ExpressionSyntax expressionSyntax2 = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax2 != null)
			{
				AdjustFlagsAndWidth(expressionSyntax2);
				_right = expressionSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_left);
			writer.WriteValue(_operatorToken);
			writer.WriteValue(_right);
		}

		static BinaryExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new BinaryExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(BinaryExpressionSyntax), (ObjectReader r) => new BinaryExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _left, 
				1 => _operatorToken, 
				2 => _right, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new BinaryExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _left, _operatorToken, _right);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new BinaryExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _left, _operatorToken, _right);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitBinaryExpression(this);
		}
	}
}
