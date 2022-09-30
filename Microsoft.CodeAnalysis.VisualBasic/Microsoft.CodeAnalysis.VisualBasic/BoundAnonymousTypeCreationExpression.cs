using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAnonymousTypeCreationExpression : BoundExpression
	{
		private readonly Binder.AnonymousTypeCreationBinder _BinderOpt;

		private readonly ImmutableArray<BoundAnonymousTypePropertyAccess> _Declarations;

		private readonly ImmutableArray<BoundExpression> _Arguments;

		public override Symbol ExpressionSymbol
		{
			get
			{
				TypeSymbol type = base.Type;
				if (TypeSymbolExtensions.IsErrorType(type))
				{
					return null;
				}
				return ((NamedTypeSymbol)type).InstanceConstructors[0];
			}
		}

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(Arguments);

		public Binder.AnonymousTypeCreationBinder BinderOpt => _BinderOpt;

		public ImmutableArray<BoundAnonymousTypePropertyAccess> Declarations => _Declarations;

		public ImmutableArray<BoundExpression> Arguments => _Arguments;

		public BoundAnonymousTypeCreationExpression(SyntaxNode syntax, Binder.AnonymousTypeCreationBinder binderOpt, ImmutableArray<BoundAnonymousTypePropertyAccess> declarations, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.AnonymousTypeCreationExpression, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(declarations) || BoundNodeExtensions.NonNullAndHasErrors(arguments))
		{
			_BinderOpt = binderOpt;
			_Declarations = declarations;
			_Arguments = arguments;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAnonymousTypeCreationExpression(this);
		}

		public BoundAnonymousTypeCreationExpression Update(Binder.AnonymousTypeCreationBinder binderOpt, ImmutableArray<BoundAnonymousTypePropertyAccess> declarations, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
		{
			if (binderOpt != BinderOpt || declarations != Declarations || arguments != Arguments || (object)type != base.Type)
			{
				BoundAnonymousTypeCreationExpression boundAnonymousTypeCreationExpression = new BoundAnonymousTypeCreationExpression(base.Syntax, binderOpt, declarations, arguments, type, base.HasErrors);
				boundAnonymousTypeCreationExpression.CopyAttributes(this);
				return boundAnonymousTypeCreationExpression;
			}
			return this;
		}
	}
}
