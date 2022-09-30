using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class DeclarationStatementSyntax : StatementSyntax
	{
		internal DeclarationStatementSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal DeclarationStatementSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal DeclarationStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal DeclarationStatementSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
