using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class TupleElementSyntax : VisualBasicSyntaxNode
	{
		internal TupleElementSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal TupleElementSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal TupleElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal TupleElementSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
