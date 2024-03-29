// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// This is base class for a bag used to accumulate information while binding is performed.
    /// Including diagnostic messages and dependencies in the form of "used" assemblies. 
    /// </summary>
    public abstract class BindingDiagnosticBag
    {
        public readonly DiagnosticBag? DiagnosticBag;

        protected BindingDiagnosticBag(DiagnosticBag? diagnosticBag)
        {
            DiagnosticBag = diagnosticBag;
        }

        public bool AccumulatesDiagnostics => DiagnosticBag is not null;

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
            Debug.Assert(DiagnosticBag is not null);
            return DiagnosticBag?.HasAnyResolvedErrors() == true;
        }

        public bool HasAnyErrors()
        {
            Debug.Assert(DiagnosticBag is not null);
            return DiagnosticBag?.HasAnyErrors() == true;
        }

        public void Add(Diagnostic diag)
        {
            DiagnosticBag?.Add(diag);
        }
    }

    public abstract class BindingDiagnosticBag<TAssemblySymbol> : BindingDiagnosticBag
        where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        public readonly ICollection<TAssemblySymbol>? DependenciesBag;

        protected BindingDiagnosticBag(DiagnosticBag? diagnosticBag, ICollection<TAssemblySymbol>? dependenciesBag)
            : base(diagnosticBag)
        {
            Debug.Assert(diagnosticBag?.GetType().IsValueType != true);
            DependenciesBag = dependenciesBag;
        }

        protected BindingDiagnosticBag(bool usePool)
            : this(usePool ? DiagnosticBag.GetInstance() : new DiagnosticBag(), usePool ? PooledHashSet<TAssemblySymbol>.GetInstance() : new HashSet<TAssemblySymbol>())
        { }

        public bool AccumulatesDependencies => DependenciesBag is object;

        public void Free()
        {
            DiagnosticBag?.Free();
            ((PooledHashSet<TAssemblySymbol>?)DependenciesBag)?.Free();
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> ToReadOnly()
        {
            return new ImmutableBindingDiagnostic<TAssemblySymbol>(DiagnosticBag?.ToReadOnly() ?? default, DependenciesBag?.ToImmutableArray() ?? default);
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> ToReadOnlyAndFree()
        {
            var result = ToReadOnly();
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
            Debug.Assert(allowMismatchInDependencyAccumulation || other.Dependencies.IsDefaultOrEmpty || this.AccumulatesDependencies || !this.AccumulatesDiagnostics);
            AddDependencies(other.Dependencies);
        }

        public void AddRange(BindingDiagnosticBag<TAssemblySymbol>? other, bool allowMismatchInDependencyAccumulation = false)
        {
            if (other is not null)
            {
                AddRange(other.DiagnosticBag);
                Debug.Assert(allowMismatchInDependencyAccumulation || !other.AccumulatesDependencies || this.AccumulatesDependencies);
                AddDependencies(other.DependenciesBag);
            }
        }

        public void AddRange(DiagnosticBag? bag)
        {
            if (bag is not null)
            {
                DiagnosticBag?.AddRange(bag);
            }
        }

        public void AddDependency(TAssemblySymbol? dependency)
        {
            if (dependency is object && DependenciesBag is object)
            {
                DependenciesBag.Add(dependency);
            }
        }

        public void AddDependencies(ICollection<TAssemblySymbol>? dependencies)
        {
            if (!dependencies.IsNullOrEmpty() && DependenciesBag is object)
            {
                foreach (var candidate in dependencies)
                {
                    DependenciesBag.Add(candidate);
                }
            }
        }

        public void AddDependencies(IReadOnlyCollection<TAssemblySymbol>? dependencies)
        {
            if (!dependencies.IsNullOrEmpty() && DependenciesBag is object)
            {
                foreach (var candidate in dependencies)
                {
                    DependenciesBag.Add(candidate);
                }
            }
        }

        public void AddDependencies(ImmutableHashSet<TAssemblySymbol>? dependencies)
        {
            if (!dependencies.IsNullOrEmpty() && DependenciesBag is object)
            {
                foreach (var candidate in dependencies)
                {
                    DependenciesBag.Add(candidate);
                }
            }
        }

        public void AddDependencies(ImmutableArray<TAssemblySymbol> dependencies)
        {
            if (!dependencies.IsDefaultOrEmpty && DependenciesBag is object)
            {
                foreach (var candidate in dependencies)
                {
                    DependenciesBag.Add(candidate);
                }
            }
        }

        public void AddDependencies(BindingDiagnosticBag<TAssemblySymbol> dependencies, bool allowMismatchInDependencyAccumulation = false)
        {
            Debug.Assert(allowMismatchInDependencyAccumulation || !dependencies.AccumulatesDependencies || this.AccumulatesDependencies);
            AddDependencies(dependencies.DependenciesBag);
        }

        public void AddDependencies(UseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            if (DependenciesBag is object)
            {
                AddDependency(useSiteInfo.PrimaryDependency);
                AddDependencies(useSiteInfo.SecondaryDependencies);
            }
        }

        public void AddDependencies(CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            Debug.Assert(!useSiteInfo.AccumulatesDependencies || this.AccumulatesDependencies);
            if (DependenciesBag is object)
            {
                AddDependencies(useSiteInfo.Dependencies);
            }
        }

        public bool AddDiagnostics(SyntaxNode node, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            return AddDiagnostics(node.Location, useSiteInfo);
        }

        public bool AddDiagnostics(Location location, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            if (DiagnosticBag is DiagnosticBag diagnosticBag)
            {
                if (!useSiteInfo.Diagnostics.IsNullOrEmpty())
                {
                    bool haveError = false;
                    if (useSiteInfo.Diagnostics.Any(diagnosticInfo => ReportUseSiteDiagnostic(diagnosticInfo, diagnosticBag, location)))
                        haveError = true;

                    if (haveError)
                    {
                        return true;
                    }
                }
            }
            else if (useSiteInfo.AccumulatesDiagnostics && !useSiteInfo.Diagnostics.IsNullOrEmpty())
            {
                foreach (var info in useSiteInfo.Diagnostics)
                {
                    if (info.Severity == DiagnosticSeverity.Error)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Add(SyntaxNode node, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            return Add(node.Location, useSiteInfo);
        }

        public bool Add(Location location, CompoundUseSiteInfo<TAssemblySymbol> useSiteInfo)
        {
            Debug.Assert(!useSiteInfo.AccumulatesDependencies || this.AccumulatesDependencies);
            if (AddDiagnostics(location, useSiteInfo))
            {
                return true;
            }

            AddDependencies(useSiteInfo);
            return false;
        }

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

        protected abstract bool ReportUseSiteDiagnostic(DiagnosticInfo diagnosticInfo, DiagnosticBag diagnosticBag, Location location);

        public bool ReportUseSiteDiagnostic(DiagnosticInfo? info, Location location)
        {
            if (info is null)
            {
                return false;
            }

            if (DiagnosticBag is not null)
            {
                return ReportUseSiteDiagnostic(info, DiagnosticBag, location);
            }

            return info.Severity == DiagnosticSeverity.Error;
        }
    }

    public readonly struct ImmutableBindingDiagnostic<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        private readonly ImmutableArray<Diagnostic> _diagnostics;
        private readonly ImmutableArray<TAssemblySymbol> _dependencies;

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics.NullToEmpty();
        public ImmutableArray<TAssemblySymbol> Dependencies => _dependencies.NullToEmpty();

        public static ImmutableBindingDiagnostic<TAssemblySymbol> Empty => new(default, default);

        public ImmutableBindingDiagnostic(ImmutableArray<Diagnostic> diagnostics, ImmutableArray<TAssemblySymbol> dependencies)
        {
            _diagnostics = diagnostics.NullToEmpty();
            _dependencies = dependencies.NullToEmpty();
        }

        public ImmutableBindingDiagnostic<TAssemblySymbol> NullToEmpty() => new(Diagnostics, Dependencies);

        public static bool operator ==(ImmutableBindingDiagnostic<TAssemblySymbol> first, ImmutableBindingDiagnostic<TAssemblySymbol> second)
        {
            return first.Diagnostics == second.Diagnostics && first.Dependencies == second.Dependencies;
        }

        public static bool operator !=(ImmutableBindingDiagnostic<TAssemblySymbol> first, ImmutableBindingDiagnostic<TAssemblySymbol> second)
        {
            return !(first == second);
        }

        public override bool Equals(object? obj)
        {
            return (obj as ImmutableBindingDiagnostic<TAssemblySymbol>?)?.Equals(this) ?? false;
        }

        public bool Equals(ImmutableBindingDiagnostic<TAssemblySymbol> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Diagnostics.GetHashCode();
        }
    }
}
