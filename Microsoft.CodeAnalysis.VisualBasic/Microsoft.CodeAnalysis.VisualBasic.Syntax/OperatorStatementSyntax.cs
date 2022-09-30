using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class OperatorStatementSyntax : MethodBaseSyntax
	{
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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken OperatorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax)base.Green)._operatorKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax)base.Green)._operatorToken, GetChildPosition(3), GetChildIndex(3));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 4);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 5);

		public override SyntaxToken DeclarationKeyword => OperatorKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal OperatorStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal OperatorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax operatorKeyword, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, operatorKeyword, operatorToken, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
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

		public new OperatorStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, OperatorKeyword, OperatorToken, ParameterList, AsClause);
		}

		public new OperatorStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new OperatorStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, OperatorKeyword, OperatorToken, ParameterList, AsClause);
		}

		public new OperatorStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public OperatorStatementSyntax WithOperatorKeyword(SyntaxToken operatorKeyword)
		{
			return Update(AttributeLists, Modifiers, operatorKeyword, OperatorToken, ParameterList, AsClause);
		}

		public OperatorStatementSyntax WithOperatorToken(SyntaxToken operatorToken)
		{
			return Update(AttributeLists, Modifiers, OperatorKeyword, operatorToken, ParameterList, AsClause);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new OperatorStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(AttributeLists, Modifiers, OperatorKeyword, OperatorToken, parameterList, AsClause);
		}

		public new OperatorStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public OperatorStatementSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(AttributeLists, Modifiers, OperatorKeyword, OperatorToken, ParameterList, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				4 => _parameterList, 
				5 => _asClause, 
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
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitOperatorStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitOperatorStatement(this);
		}

		public OperatorStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || operatorKeyword != OperatorKeyword || operatorToken != OperatorToken || parameterList != ParameterList || asClause != AsClause)
			{
				OperatorStatementSyntax operatorStatementSyntax = SyntaxFactory.OperatorStatement(attributeLists, modifiers, operatorKeyword, operatorToken, parameterList, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(operatorStatementSyntax, annotations);
				}
				return operatorStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithOperatorKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new OperatorStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithOperatorKeyword(keyword);
		}
	}
}
