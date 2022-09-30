namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ProjectImportsBinder : Binder
	{
		private readonly SyntaxTree _tree;

		internal override bool SuppressObsoleteDiagnostics => true;

		public ProjectImportsBinder(Binder containingBinder, SyntaxTree tree)
			: base(containingBinder)
		{
			_tree = tree;
		}

		public override SyntaxReference GetSyntaxReference(VisualBasicSyntaxNode node)
		{
			return _tree.GetReference(node);
		}
	}
}
