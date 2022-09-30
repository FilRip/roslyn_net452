using System;

namespace Microsoft.CodeAnalysis
{
    public sealed class ErrorLogOptions
    {
        public string Path { get; }

        public SarifVersion SarifVersion { get; }

        public ErrorLogOptions(string path, SarifVersion sarifVersion)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            Path = path;
            SarifVersion = sarifVersion;
        }
    }
}
