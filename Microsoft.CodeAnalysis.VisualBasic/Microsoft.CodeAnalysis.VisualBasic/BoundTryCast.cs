using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTryCast : BoundConversionOrCast
	{
		private readonly BoundExpression _Operand;

		private readonly ConversionKind _ConversionKind;

		private readonly ConstantValue _ConstantValueOpt;

		private readonly BoundLambda _RelaxationLambdaOpt;

		public override bool ExplicitCastInCode => true;

		public override BoundExpression Operand => _Operand;

		public override ConversionKind ConversionKind => _ConversionKind;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundLambda RelaxationLambdaOpt => _RelaxationLambdaOpt;

		public BoundTryCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, (ConstantValue)null, type, hasErrors)
		{
		}

		public BoundTryCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, BoundLambda relaxationLambdaOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, null, relaxationLambdaOpt, type, hasErrors)
		{
		}

		public BoundTryCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, constantValueOpt, null, type, hasErrors)
		{
		}

		public BoundTryCast(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, ConstantValue constantValueOpt, BoundLambda relaxationLambdaOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.TryCast, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand) || BoundNodeExtensions.NonNullAndHasErrors(relaxationLambdaOpt))
		{
			_Operand = operand;
			_ConversionKind = conversionKind;
			_ConstantValueOpt = constantValueOpt;
			_RelaxationLambdaOpt = relaxationLambdaOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitTryCast(this);
		}

		public BoundTryCast Update(BoundExpression operand, ConversionKind conversionKind, ConstantValue constantValueOpt, BoundLambda relaxationLambdaOpt, TypeSymbol type)
		{
			if (operand != Operand || conversionKind != ConversionKind || (object)constantValueOpt != ConstantValueOpt || relaxationLambdaOpt != RelaxationLambdaOpt || (object)type != base.Type)
			{
				BoundTryCast boundTryCast = new BoundTryCast(base.Syntax, operand, conversionKind, constantValueOpt, relaxationLambdaOpt, type, base.HasErrors);
				boundTryCast.CopyAttributes(this);
				return boundTryCast;
			}
			return this;
		}
	}
}
