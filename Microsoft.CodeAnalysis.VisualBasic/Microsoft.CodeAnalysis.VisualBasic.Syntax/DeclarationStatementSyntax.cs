namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class DeclarationStatementSyntax : StatementSyntax
	{
		internal DeclarationStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
