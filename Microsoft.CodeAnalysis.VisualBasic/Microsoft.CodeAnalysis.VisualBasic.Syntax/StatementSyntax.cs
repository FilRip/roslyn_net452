namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class StatementSyntax : VisualBasicSyntaxNode
	{
		internal StatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
