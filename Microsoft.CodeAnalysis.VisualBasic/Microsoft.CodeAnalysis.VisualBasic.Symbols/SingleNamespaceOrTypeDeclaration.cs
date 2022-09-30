using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SingleNamespaceOrTypeDeclaration : Declaration
	{
		public readonly SyntaxReference SyntaxReference;

		public readonly Location NameLocation;

		public Location Location => SyntaxReference.GetLocation();

		public new ImmutableArray<SingleNamespaceOrTypeDeclaration> Children => GetNamespaceOrTypeDeclarationChildren();

		protected SingleNamespaceOrTypeDeclaration(string name, SyntaxReference syntaxReference, Location nameLocation)
			: base(name)
		{
			SyntaxReference = syntaxReference;
			NameLocation = nameLocation;
		}

		protected abstract ImmutableArray<SingleNamespaceOrTypeDeclaration> GetNamespaceOrTypeDeclarationChildren();

		protected override ImmutableArray<Declaration> GetDeclarationChildren()
		{
			return StaticCast<Declaration>.From(GetNamespaceOrTypeDeclarationChildren());
		}

		public static string BestName<T>(ImmutableArray<T> singleDeclarations, ref bool multipleSpellings) where T : SingleNamespaceOrTypeDeclaration
		{
			multipleSpellings = false;
			string text = singleDeclarations[0].Name;
			int num = singleDeclarations.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				string name = singleDeclarations[i].Name;
				int num2 = string.Compare(text, name, StringComparison.Ordinal);
				if (num2 != 0)
				{
					multipleSpellings = true;
					if (num2 > 0)
					{
						text = name;
					}
				}
			}
			return text;
		}

		public static string BestName<T>(ImmutableArray<T> singleDeclarations) where T : SingleNamespaceOrTypeDeclaration
		{
			bool multipleSpellings = false;
			return BestName(singleDeclarations, ref multipleSpellings);
		}
	}
}
