// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Emit.NoPia
{
    public abstract class CommonEmbeddedTypesManager
    {
        public abstract bool IsFrozen { get; }
        public abstract ImmutableArray<Cci.INamespaceTypeDefinition> GetTypes(DiagnosticBag diagnostics, HashSet<string> namesOfTopLevelTypes);
    }

    public abstract partial class EmbeddedTypesManager<
        TPEModuleBuilder,
        TModuleCompilationState,
        TEmbeddedTypesManager,
        TSyntaxNode,
        TAttributeData,
        TSymbol,
        TAssemblySymbol,
        TNamedTypeSymbol,
        TFieldSymbol,
        TMethodSymbol,
        TEventSymbol,
        TPropertySymbol,
        TParameterSymbol,
        TTypeParameterSymbol,
        TEmbeddedType,
        TEmbeddedField,
        TEmbeddedMethod,
        TEmbeddedEvent,
        TEmbeddedProperty,
        TEmbeddedParameter,
        TEmbeddedTypeParameter> : CommonEmbeddedTypesManager
        where TPEModuleBuilder : CommonPEModuleBuilder
        where TModuleCompilationState : CommonModuleCompilationState
        where TEmbeddedTypesManager : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>
        where TSyntaxNode : SyntaxNode
        where TAttributeData : AttributeData, Cci.ICustomAttribute
        where TAssemblySymbol : class
        where TNamedTypeSymbol : class, TSymbol, Cci.INamespaceTypeReference
        where TFieldSymbol : class, TSymbol, Cci.IFieldReference
        where TMethodSymbol : class, TSymbol, Cci.IMethodReference
        where TEventSymbol : class, TSymbol, Cci.ITypeMemberReference
        where TPropertySymbol : class, TSymbol, Cci.ITypeMemberReference
        where TParameterSymbol : class, TSymbol, Cci.IParameterListEntry, Cci.INamedEntity
        where TTypeParameterSymbol : class, TSymbol, Cci.IGenericMethodParameterReference
        where TEmbeddedType : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedType
        where TEmbeddedField : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedField
        where TEmbeddedMethod : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedMethod
        where TEmbeddedEvent : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedEvent
        where TEmbeddedProperty : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedProperty
        where TEmbeddedParameter : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedParameter
        where TEmbeddedTypeParameter : EmbeddedTypesManager<TPEModuleBuilder, TModuleCompilationState, TEmbeddedTypesManager, TSyntaxNode, TAttributeData, TSymbol, TAssemblySymbol, TNamedTypeSymbol, TFieldSymbol, TMethodSymbol, TEventSymbol, TPropertySymbol, TParameterSymbol, TTypeParameterSymbol, TEmbeddedType, TEmbeddedField, TEmbeddedMethod, TEmbeddedEvent, TEmbeddedProperty, TEmbeddedParameter, TEmbeddedTypeParameter>.CommonEmbeddedTypeParameter
    {
        public readonly TPEModuleBuilder ModuleBeingBuilt;

        public readonly ConcurrentDictionary<TNamedTypeSymbol, TEmbeddedType> EmbeddedTypesMap = new(ReferenceEqualityComparer.Instance);
        public readonly ConcurrentDictionary<TFieldSymbol, TEmbeddedField> EmbeddedFieldsMap = new(ReferenceEqualityComparer.Instance);
        public readonly ConcurrentDictionary<TMethodSymbol, TEmbeddedMethod> EmbeddedMethodsMap = new(ReferenceEqualityComparer.Instance);
        public readonly ConcurrentDictionary<TPropertySymbol, TEmbeddedProperty> EmbeddedPropertiesMap = new(ReferenceEqualityComparer.Instance);
        public readonly ConcurrentDictionary<TEventSymbol, TEmbeddedEvent> EmbeddedEventsMap = new(ReferenceEqualityComparer.Instance);

        private ImmutableArray<TEmbeddedType> _frozen;

        protected EmbeddedTypesManager(TPEModuleBuilder moduleBeingBuilt)
        {
            this.ModuleBeingBuilt = moduleBeingBuilt;
        }

        public override bool IsFrozen
        {
            get
            {
                return !_frozen.IsDefault;
            }
        }

        public override ImmutableArray<Cci.INamespaceTypeDefinition> GetTypes(DiagnosticBag diagnostics, HashSet<string> namesOfTopLevelTypes)
        {
            if (_frozen.IsDefault)
            {
                var builder = ArrayBuilder<TEmbeddedType>.GetInstance();
                builder.AddRange(EmbeddedTypesMap.Values);
                builder.Sort(TypeComparer.Instance);

                if (ImmutableInterlocked.InterlockedInitialize(ref _frozen, builder.ToImmutableAndFree()) && _frozen.Length > 0)
                {
                    Cci.INamespaceTypeDefinition prev = _frozen[0];
                    bool reportedDuplicate = HasNameConflict(namesOfTopLevelTypes, _frozen[0], diagnostics);

                    for (int i = 1; i < _frozen.Length; i++)
                    {
                        Cci.INamespaceTypeDefinition current = _frozen[i];

                        if (prev.NamespaceName == current.NamespaceName &&
                            prev.Name == current.Name)
                        {
                            if (!reportedDuplicate)
                            {
                                Debug.Assert(_frozen[i - 1] == prev);

                                // ERR_DuplicateLocalTypes3/ERR_InteropTypesWithSameNameAndGuid
                                ReportNameCollisionBetweenEmbeddedTypes(_frozen[i - 1], _frozen[i], diagnostics);
                                reportedDuplicate = true;
                            }
                        }
                        else
                        {
                            prev = current;
                            reportedDuplicate = HasNameConflict(namesOfTopLevelTypes, _frozen[i], diagnostics);
                        }
                    }

                    OnGetTypesCompleted(_frozen, diagnostics);
                }
            }

            return StaticCast<Cci.INamespaceTypeDefinition>.From(_frozen);
        }

        private bool HasNameConflict(HashSet<string> namesOfTopLevelTypes, TEmbeddedType type, DiagnosticBag diagnostics)
        {
            Cci.INamespaceTypeDefinition def = type;

            if (namesOfTopLevelTypes.Contains(MetadataHelpers.BuildQualifiedName(def.NamespaceName, def.Name)))
            {
                // ERR_LocalTypeNameClash2/ERR_LocalTypeNameClash
                ReportNameCollisionWithAlreadyDeclaredType(type, diagnostics);
                return true;
            }

            return false;
        }

        public abstract int GetTargetAttributeSignatureIndex(TSymbol underlyingSymbol, TAttributeData attrData, AttributeDescription description);

        internal bool IsTargetAttribute(TSymbol underlyingSymbol, TAttributeData attrData, AttributeDescription description)
        {
            return GetTargetAttributeSignatureIndex(underlyingSymbol, attrData, description) != -1;
        }

        public abstract TAttributeData CreateSynthesizedAttribute(WellKnownMember constructor, TAttributeData attrData, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);
        public abstract void ReportIndirectReferencesToLinkedAssemblies(TAssemblySymbol assembly, DiagnosticBag diagnostics);

        protected abstract void OnGetTypesCompleted(ImmutableArray<TEmbeddedType> types, DiagnosticBag diagnostics);
        protected abstract void ReportNameCollisionBetweenEmbeddedTypes(TEmbeddedType typeA, TEmbeddedType typeB, DiagnosticBag diagnostics);
        protected abstract void ReportNameCollisionWithAlreadyDeclaredType(TEmbeddedType type, DiagnosticBag diagnostics);
        protected abstract TAttributeData CreateCompilerGeneratedAttribute();

        private sealed class TypeComparer : IComparer<TEmbeddedType>
        {
            public static readonly TypeComparer Instance = new();

            private TypeComparer()
            {
            }

            public int Compare(TEmbeddedType x, TEmbeddedType y)
            {
                Cci.INamespaceTypeDefinition dx = x;
                Cci.INamespaceTypeDefinition dy = y;

                int result = string.Compare(dx.NamespaceName, dy.NamespaceName, StringComparison.Ordinal);

                if (result == 0)
                {
                    result = string.Compare(dx.Name, dy.Name, StringComparison.Ordinal);

                    if (result == 0)
                    {
                        // this is a name conflict.
                        result = x.AssemblyRefIndex - y.AssemblyRefIndex;
                    }
                }

                return result;
            }
        }

        protected void EmbedReferences(Cci.ITypeDefinitionMember embeddedMember, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            var noPiaIndexer = new Cci.TypeReferenceIndexer(new EmitContext(ModuleBeingBuilt, syntaxNodeOpt, diagnostics, metadataOnly: false, includePrivateMembers: true));
            noPiaIndexer.Visit(embeddedMember);
        }

        /// <summary>
        /// Returns null if member doesn't belong to an embedded NoPia type.
        /// </summary>
        protected abstract TEmbeddedType GetEmbeddedTypeForMember(TSymbol member, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);

        public abstract TEmbeddedField EmbedField(TEmbeddedType type, TFieldSymbol field, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);
        public abstract TEmbeddedMethod EmbedMethod(TEmbeddedType type, TMethodSymbol method, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);
        public abstract TEmbeddedProperty EmbedProperty(TEmbeddedType type, TPropertySymbol property, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics);
        public abstract TEmbeddedEvent EmbedEvent(TEmbeddedType type, TEventSymbol @event, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding);

        public Cci.IFieldReference EmbedFieldIfNeedTo(TFieldSymbol fieldSymbol, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            TEmbeddedType type = GetEmbeddedTypeForMember(fieldSymbol, syntaxNodeOpt, diagnostics);
            if (type != null)
            {
                return EmbedField(type, fieldSymbol, syntaxNodeOpt, diagnostics);
            }
            return fieldSymbol;
        }

        public Cci.IMethodReference EmbedMethodIfNeedTo(TMethodSymbol methodSymbol, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            TEmbeddedType type = GetEmbeddedTypeForMember(methodSymbol, syntaxNodeOpt, diagnostics);
            if (type != null)
            {
                return EmbedMethod(type, methodSymbol, syntaxNodeOpt, diagnostics);
            }
            return methodSymbol;
        }

        public void EmbedEventIfNeedTo(TEventSymbol eventSymbol, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics, bool isUsedForComAwareEventBinding)
        {
            TEmbeddedType type = GetEmbeddedTypeForMember(eventSymbol, syntaxNodeOpt, diagnostics);
            if (type != null)
            {
                EmbedEvent(type, eventSymbol, syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding);
            }
        }

        public void EmbedPropertyIfNeedTo(TPropertySymbol propertySymbol, TSyntaxNode syntaxNodeOpt, DiagnosticBag diagnostics)
        {
            TEmbeddedType type = GetEmbeddedTypeForMember(propertySymbol, syntaxNodeOpt, diagnostics);
            if (type != null)
            {
                EmbedProperty(type, propertySymbol, syntaxNodeOpt, diagnostics);
            }
        }
    }
}
