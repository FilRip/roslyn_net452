using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct CompoundUseSiteInfo<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        private enum DiscardLevel : byte
        {
            None,
            Dependencies,
            DiagnosticsAndDependencies
        }

        private bool _hasErrors;

        private readonly DiscardLevel _discardLevel;

        private HashSet<DiagnosticInfo>? _diagnostics;

        private HashSet<TAssemblySymbol>? _dependencies;

        private readonly TAssemblySymbol? _assemblyBeingBuilt;

        public static CompoundUseSiteInfo<TAssemblySymbol> Discarded => new CompoundUseSiteInfo<TAssemblySymbol>(DiscardLevel.DiagnosticsAndDependencies);

        public static CompoundUseSiteInfo<TAssemblySymbol> DiscardedDependencies => new CompoundUseSiteInfo<TAssemblySymbol>(DiscardLevel.Dependencies);

        public TAssemblySymbol? AssemblyBeingBuilt => _assemblyBeingBuilt;

        private DiscardLevel DiscardLevelWithValidation => _discardLevel;

        public bool AccumulatesDiagnostics => DiscardLevelWithValidation != DiscardLevel.DiagnosticsAndDependencies;

        // FilRip return null or cast to ReadOnlyCollection
        public IReadOnlyCollection<DiagnosticInfo>? Diagnostics
        {
            get
            {
                if (_diagnostics == null)
                    return null;
                return _diagnostics.ToReadOnlyCollection();
            }
        }

        public bool AccumulatesDependencies => DiscardLevelWithValidation == DiscardLevel.None;

        // FilRip return null or cast to ReadOnlyCollection
        public IReadOnlyCollection<TAssemblySymbol>? Dependencies
        {
            get
            {
                if (_dependencies == null)
                    return null;
                return _dependencies.ToReadOnlyCollection();
            }
        }

        public bool HasErrors => _hasErrors;

        public CompoundUseSiteInfo(TAssemblySymbol assemblyBeingBuilt)
        {
            this = default(CompoundUseSiteInfo<TAssemblySymbol>);
            _assemblyBeingBuilt = assemblyBeingBuilt;
        }

        public CompoundUseSiteInfo(BindingDiagnosticBag<TAssemblySymbol>? futureDestination, TAssemblySymbol assemblyBeingBuilt)
        {
            this = default(CompoundUseSiteInfo<TAssemblySymbol>);
            if (futureDestination == null)
            {
                _discardLevel = DiscardLevel.DiagnosticsAndDependencies;
                return;
            }
            if (!futureDestination!.AccumulatesDependencies)
            {
                _discardLevel = DiscardLevel.Dependencies;
                return;
            }
            _discardLevel = DiscardLevel.None;
            _assemblyBeingBuilt = assemblyBeingBuilt;
        }

        public CompoundUseSiteInfo(CompoundUseSiteInfo<TAssemblySymbol> template)
        {
            this = default(CompoundUseSiteInfo<TAssemblySymbol>);
            _discardLevel = template._discardLevel;
            _assemblyBeingBuilt = template._assemblyBeingBuilt;
        }

        private CompoundUseSiteInfo(DiscardLevel discardLevel)
        {
            this = default(CompoundUseSiteInfo<TAssemblySymbol>);
            _discardLevel = discardLevel;
        }

        [Conditional("DEBUG")]
        private readonly void AssertInternalConsistency()
        {
        }

        public void AddDiagnostics(UseSiteInfo<TAssemblySymbol> info)
        {
            if (AccumulatesDiagnostics && HashSetExtensions.InitializeAndAdd(ref _diagnostics, info.DiagnosticInfo))
            {
                DiagnosticInfo? diagnosticInfo = info.DiagnosticInfo;
                if (diagnosticInfo != null && diagnosticInfo!.Severity == DiagnosticSeverity.Error)
                {
                    RecordPresenceOfAnError();
                }
            }
        }

        private void RecordPresenceOfAnError()
        {
            if (!_hasErrors)
            {
                _hasErrors = true;
                _dependencies = null;
            }
        }

        public void AddDiagnostics(ICollection<DiagnosticInfo>? diagnostics)
        {
            if (!AccumulatesDiagnostics || diagnostics.IsNullOrEmpty())
            {
                return;
            }
            if (_diagnostics == null)
            {
                _diagnostics = new HashSet<DiagnosticInfo>();
            }
            foreach (DiagnosticInfo item in diagnostics!)
            {
                if (_diagnostics!.Add(item) && item != null && item.Severity == DiagnosticSeverity.Error)
                {
                    RecordPresenceOfAnError();
                }
            }
        }

        public void AddDiagnostics(IReadOnlyCollection<DiagnosticInfo>? diagnostics)
        {
            if (!AccumulatesDiagnostics || diagnostics.IsNullOrEmpty())
            {
                return;
            }
            if (_diagnostics == null)
            {
                _diagnostics = new HashSet<DiagnosticInfo>();
            }
            foreach (DiagnosticInfo item in diagnostics!)
            {
                if (_diagnostics!.Add(item) && item != null && item.Severity == DiagnosticSeverity.Error)
                {
                    RecordPresenceOfAnError();
                }
            }
        }

        public void AddDiagnostics(ImmutableArray<DiagnosticInfo> diagnostics)
        {
            if (!AccumulatesDiagnostics || diagnostics.IsDefaultOrEmpty)
            {
                return;
            }
            if (_diagnostics == null)
            {
                _diagnostics = new HashSet<DiagnosticInfo>();
            }
            ImmutableArray<DiagnosticInfo>.Enumerator enumerator = diagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DiagnosticInfo current = enumerator.Current;
                if (_diagnostics!.Add(current) && current != null && current.Severity == DiagnosticSeverity.Error)
                {
                    RecordPresenceOfAnError();
                }
            }
        }

        public void AddDependencies(UseSiteInfo<TAssemblySymbol> info)
        {
            if (!_hasErrors && AccumulatesDependencies)
            {
                if (info.PrimaryDependency != _assemblyBeingBuilt)
                {
                    HashSetExtensions.InitializeAndAdd(ref _dependencies, info.PrimaryDependency);
                }
                ImmutableHashSet<TAssemblySymbol>? secondaryDependencies = info.SecondaryDependencies;
                if (secondaryDependencies != null && !secondaryDependencies!.IsEmpty && (_assemblyBeingBuilt == null || info.SecondaryDependencies.AsSingleton() != _assemblyBeingBuilt))
                {
                    (_dependencies ?? (_dependencies = new HashSet<TAssemblySymbol>())).AddAll(info.SecondaryDependencies);
                }
            }
        }

        public void AddDependencies(CompoundUseSiteInfo<TAssemblySymbol> info)
        {
            if (!_hasErrors && AccumulatesDependencies)
            {
                AddDependencies(info.Dependencies);
            }
        }

        public void AddDependencies(ICollection<TAssemblySymbol>? dependencies)
        {
            if (!_hasErrors && AccumulatesDependencies && !dependencies.IsNullOrEmpty() && (_assemblyBeingBuilt == null || dependencies.AsSingleton() != _assemblyBeingBuilt))
            {
                (_dependencies ?? (_dependencies = new HashSet<TAssemblySymbol>())).AddAll(dependencies);
            }
        }

        public void AddDependencies(IReadOnlyCollection<TAssemblySymbol>? dependencies)
        {
            if (!_hasErrors && AccumulatesDependencies && !dependencies.IsNullOrEmpty() && (_assemblyBeingBuilt == null || dependencies.AsSingleton() != _assemblyBeingBuilt))
            {
                (_dependencies ?? (_dependencies = new HashSet<TAssemblySymbol>())).AddAll(dependencies);
            }
        }

        public void AddDependencies(ImmutableArray<TAssemblySymbol> dependencies)
        {
            if (!_hasErrors && AccumulatesDependencies && !dependencies.IsDefaultOrEmpty && (_assemblyBeingBuilt == null || dependencies.Length != 1 || dependencies[0] != _assemblyBeingBuilt))
            {
                (_dependencies ?? (_dependencies = new HashSet<TAssemblySymbol>())).AddAll(dependencies);
            }
        }

        public void MergeAndClear(ref CompoundUseSiteInfo<TAssemblySymbol> other)
        {
            if (!AccumulatesDiagnostics)
            {
                other._diagnostics = null;
                other._dependencies = null;
                other._hasErrors = false;
                return;
            }
            mergeAndClear<DiagnosticInfo>(ref _diagnostics, ref other._diagnostics);
            if (other._hasErrors)
            {
                RecordPresenceOfAnError();
                other._hasErrors = false;
            }
            if (!_hasErrors && AccumulatesDependencies)
            {
                mergeAndClear<TAssemblySymbol>(ref _dependencies, ref other._dependencies);
            }
            else
            {
                other._dependencies = null;
            }
            static void mergeAndClear<T>(ref HashSet<T>? self, ref HashSet<T>? other)
            {
                if (self == null)
                {
                    self = other;
                }
                else if (other != null)
                {
                    self.AddAll(other);
                }
                other = null;
            }
        }

        public void Add(UseSiteInfo<TAssemblySymbol> other)
        {
            if (AccumulatesDiagnostics)
            {
                AddDiagnostics(other);
                AddDependencies(other);
            }
        }
    }
}
