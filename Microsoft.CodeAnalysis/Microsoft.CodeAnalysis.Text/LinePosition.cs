using System;
using System.Runtime.Serialization;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Text
{
    [DataContract]
    public readonly struct LinePosition : IEquatable<LinePosition>, IComparable<LinePosition>
    {
        [DataMember(Order = 0)]
        private readonly int _line;

        [DataMember(Order = 1)]
        private readonly int _character;

        public static LinePosition Zero => default(LinePosition);

        public int Line => _line;

        public int Character => _character;

        public LinePosition(int line, int character)
        {
            if (line < 0)
            {
                throw new ArgumentOutOfRangeException("line");
            }
            if (character < 0)
            {
                throw new ArgumentOutOfRangeException("character");
            }
            _line = line;
            _character = character;
        }

        internal LinePosition(int character)
        {
            if (character < 0)
            {
                throw new ArgumentOutOfRangeException("character");
            }
            _line = -1;
            _character = character;
        }

        public static bool operator ==(LinePosition left, LinePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LinePosition left, LinePosition right)
        {
            return !left.Equals(right);
        }

        public bool Equals(LinePosition other)
        {
            if (other.Line == Line)
            {
                return other.Character == Character;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LinePosition)
            {
                return Equals((LinePosition)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Line, Character);
        }

        public override string ToString()
        {
            return Line + "," + Character;
        }

        public int CompareTo(LinePosition other)
        {
            int num = _line.CompareTo(other._line);
            if (num == 0)
            {
                return _character.CompareTo(other.Character);
            }
            return num;
        }

        public static bool operator >(LinePosition left, LinePosition right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(LinePosition left, LinePosition right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(LinePosition left, LinePosition right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(LinePosition left, LinePosition right)
        {
            return left.CompareTo(right) <= 0;
        }
    }
}
