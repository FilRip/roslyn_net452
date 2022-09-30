using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SimpleImportsClauseSyntax : ImportsClauseSyntax
	{
		internal ImportAliasClauseSyntax _alias;

		internal NameSyntax _name;

		public ImportAliasClauseSyntax Alias => GetRedAtZero(ref _alias);

		public NameSyntax Name => GetRed(ref _name, 1);

		internal SimpleImportsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SimpleImportsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ImportAliasClauseSyntax alias, NameSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax(kind, errors, annotations, (alias != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax)alias.Green) : null, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)name.Green), null, 0)
		{
		}

		public SimpleImportsClauseSyntax WithAlias(ImportAliasClauseSyntax alias)
		{
			return Update(alias, Name);
		}

		public SimpleImportsClauseSyntax WithName(NameSyntax name)
		{
			return Update(Alias, name);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _alias, 
				1 => _name, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => Alias, 
				1 => Name, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSimpleImportsClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSimpleImportsClause(this);
		}

		public SimpleImportsClauseSyntax Update(ImportAliasClauseSyntax alias, NameSyntax name)
		{
			if (alias != Alias || name != Name)
			{
				SimpleImportsClauseSyntax simpleImportsClauseSyntax = SyntaxFactory.SimpleImportsClause(alias, name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(simpleImportsClauseSyntax, annotations);
				}
				return simpleImportsClauseSyntax;
			}
			return this;
		}
	}
}
