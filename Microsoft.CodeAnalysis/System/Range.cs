using System.Runtime.CompilerServices;

using Roslyn.Utilities;

#nullable enable

namespace System
{
    internal readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }

        public Index End { get; }

        public static Range All => Index.Start..Index.End;

        public Range(Index start, Index end)
        {
            Start = start;
            End = end;
        }

        public override bool Equals(object? value)
        {
            if (value is Range range && range.Start.Equals(Start))
            {
                return range.End.Equals(End);
            }
            return false;
        }

        public bool Equals(Range other)
        {
            if (other.Start.Equals(Start))
            {
                return other.End.Equals(End);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Start.GetHashCode(), End.GetHashCode());
        }

        public override string ToString()
        {
            return getFromEndSpecifier(Start) + toString(Start) + ".." + getFromEndSpecifier(End) + toString(End);
            static string getFromEndSpecifier(Index index)
            {
                if (!index.IsFromEnd)
                {
                    return string.Empty;
                }
                return "^";
            }
            static string toString(Index index)
            {
                return ((uint)index.Value).ToString();
            }
        }

        public static Range StartAt(Index start)
        {
            return start..Index.End;
        }

        public static Range EndAt(Index end)
        {
            return Index.Start..end;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            Index start = Start;
            int num = ((!start.IsFromEnd) ? start.Value : (length - start.Value));
            Index end = End;
            int num2 = ((!end.IsFromEnd) ? end.Value : (length - end.Value));
            if ((uint)num2 > (uint)length || (uint)num > (uint)num2)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            return (num, num2 - num);
        }
    }
}
