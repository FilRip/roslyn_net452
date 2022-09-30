using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlDeclarationSyntax : VisualBasicSyntaxNode
	{
		internal XmlDeclarationOptionSyntax _version;

		internal XmlDeclarationOptionSyntax _encoding;

		internal XmlDeclarationOptionSyntax _standalone;

		public SyntaxToken LessThanQuestionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)base.Green)._lessThanQuestionToken, base.Position, 0);

		public SyntaxToken XmlKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)base.Green)._xmlKeyword, GetChildPosition(1), GetChildIndex(1));

		public XmlDeclarationOptionSyntax Version => GetRed(ref _version, 2);

		public XmlDeclarationOptionSyntax Encoding => GetRed(ref _encoding, 3);

		public XmlDeclarationOptionSyntax Standalone => GetRed(ref _standalone, 4);

		public SyntaxToken QuestionGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax)base.Green)._questionGreaterThanToken, GetChildPosition(5), GetChildIndex(5));

		internal XmlDeclarationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax(kind, errors, annotations, lessThanQuestionToken, xmlKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)version.Green, (encoding != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)encoding.Green) : null, (standalone != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax)standalone.Green) : null, questionGreaterThanToken), null, 0)
		{
		}

		public XmlDeclarationSyntax WithLessThanQuestionToken(SyntaxToken lessThanQuestionToken)
		{
			return Update(lessThanQuestionToken, XmlKeyword, Version, Encoding, Standalone, QuestionGreaterThanToken);
		}

		public XmlDeclarationSyntax WithXmlKeyword(SyntaxToken xmlKeyword)
		{
			return Update(LessThanQuestionToken, xmlKeyword, Version, Encoding, Standalone, QuestionGreaterThanToken);
		}

		public XmlDeclarationSyntax WithVersion(XmlDeclarationOptionSyntax version)
		{
			return Update(LessThanQuestionToken, XmlKeyword, version, Encoding, Standalone, QuestionGreaterThanToken);
		}

		public XmlDeclarationSyntax WithEncoding(XmlDeclarationOptionSyntax encoding)
		{
			return Update(LessThanQuestionToken, XmlKeyword, Version, encoding, Standalone, QuestionGreaterThanToken);
		}

		public XmlDeclarationSyntax WithStandalone(XmlDeclarationOptionSyntax standalone)
		{
			return Update(LessThanQuestionToken, XmlKeyword, Version, Encoding, standalone, QuestionGreaterThanToken);
		}

		public XmlDeclarationSyntax WithQuestionGreaterThanToken(SyntaxToken questionGreaterThanToken)
		{
			return Update(LessThanQuestionToken, XmlKeyword, Version, Encoding, Standalone, questionGreaterThanToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				2 => _version, 
				3 => _encoding, 
				4 => _standalone, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				2 => Version, 
				3 => Encoding, 
				4 => Standalone, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlDeclaration(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlDeclaration(this);
		}

		public XmlDeclarationSyntax Update(SyntaxToken lessThanQuestionToken, SyntaxToken xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, SyntaxToken questionGreaterThanToken)
		{
			if (lessThanQuestionToken != LessThanQuestionToken || xmlKeyword != XmlKeyword || version != Version || encoding != Encoding || standalone != Standalone || questionGreaterThanToken != QuestionGreaterThanToken)
			{
				XmlDeclarationSyntax xmlDeclarationSyntax = SyntaxFactory.XmlDeclaration(lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlDeclarationSyntax, annotations);
				}
				return xmlDeclarationSyntax;
			}
			return this;
		}
	}
}
