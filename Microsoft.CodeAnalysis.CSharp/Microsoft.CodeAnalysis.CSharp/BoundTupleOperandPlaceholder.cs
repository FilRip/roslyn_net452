using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundTupleOperandPlaceholder : BoundValuePlaceholderBase
    {
        public new TypeSymbol Type => base.Type;

        public BoundTupleOperandPlaceholder(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.TupleOperandPlaceholder, syntax, type, hasErrors)
        {
        }

        public BoundTupleOperandPlaceholder(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.TupleOperandPlaceholder, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitTupleOperandPlaceholder(this);
        }

        public BoundTupleOperandPlaceholder Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundTupleOperandPlaceholder boundTupleOperandPlaceholder = new BoundTupleOperandPlaceholder(Syntax, type, base.HasErrors);
                boundTupleOperandPlaceholder.CopyAttributes(this);
                return boundTupleOperandPlaceholder;
            }
            return this;
        }
    }
}
