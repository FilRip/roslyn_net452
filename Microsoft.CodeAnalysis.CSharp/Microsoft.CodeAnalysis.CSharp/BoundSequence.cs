using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSequence : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(SideEffects.Add(Value));

        public new TypeSymbol Type => base.Type;

        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundExpression> SideEffects { get; }

        public BoundExpression Value { get; }

        public BoundSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Sequence, syntax, type, hasErrors || sideEffects.HasErrors() || value.HasErrors())
        {
            Locals = locals;
            SideEffects = sideEffects;
            Value = value;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSequence(this);
        }

        public BoundSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression value, TypeSymbol type)
        {
            if (locals != Locals || sideEffects != SideEffects || value != Value || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundSequence boundSequence = new BoundSequence(Syntax, locals, sideEffects, value, type, base.HasErrors);
                boundSequence.CopyAttributes(this);
                return boundSequence;
            }
            return this;
        }
    }
}
