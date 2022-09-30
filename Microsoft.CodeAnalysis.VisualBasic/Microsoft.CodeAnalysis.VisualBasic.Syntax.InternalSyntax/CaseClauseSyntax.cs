using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class CaseClauseSyntax : VisualBasicSyntaxNode
	{
		internal CaseClauseSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal CaseClauseSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal CaseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal CaseClauseSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
