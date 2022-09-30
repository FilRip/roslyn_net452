using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class UsingBlockSyntax : ExecutableStatementSyntax
	{
		internal UsingStatementSyntax _usingStatement;

		internal SyntaxNode _statements;

		internal EndBlockStatementSyntax _endUsingStatement;

		public UsingStatementSyntax UsingStatement => GetRedAtZero(ref _usingStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndUsingStatement => GetRed(ref _endUsingStatement, 2);

		internal UsingBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal UsingBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, UsingStatementSyntax usingStatement, SyntaxNode statements, EndBlockStatementSyntax endUsingStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)usingStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endUsingStatement.Green), null, 0)
		{
		}

		public UsingBlockSyntax WithUsingStatement(UsingStatementSyntax usingStatement)
		{
			return Update(usingStatement, Statements, EndUsingStatement);
		}

		public UsingBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(UsingStatement, statements, EndUsingStatement);
		}

		public UsingBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public UsingBlockSyntax WithEndUsingStatement(EndBlockStatementSyntax endUsingStatement)
		{
			return Update(UsingStatement, Statements, endUsingStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _usingStatement, 
				1 => _statements, 
				2 => _endUsingStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => UsingStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndUsingStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitUsingBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitUsingBlock(this);
		}

		public UsingBlockSyntax Update(UsingStatementSyntax usingStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endUsingStatement)
		{
			if (usingStatement != UsingStatement || statements != Statements || endUsingStatement != EndUsingStatement)
			{
				UsingBlockSyntax usingBlockSyntax = SyntaxFactory.UsingBlock(usingStatement, statements, endUsingStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(usingBlockSyntax, annotations);
				}
				return usingBlockSyntax;
			}
			return this;
		}
	}
}
