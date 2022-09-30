using System;
using System.Diagnostics;

#nullable enable

namespace Roslyn.Utilities
{
    internal static class RoslynDebug
    {
        [Conditional("DEBUG")]
        public static void Assert([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool b)
        {
        }

        [Conditional("DEBUG")]
        public static void Assert([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool b, string message)
        {
        }

        [Conditional("DEBUG")]
        public static void AssertNotNull<T>([System.Diagnostics.CodeAnalysis.NotNull] T value)
        {
        }

        [Conditional("DEBUG")]
        internal static void AssertOrFailFast([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool condition, string? message = null)
        {
            if (!condition && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HELIX_DUMP_FOLDER")))
            {
                if (message == null)
                {
                    message = "AssertOrFailFast failed";
                }
                StackTrace value = new StackTrace();
                Console.WriteLine(message);
                Console.WriteLine(value);
                Environment.FailFast(message);
            }
        }
    }
}
