using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class EventStatementSyntax : MethodBaseSyntax
	{
		internal SimpleAsClauseSyntax _asClause;

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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken CustomKeyword
		{
			get
			{
				KeywordSyntax customKeyword = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)base.Green)._customKeyword;
				return (customKeyword == null) ? default(SyntaxToken) : new SyntaxToken(this, customKeyword, GetChildPosition(2), GetChildIndex(2));
			}
		}

		public SyntaxToken EventKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)base.Green)._eventKeyword, GetChildPosition(3), GetChildIndex(3));

		public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)base.Green)._identifier, GetChildPosition(4), GetChildIndex(4));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 5);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 6);

		public ImplementsClauseSyntax ImplementsClause => GetRed(ref _implementsClause, 7);

		public override SyntaxToken DeclarationKeyword => EventKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal EventStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal EventStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, customKeyword, eventKeyword, identifier, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null, (implementsClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)implementsClause.Green) : null), null, 0)
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

		public new EventStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, CustomKeyword, EventKeyword, Identifier, ParameterList, AsClause, ImplementsClause);
		}

		public new EventStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new EventStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, CustomKeyword, EventKeyword, Identifier, ParameterList, AsClause, ImplementsClause);
		}

		public new EventStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public EventStatementSyntax WithCustomKeyword(SyntaxToken customKeyword)
		{
			return Update(AttributeLists, Modifiers, customKeyword, EventKeyword, Identifier, ParameterList, AsClause, ImplementsClause);
		}

		public EventStatementSyntax WithEventKeyword(SyntaxToken eventKeyword)
		{
			return Update(AttributeLists, Modifiers, CustomKeyword, eventKeyword, Identifier, ParameterList, AsClause, ImplementsClause);
		}

		public EventStatementSyntax WithIdentifier(SyntaxToken identifier)
		{
			return Update(AttributeLists, Modifiers, CustomKeyword, EventKeyword, identifier, ParameterList, AsClause, ImplementsClause);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new EventStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(AttributeLists, Modifiers, CustomKeyword, EventKeyword, Identifier, parameterList, AsClause, ImplementsClause);
		}

		public new EventStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public EventStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(AttributeLists, Modifiers, CustomKeyword, EventKeyword, Identifier, ParameterList, asClause, ImplementsClause);
		}

		public EventStatementSyntax WithImplementsClause(ImplementsClauseSyntax implementsClause)
		{
			return Update(AttributeLists, Modifiers, CustomKeyword, EventKeyword, Identifier, ParameterList, AsClause, implementsClause);
		}

		public EventStatementSyntax AddImplementsClauseInterfaceMembers(params QualifiedNameSyntax[] items)
		{
			ImplementsClauseSyntax implementsClauseSyntax = ((ImplementsClause != null) ? ImplementsClause : SyntaxFactory.ImplementsClause());
			return WithImplementsClause(implementsClauseSyntax.AddInterfaceMembers(items));
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				5 => _parameterList, 
				6 => _asClause, 
				7 => _implementsClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				5 => ParameterList, 
				6 => AsClause, 
				7 => ImplementsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitEventStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitEventStatement(this);
		}

		public EventStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken customKeyword, SyntaxToken eventKeyword, SyntaxToken identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || customKeyword != CustomKeyword || eventKeyword != EventKeyword || identifier != Identifier || parameterList != ParameterList || asClause != AsClause || implementsClause != ImplementsClause)
			{
				EventStatementSyntax eventStatementSyntax = SyntaxFactory.EventStatement(attributeLists, modifiers, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(eventStatementSyntax, annotations);
				}
				return eventStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithEventKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new EventStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithEventKeyword(keyword);
		}
	}
}
