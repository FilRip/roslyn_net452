namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ImportsClauseSyntax : VisualBasicSyntaxNode
	{
		internal ImportsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
