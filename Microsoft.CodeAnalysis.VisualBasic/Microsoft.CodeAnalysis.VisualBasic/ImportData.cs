using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class ImportData
	{
		public readonly HashSet<NamespaceOrTypeSymbol> Members;

		public readonly Dictionary<string, AliasAndImportsClausePosition> Aliases;

		public readonly Dictionary<string, XmlNamespaceAndImportsClausePosition> XmlNamespaces;

		protected ImportData(HashSet<NamespaceOrTypeSymbol> members, Dictionary<string, AliasAndImportsClausePosition> aliases, Dictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespaces)
		{
			Members = members;
			Aliases = aliases;
			XmlNamespaces = xmlNamespaces;
		}

		public abstract void AddMember(SyntaxReference syntaxRef, NamespaceOrTypeSymbol member, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies);

		public abstract void AddAlias(SyntaxReference syntaxRef, string name, AliasSymbol alias, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies);
	}
}
