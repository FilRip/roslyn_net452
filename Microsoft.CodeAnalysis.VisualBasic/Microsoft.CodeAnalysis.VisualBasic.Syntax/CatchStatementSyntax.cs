using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class CatchStatementSyntax : StatementSyntax
	{
		internal IdentifierNameSyntax _identifierName;

		internal SimpleAsClauseSyntax _asClause;

		internal CatchFilterClauseSyntax _whenClause;

		public SyntaxToken CatchKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax)base.Green)._catchKeyword, base.Position, 0);

		public IdentifierNameSyntax IdentifierName => GetRed(ref _identifierName, 1);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 2);

		public CatchFilterClauseSyntax WhenClause => GetRed(ref _whenClause, 3);

		internal CatchStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal CatchStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax(kind, errors, annotations, catchKeyword, (identifierName != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)identifierName.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, (whenClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax)whenClause.Green) : null), null, 0)
		{
		}

		public CatchStatementSyntax WithCatchKeyword(SyntaxToken catchKeyword)
		{
			return Update(catchKeyword, IdentifierName, AsClause, WhenClause);
		}

		public CatchStatementSyntax WithIdentifierName(IdentifierNameSyntax identifierName)
		{
			return Update(CatchKeyword, identifierName, AsClause, WhenClause);
		}

		public CatchStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(CatchKeyword, IdentifierName, asClause, WhenClause);
		}

		public CatchStatementSyntax WithWhenClause(CatchFilterClauseSyntax whenClause)
		{
			return Update(CatchKeyword, IdentifierName, AsClause, whenClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				1 => _identifierName, 
				2 => _asClause, 
				3 => _whenClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				1 => IdentifierName, 
				2 => AsClause, 
				3 => WhenClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitCatchStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitCatchStatement(this);
		}

		public CatchStatementSyntax Update(SyntaxToken catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
		{
			if (catchKeyword != CatchKeyword || identifierName != IdentifierName || asClause != AsClause || whenClause != WhenClause)
			{
				CatchStatementSyntax catchStatementSyntax = SyntaxFactory.CatchStatement(catchKeyword, identifierName, asClause, whenClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(catchStatementSyntax, annotations);
				}
				return catchStatementSyntax;
			}
			return this;
		}
	}
}
