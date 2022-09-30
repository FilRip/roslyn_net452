using System;
using System.Collections.Generic;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public readonly struct TextChangeRange : IEquatable<TextChangeRange>
    {
        public TextSpan Span { get; }

        public int NewLength { get; }

        internal int NewEnd => Span.Start + NewLength;

        public static IReadOnlyList<TextChangeRange> NoChanges => SpecializedCollections.EmptyReadOnlyList<TextChangeRange>();

        public TextChangeRange(TextSpan span, int newLength)
        {
            this = default(TextChangeRange);
            if (newLength < 0)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            Span = span;
            NewLength = newLength;
        }

        public bool Equals(TextChangeRange other)
        {
            if (other.Span == Span)
            {
                return other.NewLength == NewLength;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TextChangeRange other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(NewLength, Span.GetHashCode());
        }

        public static bool operator ==(TextChangeRange left, TextChangeRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextChangeRange left, TextChangeRange right)
        {
            return !(left == right);
        }

        public static TextChangeRange Collapse(IEnumerable<TextChangeRange> changes)
        {
            int num = 0;
            int num2 = int.MaxValue;
            int num3 = 0;
            foreach (TextChangeRange change in changes)
            {
                num += change.NewLength - change.Span.Length;
                if (change.Span.Start < num2)
                {
                    num2 = change.Span.Start;
                }
                if (change.Span.End > num3)
                {
                    num3 = change.Span.End;
                }
            }
            if (num2 > num3)
            {
                return default(TextChangeRange);
            }
            TextSpan span = TextSpan.FromBounds(num2, num3);
            int newLength = span.Length + num;
            return new TextChangeRange(span, newLength);
        }

        private string GetDebuggerDisplay()
        {
            return $"new TextChangeRange(new TextSpan({Span.Start}, {Span.Length}), {NewLength})";
        }

        public override string ToString()
        {
            return $"TextChangeRange(Span={Span}, NewLength={NewLength})";
        }
    }
}
