namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class CaseClauseSyntax : VisualBasicSyntaxNode
	{
		internal CaseClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
