using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class AnalyzerConfigSet
    {
        private sealed class SequenceEqualComparer : IEqualityComparer<List<AnalyzerConfig.Section>>
        {
            public static SequenceEqualComparer Instance { get; } = new SequenceEqualComparer();


            public bool Equals(List<AnalyzerConfig.Section>? x, List<AnalyzerConfig.Section>? y)
            {
                if (x == null || y == null)
                {
                    if (x == null)
                    {
                        return y == null;
                    }
                    return false;
                }
                if (x!.Count != y!.Count)
                {
                    return false;
                }
                for (int i = 0; i < x!.Count; i++)
                {
                    if (x![i] != y![i])
                    {
                        return false;
                    }
                }
                return true;
            }

            public int GetHashCode(List<AnalyzerConfig.Section> obj)
            {
                return Hash.CombineValues(obj);
            }
        }

        internal struct GlobalAnalyzerConfigBuilder
        {
            private ImmutableDictionary<string, ImmutableDictionary<string, (string value, string configPath, int globalLevel)>.Builder>.Builder? _values;

            private ImmutableDictionary<string, ImmutableDictionary<string, (int globalLevel, ArrayBuilder<string> configPaths)>.Builder>.Builder? _duplicates;

            internal const string GlobalConfigPath = "<Global Config>";

            internal const string GlobalSectionName = "Global Section";

            internal void MergeIntoGlobalConfig(AnalyzerConfig config, DiagnosticBag diagnostics)
            {
                if (_values == null)
                {
                    _values = ImmutableDictionary.CreateBuilder<string, ImmutableDictionary<string, (string, string, int)>.Builder>(AnalyzerConfig.Section.NameEqualityComparer);
                    _duplicates = ImmutableDictionary.CreateBuilder<string, ImmutableDictionary<string, (int, ArrayBuilder<string>)>.Builder>(AnalyzerConfig.Section.NameEqualityComparer);
                }
                MergeSection(config.PathToFile, config.GlobalSection, config.GlobalLevel, isGlobalSection: true);
                ImmutableArray<AnalyzerConfig.Section>.Enumerator enumerator = config.NamedSections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AnalyzerConfig.Section current = enumerator.Current;
                    if (AnalyzerConfig.IsAbsoluteEditorConfigPath(current.Name))
                    {
                        MergeSection(config.PathToFile, current, config.GlobalLevel, isGlobalSection: false);
                        continue;
                    }
                    diagnostics.Add(Diagnostic.Create(InvalidGlobalAnalyzerSectionDescriptor, Location.None, current.Name, config.PathToFile));
                }
            }

            internal GlobalAnalyzerConfig Build(DiagnosticBag diagnostics)
            {
                if (_values == null || _duplicates == null)
                {
                    return new GlobalAnalyzerConfig(new AnalyzerConfig.Section("Global Section", ImmutableDictionary<string, string>.Empty), ImmutableArray<AnalyzerConfig.Section>.Empty);
                }
                foreach (KeyValuePair<string, ImmutableDictionary<string, (int, ArrayBuilder<string>)>.Builder> item2 in _duplicates!)
                {
                    KeyValuePairUtil.Deconstruct(item2, out var key, out var value);
                    string text = key;
                    ImmutableDictionary<string, (int, ArrayBuilder<string>)>.Builder builder = value;
                    string text2 = (string.IsNullOrWhiteSpace(text) ? "Global Section" : text);
                    foreach (KeyValuePair<string, (int, ArrayBuilder<string>)> item3 in builder)
                    {
                        KeyValuePairUtil.Deconstruct(item3, out key, out var value2);
                        (int, ArrayBuilder<string>) tuple = value2;
                        string text3 = key;
                        ArrayBuilder<string> item = tuple.Item2;
                        diagnostics.Add(Diagnostic.Create(MultipleGlobalAnalyzerKeysDescriptor, Location.None, text3, text2, string.Join(", ", item)));
                    }
                }
                _duplicates = null;
                AnalyzerConfig.Section section = GetSection(string.Empty);
                _values!.Remove(string.Empty);
                ArrayBuilder<AnalyzerConfig.Section> arrayBuilder = new ArrayBuilder<AnalyzerConfig.Section>(_values!.Count);
                foreach (string item4 in _values!.Keys.Order())
                {
                    arrayBuilder.Add(GetSection(item4));
                }
                GlobalAnalyzerConfig result = new GlobalAnalyzerConfig(section, arrayBuilder.ToImmutableAndFree());
                _values = null;
                return result;
            }

            private AnalyzerConfig.Section GetSection(string sectionName)
            {
                ImmutableDictionary<string, string> properties = _values![sectionName].ToImmutableDictionary<KeyValuePair<string, (string, string, int)>, string, string>((KeyValuePair<string, (string value, string configPath, int globalLevel)> d) => d.Key, (KeyValuePair<string, (string value, string configPath, int globalLevel)> d) => d.Value.value, AnalyzerConfig.Section.PropertiesKeyComparer);
                return new AnalyzerConfig.Section(sectionName, properties);
            }

            private void MergeSection(string configPath, AnalyzerConfig.Section section, int globalLevel, bool isGlobalSection)
            {
                if (!_values!.TryGetValue(section.Name, out var value))
                {
                    value = ImmutableDictionary.CreateBuilder<string, (string, string, int)>(AnalyzerConfig.Section.PropertiesKeyComparer);
                    _values!.Add(section.Name, value);
                }
                _duplicates!.TryGetValue(section.Name, out var value2);
                foreach (var (text3, item) in section.Properties)
                {
                    if (isGlobalSection && (AnalyzerConfig.Section.PropertiesKeyComparer.Equals(text3, "is_global") || AnalyzerConfig.Section.PropertiesKeyComparer.Equals(text3, "global_level")))
                    {
                        continue;
                    }
                    bool flag = value.TryGetValue(text3, out (string, string, int) value3);
                    (int, ArrayBuilder<string>) value4 = default((int, ArrayBuilder<string>));
                    bool flag2 = !flag && (value2?.TryGetValue(text3, out value4) ?? false);
                    if (!flag && !flag2)
                    {
                        value.Add(text3, (item, configPath, globalLevel));
                        continue;
                    }
                    int num;
                    if (!flag)
                    {
                        (num, _) = value4;
                    }
                    else
                    {
                        num = value3.Item3;
                    }
                    int num2 = num;
                    if (num2 < globalLevel)
                    {
                        value[text3] = (item, configPath, globalLevel);
                        if (flag2)
                        {
                            value2.Remove(text3);
                        }
                    }
                    else if (num2 == globalLevel)
                    {
                        if (value2 == null)
                        {
                            value2 = ImmutableDictionary.CreateBuilder<string, (int, ArrayBuilder<string>)>(AnalyzerConfig.Section.PropertiesKeyComparer);
                            _duplicates!.Add(section.Name, value2);
                        }
                        ArrayBuilder<string> arrayBuilder = value4.Item2 ?? ArrayBuilder<string>.GetInstance();
                        arrayBuilder.Add(configPath);
                        value2[text3] = (globalLevel, arrayBuilder);
                        if (flag)
                        {
                            (string, string, int) tuple2 = value3;
                            value.Remove(text3);
                            arrayBuilder.Insert(0, tuple2.Item2);
                        }
                    }
                }
            }
        }

        internal sealed class GlobalAnalyzerConfig
        {
            internal AnalyzerConfig.Section GlobalSection { get; }

            internal ImmutableArray<AnalyzerConfig.Section> NamedSections { get; }

            public GlobalAnalyzerConfig(AnalyzerConfig.Section globalSection, ImmutableArray<AnalyzerConfig.Section> namedSections)
            {
                GlobalSection = globalSection;
                NamedSections = namedSections;
            }
        }

        private readonly ImmutableArray<AnalyzerConfig> _analyzerConfigs;

        private readonly GlobalAnalyzerConfig _globalConfig;

        private readonly ImmutableArray<ImmutableArray<AnalyzerConfig.SectionNameMatcher?>> _analyzerMatchers;

        private readonly ConcurrentDictionary<ReadOnlyMemory<char>, string> _diagnosticIdCache = new ConcurrentDictionary<ReadOnlyMemory<char>, string>(CharMemoryEqualityComparer.Instance);

        private readonly ConcurrentCache<List<AnalyzerConfig.Section>, AnalyzerConfigOptionsResult> _optionsCache = new ConcurrentCache<List<AnalyzerConfig.Section>, AnalyzerConfigOptionsResult>(50, SequenceEqualComparer.Instance);

        private readonly ObjectPool<ImmutableDictionary<string, ReportDiagnostic>.Builder> _treeOptionsPool = new ObjectPool<ImmutableDictionary<string, ReportDiagnostic>.Builder>(() => ImmutableDictionary.CreateBuilder<string, ReportDiagnostic>(AnalyzerConfig.Section.PropertiesKeyComparer));

        private readonly ObjectPool<ImmutableDictionary<string, string>.Builder> _analyzerOptionsPool = new ObjectPool<ImmutableDictionary<string, string>.Builder>(() => ImmutableDictionary.CreateBuilder<string, string>(AnalyzerConfig.Section.PropertiesKeyComparer));

        private readonly ObjectPool<List<AnalyzerConfig.Section>> _sectionKeyPool = new ObjectPool<List<AnalyzerConfig.Section>>(() => new List<AnalyzerConfig.Section>());

        private StrongBox<AnalyzerConfigOptionsResult>? _lazyConfigOptions;

        private static readonly DiagnosticDescriptor InvalidAnalyzerConfigSeverityDescriptor = new DiagnosticDescriptor("InvalidSeverityInAnalyzerConfig", CodeAnalysisResources.WRN_InvalidSeverityInAnalyzerConfig_Title, CodeAnalysisResources.WRN_InvalidSeverityInAnalyzerConfig, "AnalyzerConfig", DiagnosticSeverity.Warning, true, null, null);

        private static readonly DiagnosticDescriptor MultipleGlobalAnalyzerKeysDescriptor = new DiagnosticDescriptor("MultipleGlobalAnalyzerKeys", CodeAnalysisResources.WRN_MultipleGlobalAnalyzerKeys_Title, CodeAnalysisResources.WRN_MultipleGlobalAnalyzerKeys, "AnalyzerConfig", DiagnosticSeverity.Warning, true, null, null);

        private static readonly DiagnosticDescriptor InvalidGlobalAnalyzerSectionDescriptor = new DiagnosticDescriptor("InvalidGlobalSectionName", CodeAnalysisResources.WRN_InvalidGlobalSectionName_Title, CodeAnalysisResources.WRN_InvalidGlobalSectionName, "AnalyzerConfig", DiagnosticSeverity.Warning, true, null, null);

        public AnalyzerConfigOptionsResult GlobalConfigOptions
        {
            get
            {
                if (_lazyConfigOptions == null)
                {
                    Interlocked.CompareExchange(ref _lazyConfigOptions, new StrongBox<AnalyzerConfigOptionsResult>(ParseGlobalConfigOptions()), null);
                }
                return _lazyConfigOptions!.Value;
            }
        }

        public static AnalyzerConfigSet Create<TList>(TList analyzerConfigs) where TList : IReadOnlyCollection<AnalyzerConfig>
        {
            return Create(analyzerConfigs, out ImmutableArray<Diagnostic> diagnostics);
        }

        public static AnalyzerConfigSet Create<TList>(TList analyzerConfigs, out ImmutableArray<Diagnostic> diagnostics) where TList : IReadOnlyCollection<AnalyzerConfig>
        {
            ArrayBuilder<AnalyzerConfig> instance = ArrayBuilder<AnalyzerConfig>.GetInstance(analyzerConfigs.Count);
            instance.AddRange(analyzerConfigs);
            instance.Sort(AnalyzerConfig.DirectoryLengthComparer);
            return new AnalyzerConfigSet(globalConfig: MergeGlobalConfigs(instance, out diagnostics), analyzerConfigs: instance.ToImmutableAndFree());
        }

        private AnalyzerConfigSet(ImmutableArray<AnalyzerConfig> analyzerConfigs, GlobalAnalyzerConfig globalConfig)
        {
            _analyzerConfigs = analyzerConfigs;
            _globalConfig = globalConfig;
            ArrayBuilder<ImmutableArray<AnalyzerConfig.SectionNameMatcher?>> instance = ArrayBuilder<ImmutableArray<AnalyzerConfig.SectionNameMatcher?>>.GetInstance(_analyzerConfigs.Length);
            ImmutableArray<AnalyzerConfig>.Enumerator enumerator = _analyzerConfigs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AnalyzerConfig current = enumerator.Current;
                ArrayBuilder<AnalyzerConfig.SectionNameMatcher?> instance2 = ArrayBuilder<AnalyzerConfig.SectionNameMatcher?>.GetInstance(current.NamedSections.Length);
                ImmutableArray<AnalyzerConfig.Section>.Enumerator enumerator2 = current.NamedSections.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    AnalyzerConfig.SectionNameMatcher? item = AnalyzerConfig.TryCreateSectionNameMatcher(enumerator2.Current.Name);
                    instance2.Add(item);
                }
                instance.Add(instance2.ToImmutableAndFree());
            }
            _analyzerMatchers = instance.ToImmutableAndFree();
        }

        public AnalyzerConfigOptionsResult GetOptionsForSourcePath(string sourcePath)
        {
            if (sourcePath == null)
            {
                throw new ArgumentNullException("sourcePath");
            }
            List<AnalyzerConfig.Section> list = _sectionKeyPool.Allocate();
            string text = PathUtilities.NormalizeWithForwardSlash(sourcePath);
            ImmutableArray<AnalyzerConfig.Section>.Enumerator enumerator = _globalConfig.NamedSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AnalyzerConfig.Section current = enumerator.Current;
                if (text.Equals(current.Name, AnalyzerConfig.Section.NameComparer))
                {
                    list.Add(current);
                }
            }
            int count = list.Count;
            for (int i = 0; i < _analyzerConfigs.Length; i++)
            {
                AnalyzerConfig analyzerConfig = _analyzerConfigs[i];
                if (!text.StartsWith(analyzerConfig.NormalizedDirectory, StringComparison.Ordinal))
                {
                    continue;
                }
                if (analyzerConfig.IsRoot)
                {
                    list.RemoveRange(count, list.Count - count);
                }
                int num = analyzerConfig.NormalizedDirectory.Length;
                if (analyzerConfig.NormalizedDirectory[num - 1] == '/')
                {
                    num--;
                }
                string s = text.Substring(num);
                ImmutableArray<AnalyzerConfig.SectionNameMatcher?> immutableArray = _analyzerMatchers[i];
                for (int j = 0; j < immutableArray.Length; j++)
                {
                    AnalyzerConfig.SectionNameMatcher? sectionNameMatcher = immutableArray[j];
                    if (sectionNameMatcher.HasValue && sectionNameMatcher.GetValueOrDefault().IsMatch(s))
                    {
                        AnalyzerConfig.Section item = analyzerConfig.NamedSections[j];
                        list.Add(item);
                    }
                }
            }
            if (!_optionsCache.TryGetValue(list, out var value))
            {
                ImmutableDictionary<string, ReportDiagnostic>.Builder builder = _treeOptionsPool.Allocate();
                ImmutableDictionary<string, string>.Builder builder2 = _analyzerOptionsPool.Allocate();
                ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance();
                int num2 = 0;
                builder2.AddRange(GlobalConfigOptions.AnalyzerOptions);
                enumerator = _globalConfig.NamedSections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AnalyzerConfig.Section current2 = enumerator.Current;
                    if (list.Count > 0 && current2 == list[num2])
                    {
                        ParseSectionOptions(list[num2], builder, builder2, instance, "<Global Config>", _diagnosticIdCache);
                        num2++;
                        if (num2 == list.Count)
                        {
                            break;
                        }
                    }
                }
                for (int k = 0; k < _analyzerConfigs.Length; k++)
                {
                    if (num2 >= list.Count)
                    {
                        break;
                    }
                    AnalyzerConfig analyzerConfig2 = _analyzerConfigs[k];
                    ImmutableArray<AnalyzerConfig.SectionNameMatcher?> immutableArray2 = _analyzerMatchers[k];
                    for (int l = 0; l < immutableArray2.Length; l++)
                    {
                        if (list[num2] == analyzerConfig2.NamedSections[l])
                        {
                            ParseSectionOptions(list[num2], builder, builder2, instance, analyzerConfig2.PathToFile, _diagnosticIdCache);
                            num2++;
                            if (num2 == list.Count)
                            {
                                break;
                            }
                        }
                    }
                }
                value = new AnalyzerConfigOptionsResult((builder.Count > 0) ? builder.ToImmutable() : SyntaxTree.EmptyDiagnosticOptions, (builder2.Count > 0) ? builder2.ToImmutable() : AnalyzerConfigOptions.EmptyDictionary, instance.ToImmutableAndFree());
                if (!_optionsCache.TryAdd(list, value))
                {
                    freeKey(list, _sectionKeyPool);
                }
                builder.Clear();
                builder2.Clear();
                _treeOptionsPool.Free(builder);
                _analyzerOptionsPool.Free(builder2);
            }
            else
            {
                freeKey(list, _sectionKeyPool);
            }
            return value;
            static void freeKey(List<AnalyzerConfig.Section> sectionKey, ObjectPool<List<AnalyzerConfig.Section>> pool)
            {
                sectionKey.Clear();
                pool.Free(sectionKey);
            }
        }

        internal static bool TryParseSeverity(string value, out ReportDiagnostic severity)
        {
            StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
            if (ordinalIgnoreCase.Equals(value, "default"))
            {
                severity = ReportDiagnostic.Default;
                return true;
            }
            if (ordinalIgnoreCase.Equals(value, "error"))
            {
                severity = ReportDiagnostic.Error;
                return true;
            }
            if (ordinalIgnoreCase.Equals(value, "warning"))
            {
                severity = ReportDiagnostic.Warn;
                return true;
            }
            if (ordinalIgnoreCase.Equals(value, "suggestion"))
            {
                severity = ReportDiagnostic.Info;
                return true;
            }
            if (ordinalIgnoreCase.Equals(value, "silent") || ordinalIgnoreCase.Equals(value, "refactoring"))
            {
                severity = ReportDiagnostic.Hidden;
                return true;
            }
            if (ordinalIgnoreCase.Equals(value, "none"))
            {
                severity = ReportDiagnostic.Suppress;
                return true;
            }
            severity = ReportDiagnostic.Default;
            return false;
        }

        private AnalyzerConfigOptionsResult ParseGlobalConfigOptions()
        {
            ImmutableDictionary<string, ReportDiagnostic>.Builder builder = _treeOptionsPool.Allocate();
            ImmutableDictionary<string, string>.Builder builder2 = _analyzerOptionsPool.Allocate();
            ArrayBuilder<Diagnostic> instance = ArrayBuilder<Diagnostic>.GetInstance();
            ParseSectionOptions(_globalConfig.GlobalSection, builder, builder2, instance, "<Global Config>", _diagnosticIdCache);
            AnalyzerConfigOptionsResult result = new AnalyzerConfigOptionsResult(builder.ToImmutable(), builder2.ToImmutable(), instance.ToImmutableAndFree());
            builder.Clear();
            builder2.Clear();
            _treeOptionsPool.Free(builder);
            _analyzerOptionsPool.Free(builder2);
            return result;
        }

        private static void ParseSectionOptions(AnalyzerConfig.Section section, ImmutableDictionary<string, ReportDiagnostic>.Builder treeBuilder, ImmutableDictionary<string, string>.Builder analyzerBuilder, ArrayBuilder<Diagnostic> diagnosticBuilder, string analyzerConfigPath, ConcurrentDictionary<ReadOnlyMemory<char>, string> diagIdCache)
        {
            foreach (KeyValuePair<string, string> property in section.Properties)
            {
                KeyValuePairUtil.Deconstruct(property, out var key, out var value);
                string text = key;
                string text2 = value;
                int num = -1;
                if (text.StartsWith("dotnet_diagnostic.", StringComparison.Ordinal) && text.EndsWith(".severity", StringComparison.Ordinal))
                {
                    num = text.Length - ("dotnet_diagnostic.".Length + ".severity".Length);
                }
                if (num >= 0)
                {
                    ReadOnlyMemory<char> key2 = MemoryExtensions.AsMemory(text).Slice("dotnet_diagnostic.".Length, num);
                    if (!diagIdCache.TryGetValue(key2, out var value2))
                    {
                        value2 = key2.ToString();
                        value2 = diagIdCache.GetOrAdd(MemoryExtensions.AsMemory(value2), value2);
                    }
                    if (TryParseSeverity(text2, out var severity))
                    {
                        treeBuilder[value2] = severity;
                        continue;
                    }
                    diagnosticBuilder.Add(Diagnostic.Create(InvalidAnalyzerConfigSeverityDescriptor, Location.None, value2, text2, analyzerConfigPath));
                }
                else
                {
                    analyzerBuilder[text] = text2;
                }
            }
        }

        internal static GlobalAnalyzerConfig MergeGlobalConfigs(ArrayBuilder<AnalyzerConfig> analyzerConfigs, out ImmutableArray<Diagnostic> diagnostics)
        {
            GlobalAnalyzerConfigBuilder globalAnalyzerConfigBuilder = default(GlobalAnalyzerConfigBuilder);
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            for (int i = 0; i < analyzerConfigs.Count; i++)
            {
                if (analyzerConfigs[i].IsGlobal)
                {
                    globalAnalyzerConfigBuilder.MergeIntoGlobalConfig(analyzerConfigs[i], instance);
                    analyzerConfigs.RemoveAt(i);
                    i--;
                }
            }
            GlobalAnalyzerConfig result = globalAnalyzerConfigBuilder.Build(instance);
            diagnostics = instance.ToReadOnlyAndFree();
            return result;
        }
    }
}
