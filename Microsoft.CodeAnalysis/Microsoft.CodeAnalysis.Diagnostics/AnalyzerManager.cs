using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalyzerManager
    {
        private sealed class AnalyzerExecutionContext
        {
            private readonly DiagnosticAnalyzer _analyzer;

            private readonly object _gate;

            private Dictionary<ISymbol, HashSet<ISymbol>?>? _lazyPendingMemberSymbolsMap;

            private Dictionary<ISymbol, (ImmutableArray<SymbolEndAnalyzerAction>, SymbolDeclaredCompilationEvent)>? _lazyPendingSymbolEndActionsMap;

            private Task<HostSessionStartAnalysisScope>? _lazySessionScopeTask;

            private Task<HostCompilationStartAnalysisScope>? _lazyCompilationScopeTask;

            private Dictionary<ISymbol, Task<HostSymbolStartAnalysisScope>>? _lazySymbolScopeTasks;

            private ImmutableArray<DiagnosticDescriptor> _lazyDiagnosticDescriptors;

            private ImmutableArray<SuppressionDescriptor> _lazySuppressionDescriptors;

            public AnalyzerExecutionContext(DiagnosticAnalyzer analyzer)
            {
                _analyzer = analyzer;
                _gate = new object();
            }

            public Task<HostSessionStartAnalysisScope> GetSessionAnalysisScopeAsync(AnalyzerExecutor analyzerExecutor)
            {
                lock (_gate)
                {
                    if (_lazySessionScopeTask != null)
                    {
                        return _lazySessionScopeTask;
                    }
                    return _lazySessionScopeTask = getSessionAnalysisScopeTaskSlow(this, analyzerExecutor);
                }
                static Task<HostSessionStartAnalysisScope> getSessionAnalysisScopeTaskSlow(AnalyzerExecutionContext context, AnalyzerExecutor executor)
                {
                    AnalyzerExecutor executor2 = executor;
                    AnalyzerExecutionContext context2 = context;
                    return Task.Run(delegate
                    {
                        HostSessionStartAnalysisScope hostSessionStartAnalysisScope = new HostSessionStartAnalysisScope();
                        executor2.ExecuteInitializeMethod(context2._analyzer, hostSessionStartAnalysisScope);
                        return hostSessionStartAnalysisScope;
                    }, executor2.CancellationToken);
                }
            }

            public void ClearSessionScopeTask()
            {
                lock (_gate)
                {
                    _lazySessionScopeTask = null;
                }
            }

            public Task<HostCompilationStartAnalysisScope> GetCompilationAnalysisScopeAsync(HostSessionStartAnalysisScope sessionScope, AnalyzerExecutor analyzerExecutor)
            {
                HostSessionStartAnalysisScope sessionScope2 = sessionScope;
                AnalyzerExecutor analyzerExecutor2 = analyzerExecutor;
                lock (_gate)
                {
                    if (_lazyCompilationScopeTask == null)
                    {
                        _lazyCompilationScopeTask = Task.Run(delegate
                        {
                            HostCompilationStartAnalysisScope hostCompilationStartAnalysisScope = new HostCompilationStartAnalysisScope(sessionScope2);
                            analyzerExecutor2.ExecuteCompilationStartActions(sessionScope2.GetAnalyzerActions(_analyzer).CompilationStartActions, hostCompilationStartAnalysisScope);
                            return hostCompilationStartAnalysisScope;
                        }, analyzerExecutor2.CancellationToken);
                    }
                    return _lazyCompilationScopeTask;
                }
            }

            public void ClearCompilationScopeTask()
            {
                lock (_gate)
                {
                    _lazyCompilationScopeTask = null;
                }
            }

            public Task<HostSymbolStartAnalysisScope> GetSymbolAnalysisScopeAsync(ISymbol symbol, ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions, AnalyzerExecutor analyzerExecutor)
            {
                AnalyzerExecutor analyzerExecutor2 = analyzerExecutor;
                ISymbol symbol2 = symbol;
                lock (_gate)
                {
                    if (_lazySymbolScopeTasks == null)
                    {
                        _lazySymbolScopeTasks = new Dictionary<ISymbol, Task<HostSymbolStartAnalysisScope>>();
                    }
                    if (!_lazySymbolScopeTasks!.TryGetValue(symbol2, out var value))
                    {
                        value = Task.Run(() => getSymbolAnalysisScopeCore(), analyzerExecutor2.CancellationToken);
                        _lazySymbolScopeTasks!.Add(symbol2, value);
                    }
                    return value;
                    HostSymbolStartAnalysisScope getSymbolAnalysisScopeCore()
                    {
                        HostSymbolStartAnalysisScope hostSymbolStartAnalysisScope = new HostSymbolStartAnalysisScope();
                        analyzerExecutor2.ExecuteSymbolStartActions(symbol2, _analyzer, symbolStartActions, hostSymbolStartAnalysisScope);
                        if (hostSymbolStartAnalysisScope.GetAnalyzerActions(_analyzer).SymbolEndActionsCount > 0)
                        {
                            HashSet<ISymbol> dependentSymbols = getDependentSymbols();
                            lock (_gate)
                            {
                                if (_lazyPendingMemberSymbolsMap == null)
                                {
                                    _lazyPendingMemberSymbolsMap = new Dictionary<ISymbol, HashSet<ISymbol>>();
                                }
                                _lazyPendingMemberSymbolsMap![symbol2] = dependentSymbols;
                                return hostSymbolStartAnalysisScope;
                            }
                        }
                        return hostSymbolStartAnalysisScope;
                    }
                }
                HashSet<ISymbol>? getDependentSymbols()
                {
                    HashSet<ISymbol> memberSet = null;
                    switch (symbol2.Kind)
                    {
                        case SymbolKind.NamedType:
                            processMembers(((INamedTypeSymbol)symbol2).GetMembers());
                            break;
                        case SymbolKind.Namespace:
                            processMembers(((INamespaceSymbol)symbol2).GetMembers());
                            break;
                    }
                    return memberSet;

                    void processMembers(IEnumerable<ISymbol> members)
                    {
                        foreach (ISymbol member in members)
                        {
                            if (!member.IsImplicitlyDeclared && member.IsInSource())
                            {
                                if (memberSet == null)
                                {
                                    memberSet = new HashSet<ISymbol>();
                                }
                                memberSet.Add(member);
                                if (member is IMethodSymbol methodSymbol && methodSymbol.PartialImplementationPart != null)
                                {
                                    memberSet.Add(methodSymbol.PartialImplementationPart);
                                }
                            }
                            if (member.Kind != symbol2.Kind && member is INamedTypeSymbol namedTypeSymbol)
                            {
                                processMembers(namedTypeSymbol.GetMembers());
                            }
                        }
                    }
                }
            }

            [Conditional("DEBUG")]
            private void VerifyNewEntryForPendingMemberSymbolsMap(ISymbol symbol, HashSet<ISymbol>? dependentSymbols)
            {
                if (!_lazyPendingMemberSymbolsMap!.TryGetValue(symbol, out var _))
                {
                }
            }

            public void ClearSymbolScopeTask(ISymbol symbol)
            {
                lock (_gate)
                {
                    _lazySymbolScopeTasks?.Remove(symbol);
                }
            }

            public ImmutableArray<DiagnosticDescriptor> GetOrComputeDiagnosticDescriptors(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
            {
                return GetOrComputeDescriptors(ref _lazyDiagnosticDescriptors, ComputeDiagnosticDescriptors, analyzer, analyzerExecutor);
            }

            public ImmutableArray<SuppressionDescriptor> GetOrComputeSuppressionDescriptors(DiagnosticSuppressor suppressor, AnalyzerExecutor analyzerExecutor)
            {
                return GetOrComputeDescriptors(ref _lazySuppressionDescriptors, ComputeSuppressionDescriptors, suppressor, analyzerExecutor);
            }

            private static ImmutableArray<TDescriptor> GetOrComputeDescriptors<TDescriptor>(ref ImmutableArray<TDescriptor> lazyDescriptors, Func<DiagnosticAnalyzer, AnalyzerExecutor, ImmutableArray<TDescriptor>> computeDescriptors, DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
            {
                if (!lazyDescriptors.IsDefault)
                {
                    return lazyDescriptors;
                }
                ImmutableArray<TDescriptor> value = computeDescriptors(analyzer, analyzerExecutor);
                ImmutableInterlocked.InterlockedInitialize(ref lazyDescriptors, value);
                return lazyDescriptors;
            }

            private static ImmutableArray<DiagnosticDescriptor> ComputeDiagnosticDescriptors(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
            {
                DiagnosticAnalyzer analyzer2 = analyzer;
                ImmutableArray<DiagnosticDescriptor> supportedDiagnostics = ImmutableArray<DiagnosticDescriptor>.Empty;
                analyzerExecutor.ExecuteAndCatchIfThrows<object>(analyzer2, delegate
                {
                    ImmutableArray<DiagnosticDescriptor> supportedDiagnostics2 = analyzer2.SupportedDiagnostics;
                    if (!supportedDiagnostics2.IsDefaultOrEmpty)
                    {
                        ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator2 = supportedDiagnostics2.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            if (enumerator2.Current == null)
                            {
                                throw new ArgumentException(string.Format(CodeAnalysisResources.SupportedDiagnosticsHasNullDescriptor, analyzer2.ToString()), "SupportedDiagnostics");
                            }
                        }
                        supportedDiagnostics = supportedDiagnostics2;
                    }
                }, null);
                Action<Exception, DiagnosticAnalyzer, Diagnostic> onAnalyzerException = analyzerExecutor.OnAnalyzerException;
                if (onAnalyzerException != null)
                {
                    void handler(object sender, Exception ex)
                    {
                        Diagnostic arg = AnalyzerExecutor.CreateAnalyzerExceptionDiagnostic(analyzer2, ex);
                        onAnalyzerException(ex, analyzer2, arg);
                    }
                    ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator = supportedDiagnostics.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        DiagnosticDescriptor current = enumerator.Current;
                        ForceLocalizableStringExceptions(current.Title, handler);
                        ForceLocalizableStringExceptions(current.MessageFormat, handler);
                        ForceLocalizableStringExceptions(current.Description, handler);
                    }
                }
                return supportedDiagnostics;
            }

            private static ImmutableArray<SuppressionDescriptor> ComputeSuppressionDescriptors(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
            {
                DiagnosticAnalyzer analyzer2 = analyzer;
                ImmutableArray<SuppressionDescriptor> descriptors = ImmutableArray<SuppressionDescriptor>.Empty;
                DiagnosticSuppressor suppressor = analyzer2 as DiagnosticSuppressor;
                if (suppressor != null)
                {
                    analyzerExecutor.ExecuteAndCatchIfThrows<object>(analyzer2, delegate
                    {
                        ImmutableArray<SuppressionDescriptor> supportedSuppressions = suppressor.SupportedSuppressions;
                        if (!supportedSuppressions.IsDefaultOrEmpty)
                        {
                            ImmutableArray<SuppressionDescriptor>.Enumerator enumerator = supportedSuppressions.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current == null)
                                {
                                    throw new ArgumentException(string.Format(CodeAnalysisResources.SupportedSuppressionsHasNullDescriptor, analyzer2.ToString()), "SupportedSuppressions");
                                }
                            }
                            descriptors = supportedSuppressions;
                        }
                    }, null);
                }
                return descriptors;
            }

            public bool TryProcessCompletedMemberAndGetPendingSymbolEndActionsForContainer(ISymbol containingSymbol, ISymbol processedMemberSymbol, out (ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent) containerEndActionsAndEvent)
            {
                containerEndActionsAndEvent = default((ImmutableArray<SymbolEndAnalyzerAction>, SymbolDeclaredCompilationEvent));
                lock (_gate)
                {
                    if (_lazyPendingMemberSymbolsMap == null || !_lazyPendingMemberSymbolsMap!.TryGetValue(containingSymbol, out var value))
                    {
                        return false;
                    }
                    value.Remove(processedMemberSymbol);
                    if (value.Count > 0 || _lazyPendingSymbolEndActionsMap == null || !_lazyPendingSymbolEndActionsMap!.TryGetValue(containingSymbol, out containerEndActionsAndEvent))
                    {
                        return false;
                    }
                    _lazyPendingSymbolEndActionsMap!.Remove(containingSymbol);
                    return true;
                }
            }

            public bool TryStartExecuteSymbolEndActions(ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                ISymbol symbol = symbolDeclaredEvent.Symbol;
                lock (_gate)
                {
                    if (_lazyPendingMemberSymbolsMap!.TryGetValue(symbol, out var value) && value != null && value.Count > 0)
                    {
                        MarkSymbolEndAnalysisPending_NoLock(symbol, symbolEndActions, symbolDeclaredEvent);
                        return false;
                    }
                    _lazyPendingSymbolEndActionsMap?.Remove(symbol);
                    return true;
                }
            }

            public void MarkSymbolEndAnalysisComplete(ISymbol symbol)
            {
                lock (_gate)
                {
                    _lazyPendingMemberSymbolsMap?.Remove(symbol);
                }
            }

            public void MarkSymbolEndAnalysisPending(ISymbol symbol, ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                lock (_gate)
                {
                    MarkSymbolEndAnalysisPending_NoLock(symbol, symbolEndActions, symbolDeclaredEvent);
                }
            }

            private void MarkSymbolEndAnalysisPending_NoLock(ISymbol symbol, ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent)
            {
                if (_lazyPendingSymbolEndActionsMap == null)
                {
                    _lazyPendingSymbolEndActionsMap = new Dictionary<ISymbol, (ImmutableArray<SymbolEndAnalyzerAction>, SymbolDeclaredCompilationEvent)>();
                }
                _lazyPendingSymbolEndActionsMap![symbol] = (symbolEndActions, symbolDeclaredEvent);
            }

            [Conditional("DEBUG")]
            public void VerifyAllSymbolEndActionsExecuted()
            {
                lock (_gate)
                {
                }
            }
        }

        private readonly ImmutableDictionary<DiagnosticAnalyzer, AnalyzerExecutionContext> _analyzerExecutionContextMap;

        public AnalyzerManager(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            _analyzerExecutionContextMap = CreateAnalyzerExecutionContextMap(analyzers);
        }

        public AnalyzerManager(DiagnosticAnalyzer analyzer)
        {
            _analyzerExecutionContextMap = CreateAnalyzerExecutionContextMap(SpecializedCollections.SingletonEnumerable(analyzer));
        }

        private ImmutableDictionary<DiagnosticAnalyzer, AnalyzerExecutionContext> CreateAnalyzerExecutionContextMap(IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerExecutionContext>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, AnalyzerExecutionContext>();
            foreach (DiagnosticAnalyzer analyzer in analyzers)
            {
                builder.Add(analyzer, new AnalyzerExecutionContext(analyzer));
            }
            return builder.ToImmutable();
        }

        private AnalyzerExecutionContext GetAnalyzerExecutionContext(DiagnosticAnalyzer analyzer)
        {
            return _analyzerExecutionContextMap[analyzer];
        }

        private async ValueTask<HostCompilationStartAnalysisScope> GetCompilationAnalysisScopeAsync(DiagnosticAnalyzer analyzer, HostSessionStartAnalysisScope sessionScope, AnalyzerExecutor analyzerExecutor)
        {
            AnalyzerExecutionContext analyzerExecutionContext = GetAnalyzerExecutionContext(analyzer);
            return await GetCompilationAnalysisScopeCoreAsync(sessionScope, analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async ValueTask<HostCompilationStartAnalysisScope> GetCompilationAnalysisScopeCoreAsync(HostSessionStartAnalysisScope sessionScope, AnalyzerExecutor analyzerExecutor, AnalyzerExecutionContext analyzerExecutionContext)
        {
            HostCompilationStartAnalysisScope result = default(HostCompilationStartAnalysisScope);
            int num;
            try
            {
                result = await analyzerExecutionContext.GetCompilationAnalysisScopeAsync(sessionScope, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
                return result;
            }
            catch (OperationCanceledException)
            {
                num = 1;
            }
            if (num != 1)
            {
                return result;
            }
            analyzerExecutionContext.ClearCompilationScopeTask();
            analyzerExecutor.CancellationToken.ThrowIfCancellationRequested();
            return await GetCompilationAnalysisScopeCoreAsync(sessionScope, analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task<HostSymbolStartAnalysisScope> GetSymbolAnalysisScopeAsync(ISymbol symbol, DiagnosticAnalyzer analyzer, ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions, AnalyzerExecutor analyzerExecutor)
        {
            AnalyzerExecutionContext analyzerExecutionContext = GetAnalyzerExecutionContext(analyzer);
            return await GetSymbolAnalysisScopeCoreAsync(symbol, symbolStartActions, analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async Task<HostSymbolStartAnalysisScope> GetSymbolAnalysisScopeCoreAsync(ISymbol symbol, ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions, AnalyzerExecutor analyzerExecutor, AnalyzerExecutionContext analyzerExecutionContext)
        {
            HostSymbolStartAnalysisScope result = default(HostSymbolStartAnalysisScope);
            int num;
            try
            {
                result = await analyzerExecutionContext.GetSymbolAnalysisScopeAsync(symbol, symbolStartActions, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
                return result;
            }
            catch (OperationCanceledException)
            {
                num = 1;
            }
            if (num != 1)
            {
                return result;
            }
            analyzerExecutionContext.ClearSymbolScopeTask(symbol);
            analyzerExecutor.CancellationToken.ThrowIfCancellationRequested();
            return await GetSymbolAnalysisScopeCoreAsync(symbol, symbolStartActions, analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async ValueTask<HostSessionStartAnalysisScope> GetSessionAnalysisScopeAsync(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            AnalyzerExecutionContext analyzerExecutionContext = GetAnalyzerExecutionContext(analyzer);
            return await GetSessionAnalysisScopeCoreAsync(analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        private async ValueTask<HostSessionStartAnalysisScope> GetSessionAnalysisScopeCoreAsync(AnalyzerExecutor analyzerExecutor, AnalyzerExecutionContext analyzerExecutionContext)
        {
            HostSessionStartAnalysisScope result = default(HostSessionStartAnalysisScope);
            int num;
            try
            {
                result = await analyzerExecutionContext.GetSessionAnalysisScopeAsync(analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
                return result;
            }
            catch (OperationCanceledException)
            {
                num = 1;
            }
            if (num != 1)
            {
                return result;
            }
            analyzerExecutionContext.ClearSessionScopeTask();
            analyzerExecutor.CancellationToken.ThrowIfCancellationRequested();
            return await GetSessionAnalysisScopeCoreAsync(analyzerExecutor, analyzerExecutionContext).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async ValueTask<AnalyzerActions> GetAnalyzerActionsAsync(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            HostSessionStartAnalysisScope hostSessionStartAnalysisScope = await GetSessionAnalysisScopeAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
            if (hostSessionStartAnalysisScope.GetAnalyzerActions(analyzer).CompilationStartActionsCount > 0 && analyzerExecutor.Compilation != null)
            {
                return (await GetCompilationAnalysisScopeAsync(analyzer, hostSessionStartAnalysisScope, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false)).GetAnalyzerActions(analyzer);
            }
            return hostSessionStartAnalysisScope.GetAnalyzerActions(analyzer);
        }

        public async ValueTask<AnalyzerActions> GetPerSymbolAnalyzerActionsAsync(ISymbol symbol, DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            ISymbol symbol2 = symbol;
            AnalyzerActions analyzerActions = await GetAnalyzerActionsAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false);
            if (analyzerActions.SymbolStartActionsCount > 0)
            {
                ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions2 = getFilteredActionsByKind(analyzerActions.SymbolStartActions);
                if (symbolStartActions2.Length > 0)
                {
                    return (await GetSymbolAnalysisScopeAsync(symbol2, analyzer, symbolStartActions2, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false)).GetAnalyzerActions(analyzer);
                }
            }
            return AnalyzerActions.Empty;
            ImmutableArray<SymbolStartAnalyzerAction> getFilteredActionsByKind(ImmutableArray<SymbolStartAnalyzerAction> symbolStartActions)
            {
                ArrayBuilder<SymbolStartAnalyzerAction> arrayBuilder = null;
                for (int i = 0; i < symbolStartActions.Length; i++)
                {
                    SymbolStartAnalyzerAction symbolStartAnalyzerAction = symbolStartActions[i];
                    if (symbolStartAnalyzerAction.Kind != symbol2.Kind)
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<SymbolStartAnalyzerAction>.GetInstance();
                            arrayBuilder.AddRange(symbolStartActions, i);
                        }
                    }
                    else
                    {
                        arrayBuilder?.Add(symbolStartAnalyzerAction);
                    }
                }
                return arrayBuilder?.ToImmutableAndFree() ?? symbolStartActions;
            }
        }

        public async Task<bool> IsConcurrentAnalyzerAsync(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            return (await GetSessionAnalysisScopeAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false)).IsConcurrentAnalyzer(analyzer);
        }

        public async Task<GeneratedCodeAnalysisFlags> GetGeneratedCodeAnalysisFlagsAsync(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            return (await GetSessionAnalysisScopeAsync(analyzer, analyzerExecutor).ConfigureAwait(continueOnCapturedContext: false)).GetGeneratedCodeAnalysisFlags(analyzer);
        }

        private static void ForceLocalizableStringExceptions(LocalizableString localizableString, EventHandler<Exception> handler)
        {
            if (localizableString.CanThrowExceptions)
            {
                localizableString.OnException += handler;
                localizableString.ToString();
                localizableString.OnException -= handler;
            }
        }

        public ImmutableArray<DiagnosticDescriptor> GetSupportedDiagnosticDescriptors(DiagnosticAnalyzer analyzer, AnalyzerExecutor analyzerExecutor)
        {
            return GetAnalyzerExecutionContext(analyzer).GetOrComputeDiagnosticDescriptors(analyzer, analyzerExecutor);
        }

        public ImmutableArray<SuppressionDescriptor> GetSupportedSuppressionDescriptors(DiagnosticSuppressor suppressor, AnalyzerExecutor analyzerExecutor)
        {
            return GetAnalyzerExecutionContext(suppressor).GetOrComputeSuppressionDescriptors(suppressor, analyzerExecutor);
        }

        internal bool IsSupportedDiagnostic(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Func<DiagnosticAnalyzer, bool> isCompilerAnalyzer, AnalyzerExecutor analyzerExecutor)
        {
            if (isCompilerAnalyzer(analyzer))
            {
                return true;
            }
            ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator = GetSupportedDiagnosticDescriptors(analyzer, analyzerExecutor).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Id.Equals(diagnostic.Id, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsDiagnosticAnalyzerSuppressed(DiagnosticAnalyzer analyzer, CompilationOptions options, Func<DiagnosticAnalyzer, bool> isCompilerAnalyzer, AnalyzerExecutor analyzerExecutor, SeverityFilter severityFilter)
        {
            if (isCompilerAnalyzer(analyzer))
            {
                return false;
            }
            ImmutableArray<DiagnosticDescriptor> supportedDiagnosticDescriptors = GetSupportedDiagnosticDescriptors(analyzer, analyzerExecutor);
            ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions = options.SpecificDiagnosticOptions;
            analyzerExecutor.TryGetCompilationAndAnalyzerOptions(out var compilation2, out var analyzerOptions2);
            ImmutableArray<DiagnosticDescriptor>.Enumerator enumerator = supportedDiagnosticDescriptors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticDescriptor current = enumerator.Current;
                if (HasNotConfigurableTag(current.CustomTags))
                {
                    if (current.IsEnabledByDefault)
                    {
                        return false;
                    }
                    continue;
                }
                bool flag = !current.IsEnabledByDefault;
                if ((specificDiagnosticOptions.TryGetValue(current.Id, out var value) || (options.SyntaxTreeOptionsProvider != null && options.SyntaxTreeOptionsProvider!.TryGetGlobalDiagnosticValue(current.Id, analyzerExecutor.CancellationToken, out value))) && value != 0)
                {
                    flag = value == ReportDiagnostic.Suppress;
                }
                else
                {
                    value = (flag ? ReportDiagnostic.Suppress : DiagnosticDescriptor.MapSeverityToReport(current.DefaultSeverity));
                }
                if (severityFilter.Contains(value))
                {
                    flag = true;
                }
                if (flag && isEnabledWithAnalyzerConfigOptions(current, severityFilter, compilation2, analyzerOptions2, analyzerExecutor.CancellationToken))
                {
                    flag = false;
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (analyzer is DiagnosticSuppressor suppressor)
            {
                ImmutableArray<SuppressionDescriptor>.Enumerator enumerator2 = GetSupportedSuppressionDescriptors(suppressor, analyzerExecutor).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (!enumerator2.Current.IsDisabled(options))
                    {
                        return false;
                    }
                }
            }
            return true;
            static bool isEnabledWithAnalyzerConfigOptions(DiagnosticDescriptor descriptor, SeverityFilter severityFilter, Compilation? compilation, AnalyzerOptions? analyzerOptions, CancellationToken cancellationToken)
            {
                if (compilation != null)
                {
                    SyntaxTreeOptionsProvider syntaxTreeOptionsProvider = compilation!.Options.SyntaxTreeOptionsProvider;
                    if (syntaxTreeOptionsProvider != null)
                    {
                        foreach (SyntaxTree syntaxTree in compilation!.SyntaxTrees)
                        {
                            if ((syntaxTreeOptionsProvider.TryGetDiagnosticValue(syntaxTree, descriptor.Id, cancellationToken, out var severity) || analyzerOptions.TryGetSeverityFromBulkConfiguration(syntaxTree, compilation, descriptor, cancellationToken, out severity)) && severity != ReportDiagnostic.Suppress && !severityFilter.Contains(severity))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        internal static bool HasNotConfigurableTag(IEnumerable<string> customTags)
        {
            foreach (string customTag in customTags)
            {
                if (customTag == "NotConfigurable")
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryProcessCompletedMemberAndGetPendingSymbolEndActionsForContainer(ISymbol containingSymbol, ISymbol processedMemberSymbol, DiagnosticAnalyzer analyzer, out (ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent) containerEndActionsAndEvent)
        {
            return GetAnalyzerExecutionContext(analyzer).TryProcessCompletedMemberAndGetPendingSymbolEndActionsForContainer(containingSymbol, processedMemberSymbol, out containerEndActionsAndEvent);
        }

        public bool TryStartExecuteSymbolEndActions(ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, DiagnosticAnalyzer analyzer, SymbolDeclaredCompilationEvent symbolDeclaredEvent)
        {
            return GetAnalyzerExecutionContext(analyzer).TryStartExecuteSymbolEndActions(symbolEndActions, symbolDeclaredEvent);
        }

        public void MarkSymbolEndAnalysisPending(ISymbol symbol, DiagnosticAnalyzer analyzer, ImmutableArray<SymbolEndAnalyzerAction> symbolEndActions, SymbolDeclaredCompilationEvent symbolDeclaredEvent)
        {
            GetAnalyzerExecutionContext(analyzer).MarkSymbolEndAnalysisPending(symbol, symbolEndActions, symbolDeclaredEvent);
        }

        public void MarkSymbolEndAnalysisComplete(ISymbol symbol, DiagnosticAnalyzer analyzer)
        {
            GetAnalyzerExecutionContext(analyzer).MarkSymbolEndAnalysisComplete(symbol);
        }

        [Conditional("DEBUG")]
        public void VerifyAllSymbolEndActionsExecuted()
        {
            foreach (AnalyzerExecutionContext value in _analyzerExecutionContextMap.Values)
            {
                _ = value;
            }
        }
    }
}
