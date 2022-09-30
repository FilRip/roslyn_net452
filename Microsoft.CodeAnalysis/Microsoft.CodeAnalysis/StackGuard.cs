using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis
{
    public static class StackGuard
    {
        public const int MaxUncheckedRecursionDepth = 20;

        [DebuggerStepThrough]
        public static void EnsureSufficientExecutionStack(int recursionDepth)
        {
            if (recursionDepth > 20)
            {
                RuntimeHelpers.EnsureSufficientExecutionStack();
            }
        }
    }
}
