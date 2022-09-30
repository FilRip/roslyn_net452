using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class AnalyzerDriver : IDisposable
    {
        public class CompilationData
        {
            private readonly Dictionary<SyntaxReference, DeclarationAnalysisData> _declarationAnalysisDataMap;

            public CachingSemanticModelProvider SemanticModelProvider { get; }

            public SuppressMessageAttributeState SuppressMessageAttributeState { get; }

            public CompilationData(Compilation compilation)
            {
                SemanticModelProvider = (CachingSemanticModelProvider)compilation.SemanticModelProvider;
                SuppressMessageAttributeState = new SuppressMessageAttributeState(compilation);
                _declarationAnalysisDataMap = new Dictionary<SyntaxReference, DeclarationAnalysisData>();
            }

            internal DeclarationAnalysisData GetOrComputeDeclarationAnalysisData(SyntaxReference declaration, Func<DeclarationAnalysisData> computeDeclarationAnalysisData, bool cacheAnalysisData)
            {
                if (!cacheAnalysisData)
                {
                    return computeDeclarationAnalysisData();
                }
                lock (_declarationAnalysisDataMap)
                {
                    if (_declarationAnalysisDataMap.TryGetValue(declaration, out var value))
                    {
                        return value;
                    }
                }
                DeclarationAnalysisData declarationAnalysisData = computeDeclarationAnalysisData();
                lock (_declarationAnalysisDataMap)
                {
                    if (!_declarationAnalysisDataMap.TryGetValue(declaration, out var value2))
                    {
                        _declarationAnalysisDataMap.Add(declaration, declarationAnalysisData);
                        return declarationAnalysisData;
                    }
                    return value2;
                }
            }

            internal void ClearDeclarationAnalysisData(SyntaxReference declaration)
            {
                lock (_declarationAnalysisDataMap)
                {
                    _declarationAnalysisDataMap.Remove(declaration);
                }
            }
        }

        internal sealed class DeclarationAnalysisData
        {
            public SyntaxNode DeclaringReferenceSyntax { get; }

            public SyntaxNode TopmostNodeForAnalysis { get; }

            public ImmutableArray<DeclarationInfo> DeclarationsInNode { get; }

            public ImmutableArray<SyntaxNode> DescendantNodesToAnalyze { get; }

            public bool IsPartialAnalysis { get; }

            public DeclarationAnalysisData(SyntaxNode declaringReferenceSyntax, SyntaxNode topmostNodeForAnalysis, ImmutableArray<DeclarationInfo> declarationsInNodeBuilder, ImmutableArray<SyntaxNode> descendantNodesToAnalyze, bool isPartialAnalysis)
            {
                DeclaringReferenceSyntax = declaringReferenceSyntax;
                TopmostNodeForAnalysis = topmostNodeForAnalysis;
                DeclarationsInNode = declarationsInNodeBuilder;
                DescendantNodesToAnalyze = descendantNodesToAnalyze;
                IsPartialAnalysis = isPartialAnalysis;
            }
        }

        private sealed class EventProcessedState
        {
            public static readonly EventProcessedState Processed = new EventProcessedState(EventProcessedStateKind.Processed);

            public static readonly EventProcessedState NotProcessed = new EventProcessedState(EventProcessedStateKind.NotProcessed);

            public EventProcessedStateKind Kind { get; }

            public ImmutableArray<DiagnosticAnalyzer> SubsetProcessedAnalyzers { get; }

            private EventProcessedState(EventProcessedStateKind kind)
            {
                Kind = kind;
                SubsetProcessedAnalyzers = default(ImmutableArray<DiagnosticAnalyzer>);
            }

            private EventProcessedState(ImmutableArray<DiagnosticAnalyzer> subsetProcessedAnalyzers)
            {
                SubsetProcessedAnalyzers = subsetProcessedAnalyzers;
                Kind = EventProcessedStateKind.PartiallyProcessed;
            }

            public static EventProcessedState CreatePartiallyProcessed(ImmutableArray<DiagnosticAnalyzer> subsetProcessedAnalyzers)
            {
                return new EventProcessedState(subsetProcessedAnalyzers);
            }
        }

        private enum EventProcessedStateKind
        {
            Processed,
            NotProcessed,
            PartiallyProcessed
        }

        private sealed class GeneratedCodeTokenWalker : SyntaxWalker
        {
            private readonly CancellationToken _cancellationToken;

            public bool HasGeneratedCodeIdentifier { get; private set; }

            public GeneratedCodeTokenWalker(CancellationToken cancellationToken)
                : base(SyntaxWalkerDepth.Token)
            {
                _cancellationToken = cancellationToken;
            }

            public override void Visit(SyntaxNode node)
            {
                if (!HasGeneratedCodeIdentifier)
                {
                    CancellationToken cancellationToken = _cancellationToken;
                    cancellationToken.ThrowIfCancellationRequested();
                    base.Visit(node);
                }
            }

            protected override void VisitToken(SyntaxToken token)
            {
                HasGeneratedCodeIdentifier |= string.Equals(token.ValueText, "GeneratedCode", StringComparison.Ordinal) || string.Equals(token.ValueText, "GeneratedCodeAttribute", StringComparison.Ordinal);
            }
        }

        protected interface IGroupedAnalyzerActions
        {
            bool IsEmpty { get; }

            AnalyzerActions AnalyzerActions { get; }

            IGroupedAnalyzerActions Append(IGroupedAnalyzerActions groupedAnalyzerActions);
        }

        private const int MaxSymbolKind = 100;

        private static readonly Func<DiagnosticAnalyzer, bool> s_IsCompilerAnalyzerFunc = IsCompilerAnalyzer;

        private static readonly Func<ISymbol, SyntaxReference, Compilation, CancellationToken, SyntaxNode> s_getTopmostNodeForAnalysis = GetTopmostNodeForAnalysis;

        private readonly Func<SyntaxTree, CancellationToken, bool> _isGeneratedCode;

        private readonly ConcurrentSet<Suppression>? _programmaticSuppressions;

        private readonly ConcurrentSet<Diagnostic>? _diagnosticsProcessedForProgrammaticSuppressions;

        private readonly bool _hasDiagnosticSuppressors;

        private readonly SeverityFilter _severityFilter;

        private CancellationTokenRegistration? _lazyQueueRegistration;

        private AnalyzerExecutor? _lazyAnalyzerExecutor;

        private CompilationData? _lazyCurrentCompilationData;

        private ImmutableHashSet<DiagnosticAnalyzer>? _lazyUnsuppressedAnalyzers;

        private ConcurrentDictionary<(INamespaceOrTypeSymbol, DiagnosticAnalyzer), IGroupedAnalyzerActions>? _lazyPerSymbolAnalyzerActionsCache;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<ImmutableArray<SymbolAnalyzerAction>>)> _lazySymbolActionsByKind;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<SemanticModelAnalyzerAction>)> _lazySemanticModelActions;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<SyntaxTreeAnalyzerAction>)> _lazySyntaxTreeActions;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<AdditionalFileAnalyzerAction>)> _lazyAdditionalFileActions;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)> _lazyCompilationActions;

        private ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)> _lazyCompilationEndActions;

        private ImmutableHashSet<DiagnosticAnalyzer>? _lazyCompilationEndAnalyzers;

        internal const GeneratedCodeAnalysisFlags DefaultGeneratedCodeAnalysisFlags = GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics;

        private ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim> _lazyAnalyzerGateMap;

        private ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> _lazyGeneratedCodeAnalysisFlagsMap;

        private AnalyzerActions _lazyAnalyzerActions;

        private ImmutableHashSet<DiagnosticAnalyzer>? _lazyNonConfigurableAnalyzers;

        private ImmutableHashSet<DiagnosticAnalyzer>? _lazySymbolStartAnalyzers;

        private bool? _lazyTreatAllCodeAsNonGeneratedCode;

        private bool? _lazyDoNotAnalyzeGeneratedCode;

        private ConcurrentDictionary<SyntaxTree, bool>? _lazyGeneratedCodeFilesMap;

        private Dictionary<SyntaxTree, ImmutableHashSet<ISymbol>>? _lazyGeneratedCodeSymbolsForTreeMap;

        private ConcurrentDictionary<SyntaxTree, ImmutableHashSet<DiagnosticAnalyzer>>? _lazySuppressedAnalyzersForTreeMap;

        private ConcurrentDictionary<ISymbol, bool>? _lazyIsGeneratedCodeSymbolMap;

        private ConcurrentDictionary<SyntaxTree, bool>? _lazyTreesWithHiddenRegionsMap;

        private INamedTypeSymbol? _lazyGeneratedCodeAttribute;

        private Task? _lazyInitializeTask;

        private bool _initializeSucceeded;

        private Task? _lazyPrimaryTask;

        private readonly int _workerCount = Environment.ProcessorCount;

        private AsyncQueue<CompilationEvent>? _lazyCompilationEventQueue;

        private DiagnosticQueue? _lazyDiagnosticQueue;

        protected ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        protected AnalyzerManager AnalyzerManager { get; }

        protected AnalyzerExecutor AnalyzerExecutor => _lazyAnalyzerExecutor;

        protected CompilationData CurrentCompilationData => _lazyCurrentCompilationData;

        protected CachingSemanticModelProvider SemanticModelProvider => CurrentCompilationData.SemanticModelProvider;

        protected ref readonly AnalyzerActions AnalyzerActions => ref _lazyAnalyzerActions;

        protected ImmutableHashSet<DiagnosticAnalyzer> UnsuppressedAnalyzers => _lazyUnsuppressedAnalyzers;

        private ConcurrentDictionary<(INamespaceOrTypeSymbol, DiagnosticAnalyzer), IGroupedAnalyzerActions> PerSymbolAnalyzerActionsCache => _lazyPerSymbolAnalyzerActionsCache;

        private ImmutableHashSet<DiagnosticAnalyzer> CompilationEndAnalyzers => _lazyCompilationEndAnalyzers;

        private ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim> AnalyzerGateMap => _lazyAnalyzerGateMap;

        private ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> GeneratedCodeAnalysisFlagsMap => _lazyGeneratedCodeAnalysisFlagsMap;

        private ImmutableHashSet<DiagnosticAnalyzer> NonConfigurableAnalyzers => _lazyNonConfigurableAnalyzers;

        private ImmutableHashSet<DiagnosticAnalyzer> SymbolStartAnalyzers => _lazySymbolStartAnalyzers;

        private bool TreatAllCodeAsNonGeneratedCode => _lazyTreatAllCodeAsNonGeneratedCode.Value;

        private ConcurrentDictionary<SyntaxTree, bool> GeneratedCodeFilesMap => _lazyGeneratedCodeFilesMap;

        private Dictionary<SyntaxTree, ImmutableHashSet<ISymbol>> GeneratedCodeSymbolsForTreeMap => _lazyGeneratedCodeSymbolsForTreeMap;

        private ConcurrentDictionary<SyntaxTree, ImmutableHashSet<DiagnosticAnalyzer>> SuppressedAnalyzersForTreeMap => _lazySuppressedAnalyzersForTreeMap;

        private ConcurrentDictionary<ISymbol, bool> IsGeneratedCodeSymbolMap => _lazyIsGeneratedCodeSymbolMap;

        public AsyncQueue<CompilationEvent> CompilationEventQueue => _lazyCompilationEventQueue;

        public DiagnosticQueue DiagnosticQueue => _lazyDiagnosticQueue;

        public bool IsInitialized => _lazyInitializeTask != null;

        public Task WhenInitializedTask => _lazyInitializeTask;

        public Task WhenCompletedTask => _lazyPrimaryTask;

        internal ImmutableDictionary<DiagnosticAnalyzer, TimeSpan> AnalyzerExecutionTimes => AnalyzerExecutor.AnalyzerExecutionTimes;

        protected bool DoNotAnalyzeGeneratedCode => _lazyDoNotAnalyzeGeneratedCode.Value;

        protected abstract IGroupedAnalyzerActions EmptyGroupedActions { get; }

        protected AnalyzerDriver(ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, SeverityFilter severityFilter, Func<SyntaxTrivia, bool> isComment)
        {
            Func<SyntaxTrivia, bool> isComment2 = isComment;
            //base._002Ector();
            Analyzers = analyzers;
            AnalyzerManager = analyzerManager;
            _isGeneratedCode = (SyntaxTree tree, CancellationToken ct) => GeneratedCodeUtilities.IsGeneratedCode(tree, isComment2, ct);
            _severityFilter = severityFilter;
            _hasDiagnosticSuppressors = Analyzers.Any((DiagnosticAnalyzer a) => a is DiagnosticSuppressor);
            _programmaticSuppressions = (_hasDiagnosticSuppressors ? new ConcurrentSet<Suppression>() : null);
            _diagnosticsProcessedForProgrammaticSuppressions = (_hasDiagnosticSuppressors ? new ConcurrentSet<Diagnostic>(ReferenceEqualityComparer.Instance) : null);
            _lazyAnalyzerGateMap = ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim>.Empty;
        }

        private void Initialize(AnalyzerExecutor analyzerExecutor, DiagnosticQueue diagnosticQueue, CompilationData compilationData, CancellationToken cancellationToken)
        {
            AnalyzerExecutor analyzerExecutor2 = analyzerExecutor;
            try
            {
                _lazyAnalyzerExecutor = analyzerExecutor2;
                _lazyCurrentCompilationData = compilationData;
                _lazyDiagnosticQueue = diagnosticQueue;
                _lazyInitializeTask = Task.Run(async delegate
                {
                    (AnalyzerActions, ImmutableHashSet<DiagnosticAnalyzer>) tuple = await GetAnalyzerActionsAsync(Analyzers, AnalyzerManager, analyzerExecutor2, _severityFilter).ConfigureAwait(continueOnCapturedContext: false);
                    _lazyAnalyzerActions = tuple.Item1;
                    _lazyUnsuppressedAnalyzers = tuple.Item2;
                    ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim> lazyAnalyzerGateMap = await CreateAnalyzerGateMapAsync(UnsuppressedAnalyzers, AnalyzerManager, analyzerExecutor2, _severityFilter).ConfigureAwait(continueOnCapturedContext: false);
                    _lazyAnalyzerGateMap = lazyAnalyzerGateMap;
                    _lazyNonConfigurableAnalyzers = ComputeNonConfigurableAnalyzers(UnsuppressedAnalyzers);
                    _lazySymbolStartAnalyzers = ComputeSymbolStartAnalyzers(UnsuppressedAnalyzers);
                    ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> lazyGeneratedCodeAnalysisFlagsMap = await CreateGeneratedCodeAnalysisFlagsMapAsync(UnsuppressedAnalyzers, AnalyzerManager, analyzerExecutor2, _severityFilter).ConfigureAwait(continueOnCapturedContext: false);
                    _lazyGeneratedCodeAnalysisFlagsMap = lazyGeneratedCodeAnalysisFlagsMap;
                    _lazyTreatAllCodeAsNonGeneratedCode = ComputeShouldTreatAllCodeAsNonGeneratedCode(UnsuppressedAnalyzers, GeneratedCodeAnalysisFlagsMap);
                    _lazyDoNotAnalyzeGeneratedCode = ComputeShouldSkipAnalysisOnGeneratedCode(UnsuppressedAnalyzers, GeneratedCodeAnalysisFlagsMap, TreatAllCodeAsNonGeneratedCode);
                    _lazyGeneratedCodeFilesMap = (TreatAllCodeAsNonGeneratedCode ? null : new ConcurrentDictionary<SyntaxTree, bool>());
                    _lazyGeneratedCodeSymbolsForTreeMap = (TreatAllCodeAsNonGeneratedCode ? null : new Dictionary<SyntaxTree, ImmutableHashSet<ISymbol>>());
                    _lazyIsGeneratedCodeSymbolMap = (TreatAllCodeAsNonGeneratedCode ? null : new ConcurrentDictionary<ISymbol, bool>());
                    _lazyTreesWithHiddenRegionsMap = (TreatAllCodeAsNonGeneratedCode ? null : new ConcurrentDictionary<SyntaxTree, bool>());
                    _lazySuppressedAnalyzersForTreeMap = new ConcurrentDictionary<SyntaxTree, ImmutableHashSet<DiagnosticAnalyzer>>();
                    _lazyGeneratedCodeAttribute = analyzerExecutor2.Compilation?.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");
                    _lazySymbolActionsByKind = MakeSymbolActionsByKind(in AnalyzerActions);
                    AnalyzerDriver analyzerDriver = this;
                    ImmutableArray<SemanticModelAnalyzerAction> analyzerActions = AnalyzerActions.SemanticModelActions;
                    analyzerDriver._lazySemanticModelActions = MakeActionsByAnalyzer(in analyzerActions);
                    AnalyzerDriver analyzerDriver2 = this;
                    ImmutableArray<SyntaxTreeAnalyzerAction> analyzerActions2 = AnalyzerActions.SyntaxTreeActions;
                    analyzerDriver2._lazySyntaxTreeActions = MakeActionsByAnalyzer(in analyzerActions2);
                    AnalyzerDriver analyzerDriver3 = this;
                    ImmutableArray<AdditionalFileAnalyzerAction> analyzerActions3 = AnalyzerActions.AdditionalFileActions;
                    analyzerDriver3._lazyAdditionalFileActions = MakeActionsByAnalyzer(in analyzerActions3);
                    AnalyzerDriver analyzerDriver4 = this;
                    ImmutableArray<CompilationAnalyzerAction> analyzerActions4 = AnalyzerActions.CompilationActions;
                    analyzerDriver4._lazyCompilationActions = MakeActionsByAnalyzer(in analyzerActions4);
                    AnalyzerDriver analyzerDriver5 = this;
                    analyzerActions4 = AnalyzerActions.CompilationEndActions;
                    analyzerDriver5._lazyCompilationEndActions = MakeActionsByAnalyzer(in analyzerActions4);
                    _lazyCompilationEndAnalyzers = MakeCompilationEndAnalyzers(_lazyCompilationEndActions);
                    if (AnalyzerActions.SymbolStartActionsCount > 0)
                    {
                        _lazyPerSymbolAnalyzerActionsCache = new ConcurrentDictionary<(INamespaceOrTypeSymbol, DiagnosticAnalyzer), IGroupedAnalyzerActions>();
                    }
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _initializeSucceeded = true;
            }
            finally
            {
                if (_lazyInitializeTask == null)
                {
                    /*_lazyInitializeTask = Task.FromCanceled(new CancellationToken(canceled: true));
					_lazyPrimaryTask = Task.FromCanceled(new CancellationToken(canceled: true));*/
                    // FilRip : Create canceled task
                    _lazyInitializeTask = new Task(new Action(() => { }), new CancellationToken(true));
                    _lazyPrimaryTask = new Task(new Action(() => { }), new CancellationToken(true));
                    // FilRip : End create canceled task
                    DiagnosticQueue.TryComplete();
                }
            }
        }

        internal void Initialize(Compilation compilation, CompilationWithAnalyzersOptions analysisOptions, CompilationData compilationData, bool categorizeDiagnostics, CancellationToken cancellationToken)
        {
            Compilation compilation2 = compilation;
            CompilationWithAnalyzersOptions analysisOptions2 = analysisOptions;
            DiagnosticQueue diagnosticQueue = Microsoft.CodeAnalysis.Diagnostics.DiagnosticQueue.Create(categorizeDiagnostics);
            Action<Diagnostic> addNotCategorizedDiagnostic = null;
            Action<Diagnostic, DiagnosticAnalyzer, bool> addCategorizedLocalDiagnostic = null;
            Action<Diagnostic, DiagnosticAnalyzer> addCategorizedNonLocalDiagnostic = null;
            if (categorizeDiagnostics)
            {
                addCategorizedLocalDiagnostic = GetDiagnosticSink(diagnosticQueue.EnqueueLocal, compilation2, analysisOptions2.Options, _severityFilter, cancellationToken);
                addCategorizedNonLocalDiagnostic = GetDiagnosticSink(diagnosticQueue.EnqueueNonLocal, compilation2, analysisOptions2.Options, _severityFilter, cancellationToken);
            }
            else
            {
                addNotCategorizedDiagnostic = GetDiagnosticSink(diagnosticQueue.Enqueue, compilation2, analysisOptions2.Options, _severityFilter, cancellationToken);
            }
            void onAnalyzerException(Exception ex, DiagnosticAnalyzer analyzer, Diagnostic diagnostic)
            {
                Diagnostic? filteredDiagnostic = GetFilteredDiagnostic(diagnostic, compilation2, analysisOptions2.Options, _severityFilter, cancellationToken);
                if (filteredDiagnostic != null)
                {
                    if (analysisOptions2.OnAnalyzerException != null)
                    {
                        analysisOptions2.OnAnalyzerException!(ex, analyzer, filteredDiagnostic);
                    }
                    else if (categorizeDiagnostics)
                    {
                        addCategorizedNonLocalDiagnostic!(filteredDiagnostic, analyzer);
                    }
                    else
                    {
                        addNotCategorizedDiagnostic!(filteredDiagnostic);
                    }
                }
            }
            AnalyzerExecutor analyzerExecutor = Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.Create(compilation2, analysisOptions2.Options ?? AnalyzerOptions.Empty, addNotCategorizedDiagnostic, onAnalyzerException, analysisOptions2.AnalyzerExceptionFilter, IsCompilerAnalyzer, AnalyzerManager, ShouldSkipAnalysisOnGeneratedCode, ShouldSuppressGeneratedCodeDiagnostic, IsGeneratedOrHiddenCodeLocation, IsAnalyzerSuppressedForTree, GetAnalyzerGate, GetOrCreateSemanticModel, analysisOptions2.LogAnalyzerExecutionTime, addCategorizedLocalDiagnostic, addCategorizedNonLocalDiagnostic, delegate (Suppression s)
            {
                _programmaticSuppressions!.Add(s);
            }, cancellationToken);
            Initialize(analyzerExecutor, diagnosticQueue, compilationData, cancellationToken);
        }

        private SemaphoreSlim? GetAnalyzerGate(DiagnosticAnalyzer analyzer)
        {
            if (AnalyzerGateMap.TryGetValue(analyzer, out var value))
            {
                return value;
            }
            return null;
        }

        private ImmutableHashSet<DiagnosticAnalyzer> ComputeNonConfigurableAnalyzers(ImmutableHashSet<DiagnosticAnalyzer> unsuppressedAnalyzers)
        {
            ImmutableHashSet<DiagnosticAnalyzer>.Builder builder = ImmutableHashSet.CreateBuilder<DiagnosticAnalyzer>();
            foreach (DiagnosticAnalyzer unsuppressedAnalyzer in unsuppressedAnalyzers)
            {
                ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator2 = AnalyzerManager.GetSupportedDiagnosticDescriptors(unsuppressedAnalyzer, AnalyzerExecutor).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.IsNotConfigurable())
                    {
                        builder.Add(unsuppressedAnalyzer);
                        break;
                    }
                }
            }
            return builder.ToImmutableHashSet();
        }

        private ImmutableHashSet<DiagnosticAnalyzer> ComputeSymbolStartAnalyzers(ImmutableHashSet<DiagnosticAnalyzer> unsuppressedAnalyzers)
        {
            ImmutableHashSet<DiagnosticAnalyzer>.Builder builder = ImmutableHashSet.CreateBuilder<DiagnosticAnalyzer>();
            ImmutableArray<SymbolStartAnalyzerAction>.Enumerator enumerator = AnalyzerActions.SymbolStartActions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SymbolStartAnalyzerAction current = enumerator.Current;
                if (unsuppressedAnalyzers.Contains(current.Analyzer))
                {
                    builder.Add(current.Analyzer);
                }
            }
            return builder.ToImmutableHashSet();
        }

        private static bool ComputeShouldSkipAnalysisOnGeneratedCode(ImmutableHashSet<DiagnosticAnalyzer> analyzers, ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> generatedCodeAnalysisFlagsMap, bool treatAllCodeAsNonGeneratedCode)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                if (!ShouldSkipAnalysisOnGeneratedCode(analyzer, generatedCodeAnalysisFlagsMap, treatAllCodeAsNonGeneratedCode))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ComputeShouldTreatAllCodeAsNonGeneratedCode(ImmutableHashSet<DiagnosticAnalyzer> analyzers, ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> generatedCodeAnalysisFlagsMap)
        {
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                GeneratedCodeAnalysisFlags num = generatedCodeAnalysisFlagsMap[analyzer];
                bool flag = (num & GeneratedCodeAnalysisFlags.Analyze) != 0;
                bool flag2 = (num & GeneratedCodeAnalysisFlags.ReportDiagnostics) != 0;
                if (!flag || !flag2)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ShouldSkipAnalysisOnGeneratedCode(DiagnosticAnalyzer analyzer)
        {
            return ShouldSkipAnalysisOnGeneratedCode(analyzer, GeneratedCodeAnalysisFlagsMap, TreatAllCodeAsNonGeneratedCode);
        }

        private static bool ShouldSkipAnalysisOnGeneratedCode(DiagnosticAnalyzer analyzer, ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags> generatedCodeAnalysisFlagsMap, bool treatAllCodeAsNonGeneratedCode)
        {
            if (treatAllCodeAsNonGeneratedCode)
            {
                return false;
            }
            return (generatedCodeAnalysisFlagsMap[analyzer] & GeneratedCodeAnalysisFlags.Analyze) == 0;
        }

        private bool ShouldSuppressGeneratedCodeDiagnostic(Diagnostic diagnostic, DiagnosticAnalyzer analyzer, Compilation compilation, CancellationToken cancellationToken)
        {
            if (TreatAllCodeAsNonGeneratedCode)
            {
                return false;
            }
            if ((GeneratedCodeAnalysisFlagsMap[analyzer] & GeneratedCodeAnalysisFlags.ReportDiagnostics) == 0)
            {
                return IsInGeneratedCode(diagnostic.Location, compilation, cancellationToken);
            }
            return false;
        }

        internal async Task AttachQueueAndProcessAllEventsAsync(AsyncQueue<CompilationEvent> eventQueue, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            try
            {
                if (_initializeSucceeded)
                {
                    _lazyCompilationEventQueue = eventQueue;
                    _lazyQueueRegistration = default(CancellationTokenRegistration);
                    await ExecutePrimaryAnalysisTaskAsync(analysisScope, analysisState, usingPrePopulatedEventQueue: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    _lazyPrimaryTask = Task.FromResult(result: true);
                }
            }
            finally
            {
                if (_lazyPrimaryTask == null)
                {
                    //_lazyPrimaryTask = Task.FromCanceled(new CancellationToken(canceled: true));
                    // FilRip : Create canceled task
                    _lazyPrimaryTask = new Task(new Action(() => { }), new CancellationToken(true));
                }
            }
        }

        internal void AttachQueueAndStartProcessingEvents(AsyncQueue<CompilationEvent> eventQueue, AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            try
            {
                if (_initializeSucceeded)
                {
                    _lazyCompilationEventQueue = eventQueue;
                    _lazyQueueRegistration = cancellationToken.Register(delegate
                    {
                        CompilationEventQueue.TryComplete();
                        DiagnosticQueue.TryComplete();
                    });
                    _lazyPrimaryTask = ExecutePrimaryAnalysisTaskAsync(analysisScope, null, usingPrePopulatedEventQueue: false, cancellationToken).ContinueWith((Task c) => DiagnosticQueue.TryComplete(), cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
            }
            finally
            {
                if (_lazyPrimaryTask == null)
                {
                    //_lazyPrimaryTask = Task.FromCanceled(new CancellationToken(canceled: true));
                    // FilRip : Create canceled task
                    _lazyPrimaryTask = new Task(new Action(() => { }), new CancellationToken(true));
                    DiagnosticQueue.TryComplete();
                }
            }
        }

        private async Task ExecutePrimaryAnalysisTaskAsync(AnalysisScope analysisScope, AnalysisState? analysisState, bool usingPrePopulatedEventQueue, CancellationToken cancellationToken)
        {
            await WhenInitializedTask.ConfigureAwait(continueOnCapturedContext: false);
            if (WhenInitializedTask.IsFaulted)
            {
                OnDriverException(WhenInitializedTask, AnalyzerExecutor, analysisScope.Analyzers);
            }
            else if (!WhenInitializedTask.IsCanceled)
            {
                _lazyAnalyzerExecutor = AnalyzerExecutor.WithCancellationToken(cancellationToken);
                await ProcessCompilationEventsAsync(analysisScope, analysisState, usingPrePopulatedEventQueue, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                _ = usingPrePopulatedEventQueue;
            }
        }

        private static void OnDriverException(Task faultedTask, AnalyzerExecutor analyzerExecutor, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            Exception ex = faultedTask.Exception?.InnerException;
            if (ex != null && !(ex is OperationCanceledException))
            {
                Diagnostic arg = Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.CreateDriverExceptionDiagnostic(ex);
                DiagnosticAnalyzer arg2 = analyzers[0];
                analyzerExecutor.OnAnalyzerException(ex, arg2, arg);
            }
        }

        private void ExecuteSyntaxTreeActions(AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            if (analysisScope.IsSingleFileAnalysis && !analysisScope.IsSyntacticSingleFileAnalysis)
            {
                return;
            }
            foreach (SyntaxTree syntaxTree in analysisScope.SyntaxTrees)
            {
                bool flag = IsGeneratedCode(syntaxTree);
                SourceOrAdditionalFile file = new SourceOrAdditionalFile(syntaxTree);
                if (flag && DoNotAnalyzeGeneratedCode)
                {
                    analysisState?.MarkSyntaxAnalysisComplete(file, analysisScope.Analyzers);
                    continue;
                }
                PooledHashSet<DiagnosticAnalyzer> pooledHashSet = ((analysisState != null) ? PooledHashSet<DiagnosticAnalyzer>.GetInstance() : null);
                try
                {
                    ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<SyntaxTreeAnalyzerAction>)>.Enumerator enumerator2 = _lazySyntaxTreeActions.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        var (diagnosticAnalyzer, syntaxTreeActions) = enumerator2.Current;
                        if (analysisScope.Contains(diagnosticAnalyzer))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            AnalyzerExecutor.TryExecuteSyntaxTreeActions(syntaxTreeActions, diagnosticAnalyzer, file, analysisScope, analysisState, flag);
                            pooledHashSet?.Add(diagnosticAnalyzer);
                        }
                    }
                    analysisState?.MarkSyntaxAnalysisCompleteForUnprocessedAnalyzers(file, analysisScope, pooledHashSet);
                }
                finally
                {
                    pooledHashSet?.Free();
                }
            }
        }

        private void ExecuteAdditionalFileActions(AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            if (analysisScope.IsSingleFileAnalysis && !analysisScope.IsSyntacticSingleFileAnalysis)
            {
                return;
            }
            foreach (AdditionalText additionalFile in analysisScope.AdditionalFiles)
            {
                SourceOrAdditionalFile file = new SourceOrAdditionalFile(additionalFile);
                PooledHashSet<DiagnosticAnalyzer> pooledHashSet = ((analysisState != null) ? PooledHashSet<DiagnosticAnalyzer>.GetInstance() : null);
                try
                {
                    ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<AdditionalFileAnalyzerAction>)>.Enumerator enumerator2 = _lazyAdditionalFileActions.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        var (diagnosticAnalyzer, additionalFileActions) = enumerator2.Current;
                        if (analysisScope.Contains(diagnosticAnalyzer))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            AnalyzerExecutor.TryExecuteAdditionalFileActions(additionalFileActions, diagnosticAnalyzer, file, analysisScope, analysisState);
                            pooledHashSet?.Add(diagnosticAnalyzer);
                        }
                    }
                    analysisState?.MarkSyntaxAnalysisCompleteForUnprocessedAnalyzers(file, analysisScope, pooledHashSet);
                }
                finally
                {
                    pooledHashSet?.Free();
                }
            }
        }

        public static AnalyzerDriver CreateAndAttachToCompilation(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions options, AnalyzerManager analyzerManager, Action<Diagnostic> addExceptionDiagnostic, bool reportAnalyzer, SeverityFilter severityFilter, out Compilation newCompilation, CancellationToken cancellationToken)
        {
            Action<Diagnostic> addExceptionDiagnostic2 = addExceptionDiagnostic;
            void onAnalyzerException(Exception ex, DiagnosticAnalyzer analyzer, Diagnostic diagnostic)
            {
                addExceptionDiagnostic2?.Invoke(diagnostic);
            }
            Func<Exception, bool> analyzerExceptionFilter = null;
            return CreateAndAttachToCompilation(compilation, analyzers, options, analyzerManager, onAnalyzerException, analyzerExceptionFilter, reportAnalyzer, severityFilter, out newCompilation, cancellationToken);
        }

        internal static AnalyzerDriver CreateAndAttachToCompilation(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions options, AnalyzerManager analyzerManager, Action<Exception, DiagnosticAnalyzer, Diagnostic> onAnalyzerException, Func<Exception, bool>? analyzerExceptionFilter, bool reportAnalyzer, SeverityFilter severityFilter, out Compilation newCompilation, CancellationToken cancellationToken)
        {
            AnalyzerDriver analyzerDriver = compilation.CreateAnalyzerDriver(analyzers, analyzerManager, severityFilter);
            newCompilation = compilation.WithSemanticModelProvider(new CachingSemanticModelProvider()).WithEventQueue(new AsyncQueue<CompilationEvent>());
            bool categorizeDiagnostics = false;
            analyzerDriver.Initialize(analysisOptions: new CompilationWithAnalyzersOptions(options, onAnalyzerException, concurrentAnalysis: true, reportAnalyzer, reportSuppressedDiagnostics: false, analyzerExceptionFilter), compilation: newCompilation, compilationData: new CompilationData(newCompilation), categorizeDiagnostics: categorizeDiagnostics, cancellationToken: cancellationToken);
            analyzerDriver.AttachQueueAndStartProcessingEvents(analysisScope: new AnalysisScope(newCompilation, options, analyzers, hasAllAnalyzers: true, newCompilation.Options.ConcurrentBuild, categorizeDiagnostics), eventQueue: newCompilation.EventQueue, cancellationToken: cancellationToken);
            return analyzerDriver;
        }

        public async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(Compilation compilation)
        {
            DiagnosticBag allDiagnostics = DiagnosticBag.GetInstance();
            if (CompilationEventQueue.IsCompleted)
            {
                await WhenCompletedTask.ConfigureAwait(continueOnCapturedContext: false);
                if (WhenCompletedTask.IsFaulted)
                {
                    OnDriverException(WhenCompletedTask, AnalyzerExecutor, Analyzers);
                }
            }
            SuppressMessageAttributeState suppressMessageAttributeState = CurrentCompilationData.SuppressMessageAttributeState;
            bool reportSuppressedDiagnostics = compilation.Options.ReportSuppressedDiagnostics;
            while (DiagnosticQueue.TryDequeue(out Diagnostic d))
            {
                d = suppressMessageAttributeState.ApplySourceSuppressions(d);
                if (reportSuppressedDiagnostics || !d.IsSuppressed)
                {
                    allDiagnostics.Add(d);
                }
            }
            return allDiagnostics.ToReadOnlyAndFree();
        }

        private SemanticModel GetOrCreateSemanticModel(SyntaxTree tree)
        {
            return GetOrCreateSemanticModel(tree, AnalyzerExecutor.Compilation);
        }

        private SemanticModel GetOrCreateSemanticModel(SyntaxTree tree, Compilation compilation)
        {
            return SemanticModelProvider.GetSemanticModel(tree, compilation);
        }

        public void ApplyProgrammaticSuppressions(DiagnosticBag reportedDiagnostics, Compilation compilation)
        {
            if (_hasDiagnosticSuppressors)
            {
                ImmutableArray<Diagnostic> diagnostics = ApplyProgrammaticSuppressionsCore(reportedDiagnostics.ToReadOnly(), compilation);
                reportedDiagnostics.Clear();
                reportedDiagnostics.AddRange(diagnostics);
            }
        }

        public ImmutableArray<Diagnostic> ApplyProgrammaticSuppressions(ImmutableArray<Diagnostic> reportedDiagnostics, Compilation compilation)
        {
            if (reportedDiagnostics.IsEmpty || !_hasDiagnosticSuppressors)
            {
                return reportedDiagnostics;
            }
            return ApplyProgrammaticSuppressionsCore(reportedDiagnostics, compilation);
        }

        private ImmutableArray<Diagnostic> ApplyProgrammaticSuppressionsCore(ImmutableArray<Diagnostic> reportedDiagnostics, Compilation compilation)
        {
            try
            {
                IEnumerable<Diagnostic> enumerable = reportedDiagnostics.Where((Diagnostic d) => !d.IsSuppressed && !d.IsNotConfigurable() && d.DefaultSeverity != DiagnosticSeverity.Error && !_diagnosticsProcessedForProgrammaticSuppressions!.Contains(d));
                if (enumerable.IsEmpty())
                {
                    return reportedDiagnostics;
                }
                executeSuppressionActions(enumerable, compilation.Options.ConcurrentBuild);
                if (_programmaticSuppressions!.IsEmpty)
                {
                    return reportedDiagnostics;
                }
                ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance(reportedDiagnostics.Length);
                ImmutableDictionary<Diagnostic, ProgrammaticSuppressionInfo> immutableDictionary = createProgrammaticSuppressionsByDiagnosticMap(_programmaticSuppressions);
                ImmutableArray<Diagnostic>.Enumerator enumerator = reportedDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Diagnostic current = enumerator.Current;
                    if (immutableDictionary.TryGetValue(current, out var value))
                    {
                        Diagnostic item = current.WithProgrammaticSuppression(value);
                        instance.Add(item);
                    }
                    else
                    {
                        instance.Add(current);
                    }
                }
                return instance.ToImmutableAndFree();
            }
            finally
            {
                _diagnosticsProcessedForProgrammaticSuppressions!.AddRange(reportedDiagnostics);
            }
            static ImmutableDictionary<Diagnostic, ProgrammaticSuppressionInfo> createProgrammaticSuppressionsByDiagnosticMap(ConcurrentSet<Suppression> programmaticSuppressions)
            {
                PooledDictionary<Diagnostic, ImmutableHashSet<(string, LocalizableString)>.Builder> instance2 = PooledDictionary<Diagnostic, ImmutableHashSet<(string, LocalizableString)>.Builder>.GetInstance();
                ConcurrentSet<Suppression>.KeyEnumerator enumerator2 = programmaticSuppressions.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Suppression current2 = enumerator2.Current;
                    if (!instance2.TryGetValue(current2.SuppressedDiagnostic, out var value2))
                    {
                        value2 = ImmutableHashSet.CreateBuilder<(string, LocalizableString)>();
                        instance2.Add(current2.SuppressedDiagnostic, value2);
                    }
                    value2.Add((current2.Descriptor.Id, current2.Descriptor.Justification));
                }
                ImmutableDictionary<Diagnostic, ProgrammaticSuppressionInfo>.Builder builder = ImmutableDictionary.CreateBuilder<Diagnostic, ProgrammaticSuppressionInfo>();
                foreach (var (key, builder3) in instance2)
                {
                    builder.Add(key, new ProgrammaticSuppressionInfo(builder3.ToImmutable()));
                }
                return builder.ToImmutable();
            }
            void executeSuppressionActions(IEnumerable<Diagnostic> reportedDiagnostics, bool concurrent)
            {
                IEnumerable<Diagnostic> reportedDiagnostics2 = reportedDiagnostics;
                IEnumerable<DiagnosticSuppressor> enumerable2 = Analyzers.OfType<DiagnosticSuppressor>();
                if (concurrent)
                {
                    ArrayBuilder<Task> instance3 = ArrayBuilder<Task>.GetInstance();
                    try
                    {
                        foreach (DiagnosticSuppressor suppressor2 in enumerable2)
                        {
                            Task item2 = Task.Run(delegate
                            {
                                AnalyzerExecutor.ExecuteSuppressionAction(suppressor2, getSuppressableDiagnostics(suppressor2));
                            }, AnalyzerExecutor.CancellationToken);
                            instance3.Add(item2);
                        }
                        Task.WaitAll(instance3.ToArray(), AnalyzerExecutor.CancellationToken);
                        return;
                    }
                    finally
                    {
                        instance3.Free();
                    }
                }
                foreach (DiagnosticSuppressor item3 in enumerable2)
                {
                    AnalyzerExecutor.ExecuteSuppressionAction(item3, getSuppressableDiagnostics(item3));
                }
                ImmutableArray<Diagnostic> getSuppressableDiagnostics(DiagnosticSuppressor suppressor)
                {
                    ImmutableArray<SuppressionDescriptor> supportedSuppressionDescriptors = AnalyzerManager.GetSupportedSuppressionDescriptors(suppressor, AnalyzerExecutor);
                    if (supportedSuppressionDescriptors.IsEmpty)
                    {
                        return ImmutableArray<Diagnostic>.Empty;
                    }
                    ArrayBuilder<Diagnostic> instance4 = ArrayBuilder<Diagnostic>.GetInstance();
                    foreach (Diagnostic diagnostic2 in reportedDiagnostics2)
                    {
                        if (supportedSuppressionDescriptors.Contains((SuppressionDescriptor s) => s.SuppressedDiagnosticId == diagnostic2.Id))
                        {
                            instance4.Add(diagnostic2);
                        }
                    }
                    return instance4.ToImmutableAndFree();
                }
            }
        }

        public ImmutableArray<Diagnostic> DequeueLocalDiagnosticsAndApplySuppressions(DiagnosticAnalyzer analyzer, bool syntax, Compilation compilation)
        {
            ImmutableArray<Diagnostic> diagnostics = (syntax ? DiagnosticQueue.DequeueLocalSyntaxDiagnostics(analyzer) : DiagnosticQueue.DequeueLocalSemanticDiagnostics(analyzer));
            return FilterDiagnosticsSuppressedInSourceOrByAnalyzers(diagnostics, compilation);
        }

        public ImmutableArray<Diagnostic> DequeueNonLocalDiagnosticsAndApplySuppressions(DiagnosticAnalyzer analyzer, Compilation compilation)
        {
            ImmutableArray<Diagnostic> diagnostics = DiagnosticQueue.DequeueNonLocalDiagnostics(analyzer);
            return FilterDiagnosticsSuppressedInSourceOrByAnalyzers(diagnostics, compilation);
        }

        private ImmutableArray<Diagnostic> FilterDiagnosticsSuppressedInSourceOrByAnalyzers(ImmutableArray<Diagnostic> diagnostics, Compilation compilation)
        {
            diagnostics = FilterDiagnosticsSuppressedInSource(diagnostics, compilation, CurrentCompilationData.SuppressMessageAttributeState);
            return ApplyProgrammaticSuppressions(diagnostics, compilation);
        }

        private static ImmutableArray<Diagnostic> FilterDiagnosticsSuppressedInSource(ImmutableArray<Diagnostic> diagnostics, Compilation compilation, SuppressMessageAttributeState suppressMessageState)
        {
            if (diagnostics.IsEmpty)
            {
                return diagnostics;
            }
            bool reportSuppressedDiagnostics = compilation.Options.ReportSuppressedDiagnostics;
            ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();
            for (int i = 0; i < diagnostics.Length; i++)
            {
                Diagnostic diagnostic = suppressMessageState.ApplySourceSuppressions(diagnostics[i]);
                if (reportSuppressedDiagnostics || !diagnostic.IsSuppressed)
                {
                    builder.Add(diagnostic);
                }
            }
            return builder.ToImmutable();
        }

        private bool IsInGeneratedCode(Location location, Compilation compilation, CancellationToken cancellationToken)
        {
            if (TreatAllCodeAsNonGeneratedCode || !location.IsInSource)
            {
                return false;
            }
            if (IsGeneratedOrHiddenCodeLocation(location.SourceTree, location.SourceSpan))
            {
                return true;
            }
            if (_lazyGeneratedCodeAttribute != null)
            {
                ImmutableHashSet<ISymbol> immutableHashSet = getOrComputeGeneratedCodeSymbolsInTree(location.SourceTree, compilation, cancellationToken);
                if (immutableHashSet.Count > 0)
                {
                    SemanticModel semanticModel = compilation.GetSemanticModel(location.SourceTree);
                    for (SyntaxNode syntaxNode = location.SourceTree!.GetRoot(cancellationToken).FindNode(location.SourceSpan, findInsideTrivia: false, getInnermostNodeForTie: true); syntaxNode != null; syntaxNode = syntaxNode.Parent)
                    {
                        ImmutableArray<ISymbol>.Enumerator enumerator = semanticModel.GetDeclaredSymbolsForNode(syntaxNode, cancellationToken).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ISymbol current = enumerator.Current;
                            if (immutableHashSet.Contains(current))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
            static ImmutableHashSet<ISymbol> computeGeneratedCodeSymbolsInTree(SyntaxTree tree, Compilation compilation, INamedTypeSymbol generatedCodeAttribute, CancellationToken cancellationToken)
            {
                GeneratedCodeTokenWalker generatedCodeTokenWalker = new GeneratedCodeTokenWalker(cancellationToken);
                generatedCodeTokenWalker.Visit(tree.GetRoot(cancellationToken));
                if (!generatedCodeTokenWalker.HasGeneratedCodeIdentifier)
                {
                    return ImmutableHashSet<ISymbol>.Empty;
                }
                SemanticModel semanticModel2 = compilation.GetSemanticModel(tree);
                TextSpan fullSpan = tree.GetRoot(cancellationToken).FullSpan;
                ArrayBuilder<DeclarationInfo> instance = ArrayBuilder<DeclarationInfo>.GetInstance();
                semanticModel2.ComputeDeclarationsInSpan(fullSpan, getSymbol: true, instance, cancellationToken);
                ImmutableHashSet<ISymbol>.Builder builder = null;
                ArrayBuilder<DeclarationInfo>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ISymbol declaredSymbol = enumerator2.Current.DeclaredSymbol;
                    if (declaredSymbol != null && GeneratedCodeUtilities.IsGeneratedSymbolWithGeneratedCodeAttribute(declaredSymbol, generatedCodeAttribute))
                    {
                        if (builder == null)
                        {
                            builder = ImmutableHashSet.CreateBuilder<ISymbol>();
                        }
                        builder.Add(declaredSymbol);
                    }
                }
                instance.Free();
                if (builder == null)
                {
                    return ImmutableHashSet<ISymbol>.Empty;
                }
                return builder.ToImmutable();
            }
            ImmutableHashSet<ISymbol> getOrComputeGeneratedCodeSymbolsInTree(SyntaxTree tree, Compilation compilation, CancellationToken cancellationToken)
            {
                ImmutableHashSet<ISymbol> value;
                lock (GeneratedCodeSymbolsForTreeMap)
                {
                    if (GeneratedCodeSymbolsForTreeMap.TryGetValue(tree, out value))
                    {
                        return value;
                    }
                }
                value = computeGeneratedCodeSymbolsInTree(tree, compilation, _lazyGeneratedCodeAttribute, cancellationToken);
                lock (GeneratedCodeSymbolsForTreeMap)
                {
                    if (GeneratedCodeSymbolsForTreeMap.TryGetValue(tree, out var _))
                    {
                        return value;
                    }
                    GeneratedCodeSymbolsForTreeMap.Add(tree, value);
                    return value;
                }
            }
        }

        private bool IsAnalyzerSuppressedForTree(DiagnosticAnalyzer analyzer, SyntaxTree tree, SyntaxTreeOptionsProvider? options)
        {
            if (!SuppressedAnalyzersForTreeMap.TryGetValue(tree, out var value))
            {
                value = SuppressedAnalyzersForTreeMap.GetOrAdd(tree, ComputeSuppressedAnalyzersForTree(tree, options));
            }
            return value.Contains(analyzer);
        }

        private ImmutableHashSet<DiagnosticAnalyzer> ComputeSuppressedAnalyzersForTree(SyntaxTree tree, SyntaxTreeOptionsProvider? options)
        {
            if (options == null)
            {
                return ImmutableHashSet<DiagnosticAnalyzer>.Empty;
            }
            ImmutableHashSet<DiagnosticAnalyzer>.Builder builder = null;
            foreach (DiagnosticAnalyzer unsuppressedAnalyzer in UnsuppressedAnalyzers)
            {
                if (NonConfigurableAnalyzers.Contains(unsuppressedAnalyzer) || ((SymbolStartAnalyzers.Contains(unsuppressedAnalyzer) || CompilationEndAnalyzers.Contains(unsuppressedAnalyzer)) && !ShouldSkipAnalysisOnGeneratedCode(unsuppressedAnalyzer)))
                {
                    continue;
                }
                ImmutableArray<DiagnosticDescriptor> supportedDiagnosticDescriptors = AnalyzerManager.GetSupportedDiagnosticDescriptors(unsuppressedAnalyzer, AnalyzerExecutor);
                bool flag = false;
                ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator2 = supportedDiagnosticDescriptors.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    DiagnosticDescriptor current2 = enumerator2.Current;
                    options!.TryGetGlobalDiagnosticValue(current2.Id, AnalyzerExecutor.CancellationToken, out var severity);
                    if (options!.TryGetDiagnosticValue(tree, current2.Id, AnalyzerExecutor.CancellationToken, out var severity2))
                    {
                        severity = severity2;
                    }
                    if (severity != ReportDiagnostic.Suppress)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    if (builder == null)
                    {
                        builder = ImmutableHashSet.CreateBuilder<DiagnosticAnalyzer>();
                    }
                    builder.Add(unsuppressedAnalyzer);
                }
            }
            if (builder == null)
            {
                return ImmutableHashSet<DiagnosticAnalyzer>.Empty;
            }
            return builder.ToImmutable();
        }

        internal TimeSpan ResetAnalyzerExecutionTime(DiagnosticAnalyzer analyzer)
        {
            return AnalyzerExecutor.ResetAnalyzerExecutionTime(analyzer);
        }

        private static ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<ImmutableArray<SymbolAnalyzerAction>>)> MakeSymbolActionsByKind(in AnalyzerActions analyzerActions)
        {
            ArrayBuilder<(DiagnosticAnalyzer, ImmutableArray<ImmutableArray<SymbolAnalyzerAction>>)> instance = ArrayBuilder<(DiagnosticAnalyzer, ImmutableArray<ImmutableArray<SymbolAnalyzerAction>>)>.GetInstance();
            IEnumerable<IGrouping<DiagnosticAnalyzer, SymbolAnalyzerAction>> enumerable = from action in analyzerActions.SymbolActions
                                                                                          group action by action.Analyzer;
            ArrayBuilder<ArrayBuilder<SymbolAnalyzerAction>> instance2 = ArrayBuilder<ArrayBuilder<SymbolAnalyzerAction>>.GetInstance();
            foreach (IGrouping<DiagnosticAnalyzer, SymbolAnalyzerAction> item2 in enumerable)
            {
                instance2.Clear();
                foreach (SymbolAnalyzerAction item3 in item2)
                {
                    ImmutableArray<SymbolKind>.Enumerator enumerator3 = item3.Kinds.Distinct().GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        int current3 = (int)enumerator3.Current;
                        if (current3 <= 100)
                        {
                            while (current3 >= instance2.Count)
                            {
                                instance2.Add(ArrayBuilder<SymbolAnalyzerAction>.GetInstance());
                            }
                            instance2[current3].Add(item3);
                        }
                    }
                }
                ImmutableArray<ImmutableArray<SymbolAnalyzerAction>> item = instance2.Select((ArrayBuilder<SymbolAnalyzerAction> a) => a.ToImmutableAndFree()).ToImmutableArray();
                instance.Add((item2.Key, item));
            }
            instance2.Free();
            return instance.ToImmutableAndFree();
        }

        private static ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<TAnalyzerAction>)> MakeActionsByAnalyzer<TAnalyzerAction>(in ImmutableArray<TAnalyzerAction> analyzerActions) where TAnalyzerAction : AnalyzerAction
        {
            ArrayBuilder<(DiagnosticAnalyzer, ImmutableArray<TAnalyzerAction>)> instance = ArrayBuilder<(DiagnosticAnalyzer, ImmutableArray<TAnalyzerAction>)>.GetInstance();
            foreach (IGrouping<DiagnosticAnalyzer, TAnalyzerAction> item in from action in analyzerActions
                                                                            group action by action.Analyzer)
            {
                instance.Add((item.Key, item.ToImmutableArray()));
            }
            return instance.ToImmutableAndFree();
        }

        private static ImmutableHashSet<DiagnosticAnalyzer> MakeCompilationEndAnalyzers(ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)> compilationEndActionsByAnalyzer)
        {
            ImmutableHashSet<DiagnosticAnalyzer>.Builder builder = ImmutableHashSet.CreateBuilder<DiagnosticAnalyzer>();
            ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)>.Enumerator enumerator = compilationEndActionsByAnalyzer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer item = enumerator.Current.Item1;
                builder.Add(item);
            }
            return builder.ToImmutable();
        }

        private async Task ProcessCompilationEventsAsync(AnalysisScope analysisScope, AnalysisState? analysisState, bool prePopulatedEventQueue, CancellationToken cancellationToken)
        {
            AnalysisScope analysisScope2 = analysisScope;
            AnalysisState analysisState2 = analysisState;
            try
            {
                CompilationCompletedEvent completedEvent = null;
                if (analysisScope2.ConcurrentAnalysis)
                {
                    int workerCount = (prePopulatedEventQueue ? Math.Min(CompilationEventQueue.Count, _workerCount) : _workerCount);
                    Task<CompilationCompletedEvent?>[] workerTasks = new Task<CompilationCompletedEvent>[workerCount];
                    for (int i = 0; i < workerCount; i++)
                    {
                        workerTasks[i] = Task.Run(async () => await ProcessCompilationEventsCoreAsync(analysisScope2, analysisState2, prePopulatedEventQueue, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    Task value = Task.Run(delegate
                    {
                        ExecuteSyntaxTreeActions(analysisScope2, analysisState2, cancellationToken);
                    }, cancellationToken);
                    Task value2 = Task.Run(delegate
                    {
                        ExecuteAdditionalFileActions(analysisScope2, analysisState2, cancellationToken);
                    }, cancellationToken);
                    await Task.WhenAll(workerTasks.Concat(value).Concat(value2)).ConfigureAwait(continueOnCapturedContext: false);
                    for (int j = 0; j < workerCount; j++)
                    {
                        if (workerTasks[j].Status == TaskStatus.RanToCompletion && workerTasks[j].Result != null)
                        {
                            completedEvent = workerTasks[j].Result;
                            break;
                        }
                    }
                }
                else
                {
                    completedEvent = await ProcessCompilationEventsCoreAsync(analysisScope2, analysisState2, prePopulatedEventQueue, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    ExecuteSyntaxTreeActions(analysisScope2, analysisState2, cancellationToken);
                    ExecuteAdditionalFileActions(analysisScope2, analysisState2, cancellationToken);
                }
                if (completedEvent != null)
                {
                    await ProcessEventAsync(completedEvent, analysisScope2, analysisState2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private async Task<CompilationCompletedEvent?> ProcessCompilationEventsCoreAsync(AnalysisScope analysisScope, AnalysisState? analysisState, bool prePopulatedEventQueue, CancellationToken cancellationToken)
        {
            _ = 1;
            try
            {
                CompilationCompletedEvent completedEvent = null;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if ((prePopulatedEventQueue || CompilationEventQueue.IsCompleted) && CompilationEventQueue.Count == 0)
                    {
                        break;
                    }
                    if (!CompilationEventQueue.TryDequeue(out var d))
                    {
                        if (prePopulatedEventQueue)
                        {
                            return completedEvent;
                        }
                        Optional<CompilationEvent> optional = await CompilationEventQueue.TryDequeueAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                        if (!optional.HasValue)
                        {
                            break;
                        }
                        d = optional.Value;
                    }
                    if (!(d is CompilationCompletedEvent compilationCompletedEvent))
                    {
                        await ProcessEventAsync(d, analysisScope, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        completedEvent = compilationCompletedEvent;
                    }
                }
                return completedEvent;
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private async Task ProcessEventAsync(CompilationEvent e, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            EventProcessedState eventProcessedState = await TryProcessEventCoreAsync(e, analysisScope, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            ImmutableArray<DiagnosticAnalyzer> processedAnalyzers;
            switch (eventProcessedState.Kind)
            {
                default:
                    return;
                case EventProcessedStateKind.Processed:
                    processedAnalyzers = analysisScope.Analyzers;
                    break;
                case EventProcessedStateKind.PartiallyProcessed:
                    processedAnalyzers = eventProcessedState.SubsetProcessedAnalyzers;
                    break;
            }
            await OnEventProcessedCoreAsync(e, processedAnalyzers, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task OnEventProcessedCoreAsync(CompilationEvent compilationEvent, ImmutableArray<DiagnosticAnalyzer> processedAnalyzers, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            AnalysisState analysisState2 = analysisState;
            if (analysisState2 != null)
            {
                await analysisState2.OnCompilationEventProcessedAsync(compilationEvent, processedAnalyzers, onSymbolAndMembersProcessedAsync).ConfigureAwait(continueOnCapturedContext: false);
            }
            else if (!(compilationEvent is SymbolDeclaredCompilationEvent symbolDeclaredEvent))
            {
                if (!(compilationEvent is CompilationUnitCompletedEvent compilationUnitCompletedEvent))
                {
                    if (compilationEvent is CompilationCompletedEvent compilationCompletedEvent)
                    {
                        SemanticModelProvider.ClearCache(compilationCompletedEvent.Compilation);
                    }
                }
                else
                {
                    SemanticModelProvider.ClearCache(compilationUnitCompletedEvent.CompilationUnit, compilationUnitCompletedEvent.Compilation);
                }
            }
            else if (AnalyzerActions.SymbolStartActionsCount > 0)
            {
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = processedAnalyzers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticAnalyzer current = enumerator.Current;
                    await onSymbolAndMembersProcessedAsync(symbolDeclaredEvent.Symbol, current).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            async Task onSymbolAndMembersProcessedAsync(ISymbol symbol, DiagnosticAnalyzer analyzer)
            {
                if (AnalyzerActions.SymbolStartActionsCount != 0 && !symbol.IsImplicitlyDeclared)
                {
                    if (symbol is INamespaceOrTypeSymbol item)
                    {
                        PerSymbolAnalyzerActionsCache.TryRemove((item, analyzer), out var _);
                    }
                    await processContainerOnMemberCompletedAsync(symbol.ContainingNamespace, symbol, analyzer).ConfigureAwait(continueOnCapturedContext: false);
                    await processContainerOnMemberCompletedAsync(symbol.ContainingType, symbol, analyzer).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            async Task processContainerOnMemberCompletedAsync(INamespaceOrTypeSymbol containerSymbol, ISymbol processedMemberSymbol, DiagnosticAnalyzer analyzer)
            {
                if (containerSymbol != null && AnalyzerExecutor.TryExecuteSymbolEndActionsForContainer(containerSymbol, processedMemberSymbol, analyzer, s_getTopmostNodeForAnalysis, analysisState2, out var containingSymbolDeclaredEvent))
                {
                    await OnEventProcessedCoreAsync(containingSymbolDeclaredEvent, ImmutableArray.Create(analyzer), analysisState2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
        }

        private async ValueTask<EventProcessedState> TryProcessEventCoreAsync(CompilationEvent compilationEvent, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EventProcessedState result;
            if (!(compilationEvent is SymbolDeclaredCompilationEvent symbolEvent))
            {
                if (!(compilationEvent is CompilationUnitCompletedEvent completedEvent))
                {
                    if (!(compilationEvent is CompilationCompletedEvent endEvent))
                    {
                        if (!(compilationEvent is CompilationStartedEvent startedEvent))
                        {
                            throw new InvalidOperationException("Unexpected compilation event of type " + compilationEvent.GetType().Name);
                        }
                        result = (TryProcessCompilationStarted(startedEvent, analysisScope, analysisState) ? EventProcessedState.Processed : EventProcessedState.NotProcessed);
                    }
                    else
                    {
                        result = (TryProcessCompilationCompleted(endEvent, analysisScope, analysisState) ? EventProcessedState.Processed : EventProcessedState.NotProcessed);
                    }
                }
                else
                {
                    result = (TryProcessCompilationUnitCompleted(completedEvent, analysisScope, analysisState, cancellationToken) ? EventProcessedState.Processed : EventProcessedState.NotProcessed);
                }
            }
            else
            {
                result = await TryProcessSymbolDeclaredAsync(symbolEvent, analysisScope, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            return result;
        }

        private async ValueTask<EventProcessedState> TryProcessSymbolDeclaredAsync(SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            EventProcessedState processedState = EventProcessedState.Processed;
            ISymbol symbol = symbolEvent.Symbol;
            bool isGeneratedCodeSymbol = IsGeneratedCodeSymbol(symbol);
            bool skipSymbolAnalysis = AnalysisScope.ShouldSkipSymbolAnalysis(symbolEvent);
            bool skipDeclarationAnalysis = AnalysisScope.ShouldSkipDeclarationAnalysis(symbol);
            bool hasPerSymbolActions = AnalyzerActions.SymbolStartActionsCount > 0 && (!skipSymbolAnalysis || !skipDeclarationAnalysis);
            IGroupedAnalyzerActions groupedAnalyzerActions = ((!hasPerSymbolActions) ? EmptyGroupedActions : (await GetPerSymbolAnalyzerActionsAsync(symbol, analysisScope, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
            IGroupedAnalyzerActions groupedAnalyzerActions2 = groupedAnalyzerActions;
            if (!skipSymbolAnalysis && !TryExecuteSymbolActions(symbolEvent, analysisScope, analysisState, isGeneratedCodeSymbol, cancellationToken))
            {
                processedState = EventProcessedState.NotProcessed;
            }
            if (!skipDeclarationAnalysis && !TryExecuteDeclaringReferenceActions(symbolEvent, analysisScope, analysisState, isGeneratedCodeSymbol, groupedAnalyzerActions2, cancellationToken))
            {
                processedState = EventProcessedState.NotProcessed;
            }
            if (processedState.Kind == EventProcessedStateKind.Processed && hasPerSymbolActions)
            {
                AnalyzerDriver analyzerDriver = this;
                AnalyzerActions perSymbolActions = groupedAnalyzerActions2.AnalyzerActions;
                if (!analyzerDriver.TryExecuteSymbolEndActions(in perSymbolActions, symbolEvent, analysisScope, analysisState, out var subsetProcessedAnalyzers))
                {
                    processedState = (subsetProcessedAnalyzers.IsEmpty ? EventProcessedState.NotProcessed : EventProcessedState.CreatePartiallyProcessed(subsetProcessedAnalyzers));
                }
            }
            return processedState;
        }

        private bool TryExecuteSymbolActions(SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCodeSymbol, CancellationToken cancellationToken)
        {
            ISymbol symbol = symbolEvent.Symbol;
            if (!analysisScope.ShouldAnalyze(symbol))
            {
                return true;
            }
            PooledHashSet<DiagnosticAnalyzer> pooledHashSet = ((analysisState != null) ? PooledHashSet<DiagnosticAnalyzer>.GetInstance() : null);
            try
            {
                bool result = true;
                ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<ImmutableArray<SymbolAnalyzerAction>>)>.Enumerator enumerator = _lazySymbolActionsByKind.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var (diagnosticAnalyzer, immutableArray) = enumerator.Current;
                    if (analysisScope.Contains(diagnosticAnalyzer) && (int)symbol.Kind < immutableArray.Length)
                    {
                        if (!AnalyzerExecutor.TryExecuteSymbolActions(immutableArray[(int)symbol.Kind], diagnosticAnalyzer, symbolEvent, s_getTopmostNodeForAnalysis, analysisScope, analysisState, isGeneratedCodeSymbol))
                        {
                            result = false;
                        }
                        pooledHashSet?.Add(diagnosticAnalyzer);
                    }
                }
                analysisState?.MarkSymbolCompleteForUnprocessedAnalyzers(symbol, analysisScope, pooledHashSet);
                return result;
            }
            finally
            {
                pooledHashSet?.Free();
            }
        }

        private bool TryExecuteSymbolEndActions(in AnalyzerActions perSymbolActions, SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, out ImmutableArray<DiagnosticAnalyzer> subsetProcessedAnalyzers)
        {
            ISymbol symbol = symbolEvent.Symbol;
            ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions = perSymbolActions.SymbolEndActions;
            if (!analysisScope.ShouldAnalyze(symbol) || symbolEndActions.IsEmpty)
            {
                analysisState?.MarkSymbolEndAnalysisComplete(symbol, analysisScope.Analyzers);
                subsetProcessedAnalyzers = ImmutableArray<DiagnosticAnalyzer>.Empty;
                return true;
            }
            bool flag = true;
            ArrayBuilder<DiagnosticAnalyzer> instance = ArrayBuilder<DiagnosticAnalyzer>.GetInstance();
            PooledHashSet<DiagnosticAnalyzer> instance2 = PooledHashSet<DiagnosticAnalyzer>.GetInstance();
            try
            {
                foreach (IGrouping<DiagnosticAnalyzer, SymbolEndAnalyzerAction> item in from a in symbolEndActions
                                                                                        group a by a.Analyzer)
                {
                    DiagnosticAnalyzer key = item.Key;
                    if (analysisScope.Contains(key))
                    {
                        instance2.Add(key);
                        ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions2 = item.ToImmutableArrayOrEmpty();
                        if (!symbolEndActions2.IsEmpty && !AnalyzerExecutor.TryExecuteSymbolEndActions(symbolEndActions2, key, symbolEvent, s_getTopmostNodeForAnalysis, analysisState))
                        {
                            flag = false;
                            continue;
                        }
                        AnalyzerExecutor.MarkSymbolEndAnalysisComplete(symbol, key, analysisState);
                        instance.Add(key);
                    }
                }
                if (instance2.Count < analysisScope.Analyzers.Length)
                {
                    ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator2 = analysisScope.Analyzers.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        DiagnosticAnalyzer current2 = enumerator2.Current;
                        if (!instance2.Contains(current2))
                        {
                            AnalyzerExecutor.MarkSymbolEndAnalysisComplete(symbol, current2, analysisState);
                            instance.Add(current2);
                        }
                    }
                }
                if (!flag)
                {
                    subsetProcessedAnalyzers = instance.ToImmutable();
                    return false;
                }
                subsetProcessedAnalyzers = ImmutableArray<DiagnosticAnalyzer>.Empty;
                return true;
            }
            finally
            {
                instance2.Free();
                instance.Free();
            }
        }

        private static SyntaxNode GetTopmostNodeForAnalysis(ISymbol symbol, SyntaxReference syntaxReference, Compilation compilation, CancellationToken cancellationToken)
        {
            return compilation.GetSemanticModel(syntaxReference.SyntaxTree).GetTopmostNodeForDiagnosticAnalysis(symbol, syntaxReference.GetSyntax(cancellationToken));
        }

        protected abstract bool TryExecuteDeclaringReferenceActions(SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCodeSymbol, IGroupedAnalyzerActions additionalPerSymbolActions, CancellationToken cancellationToken);

        private bool TryProcessCompilationUnitCompleted(CompilationUnitCompletedEvent completedEvent, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = SemanticModelProvider.GetSemanticModel(completedEvent.CompilationUnit, completedEvent.Compilation);
            if (!analysisScope.ShouldAnalyze(semanticModel.SyntaxTree))
            {
                return true;
            }
            bool flag = IsGeneratedCode(semanticModel.SyntaxTree);
            if (flag && DoNotAnalyzeGeneratedCode)
            {
                analysisState?.MarkEventComplete(completedEvent, analysisScope.Analyzers);
                return true;
            }
            PooledHashSet<DiagnosticAnalyzer> pooledHashSet = ((analysisState != null) ? PooledHashSet<DiagnosticAnalyzer>.GetInstance() : null);
            try
            {
                bool result = true;
                ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<SemanticModelAnalyzerAction>)>.Enumerator enumerator = _lazySemanticModelActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var (diagnosticAnalyzer, semanticModelActions) = enumerator.Current;
                    if (analysisScope.Contains(diagnosticAnalyzer))
                    {
                        if (!AnalyzerExecutor.TryExecuteSemanticModelActions(semanticModelActions, diagnosticAnalyzer, semanticModel, completedEvent, analysisScope, analysisState, flag))
                        {
                            result = false;
                        }
                        pooledHashSet?.Add(diagnosticAnalyzer);
                    }
                }
                analysisState?.MarkEventCompleteForUnprocessedAnalyzers(completedEvent, analysisScope, pooledHashSet);
                return result;
            }
            finally
            {
                pooledHashSet?.Free();
            }
        }

        private bool TryProcessCompilationStarted(CompilationStartedEvent startedEvent, AnalysisScope analysisScope, AnalysisState? analysisState)
        {
            return TryExecuteCompilationActions(_lazyCompilationActions, startedEvent, analysisScope, analysisState);
        }

        private bool TryProcessCompilationCompleted(CompilationCompletedEvent endEvent, AnalysisScope analysisScope, AnalysisState? analysisState)
        {
            return TryExecuteCompilationActions(_lazyCompilationEndActions, endEvent, analysisScope, analysisState);
        }

        private bool TryExecuteCompilationActions(ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)> compilationActionsMap, CompilationEvent compilationEvent, AnalysisScope analysisScope, AnalysisState? analysisState)
        {
            PooledHashSet<DiagnosticAnalyzer> pooledHashSet = ((analysisState != null) ? PooledHashSet<DiagnosticAnalyzer>.GetInstance() : null);
            try
            {
                bool result = true;
                ImmutableArray<(DiagnosticAnalyzer, ImmutableArray<CompilationAnalyzerAction>)>.Enumerator enumerator = compilationActionsMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var (diagnosticAnalyzer, compilationActions) = enumerator.Current;
                    if (analysisScope.Contains(diagnosticAnalyzer))
                    {
                        if (!AnalyzerExecutor.TryExecuteCompilationActions(compilationActions, diagnosticAnalyzer, compilationEvent, analysisScope, analysisState))
                        {
                            result = false;
                        }
                        pooledHashSet?.Add(diagnosticAnalyzer);
                    }
                }
                analysisState?.MarkEventCompleteForUnprocessedAnalyzers(compilationEvent, analysisScope, pooledHashSet);
                return result;
            }
            finally
            {
                pooledHashSet?.Free();
            }
        }

        internal static Action<Diagnostic> GetDiagnosticSink(Action<Diagnostic> addDiagnosticCore, Compilation compilation, AnalyzerOptions? analyzerOptions, SeverityFilter severityFilter, CancellationToken cancellationToken)
        {
            Compilation compilation2 = compilation;
            AnalyzerOptions analyzerOptions2 = analyzerOptions;
            Action<Diagnostic> addDiagnosticCore2 = addDiagnosticCore;
            return delegate (Diagnostic diagnostic)
            {
                Diagnostic filteredDiagnostic = GetFilteredDiagnostic(diagnostic, compilation2, analyzerOptions2, severityFilter, cancellationToken);
                if (filteredDiagnostic != null)
                {
                    addDiagnosticCore2(filteredDiagnostic);
                }
            };
        }

        internal static Action<Diagnostic, DiagnosticAnalyzer, bool> GetDiagnosticSink(Action<Diagnostic, DiagnosticAnalyzer, bool> addLocalDiagnosticCore, Compilation compilation, AnalyzerOptions? analyzerOptions, SeverityFilter severityFilter, CancellationToken cancellationToken)
        {
            Compilation compilation2 = compilation;
            AnalyzerOptions analyzerOptions2 = analyzerOptions;
            Action<Diagnostic, DiagnosticAnalyzer, bool> addLocalDiagnosticCore2 = addLocalDiagnosticCore;
            return delegate (Diagnostic diagnostic, DiagnosticAnalyzer analyzer, bool isSyntaxDiagnostic)
            {
                Diagnostic filteredDiagnostic = GetFilteredDiagnostic(diagnostic, compilation2, analyzerOptions2, severityFilter, cancellationToken);
                if (filteredDiagnostic != null)
                {
                    addLocalDiagnosticCore2(filteredDiagnostic, analyzer, isSyntaxDiagnostic);
                }
            };
        }

        internal static Action<Diagnostic, DiagnosticAnalyzer> GetDiagnosticSink(Action<Diagnostic, DiagnosticAnalyzer> addDiagnosticCore, Compilation compilation, AnalyzerOptions? analyzerOptions, SeverityFilter severityFilter, CancellationToken cancellationToken)
        {
            Compilation compilation2 = compilation;
            AnalyzerOptions analyzerOptions2 = analyzerOptions;
            Action<Diagnostic, DiagnosticAnalyzer> addDiagnosticCore2 = addDiagnosticCore;
            return delegate (Diagnostic diagnostic, DiagnosticAnalyzer analyzer)
            {
                Diagnostic filteredDiagnostic = GetFilteredDiagnostic(diagnostic, compilation2, analyzerOptions2, severityFilter, cancellationToken);
                if (filteredDiagnostic != null)
                {
                    addDiagnosticCore2(filteredDiagnostic, analyzer);
                }
            };
        }

        private static Diagnostic? GetFilteredDiagnostic(Diagnostic diagnostic, Compilation compilation, AnalyzerOptions? analyzerOptions, SeverityFilter severityFilter, CancellationToken cancellationToken)
        {
            AnalyzerOptions analyzerOptions2 = analyzerOptions;
            Compilation compilation2 = compilation;
            return applyFurtherFiltering(compilation2.Options.FilterDiagnostic(diagnostic, cancellationToken));
            Diagnostic? applyFurtherFiltering(Diagnostic? diagnostic)
            {
                SyntaxTree syntaxTree = diagnostic?.Location.SourceTree;
                if (syntaxTree != null && analyzerOptions2.TryGetSeverityFromBulkConfiguration(syntaxTree, compilation2, diagnostic!.Descriptor, cancellationToken, out var severity))
                {
                    diagnostic = diagnostic!.WithReportDiagnostic(severity);
                }
                if (diagnostic != null && severityFilter.Contains(DiagnosticDescriptor.MapSeverityToReport(diagnostic!.Severity)))
                {
                    return null;
                }
                return diagnostic;
            }
        }

        private static async Task<(AnalyzerActions actions, ImmutableHashSet<DiagnosticAnalyzer> unsuppressedAnalyzers)> GetAnalyzerActionsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, AnalyzerExecutor analyzerExecutor, SeverityFilter severityFilter)
        {
            AnalyzerActions allAnalyzerActions = AnalyzerActions.Empty;
            PooledHashSet<DiagnosticAnalyzer> unsuppressedAnalyzersBuilder = PooledHashSet<DiagnosticAnalyzer>.GetInstance();
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (!IsDiagnosticAnalyzerSuppressed(current, analyzerExecutor.Compilation.Options, analyzerManager, analyzerExecutor, severityFilter))
                {
                    unsuppressedAnalyzersBuilder.Add(current);
                    AnalyzerActions otherActions = await analyzerManager.GetAnalyzerActionsAsync(current, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
                    allAnalyzerActions = allAnalyzerActions.Append(in otherActions);
                }
            }
            ImmutableHashSet<DiagnosticAnalyzer> item = unsuppressedAnalyzersBuilder.ToImmutableHashSet();
            unsuppressedAnalyzersBuilder.Free();
            return (allAnalyzerActions, item);
        }

        public bool HasSymbolStartedActions(AnalysisScope analysisScope)
        {
            if (AnalyzerActions.SymbolStartActionsCount == 0)
            {
                return false;
            }
            if (analysisScope.Analyzers.Length == Analyzers.Length)
            {
                return true;
            }
            if (analysisScope.Analyzers.Length == 1)
            {
                DiagnosticAnalyzer diagnosticAnalyzer = analysisScope.Analyzers[0];
                ImmutableArray<SymbolStartAnalyzerAction>.Enumerator enumerator = AnalyzerActions.SymbolStartActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Analyzer == diagnosticAnalyzer)
                    {
                        return true;
                    }
                }
                return false;
            }
            PooledHashSet<DiagnosticAnalyzer> instance = PooledHashSet<DiagnosticAnalyzer>.GetInstance();
            try
            {
                ImmutableArray<SymbolStartAnalyzerAction>.Enumerator enumerator = AnalyzerActions.SymbolStartActions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SymbolStartAnalyzerAction current = enumerator.Current;
                    instance.Add(current.Analyzer);
                }
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator2 = analysisScope.Analyzers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    DiagnosticAnalyzer current2 = enumerator2.Current;
                    if (instance.Contains(current2))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                instance.Free();
            }
        }

        private async ValueTask<IGroupedAnalyzerActions> GetPerSymbolAnalyzerActionsAsync(ISymbol symbol, AnalysisScope analysisScope, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            if (AnalyzerActions.SymbolStartActionsCount == 0 || symbol.IsImplicitlyDeclared)
            {
                return EmptyGroupedActions;
            }
            IGroupedAnalyzerActions allActions = EmptyGroupedActions;
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analysisScope.Analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (SymbolStartAnalyzers.Contains(current))
                {
                    IGroupedAnalyzerActions groupedAnalyzerActions = await GetPerSymbolAnalyzerActionsAsync(symbol, current, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    if (!groupedAnalyzerActions.IsEmpty)
                    {
                        allActions = allActions.Append(groupedAnalyzerActions);
                    }
                }
            }
            return allActions;
        }

        private async ValueTask<IGroupedAnalyzerActions> GetPerSymbolAnalyzerActionsAsync(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState, CancellationToken cancellationToken)
        {
            if (symbol.IsImplicitlyDeclared)
            {
                return EmptyGroupedActions;
            }
            if (!(symbol is INamespaceOrTypeSymbol namespaceOrType))
            {
                return await getAllActionsAsync(this, symbol, analyzer, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            if (PerSymbolAnalyzerActionsCache.TryGetValue((namespaceOrType, analyzer), out var value))
            {
                return value;
            }
            IGroupedAnalyzerActions value2 = await getAllActionsAsync(this, symbol, analyzer, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return PerSymbolAnalyzerActionsCache.GetOrAdd((namespaceOrType, analyzer), value2);
            async ValueTask<IGroupedAnalyzerActions> getAllActionsAsync(AnalyzerDriver driver, ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState, CancellationToken cancellationToken)
            {
                IGroupedAnalyzerActions inheritedActions = await getInheritedActionsAsync(driver, symbol, analyzer, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                AnalyzerActions otherActions2 = await getSymbolActionsCoreAsync(driver, symbol, analyzer).ConfigureAwait(continueOnCapturedContext: false);
                if (otherActions2.IsEmpty)
                {
                    return inheritedActions;
                }
                AnalyzerActions analyzerActions2 = inheritedActions.AnalyzerActions.Append(in otherActions2);
                return CreateGroupedActions(analyzer, in analyzerActions2);
            }
            async ValueTask<IGroupedAnalyzerActions> getInheritedActionsAsync(AnalyzerDriver driver, ISymbol symbol, DiagnosticAnalyzer analyzer, AnalysisState? analysisState, CancellationToken cancellationToken)
            {
                if (symbol.ContainingSymbol != null)
                {
                    IGroupedAnalyzerActions groupedAnalyzerActions = await driver.GetPerSymbolAnalyzerActionsAsync(symbol.ContainingSymbol, analyzer, analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    if (!groupedAnalyzerActions.IsEmpty && symbol.ContainingSymbol.Kind != symbol.Kind)
                    {
                        AnalyzerActions otherActions = groupedAnalyzerActions.AnalyzerActions;
                        AnalyzerActions analyzerActions = AnalyzerActions.Empty.Append(in otherActions, appendSymbolStartAndSymbolEndActions: false);
                        return CreateGroupedActions(analyzer, in analyzerActions);
                    }
                }
                return EmptyGroupedActions;
            }
            static async ValueTask<AnalyzerActions> getSymbolActionsCoreAsync(AnalyzerDriver driver, ISymbol symbol, DiagnosticAnalyzer analyzer)
            {
                if (!driver.UnsuppressedAnalyzers.Contains(analyzer) || (driver.IsGeneratedCodeSymbol(symbol) && driver.ShouldSkipAnalysisOnGeneratedCode(analyzer)))
                {
                    return AnalyzerActions.Empty;
                }
                return await driver.AnalyzerManager.GetPerSymbolAnalyzerActionsAsync(symbol, analyzer, driver.AnalyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
            }
        }

        private static async Task<ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim>> CreateAnalyzerGateMapAsync(ImmutableHashSet<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, AnalyzerExecutor analyzerExecutor, SeverityFilter severityFilter)
        {
            ImmutableSegmentedDictionary<DiagnosticAnalyzer, SemaphoreSlim>.Builder builder = ImmutableSegmentedDictionary.CreateBuilder<DiagnosticAnalyzer, SemaphoreSlim>();
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                if (!(await analyzerManager.IsConcurrentAnalyzerAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false)))
                {
                    SemaphoreSlim value = new SemaphoreSlim(1);
                    builder.Add(analyzer, value);
                }
            }
            return builder.ToImmutable();
        }

        private static async Task<ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags>> CreateGeneratedCodeAnalysisFlagsMapAsync(ImmutableHashSet<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, AnalyzerExecutor analyzerExecutor, SeverityFilter severityFilter)
        {
            ImmutableSegmentedDictionary<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags>.Builder builder = ImmutableSegmentedDictionary.CreateBuilder<DiagnosticAnalyzer, GeneratedCodeAnalysisFlags>();
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                builder.Add(analyzer, await analyzerManager.GetGeneratedCodeAnalysisFlagsAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false));
            }
            return builder.ToImmutable();
        }

        private bool IsGeneratedCodeSymbol(ISymbol symbol)
        {
            ISymbol symbol2 = symbol;
            if (TreatAllCodeAsNonGeneratedCode)
            {
                return false;
            }
            if (!IsGeneratedCodeSymbolMap.TryGetValue(symbol2, out var value))
            {
                return IsGeneratedCodeSymbolMap.GetOrAdd(symbol2, computeIsGeneratedCodeSymbol());
            }
            return value;
            bool computeIsGeneratedCodeSymbol()
            {
                if (_lazyGeneratedCodeAttribute != null && GeneratedCodeUtilities.IsGeneratedSymbolWithGeneratedCodeAttribute(symbol2, _lazyGeneratedCodeAttribute))
                {
                    return true;
                }
                ImmutableArray<SyntaxReference>.Enumerator enumerator = symbol2.DeclaringSyntaxReferences.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxReference current = enumerator.Current;
                    if (!IsGeneratedOrHiddenCodeLocation(current.SyntaxTree, current.Span))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        protected bool IsGeneratedCode(SyntaxTree tree)
        {
            SyntaxTree tree2 = tree;
            if (TreatAllCodeAsNonGeneratedCode)
            {
                return false;
            }
            if (!GeneratedCodeFilesMap.TryGetValue(tree2, out var value))
            {
                value = computeIsGeneratedCode();
                GeneratedCodeFilesMap.TryAdd(tree2, value);
            }
            return value;
            bool computeIsGeneratedCode()
            {
                return GeneratedCodeUtilities.GetIsGeneratedCodeFromOptions(AnalyzerExecutor.AnalyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(tree2)) ?? _isGeneratedCode(tree2, AnalyzerExecutor.CancellationToken);
            }
        }

        protected bool IsGeneratedOrHiddenCodeLocation(SyntaxTree syntaxTree, TextSpan span)
        {
            if (!IsGeneratedCode(syntaxTree))
            {
                return IsHiddenSourceLocation(syntaxTree, span);
            }
            return true;
        }

        protected bool IsHiddenSourceLocation(SyntaxTree syntaxTree, TextSpan span)
        {
            if (HasHiddenRegions(syntaxTree))
            {
                return syntaxTree.IsHiddenPosition(span.Start);
            }
            return false;
        }

        private bool HasHiddenRegions(SyntaxTree tree)
        {
            if (_lazyTreesWithHiddenRegionsMap == null)
            {
                return false;
            }
            if (!_lazyTreesWithHiddenRegionsMap!.TryGetValue(tree, out var value))
            {
                value = tree.HasHiddenRegions();
                _lazyTreesWithHiddenRegionsMap!.TryAdd(tree, value);
            }
            return value;
        }

        internal async Task<AnalyzerActionCounts> GetAnalyzerActionCountsAsync(DiagnosticAnalyzer analyzer, CompilationOptions compilationOptions, CancellationToken cancellationToken)
        {
            AnalyzerExecutor analyzerExecutor = AnalyzerExecutor.WithCancellationToken(cancellationToken);
            if (IsDiagnosticAnalyzerSuppressed(analyzer, compilationOptions, AnalyzerManager, analyzerExecutor, _severityFilter))
            {
                return AnalyzerActionCounts.Empty;
            }
            AnalyzerActions analyzerActions = await AnalyzerManager.GetAnalyzerActionsAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
            if (analyzerActions.IsEmpty)
            {
                return AnalyzerActionCounts.Empty;
            }
            return new AnalyzerActionCounts(in analyzerActions);
        }

        internal static bool IsDiagnosticAnalyzerSuppressed(DiagnosticAnalyzer analyzer, CompilationOptions options, AnalyzerManager analyzerManager, AnalyzerExecutor analyzerExecutor, SeverityFilter severityFilter)
        {
            return analyzerManager.IsDiagnosticAnalyzerSuppressed(analyzer, options, s_IsCompilerAnalyzerFunc, analyzerExecutor, severityFilter);
        }

        private static bool IsCompilerAnalyzer(DiagnosticAnalyzer analyzer)
        {
            return analyzer is CompilerDiagnosticAnalyzer;
        }

        public void Dispose()
        {
            _lazyCompilationEventQueue?.TryComplete();
            _lazyDiagnosticQueue?.TryComplete();
            _lazyQueueRegistration?.Dispose();
        }

        protected abstract IGroupedAnalyzerActions CreateGroupedActions(DiagnosticAnalyzer analyzer, in AnalyzerActions analyzerActions);
    }
    public class AnalyzerDriver<TLanguageKindEnum> : AnalyzerDriver where TLanguageKindEnum : struct
    {
        [StructLayout(LayoutKind.Auto)]
        private struct ExecutableCodeBlockAnalyzerActions
        {
            public DiagnosticAnalyzer Analyzer;

            public ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> CodeBlockStartActions;

            public ImmutableArray<CodeBlockAnalyzerAction> CodeBlockActions;

            public ImmutableArray<CodeBlockAnalyzerAction> CodeBlockEndActions;

            public ImmutableArray<OperationBlockStartAnalyzerAction> OperationBlockStartActions;

            public ImmutableArray<OperationBlockAnalyzerAction> OperationBlockActions;

            public ImmutableArray<OperationBlockAnalyzerAction> OperationBlockEndActions;
        }

        private sealed class GroupedAnalyzerActions : IGroupedAnalyzerActions
        {
            public static readonly GroupedAnalyzerActions Empty = new GroupedAnalyzerActions(ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)>.Empty, in AnalyzerActions.Empty);

            public ImmutableArray<(DiagnosticAnalyzer analyzer, GroupedAnalyzerActionsForAnalyzer groupedActions)> GroupedActionsByAnalyzer { get; }

            public AnalyzerActions AnalyzerActions { get; }

            public bool IsEmpty => this == Empty;

            private GroupedAnalyzerActions(ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)> groupedActionsAndAnalyzers, in AnalyzerActions analyzerActions)
            {
                GroupedActionsByAnalyzer = groupedActionsAndAnalyzers;
                AnalyzerActions = analyzerActions;
            }

            public static GroupedAnalyzerActions Create(DiagnosticAnalyzer analyzer, in AnalyzerActions analyzerActions)
            {
                if (analyzerActions.IsEmpty)
                {
                    return Empty;
                }
                GroupedAnalyzerActionsForAnalyzer item = new GroupedAnalyzerActionsForAnalyzer(analyzer, in analyzerActions, analyzerActionsNeedFiltering: false);
                return new GroupedAnalyzerActions(ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)>.Empty.Add((analyzer, item)), in analyzerActions);
            }

            public static GroupedAnalyzerActions Create(ImmutableArray<DiagnosticAnalyzer> analyzers, in AnalyzerActions analyzerActions)
            {
                return new GroupedAnalyzerActions(analyzers.SelectAsArray((DiagnosticAnalyzer analyzer, AnalyzerActions analyzerActions) => (analyzer, new GroupedAnalyzerActionsForAnalyzer(analyzer, in analyzerActions, analyzerActionsNeedFiltering: true)), analyzerActions), in analyzerActions);
            }

            IGroupedAnalyzerActions IGroupedAnalyzerActions.Append(IGroupedAnalyzerActions igroupedAnalyzerActions)
            {
                GroupedAnalyzerActions groupedAnalyzerActions = (GroupedAnalyzerActions)igroupedAnalyzerActions;
                ImmutableArray<(DiagnosticAnalyzer analyzer, GroupedAnalyzerActionsForAnalyzer groupedActions)> groupedActionsAndAnalyzers = GroupedActionsByAnalyzer.AddRange(groupedAnalyzerActions.GroupedActionsByAnalyzer);
                AnalyzerActions analyzerActions = AnalyzerActions;
                AnalyzerActions otherActions = groupedAnalyzerActions.AnalyzerActions;
                AnalyzerActions analyzerActions2 = analyzerActions.Append(in otherActions);
                return new GroupedAnalyzerActions(groupedActionsAndAnalyzers, in analyzerActions2);
            }
        }

        private sealed class GroupedAnalyzerActionsForAnalyzer
        {
            private readonly DiagnosticAnalyzer _analyzer;

            private readonly bool _analyzerActionsNeedFiltering;

            private ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> _lazyNodeActionsByKind;

            private ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> _lazyOperationActionsByKind;

            private ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> _lazyCodeBlockStartActions;

            private ImmutableArray<CodeBlockAnalyzerAction> _lazyCodeBlockEndActions;

            private ImmutableArray<CodeBlockAnalyzerAction> _lazyCodeBlockActions;

            private ImmutableArray<OperationBlockStartAnalyzerAction> _lazyOperationBlockStartActions;

            private ImmutableArray<OperationBlockAnalyzerAction> _lazyOperationBlockActions;

            private ImmutableArray<OperationBlockAnalyzerAction> _lazyOperationBlockEndActions;

            public AnalyzerActions AnalyzerActions { get; }

            public ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> NodeActionsByAnalyzerAndKind
            {
                get
                {
                    if (_lazyNodeActionsByKind == null)
                    {
                        ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>> immutableArray = (_analyzerActionsNeedFiltering ? AnalyzerActions.GetSyntaxNodeActions<TLanguageKindEnum>(_analyzer) : AnalyzerActions.GetSyntaxNodeActions<TLanguageKindEnum>());
                        ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> value = ((!immutableArray.IsEmpty) ? Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.GetNodeActionsByKind(immutableArray) : ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>>.Empty);
                        RoslynImmutableInterlocked.InterlockedInitialize<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>>(ref _lazyNodeActionsByKind, value);
                    }
                    return _lazyNodeActionsByKind;
                }
            }

            public ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> OperationActionsByAnalyzerAndKind
            {
                get
                {
                    if (_lazyOperationActionsByKind == null)
                    {
                        ImmutableArray<OperationAnalyzerAction> actions = AnalyzerActions.OperationActions;
                        ImmutableArray<OperationAnalyzerAction> filteredActions = GetFilteredActions(in actions);
                        ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> value = (filteredActions.Any() ? Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.GetOperationActionsByKind(filteredActions) : ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>>.Empty);
                        RoslynImmutableInterlocked.InterlockedInitialize<OperationKind, ImmutableArray<OperationAnalyzerAction>>(ref _lazyOperationActionsByKind, value);
                    }
                    return _lazyOperationActionsByKind;
                }
            }

            private ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> CodeBlockStartActions
            {
                get
                {
                    if (_lazyCodeBlockStartActions.IsDefault)
                    {
                        ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> actions = AnalyzerActions.GetCodeBlockStartActions<TLanguageKindEnum>();
                        ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> filteredActions = GetFilteredActions(in actions);
                        ImmutableInterlocked.InterlockedInitialize(ref _lazyCodeBlockStartActions, filteredActions);
                    }
                    return _lazyCodeBlockStartActions;
                }
            }

            private ImmutableArray<CodeBlockAnalyzerAction> CodeBlockEndActions => GetExecutableCodeActions(ref _lazyCodeBlockEndActions, AnalyzerActions.CodeBlockEndActions, _analyzer, _analyzerActionsNeedFiltering);

            private ImmutableArray<CodeBlockAnalyzerAction> CodeBlockActions => GetExecutableCodeActions(ref _lazyCodeBlockActions, AnalyzerActions.CodeBlockActions, _analyzer, _analyzerActionsNeedFiltering);

            private ImmutableArray<OperationBlockStartAnalyzerAction> OperationBlockStartActions => GetExecutableCodeActions(ref _lazyOperationBlockStartActions, AnalyzerActions.OperationBlockStartActions, _analyzer, _analyzerActionsNeedFiltering);

            private ImmutableArray<OperationBlockAnalyzerAction> OperationBlockEndActions => GetExecutableCodeActions(ref _lazyOperationBlockEndActions, AnalyzerActions.OperationBlockEndActions, _analyzer, _analyzerActionsNeedFiltering);

            private ImmutableArray<OperationBlockAnalyzerAction> OperationBlockActions => GetExecutableCodeActions(ref _lazyOperationBlockActions, AnalyzerActions.OperationBlockActions, _analyzer, _analyzerActionsNeedFiltering);

            public GroupedAnalyzerActionsForAnalyzer(DiagnosticAnalyzer analyzer, in AnalyzerActions analyzerActions, bool analyzerActionsNeedFiltering)
            {
                _analyzer = analyzer;
                AnalyzerActions = analyzerActions;
                _analyzerActionsNeedFiltering = analyzerActionsNeedFiltering;
            }

            [Conditional("DEBUG")]
            private static void VerifyActions<TAnalyzerAction>(in ImmutableArray<TAnalyzerAction> actions, DiagnosticAnalyzer analyzer) where TAnalyzerAction : AnalyzerAction
            {
                ImmutableArray<TAnalyzerAction>.Enumerator enumerator = actions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    _ = enumerator.Current;
                }
            }

            private ImmutableArray<TAnalyzerAction> GetFilteredActions<TAnalyzerAction>(in ImmutableArray<TAnalyzerAction> actions) where TAnalyzerAction : AnalyzerAction
            {
                return GetFilteredActions(in actions, _analyzer, _analyzerActionsNeedFiltering);
            }

            private static ImmutableArray<TAnalyzerAction> GetFilteredActions<TAnalyzerAction>(in ImmutableArray<TAnalyzerAction> actions, DiagnosticAnalyzer analyzer, bool analyzerActionsNeedFiltering) where TAnalyzerAction : AnalyzerAction
            {
                if (!analyzerActionsNeedFiltering)
                {
                    return actions;
                }
                return actions.WhereAsArray((TAnalyzerAction action, DiagnosticAnalyzer analyzer) => action.Analyzer == analyzer, analyzer);
            }

            private static ImmutableArray<ActionType> GetExecutableCodeActions<ActionType>(ref ImmutableArray<ActionType> lazyCodeBlockActions, ImmutableArray<ActionType> codeBlockActions, DiagnosticAnalyzer analyzer, bool analyzerActionsNeedFiltering) where ActionType : AnalyzerAction
            {
                if (lazyCodeBlockActions.IsDefault)
                {
                    codeBlockActions = GetFilteredActions(in codeBlockActions, analyzer, analyzerActionsNeedFiltering);
                    ImmutableInterlocked.InterlockedInitialize(ref lazyCodeBlockActions, codeBlockActions);
                }
                return lazyCodeBlockActions;
            }

            public bool TryGetExecutableCodeBlockActions(out ExecutableCodeBlockAnalyzerActions actions)
            {
                if (!OperationBlockStartActions.IsEmpty || !OperationBlockActions.IsEmpty || !OperationBlockEndActions.IsEmpty || !CodeBlockStartActions.IsEmpty || !CodeBlockActions.IsEmpty || !CodeBlockEndActions.IsEmpty)
                {
                    actions = new ExecutableCodeBlockAnalyzerActions
                    {
                        Analyzer = _analyzer,
                        CodeBlockStartActions = CodeBlockStartActions,
                        CodeBlockActions = CodeBlockActions,
                        CodeBlockEndActions = CodeBlockEndActions,
                        OperationBlockStartActions = OperationBlockStartActions,
                        OperationBlockActions = OperationBlockActions,
                        OperationBlockEndActions = OperationBlockEndActions
                    };
                    return true;
                }
                actions = default(ExecutableCodeBlockAnalyzerActions);
                return false;
            }
        }

        private readonly Func<SyntaxNode, TLanguageKindEnum> _getKind;

        private GroupedAnalyzerActions? _lazyCoreActions;

        protected override IGroupedAnalyzerActions EmptyGroupedActions => GroupedAnalyzerActions.Empty;

        public AnalyzerDriver(ImmutableArray<DiagnosticAnalyzer> analyzers, Func<SyntaxNode, TLanguageKindEnum> getKind, AnalyzerManager analyzerManager, SeverityFilter severityFilter, Func<SyntaxTrivia, bool> isComment)
            : base(analyzers, analyzerManager, severityFilter, isComment)
        {
            _getKind = getKind;
        }

        private GroupedAnalyzerActions GetOrCreateCoreActions()
        {
            if (_lazyCoreActions == null)
            {
                Interlocked.CompareExchange(ref _lazyCoreActions, createCoreActions(), null);
            }
            return _lazyCoreActions;
            GroupedAnalyzerActions createCoreActions()
            {
                if (base.AnalyzerActions.IsEmpty)
                {
                    return GroupedAnalyzerActions.Empty;
                }
                return GroupedAnalyzerActions.Create(base.Analyzers.WhereAsArray(base.UnsuppressedAnalyzers.Contains), in base.AnalyzerActions);
            }
        }

        private static void ComputeShouldExecuteActions(in AnalyzerActions coreActions, in AnalyzerActions additionalActions, ISymbol symbol, out bool executeSyntaxNodeActions, out bool executeCodeBlockActions, out bool executeOperationActions, out bool executeOperationBlockActions)
        {
            executeSyntaxNodeActions = false;
            executeCodeBlockActions = false;
            executeOperationActions = false;
            executeOperationBlockActions = false;
            bool canHaveExecutableCodeBlock2 = Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.CanHaveExecutableCodeBlock(symbol);
            computeShouldExecuteActions(coreActions, canHaveExecutableCodeBlock2, ref executeSyntaxNodeActions, ref executeCodeBlockActions, ref executeOperationActions, ref executeOperationBlockActions);
            computeShouldExecuteActions(additionalActions, canHaveExecutableCodeBlock2, ref executeSyntaxNodeActions, ref executeCodeBlockActions, ref executeOperationActions, ref executeOperationBlockActions);
            static void computeShouldExecuteActions(AnalyzerActions analyzerActions, bool canHaveExecutableCodeBlock, ref bool executeSyntaxNodeActions, ref bool executeCodeBlockActions, ref bool executeOperationActions, ref bool executeOperationBlockActions)
            {
                if (!analyzerActions.IsEmpty)
                {
                    executeSyntaxNodeActions |= analyzerActions.SyntaxNodeActionsCount > 0;
                    executeOperationActions |= analyzerActions.OperationActionsCount > 0;
                    if (canHaveExecutableCodeBlock)
                    {
                        executeCodeBlockActions |= analyzerActions.CodeBlockStartActionsCount > 0 || analyzerActions.CodeBlockActionsCount > 0;
                        executeOperationBlockActions |= analyzerActions.OperationBlockStartActionsCount > 0 || analyzerActions.OperationBlockActionsCount > 0;
                    }
                }
            }
        }

        protected override IGroupedAnalyzerActions CreateGroupedActions(DiagnosticAnalyzer analyzer, in AnalyzerActions analyzerActions)
        {
            return GroupedAnalyzerActions.Create(analyzer, in analyzerActions);
        }

        protected override bool TryExecuteDeclaringReferenceActions(SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, bool isGeneratedCodeSymbol, IGroupedAnalyzerActions additionalPerSymbolActions, CancellationToken cancellationToken)
        {
            ISymbol symbol = symbolEvent.Symbol;
            ref readonly AnalyzerActions analyzerActions = ref base.AnalyzerActions;
            AnalyzerActions additionalActions = additionalPerSymbolActions.AnalyzerActions;
            ComputeShouldExecuteActions(in analyzerActions, in additionalActions, symbol, out var executeSyntaxNodeActions, out var executeCodeBlockActions, out var executeOperationActions, out var executeOperationBlockActions);
            bool result = true;
            if (executeSyntaxNodeActions || executeOperationActions || executeCodeBlockActions || executeOperationBlockActions)
            {
                ImmutableArray<SyntaxReference> declaringSyntaxReferences = symbolEvent.DeclaringSyntaxReferences;
                GroupedAnalyzerActions orCreateCoreActions = GetOrCreateCoreActions();
                for (int i = 0; i < declaringSyntaxReferences.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    SyntaxReference syntaxReference = declaringSyntaxReferences[i];
                    if (!analysisScope.FilterFileOpt.HasValue || analysisScope.FilterFileOpt?.SourceTree == syntaxReference.SyntaxTree)
                    {
                        bool flag = isGeneratedCodeSymbol || IsGeneratedOrHiddenCodeLocation(syntaxReference.SyntaxTree, syntaxReference.Span);
                        if (flag && base.DoNotAnalyzeGeneratedCode)
                        {
                            analysisState?.MarkDeclarationComplete(symbol, i, analysisScope.Analyzers);
                        }
                        else if (!TryExecuteDeclaringReferenceActions(syntaxReference, i, symbolEvent, analysisScope, analysisState, orCreateCoreActions, (GroupedAnalyzerActions)additionalPerSymbolActions, executeSyntaxNodeActions, executeOperationActions, executeCodeBlockActions, executeOperationBlockActions, flag, cancellationToken))
                        {
                            result = false;
                        }
                    }
                }
            }
            else if (analysisState != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                analysisState!.MarkDeclarationsComplete(symbol, analysisScope.Analyzers);
                ImmutableArray<SyntaxReference> declaringSyntaxReferences2 = symbolEvent.DeclaringSyntaxReferences;
                for (int j = 0; j < declaringSyntaxReferences2.Length; j++)
                {
                    SyntaxReference declaration = declaringSyntaxReferences2[j];
                    ClearCachedAnalysisDataIfAnalyzed(declaration, symbol, j, analysisState);
                }
            }
            return result;
        }

        private void ClearCachedAnalysisDataIfAnalyzed(SyntaxReference declaration, ISymbol symbol, int declarationIndex, AnalysisState analysisState)
        {
            if (analysisState.IsDeclarationComplete(symbol, declarationIndex))
            {
                base.CurrentCompilationData.ClearDeclarationAnalysisData(declaration);
            }
        }

        private DeclarationAnalysisData ComputeDeclarationAnalysisData(ISymbol symbol, SyntaxReference declaration, SemanticModel semanticModel, AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArrayBuilder<DeclarationInfo> instance = ArrayBuilder<DeclarationInfo>.GetInstance();
            SyntaxNode syntax = declaration.GetSyntax(cancellationToken);
            SyntaxNode topmostNodeForDiagnosticAnalysis = semanticModel.GetTopmostNodeForDiagnosticAnalysis(symbol, syntax);
            ComputeDeclarationsInNode(semanticModel, symbol, syntax, topmostNodeForDiagnosticAnalysis, instance, cancellationToken);
            ImmutableArray<DeclarationInfo> immutableArray = instance.ToImmutableAndFree();
            bool flag = analysisScope.FilterSpanOpt.HasValue && !analysisScope.ContainsSpan(topmostNodeForDiagnosticAnalysis.FullSpan);
            ImmutableArray<SyntaxNode> syntaxNodesToAnalyze = GetSyntaxNodesToAnalyze(topmostNodeForDiagnosticAnalysis, symbol, immutableArray, analysisScope, flag, semanticModel, base.AnalyzerExecutor);
            return new DeclarationAnalysisData(syntax, topmostNodeForDiagnosticAnalysis, immutableArray, syntaxNodesToAnalyze, flag);
        }

        private static void ComputeDeclarationsInNode(SemanticModel semanticModel, ISymbol declaredSymbol, SyntaxNode declaringReferenceSyntax, SyntaxNode topmostNodeForAnalysis, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken)
        {
            int? levelsToCompute = 2;
            bool getSymbol = topmostNodeForAnalysis != declaringReferenceSyntax || declaredSymbol.Kind == SymbolKind.Namespace;
            semanticModel.ComputeDeclarationsInNode(topmostNodeForAnalysis, declaredSymbol, getSymbol, builder, cancellationToken, levelsToCompute);
        }

        private bool TryExecuteDeclaringReferenceActions(SyntaxReference decl, int declarationIndex, SymbolDeclaredCompilationEvent symbolEvent, AnalysisScope analysisScope, AnalysisState? analysisState, GroupedAnalyzerActions coreActions, GroupedAnalyzerActions additionalPerSymbolActions, bool shouldExecuteSyntaxNodeActions, bool shouldExecuteOperationActions, bool shouldExecuteCodeBlockActions, bool shouldExecuteOperationBlockActions, bool isInGeneratedCode, CancellationToken cancellationToken)
        {
            SyntaxReference decl2 = decl;
            AnalysisScope analysisScope2 = analysisScope;
            GroupedAnalyzerActions coreActions2 = coreActions;
            GroupedAnalyzerActions additionalPerSymbolActions2 = additionalPerSymbolActions;
            AnalysisState analysisState2 = analysisState;
            ISymbol symbol = symbolEvent.Symbol;
            SemanticModel semanticModel = symbolEvent.SemanticModelWithCachedBoundNodes ?? base.SemanticModelProvider.GetSemanticModel(decl2.SyntaxTree, symbolEvent.Compilation);
            bool flag = analysisScope2.Analyzers.Length < base.Analyzers.Length && (!analysisScope2.FilterSpanOpt.HasValue || analysisScope2.FilterSpanOpt.Value.Length >= decl2.SyntaxTree.GetRoot(cancellationToken).Span.Length);
            DeclarationAnalysisData declarationAnalysisData = base.CurrentCompilationData.GetOrComputeDeclarationAnalysisData(decl2, () => ComputeDeclarationAnalysisData(symbol, decl2, semanticModel, analysisScope2, cancellationToken), flag);
            if (!analysisScope2.ShouldAnalyze(declarationAnalysisData.TopmostNodeForAnalysis))
            {
                return true;
            }
            bool success = true;
            executeNodeActions();
            executeExecutableCodeActions();
            if (success && analysisState2 != null && !declarationAnalysisData.IsPartialAnalysis)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analysisScope2.Analyzers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticAnalyzer current = enumerator.Current;
                    analysisState2.MarkDeclarationComplete(symbol, declarationIndex, current);
                }
                if (flag)
                {
                    ClearCachedAnalysisDataIfAnalyzed(decl2, symbol, declarationIndex, analysisState2);
                }
            }
            return success;
            static void addExecutableCodeBlockAnalyzerActions(GroupedAnalyzerActions groupedActions, AnalysisScope analysisScope, ArrayBuilder<ExecutableCodeBlockAnalyzerActions> builder)
            {
                ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)>.Enumerator enumerator5 = groupedActions.GroupedActionsByAnalyzer.GetEnumerator();
                while (enumerator5.MoveNext())
                {
                    var (analyzer, groupedAnalyzerActionsForAnalyzer) = enumerator5.Current;
                    if (analysisScope.Contains(analyzer) && groupedAnalyzerActionsForAnalyzer.TryGetExecutableCodeBlockActions(out var actions))
                    {
                        builder.Add(actions);
                    }
                }
            }
            void executeCodeBlockActions(ImmutableArray<SyntaxNode> executableCodeBlocks, IEnumerable<ExecutableCodeBlockAnalyzerActions> codeBlockActions)
            {
                if (executableCodeBlocks.IsEmpty || !shouldExecuteCodeBlockActions)
                {
                    return;
                }
                foreach (ExecutableCodeBlockAnalyzerActions codeBlockAction in codeBlockActions)
                {
                    ImmutableArray<CodeBlockStartAnalyzerAction<TLanguageKindEnum>> codeBlockStartActions = codeBlockAction.CodeBlockStartActions;
                    if (codeBlockStartActions.IsEmpty)
                    {
                        ImmutableArray<CodeBlockAnalyzerAction> codeBlockActions2 = codeBlockAction.CodeBlockActions;
                        if (codeBlockActions2.IsEmpty)
                        {
                            codeBlockActions2 = codeBlockAction.CodeBlockEndActions;
                            if (codeBlockActions2.IsEmpty)
                            {
                                continue;
                            }
                        }
                    }
                    if (analysisScope2.Contains(codeBlockAction.Analyzer) && !base.AnalyzerExecutor.TryExecuteCodeBlockActions(codeBlockAction.CodeBlockStartActions, codeBlockAction.CodeBlockActions, codeBlockAction.CodeBlockEndActions, codeBlockAction.Analyzer, declarationAnalysisData.TopmostNodeForAnalysis, symbol, executableCodeBlocks, semanticModel, _getKind, declarationIndex, analysisScope2, analysisState2, isInGeneratedCode))
                    {
                        success = false;
                    }
                }
            }
            void executeExecutableCodeActions()
            {
                if (!shouldExecuteCodeBlockActions && !shouldExecuteOperationActions && !shouldExecuteOperationBlockActions)
                {
                    return;
                }
                ImmutableArray<SyntaxNode> immutableArray = ImmutableArray<SyntaxNode>.Empty;
                ArrayBuilder<ExecutableCodeBlockAnalyzerActions> instance = ArrayBuilder<ExecutableCodeBlockAnalyzerActions>.GetInstance();
                try
                {
                    ImmutableArray<DeclarationInfo>.Enumerator enumerator6 = declarationAnalysisData.DeclarationsInNode.GetEnumerator();
                    while (enumerator6.MoveNext())
                    {
                        DeclarationInfo current5 = enumerator6.Current;
                        if (current5.DeclaredNode == declarationAnalysisData.TopmostNodeForAnalysis || current5.DeclaredNode == declarationAnalysisData.DeclaringReferenceSyntax)
                        {
                            immutableArray = current5.ExecutableCodeBlocks;
                            if (!immutableArray.IsEmpty)
                            {
                                if (shouldExecuteCodeBlockActions || shouldExecuteOperationBlockActions)
                                {
                                    addExecutableCodeBlockAnalyzerActions(coreActions2, analysisScope2, instance);
                                    addExecutableCodeBlockAnalyzerActions(additionalPerSymbolActions2, analysisScope2, instance);
                                }
                                if (shouldExecuteOperationActions || shouldExecuteOperationBlockActions)
                                {
                                    ImmutableArray<IOperation> operationBlocksToAnalyze2 = GetOperationBlocksToAnalyze(immutableArray, semanticModel, cancellationToken);
                                    ImmutableArray<IOperation> operationsToAnalyzeWithStackGuard = getOperationsToAnalyzeWithStackGuard(operationBlocksToAnalyze2);
                                    if (!operationsToAnalyzeWithStackGuard.IsEmpty)
                                    {
                                        try
                                        {
                                            executeOperationsActions(operationsToAnalyzeWithStackGuard);
                                            executeOperationsBlockActions(operationBlocksToAnalyze2, operationsToAnalyzeWithStackGuard, instance);
                                        }
                                        finally
                                        {
                                            base.AnalyzerExecutor.OnOperationBlockActionsExecuted(operationBlocksToAnalyze2);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    executeCodeBlockActions(immutableArray, instance);
                }
                finally
                {
                    instance.Free();
                }
            }
            void executeNodeActions()
            {
                if (shouldExecuteSyntaxNodeActions)
                {
                    ImmutableArray<SyntaxNode> descendantNodesToAnalyze = declarationAnalysisData.DescendantNodesToAnalyze;
                    executeNodeActionsByKind(analysisScope2, descendantNodesToAnalyze, coreActions2);
                    executeNodeActionsByKind(analysisScope2, descendantNodesToAnalyze, additionalPerSymbolActions2);
                }
            }
            void executeNodeActionsByKind(AnalysisScope analysisScope, ImmutableArray<SyntaxNode> nodesToAnalyze, GroupedAnalyzerActions groupedActions)
            {
                ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)>.Enumerator enumerator7 = groupedActions.GroupedActionsByAnalyzer.GetEnumerator();
                while (enumerator7.MoveNext())
                {
                    (DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer) current6 = enumerator7.Current;
                    DiagnosticAnalyzer item2 = current6.Item1;
                    ImmutableSegmentedDictionary<TLanguageKindEnum, ImmutableArray<SyntaxNodeAnalyzerAction<TLanguageKindEnum>>> nodeActionsByAnalyzerAndKind = current6.Item2.NodeActionsByAnalyzerAndKind;
                    if (!nodeActionsByAnalyzerAndKind.IsEmpty && analysisScope.Contains(item2) && !base.AnalyzerExecutor.TryExecuteSyntaxNodeActions(nodesToAnalyze, nodeActionsByAnalyzerAndKind, item2, semanticModel, _getKind, declarationAnalysisData.TopmostNodeForAnalysis.FullSpan, declarationIndex, symbol, analysisScope, analysisState2, isInGeneratedCode))
                    {
                        success = false;
                    }
                }
            }
            void executeOperationsActions(ImmutableArray<IOperation> operationsToAnalyze)
            {
                if (shouldExecuteOperationActions)
                {
                    executeOperationsActionsByKind(analysisScope2, operationsToAnalyze, coreActions2);
                    executeOperationsActionsByKind(analysisScope2, operationsToAnalyze, additionalPerSymbolActions2);
                }
            }
            void executeOperationsActionsByKind(AnalysisScope analysisScope, ImmutableArray<IOperation> operationsToAnalyze, GroupedAnalyzerActions groupedActions)
            {
                ImmutableArray<(DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer)>.Enumerator enumerator4 = groupedActions.GroupedActionsByAnalyzer.GetEnumerator();
                while (enumerator4.MoveNext())
                {
                    (DiagnosticAnalyzer, GroupedAnalyzerActionsForAnalyzer) current4 = enumerator4.Current;
                    DiagnosticAnalyzer item = current4.Item1;
                    ImmutableSegmentedDictionary<OperationKind, ImmutableArray<OperationAnalyzerAction>> operationActionsByAnalyzerAndKind = current4.Item2.OperationActionsByAnalyzerAndKind;
                    if (!operationActionsByAnalyzerAndKind.IsEmpty && analysisScope.Contains(item) && !base.AnalyzerExecutor.TryExecuteOperationActions(operationsToAnalyze, operationActionsByAnalyzerAndKind, item, semanticModel, declarationAnalysisData.TopmostNodeForAnalysis.FullSpan, declarationIndex, symbol, analysisScope, analysisState2, isInGeneratedCode))
                    {
                        success = false;
                    }
                }
            }
            void executeOperationsBlockActions(ImmutableArray<IOperation> operationBlocksToAnalyze, ImmutableArray<IOperation> operationsToAnalyze, IEnumerable<ExecutableCodeBlockAnalyzerActions> codeBlockActions)
            {
                if (!shouldExecuteOperationBlockActions)
                {
                    return;
                }
                foreach (ExecutableCodeBlockAnalyzerActions codeBlockAction2 in codeBlockActions)
                {
                    ImmutableArray<OperationBlockStartAnalyzerAction> operationBlockStartActions = codeBlockAction2.OperationBlockStartActions;
                    if (operationBlockStartActions.IsEmpty)
                    {
                        ImmutableArray<OperationBlockAnalyzerAction> operationBlockActions = codeBlockAction2.OperationBlockActions;
                        if (operationBlockActions.IsEmpty)
                        {
                            operationBlockActions = codeBlockAction2.OperationBlockEndActions;
                            if (operationBlockActions.IsEmpty)
                            {
                                continue;
                            }
                        }
                    }
                    if (analysisScope2.Contains(codeBlockAction2.Analyzer) && !base.AnalyzerExecutor.TryExecuteOperationBlockActions(codeBlockAction2.OperationBlockStartActions, codeBlockAction2.OperationBlockActions, codeBlockAction2.OperationBlockEndActions, codeBlockAction2.Analyzer, declarationAnalysisData.TopmostNodeForAnalysis, symbol, operationBlocksToAnalyze, operationsToAnalyze, semanticModel, declarationIndex, analysisScope2, analysisState2, isInGeneratedCode))
                    {
                        success = false;
                    }
                }
            }
            ImmutableArray<IOperation> getOperationsToAnalyzeWithStackGuard(ImmutableArray<IOperation> operationBlocksToAnalyze)
            {
                try
                {
                    return GetOperationsToAnalyze(operationBlocksToAnalyze);
                }
                catch (Exception ex) when (ex is InsufficientExecutionStackException || FatalError.ReportAndCatchUnlessCanceled(ex, cancellationToken))
                {
                    Diagnostic arg = Microsoft.CodeAnalysis.Diagnostics.AnalyzerExecutor.CreateDriverExceptionDiagnostic(ex);
                    DiagnosticAnalyzer arg2 = base.Analyzers[0];
                    base.AnalyzerExecutor.OnAnalyzerException(ex, arg2, arg);
                    return ImmutableArray<IOperation>.Empty;
                }
            }
        }

        private static ImmutableArray<SyntaxNode> GetSyntaxNodesToAnalyze(SyntaxNode declaredNode, ISymbol declaredSymbol, ImmutableArray<DeclarationInfo> declarationsInNode, AnalysisScope analysisScope, bool isPartialDeclAnalysis, SemanticModel semanticModel, AnalyzerExecutor analyzerExecutor)
        {
            HashSet<SyntaxNode> descendantDeclsToSkip = null;
            bool flag = true;
            ImmutableArray<DeclarationInfo>.Enumerator enumerator = declarationsInNode.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DeclarationInfo current = enumerator.Current;
                analyzerExecutor.CancellationToken.ThrowIfCancellationRequested();
                if (current.DeclaredNode != declaredNode)
                {
                    if (IsEquivalentSymbol(declaredSymbol, current.DeclaredSymbol))
                    {
                        if (flag)
                        {
                            break;
                        }
                        return ImmutableArray<SyntaxNode>.Empty;
                    }
                    SyntaxNode item = current.DeclaredNode;
                    ISymbol symbol = current.DeclaredSymbol ?? semanticModel.GetDeclaredSymbol(current.DeclaredNode, analyzerExecutor.CancellationToken);
                    if (symbol != null)
                    {
                        item = semanticModel.GetTopmostNodeForDiagnosticAnalysis(symbol, current.DeclaredNode);
                    }
                    if (descendantDeclsToSkip == null)
                    {
                        descendantDeclsToSkip = new HashSet<SyntaxNode>();
                    }
                    descendantDeclsToSkip.Add(item);
                }
                flag = false;
            }
            Func<SyntaxNode, bool> additionalFilter = semanticModel.GetSyntaxNodesToAnalyzeFilter(declaredNode, declaredSymbol);
            ArrayBuilder<SyntaxNode> instance = ArrayBuilder<SyntaxNode>.GetInstance();
            foreach (SyntaxNode item2 in declaredNode.DescendantNodesAndSelf(shouldAddNode, descendIntoTrivia: true))
            {
                if (shouldAddNode(item2) && (!isPartialDeclAnalysis || analysisScope.ShouldAnalyze(item2)))
                {
                    instance.Add(item2);
                }
            }
            return instance.ToImmutableAndFree();
            bool shouldAddNode(SyntaxNode node)
            {
                if (descendantDeclsToSkip == null || !descendantDeclsToSkip.Contains(node))
                {
                    if (additionalFilter != null)
                    {
                        return additionalFilter(node);
                    }
                    return true;
                }
                return false;
            }
        }

        private static bool IsEquivalentSymbol(ISymbol declaredSymbol, ISymbol? otherSymbol)
        {
            if (declaredSymbol.Equals(otherSymbol))
            {
                return true;
            }
            if (otherSymbol != null && declaredSymbol.Kind == SymbolKind.Namespace && otherSymbol!.Kind == SymbolKind.Namespace && declaredSymbol.Name == otherSymbol!.Name)
            {
                return declaredSymbol.ToDisplayString() == otherSymbol!.ToDisplayString();
            }
            return false;
        }

        private static ImmutableArray<IOperation> GetOperationBlocksToAnalyze(ImmutableArray<SyntaxNode> executableBlocks, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance();
            ImmutableArray<SyntaxNode>.Enumerator enumerator = executableBlocks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxNode current = enumerator.Current;
                IOperation operation = semanticModel.GetOperation(current, cancellationToken);
                if (operation != null)
                {
                    instance.AddRange(operation);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private static ImmutableArray<IOperation> GetOperationsToAnalyze(ImmutableArray<IOperation> operationBlocks)
        {
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance();
            bool flag = true;
            ImmutableArray<IOperation>.Enumerator enumerator = operationBlocks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IOperation current = enumerator.Current;
                if (flag && current.Parent != null)
                {
                    switch (current.Parent!.Kind)
                    {
                        case OperationKind.MethodBody:
                        case OperationKind.ConstructorBody:
                            instance.Add(current.Parent);
                            break;
                        case OperationKind.ExpressionStatement:
                            instance.Add(current.Parent!.Parent);
                            break;
                    }
                    flag = false;
                }
                instance.AddRange(current.DescendantsAndSelf());
            }
            return instance.ToImmutableAndFree();
        }
    }
}
