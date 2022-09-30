using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlCrefAttributeSyntax : BaseXmlAttributeSyntax
	{
		internal XmlNameSyntax _name;

		internal CrefReferenceSyntax _reference;

		public XmlNameSyntax Name => GetRedAtZero(ref _name);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax)base.Green)._equalsToken, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax)base.Green)._startQuoteToken, GetChildPosition(2), GetChildIndex(2));

		public CrefReferenceSyntax Reference => GetRed(ref _reference, 3);

		public SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax)base.Green)._endQuoteToken, GetChildPosition(4), GetChildIndex(4));

		internal XmlCrefAttributeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlCrefAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, CrefReferenceSyntax reference, PunctuationSyntax endQuoteToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)name.Green, equalsToken, startQuoteToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax)reference.Green, endQuoteToken), null, 0)
		{
		}

		public XmlCrefAttributeSyntax WithName(XmlNameSyntax name)
		{
			return Update(name, EqualsToken, StartQuoteToken, Reference, EndQuoteToken);
		}

		public XmlCrefAttributeSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(Name, equalsToken, StartQuoteToken, Reference, EndQuoteToken);
		}

		public XmlCrefAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken)
		{
			return Update(Name, EqualsToken, startQuoteToken, Reference, EndQuoteToken);
		}

		public XmlCrefAttributeSyntax WithReference(CrefReferenceSyntax reference)
		{
			return Update(Name, EqualsToken, StartQuoteToken, reference, EndQuoteToken);
		}

		public XmlCrefAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken)
		{
			return Update(Name, EqualsToken, StartQuoteToken, Reference, endQuoteToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				3 => _reference, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Name, 
				3 => Reference, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlCrefAttribute(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlCrefAttribute(this);
		}

		public XmlCrefAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, CrefReferenceSyntax reference, SyntaxToken endQuoteToken)
		{
			if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || reference != Reference || endQuoteToken != EndQuoteToken)
			{
				XmlCrefAttributeSyntax xmlCrefAttributeSyntax = SyntaxFactory.XmlCrefAttribute(name, equalsToken, startQuoteToken, reference, endQuoteToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlCrefAttributeSyntax, annotations);
				}
				return xmlCrefAttributeSyntax;
			}
			return this;
		}
	}
}
