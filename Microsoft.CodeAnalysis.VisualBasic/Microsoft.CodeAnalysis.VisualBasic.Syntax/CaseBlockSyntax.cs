using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CaseBlockSyntax : VisualBasicSyntaxNode
	{
		internal CaseStatementSyntax _caseStatement;

		internal SyntaxNode _statements;

		public CaseStatementSyntax CaseStatement => GetRedAtZero(ref _caseStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use CaseStatement instead.", true)]
		public CaseStatementSyntax Begin => CaseStatement;

		internal CaseBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CaseBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, CaseStatementSyntax caseStatement, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax)caseStatement.Green, statements?.Green), null, 0)
		{
		}

		public CaseBlockSyntax WithCaseStatement(CaseStatementSyntax caseStatement)
		{
			return Update(Kind(), caseStatement, Statements);
		}

		public CaseBlockSyntax AddCaseStatementCases(params CaseClauseSyntax[] items)
		{
			CaseStatementSyntax caseStatementSyntax = ((CaseStatement != null) ? CaseStatement : SyntaxFactory.CaseStatement());
			return WithCaseStatement(caseStatementSyntax.AddCases(items));
		}

		public CaseBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(Kind(), CaseStatement, statements);
		}

		public CaseBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _caseStatement, 
				1 => _statements, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => CaseStatement, 
				1 => GetRed(ref _statements, 1), 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCaseBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCaseBlock(this);
		}

		public CaseBlockSyntax Update(SyntaxKind kind, CaseStatementSyntax caseStatement, SyntaxList<StatementSyntax> statements)
		{
			if (kind != Kind() || caseStatement != CaseStatement || statements != Statements)
			{
				CaseBlockSyntax caseBlockSyntax = SyntaxFactory.CaseBlock(kind, caseStatement, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(caseBlockSyntax, annotations);
				}
				return caseBlockSyntax;
			}
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use WithCaseStatement instead.", true)]
		public CaseBlockSyntax WithBegin(CaseStatementSyntax begin)
		{
			return WithCaseStatement(begin);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use AddCaseStatementCases instead.", true)]
		public CaseBlockSyntax AddBeginCases(params CaseClauseSyntax[] items)
		{
			return AddCaseStatementCases(items);
		}
	}
}
