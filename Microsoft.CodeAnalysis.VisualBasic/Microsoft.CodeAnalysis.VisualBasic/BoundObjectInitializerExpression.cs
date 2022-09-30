using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundObjectInitializerExpression : BoundObjectInitializerExpressionBase
	{
		private readonly bool _CreateTemporaryLocalForInitialization;

		private readonly Binder _Binder;

		public bool CreateTemporaryLocalForInitialization => _CreateTemporaryLocalForInitialization;

		public Binder Binder => _Binder;

		public BoundObjectInitializerExpression(SyntaxNode syntax, bool createTemporaryLocalForInitialization, Binder binder, BoundWithLValueExpressionPlaceholder placeholderOpt, ImmutableArray<BoundExpression> initializers, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ObjectInitializerExpression, syntax, placeholderOpt, initializers, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(placeholderOpt) || BoundNodeExtensions.NonNullAndHasErrors(initializers))
		{
			_CreateTemporaryLocalForInitialization = createTemporaryLocalForInitialization;
			_Binder = binder;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitObjectInitializerExpression(this);
		}

		public BoundObjectInitializerExpression Update(bool createTemporaryLocalForInitialization, Binder binder, BoundWithLValueExpressionPlaceholder placeholderOpt, ImmutableArray<BoundExpression> initializers, TypeSymbol type)
		{
			if (createTemporaryLocalForInitialization != CreateTemporaryLocalForInitialization || binder != Binder || placeholderOpt != base.PlaceholderOpt || initializers != base.Initializers || (object)type != base.Type)
			{
				BoundObjectInitializerExpression boundObjectInitializerExpression = new BoundObjectInitializerExpression(base.Syntax, createTemporaryLocalForInitialization, binder, placeholderOpt, initializers, type, base.HasErrors);
				boundObjectInitializerExpression.CopyAttributes(this);
				return boundObjectInitializerExpression;
			}
			return this;
		}
	}
}
