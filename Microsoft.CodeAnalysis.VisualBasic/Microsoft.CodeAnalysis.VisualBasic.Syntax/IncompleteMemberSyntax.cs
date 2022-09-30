using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class IncompleteMemberSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		public SyntaxTokenList Modifiers
		{
			get
			{
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken MissingIdentifier
		{
			get
			{
				IdentifierTokenSyntax missingIdentifier = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax)base.Green)._missingIdentifier;
				return (missingIdentifier == null) ? default(SyntaxToken) : new SyntaxToken(this, missingIdentifier, GetChildPosition(2), GetChildIndex(2));
			}
		}

		internal IncompleteMemberSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal IncompleteMemberSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, IdentifierTokenSyntax missingIdentifier)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, missingIdentifier), null, 0)
		{
		}

		public IncompleteMemberSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, MissingIdentifier);
		}

		public IncompleteMemberSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public IncompleteMemberSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, MissingIdentifier);
		}

		public IncompleteMemberSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		public IncompleteMemberSyntax WithMissingIdentifier(SyntaxToken missingIdentifier)
		{
			return Update(AttributeLists, Modifiers, missingIdentifier);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			if (i == 0)
			{
				return _attributeLists;
			}
			return null;
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			if (i == 0)
			{
				return GetRedAtZero(ref _attributeLists);
			}
			return null;
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitIncompleteMember(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitIncompleteMember(this);
		}

		public IncompleteMemberSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken missingIdentifier)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || missingIdentifier != MissingIdentifier)
			{
				IncompleteMemberSyntax incompleteMemberSyntax = SyntaxFactory.IncompleteMember(attributeLists, modifiers, missingIdentifier);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(incompleteMemberSyntax, annotations);
				}
				return incompleteMemberSyntax;
			}
			return this;
		}
	}
}
