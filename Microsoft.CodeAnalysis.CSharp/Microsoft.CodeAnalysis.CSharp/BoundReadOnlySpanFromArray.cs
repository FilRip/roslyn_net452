using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundReadOnlySpanFromArray : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public MethodSymbol ConversionMethod { get; }

        public BoundReadOnlySpanFromArray(SyntaxNode syntax, BoundExpression operand, MethodSymbol conversionMethod, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ReadOnlySpanFromArray, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
            ConversionMethod = conversionMethod;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitReadOnlySpanFromArray(this);
        }

        public BoundReadOnlySpanFromArray Update(BoundExpression operand, MethodSymbol conversionMethod, TypeSymbol type)
        {
            if (operand != Operand || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(conversionMethod, ConversionMethod) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundReadOnlySpanFromArray boundReadOnlySpanFromArray = new BoundReadOnlySpanFromArray(Syntax, operand, conversionMethod, type, base.HasErrors);
                boundReadOnlySpanFromArray.CopyAttributes(this);
                return boundReadOnlySpanFromArray;
            }
            return this;
        }
    }
}
