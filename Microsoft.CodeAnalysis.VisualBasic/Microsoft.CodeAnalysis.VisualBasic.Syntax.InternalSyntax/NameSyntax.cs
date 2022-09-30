using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class NameSyntax : TypeSyntax
	{
		internal NameSyntax(SyntaxKind kind)
			: base(kind)
		{
		}

		internal NameSyntax(SyntaxKind kind, ISyntaxFactoryContext context)
			: base(kind)
		{
			SetFactoryContext(context);
		}

		internal NameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations)
			: base(kind, errors, annotations)
		{
		}

		internal NameSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
