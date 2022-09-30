using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AssignmentStatementSyntax : ExecutableStatementSyntax
	{
		internal ExpressionSyntax _left;

		internal ExpressionSyntax _right;

		public ExpressionSyntax Left => GetRedAtZero(ref _left);

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax)base.Green)._operatorToken, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Right => GetRed(ref _right, 2);

		internal AssignmentStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AssignmentStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)left.Green, operatorToken, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)right.Green), null, 0)
		{
		}

		public AssignmentStatementSyntax WithLeft(ExpressionSyntax left)
		{
			return Update(Kind(), left, OperatorToken, Right);
		}

		public AssignmentStatementSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(Kind(), Left, operatorToken, Right);
		}

		public AssignmentStatementSyntax WithRight(ExpressionSyntax right)
		{
			return Update(Kind(), Left, OperatorToken, right);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _left, 
				2 => _right, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Left, 
				2 => Right, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAssignmentStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAssignmentStatement(this);
		}

		public AssignmentStatementSyntax Update(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			if (kind != Kind() || left != Left || operatorToken != OperatorToken || right != Right)
			{
				AssignmentStatementSyntax assignmentStatementSyntax = SyntaxFactory.AssignmentStatement(kind, left, operatorToken, right);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(assignmentStatementSyntax, annotations);
				}
				return assignmentStatementSyntax;
			}
			return this;
		}
	}
}
