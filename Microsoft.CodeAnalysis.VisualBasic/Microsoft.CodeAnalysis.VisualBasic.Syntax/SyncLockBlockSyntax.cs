using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SyncLockBlockSyntax : ExecutableStatementSyntax
	{
		internal SyncLockStatementSyntax _syncLockStatement;

		internal SyntaxNode _statements;

		internal EndBlockStatementSyntax _endSyncLockStatement;

		public SyncLockStatementSyntax SyncLockStatement => GetRedAtZero(ref _syncLockStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndSyncLockStatement => GetRed(ref _endSyncLockStatement, 2);

		internal SyncLockBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SyncLockBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyncLockStatementSyntax syncLockStatement, SyntaxNode statements, EndBlockStatementSyntax endSyncLockStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)syncLockStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endSyncLockStatement.Green), null, 0)
		{
		}

		public SyncLockBlockSyntax WithSyncLockStatement(SyncLockStatementSyntax syncLockStatement)
		{
			return Update(syncLockStatement, Statements, EndSyncLockStatement);
		}

		public SyncLockBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(SyncLockStatement, statements, EndSyncLockStatement);
		}

		public SyncLockBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public SyncLockBlockSyntax WithEndSyncLockStatement(EndBlockStatementSyntax endSyncLockStatement)
		{
			return Update(SyncLockStatement, Statements, endSyncLockStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _syncLockStatement, 
				1 => _statements, 
				2 => _endSyncLockStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SyncLockStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndSyncLockStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSyncLockBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSyncLockBlock(this);
		}

		public SyncLockBlockSyntax Update(SyncLockStatementSyntax syncLockStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endSyncLockStatement)
		{
			if (syncLockStatement != SyncLockStatement || statements != Statements || endSyncLockStatement != EndSyncLockStatement)
			{
				SyncLockBlockSyntax syncLockBlockSyntax = SyntaxFactory.SyncLockBlock(syncLockStatement, statements, endSyncLockStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(syncLockBlockSyntax, annotations);
				}
				return syncLockBlockSyntax;
			}
			return this;
		}
	}
}
