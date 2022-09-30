using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundDirectCast : BoundConversionOrCast
	{
		private readonly BoundExpression _Operand;

		private readonly ConversionKind _ConversionKind;

		private readonly bool _SuppressVirtualCalls;

		private readonly ConstantValue _ConstantValueOpt;

		private readonly BoundLambda _RelaxationLambdaOpt;

		public override bool ExplicitCastInCode => true;

		public override BoundExpression Operand => _Operand;

		public override ConversionKind ConversionKind => _ConversionKind;

		public override bool SuppressVirtualCalls => _SuppressVirtualCalls;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundLambda RelaxationLambdaOpt => _RelaxationLambdaOpt;

		public BoundDirectCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, suppressVirtualCalls: false, null, null, type, hasErrors)
		{
		}

		public BoundDirectCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, BoundLambda relaxationLambdaOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, suppressVirtualCalls: false, null, relaxationLambdaOpt, type, hasErrors)
		{
		}

		public BoundDirectCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, suppressVirtualCalls: false, constantValueOpt, null, type, hasErrors)
		{
		}

		public BoundDirectCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, bool suppressVirtualCalls, ConstantValue constantValueOpt, BoundLambda relaxationLambdaOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.DirectCast, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand) || BoundNodeExtensions.NonNullAndHasErrors(relaxationLambdaOpt))
		{
			_Operand = operand;
			_ConversionKind = conversionKind;
			_SuppressVirtualCalls = suppressVirtualCalls;
			_ConstantValueOpt = constantValueOpt;
			_RelaxationLambdaOpt = relaxationLambdaOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitDirectCast(this);
		}

		public BoundDirectCast Update(BoundExpression operand, ConversionKind conversionKind, bool suppressVirtualCalls, ConstantValue constantValueOpt, BoundLambda relaxationLambdaOpt, TypeSymbol type)
		{
			if (operand != Operand || conversionKind != ConversionKind || suppressVirtualCalls != SuppressVirtualCalls || (object)constantValueOpt != ConstantValueOpt || relaxationLambdaOpt != RelaxationLambdaOpt || (object)type != base.Type)
			{
				BoundDirectCast boundDirectCast = new BoundDirectCast(base.Syntax, operand, conversionKind, suppressVirtualCalls, constantValueOpt, relaxationLambdaOpt, type, base.HasErrors);
				boundDirectCast.CopyAttributes(this);
				return boundDirectCast;
			}
			return this;
		}
	}
}
