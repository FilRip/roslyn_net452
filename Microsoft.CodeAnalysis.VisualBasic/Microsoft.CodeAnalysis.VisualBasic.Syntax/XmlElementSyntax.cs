using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlElementSyntax : XmlNodeSyntax
	{
		internal XmlElementStartTagSyntax _startTag;

		internal SyntaxNode _content;

		internal XmlElementEndTagSyntax _endTag;

		public XmlElementStartTagSyntax StartTag => GetRedAtZero(ref _startTag);

		public SyntaxList<XmlNodeSyntax> Content
		{
			get
			{
				SyntaxNode red = GetRed(ref _content, 1);
				return new SyntaxList<XmlNodeSyntax>(red);
			}
		}

		public XmlElementEndTagSyntax EndTag => GetRed(ref _endTag, 2);

		internal XmlElementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlElementStartTagSyntax startTag, SyntaxNode content, XmlElementEndTagSyntax endTag)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)startTag.Green, content?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)endTag.Green), null, 0)
		{
		}

		public XmlElementSyntax WithStartTag(XmlElementStartTagSyntax startTag)
		{
			return Update(startTag, Content, EndTag);
		}

		public XmlElementSyntax WithContent(SyntaxList<XmlNodeSyntax> content)
		{
			return Update(StartTag, content, EndTag);
		}

		public XmlElementSyntax AddContent(params XmlNodeSyntax[] items)
		{
			return WithContent(Content.AddRange(items));
		}

		public XmlElementSyntax WithEndTag(XmlElementEndTagSyntax endTag)
		{
			return Update(StartTag, Content, endTag);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _startTag, 
				1 => _content, 
				2 => _endTag, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => StartTag, 
				1 => GetRed(ref _content, 1), 
				2 => EndTag, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlElement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlElement(this);
		}

		public XmlElementSyntax Update(XmlElementStartTagSyntax startTag, SyntaxList<XmlNodeSyntax> content, XmlElementEndTagSyntax endTag)
		{
			if (startTag != StartTag || content != Content || endTag != EndTag)
			{
				XmlElementSyntax xmlElementSyntax = SyntaxFactory.XmlElement(startTag, content, endTag);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlElementSyntax, annotations);
				}
				return xmlElementSyntax;
			}
			return this;
		}
	}
}
