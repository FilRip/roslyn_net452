using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class ArgumentSyntax : VisualBasicSyntaxNode
	{
		internal ArgumentSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal ArgumentSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal ArgumentSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal ArgumentSyntax(ObjectReader reader)
			: base(reader)
		{
		}

		public ExpressionSyntax GetExpression()
		{
			return base.Kind switch
			{
				SyntaxKind.OmittedArgument => SyntaxFactory.MissingExpression(), 
				SyntaxKind.RangeArgument => ((RangeArgumentSyntax)this).UpperBound, 
				_ => ((SimpleArgumentSyntax)this).Expression, 
			};
		}
	}
}
