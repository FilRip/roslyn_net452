using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEAssemblySymbol : MetadataOrSourceAssemblySymbol
	{
		private readonly PEAssembly _assembly;

		private readonly DocumentationProvider _documentationProvider;

		private readonly ImmutableArray<ModuleSymbol> _modules;

		private ImmutableArray<AssemblySymbol> _noPiaResolutionAssemblies;

		private ImmutableArray<AssemblySymbol> _linkedReferencedAssemblies;

		private readonly bool _isLinked;

		private byte _lazyMightContainExtensionMethods;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		internal PEAssembly Assembly => _assembly;

		public override AssemblyIdentity Identity => _assembly.Identity;

		public override Version AssemblyVersionPattern => null;

		internal override ImmutableArray<byte> PublicKey => _assembly.Identity.PublicKey;

		public override ImmutableArray<Location> Locations => StaticCast<Location>.From(PrimaryModule.MetadataLocation);

		public override ImmutableArray<ModuleSymbol> Modules => _modules;

		internal PEModuleSymbol PrimaryModule => (PEModuleSymbol)Modules[0];

		internal override bool IsLinked => _isLinked;

		internal DocumentationProvider DocumentationProvider => _documentationProvider;

		public override bool MightContainExtensionMethods
		{
			get
			{
				if (_lazyMightContainExtensionMethods == 0)
				{
					if (PrimaryModule.Module.HasExtensionAttribute(_assembly.Handle, ignoreCase: true))
					{
						_lazyMightContainExtensionMethods = 2;
					}
					else
					{
						_lazyMightContainExtensionMethods = 1;
					}
				}
				return _lazyMightContainExtensionMethods == 2;
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PEAssemblySymbol(PEAssembly assembly, DocumentationProvider documentationProvider, bool isLinked, MetadataImportOptions importOptions)
		{
			_lazyMightContainExtensionMethods = 0;
			_assembly = assembly;
			_documentationProvider = documentationProvider;
			ModuleSymbol[] array = new ModuleSymbol[assembly.Modules.Length - 1 + 1];
			int num = assembly.Modules.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = new PEModuleSymbol(this, assembly.Modules[i], importOptions, i);
			}
			_modules = array.AsImmutableOrNull();
			_isLinked = isLinked;
		}

		internal override bool GetGuidString(ref string guidString)
		{
			return Assembly.Modules[0].HasGuidAttribute(Assembly.Handle, out guidString);
		}

		internal override bool AreInternalsVisibleToThisAssembly(AssemblySymbol potentialGiverOfAccess)
		{
			return MakeFinalIVTDetermination(potentialGiverOfAccess) == IVTConclusion.Match;
		}

		internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
		{
			return Assembly.GetInternalsVisibleToPublicKeys(simpleName);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				PrimaryModule.LoadCustomAttributes(Assembly.Handle, ref _lazyCustomAttributes);
			}
			return _lazyCustomAttributes;
		}

		internal (AssemblySymbol FirstSymbol, AssemblySymbol SecondSymbol) LookupAssembliesForForwardedMetadataType(ref MetadataTypeName emittedName, bool ignoreCase, out string matchedName)
		{
			return PrimaryModule.GetAssembliesForForwardedType(ref emittedName, ignoreCase, out matchedName);
		}

		internal override IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes()
		{
			return PrimaryModule.GetForwardedTypes();
		}

		internal override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool ignoreCase)
		{
			string matchedName = null;
			(AssemblySymbol, AssemblySymbol) tuple = LookupAssembliesForForwardedMetadataType(ref emittedName, ignoreCase, out matchedName);
			if ((object)tuple.Item1 != null)
			{
				if ((object)tuple.Item2 != null)
				{
					return CreateMultipleForwardingErrorTypeSymbol(ref emittedName, PrimaryModule, tuple.Item1, tuple.Item2);
				}
				if (visitedAssemblies != null && visitedAssemblies.Contains(tuple.Item1))
				{
					return CreateCycleInTypeForwarderErrorTypeSymbol(ref emittedName);
				}
				visitedAssemblies = new ConsList<AssemblySymbol>(this, visitedAssemblies ?? ConsList<AssemblySymbol>.Empty);
				if (ignoreCase && !string.Equals(emittedName.FullName, matchedName, StringComparison.Ordinal))
				{
					emittedName = MetadataTypeName.FromFullName(matchedName, emittedName.UseCLSCompliantNameArityEncoding, emittedName.ForcedArity);
				}
				return tuple.Item1.LookupTopLevelMetadataTypeWithCycleDetection(ref emittedName, visitedAssemblies, digThroughForwardedTypes: true);
			}
			return null;
		}

		internal override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies()
		{
			return _noPiaResolutionAssemblies;
		}

		internal override void SetNoPiaResolutionAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			_noPiaResolutionAssemblies = assemblies;
		}

		internal override void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			_linkedReferencedAssemblies = assemblies;
		}

		internal override ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies()
		{
			return _linkedReferencedAssemblies;
		}

		public override AssemblyMetadata GetMetadata()
		{
			return _assembly.GetNonDisposableMetadata();
		}
	}
}
