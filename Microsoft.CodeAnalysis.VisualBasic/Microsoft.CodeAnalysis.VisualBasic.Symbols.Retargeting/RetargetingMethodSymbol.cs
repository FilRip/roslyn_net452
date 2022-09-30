using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingMethodSymbol : MethodSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly MethodSymbol _underlyingMethod;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private CustomModifiersTuple _lazyCustomModifiers;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ImmutableArray<VisualBasicAttributeData> _lazyReturnTypeCustomAttributes;

		private ImmutableArray<MethodSymbol> _lazyExplicitInterfaceImplementations;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public MethodSymbol UnderlyingMethod => _underlyingMethod;

		public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

		public override bool IsVararg => _underlyingMethod.IsVararg;

		public override bool IsGenericMethod => _underlyingMethod.IsGenericMethod;

		public override int Arity => _underlyingMethod.Arity;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				if (_lazyTypeParameters.IsDefault)
				{
					if (!IsGenericMethod)
					{
						_lazyTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
					}
					else
					{
						ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, RetargetingTranslator.Retarget(_underlyingMethod.TypeParameters), default(ImmutableArray<TypeParameterSymbol>));
					}
				}
				return _lazyTypeParameters;
			}
		}

		public override ImmutableArray<TypeSymbol> TypeArguments
		{
			get
			{
				if (IsGenericMethod)
				{
					return StaticCast<TypeSymbol>.From(TypeParameters);
				}
				return ImmutableArray<TypeSymbol>.Empty;
			}
		}

		public override bool IsSub => _underlyingMethod.IsSub;

		public override bool IsAsync => _underlyingMethod.IsAsync;

		public override bool IsIterator => _underlyingMethod.IsIterator;

		public override bool IsInitOnly => _underlyingMethod.IsInitOnly;

		public override bool ReturnsByRef => _underlyingMethod.ReturnsByRef;

		public override TypeSymbol ReturnType => RetargetingTranslator.Retarget(_underlyingMethod.ReturnType, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => CustomModifiersTuple.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => CustomModifiersTuple.RefCustomModifiers;

		private CustomModifiersTuple CustomModifiersTuple => RetargetingTranslator.RetargetModifiers(_underlyingMethod.ReturnTypeCustomModifiers, _underlyingMethod.RefCustomModifiers, ref _lazyCustomModifiers);

		internal override int ParameterCount => _underlyingMethod.ParameterCount;

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyParameters, RetargetParameters(), default(ImmutableArray<ParameterSymbol>));
				}
				return _lazyParameters;
			}
		}

		public override Symbol AssociatedSymbol
		{
			get
			{
				Symbol associatedSymbol = _underlyingMethod.AssociatedSymbol;
				if ((object)associatedSymbol != null)
				{
					return RetargetingTranslator.Retarget(associatedSymbol);
				}
				return null;
			}
		}

		public override bool IsExtensionMethod => _underlyingMethod.IsExtensionMethod;

		internal override bool MayBeReducibleExtensionMethod => _underlyingMethod.MayBeReducibleExtensionMethod;

		public override bool IsOverloads => _underlyingMethod.IsOverloads;

		internal override bool IsHiddenBySignature => _underlyingMethod.IsHiddenBySignature;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingMethod.ContainingSymbol);

		public override ImmutableArray<Location> Locations => _underlyingMethod.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingMethod.DeclaringSyntaxReferences;

		public override Accessibility DeclaredAccessibility => _underlyingMethod.DeclaredAccessibility;

		public override bool IsShared => _underlyingMethod.IsShared;

		public override bool IsOverridable => _underlyingMethod.IsOverridable;

		public override bool IsOverrides => _underlyingMethod.IsOverrides;

		public override bool IsMustOverride => _underlyingMethod.IsMustOverride;

		public override bool IsNotOverridable => _underlyingMethod.IsNotOverridable;

		public override bool IsExternalMethod => _underlyingMethod.IsExternalMethod;

		internal override bool ReturnValueIsMarshalledExplicitly => _underlyingMethod.ReturnValueIsMarshalledExplicitly;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => RetargetingTranslator.Retarget(_underlyingMethod.ReturnTypeMarshallingInformation);

		internal override ImmutableArray<byte> ReturnValueMarshallingDescriptor => _underlyingMethod.ReturnValueMarshallingDescriptor;

		internal override bool IsAccessCheckedOnOverride => _underlyingMethod.IsAccessCheckedOnOverride;

		internal override bool IsExternal => _underlyingMethod.IsExternal;

		internal override MethodImplAttributes ImplementationAttributes => _underlyingMethod.ImplementationAttributes;

		internal override bool HasDeclarativeSecurity => _underlyingMethod.HasDeclarativeSecurity;

		public override bool IsImplicitlyDeclared => _underlyingMethod.IsImplicitlyDeclared;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingMethod.ObsoleteAttributeData;

		public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _retargetingModule;

		public override string Name => _underlyingMethod.Name;

		public override string MetadataName => _underlyingMethod.MetadataName;

		internal override bool HasSpecialName => _underlyingMethod.HasSpecialName;

		internal override bool HasRuntimeSpecialName => _underlyingMethod.HasRuntimeSpecialName;

		internal override bool IsMetadataFinal => _underlyingMethod.IsMetadataFinal;

		public override MethodKind MethodKind => _underlyingMethod.MethodKind;

		internal override bool IsMethodKindBasedOnSyntax => _underlyingMethod.IsMethodKindBasedOnSyntax;

		internal override CallingConvention CallingConvention => _underlyingMethod.CallingConvention;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitInterfaceImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitInterfaceImplementations, RetargetExplicitInterfaceImplementations(), default(ImmutableArray<MethodSymbol>));
				}
				return _lazyExplicitInterfaceImplementations;
			}
		}

		internal override SyntaxNode Syntax => null;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal override bool GenerateDebugInfoImpl => false;

		public RetargetingMethodSymbol(RetargetingModuleSymbol retargetingModule, MethodSymbol underlyingMethod)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			if (underlyingMethod is RetargetingMethodSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingMethod = underlyingMethod;
		}

		private ImmutableArray<ParameterSymbol> RetargetParameters()
		{
			ImmutableArray<ParameterSymbol> parameters = _underlyingMethod.Parameters;
			int length = parameters.Length;
			if (length == 0)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = RetargetingParameterSymbol.CreateMethodParameter(this, parameters[i]);
			}
			return array.AsImmutableOrNull();
		}

		public override DllImportData GetDllImportData()
		{
			return _underlyingMethod.GetDllImportData();
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return _underlyingMethod.IsMetadataNewSlot(ignoreInterfaceImplementationChanges);
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return _underlyingMethod.GetSecurityInformation();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingMethod, ref _lazyCustomAttributes);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingMethod, ref _lazyReturnTypeCustomAttributes, getReturnTypeAttributes: true);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingMethod.GetCustomAttributesToEmit(compilationState));
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return _underlyingMethod.GetAppliedConditionalSymbols();
		}

		private ImmutableArray<MethodSymbol> RetargetExplicitInterfaceImplementations()
		{
			ImmutableArray<MethodSymbol> explicitInterfaceImplementations = UnderlyingMethod.ExplicitInterfaceImplementations;
			if (explicitInterfaceImplementations.IsEmpty)
			{
				return explicitInterfaceImplementations;
			}
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			int num = explicitInterfaceImplementations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				MethodSymbol methodSymbol = RetargetingTranslator.Retarget(explicitInterfaceImplementations[i], MethodSignatureComparer.RetargetedExplicitMethodImplementationComparer);
				if ((object)methodSymbol != null)
				{
					instance.Add(methodSymbol);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			AssemblySymbol primaryDependency = base.PrimaryDependency;
			if (!_lazyCachedUseSiteInfo.IsInitialized)
			{
				_lazyCachedUseSiteInfo.Initialize(primaryDependency, CalculateUseSiteInfo());
			}
			return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingMethod.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
