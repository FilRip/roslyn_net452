using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public abstract class DefinitionMap
    {
        protected readonly struct MappedMethod
        {
            public readonly IMethodSymbolInternal PreviousMethod;

            public readonly Func<SyntaxNode, SyntaxNode?>? SyntaxMap;

            public MappedMethod(IMethodSymbolInternal previousMethod, Func<SyntaxNode, SyntaxNode?>? syntaxMap)
            {
                PreviousMethod = previousMethod;
                SyntaxMap = syntaxMap;
            }
        }

        protected readonly IReadOnlyDictionary<IMethodSymbolInternal, MappedMethod> mappedMethods;

        protected abstract SymbolMatcher MapToMetadataSymbolMatcher { get; }

        protected abstract SymbolMatcher MapToPreviousSymbolMatcher { get; }

        public abstract CommonMessageProvider MessageProvider { get; }

        protected DefinitionMap(IEnumerable<SemanticEdit> edits)
        {
            mappedMethods = GetMappedMethods(edits);
        }

        private IReadOnlyDictionary<IMethodSymbolInternal, MappedMethod> GetMappedMethods(IEnumerable<SemanticEdit> edits)
        {
            Dictionary<IMethodSymbolInternal, MappedMethod> dictionary = new Dictionary<IMethodSymbolInternal, MappedMethod>();
            foreach (SemanticEdit edit in edits)
            {
                if (edit.Kind == SemanticEditKind.Update && edit.PreserveLocalVariables && GetISymbolInternalOrNull(edit.NewSymbol) is IMethodSymbolInternal key && GetISymbolInternalOrNull(edit.OldSymbol) is IMethodSymbolInternal previousMethod)
                {
                    dictionary.Add(key, new MappedMethod(previousMethod, edit.SyntaxMap));
                }
            }
            return dictionary;
        }

        protected abstract ISymbolInternal? GetISymbolInternalOrNull(ISymbol symbol);

        internal IDefinition? MapDefinition(IDefinition definition)
        {
            IDefinition? definition2 = MapToPreviousSymbolMatcher.MapDefinition(definition);
            if (definition2 == null)
            {
                if (MapToMetadataSymbolMatcher == MapToPreviousSymbolMatcher)
                {
                    return null;
                }
                definition2 = MapToMetadataSymbolMatcher.MapDefinition(definition);
            }
            return definition2;
        }

        internal INamespace? MapNamespace(INamespace @namespace)
        {
            INamespace? namespace2 = MapToPreviousSymbolMatcher.MapNamespace(@namespace);
            if (namespace2 == null)
            {
                if (MapToMetadataSymbolMatcher == MapToPreviousSymbolMatcher)
                {
                    return null;
                }
                namespace2 = MapToMetadataSymbolMatcher.MapNamespace(@namespace);
            }
            return namespace2;
        }

        internal bool DefinitionExists(IDefinition definition)
        {
            return MapDefinition(definition) != null;
        }

        internal bool NamespaceExists(INamespace @namespace)
        {
            return MapNamespace(@namespace) != null;
        }

        public abstract bool TryGetTypeHandle(ITypeDefinition def, out TypeDefinitionHandle handle);

        public abstract bool TryGetEventHandle(IEventDefinition def, out EventDefinitionHandle handle);

        public abstract bool TryGetFieldHandle(IFieldDefinition def, out FieldDefinitionHandle handle);

        public abstract bool TryGetMethodHandle(IMethodDefinition def, out MethodDefinitionHandle handle);

        public abstract bool TryGetPropertyHandle(IPropertyDefinition def, out PropertyDefinitionHandle handle);

        private bool TryGetMethodHandle(EmitBaseline baseline, IMethodDefinition def, out MethodDefinitionHandle handle)
        {
            if (TryGetMethodHandle(def, out handle))
            {
                return true;
            }
            IMethodDefinition methodDefinition = (IMethodDefinition)MapToPreviousSymbolMatcher.MapDefinition(def);
            if (methodDefinition != null && baseline.MethodsAdded.TryGetValue(methodDefinition, out var value))
            {
                handle = MetadataTokens.MethodDefinitionHandle(value);
                return true;
            }
            handle = default(MethodDefinitionHandle);
            return false;
        }

        protected static IReadOnlyDictionary<SyntaxNode, int> CreateDeclaratorToSyntaxOrdinalMap(ImmutableArray<SyntaxNode> declarators)
        {
            Dictionary<SyntaxNode, int> dictionary = new Dictionary<SyntaxNode, int>();
            for (int i = 0; i < declarators.Length; i++)
            {
                dictionary.Add(declarators[i], i);
            }
            return dictionary;
        }

        protected abstract void GetStateMachineFieldMapFromMetadata(ITypeSymbolInternal stateMachineType, ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo, out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap, out IReadOnlyDictionary<ITypeReference, int> awaiterMap, out int awaiterSlotCount);

        protected abstract ImmutableArray<EncLocalInfo> GetLocalSlotMapFromMetadata(StandaloneSignatureHandle handle, EditAndContinueMethodDebugInformation debugInfo);

        protected abstract ITypeSymbolInternal? TryGetStateMachineType(EntityHandle methodHandle);

        public VariableSlotAllocator? TryCreateVariableSlotAllocator(EmitBaseline baseline, Compilation compilation, IMethodSymbolInternal method, IMethodSymbolInternal topLevelMethod, DiagnosticBag diagnostics)
        {
            if (!mappedMethods.TryGetValue(topLevelMethod, out var value))
            {
                return null;
            }
            if (!TryGetMethodHandle(baseline, (IMethodDefinition)method.GetCciAdapter(), out var handle))
            {
                return null;
            }
            IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap = null;
            IReadOnlyDictionary<ITypeReference, int> awaiterMap = null;
            IReadOnlyDictionary<int, KeyValuePair<DebugId, int>> lambdaMap = null;
            IReadOnlyDictionary<int, DebugId> closureMap = null;
            int hoistedLocalSlotCount = 0;
            int awaiterSlotCount = 0;
            string stateMachineTypeName = null;
            int rowNumber = MetadataTokens.GetRowNumber(handle);
            DebugId methodId;
            ImmutableArray<EncLocalInfo> previousLocals;
            SymbolMatcher symbolMap;
            if (baseline.AddedOrChangedMethods.TryGetValue(rowNumber, out var value2))
            {
                methodId = value2.MethodId;
                MakeLambdaAndClosureMaps(value2.LambdaDebugInfo, value2.ClosureDebugInfo, out lambdaMap, out closureMap);
                if (value2.StateMachineTypeName != null)
                {
                    GetStateMachineFieldMapFromPreviousCompilation(value2.StateMachineHoistedLocalSlotsOpt, value2.StateMachineAwaiterSlotsOpt, out hoistedLocalMap, out awaiterMap);
                    hoistedLocalSlotCount = value2.StateMachineHoistedLocalSlotsOpt.Length;
                    awaiterSlotCount = value2.StateMachineAwaiterSlotsOpt.Length;
                    previousLocals = ImmutableArray<EncLocalInfo>.Empty;
                    stateMachineTypeName = value2.StateMachineTypeName;
                }
                else
                {
                    previousLocals = value2.Locals;
                }
                symbolMap = MapToPreviousSymbolMatcher;
            }
            else
            {
                EditAndContinueMethodDebugInformation debugInfo;
                StandaloneSignatureHandle standaloneSignatureHandle;
                try
                {
                    debugInfo = baseline.DebugInformationProvider(handle);
                    standaloneSignatureHandle = baseline.LocalSignatureProvider(handle);
                }
                catch (Exception ex) when (ex is InvalidDataException || ex is IOException)
                {
                    diagnostics.Add(MessageProvider.CreateDiagnostic(MessageProvider.ERR_InvalidDebugInfo, method.Locations.First(), method, MetadataTokens.GetToken(handle), method.ContainingAssembly));
                    return null;
                }
                methodId = new DebugId(debugInfo.MethodOrdinal, 0);
                if (!debugInfo.Lambdas.IsDefaultOrEmpty)
                {
                    MakeLambdaAndClosureMaps(debugInfo.Lambdas, debugInfo.Closures, out lambdaMap, out closureMap);
                }
                ITypeSymbolInternal typeSymbolInternal = TryGetStateMachineType(handle);
                if (typeSymbolInternal != null)
                {
                    ImmutableArray<LocalSlotDebugInfo> localSlotDebugInfo = debugInfo.LocalSlots.NullToEmpty();
                    GetStateMachineFieldMapFromMetadata(typeSymbolInternal, localSlotDebugInfo, out hoistedLocalMap, out awaiterMap, out awaiterSlotCount);
                    hoistedLocalSlotCount = localSlotDebugInfo.Length;
                    previousLocals = ImmutableArray<EncLocalInfo>.Empty;
                    stateMachineTypeName = typeSymbolInternal.Name;
                }
                else
                {
                    if (method.IsAsync)
                    {
                        if (compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor) == null)
                        {
                            ReportMissingStateMachineAttribute(diagnostics, method, AttributeDescription.AsyncStateMachineAttribute.FullName);
                            return null;
                        }
                    }
                    else if (method.IsIterator && compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor) == null)
                    {
                        ReportMissingStateMachineAttribute(diagnostics, method, AttributeDescription.IteratorStateMachineAttribute.FullName);
                        return null;
                    }
                    try
                    {
                        previousLocals = (standaloneSignatureHandle.IsNil ? ImmutableArray<EncLocalInfo>.Empty : GetLocalSlotMapFromMetadata(standaloneSignatureHandle, debugInfo));
                    }
                    catch (Exception ex2) when (ex2 is UnsupportedSignatureContent || ex2 is BadImageFormatException || ex2 is IOException)
                    {
                        diagnostics.Add(MessageProvider.CreateDiagnostic(MessageProvider.ERR_InvalidDebugInfo, method.Locations.First(), method, MetadataTokens.GetToken(standaloneSignatureHandle), method.ContainingAssembly));
                        return null;
                    }
                }
                symbolMap = MapToMetadataSymbolMatcher;
            }
            return new EncVariableSlotAllocator(symbolMap, value.SyntaxMap, value.PreviousMethod, methodId, previousLocals, lambdaMap, closureMap, stateMachineTypeName, hoistedLocalSlotCount, hoistedLocalMap, awaiterSlotCount, awaiterMap, GetLambdaSyntaxFacts());
        }

        protected abstract LambdaSyntaxFacts GetLambdaSyntaxFacts();

        private void ReportMissingStateMachineAttribute(DiagnosticBag diagnostics, IMethodSymbolInternal method, string stateMachineAttributeFullName)
        {
            diagnostics.Add(MessageProvider.CreateDiagnostic(MessageProvider.ERR_EncUpdateFailedMissingAttribute, method.Locations.First(), MessageProvider.GetErrorDisplayString(method.GetISymbol()), stateMachineAttributeFullName));
        }

        private static void MakeLambdaAndClosureMaps(ImmutableArray<LambdaDebugInfo> lambdaDebugInfo, ImmutableArray<ClosureDebugInfo> closureDebugInfo, out IReadOnlyDictionary<int, KeyValuePair<DebugId, int>> lambdaMap, out IReadOnlyDictionary<int, DebugId> closureMap)
        {
            Dictionary<int, KeyValuePair<DebugId, int>> dictionary = new Dictionary<int, KeyValuePair<DebugId, int>>(lambdaDebugInfo.Length);
            Dictionary<int, DebugId> dictionary2 = new Dictionary<int, DebugId>(closureDebugInfo.Length);
            for (int i = 0; i < lambdaDebugInfo.Length; i++)
            {
                LambdaDebugInfo lambdaDebugInfo2 = lambdaDebugInfo[i];
                dictionary[lambdaDebugInfo2.SyntaxOffset] = KeyValuePairUtil.Create(lambdaDebugInfo2.LambdaId, lambdaDebugInfo2.ClosureOrdinal);
            }
            for (int j = 0; j < closureDebugInfo.Length; j++)
            {
                ClosureDebugInfo closureDebugInfo2 = closureDebugInfo[j];
                dictionary2[closureDebugInfo2.SyntaxOffset] = closureDebugInfo2.ClosureId;
            }
            lambdaMap = dictionary;
            closureMap = dictionary2;
        }

        private static void GetStateMachineFieldMapFromPreviousCompilation(ImmutableArray<EncHoistedLocalInfo> hoistedLocalSlots, ImmutableArray<ITypeReference?> hoistedAwaiters, out IReadOnlyDictionary<EncHoistedLocalInfo, int> hoistedLocalMap, out IReadOnlyDictionary<ITypeReference, int> awaiterMap)
        {
            Dictionary<EncHoistedLocalInfo, int> dictionary = new Dictionary<EncHoistedLocalInfo, int>();
            Dictionary<ITypeReference, int> dictionary2 = new Dictionary<ITypeReference, int>(SymbolEquivalentEqualityComparer.Instance);
            for (int i = 0; i < hoistedLocalSlots.Length; i++)
            {
                EncHoistedLocalInfo key = hoistedLocalSlots[i];
                if (!key.IsUnused)
                {
                    dictionary.Add(key, i);
                }
            }
            for (int j = 0; j < hoistedAwaiters.Length; j++)
            {
                ITypeReference typeReference = hoistedAwaiters[j];
                if (typeReference != null)
                {
                    dictionary2.Add(typeReference, j);
                }
            }
            hoistedLocalMap = dictionary;
            awaiterMap = dictionary2;
        }
    }
}
