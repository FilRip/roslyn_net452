using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EnumMemberDeclarationSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		internal EqualsValueSyntax _initializer;

		public SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax)base.Green)._identifier, GetChildPosition(1), GetChildIndex(1));

		public EqualsValueSyntax Initializer => GetRed(ref _initializer, 2);

		internal EnumMemberDeclarationSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EnumMemberDeclarationSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax(kind, errors, annotations, attributeLists?.Green, identifier, (initializer != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)initializer.Green) : null), null, 0)
		{
		}

		public EnumMemberDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Identifier, Initializer);
		}

		public EnumMemberDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public EnumMemberDeclarationSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, identifier, Initializer);
		}

		public EnumMemberDeclarationSyntax WithInitializer(EqualsValueSyntax initializer)
		{
			return Update(AttributeLists, Identifier, initializer);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				2 => _initializer, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				2 => Initializer, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEnumMemberDeclaration(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEnumMemberDeclaration(this);
		}

		public EnumMemberDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, EqualsValueSyntax initializer)
		{
			if (attributeLists != AttributeLists || identifier != Identifier || initializer != Initializer)
			{
				EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = SyntaxFactory.EnumMemberDeclaration(attributeLists, identifier, initializer);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(enumMemberDeclarationSyntax, annotations);
				}
				return enumMemberDeclarationSyntax;
			}
			return this;
		}
	}
}
