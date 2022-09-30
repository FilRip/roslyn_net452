using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ImplementsClauseSyntax : VisualBasicSyntaxNode
	{
		internal SyntaxNode _interfaceMembers;

		public SyntaxToken ImplementsKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)base.Green)._implementsKeyword, base.Position, 0);

		public SeparatedSyntaxList<QualifiedNameSyntax> InterfaceMembers
		{
			get
			{
				SyntaxNode red = GetRed(ref _interfaceMembers, 1);
				return (red == null) ? default(SeparatedSyntaxList<QualifiedNameSyntax>) : new SeparatedSyntaxList<QualifiedNameSyntax>(red, GetChildIndex(1));
			}
		}

		internal ImplementsClauseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ImplementsClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax implementsKeyword, SyntaxNode interfaceMembers)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax(kind, errors, annotations, implementsKeyword, interfaceMembers?.Green), null, 0)
		{
		}

		public ImplementsClauseSyntax WithImplementsKeyword(SyntaxToken implementsKeyword)
		{
			return Update(implementsKeyword, InterfaceMembers);
		}

		public ImplementsClauseSyntax WithInterfaceMembers(SeparatedSyntaxList<QualifiedNameSyntax> interfaceMembers)
		{
			return Update(ImplementsKeyword, interfaceMembers);
		}

		public ImplementsClauseSyntax AddInterfaceMembers(params QualifiedNameSyntax[] items)
		{
			return WithInterfaceMembers(InterfaceMembers.AddRange(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 1)
			{
				return _interfaceMembers;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 1)
			{
				return GetRed(ref _interfaceMembers, 1);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitImplementsClause(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitImplementsClause(this);
		}

		public ImplementsClauseSyntax Update(SyntaxToken implementsKeyword, SeparatedSyntaxList<QualifiedNameSyntax> interfaceMembers)
		{
			if (implementsKeyword != ImplementsKeyword || interfaceMembers != InterfaceMembers)
			{
				ImplementsClauseSyntax implementsClauseSyntax = SyntaxFactory.ImplementsClause(implementsKeyword, interfaceMembers);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(implementsClauseSyntax, annotations);
				}
				return implementsClauseSyntax;
			}
			return this;
		}
	}
}
