using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ImportsClauseSyntax : VisualBasicSyntaxNode
	{
		internal ImportsClauseSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ImportsClauseSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ImportsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ImportsClauseSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
