using System.Threading;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public struct LexicalSortKey
    {
        private int _treeOrdinal;

        private int _position;

        public static readonly LexicalSortKey NotInSource = new LexicalSortKey
        {
            _treeOrdinal = -1,
            _position = 0
        };

        public static readonly LexicalSortKey NotInitialized = new LexicalSortKey
        {
            _treeOrdinal = -1,
            _position = -1
        };

        public static readonly LexicalSortKey SynthesizedCtor = new LexicalSortKey
        {
            _treeOrdinal = int.MaxValue,
            _position = 2147483646
        };

        public static readonly LexicalSortKey SynthesizedCCtor = new LexicalSortKey
        {
            _treeOrdinal = int.MaxValue,
            _position = int.MaxValue
        };

        public int TreeOrdinal => _treeOrdinal;

        public int Position => _position;

        public bool IsInitialized => Volatile.Read(ref _position) >= 0;

        public static LexicalSortKey GetSynthesizedMemberKey(int offset)
        {
            LexicalSortKey result = default(LexicalSortKey);
            result._treeOrdinal = int.MaxValue;
            result._position = 2147483645 - offset;
            return result;
        }

        private LexicalSortKey(int treeOrdinal, int position)
        {
            _treeOrdinal = treeOrdinal;
            _position = position;
        }

        private LexicalSortKey(SyntaxTree tree, int position, CSharpCompilation compilation)
            : this((tree == null) ? (-1) : compilation.GetSyntaxTreeOrdinal(tree), position)
        {
        }

        public LexicalSortKey(SyntaxReference syntaxRef, CSharpCompilation compilation)
            : this(syntaxRef.SyntaxTree, syntaxRef.Span.Start, compilation)
        {
        }

        public LexicalSortKey(Location location, CSharpCompilation compilation)
            : this(location.SourceTree, location.SourceSpan.Start, compilation)
        {
        }

        public LexicalSortKey(CSharpSyntaxNode node, CSharpCompilation compilation)
            : this(node.SyntaxTree, node.SpanStart, compilation)
        {
        }

        public LexicalSortKey(SyntaxToken token, CSharpCompilation compilation)
            : this(token.SyntaxTree, token.SpanStart, compilation)
        {
        }

        public static int Compare(LexicalSortKey xSortKey, LexicalSortKey ySortKey)
        {
            if (xSortKey.TreeOrdinal != ySortKey.TreeOrdinal)
            {
                if (xSortKey.TreeOrdinal < 0)
                {
                    return 1;
                }
                if (ySortKey.TreeOrdinal < 0)
                {
                    return -1;
                }
                return xSortKey.TreeOrdinal - ySortKey.TreeOrdinal;
            }
            return xSortKey.Position - ySortKey.Position;
        }

        public static LexicalSortKey First(LexicalSortKey xSortKey, LexicalSortKey ySortKey)
        {
            if (Compare(xSortKey, ySortKey) <= 0)
            {
                return xSortKey;
            }
            return ySortKey;
        }

        public void SetFrom(LexicalSortKey other)
        {
            _treeOrdinal = other._treeOrdinal;
            Volatile.Write(ref _position, other._position);
        }
    }
}
