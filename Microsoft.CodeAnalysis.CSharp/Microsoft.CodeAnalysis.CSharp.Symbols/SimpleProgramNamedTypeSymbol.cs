using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SimpleProgramNamedTypeSymbol : SourceMemberContainerTypeSymbol
    {
        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => DeclaringCompilation.GetSpecialType(SpecialType.System_Object);

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => ImmutableArray<TypeWithAnnotations>.Empty;

        public sealed override bool AreLocalsZeroed => ContainingModule.AreLocalsZeroed;

        internal override bool IsComImport => false;

        internal override NamedTypeSymbol? ComImportCoClass => null;

        internal override bool HasSpecialName => false;

        internal override bool ShouldAddWinRTMembers => false;

        internal sealed override bool IsWindowsRuntimeImport => false;

        public sealed override bool IsSerializable => false;

        internal sealed override TypeLayout Layout => default(TypeLayout);

        internal bool HasStructLayoutAttribute => false;

        internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

        internal sealed override bool HasDeclarativeSecurity => false;

        internal override ObsoleteAttributeData? ObsoleteAttributeData => null;

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        public override bool IsImplicitlyDeclared => false;

        internal override NamedTypeSymbol? NativeIntegerUnderlyingType => null;

        internal SimpleProgramNamedTypeSymbol(NamespaceSymbol globalNamespace, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
            : base(globalNamespace, declaration, diagnostics)
        {
            state.NotePartComplete(CompletionPart.EnumUnderlyingType);
        }

        internal static SynthesizedSimpleProgramEntryPointSymbol? GetSimpleProgramEntryPoint(CSharpCompilation compilation)
        {
            return (SynthesizedSimpleProgramEntryPointSymbol)(GetSimpleProgramNamedTypeSymbol(compilation)?.GetMembersAndInitializers().NonTypeMembers[0]);
        }

        private static SimpleProgramNamedTypeSymbol? GetSimpleProgramNamedTypeSymbol(CSharpCompilation compilation)
        {
            return compilation.SourceModule.GlobalNamespace.GetTypeMembers("<Program>$").OfType<SimpleProgramNamedTypeSymbol>().SingleOrDefault();
        }

        internal static SynthesizedSimpleProgramEntryPointSymbol? GetSimpleProgramEntryPoint(CSharpCompilation compilation, CompilationUnitSyntax compilationUnit, bool fallbackToMainEntryPoint)
        {
            SimpleProgramNamedTypeSymbol simpleProgramNamedTypeSymbol = GetSimpleProgramNamedTypeSymbol(compilation);
            if ((object)simpleProgramNamedTypeSymbol == null)
            {
                return null;
            }
            ImmutableArray<Symbol> nonTypeMembers = simpleProgramNamedTypeSymbol.GetMembersAndInitializers().NonTypeMembers;
            ImmutableArray<Symbol>.Enumerator enumerator = nonTypeMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SynthesizedSimpleProgramEntryPointSymbol synthesizedSimpleProgramEntryPointSymbol = (SynthesizedSimpleProgramEntryPointSymbol)enumerator.Current;
                if (synthesizedSimpleProgramEntryPointSymbol.SyntaxTree == compilationUnit.SyntaxTree && synthesizedSimpleProgramEntryPointSymbol.SyntaxNode == compilationUnit)
                {
                    return synthesizedSimpleProgramEntryPointSymbol;
                }
            }
            if (!fallbackToMainEntryPoint)
            {
                return null;
            }
            return (SynthesizedSimpleProgramEntryPointSymbol)nonTypeMembers[0];
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
            Binder.GetSpecialType(DeclaringCompilation, SpecialType.System_Object, NoLocation.Singleton, diagnostics);
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

        protected override MembersAndInitializers BuildMembersAndInitializers(BindingDiagnosticBag diagnostics)
        {
            BindingDiagnosticBag diagnostics2 = diagnostics;
            bool flag = false;
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                if (flag)
                {
                    Binder.Error(diagnostics2, ErrorCode.ERR_SimpleProgramMultipleUnitsWithTopLevelStatements, current.NameLocation);
                }
                else
                {
                    flag = true;
                }
            }
            return new MembersAndInitializers(declaration.Declarations.SelectAsArray((Func<SingleTypeDeclaration, Symbol>)((SingleTypeDeclaration singleDeclaration) => new SynthesizedSimpleProgramEntryPointSymbol(this, singleDeclaration, diagnostics2))), ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Empty, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Empty, haveIndexers: false, isNullableEnabledForInstanceConstructorsAndFields: false, isNullableEnabledForStaticConstructorsAndFields: false);
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembersAndInitializers().NonTypeMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                if (current.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken))
                {
                    return true;
                }
            }
            return false;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
        }

        internal override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
