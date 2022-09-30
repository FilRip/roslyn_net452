using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ExitStatementSyntax : ExecutableStatementSyntax
	{
		public SyntaxToken ExitKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)base.Green)._exitKeyword, base.Position, 0);

		public SyntaxToken BlockKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax)base.Green)._blockKeyword, GetChildPosition(1), GetChildIndex(1));

		internal ExitStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ExitStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax(kind, errors, annotations, exitKeyword, blockKeyword), null, 0)
		{
		}

		public ExitStatementSyntax WithExitKeyword(SyntaxToken exitKeyword)
		{
			return Update(Kind(), exitKeyword, BlockKeyword);
		}

		public ExitStatementSyntax WithBlockKeyword(SyntaxToken blockKeyword)
		{
			return Update(Kind(), ExitKeyword, blockKeyword);
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
			return visitor.VisitExitStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitExitStatement(this);
		}

		public ExitStatementSyntax Update(SyntaxKind kind, SyntaxToken exitKeyword, SyntaxToken blockKeyword)
		{
			if (kind != Kind() || exitKeyword != ExitKeyword || blockKeyword != BlockKeyword)
			{
				ExitStatementSyntax exitStatementSyntax = SyntaxFactory.ExitStatement(kind, exitKeyword, blockKeyword);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(exitStatementSyntax, annotations);
				}
				return exitStatementSyntax;
			}
			return this;
		}
	}
}
