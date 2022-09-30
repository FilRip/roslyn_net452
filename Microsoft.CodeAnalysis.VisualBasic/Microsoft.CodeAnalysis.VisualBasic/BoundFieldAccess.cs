using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundFieldAccess : BoundExpression
	{
		private readonly BoundExpression _ReceiverOpt;

		private readonly FieldSymbol _FieldSymbol;

		private readonly bool _IsLValue;

		private readonly bool _SuppressVirtualCalls;

		private readonly ConstantFieldsInProgress _ConstantsInProgressOpt;

		public override Symbol ExpressionSymbol => FieldSymbol;

		public override ConstantValue ConstantValueOpt
		{
			get
			{
				if (_IsLValue)
				{
					return null;
				}
				ConstantFieldsInProgress constantsInProgressOpt = ConstantsInProgressOpt;
				return (constantsInProgressOpt == null) ? FieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty) : FieldSymbol.GetConstantValue(constantsInProgressOpt);
			}
		}

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public FieldSymbol FieldSymbol => _FieldSymbol;

		public override bool IsLValue => _IsLValue;

		public override bool SuppressVirtualCalls => _SuppressVirtualCalls;

		public ConstantFieldsInProgress ConstantsInProgressOpt => _ConstantsInProgressOpt;

		public BoundFieldAccess(SyntaxNode syntax, BoundExpression receiverOpt, FieldSymbol fieldSymbol, bool isLValue, TypeSymbol type, bool hasErrors = false)
			: this(syntax, receiverOpt, fieldSymbol, isLValue, suppressVirtualCalls: false, null, type, hasErrors)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundFieldAccess MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(_ReceiverOpt, _FieldSymbol, isLValue: false, SuppressVirtualCalls, ConstantsInProgressOpt, base.Type);
			}
			return this;
		}

		public BoundFieldAccess(SyntaxNode syntax, BoundExpression receiverOpt, FieldSymbol fieldSymbol, bool isLValue, bool suppressVirtualCalls, ConstantFieldsInProgress constantsInProgressOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.FieldAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt))
		{
			_ReceiverOpt = receiverOpt;
			_FieldSymbol = fieldSymbol;
			_IsLValue = isLValue;
			_SuppressVirtualCalls = suppressVirtualCalls;
			_ConstantsInProgressOpt = constantsInProgressOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitFieldAccess(this);
		}

		public BoundFieldAccess Update(BoundExpression receiverOpt, FieldSymbol fieldSymbol, bool isLValue, bool suppressVirtualCalls, ConstantFieldsInProgress constantsInProgressOpt, TypeSymbol type)
		{
			if (receiverOpt != ReceiverOpt || (object)fieldSymbol != FieldSymbol || isLValue != IsLValue || suppressVirtualCalls != SuppressVirtualCalls || constantsInProgressOpt != ConstantsInProgressOpt || (object)type != base.Type)
			{
				BoundFieldAccess boundFieldAccess = new BoundFieldAccess(base.Syntax, receiverOpt, fieldSymbol, isLValue, suppressVirtualCalls, constantsInProgressOpt, type, base.HasErrors);
				boundFieldAccess.CopyAttributes(this);
				return boundFieldAccess;
			}
			return this;
		}
	}
}
