using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAnonymousTypeFieldInitializer : BoundExpression
	{
		private readonly Binder.AnonymousTypeFieldInitializerBinder _Binder;

		private readonly BoundExpression _Value;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Value);

		public Binder.AnonymousTypeFieldInitializerBinder Binder => _Binder;

		public BoundExpression Value => _Value;

		public BoundAnonymousTypeFieldInitializer(SyntaxNode syntax, Binder.AnonymousTypeFieldInitializerBinder binder, BoundExpression value, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.AnonymousTypeFieldInitializer, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value))
		{
			_Binder = binder;
			_Value = value;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAnonymousTypeFieldInitializer(this);
		}

		public BoundAnonymousTypeFieldInitializer Update(Binder.AnonymousTypeFieldInitializerBinder binder, BoundExpression value, TypeSymbol type)
		{
			if (binder != Binder || value != Value || (object)type != base.Type)
			{
				BoundAnonymousTypeFieldInitializer boundAnonymousTypeFieldInitializer = new BoundAnonymousTypeFieldInitializer(base.Syntax, binder, value, type, base.HasErrors);
				boundAnonymousTypeFieldInitializer.CopyAttributes(this);
				return boundAnonymousTypeFieldInitializer;
			}
			return this;
		}
	}
}
