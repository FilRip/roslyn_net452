using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class Declaration
	{
		public abstract DeclarationKind Kind { get; }

		public string Name { get; set; }

		public ImmutableArray<Declaration> Children => GetDeclarationChildren();

		protected Declaration(string name)
		{
			Name = name;
		}

		protected abstract ImmutableArray<Declaration> GetDeclarationChildren();
	}
}
