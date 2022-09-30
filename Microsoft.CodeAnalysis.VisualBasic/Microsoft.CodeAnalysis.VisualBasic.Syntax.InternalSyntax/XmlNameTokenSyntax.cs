using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlNameTokenSyntax : SyntaxToken
	{
		internal readonly SyntaxKind _possibleKeywordKind;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SyntaxKind PossibleKeywordKind => _possibleKeywordKind;

		internal XmlNameTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, SyntaxKind possibleKeywordKind)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_possibleKeywordKind = possibleKeywordKind;
		}

		internal XmlNameTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, SyntaxKind possibleKeywordKind, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_possibleKeywordKind = possibleKeywordKind;
		}

		internal XmlNameTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, SyntaxKind possibleKeywordKind)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_possibleKeywordKind = possibleKeywordKind;
		}

		internal XmlNameTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_possibleKeywordKind = (SyntaxKind)reader.ReadInt32();
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteInt32((int)_possibleKeywordKind);
		}

		static XmlNameTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlNameTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlNameTokenSyntax), (ObjectReader r) => new XmlNameTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new XmlNameTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _possibleKeywordKind);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new XmlNameTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _possibleKeywordKind);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlNameTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _possibleKeywordKind);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlNameTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _possibleKeywordKind);
		}
	}
}
