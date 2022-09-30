using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTypeOfOperator : BoundTypeOf
    {
        public BoundTypeExpression SourceType { get; }

        public BoundTypeOfOperator(SyntaxNode syntax, BoundTypeExpression sourceType, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.TypeOfOperator, syntax, getTypeFromHandle, type, hasErrors || sourceType.HasErrors())
        {
            SourceType = sourceType;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTypeOfOperator(this);
        }

        public BoundTypeOfOperator Update(BoundTypeExpression sourceType, MethodSymbol? getTypeFromHandle, TypeSymbol type)
        {
            if (sourceType != SourceType || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getTypeFromHandle, base.GetTypeFromHandle) || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTypeOfOperator boundTypeOfOperator = new BoundTypeOfOperator(Syntax, sourceType, getTypeFromHandle, type, base.HasErrors);
                boundTypeOfOperator.CopyAttributes(this);
                return boundTypeOfOperator;
            }
            return this;
        }
    }
}
