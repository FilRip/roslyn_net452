using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OrderingSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _expression;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		public SyntaxToken AscendingOrDescendingKeyword
		{
			get
			{
				KeywordSyntax ascendingOrDescendingKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)base.Green)._ascendingOrDescendingKeyword;
				return (ascendingOrDescendingKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, ascendingOrDescendingKeyword, GetChildPosition(1), GetChildIndex(1));
			}
		}

		internal OrderingSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OrderingSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, ascendingOrDescendingKeyword), null, 0)
		{
		}

		public OrderingSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(Kind(), expression, AscendingOrDescendingKeyword);
		}

		public OrderingSyntax WithAscendingOrDescendingKeyword(SyntaxToken ascendingOrDescendingKeyword)
		{
			return Update(Kind(), Expression, ascendingOrDescendingKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOrdering(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOrdering(this);
		}

		public OrderingSyntax Update(SyntaxKind kind, ExpressionSyntax expression, SyntaxToken ascendingOrDescendingKeyword)
		{
			if (kind != Kind() || expression != Expression || ascendingOrDescendingKeyword != AscendingOrDescendingKeyword)
			{
				OrderingSyntax orderingSyntax = SyntaxFactory.Ordering(kind, expression, ascendingOrDescendingKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(orderingSyntax, annotations);
				}
				return orderingSyntax;
			}
			return this;
		}
	}
}
