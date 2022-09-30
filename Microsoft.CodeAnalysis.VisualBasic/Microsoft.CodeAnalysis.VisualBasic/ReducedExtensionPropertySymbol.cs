using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ReducedExtensionPropertySymbol : PropertySymbol
	{
		private sealed class ReducedExtensionAccessorSymbol : MethodSymbol
		{
			private readonly ReducedExtensionPropertySymbol _associatedProperty;

			private readonly MethodSymbol _originalDefinition;

			private ImmutableArray<ParameterSymbol> _lazyParameters;

			internal override MethodSymbol CallsiteReducedFromMethod => _originalDefinition;

			public override MethodSymbol ReducedFrom => _originalDefinition;

			public override int Arity => 0;

			public override Symbol AssociatedSymbol => _associatedProperty;

			internal override bool HasSpecialName => _originalDefinition.HasSpecialName;

			internal override CallingConvention CallingConvention => CallingConvention.HasThis;

			public override Symbol ContainingSymbol => _originalDefinition.ContainingSymbol;

			public override Accessibility DeclaredAccessibility => _originalDefinition.DeclaredAccessibility;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

			public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

			public override bool IsExtensionMethod => false;

			public override bool IsExternalMethod => _originalDefinition.IsExternalMethod;

			internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => _originalDefinition.ReturnTypeMarshallingInformation;

			internal override MethodImplAttributes ImplementationAttributes => _originalDefinition.ImplementationAttributes;

			internal override bool HasDeclarativeSecurity => _originalDefinition.HasDeclarativeSecurity;

			internal override ObsoleteAttributeData ObsoleteAttributeData => _originalDefinition.ObsoleteAttributeData;

			public override bool IsMustOverride => false;

			public override bool IsNotOverridable => false;

			public override bool IsOverloads => false;

			public override bool IsOverridable => false;

			public override bool IsOverrides => false;

			public override bool IsShared => false;

			public override bool IsSub => _originalDefinition.IsSub;

			public override bool IsAsync => false;

			public override bool IsIterator => false;

			public override bool IsInitOnly => _originalDefinition.IsInitOnly;

			public override bool IsVararg => _originalDefinition.IsVararg;

			public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

			public override MethodKind MethodKind => _originalDefinition.MethodKind;

			internal override bool IsMethodKindBasedOnSyntax => _originalDefinition.IsMethodKindBasedOnSyntax;

			internal override int ParameterCount => _originalDefinition.ParameterCount - 1;

			public override ImmutableArray<ParameterSymbol> Parameters
			{
				get
				{
					if (_lazyParameters.IsDefault)
					{
						ImmutableInterlocked.InterlockedInitialize(ref _lazyParameters, ReducedAccessorParameterSymbol.MakeParameters(this, _originalDefinition.Parameters));
					}
					return _lazyParameters;
				}
			}

			public override bool ReturnsByRef => _originalDefinition.ReturnsByRef;

			public override TypeSymbol ReturnType => _originalDefinition.ReturnType;

			public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => _originalDefinition.ReturnTypeCustomModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _originalDefinition.RefCustomModifiers;

			internal override SyntaxNode Syntax => _originalDefinition.Syntax;

			public override ImmutableArray<TypeSymbol> TypeArguments => _originalDefinition.TypeArguments;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _originalDefinition.TypeParameters;

			internal override bool GenerateDebugInfoImpl => _originalDefinition.GenerateDebugInfo;

			public ReducedExtensionAccessorSymbol(ReducedExtensionPropertySymbol associatedProperty, MethodSymbol originalDefinition)
			{
				_associatedProperty = associatedProperty;
				_originalDefinition = originalDefinition;
			}

			public override DllImportData GetDllImportData()
			{
				return _originalDefinition.GetDllImportData();
			}

			internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
			{
				return _originalDefinition.GetSecurityInformation();
			}

			internal override ImmutableArray<string> GetAppliedConditionalSymbols()
			{
				return _originalDefinition.GetAppliedConditionalSymbols();
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

		private sealed class ReducedAccessorParameterSymbol : ReducedParameterSymbolBase
		{
			private readonly Symbol _propertyOrAccessor;

			public override Symbol ContainingSymbol => _propertyOrAccessor;

			public override TypeSymbol Type => m_CurriedFromParameter.Type;

			public static ImmutableArray<ParameterSymbol> MakeParameters(Symbol propertyOrAccessor, ImmutableArray<ParameterSymbol> originalParameters)
			{
				int length = originalParameters.Length;
				if (length <= 1)
				{
					return ImmutableArray<ParameterSymbol>.Empty;
				}
				ParameterSymbol[] array = new ParameterSymbol[length - 2 + 1];
				int num = length - 2;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new ReducedAccessorParameterSymbol(propertyOrAccessor, originalParameters[i + 1]);
				}
				return array.AsImmutableOrNull();
			}

			public ReducedAccessorParameterSymbol(Symbol propertyOrAccessor, ParameterSymbol underlyingParameter)
				: base(underlyingParameter)
			{
				_propertyOrAccessor = propertyOrAccessor;
			}
		}

		private readonly PropertySymbol _originalDefinition;

		internal override PropertySymbol ReducedFrom => _originalDefinition;

		internal override PropertySymbol ReducedFromDefinition => _originalDefinition;

		internal override TypeSymbol ReceiverType => _originalDefinition.Parameters[0].Type;

		public override string Name => _originalDefinition.Name;

		internal override bool HasSpecialName => _originalDefinition.HasSpecialName;

		internal override CallingConvention CallingConvention => _originalDefinition.CallingConvention;

		public override Symbol ContainingSymbol => _originalDefinition.ContainingSymbol;

		public override Accessibility DeclaredAccessibility => _originalDefinition.DeclaredAccessibility;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

		public override MethodSymbol GetMethod => ReduceAccessorIfAny(_originalDefinition.GetMethod);

		internal override FieldSymbol AssociatedField => _originalDefinition.AssociatedField;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _originalDefinition.ObsoleteAttributeData;

		public override bool IsDefault => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

		public override int ParameterCount => 0;

		public override MethodSymbol SetMethod => ReduceAccessorIfAny(_originalDefinition.SetMethod);

		public override bool ReturnsByRef => _originalDefinition.ReturnsByRef;

		public override TypeSymbol Type => _originalDefinition.Type;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => _originalDefinition.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _originalDefinition.RefCustomModifiers;

		internal override bool IsMyGroupCollectionProperty => false;

		public ReducedExtensionPropertySymbol(PropertySymbol originalDefinition)
		{
			_originalDefinition = originalDefinition;
		}

		private ReducedExtensionAccessorSymbol ReduceAccessorIfAny(MethodSymbol methodOpt)
		{
			if ((object)methodOpt != null)
			{
				return new ReducedExtensionAccessorSymbol(this, methodOpt);
			}
			return null;
		}
	}
}
