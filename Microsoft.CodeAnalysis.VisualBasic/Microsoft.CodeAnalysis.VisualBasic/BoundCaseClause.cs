namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundCaseClause : BoundNode
	{
		protected BoundCaseClause(BoundKind kind, SyntaxNode syntax, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
		}

		protected BoundCaseClause(BoundKind kind, SyntaxNode syntax)
			: base(kind, syntax)
		{
		}
	}
}
