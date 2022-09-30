using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class AccessorStatementSyntax : MethodBaseSyntax
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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken AccessorKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax)base.Green)._accessorKeyword, GetChildPosition(2), GetChildIndex(2));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 3);

		public override SyntaxToken DeclarationKeyword => AccessorKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal AccessorStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal AccessorStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, accessorKeyword, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null), null, 0)
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

		public new AccessorStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(Kind(), attributeLists, Modifiers, AccessorKeyword, ParameterList);
		}

		public new AccessorStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new AccessorStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(Kind(), AttributeLists, modifiers, AccessorKeyword, ParameterList);
		}

		public new AccessorStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public AccessorStatementSyntax WithAccessorKeyword(SyntaxToken accessorKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, accessorKeyword, ParameterList);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new AccessorStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, AccessorKeyword, parameterList);
		}

		public new AccessorStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				3 => _parameterList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				3 => ParameterList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitAccessorStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitAccessorStatement(this);
		}

		public AccessorStatementSyntax Update(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken accessorKeyword, ParameterListSyntax parameterList)
		{
			if (kind != Kind() || attributeLists != AttributeLists || modifiers != Modifiers || accessorKeyword != AccessorKeyword || parameterList != ParameterList)
			{
				AccessorStatementSyntax accessorStatementSyntax = SyntaxFactory.AccessorStatement(kind, attributeLists, modifiers, accessorKeyword, parameterList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(accessorStatementSyntax, annotations);
				}
				return accessorStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithAccessorKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new AccessorStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithAccessorKeyword(keyword);
		}
	}
}
