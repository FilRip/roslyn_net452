using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class CompilationWithAnalyzers
    {
        private readonly Compilation _compilation;

        private readonly AnalyzerDriver.CompilationData _compilationData;

        private readonly ImmutableArray<DiagnosticAnalyzer> _analyzers;

        private readonly CompilationWithAnalyzersOptions _analysisOptions;

        private readonly CancellationToken _cancellationToken;

        private readonly AnalyzerManager _analyzerManager;

        private readonly ObjectPool<AnalyzerDriver> _driverPool;

        private readonly AnalysisState _analysisState;

        private readonly AnalysisResultBuilder _analysisResultBuilder;

        private readonly ConcurrentSet<Diagnostic> _exceptionDiagnostics = new ConcurrentSet<Diagnostic>();

        private readonly object _executingTasksLock = new object();

        private readonly Dictionary<SourceOrAdditionalFile, Tuple<Task, CancellationTokenSource>>? _executingConcurrentTreeTasksOpt;

        private Tuple<Task, CancellationTokenSource>? _executingCompilationOrNonConcurrentTreeTask;

        private int _currentToken;

        private readonly Dictionary<Task, int>? _concurrentTreeTaskTokensOpt;

        private readonly ObjectPool<AsyncQueue<CompilationEvent>> _eventQueuePool = new ObjectPool<AsyncQueue<CompilationEvent>>(() => new AsyncQueue<CompilationEvent>());

        private static readonly AsyncQueue<CompilationEvent> s_EmptyEventQueue = new AsyncQueue<CompilationEvent>();

        public Compilation Compilation => _compilation;

        public ImmutableArray<DiagnosticAnalyzer> Analyzers => _analyzers;

        public CompilationWithAnalyzersOptions AnalysisOptions => _analysisOptions;

        public CancellationToken CancellationToken => _cancellationToken;

        public CompilationWithAnalyzers(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerOptions? options, CancellationToken cancellationToken)
            : this(compilation, analyzers, new CompilationWithAnalyzersOptions(options, null, concurrentAnalysis: true, logAnalyzerExecutionTime: true, reportSuppressedDiagnostics: false, null), cancellationToken)
        {
        }

        public CompilationWithAnalyzers(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, CompilationWithAnalyzersOptions analysisOptions)
            : this(compilation, analyzers, analysisOptions, CancellationToken.None)
        {
        }

        private CompilationWithAnalyzers(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, CompilationWithAnalyzersOptions analysisOptions, CancellationToken cancellationToken)
        {
            CompilationWithAnalyzers compilationWithAnalyzers = this;
            VerifyArguments(compilation, analyzers, analysisOptions);
            compilation = compilation.WithOptions(compilation.Options.WithReportSuppressedDiagnostics(analysisOptions.ReportSuppressedDiagnostics)).WithSemanticModelProvider(new CachingSemanticModelProvider()).WithEventQueue(new AsyncQueue<CompilationEvent>());
            _compilation = compilation;
            _analyzers = analyzers;
            _analysisOptions = analysisOptions;
            _cancellationToken = cancellationToken;
            _compilationData = new AnalyzerDriver.CompilationData(_compilation);
            _analysisState = new AnalysisState(analyzers, _compilationData.SemanticModelProvider, _compilation.Options);
            _analysisResultBuilder = new AnalysisResultBuilder(analysisOptions.LogAnalyzerExecutionTime, analyzers, _analysisOptions.Options?.AdditionalFiles ?? ImmutableArray<AdditionalText>.Empty);
            _analyzerManager = new AnalyzerManager(analyzers);
            _driverPool = new ObjectPool<AnalyzerDriver>(() => compilationWithAnalyzers._compilation.CreateAnalyzerDriver(analyzers, compilationWithAnalyzers._analyzerManager, SeverityFilter.None));
            _executingConcurrentTreeTasksOpt = (analysisOptions.ConcurrentAnalysis ? new Dictionary<SourceOrAdditionalFile, Tuple<Task, CancellationTokenSource>>() : null);
            _concurrentTreeTaskTokensOpt = (analysisOptions.ConcurrentAnalysis ? new Dictionary<Task, int>() : null);
            _executingCompilationOrNonConcurrentTreeTask = null;
        }

        private static void VerifyArguments(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers, CompilationWithAnalyzersOptions analysisOptions)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }
            if (analysisOptions == null)
            {
                throw new ArgumentNullException("analysisOptions");
            }
            VerifyAnalyzersArgumentForStaticApis(analyzers);
        }

        private static void VerifyAnalyzersArgumentForStaticApis(ImmutableArray<DiagnosticAnalyzer> analyzers, bool allowDefaultOrEmpty = false)
        {
            if (analyzers.IsDefaultOrEmpty)
            {
                if (!allowDefaultOrEmpty)
                {
                    throw new ArgumentException(CodeAnalysisResources.ArgumentCannotBeEmpty, "analyzers");
                }
                return;
            }
            if (analyzers.Any((DiagnosticAnalyzer a) => a == null))
            {
                throw new ArgumentException(CodeAnalysisResources.ArgumentElementCannotBeNull, "analyzers");
            }
            if (analyzers.Distinct().Length != analyzers.Length)
            {
                throw new ArgumentException(CodeAnalysisResources.DuplicateAnalyzerInstances, "analyzers");
            }
        }

        private void VerifyAnalyzerArgument(DiagnosticAnalyzer analyzer)
        {
            VerifyAnalyzerArgumentForStaticApis(analyzer);
            if (!_analyzers.Contains(analyzer))
            {
                throw new ArgumentException(CodeAnalysisResources.UnsupportedAnalyzerInstance, "analyzer");
            }
        }

        private static void VerifyAnalyzerArgumentForStaticApis(DiagnosticAnalyzer analyzer)
        {
            if (analyzer == null)
            {
                throw new ArgumentException(CodeAnalysisResources.ArgumentCannotBeEmpty, "analyzer");
            }
        }

        private void VerifyExistingAnalyzersArgument(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            VerifyAnalyzersArgumentForStaticApis(analyzers);
            if (analyzers.Any((DiagnosticAnalyzer a) => !_analyzers.Contains(a)))
            {
                throw new ArgumentException(CodeAnalysisResources.UnsupportedAnalyzerInstance, "_analyzers");
            }
            if (analyzers.Distinct().Length != analyzers.Length)
            {
                throw new ArgumentException(CodeAnalysisResources.DuplicateAnalyzerInstances, "analyzers");
            }
        }

        private void VerifyModel(SemanticModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (!_compilation.ContainsSyntaxTree(model.SyntaxTree))
            {
                throw new ArgumentException(CodeAnalysisResources.InvalidTree, "model");
            }
        }

        private void VerifyTree(SyntaxTree tree)
        {
            if (tree == null)
            {
                throw new ArgumentNullException("tree");
            }
            if (!_compilation.ContainsSyntaxTree(tree))
            {
                throw new ArgumentException(CodeAnalysisResources.InvalidTree, "tree");
            }
        }

        private void VerifyAdditionalFile(AdditionalText file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (_analysisOptions.Options == null || !_analysisOptions.Options!.AdditionalFiles.Contains(file))
            {
                throw new ArgumentException(CodeAnalysisResources.InvalidAdditionalFile, "file");
            }
        }

        public Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync()
        {
            return GetAnalyzerDiagnosticsAsync(_cancellationToken);
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(CancellationToken cancellationToken)
        {
            return await GetAnalyzerDiagnosticsWithoutStateTrackingAsync(Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalyzerDiagnosticsWithoutStateTrackingAsync(analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<AnalysisResult> GetAnalysisResultAsync(CancellationToken cancellationToken)
        {
            return await GetAnalysisResultWithoutStateTrackingAsync(Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<AnalysisResult> GetAnalysisResultAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalysisResultWithoutStateTrackingAsync(analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync()
        {
            return GetAllDiagnosticsAsync(_cancellationToken);
        }

        public async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync(CancellationToken cancellationToken)
        {
            return (await GetAllDiagnosticsWithoutStateTrackingAsync(Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).AddRange(_exceptionDiagnostics);
        }

        [Obsolete("This API was found to have performance issues and hence has been deprecated. Instead, invoke the API 'GetAnalysisResultAsync' and access the property 'CompilationDiagnostics' on the returned 'AnalysisResult' to fetch the compilation diagnostics.")]
        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerCompilationDiagnosticsAsync(CancellationToken cancellationToken)
        {
            return await GetAnalyzerCompilationDiagnosticsCoreAsync(Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        [Obsolete("This API was found to have performance issues and hence has been deprecated. Instead, invoke the API 'GetAnalysisResultAsync' and access the property 'CompilationDiagnostics' on the returned 'AnalysisResult' to fetch the compilation diagnostics.")]
        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerCompilationDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalyzerCompilationDiagnosticsCoreAsync(analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task<ImmutableArray<Diagnostic>> GetAnalyzerCompilationDiagnosticsCoreAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            await WaitForActiveAnalysisTasksAsync(waitForTreeTasks: true, waitForCompilationOrNonConcurrentTask: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            _ = ImmutableArray<Diagnostic>.Empty;
            bool hasAllAnalyzers = analyzers.Length == Analyzers.Length;
            AnalysisScope analysisScope = new AnalysisScope(_compilation, _analysisOptions.Options, analyzers, hasAllAnalyzers, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            ImmutableArray<CompilationEvent> getPendingEventsOpt() => _analysisState.GetPendingEvents(analyzers, includeSourceEvents: true, includeNonSourceEvents: true, cancellationToken);
            await ComputeAnalyzerDiagnosticsAsync(analysisScope, getPendingEventsOpt, 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.GetDiagnostics(analysisScope, getLocalDiagnostics: false, getNonLocalDiagnostics: true);
        }

        private async Task<AnalysisResult> GetAnalysisResultWithoutStateTrackingAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            await ComputeAnalyzerDiagnosticsWithoutStateTrackingAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.ToAnalysisResult(analyzers, cancellationToken);
        }

        private async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsWithoutStateTrackingAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            await ComputeAnalyzerDiagnosticsWithoutStateTrackingAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            bool hasAllAnalyzers = analyzers.Length == Analyzers.Length;
            AnalysisScope analysisScope = new AnalysisScope(_compilation, _analysisOptions.Options, analyzers, hasAllAnalyzers, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            return _analysisResultBuilder.GetDiagnostics(analysisScope, getLocalDiagnostics: true, getNonLocalDiagnostics: true);
        }

        private static AnalyzerDriver CreateDriverForComputingDiagnosticsWithoutStateTracking(Compilation compilation, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            return compilation.CreateAnalyzerDriver(analyzers, new AnalyzerManager(analyzers), SeverityFilter.None);
        }

        private async Task ComputeAnalyzerDiagnosticsWithoutStateTrackingAsync(CancellationToken cancellationToken)
        {
            ImmutableArray<DiagnosticAnalyzer> analyzers = _analysisResultBuilder.GetPendingAnalyzers(Analyzers);
            if (analyzers.IsEmpty)
            {
                return;
            }
            AsyncQueue<CompilationEvent> eventQueue = _eventQueuePool.Allocate();
            AnalyzerDriver driver = null;
            try
            {
                Compilation compilation = _compilation.WithEventQueue(eventQueue);
                AnalyzerDriver.CompilationData compilationData = new AnalyzerDriver.CompilationData(compilation);
                bool categorizeDiagnostics = true;
                driver = CreateDriverForComputingDiagnosticsWithoutStateTracking(compilation, analyzers);
                driver.Initialize(compilation, _analysisOptions, compilationData, categorizeDiagnostics, cancellationToken);
                bool hasAllAnalyzers = analyzers.Length == Analyzers.Length;
                AnalysisScope analysisScope = new AnalysisScope(compilation, _analysisOptions.Options, analyzers, hasAllAnalyzers, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics);
                driver.AttachQueueAndStartProcessingEvents(compilation.EventQueue, analysisScope, cancellationToken);
                compilation.GetDiagnostics(cancellationToken);
                await driver.WhenCompletedTask.ConfigureAwait(continueOnCapturedContext: false);
                Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts> analyzerActionCounts = new Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts>(analyzers.Length);
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticAnalyzer analyzer2 = enumerator.Current;
                    AnalyzerActionCounts value = await driver.GetAnalyzerActionCountsAsync(analyzer2, compilation.Options, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    analyzerActionCounts.Add(analyzer2, value);
                }
                AnalyzerActionCounts getAnalyzerActionCounts(DiagnosticAnalyzer analyzer) => analyzerActionCounts[analyzer];
                _analysisResultBuilder.ApplySuppressionsAndStoreAnalysisResult(analysisScope, driver, compilation, getAnalyzerActionCounts, fullAnalysisResultForAnalyzersInScope: true);
            }
            finally
            {
                driver?.Dispose();
                FreeEventQueue(eventQueue, _eventQueuePool);
            }
        }

        private async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsWithoutStateTrackingAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            AsyncQueue<CompilationEvent> eventQueue = _eventQueuePool.Allocate();
            AnalyzerDriver driver = null;
            try
            {
                Compilation compilation = _compilation.WithEventQueue(eventQueue);
                AnalyzerDriver.CompilationData compilationData = new AnalyzerDriver.CompilationData(compilation);
                bool categorizeDiagnostics = false;
                driver = CreateDriverForComputingDiagnosticsWithoutStateTracking(compilation, analyzers);
                driver.Initialize(compilation, _analysisOptions, compilationData, categorizeDiagnostics, cancellationToken);
                bool hasAllAnalyzers = analyzers.Length == Analyzers.Length;
                AnalysisScope analysisScope = new AnalysisScope(compilation, _analysisOptions.Options, analyzers, hasAllAnalyzers, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics);
                driver.AttachQueueAndStartProcessingEvents(compilation.EventQueue, analysisScope, cancellationToken);
                ImmutableArray<Diagnostic> reportedDiagnostics = compilation.GetDiagnostics(cancellationToken).AddRange(await driver.GetDiagnosticsAsync(compilation).ConfigureAwait(continueOnCapturedContext: false));
                return driver.ApplyProgrammaticSuppressions(reportedDiagnostics, compilation);
            }
            finally
            {
                driver?.Dispose();
                FreeEventQueue(eventQueue, _eventQueuePool);
            }
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerSyntaxDiagnosticsAsync(SyntaxTree tree, CancellationToken cancellationToken)
        {
            VerifyTree(tree);
            return await GetAnalyzerSyntaxDiagnosticsCoreAsync(tree, Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerSyntaxDiagnosticsAsync(SyntaxTree tree, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyTree(tree);
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalyzerSyntaxDiagnosticsCoreAsync(tree, analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Task<AnalysisResult> GetAnalysisResultAsync(SyntaxTree tree, CancellationToken cancellationToken)
        {
            VerifyTree(tree);
            return GetAnalysisResultCoreAsync(new SourceOrAdditionalFile(tree), Analyzers, cancellationToken);
        }

        public Task<AnalysisResult> GetAnalysisResultAsync(SyntaxTree tree, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyTree(tree);
            VerifyExistingAnalyzersArgument(analyzers);
            return GetAnalysisResultCoreAsync(new SourceOrAdditionalFile(tree), analyzers, cancellationToken);
        }

        public async Task<AnalysisResult> GetAnalysisResultAsync(AdditionalText file, CancellationToken cancellationToken)
        {
            VerifyAdditionalFile(file);
            return await GetAnalysisResultCoreAsync(new SourceOrAdditionalFile(file), Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<AnalysisResult> GetAnalysisResultAsync(AdditionalText file, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyAdditionalFile(file);
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalysisResultCoreAsync(new SourceOrAdditionalFile(file), analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task<AnalysisResult> GetAnalysisResultCoreAsync(SourceOrAdditionalFile file, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            AnalysisScope analysisScope = new AnalysisScope(analyzers, file, null, isSyntacticSingleFileAnalysis: true, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            await ComputeAnalyzerSyntaxDiagnosticsAsync(analysisScope, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.ToAnalysisResult(analyzers, cancellationToken);
        }

        private async Task<ImmutableArray<Diagnostic>> GetAnalyzerSyntaxDiagnosticsCoreAsync(SyntaxTree tree, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            AnalysisScope analysisScope = new AnalysisScope(analyzers, new SourceOrAdditionalFile(tree), null, isSyntacticSingleFileAnalysis: true, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            await ComputeAnalyzerSyntaxDiagnosticsAsync(analysisScope, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.GetDiagnostics(analysisScope, getLocalDiagnostics: true, getNonLocalDiagnostics: false);
        }

        private async Task ComputeAnalyzerSyntaxDiagnosticsAsync(AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            try
            {
                int newTaskToken = Interlocked.Increment(ref _currentToken);
                ImmutableArray<DiagnosticAnalyzer> pendingAnalyzers = _analysisResultBuilder.GetPendingAnalyzers(analysisScope.Analyzers);
                if (pendingAnalyzers.Length > 0)
                {
                    AnalysisScope analysisScope2 = ((pendingAnalyzers.Length < analysisScope.Analyzers.Length) ? analysisScope.WithAnalyzers(pendingAnalyzers, hasAllAnalyzers: false) : analysisScope);
                    await ComputeAnalyzerDiagnosticsAsync(analysisScope2, null, newTaskToken, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerSemanticDiagnosticsAsync(SemanticModel model, TextSpan? filterSpan, CancellationToken cancellationToken)
        {
            VerifyModel(model);
            return await GetAnalyzerSemanticDiagnosticsCoreAsync(model, filterSpan, Analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ImmutableArray<Diagnostic>> GetAnalyzerSemanticDiagnosticsAsync(SemanticModel model, TextSpan? filterSpan, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyModel(model);
            VerifyExistingAnalyzersArgument(analyzers);
            return await GetAnalyzerSemanticDiagnosticsCoreAsync(model, filterSpan, analyzers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Task<AnalysisResult> GetAnalysisResultAsync(SemanticModel model, TextSpan? filterSpan, CancellationToken cancellationToken)
        {
            VerifyModel(model);
            return GetAnalysisResultCoreAsync(model, filterSpan, Analyzers, cancellationToken);
        }

        public Task<AnalysisResult> GetAnalysisResultAsync(SemanticModel model, TextSpan? filterSpan, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            VerifyModel(model);
            VerifyExistingAnalyzersArgument(analyzers);
            return GetAnalysisResultCoreAsync(model, filterSpan, analyzers, cancellationToken);
        }

        private async Task<AnalysisResult> GetAnalysisResultCoreAsync(SemanticModel model, TextSpan? filterSpan, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            AnalysisScope analysisScope = new AnalysisScope(analyzers, new SourceOrAdditionalFile(model.SyntaxTree), filterSpan, isSyntacticSingleFileAnalysis: false, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            await ComputeAnalyzerSemanticDiagnosticsAsync(model, analysisScope, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.ToAnalysisResult(analyzers, cancellationToken);
        }

        private async Task<ImmutableArray<Diagnostic>> GetAnalyzerSemanticDiagnosticsCoreAsync(SemanticModel model, TextSpan? filterSpan, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            AnalysisScope analysisScope = new AnalysisScope(analyzers, new SourceOrAdditionalFile(model.SyntaxTree), filterSpan, isSyntacticSingleFileAnalysis: false, _analysisOptions.ConcurrentAnalysis, categorizeDiagnostics: true);
            await ComputeAnalyzerSemanticDiagnosticsAsync(model, analysisScope, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            return _analysisResultBuilder.GetDiagnostics(analysisScope, getLocalDiagnostics: true, getNonLocalDiagnostics: false);
        }

        private async Task ComputeAnalyzerSemanticDiagnosticsAsync(SemanticModel model, AnalysisScope analysisScope, CancellationToken cancellationToken, bool forceCompletePartialTrees = true)
        {
            AnalysisScope analysisScope2 = analysisScope;
            SemanticModel model2 = model;
            try
            {
                int newTaskToken = Interlocked.Increment(ref _currentToken);
                ImmutableArray<DiagnosticAnalyzer> pendingAnalyzers = _analysisResultBuilder.GetPendingAnalyzers(analysisScope2.Analyzers);
                if (pendingAnalyzers.Length > 0)
                {
                    AnalysisScope analysisScope3 = ((pendingAnalyzers.Length < analysisScope2.Analyzers.Length) ? analysisScope2.WithAnalyzers(pendingAnalyzers, hasAllAnalyzers: false) : analysisScope2);
                    ImmutableArray<CompilationEvent> getPendingEventsOpt() => _analysisState.GetPendingEvents(analysisScope2.Analyzers, model2.SyntaxTree, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    (ImmutableArray<CompilationEvent>, bool) obj = await ComputeAnalyzerDiagnosticsAsync(analysisScope3, getPendingEventsOpt, newTaskToken, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    var (compilationEvents2, _) = obj;
                    if (obj.Item2 && forceCompletePartialTrees)
                    {
                        await processPartialSymbolLocationsAsync(compilationEvents2, analysisScope2).ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception))
            {
                throw ExceptionUtilities.Unreachable;
            }
            async Task processPartialSymbolLocationsAsync(ImmutableArray<CompilationEvent> compilationEvents, AnalysisScope analysisScope)
            {
                AnalysisScope analysisScope4 = analysisScope;
                if (!compilationEvents.IsDefaultOrEmpty && (!analysisScope4.FilterSpanOpt.HasValue || analysisScope4.ContainsSpan(model2.SyntaxTree.GetRoot(cancellationToken).Span)))
                {
                    HashSet<SyntaxTree> hashSet = null;
                    ImmutableArray<CompilationEvent>.Enumerator enumerator = compilationEvents.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current is SymbolDeclaredCompilationEvent symbolDeclaredCompilationEvent && symbolDeclaredCompilationEvent.Symbol.Kind != SymbolKind.Namespace && symbolDeclaredCompilationEvent.Symbol.Locations.Length > 1)
                        {
                            ImmutableArray<Location>.Enumerator enumerator2 = symbolDeclaredCompilationEvent.Symbol.Locations.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Location current = enumerator2.Current;
                                if (current.SourceTree != null && current.SourceTree != model2.SyntaxTree)
                                {
                                    hashSet = hashSet ?? new HashSet<SyntaxTree>();
                                    hashSet.Add(current.SourceTree);
                                }
                            }
                        }
                    }
                    if (hashSet != null)
                    {
                        if (AnalysisOptions.ConcurrentAnalysis)
                        {
                            await Task.WhenAll(hashSet.Select((SyntaxTree tree) => Task.Run(delegate
                            {
                                SemanticModel semanticModel2 = _compilation.GetSemanticModel(tree);
                                analysisScope4 = new AnalysisScope(analysisScope4.Analyzers, new SourceOrAdditionalFile(tree), null, isSyntacticSingleFileAnalysis: false, analysisScope4.ConcurrentAnalysis, analysisScope4.CategorizeDiagnostics);
                                return ComputeAnalyzerSemanticDiagnosticsAsync(semanticModel2, analysisScope4, cancellationToken, forceCompletePartialTrees: false);
                            }, cancellationToken))).ConfigureAwait(continueOnCapturedContext: false);
                        }
                        else
                        {
                            foreach (SyntaxTree item in hashSet)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                SemanticModel semanticModel = _compilation.GetSemanticModel(item);
                                analysisScope4 = new AnalysisScope(analysisScope4.Analyzers, new SourceOrAdditionalFile(item), null, isSyntacticSingleFileAnalysis: false, analysisScope4.ConcurrentAnalysis, analysisScope4.CategorizeDiagnostics);
                                await ComputeAnalyzerSemanticDiagnosticsAsync(semanticModel, analysisScope4, cancellationToken, forceCompletePartialTrees: false).ConfigureAwait(continueOnCapturedContext: false);
                            }
                        }
                    }
                }
            }
        }

        private async Task<(ImmutableArray<CompilationEvent> events, bool hasSymbolStartActions)> ComputeAnalyzerDiagnosticsAsync(AnalysisScope analysisScope, Func<ImmutableArray<CompilationEvent>>? getPendingEventsOpt, int newTaskToken, CancellationToken cancellationToken)
        {
            try
            {
                AnalyzerDriver? driver = null;
                Task? computeTask = null;
                CancellationTokenSource cancellationSource;

                try
                {
                    // Get the analyzer driver to execute analysis.
                    driver = await GetAnalyzerDriverAsync(cancellationToken).ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();

                    GenerateCompilationEvents(analysisScope, cancellationToken);

                    await PopulateEventsCacheAsync(cancellationToken).ConfigureAwait(false);

                    // Track if this task was suspended by another tree diagnostics request for the same tree.
                    // If so, we wait for the high priority requests to complete before restarting analysis.
                    bool suspended;
                    var pendingEvents = ImmutableArray<CompilationEvent>.Empty;
                    do
                    {
                        suspended = false;

                        // Create a new cancellation source to allow higher priority requests to suspend our analysis.
                        using (cancellationSource = new CancellationTokenSource())
                        {
                            // Link the cancellation source with client supplied cancellation source, so the public API callee can also cancel analysis.
                            using var linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationSource.Token, cancellationToken);

                            try
                            {
                                // Fetch the cancellation token here to avoid capturing linkedCts in the getComputeTask lambda as the task may run after linkedCts has been disposed due to cancellation.
                                var linkedCancellationToken = linkedCancellationSource.Token;

                                // Core task to compute analyzer diagnostics.
                                Tuple<Task, CancellationTokenSource> getComputeTask() => Tuple.Create(
                                    Task.Run(async () =>
                                    {
                                        try
                                        {
                                            AsyncQueue<CompilationEvent> eventQueue = s_EmptyEventQueue;
                                            try
                                            {
                                                // Get event queue with pending events to analyze.
                                                if (getPendingEventsOpt != null)
                                                {
                                                    pendingEvents = getPendingEventsOpt();
                                                    eventQueue = CreateEventsQueue(pendingEvents);
                                                }

                                                linkedCancellationToken.ThrowIfCancellationRequested();

                                                // Execute analyzer driver on the given analysis scope with the given event queue.
                                                await ComputeAnalyzerDiagnosticsCoreAsync(driver, eventQueue, analysisScope, cancellationToken: linkedCancellationToken).ConfigureAwait(false);
                                            }
                                            finally
                                            {
                                                FreeEventQueue(eventQueue, _eventQueuePool);
                                            }
                                        }
                                        catch (Exception e) when (FatalError.ReportAndPropagateUnlessCanceled(e))
                                        {
                                            throw ExceptionUtilities.Unreachable;
                                        }
                                    }, linkedCancellationToken),
                                    cancellationSource);

                                // Wait for higher priority tree document tasks to complete.
                                computeTask = await SetActiveAnalysisTaskAsync(getComputeTask, analysisScope.FilterFileOpt, newTaskToken, cancellationToken).ConfigureAwait(false);

                                cancellationToken.ThrowIfCancellationRequested();

                                await computeTask.ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                if (!cancellationSource.IsCancellationRequested)
                                {
                                    throw;
                                }

                                suspended = true;
                            }
                            finally
                            {
                                ClearExecutingTask(computeTask, analysisScope.FilterFileOpt);
                                computeTask = null;
                            }
                        }
                    } while (suspended);

                    return (pendingEvents, hasSymbolStartActions: driver?.HasSymbolStartedActions(analysisScope) ?? false);
                }
                finally
                {
                    FreeDriver(driver);
                }
            }
            catch (Exception e) when (FatalError.ReportAndPropagateUnlessCanceled(e, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private void GenerateCompilationEvents(AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            if (!analysisScope.FilterFileOpt.HasValue)
            {
                _compilation.GetDiagnostics(cancellationToken);
            }
            else if (!analysisScope.IsSyntacticSingleFileAnalysis)
            {
                _compilation.GetSemanticModel(analysisScope.FilterFileOpt.Value.SourceTree).GetDiagnostics(null, cancellationToken);
            }
        }

        private async Task PopulateEventsCacheAsync(CancellationToken cancellationToken)
        {
            AsyncQueue<CompilationEvent>? eventQueue2 = _compilation.EventQueue;
            if (eventQueue2 == null || eventQueue2!.Count <= 0)
            {
                return;
            }
            AnalyzerDriver driver = null;
            try
            {
                driver = await GetAnalyzerDriverAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                cancellationToken.ThrowIfCancellationRequested();
                static ImmutableArray<CompilationEvent> getCompilationEvents(AsyncQueue<CompilationEvent> eventQueue, ImmutableArray<AdditionalText> additionalFiles) => dequeueGeneratedCompilationEvents(eventQueue, additionalFiles);
                ImmutableArray<AdditionalText> additionalFiles2 = _analysisOptions.Options?.AdditionalFiles ?? ImmutableArray<AdditionalText>.Empty;
                await _analysisState.OnCompilationEventsGeneratedAsync(getCompilationEvents, _compilation.EventQueue, additionalFiles2, driver, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                FreeDriver(driver);
            }
            static ImmutableArray<CompilationEvent> dequeueGeneratedCompilationEvents(AsyncQueue<CompilationEvent> eventQueue, ImmutableArray<AdditionalText> additionalFiles)
            {
                ImmutableArray<CompilationEvent>.Builder builder = ImmutableArray.CreateBuilder<CompilationEvent>();
                while (eventQueue.TryDequeue(out CompilationEvent d))
                {
                    if (d is CompilationStartedEvent compilationStartedEvent && !additionalFiles.IsEmpty)
                    {
                        d = compilationStartedEvent.WithAdditionalFiles(additionalFiles);
                    }
                    builder.Add(d);
                }
                return builder.ToImmutable();
            }
        }

        private async Task<AnalyzerDriver> GetAnalyzerDriverAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                AnalyzerDriver driver = _driverPool.Allocate();
                bool success = false;
                try
                {
                    if (!driver.IsInitialized)
                    {
                        driver.Initialize(_compilation, _analysisOptions, _compilationData, categorizeDiagnostics: true, cancellationToken);
                    }
                    await driver.WhenInitializedTask.ConfigureAwait(continueOnCapturedContext: false);
                    success = true;
                    return driver;
                }
                finally
                {
                    if (!success)
                    {
                        FreeDriver(driver);
                    }
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private void FreeDriver(AnalyzerDriver? driver)
        {
            if (driver != null && driver!.IsInitialized && !driver!.WhenInitializedTask.IsCanceled)
            {
                _driverPool.Free(driver);
            }
        }

        private async Task ComputeAnalyzerDiagnosticsCoreAsync(AnalyzerDriver driver, AsyncQueue<CompilationEvent> eventQueue, AnalysisScope analysisScope, CancellationToken cancellationToken)
        {
            try
            {
                if (eventQueue.Count > 0 || _analysisState.HasPendingSyntaxAnalysis(analysisScope))
                {
                    try
                    {
                        await driver.AttachQueueAndProcessAllEventsAsync(eventQueue, analysisScope, _analysisState, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                        return;
                    }
                    finally
                    {
                        _analysisResultBuilder.ApplySuppressionsAndStoreAnalysisResult(analysisScope, driver, _compilation, _analysisState.GetAnalyzerActionCounts, fullAnalysisResultForAnalyzersInScope: false);
                    }
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private Task<Task> SetActiveAnalysisTaskAsync(Func<Tuple<Task, CancellationTokenSource>> getNewAnalysisTask, SourceOrAdditionalFile? fileOpt, int newTaskToken, CancellationToken cancellationToken)
        {
            if (fileOpt.HasValue)
            {
                return SetActiveTreeAnalysisTaskAsync(getNewAnalysisTask, fileOpt.Value, newTaskToken, cancellationToken);
            }
            return SetActiveCompilationAnalysisTaskAsync(getNewAnalysisTask, cancellationToken);
        }

        private async Task<Task> SetActiveCompilationAnalysisTaskAsync(Func<Tuple<Task, CancellationTokenSource>> getNewCompilationTask, CancellationToken cancellationToken)
        {
            while (true)
            {
                await WaitForActiveAnalysisTasksAsync(waitForTreeTasks: true, waitForCompilationOrNonConcurrentTask: true, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                lock (_executingTasksLock)
                {
                    if ((_executingConcurrentTreeTasksOpt == null || _executingConcurrentTreeTasksOpt!.Count == 0) && _executingCompilationOrNonConcurrentTreeTask == null)
                    {
                        _executingCompilationOrNonConcurrentTreeTask = getNewCompilationTask();
                        return _executingCompilationOrNonConcurrentTreeTask!.Item1;
                    }
                }
            }
        }

        private async Task WaitForActiveAnalysisTasksAsync(bool waitForTreeTasks, bool waitForCompilationOrNonConcurrentTask, CancellationToken cancellationToken)
        {
            ArrayBuilder<Tuple<Task, CancellationTokenSource>> executingTasks = ArrayBuilder<Tuple<Task, CancellationTokenSource>>.GetInstance();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lock (_executingTasksLock)
                {
                    if (waitForTreeTasks)
                    {
                        Dictionary<SourceOrAdditionalFile, Tuple<Task, CancellationTokenSource>>? executingConcurrentTreeTasksOpt = _executingConcurrentTreeTasksOpt;
                        if (executingConcurrentTreeTasksOpt != null && executingConcurrentTreeTasksOpt!.Count > 0)
                        {
                            executingTasks.AddRange(_executingConcurrentTreeTasksOpt!.Values);
                        }
                    }
                    if (waitForCompilationOrNonConcurrentTask && _executingCompilationOrNonConcurrentTreeTask != null)
                    {
                        executingTasks.Add(_executingCompilationOrNonConcurrentTreeTask);
                    }
                }
                if (executingTasks.Count == 0)
                {
                    break;
                }
                ArrayBuilder<Tuple<Task, CancellationTokenSource>>.Enumerator enumerator = executingTasks.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Tuple<Task, CancellationTokenSource> current = enumerator.Current;
                    cancellationToken.ThrowIfCancellationRequested();
                    await WaitForExecutingTaskAsync(current.Item1, alwaysYield: false).ConfigureAwait(continueOnCapturedContext: false);
                }
                executingTasks.Clear();
            }
            executingTasks.Free();
        }

        private async Task<Task> SetActiveTreeAnalysisTaskAsync(Func<Tuple<Task, CancellationTokenSource>> getNewTreeAnalysisTask, SourceOrAdditionalFile tree, int newTaskToken, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    Tuple<Task, CancellationTokenSource> value = null;
                    lock (_executingTasksLock)
                    {
                        if (!_analysisOptions.ConcurrentAnalysis)
                        {
                            if (_executingCompilationOrNonConcurrentTreeTask != null)
                            {
                                SuspendAnalysis_NoLock(_executingCompilationOrNonConcurrentTreeTask!.Item1, _executingCompilationOrNonConcurrentTreeTask!.Item2);
                                _executingCompilationOrNonConcurrentTreeTask = null;
                            }
                            return (_executingCompilationOrNonConcurrentTreeTask = getNewTreeAnalysisTask())!.Item1;
                        }
                        if (!_executingConcurrentTreeTasksOpt!.TryGetValue(tree, out value) || _concurrentTreeTaskTokensOpt![value.Item1] < newTaskToken)
                        {
                            if (value != null)
                            {
                                SuspendAnalysis_NoLock(value.Item1, value.Item2);
                            }
                            if (_executingCompilationOrNonConcurrentTreeTask != null)
                            {
                                SuspendAnalysis_NoLock(_executingCompilationOrNonConcurrentTreeTask!.Item1, _executingCompilationOrNonConcurrentTreeTask!.Item2);
                                _executingCompilationOrNonConcurrentTreeTask = null;
                            }
                            Tuple<Task, CancellationTokenSource> tuple = getNewTreeAnalysisTask();
                            _concurrentTreeTaskTokensOpt![tuple.Item1] = newTaskToken;
                            _executingConcurrentTreeTasksOpt![tree] = tuple;
                            return tuple.Item1;
                        }
                    }
                    await WaitForExecutingTaskAsync(value.Item1, alwaysYield: true).ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private static async Task WaitForExecutingTaskAsync(Task executingTask, bool alwaysYield)
        {
            if (executingTask.IsCompleted)
            {
                if (alwaysYield)
                {
                    await Task.Yield().ConfigureAwait(continueOnCapturedContext: false);
                }
                return;
            }
            try
            {
                await executingTask.ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void SuspendAnalysis_NoLock(Task computeTask, CancellationTokenSource cts)
        {
            if (!computeTask.IsCompleted)
            {
                cts.Cancel();
            }
        }

        private void ClearExecutingTask(Task? computeTask, SourceOrAdditionalFile? fileOpt)
        {
            if (computeTask == null)
            {
                return;
            }
            lock (_executingTasksLock)
            {
                if (fileOpt.HasValue && _analysisOptions.ConcurrentAnalysis)
                {
                    if (_executingConcurrentTreeTasksOpt!.TryGetValue(fileOpt.Value, out var value) && value.Item1 == computeTask)
                    {
                        _executingConcurrentTreeTasksOpt!.Remove(fileOpt.Value);
                    }
                    if (_concurrentTreeTaskTokensOpt!.ContainsKey(computeTask))
                    {
                        _concurrentTreeTaskTokensOpt!.Remove(computeTask);
                    }
                }
                else if (_executingCompilationOrNonConcurrentTreeTask?.Item1 == computeTask)
                {
                    _executingCompilationOrNonConcurrentTreeTask = null;
                }
            }
        }

        private AsyncQueue<CompilationEvent> CreateEventsQueue(ImmutableArray<CompilationEvent> compilationEvents)
        {
            if (compilationEvents.IsEmpty)
            {
                return s_EmptyEventQueue;
            }
            AsyncQueue<CompilationEvent> asyncQueue = _eventQueuePool.Allocate();
            ImmutableArray<CompilationEvent>.Enumerator enumerator = compilationEvents.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CompilationEvent current = enumerator.Current;
                asyncQueue.TryEnqueue(current);
            }
            return asyncQueue;
        }

        private static void FreeEventQueue(AsyncQueue<CompilationEvent> eventQueue, ObjectPool<AsyncQueue<CompilationEvent>> eventQueuePool)
        {
            if (eventQueue == null || eventQueue == s_EmptyEventQueue)
            {
                return;
            }
            if (eventQueue.Count > 0)
            {
                while (eventQueue.TryDequeue(out CompilationEvent d))
                {
                }
            }
            if (!eventQueue.IsCompleted)
            {
                eventQueuePool.Free(eventQueue);
            }
        }

        public static IEnumerable<Diagnostic> GetEffectiveDiagnostics(IEnumerable<Diagnostic> diagnostics, Compilation compilation)
        {
            return GetEffectiveDiagnostics(diagnostics.AsImmutableOrNull(), compilation);
        }

        public static IEnumerable<Diagnostic> GetEffectiveDiagnostics(ImmutableArray<Diagnostic> diagnostics, Compilation compilation)
        {
            if (diagnostics.IsDefault)
            {
                throw new ArgumentNullException("diagnostics");
            }
            if (compilation == null)
            {
                throw new ArgumentNullException("compilation");
            }
            return GetEffectiveDiagnosticsImpl(diagnostics, compilation);
        }

        private static IEnumerable<Diagnostic> GetEffectiveDiagnosticsImpl(ImmutableArray<Diagnostic> diagnostics, Compilation compilation)
        {
            if (diagnostics.IsEmpty)
            {
                yield break;
            }
            if (compilation.SemanticModelProvider == null)
            {
                compilation = compilation.WithSemanticModelProvider(new CachingSemanticModelProvider());
            }
            SuppressMessageAttributeState suppressMessageState = new SuppressMessageAttributeState(compilation);
            ImmutableArray<Diagnostic>.Enumerator enumerator = diagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Diagnostic current = enumerator.Current;
                if (current != null)
                {
                    Diagnostic diagnostic = compilation.Options.FilterDiagnostic(current, CancellationToken.None);
                    if (diagnostic != null)
                    {
                        yield return suppressMessageState.ApplySourceSuppressions(diagnostic);
                    }
                }
            }
        }

        public static bool IsDiagnosticAnalyzerSuppressed(DiagnosticAnalyzer analyzer, CompilationOptions options, Action<Exception, DiagnosticAnalyzer, Diagnostic>? onAnalyzerException = null)
        {
            VerifyAnalyzerArgumentForStaticApis(analyzer);
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            AnalyzerManager analyzerManager = new AnalyzerManager(analyzer);
            AnalyzerExecutor analyzerExecutor = AnalyzerExecutor.CreateForSupportedDiagnostics(onAnalyzerException, analyzerManager);
            return AnalyzerDriver.IsDiagnosticAnalyzerSuppressed(analyzer, options, analyzerManager, analyzerExecutor, SeverityFilter.None);
        }

        [Obsolete("This API is no longer required to be invoked. Analyzer state is automatically cleaned up when CompilationWithAnalyzers instance is released.")]
        public static void ClearAnalyzerState(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
        }

        public async Task<AnalyzerTelemetryInfo> GetAnalyzerTelemetryInfoAsync(DiagnosticAnalyzer analyzer, CancellationToken cancellationToken)
        {
            VerifyAnalyzerArgument(analyzer);
            try
            {
                AnalyzerActionCounts actionCounts = await GetAnalyzerActionCountsAsync(analyzer, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                int suppressionActionCounts = ((analyzer is DiagnosticSuppressor) ? 1 : 0);
                TimeSpan analyzerExecutionTime = GetAnalyzerExecutionTime(analyzer);
                return new AnalyzerTelemetryInfo(actionCounts, suppressionActionCounts, analyzerExecutionTime);
            }
            catch (Exception exception) when (FatalError.ReportAndPropagateUnlessCanceled(exception, cancellationToken))
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        private async Task<AnalyzerActionCounts> GetAnalyzerActionCountsAsync(DiagnosticAnalyzer analyzer, CancellationToken cancellationToken)
        {
            AnalyzerDriver driver = null;
            try
            {
                driver = await GetAnalyzerDriverAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                cancellationToken.ThrowIfCancellationRequested();
                return await _analysisState.GetOrComputeAnalyzerActionCountsAsync(analyzer, driver, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
            }
            finally
            {
                FreeDriver(driver);
            }
        }

        private TimeSpan GetAnalyzerExecutionTime(DiagnosticAnalyzer analyzer)
        {
            if (!_analysisOptions.LogAnalyzerExecutionTime)
            {
                return default(TimeSpan);
            }
            return _analysisResultBuilder.GetAnalyzerExecutionTime(analyzer);
        }
    }
}
