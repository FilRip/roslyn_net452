namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class XmlNodeSyntax : ExpressionSyntax
	{
		internal XmlNodeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
