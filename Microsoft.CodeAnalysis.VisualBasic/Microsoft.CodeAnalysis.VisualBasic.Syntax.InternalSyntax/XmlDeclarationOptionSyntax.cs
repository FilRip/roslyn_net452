using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlDeclarationOptionSyntax : VisualBasicSyntaxNode
	{
		internal readonly XmlNameTokenSyntax _name;

		internal readonly PunctuationSyntax _equals;

		internal readonly XmlStringSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlNameTokenSyntax Name => _name;

		internal new PunctuationSyntax Equals => _equals;

		internal XmlStringSyntax Value => _value;

		internal XmlDeclarationOptionSyntax(SyntaxKind kind, XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equals);
			_equals = equals;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlDeclarationOptionSyntax(SyntaxKind kind, XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equals);
			_equals = equals;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlDeclarationOptionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equals);
			_equals = equals;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlDeclarationOptionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			XmlNameTokenSyntax xmlNameTokenSyntax = (XmlNameTokenSyntax)reader.ReadValue();
			if (xmlNameTokenSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNameTokenSyntax);
				_name = xmlNameTokenSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equals = punctuationSyntax;
			}
			XmlStringSyntax xmlStringSyntax = (XmlStringSyntax)reader.ReadValue();
			if (xmlStringSyntax != null)
			{
				AdjustFlagsAndWidth(xmlStringSyntax);
				_value = xmlStringSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
			writer.WriteValue(_equals);
			writer.WriteValue(_value);
		}

		static XmlDeclarationOptionSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlDeclarationOptionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlDeclarationOptionSyntax), (ObjectReader r) => new XmlDeclarationOptionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlDeclarationOptionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _equals, 
				2 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlDeclarationOptionSyntax(base.Kind, newErrors, GetAnnotations(), _name, _equals, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlDeclarationOptionSyntax(base.Kind, GetDiagnostics(), annotations, _name, _equals, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlDeclarationOption(this);
		}
	}
}
