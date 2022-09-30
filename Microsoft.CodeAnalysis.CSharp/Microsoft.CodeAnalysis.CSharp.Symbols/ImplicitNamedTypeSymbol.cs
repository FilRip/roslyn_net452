using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class ImplicitNamedTypeSymbol : SourceMemberContainerTypeSymbol
    {
        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
        {
            get
            {
                if (!IsScriptClass)
                {
                    return DeclaringCompilation.GetSpecialType(SpecialType.System_Object);
                }
                return null;
            }
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => ImmutableArray<TypeWithAnnotations>.Empty;

        public sealed override bool AreLocalsZeroed => ContainingType?.AreLocalsZeroed ?? ContainingModule.AreLocalsZeroed;

        internal override bool IsComImport => false;

        internal override NamedTypeSymbol ComImportCoClass => null;

        internal override bool HasSpecialName => false;

        internal override bool ShouldAddWinRTMembers => false;

        internal sealed override bool IsWindowsRuntimeImport => false;

        public sealed override bool IsSerializable => false;

        internal sealed override TypeLayout Layout => default(TypeLayout);

        internal bool HasStructLayoutAttribute => false;

        internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

        internal sealed override bool HasDeclarativeSecurity => false;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;

        internal ImplicitNamedTypeSymbol(NamespaceOrTypeSymbol containingSymbol, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
            : base(containingSymbol, declaration, diagnostics)
        {
            state.NotePartComplete(CompletionPart.EnumUnderlyingType);
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            state.NotePartComplete(CompletionPart.Attributes);
            return ImmutableArray<CSharpAttributeData>.Empty;
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {
            return AttributeUsageInfo.Null;
        }

        protected override Location GetCorrespondingBaseListLocation(NamedTypeSymbol @base)
        {
            return NoLocation.Singleton;
        }

        protected override void CheckBase(BindingDiagnosticBag diagnostics)
        {
            diagnostics.ReportUseSite(DeclaringCompilation.GetSpecialType(SpecialType.System_Object), Locations[0]);
        }

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return BaseTypeNoUseSiteDiagnostics;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        protected override void CheckInterfaces(BindingDiagnosticBag diagnostics)
        {
        }

        internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal sealed override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
