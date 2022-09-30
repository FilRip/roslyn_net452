using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalysisState
    {
        public class AnalyzerStateData
        {
            public static readonly AnalyzerStateData FullyProcessedInstance = CreateFullyProcessedInstance();

            public StateKind StateKind { get; private set; }

            public HashSet<AnalyzerAction> ProcessedActions { get; }

            public AnalyzerStateData()
            {
                StateKind = StateKind.InProcess;
                ProcessedActions = new HashSet<AnalyzerAction>();
            }

            private static AnalyzerStateData CreateFullyProcessedInstance()
            {
                AnalyzerStateData analyzerStateData = new();
                analyzerStateData.SetStateKind(StateKind.FullyProcessed);
                return analyzerStateData;
            }

            public virtual void SetStateKind(StateKind stateKind)
            {
                StateKind = stateKind;
            }

            public void ResetToReadyState()
            {
                SetStateKind(StateKind.ReadyToProcess);
            }

            public virtual void Free()
            {
                StateKind = StateKind.ReadyToProcess;
                ProcessedActions.Clear();
            }
        }

        private class PerAnalyzerState
        {
            private readonly object _gate;

            private readonly Dictionary<CompilationEvent, AnalyzerStateData?> _pendingEvents;

            private readonly Dictionary<ISymbol, AnalyzerStateData?> _pendingSymbols;

            private readonly Dictionary<ISymbol, Dictionary<int, DeclarationAnalyzerStateData>?> _pendingDeclarations;

            private Dictionary<SourceOrAdditionalFile, AnalyzerStateData>? _lazyFilesWithAnalysisData;

            private int _pendingSyntaxAnalysisFilesCount;

            private Dictionary<ISymbol, AnalyzerStateData?>? _lazyPendingSymbolEndAnalyses;

            private readonly ObjectPool<AnalyzerStateData> _analyzerStateDataPool;

            private readonly ObjectPool<DeclarationAnalyzerStateData> _declarationAnalyzerStateDataPool;

            private readonly ObjectPool<Dictionary<int, DeclarationAnalyzerStateData>> _currentlyAnalyzingDeclarationsMapPool;

            public PerAnalyzerState(ObjectPool<AnalyzerStateData> analyzerStateDataPool, ObjectPool<DeclarationAnalyzerStateData> declarationAnalyzerStateDataPool, ObjectPool<Dictionary<int, DeclarationAnalyzerStateData>> currentlyAnalyzingDeclarationsMapPool)
            {
                _gate = new object();
                _pendingEvents = new Dictionary<CompilationEvent, AnalyzerStateData?>();
                _pendingSymbols = new Dictionary<ISymbol, AnalyzerStateData?>();
                _pendingDeclarations = new Dictionary<ISymbol, Dictionary<int, DeclarationAnalyzerStateData>?>();
                _analyzerStateDataPool = analyzerStateDataPool;
                _declarationAnalyzerStateDataPool = declarationAnalyzerStateDataPool;
                _currentlyAnalyzingDeclarationsMapPool = currentlyAnalyzingDeclarationsMapPool;
            }

            public void AddPendingEvents(HashSet<CompilationEvent> uniqueEvents)
            {
                lock (_gate)
                {
                    foreach (CompilationEvent key in _pendingEvents.Keys)
                    {
                        uniqueEvents.Add(key);
                    }
                }
            }

            public bool HasPendingSyntaxAnalysis(SourceOrAdditionalFile? file)
            {
                lock (_gate)
                {
                    if (_pendingSyntaxAnalysisFilesCount == 0)
                    {
                        return false;
                    }
                    if (!file.HasValue)
                    {
                        return true;
                    }
                    if (!_lazyFilesWithAnalysisData!.TryGetValue(file.Value, out var value))
                    {
                        return true;
                    }
                    return value.StateKind != StateKind.FullyProcessed;
                }
            }

            public bool HasPendingSymbolAnalysis(ISymbol symbol)
            {
                lock (_gate)
                {
                    return _pendingSymbols.ContainsKey(symbol) || (_lazyPendingSymbolEndAnalyses?.ContainsKey(symbol) ?? false);
                }
            }

            private bool TryStartProcessingEntity<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities, ObjectPool<TAnalyzerStateData> pool, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TAnalyzerStateData? newState) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData, new()
            {
                lock (_gate)
                {
                    return TryStartProcessingEntity_NoLock(analysisEntity, pendingEntities, pool, out newState);
                }
            }

            private static bool TryStartProcessingEntity_NoLock<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities, ObjectPool<TAnalyzerStateData> pool, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out TAnalyzerStateData? state) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData
            {
                if (pendingEntities.TryGetValue(analysisEntity, out state) && (state == null || state!.StateKind == StateKind.ReadyToProcess))
                {
                    if (state == null)
                    {
                        state = pool.Allocate();
                    }
                    state!.SetStateKind(StateKind.InProcess);
                    pendingEntities[analysisEntity] = state;
                    return true;
                }
                state = null;
                return false;
            }

            private void MarkEntityProcessed<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities, ObjectPool<TAnalyzerStateData> pool) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData
            {
                lock (_gate)
                {
                    MarkEntityProcessed_NoLock(analysisEntity, pendingEntities, pool);
                }
            }

            private static bool MarkEntityProcessed_NoLock<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities, ObjectPool<TAnalyzerStateData> pool) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData
            {
                if (pendingEntities.TryGetValue(analysisEntity, out var value))
                {
                    pendingEntities.Remove(analysisEntity);
                    FreeState_NoLock(value, pool);
                    return true;
                }
                return false;
            }

            private bool TryStartSyntaxAnalysis_NoLock(SourceOrAdditionalFile file, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
            {
                if (_pendingSyntaxAnalysisFilesCount == 0)
                {
                    state = null;
                    return false;
                }
                if (_lazyFilesWithAnalysisData!.TryGetValue(file, out state))
                {
                    if (state!.StateKind != 0)
                    {
                        state = null;
                        return false;
                    }
                }
                else
                {
                    state = _analyzerStateDataPool.Allocate();
                }
                state!.SetStateKind(StateKind.InProcess);
                _lazyFilesWithAnalysisData![file] = state;
                return true;
            }

            private void MarkSyntaxAnalysisComplete_NoLock(SourceOrAdditionalFile file)
            {
                if (_pendingSyntaxAnalysisFilesCount == 0)
                {
                    return;
                }
                bool flag = false;
                if (_lazyFilesWithAnalysisData!.TryGetValue(file, out var value))
                {
                    if (value.StateKind != StateKind.FullyProcessed)
                    {
                        FreeState_NoLock(value, _analyzerStateDataPool);
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    _pendingSyntaxAnalysisFilesCount--;
                }
                _lazyFilesWithAnalysisData![file] = AnalyzerStateData.FullyProcessedInstance;
            }

            private Dictionary<int, DeclarationAnalyzerStateData> EnsureDeclarationDataMap_NoLock(ISymbol symbol, Dictionary<int, DeclarationAnalyzerStateData>? declarationDataMap)
            {
                if (declarationDataMap == null)
                {
                    declarationDataMap = _currentlyAnalyzingDeclarationsMapPool.Allocate();
                    _pendingDeclarations[symbol] = declarationDataMap;
                }
                return declarationDataMap;
            }

            private bool TryStartAnalyzingDeclaration_NoLock(ISymbol symbol, int declarationIndex, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out DeclarationAnalyzerStateData? state)
            {
                if (!_pendingDeclarations.TryGetValue(symbol, out var value))
                {
                    state = null;
                    return false;
                }
                value = EnsureDeclarationDataMap_NoLock(symbol, value);
                if (value.TryGetValue(declarationIndex, out state))
                {
                    if (state!.StateKind != 0)
                    {
                        state = null;
                        return false;
                    }
                }
                else
                {
                    state = _declarationAnalyzerStateDataPool.Allocate();
                }
                state!.SetStateKind(StateKind.InProcess);
                value[declarationIndex] = state;
                return true;
            }

            private void MarkDeclarationProcessed_NoLock(ISymbol symbol, int declarationIndex)
            {
                if (_pendingDeclarations.TryGetValue(symbol, out var value))
                {
                    value = EnsureDeclarationDataMap_NoLock(symbol, value);
                    if (value.TryGetValue(declarationIndex, out var value2))
                    {
                        FreeDeclarationAnalyzerState_NoLock(value2);
                    }
                    value[declarationIndex] = DeclarationAnalyzerStateData.FullyProcessedInstance;
                }
            }

            private void MarkDeclarationsProcessed_NoLock(ISymbol symbol)
            {
                if (_pendingDeclarations.TryGetValue(symbol, out var value))
                {
                    FreeDeclarationDataMap_NoLock(value);
                    _pendingDeclarations.Remove(symbol);
                }
            }

            private void FreeDeclarationDataMap_NoLock(Dictionary<int, DeclarationAnalyzerStateData>? declarationDataMap)
            {
                if (declarationDataMap != null)
                {
                    declarationDataMap!.Clear();
                    _currentlyAnalyzingDeclarationsMapPool.Free(declarationDataMap);
                }
            }

            private void FreeDeclarationAnalyzerState_NoLock(DeclarationAnalyzerStateData state)
            {
                if (state != DeclarationAnalyzerStateData.FullyProcessedInstance)
                {
                    FreeState_NoLock(state, _declarationAnalyzerStateDataPool);
                }
            }

            private static void FreeState_NoLock<TAnalyzerStateData>(TAnalyzerStateData? state, ObjectPool<TAnalyzerStateData> pool) where TAnalyzerStateData : AnalyzerStateData
            {
                if (state != null && state != AnalyzerStateData.FullyProcessedInstance)
                {
                    state!.Free();
                    pool.Free(state);
                }
            }

            private bool IsEntityFullyProcessed<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData
            {
                lock (_gate)
                {
                    return IsEntityFullyProcessed_NoLock(analysisEntity, pendingEntities);
                }
            }

            private static bool IsEntityFullyProcessed_NoLock<TAnalysisEntity, TAnalyzerStateData>(TAnalysisEntity analysisEntity, Dictionary<TAnalysisEntity, TAnalyzerStateData?> pendingEntities) where TAnalysisEntity : notnull where TAnalyzerStateData : AnalyzerStateData
            {
                if (pendingEntities.TryGetValue(analysisEntity, out var value))
                {
                    TAnalyzerStateData? val = value;
                    if (val == null)
                    {
                        return false;
                    }
                    return val.StateKind == StateKind.FullyProcessed;
                }
                return true;
            }

            private bool IsDeclarationComplete_NoLock(ISymbol symbol, int declarationIndex)
            {
                if (!_pendingDeclarations.TryGetValue(symbol, out var value))
                {
                    return true;
                }
                if (value == null || !value.TryGetValue(declarationIndex, out var value2))
                {
                    return false;
                }
                return value2.StateKind == StateKind.FullyProcessed;
            }

            private bool AreDeclarationsProcessed_NoLock(ISymbol symbol, int declarationsCount)
            {
                if (!_pendingDeclarations.TryGetValue(symbol, out var value))
                {
                    return true;
                }
                if (value != null && value.Count == declarationsCount)
                {
                    return value.Values.All((DeclarationAnalyzerStateData state) => state.StateKind == StateKind.FullyProcessed);
                }
                return false;
            }

            public bool TryStartProcessingEvent(CompilationEvent compilationEvent, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
            {
                return TryStartProcessingEntity(compilationEvent, _pendingEvents, _analyzerStateDataPool, out state);
            }

            public void MarkEventComplete(CompilationEvent compilationEvent)
            {
                MarkEntityProcessed(compilationEvent, _pendingEvents, _analyzerStateDataPool);
            }

            public bool TryStartAnalyzingSymbol(ISymbol symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
            {
                return TryStartProcessingEntity(symbol, _pendingSymbols, _analyzerStateDataPool, out state);
            }

            public bool TryStartSymbolEndAnalysis(ISymbol symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
            {
                return TryStartProcessingEntity(symbol, _lazyPendingSymbolEndAnalyses!, _analyzerStateDataPool, out state);
            }

            public void MarkSymbolComplete(ISymbol symbol)
            {
                MarkEntityProcessed(symbol, _pendingSymbols, _analyzerStateDataPool);
            }

            public void MarkSymbolEndAnalysisComplete(ISymbol symbol)
            {
                if (_lazyPendingSymbolEndAnalyses != null)
                {
                    MarkEntityProcessed(symbol, _lazyPendingSymbolEndAnalyses, _analyzerStateDataPool);
                }
            }

            public bool TryStartAnalyzingDeclaration(ISymbol symbol, int declarationIndex, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out DeclarationAnalyzerStateData? state)
            {
                lock (_gate)
                {
                    return TryStartAnalyzingDeclaration_NoLock(symbol, declarationIndex, out state);
                }
            }

            public bool IsDeclarationComplete(ISymbol symbol, int declarationIndex)
            {
                lock (_gate)
                {
                    return IsDeclarationComplete_NoLock(symbol, declarationIndex);
                }
            }

            public void MarkDeclarationComplete(ISymbol symbol, int declarationIndex)
            {
                lock (_gate)
                {
                    MarkDeclarationProcessed_NoLock(symbol, declarationIndex);
                }
            }

            public void MarkDeclarationsComplete(ISymbol symbol)
            {
                lock (_gate)
                {
                    MarkDeclarationsProcessed_NoLock(symbol);
                }
            }

            public bool TryStartSyntaxAnalysis(SourceOrAdditionalFile tree, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
            {
                lock (_gate)
                {
                    return TryStartSyntaxAnalysis_NoLock(tree, out state);
                }
            }

            public void MarkSyntaxAnalysisComplete(SourceOrAdditionalFile file)
            {
                lock (_gate)
                {
                    MarkSyntaxAnalysisComplete_NoLock(file);
                }
            }

            public void OnCompilationEventGenerated(CompilationEvent compilationEvent, AnalyzerActionCounts actionCounts)
            {
                lock (_gate)
                {
                    if (compilationEvent is SymbolDeclaredCompilationEvent symbolDeclaredCompilationEvent)
                    {
                        bool flag = false;
                        ISymbol symbol = symbolDeclaredCompilationEvent.Symbol;
                        bool flag2 = AnalysisScope.ShouldSkipSymbolAnalysis(symbolDeclaredCompilationEvent);
                        if (!flag2 && actionCounts.SymbolActionsCount > 0)
                        {
                            flag = true;
                            _pendingSymbols[symbol] = null;
                        }
                        bool flag3 = AnalysisScope.ShouldSkipDeclarationAnalysis(symbol);
                        if (!flag3 && actionCounts.HasAnyExecutableCodeActions)
                        {
                            flag = true;
                            _pendingDeclarations[symbol] = null;
                        }
                        if (actionCounts.SymbolStartActionsCount > 0 && (!flag2 || !flag3))
                        {
                            flag = true;
                            if (_lazyPendingSymbolEndAnalyses == null)
                            {
                                _lazyPendingSymbolEndAnalyses = new Dictionary<ISymbol, AnalyzerStateData?>();
                            }
                            _lazyPendingSymbolEndAnalyses![symbol] = null;
                        }
                        if (!flag)
                        {
                            return;
                        }
                    }
                    else if (compilationEvent is CompilationStartedEvent compilationStartedEvent)
                    {
                        int num = ((actionCounts.SyntaxTreeActionsCount > 0) ? compilationEvent.Compilation.SyntaxTrees.Count() : 0);
                        num += ((actionCounts.AdditionalFileActionsCount > 0) ? compilationStartedEvent.AdditionalFiles.Length : 0);
                        if (num > 0)
                        {
                            _lazyFilesWithAnalysisData = new Dictionary<SourceOrAdditionalFile, AnalyzerStateData>();
                            _pendingSyntaxAnalysisFilesCount = num;
                        }
                        if (actionCounts.CompilationActionsCount == 0)
                        {
                            return;
                        }
                    }
                    _pendingEvents[compilationEvent] = null;
                }
            }

            public bool IsEventAnalyzed(CompilationEvent compilationEvent)
            {
                return IsEntityFullyProcessed(compilationEvent, _pendingEvents);
            }

            public bool IsSymbolComplete(ISymbol symbol)
            {
                return IsEntityFullyProcessed(symbol, _pendingSymbols);
            }

            public bool IsSymbolEndAnalysisComplete(ISymbol symbol)
            {
                return IsEntityFullyProcessed(symbol, _lazyPendingSymbolEndAnalyses!);
            }

            public bool OnSymbolDeclaredEventProcessed(SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                lock (_gate)
                {
                    return OnSymbolDeclaredEventProcessed_NoLock(symbolDeclaredEvent);
                }
            }

            private bool OnSymbolDeclaredEventProcessed_NoLock(SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                if (!IsEntityFullyProcessed_NoLock(symbolDeclaredEvent.Symbol, _pendingSymbols))
                {
                    return false;
                }
                if (!AreDeclarationsProcessed_NoLock(symbolDeclaredEvent.Symbol, symbolDeclaredEvent.DeclaringSyntaxReferences.Length))
                {
                    return false;
                }
                if (_lazyPendingSymbolEndAnalyses != null && !IsEntityFullyProcessed_NoLock(symbolDeclaredEvent.Symbol, _lazyPendingSymbolEndAnalyses))
                {
                    return false;
                }
                MarkDeclarationsProcessed_NoLock(symbolDeclaredEvent.Symbol);
                return MarkEntityProcessed_NoLock(symbolDeclaredEvent, _pendingEvents, _analyzerStateDataPool);
            }
        }

        public enum StateKind
        {
            ReadyToProcess,
            InProcess,
            FullyProcessed
        }

        public sealed class DeclarationAnalyzerStateData : SyntaxNodeAnalyzerStateData
        {
            public new static readonly DeclarationAnalyzerStateData FullyProcessedInstance = CreateFullyProcessedInstance();

            public CodeBlockAnalyzerStateData CodeBlockAnalysisState { get; }

            public OperationBlockAnalyzerStateData OperationBlockAnalysisState { get; }

            public DeclarationAnalyzerStateData()
            {
                CodeBlockAnalysisState = new CodeBlockAnalyzerStateData();
                OperationBlockAnalysisState = new OperationBlockAnalyzerStateData();
            }

            private static DeclarationAnalyzerStateData CreateFullyProcessedInstance()
            {
                DeclarationAnalyzerStateData declarationAnalyzerStateData = new();
                declarationAnalyzerStateData.SetStateKind(StateKind.FullyProcessed);
                return declarationAnalyzerStateData;
            }

            public override void SetStateKind(StateKind stateKind)
            {
                CodeBlockAnalysisState.SetStateKind(stateKind);
                OperationBlockAnalysisState.SetStateKind(stateKind);
                base.SetStateKind(stateKind);
            }

            public override void Free()
            {
                base.Free();
                CodeBlockAnalysisState.Free();
                OperationBlockAnalysisState.Free();
            }
        }

        public class SyntaxNodeAnalyzerStateData : AnalyzerStateData
        {
            public HashSet<SyntaxNode> ProcessedNodes { get; }

            public SyntaxNode? CurrentNode { get; set; }

            public SyntaxNodeAnalyzerStateData()
            {
                CurrentNode = null;
                ProcessedNodes = new HashSet<SyntaxNode>();
            }

            public void ClearNodeAnalysisState()
            {
                CurrentNode = null;
                base.ProcessedActions.Clear();
            }

            public override void Free()
            {
                base.Free();
                CurrentNode = null;
                ProcessedNodes.Clear();
            }
        }

        public class OperationAnalyzerStateData : AnalyzerStateData
        {
            public HashSet<IOperation> ProcessedOperations { get; }

            public IOperation? CurrentOperation { get; set; }

            public OperationAnalyzerStateData()
            {
                CurrentOperation = null;
                ProcessedOperations = new HashSet<IOperation>();
            }

            public void ClearNodeAnalysisState()
            {
                CurrentOperation = null;
                base.ProcessedActions.Clear();
            }

            public override void Free()
            {
                base.Free();
                CurrentOperation = null;
                ProcessedOperations.Clear();
            }
        }

        public abstract class BlockAnalyzerStateData<TBlockAction, TNodeStateData> : AnalyzerStateData where TBlockAction : AnalyzerAction where TNodeStateData : AnalyzerStateData, new()
        {
            public TNodeStateData ExecutableNodesAnalysisState { get; }

            public ImmutableHashSet<TBlockAction>? CurrentBlockEndActions { get; set; }

            public ImmutableHashSet<AnalyzerAction>? CurrentBlockNodeActions { get; set; }

            public BlockAnalyzerStateData()
            {
                ExecutableNodesAnalysisState = new TNodeStateData();
                CurrentBlockEndActions = null;
                CurrentBlockNodeActions = null;
            }

            public override void SetStateKind(StateKind stateKind)
            {
                ExecutableNodesAnalysisState.SetStateKind(stateKind);
                base.SetStateKind(stateKind);
            }

            public override void Free()
            {
                base.Free();
                ExecutableNodesAnalysisState.Free();
                CurrentBlockEndActions = null;
                CurrentBlockNodeActions = null;
            }
        }

        public sealed class CodeBlockAnalyzerStateData : BlockAnalyzerStateData<CodeBlockAnalyzerAction, SyntaxNodeAnalyzerStateData>
        {
        }

        public sealed class OperationBlockAnalyzerStateData : BlockAnalyzerStateData<OperationBlockAnalyzerAction, OperationAnalyzerStateData>
        {
        }

        private readonly SemaphoreSlim _gate;

        private readonly ImmutableDictionary<DiagnosticAnalyzer, int> _analyzerStateMap;

        private readonly ImmutableArray<PerAnalyzerState> _analyzerStates;

        private readonly Dictionary<SyntaxTree, HashSet<CompilationEvent>> _pendingSourceEvents;

        private readonly HashSet<CompilationEvent> _pendingNonSourceEvents;

        private ImmutableDictionary<DiagnosticAnalyzer, AnalyzerActionCounts>? _lazyAnalyzerActionCountsMap;

        private readonly HashSet<ISymbol> _partialSymbolsWithGeneratedSourceEvents;

        private readonly CachingSemanticModelProvider _semanticModelProvider;

        private readonly CompilationOptions _compilationOptions;

        private readonly ObjectPool<HashSet<CompilationEvent>> _compilationEventsPool;

        private readonly HashSet<CompilationEvent> _pooledEventsWithAnyActionsSet;

        private ImmutableDictionary<DiagnosticAnalyzer, AnalyzerActionCounts> AnalyzerActionCountsMap => _lazyAnalyzerActionCountsMap!;

        public AnalysisState(ImmutableArray<DiagnosticAnalyzer> analyzers, CachingSemanticModelProvider semanticModelProvider, CompilationOptions compilationOptions)
        {
            _gate = new SemaphoreSlim(1);
            _analyzerStateMap = CreateAnalyzerStateMap(analyzers, out _analyzerStates);
            _semanticModelProvider = semanticModelProvider;
            _compilationOptions = compilationOptions;
            _pendingSourceEvents = new Dictionary<SyntaxTree, HashSet<CompilationEvent>>();
            _pendingNonSourceEvents = new HashSet<CompilationEvent>();
            _partialSymbolsWithGeneratedSourceEvents = new HashSet<ISymbol>();
            _compilationEventsPool = new ObjectPool<HashSet<CompilationEvent>>(() => new HashSet<CompilationEvent>());
            _pooledEventsWithAnyActionsSet = new HashSet<CompilationEvent>();
        }

        private static ImmutableDictionary<DiagnosticAnalyzer, int> CreateAnalyzerStateMap(ImmutableArray<DiagnosticAnalyzer> analyzers, out ImmutableArray<PerAnalyzerState> analyzerStates)
        {
            ObjectPool<AnalyzerStateData> analyzerStateDataPool = new(() => new AnalyzerStateData());
            ObjectPool<DeclarationAnalyzerStateData> declarationAnalyzerStateDataPool = new(() => new DeclarationAnalyzerStateData());
            ObjectPool<Dictionary<int, DeclarationAnalyzerStateData>> currentlyAnalyzingDeclarationsMapPool = new(() => new Dictionary<int, DeclarationAnalyzerStateData>());
            ImmutableArray<PerAnalyzerState>.Builder builder = ImmutableArray.CreateBuilder<PerAnalyzerState>();
            ImmutableDictionary<DiagnosticAnalyzer, int>.Builder builder2 = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, int>();
            int num = 0;
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                builder.Add(new PerAnalyzerState(analyzerStateDataPool, declarationAnalyzerStateDataPool, currentlyAnalyzingDeclarationsMapPool));
                builder2[current] = num;
                num++;
            }
            analyzerStates = builder.ToImmutable();
            return builder2.ToImmutable();
        }

        private PerAnalyzerState GetAnalyzerState(DiagnosticAnalyzer analyzer)
        {
            int index = _analyzerStateMap[analyzer];
            return _analyzerStates[index];
        }

        public async Task OnCompilationEventsGeneratedAsync(Func<AsyncQueue<CompilationEvent>, ImmutableArray<AdditionalText>, ImmutableArray<CompilationEvent>> getCompilationEvents, AsyncQueue<CompilationEvent> eventQueue, ImmutableArray<AdditionalText> additionalFiles, AnalyzerDriver driver, CancellationToken cancellationToken)
        {
            try
            {
                await EnsureAnalyzerActionCountsInitializedAsync(driver, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                using (_gate.DisposableWait(cancellationToken))
                {
                    OnCompilationEventsGenerated_NoLock(getCompilationEvents(eventQueue, additionalFiles));
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private void OnCompilationEventsGenerated_NoLock(ImmutableArray<CompilationEvent> compilationEvents)
        {
            AddToEventsMap_NoLock(compilationEvents);
            ArrayBuilder<ISymbol>? arrayBuilder = null;
            ImmutableArray<CompilationEvent>.Enumerator enumerator2;
            foreach (KeyValuePair<DiagnosticAnalyzer, int> item in _analyzerStateMap)
            {
                DiagnosticAnalyzer key = item.Key;
                PerAnalyzerState perAnalyzerState = _analyzerStates[item.Value];
                AnalyzerActionCounts actionCounts = AnalyzerActionCountsMap[key];
                enumerator2 = compilationEvents.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    CompilationEvent current2 = enumerator2.Current;
                    if (!HasActionsForEvent(current2, actionCounts))
                    {
                        continue;
                    }
                    _pooledEventsWithAnyActionsSet.Add(current2);
                    if (current2 is SymbolDeclaredCompilationEvent symbolDeclaredCompilationEvent && symbolDeclaredCompilationEvent.DeclaringSyntaxReferences.Length > 1)
                    {
                        if (_partialSymbolsWithGeneratedSourceEvents.Contains(symbolDeclaredCompilationEvent.Symbol))
                        {
                            continue;
                        }
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<ISymbol>.GetInstance();
                        }
                        arrayBuilder.Add(symbolDeclaredCompilationEvent.Symbol);
                    }
                    perAnalyzerState.OnCompilationEventGenerated(current2, actionCounts);
                }
            }
            enumerator2 = compilationEvents.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                CompilationEvent current3 = enumerator2.Current;
                if (!_pooledEventsWithAnyActionsSet.Remove(current3))
                {
                    UpdateEventsMap_NoLock(current3, add: false);
                }
            }
            if (arrayBuilder != null)
            {
                _partialSymbolsWithGeneratedSourceEvents.AddAll(arrayBuilder);
                arrayBuilder.Free();
            }
        }

        private void AddToEventsMap_NoLock(ImmutableArray<CompilationEvent> compilationEvents)
        {
            ImmutableArray<CompilationEvent>.Enumerator enumerator = compilationEvents.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CompilationEvent current = enumerator.Current;
                UpdateEventsMap_NoLock(current, add: true);
            }
        }

        private void UpdateEventsMap_NoLock(CompilationEvent compilationEvent, bool add)
        {
            if (compilationEvent is not SymbolDeclaredCompilationEvent symbolDeclaredCompilationEvent)
            {
                if (compilationEvent is not CompilationUnitCompletedEvent compilationUnitCompletedEvent)
                {
                    if (!(compilationEvent is CompilationStartedEvent))
                    {
                        if (!(compilationEvent is CompilationCompletedEvent))
                        {
                            throw new InvalidOperationException("Unexpected compilation event of type " + compilationEvent.GetType().Name);
                        }
                        compilationStartedOrCompletedEventCommon(compilationEvent, add);
                        if (!add)
                        {
                            _semanticModelProvider.ClearCache(compilationEvent.Compilation);
                        }
                    }
                    else
                    {
                        compilationStartedOrCompletedEventCommon(compilationEvent, add);
                    }
                }
                else
                {
                    SyntaxTree compilationUnit = compilationUnitCompletedEvent.CompilationUnit;
                    if (add)
                    {
                        AddPendingSourceEvent_NoLock(compilationUnit, compilationEvent);
                    }
                    else
                    {
                        RemovePendingSourceEvent_NoLock(compilationUnit, compilationEvent);
                    }
                }
                return;
            }
            ImmutableArray<Location>.Enumerator enumerator = symbolDeclaredCompilationEvent.Symbol.Locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Location current = enumerator.Current;
                if (current.SourceTree != null)
                {
                    if (add)
                    {
                        AddPendingSourceEvent_NoLock(current.SourceTree, compilationEvent);
                    }
                    else
                    {
                        RemovePendingSourceEvent_NoLock(current.SourceTree, compilationEvent);
                    }
                }
            }
            void compilationStartedOrCompletedEventCommon(CompilationEvent compilationEvent, bool add)
            {
                if (add)
                {
                    _pendingNonSourceEvents.Add(compilationEvent);
                }
                else
                {
                    _pendingNonSourceEvents.Remove(compilationEvent);
                }
            }
        }

        private void AddPendingSourceEvent_NoLock(SyntaxTree tree, CompilationEvent compilationEvent)
        {
            if (!_pendingSourceEvents.TryGetValue(tree, out var value))
            {
                value = new HashSet<CompilationEvent>();
                _pendingSourceEvents[tree] = value;
                _semanticModelProvider.ClearCache(tree, compilationEvent.Compilation);
            }
            value.Add(compilationEvent);
        }

        private void RemovePendingSourceEvent_NoLock(SyntaxTree tree, CompilationEvent compilationEvent)
        {
            if (_pendingSourceEvents.TryGetValue(tree, out var value) && value.Remove(compilationEvent) && value.Count == 0)
            {
                _pendingSourceEvents.Remove(tree);
                _semanticModelProvider.ClearCache(tree, compilationEvent.Compilation);
            }
        }

        private async Task EnsureAnalyzerActionCountsInitializedAsync(AnalyzerDriver driver, CancellationToken cancellationToken)
        {
            if (_lazyAnalyzerActionCountsMap != null)
            {
                return;
            }
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerActionCounts>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, AnalyzerActionCounts>();
            foreach (var (analyzer, _) in _analyzerStateMap)
            {
                builder.Add(analyzer, await driver.GetAnalyzerActionCountsAsync(analyzer, _compilationOptions, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
            }
            Interlocked.CompareExchange(ref _lazyAnalyzerActionCountsMap, builder.ToImmutable(), null);
        }

        internal async Task<AnalyzerActionCounts> GetOrComputeAnalyzerActionCountsAsync(DiagnosticAnalyzer analyzer, AnalyzerDriver driver, CancellationToken cancellationToken)
        {
            await EnsureAnalyzerActionCountsInitializedAsync(driver, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return AnalyzerActionCountsMap[analyzer];
        }

        internal AnalyzerActionCounts GetAnalyzerActionCounts(DiagnosticAnalyzer analyzer)
        {
            return AnalyzerActionCountsMap[analyzer];
        }

        private static bool HasActionsForEvent(CompilationEvent compilationEvent, AnalyzerActionCounts actionCounts)
        {
            if (!(compilationEvent is CompilationStartedEvent))
            {
                if (!(compilationEvent is CompilationCompletedEvent))
                {
                    if (compilationEvent is SymbolDeclaredCompilationEvent)
                    {
                        return actionCounts.SymbolActionsCount > 0 || actionCounts.HasAnyExecutableCodeActions;
                    }
                    return actionCounts.SemanticModelActionsCount > 0;
                }
                return actionCounts.CompilationEndActionsCount > 0;
            }
            return actionCounts.CompilationActionsCount > 0 || actionCounts.SyntaxTreeActionsCount > 0 || actionCounts.AdditionalFileActionsCount > 0;
        }

        private async Task OnSymbolDeclaredEventProcessedAsync(SymbolDeclaredCompilationEvent symbolDeclaredEvent, ImmutableArray<DiagnosticAnalyzer> analyzers, Func<ISymbol, DiagnosticAnalyzer, Task> onSymbolAndMembersProcessedAsync)
        {
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (GetAnalyzerState(current).OnSymbolDeclaredEventProcessed(symbolDeclaredEvent))
                {
                    await onSymbolAndMembersProcessedAsync(symbolDeclaredEvent.Symbol, current).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
        }

        public async Task OnCompilationEventProcessedAsync(CompilationEvent compilationEvent, ImmutableArray<DiagnosticAnalyzer> analyzers, Func<ISymbol, DiagnosticAnalyzer, Task> onSymbolAndMembersProcessedAsync)
        {
            if (compilationEvent is SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                await OnSymbolDeclaredEventProcessedAsync(symbolDeclaredEvent, analyzers, onSymbolAndMembersProcessedAsync).ConfigureAwait(continueOnCapturedContext: false);
            }
            ImmutableArray<PerAnalyzerState>.Enumerator enumerator = _analyzerStates.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!enumerator.Current.IsEventAnalyzed(compilationEvent))
                {
                    return;
                }
            }
            using (_gate.DisposableWait())
            {
                UpdateEventsMap_NoLock(compilationEvent, add: false);
            }
        }

        public ImmutableArray<CompilationEvent> GetPendingEvents(ImmutableArray<DiagnosticAnalyzer> analyzers, SyntaxTree tree, CancellationToken cancellationToken)
        {
            using (_gate.DisposableWait(cancellationToken))
            {
                return GetPendingEvents_NoLock(analyzers, tree);
            }
        }

        private HashSet<CompilationEvent> GetPendingEvents_NoLock(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            HashSet<CompilationEvent> hashSet = _compilationEventsPool.Allocate();
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                GetAnalyzerState(current).AddPendingEvents(hashSet);
            }
            return hashSet;
        }

        private ImmutableArray<CompilationEvent> GetPendingEvents_NoLock(ImmutableArray<DiagnosticAnalyzer> analyzers, SyntaxTree tree)
        {
            if (_pendingSourceEvents.TryGetValue(tree, out var value) && value != null && value.Count > 0)
            {
                HashSet<CompilationEvent>? hashSet = null;
                try
                {
                    hashSet = GetPendingEvents_NoLock(analyzers);
                    if (hashSet.Count > 0)
                    {
                        hashSet.IntersectWith(value);
                        return hashSet.ToImmutableArray();
                    }
                }
                finally
                {
                    Free(hashSet);
                }
            }
            return ImmutableArray<CompilationEvent>.Empty;
        }

        public ImmutableArray<CompilationEvent> GetPendingEvents(ImmutableArray<DiagnosticAnalyzer> analyzers, bool includeSourceEvents, bool includeNonSourceEvents, CancellationToken cancellationToken)
        {
            using (_gate.DisposableWait(cancellationToken))
            {
                return GetPendingEvents_NoLock(analyzers, includeSourceEvents, includeNonSourceEvents);
            }
        }

        private ImmutableArray<CompilationEvent> GetPendingEvents_NoLock(ImmutableArray<DiagnosticAnalyzer> analyzers, bool includeSourceEvents, bool includeNonSourceEvents)
        {
            HashSet<CompilationEvent>? hashSet = null;
            HashSet<CompilationEvent>? hashSet2 = null;
            try
            {
                hashSet = GetPendingEvents_NoLock(analyzers);
                if (hashSet.Count == 0)
                {
                    return ImmutableArray<CompilationEvent>.Empty;
                }
                hashSet2 = _compilationEventsPool.Allocate();
                if (includeSourceEvents)
                {
                    foreach (HashSet<CompilationEvent> value in _pendingSourceEvents.Values)
                    {
                        foreach (CompilationEvent item in value)
                        {
                            hashSet2.Add(item);
                        }
                    }
                }
                if (includeNonSourceEvents)
                {
                    foreach (CompilationEvent pendingNonSourceEvent in _pendingNonSourceEvents)
                    {
                        hashSet2.Add(pendingNonSourceEvent);
                    }
                }
                hashSet2.IntersectWith(hashSet);
                return hashSet2.ToImmutableArray();
            }
            finally
            {
                Free(hashSet);
                Free(hashSet2);
            }
        }

        private void Free(HashSet<CompilationEvent>? events)
        {
            if (events != null)
            {
                events!.Clear();
                _compilationEventsPool.Free(events);
            }
        }

        public bool HasPendingSyntaxAnalysis(AnalysisScope analysisScope)
        {
            if (analysisScope.IsSingleFileAnalysis && !analysisScope.IsSyntacticSingleFileAnalysis)
            {
                return false;
            }
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analysisScope.Analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (GetAnalyzerState(current).HasPendingSyntaxAnalysis(analysisScope.FilterFileOpt))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasPendingSymbolAnalysis(AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            var symbolDeclaredEvents = GetPendingSymbolDeclaredEvents(analysisScope.FilterFileOpt!.Value.SourceTree!, cancellationToken);
            foreach (var symbolDeclaredEvent in symbolDeclaredEvents)
            {
                if (analysisScope.ShouldAnalyze(symbolDeclaredEvent.Symbol))
                {
                    foreach (var analyzer in analysisScope.Analyzers)
                    {
                        var analyzerState = GetAnalyzerState(analyzer);
                        if (analyzerState.HasPendingSymbolAnalysis(symbolDeclaredEvent.Symbol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private ImmutableArray<SymbolDeclaredCompilationEvent> GetPendingSymbolDeclaredEvents(SyntaxTree tree, CancellationToken cancellationToken)
        {
            using (_gate.DisposableWait(cancellationToken))
            {
                if (!_pendingSourceEvents.TryGetValue(tree, out var value))
                {
                    return ImmutableArray<SymbolDeclaredCompilationEvent>.Empty;
                }
                return value.OfType<SymbolDeclaredCompilationEvent>().ToImmutableArray();
            }
        }

        public bool TryStartProcessingEvent(CompilationEvent compilationEvent, DiagnosticAnalyzer analyzer, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
        {
            return GetAnalyzerState(analyzer).TryStartProcessingEvent(compilationEvent, out state);
        }

        public void MarkEventComplete(CompilationEvent compilationEvent, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerState(analyzer).MarkEventComplete(compilationEvent);
        }

        public void MarkEventComplete(CompilationEvent compilationEvent, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                GetAnalyzerState(analyzer).MarkEventComplete(compilationEvent);
            }
        }

        public void MarkEventCompleteForUnprocessedAnalyzers(CompilationEvent completedEvent, AnalysisScope analysisScope, HashSet<DiagnosticAnalyzer> processedAnalyzers)
        {
            MarkAnalysisCompleteForUnprocessedAnalyzers(analysisScope, processedAnalyzers, MarkEventComplete, completedEvent);
        }

        public bool IsEventComplete(CompilationEvent compilationEvent, DiagnosticAnalyzer analyzer)
        {
            return GetAnalyzerState(analyzer).IsEventAnalyzed(compilationEvent);
        }

        public bool TryStartAnalyzingSymbol(ISymbol symbol, DiagnosticAnalyzer analyzer, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
        {
            return GetAnalyzerState(analyzer).TryStartAnalyzingSymbol(symbol, out state);
        }

        public bool TryStartSymbolEndAnalysis(ISymbol symbol, DiagnosticAnalyzer analyzer, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
        {
            return GetAnalyzerState(analyzer).TryStartSymbolEndAnalysis(symbol, out state);
        }

        public void MarkSymbolComplete(ISymbol symbol, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerState(analyzer).MarkSymbolComplete(symbol);
        }

        public void MarkSymbolCompleteForUnprocessedAnalyzers(ISymbol symbol, AnalysisScope analysisScope, HashSet<DiagnosticAnalyzer> processedAnalyzers)
        {
            MarkAnalysisCompleteForUnprocessedAnalyzers(analysisScope, processedAnalyzers, MarkSymbolComplete, symbol);
        }

        public bool IsSymbolComplete(ISymbol symbol, DiagnosticAnalyzer analyzer)
        {
            return GetAnalyzerState(analyzer).IsSymbolComplete(symbol);
        }

        public void MarkSymbolEndAnalysisComplete(ISymbol symbol, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                MarkSymbolEndAnalysisComplete(symbol, analyzer);
            }
        }

        public void MarkSymbolEndAnalysisComplete(ISymbol symbol, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerState(analyzer).MarkSymbolEndAnalysisComplete(symbol);
        }

        public bool IsSymbolEndAnalysisComplete(ISymbol symbol, DiagnosticAnalyzer analyzer)
        {
            return GetAnalyzerState(analyzer).IsSymbolEndAnalysisComplete(symbol);
        }

        public bool TryStartAnalyzingDeclaration(ISymbol symbol, int declarationIndex, DiagnosticAnalyzer analyzer, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out DeclarationAnalyzerStateData? state)
        {
            return GetAnalyzerState(analyzer).TryStartAnalyzingDeclaration(symbol, declarationIndex, out state);
        }

        public bool IsDeclarationComplete(ISymbol symbol, int declarationIndex)
        {
            return IsDeclarationComplete(symbol, declarationIndex, _analyzerStates);
        }

        public bool IsDeclarationComplete(ISymbol symbol, int declarationIndex, DiagnosticAnalyzer analyzer)
        {
            PerAnalyzerState analyzerState = GetAnalyzerState(analyzer);
            return IsDeclarationComplete(symbol, declarationIndex, SpecializedCollections.SingletonEnumerable(analyzerState));
        }

        private static bool IsDeclarationComplete(ISymbol symbol, int declarationIndex, IEnumerable<PerAnalyzerState> analyzerStates)
        {
            foreach (PerAnalyzerState analyzerState in analyzerStates)
            {
                if (!analyzerState.IsDeclarationComplete(symbol, declarationIndex))
                {
                    return false;
                }
            }
            return true;
        }

        public void MarkDeclarationComplete(ISymbol symbol, int declarationIndex, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerState(analyzer).MarkDeclarationComplete(symbol, declarationIndex);
        }

        public void MarkDeclarationComplete(ISymbol symbol, int declarationIndex, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                GetAnalyzerState(analyzer).MarkDeclarationComplete(symbol, declarationIndex);
            }
        }

        public void MarkDeclarationsComplete(ISymbol symbol, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                GetAnalyzerState(analyzer).MarkDeclarationsComplete(symbol);
            }
        }

        public bool TryStartSyntaxAnalysis(SourceOrAdditionalFile file, DiagnosticAnalyzer analyzer, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerStateData? state)
        {
            return GetAnalyzerState(analyzer).TryStartSyntaxAnalysis(file, out state);
        }

        public void MarkSyntaxAnalysisComplete(SourceOrAdditionalFile file, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerState(analyzer).MarkSyntaxAnalysisComplete(file);
        }

        public void MarkSyntaxAnalysisComplete(SourceOrAdditionalFile file, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                GetAnalyzerState(analyzer).MarkSyntaxAnalysisComplete(file);
            }
        }

        public void MarkSyntaxAnalysisCompleteForUnprocessedAnalyzers(SourceOrAdditionalFile file, AnalysisScope analysisScope, HashSet<DiagnosticAnalyzer> processedAnalyzers)
        {
            MarkAnalysisCompleteForUnprocessedAnalyzers(analysisScope, processedAnalyzers, MarkSyntaxAnalysisComplete, file);
        }

        private static void MarkAnalysisCompleteForUnprocessedAnalyzers<T>(AnalysisScope analysisScope, HashSet<DiagnosticAnalyzer> processedAnalyzers, Action<T, DiagnosticAnalyzer> markComplete, T arg)
        {
            if (analysisScope.Analyzers.Length == processedAnalyzers.Count)
            {
                return;
            }
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analysisScope.Analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (!processedAnalyzers.Contains(current))
                {
                    markComplete(arg, current);
                }
            }
        }
    }
}
