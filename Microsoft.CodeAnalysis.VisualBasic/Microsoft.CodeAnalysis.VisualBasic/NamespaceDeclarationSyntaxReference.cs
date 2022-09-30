using System.Threading;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class NamespaceDeclarationSyntaxReference : TranslationSyntaxReference
	{
		public NamespaceDeclarationSyntaxReference(SyntaxReference reference)
			: base(reference)
		{
		}

		protected override SyntaxNode Translate(SyntaxReference reference, CancellationToken cancellationToken)
		{
			SyntaxNode syntaxNode = reference.GetSyntax(cancellationToken);
			while (syntaxNode is NameSyntax)
			{
				syntaxNode = syntaxNode.Parent;
			}
			return syntaxNode;
		}
	}
}
