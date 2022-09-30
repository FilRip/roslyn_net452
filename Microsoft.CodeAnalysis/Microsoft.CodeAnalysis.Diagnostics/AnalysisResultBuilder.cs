using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.Diagnostics.Telemetry;
using Microsoft.CodeAnalysis.PooledObjects;

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

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal sealed class AnalysisResultBuilder
    {
        private static readonly ImmutableDictionary<string, OneOrMany<AdditionalText>> s_emptyPathToAdditionalTextMap = ImmutableDictionary<string, OneOrMany<AdditionalText>>.Empty.WithComparers(PathUtilities.Comparer);

        private readonly object _gate = new();

        private readonly Dictionary<DiagnosticAnalyzer, TimeSpan>? _analyzerExecutionTimeOpt;

        private readonly HashSet<DiagnosticAnalyzer> _completedAnalyzers;

        private readonly Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts> _analyzerActionCounts;

        private readonly ImmutableDictionary<string, OneOrMany<AdditionalText>> _pathToAdditionalTextMap;

        private Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? _localSemanticDiagnosticsOpt;

        private Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? _localSyntaxDiagnosticsOpt;

        private Dictionary<AdditionalText, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? _localAdditionalFileDiagnosticsOpt;

        private Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>? _nonLocalDiagnosticsOpt;

        internal AnalysisResultBuilder(bool logAnalyzerExecutionTime, ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<AdditionalText> additionalFiles)
        {
            _analyzerExecutionTimeOpt = (logAnalyzerExecutionTime ? CreateAnalyzerExecutionTimeMap(analyzers) : null);
            _completedAnalyzers = new HashSet<DiagnosticAnalyzer>();
            _analyzerActionCounts = new Dictionary<DiagnosticAnalyzer, AnalyzerActionCounts>(analyzers.Length);
            _pathToAdditionalTextMap = CreatePathToAdditionalTextMap(additionalFiles);
        }

        private static Dictionary<DiagnosticAnalyzer, TimeSpan> CreateAnalyzerExecutionTimeMap(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            Dictionary<DiagnosticAnalyzer, TimeSpan> dictionary = new(analyzers.Length);
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                dictionary[current] = default;
            }
            return dictionary;
        }

        private static ImmutableDictionary<string, OneOrMany<AdditionalText>> CreatePathToAdditionalTextMap(ImmutableArray<AdditionalText> additionalFiles)
        {
            if (additionalFiles.IsEmpty)
            {
                return s_emptyPathToAdditionalTextMap;
            }
            ImmutableDictionary<string, OneOrMany<AdditionalText>>.Builder builder = ImmutableDictionary.CreateBuilder<string, OneOrMany<AdditionalText>>(PathUtilities.Comparer);
            ImmutableArray<AdditionalText>.Enumerator enumerator = additionalFiles.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AdditionalText current = enumerator.Current;
                string key = current.Path ?? string.Empty;
                OneOrMany<AdditionalText> value = (builder[key] = ((!builder.TryGetValue(key, out value)) ? new OneOrMany<AdditionalText>(current) : value.Add(current)));
            }
            return builder.ToImmutable();
        }

        public TimeSpan GetAnalyzerExecutionTime(DiagnosticAnalyzer analyzer)
        {
            lock (_gate)
            {
                return _analyzerExecutionTimeOpt![analyzer];
            }
        }

        internal ImmutableArray<DiagnosticAnalyzer> GetPendingAnalyzers(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            lock (_gate)
            {
                ArrayBuilder<DiagnosticAnalyzer> arrayBuilder = null;
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticAnalyzer current = enumerator.Current;
                    if (!_completedAnalyzers.Contains(current))
                    {
                        arrayBuilder ??= ArrayBuilder<DiagnosticAnalyzer>.GetInstance();
                        arrayBuilder.Add(current);
                    }
                }
                return arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<DiagnosticAnalyzer>.Empty;
            }
        }

        internal void ApplySuppressionsAndStoreAnalysisResult(AnalysisScope analysisScope, AnalyzerDriver driver, Compilation compilation, Func<DiagnosticAnalyzer, AnalyzerActionCounts> getAnalyzerActionCounts, bool fullAnalysisResultForAnalyzersInScope)
        {
            AnalysisScope analysisScope2 = analysisScope;
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analysisScope2.Analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                ImmutableArray<Diagnostic> diagnostics = driver.DequeueLocalDiagnosticsAndApplySuppressions(current, syntax: true, compilation);
                ImmutableArray<Diagnostic> diagnostics2 = driver.DequeueLocalDiagnosticsAndApplySuppressions(current, syntax: false, compilation);
                ImmutableArray<Diagnostic> diagnostics3 = driver.DequeueNonLocalDiagnosticsAndApplySuppressions(current, compilation);
                lock (_gate)
                {
                    if (!_completedAnalyzers.Contains(current))
                    {
                        if (diagnostics.Length > 0 || diagnostics2.Length > 0 || diagnostics3.Length > 0 || fullAnalysisResultForAnalyzersInScope)
                        {
                            UpdateLocalDiagnostics_NoLock(current, diagnostics, fullAnalysisResultForAnalyzersInScope, getSourceTree, ref _localSyntaxDiagnosticsOpt);
                            UpdateLocalDiagnostics_NoLock(current, diagnostics, fullAnalysisResultForAnalyzersInScope, getAdditionalTextKey, ref _localAdditionalFileDiagnosticsOpt);
                            UpdateLocalDiagnostics_NoLock(current, diagnostics2, fullAnalysisResultForAnalyzersInScope, getSourceTree, ref _localSemanticDiagnosticsOpt);
                            UpdateNonLocalDiagnostics_NoLock(current, diagnostics3, fullAnalysisResultForAnalyzersInScope);
                        }
                        if (_analyzerExecutionTimeOpt != null)
                        {
                            TimeSpan timeSpan = driver.ResetAnalyzerExecutionTime(current);
                            _analyzerExecutionTimeOpt![current] = (fullAnalysisResultForAnalyzersInScope ? timeSpan : (_analyzerExecutionTimeOpt![current] + timeSpan));
                        }
                        if (!_analyzerActionCounts.ContainsKey(current))
                        {
                            _analyzerActionCounts.Add(current, getAnalyzerActionCounts(current));
                        }
                        if (fullAnalysisResultForAnalyzersInScope)
                        {
                            _completedAnalyzers.Add(current);
                        }
                    }
                }
            }
            AdditionalText? getAdditionalTextKey(Diagnostic diagnostic)
            {
                if (diagnostic.Location is ExternalFileLocation externalFileLocation && _pathToAdditionalTextMap.TryGetValue(externalFileLocation.FilePath, out var value))
                {
                    OneOrMany<AdditionalText>.Enumerator enumerator2 = value.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        AdditionalText current2 = enumerator2.Current;
                        if (analysisScope2.AdditionalFiles.Contains<AdditionalText>(current2))
                        {
                            return current2;
                        }
                    }
                }
                return null;
            }
            static SyntaxTree? getSourceTree(Diagnostic diagnostic)
            {
                return diagnostic.Location.SourceTree;
            }
        }

        private void UpdateLocalDiagnostics_NoLock<TKey>(DiagnosticAnalyzer analyzer, ImmutableArray<Diagnostic> diagnostics, bool overwrite, Func<Diagnostic, TKey?> getKeyFunc, ref Dictionary<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? lazyLocalDiagnostics) where TKey : class
        {
            if (diagnostics.IsEmpty)
            {
                return;
            }
            lazyLocalDiagnostics = lazyLocalDiagnostics ?? new Dictionary<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>();
            foreach (IGrouping<TKey, Diagnostic> item in diagnostics.GroupBy<Diagnostic, TKey>(getKeyFunc))
            {
                TKey key = item.Key;
                if (key != null)
                {
                    if (!lazyLocalDiagnostics!.TryGetValue(key, out var value))
                    {
                        value = new Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>();
                        lazyLocalDiagnostics![key] = value;
                    }
                    if (!value.TryGetValue(analyzer, out var value2))
                    {
                        value2 = (value[analyzer] = ImmutableArray.CreateBuilder<Diagnostic>());
                    }
                    if (overwrite)
                    {
                        value2.Clear();
                    }
                    value2.AddRange(item);
                }
            }
        }

        private void UpdateNonLocalDiagnostics_NoLock(DiagnosticAnalyzer analyzer, ImmutableArray<Diagnostic> diagnostics, bool overwrite)
        {
            if (!diagnostics.IsEmpty)
            {
                _nonLocalDiagnosticsOpt ??= new Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>();
                if (!_nonLocalDiagnosticsOpt!.TryGetValue(analyzer, out var value))
                {
                    value = ImmutableArray.CreateBuilder<Diagnostic>();
                    _nonLocalDiagnosticsOpt![analyzer] = value;
                }
                if (overwrite)
                {
                    value.Clear();
                }
                value.AddRange(diagnostics);
            }
        }

        internal ImmutableArray<Diagnostic> GetDiagnostics(AnalysisScope analysisScope, bool getLocalDiagnostics, bool getNonLocalDiagnostics)
        {
            lock (_gate)
            {
                return GetDiagnostics_NoLock(analysisScope, getLocalDiagnostics, getNonLocalDiagnostics);
            }
        }

        private ImmutableArray<Diagnostic> GetDiagnostics_NoLock(AnalysisScope analysisScope, bool getLocalDiagnostics, bool getNonLocalDiagnostics)
        {
            ImmutableArray<Diagnostic>.Builder builder = ImmutableArray.CreateBuilder<Diagnostic>();
            if (getLocalDiagnostics)
            {
                if (!analysisScope.IsSingleFileAnalysis)
                {
                    AddAllLocalDiagnostics_NoLock(_localSyntaxDiagnosticsOpt, analysisScope, builder);
                    AddAllLocalDiagnostics_NoLock(_localSemanticDiagnosticsOpt, analysisScope, builder);
                    AddAllLocalDiagnostics_NoLock(_localAdditionalFileDiagnosticsOpt, analysisScope, builder);
                }
                else if (analysisScope.IsSyntacticSingleFileAnalysis)
                {
                    AddLocalDiagnosticsForPartialAnalysis_NoLock(_localSyntaxDiagnosticsOpt, analysisScope, builder);
                    AddLocalDiagnosticsForPartialAnalysis_NoLock(_localAdditionalFileDiagnosticsOpt, analysisScope, builder);
                }
                else
                {
                    AddLocalDiagnosticsForPartialAnalysis_NoLock(_localSemanticDiagnosticsOpt, analysisScope, builder);
                }
            }
            if (getNonLocalDiagnostics && _nonLocalDiagnosticsOpt != null)
            {
                AddDiagnostics_NoLock(_nonLocalDiagnosticsOpt, analysisScope.Analyzers, builder);
            }
            return builder.ToImmutableArray();
        }

        private static void AddAllLocalDiagnostics_NoLock<TKey>(Dictionary<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? lazyLocalDiagnostics, AnalysisScope analysisScope, ImmutableArray<Diagnostic>.Builder builder) where TKey : class
        {
            if (lazyLocalDiagnostics == null)
            {
                return;
            }
            foreach (Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> value in lazyLocalDiagnostics!.Values)
            {
                AddDiagnostics_NoLock(value, analysisScope.Analyzers, builder);
            }
        }

        private static void AddLocalDiagnosticsForPartialAnalysis_NoLock(Dictionary<SyntaxTree, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? localDiagnostics, AnalysisScope analysisScope, ImmutableArray<Diagnostic>.Builder builder)
        {
            AddLocalDiagnosticsForPartialAnalysis_NoLock(localDiagnostics, analysisScope.FilterFileOpt!.Value.SourceTree, analysisScope.Analyzers, builder);
        }

        private static void AddLocalDiagnosticsForPartialAnalysis_NoLock(Dictionary<AdditionalText, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? localDiagnostics, AnalysisScope analysisScope, ImmutableArray<Diagnostic>.Builder builder)
        {
            AddLocalDiagnosticsForPartialAnalysis_NoLock(localDiagnostics, analysisScope.FilterFileOpt!.Value.AdditionalFile, analysisScope.Analyzers, builder);
        }

        private static void AddLocalDiagnosticsForPartialAnalysis_NoLock<TKey>(Dictionary<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? localDiagnostics, TKey? key, ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<Diagnostic>.Builder builder) where TKey : class
        {
            if (key != null && localDiagnostics != null && localDiagnostics!.TryGetValue(key, out var value))
            {
                AddDiagnostics_NoLock(value, analyzers, builder);
            }
        }

        private static void AddDiagnostics_NoLock(Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> diagnostics, ImmutableArray<DiagnosticAnalyzer> analyzers, ImmutableArray<Diagnostic>.Builder builder)
        {
            ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticAnalyzer current = enumerator.Current;
                if (diagnostics.TryGetValue(current, out var value))
                {
                    builder.AddRange(value);
                }
            }
        }

        internal AnalysisResult ToAnalysisResult(ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ImmutableHashSet<DiagnosticAnalyzer> analyzers2 = analyzers.ToImmutableHashSet();
            ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> immutable;
            ImmutableDictionary<SyntaxTree, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> immutable2;
            ImmutableDictionary<AdditionalText, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> immutable3;
            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> immutable4;
            lock (_gate)
            {
                immutable = GetImmutable(analyzers2, _localSyntaxDiagnosticsOpt);
                immutable2 = GetImmutable(analyzers2, _localSemanticDiagnosticsOpt);
                immutable3 = GetImmutable(analyzers2, _localAdditionalFileDiagnosticsOpt);
                immutable4 = GetImmutable(analyzers2, _nonLocalDiagnosticsOpt);
            }
            cancellationToken.ThrowIfCancellationRequested();
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> telemetryInfo = GetTelemetryInfo(analyzers);
            return new AnalysisResult(analyzers, immutable, immutable2, immutable3, immutable4, telemetryInfo);
        }

        private static ImmutableDictionary<TKey, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>> GetImmutable<TKey>(ImmutableHashSet<DiagnosticAnalyzer> analyzers, Dictionary<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>>? localDiagnosticsOpt) where TKey : class
        {
            if (localDiagnosticsOpt == null)
            {
                return ImmutableDictionary<TKey, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>.Empty;
            }
            ImmutableDictionary<TKey, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>>();
            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Builder builder2 = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>();
            foreach (KeyValuePair<TKey, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>> item in localDiagnosticsOpt!)
            {
                TKey key = item.Key;
                foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> item2 in item.Value)
                {
                    if (analyzers.Contains(item2.Key))
                    {
                        builder2.Add(item2.Key, item2.Value.ToImmutable());
                    }
                }
                builder.Add(key, builder2.ToImmutable());
                builder2.Clear();
            }
            return builder.ToImmutable();
        }

        private static ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>> GetImmutable(ImmutableHashSet<DiagnosticAnalyzer> analyzers, Dictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder>? nonLocalDiagnosticsOpt)
        {
            if (nonLocalDiagnosticsOpt == null)
            {
                return ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Empty;
            }
            ImmutableDictionary<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, ImmutableArray<Diagnostic>>();
            foreach (KeyValuePair<DiagnosticAnalyzer, ImmutableArray<Diagnostic>.Builder> item in nonLocalDiagnosticsOpt!)
            {
                if (analyzers.Contains(item.Key))
                {
                    builder.Add(item.Key, item.Value.ToImmutable());
                }
            }
            return builder.ToImmutable();
        }

        private ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo> GetTelemetryInfo(ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            ImmutableDictionary<DiagnosticAnalyzer, AnalyzerTelemetryInfo>.Builder builder = ImmutableDictionary.CreateBuilder<DiagnosticAnalyzer, AnalyzerTelemetryInfo>();
            lock (_gate)
            {
                ImmutableArray<DiagnosticAnalyzer>.Enumerator enumerator = analyzers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DiagnosticAnalyzer current = enumerator.Current;
                    if (!_analyzerActionCounts.TryGetValue(current, out var value))
                    {
                        value = AnalyzerActionCounts.Empty;
                    }
                    int suppressionActionCounts = ((current is DiagnosticSuppressor) ? 1 : 0);
                    TimeSpan executionTime = ((_analyzerExecutionTimeOpt != null) ? _analyzerExecutionTimeOpt![current] : default(TimeSpan));
                    AnalyzerTelemetryInfo value2 = new(value, suppressionActionCounts, executionTime);
                    builder.Add(current, value2);
                }
            }
            return builder.ToImmutable();
        }
    }
}
