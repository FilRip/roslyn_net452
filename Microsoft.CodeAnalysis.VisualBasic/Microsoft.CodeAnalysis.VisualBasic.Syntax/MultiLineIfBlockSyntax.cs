using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MultiLineIfBlockSyntax : ExecutableStatementSyntax
	{
		internal IfStatementSyntax _ifStatement;

		internal SyntaxNode _statements;

		internal SyntaxNode _elseIfBlocks;

		internal ElseBlockSyntax _elseBlock;

		internal EndBlockStatementSyntax _endIfStatement;

		public IfStatementSyntax IfStatement => GetRedAtZero(ref _ifStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public SyntaxList<ElseIfBlockSyntax> ElseIfBlocks
		{
			get
			{
				SyntaxNode red = GetRed(ref _elseIfBlocks, 2);
				return new SyntaxList<ElseIfBlockSyntax>(red);
			}
		}

		public ElseBlockSyntax ElseBlock => GetRed(ref _elseBlock, 3);

		public EndBlockStatementSyntax EndIfStatement => GetRed(ref _endIfStatement, 4);

		internal MultiLineIfBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MultiLineIfBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IfStatementSyntax ifStatement, SyntaxNode statements, SyntaxNode elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)ifStatement.Green, statements?.Green, elseIfBlocks?.Green, (elseBlock != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax)elseBlock.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endIfStatement.Green), null, 0)
		{
		}

		public MultiLineIfBlockSyntax WithIfStatement(IfStatementSyntax ifStatement)
		{
			return Update(ifStatement, Statements, ElseIfBlocks, ElseBlock, EndIfStatement);
		}

		public MultiLineIfBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(IfStatement, statements, ElseIfBlocks, ElseBlock, EndIfStatement);
		}

		public MultiLineIfBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public MultiLineIfBlockSyntax WithElseIfBlocks(SyntaxList<ElseIfBlockSyntax> elseIfBlocks)
		{
			return Update(IfStatement, Statements, elseIfBlocks, ElseBlock, EndIfStatement);
		}

		public MultiLineIfBlockSyntax AddElseIfBlocks(params ElseIfBlockSyntax[] items)
		{
			return WithElseIfBlocks(ElseIfBlocks.AddRange(items));
		}

		public MultiLineIfBlockSyntax WithElseBlock(ElseBlockSyntax elseBlock)
		{
			return Update(IfStatement, Statements, ElseIfBlocks, elseBlock, EndIfStatement);
		}

		public MultiLineIfBlockSyntax AddElseBlockStatements(params StatementSyntax[] items)
		{
			ElseBlockSyntax elseBlockSyntax = ((ElseBlock != null) ? ElseBlock : SyntaxFactory.ElseBlock());
			return WithElseBlock(elseBlockSyntax.AddStatements(items));
		}

		public MultiLineIfBlockSyntax WithEndIfStatement(EndBlockStatementSyntax endIfStatement)
		{
			return Update(IfStatement, Statements, ElseIfBlocks, ElseBlock, endIfStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _ifStatement, 
				1 => _statements, 
				2 => _elseIfBlocks, 
				3 => _elseBlock, 
				4 => _endIfStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => IfStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => GetRed(ref _elseIfBlocks, 2), 
				3 => ElseBlock, 
				4 => EndIfStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMultiLineIfBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMultiLineIfBlock(this);
		}

		public MultiLineIfBlockSyntax Update(IfStatementSyntax ifStatement, SyntaxList<StatementSyntax> statements, SyntaxList<ElseIfBlockSyntax> elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
		{
			if (ifStatement != IfStatement || statements != Statements || elseIfBlocks != ElseIfBlocks || elseBlock != ElseBlock || endIfStatement != EndIfStatement)
			{
				MultiLineIfBlockSyntax multiLineIfBlockSyntax = SyntaxFactory.MultiLineIfBlock(ifStatement, statements, elseIfBlocks, elseBlock, endIfStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(multiLineIfBlockSyntax, annotations);
				}
				return multiLineIfBlockSyntax;
			}
			return this;
		}
	}
}
