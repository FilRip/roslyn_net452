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
            this.ClientDirectory = clientDir;
            this.WorkingDirectory = workingDir;
            this.SdkDirectory = sdkDir;
            this.TempDirectory = tempDir;
        }
    }
}
