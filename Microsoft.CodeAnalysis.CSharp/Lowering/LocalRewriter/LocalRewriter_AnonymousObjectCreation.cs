// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed partial class LocalRewriter
    {
        public override BoundNode VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            // Rewrite the arguments.
            var rewrittenArguments = VisitList(node.Arguments);

            return new BoundObjectCreationExpression(
                syntax: node.Syntax,
                constructor: node.Constructor,
                arguments: rewrittenArguments,
                argumentNamesOpt: default,
                argumentRefKindsOpt: default,
                expanded: false,
                argsToParamsOpt: default,
                defaultArguments: default,
                constantValueOpt: null,
                initializerExpressionOpt: null,
                type: node.Type);
        }
    }
}
