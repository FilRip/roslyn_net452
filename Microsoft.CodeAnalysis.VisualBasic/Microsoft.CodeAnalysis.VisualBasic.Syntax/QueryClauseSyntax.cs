namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class QueryClauseSyntax : VisualBasicSyntaxNode
	{
		internal QueryClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
