// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Represents implicitly declared type for a Simple Program feature.
    /// </summary>
    internal sealed class SimpleProgramNamedTypeSymbol : SourceMemberContainerTypeSymbol
    {
        internal SimpleProgramNamedTypeSymbol(NamespaceSymbol globalNamespace, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
            : base(globalNamespace, declaration, diagnostics)
        {

            state.NotePartComplete(CompletionPart.EnumUnderlyingType); // No work to do for this.
        }

        internal static SynthesizedSimpleProgramEntryPointSymbol? GetSimpleProgramEntryPoint(CSharpCompilation compilation)
        {
            return (SynthesizedSimpleProgramEntryPointSymbol?)GetSimpleProgramNamedTypeSymbol(compilation)?.GetMembersAndInitializers().NonTypeMembers[0];
        }

        private static SimpleProgramNamedTypeSymbol? GetSimpleProgramNamedTypeSymbol(CSharpCompilation compilation)
        {
            return compilation.SourceModule.GlobalNamespace.GetTypeMembers(WellKnownMemberNames.TopLevelStatementsEntryPointTypeName).OfType<SimpleProgramNamedTypeSymbol>().SingleOrDefault();
        }

        internal static SynthesizedSimpleProgramEntryPointSymbol? GetSimpleProgramEntryPoint(CSharpCompilation compilation, CompilationUnitSyntax compilationUnit, bool fallbackToMainEntryPoint)
        {
            var type = GetSimpleProgramNamedTypeSymbol(compilation);

            if (type is null)
            {
                return null;
            }

            ImmutableArray<Symbol> entryPoints = type.GetMembersAndInitializers().NonTypeMembers;

            foreach (SynthesizedSimpleProgramEntryPointSymbol entryPoint in entryPoints.OfType<SynthesizedSimpleProgramEntryPointSymbol>())
            {
                if (entryPoint.SyntaxTree == compilationUnit.SyntaxTree && entryPoint.SyntaxNode == compilationUnit)
                {
                    return entryPoint;
                }
            }

            return fallbackToMainEntryPoint ? (SynthesizedSimpleProgramEntryPointSymbol)entryPoints[0] : null;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            => throw ExceptionUtilities.Unreachable;

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
            return NoLocation.Singleton; // No explicit base list
        }

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
            => this.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object);

        protected override void CheckBase(BindingDiagnosticBag diagnostics)
        {
            // check that System.Object is available. 
            Binder.GetSpecialType(this.DeclaringCompilation, SpecialType.System_Object, NoLocation.Singleton, diagnostics);
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
            // nop
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return ImmutableArray<TypeParameterSymbol>.Empty; }
        }

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics
        {
            get { return ImmutableArray<TypeWithAnnotations>.Empty; }
        }

        public sealed override bool AreLocalsZeroed
        {
            get { return ContainingModule.AreLocalsZeroed; }
        }

        internal override bool IsComImport
        {
            get { return false; }
        }

        internal override NamedTypeSymbol? ComImportCoClass
        {
            get { return null; }
        }

        internal override bool HasSpecialName
        {
            get { return false; }
        }

        internal override bool ShouldAddWinRTMembers
        {
            get { return false; }
        }

        internal sealed override bool IsWindowsRuntimeImport
        {
            get { return false; }
        }

        public sealed override bool IsSerializable
        {
            get { return false; }
        }

        internal sealed override TypeLayout Layout
        {
            get { return default; }
        }

        internal bool HasStructLayoutAttribute
        {
            get { return false; }
        }

        internal override CharSet MarshallingCharSet
        {
            get { return DefaultMarshallingCharSet; }
        }

        internal sealed override bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override ObsoleteAttributeData? ObsoleteAttributeData
        {
            get { return null; }
        }

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        protected override MembersAndInitializers BuildMembersAndInitializers(BindingDiagnosticBag diagnostics)
        {
            bool reportAnError = false;
            foreach (var singleDecl in declaration.Declarations)
            {
                if (reportAnError)
                {
                    Binder.Error(diagnostics, ErrorCode.ERR_SimpleProgramMultipleUnitsWithTopLevelStatements, singleDecl.NameLocation);
                }
                else
                {
                    reportAnError = true;
                }
            }

            return new MembersAndInitializers(nonTypeMembers: declaration.Declarations.SelectAsArray<SingleTypeDeclaration, Symbol>(singleDeclaration => new SynthesizedSimpleProgramEntryPointSymbol(this, singleDeclaration, diagnostics)),
                                              staticInitializers: ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Empty,
                                              instanceInitializers: ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Empty,
                                              haveIndexers: false,
                                              isNullableEnabledForInstanceConstructorsAndFields: false,
                                              isNullableEnabledForStaticConstructorsAndFields: false);
        }

        public override bool IsImplicitlyDeclared => false;

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken)
        {
            foreach (var member in GetMembersAndInitializers().NonTypeMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (member.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);

            AddSynthesizedAttribute(ref attributes,
                this.DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
        }

        /// <summary>
        /// Returns an instance of a symbol that represents an native integer
        /// if this underlying symbol represents System.IntPtr or System.UIntPtr.
        /// For other symbols, throws <see cref="System.InvalidOperationException"/>.
        /// </summary>
        internal override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }

        /// <summary>
        /// If this is a native integer, returns the symbol for the underlying type,
        /// either <see cref="System.IntPtr"/> or <see cref="System.UIntPtr"/>.
        /// Otherwise, returns null.
        /// </summary>
        internal override NamedTypeSymbol? NativeIntegerUnderlyingType => null;
    }
}
