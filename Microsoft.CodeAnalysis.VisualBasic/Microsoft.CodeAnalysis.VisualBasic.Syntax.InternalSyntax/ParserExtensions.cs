using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class ParserExtensions
	{
		internal static bool Any<T>(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> @this, params SyntaxKind[] kinds) where T : VisualBasicSyntaxNode
		{
			int num = kinds.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (@this.Any((int)kinds[i]))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool AnyAndOnly<T>(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> @this, params SyntaxKind[] kinds) where T : VisualBasicSyntaxNode
		{
			bool flag = false;
			int num = @this.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				flag = SyntaxKindExtensions.Contains(kinds, @this[i]!.Kind);
				if (!flag)
				{
					return false;
				}
			}
			return flag;
		}

		internal static bool ContainsDiagnostics<T>(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<T> @this) where T : VisualBasicSyntaxNode
		{
			int num = @this.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (@this[i]!.ContainsDiagnostics)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsDiagnostics<T>(this SyntaxListBuilder<T> @this) where T : VisualBasicSyntaxNode
		{
			int num = @this.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (@this[i]!.ContainsDiagnostics)
				{
					return true;
				}
			}
			return false;
		}
	}
}
