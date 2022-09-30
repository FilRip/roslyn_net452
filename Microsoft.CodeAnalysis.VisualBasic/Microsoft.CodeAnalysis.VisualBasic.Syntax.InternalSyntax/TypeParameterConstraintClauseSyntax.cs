using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class TypeParameterConstraintClauseSyntax : VisualBasicSyntaxNode
	{
		internal TypeParameterConstraintClauseSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal TypeParameterConstraintClauseSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal TypeParameterConstraintClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal TypeParameterConstraintClauseSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
