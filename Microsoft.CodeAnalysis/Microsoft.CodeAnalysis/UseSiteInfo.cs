using System.Collections.Immutable;

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

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct UseSiteInfo<TAssemblySymbol> where TAssemblySymbol : class, IAssemblySymbolInternal
    {
        public readonly DiagnosticInfo? DiagnosticInfo;

        public readonly TAssemblySymbol? PrimaryDependency;

        public readonly ImmutableHashSet<TAssemblySymbol>? SecondaryDependencies;

        public bool IsEmpty
        {
            get
            {
                if (DiagnosticInfo == null && PrimaryDependency == null)
                {
                    return SecondaryDependencies?.IsEmpty ?? true;
                }
                return false;
            }
        }

        public UseSiteInfo(TAssemblySymbol? primaryDependency)
            : this(null, primaryDependency, null)
        {
        }

        public UseSiteInfo(ImmutableHashSet<TAssemblySymbol>? secondaryDependencies)
            : this(null, null, secondaryDependencies)
        {
        }

        public UseSiteInfo(DiagnosticInfo? diagnosticInfo)
            : this(diagnosticInfo, null, null)
        {
        }

        public UseSiteInfo(DiagnosticInfo? diagnosticInfo, TAssemblySymbol? primaryDependency)
            : this(diagnosticInfo, primaryDependency, null)
        {
        }

        public UseSiteInfo(DiagnosticInfo? diagnosticInfo, TAssemblySymbol? primaryDependency, ImmutableHashSet<TAssemblySymbol>? secondaryDependencies)
        {
            DiagnosticInfo = diagnosticInfo;
            PrimaryDependency = primaryDependency;
            SecondaryDependencies = secondaryDependencies ?? ImmutableHashSet<TAssemblySymbol>.Empty;
        }

        public UseSiteInfo<TAssemblySymbol> AdjustDiagnosticInfo(DiagnosticInfo? diagnosticInfo)
        {
            if (DiagnosticInfo != diagnosticInfo)
            {
                if (diagnosticInfo != null && diagnosticInfo!.Severity == DiagnosticSeverity.Error)
                {
                    return new UseSiteInfo<TAssemblySymbol>(diagnosticInfo);
                }
                return new UseSiteInfo<TAssemblySymbol>(diagnosticInfo, PrimaryDependency, SecondaryDependencies);
            }
            return this;
        }

        public void MergeDependencies(ref TAssemblySymbol? primaryDependency, ref ImmutableHashSet<TAssemblySymbol>? secondaryDependencies)
        {
            secondaryDependencies = (secondaryDependencies ?? ImmutableHashSet<TAssemblySymbol>.Empty)!.Union(SecondaryDependencies ?? ImmutableHashSet<TAssemblySymbol>.Empty);
            if (primaryDependency == null)
            {
                primaryDependency = PrimaryDependency;
            }
            if (!object.Equals(primaryDependency, PrimaryDependency) && PrimaryDependency != null)
            {
                secondaryDependencies = secondaryDependencies!.Add(PrimaryDependency);
            }
        }
    }
}
