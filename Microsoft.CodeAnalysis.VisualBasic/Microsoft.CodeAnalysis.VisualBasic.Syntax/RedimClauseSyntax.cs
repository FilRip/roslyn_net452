using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class RedimClauseSyntax : VisualBasicSyntaxNode
	{
		internal ExpressionSyntax _expression;

		internal ArgumentListSyntax _arrayBounds;

		public ExpressionSyntax Expression => GetRedAtZero(ref _expression);

		public ArgumentListSyntax ArrayBounds => GetRed(ref _arrayBounds, 1);

		internal RedimClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal RedimClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)arrayBounds.Green), null, 0)
		{
		}

		public RedimClauseSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(expression, ArrayBounds);
		}

		public RedimClauseSyntax WithArrayBounds(ArgumentListSyntax arrayBounds)
		{
			return Update(Expression, arrayBounds);
		}

		public RedimClauseSyntax AddArrayBoundsArguments(params ArgumentSyntax[] items)
		{
			ArgumentListSyntax argumentListSyntax = ((ArrayBounds != null) ? ArrayBounds : SyntaxFactory.ArgumentList());
			return WithArrayBounds(argumentListSyntax.AddArguments(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _expression, 
				1 => _arrayBounds, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Expression, 
				1 => ArrayBounds, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitRedimClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitRedimClause(this);
		}

		public RedimClauseSyntax Update(ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
		{
			if (expression != Expression || arrayBounds != ArrayBounds)
			{
				RedimClauseSyntax redimClauseSyntax = SyntaxFactory.RedimClause(expression, arrayBounds);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(redimClauseSyntax, annotations);
				}
				return redimClauseSyntax;
			}
			return this;
		}
	}
}
