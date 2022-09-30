#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct BuildPaths
    {
        public string ClientDirectory { get; }

        public string WorkingDirectory { get; }

        public string? SdkDirectory { get; }

        public string? TempDirectory { get; }

        public BuildPaths(string clientDir, string workingDir, string? sdkDir, string? tempDir)
        {
            ClientDirectory = clientDir;
            WorkingDirectory = workingDir;
            SdkDirectory = sdkDir;
            TempDirectory = tempDir;
        }
    }
}
