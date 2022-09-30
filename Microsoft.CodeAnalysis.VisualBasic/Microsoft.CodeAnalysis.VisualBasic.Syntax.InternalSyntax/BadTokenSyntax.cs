using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class BadTokenSyntax : PunctuationSyntax
	{
		private readonly SyntaxSubKind _subKind;

		internal new static Func<ObjectReader, object> CreateInstance;

		internal SyntaxSubKind SubKind => _subKind;

		internal BadTokenSyntax(SyntaxKind kind, SyntaxSubKind subKind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_subKind = subKind;
		}

		internal BadTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_subKind = (SyntaxSubKind)reader.ReadUInt16();
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteUInt16((ushort)_subKind);
		}

		static BadTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new BadTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(BadTokenSyntax), (ObjectReader r) => new BadTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new BadTokenSyntax(base.Kind, SubKind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia());
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new BadTokenSyntax(base.Kind, SubKind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new BadTokenSyntax(base.Kind, SubKind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new BadTokenSyntax(base.Kind, SubKind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}
	}
}
