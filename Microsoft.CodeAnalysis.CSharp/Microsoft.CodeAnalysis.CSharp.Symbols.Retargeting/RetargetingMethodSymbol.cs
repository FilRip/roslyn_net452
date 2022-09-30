using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    internal sealed class RetargetingMethodSymbol : WrappedMethodSymbol
    {
        private readonly RetargetingModuleSymbol _retargetingModule;

        private readonly MethodSymbol _underlyingMethod;

        private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

        private ImmutableArray<ParameterSymbol> _lazyParameters;

        private ImmutableArray<CustomModifier> _lazyRefCustomModifiers;

        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        private ImmutableArray<CSharpAttributeData> _lazyReturnTypeCustomAttributes;

        private ImmutableArray<MethodSymbol> _lazyExplicitInterfaceImplementations;

        private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;

        private TypeWithAnnotations.Boxed _lazyReturnType;

        private UnmanagedCallersOnlyAttributeData _lazyUnmanagedAttributeData = UnmanagedCallersOnlyAttributeData.Uninitialized;

        private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

        public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

        public override MethodSymbol UnderlyingMethod => _underlyingMethod;

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

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations
        {
            get
            {
                if (IsGenericMethod)
                {
                    return GetTypeParametersAsTypeArguments();
                }
                return ImmutableArray<TypeWithAnnotations>.Empty;
            }
        }

        public override TypeWithAnnotations ReturnTypeWithAnnotations
        {
            get
            {
                if (_lazyReturnType == null)
                {
                    Interlocked.CompareExchange(ref _lazyReturnType, new TypeWithAnnotations.Boxed(RetargetingTranslator.Retarget(_underlyingMethod.ReturnTypeWithAnnotations, RetargetOptions.RetargetPrimitiveTypesByTypeCode, ContainingType)), null);
                }
                return _lazyReturnType.Value;
            }
        }

        public override ImmutableArray<CustomModifier> RefCustomModifiers => RetargetingTranslator.RetargetModifiers(_underlyingMethod.RefCustomModifiers, ref _lazyRefCustomModifiers);

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

        public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingMethod.ContainingSymbol);

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => _retargetingModule.RetargetingTranslator.Retarget(_underlyingMethod.ReturnValueMarshallingInformation);

        public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

        internal override ModuleSymbol ContainingModule => _retargetingModule;

        internal override bool IsExplicitInterfaceImplementation => _underlyingMethod.IsExplicitInterfaceImplementation;

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

        internal MethodSymbol ExplicitlyOverriddenClassMethod
        {
            get
            {
                if (!_underlyingMethod.RequiresExplicitOverride(out var _))
                {
                    return null;
                }
                return RetargetingTranslator.Retarget(_underlyingMethod.OverriddenMethod, MemberSignatureComparer.RetargetedExplicitImplementationComparer);
            }
        }

        internal sealed override CSharpCompilation DeclaringCompilation => null;

        internal override bool GenerateDebugInfo => false;

        public RetargetingMethodSymbol(RetargetingModuleSymbol retargetingModule, MethodSymbol underlyingMethod)
        {
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
            ParameterSymbol[] array = new ParameterSymbol[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = new RetargetingMethodParameterSymbol(this, parameters[i]);
            }
            return array.AsImmutableOrNull();
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return RetargetingTranslator.GetRetargetedAttributes(_underlyingMethod.GetAttributes(), ref _lazyCustomAttributes);
        }

        internal override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return RetargetingTranslator.RetargetAttributes(_underlyingMethod.GetCustomAttributesToEmit(moduleBuilder));
        }

        public override ImmutableArray<CSharpAttributeData> GetReturnTypeAttributes()
        {
            return RetargetingTranslator.GetRetargetedAttributes(_underlyingMethod.GetReturnTypeAttributes(), ref _lazyReturnTypeCustomAttributes);
        }

        internal override UnmanagedCallersOnlyAttributeData? GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            if (_lazyUnmanagedAttributeData == UnmanagedCallersOnlyAttributeData.Uninitialized)
            {
                UnmanagedCallersOnlyAttributeData unmanagedCallersOnlyAttributeData = _underlyingMethod.GetUnmanagedCallersOnlyAttributeData(forceComplete);
                if (unmanagedCallersOnlyAttributeData == UnmanagedCallersOnlyAttributeData.Uninitialized || unmanagedCallersOnlyAttributeData == UnmanagedCallersOnlyAttributeData.AttributePresentDataNotBound)
                {
                    return unmanagedCallersOnlyAttributeData;
                }
                if (unmanagedCallersOnlyAttributeData != null && !unmanagedCallersOnlyAttributeData.CallingConventionTypes.IsEmpty)
                {
                    PooledHashSet<INamedTypeSymbolInternal> instance = PooledHashSet<INamedTypeSymbolInternal>.GetInstance();
                    foreach (INamedTypeSymbolInternal callingConventionType in unmanagedCallersOnlyAttributeData.CallingConventionTypes)
                    {
                        instance.Add((INamedTypeSymbolInternal)RetargetingTranslator.Retarget((NamedTypeSymbol)callingConventionType));
                    }
                    unmanagedCallersOnlyAttributeData = UnmanagedCallersOnlyAttributeData.Create(instance.ToImmutableHashSet());
                    instance.Free();
                }
                Interlocked.CompareExchange(ref _lazyUnmanagedAttributeData, unmanagedCallersOnlyAttributeData, UnmanagedCallersOnlyAttributeData.Uninitialized);
            }
            return _lazyUnmanagedAttributeData;
        }

        private ImmutableArray<MethodSymbol> RetargetExplicitInterfaceImplementations()
        {
            ImmutableArray<MethodSymbol> explicitInterfaceImplementations = _underlyingMethod.ExplicitInterfaceImplementations;
            if (explicitInterfaceImplementations.IsEmpty)
            {
                return explicitInterfaceImplementations;
            }
            ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
            for (int i = 0; i < explicitInterfaceImplementations.Length; i++)
            {
                MethodSymbol methodSymbol = RetargetingTranslator.Retarget(explicitInterfaceImplementations[i], MemberSignatureComparer.RetargetedExplicitImplementationComparer);
                if ((object)methodSymbol != null)
                {
                    instance.Add(methodSymbol);
                }
            }
            return instance.ToImmutableAndFree();
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            if (!_lazyCachedUseSiteInfo.IsInitialized)
            {
                AssemblySymbol primaryDependency = base.PrimaryDependency;
                UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(primaryDependency);
                CalculateUseSiteDiagnostic(ref result);
                _lazyCachedUseSiteInfo.Initialize(primaryDependency, result);
            }
            return _lazyCachedUseSiteInfo.ToUseSiteInfo(base.PrimaryDependency);
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
