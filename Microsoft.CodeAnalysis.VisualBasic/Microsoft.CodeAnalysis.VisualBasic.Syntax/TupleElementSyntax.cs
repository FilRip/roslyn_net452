namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class TupleElementSyntax : VisualBasicSyntaxNode
	{
		internal TupleElementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
