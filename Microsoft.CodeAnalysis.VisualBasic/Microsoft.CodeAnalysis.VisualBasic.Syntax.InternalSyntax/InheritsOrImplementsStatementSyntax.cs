using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class InheritsOrImplementsStatementSyntax : DeclarationStatementSyntax
	{
		internal InheritsOrImplementsStatementSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal InheritsOrImplementsStatementSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal InheritsOrImplementsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal InheritsOrImplementsStatementSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
