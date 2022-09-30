using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class XmlMemberAccessExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _base;

		internal XmlNodeSyntax _name;

		public ExpressionSyntax Base => GetRedAtZero(ref _base);

		public SyntaxToken Token1 => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)base.Green)._token1, GetChildPosition(1), GetChildIndex(1));

		public SyntaxToken Token2
		{
			get
			{
				PunctuationSyntax token = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)base.Green)._token2;
				return (token == null) ? default(SyntaxToken) : new SyntaxToken(this, token, GetChildPosition(2), GetChildIndex(2));
			}
		}

		public SyntaxToken Token3
		{
			get
			{
				PunctuationSyntax token = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax)base.Green)._token3;
				return (token == null) ? default(SyntaxToken) : new SyntaxToken(this, token, GetChildPosition(3), GetChildIndex(3));
			}
		}

		public XmlNodeSyntax Name => GetRed(ref _name, 4);

		internal XmlMemberAccessExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal XmlMemberAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax(kind, errors, annotations, (@base != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)@base.Green) : null, token1, token2, token3, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax)name.Green), null, 0)
		{
		}

		public XmlMemberAccessExpressionSyntax WithBase(ExpressionSyntax @base)
		{
			return Update(Kind(), @base, Token1, Token2, Token3, Name);
		}

		public XmlMemberAccessExpressionSyntax WithToken1(SyntaxToken token1)
		{
			return Update(Kind(), Base, token1, Token2, Token3, Name);
		}

		public XmlMemberAccessExpressionSyntax WithToken2(SyntaxToken token2)
		{
			return Update(Kind(), Base, Token1, token2, Token3, Name);
		}

		public XmlMemberAccessExpressionSyntax WithToken3(SyntaxToken token3)
		{
			return Update(Kind(), Base, Token1, Token2, token3, Name);
		}

		public XmlMemberAccessExpressionSyntax WithName(XmlNodeSyntax name)
		{
			return Update(Kind(), Base, Token1, Token2, Token3, name);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _base, 
				4 => _name, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Base, 
				4 => Name, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitXmlMemberAccessExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitXmlMemberAccessExpression(this);
		}

		public XmlMemberAccessExpressionSyntax Update(SyntaxKind kind, ExpressionSyntax @base, SyntaxToken token1, SyntaxToken token2, SyntaxToken token3, XmlNodeSyntax name)
		{
			if (kind != Kind() || @base != Base || token1 != Token1 || token2 != Token2 || token3 != Token3 || name != Name)
			{
				XmlMemberAccessExpressionSyntax xmlMemberAccessExpressionSyntax = SyntaxFactory.XmlMemberAccessExpression(kind, @base, token1, token2, token3, name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(xmlMemberAccessExpressionSyntax, annotations);
				}
				return xmlMemberAccessExpressionSyntax;
			}
			return this;
		}
	}
}
