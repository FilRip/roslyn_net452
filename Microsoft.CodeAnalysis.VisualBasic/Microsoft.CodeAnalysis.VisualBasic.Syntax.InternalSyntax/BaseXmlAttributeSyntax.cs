using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class BaseXmlAttributeSyntax : XmlNodeSyntax
	{
		internal BaseXmlAttributeSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal BaseXmlAttributeSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal BaseXmlAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal BaseXmlAttributeSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
