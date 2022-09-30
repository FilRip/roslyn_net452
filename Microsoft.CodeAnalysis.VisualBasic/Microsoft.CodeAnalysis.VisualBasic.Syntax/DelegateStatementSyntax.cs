using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class DelegateStatementSyntax : MethodBaseSyntax
	{
		internal TypeParameterListSyntax _typeParameterList;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)base.Green)._delegateKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken SubOrFunctionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)base.Green)._subOrFunctionKeyword, GetChildPosition(3), GetChildIndex(3));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax)base.Green)._identifier, GetChildPosition(4), GetChildIndex(4));

		public TypeParameterListSyntax TypeParameterList => GetRed(ref _typeParameterList, 5);

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 6);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 7);

		public override SyntaxToken DeclarationKeyword => SubOrFunctionKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal DelegateStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal DelegateStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, delegateKeyword, subOrFunctionKeyword, identifier, (typeParameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green) : null, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
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

		public new DelegateStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(Kind(), attributeLists, Modifiers, DelegateKeyword, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause);
		}

		public new DelegateStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new DelegateStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(Kind(), AttributeLists, modifiers, DelegateKeyword, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause);
		}

		public new DelegateStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public DelegateStatementSyntax WithDelegateKeyword(SyntaxToken delegateKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, delegateKeyword, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause);
		}

		public DelegateStatementSyntax WithSubOrFunctionKeyword(SyntaxToken subOrFunctionKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, DelegateKeyword, subOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause);
		}

		public DelegateStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(Kind(), AttributeLists, Modifiers, DelegateKeyword, SubOrFunctionKeyword, identifier, TypeParameterList, ParameterList, AsClause);
		}

		public DelegateStatementSyntax WithTypeParameterList(TypeParameterListSyntax typeParameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, DelegateKeyword, SubOrFunctionKeyword, Identifier, typeParameterList, ParameterList, AsClause);
		}

		public DelegateStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
		{
			TypeParameterListSyntax typeParameterListSyntax = ((TypeParameterList != null) ? TypeParameterList : SyntaxFactory.TypeParameterList());
			return WithTypeParameterList(typeParameterListSyntax.AddParameters(items));
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new DelegateStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, DelegateKeyword, SubOrFunctionKeyword, Identifier, TypeParameterList, parameterList, AsClause);
		}

		public new DelegateStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public DelegateStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, DelegateKeyword, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				5 => _typeParameterList, 
				6 => _parameterList, 
				7 => _asClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				5 => TypeParameterList, 
				6 => ParameterList, 
				7 => AsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitDelegateStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitDelegateStatement(this);
		}

		public DelegateStatementSyntax Update(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken delegateKeyword, SyntaxToken subOrFunctionKeyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			if (kind != Kind() || attributeLists != AttributeLists || modifiers != Modifiers || delegateKeyword != DelegateKeyword || subOrFunctionKeyword != SubOrFunctionKeyword || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || asClause != AsClause)
			{
				DelegateStatementSyntax delegateStatementSyntax = SyntaxFactory.DelegateStatement(kind, attributeLists, modifiers, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(delegateStatementSyntax, annotations);
				}
				return delegateStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new DelegateStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}
	}
}
