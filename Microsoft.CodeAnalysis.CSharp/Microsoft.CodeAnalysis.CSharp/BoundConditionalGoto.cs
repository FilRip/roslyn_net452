using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundConditionalGoto : BoundStatement
    {
        public BoundExpression Condition { get; }

        public bool JumpIfTrue { get; }

        public LabelSymbol Label { get; }

        public BoundConditionalGoto(SyntaxNode syntax, BoundExpression condition, bool jumpIfTrue, LabelSymbol label, bool hasErrors = false)
            : base(BoundKind.ConditionalGoto, syntax, hasErrors || condition.HasErrors())
        {
            Condition = condition;
            JumpIfTrue = jumpIfTrue;
            Label = label;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitConditionalGoto(this);
        }

        public BoundConditionalGoto Update(BoundExpression condition, bool jumpIfTrue, LabelSymbol label)
        {
            if (condition != Condition || jumpIfTrue != JumpIfTrue || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label))
            {
                BoundConditionalGoto boundConditionalGoto = new BoundConditionalGoto(Syntax, condition, jumpIfTrue, label, base.HasErrors);
                boundConditionalGoto.CopyAttributes(this);
                return boundConditionalGoto;
            }
            return this;
        }
    }
}
