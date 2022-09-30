using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal struct XmlContext
	{
		private readonly XmlElementStartTagSyntax _start;

		private readonly SyntaxListBuilder<XmlNodeSyntax> _content;

		private readonly SyntaxListPool _pool;

		public XmlElementStartTagSyntax StartElement => _start;

		public XmlContext(SyntaxListPool pool, XmlElementStartTagSyntax start)
		{
			this = default(XmlContext);
			_pool = pool;
			_start = start;
			_content = _pool.Allocate<XmlNodeSyntax>();
		}

		public void Add(XmlNodeSyntax xml)
		{
			_content.Add(xml);
		}

		public XmlNodeSyntax CreateElement(XmlElementEndTagSyntax endElement)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = _content.ToList();
			_pool.Free(_content);
			return SyntaxFactory.XmlElement(_start, syntaxList, endElement);
		}

		public XmlNodeSyntax CreateElement(XmlElementEndTagSyntax endElement, DiagnosticInfo diagnostic)
		{
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<XmlNodeSyntax> syntaxList = _content.ToList();
			_pool.Free(_content);
			return SyntaxFactory.XmlElement((XmlElementStartTagSyntax)_start.AddError(diagnostic), syntaxList, endElement);
		}
	}
}
