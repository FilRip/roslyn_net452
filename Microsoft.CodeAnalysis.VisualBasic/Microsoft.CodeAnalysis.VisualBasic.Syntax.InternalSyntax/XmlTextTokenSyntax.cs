using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlTextTokenSyntax : SyntaxToken
	{
		internal readonly string _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal string Value => _value;

		internal sealed override string ValueText => Value;

		internal XmlTextTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal XmlTextTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_value = value;
		}

		internal XmlTextTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, string value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_value = value;
		}

		internal XmlTextTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(reader.ReadValue());
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		static XmlTextTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlTextTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlTextTokenSyntax), (ObjectReader r) => new XmlTextTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new XmlTextTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new XmlTextTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlTextTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlTextTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _value);
		}
	}
}
