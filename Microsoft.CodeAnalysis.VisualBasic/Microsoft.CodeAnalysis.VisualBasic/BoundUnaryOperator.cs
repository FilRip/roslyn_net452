using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUnaryOperator : BoundExpression
	{
		private readonly UnaryOperatorKind _OperatorKind;

		private readonly BoundExpression _Operand;

		private readonly bool _Checked;

		private readonly ConstantValue _ConstantValueOpt;

		public override Symbol ExpressionSymbol
		{
			get
			{
				if ((OperatorKind & UnaryOperatorKind.Error) == 0)
				{
					string text = OverloadResolution.TryGetOperatorName(OperatorKind);
					if (text != null)
					{
						UnaryOperatorKind unaryOperatorKind = OperatorKind & UnaryOperatorKind.OpMask;
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(Operand.Type);
						return new SynthesizedIntrinsicOperatorSymbol(namedTypeSymbol, text, TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(base.Type), Checked && TypeSymbolExtensions.IsIntegralType(namedTypeSymbol) && unaryOperatorKind == UnaryOperatorKind.Minus);
					}
				}
				return null;
			}
		}

		public UnaryOperatorKind OperatorKind => _OperatorKind;

		public BoundExpression Operand => _Operand;

		public bool Checked => _Checked;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, bool @checked, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operatorKind, operand, @checked, null, type, hasErrors || operand.HasErrors)
		{
		}

		public BoundUnaryOperator(SyntaxNode syntax, UnaryOperatorKind operatorKind, BoundExpression operand, bool @checked, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UnaryOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand))
		{
			_OperatorKind = operatorKind;
			_Operand = operand;
			_Checked = @checked;
			_ConstantValueOpt = constantValueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnaryOperator(this);
		}

		public BoundUnaryOperator Update(UnaryOperatorKind operatorKind, BoundExpression operand, bool @checked, ConstantValue constantValueOpt, TypeSymbol type)
		{
			if (operatorKind != OperatorKind || operand != Operand || @checked != Checked || (object)constantValueOpt != ConstantValueOpt || (object)type != base.Type)
			{
				BoundUnaryOperator boundUnaryOperator = new BoundUnaryOperator(base.Syntax, operatorKind, operand, @checked, constantValueOpt, type, base.HasErrors);
				boundUnaryOperator.CopyAttributes(this);
				return boundUnaryOperator;
			}
			return this;
		}
	}
}
