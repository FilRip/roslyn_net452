using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundEvaluationDecisionDagNode : BoundDecisionDagNode
    {
        public BoundDagEvaluation Evaluation { get; }

        public BoundDecisionDagNode Next { get; }

        public BoundEvaluationDecisionDagNode(SyntaxNode syntax, BoundDagEvaluation evaluation, BoundDecisionDagNode next, bool hasErrors = false)
            : base(BoundKind.EvaluationDecisionDagNode, syntax, hasErrors || evaluation.HasErrors() || next.HasErrors())
        {
            Evaluation = evaluation;
            Next = next;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitEvaluationDecisionDagNode(this);
        }

        public BoundEvaluationDecisionDagNode Update(BoundDagEvaluation evaluation, BoundDecisionDagNode next)
        {
            if (evaluation != Evaluation || next != Next)
            {
                BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode = new BoundEvaluationDecisionDagNode(Syntax, evaluation, next, base.HasErrors);
                boundEvaluationDecisionDagNode.CopyAttributes(this);
                return boundEvaluationDecisionDagNode;
            }
            return this;
        }
    }
}
