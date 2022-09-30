namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ExecutableStatementSyntax : StatementSyntax
	{
		internal ExecutableStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
