using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDynamicInvocation : BoundDynamicInvocableBase
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(base.Arguments.Insert(0, base.Expression));

        public new TypeSymbol Type => base.Type;

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public ImmutableArray<MethodSymbol> ApplicableMethods { get; }

        public BoundDynamicInvocation(SyntaxNode syntax, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicInvocation, syntax, expression, arguments, type, hasErrors || expression.HasErrors() || arguments.HasErrors())
        {
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            ApplicableMethods = applicableMethods;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDynamicInvocation(this);
        }

        public BoundDynamicInvocation Update(ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<MethodSymbol> applicableMethods, BoundExpression expression, ImmutableArray<BoundExpression> arguments, TypeSymbol type)
        {
            if (argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || applicableMethods != ApplicableMethods || expression != base.Expression || arguments != base.Arguments || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDynamicInvocation boundDynamicInvocation = new BoundDynamicInvocation(Syntax, argumentNamesOpt, argumentRefKindsOpt, applicableMethods, expression, arguments, type, base.HasErrors);
                boundDynamicInvocation.CopyAttributes(this);
                return boundDynamicInvocation;
            }
            return this;
        }
    }
}
