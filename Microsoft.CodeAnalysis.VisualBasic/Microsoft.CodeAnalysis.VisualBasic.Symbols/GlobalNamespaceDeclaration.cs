using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class GlobalNamespaceDeclaration : SingleNamespaceDeclaration
	{
		public override bool IsGlobalNamespace => true;

		public GlobalNamespaceDeclaration(bool hasImports, SyntaxReference syntaxReference, Location nameLocation, ImmutableArray<SingleNamespaceOrTypeDeclaration> children)
			: base(string.Empty, hasImports, syntaxReference, nameLocation, children)
		{
		}
	}
}
