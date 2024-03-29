// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// A container synthesized for a lambda, iterator method, async method, or dynamic-sites.
    /// </summary>
    internal abstract class SynthesizedContainer : NamedTypeSymbol
    {
        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;
        private readonly ImmutableArray<TypeParameterSymbol> _constructedFromTypeParameters;

        protected SynthesizedContainer(string name, int parameterCount, bool returnsVoid)
        {
            Name = name;
            TypeMap = TypeMap.Empty;
            _typeParameters = CreateTypeParameters(parameterCount, returnsVoid);
            _constructedFromTypeParameters = default;
        }

        protected SynthesizedContainer(string name, MethodSymbol containingMethod)
        {
            Name = name;
            if (containingMethod == null)
            {
                TypeMap = TypeMap.Empty;
                _typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
            }
            else
            {
                TypeMap = TypeMap.Empty.WithConcatAlphaRename(containingMethod, this, out _typeParameters, out _constructedFromTypeParameters);
            }
        }

        protected SynthesizedContainer(string name, ImmutableArray<TypeParameterSymbol> typeParameters, TypeMap typeMap)
        {

            Name = name;
            _typeParameters = typeParameters;
            TypeMap = typeMap;
        }

        private ImmutableArray<TypeParameterSymbol> CreateTypeParameters(int parameterCount, bool returnsVoid)
        {
            var typeParameters = ArrayBuilder<TypeParameterSymbol>.GetInstance(parameterCount + (returnsVoid ? 0 : 1));
            for (int i = 0; i < parameterCount; i++)
            {
                typeParameters.Add(new AnonymousTypeManager.AnonymousTypeParameterSymbol(this, i, "T" + (i + 1)));
            }

            if (!returnsVoid)
            {
                typeParameters.Add(new AnonymousTypeManager.AnonymousTypeParameterSymbol(this, parameterCount, "TResult"));
            }

            return typeParameters.ToImmutableAndFree();
        }

        internal TypeMap TypeMap { get; }

        internal virtual MethodSymbol Constructor => null;

        internal sealed override bool IsInterface => this.TypeKind == TypeKind.Interface;

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);

            if (ContainingSymbol.Kind == SymbolKind.NamedType && (ContainingSymbol.IsImplicitlyDeclared || ContainingSymbol is SimpleProgramNamedTypeSymbol))
            {
                return;
            }

            var compilation = ContainingSymbol.DeclaringCompilation;

            // this can only happen if frame is not nested in a source type/namespace (so far we do not do this)
            // if this happens for whatever reason, we do not need "CompilerGenerated" anyways

            AddSynthesizedAttribute(ref attributes, compilation.TrySynthesizeAttribute(
                WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            => throw ExceptionUtilities.Unreachable;

        /// <summary>
        /// Note: Can be default if this SynthesizedContainer was constructed with <see cref="SynthesizedContainer(string, int, bool)"/>
        /// </summary>
        internal ImmutableArray<TypeParameterSymbol> ConstructedFromTypeParameters => _constructedFromTypeParameters;

        public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

        public sealed override string Name { get; }

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override IEnumerable<string> MemberNames => SpecializedCollections.EmptyEnumerable<string>();

        public override NamedTypeSymbol ConstructedFrom => this;

        public override bool IsSealed => true;

        public override bool IsAbstract => Constructor is null && this.TypeKind != TypeKind.Struct;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics
        {
            get { return GetTypeParametersAsTypeArguments(); }
        }

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        public override ImmutableArray<Symbol> GetMembers()
        {
            Symbol constructor = this.Constructor;
            return constructor is null ? ImmutableArray<Symbol>.Empty : ImmutableArray.Create(constructor);
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            var ctor = Constructor;
            return (ctor is not null && name == ctor.Name) ? ImmutableArray.Create<Symbol>(ctor) : ImmutableArray<Symbol>.Empty;
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            foreach (var m in this.GetMembers())
            {
                switch (m.Kind)
                {
                    case SymbolKind.Field:
                        yield return (FieldSymbol)m;
                        break;
                }
            }
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers() => this.GetMembersUnordered();

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name) => this.GetMembers(name);

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers() => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name) => ImmutableArray<NamedTypeSymbol>.Empty;

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity) => ImmutableArray<NamedTypeSymbol>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override bool IsStatic => false;

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved) => ImmutableArray<NamedTypeSymbol>.Empty;

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit() => CalculateInterfacesToEmit();

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => ContainingAssembly.GetSpecialType(this.TypeKind == TypeKind.Struct ? SpecialType.System_ValueType : SpecialType.System_Object);

        internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved) => BaseTypeNoUseSiteDiagnostics;

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved) => InterfacesNoUseSiteDiagnostics(basesBeingResolved);

        public override bool MightContainExtensionMethods => false;

        public override int Arity => TypeParameters.Length;

        internal override bool MangleName => Arity > 0;

        public override bool IsImplicitlyDeclared => true;

        internal override bool ShouldAddWinRTMembers => false;

        internal override bool IsWindowsRuntimeImport => false;

        internal override bool IsComImport => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols() => ImmutableArray<string>.Empty;

        internal override bool HasDeclarativeSecurity => false;

        internal override CharSet MarshallingCharSet => DefaultMarshallingCharSet;

        public override bool IsSerializable => false;

        internal override IEnumerable<Cci.SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo() => default;

        internal override TypeLayout Layout => default;

        internal override bool HasSpecialName => false;

        internal sealed override NamedTypeSymbol AsNativeInteger() => throw ExceptionUtilities.Unreachable;

        internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;
    }
}
