using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlBracketedNameSyntax : XmlNodeSyntax
	{
		internal XmlNameSyntax _name;

		public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)base.Green)._lessThanToken, base.Position, 0);

		public XmlNameSyntax Name => GetRed(ref _name, 1);

		public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax)base.Green)._greaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlBracketedNameSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlBracketedNameSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax(kind, errors, annotations, lessThanToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)name.Green, greaterThanToken), null, 0)
		{
		}

		public XmlBracketedNameSyntax WithLessThanToken(SyntaxToken lessThanToken)
		{
			return Update(lessThanToken, Name, GreaterThanToken);
		}

		public XmlBracketedNameSyntax WithName(XmlNameSyntax name)
		{
			return Update(LessThanToken, name, GreaterThanToken);
		}

		public XmlBracketedNameSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
		{
			return Update(LessThanToken, Name, greaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _name;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Name;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlBracketedName(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlBracketedName(this);
		}

		public XmlBracketedNameSyntax Update(SyntaxToken lessThanToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
		{
			if (lessThanToken != LessThanToken || name != Name || greaterThanToken != GreaterThanToken)
			{
				XmlBracketedNameSyntax xmlBracketedNameSyntax = SyntaxFactory.XmlBracketedName(lessThanToken, name, greaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlBracketedNameSyntax, annotations);
				}
				return xmlBracketedNameSyntax;
			}
			return this;
		}
	}
}
