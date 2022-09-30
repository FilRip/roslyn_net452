using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundCall : BoundExpression
	{
		private readonly MethodSymbol _Method;

		private readonly BoundMethodGroup _MethodGroupOpt;

		private readonly BoundExpression _ReceiverOpt;

		private readonly ImmutableArray<BoundExpression> _Arguments;

		private readonly BitVector _DefaultArguments;

		private readonly ConstantValue _ConstantValueOpt;

		private readonly bool _IsLValue;

		private readonly bool _SuppressObjectClone;

		public override Symbol ExpressionSymbol => Method;

		public override LookupResultKind ResultKind
		{
			get
			{
				if (MethodGroupOpt != null)
				{
					return MethodGroupOpt.ResultKind;
				}
				return base.ResultKind;
			}
		}

		public MethodSymbol Method => _Method;

		public BoundMethodGroup MethodGroupOpt => _MethodGroupOpt;

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public ImmutableArray<BoundExpression> Arguments => _Arguments;

		public BitVector DefaultArguments => _DefaultArguments;

		public override ConstantValue ConstantValueOpt => _ConstantValueOpt;

		public override bool IsLValue => _IsLValue;

		public bool SuppressObjectClone => _SuppressObjectClone;

		public BoundCall(SyntaxNode syntax, MethodSymbol method, BoundMethodGroup methodGroupOpt, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, ConstantValue constantValueOpt, TypeSymbol type, bool suppressObjectClone = false, bool hasErrors = false, BitVector defaultArguments = default(BitVector))
			: this(syntax, method, methodGroupOpt, receiverOpt, arguments, defaultArguments, constantValueOpt, method.ReturnsByRef, suppressObjectClone, type, hasErrors)
		{
		}

		public BoundCall(SyntaxNode syntax, MethodSymbol method, BoundMethodGroup methodGroupOpt, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, ConstantValue constantValueOpt, bool isLValue, bool suppressObjectClone, TypeSymbol type, bool hasErrors = false)
			: this(syntax, method, methodGroupOpt, receiverOpt, arguments, BitVector.Null, constantValueOpt, isLValue, suppressObjectClone, type, hasErrors)
		{
		}

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundCall MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(Method, MethodGroupOpt, ReceiverOpt, Arguments, DefaultArguments, ConstantValueOpt, isLValue: false, SuppressObjectClone, base.Type);
			}
			return this;
		}

		public BoundCall(SyntaxNode syntax, MethodSymbol method, BoundMethodGroup methodGroupOpt, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, ConstantValue constantValueOpt, bool isLValue, bool suppressObjectClone, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Call, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(methodGroupOpt) || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt) || BoundNodeExtensions.NonNullAndHasErrors(arguments))
		{
			_Method = method;
			_MethodGroupOpt = methodGroupOpt;
			_ReceiverOpt = receiverOpt;
			_Arguments = arguments;
			_DefaultArguments = defaultArguments;
			_ConstantValueOpt = constantValueOpt;
			_IsLValue = isLValue;
			_SuppressObjectClone = suppressObjectClone;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitCall(this);
		}

		public BoundCall Update(MethodSymbol method, BoundMethodGroup methodGroupOpt, BoundExpression receiverOpt, ImmutableArray<BoundExpression> arguments, BitVector defaultArguments, ConstantValue constantValueOpt, bool isLValue, bool suppressObjectClone, TypeSymbol type)
		{
			if ((object)method != Method || methodGroupOpt != MethodGroupOpt || receiverOpt != ReceiverOpt || arguments != Arguments || defaultArguments != DefaultArguments || (object)constantValueOpt != ConstantValueOpt || isLValue != IsLValue || suppressObjectClone != SuppressObjectClone || (object)type != base.Type)
			{
				BoundCall boundCall = new BoundCall(base.Syntax, method, methodGroupOpt, receiverOpt, arguments, defaultArguments, constantValueOpt, isLValue, suppressObjectClone, type, base.HasErrors);
				boundCall.CopyAttributes(this);
				return boundCall;
			}
			return this;
		}
	}
}
