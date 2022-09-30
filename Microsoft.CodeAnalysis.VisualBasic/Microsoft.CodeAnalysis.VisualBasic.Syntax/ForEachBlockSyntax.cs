using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ForEachBlockSyntax : ForOrForEachBlockSyntax
	{
		internal ForEachStatementSyntax _forEachStatement;

		public ForEachStatementSyntax ForEachStatement => GetRedAtZero(ref _forEachStatement);

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
		public override ForOrForEachStatementSyntax ForOrForEachStatement => ForEachStatement;

		internal ForEachBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ForEachBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ForEachStatementSyntax forEachStatement, SyntaxNode statements, NextStatementSyntax nextStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax)forEachStatement.Green, statements?.Green, (nextStatement != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)nextStatement.Green) : null), null, 0)
		{
		}

		public ForEachBlockSyntax WithForEachStatement(ForEachStatementSyntax forEachStatement)
		{
			return Update(forEachStatement, Statements, NextStatement);
		}

		internal override SyntaxList<StatementSyntax> GetStatementsCore()
		{
			return Statements;
		}

		internal override ForOrForEachBlockSyntax WithStatementsCore(SyntaxList<StatementSyntax> statements)
		{
			return WithStatements(statements);
		}

		public new ForEachBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(ForEachStatement, statements, NextStatement);
		}

		public new ForEachBlockSyntax AddStatements(params StatementSyntax[] items)
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

		public new ForEachBlockSyntax WithNextStatement(NextStatementSyntax nextStatement)
		{
			return Update(ForEachStatement, Statements, nextStatement);
		}

		public new ForEachBlockSyntax AddNextStatementControlVariables(params ExpressionSyntax[] items)
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
				0 => _forEachStatement, 
				1 => _statements, 
				2 => _nextStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => ForEachStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => NextStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitForEachBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitForEachBlock(this);
		}

		public ForEachBlockSyntax Update(ForEachStatementSyntax forEachStatement, SyntaxList<StatementSyntax> statements, NextStatementSyntax nextStatement)
		{
			if (forEachStatement != ForEachStatement || statements != Statements || nextStatement != NextStatement)
			{
				ForEachBlockSyntax forEachBlockSyntax = SyntaxFactory.ForEachBlock(forEachStatement, statements, nextStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(forEachBlockSyntax, annotations);
				}
				return forEachBlockSyntax;
			}
			return this;
		}
	}
}
