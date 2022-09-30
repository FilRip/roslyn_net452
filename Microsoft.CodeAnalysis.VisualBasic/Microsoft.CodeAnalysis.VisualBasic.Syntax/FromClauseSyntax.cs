using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class FromClauseSyntax : QueryClauseSyntax
	{
		internal SyntaxNode _variables;

		public SyntaxToken FromKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax)base.Green)._fromKeyword, base.Position, 0);

		public SeparatedSyntaxList<CollectionRangeVariableSyntax> Variables
		{
			get
			{
				SyntaxNode red = GetRed(ref _variables, 1);
				return (red == null) ? default(SeparatedSyntaxList<CollectionRangeVariableSyntax>) : new SeparatedSyntaxList<CollectionRangeVariableSyntax>(red, GetChildIndex(1));
			}
		}

		internal FromClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal FromClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax fromKeyword, SyntaxNode variables)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(kind, errors, annotations, fromKeyword, variables?.Green), null, 0)
		{
		}

		public FromClauseSyntax WithFromKeyword(SyntaxToken fromKeyword)
		{
			return Update(fromKeyword, Variables);
		}

		public FromClauseSyntax WithVariables(SeparatedSyntaxList<CollectionRangeVariableSyntax> variables)
		{
			return Update(FromKeyword, variables);
		}

		public FromClauseSyntax AddVariables(params CollectionRangeVariableSyntax[] items)
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
			return visitor.VisitFromClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitFromClause(this);
		}

		public FromClauseSyntax Update(SyntaxToken fromKeyword, SeparatedSyntaxList<CollectionRangeVariableSyntax> variables)
		{
			if (fromKeyword != FromKeyword || variables != Variables)
			{
				FromClauseSyntax fromClauseSyntax = SyntaxFactory.FromClause(fromKeyword, variables);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(fromClauseSyntax, annotations);
				}
				return fromClauseSyntax;
			}
			return this;
		}
	}
}
