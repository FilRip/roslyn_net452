using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedStaticConstructor : MethodSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        private ThreeState _lazyShouldEmit;

        public override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override string Name => ".cctor";

        internal override bool HasSpecialName => true;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        public override bool IsVararg => false;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal override int ParameterCount => 0;

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<Location> Locations => ContainingType.Locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override RefKind RefKind => RefKind.None;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(ContainingAssembly.GetSpecialType(SpecialType.System_Void));

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public override Symbol? AssociatedSymbol => null;

        public override int Arity => 0;

        public override bool ReturnsVoid => true;

        public override MethodKind MethodKind => MethodKind.StaticConstructor;

        public override bool IsExtern => false;

        public override bool IsSealed => false;

        public override bool IsAbstract => false;

        public override bool IsOverride => false;

        public override bool IsVirtual => false;

        public override bool IsStatic => true;

        public override bool IsAsync => false;

        public override bool HidesBaseMethodsByName => false;

        public override bool IsExtensionMethod => false;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        internal override bool IsExplicitInterfaceImplementation => false;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public sealed override bool IsImplicitlyDeclared => true;

        internal sealed override bool GenerateDebugInfo => true;

        internal override bool IsMetadataFinal => false;

        internal override bool RequiresSecurityObject => false;

        public sealed override bool AreLocalsZeroed => ContainingType.AreLocalsZeroed;

        internal override MarshalPseudoCustomAttributeData? ReturnValueMarshallingInformation => null;

        internal override bool HasDeclarativeSecurity => false;

        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData => null;

        internal SynthesizedStaticConstructor(NamedTypeSymbol containingType)
        {
            _containingType = containingType;
        }

        internal override bool TryGetThisParameter(out ParameterSymbol? thisParameter)
        {
            thisParameter = null;
            return true;
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return LexicalSortKey.SynthesizedCCtor;
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override DllImportData? GetDllImportData()
        {
            return null;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override UnmanagedCallersOnlyAttributeData? GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            return null;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            return ((SourceMemberContainerTypeSymbol)ContainingType).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, isStatic: true);
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            return (ContainingType as SourceMemberContainerTypeSymbol)?.IsNullableEnabledForConstructorsAndInitializers(useStatic: true) ?? false;
        }

        internal bool ShouldEmit(ImmutableArray<BoundInitializer> boundInitializersOpt = default(ImmutableArray<BoundInitializer>))
        {
            if (_lazyShouldEmit.HasValue())
            {
                return _lazyShouldEmit.Value();
            }
            bool flag = CalculateShouldEmit(boundInitializersOpt);
            _lazyShouldEmit = flag.ToThreeState();
            return flag;
        }

        private bool CalculateShouldEmit(ImmutableArray<BoundInitializer> boundInitializersOpt = default(ImmutableArray<BoundInitializer>))
        {
            if (boundInitializersOpt.IsDefault)
            {
                if (!(ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol))
                {
                    return true;
                }
                boundInitializersOpt = Binder.BindFieldInitializers(DeclaringCompilation, sourceMemberContainerTypeSymbol.IsScriptClass ? sourceMemberContainerTypeSymbol.GetScriptInitializer() : null, sourceMemberContainerTypeSymbol.StaticInitializers, BindingDiagnosticBag.Discarded, out var _);
            }
            ImmutableArray<BoundInitializer>.Enumerator enumerator = boundInitializersOpt.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is BoundFieldEqualsValue boundFieldEqualsValue)
                {
                    BoundExpression value = boundFieldEqualsValue.Value;
                    if (value != null)
                    {
                        if (!value.IsDefaultValue())
                        {
                            return true;
                        }
                        continue;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
