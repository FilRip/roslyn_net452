using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CatchBlockSyntax : VisualBasicSyntaxNode
	{
		internal CatchStatementSyntax _catchStatement;

		internal SyntaxNode _statements;

		public CatchStatementSyntax CatchStatement => GetRedAtZero(ref _catchStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		internal CatchBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CatchBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, CatchStatementSyntax catchStatement, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax)catchStatement.Green, statements?.Green), null, 0)
		{
		}

		public CatchBlockSyntax WithCatchStatement(CatchStatementSyntax catchStatement)
		{
			return Update(catchStatement, Statements);
		}

		public CatchBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(CatchStatement, statements);
		}

		public CatchBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _catchStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => CatchStatement, 
				1 => GetRed(ref _statements, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCatchBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCatchBlock(this);
		}

		public CatchBlockSyntax Update(CatchStatementSyntax catchStatement, SyntaxList<StatementSyntax> statements)
		{
			if (catchStatement != CatchStatement || statements != Statements)
			{
				CatchBlockSyntax catchBlockSyntax = SyntaxFactory.CatchBlock(catchStatement, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(catchBlockSyntax, annotations);
				}
				return catchBlockSyntax;
			}
			return this;
		}
	}
}
