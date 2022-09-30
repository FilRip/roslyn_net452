using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBinaryOperator : BoundExpression
	{
		private readonly BinaryOperatorKind _OperatorKind;

		private readonly BoundExpression _Left;

		private readonly BoundExpression _Right;

		private readonly bool _Checked;

		private readonly ConstantValue _ConstantValueOpt;

		public override Symbol ExpressionSymbol
		{
			get
			{
				if ((OperatorKind & BinaryOperatorKind.Error) == 0)
				{
					string text = OverloadResolution.TryGetOperatorName(OperatorKind);
					if (text != null)
					{
						BinaryOperatorKind binaryOperatorKind = OperatorKind & BinaryOperatorKind.OpMask;
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(Left.Type);
						return new SynthesizedIntrinsicOperatorSymbol(namedTypeSymbol, text, TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(Right.Type), TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(base.Type), Checked && TypeSymbolExtensions.IsIntegralType(namedTypeSymbol) && (binaryOperatorKind == BinaryOperatorKind.Multiply || binaryOperatorKind == BinaryOperatorKind.Add || binaryOperatorKind == BinaryOperatorKind.Subtract || binaryOperatorKind == BinaryOperatorKind.IntegerDivide));
					}
				}
				return null;
			}
		}

		public BinaryOperatorKind OperatorKind => _OperatorKind;

		public BoundExpression Left => _Left;

		public BoundExpression Right => _Right;

		public bool Checked => _Checked;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, bool @checked, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operatorKind, left, right, @checked, null, type, hasErrors)
		{
		}

		public BoundBinaryOperator(SyntaxNode syntax, BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, bool @checked, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.BinaryOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(left) || BoundNodeExtensions.NonNullAndHasErrors(right))
		{
			_OperatorKind = operatorKind;
			_Left = left;
			_Right = right;
			_Checked = @checked;
			_ConstantValueOpt = constantValueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBinaryOperator(this);
		}

		public BoundBinaryOperator Update(BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, bool @checked, ConstantValue constantValueOpt, TypeSymbol type)
		{
			if (operatorKind != OperatorKind || left != Left || right != Right || @checked != Checked || (object)constantValueOpt != ConstantValueOpt || (object)type != base.Type)
			{
				BoundBinaryOperator boundBinaryOperator = new BoundBinaryOperator(base.Syntax, operatorKind, left, right, @checked, constantValueOpt, type, base.HasErrors);
				boundBinaryOperator.CopyAttributes(this);
				return boundBinaryOperator;
			}
			return this;
		}
	}
}
