using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class FailFast
    {
        [DebuggerHidden]
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        public static void OnFatalException(Exception exception)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            if (exception is AggregateException ex && ex.InnerExceptions.Count == 1)
            {
                exception = ex.InnerExceptions[0];
            }
            Environment.FailFast(exception.ToString(), exception);
            throw ExceptionUtilities.Unreachable;
        }

        [DebuggerHidden]
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        internal static void Fail(string message)
        {
            Environment.FailFast(message);
            throw ExceptionUtilities.Unreachable;
        }

        [Conditional("DEBUG")]
        internal static void DumpStackTrace(Exception? exception = null, string? message = null)
        {
            Console.WriteLine("Dumping info before call to failfast");
            if (message != null)
            {
                Console.WriteLine(message);
            }
            if (exception != null)
            {
                Console.WriteLine("Exception info");
                for (Exception ex = exception; ex != null; ex = ex.InnerException)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            Console.WriteLine("Stack trace of handler");
            Console.WriteLine(new StackTrace().ToString());
            Console.Out.Flush();
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        internal static void Assert([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] bool condition, string? message = null)
        {
            if (!condition)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                Environment.FailFast("ASSERT FAILED" + Environment.NewLine + message);
            }
        }
    }
}
