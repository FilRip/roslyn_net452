namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ObjectCreationInitializerSyntax : VisualBasicSyntaxNode
	{
		internal ObjectCreationInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
