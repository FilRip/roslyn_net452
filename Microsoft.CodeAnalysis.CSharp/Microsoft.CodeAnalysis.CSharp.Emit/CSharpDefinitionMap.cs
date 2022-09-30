using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Emit
{
    internal sealed class CSharpDefinitionMap : DefinitionMap
    {
        private readonly MetadataDecoder _metadataDecoder;

        private readonly CSharpSymbolMatcher _mapToMetadata;

        private readonly CSharpSymbolMatcher _mapToPrevious;

        protected override SymbolMatcher MapToMetadataSymbolMatcher => _mapToMetadata;

        protected override SymbolMatcher MapToPreviousSymbolMatcher => _mapToPrevious;

        public override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance;

        public CSharpDefinitionMap(IEnumerable<SemanticEdit> edits, MetadataDecoder metadataDecoder, CSharpSymbolMatcher mapToMetadata, CSharpSymbolMatcher? mapToPrevious)
            : base(edits)
        {
            _metadataDecoder = metadataDecoder;
            _mapToMetadata = mapToMetadata;
            _mapToPrevious = mapToPrevious ?? mapToMetadata;
        }

        protected override ISymbolInternal? GetISymbolInternalOrNull(ISymbol symbol)
        {
            return (symbol as Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.Symbol)?.UnderlyingSymbol;
        }

        protected override LambdaSyntaxFacts GetLambdaSyntaxFacts()
        {
            return CSharpLambdaSyntaxFacts.Instance;
        }

        internal bool TryGetAnonymousTypeName(AnonymousTypeManager.AnonymousTypeTemplateSymbol template, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out string? name, out int index)
        {
            return _mapToPrevious.TryGetAnonymousTypeName(template, out name, out index);
        }

        public override bool TryGetTypeHandle(ITypeDefinition def, out TypeDefinitionHandle handle)
        {
            if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PENamedTypeSymbol pENamedTypeSymbol)
            {
                handle = pENamedTypeSymbol.Handle;
                return true;
            }
            handle = default(TypeDefinitionHandle);
            return false;
        }

        public override bool TryGetEventHandle(IEventDefinition def, out EventDefinitionHandle handle)
        {
            if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEEventSymbol pEEventSymbol)
            {
                handle = pEEventSymbol.Handle;
                return true;
            }
            handle = default(EventDefinitionHandle);
            return false;
        }

        public override bool TryGetFieldHandle(IFieldDefinition def, out FieldDefinitionHandle handle)
        {
            if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEFieldSymbol pEFieldSymbol)
            {
                handle = pEFieldSymbol.Handle;
                return true;
            }
            handle = default(FieldDefinitionHandle);
            return false;
        }

        public override bool TryGetMethodHandle(IMethodDefinition def, out MethodDefinitionHandle handle)
        {
            if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEMethodSymbol pEMethodSymbol)
            {
                handle = pEMethodSymbol.Handle;
                return true;
            }
            handle = default(MethodDefinitionHandle);
            return false;
        }

        public override bool TryGetPropertyHandle(IPropertyDefinition def, out PropertyDefinitionHandle handle)
        {
            if (_mapToMetadata.MapDefinition(def)?.GetInternalSymbol() is PEPropertySymbol pEPropertySymbol)
            {
                handle = pEPropertySymbol.Handle;
                return true;
            }
            handle = default(PropertyDefinitionHandle);
            return false;
        }

        protected override void GetStateMachineFieldMapFromMetadata(ITypeSymbolInternal stateMachineType, ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo, out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap, out IReadOnlyDictionary<ITypeReference, int> awaiterMap, out int awaiterSlotCount)
        {
            Dictionary<EncHoistedLocalInfo, int> dictionary = new Dictionary<EncHoistedLocalInfo, int>();
            Dictionary<ITypeReference, int> dictionary2 = new Dictionary<ITypeReference, int>(SymbolEquivalentEqualityComparer.Instance);
            int num = -1;
            ImmutableArray<Symbol>.Enumerator enumerator = ((Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol)stateMachineType).GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Field)
                {
                    continue;
                }
                string name = current.Name;
                int slotIndex;
                switch (GeneratedNames.GetKind(name))
                {
                    case GeneratedNameKind.AwaiterField:
                        if (GeneratedNames.TryParseSlotIndex(name, out slotIndex))
                        {
                            Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol fieldSymbol2 = (Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol)current;
                            dictionary2[(ITypeReference)fieldSymbol2.Type.GetCciAdapter()] = slotIndex;
                            if (slotIndex > num)
                            {
                                num = slotIndex;
                            }
                        }
                        break;
                    case GeneratedNameKind.HoistedLocalField:
                    case GeneratedNameKind.HoistedSynthesizedLocalField:
                        if (GeneratedNames.TryParseSlotIndex(name, out slotIndex))
                        {
                            Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol fieldSymbol = (Microsoft.CodeAnalysis.CSharp.Symbols.FieldSymbol)current;
                            if (slotIndex < localSlotDebugInfo.Length)
                            {
                                EncHoistedLocalInfo key = new EncHoistedLocalInfo(localSlotDebugInfo[slotIndex], (ITypeReference)fieldSymbol.Type.GetCciAdapter());
                                dictionary[key] = slotIndex;
                            }
                        }
                        break;
                }
            }
            hoistedLocalMap = dictionary;
            awaiterMap = dictionary2;
            awaiterSlotCount = num + 1;
        }

        protected override ImmutableArray<EncLocalInfo> GetLocalSlotMapFromMetadata(StandaloneSignatureHandle handle, EditAndContinueMethodDebugInformation debugInfo)
        {
            ImmutableArray<LocalInfo<Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol>> localsOrThrow = _metadataDecoder.GetLocalsOrThrow(handle);
            return CreateLocalSlotMap(debugInfo, localsOrThrow);
        }

        protected override ITypeSymbolInternal? TryGetStateMachineType(EntityHandle methodHandle)
        {
            if (_metadataDecoder.Module.HasStringValuedAttribute(methodHandle, AttributeDescription.AsyncStateMachineAttribute, out var value) || _metadataDecoder.Module.HasStringValuedAttribute(methodHandle, AttributeDescription.IteratorStateMachineAttribute, out value))
            {
                return _metadataDecoder.GetTypeSymbolForSerializedType(value);
            }
            return null;
        }

        private static ImmutableArray<EncLocalInfo> CreateLocalSlotMap(EditAndContinueMethodDebugInformation methodEncInfo, ImmutableArray<LocalInfo<Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol>> slotMetadata)
        {
            EncLocalInfo[] array = new EncLocalInfo[slotMetadata.Length];
            ImmutableArray<LocalSlotDebugInfo> localSlots = methodEncInfo.LocalSlots;
            if (!localSlots.IsDefault)
            {
                int num = Math.Min(localSlots.Length, slotMetadata.Length);
                Dictionary<EncLocalInfo, int> dictionary = new Dictionary<EncLocalInfo, int>();
                for (int i = 0; i < num; i++)
                {
                    LocalSlotDebugInfo slotInfo = localSlots[i];
                    if (slotInfo.SynthesizedKind.IsLongLived())
                    {
                        LocalInfo<Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol> localInfo = slotMetadata[i];
                        if (localInfo.CustomModifiers.IsDefaultOrEmpty)
                        {
                            EncLocalInfo key = new EncLocalInfo(slotInfo, (ITypeReference)localInfo.Type.GetCciAdapter(), localInfo.Constraints, localInfo.SignatureOpt);
                            dictionary.Add(key, i);
                        }
                    }
                }
                foreach (KeyValuePair<EncLocalInfo, int> item in dictionary)
                {
                    array[item.Value] = item.Key;
                }
            }
            for (int j = 0; j < array.Length; j++)
            {
                if (array[j].IsDefault)
                {
                    array[j] = new EncLocalInfo(slotMetadata[j].SignatureOpt);
                }
            }
            return ImmutableArray.Create(array);
        }
    }
}
