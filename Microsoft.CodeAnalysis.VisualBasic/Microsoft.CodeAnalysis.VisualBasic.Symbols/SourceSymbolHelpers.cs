using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class SourceSymbolHelpers
	{
		public static SyntaxNodeOrToken GetAsClauseLocation(SyntaxToken identifier, AsClauseSyntax asClauseOpt)
		{
			if (asClauseOpt != null && (asClauseOpt.Kind() != SyntaxKind.AsNewClause || ((AsNewClauseSyntax)asClauseOpt).NewExpression.Kind() != SyntaxKind.AnonymousObjectCreationExpression))
			{
				return SyntaxExtensions.Type(asClauseOpt);
			}
			return identifier;
		}
	}
}
