using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFromEndIndexExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Operand { get; }

        public MethodSymbol? MethodOpt { get; }

        public BoundFromEndIndexExpression(SyntaxNode syntax, BoundExpression operand, MethodSymbol? methodOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.FromEndIndexExpression, syntax, type, hasErrors || operand.HasErrors())
        {
            Operand = operand;
            MethodOpt = methodOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFromEndIndexExpression(this);
        }

        public BoundFromEndIndexExpression Update(BoundExpression operand, MethodSymbol? methodOpt, TypeSymbol type)
        {
            if (operand != Operand || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundFromEndIndexExpression boundFromEndIndexExpression = new BoundFromEndIndexExpression(Syntax, operand, methodOpt, type, base.HasErrors);
                boundFromEndIndexExpression.CopyAttributes(this);
                return boundFromEndIndexExpression;
            }
            return this;
        }
    }
}
