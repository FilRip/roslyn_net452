using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SingleNamespaceDeclaration : SingleNamespaceOrTypeDeclaration
	{
		private class Comparer : IEqualityComparer<SingleNamespaceDeclaration>
		{
			private bool Equals(SingleNamespaceDeclaration decl1, SingleNamespaceDeclaration decl2)
			{
				return CaseInsensitiveComparison.Equals(decl1.Name, decl2.Name);
			}

			bool IEqualityComparer<SingleNamespaceDeclaration>.Equals(SingleNamespaceDeclaration decl1, SingleNamespaceDeclaration decl2)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Equals
				return this.Equals(decl1, decl2);
			}

			private int GetHashCode(SingleNamespaceDeclaration decl1)
			{
				return CaseInsensitiveComparison.GetHashCode(decl1.Name);
			}

			int IEqualityComparer<SingleNamespaceDeclaration>.GetHashCode(SingleNamespaceDeclaration decl1)
			{
				//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
				return this.GetHashCode(decl1);
			}
		}

		private readonly ImmutableArray<SingleNamespaceOrTypeDeclaration> _children;

		public readonly bool IsPartOfRootNamespace;

		public static readonly IEqualityComparer<SingleNamespaceDeclaration> EqualityComparer = new Comparer();

		public bool HasImports { get; set; }

		public virtual bool IsGlobalNamespace => false;

		public override DeclarationKind Kind => DeclarationKind.Namespace;

		public SingleNamespaceDeclaration(string name, bool hasImports, SyntaxReference syntaxReference, Location nameLocation, ImmutableArray<SingleNamespaceOrTypeDeclaration> children, bool isPartOfRootNamespace = false)
			: base(name, syntaxReference, nameLocation)
		{
			_children = children;
			HasImports = hasImports;
			IsPartOfRootNamespace = isPartOfRootNamespace;
		}

		protected override ImmutableArray<SingleNamespaceOrTypeDeclaration> GetNamespaceOrTypeDeclarationChildren()
		{
			return _children;
		}

		public NamespaceBlockSyntax GetNamespaceBlockSyntax()
		{
			if (SyntaxReference == null)
			{
				return null;
			}
			return SyntaxReference.GetSyntax().AncestorsAndSelf().OfType<NamespaceBlockSyntax>()
				.FirstOrDefault();
		}

		public new static string BestName<T>(ImmutableArray<T> singleDeclarations, ref bool multipleSpellings) where T : SingleNamespaceDeclaration
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
					if (singleDeclarations[0].IsPartOfRootNamespace)
					{
						return text;
					}
					if (singleDeclarations[i].IsPartOfRootNamespace)
					{
						return name;
					}
					if (num2 > 0)
					{
						text = name;
					}
				}
			}
			return text;
		}
	}
}
