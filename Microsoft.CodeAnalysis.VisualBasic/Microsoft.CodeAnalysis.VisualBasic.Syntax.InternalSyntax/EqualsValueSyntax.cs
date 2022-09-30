using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class EqualsValueSyntax : VisualBasicSyntaxNode
	{
		internal readonly PunctuationSyntax _equalsToken;

		internal readonly ExpressionSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal ExpressionSyntax Value => _value;

		internal EqualsValueSyntax(SyntaxKind kind, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal EqualsValueSyntax(SyntaxKind kind, PunctuationSyntax equalsToken, ExpressionSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal EqualsValueSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax equalsToken, ExpressionSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal EqualsValueSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_value = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_value);
		}

		static EqualsValueSyntax()
		{
			CreateInstance = (ObjectReader o) => new EqualsValueSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(EqualsValueSyntax), (ObjectReader r) => new EqualsValueSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.EqualsValueSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _equalsToken, 
				1 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new EqualsValueSyntax(base.Kind, newErrors, GetAnnotations(), _equalsToken, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new EqualsValueSyntax(base.Kind, GetDiagnostics(), annotations, _equalsToken, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitEqualsValue(this);
		}
	}
}
