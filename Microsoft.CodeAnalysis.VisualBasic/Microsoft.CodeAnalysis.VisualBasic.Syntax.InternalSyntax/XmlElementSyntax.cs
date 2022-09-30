using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class XmlElementSyntax : XmlNodeSyntax
	{
		internal readonly XmlElementStartTagSyntax _startTag;

		internal readonly GreenNode _content;

		internal readonly XmlElementEndTagSyntax _endTag;

		internal static Func<ObjectReader, object> CreateInstance;

		internal XmlElementStartTagSyntax StartTag => _startTag;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> Content => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax>(_content);

		internal XmlElementEndTagSyntax EndTag => _endTag;

		internal XmlElementSyntax(SyntaxKind kind, XmlElementStartTagSyntax startTag, GreenNode content, XmlElementEndTagSyntax endTag)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(startTag);
			_startTag = startTag;
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
			AdjustFlagsAndWidth(endTag);
			_endTag = endTag;
		}

		internal XmlElementSyntax(SyntaxKind kind, XmlElementStartTagSyntax startTag, GreenNode content, XmlElementEndTagSyntax endTag, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(startTag);
			_startTag = startTag;
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
			AdjustFlagsAndWidth(endTag);
			_endTag = endTag;
		}

		internal XmlElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlElementStartTagSyntax startTag, GreenNode content, XmlElementEndTagSyntax endTag)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(startTag);
			_startTag = startTag;
			if (content != null)
			{
				AdjustFlagsAndWidth(content);
				_content = content;
			}
			AdjustFlagsAndWidth(endTag);
			_endTag = endTag;
		}

		internal XmlElementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			XmlElementStartTagSyntax xmlElementStartTagSyntax = (XmlElementStartTagSyntax)reader.ReadValue();
			if (xmlElementStartTagSyntax != null)
			{
				AdjustFlagsAndWidth(xmlElementStartTagSyntax);
				_startTag = xmlElementStartTagSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_content = greenNode;
			}
			XmlElementEndTagSyntax xmlElementEndTagSyntax = (XmlElementEndTagSyntax)reader.ReadValue();
			if (xmlElementEndTagSyntax != null)
			{
				AdjustFlagsAndWidth(xmlElementEndTagSyntax);
				_endTag = xmlElementEndTagSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_startTag);
			writer.WriteValue(_content);
			writer.WriteValue(_endTag);
		}

		static XmlElementSyntax()
		{
			CreateInstance = (ObjectReader o) => new XmlElementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(XmlElementSyntax), (ObjectReader r) => new XmlElementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlElementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _startTag, 
				1 => _content, 
				2 => _endTag, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new XmlElementSyntax(base.Kind, newErrors, GetAnnotations(), _startTag, _content, _endTag);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new XmlElementSyntax(base.Kind, GetDiagnostics(), annotations, _startTag, _content, _endTag);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitXmlElement(this);
		}
	}
}
