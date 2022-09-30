namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class InheritsOrImplementsStatementSyntax : DeclarationStatementSyntax
	{
		internal InheritsOrImplementsStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
