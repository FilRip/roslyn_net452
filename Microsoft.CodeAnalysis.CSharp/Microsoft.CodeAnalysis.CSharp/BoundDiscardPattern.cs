using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDiscardPattern : BoundPattern
    {
        public BoundDiscardPattern(SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType, bool hasErrors)
            : base(BoundKind.DiscardPattern, syntax, inputType, narrowedType, hasErrors)
        {
        }

        public BoundDiscardPattern(SyntaxNode syntax, TypeSymbol inputType, TypeSymbol narrowedType)
            : base(BoundKind.DiscardPattern, syntax, inputType, narrowedType)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDiscardPattern(this);
        }

        public BoundDiscardPattern Update(TypeSymbol inputType, TypeSymbol narrowedType)
        {
            if (!TypeSymbol.Equals(inputType, base.InputType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(narrowedType, base.NarrowedType, TypeCompareKind.ConsiderEverything))
            {
                BoundDiscardPattern boundDiscardPattern = new BoundDiscardPattern(Syntax, inputType, narrowedType, base.HasErrors);
                boundDiscardPattern.CopyAttributes(this);
                return boundDiscardPattern;
            }
            return this;
        }
    }
}
