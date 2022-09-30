using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundStringInsert : BoundExpression
    {
        public BoundExpression Value { get; }

        public BoundExpression? Alignment { get; }

        public BoundLiteral? Format { get; }

        public BoundStringInsert(SyntaxNode syntax, BoundExpression value, BoundExpression? alignment, BoundLiteral? format, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.StringInsert, syntax, type, hasErrors || value.HasErrors() || alignment.HasErrors() || format.HasErrors())
        {
            Value = value;
            Alignment = alignment;
            Format = format;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitStringInsert(this);
        }

        public BoundStringInsert Update(BoundExpression value, BoundExpression? alignment, BoundLiteral? format, TypeSymbol? type)
        {
            if (value != Value || alignment != Alignment || format != Format || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundStringInsert boundStringInsert = new BoundStringInsert(Syntax, value, alignment, format, type, base.HasErrors);
                boundStringInsert.CopyAttributes(this);
                return boundStringInsert;
            }
            return this;
        }
    }
}
