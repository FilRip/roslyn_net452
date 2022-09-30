using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ExecutableStatementSyntax : StatementSyntax
	{
		internal ExecutableStatementSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ExecutableStatementSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ExecutableStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ExecutableStatementSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
