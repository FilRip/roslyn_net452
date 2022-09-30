using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class ClassStatementSyntax : TypeStatementSyntax
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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken ClassKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)base.Green)._classKeyword, GetChildPosition(2), GetChildIndex(2));

		public new SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)base.Green)._identifier, GetChildPosition(3), GetChildIndex(3));

		public new TypeParameterListSyntax TypeParameterList => GetRed(ref _typeParameterList, 4);

		public override SyntaxToken DeclarationKeyword => ClassKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal ClassStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal ClassStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, classKeyword, identifier, (typeParameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green) : null), null, 0)
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

		public new ClassStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, ClassKeyword, Identifier, TypeParameterList);
		}

		public new ClassStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new ClassStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, ClassKeyword, Identifier, TypeParameterList);
		}

		public new ClassStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override TypeStatementSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public ClassStatementSyntax WithClassKeyword(SyntaxToken classKeyword)
		{
			return Update(AttributeLists, Modifiers, classKeyword, Identifier, TypeParameterList);
		}

		internal override SyntaxToken GetIdentifierCore()
		{
			return Identifier;
		}

		internal override TypeStatementSyntax WithIdentifierCore(SyntaxToken identifier)
		{
			return WithIdentifier(identifier);
		}

		public new ClassStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, Modifiers, ClassKeyword, identifier, TypeParameterList);
		}

		internal override TypeParameterListSyntax GetTypeParameterListCore()
		{
			return TypeParameterList;
		}

		internal override TypeStatementSyntax WithTypeParameterListCore(TypeParameterListSyntax typeParameterList)
		{
			return WithTypeParameterList(typeParameterList);
		}

		public new ClassStatementSyntax WithTypeParameterList(TypeParameterListSyntax typeParameterList)
		{
			return Update(AttributeLists, Modifiers, ClassKeyword, Identifier, typeParameterList);
		}

		public new ClassStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
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
			return visitor.VisitClassStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitClassStatement(this);
		}

		public ClassStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken classKeyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || classKeyword != ClassKeyword || identifier != Identifier || typeParameterList != TypeParameterList)
			{
				ClassStatementSyntax classStatementSyntax = SyntaxFactory.ClassStatement(attributeLists, modifiers, classKeyword, identifier, typeParameterList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(classStatementSyntax, annotations);
				}
				return classStatementSyntax;
			}
			return this;
		}

		public override TypeStatementSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithClassKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new ClassStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithClassKeyword(keyword);
		}
	}
}
