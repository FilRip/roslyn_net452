using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WhileBlockSyntax : ExecutableStatementSyntax
	{
		internal WhileStatementSyntax _whileStatement;

		internal SyntaxNode _statements;

		internal EndBlockStatementSyntax _endWhileStatement;

		public WhileStatementSyntax WhileStatement => GetRedAtZero(ref _whileStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndWhileStatement => GetRed(ref _endWhileStatement, 2);

		internal WhileBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WhileBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WhileStatementSyntax whileStatement, SyntaxNode statements, EndBlockStatementSyntax endWhileStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax)whileStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endWhileStatement.Green), null, 0)
		{
		}

		public WhileBlockSyntax WithWhileStatement(WhileStatementSyntax whileStatement)
		{
			return Update(whileStatement, Statements, EndWhileStatement);
		}

		public WhileBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(WhileStatement, statements, EndWhileStatement);
		}

		public WhileBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public WhileBlockSyntax WithEndWhileStatement(EndBlockStatementSyntax endWhileStatement)
		{
			return Update(WhileStatement, Statements, endWhileStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _whileStatement, 
				1 => _statements, 
				2 => _endWhileStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => WhileStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndWhileStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitWhileBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWhileBlock(this);
		}

		public WhileBlockSyntax Update(WhileStatementSyntax whileStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endWhileStatement)
		{
			if (whileStatement != WhileStatement || statements != Statements || endWhileStatement != EndWhileStatement)
			{
				WhileBlockSyntax whileBlockSyntax = SyntaxFactory.WhileBlock(whileStatement, statements, endWhileStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(whileBlockSyntax, annotations);
				}
				return whileBlockSyntax;
			}
			return this;
		}
	}
}
