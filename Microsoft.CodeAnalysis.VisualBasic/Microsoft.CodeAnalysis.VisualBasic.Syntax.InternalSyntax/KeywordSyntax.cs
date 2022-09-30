using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class KeywordSyntax : SyntaxToken
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal sealed override object ObjectValue => base.Kind switch
		{
			SyntaxKind.NothingKeyword => null, 
			SyntaxKind.TrueKeyword => Boxes.BoxedTrue, 
			SyntaxKind.FalseKeyword => Boxes.BoxedFalse, 
			_ => base.Text, 
		};

		internal sealed override bool IsKeyword => true;

		internal KeywordSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
		}

		internal KeywordSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
		}

		internal KeywordSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
		}

		internal KeywordSyntax(ObjectReader reader)
			: base(reader)
		{
		}

		static KeywordSyntax()
		{
			CreateInstance = (ObjectReader o) => new KeywordSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(KeywordSyntax), (ObjectReader r) => new KeywordSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new KeywordSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia());
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new KeywordSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new KeywordSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new KeywordSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia());
		}
	}
}
