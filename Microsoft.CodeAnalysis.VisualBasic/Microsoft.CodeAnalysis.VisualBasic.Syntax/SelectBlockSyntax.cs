using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SelectBlockSyntax : ExecutableStatementSyntax
	{
		internal SelectStatementSyntax _selectStatement;

		internal SyntaxNode _caseBlocks;

		internal EndBlockStatementSyntax _endSelectStatement;

		public SelectStatementSyntax SelectStatement => GetRedAtZero(ref _selectStatement);

		public SyntaxList<CaseBlockSyntax> CaseBlocks
		{
			get
			{
				SyntaxNode red = GetRed(ref _caseBlocks, 1);
				return new SyntaxList<CaseBlockSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndSelectStatement => GetRed(ref _endSelectStatement, 2);

		internal SelectBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SelectBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SelectStatementSyntax selectStatement, SyntaxNode caseBlocks, EndBlockStatementSyntax endSelectStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax)selectStatement.Green, caseBlocks?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endSelectStatement.Green), null, 0)
		{
		}

		public SelectBlockSyntax WithSelectStatement(SelectStatementSyntax selectStatement)
		{
			return Update(selectStatement, CaseBlocks, EndSelectStatement);
		}

		public SelectBlockSyntax WithCaseBlocks(SyntaxList<CaseBlockSyntax> caseBlocks)
		{
			return Update(SelectStatement, caseBlocks, EndSelectStatement);
		}

		public SelectBlockSyntax AddCaseBlocks(params CaseBlockSyntax[] items)
		{
			return WithCaseBlocks(CaseBlocks.AddRange(items));
		}

		public SelectBlockSyntax WithEndSelectStatement(EndBlockStatementSyntax endSelectStatement)
		{
			return Update(SelectStatement, CaseBlocks, endSelectStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _selectStatement, 
				1 => _caseBlocks, 
				2 => _endSelectStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => SelectStatement, 
				1 => GetRed(ref _caseBlocks, 1), 
				2 => EndSelectStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSelectBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSelectBlock(this);
		}

		public SelectBlockSyntax Update(SelectStatementSyntax selectStatement, SyntaxList<CaseBlockSyntax> caseBlocks, EndBlockStatementSyntax endSelectStatement)
		{
			if (selectStatement != SelectStatement || caseBlocks != CaseBlocks || endSelectStatement != EndSelectStatement)
			{
				SelectBlockSyntax selectBlockSyntax = SyntaxFactory.SelectBlock(selectStatement, caseBlocks, endSelectStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(selectBlockSyntax, annotations);
				}
				return selectBlockSyntax;
			}
			return this;
		}
	}
}
