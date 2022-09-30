using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ExpressionRangeVariableSyntax : VisualBasicSyntaxNode
	{
		internal VariableNameEqualsSyntax _nameEquals;

		internal ExpressionSyntax _expression;

		public VariableNameEqualsSyntax NameEquals => GetRedAtZero(ref _nameEquals);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal ExpressionRangeVariableSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ExpressionRangeVariableSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax(kind, errors, annotations, (nameEquals != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax)nameEquals.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public ExpressionRangeVariableSyntax WithNameEquals(VariableNameEqualsSyntax nameEquals)
		{
			return Update(nameEquals, Expression);
		}

		public ExpressionRangeVariableSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(NameEquals, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _nameEquals, 
				1 => _expression, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => NameEquals, 
				1 => Expression, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitExpressionRangeVariable(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitExpressionRangeVariable(this);
		}

		public ExpressionRangeVariableSyntax Update(VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
		{
			if (nameEquals != NameEquals || expression != Expression)
			{
				ExpressionRangeVariableSyntax expressionRangeVariableSyntax = SyntaxFactory.ExpressionRangeVariable(nameEquals, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(expressionRangeVariableSyntax, annotations);
				}
				return expressionRangeVariableSyntax;
			}
			return this;
		}
	}
}
