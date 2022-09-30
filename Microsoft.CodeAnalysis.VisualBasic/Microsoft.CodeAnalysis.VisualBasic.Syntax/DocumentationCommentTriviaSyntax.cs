using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DocumentationCommentTriviaSyntax : StructuredTriviaSyntax
	{
		internal SyntaxNode _content;

		public SyntaxList<XmlNodeSyntax> Content
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _content);
				return new SyntaxList<XmlNodeSyntax>(redAtZero);
			}
		}

		internal DocumentationCommentTriviaSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DocumentationCommentTriviaSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode content)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax(kind, errors, annotations, content?.Green), null, 0)
		{
		}

		public DocumentationCommentTriviaSyntax WithContent(SyntaxList<XmlNodeSyntax> content)
		{
			return Update(content);
		}

		public DocumentationCommentTriviaSyntax AddContent(params XmlNodeSyntax[] items)
		{
			return WithContent(Content.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _content;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return GetRedAtZero(ref _content);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitDocumentationCommentTrivia(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDocumentationCommentTrivia(this);
		}

		public DocumentationCommentTriviaSyntax Update(SyntaxList<XmlNodeSyntax> content)
		{
			if (content != Content)
			{
				DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax = SyntaxFactory.DocumentationCommentTrivia(content);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(documentationCommentTriviaSyntax, annotations);
				}
				return documentationCommentTriviaSyntax;
			}
			return this;
		}

		internal string GetInteriorXml()
		{
			return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax)base.Green).GetInteriorXml();
		}
	}
}
