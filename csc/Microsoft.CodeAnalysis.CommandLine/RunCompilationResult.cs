namespace Microsoft.CodeAnalysis.CommandLine
{
	internal readonly struct RunCompilationResult
	{
		internal static readonly RunCompilationResult Succeeded = new RunCompilationResult(0);

		internal static readonly RunCompilationResult Failed = new RunCompilationResult(1);

		internal int ExitCode { get; }

		internal bool RanOnServer { get; }

		internal RunCompilationResult(int exitCode, bool ranOnServer = false)
		{
			ExitCode = exitCode;
			RanOnServer = ranOnServer;
		}
	}
}
