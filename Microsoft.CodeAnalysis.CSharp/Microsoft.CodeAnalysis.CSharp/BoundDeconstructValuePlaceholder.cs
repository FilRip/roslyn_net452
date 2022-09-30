using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDeconstructValuePlaceholder : BoundValuePlaceholderBase
    {
        public new TypeSymbol Type => base.Type;

        public uint ValEscape { get; }

        public BoundDeconstructValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol type, bool hasErrors)
            : base(BoundKind.DeconstructValuePlaceholder, syntax, type, hasErrors)
        {
            ValEscape = valEscape;
        }

        public BoundDeconstructValuePlaceholder(SyntaxNode syntax, uint valEscape, TypeSymbol type)
            : base(BoundKind.DeconstructValuePlaceholder, syntax, type)
        {
            ValEscape = valEscape;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDeconstructValuePlaceholder(this);
        }

        public BoundDeconstructValuePlaceholder Update(uint valEscape, TypeSymbol type)
        {
            if (valEscape != ValEscape || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDeconstructValuePlaceholder boundDeconstructValuePlaceholder = new BoundDeconstructValuePlaceholder(Syntax, valEscape, type, base.HasErrors);
                boundDeconstructValuePlaceholder.CopyAttributes(this);
                return boundDeconstructValuePlaceholder;
            }
            return this;
        }
    }
}
