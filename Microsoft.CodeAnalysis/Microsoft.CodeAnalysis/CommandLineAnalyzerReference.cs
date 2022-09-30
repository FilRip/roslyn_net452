using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{FilePath,nq}")]
    public struct CommandLineAnalyzerReference : IEquatable<CommandLineAnalyzerReference>
    {
        private readonly string _path;

        public string FilePath => _path;

        public CommandLineAnalyzerReference(string path)
        {
            _path = path;
        }

        public override bool Equals(object? obj)
        {
            if (obj is CommandLineAnalyzerReference)
            {
                return base.Equals((CommandLineAnalyzerReference)obj);
            }
            return false;
        }

        public bool Equals(CommandLineAnalyzerReference other)
        {
            return _path == other._path;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_path, 0);
        }
    }
}
