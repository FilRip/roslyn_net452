using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundArrayInitialization : BoundExpression
	{
		private readonly ImmutableArray<BoundExpression> _Initializers;

		public ImmutableArray<BoundExpression> Initializers => _Initializers;

		public BoundArrayInitialization(SyntaxNode syntax, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ArrayInitialization, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(initializers))
		{
			_Initializers = initializers;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitArrayInitialization(this);
		}

		public BoundArrayInitialization Update(ImmutableArray<BoundExpression> initializers, TypeSymbol type)
		{
			if (initializers != Initializers || (object)type != base.Type)
			{
				BoundArrayInitialization boundArrayInitialization = new BoundArrayInitialization(base.Syntax, initializers, type, base.HasErrors);
				boundArrayInitialization.CopyAttributes(this);
				return boundArrayInitialization;
			}
			return this;
		}
	}
}
