using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TypeOfExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		internal TypeSyntax _type;

		public SyntaxToken TypeOfKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax)base.Green)._typeOfKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax)base.Green)._operatorToken, GetChildPosition(2), GetChildIndex(2));

		public TypeSyntax Type => GetRed(ref _type, 3);

		internal TypeOfExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TypeOfExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(kind, errors, annotations, typeOfKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, operatorToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)type.Green), null, 0)
		{
		}

		public TypeOfExpressionSyntax WithTypeOfKeyword(SyntaxToken typeOfKeyword)
		{
			return Update(Kind(), typeOfKeyword, Expression, OperatorToken, Type);
		}

		public TypeOfExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Kind(), TypeOfKeyword, expression, OperatorToken, Type);
		}

		public TypeOfExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(Kind(), TypeOfKeyword, Expression, operatorToken, Type);
		}

		public TypeOfExpressionSyntax WithType(TypeSyntax type)
		{
			return Update(Kind(), TypeOfKeyword, Expression, OperatorToken, type);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _expression, 
				3 => _type, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Expression, 
				3 => Type, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTypeOfExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTypeOfExpression(this);
		}

		public TypeOfExpressionSyntax Update(SyntaxKind kind, SyntaxToken typeOfKeyword, ExpressionSyntax expression, SyntaxToken operatorToken, TypeSyntax type)
		{
			if (kind != Kind() || typeOfKeyword != TypeOfKeyword || expression != Expression || operatorToken != OperatorToken || type != Type)
			{
				TypeOfExpressionSyntax typeOfExpressionSyntax = SyntaxFactory.TypeOfExpression(kind, typeOfKeyword, expression, operatorToken, type);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(typeOfExpressionSyntax, annotations);
				}
				return typeOfExpressionSyntax;
			}
			return this;
		}
	}
}
