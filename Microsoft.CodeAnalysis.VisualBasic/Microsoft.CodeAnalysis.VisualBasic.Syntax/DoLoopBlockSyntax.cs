using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DoLoopBlockSyntax : ExecutableStatementSyntax
	{
		internal DoStatementSyntax _doStatement;

		internal SyntaxNode _statements;

		internal LoopStatementSyntax _loopStatement;

		public DoStatementSyntax DoStatement => GetRedAtZero(ref _doStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public LoopStatementSyntax LoopStatement => GetRed(ref _loopStatement, 2);

		internal DoLoopBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DoLoopBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, DoStatementSyntax doStatement, SyntaxNode statements, LoopStatementSyntax loopStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax)doStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax)loopStatement.Green), null, 0)
		{
		}

		public DoLoopBlockSyntax WithDoStatement(DoStatementSyntax doStatement)
		{
			return Update(Kind(), doStatement, Statements, LoopStatement);
		}

		public DoLoopBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(Kind(), DoStatement, statements, LoopStatement);
		}

		public DoLoopBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public DoLoopBlockSyntax WithLoopStatement(LoopStatementSyntax loopStatement)
		{
			return Update(Kind(), DoStatement, Statements, loopStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _doStatement, 
				1 => _statements, 
				2 => _loopStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => DoStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => LoopStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitDoLoopBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDoLoopBlock(this);
		}

		public DoLoopBlockSyntax Update(SyntaxKind kind, DoStatementSyntax doStatement, SyntaxList<StatementSyntax> statements, LoopStatementSyntax loopStatement)
		{
			if (kind != Kind() || doStatement != DoStatement || statements != Statements || loopStatement != LoopStatement)
			{
				DoLoopBlockSyntax doLoopBlockSyntax = SyntaxFactory.DoLoopBlock(kind, doStatement, statements, loopStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(doLoopBlockSyntax, annotations);
				}
				return doLoopBlockSyntax;
			}
			return this;
		}
	}
}
