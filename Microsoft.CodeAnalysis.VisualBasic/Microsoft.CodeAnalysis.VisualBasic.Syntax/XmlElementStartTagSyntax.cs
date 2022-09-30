using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlElementStartTagSyntax : XmlNodeSyntax
	{
		internal XmlNodeSyntax _name;

		internal SyntaxNode _attributes;

		public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)base.Green)._lessThanToken, base.Position, 0);

		public XmlNodeSyntax Name => GetRed(ref _name, 1);

		public SyntaxList<XmlNodeSyntax> Attributes
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributes, 2);
				return new SyntaxList<XmlNodeSyntax>(red);
			}
		}

		public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax)base.Green)._greaterThanToken, GetChildPosition(3), GetChildIndex(3));

		internal XmlElementStartTagSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlElementStartTagSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlNodeSyntax name, SyntaxNode attributes, PunctuationSyntax greaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax(kind, errors, annotations, lessThanToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)name.Green, attributes?.Green, greaterThanToken), null, 0)
		{
		}

		public XmlElementStartTagSyntax WithLessThanToken(SyntaxToken lessThanToken)
		{
			return Update(lessThanToken, Name, Attributes, GreaterThanToken);
		}

		public XmlElementStartTagSyntax WithName(XmlNodeSyntax name)
		{
			return Update(LessThanToken, name, Attributes, GreaterThanToken);
		}

		public XmlElementStartTagSyntax WithAttributes(SyntaxList<XmlNodeSyntax> attributes)
		{
			return Update(LessThanToken, Name, attributes, GreaterThanToken);
		}

		public XmlElementStartTagSyntax AddAttributes(params XmlNodeSyntax[] items)
		{
			return WithAttributes(Attributes.AddRange(items));
		}

		public XmlElementStartTagSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
		{
			return Update(LessThanToken, Name, Attributes, greaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _name, 
				2 => _attributes, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Name, 
				2 => GetRed(ref _attributes, 2), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlElementStartTag(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlElementStartTag(this);
		}

		public XmlElementStartTagSyntax Update(SyntaxToken lessThanToken, XmlNodeSyntax name, SyntaxList<XmlNodeSyntax> attributes, SyntaxToken greaterThanToken)
		{
			if (lessThanToken != LessThanToken || name != Name || attributes != Attributes || greaterThanToken != GreaterThanToken)
			{
				XmlElementStartTagSyntax xmlElementStartTagSyntax = SyntaxFactory.XmlElementStartTag(lessThanToken, name, attributes, greaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlElementStartTagSyntax, annotations);
				}
				return xmlElementStartTagSyntax;
			}
			return this;
		}
	}
}
