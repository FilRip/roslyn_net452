using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNullableIsTrueOperator : BoundExpression, IBoundInvalidNode
	{
		private readonly BoundExpression _Operand;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Operand);

		private ImmutableArray<BoundNode> IBoundInvalidNode_InvalidNodeChildren => ImmutableArray.Create((BoundNode)Operand);

		public BoundExpression Operand => _Operand;

		public BoundNullableIsTrueOperator(SyntaxNode syntax, BoundExpression operand, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.NullableIsTrueOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand))
		{
			_Operand = operand;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNullableIsTrueOperator(this);
		}

		public BoundNullableIsTrueOperator Update(BoundExpression operand, TypeSymbol type)
		{
			if (operand != Operand || (object)type != base.Type)
			{
				BoundNullableIsTrueOperator boundNullableIsTrueOperator = new BoundNullableIsTrueOperator(base.Syntax, operand, type, base.HasErrors);
				boundNullableIsTrueOperator.CopyAttributes(this);
				return boundNullableIsTrueOperator;
			}
			return this;
		}
	}
}
