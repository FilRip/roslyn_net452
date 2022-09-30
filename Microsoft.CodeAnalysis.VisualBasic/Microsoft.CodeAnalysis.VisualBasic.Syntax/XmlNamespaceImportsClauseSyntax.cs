using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlNamespaceImportsClauseSyntax : ImportsClauseSyntax
	{
		internal XmlAttributeSyntax _xmlNamespace;

		public SyntaxToken LessThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)base.Green)._lessThanToken, base.Position, 0);

		public XmlAttributeSyntax XmlNamespace => GetRed(ref _xmlNamespace, 1);

		public SyntaxToken GreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax)base.Green)._greaterThanToken, GetChildPosition(2), GetChildIndex(2));

		internal XmlNamespaceImportsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlNamespaceImportsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax(kind, errors, annotations, lessThanToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax)xmlNamespace.Green, greaterThanToken), null, 0)
		{
		}

		public XmlNamespaceImportsClauseSyntax WithLessThanToken(SyntaxToken lessThanToken)
		{
			return Update(lessThanToken, XmlNamespace, GreaterThanToken);
		}

		public XmlNamespaceImportsClauseSyntax WithXmlNamespace(XmlAttributeSyntax xmlNamespace)
		{
			return Update(LessThanToken, xmlNamespace, GreaterThanToken);
		}

		public XmlNamespaceImportsClauseSyntax WithGreaterThanToken(SyntaxToken greaterThanToken)
		{
			return Update(LessThanToken, XmlNamespace, greaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _xmlNamespace;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return XmlNamespace;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlNamespaceImportsClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlNamespaceImportsClause(this);
		}

		public XmlNamespaceImportsClauseSyntax Update(SyntaxToken lessThanToken, XmlAttributeSyntax xmlNamespace, SyntaxToken greaterThanToken)
		{
			if (lessThanToken != LessThanToken || xmlNamespace != XmlNamespace || greaterThanToken != GreaterThanToken)
			{
				XmlNamespaceImportsClauseSyntax xmlNamespaceImportsClauseSyntax = SyntaxFactory.XmlNamespaceImportsClause(lessThanToken, xmlNamespace, greaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlNamespaceImportsClauseSyntax, annotations);
				}
				return xmlNamespaceImportsClauseSyntax;
			}
			return this;
		}
	}
}
