using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundBreakStatement : BoundStatement
    {
        public GeneratedLabelSymbol Label { get; }

        public BoundBreakStatement(SyntaxNode syntax, GeneratedLabelSymbol label, bool hasErrors)
            : base(BoundKind.BreakStatement, syntax, hasErrors)
        {
            Label = label;
        }

        public BoundBreakStatement(SyntaxNode syntax, GeneratedLabelSymbol label)
            : base(BoundKind.BreakStatement, syntax)
        {
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitBreakStatement(this);
        }

        public BoundBreakStatement Update(GeneratedLabelSymbol label)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundBreakStatement boundBreakStatement = new BoundBreakStatement(Syntax, label, base.HasErrors);
                boundBreakStatement.CopyAttributes(this);
                return boundBreakStatement;
            }
            return this;
        }
    }
}
