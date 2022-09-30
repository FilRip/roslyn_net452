using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class QueryClauseSyntax : VisualBasicSyntaxNode
	{
		internal QueryClauseSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal QueryClauseSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal QueryClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal QueryClauseSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
