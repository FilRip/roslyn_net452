using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CrefSignatureSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _argumentTypes;

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)base.Green)._openParenToken, base.Position, 0);

		public SeparatedSyntaxList<CrefSignaturePartSyntax> ArgumentTypes
		{
			get
			{
				SyntaxNode red = GetRed(ref _argumentTypes, 1);
				return (red == null) ? default(SeparatedSyntaxList<CrefSignaturePartSyntax>) : new SeparatedSyntaxList<CrefSignaturePartSyntax>(red, GetChildIndex(1));
			}
		}

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax)base.Green)._closeParenToken, GetChildPosition(2), GetChildIndex(2));

		internal CrefSignatureSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CrefSignatureSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, PunctuationSyntax openParenToken, SyntaxNode argumentTypes, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax(kind, errors, annotations, openParenToken, argumentTypes?.Green, closeParenToken), null, 0)
		{
		}

		public CrefSignatureSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(openParenToken, ArgumentTypes, CloseParenToken);
		}

		public CrefSignatureSyntax WithArgumentTypes(SeparatedSyntaxList<CrefSignaturePartSyntax> argumentTypes)
		{
			return Update(OpenParenToken, argumentTypes, CloseParenToken);
		}

		public CrefSignatureSyntax AddArgumentTypes(params CrefSignaturePartSyntax[] items)
		{
			return WithArgumentTypes(ArgumentTypes.AddRange(items));
		}

		public CrefSignatureSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(OpenParenToken, ArgumentTypes, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _argumentTypes;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _argumentTypes, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCrefSignature(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCrefSignature(this);
		}

		public CrefSignatureSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<CrefSignaturePartSyntax> argumentTypes, SyntaxToken closeParenToken)
		{
			if (openParenToken != OpenParenToken || argumentTypes != ArgumentTypes || closeParenToken != CloseParenToken)
			{
				CrefSignatureSyntax crefSignatureSyntax = SyntaxFactory.CrefSignature(openParenToken, argumentTypes, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(crefSignatureSyntax, annotations);
				}
				return crefSignatureSyntax;
			}
			return this;
		}
	}
}
