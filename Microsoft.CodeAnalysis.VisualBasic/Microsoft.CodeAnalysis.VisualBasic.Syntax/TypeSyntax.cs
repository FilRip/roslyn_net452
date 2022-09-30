namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class TypeSyntax : ExpressionSyntax
	{
		internal TypeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
