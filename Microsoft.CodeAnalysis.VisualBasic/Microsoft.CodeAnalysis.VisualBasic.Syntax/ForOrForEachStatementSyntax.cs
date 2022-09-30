using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class ForOrForEachStatementSyntax : StatementSyntax
	{
		internal VisualBasicSyntaxNode _controlVariable;

		public SyntaxToken ForKeyword => GetForKeywordCore();

		public VisualBasicSyntaxNode ControlVariable => GetControlVariableCore();

		internal ForOrForEachStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetForKeywordCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax)base.Green)._forKeyword, base.Position, 0);
		}

		public ForOrForEachStatementSyntax WithForKeyword(SyntaxToken forKeyword)
		{
			return WithForKeywordCore(forKeyword);
		}

		internal abstract ForOrForEachStatementSyntax WithForKeywordCore(SyntaxToken forKeyword);

		internal virtual VisualBasicSyntaxNode GetControlVariableCore()
		{
			return GetRed(ref _controlVariable, 1);
		}

		public ForOrForEachStatementSyntax WithControlVariable(VisualBasicSyntaxNode controlVariable)
		{
			return WithControlVariableCore(controlVariable);
		}

		internal abstract ForOrForEachStatementSyntax WithControlVariableCore(VisualBasicSyntaxNode controlVariable);
	}
}
