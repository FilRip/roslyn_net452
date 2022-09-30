using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ForBlockSyntax : ForOrForEachBlockSyntax
	{
		internal ForStatementSyntax _forStatement;

		public ForStatementSyntax ForStatement => GetRedAtZero(ref _forStatement);

		public new SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public new NextStatementSyntax NextStatement => GetRed(ref _nextStatement, 2);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ForOrForEachStatementSyntax ForOrForEachStatement => ForStatement;

		internal ForBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ForBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ForStatementSyntax forStatement, SyntaxNode statements, NextStatementSyntax nextStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)forStatement.Green, statements?.Green, (nextStatement != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)nextStatement.Green) : null), null, 0)
		{
		}

		public ForBlockSyntax WithForStatement(ForStatementSyntax forStatement)
		{
			return Update(forStatement, Statements, NextStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override ForOrForEachBlockSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new ForBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(ForStatement, statements, NextStatement);
		}

		public new ForBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override ForOrForEachBlockSyntax AddStatementsCore(params StatementSyntax[] items)
		{
			return AddStatements(items);
		}

		internal override NextStatementSyntax GetNextStatementCore()
		{
			return NextStatement;
		}

		internal override ForOrForEachBlockSyntax WithNextStatementCore(NextStatementSyntax nextStatement)
		{
			return WithNextStatement(nextStatement);
		}

		public new ForBlockSyntax WithNextStatement(NextStatementSyntax nextStatement)
		{
			return Update(ForStatement, Statements, nextStatement);
		}

		public new ForBlockSyntax AddNextStatementControlVariables(params ExpressionSyntax[] items)
		{
			NextStatementSyntax nextStatementSyntax = ((NextStatement != null) ? NextStatement : SyntaxFactory.NextStatement());
			return WithNextStatement(nextStatementSyntax.AddControlVariables(items));
		}

		internal override ForOrForEachBlockSyntax AddNextStatementControlVariablesCore(params ExpressionSyntax[] items)
		{
			return AddNextStatementControlVariables(items);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _forStatement, 
				1 => _statements, 
				2 => _nextStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ForStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => NextStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitForBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitForBlock(this);
		}

		public ForBlockSyntax Update(ForStatementSyntax forStatement, SyntaxList<StatementSyntax> statements, NextStatementSyntax nextStatement)
		{
			if (forStatement != ForStatement || statements != Statements || nextStatement != NextStatement)
			{
				ForBlockSyntax forBlockSyntax = SyntaxFactory.ForBlock(forStatement, statements, nextStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(forBlockSyntax, annotations);
				}
				return forBlockSyntax;
			}
			return this;
		}
	}
}
