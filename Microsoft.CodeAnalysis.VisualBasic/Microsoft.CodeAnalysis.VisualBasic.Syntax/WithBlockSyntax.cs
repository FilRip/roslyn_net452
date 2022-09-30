using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class WithBlockSyntax : ExecutableStatementSyntax
	{
		internal WithStatementSyntax _withStatement;

		internal SyntaxNode _statements;

		internal EndBlockStatementSyntax _endWithStatement;

		public WithStatementSyntax WithStatement => GetRedAtZero(ref _withStatement);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		public EndBlockStatementSyntax EndWithStatement => GetRed(ref _endWithStatement, 2);

		internal WithBlockSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal WithBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, WithStatementSyntax withStatement, SyntaxNode statements, EndBlockStatementSyntax endWithStatement)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax(kind, errors, annotations, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax)withStatement.Green, statements?.Green, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)endWithStatement.Green), null, 0)
		{
		}

		public WithBlockSyntax WithWithStatement(WithStatementSyntax withStatement)
		{
			return Update(withStatement, Statements, EndWithStatement);
		}

		public WithBlockSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(WithStatement, statements, EndWithStatement);
		}

		public WithBlockSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		public WithBlockSyntax WithEndWithStatement(EndBlockStatementSyntax endWithStatement)
		{
			return Update(WithStatement, Statements, endWithStatement);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _withStatement, 
				1 => _statements, 
				2 => _endWithStatement, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => WithStatement, 
				1 => GetRed(ref _statements, 1), 
				2 => EndWithStatement, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitWithBlock(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitWithBlock(this);
		}

		public WithBlockSyntax Update(WithStatementSyntax withStatement, SyntaxList<StatementSyntax> statements, EndBlockStatementSyntax endWithStatement)
		{
			if (withStatement != WithStatement || statements != Statements || endWithStatement != EndWithStatement)
			{
				WithBlockSyntax withBlockSyntax = SyntaxFactory.WithBlock(withStatement, statements, endWithStatement);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(withBlockSyntax, annotations);
				}
				return withBlockSyntax;
			}
			return this;
		}
	}
}
