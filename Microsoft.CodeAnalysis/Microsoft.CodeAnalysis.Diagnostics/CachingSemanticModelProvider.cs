using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CachingSemanticModelProvider : SemanticModelProvider
    {
        private sealed class PerCompilationProvider
        {
            private readonly Compilation _compilation;

            private readonly ConcurrentDictionary<SyntaxTree, SemanticModel> _semanticModelsMap;

            private readonly Func<SyntaxTree, SemanticModel> _createSemanticModel;

            public PerCompilationProvider(Compilation compilation)
            {
                Compilation compilation2 = compilation;
                //base._002Ector();
                _compilation = compilation2;
                _semanticModelsMap = new ConcurrentDictionary<SyntaxTree, SemanticModel>();
                _createSemanticModel = (SyntaxTree tree) => compilation2.CreateSemanticModel(tree, ignoreAccessibility: false);
            }

            public SemanticModel GetSemanticModel(SyntaxTree tree, bool ignoreAccessibility)
            {
                if (ignoreAccessibility)
                {
                    return _compilation.CreateSemanticModel(tree, ignoreAccessibility: true);
                }
                return _semanticModelsMap.GetOrAdd(tree, _createSemanticModel);
            }

            public void ClearCachedSemanticModel(SyntaxTree tree)
            {
                _semanticModelsMap.TryRemove(tree, out var _);
            }
        }

        private static readonly ConditionalWeakTable<Compilation, PerCompilationProvider>.CreateValueCallback s_createProviderCallback = (Compilation compilation) => new PerCompilationProvider(compilation);

        private readonly ConditionalWeakTable<Compilation, PerCompilationProvider> _providerCache;

        public CachingSemanticModelProvider()
        {
            _providerCache = new ConditionalWeakTable<Compilation, PerCompilationProvider>();
        }

        public override SemanticModel GetSemanticModel(SyntaxTree tree, Compilation compilation, bool ignoreAccessibility = false)
        {
            return _providerCache.GetValue(compilation, s_createProviderCallback).GetSemanticModel(tree, ignoreAccessibility);
        }

        internal void ClearCache(SyntaxTree tree, Compilation compilation)
        {
            if (_providerCache.TryGetValue(compilation, out var value))
            {
                value.ClearCachedSemanticModel(tree);
            }
        }

        internal void ClearCache(Compilation compilation)
        {
            _providerCache.Remove(compilation);
        }
    }
}
