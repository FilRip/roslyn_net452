namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class AggregationSyntax : ExpressionSyntax
	{
		internal AggregationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
