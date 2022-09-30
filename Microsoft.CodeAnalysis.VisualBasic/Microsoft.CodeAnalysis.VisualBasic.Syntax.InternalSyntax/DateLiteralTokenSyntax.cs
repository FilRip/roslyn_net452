using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DateLiteralTokenSyntax : SyntaxToken
	{
		internal readonly DateTime _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal DateTime Value => _value;

		internal sealed override object ObjectValue => Value;

		internal DateLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, DateTime value)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal DateLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, DateTime value, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_value = value;
		}

		internal DateLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, DateTime value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal DateLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToDate(reader.ReadValue());
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		static DateLiteralTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new DateLiteralTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DateLiteralTokenSyntax), (ObjectReader r) => new DateLiteralTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new DateLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new DateLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DateLiteralTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DateLiteralTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}
	}
}
