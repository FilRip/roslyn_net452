using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CaseStatementSyntax : StatementSyntax
	{
		internal SyntaxNode _cases;

		public SyntaxToken CaseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)base.Green)._caseKeyword, base.Position, 0);

		public SeparatedSyntaxList<CaseClauseSyntax> Cases
		{
			get
			{
				SyntaxNode red = GetRed(ref _cases, 1);
				return (red == null) ? default(SeparatedSyntaxList<CaseClauseSyntax>) : new SeparatedSyntaxList<CaseClauseSyntax>(red, GetChildIndex(1));
			}
		}

		internal CaseStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CaseStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax caseKeyword, SyntaxNode cases)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax(kind, errors, annotations, caseKeyword, cases?.Green), null, 0)
		{
		}

		public CaseStatementSyntax WithCaseKeyword(SyntaxToken caseKeyword)
		{
			return Update(Kind(), caseKeyword, Cases);
		}

		public CaseStatementSyntax WithCases(SeparatedSyntaxList<CaseClauseSyntax> cases)
		{
			return Update(Kind(), CaseKeyword, cases);
		}

		public CaseStatementSyntax AddCases(params CaseClauseSyntax[] items)
		{
			return WithCases(Cases.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _cases;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _cases, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCaseStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCaseStatement(this);
		}

		public CaseStatementSyntax Update(SyntaxKind kind, SyntaxToken caseKeyword, SeparatedSyntaxList<CaseClauseSyntax> cases)
		{
			if (kind != Kind() || caseKeyword != CaseKeyword || cases != Cases)
			{
				CaseStatementSyntax caseStatementSyntax = SyntaxFactory.CaseStatement(kind, caseKeyword, cases);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(caseStatementSyntax, annotations);
				}
				return caseStatementSyntax;
			}
			return this;
		}
	}
}
