using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundHoistedFieldAccess : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public FieldSymbol FieldSymbol { get; }

        public BoundHoistedFieldAccess(SyntaxNode syntax, FieldSymbol fieldSymbol, TypeSymbol type, bool hasErrors)
            : base(BoundKind.HoistedFieldAccess, syntax, type, hasErrors)
        {
            FieldSymbol = fieldSymbol;
        }

        public BoundHoistedFieldAccess(SyntaxNode syntax, FieldSymbol fieldSymbol, TypeSymbol type)
            : base(BoundKind.HoistedFieldAccess, syntax, type)
        {
            FieldSymbol = fieldSymbol;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitHoistedFieldAccess(this);
        }

        public BoundHoistedFieldAccess Update(FieldSymbol fieldSymbol, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(fieldSymbol, FieldSymbol) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundHoistedFieldAccess boundHoistedFieldAccess = new BoundHoistedFieldAccess(Syntax, fieldSymbol, type, base.HasErrors);
                boundHoistedFieldAccess.CopyAttributes(this);
                return boundHoistedFieldAccess;
            }
            return this;
        }
    }
}
