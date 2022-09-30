using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Emit.NoPia;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Emit.NoPia
{
    public sealed class EmbeddedType : EmbeddedTypesManager<PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, CSharpAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter>.CommonEmbeddedType
    {
        private bool _embeddedAllMembersOfImplementedInterface;

        protected override bool IsPublic => UnderlyingNamedType.AdaptedNamedTypeSymbol.DeclaredAccessibility == Accessibility.Public;

        protected override bool IsAbstract => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsMetadataAbstract;

        protected override bool IsBeforeFieldInit
        {
            get
            {
                switch (UnderlyingNamedType.AdaptedNamedTypeSymbol.TypeKind)
                {
                    case TypeKind.Delegate:
                    case TypeKind.Enum:
                    case TypeKind.Interface:
                        return false;
                    default:
                        return true;
                }
            }
        }

        protected override bool IsComImport => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsComImport;

        protected override bool IsInterface => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsInterfaceType();

        protected override bool IsDelegate => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsDelegateType();

        protected override bool IsSerializable => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsSerializable;

        protected override bool IsSpecialName => UnderlyingNamedType.AdaptedNamedTypeSymbol.HasSpecialName;

        protected override bool IsWindowsRuntimeImport => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsWindowsRuntimeImport;

        protected override bool IsSealed => UnderlyingNamedType.AdaptedNamedTypeSymbol.IsMetadataSealed;

        protected override CharSet StringFormat => UnderlyingNamedType.AdaptedNamedTypeSymbol.MarshallingCharSet;

        public EmbeddedType(EmbeddedTypesManager typeManager, NamedTypeSymbol underlyingNamedType)
            : base(typeManager, underlyingNamedType)
        {
        }

        public void EmbedAllMembersOfImplementedInterface(SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            if (_embeddedAllMembersOfImplementedInterface)
            {
                return;
            }
            _embeddedAllMembersOfImplementedInterface = true;
            foreach (MethodSymbol item in UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMethodsToEmit())
            {
                if ((object)item != null)
                {
                    TypeManager.EmbedMethod(this, item.GetCciAdapter(), syntaxNodeOpt, diagnostics);
                }
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = UnderlyingNamedType.AdaptedNamedTypeSymbol.GetInterfacesToEmit().GetEnumerator();
            while (enumerator2.MoveNext())
            {
                NamedTypeSymbol current2 = enumerator2.Current;
                TypeManager.ModuleBeingBuilt.Translate(current2, syntaxNodeOpt, diagnostics, fromImplements: true);
            }
        }

        protected override int GetAssemblyRefIndex()
        {
            return TypeManager.ModuleBeingBuilt.SourceModule.GetReferencedAssemblySymbols().IndexOf(UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly, ReferenceEqualityComparer.Instance);
        }

        protected override ITypeReference GetBaseClass(PEModuleBuilder moduleBuilder, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = UnderlyingNamedType.AdaptedNamedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics == null)
            {
                return null;
            }
            return moduleBuilder.Translate(baseTypeNoUseSiteDiagnostics, syntaxNodeOpt, diagnostics);
        }

        protected override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            return UnderlyingNamedType.AdaptedNamedTypeSymbol.GetFieldsToEmit();
        }

        protected override IEnumerable<MethodSymbol> GetMethodsToEmit()
        {
            return UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMethodsToEmit();
        }

        protected override IEnumerable<EventSymbol> GetEventsToEmit()
        {
            return UnderlyingNamedType.AdaptedNamedTypeSymbol.GetEventsToEmit();
        }

        protected override IEnumerable<PropertySymbol> GetPropertiesToEmit()
        {
            return UnderlyingNamedType.AdaptedNamedTypeSymbol.GetPropertiesToEmit();
        }

        protected override IEnumerable<TypeReferenceWithAttributes> GetInterfaces(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = UnderlyingNamedType.AdaptedNamedTypeSymbol.GetInterfacesToEmit().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                INamedTypeReference typeRef = moduleBeingBuilt.Translate(current, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
                TypeWithAnnotations type = TypeWithAnnotations.Create(current);
                yield return type.GetTypeRefWithAttributes(moduleBeingBuilt, UnderlyingNamedType.AdaptedNamedTypeSymbol, typeRef);
            }
        }

        protected override TypeLayout? GetTypeLayoutIfStruct()
        {
            if (UnderlyingNamedType.AdaptedNamedTypeSymbol.IsStructType())
            {
                return UnderlyingNamedType.AdaptedNamedTypeSymbol.Layout;
            }
            return null;
        }

        protected override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return UnderlyingNamedType.AdaptedNamedTypeSymbol.GetCustomAttributesToEmit(moduleBuilder);
        }

        protected override CSharpAttributeData CreateTypeIdentifierAttribute(bool hasGuid, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            WellKnownMember method = (hasGuid ? WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctor : WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString);
            MethodSymbol wellKnownMethod = TypeManager.GetWellKnownMethod(method, syntaxNodeOpt, diagnostics);
            if ((object)wellKnownMethod == null)
            {
                return null;
            }
            if (hasGuid)
            {
                return new SynthesizedAttributeData(wellKnownMethod, ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            NamedTypeSymbol systemStringType = TypeManager.GetSystemStringType(syntaxNodeOpt, diagnostics);
            if ((object)systemStringType != null)
            {
                string assemblyGuidString = TypeManager.GetAssemblyGuidString(UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly);
                return new SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(new TypedConstant(systemStringType, TypedConstantKind.Primitive, assemblyGuidString), new TypedConstant(systemStringType, TypedConstantKind.Primitive, UnderlyingNamedType.AdaptedNamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat))), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }
            return null;
        }

        protected override void ReportMissingAttribute(AttributeDescription description, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            EmbeddedTypesManager.Error(diagnostics, ErrorCode.ERR_InteropTypeMissingAttribute, syntaxNodeOpt, UnderlyingNamedType.AdaptedNamedTypeSymbol, description.FullName);
        }

        protected override void EmbedDefaultMembers(string defaultMember, SyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMembers(defaultMember).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                switch (current.Kind)
                {
                    case SymbolKind.Field:
                        TypeManager.EmbedField(this, ((FieldSymbol)current).GetCciAdapter(), syntaxNodeOpt, diagnostics);
                        break;
                    case SymbolKind.Method:
                        TypeManager.EmbedMethod(this, ((MethodSymbol)current).GetCciAdapter(), syntaxNodeOpt, diagnostics);
                        break;
                    case SymbolKind.Property:
                        TypeManager.EmbedProperty(this, ((PropertySymbol)current).GetCciAdapter(), syntaxNodeOpt, diagnostics);
                        break;
                    case SymbolKind.Event:
                        TypeManager.EmbedEvent(this, ((EventSymbol)current).GetCciAdapter(), syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding: false);
                        break;
                }
            }
        }
    }
}
