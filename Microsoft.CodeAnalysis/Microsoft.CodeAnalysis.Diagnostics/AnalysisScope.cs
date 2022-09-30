using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public class AnalysisScope
    {
        private readonly Lazy<ImmutableHashSet<DiagnosticAnalyzer>> _lazyAnalyzersSet;

        public SourceOrAdditionalFile? FilterFileOpt { get; }

        public TextSpan? FilterSpanOpt { get; }

        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }

        public IEnumerable<SyntaxTree> SyntaxTrees { get; }

        public IEnumerable<AdditionalText> AdditionalFiles { get; }

        public bool ConcurrentAnalysis { get; }

        public bool CategorizeDiagnostics { get; }

        public bool IsSyntacticSingleFileAnalysis { get; }

        public bool IsSingleFileAnalysis => FilterFileOpt.HasValue;

        public bool IsPartialAnalysis { get; }

        public AnalysisScope(Compilation compilation, AnalyzerOptions? analyzerOptions, ImmutableArray<DiagnosticAnalyzer> analyzers, bool hasAllAnalyzers, bool concurrentAnalysis, bool categorizeDiagnostics)
            : this(compilation.SyntaxTrees, analyzerOptions?.AdditionalFiles ?? ImmutableArray<AdditionalText>.Empty, analyzers, !hasAllAnalyzers, null, null, isSyntacticSingleFileAnalysis: false, concurrentAnalysis, categorizeDiagnostics)
        {
        }

        public AnalysisScope(ImmutableArray<DiagnosticAnalyzer> analyzers, SourceOrAdditionalFile filterFile, TextSpan? filterSpan, bool isSyntacticSingleFileAnalysis, bool concurrentAnalysis, bool categorizeDiagnostics)
            : this((filterFile.SourceTree != null) ? SpecializedCollections.SingletonEnumerable(filterFile.SourceTree) : SpecializedCollections.EmptyEnumerable<SyntaxTree>(), (filterFile.AdditionalFile != null) ? SpecializedCollections.SingletonEnumerable(filterFile.AdditionalFile) : SpecializedCollections.EmptyEnumerable<AdditionalText>(), analyzers, isPartialAnalysis: true, filterFile, filterSpan, isSyntacticSingleFileAnalysis, concurrentAnalysis, categorizeDiagnostics)
        {
        }

        private AnalysisScope(IEnumerable<SyntaxTree> trees, IEnumerable<AdditionalText> additionalFiles, ImmutableArray<DiagnosticAnalyzer> analyzers, bool isPartialAnalysis, SourceOrAdditionalFile? filterFile, TextSpan? filterSpanOpt, bool isSyntacticSingleFileAnalysis, bool concurrentAnalysis, bool categorizeDiagnostics)
        {
            SyntaxTrees = trees;
            AdditionalFiles = additionalFiles;
            Analyzers = analyzers;
            IsPartialAnalysis = isPartialAnalysis;
            FilterFileOpt = filterFile;
            FilterSpanOpt = filterSpanOpt;
            IsSyntacticSingleFileAnalysis = isSyntacticSingleFileAnalysis;
            ConcurrentAnalysis = concurrentAnalysis;
            CategorizeDiagnostics = categorizeDiagnostics;
            _lazyAnalyzersSet = new Lazy<ImmutableHashSet<DiagnosticAnalyzer>>(CreateAnalyzersSet);
        }

        private ImmutableHashSet<DiagnosticAnalyzer> CreateAnalyzersSet()
        {
            return Analyzers.ToImmutableHashSet();
        }

        public bool Contains(DiagnosticAnalyzer analyzer)
        {
            if (!IsPartialAnalysis)
            {
                return true;
            }
            return _lazyAnalyzersSet.Value.Contains(analyzer);
        }

        public AnalysisScope WithAnalyzers(ImmutableArray<DiagnosticAnalyzer> analyzers, bool hasAllAnalyzers)
        {
            bool isPartialAnalysis = IsSingleFileAnalysis || !hasAllAnalyzers;
            return new AnalysisScope(SyntaxTrees, AdditionalFiles, analyzers, isPartialAnalysis, FilterFileOpt, FilterSpanOpt, IsSyntacticSingleFileAnalysis, ConcurrentAnalysis, CategorizeDiagnostics);
        }

        public static bool ShouldSkipSymbolAnalysis(SymbolDeclaredCompilationEvent symbolEvent)
        {
            if (!symbolEvent.Symbol.IsImplicitlyDeclared)
            {
                return symbolEvent.DeclaringSyntaxReferences.All((SyntaxReference s) => s.SyntaxTree == null);
            }
            return true;
        }

        public static bool ShouldSkipDeclarationAnalysis(ISymbol symbol)
        {
            if (symbol.IsImplicitlyDeclared)
            {
                if (symbol.Kind == SymbolKind.Namespace)
                {
                    return !((INamespaceSymbol)symbol).IsGlobalNamespace;
                }
                return true;
            }
            return false;
        }

        public bool ShouldAnalyze(SyntaxTree tree)
        {
            if (FilterFileOpt.HasValue)
            {
                return FilterFileOpt.Value.SourceTree == tree;
            }
            return true;
        }

        public bool ShouldAnalyze(AdditionalText file)
        {
            if (FilterFileOpt.HasValue)
            {
                return FilterFileOpt.Value.AdditionalFile == file;
            }
            return true;
        }

        public bool ShouldAnalyze(ISymbol symbol)
        {
            if (!FilterFileOpt.HasValue)
            {
                return true;
            }
            if (FilterFileOpt.Value.SourceTree == null)
            {
                return false;
            }
            ImmutableArray<Location>.Enumerator enumerator = symbol.Locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Location current = enumerator.Current;
                if (FilterFileOpt.Value.SourceTree == current.SourceTree && ShouldInclude(current.SourceSpan))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ShouldAnalyze(SyntaxNode node)
        {
            if (!FilterFileOpt.HasValue)
            {
                return true;
            }
            if (FilterFileOpt.Value.SourceTree == null)
            {
                return false;
            }
            return ShouldInclude(node.FullSpan);
        }

        private bool ShouldInclude(TextSpan filterSpan)
        {
            if (FilterSpanOpt.HasValue)
            {
                return FilterSpanOpt.Value.IntersectsWith(filterSpan);
            }
            return true;
        }

        public bool ContainsSpan(TextSpan filterSpan)
        {
            if (FilterSpanOpt.HasValue)
            {
                return FilterSpanOpt.Value.Contains(filterSpan);
            }
            return true;
        }

        public bool ShouldInclude(Diagnostic diagnostic)
        {
            if (!FilterFileOpt.HasValue)
            {
                return true;
            }
            if (diagnostic.Location.IsInSource)
            {
                if (diagnostic.Location.SourceTree != FilterFileOpt.Value.SourceTree)
                {
                    return false;
                }
            }
            else if (diagnostic.Location is ExternalFileLocation externalFileLocation && (FilterFileOpt.Value.AdditionalFile == null || !PathUtilities.Comparer.Equals(externalFileLocation.FilePath, FilterFileOpt.Value.AdditionalFile!.Path)))
            {
                return false;
            }
            return ShouldInclude(diagnostic.Location.SourceSpan);
        }
    }
}
