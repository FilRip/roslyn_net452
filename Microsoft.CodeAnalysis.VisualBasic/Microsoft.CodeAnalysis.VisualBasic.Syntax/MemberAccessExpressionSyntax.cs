using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		internal SimpleNameSyntax _name;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax)base.Green)._operatorToken, GetChildPosition(1), GetChildIndex(1));

		public SimpleNameSyntax Name => GetRed(ref _name, 2);

		internal MemberAccessExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MemberAccessExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(kind, errors, annotations, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null, operatorToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)name.Green), null, 0)
		{
		}

		public MemberAccessExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Kind(), expression, OperatorToken, Name);
		}

		public MemberAccessExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(Kind(), Expression, operatorToken, Name);
		}

		public MemberAccessExpressionSyntax WithName(SimpleNameSyntax name)
		{
			return Update(Kind(), Expression, OperatorToken, name);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				2 => _name, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Expression, 
				2 => Name, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMemberAccessExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMemberAccessExpression(this);
		}

		public MemberAccessExpressionSyntax Update(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
		{
			if (kind != Kind() || expression != Expression || operatorToken != OperatorToken || name != Name)
			{
				MemberAccessExpressionSyntax memberAccessExpressionSyntax = SyntaxFactory.MemberAccessExpression(kind, expression, operatorToken, name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(memberAccessExpressionSyntax, annotations);
				}
				return memberAccessExpressionSyntax;
			}
			return this;
		}
	}
}
