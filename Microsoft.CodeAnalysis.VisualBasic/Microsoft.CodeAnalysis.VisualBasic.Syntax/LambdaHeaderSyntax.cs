using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class LambdaHeaderSyntax : MethodBaseSyntax
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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken SubOrFunctionKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax)base.Green)._subOrFunctionKeyword, GetChildPosition(2), GetChildIndex(2));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 3);

		public SimpleAsClauseSyntax AsClause => GetRed(ref _asClause, 4);

		public override SyntaxToken DeclarationKeyword => SubOrFunctionKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal LambdaHeaderSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal LambdaHeaderSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, subOrFunctionKeyword, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null, (asClause != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)asClause.Green) : null), null, 0)
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

		public new LambdaHeaderSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(Kind(), attributeLists, Modifiers, SubOrFunctionKeyword, ParameterList, AsClause);
		}

		public new LambdaHeaderSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new LambdaHeaderSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(Kind(), AttributeLists, modifiers, SubOrFunctionKeyword, ParameterList, AsClause);
		}

		public new LambdaHeaderSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public LambdaHeaderSyntax WithSubOrFunctionKeyword(SyntaxToken subOrFunctionKeyword)
		{
			return Update(Kind(), AttributeLists, Modifiers, subOrFunctionKeyword, ParameterList, AsClause);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new LambdaHeaderSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, parameterList, AsClause);
		}

		public new LambdaHeaderSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			ParameterListSyntax parameterListSyntax = ((ParameterList != null) ? ParameterList : SyntaxFactory.ParameterList());
			return WithParameterList(parameterListSyntax.AddParameters(items));
		}

		internal override MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items)
		{
			return AddParameterListParameters(items);
		}

		public LambdaHeaderSyntax WithAsClause(SimpleAsClauseSyntax asClause)
		{
			return Update(Kind(), AttributeLists, Modifiers, SubOrFunctionKeyword, ParameterList, asClause);
		}

		internal override SyntaxNode GetCachedSlot(int i)
		{
			return i switch
			{
				0 => _attributeLists, 
				3 => _parameterList, 
				4 => _asClause, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				3 => ParameterList, 
				4 => AsClause, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitLambdaHeader(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitLambdaHeader(this);
		}

		public LambdaHeaderSyntax Update(SyntaxKind kind, SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			if (kind != Kind() || attributeLists != AttributeLists || modifiers != Modifiers || subOrFunctionKeyword != SubOrFunctionKeyword || parameterList != ParameterList || asClause != AsClause)
			{
				LambdaHeaderSyntax lambdaHeaderSyntax = SyntaxFactory.LambdaHeader(kind, attributeLists, modifiers, subOrFunctionKeyword, parameterList, asClause);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(lambdaHeaderSyntax, annotations);
				}
				return lambdaHeaderSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new LambdaHeaderSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithSubOrFunctionKeyword(keyword);
		}
	}
}
