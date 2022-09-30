using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class JoinConditionSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _left;

		internal ExpressionSyntax _right;

		public ExpressionSyntax Left => GetRedAtZero(ref _left);

		public SyntaxToken EqualsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax)base.Green)._equalsKeyword, GetChildPosition(1), GetChildIndex(1));

		public ExpressionSyntax Right => GetRed(ref _right, 2);

		internal JoinConditionSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal JoinConditionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)left.Green, equalsKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)right.Green), null, 0)
		{
		}

		public JoinConditionSyntax WithLeft(ExpressionSyntax left)
		{
			return Update(left, EqualsKeyword, Right);
		}

		public JoinConditionSyntax WithEqualsKeyword(SyntaxToken equalsKeyword)
		{
			return Update(Left, equalsKeyword, Right);
		}

		public JoinConditionSyntax WithRight(ExpressionSyntax right)
		{
			return Update(Left, EqualsKeyword, right);
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
			return visitor.VisitJoinCondition(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitJoinCondition(this);
		}

		public JoinConditionSyntax Update(ExpressionSyntax left, SyntaxToken equalsKeyword, ExpressionSyntax right)
		{
			if (left != Left || equalsKeyword != EqualsKeyword || right != Right)
			{
				JoinConditionSyntax joinConditionSyntax = SyntaxFactory.JoinCondition(left, equalsKeyword, right);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(joinConditionSyntax, annotations);
				}
				return joinConditionSyntax;
			}
			return this;
		}
	}
}
