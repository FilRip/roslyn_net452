using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ElseBlockSyntax : VisualBasicSyntaxNode
	{
		internal ElseStatementSyntax _elseStatement;

		internal SyntaxNode _statements;

		public ElseStatementSyntax ElseStatement => GetRedAtZero(ref _elseStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		internal ElseBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ElseBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ElseStatementSyntax elseStatement, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax)elseStatement.Green, statements?.Green), null, 0)
		{
		}

		public ElseBlockSyntax WithElseStatement(ElseStatementSyntax elseStatement)
		{
			return Update(elseStatement, Statements);
		}

		public ElseBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(ElseStatement, statements);
		}

		public ElseBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _elseStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ElseStatement, 
				1 => GetRed(ref _statements, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitElseBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitElseBlock(this);
		}

		public ElseBlockSyntax Update(ElseStatementSyntax elseStatement, SyntaxList<StatementSyntax> statements)
		{
			if (elseStatement != ElseStatement || statements != Statements)
			{
				ElseBlockSyntax elseBlockSyntax = SyntaxFactory.ElseBlock(elseStatement, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(elseBlockSyntax, annotations);
				}
				return elseBlockSyntax;
			}
			return this;
		}
	}
}
