using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class IdentifierTokenSyntax : SyntaxToken
	{
		internal abstract SyntaxKind PossibleKeywordKind { get; }

		internal abstract bool IsBracketed { get; }

		internal abstract string IdentifierText { get; }

		internal override string ValueText => IdentifierText;

		public override int RawContextualKind => (int)PossibleKeywordKind;

		internal abstract TypeCharacter TypeCharacter { get; }

		internal IdentifierTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
			: base(kind, errors, annotations, text, precedingTrivia, followingTrivia)
		{
		}

		internal IdentifierTokenSyntax(ObjectReader reader)
			: base(reader)
		{
		}
	}
}
