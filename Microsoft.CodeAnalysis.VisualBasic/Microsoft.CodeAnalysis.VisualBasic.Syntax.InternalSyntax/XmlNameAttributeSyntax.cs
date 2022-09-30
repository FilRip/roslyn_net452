using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlNameAttributeSyntax : BaseXmlAttributeSyntax
	{
		internal readonly XmlNameSyntax _name;

		internal readonly PunctuationSyntax _equalsToken;

		internal readonly PunctuationSyntax _startQuoteToken;

		internal readonly IdentifierNameSyntax _reference;

		internal readonly PunctuationSyntax _endQuoteToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlNameSyntax Name => _name;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal PunctuationSyntax StartQuoteToken => _startQuoteToken;

		internal IdentifierNameSyntax Reference => _reference;

		internal PunctuationSyntax EndQuoteToken => _endQuoteToken;

		internal XmlNameAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken)
			: base(kind)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			AdjustFlagsAndWidth(reference);
			_reference = reference;
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlNameAttributeSyntax(SyntaxKind kind, XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			AdjustFlagsAndWidth(reference);
			_reference = reference;
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlNameAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(startQuoteToken);
			_startQuoteToken = startQuoteToken;
			AdjustFlagsAndWidth(reference);
			_reference = reference;
			AdjustFlagsAndWidth(endQuoteToken);
			_endQuoteToken = endQuoteToken;
		}

		internal XmlNameAttributeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)reader.ReadValue();
			if (xmlNameSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameSyntax);
				_name = xmlNameSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_startQuoteToken = punctuationSyntax2;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_reference = identifierNameSyntax;
			}
			PunctuationSyntax punctuationSyntax3 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax3 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax3);
				_endQuoteToken = punctuationSyntax3;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_startQuoteToken);
			writer.WriteValue(_reference);
			writer.WriteValue(_endQuoteToken);
		}

		static XmlNameAttributeSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlNameAttributeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlNameAttributeSyntax), (ObjectReader r) => new XmlNameAttributeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameAttributeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _equalsToken, 
				2 => _startQuoteToken, 
				3 => _reference, 
				4 => _endQuoteToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlNameAttributeSyntax(base.Kind, newErrors, GetAnnotations(), _name, _equalsToken, _startQuoteToken, _reference, _endQuoteToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlNameAttributeSyntax(base.Kind, GetDiagnostics(), annotations, _name, _equalsToken, _startQuoteToken, _reference, _endQuoteToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlNameAttribute(this);
		}
	}
}
