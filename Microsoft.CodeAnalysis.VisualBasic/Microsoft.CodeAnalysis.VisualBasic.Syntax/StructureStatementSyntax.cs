using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class StructureStatementSyntax : TypeStatementSyntax
	{
		public new SyntaxList<AttributeListSyntax> AttributeLists
		{
			get
			{
				SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
				return new SyntaxList<AttributeListSyntax>(redAtZero);
			}
		}

		public new SyntaxTokenList Modifiers
		{
			get
			{
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken StructureKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)base.Green)._structureKeyword, GetChildPosition(2), GetChildIndex(2));

		public new SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)base.Green)._identifier, GetChildPosition(3), GetChildIndex(3));

		public new TypeParameterListSyntax TypeParameterList => GetRed(ref _typeParameterList, 4);

		public override SyntaxToken DeclarationKeyword => StructureKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal StructureStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal StructureStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, structureKeyword, identifier, (typeParameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green) : null), null, 0)
		{
		}

		internal override SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			return AttributeLists;
		}

		internal override TypeStatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeLists(attributeLists);
		}

		public new StructureStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, StructureKeyword, Identifier, TypeParameterList);
		}

		public new StructureStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override TypeStatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
		{
			return AddAttributeLists(items);
		}

		internal override SyntaxTokenList GetModifiersCore()
		{
			return Modifiers;
		}

		internal override TypeStatementSyntax WithModifiersCore(SyntaxTokenList modifiers)
		{
			return WithModifiers(modifiers);
		}

		public new StructureStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, StructureKeyword, Identifier, TypeParameterList);
		}

		public new StructureStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override TypeStatementSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public StructureStatementSyntax WithStructureKeyword(SyntaxToken structureKeyword)
		{
			return Update(AttributeLists, Modifiers, structureKeyword, Identifier, TypeParameterList);
		}

		internal override SyntaxToken GetIdentifierCore()
		{
			return Identifier;
		}

		internal override TypeStatementSyntax WithIdentifierCore(SyntaxToken identifier)
		{
			return WithIdentifier(identifier);
		}

		public new StructureStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, Modifiers, StructureKeyword, identifier, TypeParameterList);
		}

		internal override TypeParameterListSyntax GetTypeParameterListCore()
		{
			return TypeParameterList;
		}

		internal override TypeStatementSyntax WithTypeParameterListCore(TypeParameterListSyntax typeParameterList)
		{
			return WithTypeParameterList(typeParameterList);
		}

		public new StructureStatementSyntax WithTypeParameterList(TypeParameterListSyntax typeParameterList)
		{
			return Update(AttributeLists, Modifiers, StructureKeyword, Identifier, typeParameterList);
		}

		public new StructureStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
		{
			TypeParameterListSyntax typeParameterListSyntax = ((TypeParameterList != null) ? TypeParameterList : SyntaxFactory.TypeParameterList());
			return WithTypeParameterList(typeParameterListSyntax.AddParameters(items));
		}

		internal override TypeStatementSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items)
		{
			return AddTypeParameterListParameters(items);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				4 => _typeParameterList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				4 => TypeParameterList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitStructureStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitStructureStatement(this);
		}

		public StructureStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken structureKeyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || structureKeyword != StructureKeyword || identifier != Identifier || typeParameterList != TypeParameterList)
			{
				StructureStatementSyntax structureStatementSyntax = SyntaxFactory.StructureStatement(attributeLists, modifiers, structureKeyword, identifier, typeParameterList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(structureStatementSyntax, annotations);
				}
				return structureStatementSyntax;
			}
			return this;
		}

		public override TypeStatementSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithStructureKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new StructureStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithStructureKeyword(keyword);
		}
	}
}
