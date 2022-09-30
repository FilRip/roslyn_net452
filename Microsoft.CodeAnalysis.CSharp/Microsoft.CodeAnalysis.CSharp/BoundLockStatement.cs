using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLockStatement : BoundStatement
    {
        public BoundExpression Argument { get; }

        public BoundStatement Body { get; }

        public BoundLockStatement(SyntaxNode syntax, BoundExpression argument, BoundStatement body, bool hasErrors = false)
            : base(BoundKind.LockStatement, syntax, hasErrors || argument.HasErrors() || body.HasErrors())
        {
            Argument = argument;
            Body = body;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLockStatement(this);
        }

        public BoundLockStatement Update(BoundExpression argument, BoundStatement body)
        {
            if (argument != Argument || body != Body)
            {
                BoundLockStatement boundLockStatement = new BoundLockStatement(Syntax, argument, body, base.HasErrors);
                boundLockStatement.CopyAttributes(this);
                return boundLockStatement;
            }
            return this;
        }
    }
}
