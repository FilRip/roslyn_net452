using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NullableTypeSyntax : TypeSyntax
	{
		internal readonly TypeSyntax _elementType;

		internal readonly PunctuationSyntax _questionMarkToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal TypeSyntax ElementType => _elementType;

		internal PunctuationSyntax QuestionMarkToken => _questionMarkToken;

		internal NullableTypeSyntax(SyntaxKind kind, TypeSyntax elementType, PunctuationSyntax questionMarkToken)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
		}

		internal NullableTypeSyntax(SyntaxKind kind, TypeSyntax elementType, PunctuationSyntax questionMarkToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
		}

		internal NullableTypeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TypeSyntax elementType, PunctuationSyntax questionMarkToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(elementType);
			_elementType = elementType;
			AdjustFlagsAndWidth(questionMarkToken);
			_questionMarkToken = questionMarkToken;
		}

		internal NullableTypeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
			if (typeSyntax != null)
			{
				AdjustFlagsAndWidth(typeSyntax);
				_elementType = typeSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_questionMarkToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_elementType);
			writer.WriteValue(_questionMarkToken);
		}

		static NullableTypeSyntax()
		{
			CreateInstance = (ObjectReader o) => new NullableTypeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NullableTypeSyntax), (ObjectReader r) => new NullableTypeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NullableTypeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _elementType, 
				1 => _questionMarkToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NullableTypeSyntax(base.Kind, newErrors, GetAnnotations(), _elementType, _questionMarkToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NullableTypeSyntax(base.Kind, GetDiagnostics(), annotations, _elementType, _questionMarkToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNullableType(this);
		}
	}
}
