using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLateAddressOfOperator : BoundExpression
	{
		private readonly Binder _Binder;

		private readonly BoundLateMemberAccess _MemberAccess;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)MemberAccess);

		public Binder Binder => _Binder;

		public BoundLateMemberAccess MemberAccess => _MemberAccess;

		public BoundLateAddressOfOperator(SyntaxNode syntax, Binder binder, BoundLateMemberAccess memberAccess, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LateAddressOfOperator, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(memberAccess))
		{
			_Binder = binder;
			_MemberAccess = memberAccess;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLateAddressOfOperator(this);
		}

		public BoundLateAddressOfOperator Update(Binder binder, BoundLateMemberAccess memberAccess, TypeSymbol type)
		{
			if (binder != Binder || memberAccess != MemberAccess || (object)type != base.Type)
			{
				BoundLateAddressOfOperator boundLateAddressOfOperator = new BoundLateAddressOfOperator(base.Syntax, binder, memberAccess, type, base.HasErrors);
				boundLateAddressOfOperator.CopyAttributes(this);
				return boundLateAddressOfOperator;
			}
			return this;
		}
	}
}
