using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace Roslyn.Utilities
{
    public static class RoslynParallel
    {
        internal static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions();

        public static ParallelLoopResult For(int fromInclusive, int toExclusive, Action<int> body, CancellationToken cancellationToken)
        {
            Action<int> body2 = body;
            ParallelOptions parallelOptions = (cancellationToken.CanBeCanceled ? new ParallelOptions
            {
                CancellationToken = cancellationToken
            } : DefaultParallelOptions);
            return Parallel.For(fromInclusive, toExclusive, parallelOptions, errorHandlingBody);
            void errorHandlingBody(int i)
            {
                try
                {
                    body2(i);
                }
                catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
                {
                    throw ExceptionUtilities.Unreachable;
                }
                catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested && ex.CancellationToken != cancellationToken)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    throw ExceptionUtilities.Unreachable;
                }
            }
        }
    }
}
