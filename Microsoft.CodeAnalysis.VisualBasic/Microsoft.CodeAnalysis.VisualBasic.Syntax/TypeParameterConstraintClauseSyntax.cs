namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class TypeParameterConstraintClauseSyntax : VisualBasicSyntaxNode
	{
		internal TypeParameterConstraintClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
