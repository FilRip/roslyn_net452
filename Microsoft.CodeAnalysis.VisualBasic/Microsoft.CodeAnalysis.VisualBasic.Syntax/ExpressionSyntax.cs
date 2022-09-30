namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ExpressionSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
