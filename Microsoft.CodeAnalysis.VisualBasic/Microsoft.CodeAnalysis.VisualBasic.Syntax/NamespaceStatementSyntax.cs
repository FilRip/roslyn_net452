using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class NamespaceStatementSyntax : DeclarationStatementSyntax
	{
		internal NameSyntax _name;

		public SyntaxToken NamespaceKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax)base.Green)._namespaceKeyword, base.Position, 0);

		public NameSyntax Name => GetRed(ref _name, 1);

		internal NamespaceStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal NamespaceStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax namespaceKeyword, NameSyntax name)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax(kind, errors, annotations, namespaceKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax)name.Green), null, 0)
		{
		}

		public NamespaceStatementSyntax WithNamespaceKeyword(SyntaxToken namespaceKeyword)
		{
			return Update(namespaceKeyword, Name);
		}

		public NamespaceStatementSyntax WithName(NameSyntax name)
		{
			return Update(NamespaceKeyword, name);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _name;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return Name;
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitNamespaceStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitNamespaceStatement(this);
		}

		public NamespaceStatementSyntax Update(SyntaxToken namespaceKeyword, NameSyntax name)
		{
			if (namespaceKeyword != NamespaceKeyword || name != Name)
			{
				NamespaceStatementSyntax namespaceStatementSyntax = SyntaxFactory.NamespaceStatement(namespaceKeyword, name);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(namespaceStatementSyntax, annotations);
				}
				return namespaceStatementSyntax;
			}
			return this;
		}
	}
}
