using System;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct LexicalSortKey
	{
		[Flags]
		private enum SyntaxTreeKind : byte
		{
			None = 0,
			Unset = 1,
			EmbeddedAttribute = 2,
			VbCore = 4,
			XmlHelper = 8,
			MyTemplate = 0x10
		}

		private SyntaxTreeKind _embeddedKind;

		private int _treeOrdinal;

		private int _position;

		public static readonly LexicalSortKey NotInSource = new LexicalSortKey(SyntaxTreeKind.None, -1, 0);

		public static readonly LexicalSortKey NotInitialized = new LexicalSortKey
		{
			_embeddedKind = SyntaxTreeKind.None,
			_treeOrdinal = -1,
			_position = -1
		};

		private SyntaxTreeKind EmbeddedKind => _embeddedKind;

		public int TreeOrdinal => _treeOrdinal;

		public int Position => _position;

		public bool IsInitialized => Volatile.Read(ref _position) >= 0;

		private LexicalSortKey(SyntaxTreeKind embeddedKind, int treeOrdinal, int location)
		{
			this = default(LexicalSortKey);
			_embeddedKind = embeddedKind;
			_treeOrdinal = treeOrdinal;
			_position = location;
		}

		private LexicalSortKey(SyntaxTreeKind embeddedKind, SyntaxTree tree, int location, VisualBasicCompilation compilation)
			: this(embeddedKind, (tree == null || embeddedKind != 0) ? (-1) : compilation.GetSyntaxTreeOrdinal(tree), location)
		{
		}

		private static SyntaxTreeKind GetEmbeddedKind(SyntaxTree tree)
		{
			if (tree != null)
			{
				if (!VisualBasicExtensions.IsMyTemplate(tree))
				{
					return (SyntaxTreeKind)EmbeddedSymbolExtensions.GetEmbeddedKind(tree);
				}
				return SyntaxTreeKind.MyTemplate;
			}
			return SyntaxTreeKind.None;
		}

		public LexicalSortKey(SyntaxTree tree, int position, VisualBasicCompilation compilation)
			: this(GetEmbeddedKind(tree), tree, position, compilation)
		{
		}

		public LexicalSortKey(SyntaxReference syntaxRef, VisualBasicCompilation compilation)
			: this(syntaxRef.SyntaxTree, syntaxRef.Span.Start, compilation)
		{
		}

		public LexicalSortKey(Location location, VisualBasicCompilation compilation)
		{
			this = default(LexicalSortKey);
			if ((object)location == null)
			{
				_embeddedKind = SyntaxTreeKind.None;
				_treeOrdinal = -1;
				_position = 0;
				return;
			}
			VisualBasicSyntaxTree visualBasicSyntaxTree = (VisualBasicSyntaxTree)LocationExtensions.PossiblyEmbeddedOrMySourceTree(location);
			SyntaxTreeKind embeddedKind = GetEmbeddedKind(visualBasicSyntaxTree);
			if (embeddedKind != 0)
			{
				_embeddedKind = embeddedKind;
				_treeOrdinal = -1;
			}
			else
			{
				_embeddedKind = SyntaxTreeKind.None;
				_treeOrdinal = ((visualBasicSyntaxTree == null) ? (-1) : compilation.GetSyntaxTreeOrdinal(visualBasicSyntaxTree));
			}
			_position = LocationExtensions.PossiblyEmbeddedOrMySourceSpan(location).Start;
		}

		public LexicalSortKey(VisualBasicSyntaxNode node, VisualBasicCompilation compilation)
			: this(node.SyntaxTree, node.SpanStart, compilation)
		{
		}

		public LexicalSortKey(SyntaxToken token, VisualBasicCompilation compilation)
			: this((VisualBasicSyntaxTree)token.SyntaxTree, token.SpanStart, compilation)
		{
		}

		public static int Compare(ref LexicalSortKey xSortKey, ref LexicalSortKey ySortKey)
		{
			if (xSortKey.EmbeddedKind != ySortKey.EmbeddedKind)
			{
				return (ySortKey.EmbeddedKind > xSortKey.EmbeddedKind) ? 1 : (-1);
			}
			if (xSortKey.EmbeddedKind == SyntaxTreeKind.None && xSortKey.TreeOrdinal != ySortKey.TreeOrdinal)
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

		public static int Compare(Location first, Location second, VisualBasicCompilation compilation)
		{
			if (first.SourceTree != null && first.SourceTree == second.SourceTree)
			{
				return LocationExtensions.PossiblyEmbeddedOrMySourceSpan(first).Start - LocationExtensions.PossiblyEmbeddedOrMySourceSpan(second).Start;
			}
			LexicalSortKey xSortKey = new LexicalSortKey(first, compilation);
			LexicalSortKey ySortKey = new LexicalSortKey(second, compilation);
			return Compare(ref xSortKey, ref ySortKey);
		}

		public static int Compare(SyntaxReference first, SyntaxReference second, VisualBasicCompilation compilation)
		{
			if (first.SyntaxTree != null && first.SyntaxTree == second.SyntaxTree)
			{
				return first.Span.Start - second.Span.Start;
			}
			LexicalSortKey xSortKey = new LexicalSortKey(first, compilation);
			LexicalSortKey ySortKey = new LexicalSortKey(second, compilation);
			return Compare(ref xSortKey, ref ySortKey);
		}

		public static LexicalSortKey First(LexicalSortKey xSortKey, LexicalSortKey ySortKey)
		{
			if (Compare(ref xSortKey, ref ySortKey) <= 0)
			{
				return xSortKey;
			}
			return ySortKey;
		}

		public void SetFrom(ref LexicalSortKey other)
		{
			_embeddedKind = other._embeddedKind;
			_treeOrdinal = other._treeOrdinal;
			Volatile.Write(ref _position, other._position);
		}
	}
}
