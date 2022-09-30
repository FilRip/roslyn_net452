using System.Threading.Tasks;

namespace Roslyn.Utilities
{
    internal static class ValueTaskFactory
    {
        public static ValueTask CompletedTask => default(ValueTask);

        public static ValueTask<T> FromResult<T>(T result)
        {
            return new ValueTask<T>(result);
        }
    }
}
