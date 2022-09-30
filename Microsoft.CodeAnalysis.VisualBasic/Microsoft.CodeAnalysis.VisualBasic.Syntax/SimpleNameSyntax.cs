using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class SimpleNameSyntax : NameSyntax
	{
		public SyntaxToken Identifier => GetIdentifierCore();

		internal SimpleNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxToken GetIdentifierCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)base.Green)._identifier, base.Position, 0);
		}

		public SimpleNameSyntax WithIdentifier(SyntaxToken identifier)
		{
			return WithIdentifierCore(identifier);
		}

		internal abstract SimpleNameSyntax WithIdentifierCore(SyntaxToken identifier);
	}
}
