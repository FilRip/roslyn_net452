#nullable enable

namespace Microsoft.CodeAnalysis.CommandLine
{
	internal sealed class BuildPathsAlt
	{
		internal string ClientDirectory { get; }

		internal string WorkingDirectory { get; }

		internal string? SdkDirectory { get; }

		internal string? TempDirectory { get; }

		internal BuildPathsAlt(string clientDir, string workingDir, string? sdkDir, string? tempDir)
		{
			ClientDirectory = clientDir;
			WorkingDirectory = workingDir;
			SdkDirectory = sdkDir;
			TempDirectory = tempDir;
		}
	}
}
