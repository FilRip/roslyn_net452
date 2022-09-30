namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class EventContainerSyntax : ExpressionSyntax
	{
		internal EventContainerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
