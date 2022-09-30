using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAssignmentOperator : BoundExpression
	{
		private readonly BoundExpression _Left;

		private readonly BoundCompoundAssignmentTargetPlaceholder _LeftOnTheRightOpt;

		private readonly BoundExpression _Right;

		private readonly bool _SuppressObjectClone;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Left, (BoundNode)Right);

		public BoundExpression Left => _Left;

		public BoundCompoundAssignmentTargetPlaceholder LeftOnTheRightOpt => _LeftOnTheRightOpt;

		public BoundExpression Right => _Right;

		public bool SuppressObjectClone => _SuppressObjectClone;

		public BoundAssignmentOperator(SyntaxNode syntax, BoundExpression left, BoundExpression right, bool suppressObjectClone, TypeSymbol type, bool hasErrors = false)
			: this(syntax, left, null, right, suppressObjectClone, type, hasErrors)
		{
		}

		public BoundAssignmentOperator(SyntaxNode syntax, BoundExpression left, BoundExpression right, bool suppressObjectClone, bool hasErrors = false)
			: this(syntax, left, null, right, suppressObjectClone, hasErrors)
		{
		}

		public BoundAssignmentOperator(SyntaxNode syntax, BoundExpression left, BoundCompoundAssignmentTargetPlaceholder leftOnTheRightOpt, BoundExpression right, bool suppressObjectClone, bool hasErrors = false)
			: this(syntax, left, leftOnTheRightOpt, right, suppressObjectClone, BoundExpressionExtensions.IsPropertyOrXmlPropertyAccess(left) ? BoundExpressionExtensions.GetPropertyOrXmlProperty(left).ContainingAssembly.GetSpecialType(SpecialType.System_Void) : (BoundExpressionExtensions.IsLateBound(left) ? left.Type.ContainingAssembly.GetSpecialType(SpecialType.System_Void) : left.Type), hasErrors)
		{
		}

		public BoundAssignmentOperator(SyntaxNode syntax, BoundExpression left, BoundCompoundAssignmentTargetPlaceholder leftOnTheRightOpt, BoundExpression right, bool suppressObjectClone, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.AssignmentOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(left) || BoundNodeExtensions.NonNullAndHasErrors(leftOnTheRightOpt) || BoundNodeExtensions.NonNullAndHasErrors(right))
		{
			_Left = left;
			_LeftOnTheRightOpt = leftOnTheRightOpt;
			_Right = right;
			_SuppressObjectClone = suppressObjectClone;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAssignmentOperator(this);
		}

		public BoundAssignmentOperator Update(BoundExpression left, BoundCompoundAssignmentTargetPlaceholder leftOnTheRightOpt, BoundExpression right, bool suppressObjectClone, TypeSymbol type)
		{
			if (left != Left || leftOnTheRightOpt != LeftOnTheRightOpt || right != Right || suppressObjectClone != SuppressObjectClone || (object)type != base.Type)
			{
				BoundAssignmentOperator boundAssignmentOperator = new BoundAssignmentOperator(base.Syntax, left, leftOnTheRightOpt, right, suppressObjectClone, type, base.HasErrors);
				boundAssignmentOperator.CopyAttributes(this);
				return boundAssignmentOperator;
			}
			return this;
		}
	}
}
