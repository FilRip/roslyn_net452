using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundContinueStatement : BoundStatement
    {
        public GeneratedLabelSymbol Label { get; }

        public BoundContinueStatement(SyntaxNode syntax, GeneratedLabelSymbol label, bool hasErrors)
            : base(BoundKind.ContinueStatement, syntax, hasErrors)
        {
            Label = label;
        }

        public BoundContinueStatement(SyntaxNode syntax, GeneratedLabelSymbol label)
            : base(BoundKind.ContinueStatement, syntax)
        {
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitContinueStatement(this);
        }

        public BoundContinueStatement Update(GeneratedLabelSymbol label)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundContinueStatement boundContinueStatement = new BoundContinueStatement(Syntax, label, base.HasErrors);
                boundContinueStatement.CopyAttributes(this);
                return boundContinueStatement;
            }
            return this;
        }
    }
}
