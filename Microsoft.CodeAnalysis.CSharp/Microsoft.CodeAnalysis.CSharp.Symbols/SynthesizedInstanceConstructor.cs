using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class SynthesizedInstanceConstructor : SynthesizedInstanceMethodSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        internal override bool GenerateDebugInfo => true;

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                if (!ContainingType.IsAbstract)
                {
                    return Accessibility.Public;
                }
                return Accessibility.Protected;
            }
        }

        internal override bool IsMetadataFinal => false;

        public sealed override Symbol ContainingSymbol => _containingType;

        public sealed override NamedTypeSymbol ContainingType => _containingType;

        public sealed override string Name => ".ctor";

        internal sealed override bool HasSpecialName => true;

        internal sealed override MethodImplAttributes ImplementationAttributes
        {
            get
            {
                if (_containingType.IsComImport)
                {
                    return (MethodImplAttributes)4099;
                }
                if (_containingType.TypeKind == TypeKind.Delegate)
                {
                    return MethodImplAttributes.CodeTypeMask;
                }
                return MethodImplAttributes.IL;
            }
        }

        internal sealed override bool RequiresSecurityObject => false;

        internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => null;

        internal sealed override bool HasDeclarativeSecurity => false;

        public sealed override bool IsVararg => false;

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public sealed override ImmutableArray<Location> Locations => ContainingType.Locations;

        public override RefKind RefKind => RefKind.None;

        public sealed override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(ContainingAssembly.GetSpecialType(SpecialType.System_Void));

        public sealed override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public sealed override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public sealed override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public sealed override Symbol AssociatedSymbol => null;

        public sealed override int Arity => 0;

        public sealed override bool ReturnsVoid => true;

        public sealed override MethodKind MethodKind => MethodKind.Constructor;

        public sealed override bool IsExtern => ContainingType?.IsComImport ?? false;

        public sealed override bool IsSealed => false;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsOverride => false;

        public sealed override bool IsVirtual => false;

        public sealed override bool IsStatic => false;

        public sealed override bool IsAsync => false;

        public sealed override bool HidesBaseMethodsByName => false;

        public sealed override bool IsExtensionMethod => false;

        internal sealed override CallingConvention CallingConvention => CallingConvention.HasThis;

        internal sealed override bool IsExplicitInterfaceImplementation => false;

        public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        internal SynthesizedInstanceConstructor(NamedTypeSymbol containingType)
        {
            _containingType = containingType;
        }

        public sealed override DllImportData GetDllImportData()
        {
            return null;
        }

        internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            return LexicalSortKey.SynthesizedCtor;
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            return ((SourceMemberContainerTypeSymbol)ContainingType).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, isStatic: false);
        }

        public sealed override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
            MergeUseSiteInfo(ref result, ReturnTypeWithAnnotations.Type.GetUseSiteInfo());
            return result;
        }

        internal sealed override bool IsNullableAnalysisEnabled()
        {
            return (ContainingType as SourceMemberContainerTypeSymbol)?.IsNullableEnabledForConstructorsAndInitializers(useStatic: false) ?? false;
        }

        protected void GenerateMethodBodyCore(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, this.GetNonNullSyntaxNode(), compilationState, diagnostics);
            syntheticBoundNodeFactory.CurrentFunction = this;
            if (ContainingType.BaseTypeNoUseSiteDiagnostics is MissingMetadataTypeSymbol)
            {
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block());
                return;
            }
            BoundCall boundCall = MethodCompiler.GenerateBaseParameterlessConstructorInitializer(this, diagnostics);
            if (boundCall == null)
            {
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block());
                return;
            }
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            instance.Add(syntheticBoundNodeFactory.ExpressionStatement(boundCall));
            GenerateMethodBodyStatements(syntheticBoundNodeFactory, instance, diagnostics);
            instance.Add(syntheticBoundNodeFactory.Return());
            BoundBlock body = syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree());
            syntheticBoundNodeFactory.CloseMethod(body);
        }

        internal virtual void GenerateMethodBodyStatements(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, BindingDiagnosticBag diagnostics)
        {
        }
    }
}
