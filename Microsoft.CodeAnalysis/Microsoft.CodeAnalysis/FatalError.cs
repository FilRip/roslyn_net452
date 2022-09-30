using System;
using System.Diagnostics;
using System.Threading;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class FatalError
    {
        private static Action<Exception>? s_fatalHandler;

        private static Action<Exception>? s_nonFatalHandler;

        private static Exception? s_reportedException;

        private static string? s_reportedExceptionMessage;

        private static readonly object s_reportedMarker = new object();

        public static Action<Exception>? Handler
        {
            get
            {
                return s_fatalHandler;
            }
            [param: System.Diagnostics.CodeAnalysis.DisallowNull]
            set
            {
                if (s_fatalHandler != value)
                {
                    s_fatalHandler = value;
                }
            }
        }

        public static Action<Exception>? NonFatalHandler
        {
            get
            {
                return s_nonFatalHandler;
            }
            [param: System.Diagnostics.CodeAnalysis.DisallowNull]
            set
            {
                if (s_nonFatalHandler != value)
                {
                    s_nonFatalHandler = value;
                }
            }
        }

        public static void OverwriteHandler(Action<Exception>? value)
        {
            s_fatalHandler = value;
        }

        private static bool IsCurrentOperationBeingCancelled(Exception exception, CancellationToken cancellationToken)
        {
            if (exception is OperationCanceledException)
            {
                return cancellationToken.IsCancellationRequested;
            }
            return false;
        }

        [DebuggerHidden]
        public static bool ReportAndPropagateUnlessCanceled(Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                return false;
            }
            return ReportAndPropagate(exception);
        }

        [DebuggerHidden]
        public static bool ReportAndPropagateUnlessCanceled(Exception exception, CancellationToken contextCancellationToken)
        {
            if (IsCurrentOperationBeingCancelled(exception, contextCancellationToken))
            {
                return false;
            }
            return ReportAndPropagate(exception);
        }

        [DebuggerHidden]
        public static bool ReportAndCatchUnlessCanceled(Exception exception)
        {
            if (exception is OperationCanceledException)
            {
                return false;
            }
            return ReportAndCatch(exception);
        }

        [DebuggerHidden]
        public static bool ReportAndCatchUnlessCanceled(Exception exception, CancellationToken contextCancellationToken)
        {
            if (IsCurrentOperationBeingCancelled(exception, contextCancellationToken))
            {
                return false;
            }
            return ReportAndCatch(exception);
        }

        [DebuggerHidden]
        public static bool ReportAndPropagate(Exception exception)
        {
            Report(exception, s_fatalHandler);
            return false;
        }

        [DebuggerHidden]
        public static bool ReportAndCatch(Exception exception)
        {
            Report(exception, s_nonFatalHandler);
            return true;
        }

        private static void Report(Exception exception, Action<Exception>? handler)
        {
            s_reportedException = exception;
            s_reportedExceptionMessage = exception.ToString();
            if (handler != null && exception.Data[s_reportedMarker] == null && (!(exception is AggregateException ex) || ex.InnerExceptions.Count != 1 || ex.InnerExceptions[0].Data[s_reportedMarker] == null))
            {
                if (!exception.Data.IsReadOnly)
                {
                    exception.Data[s_reportedMarker] = s_reportedMarker;
                }
                handler!(exception);
            }
        }
    }
}
