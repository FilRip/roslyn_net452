using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSpillSequence : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public ImmutableArray<LocalSymbol> Locals { get; }

        public ImmutableArray<BoundStatement> SideEffects { get; }

        public BoundExpression Value { get; }

        public BoundSpillSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> sideEffects, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : this(syntax, locals, MakeStatements(sideEffects), value, type, hasErrors)
        {
        }

        private static ImmutableArray<BoundStatement> MakeStatements(ImmutableArray<BoundExpression> expressions)
        {
            return expressions.SelectAsArray((Func<BoundExpression, BoundStatement>)((BoundExpression expression) => new BoundExpressionStatement(expression.Syntax, expression, expression.HasErrors)));
        }

        public BoundSpillSequence(SyntaxNode syntax, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> sideEffects, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.SpillSequence, syntax, type, hasErrors || sideEffects.HasErrors() || value.HasErrors())
        {
            Locals = locals;
            SideEffects = sideEffects;
            Value = value;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSpillSequence(this);
        }

        public BoundSpillSequence Update(ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundStatement> sideEffects, BoundExpression value, TypeSymbol type)
        {
            if (locals != Locals || sideEffects != SideEffects || value != Value || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundSpillSequence boundSpillSequence = new BoundSpillSequence(Syntax, locals, sideEffects, value, type, base.HasErrors);
                boundSpillSequence.CopyAttributes(this);
                return boundSpillSequence;
            }
            return this;
        }
    }
}
