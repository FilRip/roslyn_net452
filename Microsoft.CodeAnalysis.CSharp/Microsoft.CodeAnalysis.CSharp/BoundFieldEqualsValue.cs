using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFieldEqualsValue : BoundEqualsValue
    {
        public FieldSymbol Field { get; }

        public BoundFieldEqualsValue(SyntaxNode syntax, FieldSymbol field, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.FieldEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            Field = field;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFieldEqualsValue(this);
        }

        public BoundFieldEqualsValue Update(FieldSymbol field, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, Field) || locals != base.Locals || value != base.Value)
            {
                BoundFieldEqualsValue boundFieldEqualsValue = new BoundFieldEqualsValue(Syntax, field, locals, value, base.HasErrors);
                boundFieldEqualsValue.CopyAttributes(this);
                return boundFieldEqualsValue;
            }
            return this;
        }
    }
}
