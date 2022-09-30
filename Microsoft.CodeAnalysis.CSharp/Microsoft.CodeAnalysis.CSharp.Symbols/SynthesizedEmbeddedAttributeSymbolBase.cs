using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedEmbeddedAttributeSymbolBase : NamedTypeSymbol
    {
        private readonly string _name;

        private readonly NamedTypeSymbol _baseType;

        private readonly NamespaceSymbol _namespace;

        private readonly ModuleSymbol _module;

        public new abstract ImmutableArray<MethodSymbol> Constructors { get; }

        public override int Arity => 0;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override bool IsImplicitlyDeclared => true;

        public override NamedTypeSymbol ConstructedFrom => this;

        public override bool MightContainExtensionMethods => false;

        public override string Name => _name;

        public override IEnumerable<string> MemberNames => Constructors.Select((MethodSymbol m) => m.Name);

        public override Accessibility DeclaredAccessibility => Accessibility.Internal;

        public override TypeKind TypeKind => TypeKind.Class;

        public override Symbol ContainingSymbol => _namespace;

        internal override ModuleSymbol ContainingModule => _module;

        public override AssemblySymbol ContainingAssembly => _module.ContainingAssembly;

        public override NamespaceSymbol ContainingNamespace => _namespace;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override bool IsStatic => false;

        public override bool IsRefLikeType => false;

        public override bool IsReadOnly => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => true;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => ImmutableArray<TypeWithAnnotations>.Empty;

        internal override bool MangleName => false;

        internal override bool HasCodeAnalysisEmbeddedAttribute => true;

        internal override bool HasSpecialName => false;

        internal override bool IsComImport => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override bool ShouldAddWinRTMembers => false;

        public override bool IsSerializable => false;

        public sealed override bool AreLocalsZeroed => ContainingModule.AreLocalsZeroed;

        internal override TypeLayout Layout => default(TypeLayout);

        internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

        internal override bool HasDeclarativeSecurity => false;

        internal override bool IsInterface => false;

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _baseType;

        internal override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;

        public SynthesizedEmbeddedAttributeSymbolBase(string name, NamespaceSymbol containingNamespace, ModuleSymbol containingModule, NamedTypeSymbol baseType)
        {
            _name = name;
            _namespace = containingNamespace;
            _module = containingModule;
            _baseType = baseType;
        }

        internal override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Managed;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return Constructors.CastArray<Symbol>();
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (!(name == ".ctor"))
            {
                return ImmutableArray<Symbol>.Empty;
            }
            return Constructors.CastArray<Symbol>();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {
            return AttributeUsageInfo.Default;
        }

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return _baseType;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            return GetMembers();
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            return GetMembers(name);
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            return null;
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved = null)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
            Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeEmbeddedAttribute());
            AttributeUsageInfo attributeUsageInfo = GetAttributeUsageInfo();
            if (attributeUsageInfo != AttributeUsageInfo.Default)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.Compilation.SynthesizeAttributeUsageAttribute(attributeUsageInfo.ValidTargets, attributeUsageInfo.AllowMultiple, attributeUsageInfo.Inherited));
            }
        }

        internal sealed override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
