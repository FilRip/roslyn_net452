using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConversion : BoundConversionOrCast
	{
		private readonly BoundExpression _Operand;

		private readonly ConversionKind _ConversionKind;

		private readonly bool _Checked;

		private readonly bool _ExplicitCastInCode;

		private readonly ConstantValue _ConstantValueOpt;

		private readonly BoundExtendedConversionInfo _ExtendedInfoOpt;

		public override Symbol ExpressionSymbol
		{
			get
			{
				MethodSymbol result = null;
				if ((ConversionKind & ConversionKind.UserDefined) != 0 && Operand.Kind == BoundKind.UserDefinedConversion)
				{
					BoundExpression boundExpression = ((BoundUserDefinedConversion)Operand).UnderlyingExpression;
					if (boundExpression.Kind == BoundKind.Conversion)
					{
						boundExpression = ((BoundConversion)boundExpression).Operand;
					}
					if (boundExpression.Kind == BoundKind.Call)
					{
						result = ((BoundCall)boundExpression).Method;
					}
				}
				return result;
			}
		}

		public override BoundExpression Operand => _Operand;

		public override ConversionKind ConversionKind => _ConversionKind;

		public bool Checked => _Checked;

		public override bool ExplicitCastInCode => _ExplicitCastInCode;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public BoundExtendedConversionInfo ExtendedInfoOpt => _ExtendedInfoOpt;

		public BoundConversion(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, bool @checked, bool explicitCastInCode, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, @checked, explicitCastInCode, null, type, hasErrors)
		{
		}

		public BoundConversion(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, bool @checked, bool explicitCastInCode, ConstantValue constantValueOpt, TypeSymbol type, bool hasErrors = false)
			: this(syntax, operand, conversionKind, @checked, explicitCastInCode, constantValueOpt, null, type, hasErrors)
		{
		}

		public BoundConversion(SyntaxNode syntax, BoundExpression operand, ConversionKind conversionKind, bool @checked, bool explicitCastInCode, ConstantValue constantValueOpt, BoundExtendedConversionInfo extendedInfoOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Conversion, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(operand) || BoundNodeExtensions.NonNullAndHasErrors(extendedInfoOpt))
		{
			_Operand = operand;
			_ConversionKind = conversionKind;
			_Checked = @checked;
			_ExplicitCastInCode = explicitCastInCode;
			_ConstantValueOpt = constantValueOpt;
			_ExtendedInfoOpt = extendedInfoOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConversion(this);
		}

		public BoundConversion Update(BoundExpression operand, ConversionKind conversionKind, bool @checked, bool explicitCastInCode, ConstantValue constantValueOpt, BoundExtendedConversionInfo extendedInfoOpt, TypeSymbol type)
		{
			if (operand != Operand || conversionKind != ConversionKind || @checked != Checked || explicitCastInCode != ExplicitCastInCode || (object)constantValueOpt != ConstantValueOpt || extendedInfoOpt != ExtendedInfoOpt || (object)type != base.Type)
			{
				BoundConversion boundConversion = new BoundConversion(base.Syntax, operand, conversionKind, @checked, explicitCastInCode, constantValueOpt, extendedInfoOpt, type, base.HasErrors);
				boundConversion.CopyAttributes(this);
				return boundConversion;
			}
			return this;
		}
	}
}
