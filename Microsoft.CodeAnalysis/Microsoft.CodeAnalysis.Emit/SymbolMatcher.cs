using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class SymbolMatcher
    {
        public abstract ITypeReference? MapReference(ITypeReference reference);

        public abstract IDefinition? MapDefinition(IDefinition definition);

        public abstract INamespace? MapNamespace(INamespace @namespace);

        public ISymbolInternal? MapDefinitionOrNamespace(ISymbolInternal symbol)
        {
            IReference cciAdapter = symbol.GetCciAdapter();
            if (!(cciAdapter is IDefinition definition))
            {
                return MapNamespace((INamespace)cciAdapter)?.GetInternalSymbol();
            }
            return MapDefinition(definition)?.GetInternalSymbol();
        }

        public EmitBaseline MapBaselineToCompilation(EmitBaseline baseline, Compilation targetCompilation, CommonPEModuleBuilder targetModuleBuilder, ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> mappedSynthesizedMembers)
        {
            IReadOnlyDictionary<ITypeDefinition, int> typesAdded = MapDefinitions(baseline.TypesAdded);
            IReadOnlyDictionary<IEventDefinition, int> eventsAdded = MapDefinitions(baseline.EventsAdded);
            IReadOnlyDictionary<IFieldDefinition, int> fieldsAdded = MapDefinitions(baseline.FieldsAdded);
            IReadOnlyDictionary<IMethodDefinition, int> methodsAdded = MapDefinitions(baseline.MethodsAdded);
            IReadOnlyDictionary<IPropertyDefinition, int> propertiesAdded = MapDefinitions(baseline.PropertiesAdded);
            return baseline.With(targetCompilation, targetModuleBuilder, baseline.Ordinal, baseline.EncId, typesAdded, eventsAdded, fieldsAdded, methodsAdded, propertiesAdded, baseline.EventMapAdded, baseline.PropertyMapAdded, baseline.MethodImplsAdded, baseline.TableEntriesAdded, baseline.BlobStreamLengthAdded, baseline.StringStreamLengthAdded, baseline.UserStringStreamLengthAdded, baseline.GuidStreamLengthAdded, MapAnonymousTypes(baseline.AnonymousTypeMap), mappedSynthesizedMembers, MapAddedOrChangedMethods(baseline.AddedOrChangedMethods), baseline.DebugInformationProvider, baseline.LocalSignatureProvider);
        }

        private IReadOnlyDictionary<K, V> MapDefinitions<K, V>(IReadOnlyDictionary<K, V> items) where K : class, IDefinition
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>(SymbolEquivalentEqualityComparer.Instance);
            foreach (KeyValuePair<K, V> item in items)
            {
                K val = (K)MapDefinition(item.Key);
                if (val != null)
                {
                    dictionary.Add(val, item.Value);
                }
            }
            return dictionary;
        }

        private IReadOnlyDictionary<int, AddedOrChangedMethodInfo> MapAddedOrChangedMethods(IReadOnlyDictionary<int, AddedOrChangedMethodInfo> addedOrChangedMethods)
        {
            Dictionary<int, AddedOrChangedMethodInfo> dictionary = new Dictionary<int, AddedOrChangedMethodInfo>();
            foreach (KeyValuePair<int, AddedOrChangedMethodInfo> addedOrChangedMethod in addedOrChangedMethods)
            {
                dictionary.Add(addedOrChangedMethod.Key, addedOrChangedMethod.Value.MapTypes(this));
            }
            return dictionary;
        }

        private IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> MapAnonymousTypes(IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> anonymousTypeMap)
        {
            Dictionary<AnonymousTypeKey, AnonymousTypeValue> dictionary = new Dictionary<AnonymousTypeKey, AnonymousTypeValue>();
            foreach (KeyValuePair<AnonymousTypeKey, AnonymousTypeValue> item in anonymousTypeMap)
            {
                AnonymousTypeKey key = item.Key;
                AnonymousTypeValue value = item.Value;
                ITypeDefinition type = (ITypeDefinition)MapDefinition(value.Type);
                dictionary.Add(key, new AnonymousTypeValue(value.Name, value.UniqueIndex, type));
            }
            return dictionary;
        }

        public ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> MapSynthesizedMembers(ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> previousMembers, ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>> newMembers)
        {
            if (previousMembers.Count == 0)
            {
                return newMembers;
            }
            ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>>.Builder builder = ImmutableDictionary.CreateBuilder<ISymbolInternal, ImmutableArray<ISymbolInternal>>();
            builder.AddRange(newMembers);
            foreach (KeyValuePair<ISymbolInternal, ImmutableArray<ISymbolInternal>> previousMember in previousMembers)
            {
                ISymbolInternal key = previousMember.Key;
                ImmutableArray<ISymbolInternal> value = previousMember.Value;
                ISymbolInternal symbolInternal = MapDefinitionOrNamespace(key);
                if (symbolInternal == null)
                {
                    builder.Add(key, value);
                    continue;
                }
                if (!newMembers.TryGetValue(symbolInternal, out var value2))
                {
                    builder.Add(symbolInternal, value);
                    continue;
                }
                ArrayBuilder<ISymbolInternal> instance = ArrayBuilder<ISymbolInternal>.GetInstance();
                instance.AddRange(value2);
                ImmutableArray<ISymbolInternal>.Enumerator enumerator2 = value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ISymbolInternal current2 = enumerator2.Current;
                    if (MapDefinitionOrNamespace(current2) == null)
                    {
                        instance.Add(current2);
                    }
                }
                builder[symbolInternal] = instance.ToImmutableAndFree();
            }
            return builder.ToImmutable();
        }
    }
}
