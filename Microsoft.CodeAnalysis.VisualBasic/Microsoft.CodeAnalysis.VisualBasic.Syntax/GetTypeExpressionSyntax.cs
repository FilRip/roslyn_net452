using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class GetTypeExpressionSyntax : ExpressionSyntax
	{
		internal TypeSyntax _type;

		public SyntaxToken GetTypeKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax)base.Green)._getTypeKeyword, base.Position, 0);

		public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax)base.Green)._openParenToken, GetChildPosition(1), GetChildIndex(1));

		public TypeSyntax Type => GetRed(ref _type, 2);

		public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax)base.Green)._closeParenToken, GetChildPosition(3), GetChildIndex(3));

		internal GetTypeExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal GetTypeExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax(kind, errors, annotations, getTypeKeyword, openParenToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green, closeParenToken), null, 0)
		{
		}

		public GetTypeExpressionSyntax WithGetTypeKeyword(SyntaxToken getTypeKeyword)
		{
			return Update(getTypeKeyword, OpenParenToken, Type, CloseParenToken);
		}

		public GetTypeExpressionSyntax WithOpenParenToken(SyntaxToken openParenToken)
		{
			return Update(GetTypeKeyword, openParenToken, Type, CloseParenToken);
		}

		public GetTypeExpressionSyntax WithType(TypeSyntax type)
		{
			return Update(GetTypeKeyword, OpenParenToken, type, CloseParenToken);
		}

		public GetTypeExpressionSyntax WithCloseParenToken(SyntaxToken closeParenToken)
		{
			return Update(GetTypeKeyword, OpenParenToken, Type, closeParenToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _type;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return Type;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitGetTypeExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitGetTypeExpression(this);
		}

		public GetTypeExpressionSyntax Update(SyntaxToken getTypeKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken closeParenToken)
		{
			if (getTypeKeyword != GetTypeKeyword || openParenToken != OpenParenToken || type != Type || closeParenToken != CloseParenToken)
			{
				GetTypeExpressionSyntax typeExpression = SyntaxFactory.GetTypeExpression(getTypeKeyword, openParenToken, type, closeParenToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeExpression, annotations);
				}
				return typeExpression;
			}
			return this;
		}
	}
}
