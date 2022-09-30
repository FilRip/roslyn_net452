using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class UsingStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _expression;

		internal SyntaxNode _variables;

		public SyntaxToken UsingKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)base.Green)._usingKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		public SeparatedSyntaxList<VariableDeclaratorSyntax> Variables
		{
			get
			{
				SyntaxNode red = GetRed(ref _variables, 2);
				return (red == null) ? default(SeparatedSyntaxList<VariableDeclaratorSyntax>) : new SeparatedSyntaxList<VariableDeclaratorSyntax>(red, GetChildIndex(2));
			}
		}

		internal UsingStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal UsingStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax usingKeyword, ExpressionSyntax expression, SyntaxNode variables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(kind, errors, annotations, usingKeyword, (expression != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green) : null, variables?.Green), null, 0)
		{
		}

		public UsingStatementSyntax WithUsingKeyword(SyntaxToken usingKeyword)
		{
			return Update(usingKeyword, Expression, Variables);
		}

		public UsingStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(UsingKeyword, expression, Variables);
		}

		public UsingStatementSyntax WithVariables(SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
		{
			return Update(UsingKeyword, Expression, variables);
		}

		public UsingStatementSyntax AddVariables(params VariableDeclaratorSyntax[] items)
		{
			return WithVariables(Variables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _expression, 
				2 => _variables, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => Expression, 
				2 => GetRed(ref _variables, 2), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitUsingStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitUsingStatement(this);
		}

		public UsingStatementSyntax Update(SyntaxToken usingKeyword, ExpressionSyntax expression, SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
		{
			if (usingKeyword != UsingKeyword || expression != Expression || variables != Variables)
			{
				UsingStatementSyntax usingStatementSyntax = SyntaxFactory.UsingStatement(usingKeyword, expression, variables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(usingStatementSyntax, annotations);
				}
				return usingStatementSyntax;
			}
			return this;
		}
	}
}
