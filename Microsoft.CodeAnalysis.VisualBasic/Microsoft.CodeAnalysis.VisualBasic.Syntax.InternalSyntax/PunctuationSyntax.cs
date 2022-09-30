using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class PunctuationSyntax : SyntaxToken
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
		}

		internal PunctuationSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
		}

		internal PunctuationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
		}

		internal PunctuationSyntax(ObjectReader reader)
			: base(reader)
		{
		}

		static PunctuationSyntax()
		{
			CreateInstance = (ObjectReader o) => new PunctuationSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(PunctuationSyntax), (ObjectReader r) => new PunctuationSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new PunctuationSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia());
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new PunctuationSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new PunctuationSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new PunctuationSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}
	}
}
