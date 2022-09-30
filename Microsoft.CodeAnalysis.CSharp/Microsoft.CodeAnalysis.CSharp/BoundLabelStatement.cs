using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLabelStatement : BoundStatement
    {
        public LabelSymbol Label { get; }

        public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors)
            : base(BoundKind.LabelStatement, syntax, hasErrors)
        {
            Label = label;
        }

        public BoundLabelStatement(SyntaxNode syntax, LabelSymbol label)
            : base(BoundKind.LabelStatement, syntax)
        {
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLabelStatement(this);
        }

        public BoundLabelStatement Update(LabelSymbol label)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundLabelStatement boundLabelStatement = new BoundLabelStatement(Syntax, label, base.HasErrors);
                boundLabelStatement.CopyAttributes(this);
                return boundLabelStatement;
            }
            return this;
        }
    }
}
