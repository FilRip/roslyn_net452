using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedDelegateMethodSymbol : MethodSymbol
	{
		private readonly string _name;

		private readonly NamedTypeSymbol _containingType;

		private readonly TypeSymbol _returnType;

		private readonly SourceMemberFlags _flags;

		private ImmutableArray<ParameterSymbol> _parameters;

		public override int Arity => 0;

		public override Symbol AssociatedSymbol => null;

		internal override CallingConvention CallingConvention => CallingConvention.HasThis;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override Accessibility DeclaredAccessibility => Accessibility.Public;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override bool IsExtensionMethod => false;

		public override bool IsExternalMethod => true;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => null;

		internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.CodeTypeMask;

		internal override bool HasDeclarativeSecurity => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => (_flags & SourceMemberFlags.Overridable) != 0;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public override bool IsSub => _returnType.SpecialType == SpecialType.System_Void;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public override bool IsInitOnly => false;

		public override bool IsVararg => false;

		public override ImmutableArray<Location> Locations => _containingType.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override MethodKind MethodKind => SourceMemberFlagsExtensions.ToMethodKind(_flags);

		internal override bool IsMethodKindBasedOnSyntax => false;

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (!_parameters.IsDefault)
				{
					return _parameters;
				}
				return ImmutableArray<ParameterSymbol>.Empty;
			}
		}

		public override bool ReturnsByRef => false;

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override bool IsImplicitlyDeclared => true;

		public override string Name => _name;

		internal override bool HasSpecialName => MethodKind == MethodKind.Constructor;

		internal override SyntaxNode Syntax => null;

		internal override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal override bool GenerateDebugInfoImpl => false;

		public SynthesizedDelegateMethodSymbol(string name, NamedTypeSymbol containingSymbol, SourceMemberFlags flags, TypeSymbol returnType)
		{
			_name = name;
			_containingType = containingSymbol;
			_flags = flags;
			_returnType = returnType;
		}

		internal void SetParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			_parameters = parameters;
		}

		public override DllImportData GetDllImportData()
		{
			return null;
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return _containingType.GetLexicalSortKey();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
