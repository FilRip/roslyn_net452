using System;
using System.Threading;

namespace Microsoft.CodeAnalysis.FlowAnalysis
{
    public static class ControlFlowGraphExtensions
    {
        public static ControlFlowGraph GetLocalFunctionControlFlowGraphInScope(this ControlFlowGraph controlFlowGraph, IMethodSymbol localFunction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (controlFlowGraph == null)
            {
                throw new ArgumentNullException("controlFlowGraph");
            }
            if (localFunction == null)
            {
                throw new ArgumentNullException("localFunction");
            }
            ControlFlowGraph controlFlowGraph2 = controlFlowGraph;
            do
            {
                if (controlFlowGraph2.TryGetLocalFunctionControlFlowGraph(localFunction, cancellationToken, out var controlFlowGraph3))
                {
                    return controlFlowGraph3;
                }
            }
            while ((controlFlowGraph2 = controlFlowGraph2.Parent) != null);
            throw new ArgumentOutOfRangeException("localFunction");
        }

        public static ControlFlowGraph GetAnonymousFunctionControlFlowGraphInScope(this ControlFlowGraph controlFlowGraph, IFlowAnonymousFunctionOperation anonymousFunction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (controlFlowGraph == null)
            {
                throw new ArgumentNullException("controlFlowGraph");
            }
            if (anonymousFunction == null)
            {
                throw new ArgumentNullException("anonymousFunction");
            }
            ControlFlowGraph controlFlowGraph2 = controlFlowGraph;
            do
            {
                if (controlFlowGraph2.TryGetAnonymousFunctionControlFlowGraph(anonymousFunction, cancellationToken, out var controlFlowGraph3))
                {
                    return controlFlowGraph3;
                }
            }
            while ((controlFlowGraph2 = controlFlowGraph2.Parent) != null);
            throw new ArgumentOutOfRangeException("anonymousFunction");
        }
    }
}
