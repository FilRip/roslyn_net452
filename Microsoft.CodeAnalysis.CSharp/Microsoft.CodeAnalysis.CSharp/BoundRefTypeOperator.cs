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
    public sealed class BoundRefTypeOperator : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Operand);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public MethodSymbol? GetTypeFromHandle { get; }

        public BoundRefTypeOperator(SyntaxNode syntax, BoundExpression operand, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.RefTypeOperator, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
            GetTypeFromHandle = getTypeFromHandle;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRefTypeOperator(this);
        }

        public BoundRefTypeOperator Update(BoundExpression operand, MethodSymbol? getTypeFromHandle, TypeSymbol type)
        {
            if (operand != Operand || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getTypeFromHandle, GetTypeFromHandle) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundRefTypeOperator boundRefTypeOperator = new BoundRefTypeOperator(Syntax, operand, getTypeFromHandle, type, base.HasErrors);
                boundRefTypeOperator.CopyAttributes(this);
                return boundRefTypeOperator;
            }
            return this;
        }
    }
}
