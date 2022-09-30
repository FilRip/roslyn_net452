using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ReturnStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken ReturnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax)base.Green)._returnKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal ReturnStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ReturnStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax returnKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax(kind, errors, annotations, returnKeyword, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null), null, 0)
		{
		}

		public ReturnStatementSyntax WithReturnKeyword(SyntaxToken returnKeyword)
		{
			return Update(returnKeyword, Expression);
		}

		public ReturnStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(ReturnKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitReturnStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitReturnStatement(this);
		}

		public ReturnStatementSyntax Update(SyntaxToken returnKeyword, ExpressionSyntax expression)
		{
			if (returnKeyword != ReturnKeyword || expression != Expression)
			{
				ReturnStatementSyntax returnStatementSyntax = SyntaxFactory.ReturnStatement(returnKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(returnStatementSyntax, annotations);
				}
				return returnStatementSyntax;
			}
			return this;
		}
	}
}
