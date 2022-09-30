using System.Runtime.CompilerServices;

namespace Roslyn.Utilities
{
    internal static class YieldAwaitableExtensions
    {
        public static ConfiguredYieldAwaitable ConfigureAwait(this YieldAwaitable awaitable, bool continueOnCapturedContext)
        {
            return new ConfiguredYieldAwaitable(awaitable, continueOnCapturedContext);
        }
    }
}
