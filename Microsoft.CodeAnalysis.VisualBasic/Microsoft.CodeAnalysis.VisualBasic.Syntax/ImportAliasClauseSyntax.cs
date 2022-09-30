using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ImportAliasClauseSyntax : VisualBasicSyntaxNode
	{
		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)base.Green)._identifier, base.Position, 0);

		public SyntaxToken EqualsToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)base.Green)._equalsToken, GetChildPosition(1), GetChildIndex(1));

		internal ImportAliasClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ImportAliasClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax(kind, errors, annotations, identifier, equalsToken), null, 0)
		{
		}

		public ImportAliasClauseSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(identifier, EqualsToken);
		}

		public ImportAliasClauseSyntax WithEqualsToken(SyntaxToken equalsToken)
		{
			return Update(Identifier, equalsToken);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitImportAliasClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitImportAliasClause(this);
		}

		public ImportAliasClauseSyntax Update(SyntaxToken identifier, SyntaxToken equalsToken)
		{
			if (identifier != Identifier || equalsToken != EqualsToken)
			{
				ImportAliasClauseSyntax importAliasClauseSyntax = SyntaxFactory.ImportAliasClause(identifier, equalsToken);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(importAliasClauseSyntax, annotations);
				}
				return importAliasClauseSyntax;
			}
			return this;
		}
	}
}
