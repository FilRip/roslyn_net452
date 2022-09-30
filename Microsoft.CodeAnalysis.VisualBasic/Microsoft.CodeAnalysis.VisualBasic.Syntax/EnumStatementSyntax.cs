using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EnumStatementSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		internal AsClauseSyntax _underlyingType;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken EnumKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)base.Green)._enumKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax)base.Green)._identifier, GetChildPosition(3), GetChildIndex(3));

		public AsClauseSyntax UnderlyingType => GetRed(ref _underlyingType, 4);

		internal EnumStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EnumStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, enumKeyword, identifier, (underlyingType != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)underlyingType.Green) : null), null, 0)
		{
		}

		public EnumStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, EnumKeyword, Identifier, UnderlyingType);
		}

		public EnumStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		public EnumStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, EnumKeyword, Identifier, UnderlyingType);
		}

		public EnumStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		public EnumStatementSyntax WithEnumKeyword(SyntaxToken enumKeyword)
		{
			return Update(AttributeLists, Modifiers, enumKeyword, Identifier, UnderlyingType);
		}

		public EnumStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, Modifiers, EnumKeyword, identifier, UnderlyingType);
		}

		public EnumStatementSyntax WithUnderlyingType(AsClauseSyntax underlyingType)
		{
			return Update(AttributeLists, Modifiers, EnumKeyword, Identifier, underlyingType);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				4 => _underlyingType, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				4 => UnderlyingType, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEnumStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEnumStatement(this);
		}

		public EnumStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken enumKeyword, SyntaxToken identifier, AsClauseSyntax underlyingType)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || enumKeyword != EnumKeyword || identifier != Identifier || underlyingType != UnderlyingType)
			{
				EnumStatementSyntax enumStatementSyntax = SyntaxFactory.EnumStatement(attributeLists, modifiers, enumKeyword, identifier, underlyingType);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(enumStatementSyntax, annotations);
				}
				return enumStatementSyntax;
			}
			return this;
		}
	}
}
