using System.Runtime.CompilerServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundDecisionDagNode : BoundNode
    {
        public override bool Equals(object? other)
        {
            if (this == other)
            {
                return true;
            }
            if (this is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode)
            {
                if (other is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode2)
                {
                    if (boundEvaluationDecisionDagNode.Evaluation.Equals(boundEvaluationDecisionDagNode2.Evaluation))
                    {
                        return boundEvaluationDecisionDagNode.Next == boundEvaluationDecisionDagNode2.Next;
                    }
                    return false;
                }
            }
            else if (this is BoundTestDecisionDagNode boundTestDecisionDagNode)
            {
                if (other is BoundTestDecisionDagNode boundTestDecisionDagNode2)
                {
                    if (boundTestDecisionDagNode.Test.Equals(boundTestDecisionDagNode2.Test) && boundTestDecisionDagNode.WhenTrue == boundTestDecisionDagNode2.WhenTrue)
                    {
                        return boundTestDecisionDagNode.WhenFalse == boundTestDecisionDagNode2.WhenFalse;
                    }
                    return false;
                }
            }
            else if (this is BoundWhenDecisionDagNode boundWhenDecisionDagNode)
            {
                if (other is BoundWhenDecisionDagNode boundWhenDecisionDagNode2)
                {
                    if (boundWhenDecisionDagNode.WhenExpression == boundWhenDecisionDagNode2.WhenExpression && boundWhenDecisionDagNode.WhenTrue == boundWhenDecisionDagNode2.WhenTrue)
                    {
                        return boundWhenDecisionDagNode.WhenFalse == boundWhenDecisionDagNode2.WhenFalse;
                    }
                    return false;
                }
            }
            else if (this is BoundLeafDecisionDagNode boundLeafDecisionDagNode && other is BoundLeafDecisionDagNode boundLeafDecisionDagNode2)
            {
                return boundLeafDecisionDagNode.Label == boundLeafDecisionDagNode2.Label;
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (!(this is BoundEvaluationDecisionDagNode boundEvaluationDecisionDagNode))
            {
                if (!(this is BoundTestDecisionDagNode boundTestDecisionDagNode))
                {
                    if (!(this is BoundWhenDecisionDagNode boundWhenDecisionDagNode))
                    {
                        if (this is BoundLeafDecisionDagNode boundLeafDecisionDagNode)
                        {
                            return RuntimeHelpers.GetHashCode(boundLeafDecisionDagNode.Label);
                        }
                        throw ExceptionUtilities.UnexpectedValue(this);
                    }
                    return Hash.Combine(RuntimeHelpers.GetHashCode(boundWhenDecisionDagNode.WhenExpression), Hash.Combine(RuntimeHelpers.GetHashCode(boundWhenDecisionDagNode.WhenFalse), RuntimeHelpers.GetHashCode(boundWhenDecisionDagNode.WhenTrue)));
                }
                return Hash.Combine(boundTestDecisionDagNode.Test.GetHashCode(), Hash.Combine(RuntimeHelpers.GetHashCode(boundTestDecisionDagNode.WhenFalse), RuntimeHelpers.GetHashCode(boundTestDecisionDagNode.WhenTrue)));
            }
            return Hash.Combine(boundEvaluationDecisionDagNode.Evaluation.GetHashCode(), RuntimeHelpers.GetHashCode(boundEvaluationDecisionDagNode.Next));
        }

        protected BoundDecisionDagNode(BoundKind kind, SyntaxNode syntax, bool hasErrors)
            : base(kind, syntax, hasErrors)
        {
        }

        protected BoundDecisionDagNode(BoundKind kind, SyntaxNode syntax)
            : base(kind, syntax)
        {
        }
    }
}
