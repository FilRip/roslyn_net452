using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlElementStartTagSyntax : XmlNodeSyntax
	{
		internal readonly PunctuationSyntax _lessThanToken;

		internal readonly XmlNodeSyntax _name;

		internal readonly GreenNode _attributes;

		internal readonly PunctuationSyntax _greaterThanToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal PunctuationSyntax LessThanToken => _lessThanToken;

		internal XmlNodeSyntax Name => _name;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> Attributes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(_attributes);

		internal PunctuationSyntax GreaterThanToken => _greaterThanToken;

		internal XmlElementStartTagSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlNodeSyntax name, GreenNode attributes, PunctuationSyntax greaterThanToken)
			: base(kind)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementStartTagSyntax(SyntaxKind kind, PunctuationSyntax lessThanToken, XmlNodeSyntax name, GreenNode attributes, PunctuationSyntax greaterThanToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 4;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementStartTagSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlNodeSyntax name, GreenNode attributes, PunctuationSyntax greaterThanToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 4;
			AdjustFlagsAndWidth(lessThanToken);
			_lessThanToken = lessThanToken;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			AdjustFlagsAndWidth(greaterThanToken);
			_greaterThanToken = greaterThanToken;
		}

		internal XmlElementStartTagSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 4;
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_lessThanToken = punctuationSyntax;
			}
			XmlNodeSyntax xmlNodeSyntax = (XmlNodeSyntax)reader.ReadValue();
			if (xmlNodeSyntax != null)
			{
				AdjustFlagsAndWidth(xmlNodeSyntax);
				_name = xmlNodeSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_attributes = greenNode;
			}
			PunctuationSyntax punctuationSyntax2 = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax2 != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax2);
				_greaterThanToken = punctuationSyntax2;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_lessThanToken);
			writer.WriteValue(_name);
			writer.WriteValue(_attributes);
			writer.WriteValue(_greaterThanToken);
		}

		static XmlElementStartTagSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlElementStartTagSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlElementStartTagSyntax), (ObjectReader r) => new XmlElementStartTagSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementStartTagSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _lessThanToken, 
				1 => _name, 
				2 => _attributes, 
				3 => _greaterThanToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlElementStartTagSyntax(base.Kind, newErrors, GetAnnotations(), _lessThanToken, _name, _attributes, _greaterThanToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlElementStartTagSyntax(base.Kind, GetDiagnostics(), annotations, _lessThanToken, _name, _attributes, _greaterThanToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlElementStartTag(this);
		}
	}
}
