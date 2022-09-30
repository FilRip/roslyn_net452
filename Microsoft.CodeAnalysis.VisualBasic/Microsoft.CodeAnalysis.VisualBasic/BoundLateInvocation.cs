using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLateInvocation : BoundExpression
	{
		private readonly BoundExpression _Member;

		private readonly ImmutableArray<BoundExpression> _ArgumentsOpt;

		private readonly ImmutableArray<string> _ArgumentNamesOpt;

		private readonly LateBoundAccessKind _AccessKind;

		private readonly BoundMethodOrPropertyGroup _MethodOrPropertyGroupOpt;

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(ArgumentsOpt.Insert(0, Member));

		public BoundExpression Member => _Member;

		public ImmutableArray<BoundExpression> ArgumentsOpt => _ArgumentsOpt;

		public ImmutableArray<string> ArgumentNamesOpt => _ArgumentNamesOpt;

		public LateBoundAccessKind AccessKind => _AccessKind;

		public BoundMethodOrPropertyGroup MethodOrPropertyGroupOpt => _MethodOrPropertyGroupOpt;

		public BoundLateInvocation SetAccessKind(LateBoundAccessKind newAccessKind)
		{
			BoundExpression boundExpression = Member;
			if (boundExpression.Kind == BoundKind.LateMemberAccess)
			{
				boundExpression = ((BoundLateMemberAccess)boundExpression).SetAccessKind(newAccessKind);
			}
			return Update(boundExpression, ArgumentsOpt, ArgumentNamesOpt, newAccessKind, MethodOrPropertyGroupOpt, base.Type);
		}

		public BoundLateInvocation(SyntaxNode syntax, BoundExpression member, ImmutableArray<BoundExpression> argumentsOpt, ImmutableArray<string> argumentNamesOpt, LateBoundAccessKind accessKind, BoundMethodOrPropertyGroup methodOrPropertyGroupOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LateInvocation, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(member) || BoundNodeExtensions.NonNullAndHasErrors(argumentsOpt) || BoundNodeExtensions.NonNullAndHasErrors(methodOrPropertyGroupOpt))
		{
			_Member = member;
			_ArgumentsOpt = argumentsOpt;
			_ArgumentNamesOpt = argumentNamesOpt;
			_AccessKind = accessKind;
			_MethodOrPropertyGroupOpt = methodOrPropertyGroupOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLateInvocation(this);
		}

		public BoundLateInvocation Update(BoundExpression member, ImmutableArray<BoundExpression> argumentsOpt, ImmutableArray<string> argumentNamesOpt, LateBoundAccessKind accessKind, BoundMethodOrPropertyGroup methodOrPropertyGroupOpt, TypeSymbol type)
		{
			if (member != Member || argumentsOpt != ArgumentsOpt || argumentNamesOpt != ArgumentNamesOpt || accessKind != AccessKind || methodOrPropertyGroupOpt != MethodOrPropertyGroupOpt || (object)type != base.Type)
			{
				BoundLateInvocation boundLateInvocation = new BoundLateInvocation(base.Syntax, member, argumentsOpt, argumentNamesOpt, accessKind, methodOrPropertyGroupOpt, type, base.HasErrors);
				boundLateInvocation.CopyAttributes(this);
				return boundLateInvocation;
			}
			return this;
		}
	}
}
