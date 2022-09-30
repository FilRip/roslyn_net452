using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ElseIfBlockSyntax : VisualBasicSyntaxNode
	{
		internal ElseIfStatementSyntax _elseIfStatement;

		internal SyntaxNode _statements;

		public ElseIfStatementSyntax ElseIfStatement => GetRedAtZero(ref _elseIfStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		internal ElseIfBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ElseIfBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ElseIfStatementSyntax elseIfStatement, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax)elseIfStatement.Green, statements?.Green), null, 0)
		{
		}

		public ElseIfBlockSyntax WithElseIfStatement(ElseIfStatementSyntax elseIfStatement)
		{
			return Update(elseIfStatement, Statements);
		}

		public ElseIfBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(ElseIfStatement, statements);
		}

		public ElseIfBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _elseIfStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ElseIfStatement, 
				1 => GetRed(ref _statements, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitElseIfBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitElseIfBlock(this);
		}

		public ElseIfBlockSyntax Update(ElseIfStatementSyntax elseIfStatement, SyntaxList<StatementSyntax> statements)
		{
			if (elseIfStatement != ElseIfStatement || statements != Statements)
			{
				ElseIfBlockSyntax elseIfBlockSyntax = SyntaxFactory.ElseIfBlock(elseIfStatement, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(elseIfBlockSyntax, annotations);
				}
				return elseIfBlockSyntax;
			}
			return this;
		}
	}
}
