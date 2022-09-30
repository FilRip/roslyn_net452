using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class StructuredTriviaSyntax : VisualBasicSyntaxNode
	{
		internal StructuredTriviaSyntax(ObjectReader reader)
			: base(reader)
		{
			Initialize();
		}

		internal StructuredTriviaSyntax(SyntaxKind kind)
			: base(kind)
		{
			Initialize();
		}

		internal StructuredTriviaSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			Initialize();
			SetFactoryContext(context);
		}

		internal StructuredTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
			Initialize();
		}

		private void Initialize()
		{
			SetFlags(NodeFlags.ContainsStructuredTrivia);
			if (base.Kind == SyntaxKind.SkippedTokensTrivia)
			{
				SetFlags(NodeFlags.ContainsSkippedText);
			}
		}
	}
}
