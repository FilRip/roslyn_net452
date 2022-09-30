using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundParameterEqualsValue : BoundEqualsValue
    {
        public ParameterSymbol Parameter { get; }

        public BoundParameterEqualsValue(SyntaxNode syntax, ParameterSymbol parameter, ImmutableArray<LocalSymbol> locals, BoundExpression value, bool hasErrors = false)
            : base(BoundKind.ParameterEqualsValue, syntax, locals, value, hasErrors || value.HasErrors())
        {
            Parameter = parameter;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitParameterEqualsValue(this);
        }

        public BoundParameterEqualsValue Update(ParameterSymbol parameter, ImmutableArray<LocalSymbol> locals, BoundExpression value)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(parameter, Parameter) || locals != base.Locals || value != base.Value)
            {
                BoundParameterEqualsValue boundParameterEqualsValue = new BoundParameterEqualsValue(Syntax, parameter, locals, value, base.HasErrors);
                boundParameterEqualsValue.CopyAttributes(this);
                return boundParameterEqualsValue;
            }
            return this;
        }
    }
}
