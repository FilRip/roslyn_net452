namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SpeculativeStatementBinder : ExecutableCodeBinder
	{
		public override bool IsSemanticModelBinder => true;

		public SpeculativeStatementBinder(VisualBasicSyntaxNode root, Binder containingBinder)
			: base(root, containingBinder)
		{
		}
	}
}
