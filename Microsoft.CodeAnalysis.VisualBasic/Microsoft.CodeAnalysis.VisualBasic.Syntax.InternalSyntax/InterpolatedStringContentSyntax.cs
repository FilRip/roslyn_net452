using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class InterpolatedStringContentSyntax : VisualBasicSyntaxNode
	{
		internal InterpolatedStringContentSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal InterpolatedStringContentSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal InterpolatedStringContentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal InterpolatedStringContentSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
