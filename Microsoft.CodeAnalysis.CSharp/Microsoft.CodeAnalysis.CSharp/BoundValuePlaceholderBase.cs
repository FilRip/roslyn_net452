using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundValuePlaceholderBase : BoundExpression
    {
        protected BoundValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(kind, syntax, type, hasErrors)
        {
        }

        protected BoundValuePlaceholderBase(BoundKind kind, SyntaxNode syntax, TypeSymbol? type)
            : base(kind, syntax, type)
        {
        }
    }
}
