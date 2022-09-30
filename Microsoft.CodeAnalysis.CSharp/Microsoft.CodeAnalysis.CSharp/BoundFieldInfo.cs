using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundFieldInfo : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public FieldSymbol Field { get; }

        public MethodSymbol? GetFieldFromHandle { get; }

        public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type, bool hasErrors)
            : base(BoundKind.FieldInfo, syntax, type, hasErrors)
        {
            Field = field;
            GetFieldFromHandle = getFieldFromHandle;
        }

        public BoundFieldInfo(SyntaxNode syntax, FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type)
            : base(BoundKind.FieldInfo, syntax, type)
        {
            Field = field;
            GetFieldFromHandle = getFieldFromHandle;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitFieldInfo(this);
        }

        public BoundFieldInfo Update(FieldSymbol field, MethodSymbol? getFieldFromHandle, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(field, Field) || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(getFieldFromHandle, GetFieldFromHandle) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundFieldInfo boundFieldInfo = new BoundFieldInfo(Syntax, field, getFieldFromHandle, type, base.HasErrors);
                boundFieldInfo.CopyAttributes(this);
                return boundFieldInfo;
            }
            return this;
        }
    }
}
