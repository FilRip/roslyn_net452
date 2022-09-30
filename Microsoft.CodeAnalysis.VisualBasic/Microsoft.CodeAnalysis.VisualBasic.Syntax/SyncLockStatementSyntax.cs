using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SyncLockStatementSyntax : StatementSyntax
	{
		internal ExpressionSyntax _expression;

		public SyntaxToken SyncLockKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)base.Green)._syncLockKeyword, base.Position, 0);

		public ExpressionSyntax Expression => GetRed(ref _expression, 1);

		internal SyncLockStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SyncLockStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax syncLockKeyword, ExpressionSyntax expression)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax(kind, errors, annotations, syncLockKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)expression.Green), null, 0)
		{
		}

		public SyncLockStatementSyntax WithSyncLockKeyword(SyntaxToken syncLockKeyword)
		{
			return Update(syncLockKeyword, Expression);
		}

		public SyncLockStatementSyntax WithExpression(ExpressionSyntax expression)
		{
			return Update(SyncLockKeyword, expression);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _expression;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Expression;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSyncLockStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSyncLockStatement(this);
		}

		public SyncLockStatementSyntax Update(SyntaxToken syncLockKeyword, ExpressionSyntax expression)
		{
			if (syncLockKeyword != SyncLockKeyword || expression != Expression)
			{
				SyncLockStatementSyntax syncLockStatementSyntax = SyntaxFactory.SyncLockStatement(syncLockKeyword, expression);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(syncLockStatementSyntax, annotations);
				}
				return syncLockStatementSyntax;
			}
			return this;
		}
	}
}
