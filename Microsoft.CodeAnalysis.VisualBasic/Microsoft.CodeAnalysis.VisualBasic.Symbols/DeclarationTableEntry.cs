using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class DeclarationTableEntry
	{
		public readonly Lazy<RootSingleNamespaceDeclaration> Root;

		public readonly bool IsEmbedded;

		public DeclarationTableEntry(Lazy<RootSingleNamespaceDeclaration> root, bool isEmbedded)
		{
			Root = root;
			IsEmbedded = isEmbedded;
		}
	}
}
