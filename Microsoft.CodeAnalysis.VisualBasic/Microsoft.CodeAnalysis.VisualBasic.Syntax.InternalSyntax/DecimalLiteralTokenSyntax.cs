using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DecimalLiteralTokenSyntax : SyntaxToken
	{
		internal readonly TypeCharacter _typeSuffix;

		internal readonly decimal _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeCharacter TypeSuffix => _typeSuffix;

		internal decimal Value => _value;

		internal sealed override object ObjectValue => Value;

		internal DecimalLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix, decimal value)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_typeSuffix = typeSuffix;
			_value = value;
		}

		internal DecimalLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix, decimal value, ISyntaxFactoryContext context)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			SetFactoryContext(context);
			_typeSuffix = typeSuffix;
			_value = value;
		}

		internal DecimalLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix, decimal value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_typeSuffix = typeSuffix;
			_value = value;
		}

		internal DecimalLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_typeSuffix = (TypeCharacter)reader.ReadInt32();
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(reader.ReadValue());
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteInt32((int)_typeSuffix);
			writer.WriteValue(_value);
		}

		static DecimalLiteralTokenSyntax()
		{
			CreateInstance = (ObjectReader o) => new DecimalLiteralTokenSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DecimalLiteralTokenSyntax), (ObjectReader r) => new DecimalLiteralTokenSyntax(r));
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new DecimalLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _typeSuffix, _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new DecimalLiteralTokenSyntax(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _typeSuffix, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DecimalLiteralTokenSyntax(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _typeSuffix, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DecimalLiteralTokenSyntax(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _typeSuffix, _value);
		}
	}
}
