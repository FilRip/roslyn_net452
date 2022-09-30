using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.Symbols;

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

namespace Microsoft.CodeAnalysis
{
    public struct CachedUseSiteInfo<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        private class Boxed
        {
            public readonly DiagnosticInfo DiagnosticInfo;

            public readonly ImmutableHashSet<TAssemblySymbol> Dependencies;

            public Boxed(DiagnosticInfo diagnosticInfo, ImmutableHashSet<TAssemblySymbol> dependencies)
            {
                DiagnosticInfo = diagnosticInfo;
                Dependencies = dependencies;
            }
        }

        private object? _info;

        private static readonly object Sentinel = new object();

        public static readonly CachedUseSiteInfo<TAssemblySymbol> Uninitialized = new CachedUseSiteInfo<TAssemblySymbol>(Sentinel);

        public bool IsInitialized => _info != Sentinel;

        private CachedUseSiteInfo(object info)
        {
            _info = info;
        }

        public void Initialize(DiagnosticInfo? diagnosticInfo)
        {
            Initialize(diagnosticInfo, ImmutableHashSet<TAssemblySymbol>.Empty);
        }

        public void Initialize(TAssemblySymbol? primaryDependency, UseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            Initialize(useSiteInfo.DiagnosticInfo, GetDependenciesToCache(primaryDependency, useSiteInfo));
        }

        private static ImmutableHashSet<TAssemblySymbol> GetDependenciesToCache(TAssemblySymbol? primaryDependency, UseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            ImmutableHashSet<TAssemblySymbol> immutableHashSet = useSiteInfo.SecondaryDependencies ?? ImmutableHashSet<TAssemblySymbol>.Empty;
            if (useSiteInfo.PrimaryDependency != null)
            {
                return immutableHashSet.Remove(useSiteInfo.PrimaryDependency);
            }
            return immutableHashSet;
        }

        public UseSiteInfo<TAssemblySymbol> ToUseSiteInfo(TAssemblySymbol primaryDependency)
        {
            Expand(_info, out var diagnosticInfo, out var dependencies);
            if (diagnosticInfo != null && diagnosticInfo.Severity == DiagnosticSeverity.Error)
            {
                return new UseSiteInfo<TAssemblySymbol>(diagnosticInfo);
            }
            return new UseSiteInfo<TAssemblySymbol>(diagnosticInfo, primaryDependency, dependencies);
        }

        private void Initialize(DiagnosticInfo? diagnosticInfo, ImmutableHashSet<TAssemblySymbol> dependencies)
        {
            _info = Compact(diagnosticInfo, dependencies);
        }

        private static object? Compact(DiagnosticInfo? diagnosticInfo, ImmutableHashSet<TAssemblySymbol> dependencies)
        {
            if (dependencies.IsEmpty)
            {
                return diagnosticInfo;
            }
            if (diagnosticInfo == null)
            {
                return dependencies;
            }
            return new Boxed(diagnosticInfo, dependencies);
        }

        public void InterlockedCompareExchange(TAssemblySymbol? primaryDependency, UseSiteInfo<TAssemblySymbol> value)
        {
            if (_info == Sentinel)
            {
                object value2 = Compact(value.DiagnosticInfo, GetDependenciesToCache(primaryDependency, value));
                Interlocked.CompareExchange(ref _info, value2, Sentinel);
            }
        }

        public UseSiteInfo<TAssemblySymbol> InterlockedInitialize(TAssemblySymbol? primaryDependency, UseSiteInfo<TAssemblySymbol> value)
        {
            object value2 = Compact(value.DiagnosticInfo, GetDependenciesToCache(primaryDependency, value));
            value2 = Interlocked.CompareExchange(ref _info, value2, null);
            if (value2 == null)
            {
                return value;
            }
            Expand(value2, out var diagnosticInfo, out var dependencies);
            return new UseSiteInfo<TAssemblySymbol>(diagnosticInfo, value.PrimaryDependency, dependencies);
        }

        private static void Expand(object? info, out DiagnosticInfo? diagnosticInfo, out ImmutableHashSet<TAssemblySymbol>? dependencies)
        {
            if (info != null)
            {
                if (!(info is DiagnosticInfo diagnosticInfo2))
                {
                    if (info is ImmutableHashSet<TAssemblySymbol> immutableHashSet)
                    {
                        diagnosticInfo = null;
                        dependencies = immutableHashSet;
                    }
                    else
                    {
                        Boxed boxed = (Boxed)info;
                        diagnosticInfo = boxed.DiagnosticInfo;
                        dependencies = boxed.Dependencies;
                    }
                }
                else
                {
                    diagnosticInfo = diagnosticInfo2;
                    dependencies = null;
                }
            }
            else
            {
                diagnosticInfo = null;
                dependencies = null;
            }
        }
    }
}
