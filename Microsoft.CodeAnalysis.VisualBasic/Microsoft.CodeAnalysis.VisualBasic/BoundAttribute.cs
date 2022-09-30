using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAttribute : BoundExpression
	{
		private readonly MethodSymbol _Constructor;

		private readonly ImmutableArray<BoundExpression> _ConstructorArguments;

		private readonly ImmutableArray<BoundExpression> _NamedArguments;

		private readonly LookupResultKind _ResultKind;

		public override Symbol ExpressionSymbol => Constructor;

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(ConstructorArguments.AddRange(NamedArguments));

		public MethodSymbol Constructor => _Constructor;

		public ImmutableArray<BoundExpression> ConstructorArguments => _ConstructorArguments;

		public ImmutableArray<BoundExpression> NamedArguments => _NamedArguments;

		public override LookupResultKind ResultKind => _ResultKind;

		public BoundAttribute(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<BoundExpression> namedArguments, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.Attribute, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(constructorArguments) || BoundNodeExtensions.NonNullAndHasErrors(namedArguments))
		{
			_Constructor = constructor;
			_ConstructorArguments = constructorArguments;
			_NamedArguments = namedArguments;
			_ResultKind = resultKind;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAttribute(this);
		}

		public BoundAttribute Update(MethodSymbol constructor, ImmutableArray<BoundExpression> constructorArguments, ImmutableArray<BoundExpression> namedArguments, LookupResultKind resultKind, TypeSymbol type)
		{
			if ((object)constructor != Constructor || constructorArguments != ConstructorArguments || namedArguments != NamedArguments || resultKind != ResultKind || (object)type != base.Type)
			{
				BoundAttribute boundAttribute = new BoundAttribute(base.Syntax, constructor, constructorArguments, namedArguments, resultKind, type, base.HasErrors);
				boundAttribute.CopyAttributes(this);
				return boundAttribute;
			}
			return this;
		}
	}
}
