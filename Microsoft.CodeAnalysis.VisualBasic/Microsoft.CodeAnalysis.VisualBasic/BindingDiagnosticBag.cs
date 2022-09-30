using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BindingDiagnosticBag : BindingDiagnosticBag<AssemblySymbol>
	{
		public static readonly BindingDiagnosticBag Discarded = new BindingDiagnosticBag(null, null);

		internal bool IsEmpty
		{
			get
			{
				if (DiagnosticBag == null || DiagnosticBag!.IsEmptyWithoutResolution)
				{
					return DependenciesBag.IsNullOrEmpty();
				}
				return false;
			}
		}

		public BindingDiagnosticBag()
			: base(usePool: false)
		{
		}

		private BindingDiagnosticBag(bool usePool)
			: base(usePool)
		{
		}

		public BindingDiagnosticBag(DiagnosticBag diagnosticBag)
			: base(diagnosticBag, (ICollection<AssemblySymbol>?)null)
		{
		}

		public BindingDiagnosticBag(DiagnosticBag diagnosticBag, ICollection<AssemblySymbol> dependenciesBag)
			: base(diagnosticBag, dependenciesBag)
		{
		}

		internal static BindingDiagnosticBag GetInstance()
		{
			return new BindingDiagnosticBag(usePool: true);
		}

		internal static BindingDiagnosticBag GetInstance(bool withDiagnostics, bool withDependencies)
		{
			if (withDependencies)
			{
				if (withDiagnostics)
				{
					return GetInstance();
				}
				return new BindingDiagnosticBag(null, PooledHashSet<AssemblySymbol>.GetInstance());
			}
			if (withDiagnostics)
			{
				return new BindingDiagnosticBag(Microsoft.CodeAnalysis.DiagnosticBag.GetInstance());
			}
			return Discarded;
		}

		internal static BindingDiagnosticBag GetInstance(BindingDiagnosticBag template)
		{
			return GetInstance(template.AccumulatesDiagnostics, template.AccumulatesDependencies);
		}

		internal static BindingDiagnosticBag Create(bool withDiagnostics, bool withDependencies)
		{
			if (withDependencies)
			{
				if (withDiagnostics)
				{
					return new BindingDiagnosticBag();
				}
				return new BindingDiagnosticBag(null, new HashSet<AssemblySymbol>());
			}
			if (withDiagnostics)
			{
				return new BindingDiagnosticBag(new DiagnosticBag());
			}
			return Discarded;
		}

		internal static BindingDiagnosticBag Create(BindingDiagnosticBag template)
		{
			return Create(template.AccumulatesDiagnostics, template.AccumulatesDependencies);
		}

		protected override bool ReportUseSiteDiagnostic(DiagnosticInfo diagnosticInfo, DiagnosticBag diagnosticBag, Location location)
		{
			diagnosticBag.Add(new VBDiagnostic(diagnosticInfo, location));
			return true;
		}

		internal bool Add(BoundNode node, CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return Add(node.Syntax.Location, useSiteInfo);
		}

		internal bool Add(SyntaxNodeOrToken syntax, CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return Add(syntax.GetLocation(), useSiteInfo);
		}

		internal bool ReportUseSite(Symbol symbol, SyntaxNode node)
		{
			return ReportUseSite(symbol, node.Location);
		}

		internal bool ReportUseSite(Symbol symbol, SyntaxToken token)
		{
			return ReportUseSite(symbol, token.GetLocation());
		}

		internal bool ReportUseSite(Symbol symbol, Location location)
		{
			if ((object)symbol != null)
			{
				return Add(symbol.GetUseSiteInfo(), location);
			}
			return false;
		}

		internal void AddAssembliesUsedByCrefTarget(Symbol symbol)
		{
			if (DependenciesBag != null)
			{
				if (symbol is NamespaceSymbol ns)
				{
					AddAssembliesUsedByNamespaceReference(ns);
				}
				else
				{
					AddDependencies((symbol as TypeSymbol) ?? symbol.ContainingType);
				}
			}
		}

		internal void AddDependencies(Symbol symbol)
		{
			if (DependenciesBag != null && (object)symbol != null)
			{
				AddDependencies(symbol.GetUseSiteInfo());
			}
		}

		internal void AddAssembliesUsedByNamespaceReference(NamespaceSymbol ns)
		{
			if (DependenciesBag != null)
			{
				AddAssembliesUsedByNamespaceReferenceImpl(ns);
			}
		}

		private void AddAssembliesUsedByNamespaceReferenceImpl(NamespaceSymbol ns)
		{
			if (ns.Extent.Kind == NamespaceKind.Compilation)
			{
				ImmutableArray<NamespaceSymbol>.Enumerator enumerator = ns.ConstituentNamespaces.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamespaceSymbol current = enumerator.Current;
					AddAssembliesUsedByNamespaceReferenceImpl(current);
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

		internal DiagnosticInfo Add(ERRID code, Location location)
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(code);
			Add(diagnosticInfo, location);
			return diagnosticInfo;
		}

		internal DiagnosticInfo Add(ERRID code, Location location, params object[] args)
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(code, args);
			Add(diagnosticInfo, location);
			return diagnosticInfo;
		}

		internal void Add(DiagnosticInfo info, Location location)
		{
			DiagnosticBag?.Add(new VBDiagnostic(info, location));
		}
	}
}
