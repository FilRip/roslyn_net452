using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundBadExpression : BoundExpression, IBoundInvalidNode
	{
		private readonly LookupResultKind _ResultKind;

		private readonly ImmutableArray<Symbol> _Symbols;

		private readonly ImmutableArray<BoundExpression> _ChildBoundNodes;

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(ChildBoundNodes);

		private ImmutableArray<BoundNode> IBoundInvalidNode_InvalidNodeChildren => StaticCast<BoundNode>.From(ChildBoundNodes);

		public override LookupResultKind ResultKind => _ResultKind;

		public ImmutableArray<Symbol> Symbols => _Symbols;

		public ImmutableArray<BoundExpression> ChildBoundNodes => _ChildBoundNodes;

		public BoundBadExpression(SyntaxNode syntax, LookupResultKind resultKind, ImmutableArray<Symbol> symbols, ImmutableArray<BoundExpression> childBoundNodes, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.BadExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(childBoundNodes))
		{
			_ResultKind = resultKind;
			_Symbols = symbols;
			_ChildBoundNodes = childBoundNodes;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitBadExpression(this);
		}

		public BoundBadExpression Update(LookupResultKind resultKind, ImmutableArray<Symbol> symbols, ImmutableArray<BoundExpression> childBoundNodes, TypeSymbol type)
		{
			if (resultKind != ResultKind || symbols != Symbols || childBoundNodes != ChildBoundNodes || (object)type != base.Type)
			{
				BoundBadExpression boundBadExpression = new BoundBadExpression(base.Syntax, resultKind, symbols, childBoundNodes, type, base.HasErrors);
				boundBadExpression.CopyAttributes(this);
				return boundBadExpression;
			}
			return this;
		}
	}
}
