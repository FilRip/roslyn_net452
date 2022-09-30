using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class XmlNodeSyntax : ExpressionSyntax
	{
		internal XmlNodeSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal XmlNodeSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal XmlNodeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal XmlNodeSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
