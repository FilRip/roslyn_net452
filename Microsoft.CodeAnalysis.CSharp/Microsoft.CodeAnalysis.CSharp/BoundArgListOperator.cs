using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundArgListOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(Arguments);

        public override object Display => "__arglist";

        public new TypeSymbol? Type => base.Type;

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public BoundArgListOperator(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.ArgListOperator, syntax, type, hasErrors || arguments.HasErrors())
        {
            Arguments = arguments;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitArgListOperator(this);
        }

        public BoundArgListOperator Update(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, TypeSymbol? type)
        {
            if (arguments != Arguments || argumentRefKindsOpt != ArgumentRefKindsOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundArgListOperator boundArgListOperator = new BoundArgListOperator(Syntax, arguments, argumentRefKindsOpt, type, base.HasErrors);
                boundArgListOperator.CopyAttributes(this);
                return boundArgListOperator;
            }
            return this;
        }
    }
}
