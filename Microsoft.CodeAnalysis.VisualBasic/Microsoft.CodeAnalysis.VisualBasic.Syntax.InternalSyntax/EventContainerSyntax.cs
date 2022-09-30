using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class EventContainerSyntax : ExpressionSyntax
	{
		internal EventContainerSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal EventContainerSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal EventContainerSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal EventContainerSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
