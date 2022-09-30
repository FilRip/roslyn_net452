using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class StringLiteralTokenSyntax : SyntaxToken
	{
		internal readonly string _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal string Value => _value;

		internal sealed override object ObjectValue => Value;

		internal sealed override string ValueText => Value;

		internal StringLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal StringLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_value = value;
		}

		internal StringLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal StringLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(reader.ReadValue());
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		static StringLiteralTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new StringLiteralTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(StringLiteralTokenSyntax), (ObjectReader r) => new StringLiteralTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new StringLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new StringLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new StringLiteralTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new StringLiteralTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}
	}
}
