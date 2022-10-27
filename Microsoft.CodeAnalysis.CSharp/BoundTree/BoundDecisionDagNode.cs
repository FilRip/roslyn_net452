// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundDecisionDagNode
    {
        public override bool Equals(object? other)
        {
            if (this == other)
                return true;

            return (this, other)
switch
            {
                (BoundEvaluationDecisionDagNode n1, BoundEvaluationDecisionDagNode n2) => n1.Evaluation.Equals(n2.Evaluation) && n1.Next == n2.Next,
                (BoundTestDecisionDagNode n1, BoundTestDecisionDagNode n2) => n1.Test.Equals(n2.Test) && n1.WhenTrue == n2.WhenTrue && n1.WhenFalse == n2.WhenFalse,
                (BoundWhenDecisionDagNode n1, BoundWhenDecisionDagNode n2) => n1.WhenExpression == n2.WhenExpression && n1.WhenTrue == n2.WhenTrue && n1.WhenFalse == n2.WhenFalse,
                (BoundLeafDecisionDagNode n1, BoundLeafDecisionDagNode n2) => n1.Label == n2.Label,
                _ => false,
            };
        }

        public override int GetHashCode()
        {
            return this switch
            {
                BoundEvaluationDecisionDagNode n => Hash.Combine(n.Evaluation.GetHashCode(), RuntimeHelpers.GetHashCode(n.Next)),
                BoundTestDecisionDagNode n => Hash.Combine(n.Test.GetHashCode(), Hash.Combine(RuntimeHelpers.GetHashCode(n.WhenFalse), RuntimeHelpers.GetHashCode(n.WhenTrue))),
                BoundWhenDecisionDagNode n => Hash.Combine(RuntimeHelpers.GetHashCode(n.WhenExpression!), Hash.Combine(RuntimeHelpers.GetHashCode(n.WhenFalse!), RuntimeHelpers.GetHashCode(n.WhenTrue))),// See https://github.com/dotnet/runtime/pull/31819 for why ! is temporarily required below.
                BoundLeafDecisionDagNode n => RuntimeHelpers.GetHashCode(n.Label),
                _ => throw ExceptionUtilities.UnexpectedValue(this),
            };
        }
    }
}
