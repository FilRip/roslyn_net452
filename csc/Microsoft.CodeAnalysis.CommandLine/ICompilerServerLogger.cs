namespace Microsoft.CodeAnalysis.CommandLine
{
	internal interface ICompilerServerLogger
	{
		bool IsLogging { get; }

		void Log(string message);
	}
}
