using System;


#nullable enable
namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal readonly struct RunRequest
    {
        public Guid RequestId { get; }

        public string Language { get; }

        public string? WorkingDirectory { get; }

        public string? TempDirectory { get; }

        public string? LibDirectory { get; }

        public string[] Arguments { get; }

        public RunRequest(
          Guid requestId,
          string language,
          string? workingDirectory,
          string? tempDirectory,
          string? libDirectory,
          string[] arguments)
        {
            this.RequestId = requestId;
            this.Language = language;
            this.WorkingDirectory = workingDirectory;
            this.TempDirectory = tempDirectory;
            this.LibDirectory = libDirectory;
            this.Arguments = arguments;
        }
    }
}
