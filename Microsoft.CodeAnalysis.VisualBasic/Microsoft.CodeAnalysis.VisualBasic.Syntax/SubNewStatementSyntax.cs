using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public sealed class SubNewStatementSyntax : MethodBaseSyntax
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
				GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)base.Green)._modifiers;
				return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
			}
		}

		public SyntaxToken SubKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)base.Green)._subKeyword, GetChildPosition(2), GetChildIndex(2));

		public SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax)base.Green)._newKeyword, GetChildPosition(3), GetChildIndex(3));

		public new ParameterListSyntax ParameterList => GetRed(ref _parameterList, 4);

		public override SyntaxToken DeclarationKeyword => SubKeyword;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SyntaxToken Keyword => DeclarationKeyword;

		internal SubNewStatementSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal SubNewStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SyntaxNode attributeLists, GreenNode modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList)
			: this(new Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(kind, errors, annotations, attributeLists?.Green, modifiers, subKeyword, newKeyword, (parameterList != null) ? ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)parameterList.Green) : null), null, 0)
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

		public new SubNewStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return Update(attributeLists, Modifiers, SubKeyword, NewKeyword, ParameterList);
		}

		public new SubNewStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
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

		public new SubNewStatementSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return Update(AttributeLists, modifiers, SubKeyword, NewKeyword, ParameterList);
		}

		public new SubNewStatementSyntax AddModifiers(params SyntaxToken[] items)
		{
			return WithModifiers(Modifiers.AddRange(items));
		}

		internal override MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items)
		{
			return AddModifiers(items);
		}

		public SubNewStatementSyntax WithSubKeyword(SyntaxToken subKeyword)
		{
			return Update(AttributeLists, Modifiers, subKeyword, NewKeyword, ParameterList);
		}

		public SubNewStatementSyntax WithNewKeyword(SyntaxToken newKeyword)
		{
			return Update(AttributeLists, Modifiers, SubKeyword, newKeyword, ParameterList);
		}

		internal override ParameterListSyntax GetParameterListCore()
		{
			return ParameterList;
		}

		internal override MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList)
		{
			return WithParameterList(parameterList);
		}

		public new SubNewStatementSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return Update(AttributeLists, Modifiers, SubKeyword, NewKeyword, parameterList);
		}

		public new SubNewStatementSyntax AddParameterListParameters(params ParameterSyntax[] items)
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
				4 => _parameterList, 
				_ => null, 
			};
		}

		internal override SyntaxNode GetNodeSlot(int i)
		{
			return i switch
			{
				0 => GetRedAtZero(ref _attributeLists), 
				4 => ParameterList, 
				_ => null, 
			};
		}

		public override TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor)
		{
			return visitor.VisitSubNewStatement(this);
		}

		public override void Accept(VisualBasicSyntaxVisitor visitor)
		{
			visitor.VisitSubNewStatement(this);
		}

		public SubNewStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken subKeyword, SyntaxToken newKeyword, ParameterListSyntax parameterList)
		{
			if (attributeLists != AttributeLists || modifiers != Modifiers || subKeyword != SubKeyword || newKeyword != NewKeyword || parameterList != ParameterList)
			{
				SubNewStatementSyntax subNewStatementSyntax = SyntaxFactory.SubNewStatement(attributeLists, modifiers, subKeyword, newKeyword, parameterList);
				SyntaxAnnotation[] annotations = GetAnnotations();
				if (annotations != null && annotations.Length > 0)
				{
					return SyntaxNodeExtensions.WithAnnotations(subNewStatementSyntax, annotations);
				}
				return subNewStatementSyntax;
			}
			return this;
		}

		public override MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword)
		{
			return WithSubKeyword(keyword);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete.", true)]
		public new SubNewStatementSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithSubKeyword(keyword);
		}
	}
}
