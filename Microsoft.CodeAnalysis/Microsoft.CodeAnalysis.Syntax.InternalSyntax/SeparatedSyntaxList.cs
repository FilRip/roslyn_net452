using System;
using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public struct SeparatedSyntaxList<TNode> : IEquatable<SeparatedSyntaxList<TNode>> where TNode : GreenNode
    {
        private readonly SyntaxList<GreenNode> _list;

        public GreenNode? Node => _list.Node;

        public int Count => _list.Count + 1 >> 1;

        public int SeparatorCount => _list.Count >> 1;

        public TNode? this[int index] => (TNode)_list[index << 1];

        public SeparatedSyntaxList(SyntaxList<GreenNode> list)
        {
            _list = list;
        }

        [Conditional("DEBUG")]
        private static void Validate(SyntaxList<GreenNode> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.GetRequiredItem(i);
                _ = i & 1;
            }
        }

        public GreenNode? GetSeparator(int index)
        {
            return _list[(index << 1) + 1];
        }

        public SyntaxList<GreenNode> GetWithSeparators()
        {
            return _list;
        }

        public static bool operator ==(in SeparatedSyntaxList<TNode> left, in SeparatedSyntaxList<TNode> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in SeparatedSyntaxList<TNode> left, in SeparatedSyntaxList<TNode> right)
        {
            return !left.Equals(right);
        }

        public bool Equals(SeparatedSyntaxList<TNode> other)
        {
            return _list == other._list;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SeparatedSyntaxList<TNode>)
            {
                return Equals((SeparatedSyntaxList<TNode>)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            SyntaxList<GreenNode> list = _list;
            return list.GetHashCode();
        }

        public static implicit operator SeparatedSyntaxList<GreenNode>(SeparatedSyntaxList<TNode> list)
        {
            return new SeparatedSyntaxList<GreenNode>(list.GetWithSeparators());
        }
    }
}
