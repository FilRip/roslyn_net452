using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class PropertyStatementSyntax : MethodBaseSyntax
	{
		internal AsClauseSyntax _asClause;

		internal EqualsValueSyntax _initializer;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken PropertyKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)base.Green)._propertyKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax)base.Green)._identifier, GetChildPosition(3), GetChildIndex(3));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 4);

		public AsClauseSyntax AsClause => GetRed(ref _asClause, 5);

		public EqualsValueSyntax Initializer => GetRed(ref _initializer, 6);

		public ImplementsClauseSyntax ImplementsClause => GetRed(ref _implementsClause, 7);

		public override SyntaxToken DeclarationKeyword => PropertyKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal PropertyStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal PropertyStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, propertyKeyword, identifier, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax)asClause.Green) : null, (initializer != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)initializer.Green) : null, (implementsClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)implementsClause.Green) : null), null, 0)
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

		public new PropertyStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, PropertyKeyword, Identifier, ParameterList, AsClause, Initializer, ImplementsClause);
		}

		public new PropertyStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new PropertyStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, PropertyKeyword, Identifier, ParameterList, AsClause, Initializer, ImplementsClause);
		}

		public new PropertyStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public PropertyStatementSyntax WithPropertyKeyword(SyntaxToken propertyKeyword)
		{
			return Update(AttributeLists, Modifiers, propertyKeyword, Identifier, ParameterList, AsClause, Initializer, ImplementsClause);
		}

		public PropertyStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, Modifiers, PropertyKeyword, identifier, ParameterList, AsClause, Initializer, ImplementsClause);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new PropertyStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(AttributeLists, Modifiers, PropertyKeyword, Identifier, parameterList, AsClause, Initializer, ImplementsClause);
		}

		public new PropertyStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public PropertyStatementSyntax WithAsClause(AsClauseSyntax asClause)
		{
			return Update(AttributeLists, Modifiers, PropertyKeyword, Identifier, ParameterList, asClause, Initializer, ImplementsClause);
		}

		public PropertyStatementSyntax WithInitializer(EqualsValueSyntax initializer)
		{
			return Update(AttributeLists, Modifiers, PropertyKeyword, Identifier, ParameterList, AsClause, initializer, ImplementsClause);
		}

		public PropertyStatementSyntax WithImplementsClause(ImplementsClauseSyntax implementsClause)
		{
			return Update(AttributeLists, Modifiers, PropertyKeyword, Identifier, ParameterList, AsClause, Initializer, implementsClause);
		}

		public PropertyStatementSyntax AddImplementsClauseInterfaceMembers(params QualifiedNameSyntax[] items)
		{
			ImplementsClauseSyntax implementsClauseSyntax = ((ImplementsClause != null) ? ImplementsClause : SyntaxFactory.ImplementsClause());
			return WithImplementsClause(implementsClauseSyntax.AddInterfaceMembers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				4 => _parameterList, 
				5 => _asClause, 
				6 => _initializer, 
				7 => _implementsClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				4 => ParameterList, 
				5 => AsClause, 
				6 => Initializer, 
				7 => ImplementsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitPropertyStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitPropertyStatement(this);
		}

		public PropertyStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken propertyKeyword, SyntaxToken identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || propertyKeyword != PropertyKeyword || identifier != Identifier || parameterList != ParameterList || asClause != AsClause || initializer != Initializer || implementsClause != ImplementsClause)
			{
				PropertyStatementSyntax propertyStatementSyntax = SyntaxFactory.PropertyStatement(attributeLists, modifiers, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(propertyStatementSyntax, annotations);
				}
				return propertyStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithPropertyKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new PropertyStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithPropertyKeyword(keyword);
		}
	}
}
