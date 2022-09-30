using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class StatementSyntax : VisualBasicSyntaxNode
	{
		internal StatementSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal StatementSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal StatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal StatementSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
