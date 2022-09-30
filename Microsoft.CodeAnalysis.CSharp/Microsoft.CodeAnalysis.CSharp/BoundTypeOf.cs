using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundTypeOf : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public MethodSymbol? GetTypeFromHandle { get; }

        protected BoundTypeOf(BoundKind kind, SyntaxNode syntax, MethodSymbol? getTypeFromHandle, TypeSymbol type, bool hasErrors)
            : base(kind, syntax, type, hasErrors)
        {
            GetTypeFromHandle = getTypeFromHandle;
        }

        protected BoundTypeOf(BoundKind kind, SyntaxNode syntax, MethodSymbol? getTypeFromHandle, TypeSymbol type)
            : base(kind, syntax, type)
        {
            GetTypeFromHandle = getTypeFromHandle;
        }
    }
}
