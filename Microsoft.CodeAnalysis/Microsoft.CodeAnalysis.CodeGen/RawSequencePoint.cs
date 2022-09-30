using System.Diagnostics;

using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct RawSequencePoint
    {
        internal readonly SyntaxTree SyntaxTree;

        internal readonly int ILMarker;

        public readonly TextSpan Span;

        public static readonly TextSpan HiddenSequencePointSpan = new TextSpan(int.MaxValue, 0);

        public RawSequencePoint(SyntaxTree syntaxTree, int ilMarker, TextSpan span)
        {
            SyntaxTree = syntaxTree;
            ILMarker = ilMarker;
            Span = span;
        }

        private string GetDebuggerDisplay()
        {
            return string.Format("#{0}: {1}", ILMarker, (Span == HiddenSequencePointSpan) ? "hidden" : Span.ToString());
        }
    }
}
