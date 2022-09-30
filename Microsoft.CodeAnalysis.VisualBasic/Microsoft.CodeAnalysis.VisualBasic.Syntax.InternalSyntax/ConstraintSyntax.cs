using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ConstraintSyntax : VisualBasicSyntaxNode
	{
		internal ConstraintSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ConstraintSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ConstraintSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ConstraintSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
