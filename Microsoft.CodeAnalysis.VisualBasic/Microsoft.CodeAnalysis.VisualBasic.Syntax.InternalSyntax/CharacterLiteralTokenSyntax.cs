using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CharacterLiteralTokenSyntax : SyntaxToken
	{
		internal readonly char _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal char Value => _value;

		internal sealed override object ObjectValue => Value;

		internal CharacterLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, char value)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal CharacterLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, char value, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_value = value;
		}

		internal CharacterLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, char value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal CharacterLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToChar(reader.ReadValue());
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		static CharacterLiteralTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new CharacterLiteralTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CharacterLiteralTokenSyntax), (ObjectReader r) => new CharacterLiteralTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new CharacterLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new CharacterLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CharacterLiteralTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CharacterLiteralTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}
	}
}
