using System.Diagnostics;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public struct SourceSpan
    {
        public readonly int StartLine;

        public readonly int StartColumn;

        public readonly int EndLine;

        public readonly int EndColumn;

        public readonly DebugSourceDocument Document;

        public SourceSpan(DebugSourceDocument document, int startLine, int startColumn, int endLine, int endColumn)
        {
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
            Document = document;
        }

        public override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override bool Equals(object? obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private string GetDebuggerDisplay()
        {
            return $"({StartLine}, {StartColumn}) - ({EndLine}, {EndColumn})";
        }
    }
}
