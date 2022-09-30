using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundReferenceAssignment : BoundExpression
	{
		private readonly BoundLocal _ByRefLocal;

		private readonly BoundExpression _LValue;

		private readonly bool _IsLValue;

		public BoundLocal ByRefLocal => _ByRefLocal;

		public BoundExpression LValue => _LValue;

		public override bool IsLValue => _IsLValue;

		protected override BoundExpression MakeRValueImpl()
		{
			return MakeRValue();
		}

		public new BoundReferenceAssignment MakeRValue()
		{
			if (_IsLValue)
			{
				return Update(ByRefLocal, LValue, isLValue: false, base.Type);
			}
			return this;
		}

		public BoundReferenceAssignment(SyntaxNode syntax, BoundLocal byRefLocal, BoundExpression lValue, bool isLValue, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ReferenceAssignment, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(byRefLocal) || BoundNodeExtensions.NonNullAndHasErrors(lValue))
		{
			_ByRefLocal = byRefLocal;
			_LValue = lValue;
			_IsLValue = isLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitReferenceAssignment(this);
		}

		public BoundReferenceAssignment Update(BoundLocal byRefLocal, BoundExpression lValue, bool isLValue, TypeSymbol type)
		{
			if (byRefLocal != ByRefLocal || lValue != LValue || isLValue != IsLValue || (object)type != base.Type)
			{
				BoundReferenceAssignment boundReferenceAssignment = new BoundReferenceAssignment(base.Syntax, byRefLocal, lValue, isLValue, type, base.HasErrors);
				boundReferenceAssignment.CopyAttributes(this);
				return boundReferenceAssignment;
			}
			return this;
		}
	}
}
