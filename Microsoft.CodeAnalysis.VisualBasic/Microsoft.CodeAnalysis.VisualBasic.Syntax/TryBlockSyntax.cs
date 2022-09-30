using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class TryBlockSyntax : ExecutableStatementSyntax
	{
		internal TryStatementSyntax _tryStatement;

		internal SyntaxNode _statements;

		internal SyntaxNode _catchBlocks;

		internal FinallyBlockSyntax _finallyBlock;

		internal EndBlockStatementSyntax _endTryStatement;

		public TryStatementSyntax TryStatement => GetRedAtZero(ref _tryStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public SyntaxList<CatchBlockSyntax> CatchBlocks
		{
			get
			{
				SyntaxNode red = GetRed(ref _catchBlocks, 2);
				return new SyntaxList<CatchBlockSyntax>(red);
			}
		}

		public FinallyBlockSyntax FinallyBlock => GetRed(ref _finallyBlock, 3);

		public EndBlockStatementSyntax EndTryStatement => GetRed(ref _endTryStatement, 4);

		internal TryBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal TryBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, TryStatementSyntax tryStatement, SyntaxNode statements, SyntaxNode catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)tryStatement.Green, statements?.Green, catchBlocks?.Green, (finallyBlock != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax)finallyBlock.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endTryStatement.Green), null, 0)
		{
		}

		public TryBlockSyntax WithTryStatement(TryStatementSyntax tryStatement)
		{
			return Update(tryStatement, Statements, CatchBlocks, FinallyBlock, EndTryStatement);
		}

		public TryBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(TryStatement, statements, CatchBlocks, FinallyBlock, EndTryStatement);
		}

		public TryBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public TryBlockSyntax WithCatchBlocks(SyntaxList<CatchBlockSyntax> catchBlocks)
		{
			return Update(TryStatement, Statements, catchBlocks, FinallyBlock, EndTryStatement);
		}

		public TryBlockSyntax AddCatchBlocks(params CatchBlockSyntax[] items)
		{
			return WithCatchBlocks(CatchBlocks.AddRange(items));
		}

		public TryBlockSyntax WithFinallyBlock(FinallyBlockSyntax finallyBlock)
		{
			return Update(TryStatement, Statements, CatchBlocks, finallyBlock, EndTryStatement);
		}

		public TryBlockSyntax AddFinallyBlockStatements(params StatementSyntax[] items)
		{
			FinallyBlockSyntax finallyBlockSyntax = ((FinallyBlock != null) ? FinallyBlock : SyntaxFactory.FinallyBlock());
			return WithFinallyBlock(finallyBlockSyntax.AddStatements(items));
		}

		public TryBlockSyntax WithEndTryStatement(EndBlockStatementSyntax endTryStatement)
		{
			return Update(TryStatement, Statements, CatchBlocks, FinallyBlock, endTryStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _tryStatement, 
				1 => _statements, 
				2 => _catchBlocks, 
				3 => _finallyBlock, 
				4 => _endTryStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => TryStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => GetRed(ref _catchBlocks, 2), 
				3 => FinallyBlock, 
				4 => EndTryStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitTryBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitTryBlock(this);
		}

		public TryBlockSyntax Update(TryStatementSyntax tryStatement, SyntaxList<StatementSyntax> statements, SyntaxList<CatchBlockSyntax> catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
		{
			if (tryStatement != TryStatement || statements != Statements || catchBlocks != CatchBlocks || finallyBlock != FinallyBlock || endTryStatement != EndTryStatement)
			{
				TryBlockSyntax tryBlockSyntax = SyntaxFactory.TryBlock(tryStatement, statements, catchBlocks, finallyBlock, endTryStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(tryBlockSyntax, annotations);
				}
				return tryBlockSyntax;
			}
			return this;
		}
	}
}
