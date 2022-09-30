using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class TypeStatementSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		internal TypeParameterListSyntax _typeParameterList;

		public SyntaxList<AttributeListSyntax> AttributeLists => GetAttributeListsCore();

		public SyntaxTokenList Modifiers => GetModifiersCore();

		public SyntaxToken Identifier => GetIdentifierCore();

		public TypeParameterListSyntax TypeParameterList => GetTypeParameterListCore();

		public int Arity
		{
			get
			{
				if (TypeParameterList != null)
				{
					return TypeParameterList.Parameters.Count;
				}
				return 0;
			}
		}

		public abstract SyntaxToken DeclarationKeyword { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. ClassKeyword) instead.", true)]
		public SyntaxToken Keyword => DeclarationKeyword;

		internal TypeStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
			return new SyntaxList<AttributeListSyntax>(redAtZero);
		}

		public TypeStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeListsCore(attributeLists);
		}

		internal abstract TypeStatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

		public TypeStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return AddAttributeListsCore(items);
		}

		internal abstract TypeStatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

		internal virtual SyntaxTokenList GetModifiersCore()
		{
			GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax)base.Green)._modifiers;
			return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
		}

		public TypeStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return WithModifiersCore(modifiers);
		}

		internal abstract TypeStatementSyntax WithModifiersCore(SyntaxTokenList modifiers);

		public TypeStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return AddModifiersCore(items);
		}

		internal abstract TypeStatementSyntax AddModifiersCore(params SyntaxToken[] items);

		internal virtual SyntaxToken GetIdentifierCore()
		{
			return new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax)base.Green)._identifier, GetChildPosition(2), GetChildIndex(2));
		}

		public TypeStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return WithIdentifierCore(identifier);
		}

		internal abstract TypeStatementSyntax WithIdentifierCore(SyntaxToken identifier);

		internal virtual TypeParameterListSyntax GetTypeParameterListCore()
		{
			return GetRed(ref _typeParameterList, 3);
		}

		public TypeStatementSyntax WithTypeParameterList(TypeParameterListSyntax typeParameterList)
		{
			return WithTypeParameterListCore(typeParameterList);
		}

		internal abstract TypeStatementSyntax WithTypeParameterListCore(TypeParameterListSyntax typeParameterList);

		public TypeStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
		{
			return AddTypeParameterListParametersCore(items);
		}

		internal abstract TypeStatementSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items);

		public abstract TypeStatementSyntax WithDeclarationKeyword(SyntaxToken keyword);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. WithClassKeyword) instead.", true)]
		public TypeStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithDeclarationKeyword(keyword);
		}
	}
}
