using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ExpressionSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ExpressionSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
