namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class InterpolatedStringContentSyntax : VisualBasicSyntaxNode
	{
		internal InterpolatedStringContentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
