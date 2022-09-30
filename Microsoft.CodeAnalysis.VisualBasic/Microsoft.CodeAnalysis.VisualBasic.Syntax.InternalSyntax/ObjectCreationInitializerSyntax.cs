using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ObjectCreationInitializerSyntax : VisualBasicSyntaxNode
	{
		internal ObjectCreationInitializerSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ObjectCreationInitializerSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ObjectCreationInitializerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ObjectCreationInitializerSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
