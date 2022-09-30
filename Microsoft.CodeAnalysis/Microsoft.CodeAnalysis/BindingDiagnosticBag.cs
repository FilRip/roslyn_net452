using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class BindingDiagnosticBag
    {
        public readonly DiagnosticBag? DiagnosticBag;

        public bool AccumulatesDiagnostics => DiagnosticBag != null;

        protected BindingDiagnosticBag(DiagnosticBag? diagnosticBag)
        {
            DiagnosticBag = diagnosticBag;
        }

        public void AddRange<T>(ImmutableArray<T> diagnostics) where T : Diagnostic
        {
            DiagnosticBag?.AddRange(diagnostics);
        }

        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            DiagnosticBag?.AddRange(diagnostics);
        }

        public bool HasAnyResolvedErrors()
        {
            return DiagnosticBag?.HasAnyResolvedErrors() ?? false;
        }

        public bool HasAnyErrors()
        {
            return DiagnosticBag?.HasAnyErrors() ?? false;
        }

        public void Add(Diagnostic diag)
        {
            DiagnosticBag?.Add(diag);
        }
    }
    public abstract class BindingDiagnosticBag<TAssemblySymbol> : BindingDiagnosticBag where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        public readonly ICollection<TAssemblySymbol>? DependenciesBag;

        public bool AccumulatesDependencies => DependenciesBag != null;

        protected BindingDiagnosticBag(DiagnosticBag? diagnosticBag, ICollection<TAssemblySymbol>? dependenciesBag)
            : base(diagnosticBag)
        {
            DependenciesBag = dependenciesBag;
        }

        protected BindingDiagnosticBag(bool usePool)
            : this(usePool ? Microsoft.CodeAnalysis.DiagnosticBag.GetInstance() : new DiagnosticBag(), usePool ? PooledHashSet<TAssemblySymbol>.GetInstance() : new HashSet<TAssemblySymbol>())
        {
        }

        public void Free()
        {
            DiagnosticBag?.Free();
            ((PooledHashSet<TAssemblySymbol>)DependenciesBag)?.Free();
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> ToReadOnly()
        {
            return new ImmutableBindingDiagnostic<TAssemblySymbol>(DiagnosticBag?.ToReadOnly() ?? default(ImmutableArray<Diagnostic>), DependenciesBag?.ToImmutableArray() ?? default(ImmutableArray<TAssemblySymbol>));
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> ToReadOnlyAndFree()
        {
            ImmutableBindingDiagnostic<TAssemblySymbol> result = ToReadOnly();
            Free();
            return result;
        }

        public void AddRangeAndFree(BindingDiagnosticBag<TAssemblySymbol> other)
        {
            AddRange(other);
            other.Free();
        }

        public void Clear()
        {
            DiagnosticBag?.Clear();
            DependenciesBag?.Clear();
        }

        public void AddRange(ImmutableBindingDiagnostic<TAssemblySymbol> other, bool allowMismatchInDependencyAccumulation = false)
        {
            AddRange(other.Diagnostics);
            AddDependencies(other.Dependencies);
        }

        public void AddRange(BindingDiagnosticBag<TAssemblySymbol>? other, bool allowMismatchInDependencyAccumulation = false)
        {
            if (other != null)
            {
                AddRange(other!.DiagnosticBag);
                AddDependencies(other!.DependenciesBag);
            }
        }

        public void AddRange(DiagnosticBag? bag)
        {
            if (bag != null)
            {
                DiagnosticBag?.AddRange(bag);
            }
        }

        public void AddDependency(TAssemblySymbol? dependency)
        {
            if (dependency != null && DependenciesBag != null)
            {
                DependenciesBag!.Add(dependency);
            }
        }

        public void AddDependencies(ICollection<TAssemblySymbol>? dependencies)
        {
            if (dependencies.IsNullOrEmpty() || DependenciesBag == null)
            {
                return;
            }
            foreach (TAssemblySymbol item in dependencies!)
            {
                DependenciesBag!.Add(item);
            }
        }

        public void AddDependencies(IReadOnlyCollection<TAssemblySymbol>? dependencies)
        {
            if (dependencies.IsNullOrEmpty() || DependenciesBag == null)
            {
                return;
            }
            foreach (TAssemblySymbol item in dependencies!)
            {
                DependenciesBag!.Add(item);
            }
        }

        public void AddDependencies(ImmutableHashSet<TAssemblySymbol>? dependencies)
        {
            if (dependencies.IsNullOrEmpty() || DependenciesBag == null)
            {
                return;
            }
            foreach (TAssemblySymbol item in dependencies!)
            {
                DependenciesBag!.Add(item);
            }
        }

        public void AddDependencies(ImmutableArray<TAssemblySymbol> dependencies)
        {
            if (!dependencies.IsDefaultOrEmpty && DependenciesBag != null)
            {
                ImmutableArray<TAssemblySymbol>.Enumerator enumerator = dependencies.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TAssemblySymbol current = enumerator.Current;
                    DependenciesBag!.Add(current);
                }
            }
        }

        public void AddDependencies(BindingDiagnosticBag<TAssemblySymbol> dependencies, bool allowMismatchInDependencyAccumulation = false)
        {
            AddDependencies(dependencies.DependenciesBag);
        }

        public void AddDependencies(UseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            if (DependenciesBag != null)
            {
                AddDependency(useSiteInfo.PrimaryDependency);
                AddDependencies(useSiteInfo.SecondaryDependencies);
            }
        }

        public void AddDependencies(CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            if (DependenciesBag != null)
            {
                AddDependencies(useSiteInfo.Dependencies);
            }
        }

        public bool Add(SyntaxNode node, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            return Add(node.Location, useSiteInfo);
        }

        internal bool AddDiagnostics(SyntaxNode node, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            return AddDiagnostics(node.Location, useSiteInfo);
        }

        internal bool AddDiagnostics(Location location, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            DiagnosticBag diagnosticBag = DiagnosticBag;
            if (diagnosticBag != null)
            {
                if (!useSiteInfo.Diagnostics.IsNullOrEmpty())
                {
                    bool flag = false;
                    foreach (DiagnosticInfo item in useSiteInfo.Diagnostics!)
                    {
                        if (ReportUseSiteDiagnostic(item, diagnosticBag, location))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        return true;
                    }
                }
            }
            else if (useSiteInfo.AccumulatesDiagnostics && !useSiteInfo.Diagnostics.IsNullOrEmpty())
            {
                foreach (DiagnosticInfo item2 in useSiteInfo.Diagnostics!)
                {
                    if (item2.Severity == DiagnosticSeverity.Error)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Add(Location location, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            if (AddDiagnostics(location, useSiteInfo))
            {
                return true;
            }
            AddDependencies(useSiteInfo);
            return false;
        }

        protected abstract bool ReportUseSiteDiagnostic(DiagnosticInfo diagnosticInfo, DiagnosticBag diagnosticBag, Location location);

        public bool Add(UseSiteInfo<TAssemblySymbol> useSiteInfo, SyntaxNode node)
        {
            return Add(useSiteInfo, node.Location);
        }

        public bool Add(UseSiteInfo<TAssemblySymbol> info, Location location)
        {
            if (ReportUseSiteDiagnostic(info.DiagnosticInfo, location))
            {
                return true;
            }
            AddDependencies(info);
            return false;
        }

        public bool ReportUseSiteDiagnostic(DiagnosticInfo? info, Location location)
        {
            if (info == null)
            {
                return false;
            }
            if (DiagnosticBag != null)
            {
                return ReportUseSiteDiagnostic(info, DiagnosticBag, location);
            }
            return info!.Severity == DiagnosticSeverity.Error;
        }
    }
}
