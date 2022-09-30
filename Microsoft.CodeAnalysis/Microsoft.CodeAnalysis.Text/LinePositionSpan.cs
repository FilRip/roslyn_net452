using System;
using System.Runtime.Serialization;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    [DataContract]
    public readonly struct LinePositionSpan : IEquatable<LinePositionSpan>
    {
        [DataMember(Order = 0)]
        private readonly LinePosition _start;

        [DataMember(Order = 1)]
        private readonly LinePosition _end;

        public LinePosition Start => _start;

        public LinePosition End => _end;

        public LinePositionSpan(LinePosition start, LinePosition end)
        {
            if (end < start)
            {
                throw new ArgumentException(CodeAnalysisResources.EndMustNotBeLessThanStart, "end");
            }
            _start = start;
            _end = end;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LinePositionSpan)
            {
                return Equals((LinePositionSpan)obj);
            }
            return false;
        }

        public bool Equals(LinePositionSpan other)
        {
            if (_start.Equals(other._start))
            {
                return _end.Equals(other._end);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_start.GetHashCode(), _end.GetHashCode());
        }

        public static bool operator ==(LinePositionSpan left, LinePositionSpan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LinePositionSpan left, LinePositionSpan right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({_start})-({_end})";
        }
    }
}
