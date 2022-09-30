using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLabel : BoundExpression
    {
        public override Symbol ExpressionSymbol => Label;

        public LabelSymbol Label { get; }

        public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.Label, syntax, type, hasErrors)
        {
            Label = label;
        }

        public BoundLabel(SyntaxNode syntax, LabelSymbol label, TypeSymbol? type)
            : base(BoundKind.Label, syntax, type)
        {
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLabel(this);
        }

        public BoundLabel Update(LabelSymbol label, TypeSymbol? type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label) || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundLabel boundLabel = new BoundLabel(Syntax, label, type, base.HasErrors);
                boundLabel.CopyAttributes(this);
                return boundLabel;
            }
            return this;
        }
    }
}
