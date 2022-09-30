using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAwaitableValuePlaceholder : BoundValuePlaceholderBase
    {
        public new TypeSymbol? Type => base.Type;

        public uint ValEscape { get; }

        public BoundAwaitableValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.AwaitableValuePlaceholder, syntax, type, hasErrors)
        {
            ValEscape = valEscape;
        }

        public BoundAwaitableValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol? type)
            : base(BoundKind.AwaitableValuePlaceholder, syntax, type)
        {
            ValEscape = valEscape;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAwaitableValuePlaceholder(this);
        }

        public BoundAwaitableValuePlaceholder Update(uint valEscape, TypeSymbol? type)
        {
            if (valEscape != ValEscape || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAwaitableValuePlaceholder boundAwaitableValuePlaceholder = new BoundAwaitableValuePlaceholder(Syntax, valEscape, type, base.HasErrors);
                boundAwaitableValuePlaceholder.CopyAttributes(this);
                return boundAwaitableValuePlaceholder;
            }
            return this;
        }
    }
}
