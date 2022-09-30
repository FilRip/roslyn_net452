using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class FloatingLiteralTokenSyntax<T> : FloatingLiteralTokenSyntax
	{
		internal readonly T _value;

		internal T Value => _value;

		internal override string ValueText
		{
			get
			{
				T value = _value;
				return value.ToString();
			}
		}

		internal override object ObjectValue => Value;

		internal FloatingLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix, T value)
			: base(kind, text, leadingTrivia, trailingTrivia, typeSuffix)
		{
			_value = value;
		}

		internal FloatingLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix, T value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia, typeSuffix)
		{
			_value = value;
		}

		internal FloatingLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToGenericParameter<T>(reader.ReadValue());
		}

		static FloatingLiteralTokenSyntax()
		{
			ObjectBinder.RegisterTypeReader(typeof(FloatingLiteralTokenSyntax<T>), (ObjectReader r) => new FloatingLiteralTokenSyntax<T>(r));
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new FloatingLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _typeSuffix, _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new FloatingLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _typeSuffix, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new FloatingLiteralTokenSyntax<T>(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _typeSuffix, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new FloatingLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _typeSuffix, _value);
		}
	}
	internal abstract class FloatingLiteralTokenSyntax : SyntaxToken
	{
		internal readonly TypeCharacter _typeSuffix;

		internal TypeCharacter TypeSuffix => _typeSuffix;

		internal FloatingLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_typeSuffix = typeSuffix;
		}

		internal FloatingLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, TypeCharacter typeSuffix)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_typeSuffix = typeSuffix;
		}

		internal FloatingLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_typeSuffix = (TypeCharacter)reader.ReadByte();
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteByte((byte)_typeSuffix);
		}
	}
}
