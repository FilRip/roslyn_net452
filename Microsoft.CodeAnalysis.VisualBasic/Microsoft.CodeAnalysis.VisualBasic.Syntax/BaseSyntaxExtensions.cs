using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	[StandardModule]
	internal sealed class BaseSyntaxExtensions
	{
		internal static Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode ToGreen(this Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode node)
		{
			return node;
		}

		internal static Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode ToGreen(this VisualBasicSyntaxNode node)
		{
			return node?.VbGreen;
		}

		internal static SyntaxNode ToRed(this Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode node)
		{
			return node?.CreateRed(null, 0);
		}

		internal static VisualBasicSyntaxNode ToRed(this VisualBasicSyntaxNode node)
		{
			return node;
		}
	}
}
