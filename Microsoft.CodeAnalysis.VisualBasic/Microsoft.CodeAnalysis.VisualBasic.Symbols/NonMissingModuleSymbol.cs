using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class NonMissingModuleSymbol : ModuleSymbol
	{
		private ModuleReferences<AssemblySymbol> _moduleReferences;

		internal sealed override bool IsMissing => false;

		internal override bool HasUnifiedReferences => GetUnifiedAssemblies().Length > 0;

		internal override bool MightContainExtensionMethods
		{
			get
			{
				AssemblySymbol containingAssembly = ContainingAssembly;
				if ((object)containingAssembly.Modules[0] == this)
				{
					return containingAssembly.MightContainExtensionMethods;
				}
				return false;
			}
		}

		internal sealed override ImmutableArray<AssemblyIdentity> GetReferencedAssemblies()
		{
			return _moduleReferences.Identities;
		}

		internal sealed override ImmutableArray<AssemblySymbol> GetReferencedAssemblySymbols()
		{
			return _moduleReferences.Symbols;
		}

		internal ImmutableArray<UnifiedAssembly<AssemblySymbol>> GetUnifiedAssemblies()
		{
			return _moduleReferences.UnifiedAssemblies;
		}

		internal override DiagnosticInfo GetUnificationUseSiteErrorInfo(TypeSymbol dependentType)
		{
			if (!HasUnifiedReferences)
			{
				return null;
			}
			AssemblySymbol containingAssembly = ContainingAssembly;
			AssemblySymbol containingAssembly2 = dependentType.ContainingAssembly;
			if ((object)containingAssembly == containingAssembly2)
			{
				return null;
			}
			ImmutableArray<UnifiedAssembly<AssemblySymbol>>.Enumerator enumerator = GetUnifiedAssemblies().GetEnumerator();
			while (enumerator.MoveNext())
			{
				UnifiedAssembly<AssemblySymbol> current = enumerator.Current;
				if ((object)current.TargetAssembly == containingAssembly2)
				{
					AssemblyIdentity originalReference = current.OriginalReference;
					AssemblyIdentity identity = containingAssembly2.Identity;
					if (identity.Version < originalReference.Version)
					{
						return ErrorFactory.ErrorInfo(ERRID.ERR_SxSIndirectRefHigherThanDirectRef3, identity.Name, originalReference.Version.ToString(), identity.Version.ToString());
					}
				}
			}
			return null;
		}

		internal override void SetReferences(ModuleReferences<AssemblySymbol> moduleReferences, SourceAssemblySymbol originatingSourceAssemblyDebugOnly = null)
		{
			_moduleReferences = moduleReferences;
		}

		[Conditional("DEBUG")]
		internal void AssertReferencesUninitialized()
		{
		}

		[Conditional("DEBUG")]
		internal void AssertReferencesInitialized()
		{
		}

		internal sealed override NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName)
		{
			NamedTypeSymbol namedTypeSymbol = null;
			NamespaceSymbol namespaceSymbol = GlobalNamespace.LookupNestedNamespace(emittedName.NamespaceSegments);
			if ((object)namespaceSymbol == null)
			{
				return new MissingMetadataTypeSymbol.TopLevel(this, ref emittedName);
			}
			return namespaceSymbol.LookupMetadataType(ref emittedName);
		}
	}
}
