using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalyzerExecutor
    {
        private sealed class AnalyzerDiagnosticReporter
        {
            public readonly Action<Diagnostic> AddDiagnosticAction;

            private static readonly ObjectPool<AnalyzerDiagnosticReporter> s_objectPool = new ObjectPool<AnalyzerDiagnosticReporter>(() => new AnalyzerDiagnosticReporter(), 10);

            private SourceOrAdditionalFile? _contextFile;

            private TextSpan? _span;

            private Compilation _compilation;

            private DiagnosticAnalyzer _analyzer;

            private bool _isSyntaxDiagnostic;

            private Action<Diagnostic>? _addNonCategorizedDiagnostic;

            private Action<Diagnostic, DiagnosticAnalyzer, bool>? _addCategorizedLocalDiagnostic;

            private Action<Diagnostic, DiagnosticAnalyzer>? _addCategorizedNonLocalDiagnostic;

            private Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> _shouldSuppressGeneratedCodeDiagnostic;

            private CancellationToken _cancellationToken;

            public static AnalyzerDiagnosticReporter GetInstance(SourceOrAdditionalFile contextFile, TextSpan? span, Compilation compilation, DiagnosticAnalyzer analyzer, bool isSyntaxDiagnostic, Action<Diagnostic>? addNonCategorizedDiagnostic, Action<Diagnostic, DiagnosticAnalyzer, bool>? addCategorizedLocalDiagnostic, Action<Diagnostic, DiagnosticAnalyzer>? addCategorizedNonLocalDiagnostic, Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> shouldSuppressGeneratedCodeDiagnostic, CancellationToken cancellationToken)
            {
                AnalyzerDiagnosticReporter analyzerDiagnosticReporter = s_objectPool.Allocate();
                analyzerDiagnosticReporter._contextFile = contextFile;
                analyzerDiagnosticReporter._span = span;
                analyzerDiagnosticReporter._compilation = compilation;
                analyzerDiagnosticReporter._analyzer = analyzer;
                analyzerDiagnosticReporter._isSyntaxDiagnostic = isSyntaxDiagnostic;
                analyzerDiagnosticReporter._addNonCategorizedDiagnostic = addNonCategorizedDiagnostic;
                analyzerDiagnosticReporter._addCategorizedLocalDiagnostic = addCategorizedLocalDiagnostic;
                analyzerDiagnosticReporter._addCategorizedNonLocalDiagnostic = addCategorizedNonLocalDiagnostic;
                analyzerDiagnosticReporter._shouldSuppressGeneratedCodeDiagnostic = shouldSuppressGeneratedCodeDiagnostic;
                analyzerDiagnosticReporter._cancellationToken = cancellationToken;
                return analyzerDiagnosticReporter;
            }

            public void Free()
            {
                _contextFile = null;
                _span = null;
                _compilation = null;
                _analyzer = null;
                _isSyntaxDiagnostic = false;
                _addNonCategorizedDiagnostic = null;
                _addCategorizedLocalDiagnostic = null;
                _addCategorizedNonLocalDiagnostic = null;
                _shouldSuppressGeneratedCodeDiagnostic = null;
                _cancellationToken = default(CancellationToken);
                s_objectPool.Free(this);
            }

            private AnalyzerDiagnosticReporter()
            {
                AddDiagnosticAction = AddDiagnostic;
            }

            private void AddDiagnostic(Diagnostic diagnostic)
            {
                if (!_shouldSuppressGeneratedCodeDiagnostic(diagnostic, _analyzer, _compilation, _cancellationToken))
                {
                    if (_addCategorizedLocalDiagnostic == null)
                    {
                        _addNonCategorizedDiagnostic!(diagnostic);
                    }
                    else if (isLocalDiagnostic(diagnostic) && (!_span.HasValue || _span.Value.IntersectsWith(diagnostic.Location.SourceSpan)))
                    {
                        _addCategorizedLocalDiagnostic!(diagnostic, _analyzer, _isSyntaxDiagnostic);
                    }
                    else
                    {
                        _addCategorizedNonLocalDiagnostic!(diagnostic, _analyzer);
                    }
                }
                bool isLocalDiagnostic(Diagnostic diagnostic)
                {
                    if (diagnostic.Location.IsInSource)
                    {
                        if (_contextFile?.SourceTree != null)
                        {
                            return _contextFile.Value.SourceTree == diagnostic.Location.SourceTree;
                        }
                        return false;
                    }
                    if (_contextFile?.AdditionalFile != null && diagnostic.Location is ExternalFileLocation externalFileLocation)
                    {
                        return PathUtilities.Comparer.Equals(_contextFile.Value.AdditionalFile!.Path, externalFileLocation.FilePath);
                    }
                    return false;
                }
            }
        }

        private const string DiagnosticCategory = "Compiler";

        internal const string AnalyzerExceptionDiagnosticId = "AD0001";

        internal const string AnalyzerDriverExceptionDiagnosticId = "AD0002";

        private readonly Compilation? _compilation;

        private readonly AnalyzerOptions? _analyzerOptions;

        private readonly Action<Diagnostic>? _addNonCategorizedDiagnostic;

        private readonly Action<Diagnostic, DiagnosticAnalyzer, bool>? _addCategorizedLocalDiagnostic;

        private readonly Action<Diagnostic, DiagnosticAnalyzer>? _addCategorizedNonLocalDiagnostic;

        private readonly Action<Suppression>? _addSuppression;

        private readonly Action<Exception, DiagnosticAnalyzer, Diagnostic> _onAnalyzerException;

        private readonly Func<Exception, bool>? _analyzerExceptionFilter;

        private readonly AnalyzerManager _analyzerManager;

        private readonly Func<DiagnosticAnalyzer, bool>? _isCompilerAnalyzer;

        private readonly Func<DiagnosticAnalyzer, object?>? _getAnalyzerGate;

        private readonly Func<SyntaxTree, SemanticModel>? _getSemanticModel;

        private readonly Func<DiagnosticAnalyzer, bool> _shouldSkipAnalysisOnGeneratedCode;

        private readonly Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> _shouldSuppressGeneratedCodeDiagnostic;

        private readonly Func<SyntaxTree, TextSpan, bool> _isGeneratedCodeLocation;

        private readonly Func<DiagnosticAnalyzer, SyntaxTree, SyntaxTreeOptionsProvider?, bool>? _isAnalyzerSuppressedForTree;

        private readonly ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<long>>? _analyzerExecutionTimeMap;

        private readonly CompilationAnalysisValueProviderFactory _compilationAnalysisValueProviderFactory;

        private readonly CancellationToken _cancellationToken;

        private readonly Func<IOperation, ControlFlowGraph>? _lazyGetControlFlowGraph;

        private ConcurrentDictionary<IOperation, ControlFlowGraph>? _lazyControlFlowGraphMap;

        private Func<IOperation, ControlFlowGraph> GetControlFlowGraph => GetControlFlowGraphImpl;

        internal Compilation Compilation => _compilation;

        internal AnalyzerOptions AnalyzerOptions => _analyzerOptions;

        internal CancellationToken CancellationToken => _cancellationToken;

        internal Action<Exception, DiagnosticAnalyzer, Diagnostic> OnAnalyzerException => _onAnalyzerException;

        internal ImmutableDictionary<DiagnosticAnalyzer, TimeSpan> AnalyzerExecutionTimes => _analyzerExecutionTimeMap.ToImmutableDictionary<KeyValuePair<DiagnosticAnalyzer, StrongBox<long>>, DiagnosticAnalyzer, TimeSpan>((KeyValuePair<DiagnosticAnalyzer, StrongBox<long>> pair) => pair.Key, (KeyValuePair<DiagnosticAnalyzer, StrongBox<long>> pair) => TimeSpan.FromTicks(pair.Value.Value));

        private bool IsAnalyzerSuppressedForTree(DiagnosticAnalyzer analyzer, SyntaxTree tree)
        {
            return _isAnalyzerSuppressedForTree!(analyzer, tree, Compilation.Options.SyntaxTreeOptionsProvider);
        }

        public static AnalyzerExecutor Create(Compilation compilation, AnalyzerOptions analyzerOptions, Action<Diagnostic>? addNonCategorizedDiagnostic, Action<Exception, DiagnosticAnalyzer, Diagnostic> onAnalyzerException, Func<Exception, bool>? analyzerExceptionFilter, Func<DiagnosticAnalyzer, bool> isCompilerAnalyzer, AnalyzerManager analyzerManager, Func<DiagnosticAnalyzer, bool> shouldSkipAnalysisOnGeneratedCode, Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> shouldSuppressGeneratedCodeDiagnostic, Func<SyntaxTree, TextSpan, bool> isGeneratedCodeLocation, Func<DiagnosticAnalyzer, SyntaxTree, SyntaxTreeOptionsProvider?, bool> isAnalyzerSuppressedForTree, Func<DiagnosticAnalyzer, object?> getAnalyzerGate, Func<SyntaxTree, SemanticModel> getSemanticModel, bool logExecutionTime = false, Action<Diagnostic, DiagnosticAnalyzer, bool>? addCategorizedLocalDiagnostic = null, Action<Diagnostic, DiagnosticAnalyzer>? addCategorizedNonLocalDiagnostic = null, Action<Suppression>? addSuppression = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<long>> analyzerExecutionTimeMap = (logExecutionTime ? new ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<long>>() : null);
            return new AnalyzerExecutor(compilation, analyzerOptions, addNonCategorizedDiagnostic, onAnalyzerException, analyzerExceptionFilter, isCompilerAnalyzer, analyzerManager, shouldSkipAnalysisOnGeneratedCode, shouldSuppressGeneratedCodeDiagnostic, isGeneratedCodeLocation, isAnalyzerSuppressedForTree, getAnalyzerGate, getSemanticModel, analyzerExecutionTimeMap, addCategorizedLocalDiagnostic, addCategorizedNonLocalDiagnostic, addSuppression, cancellationToken);
        }

        public static AnalyzerExecutor CreateForSupportedDiagnostics(Action<Exception, DiagnosticAnalyzer, Diagnostic>? onAnalyzerException, AnalyzerManager analyzerManager, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (onAnalyzerException == null)
            {
                onAnalyzerException = delegate
                {
                };
            }
            return new AnalyzerExecutor(null, null, null, onAnalyzerException, null, null, analyzerManager, (DiagnosticAnalyzer _) => false, (Diagnostic diagnostic, DiagnosticAnalyzer analyzer, Compilation compilation, CancellationToken ct) => false, (SyntaxTree _1, TextSpan _2) => false, null, null, null, null, null, null, null, cancellationToken);
        }

        private AnalyzerExecutor(Compilation? compilation, AnalyzerOptions? analyzerOptions, Action<Diagnostic>? addNonCategorizedDiagnosticOpt, Action<Exception, DiagnosticAnalyzer, Diagnostic> onAnalyzerException, Func<Exception, bool>? analyzerExceptionFilter, Func<DiagnosticAnalyzer, bool>? isCompilerAnalyzer, AnalyzerManager analyzerManager, Func<DiagnosticAnalyzer, bool> shouldSkipAnalysisOnGeneratedCode, Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> shouldSuppressGeneratedCodeDiagnostic, Func<SyntaxTree, TextSpan, bool> isGeneratedCodeLocation, Func<DiagnosticAnalyzer, SyntaxTree, SyntaxTreeOptionsProvider?, bool>? isAnalyzerSuppressedForTree, Func<DiagnosticAnalyzer, object?>? getAnalyzerGate, Func<SyntaxTree, SemanticModel>? getSemanticModel, ConcurrentDictionary<DiagnosticAnalyzer, StrongBox<long>>? analyzerExecutionTimeMap, Action<Diagnostic, DiagnosticAnalyzer, bool>? addCategorizedLocalDiagnostic, Action<Diagnostic, DiagnosticAnalyzer>? addCategorizedNonLocalDiagnostic, Action<Suppression>? addSuppression, CancellationToken cancellationToken)
        {
            _compilation = compilation;
            _analyzerOptions = analyzerOptions;
            _addNonCategorizedDiagnostic = addNonCategorizedDiagnosticOpt;
            _onAnalyzerException = onAnalyzerException;
            _analyzerExceptionFilter = analyzerExceptionFilter;
            _isCompilerAnalyzer = isCompilerAnalyzer;
            _analyzerManager = analyzerManager;
            _shouldSkipAnalysisOnGeneratedCode = shouldSkipAnalysisOnGeneratedCode;
            _shouldSuppressGeneratedCodeDiagnostic = shouldSuppressGeneratedCodeDiagnostic;
            _isGeneratedCodeLocation = isGeneratedCodeLocation;
            _isAnalyzerSuppressedForTree = isAnalyzerSuppressedForTree;
            _getAnalyzerGate = getAnalyzerGate;
            _getSemanticModel = getSemanticModel;
            _analyzerExecutionTimeMap = analyzerExecutionTimeMap;
            _addCategorizedLocalDiagnostic = addCategorizedLocalDiagnostic;
            _addCategorizedNonLocalDiagnostic = addCategorizedNonLocalDiagnostic;
            _addSuppression = addSuppression;
            _cancellationToken = cancellationToken;
            _compilationAnalysisValueProviderFactory = new CompilationAnalysisValueProviderFactory();
        }

        public AnalyzerExecutor WithCancellationToken(CancellationToken cancellationToken)
        {
            if (cancellationToken == _cancellationToken)
            {
                return this;
            }
            return new AnalyzerExecutor(_compilation, _analyzerOptions, _addNonCategorizedDiagnostic, _onAnalyzerException, _analyzerExceptionFilter, _isCompilerAnalyzer, _analyzerManager, _shouldSkipAnalysisOnGeneratedCode, _shouldSuppressGeneratedCodeDiagnostic, _isGeneratedCodeLocation, _isAnalyzerSuppressedForTree, _getAnalyzerGate, _getSemanticModel, _analyzerExecutionTimeMap, _addCategorizedLocalDiagnostic, _addCategorizedNonLocalDiagnostic, _addSuppression, cancellationToken);
        }

        internal bool TryGetCompilationAndAnalyzerOptions([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Compilation? compilation, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AnalyzerOptions? analyzerOptions)
        {
            Compilation compilation2 = _compilation;
            AnalyzerOptions analyzerOptions2 = _analyzerOptions;
            compilation = compilation2;
            analyzerOptions = analyzerOptions2;
            if (compilation != null)
            {
                return analyzerOptions != null;
            }
            return false;
        }

        public void ExecuteInitializeMethod(DiagnosticAnalyzer analyzer, HostSessionStartAnalysisScope sessionScope)
        {
            AnalyzerAnalysisContext item = new AnalyzerAnalysisContext(analyzer, sessionScope);
            ExecuteAndCatchIfThrows(analyzer, delegate ((DiagnosticAnalyzer analyzer, AnalyzerAnalysisContext context) data)
            {
                data.analyzer.Initialize(data.context);
            }, (analyzer, item));
        }

        public void ExecuteCompilationStartActions(ImmutableArray<CompilationStartAnalyzerAction> actions, HostCompilationStartAnalysisScope compilationScope)
        {
            ImmutableArray<CompilationStartAnalyzerAction>.Enumerator enumerator = actions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CompilationStartAnalyzerAction current = enumerator.Current;
                CancellationToken cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                AnalyzerCompilationStartAnalysisContext item = new AnalyzerCompilationStartAnalysisContext(current.Analyzer, compilationScope, Compilation, AnalyzerOptions, _compilationAnalysisValueProviderFactory, _cancellationToken);
                ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<CompilationStartAnalysisContext> action, AnalyzerCompilationStartAnalysisContext context) data)
                {
                    data.action(data.context);
                }, (current.Action, item), new AnalysisContextInfo(Compilation));
            }
        }

        public void ExecuteSymbolStartActions(ISymbol symbol, DiagnosticAnalyzer analyzer, ImmutableArray<SymbolStartAnalyzerAction> actions, HostSymbolStartAnalysisScope symbolScope)
        {
            if (IsAnalyzerSuppressedForSymbol(analyzer, symbol))
            {
                return;
            }
            ImmutableArray<SymbolStartAnalyzerAction>.Enumerator enumerator = actions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SymbolStartAnalyzerAction current = enumerator.Current;
                CancellationToken cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                AnalyzerSymbolStartAnalysisContext item = new AnalyzerSymbolStartAnalysisContext(current.Analyzer, symbolScope, symbol, Compilation, AnalyzerOptions, _cancellationToken);
                ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<SymbolStartAnalysisContext> action, AnalyzerSymbolStartAnalysisContext context) data)
                {
                    data.action(data.context);
                }, (current.Action, item), new AnalysisContextInfo(Compilation, symbol));
            }
        }

        public void ExecuteSuppressionAction(DiagnosticSuppressor suppressor, ImmutableArray<Diagnostic> reportedDiagnostics)
        {
            if (!reportedDiagnostics.IsEmpty)
            {
                CancellationToken cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                Func<SuppressionDescriptor, bool> isSupportedSuppressionDescriptor = _analyzerManager.GetSupportedSuppressionDescriptors(suppressor, this).Contains;
                Action<SuppressionAnalysisContext> item = suppressor.ReportSuppressions;
                ExecuteAndCatchIfThrows(argument: (item, new SuppressionAnalysisContext(Compilation, AnalyzerOptions, reportedDiagnostics, _addSuppression, isSupportedSuppressionDescriptor, _getSemanticModel, _cancellationToken)), analyzer: suppressor, analyze: delegate ((Action<SuppressionAnalysisContext> action, SuppressionAnalysisContext context) data)
                {
                    data.action(data.context);
                }, info: new AnalysisContextInfo(Compilation));
            }
        }

        public bool TryExecuteCompilationActions(ImmutableArray<CompilationAnalyzerAction> compilationActions, DiagnosticAnalyzer analyzer, CompilationEvent compilationEvent, AnalysisScope analysisScope, AnalysisState? analysisState)
        {
            AnalysisState.AnalyzerStateData analyzerState = null;
            try
            {
                if (TryStartProcessingEvent(compilationEvent, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteCompilationActionsCore(compilationActions, analyzer, analyzerState);
                    analysisState?.MarkEventComplete(compilationEvent, analyzer);
                    return true;
                }
                return IsEventComplete(compilationEvent, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteCompilationActionsCore(ImmutableArray<CompilationAnalyzerAction> compilationActions, DiagnosticAnalyzer analyzer, AnalysisState.AnalyzerStateData? analyzerState)
        {
            Action<Diagnostic> addCompilationDiagnostic = GetAddCompilationDiagnostic(analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<CompilationAnalyzerAction>.Enumerator enumerator = compilationActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    CompilationAnalyzerAction current = enumerator.Current;
                    CancellationToken cancellationToken = _cancellationToken;
                    cancellationToken.ThrowIfCancellationRequested();
                    if (ShouldExecuteAction(analyzerState, current))
                    {
                        CompilationAnalysisContext item = new CompilationAnalysisContext(Compilation, AnalyzerOptions, addCompilationDiagnostic, boundFunction, _compilationAnalysisValueProviderFactory, _cancellationToken);
                        ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<CompilationAnalysisContext> action, CompilationAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, (current.Action, item), new AnalysisContextInfo(Compilation));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
            }
        }

        public bool TryExecuteSymbolActions(ImmutableArray<SymbolAnalyzerAction> symbolActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCodeSymbol)
        {
            AnalysisState.AnalyzerStateData analyzerState = null;
            try
            {
                ISymbol symbol = symbolDeclaredEvent.Symbol;
                if (TryStartAnalyzingSymbol(symbol, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteSymbolActionsCore(symbolActions, analyzer, symbolDeclaredEvent, getTopMostNodeForAnalysis, analyzerState, isGeneratedCodeSymbol);
                    analysisState?.MarkSymbolComplete(symbol, analyzer);
                    return true;
                }
                return IsSymbolComplete(symbol, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteSymbolActionsCore(ImmutableArray<SymbolAnalyzerAction> symbolActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisState.AnalyzerStateData? analyzerState, bool isGeneratedCodeSymbol)
        {
            if ((isGeneratedCodeSymbol && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForSymbol(analyzer, symbolDeclaredEvent.Symbol))
            {
                return;
            }
            ISymbol symbol = symbolDeclaredEvent.Symbol;
            Action<Diagnostic> addDiagnostic = GetAddDiagnostic(symbol, symbolDeclaredEvent.DeclaringSyntaxReferences, analyzer, getTopMostNodeForAnalysis);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<SymbolAnalyzerAction>.Enumerator enumerator = symbolActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SymbolAnalyzerAction current = enumerator.Current;
                    Action<SymbolAnalysisContext> action = current.Action;
                    if (current.Kinds.Contains(symbol.Kind) && ShouldExecuteAction(analyzerState, current))
                    {
                        CancellationToken cancellationToken = _cancellationToken;
                        cancellationToken.ThrowIfCancellationRequested();
                        ExecuteAndCatchIfThrows(argument: (action, new SymbolAnalysisContext(symbol, Compilation, AnalyzerOptions, addDiagnostic, boundFunction, _cancellationToken)), analyzer: current.Analyzer, analyze: delegate ((Action<SymbolAnalysisContext> action, SymbolAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, info: new AnalysisContextInfo(Compilation, symbol));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
            }
        }

        public bool TryExecuteSymbolEndActionsForContainer(INamespaceOrTypeSymbol containingSymbol, ISymbol processedMemberSymbol, DiagnosticAnalyzer analyzer, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisState? analysisState, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out SymbolDeclaredCompilationEvent? containingSymbolDeclaredEvent)
        {
            containingSymbolDeclaredEvent = null;
            if (!_analyzerManager.TryProcessCompletedMemberAndGetPendingSymbolEndActionsForContainer(containingSymbol, processedMemberSymbol, analyzer, out var containerEndActionsAndEvent))
            {
                return false;
            }
            ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions;
            (symbolEndActions, containingSymbolDeclaredEvent) = containerEndActionsAndEvent;
            return TryExecuteSymbolEndActionsCore(symbolEndActions, analyzer, containingSymbolDeclaredEvent, getTopMostNodeForAnalysis, analysisState);
        }

        public bool TryExecuteSymbolEndActions(ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisState? analysisState)
        {
            if (_analyzerManager.TryStartExecuteSymbolEndActions(symbolEndActions, analyzer, symbolDeclaredEvent))
            {
                return TryExecuteSymbolEndActionsCore(symbolEndActions, analyzer, symbolDeclaredEvent, getTopMostNodeForAnalysis, analysisState);
            }
            return false;
        }

        private bool TryExecuteSymbolEndActionsCore(ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisState? analysisState)
        {
            ISymbol symbol = symbolDeclaredEvent.Symbol;
            AnalysisState.AnalyzerStateData analyzerState = null;
            try
            {
                if (TryStartSymbolEndAnalysis(symbol, analyzer, analysisState, out analyzerState))
                {
                    ExecuteSymbolEndActionsCore(symbolEndActions, analyzer, symbolDeclaredEvent, getTopMostNodeForAnalysis, analyzerState);
                    MarkSymbolEndAnalysisComplete(symbol, analyzer, analysisState);
                    return true;
                }
                if (!IsSymbolEndAnalysisComplete(symbol, analyzer, analysisState))
                {
                    _analyzerManager.MarkSymbolEndAnalysisPending(symbol, analyzer, symbolEndActions, symbolDeclaredEvent);
                    return false;
                }
                return true;
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        public void MarkSymbolEndAnalysisComplete(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState)
        {
            analysisState?.MarkSymbolEndAnalysisComplete(symbol, analyzer);
            _analyzerManager.MarkSymbolEndAnalysisComplete(symbol, analyzer);
        }

        private void ExecuteSymbolEndActionsCore(ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, AnalysisState.AnalyzerStateData? analyzerState)
        {
            ISymbol symbol = symbolDeclaredEvent.Symbol;
            Action<Diagnostic> addDiagnostic = GetAddDiagnostic(symbol, symbolDeclaredEvent.DeclaringSyntaxReferences, analyzer, getTopMostNodeForAnalysis);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<SymbolEndAnalyzerAction>.Enumerator enumerator = symbolEndActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SymbolEndAnalyzerAction current = enumerator.Current;
                    Action<SymbolAnalysisContext> action = current.Action;
                    if (ShouldExecuteAction(analyzerState, current))
                    {
                        CancellationToken cancellationToken = _cancellationToken;
                        cancellationToken.ThrowIfCancellationRequested();
                        ExecuteAndCatchIfThrows(argument: (action, new SymbolAnalysisContext(symbol, Compilation, AnalyzerOptions, addDiagnostic, boundFunction, _cancellationToken)), analyzer: current.Analyzer, analyze: delegate ((Action<SymbolAnalysisContext> action, SymbolAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, info: new AnalysisContextInfo(Compilation, symbol));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
            }
        }

        public bool TryExecuteSemanticModelActions(ImmutableArray<SemanticModelAnalyzerAction> semanticModelActions, DiagnosticAnalyzer analyzer, SemanticModel semanticModel, CompilationEvent compilationUnitCompletedEvent, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode)
        {
            AnalysisState.AnalyzerStateData analyzerState = null;
            try
            {
                if (TryStartProcessingEvent(compilationUnitCompletedEvent, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteSemanticModelActionsCore(semanticModelActions, analyzer, semanticModel, analyzerState, isGeneratedCode);
                    analysisState?.MarkEventComplete(compilationUnitCompletedEvent, analyzer);
                    return true;
                }
                return IsEventComplete(compilationUnitCompletedEvent, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteSemanticModelActionsCore(ImmutableArray<SemanticModelAnalyzerAction> semanticModelActions, DiagnosticAnalyzer analyzer, SemanticModel semanticModel, AnalysisState.AnalyzerStateData? analyzerState, bool isGeneratedCode)
        {
            if ((isGeneratedCode && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForTree(analyzer, semanticModel.SyntaxTree))
            {
                return;
            }
            AnalyzerDiagnosticReporter addSemanticDiagnostic = GetAddSemanticDiagnostic(semanticModel.SyntaxTree, analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<SemanticModelAnalyzerAction>.Enumerator enumerator = semanticModelActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SemanticModelAnalyzerAction current = enumerator.Current;
                    if (ShouldExecuteAction(analyzerState, current))
                    {
                        CancellationToken cancellationToken = _cancellationToken;
                        cancellationToken.ThrowIfCancellationRequested();
                        SemanticModelAnalysisContext item = new SemanticModelAnalysisContext(semanticModel, AnalyzerOptions, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, _cancellationToken);
                        ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<SemanticModelAnalysisContext> action, SemanticModelAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, (current.Action, item), new AnalysisContextInfo(semanticModel));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
                addSemanticDiagnostic.Free();
            }
        }

        public bool TryExecuteSyntaxTreeActions(ImmutableArray<SyntaxTreeAnalyzerAction> syntaxTreeActions, DiagnosticAnalyzer analyzer, SourceOrAdditionalFile file, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode)
        {
            AnalysisState.AnalyzerStateData? analyzerState = null;
            try
            {
                if (TryStartSyntaxAnalysis(file, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteSyntaxTreeActionsCore(syntaxTreeActions, analyzer, file, analyzerState, isGeneratedCode);
                    analysisState?.MarkSyntaxAnalysisComplete(file, analyzer);
                    return true;
                }
                return analysisState == null || !analysisState!.HasPendingSyntaxAnalysis(analysisScope);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteSyntaxTreeActionsCore(ImmutableArray<SyntaxTreeAnalyzerAction> syntaxTreeActions, DiagnosticAnalyzer analyzer, SourceOrAdditionalFile file, AnalysisState.AnalyzerStateData? analyzerState, bool isGeneratedCode)
        {
            SyntaxTree sourceTree = file.SourceTree;
            if ((isGeneratedCode && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForTree(analyzer, sourceTree))
            {
                return;
            }
            AnalyzerDiagnosticReporter addSyntaxDiagnostic = GetAddSyntaxDiagnostic(file, analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<SyntaxTreeAnalyzerAction>.Enumerator enumerator = syntaxTreeActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTreeAnalyzerAction current = enumerator.Current;
                    if (ShouldExecuteAction(analyzerState, current))
                    {
                        CancellationToken cancellationToken = _cancellationToken;
                        cancellationToken.ThrowIfCancellationRequested();
                        SyntaxTreeAnalysisContext item = new SyntaxTreeAnalysisContext(sourceTree, AnalyzerOptions, addSyntaxDiagnostic.AddDiagnosticAction, boundFunction, Compilation, _cancellationToken);
                        ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<SyntaxTreeAnalysisContext> action, SyntaxTreeAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, (current.Action, item), new AnalysisContextInfo(Compilation, file));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
                addSyntaxDiagnostic.Free();
            }
        }

        public bool TryExecuteAdditionalFileActions(ImmutableArray<AdditionalFileAnalyzerAction> additionalFileActions, DiagnosticAnalyzer analyzer, SourceOrAdditionalFile file, AnalysisScope analysisScope, AnalysisState? analysisState)
        {
            AnalysisState.AnalyzerStateData? analyzerState = null;
            try
            {
                if (TryStartSyntaxAnalysis(file, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteAdditionalFileActionsCore(additionalFileActions, analyzer, file, analyzerState);
                    analysisState?.MarkSyntaxAnalysisComplete(file, analyzer);
                    return true;
                }
                return analysisState == null || !analysisState!.HasPendingSyntaxAnalysis(analysisScope);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteAdditionalFileActionsCore(ImmutableArray<AdditionalFileAnalyzerAction> additionalFileActions, DiagnosticAnalyzer analyzer, SourceOrAdditionalFile file, AnalysisState.AnalyzerStateData? analyzerState)
        {
            AdditionalText? additionalFile = file.AdditionalFile;
            AnalyzerDiagnosticReporter addSyntaxDiagnostic = GetAddSyntaxDiagnostic(file, analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ImmutableArray<AdditionalFileAnalyzerAction>.Enumerator enumerator = additionalFileActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AdditionalFileAnalyzerAction current = enumerator.Current;
                    if (ShouldExecuteAction(analyzerState, current))
                    {
                        CancellationToken cancellationToken = _cancellationToken;
                        cancellationToken.ThrowIfCancellationRequested();
                        AdditionalFileAnalysisContext item = new AdditionalFileAnalysisContext(additionalFile, AnalyzerOptions, addSyntaxDiagnostic.AddDiagnosticAction, boundFunction, Compilation, _cancellationToken);
                        ExecuteAndCatchIfThrows(current.Analyzer, delegate ((Action<AdditionalFileAnalysisContext> action, AdditionalFileAnalysisContext context) data)
                        {
                            data.action(data.context);
                        }, (current.Action, item), new AnalysisContextInfo(Compilation, file));
                        analyzerState?.ProcessedActions.Add(current);
                    }
                }
                addSyntaxDiagnostic.Free();
            }
        }

        private void ExecuteSyntaxNodeAction<TLanguageKindEnum>(SyntaxNodeAnalyzerAction<TLanguageKindEnum> syntaxNodeAction, SyntaxNode node, ISymbol containingSymbol, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState) where TLanguageKindEnum : struct
        {
            if (ShouldExecuteAction(analyzerState, syntaxNodeAction))
            {
                SyntaxNodeAnalysisContext item = new SyntaxNodeAnalysisContext(node, containingSymbol, semanticModel, AnalyzerOptions, addDiagnostic, isSupportedDiagnostic, _cancellationToken);
                ExecuteAndCatchIfThrows(syntaxNodeAction.Analyzer, delegate ((Action<SyntaxNodeAnalysisContext> action, SyntaxNodeAnalysisContext context) data)
                {
                    data.action(data.context);
                }, (syntaxNodeAction.Action, item), new AnalysisContextInfo(Compilation, node));
                analyzerState?.ProcessedActions.Add(syntaxNodeAction);
            }
        }

        private void ExecuteOperationAction(OperationAnalyzerAction operationAction, IOperation operation, ISymbol containingSymbol, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.OperationAnalyzerStateData? analyzerState)
        {
            if (ShouldExecuteAction(analyzerState, operationAction))
            {
                OperationAnalysisContext item = new OperationAnalysisContext(operation, containingSymbol, semanticModel.Compilation, AnalyzerOptions, addDiagnostic, isSupportedDiagnostic, GetControlFlowGraph, _cancellationToken);
                ExecuteAndCatchIfThrows(operationAction.Analyzer, delegate ((Action<OperationAnalysisContext> action, OperationAnalysisContext context) data)
                {
                    data.action(data.context);
                }, (operationAction.Action, item), new AnalysisContextInfo(Compilation, operation));
                analyzerState?.ProcessedActions.Add(operationAction);
            }
        }

        public bool TryExecuteCodeBlockActions<TLanguageKindEnum>(IEnumerable<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> codeBlockStartActions, IEnumerable<CodeBlockAnalyzerAction> codeBlockActions, IEnumerable<CodeBlockAnalyzerAction> codeBlockEndActions, DiagnosticAnalyzer analyzer, SyntaxNode declaredNode, ISymbol declaredSymbol, ImmutableArray<SyntaxNode> executableCodeBlocks, SemanticModel semanticModel, Func<SyntaxNode, TLanguageKindEnum> getKind, int declarationIndex, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode) where TLanguageKindEnum : struct
        {
            SemanticModel semanticModel2 = semanticModel;
            ISymbol declaredSymbol2 = declaredSymbol;
            AnalysisState.DeclarationAnalyzerStateData? analyzerState = null;
            try
            {
                if (TryStartAnalyzingDeclaration(declaredSymbol2, declarationIndex, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteBlockActionsCore<CodeBlockStartAnalyzerAction<TLanguageKindEnum>, CodeBlockAnalyzerAction, SyntaxNodeAnalyzerAction<TLanguageKindEnum>, AnalysisState.SyntaxNodeAnalyzerStateData, SyntaxNode, TLanguageKindEnum>(codeBlockStartActions, codeBlockActions, codeBlockEndActions, analyzer, declaredNode, declaredSymbol2, executableCodeBlocks, (ImmutableArray<SyntaxNode> codeBlocks) => codeBlocks.SelectMany(delegate (SyntaxNode cb)
                    {
                        Func<SyntaxNode, bool>? syntaxNodesToAnalyzeFilter = semanticModel2.GetSyntaxNodesToAnalyzeFilter(cb, declaredSymbol2);
                        return (syntaxNodesToAnalyzeFilter != null) ? cb.DescendantNodesAndSelf(syntaxNodesToAnalyzeFilter).Where<SyntaxNode>(syntaxNodesToAnalyzeFilter) : cb.DescendantNodesAndSelf();
                    }), semanticModel2, getKind, analyzerState?.CodeBlockAnalysisState, isGeneratedCode);
                    return true;
                }
                return IsDeclarationComplete(declaredSymbol2, declarationIndex, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        public bool TryExecuteOperationBlockActions(IEnumerable<OperationBlockStartAnalyzerAction> operationBlockStartActions, IEnumerable<OperationBlockAnalyzerAction> operationBlockActions, IEnumerable<OperationBlockAnalyzerAction> operationBlockEndActions, DiagnosticAnalyzer analyzer, SyntaxNode declaredNode, ISymbol declaredSymbol, ImmutableArray<IOperation> operationBlocks, ImmutableArray<IOperation> operations, SemanticModel semanticModel, int declarationIndex, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode)
        {
            AnalysisState.DeclarationAnalyzerStateData analyzerState = null;
            try
            {
                if (TryStartAnalyzingDeclaration(declaredSymbol, declarationIndex, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteBlockActionsCore<OperationBlockStartAnalyzerAction, OperationBlockAnalyzerAction, OperationAnalyzerAction, AnalysisState.OperationAnalyzerStateData, IOperation, int>(operationBlockStartActions, operationBlockActions, operationBlockEndActions, analyzer, declaredNode, declaredSymbol, operationBlocks, (ImmutableArray<IOperation> blocks) => operations, semanticModel, null, analyzerState?.OperationBlockAnalysisState, isGeneratedCode);
                    return true;
                }
                return IsDeclarationComplete(declaredSymbol, declarationIndex, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteBlockActionsCore<TBlockStartAction, TBlockAction, TNodeAction, TNodeStateData, TNode, TLanguageKindEnum>(IEnumerable<TBlockStartAction> startActions, IEnumerable<TBlockAction> actions, IEnumerable<TBlockAction> endActions, DiagnosticAnalyzer analyzer, SyntaxNode declaredNode, ISymbol declaredSymbol, ImmutableArray<TNode> executableBlocks, Func<ImmutableArray<TNode>, IEnumerable<TNode>> getNodesToAnalyze, SemanticModel semanticModel, Func<SyntaxNode, TLanguageKindEnum>? getKind, AnalysisState.BlockAnalyzerStateData<TBlockAction, TNodeStateData>? analyzerState, bool isGeneratedCode) where TBlockStartAction : AnalyzerAction where TBlockAction : AnalyzerAction where TNodeAction : AnalyzerAction where TNodeStateData : AnalysisState.AnalyzerStateData, new() where TLanguageKindEnum : struct
        {
            if ((isGeneratedCode && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForTree(analyzer, declaredNode.SyntaxTree))
            {
                return;
            }
            PooledHashSet<TBlockAction> instance = PooledHashSet<TBlockAction>.GetInstance();
            PooledHashSet<TBlockAction> instance2 = PooledHashSet<TBlockAction>.GetInstance();
            ArrayBuilder<TNodeAction> instance3 = ArrayBuilder<TNodeAction>.GetInstance();
            ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> arrayBuilder = instance3 as ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>;
            ArrayBuilder<OperationAnalyzerAction> arrayBuilder2 = instance3 as ArrayBuilder<OperationAnalyzerAction>;
            ImmutableArray<IOperation> operationBlocks = ((executableBlocks[0] is IOperation) ? ((ImmutableArray<IOperation>)(object)executableBlocks) : ImmutableArray<IOperation>.Empty);
            instance2.AddAll(actions);
            if (analyzerState?.CurrentBlockEndActions != null)
            {
                instance.AddAll(analyzerState!.CurrentBlockEndActions.Cast<TBlockAction>());
                instance3.AddRange(analyzerState!.CurrentBlockNodeActions.Cast<TNodeAction>());
            }
            else
            {
                instance.AddAll(endActions);
            }
            AnalyzerDiagnosticReporter addSemanticDiagnostic = GetAddSemanticDiagnostic(semanticModel.SyntaxTree, declaredNode.FullSpan, analyzer);
            try
            {
                foreach (TBlockStartAction startAction in startActions)
                {
                    if (!ShouldExecuteAction(analyzerState, startAction))
                    {
                        continue;
                    }
                    if (startAction is CodeBlockStartAnalyzerAction<TLanguageKindEnum> codeBlockStartAnalyzerAction)
                    {
                        PooledHashSet<CodeBlockAnalyzerAction>? item = instance as PooledHashSet<CodeBlockAnalyzerAction>;
                        HostCodeBlockStartAnalysisScope<TLanguageKindEnum> hostCodeBlockStartAnalysisScope = new HostCodeBlockStartAnalysisScope<TLanguageKindEnum>();
                        AnalyzerCodeBlockStartAnalysisContext<TLanguageKindEnum> item2 = new AnalyzerCodeBlockStartAnalysisContext<TLanguageKindEnum>(startAction.Analyzer, hostCodeBlockStartAnalysisScope, declaredNode, declaredSymbol, semanticModel, AnalyzerOptions, _cancellationToken);
                        ExecuteAndCatchIfThrows(startAction.Analyzer, delegate ((Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action, AnalyzerCodeBlockStartAnalysisContext<TLanguageKindEnum> context, HostCodeBlockStartAnalysisScope<TLanguageKindEnum> scope, PooledHashSet<CodeBlockAnalyzerAction>? blockEndActions, ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>? syntaxNodeActions) data)
                        {
                            data.action(data.context);
                            data.blockEndActions?.AddAll(data.scope.CodeBlockEndActions);
                            data.syntaxNodeActions?.AddRange(data.scope.SyntaxNodeActions);
                        }, (codeBlockStartAnalyzerAction.Action, item2, hostCodeBlockStartAnalysisScope, item, arrayBuilder), new AnalysisContextInfo(Compilation, declaredSymbol, declaredNode));
                    }
                    else if (startAction is OperationBlockStartAnalyzerAction operationBlockStartAnalyzerAction)
                    {
                        PooledHashSet<OperationBlockAnalyzerAction>? item3 = instance as PooledHashSet<OperationBlockAnalyzerAction>;
                        HostOperationBlockStartAnalysisScope hostOperationBlockStartAnalysisScope = new HostOperationBlockStartAnalysisScope();
                        AnalyzerOperationBlockStartAnalysisContext item4 = new AnalyzerOperationBlockStartAnalysisContext(startAction.Analyzer, hostOperationBlockStartAnalysisScope, operationBlocks, declaredSymbol, semanticModel.Compilation, AnalyzerOptions, GetControlFlowGraph, _cancellationToken);
                        ExecuteAndCatchIfThrows(startAction.Analyzer, delegate ((Action<OperationBlockStartAnalysisContext> action, AnalyzerOperationBlockStartAnalysisContext context, HostOperationBlockStartAnalysisScope scope, PooledHashSet<OperationBlockAnalyzerAction>? blockEndActions, ArrayBuilder<OperationAnalyzerAction>? operationActions) data)
                        {
                            data.action(data.context);
                            data.blockEndActions?.AddAll(data.scope.OperationBlockEndActions);
                            data.operationActions?.AddRange(data.scope.OperationActions);
                        }, (operationBlockStartAnalyzerAction.Action, item4, hostOperationBlockStartAnalysisScope, item3, arrayBuilder2), new AnalysisContextInfo(Compilation, declaredSymbol));
                    }
                    analyzerState?.ProcessedActions.Add(startAction);
                }
            }
            finally
            {
                if (analyzerState != null)
                {
                    analyzerState!.CurrentBlockEndActions = instance.ToImmutableHashSet();
                    analyzerState!.CurrentBlockNodeActions = ((IEnumerable<AnalyzerAction>)instance3).ToImmutableHashSet();
                }
            }
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                if (instance3.Any())
                {
                    if (arrayBuilder != null)
                    {
                        ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByKind = GetNodeActionsByKind(arrayBuilder);
                        IEnumerable<SyntaxNode> nodesToAnalyze = (IEnumerable<SyntaxNode>)getNodesToAnalyze(executableBlocks);
                        ExecuteSyntaxNodeActions(nodesToAnalyze, nodeActionsByKind, analyzer, declaredSymbol, semanticModel, getKind, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, ((analyzerState != null) ? analyzerState!.ExecutableNodesAnalysisState : null) as AnalysisState.SyntaxNodeAnalyzerStateData);
                    }
                    else if (arrayBuilder2 != null)
                    {
                        ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByKind = GetOperationActionsByKind(arrayBuilder2);
                        IEnumerable<IOperation> operationsToAnalyze = (IEnumerable<IOperation>)getNodesToAnalyze(executableBlocks);
                        ExecuteOperationActions(operationsToAnalyze, operationActionsByKind, analyzer, declaredSymbol, semanticModel, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, ((analyzerState != null) ? analyzerState!.ExecutableNodesAnalysisState : null) as AnalysisState.OperationAnalyzerStateData);
                    }
                }
                instance3.Free();
                ExecuteBlockActions(instance2, declaredNode, declaredSymbol, analyzer, semanticModel, operationBlocks, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, analyzerState);
                ExecuteBlockActions(instance, declaredNode, declaredSymbol, analyzer, semanticModel, operationBlocks, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, analyzerState);
                addSemanticDiagnostic.Free();
            }
        }

        private void ExecuteBlockActions<TBlockAction, TNodeStateData>(PooledHashSet<TBlockAction> blockActions, SyntaxNode declaredNode, ISymbol declaredSymbol, DiagnosticAnalyzer analyzer, SemanticModel semanticModel, ImmutableArray<IOperation> operationBlocks, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.BlockAnalyzerStateData<TBlockAction, TNodeStateData>? analyzerState) where TBlockAction : AnalyzerAction where TNodeStateData : AnalysisState.AnalyzerStateData, new()
        {
            foreach (TBlockAction blockAction in blockActions)
            {
                if (!ShouldExecuteAction(analyzerState, blockAction))
                {
                    continue;
                }
                if (blockAction is CodeBlockAnalyzerAction codeBlockAnalyzerAction)
                {
                    CodeBlockAnalysisContext item = new CodeBlockAnalysisContext(declaredNode, declaredSymbol, semanticModel, AnalyzerOptions, addDiagnostic, isSupportedDiagnostic, _cancellationToken);
                    ExecuteAndCatchIfThrows(codeBlockAnalyzerAction.Analyzer, delegate ((Action<CodeBlockAnalysisContext> action, CodeBlockAnalysisContext context) data)
                    {
                        data.action(data.context);
                    }, (codeBlockAnalyzerAction.Action, item), new AnalysisContextInfo(Compilation, declaredSymbol, declaredNode));
                }
                else if (blockAction is OperationBlockAnalyzerAction operationBlockAnalyzerAction)
                {
                    OperationBlockAnalysisContext item2 = new OperationBlockAnalysisContext(operationBlocks, declaredSymbol, semanticModel.Compilation, AnalyzerOptions, addDiagnostic, isSupportedDiagnostic, GetControlFlowGraph, _cancellationToken);
                    ExecuteAndCatchIfThrows(operationBlockAnalyzerAction.Analyzer, delegate ((Action<OperationBlockAnalysisContext> action, OperationBlockAnalysisContext context) data)
                    {
                        data.action(data.context);
                    }, (operationBlockAnalyzerAction.Action, item2), new AnalysisContextInfo(Compilation, declaredSymbol));
                }
                analyzerState?.ProcessedActions.Add(blockAction);
            }
            blockActions.Free();
        }

        internal static ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> GetNodeActionsByKind<TLanguageKindEnum>(IEnumerable<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> nodeActions) where TLanguageKindEnum : struct
        {
            PooledDictionary<TLanguageKindEnum, ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> instance = PooledDictionary<TLanguageKindEnum, ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>>.GetInstance();
            foreach (SyntaxNodeAnalyzerAction<TLanguageKindEnum> nodeAction in nodeActions)
            {
                ImmutableArray<TLanguageKindEnum>.Enumerator enumerator2 = nodeAction.Kinds.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    TLanguageKindEnum current2 = enumerator2.Current;
                    if (!instance.TryGetValue(current2, out var value))
                    {
                        instance.Add(current2, value = ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>.GetInstance());
                    }
                    value.Add(nodeAction);
                }
            }
            ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> result = ImmutableSegmentedDictionary.CreateRange(instance.Select((KeyValuePair<TLanguageKindEnum, ArrayBuilder<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> kvp) => KeyValuePairUtil.Create(kvp.Key, kvp.Value.ToImmutableAndFree())));
            instance.Free();
            return result;
        }

        public bool TryExecuteSyntaxNodeActions<TLanguageKindEnum>(IEnumerable<SyntaxNode> nodesToAnalyze, ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByKind, DiagnosticAnalyzer analyzer, SemanticModel model, Func<SyntaxNode, TLanguageKindEnum> getKind, TextSpan filterSpan, int declarationIndex, ISymbol declaredSymbol, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode) where TLanguageKindEnum : struct
        {
            AnalysisState.DeclarationAnalyzerStateData? analyzerState = null;
            try
            {
                if (TryStartAnalyzingDeclaration(declaredSymbol, declarationIndex, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteSyntaxNodeActionsCore(nodesToAnalyze, nodeActionsByKind, analyzer, declaredSymbol, model, getKind, filterSpan, analyzerState, isGeneratedCode);
                    return true;
                }
                return IsDeclarationComplete(declaredSymbol, declarationIndex, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteSyntaxNodeActionsCore<TLanguageKindEnum>(IEnumerable<SyntaxNode> nodesToAnalyze, ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByKind, DiagnosticAnalyzer analyzer, ISymbol containingSymbol, SemanticModel model, Func<SyntaxNode, TLanguageKindEnum> getKind, TextSpan filterSpan, AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState, bool isGeneratedCode) where TLanguageKindEnum : struct
        {
            if ((isGeneratedCode && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForTree(analyzer, model.SyntaxTree))
            {
                return;
            }
            AnalyzerDiagnosticReporter addSemanticDiagnostic = GetAddSemanticDiagnostic(model.SyntaxTree, filterSpan, analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ExecuteSyntaxNodeActions(nodesToAnalyze, nodeActionsByKind, analyzer, containingSymbol, model, getKind, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, analyzerState);
                addSemanticDiagnostic.Free();
            }
        }

        private void ExecuteSyntaxNodeActions<TLanguageKindEnum>(IEnumerable<SyntaxNode> nodesToAnalyze, ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByKind, DiagnosticAnalyzer analyzer, ISymbol containingSymbol, SemanticModel model, Func<SyntaxNode, TLanguageKindEnum> getKind, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState) where TLanguageKindEnum : struct
        {
            SyntaxNode syntaxNode = analyzerState?.CurrentNode;
            if (syntaxNode != null)
            {
                ExecuteSyntaxNodeActions(syntaxNode, nodeActionsByKind, containingSymbol, model, getKind, addDiagnostic, isSupportedDiagnostic, analyzerState);
            }
            foreach (SyntaxNode item in nodesToAnalyze)
            {
                if (ShouldExecuteNode(analyzerState, item, analyzer))
                {
                    SetCurrentNode(analyzerState, item);
                    ExecuteSyntaxNodeActions(item, nodeActionsByKind, containingSymbol, model, getKind, addDiagnostic, isSupportedDiagnostic, analyzerState);
                }
            }
        }

        private void ExecuteSyntaxNodeActions<TLanguageKindEnum>(SyntaxNode node, ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByKind, ISymbol containingSymbol, SemanticModel model, Func<SyntaxNode, TLanguageKindEnum> getKind, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState) where TLanguageKindEnum : struct
        {
            if (nodeActionsByKind.TryGetValue(getKind(node), out var value))
            {
                ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>.Enumerator enumerator = value.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNodeAnalyzerAction<TLanguageKindEnum> current = enumerator.Current;
                    ExecuteSyntaxNodeAction(current, node, containingSymbol, model, addDiagnostic, isSupportedDiagnostic, analyzerState);
                }
            }
            analyzerState?.ClearNodeAnalysisState();
        }

        internal static ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> GetOperationActionsByKind(IEnumerable<OperationAnalyzerAction> operationActions)
        {
            PooledDictionary<OperationKind, ArrayBuilder<OperationAnalyzerAction>> instance = PooledDictionary<OperationKind, ArrayBuilder<OperationAnalyzerAction>>.GetInstance();
            foreach (OperationAnalyzerAction operationAction in operationActions)
            {
                ImmutableArray<OperationKind>.Enumerator enumerator2 = operationAction.Kinds.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    OperationKind current2 = enumerator2.Current;
                    if (!instance.TryGetValue(current2, out var value))
                    {
                        instance.Add(current2, value = ArrayBuilder<OperationAnalyzerAction>.GetInstance());
                    }
                    value.Add(operationAction);
                }
            }
            ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> result = ImmutableSegmentedDictionary.CreateRange(instance.Select((KeyValuePair<OperationKind, ArrayBuilder<OperationAnalyzerAction>> kvp) => KeyValuePairUtil.Create(kvp.Key, kvp.Value.ToImmutableAndFree())));
            instance.Free();
            return result;
        }

        public bool TryExecuteOperationActions(IEnumerable<IOperation> operationsToAnalyze, ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByKind, DiagnosticAnalyzer analyzer, SemanticModel model, TextSpan filterSpan, int declarationIndex, ISymbol declaredSymbol, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCode)
        {
            AnalysisState.DeclarationAnalyzerStateData? analyzerState = null;
            try
            {
                if (TryStartAnalyzingDeclaration(declaredSymbol, declarationIndex, analyzer, analysisScope, analysisState, out analyzerState))
                {
                    ExecuteOperationActionsCore(operationsToAnalyze, operationActionsByKind, analyzer, declaredSymbol, model, filterSpan, analyzerState?.OperationBlockAnalysisState.ExecutableNodesAnalysisState, isGeneratedCode);
                    return true;
                }
                return IsDeclarationComplete(declaredSymbol, declarationIndex, analyzer, analysisState);
            }
            finally
            {
                analyzerState?.ResetToReadyState();
            }
        }

        private void ExecuteOperationActionsCore(IEnumerable<IOperation> operationsToAnalyze, ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByKind, DiagnosticAnalyzer analyzer, ISymbol containingSymbol, SemanticModel model, TextSpan filterSpan, AnalysisState.OperationAnalyzerStateData? analyzerState, bool isGeneratedCode)
        {
            if ((isGeneratedCode && _shouldSkipAnalysisOnGeneratedCode(analyzer)) || IsAnalyzerSuppressedForTree(analyzer, model.SyntaxTree))
            {
                return;
            }
            AnalyzerDiagnosticReporter addSemanticDiagnostic = GetAddSemanticDiagnostic(model.SyntaxTree, filterSpan, analyzer);
            using (PooledDelegates.GetPooledFunction((Diagnostic d, (AnalyzerExecutor self, DiagnosticAnalyzer analyzer) arg) => arg.self.IsSupportedDiagnostic(arg.analyzer, d), (this, analyzer), out Func<Diagnostic, bool> boundFunction))
            {
                ExecuteOperationActions(operationsToAnalyze, operationActionsByKind, analyzer, containingSymbol, model, addSemanticDiagnostic.AddDiagnosticAction, boundFunction, analyzerState);
                addSemanticDiagnostic.Free();
            }
        }

        private void ExecuteOperationActions(IEnumerable<IOperation> operationsToAnalyze, ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByKind, DiagnosticAnalyzer analyzer, ISymbol containingSymbol, SemanticModel model, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.OperationAnalyzerStateData? analyzerState)
        {
            IOperation operation = analyzerState?.CurrentOperation;
            if (operation != null)
            {
                ExecuteOperationActions(operation, operationActionsByKind, containingSymbol, model, addDiagnostic, isSupportedDiagnostic, analyzerState);
            }
            foreach (IOperation item in operationsToAnalyze)
            {
                if (ShouldExecuteOperation(analyzerState, item, analyzer))
                {
                    SetCurrentOperation(analyzerState, item);
                    ExecuteOperationActions(item, operationActionsByKind, containingSymbol, model, addDiagnostic, isSupportedDiagnostic, analyzerState);
                }
            }
        }

        private void ExecuteOperationActions(IOperation operation, ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByKind, ISymbol containingSymbol, SemanticModel model, Action<Diagnostic> addDiagnostic, Func<Diagnostic, bool> isSupportedDiagnostic, AnalysisState.OperationAnalyzerStateData? analyzerState)
        {
            if (operationActionsByKind.TryGetValue(operation.Kind, out var value))
            {
                ImmutableArray<OperationAnalyzerAction>.Enumerator enumerator = value.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    OperationAnalyzerAction current = enumerator.Current;
                    ExecuteOperationAction(current, operation, containingSymbol, model, addDiagnostic, isSupportedDiagnostic, analyzerState);
                }
            }
            analyzerState?.ClearNodeAnalysisState();
        }

        internal static bool CanHaveExecutableCodeBlock(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Method:
                case SymbolKind.NamedType:
                case SymbolKind.Namespace:
                case SymbolKind.Property:
                    return true;
                case SymbolKind.Field:
                    return true;
                default:
                    return false;
            }
        }

        internal void ExecuteAndCatchIfThrows<TArg>(DiagnosticAnalyzer analyzer, Action<TArg> analyze, TArg argument, AnalysisContextInfo? info = null)
        {
            SharedStopwatch sharedStopwatch = default(SharedStopwatch);
            if (_analyzerExecutionTimeMap != null)
            {
                SharedStopwatch.StartNew();
                sharedStopwatch = SharedStopwatch.StartNew();
            }
            object obj = _getAnalyzerGate?.Invoke(analyzer);
            if (obj != null)
            {
                lock (obj)
                {
                    ExecuteAndCatchIfThrows_NoLock(analyzer, analyze, argument, info);
                }
            }
            else
            {
                ExecuteAndCatchIfThrows_NoLock(analyzer, analyze, argument, info);
            }
            if (_analyzerExecutionTimeMap != null)
            {
                long ticks = sharedStopwatch.Elapsed.Ticks;
                Interlocked.Add(ref _analyzerExecutionTimeMap!.GetOrAdd(analyzer, (DiagnosticAnalyzer _) => new StrongBox<long>(0L)).Value, ticks);
            }
        }

        private void ExecuteAndCatchIfThrows_NoLock<TArg>(DiagnosticAnalyzer analyzer, Action<TArg> analyze, TArg argument, AnalysisContextInfo? info)
        {
            try
            {
                CancellationToken cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                analyze(argument);
            }
            catch (Exception ex) when (ExceptionFilter(ex))
            {
                Diagnostic arg = CreateAnalyzerExceptionDiagnostic(analyzer, ex, info);
                try
                {
                    _onAnalyzerException(ex, analyzer, arg);
                }
                catch (Exception)
                {
                }
            }
        }

        internal bool ExceptionFilter(Exception ex)
        {
            if ((ex as OperationCanceledException)?.CancellationToken == _cancellationToken)
            {
                return false;
            }
            if (_analyzerExceptionFilter != null)
            {
                return _analyzerExceptionFilter!(ex);
            }
            return true;
        }

        internal static Diagnostic CreateAnalyzerExceptionDiagnostic(DiagnosticAnalyzer analyzer, Exception e, AnalysisContextInfo? info = null)
        {
            string text = analyzer.ToString();
            string compilerAnalyzerFailure = CodeAnalysisResources.CompilerAnalyzerFailure;
            string compilerAnalyzerThrows = CodeAnalysisResources.CompilerAnalyzerThrows;
            string text2 = string.Join(Environment.NewLine, CreateDiagnosticDescription(info, e), CreateDisablingMessage(analyzer)).Trim();
            string[] array = new string[4]
            {
                text,
                e.GetType().ToString(),
                e.Message,
                text2
            };
            string description = string.Format(CodeAnalysisResources.CompilerAnalyzerThrowsDescription, text, CreateDiagnosticDescription(info, e));
            DiagnosticDescriptor analyzerExceptionDiagnosticDescriptor = GetAnalyzerExceptionDiagnosticDescriptor("AD0001", compilerAnalyzerFailure, description, compilerAnalyzerThrows);
            Location none = Location.None;
            object[] messageArgs = array;
            return Diagnostic.Create(analyzerExceptionDiagnosticDescriptor, none, messageArgs);
        }

        private static string CreateDiagnosticDescription(AnalysisContextInfo? info, Exception e)
        {
            if (!info.HasValue)
            {
                return e.CreateDiagnosticDescription();
            }
            return string.Join(Environment.NewLine, string.Format(CodeAnalysisResources.ExceptionContext, info?.GetContext()), e.CreateDiagnosticDescription());
        }

        private static string CreateDisablingMessage(DiagnosticAnalyzer analyzer)
        {
            ImmutableSortedSet<string> immutableSortedSet = ImmutableSortedSet<string>.Empty.WithComparer(StringComparer.OrdinalIgnoreCase);
            try
            {
                ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator = analyzer.SupportedDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticDescriptor current = enumerator.Current;
                    immutableSortedSet = immutableSortedSet.Add(current.Id);
                }
            }
            catch (Exception exception) when (FatalError.ReportAndCatch(exception))
            {
            }
            if (immutableSortedSet.IsEmpty)
            {
                return "";
            }
            return string.Format(CodeAnalysisResources.DisableAnalyzerDiagnosticsMessage, string.Join(", ", immutableSortedSet));
        }

        internal static Diagnostic CreateDriverExceptionDiagnostic(Exception e)
        {
            string analyzerDriverFailure = CodeAnalysisResources.AnalyzerDriverFailure;
            string analyzerDriverThrows = CodeAnalysisResources.AnalyzerDriverThrows;
            string[] array = new string[2]
            {
                e.GetType().ToString(),
                e.Message
            };
            string description = string.Format(CodeAnalysisResources.AnalyzerDriverThrowsDescription, e.CreateDiagnosticDescription());
            DiagnosticDescriptor analyzerExceptionDiagnosticDescriptor = GetAnalyzerExceptionDiagnosticDescriptor("AD0002", analyzerDriverFailure, description, analyzerDriverThrows);
            Location none = Location.None;
            object[] messageArgs = array;
            return Diagnostic.Create(analyzerExceptionDiagnosticDescriptor, none, messageArgs);
        }

        internal static DiagnosticDescriptor GetAnalyzerExceptionDiagnosticDescriptor(string? id = null, string? title = null, string? description = null, string? messageFormat = null)
        {
            if (id == null)
            {
                id = "AD0001";
            }
            if (title == null)
            {
                title = CodeAnalysisResources.CompilerAnalyzerFailure;
            }
            if (messageFormat == null)
            {
                messageFormat = CodeAnalysisResources.CompilerAnalyzerThrows;
            }
            if (description == null)
            {
                description = CodeAnalysisResources.CompilerAnalyzerFailure;
            }
            return new DiagnosticDescriptor(id, title, messageFormat, "Compiler", DiagnosticSeverity.Warning, true, description, null, "AnalyzerException");
        }

        internal static bool IsAnalyzerExceptionDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic.Id == "AD0001" || diagnostic.Id == "AD0002")
            {
                foreach (string customTag in diagnostic.Descriptor.CustomTags)
                {
                    if (customTag == "AnalyzerException")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool AreEquivalentAnalyzerExceptionDiagnostics(Diagnostic exceptionDiagnostic, Diagnostic other)
        {
            if (!IsAnalyzerExceptionDiagnostic(other))
            {
                return false;
            }
            if (exceptionDiagnostic.Id == other.Id && exceptionDiagnostic.Severity == other.Severity)
            {
                return exceptionDiagnostic.GetMessage() == other.GetMessage();
            }
            return false;
        }

        private bool IsSupportedDiagnostic(DiagnosticAnalyzer analyzer, Diagnostic diagnostic)
        {
            if (diagnostic is DiagnosticWithInfo)
            {
                return true;
            }
            return _analyzerManager.IsSupportedDiagnostic(analyzer, diagnostic, _isCompilerAnalyzer, this);
        }

        private Action<Diagnostic> GetAddDiagnostic(ISymbol contextSymbol, ImmutableArray<SyntaxReference> cachedDeclaringReferences, DiagnosticAnalyzer analyzer, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis)
        {
            return GetAddDiagnostic(contextSymbol, cachedDeclaringReferences, Compilation, analyzer, _addNonCategorizedDiagnostic, _addCategorizedLocalDiagnostic, _addCategorizedNonLocalDiagnostic, getTopMostNodeForAnalysis, _shouldSuppressGeneratedCodeDiagnostic, _cancellationToken);
        }

        private static Action<Diagnostic> GetAddDiagnostic(ISymbol contextSymbol, ImmutableArray<SyntaxReference> cachedDeclaringReferences, Compilation compilation, DiagnosticAnalyzer analyzer, Action<Diagnostic>? addNonCategorizedDiagnostic, Action<Diagnostic, DiagnosticAnalyzer, bool>? addCategorizedLocalDiagnostic, Action<Diagnostic, DiagnosticAnalyzer>? addCategorizedNonLocalDiagnostic, Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis, Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> shouldSuppressGeneratedCodeDiagnostic, CancellationToken cancellationToken)
        {
            Func<Diagnostic, DiagnosticAnalyzer, Compilation, CancellationToken, bool> shouldSuppressGeneratedCodeDiagnostic2 = shouldSuppressGeneratedCodeDiagnostic;
            DiagnosticAnalyzer analyzer2 = analyzer;
            Compilation compilation2 = compilation;
            Action<Diagnostic, DiagnosticAnalyzer, bool> addCategorizedLocalDiagnostic2 = addCategorizedLocalDiagnostic;
            Action<Diagnostic> addNonCategorizedDiagnostic2 = addNonCategorizedDiagnostic;
            Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> getTopMostNodeForAnalysis2 = getTopMostNodeForAnalysis;
            ISymbol contextSymbol2 = contextSymbol;
            Action<Diagnostic, DiagnosticAnalyzer> addCategorizedNonLocalDiagnostic2 = addCategorizedNonLocalDiagnostic;
            return delegate (Diagnostic diagnostic)
            {
                if (!shouldSuppressGeneratedCodeDiagnostic2(diagnostic, analyzer2, compilation2, cancellationToken))
                {
                    if (addCategorizedLocalDiagnostic2 == null)
                    {
                        addNonCategorizedDiagnostic2(diagnostic);
                    }
                    else
                    {
                        if (diagnostic.Location.IsInSource)
                        {
                            ImmutableArray<SyntaxReference>.Enumerator enumerator = cachedDeclaringReferences.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                SyntaxReference current = enumerator.Current;
                                if (current.SyntaxTree == diagnostic.Location.SourceTree)
                                {
                                    SyntaxNode syntaxNode = getTopMostNodeForAnalysis2(contextSymbol2, current, compilation2, cancellationToken);
                                    if (diagnostic.Location.SourceSpan.IntersectsWith(syntaxNode.FullSpan))
                                    {
                                        addCategorizedLocalDiagnostic2(diagnostic, analyzer2, arg3: false);
                                        return;
                                    }
                                }
                            }
                        }
                        addCategorizedNonLocalDiagnostic2(diagnostic, analyzer2);
                    }
                }
            };
        }

        private Action<Diagnostic> GetAddCompilationDiagnostic(DiagnosticAnalyzer analyzer)
        {
            DiagnosticAnalyzer analyzer2 = analyzer;
            return delegate (Diagnostic diagnostic)
            {
                if (!_shouldSuppressGeneratedCodeDiagnostic(diagnostic, analyzer2, Compilation, _cancellationToken))
                {
                    if (_addCategorizedNonLocalDiagnostic == null)
                    {
                        _addNonCategorizedDiagnostic!(diagnostic);
                    }
                    else
                    {
                        _addCategorizedNonLocalDiagnostic!(diagnostic, analyzer2);
                    }
                }
            };
        }

        private AnalyzerDiagnosticReporter GetAddSemanticDiagnostic(SyntaxTree tree, DiagnosticAnalyzer analyzer)
        {
            return AnalyzerDiagnosticReporter.GetInstance(new SourceOrAdditionalFile(tree), null, Compilation, analyzer, isSyntaxDiagnostic: false, _addNonCategorizedDiagnostic, _addCategorizedLocalDiagnostic, _addCategorizedNonLocalDiagnostic, _shouldSuppressGeneratedCodeDiagnostic, _cancellationToken);
        }

        private AnalyzerDiagnosticReporter GetAddSemanticDiagnostic(SyntaxTree tree, TextSpan? span, DiagnosticAnalyzer analyzer)
        {
            return AnalyzerDiagnosticReporter.GetInstance(new SourceOrAdditionalFile(tree), span, Compilation, analyzer, isSyntaxDiagnostic: false, _addNonCategorizedDiagnostic, _addCategorizedLocalDiagnostic, _addCategorizedNonLocalDiagnostic, _shouldSuppressGeneratedCodeDiagnostic, _cancellationToken);
        }

        private AnalyzerDiagnosticReporter GetAddSyntaxDiagnostic(SourceOrAdditionalFile file, DiagnosticAnalyzer analyzer)
        {
            return AnalyzerDiagnosticReporter.GetInstance(file, null, Compilation, analyzer, isSyntaxDiagnostic: true, _addNonCategorizedDiagnostic, _addCategorizedLocalDiagnostic, _addCategorizedNonLocalDiagnostic, _shouldSuppressGeneratedCodeDiagnostic, _cancellationToken);
        }

        private static bool ShouldExecuteAction(AnalysisState.AnalyzerStateData? analyzerState, AnalyzerAction action)
        {
            if (analyzerState != null)
            {
                return !analyzerState!.ProcessedActions.Contains(action);
            }
            return true;
        }

        private bool ShouldExecuteNode(AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState, SyntaxNode node, DiagnosticAnalyzer analyzer)
        {
            if (analyzerState != null && analyzerState!.ProcessedNodes.Contains(node))
            {
                return false;
            }
            if (_shouldSkipAnalysisOnGeneratedCode(analyzer) && _isGeneratedCodeLocation(node.SyntaxTree, node.Span))
            {
                return false;
            }
            return true;
        }

        private bool ShouldExecuteOperation(AnalysisState.OperationAnalyzerStateData? analyzerState, IOperation operation, DiagnosticAnalyzer analyzer)
        {
            if (analyzerState != null && analyzerState!.ProcessedOperations.Contains(operation))
            {
                return false;
            }
            if (operation.Syntax != null && _shouldSkipAnalysisOnGeneratedCode(analyzer) && _isGeneratedCodeLocation(operation.Syntax.SyntaxTree, operation.Syntax.Span))
            {
                return false;
            }
            return true;
        }

        private static void SetCurrentNode(AnalysisState.SyntaxNodeAnalyzerStateData? analyzerState, SyntaxNode node)
        {
            if (analyzerState != null)
            {
                analyzerState!.CurrentNode = node;
            }
        }

        private static void SetCurrentOperation(AnalysisState.OperationAnalyzerStateData? analyzerState, IOperation operation)
        {
            if (analyzerState != null)
            {
                analyzerState!.CurrentOperation = operation;
            }
        }

        private static bool TryStartProcessingEvent(CompilationEvent nonSymbolCompilationEvent, DiagnosticAnalyzer analyzer, AnalysisScope analysisScope, AnalysisState? analysisState, out AnalysisState.AnalyzerStateData? analyzerState)
        {
            analyzerState = null;
            return analysisState?.TryStartProcessingEvent(nonSymbolCompilationEvent, analyzer, out analyzerState) ?? true;
        }

        private static bool TryStartSyntaxAnalysis(SourceOrAdditionalFile file, DiagnosticAnalyzer analyzer, AnalysisScope analysisScope, AnalysisState? analysisState, out AnalysisState.AnalyzerStateData? analyzerState)
        {
            analyzerState = null;
            return analysisState?.TryStartSyntaxAnalysis(file, analyzer, out analyzerState) ?? true;
        }

        private static bool TryStartAnalyzingSymbol(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisScope analysisScope, AnalysisState? analysisState, out AnalysisState.AnalyzerStateData? analyzerState)
        {
            analyzerState = null;
            return analysisState?.TryStartAnalyzingSymbol(symbol, analyzer, out analyzerState) ?? true;
        }

        private static bool TryStartSymbolEndAnalysis(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState, out AnalysisState.AnalyzerStateData? analyzerState)
        {
            analyzerState = null;
            return analysisState?.TryStartSymbolEndAnalysis(symbol, analyzer, out analyzerState) ?? true;
        }

        private static bool TryStartAnalyzingDeclaration(ISymbol symbol, int declarationIndex, DiagnosticAnalyzer analyzer, AnalysisScope analysisScope, AnalysisState? analysisState, out AnalysisState.DeclarationAnalyzerStateData? analyzerState)
        {
            analyzerState = null;
            return analysisState?.TryStartAnalyzingDeclaration(symbol, declarationIndex, analyzer, out analyzerState) ?? true;
        }

        private static bool IsEventComplete(CompilationEvent compilationEvent, DiagnosticAnalyzer analyzer, AnalysisState? analysisState)
        {
            return analysisState?.IsEventComplete(compilationEvent, analyzer) ?? true;
        }

        private static bool IsSymbolComplete(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState)
        {
            return analysisState?.IsSymbolComplete(symbol, analyzer) ?? true;
        }

        private static bool IsSymbolEndAnalysisComplete(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState)
        {
            return analysisState?.IsSymbolEndAnalysisComplete(symbol, analyzer) ?? true;
        }

        private static bool IsDeclarationComplete(ISymbol symbol, int declarationIndex, DiagnosticAnalyzer analyzer, AnalysisState? analysisState)
        {
            return analysisState?.IsDeclarationComplete(symbol, declarationIndex, analyzer) ?? true;
        }

        internal TimeSpan ResetAnalyzerExecutionTime(DiagnosticAnalyzer analyzer)
        {
            if (!_analyzerExecutionTimeMap!.TryRemove(analyzer, out var value))
            {
                return TimeSpan.Zero;
            }
            return TimeSpan.FromTicks(value.Value);
        }

        private ControlFlowGraph GetControlFlowGraphImpl(IOperation operation)
        {
            if (_lazyControlFlowGraphMap == null)
            {
                Interlocked.CompareExchange(ref _lazyControlFlowGraphMap, new ConcurrentDictionary<IOperation, ControlFlowGraph>(), null);
            }
            return _lazyControlFlowGraphMap!.GetOrAdd(operation, delegate (IOperation op)
            {
                ControlFlowGraphBuilder.Context context = default(ControlFlowGraphBuilder.Context);
                return ControlFlowGraphBuilder.Create(op, null, null, null, in context);
            });
        }

        private bool IsAnalyzerSuppressedForSymbol(DiagnosticAnalyzer analyzer, ISymbol symbol)
        {
            ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Location current = enumerator.Current;
                if (current.SourceTree != null && !IsAnalyzerSuppressedForTree(analyzer, current.SourceTree))
                {
                    return false;
                }
            }
            return true;
        }

        public void OnOperationBlockActionsExecuted(ImmutableArray<IOperation> operationBlocks)
        {
            ConcurrentDictionary<IOperation, ControlFlowGraph>? lazyControlFlowGraphMap = _lazyControlFlowGraphMap;
            if (lazyControlFlowGraphMap != null && lazyControlFlowGraphMap!.Count > 0)
            {
                ImmutableArray<IOperation>.Enumerator enumerator = operationBlocks.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IOperation rootOperation = enumerator.Current.GetRootOperation();
                    _lazyControlFlowGraphMap!.TryRemove(rootOperation, out var _);
                }
            }
        }
    }
}
