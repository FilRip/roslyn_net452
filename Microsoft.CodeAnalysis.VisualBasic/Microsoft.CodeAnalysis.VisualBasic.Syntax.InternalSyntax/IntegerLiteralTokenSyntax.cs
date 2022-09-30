using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class IntegerLiteralTokenSyntax<T> : IntegerLiteralTokenSyntax
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

		internal IntegerLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, LiteralBase @base, TypeCharacter typeSuffix, T value)
			: base(kind, text, leadingTrivia, trailingTrivia, @base, typeSuffix)
		{
			_value = value;
		}

		internal IntegerLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, LiteralBase @base, TypeCharacter typeSuffix, T value)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia, @base, typeSuffix)
		{
			_value = value;
		}

		internal IntegerLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_value = Microsoft.VisualBasic.CompilerServices.Conversions.ToGenericParameter<T>(reader.ReadValue());
		}

		static IntegerLiteralTokenSyntax()
		{
			ObjectBinder.RegisterTypeReader(typeof(IntegerLiteralTokenSyntax<T>), (ObjectReader r) => new IntegerLiteralTokenSyntax<T>(r));
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_value);
		}

		public override GreenNode WithLeadingTrivia(GreenNode trivia)
		{
			return new IntegerLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, trivia, GetTrailingTrivia(), _base, _typeSuffix, _value);
		}

		public override GreenNode WithTrailingTrivia(GreenNode trivia)
		{
			return new IntegerLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), GetAnnotations(), base.Text, GetLeadingTrivia(), trivia, _base, _typeSuffix, _value);
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new IntegerLiteralTokenSyntax<T>(base.Kind, newErrors, GetAnnotations(), base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _base, _typeSuffix, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new IntegerLiteralTokenSyntax<T>(base.Kind, GetDiagnostics(), annotations, base.Text, GetLeadingTrivia(), GetTrailingTrivia(), _base, _typeSuffix, _value);
		}
	}
	internal abstract class IntegerLiteralTokenSyntax : SyntaxToken
	{
		internal readonly LiteralBase _base;

		internal readonly TypeCharacter _typeSuffix;

		internal LiteralBase Base => _base;

		internal TypeCharacter TypeSuffix => _typeSuffix;

		internal IntegerLiteralTokenSyntax(SyntaxKind kind, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, LiteralBase @base, TypeCharacter typeSuffix)
			: base(kind, text, leadingTrivia, trailingTrivia)
		{
			_base = @base;
			_typeSuffix = typeSuffix;
		}

		internal IntegerLiteralTokenSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, string text, GreenNode leadingTrivia, GreenNode trailingTrivia, LiteralBase @base, TypeCharacter typeSuffix)
			: base(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		{
			_base = @base;
			_typeSuffix = typeSuffix;
		}

		internal IntegerLiteralTokenSyntax(ObjectReader reader)
			: base(reader)
		{
			_base = (LiteralBase)reader.ReadByte();
			_typeSuffix = (TypeCharacter)reader.ReadByte();
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteByte((byte)_base);
			writer.WriteByte((byte)_typeSuffix);
		}
	}
}
