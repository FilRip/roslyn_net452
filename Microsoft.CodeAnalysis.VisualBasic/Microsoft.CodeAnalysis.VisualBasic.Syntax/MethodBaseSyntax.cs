using System;
using System.ComponentModel;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public abstract class MethodBaseSyntax : DeclarationStatementSyntax
	{
		internal SyntaxNode _attributeLists;

		internal ParameterListSyntax _parameterList;

		public SyntaxList<AttributeListSyntax> AttributeLists => GetAttributeListsCore();

		public SyntaxTokenList Modifiers => GetModifiersCore();

		public ParameterListSyntax ParameterList => GetParameterListCore();

		public abstract SyntaxToken DeclarationKeyword { get; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. SubOrFunctionKeyword) instead.", true)]
		public SyntaxToken Keyword => DeclarationKeyword;

		internal AsClauseSyntax AsClauseInternal
		{
			get
			{
				switch (Kind())
				{
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
					return ((MethodStatementSyntax)this).AsClause;
				case SyntaxKind.SubLambdaHeader:
				case SyntaxKind.FunctionLambdaHeader:
					return ((LambdaHeaderSyntax)this).AsClause;
				case SyntaxKind.DeclareSubStatement:
				case SyntaxKind.DeclareFunctionStatement:
					return ((DeclareStatementSyntax)this).AsClause;
				case SyntaxKind.DelegateSubStatement:
				case SyntaxKind.DelegateFunctionStatement:
					return ((DelegateStatementSyntax)this).AsClause;
				case SyntaxKind.EventStatement:
					return ((EventStatementSyntax)this).AsClause;
				case SyntaxKind.OperatorStatement:
					return ((OperatorStatementSyntax)this).AsClause;
				case SyntaxKind.PropertyStatement:
					return ((PropertyStatementSyntax)this).AsClause;
				case SyntaxKind.SubNewStatement:
				case SyntaxKind.GetAccessorStatement:
				case SyntaxKind.SetAccessorStatement:
				case SyntaxKind.AddHandlerAccessorStatement:
				case SyntaxKind.RemoveHandlerAccessorStatement:
				case SyntaxKind.RaiseEventAccessorStatement:
					return null;
				default:
					throw ExceptionUtilities.UnexpectedValue(Kind());
				}
			}
		}

		internal MethodBaseSyntax(GreenNode green, SyntaxNode parent, int startLocation)
			: base(green, parent, startLocation)
		{
		}

		internal virtual SyntaxList<AttributeListSyntax> GetAttributeListsCore()
		{
			SyntaxNode redAtZero = GetRedAtZero(ref _attributeLists);
			return new SyntaxList<AttributeListSyntax>(redAtZero);
		}

		public MethodBaseSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
		{
			return WithAttributeListsCore(attributeLists);
		}

		internal abstract MethodBaseSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists);

		public MethodBaseSyntax AddAttributeLists(params AttributeListSyntax[] items)
		{
			return AddAttributeListsCore(items);
		}

		internal abstract MethodBaseSyntax AddAttributeListsCore(params AttributeListSyntax[] items);

		internal virtual SyntaxTokenList GetModifiersCore()
		{
			GreenNode modifiers = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax)base.Green)._modifiers;
			return (modifiers == null) ? default(SyntaxTokenList) : new SyntaxTokenList(this, modifiers, GetChildPosition(1), GetChildIndex(1));
		}

		public MethodBaseSyntax WithModifiers(SyntaxTokenList modifiers)
		{
			return WithModifiersCore(modifiers);
		}

		internal abstract MethodBaseSyntax WithModifiersCore(SyntaxTokenList modifiers);

		public MethodBaseSyntax AddModifiers(params SyntaxToken[] items)
		{
			return AddModifiersCore(items);
		}

		internal abstract MethodBaseSyntax AddModifiersCore(params SyntaxToken[] items);

		internal virtual ParameterListSyntax GetParameterListCore()
		{
			return GetRed(ref _parameterList, 2);
		}

		public MethodBaseSyntax WithParameterList(ParameterListSyntax parameterList)
		{
			return WithParameterListCore(parameterList);
		}

		internal abstract MethodBaseSyntax WithParameterListCore(ParameterListSyntax parameterList);

		public MethodBaseSyntax AddParameterListParameters(params ParameterSyntax[] items)
		{
			return AddParameterListParametersCore(items);
		}

		internal abstract MethodBaseSyntax AddParameterListParametersCore(params ParameterSyntax[] items);

		public abstract MethodBaseSyntax WithDeclarationKeyword(SyntaxToken keyword);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use DeclarationKeyword or a more specific property (e.g. WithSubOrFunctionKeyword) instead.", true)]
		public MethodBaseSyntax WithKeyword(SyntaxToken keyword)
		{
			return WithDeclarationKeyword(keyword);
		}
	}
}
