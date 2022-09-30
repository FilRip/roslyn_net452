namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AttributeBinder : Binder
	{
		private readonly VisualBasicSyntaxNode _root;

		internal VisualBasicSyntaxNode Root => _root;

		internal override bool IsDefaultInstancePropertyAllowed => false;

		public AttributeBinder(Binder containingBinder, SyntaxTree tree, VisualBasicSyntaxNode node = null)
			: base(containingBinder, tree)
		{
			_root = node;
		}

		public override Binder GetBinder(SyntaxNode node)
		{
			return null;
		}
	}
}
