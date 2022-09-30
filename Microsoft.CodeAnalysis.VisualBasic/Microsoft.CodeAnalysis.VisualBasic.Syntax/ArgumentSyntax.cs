namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ArgumentSyntax : VisualBasicSyntaxNode
	{
		public abstract bool IsNamed { get; }

		public bool IsOmitted => Kind() == SyntaxKind.OmittedArgument;

		internal ArgumentSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		public abstract ExpressionSyntax GetExpression();
	}
}
