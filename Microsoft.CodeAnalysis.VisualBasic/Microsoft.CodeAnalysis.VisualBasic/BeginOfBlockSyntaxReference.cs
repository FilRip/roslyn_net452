using System.Threading;
using Microsoft.CodeAnalysis.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class BeginOfBlockSyntaxReference : TranslationSyntaxReference
	{
		public BeginOfBlockSyntaxReference(SyntaxReference reference)
			: base(reference)
		{
		}

		protected override SyntaxNode Translate(SyntaxReference reference, CancellationToken cancellationToken)
		{
			return SyntaxFacts.BeginOfBlockStatementIfAny(reference.GetSyntax(cancellationToken));
		}
	}
}
