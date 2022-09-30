using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSequence : BoundExpression
	{
		private readonly ImmutableArray<LocalSymbol> _Locals;

		private readonly ImmutableArray<BoundExpression> _SideEffects;

		private readonly BoundExpression _ValueOpt;

		public override bool IsLValue
		{
			get
			{
				if (ValueOpt != null)
				{
					return ValueOpt.IsLValue;
				}
				return false;
			}
		}

		public ImmutableArray<LocalSymbol> Locals => _Locals;

		public ImmutableArray<BoundExpression> SideEffects => _SideEffects;

		public BoundExpression ValueOpt => _ValueOpt;

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundSequence MakeRValue()
		{
			if (IsLValue)
			{
				return Update(_Locals, _SideEffects, ValueOpt.MakeRValue(), base.Type);
			}
			return this;
		}

		public BoundSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression valueOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Sequence, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(sideEffects) || BoundNodeExtensions.NonNullAndHasErrors(valueOpt))
		{
			_Locals = locals;
			_SideEffects = sideEffects;
			_ValueOpt = valueOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSequence(this);
		}

		public BoundSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression valueOpt, TypeSymbol type)
		{
			if (locals != Locals || sideEffects != SideEffects || valueOpt != ValueOpt || (object)type != base.Type)
			{
				BoundSequence boundSequence = new BoundSequence(base.Syntax, locals, sideEffects, valueOpt, type, base.HasErrors);
				boundSequence.CopyAttributes(this);
				return boundSequence;
			}
			return this;
		}
	}
}
