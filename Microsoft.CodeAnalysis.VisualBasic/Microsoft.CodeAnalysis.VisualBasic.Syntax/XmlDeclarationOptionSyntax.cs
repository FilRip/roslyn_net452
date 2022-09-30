using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlDeclarationOptionSyntax : VisualBasicSyntaxNode
	{
		internal XmlStringSyntax _value;

		public SyntaxToken Name => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)base.Green)._name, base.Position, 0);

		public new SyntaxToken Equals => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)base.Green)._equals, GetChildPosition(1), GetChildIndex(1));

		public XmlStringSyntax Value => GetRed(ref _value, 2);

		internal XmlDeclarationOptionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlDeclarationOptionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax(kind, errors, annotations, name, equals, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax)value.Green), null, 0)
		{
		}

		public XmlDeclarationOptionSyntax WithName(SyntaxToken name)
		{
			return Update(name, Equals, Value);
		}

		public XmlDeclarationOptionSyntax WithEquals(SyntaxToken equals)
		{
			return Update(Name, equals, Value);
		}

		public XmlDeclarationOptionSyntax WithValue(XmlStringSyntax value)
		{
			return Update(Name, Equals, value);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _value;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Value;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlDeclarationOption(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlDeclarationOption(this);
		}

		public XmlDeclarationOptionSyntax Update(SyntaxToken name, SyntaxToken equals, XmlStringSyntax value)
		{
			if (name != Name || equals != Equals || value != Value)
			{
				XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = SyntaxFactory.XmlDeclarationOption(name, equals, value);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlDeclarationOptionSyntax, annotations);
				}
				return xmlDeclarationOptionSyntax;
			}
			return this;
		}
	}
}
