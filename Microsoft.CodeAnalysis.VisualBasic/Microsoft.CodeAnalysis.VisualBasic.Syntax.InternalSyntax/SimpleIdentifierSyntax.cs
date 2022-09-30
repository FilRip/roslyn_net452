using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class SimpleIdentifierSyntax : IdentifierTokenSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal override SyntaxKind PossibleKeywordKind => SyntaxKind.IdentifierToken;

		internal override bool IsBracketed => false;

		internal override string IdentifierText => base.Text;

		internal override TypeCharacter TypeCharacter => TypeCharacter.None;

		internal SimpleIdentifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
			: base(kind, errors, annotations, text, precedingTrivia, followingTrivia)
		{
		}

		internal SimpleIdentifierSyntax(ObjectReader reader)
			: base(reader)
		{
		}

		static SimpleIdentifierSyntax()
		{
			CreateInstance = (ObjectReader o) => new SimpleIdentifierSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(SimpleIdentifierSyntax), (ObjectReader r) => new SimpleIdentifierSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new SimpleIdentifierSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia());
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new SimpleIdentifierSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new SimpleIdentifierSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new SimpleIdentifierSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}
	}
}
