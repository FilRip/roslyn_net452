using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLabeledStatement : BoundStatement
    {
        public LabelSymbol Label { get; }

        public BoundStatement Body { get; }

        public BoundLabeledStatement(SyntaxNode syntax, LabelSymbol label, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.LabeledStatement, syntax, hasErrors || body.HasErrors())
        {
            Label = label;
            Body = body;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLabeledStatement(this);
        }

        public BoundLabeledStatement Update(LabelSymbol label, BoundStatement body)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label) || body != Body)
            {
                BoundLabeledStatement boundLabeledStatement = new BoundLabeledStatement(Syntax, label, body, base.HasErrors);
                boundLabeledStatement.CopyAttributes(this);
                return boundLabeledStatement;
            }
            return this;
        }
    }
}
