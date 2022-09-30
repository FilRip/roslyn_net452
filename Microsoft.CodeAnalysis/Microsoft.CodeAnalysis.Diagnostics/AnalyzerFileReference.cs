using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

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

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class AnalyzerFileReference : AnalyzerReference, IEquatable<AnalyzerReference>
    {
        private delegate IEnumerable<string> AttributeLanguagesFunc(PEModule module, CustomAttributeHandle attribute);

        private sealed class Extensions<TExtension> where TExtension : class
        {
            private class ExtTypeComparer : IEqualityComparer<TExtension>
            {
                public static readonly ExtTypeComparer Instance = new ExtTypeComparer();

                public bool Equals(TExtension? x, TExtension? y)
                {
                    return object.Equals(x?.GetType(), y?.GetType());
                }

                public int GetHashCode(TExtension obj)
                {
                    return obj.GetType().GetHashCode();
                }
            }

            private readonly AnalyzerFileReference _reference;

            private readonly Type _attributeType;

            private readonly AttributeLanguagesFunc _languagesFunc;

            private readonly bool _allowNetFramework;

            private ImmutableArray<TExtension> _lazyAllExtensions;

            private ImmutableDictionary<string, ImmutableArray<TExtension>> _lazyExtensionsPerLanguage;

            private ImmutableSortedDictionary<string, ImmutableSortedSet<string>>? _lazyExtensionTypeNameMap;

            internal Extensions(AnalyzerFileReference reference, Type attributeType, AttributeLanguagesFunc languagesFunc, bool allowNetFramework)
            {
                _reference = reference;
                _attributeType = attributeType;
                _languagesFunc = languagesFunc;
                _allowNetFramework = allowNetFramework;
                _lazyAllExtensions = default(ImmutableArray<TExtension>);
                _lazyExtensionsPerLanguage = ImmutableDictionary<string, ImmutableArray<TExtension>>.Empty;
            }

            internal ImmutableArray<TExtension> GetExtensionsForAllLanguages(bool includeDuplicates)
            {
                if (_lazyAllExtensions.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyAllExtensions, CreateExtensionsForAllLanguages(this, includeDuplicates));
                }
                return _lazyAllExtensions;
            }

            private static ImmutableArray<TExtension> CreateExtensionsForAllLanguages(Extensions<TExtension> extensions, bool includeDuplicates)
            {
                ImmutableSortedDictionary<string, ImmutableArray<TExtension>>.Builder builder = ImmutableSortedDictionary.CreateBuilder<string, ImmutableArray<TExtension>>(StringComparer.OrdinalIgnoreCase);
                extensions.AddExtensions(builder);
                ImmutableArray<TExtension>.Builder builder2 = ImmutableArray.CreateBuilder<TExtension>();
                foreach (ImmutableArray<TExtension> value in builder.Values)
                {
                    ImmutableArray<TExtension>.Enumerator enumerator2 = value.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        TExtension current = enumerator2.Current;
                        builder2.Add(current);
                    }
                }
                if (includeDuplicates)
                {
                    return builder2.ToImmutable();
                }
                return builder2.Distinct(ExtTypeComparer.Instance).ToImmutableArray();
            }

            internal ImmutableArray<TExtension> GetExtensions(string language)
            {
                if (string.IsNullOrEmpty(language))
                {
                    throw new ArgumentException("language");
                }
                return ImmutableInterlocked.GetOrAdd(ref _lazyExtensionsPerLanguage, language, CreateLanguageSpecificExtensions, this);
            }

            private static ImmutableArray<TExtension> CreateLanguageSpecificExtensions(string language, Extensions<TExtension> extensions)
            {
                ImmutableArray<TExtension>.Builder builder = ImmutableArray.CreateBuilder<TExtension>();
                extensions.AddExtensions(builder, language);
                return builder.ToImmutable();
            }

            internal ImmutableSortedDictionary<string, ImmutableSortedSet<string>> GetExtensionTypeNameMap()
            {
                if (_lazyExtensionTypeNameMap == null)
                {
                    ImmutableSortedDictionary<string, ImmutableSortedSet<string>> analyzerTypeNameMap = GetAnalyzerTypeNameMap(_reference.FullPath, _attributeType, _languagesFunc);
                    Interlocked.CompareExchange(ref _lazyExtensionTypeNameMap, analyzerTypeNameMap, null);
                }
                return _lazyExtensionTypeNameMap;
            }

            internal void AddExtensions(ImmutableSortedDictionary<string, ImmutableArray<TExtension>>.Builder builder)
            {
                ImmutableSortedDictionary<string, ImmutableSortedSet<string>> extensionTypeNameMap;
                Assembly assembly;
                try
                {
                    extensionTypeNameMap = GetExtensionTypeNameMap();
                    if (extensionTypeNameMap.Count == 0)
                    {
                        return;
                    }
                    assembly = _reference.GetAssembly();
                }
                catch (Exception e)
                {
                    _reference.AnalyzerLoadFailed?.Invoke(_reference, CreateAnalyzerFailedArgs(e));
                    return;
                }
                int count = builder.Count;
                bool reportedError = false;
                foreach (var (text2, _) in extensionTypeNameMap)
                {
                    if (text2 != null)
                    {
                        ImmutableArray<TExtension> languageSpecificAnalyzers = GetLanguageSpecificAnalyzers(assembly, extensionTypeNameMap, text2, ref reportedError);
                        builder.Add(text2, languageSpecificAnalyzers);
                    }
                }
                if (builder.Count == count && !reportedError)
                {
                    _reference.AnalyzerLoadFailed?.Invoke(_reference, new AnalyzerLoadFailureEventArgs(AnalyzerLoadFailureEventArgs.FailureErrorCode.NoAnalyzers, CodeAnalysisResources.NoAnalyzersFound));
                }
            }

            internal void AddExtensions(ImmutableArray<TExtension>.Builder builder, string language)
            {
                ImmutableSortedDictionary<string, ImmutableSortedSet<string>> extensionTypeNameMap;
                Assembly assembly;
                try
                {
                    extensionTypeNameMap = GetExtensionTypeNameMap();
                    if (!extensionTypeNameMap.ContainsKey(language))
                    {
                        return;
                    }
                    assembly = _reference.GetAssembly();
                    if (assembly == null)
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    _reference.AnalyzerLoadFailed?.Invoke(_reference, CreateAnalyzerFailedArgs(e));
                    return;
                }
                int count = builder.Count;
                bool reportedError = false;
                ImmutableArray<TExtension> languageSpecificAnalyzers = GetLanguageSpecificAnalyzers(assembly, extensionTypeNameMap, language, ref reportedError);
                builder.AddRange(languageSpecificAnalyzers);
                if (builder.Count == count && !reportedError)
                {
                    _reference.AnalyzerLoadFailed?.Invoke(_reference, new AnalyzerLoadFailureEventArgs(AnalyzerLoadFailureEventArgs.FailureErrorCode.NoAnalyzers, CodeAnalysisResources.NoAnalyzersFound));
                }
            }

            private ImmutableArray<TExtension> GetLanguageSpecificAnalyzers(Assembly analyzerAssembly, ImmutableSortedDictionary<string, ImmutableSortedSet<string>> analyzerTypeNameMap, string language, ref bool reportedError)
            {
                if (!analyzerTypeNameMap.TryGetValue(language, out var value))
                {
                    return ImmutableArray<TExtension>.Empty;
                }
                return GetAnalyzersForTypeNames(analyzerAssembly, value, ref reportedError);
            }

            private ImmutableArray<TExtension> GetAnalyzersForTypeNames(Assembly analyzerAssembly, IEnumerable<string> analyzerTypeNames, ref bool reportedError)
            {
                ImmutableArray<TExtension>.Builder builder = ImmutableArray.CreateBuilder<TExtension>();
                foreach (string analyzerTypeName in analyzerTypeNames)
                {
                    Type type;
                    try
                    {
                        type = analyzerAssembly.GetType(analyzerTypeName, throwOnError: true, ignoreCase: false);
                    }
                    catch (Exception e)
                    {
                        _reference.AnalyzerLoadFailed?.Invoke(_reference, CreateAnalyzerFailedArgs(e, analyzerTypeName));
                        reportedError = true;
                        continue;
                    }
                    if (!_allowNetFramework)
                    {
                        TargetFrameworkAttribute customAttribute = analyzerAssembly.GetCustomAttribute<TargetFrameworkAttribute>();
                        if (customAttribute != null && customAttribute.FrameworkName.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase))
                        {
                            _reference.AnalyzerLoadFailed?.Invoke(_reference, new AnalyzerLoadFailureEventArgs(AnalyzerLoadFailureEventArgs.FailureErrorCode.ReferencesFramework, string.Format(CodeAnalysisResources.AssemblyReferencesNetFramework, analyzerTypeName), null, analyzerTypeName));
                            continue;
                        }
                    }
                    TExtension val;
                    try
                    {
                        val = Activator.CreateInstance(type) as TExtension;
                    }
                    catch (Exception e2)
                    {
                        _reference.AnalyzerLoadFailed?.Invoke(_reference, CreateAnalyzerFailedArgs(e2, analyzerTypeName));
                        reportedError = true;
                        continue;
                    }
                    if (val != null)
                    {
                        builder.Add(val);
                    }
                }
                return builder.ToImmutable();
            }
        }

        private readonly IAnalyzerAssemblyLoader _assemblyLoader;

        private readonly Extensions<DiagnosticAnalyzer> _diagnosticAnalyzers;

        private readonly Extensions<ISourceGenerator> _generators;

        private string? _lazyDisplay;

        private object? _lazyIdentity;

        private Assembly? _lazyAssembly;

        public override string FullPath { get; }

        public IAnalyzerAssemblyLoader AssemblyLoader => _assemblyLoader;

        public override string Display
        {
            get
            {
                if (_lazyDisplay == null)
                {
                    InitializeDisplayAndId();
                }
                return _lazyDisplay;
            }
        }

        public override object Id
        {
            get
            {
                if (_lazyIdentity == null)
                {
                    InitializeDisplayAndId();
                }
                return _lazyIdentity;
            }
        }

        public event EventHandler<AnalyzerLoadFailureEventArgs>? AnalyzerLoadFailed;

        public AnalyzerFileReference(string fullPath, IAnalyzerAssemblyLoader assemblyLoader)
        {
            CompilerPathUtilities.RequireAbsolutePath(fullPath, "fullPath");
            FullPath = fullPath;
            _assemblyLoader = assemblyLoader ?? throw new ArgumentNullException("assemblyLoader");
            _diagnosticAnalyzers = new Extensions<DiagnosticAnalyzer>(this, typeof(DiagnosticAnalyzerAttribute), GetDiagnosticsAnalyzerSupportedLanguages, allowNetFramework: true);
            _generators = new Extensions<ISourceGenerator>(this, typeof(GeneratorAttribute), GetGeneratorSupportedLanguages, allowNetFramework: false);
            assemblyLoader.AddDependencyLocation(fullPath);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AnalyzerFileReference);
        }

        public bool Equals(AnalyzerFileReference? other)
        {
            if (this == other)
            {
                return true;
            }
            if (other != null && _assemblyLoader == other!._assemblyLoader)
            {
                return FullPath == other!.FullPath;
            }
            return false;
        }

        public bool Equals(AnalyzerReference? other)
        {
            if (this == other)
            {
                return true;
            }
            if (other == null)
            {
                return false;
            }
            if (other is AnalyzerFileReference other2)
            {
                return Equals(other2);
            }
            return FullPath == other!.FullPath;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(RuntimeHelpers.GetHashCode(_assemblyLoader), FullPath.GetHashCode());
        }

        public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzersForAllLanguages()
        {
            return _diagnosticAnalyzers.GetExtensionsForAllLanguages(includeDuplicates: true);
        }

        public override ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(string language)
        {
            return _diagnosticAnalyzers.GetExtensions(language);
        }

        public override ImmutableArray<ISourceGenerator> GetGeneratorsForAllLanguages()
        {
            return _generators.GetExtensionsForAllLanguages(includeDuplicates: false);
        }

        [Obsolete("Use GetGenerators(string language) or GetGeneratorsForAllLanguages()")]
        public override ImmutableArray<ISourceGenerator> GetGenerators()
        {
            return _generators.GetExtensions("C#");
        }

        public override ImmutableArray<ISourceGenerator> GetGenerators(string language)
        {
            return _generators.GetExtensions(language);
        }

        [System.Diagnostics.CodeAnalysis.MemberNotNull(new string[] { "_lazyIdentity", "_lazyDisplay" })]
        private void InitializeDisplayAndId()
        {
            try
            {
                using PEReader peReader = new PEReader(FileUtilities.OpenRead(FullPath));
                AssemblyIdentity assemblyIdentity = peReader.GetMetadataReader().ReadAssemblyIdentityOrThrow();
                _lazyDisplay = assemblyIdentity.Name;
                _lazyIdentity = assemblyIdentity;
            }
            catch
            {
                _lazyDisplay = FileNameUtilities.GetFileName(FullPath, includeExtension: false);
                _lazyIdentity = _lazyDisplay;
            }
        }

        internal void AddAnalyzers(ImmutableArray<DiagnosticAnalyzer>.Builder builder, string language)
        {
            _diagnosticAnalyzers.AddExtensions(builder, language);
        }

        internal void AddGenerators(ImmutableArray<ISourceGenerator>.Builder builder, string language)
        {
            _generators.AddExtensions(builder, language);
        }

        private static AnalyzerLoadFailureEventArgs CreateAnalyzerFailedArgs(Exception e, string? typeName = null)
        {
            e = (e as TargetInvocationException) ?? e;
            string message = e.Message.Replace("\r", "").Replace("\n", "");
            return new AnalyzerLoadFailureEventArgs((typeName == null) ? AnalyzerLoadFailureEventArgs.FailureErrorCode.UnableToLoadAnalyzer : AnalyzerLoadFailureEventArgs.FailureErrorCode.UnableToCreateAnalyzer, message, e, typeName);
        }

        internal ImmutableSortedDictionary<string, ImmutableSortedSet<string>> GetAnalyzerTypeNameMap()
        {
            return _diagnosticAnalyzers.GetExtensionTypeNameMap();
        }

        private static ImmutableSortedDictionary<string, ImmutableSortedSet<string>> GetAnalyzerTypeNameMap(string fullPath, Type attributeType, AttributeLanguagesFunc languagesFunc)
        {
            Type attributeType2 = attributeType;
            AttributeLanguagesFunc languagesFunc2 = languagesFunc;
            using AssemblyMetadata assemblyMetadata = AssemblyMetadata.CreateFromFile(fullPath);
            return (from module in assemblyMetadata.GetModules()
                    from typeDefHandle in module.MetadataReader.TypeDefinitions
                    let typeDef = module.MetadataReader.GetTypeDefinition(typeDefHandle)
                    let supportedLanguages = GetSupportedLanguages(typeDef, module.Module, attributeType2, languagesFunc2)
                    where supportedLanguages.Any()
                    let typeName = GetFullyQualifiedTypeName(typeDef, module.Module)
                    from supportedLanguage in supportedLanguages
                    group typeName by supportedLanguage).ToImmutableSortedDictionary((IGrouping<string, string> g) => g.Key, (IGrouping<string, string> g) => g.ToImmutableSortedSet(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetSupportedLanguages(TypeDefinition typeDef, PEModule peModule, Type attributeType, AttributeLanguagesFunc languagesFunc)
        {
            IEnumerable<string> enumerable = null;
            foreach (CustomAttributeHandle customAttribute in typeDef.GetCustomAttributes())
            {
                if (peModule.IsTargetAttribute(customAttribute, attributeType.Namespace, attributeType.Name, out var _))
                {
                    IEnumerable<string> enumerable2 = languagesFunc(peModule, customAttribute);
                    if (enumerable2 != null)
                    {
                        enumerable = ((enumerable != null) ? enumerable.Concat(enumerable2) : enumerable2);
                    }
                }
            }
            return enumerable ?? SpecializedCollections.EmptyEnumerable<string>();
        }

        private static IEnumerable<string> GetDiagnosticsAnalyzerSupportedLanguages(PEModule peModule, CustomAttributeHandle customAttrHandle)
        {
            BlobReader argsReader = peModule.GetMemoryReaderOrThrow(peModule.GetCustomAttributeValueOrThrow(customAttrHandle));
            return ReadLanguagesFromAttribute(ref argsReader);
        }

        private static IEnumerable<string> GetGeneratorSupportedLanguages(PEModule peModule, CustomAttributeHandle customAttrHandle)
        {
            BlobReader argsReader = peModule.GetMemoryReaderOrThrow(peModule.GetCustomAttributeValueOrThrow(customAttrHandle));
            if (argsReader.Length == 4)
            {
                return ImmutableArray.Create("C#");
            }
            return ReadLanguagesFromAttribute(ref argsReader);
        }

        private static IEnumerable<string> ReadLanguagesFromAttribute(ref BlobReader argsReader)
        {
            if (argsReader.Length > 4 && argsReader.ReadByte() == 1 && argsReader.ReadByte() == 0)
            {
                if (!PEModule.CrackStringInAttributeValue(out var value, ref argsReader))
                {
                    return SpecializedCollections.EmptyEnumerable<string>();
                }
                if (PEModule.CrackStringArrayInAttributeValue(out var value2, ref argsReader))
                {
                    if (value2.Length == 0)
                    {
                        return SpecializedCollections.SingletonEnumerable(value);
                    }
                    return value2.Insert(0, value);
                }
            }
            return SpecializedCollections.EmptyEnumerable<string>();
        }

        private static string GetFullyQualifiedTypeName(TypeDefinition typeDef, PEModule peModule)
        {
            TypeDefinitionHandle declaringType = typeDef.GetDeclaringType();
            if (declaringType.IsNil)
            {
                return peModule.GetFullNameOrThrow(typeDef.Namespace, typeDef.Name);
            }
            return GetFullyQualifiedTypeName(peModule.MetadataReader.GetTypeDefinition(declaringType), peModule) + "+" + peModule.MetadataReader.GetString(typeDef.Name);
        }

        public Assembly GetAssembly()
        {
            if (_lazyAssembly == null)
            {
                _lazyAssembly = _assemblyLoader.LoadFromPath(FullPath);
            }
            return _lazyAssembly;
        }
    }
}
