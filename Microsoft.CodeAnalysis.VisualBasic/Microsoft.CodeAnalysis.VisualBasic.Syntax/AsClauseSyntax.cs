using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class AsClauseSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken AsKeyword => GetAsKeywordCore();

		internal AsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetAsKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)base.Green)._asKeyword, base.Position, 0);
		}

		public AsClauseSyntax WithAsKeyword(SyntaxToken asKeyword)
		{
			return WithAsKeywordCore(asKeyword);
		}

		internal abstract AsClauseSyntax WithAsKeywordCore(SyntaxToken asKeyword);
	}
}
