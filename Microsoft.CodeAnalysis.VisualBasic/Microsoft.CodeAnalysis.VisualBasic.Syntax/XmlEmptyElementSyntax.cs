using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlEmptyElementSyntax : XmlNodeSyntax
	{
		internal XmlNodeSyntax _name;

		internal SyntaxNode _attributes;

		public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax)base.Green)._lessThanToken, base.Position, 0);

		public XmlNodeSyntax Name => GetRed(ref _name, 1);

		public SyntaxList<XmlNodeSyntax> Attributes
		{
			get
			{
				SyntaxNode red = GetRed(ref _attributes, 2);
				return new SyntaxList<XmlNodeSyntax>(red);
			}
		}

		public SyntaxToken SlashGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax)base.Green)._slashGreaterThanToken, GetChildPosition(3), GetChildIndex(3));

		internal XmlEmptyElementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlEmptyElementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlNodeSyntax name, SyntaxNode attributes, PunctuationSyntax slashGreaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax(kind, errors, annotations, lessThanToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)name.Green, attributes?.Green, slashGreaterThanToken), null, 0)
		{
		}

		public XmlEmptyElementSyntax WithLessThanToken(SyntaxToken lessThanToken)
		{
			return Update(lessThanToken, Name, Attributes, SlashGreaterThanToken);
		}

		public XmlEmptyElementSyntax WithName(XmlNodeSyntax name)
		{
			return Update(LessThanToken, name, Attributes, SlashGreaterThanToken);
		}

		public XmlEmptyElementSyntax WithAttributes(SyntaxList<XmlNodeSyntax> attributes)
		{
			return Update(LessThanToken, Name, attributes, SlashGreaterThanToken);
		}

		public XmlEmptyElementSyntax AddAttributes(params XmlNodeSyntax[] items)
		{
			return WithAttributes(Attributes.AddRange(items));
		}

		public XmlEmptyElementSyntax WithSlashGreaterThanToken(SyntaxToken slashGreaterThanToken)
		{
			return Update(LessThanToken, Name, Attributes, slashGreaterThanToken);
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
			return visitor.VisitXmlEmptyElement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlEmptyElement(this);
		}

		public XmlEmptyElementSyntax Update(SyntaxToken lessThanToken, XmlNodeSyntax name, SyntaxList<XmlNodeSyntax> attributes, SyntaxToken slashGreaterThanToken)
		{
			if (lessThanToken != LessThanToken || name != Name || attributes != Attributes || slashGreaterThanToken != SlashGreaterThanToken)
			{
				XmlEmptyElementSyntax xmlEmptyElementSyntax = SyntaxFactory.XmlEmptyElement(lessThanToken, name, attributes, slashGreaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlEmptyElementSyntax, annotations);
				}
				return xmlEmptyElementSyntax;
			}
			return this;
		}
	}
}
