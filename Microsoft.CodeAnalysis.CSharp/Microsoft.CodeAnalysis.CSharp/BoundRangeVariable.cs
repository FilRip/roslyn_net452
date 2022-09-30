using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundRangeVariable : BoundExpression
    {
        public override Symbol ExpressionSymbol => RangeVariableSymbol;

        public new TypeSymbol Type => base.Type;

        public RangeVariableSymbol RangeVariableSymbol { get; }

        public BoundExpression Value { get; }

        public BoundRangeVariable(SyntaxNode syntax, RangeVariableSymbol rangeVariableSymbol, BoundExpression value, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RangeVariable, syntax, type, hasErrors || value.HasErrors())
        {
            RangeVariableSymbol = rangeVariableSymbol;
            Value = value;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRangeVariable(this);
        }

        public BoundRangeVariable Update(RangeVariableSymbol rangeVariableSymbol, BoundExpression value, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(rangeVariableSymbol, RangeVariableSymbol) || value != Value || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundRangeVariable boundRangeVariable = new BoundRangeVariable(Syntax, rangeVariableSymbol, value, type, base.HasErrors);
                boundRangeVariable.CopyAttributes(this);
                return boundRangeVariable;
            }
            return this;
        }
    }
}
