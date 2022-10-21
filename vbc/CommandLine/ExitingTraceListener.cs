using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.CodeAnalysis.CommandLine
{
    internal sealed class ExitingTraceListener : TraceListener
    {
        internal ICompilerServerLogger Logger { get; }

        internal ExitingTraceListener(ICompilerServerLogger logger) => this.Logger = logger;

        public override void Write(string message) => this.Exit(message);

        public override void WriteLine(string message) => this.Exit(message);

        internal static void Install(ICompilerServerLogger logger)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ExitingTraceListener(logger));
        }

        private void Exit(string originalMessage)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("Debug.Assert failed with message: " + originalMessage);
            stringBuilder.AppendLine("Stack Trace");
            stringBuilder.AppendLine(new StackTrace().ToString());
            string message = stringBuilder.ToString();
            this.Logger.Log(message);
            Environment.FailFast(message);
        }
    }
}
