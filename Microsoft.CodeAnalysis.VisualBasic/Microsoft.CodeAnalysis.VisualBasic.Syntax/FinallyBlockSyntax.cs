using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class FinallyBlockSyntax : VisualBasicSyntaxNode
	{
		internal FinallyStatementSyntax _finallyStatement;

		internal SyntaxNode _statements;

		public FinallyStatementSyntax FinallyStatement => GetRedAtZero(ref _finallyStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		internal FinallyBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal FinallyBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, FinallyStatementSyntax finallyStatement, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)finallyStatement.Green, statements?.Green), null, 0)
		{
		}

		public FinallyBlockSyntax WithFinallyStatement(FinallyStatementSyntax finallyStatement)
		{
			return Update(finallyStatement, Statements);
		}

		public FinallyBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(FinallyStatement, statements);
		}

		public FinallyBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _finallyStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => FinallyStatement, 
				1 => GetRed(ref _statements, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitFinallyBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitFinallyBlock(this);
		}

		public FinallyBlockSyntax Update(FinallyStatementSyntax finallyStatement, SyntaxList<StatementSyntax> statements)
		{
			if (finallyStatement != FinallyStatement || statements != Statements)
			{
				FinallyBlockSyntax finallyBlockSyntax = SyntaxFactory.FinallyBlock(finallyStatement, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(finallyBlockSyntax, annotations);
				}
				return finallyBlockSyntax;
			}
			return this;
		}
	}
}
