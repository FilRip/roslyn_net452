using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class InstanceExpressionSyntax : ExpressionSyntax
	{
		public SyntaxToken Keyword => GetKeywordCore();

		internal InstanceExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InstanceExpressionSyntax)base.Green)._keyword, base.Position, 0);
		}

		public InstanceExpressionSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithKeywordCore(keyword);
		}

		internal abstract InstanceExpressionSyntax WithKeywordCore(SyntaxToken keyword);
	}
}
