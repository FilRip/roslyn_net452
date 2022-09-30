using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SelectClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _variables;

		public SyntaxToken SelectKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax)base.Green)._selectKeyword, base.Position, 0);

		public SeparatedSyntaxList<ExpressionRangeVariableSyntax> Variables
		{
			get
			{
				SyntaxNode red = GetRed(ref _variables, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionRangeVariableSyntax>) : new SeparatedSyntaxList<ExpressionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		internal SelectClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SelectClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax selectKeyword, SyntaxNode variables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax(kind, errors, annotations, selectKeyword, variables?.Green), null, 0)
		{
		}

		public SelectClauseSyntax WithSelectKeyword(SyntaxToken selectKeyword)
		{
			return Update(selectKeyword, Variables);
		}

		public SelectClauseSyntax WithVariables(SeparatedSyntaxList<ExpressionRangeVariableSyntax> variables)
		{
			return Update(SelectKeyword, variables);
		}

		public SelectClauseSyntax AddVariables(params ExpressionRangeVariableSyntax[] items)
		{
			return WithVariables(Variables.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _variables;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _variables, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSelectClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSelectClause(this);
		}

		public SelectClauseSyntax Update(SyntaxToken selectKeyword, SeparatedSyntaxList<ExpressionRangeVariableSyntax> variables)
		{
			if (selectKeyword != SelectKeyword || variables != Variables)
			{
				SelectClauseSyntax selectClauseSyntax = SyntaxFactory.SelectClause(selectKeyword, variables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(selectClauseSyntax, annotations);
				}
				return selectClauseSyntax;
			}
			return this;
		}
	}
}
