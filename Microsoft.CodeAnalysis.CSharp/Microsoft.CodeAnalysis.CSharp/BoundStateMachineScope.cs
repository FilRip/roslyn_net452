using System.Collections.Immutable;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundStateMachineScope : BoundStatement
    {
        public ImmutableArray<StateMachineFieldSymbol> Fields { get; }

        public BoundStatement Statement { get; }

        public BoundStateMachineScope(SyntaxNode syntax, ImmutableArray<StateMachineFieldSymbol> fields, BoundStatement statement, bool hasErrors = false)
            : base(BoundKind.StateMachineScope, syntax, hasErrors || statement.HasErrors())
        {
            Fields = fields;
            Statement = statement;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitStateMachineScope(this);
        }

        public BoundStateMachineScope Update(ImmutableArray<StateMachineFieldSymbol> fields, BoundStatement statement)
        {
            if (fields != Fields || statement != Statement)
            {
                BoundStateMachineScope boundStateMachineScope = new BoundStateMachineScope(Syntax, fields, statement, base.HasErrors);
                boundStateMachineScope.CopyAttributes(this);
                return boundStateMachineScope;
            }
            return this;
        }
    }
}
