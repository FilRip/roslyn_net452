using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlElementEndTagSyntax : XmlNodeSyntax
	{
		internal XmlNameSyntax _name;

		public SyntaxToken LessThanSlashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)base.Green)._lessThanSlashToken, base.Position, 0);

		public XmlNameSyntax Name => GetRed(ref _name, 1);

		public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax)base.Green)._greaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlElementEndTagSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlElementEndTagSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax(kind, errors, annotations, lessThanSlashToken, (name != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax)name.Green) : null, greaterThanToken), null, 0)
		{
		}

		public XmlElementEndTagSyntax WithLessThanSlashToken(SyntaxToken lessThanSlashToken)
		{
			return Update(lessThanSlashToken, Name, GreaterThanToken);
		}

		public XmlElementEndTagSyntax WithName(XmlNameSyntax name)
		{
			return Update(LessThanSlashToken, name, GreaterThanToken);
		}

		public XmlElementEndTagSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
		{
			return Update(LessThanSlashToken, Name, greaterThanToken);
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
			return visitor.VisitXmlElementEndTag(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlElementEndTag(this);
		}

		public XmlElementEndTagSyntax Update(SyntaxToken lessThanSlashToken, XmlNameSyntax name, SyntaxToken greaterThanToken)
		{
			if (lessThanSlashToken != LessThanSlashToken || name != Name || greaterThanToken != GreaterThanToken)
			{
				XmlElementEndTagSyntax xmlElementEndTagSyntax = SyntaxFactory.XmlElementEndTag(lessThanSlashToken, name, greaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlElementEndTagSyntax, annotations);
				}
				return xmlElementEndTagSyntax;
			}
			return this;
		}
	}
}
