namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundLocalDeclarationBase : BoundStatement
	{
		protected BoundLocalDeclarationBase(BoundKind kind, SyntaxNode syntax, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
		}

		protected BoundLocalDeclarationBase(BoundKind kind, SyntaxNode syntax)
			: base(kind, syntax)
		{
		}
	}
}
