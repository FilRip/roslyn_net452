namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class MergedNamespaceOrTypeDeclaration : Declaration
	{
		protected MergedNamespaceOrTypeDeclaration(string name)
			: base(name)
		{
		}
	}
}
