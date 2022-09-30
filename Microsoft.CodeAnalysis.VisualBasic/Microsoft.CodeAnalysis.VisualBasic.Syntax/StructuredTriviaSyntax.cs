namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class StructuredTriviaSyntax : VisualBasicSyntaxNode, IStructuredTriviaSyntax
	{
		private SyntaxTrivia _parentTrivia;

		public override SyntaxTrivia ParentTrivia => _parentTrivia;

		internal StructuredTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, startLocation, parent?.SyntaxTree)
		{
		}

		internal static StructuredTriviaSyntax Create(SyntaxTrivia trivia)
		{
			VisualBasicSyntaxNode parent = (VisualBasicSyntaxNode)trivia.Token.Parent;
			int position = trivia.Position;
			StructuredTriviaSyntax obj = (StructuredTriviaSyntax)trivia.UnderlyingNode!.CreateRed(parent, position);
			obj._parentTrivia = trivia;
			return obj;
		}
	}
}
