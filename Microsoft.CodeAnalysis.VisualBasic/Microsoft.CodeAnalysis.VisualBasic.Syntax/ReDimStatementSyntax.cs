using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ReDimStatementSyntax : ExecutableStatementSyntax
	{
		internal SyntaxNode _clauses;

		public SyntaxToken ReDimKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)base.Green)._reDimKeyword, base.Position, 0);

		public SyntaxToken PreserveKeyword
		{
			get
			{
				KeywordSyntax preserveKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax)base.Green)._preserveKeyword;
				return (preserveKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, preserveKeyword, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SeparatedSyntaxList<RedimClauseSyntax> Clauses
		{
			get
			{
				SyntaxNode red = GetRed(ref _clauses, 2);
				return (red == null) ? default(SeparatedSyntaxList<RedimClauseSyntax>) : new SeparatedSyntaxList<RedimClauseSyntax>(red, GetChildIndex(2));
			}
		}

		internal ReDimStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ReDimStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, SyntaxNode clauses)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(kind, errors, annotations, reDimKeyword, preserveKeyword, clauses?.Green), null, 0)
		{
		}

		public ReDimStatementSyntax WithReDimKeyword(SyntaxToken reDimKeyword)
		{
			return Update(Kind(), reDimKeyword, PreserveKeyword, Clauses);
		}

		public ReDimStatementSyntax WithPreserveKeyword(SyntaxToken preserveKeyword)
		{
			return Update(Kind(), ReDimKeyword, preserveKeyword, Clauses);
		}

		public ReDimStatementSyntax WithClauses(SeparatedSyntaxList<RedimClauseSyntax> clauses)
		{
			return Update(Kind(), ReDimKeyword, PreserveKeyword, clauses);
		}

		public ReDimStatementSyntax AddClauses(params RedimClauseSyntax[] items)
		{
			return WithClauses(Clauses.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 2)
			{
				return _clauses;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 2)
			{
				return GetRed(ref _clauses, 2);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitReDimStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitReDimStatement(this);
		}

		public ReDimStatementSyntax Update(SyntaxKind kind, SyntaxToken reDimKeyword, SyntaxToken preserveKeyword, SeparatedSyntaxList<RedimClauseSyntax> clauses)
		{
			if (kind != Kind() || reDimKeyword != ReDimKeyword || preserveKeyword != PreserveKeyword || clauses != Clauses)
			{
				ReDimStatementSyntax reDimStatementSyntax = SyntaxFactory.ReDimStatement(kind, reDimKeyword, preserveKeyword, clauses);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(reDimStatementSyntax, annotations);
				}
				return reDimStatementSyntax;
			}
			return this;
		}
	}
}
