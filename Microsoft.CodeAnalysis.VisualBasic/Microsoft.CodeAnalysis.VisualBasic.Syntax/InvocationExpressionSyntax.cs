using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class InvocationExpressionSyntax : ExpressionSyntax
	{
		internal ExpressionSyntax _expression;

		internal ArgumentListSyntax _argumentList;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		public ArgumentListSyntax ArgumentList => GetRed(ref _argumentList, 1);

		internal InvocationExpressionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal InvocationExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, ArgumentListSyntax argumentList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax(kind, errors, annotations, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null, (argumentList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)argumentList.Green) : null), null, 0)
		{
		}

		public InvocationExpressionSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(expression, ArgumentList);
		}

		public InvocationExpressionSyntax WithArgumentList(ArgumentListSyntax argumentList)
		{
			return Update(Expression, argumentList);
		}

		public InvocationExpressionSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArgumentList != null) ? ArgumentList : SyntaxFactory.ArgumentList());
			return WithArgumentList(argumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _argumentList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Expression, 
				1 => ArgumentList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitInvocationExpression(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitInvocationExpression(this);
		}

		public InvocationExpressionSyntax Update(ExpressionSyntax expression, ArgumentListSyntax argumentList)
		{
			if (expression != Expression || argumentList != ArgumentList)
			{
				InvocationExpressionSyntax invocationExpressionSyntax = SyntaxFactory.InvocationExpression(expression, argumentList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(invocationExpressionSyntax, annotations);
				}
				return invocationExpressionSyntax;
			}
			return this;
		}
	}
}
