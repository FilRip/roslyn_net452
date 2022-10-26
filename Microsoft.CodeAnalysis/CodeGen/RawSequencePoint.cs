// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CodeGen
{
    /// <summary>
    /// Represents a sequence point before translation by #line/ExternalSource directives.
    /// </summary>
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct RawSequencePoint
    {
        internal readonly SyntaxTree SyntaxTree;
        internal readonly int ILMarker;
        public readonly TextSpan Span;

        // Special text span indicating a hidden sequence point.
        public static readonly TextSpan HiddenSequencePointSpan = new(0x7FFFFFFF, 0);

        public RawSequencePoint(SyntaxTree syntaxTree, int ilMarker, TextSpan span)
        {
            this.SyntaxTree = syntaxTree;
            this.ILMarker = ilMarker;
            this.Span = span;
        }

        private string GetDebuggerDisplay()
        {
            return string.Format("#{0}: {1}", ILMarker, Span == HiddenSequencePointSpan ? "hidden" : Span.ToString());
        }
    }
}
