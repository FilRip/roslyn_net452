using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTypeOf : BoundExpression
	{
		private readonly BoundExpression _Operand;

		private readonly bool _IsTypeOfIsNotExpression;

		private readonly TypeSymbol _TargetType;

		public BoundExpression Operand => _Operand;

		public bool IsTypeOfIsNotExpression => _IsTypeOfIsNotExpression;

		public TypeSymbol TargetType => _TargetType;

		public BoundTypeOf(SyntaxNode syntax, BoundExpression operand, bool isTypeOfIsNotExpression, TypeSymbol targetType, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TypeOf, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand))
		{
			_Operand = operand;
			_IsTypeOfIsNotExpression = isTypeOfIsNotExpression;
			_TargetType = targetType;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTypeOf(this);
		}

		public BoundTypeOf Update(BoundExpression operand, bool isTypeOfIsNotExpression, TypeSymbol targetType, TypeSymbol type)
		{
			if (operand != Operand || isTypeOfIsNotExpression != IsTypeOfIsNotExpression || (object)targetType != TargetType || (object)type != base.Type)
			{
				BoundTypeOf boundTypeOf = new BoundTypeOf(base.Syntax, operand, isTypeOfIsNotExpression, targetType, type, base.HasErrors);
				boundTypeOf.CopyAttributes(this);
				return boundTypeOf;
			}
			return this;
		}
	}
}
