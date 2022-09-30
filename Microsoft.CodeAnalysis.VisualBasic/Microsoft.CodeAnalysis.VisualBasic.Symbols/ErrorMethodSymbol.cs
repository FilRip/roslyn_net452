using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ErrorMethodSymbol : MethodSymbol
	{
		public static readonly ErrorMethodSymbol UnknownMethod = new ErrorMethodSymbol(ErrorTypeSymbol.UnknownResultType, ErrorTypeSymbol.UnknownResultType, string.Empty);

		private readonly TypeSymbol _containingType;

		private readonly TypeSymbol _returnType;

		private readonly string _name;

		public override int Arity => 0;

		public override Symbol AssociatedSymbol => null;

		internal override CallingConvention CallingConvention => CallingConvention.Default;

		public override Symbol ContainingSymbol => _containingType;

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		internal override bool HasDeclarativeSecurity => false;

		internal override bool HasSpecialName => false;

		internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

		public override bool IsAsync => false;

		public override bool IsExtensionMethod => false;

		public override bool IsExternalMethod => false;

		public override bool IsIterator => false;

		public override bool IsInitOnly => false;

		internal override bool IsMethodKindBasedOnSyntax => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public override bool IsSub => _returnType.SpecialType == SpecialType.System_Void;

		public override bool IsVararg => false;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override MethodKind MethodKind => MethodKind.Ordinary;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override bool ReturnsByRef => false;

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => null;

		internal override SyntaxNode Syntax => null;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		internal override bool GenerateDebugInfoImpl => false;

		public ErrorMethodSymbol(TypeSymbol containingType, TypeSymbol returnType, string name)
		{
			_containingType = containingType;
			_returnType = returnType;
			_name = name;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		public override DllImportData GetDllImportData()
		{
			return null;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
