using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlAttributeSyntax : BaseXmlAttributeSyntax
	{
		internal XmlNodeSyntax _name;

		internal XmlNodeSyntax _value;

		public XmlNodeSyntax Name => GetRedAtZero(ref _name);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)base.Green)._equalsToken, GetChildPosition(1), GetChildIndex(1));

		public XmlNodeSyntax Value => GetRed(ref _value, 2);

		internal XmlAttributeSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlAttributeSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)name.Green, equalsToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)value.Green), null, 0)
		{
		}

		public XmlAttributeSyntax WithName(XmlNodeSyntax name)
		{
			return Update(name, EqualsToken, Value);
		}

		public XmlAttributeSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(Name, equalsToken, Value);
		}

		public XmlAttributeSyntax WithValue(XmlNodeSyntax value)
		{
			return Update(Name, EqualsToken, value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _name, 
				2 => _value, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Name, 
				2 => Value, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlAttribute(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlAttribute(this);
		}

		public XmlAttributeSyntax Update(XmlNodeSyntax name, SyntaxToken equalsToken, XmlNodeSyntax value)
		{
			if (name != Name || equalsToken != EqualsToken || value != Value)
			{
				XmlAttributeSyntax xmlAttributeSyntax = SyntaxFactory.XmlAttribute(name, equalsToken, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlAttributeSyntax, annotations);
				}
				return xmlAttributeSyntax;
			}
			return this;
		}
	}
}
