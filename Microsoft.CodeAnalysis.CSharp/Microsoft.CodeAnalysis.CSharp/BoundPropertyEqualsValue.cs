using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPropertyEqualsValue : BoundEqualsValue
    {
        public PropertySymbol Property { get; }

        public BoundPropertyEqualsValue(SyntaxNode syntax, PropertySymbol property, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.PropertyEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            Property = property;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPropertyEqualsValue(this);
        }

        public BoundPropertyEqualsValue Update(PropertySymbol property, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(property, Property) || locals != base.Locals || value != base.Value)
            {
                BoundPropertyEqualsValue boundPropertyEqualsValue = new BoundPropertyEqualsValue(Syntax, property, locals, value, base.HasErrors);
                boundPropertyEqualsValue.CopyAttributes(this);
                return boundPropertyEqualsValue;
            }
            return this;
        }
    }
}
