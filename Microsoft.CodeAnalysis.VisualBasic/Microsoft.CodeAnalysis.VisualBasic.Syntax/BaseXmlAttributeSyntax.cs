namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class BaseXmlAttributeSyntax : XmlNodeSyntax
	{
		internal BaseXmlAttributeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}
	}
}
