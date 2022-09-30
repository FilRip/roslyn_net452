using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlNameAttributeSyntax : BaseXmlAttributeSyntax
	{
		internal XmlNameSyntax _name;

		internal IdentifierNameSyntax _reference;

		public XmlNameSyntax Name => GetRedAtZero(ref _name);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green)._equalsToken, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken StartQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green)._startQuoteToken, GetChildPosition(2), GetChildIndex(2));

		public IdentifierNameSyntax Reference => GetRed(ref _reference, 3);

		public SyntaxToken EndQuoteToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax)base.Green)._endQuoteToken, GetChildPosition(4), GetChildIndex(4));

		internal XmlNameAttributeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlNameAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)name.Green, equalsToken, startQuoteToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)reference.Green, endQuoteToken), null, 0)
		{
		}

		public XmlNameAttributeSyntax WithName(XmlNameSyntax name)
		{
			return Update(name, EqualsToken, StartQuoteToken, Reference, EndQuoteToken);
		}

		public XmlNameAttributeSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(Name, equalsToken, StartQuoteToken, Reference, EndQuoteToken);
		}

		public XmlNameAttributeSyntax WithStartQuoteToken(SyntaxToken startQuoteToken)
		{
			return Update(Name, EqualsToken, startQuoteToken, Reference, EndQuoteToken);
		}

		public XmlNameAttributeSyntax WithReference(IdentifierNameSyntax reference)
		{
			return Update(Name, EqualsToken, StartQuoteToken, reference, EndQuoteToken);
		}

		public XmlNameAttributeSyntax WithEndQuoteToken(SyntaxToken endQuoteToken)
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
			return visitor.VisitXmlNameAttribute(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlNameAttribute(this);
		}

		public XmlNameAttributeSyntax Update(XmlNameSyntax name, SyntaxToken equalsToken, SyntaxToken startQuoteToken, IdentifierNameSyntax reference, SyntaxToken endQuoteToken)
		{
			if (name != Name || equalsToken != EqualsToken || startQuoteToken != StartQuoteToken || reference != Reference || endQuoteToken != EndQuoteToken)
			{
				XmlNameAttributeSyntax xmlNameAttributeSyntax = SyntaxFactory.XmlNameAttribute(name, equalsToken, startQuoteToken, reference, endQuoteToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlNameAttributeSyntax, annotations);
				}
				return xmlNameAttributeSyntax;
			}
			return this;
		}
	}
}
