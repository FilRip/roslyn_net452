namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ConstraintSyntax : VisualBasicSyntaxNode
	{
		internal ConstraintSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
