using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ImportsStatementSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _importsClauses;

		public SyntaxToken ImportsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax)base.Green)._importsKeyword, base.Position, 0);

		public SeparatedSyntaxList<ImportsClauseSyntax> ImportsClauses
		{
			get
			{
				SyntaxNode red = GetRed(ref _importsClauses, 1);
				return (red == null) ? default(SeparatedSyntaxList<ImportsClauseSyntax>) : new SeparatedSyntaxList<ImportsClauseSyntax>(red, GetChildIndex(1));
			}
		}

		internal ImportsStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ImportsStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax importsKeyword, SyntaxNode importsClauses)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax(kind, errors, annotations, importsKeyword, importsClauses?.Green), null, 0)
		{
		}

		public ImportsStatementSyntax WithImportsKeyword(SyntaxToken importsKeyword)
		{
			return Update(importsKeyword, ImportsClauses);
		}

		public ImportsStatementSyntax WithImportsClauses(SeparatedSyntaxList<ImportsClauseSyntax> importsClauses)
		{
			return Update(ImportsKeyword, importsClauses);
		}

		public ImportsStatementSyntax AddImportsClauses(params ImportsClauseSyntax[] items)
		{
			return WithImportsClauses(ImportsClauses.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _importsClauses;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _importsClauses, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitImportsStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitImportsStatement(this);
		}

		public ImportsStatementSyntax Update(SyntaxToken importsKeyword, SeparatedSyntaxList<ImportsClauseSyntax> importsClauses)
		{
			if (importsKeyword != ImportsKeyword || importsClauses != ImportsClauses)
			{
				ImportsStatementSyntax importsStatementSyntax = SyntaxFactory.ImportsStatement(importsKeyword, importsClauses);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(importsStatementSyntax, annotations);
				}
				return importsStatementSyntax;
			}
			return this;
		}
	}
}
