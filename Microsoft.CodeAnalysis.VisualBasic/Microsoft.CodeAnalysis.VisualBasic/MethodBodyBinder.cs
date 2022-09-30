using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class MethodBodyBinder : SubOrFunctionBodyBinder
	{
		private readonly LocalSymbol _functionValue;

		public override bool IsInQuery => false;

		internal override bool SuppressCallerInfo
		{
			get
			{
				if (((MethodSymbol)ContainingMember).IsImplicitlyDeclared)
				{
					return ContainingMember is SynthesizedMyGroupCollectionPropertyAccessorSymbol;
				}
				return false;
			}
		}

		public MethodBodyBinder(MethodSymbol methodSymbol, SyntaxNode root, Binder containingBinder)
			: base(methodSymbol, root, containingBinder)
		{
			_functionValue = CreateFunctionValueLocal(methodSymbol, root);
			if ((object)_functionValue != null && !MethodSymbolExtensions.IsUserDefinedOperator(methodSymbol))
			{
				string name = _functionValue.Name;
				if (!string.IsNullOrEmpty(name))
				{
					_parameterMap[name] = _functionValue;
				}
			}
		}

		private LocalSymbol CreateFunctionValueLocal(MethodSymbol methodSymbol, SyntaxNode root)
		{
			if (!(root is MethodBlockBaseSyntax methodBlockBaseSyntax))
			{
				return null;
			}
			switch (methodBlockBaseSyntax.Kind())
			{
			case SyntaxKind.FunctionBlock:
			{
				MethodStatementSyntax subOrFunctionStatement = ((MethodBlockSyntax)methodBlockBaseSyntax).SubOrFunctionStatement;
				return LocalSymbol.Create(methodSymbol, this, subOrFunctionStatement.Identifier, LocalDeclarationKind.FunctionValue, TypeSymbolExtensions.IsVoidType(methodSymbol.ReturnType) ? ErrorTypeSymbol.UnknownResultType : methodSymbol.ReturnType);
			}
			case SyntaxKind.GetAccessorBlock:
				if (methodBlockBaseSyntax.Parent != null && methodBlockBaseSyntax.Parent.Kind() == SyntaxKind.PropertyBlock)
				{
					SyntaxToken identifier2 = ((PropertyBlockSyntax)methodBlockBaseSyntax.Parent).PropertyStatement.Identifier;
					return LocalSymbol.Create(methodSymbol, this, identifier2, LocalDeclarationKind.FunctionValue, methodSymbol.ReturnType);
				}
				break;
			case SyntaxKind.OperatorBlock:
				return new SynthesizedLocal(methodSymbol, methodSymbol.ReturnType, SynthesizedLocalKind.FunctionReturnValue, ((OperatorBlockSyntax)methodBlockBaseSyntax).BlockStatement);
			case SyntaxKind.AddHandlerAccessorBlock:
				if (((EventSymbol)methodSymbol.AssociatedSymbol).IsWindowsRuntimeEvent && methodBlockBaseSyntax.Parent != null && methodBlockBaseSyntax.Parent.Kind() == SyntaxKind.EventBlock)
				{
					SyntaxToken identifier = ((EventBlockSyntax)methodBlockBaseSyntax.Parent).EventStatement.Identifier;
					return LocalSymbol.Create(methodSymbol, this, identifier, LocalDeclarationKind.FunctionValue, methodSymbol.ReturnType, methodSymbol.Name);
				}
				break;
			}
			return null;
		}

		public override LocalSymbol GetLocalForFunctionValue()
		{
			return _functionValue;
		}
	}
}
