using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ComplexIdentifierSyntax : IdentifierTokenSyntax
	{
		private readonly SyntaxKind _possibleKeywordKind;

		private readonly bool _isBracketed;

		private readonly string _identifierText;

		private readonly TypeCharacter _typeCharacter;

		internal override SyntaxKind PossibleKeywordKind => _possibleKeywordKind;

		public override int RawContextualKind => (int)_possibleKeywordKind;

		internal override bool IsBracketed => _isBracketed;

		internal override string IdentifierText => _identifierText;

		internal override TypeCharacter TypeCharacter => _typeCharacter;

		internal ComplexIdentifierSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode precedingTrivia, GreenNode followingTrivia, SyntaxKind possibleKeywordKind, bool isBracketed, string identifierText, TypeCharacter typeCharacter)
			: base(kind, errors, annotations, text, precedingTrivia, followingTrivia)
		{
			_possibleKeywordKind = possibleKeywordKind;
			_isBracketed = isBracketed;
			_identifierText = identifierText;
			_typeCharacter = typeCharacter;
		}

		internal ComplexIdentifierSyntax(ObjectReader reader)
			: base(reader)
		{
			_possibleKeywordKind = (SyntaxKind)reader.ReadUInt16();
			_isBracketed = reader.ReadBoolean();
			_identifierText = reader.ReadString();
			_typeCharacter = (TypeCharacter)reader.ReadByte();
		}

		static ComplexIdentifierSyntax()
		{
			ObjectBinder.RegisterTypeReader(typeof(ComplexIdentifierSyntax), (ObjectReader r) => new ComplexIdentifierSyntax(r));
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteUInt16((ushort)_possibleKeywordKind);
			writer.WriteBoolean(_isBracketed);
			writer.WriteString(_identifierText);
			writer.WriteByte((byte)_typeCharacter);
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new ComplexIdentifierSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), PossibleKeywordKind, IsBracketed, IdentifierText, TypeCharacter);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new ComplexIdentifierSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, PossibleKeywordKind, IsBracketed, IdentifierText, TypeCharacter);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ComplexIdentifierSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), PossibleKeywordKind, IsBracketed, IdentifierText, TypeCharacter);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ComplexIdentifierSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), PossibleKeywordKind, IsBracketed, IdentifierText, TypeCharacter);
		}
	}
}
