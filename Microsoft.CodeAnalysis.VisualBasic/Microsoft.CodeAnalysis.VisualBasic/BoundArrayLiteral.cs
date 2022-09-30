using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundArrayLiteral : BoundExpression
	{
		private readonly bool _HasDominantType;

		private readonly int _NumberOfCandidates;

		private readonly ArrayTypeSymbol _InferredType;

		private readonly ImmutableArray<BoundExpression> _Bounds;

		private readonly BoundArrayInitialization _Initializer;

		private readonly Binder _Binder;

		public bool IsEmptyArrayLiteral
		{
			get
			{
				if (InferredType.Rank == 1)
				{
					return Initializer.Initializers.Length == 0;
				}
				return false;
			}
		}

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(Bounds.Add(Initializer));

		public bool HasDominantType => _HasDominantType;

		public int NumberOfCandidates => _NumberOfCandidates;

		public ArrayTypeSymbol InferredType => _InferredType;

		public ImmutableArray<BoundExpression> Bounds => _Bounds;

		public BoundArrayInitialization Initializer => _Initializer;

		public Binder Binder => _Binder;

		public BoundArrayLiteral(SyntaxNode syntax, bool hasDominantType, int numberOfCandidates, ArrayTypeSymbol inferredType, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializer, Binder binder, bool hasErrors = false)
			: base(BoundKind.ArrayLiteral, syntax, null, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(bounds) || BoundNodeExtensions.NonNullAndHasErrors(initializer))
		{
			_HasDominantType = hasDominantType;
			_NumberOfCandidates = numberOfCandidates;
			_InferredType = inferredType;
			_Bounds = bounds;
			_Initializer = initializer;
			_Binder = binder;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitArrayLiteral(this);
		}

		public BoundArrayLiteral Update(bool hasDominantType, int numberOfCandidates, ArrayTypeSymbol inferredType, ImmutableArray<BoundExpression> bounds, BoundArrayInitialization initializer, Binder binder)
		{
			if (hasDominantType != HasDominantType || numberOfCandidates != NumberOfCandidates || (object)inferredType != InferredType || bounds != Bounds || initializer != Initializer || binder != Binder)
			{
				BoundArrayLiteral boundArrayLiteral = new BoundArrayLiteral(base.Syntax, hasDominantType, numberOfCandidates, inferredType, bounds, initializer, binder, base.HasErrors);
				boundArrayLiteral.CopyAttributes(this);
				return boundArrayLiteral;
			}
			return this;
		}
	}
}
