using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPseudoVariable : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public LocalSymbol LocalSymbol { get; }

        public PseudoVariableExpressions EmitExpressions { get; }

        public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type, bool hasErrors)
            : base(BoundKind.PseudoVariable, syntax, type, hasErrors)
        {
            LocalSymbol = localSymbol;
            EmitExpressions = emitExpressions;
        }

        public BoundPseudoVariable(SyntaxNode syntax, LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type)
            : base(BoundKind.PseudoVariable, syntax, type)
        {
            LocalSymbol = localSymbol;
            EmitExpressions = emitExpressions;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPseudoVariable(this);
        }

        public BoundPseudoVariable Update(LocalSymbol localSymbol, PseudoVariableExpressions emitExpressions, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, LocalSymbol) || emitExpressions != EmitExpressions || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPseudoVariable boundPseudoVariable = new BoundPseudoVariable(Syntax, localSymbol, emitExpressions, type, base.HasErrors);
                boundPseudoVariable.CopyAttributes(this);
                return boundPseudoVariable;
            }
            return this;
        }
    }
}
