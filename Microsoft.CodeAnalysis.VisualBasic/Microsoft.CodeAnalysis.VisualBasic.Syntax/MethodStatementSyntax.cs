using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class MethodStatementSyntax : MethodBaseSyntax
	{
		internal TypeParameterListSyntax _typeParameterList;

		internal SimpleAsClauseSyntax _asClause;

		internal HandlesClauseSyntax _handlesClause;

		internal ImplementsClauseSyntax _implementsClause;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken SubOrFunctionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)base.Green)._subOrFunctionKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)base.Green)._identifier, GetChildPosition(3), GetChildIndex(3));

		public TypeParameterListSyntax TypeParameterList => GetRed(ref _typeParameterList, 4);

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 5);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 6);

		public HandlesClauseSyntax HandlesClause => GetRed(ref _handlesClause, 7);

		public ImplementsClauseSyntax ImplementsClause => GetRed(ref _implementsClause, 8);

		public override SyntaxToken DeclarationKeyword => SubOrFunctionKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal MethodStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal MethodStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, subOrFunctionKeyword, identifier, (typeParameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)typeParameterList.Green) : null, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, (handlesClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)handlesClause.Green) : null, (implementsClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)implementsClause.Green) : null), null, 0)
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

		public new MethodStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(Kind(), attributeLists, Modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public new MethodStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new MethodStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(Kind(), AttributeLists, modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public new MethodStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public MethodStatementSyntax WithSubOrFunctionKeyword(SyntaxToken subOrFunctionKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, subOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public MethodStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, identifier, TypeParameterList, ParameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public MethodStatementSyntax WithTypeParameterList(TypeParameterListSyntax typeParameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, Identifier, typeParameterList, ParameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public MethodStatementSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
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

		public new MethodStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, parameterList, AsClause, HandlesClause, ImplementsClause);
		}

		public new MethodStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public MethodStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, asClause, HandlesClause, ImplementsClause);
		}

		public MethodStatementSyntax WithHandlesClause(HandlesClauseSyntax handlesClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause, handlesClause, ImplementsClause);
		}

		public MethodStatementSyntax AddHandlesClauseEvents(params HandlesClauseItemSyntax[] items)
		{
			HandlesClauseSyntax handlesClauseSyntax = ((HandlesClause != null) ? HandlesClause : SyntaxFactory.HandlesClause());
			return WithHandlesClause(handlesClauseSyntax.AddEvents(items));
		}

		public MethodStatementSyntax WithImplementsClause(ImplementsClauseSyntax implementsClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, Identifier, TypeParameterList, ParameterList, AsClause, HandlesClause, implementsClause);
		}

		public MethodStatementSyntax AddImplementsClauseInterfaceMembers(params QualifiedNameSyntax[] items)
		{
			ImplementsClauseSyntax implementsClauseSyntax = ((ImplementsClause != null) ? ImplementsClause : SyntaxFactory.ImplementsClause());
			return WithImplementsClause(implementsClauseSyntax.AddInterfaceMembers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				4 => _typeParameterList, 
				5 => _parameterList, 
				6 => _asClause, 
				7 => _handlesClause, 
				8 => _implementsClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				4 => TypeParameterList, 
				5 => ParameterList, 
				6 => AsClause, 
				7 => HandlesClause, 
				8 => ImplementsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitMethodStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitMethodStatement(this);
		}

		public MethodStatementSyntax Update(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken subOrFunctionKeyword, SyntaxToken identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			if (kind != Kind() || attributeLists != AttributeLists || modifiers != Modifiers || subOrFunctionKeyword != SubOrFunctionKeyword || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || asClause != AsClause || handlesClause != HandlesClause || implementsClause != ImplementsClause)
			{
				MethodStatementSyntax methodStatementSyntax = SyntaxFactory.MethodStatement(kind, attributeLists, modifiers, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(methodStatementSyntax, annotations);
				}
				return methodStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new MethodStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}
	}
}
