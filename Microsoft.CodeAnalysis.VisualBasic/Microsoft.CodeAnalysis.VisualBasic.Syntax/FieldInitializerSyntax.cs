using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class FieldInitializerSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken KeyKeyword => GetKeyKeywordCore();

		internal FieldInitializerSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetKeyKeywordCore()
		{
			KeywordSyntax keyKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax)base.Green)._keyKeyword;
			return (keyKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, keyKeyword, base.Position, 0);
		}

		public FieldInitializerSyntax WithKeyKeyword(SyntaxToken keyKeyword)
		{
			return WithKeyKeywordCore(keyKeyword);
		}

		internal abstract FieldInitializerSyntax WithKeyKeywordCore(SyntaxToken keyKeyword);
	}
}
