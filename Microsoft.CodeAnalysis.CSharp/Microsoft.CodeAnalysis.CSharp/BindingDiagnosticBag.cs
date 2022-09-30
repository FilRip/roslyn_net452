using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BindingDiagnosticBag : BindingDiagnosticBag<AssemblySymbol>
    {
        public static readonly BindingDiagnosticBag Discarded = new(null, null);

        public BindingDiagnosticBag()
            : this(usePool: false)
        {
        }

        private BindingDiagnosticBag(bool usePool)
            : base(usePool)
        {
        }

        public BindingDiagnosticBag(DiagnosticBag? diagnosticBag)
            : base(diagnosticBag, null)
        {
        }

        public BindingDiagnosticBag(DiagnosticBag? diagnosticBag, ICollection<AssemblySymbol>? dependenciesBag)
            : base(diagnosticBag, dependenciesBag)
        {
        }

        internal static BindingDiagnosticBag GetInstance()
        {
            return new BindingDiagnosticBag(usePool: true);
        }

        internal static BindingDiagnosticBag GetInstance(bool withDiagnostics, bool withDependencies)
        {
            if (withDiagnostics)
            {
                if (withDependencies)
                {
                    return GetInstance();
                }
                return new BindingDiagnosticBag(Microsoft.CodeAnalysis.DiagnosticBag.GetInstance());
            }
            if (withDependencies)
            {
                return new BindingDiagnosticBag(null, PooledHashSet<AssemblySymbol>.GetInstance());
            }
            return Discarded;
        }

        internal static BindingDiagnosticBag GetInstance(BindingDiagnosticBag template)
        {
            return GetInstance(template.AccumulatesDiagnostics, template.AccumulatesDependencies);
        }

        internal static BindingDiagnosticBag Create(BindingDiagnosticBag template)
        {
            if (template.AccumulatesDiagnostics)
            {
                if (template.AccumulatesDependencies)
                {
                    return new BindingDiagnosticBag();
                }
                return new BindingDiagnosticBag(new DiagnosticBag());
            }
            if (template.AccumulatesDependencies)
            {
                return new BindingDiagnosticBag(null, new HashSet<AssemblySymbol>());
            }
            return Discarded;
        }

        public void AddDependencies(Symbol? symbol)
        {
            if ((object)symbol != null && DependenciesBag != null)
            {
                AddDependencies(symbol!.GetUseSiteInfo());
            }
        }

        internal bool ReportUseSite(Symbol? symbol, SyntaxNode node)
        {
            return ReportUseSite(symbol, node.Location);
        }

        internal bool ReportUseSite(Symbol? symbol, SyntaxToken token)
        {
            return ReportUseSite(symbol, token.GetLocation());
        }

        internal bool ReportUseSite(Symbol? symbol, Location location)
        {
            if ((object)symbol != null)
            {
                return Add(symbol!.GetUseSiteInfo(), location);
            }
            return false;
        }

        internal void AddAssembliesUsedByNamespaceReference(NamespaceSymbol ns)
        {
            if (DependenciesBag != null)
            {
                addAssembliesUsedByNamespaceReferenceImpl(ns);
            }
            void addAssembliesUsedByNamespaceReferenceImpl(NamespaceSymbol ns)
            {
                if (ns.Extent.Kind == NamespaceKind.Compilation)
                {
                    ImmutableArray<NamespaceSymbol>.Enumerator enumerator = ns.ConstituentNamespaces.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        NamespaceSymbol current = enumerator.Current;
                        addAssembliesUsedByNamespaceReferenceImpl(current);
                    }
                }
                else
                {
                    AssemblySymbol containingAssembly = ns.ContainingAssembly;
                    if ((object)containingAssembly != null && !containingAssembly.IsMissing)
                    {
                        DependenciesBag!.Add(containingAssembly);
                    }
                }
            }
        }

        protected override bool ReportUseSiteDiagnostic(DiagnosticInfo diagnosticInfo, DiagnosticBag diagnosticBag, Location location)
        {
            return Symbol.ReportUseSiteDiagnostic(diagnosticInfo, diagnosticBag, location);
        }

        public CSDiagnosticInfo Add(ErrorCode code, Location location)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new(code);
            Add(cSDiagnosticInfo, location);
            return cSDiagnosticInfo;
        }

        public CSDiagnosticInfo Add(ErrorCode code, Location location, params object[] args)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new(code, args);
            Add(cSDiagnosticInfo, location);
            return cSDiagnosticInfo;
        }

        public CSDiagnosticInfo Add(ErrorCode code, Location location, ImmutableArray<Symbol> symbols, params object[] args)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new(code, args, symbols, ImmutableArray<Location>.Empty);
            Add(cSDiagnosticInfo, location);
            return cSDiagnosticInfo;
        }

        public void Add(DiagnosticInfo? info, Location location)
        {
            if (info != null)
            {
                DiagnosticBag?.Add(info, location);
            }
        }
    }
}
