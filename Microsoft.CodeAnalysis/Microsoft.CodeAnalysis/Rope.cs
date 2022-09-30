using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class Rope
    {
        private sealed class StringRope : Rope
        {
            private readonly string _value;

            public override int Length => _value.Length;

            public StringRope(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value;
            }

            protected override IEnumerable<char> GetChars()
            {
                return _value;
            }
        }

        private sealed class ConcatRope : Rope
        {
            private readonly Rope _left;

            private readonly Rope _right;

            public override int Length { get; }

            public ConcatRope(Rope left, Rope right)
            {
                _left = left;
                _right = right;
                Length = checked(left.Length + right.Length);
            }

            public override string ToString()
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                Stack<Rope> stack = new Stack<Rope>();
                stack.Push(this);
                while (stack.Count != 0)
                {
                    Rope rope = stack.Pop();
                    if (!(rope is StringRope stringRope))
                    {
                        if (!(rope is ConcatRope concatRope))
                        {
                            throw ExceptionUtilities.UnexpectedValue(rope.GetType().Name);
                        }
                        stack.Push(concatRope._right);
                        stack.Push(concatRope._left);
                    }
                    else
                    {
                        instance.Builder.Append(stringRope.ToString());
                    }
                }
                return instance.ToStringAndFree();
            }

            protected override IEnumerable<char> GetChars()
            {
                Stack<Rope> stack = new Stack<Rope>();
                stack.Push(this);
                while (stack.Count != 0)
                {
                    Rope rope = stack.Pop();
                    if (!(rope is StringRope stringRope))
                    {
                        if (!(rope is ConcatRope concatRope))
                        {
                            throw ExceptionUtilities.UnexpectedValue(rope.GetType().Name);
                        }
                        stack.Push(concatRope._right);
                        stack.Push(concatRope._left);
                    }
                    else
                    {
                        string text = stringRope.ToString();
                        for (int i = 0; i < text.Length; i++)
                        {
                            yield return text[i];
                        }
                    }
                }
            }
        }

        public static readonly Rope Empty = ForString("");

        public abstract int Length { get; }

        public abstract override string ToString();

        protected abstract IEnumerable<char> GetChars();

        private Rope()
        {
        }

        public static Rope ForString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            return new StringRope(s);
        }

        public static Rope Concat(Rope r1, Rope r2)
        {
            if (r1 == null)
            {
                throw new ArgumentNullException("r1");
            }
            if (r2 == null)
            {
                throw new ArgumentNullException("r2");
            }
            if (r1.Length != 0)
            {
                if (r2.Length != 0)
                {
                    if (checked(r1.Length + r2.Length) >= 32)
                    {
                        return new ConcatRope(r1, r2);
                    }
                    return ForString(r1.ToString() + r2.ToString());
                }
                return r1;
            }
            return r2;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Rope rope) || Length != rope.Length)
            {
                return false;
            }
            if (Length == 0)
            {
                return true;
            }
            IEnumerator<char> enumerator = GetChars().GetEnumerator();
            IEnumerator<char> enumerator2 = rope.GetChars().GetEnumerator();
            while (enumerator.MoveNext() && enumerator2.MoveNext())
            {
                if (enumerator.Current != enumerator2.Current)
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int num = Length;
            foreach (char @char in GetChars())
            {
                num = Hash.Combine(@char, num);
            }
            return num;
        }
    }
}
