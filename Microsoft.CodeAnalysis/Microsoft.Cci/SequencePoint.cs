using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.Cci
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct SequencePoint
    {
        public const int HiddenLine = 16707566;

        public readonly int Offset;

        public readonly int StartLine;

        public readonly int EndLine;

        public readonly ushort StartColumn;

        public readonly ushort EndColumn;

        public readonly DebugSourceDocument Document;

        public bool IsHidden => StartLine == 16707566;

        public SequencePoint(DebugSourceDocument document, int offset, int startLine, ushort startColumn, int endLine, ushort endColumn)
        {
            Offset = offset;
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
            if (!IsHidden)
            {
                return $"{Offset}: ({StartLine}, {StartColumn}) - ({EndLine}, {EndColumn})";
            }
            return "<hidden>";
        }
    }
}
