using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class ExitingTraceListener : TraceListener
	{
		internal ICompilerServerLogger Logger { get; }

		internal ExitingTraceListener(ICompilerServerLogger logger)
		{
			Logger = logger;
		}

		public override void Write(string message)
		{
			Exit(message);
		}

		public override void WriteLine(string message)
		{
			Exit(message);
		}

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
			StackTrace stackTrace = new();
			stringBuilder.AppendLine(stackTrace.ToString());
			string message = stringBuilder.ToString();
			Logger.Log(message);
			Environment.FailFast(message);
		}
	}
}
