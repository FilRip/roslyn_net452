using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class EmbeddedSymbolExtensions
	{
		public static bool IsEmbeddedSyntaxTree(this SyntaxTree tree)
		{
			return EmbeddedSymbolManager.GetEmbeddedKind(tree) != EmbeddedSymbolKind.None;
		}

		public static EmbeddedSymbolKind GetEmbeddedKind(this SyntaxTree tree)
		{
			return EmbeddedSymbolManager.GetEmbeddedKind(tree);
		}

		public static bool IsEmbeddedOrMyTemplateTree(this SyntaxTree tree)
		{
			VisualBasicSyntaxTree visualBasicSyntaxTree = tree as VisualBasicSyntaxTree;
			if (visualBasicSyntaxTree == null || !visualBasicSyntaxTree.IsMyTemplate)
			{
				return IsEmbeddedSyntaxTree(visualBasicSyntaxTree);
			}
			return true;
		}

		public static bool IsEmbeddedOrMyTemplateLocation(this Location location)
		{
			if (!(location is EmbeddedTreeLocation))
			{
				return location is MyTemplateLocation;
			}
			return true;
		}
	}
}
