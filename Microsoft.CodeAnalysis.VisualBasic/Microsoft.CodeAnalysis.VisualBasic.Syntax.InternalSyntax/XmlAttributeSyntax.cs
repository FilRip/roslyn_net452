using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlAttributeSyntax : BaseXmlAttributeSyntax
	{
		internal readonly XmlNodeSyntax _name;

		internal readonly PunctuationSyntax _equalsToken;

		internal readonly XmlNodeSyntax _value;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlNodeSyntax Name => _name;

		internal PunctuationSyntax EqualsToken => _equalsToken;

		internal XmlNodeSyntax Value => _value;

		internal XmlAttributeSyntax(SyntaxKind kind, XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlAttributeSyntax(SyntaxKind kind, XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(name);
			_name = name;
			AdjustFlagsAndWidth(equalsToken);
			_equalsToken = equalsToken;
			AdjustFlagsAndWidth(value);
			_value = value;
		}

		internal XmlAttributeSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)reader.ReadValue();
			if (xmlNodeSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNodeSyntax);
				_name = xmlNodeSyntax;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_equalsToken = punctuationSyntax;
			}
			XmlNodeSyntax xmlNodeSyntax2 = (XmlNodeSyntax)reader.ReadValue();
			if (xmlNodeSyntax2 != null)
			{
				AdjustFlagsAndWidth(xmlNodeSyntax2);
				_value = xmlNodeSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_name);
			writer.WriteValue(_equalsToken);
			writer.WriteValue(_value);
		}

		static XmlAttributeSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlAttributeSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlAttributeSyntax), (ObjectReader r) => new XmlAttributeSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlAttributeSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				1 => _equalsToken, 
				2 => _value, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlAttributeSyntax(base.Kind, newErrors, GetAnnotations(), _name, _equalsToken, _value);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlAttributeSyntax(base.Kind, GetDiagnostics(), annotations, _name, _equalsToken, _value);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlAttribute(this);
		}
	}
}
