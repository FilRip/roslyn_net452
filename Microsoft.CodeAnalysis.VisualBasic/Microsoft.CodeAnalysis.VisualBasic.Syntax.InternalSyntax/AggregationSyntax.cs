using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class AggregationSyntax : ExpressionSyntax
	{
		internal AggregationSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal AggregationSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal AggregationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal AggregationSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
