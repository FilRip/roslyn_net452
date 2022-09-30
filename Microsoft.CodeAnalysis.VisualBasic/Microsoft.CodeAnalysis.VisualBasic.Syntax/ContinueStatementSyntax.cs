using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ContinueStatementSyntax : ExecutableStatementSyntax
	{
		public SyntaxToken ContinueKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)base.Green)._continueKeyword, base.Position, 0);

		public SyntaxToken BlockKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax)base.Green)._blockKeyword, GetChildPosition(1), GetChildIndex(1));

		internal ContinueStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ContinueStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax(kind, errors, annotations, continueKeyword, blockKeyword), null, 0)
		{
		}

		public ContinueStatementSyntax WithContinueKeyword(SyntaxToken continueKeyword)
		{
			return Update(Kind(), continueKeyword, BlockKeyword);
		}

		public ContinueStatementSyntax WithBlockKeyword(SyntaxToken blockKeyword)
		{
			return Update(Kind(), ContinueKeyword, blockKeyword);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitContinueStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitContinueStatement(this);
		}

		public ContinueStatementSyntax Update(SyntaxKind kind, SyntaxToken continueKeyword, SyntaxToken blockKeyword)
		{
			if (kind != Kind() || continueKeyword != ContinueKeyword || blockKeyword != BlockKeyword)
			{
				ContinueStatementSyntax continueStatementSyntax = SyntaxFactory.ContinueStatement(kind, continueKeyword, blockKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(continueStatementSyntax, annotations);
				}
				return continueStatementSyntax;
			}
			return this;
		}
	}
}
