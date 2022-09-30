using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SingleLineElseClauseSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _statements;

		public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax)base.Green)._elseKeyword, base.Position, 0);

		public SyntaxList<StatementSyntax> Statements
		{
			get
			{
				SyntaxNode red = GetRed(ref _statements, 1);
				return new SyntaxList<StatementSyntax>(red);
			}
		}

		internal SingleLineElseClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SingleLineElseClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax elseKeyword, SyntaxNode statements)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax(kind, errors, annotations, elseKeyword, statements?.Green), null, 0)
		{
		}

		public SingleLineElseClauseSyntax WithElseKeyword(SyntaxToken elseKeyword)
		{
			return Update(elseKeyword, Statements);
		}

		public SingleLineElseClauseSyntax WithStatements(SyntaxList<StatementSyntax> statements)
		{
			return Update(ElseKeyword, statements);
		}

		public SingleLineElseClauseSyntax AddStatements(params StatementSyntax[] items)
		{
			return WithStatements(Statements.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _statements;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _statements, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSingleLineElseClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSingleLineElseClause(this);
		}

		public SingleLineElseClauseSyntax Update(SyntaxToken elseKeyword, SyntaxList<StatementSyntax> statements)
		{
			if (elseKeyword != ElseKeyword || statements != Statements)
			{
				SingleLineElseClauseSyntax singleLineElseClauseSyntax = SyntaxFactory.SingleLineElseClause(elseKeyword, statements);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(singleLineElseClauseSyntax, annotations);
				}
				return singleLineElseClauseSyntax;
			}
			return this;
		}
	}
}
