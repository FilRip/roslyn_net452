using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LetClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _variables;

		public SyntaxToken LetKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax)base.Green)._letKeyword, base.Position, 0);

		public SeparatedSyntaxList<ExpressionRangeVariableSyntax> Variables
		{
			get
			{
				SyntaxNode red = GetRed(ref _variables, 1);
				return (red == null) ? default(SeparatedSyntaxList<ExpressionRangeVariableSyntax>) : new SeparatedSyntaxList<ExpressionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		internal LetClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LetClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax letKeyword, SyntaxNode variables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax(kind, errors, annotations, letKeyword, variables?.Green), null, 0)
		{
		}

		public LetClauseSyntax WithLetKeyword(SyntaxToken letKeyword)
		{
			return Update(letKeyword, Variables);
		}

		public LetClauseSyntax WithVariables(SeparatedSyntaxList<ExpressionRangeVariableSyntax> variables)
		{
			return Update(LetKeyword, variables);
		}

		public LetClauseSyntax AddVariables(params ExpressionRangeVariableSyntax[] items)
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
			return visitor.VisitLetClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLetClause(this);
		}

		public LetClauseSyntax Update(SyntaxToken letKeyword, SeparatedSyntaxList<ExpressionRangeVariableSyntax> variables)
		{
			if (letKeyword != LetKeyword || variables != Variables)
			{
				LetClauseSyntax letClauseSyntax = SyntaxFactory.LetClause(letKeyword, variables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(letClauseSyntax, annotations);
				}
				return letClauseSyntax;
			}
			return this;
		}
	}
}
