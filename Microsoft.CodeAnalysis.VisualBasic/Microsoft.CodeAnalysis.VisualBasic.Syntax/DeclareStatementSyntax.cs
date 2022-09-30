using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DeclareStatementSyntax : MethodBaseSyntax
	{
		internal LiteralExpressionSyntax _libraryName;

		internal LiteralExpressionSyntax _aliasName;

		internal SimpleAsClauseSyntax _asClause;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken DeclareKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._declareKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken CharsetKeyword
		{
			get
			{
				KeywordSyntax charsetKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._charsetKeyword;
				return (charsetKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, charsetKeyword, GetChildPosition(3), GetChildIndex(3));
			}
		}

		public SyntaxToken SubOrFunctionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._subOrFunctionKeyword, GetChildPosition(4), GetChildIndex(4));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._identifier, GetChildPosition(5), GetChildIndex(5));

		public SyntaxToken LibKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._libKeyword, GetChildPosition(6), GetChildIndex(6));

		public LiteralExpressionSyntax LibraryName => GetRed(ref _libraryName, 7);

		public SyntaxToken AliasKeyword
		{
			get
			{
				KeywordSyntax aliasKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax)base.Green)._aliasKeyword;
				return (aliasKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, aliasKeyword, GetChildPosition(8), GetChildIndex(8));
			}
		}

		public LiteralExpressionSyntax AliasName => GetRed(ref _aliasName, 9);

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 10);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 11);

		public override SyntaxToken DeclarationKeyword => SubOrFunctionKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal DeclareStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DeclareStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)libraryName.Green, aliasKeyword, (aliasName != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)aliasName.Green) : null, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
		{
		}

		internal override SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			return AttributeLists;
		}

		internal override MethodBaseSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeLists(attributeLists);
		}

		public new DeclareStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(Kind(), attributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public new DeclareStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return WithAttributeLists(AttributeLists.AddRange(items));
		}

		internal override MethodBaseSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
		{
			return AddAttributeLists(items);
		}

		internal override SyntaxTokenList GetModifiersCore()
		{
			return Modifiers;
		}

		internal override MethodBaseSyntax WithModifiersCore(SyntaxTokenList modifiers)
		{
			return WithModifiers(modifiers);
		}

		public new DeclareStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(Kind(), AttributeLists, modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public new DeclareStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public DeclareStatementSyntax WithDeclareKeyword(SyntaxToken declareKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, declareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithCharsetKeyword(SyntaxToken charsetKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, charsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithSubOrFunctionKeyword(SyntaxToken subOrFunctionKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, subOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithLibKeyword(SyntaxToken libKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, libKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithLibraryName(LiteralExpressionSyntax libraryName)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, libraryName, AliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithAliasKeyword(SyntaxToken aliasKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, aliasKeyword, AliasName, ParameterList, AsClause);
		}

		public DeclareStatementSyntax WithAliasName(LiteralExpressionSyntax aliasName)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, aliasName, ParameterList, AsClause);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new DeclareStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, parameterList, AsClause);
		}

		public new DeclareStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public DeclareStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, DeclareKeyword, CharsetKeyword, SubOrFunctionKeyword, Identifier, LibKeyword, LibraryName, AliasKeyword, AliasName, ParameterList, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				7 => _libraryName, 
				9 => _aliasName, 
				10 => _parameterList, 
				11 => _asClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				7 => LibraryName, 
				9 => AliasName, 
				10 => ParameterList, 
				11 => AsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitDeclareStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDeclareStatement(this);
		}

		public DeclareStatementSyntax Update(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken declareKeyword, SyntaxToken charsetKeyword, SyntaxToken subOrFunctionKeyword, SyntaxToken identifier, SyntaxToken libKeyword, LiteralExpressionSyntax libraryName, SyntaxToken aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			if (kind != Kind() || attributeLists != AttributeLists || modifiers != Modifiers || declareKeyword != DeclareKeyword || charsetKeyword != CharsetKeyword || subOrFunctionKeyword != SubOrFunctionKeyword || identifier != Identifier || libKeyword != LibKeyword || libraryName != LibraryName || aliasKeyword != AliasKeyword || aliasName != AliasName || parameterList != ParameterList || asClause != AsClause)
			{
				DeclareStatementSyntax declareStatementSyntax = SyntaxFactory.DeclareStatement(kind, attributeLists, modifiers, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(declareStatementSyntax, annotations);
				}
				return declareStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new DeclareStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}
	}
}
