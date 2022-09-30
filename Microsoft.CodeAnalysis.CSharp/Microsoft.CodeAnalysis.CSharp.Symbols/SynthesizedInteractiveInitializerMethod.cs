using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedInteractiveInitializerMethod : SynthesizedInstanceMethodSymbol
    {
        internal const string InitializerName = "<Initialize>";

        private readonly SourceMemberContainerTypeSymbol _containingType;

        private readonly TypeSymbol _resultType;

        private readonly TypeSymbol _returnType;

        private ThreeState _lazyIsNullableAnalysisEnabled;

        public override string Name => "<Initialize>";

        internal override bool IsScriptInitializer => true;

        public override int Arity => TypeParameters.Length;

        public override Symbol AssociatedSymbol => null;

        public override Symbol ContainingSymbol => _containingType;

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        public override bool HidesBaseMethodsByName => false;

        public override bool IsAbstract => false;

        public override bool IsAsync => true;

        public override bool IsExtensionMethod => false;

        public override bool IsExtern => false;

        public override bool IsOverride => false;

        public override bool IsSealed => false;

        public override bool IsStatic => false;

        public override bool IsVararg => false;

        public override RefKind RefKind => RefKind.None;

        public override bool IsVirtual => false;

        public override ImmutableArray<Location> Locations => _containingType.Locations;

        public override MethodKind MethodKind => MethodKind.Ordinary;

        public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

        public override bool ReturnsVoid => _returnType.IsVoidType();

        public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(_returnType);

        public override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        public override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal override CallingConvention CallingConvention => CallingConvention.HasThis;

        internal override bool GenerateDebugInfo => true;

        internal override bool HasDeclarativeSecurity => false;

        internal override bool HasSpecialName => true;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        internal override bool RequiresSecurityObject => false;

        internal override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => null;

        internal TypeSymbol ResultType => _resultType;

        internal SynthesizedInteractiveInitializerMethod(SourceMemberContainerTypeSymbol containingType, BindingDiagnosticBag diagnostics)
        {
            _containingType = containingType;
            CalculateReturnType(containingType, diagnostics, out _resultType, out _returnType);
        }

        public override DllImportData GetDllImportData()
        {
            return null;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            return _containingType.CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, isStatic: false);
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            if (_lazyIsNullableAnalysisEnabled == ThreeState.Unknown)
            {
                CSharpCompilation declaringCompilation = DeclaringCompilation;
                bool value = declaringCompilation.Options.NullableContextOptions != 0 || declaringCompilation.SyntaxTrees.Any((SyntaxTree tree) => ((CSharpSyntaxTree)tree).IsNullableAnalysisEnabled(new TextSpan(0, tree.Length)) == true);
                _lazyIsNullableAnalysisEnabled = value.ToThreeState();
            }
            return _lazyIsNullableAnalysisEnabled == ThreeState.True;
        }

        private static void CalculateReturnType(SourceMemberContainerTypeSymbol containingType, BindingDiagnosticBag diagnostics, out TypeSymbol resultType, out TypeSymbol returnType)
        {
            CSharpCompilation declaringCompilation = containingType.DeclaringCompilation;
            Type type = declaringCompilation.ScriptCompilationInfo?.ReturnTypeOpt;
            NamedTypeSymbol wellKnownType = declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T);
            diagnostics.ReportUseSite(wellKnownType, NoLocation.Singleton);
            resultType = (((object)type == null) ? declaringCompilation.GetSpecialType(SpecialType.System_Object) : declaringCompilation.GetTypeByReflectionType(type, diagnostics));
            returnType = wellKnownType.Construct(resultType);
        }
    }
}
