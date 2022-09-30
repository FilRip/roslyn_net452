using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class TypeSyntax : ExpressionSyntax
	{
		internal TypeSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal TypeSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal TypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal TypeSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
