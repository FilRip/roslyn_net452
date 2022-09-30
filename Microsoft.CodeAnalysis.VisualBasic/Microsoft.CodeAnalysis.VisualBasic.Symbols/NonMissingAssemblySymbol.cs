using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class NonMissingAssemblySymbol : AssemblySymbol
	{
		private readonly ConcurrentDictionary<MetadataTypeName.Key, NamedTypeSymbol> _emittedNameToTypeMap;

		private NamespaceSymbol _lazyGlobalNamespace;

		internal sealed override bool IsMissing => false;

		public sealed override NamespaceSymbol GlobalNamespace
		{
			get
			{
				if ((object)_lazyGlobalNamespace == null)
				{
					Interlocked.CompareExchange(ref _lazyGlobalNamespace, MergedNamespaceSymbol.CreateGlobalNamespace(this), null);
				}
				return _lazyGlobalNamespace;
			}
		}

		internal int EmittedNameToTypeMapCount => _emittedNameToTypeMap.Count;

		protected NonMissingAssemblySymbol()
		{
			_emittedNameToTypeMap = new ConcurrentDictionary<MetadataTypeName.Key, NamedTypeSymbol>();
		}

		internal sealed override NamedTypeSymbol LookupTopLevelMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool digThroughForwardedTypes)
		{
			NamedTypeSymbol namedTypeSymbol = null;
			namedTypeSymbol = LookupTopLevelMetadataTypeInCache(ref emittedName);
			if ((object)namedTypeSymbol != null)
			{
				if (digThroughForwardedTypes || (!TypeSymbolExtensions.IsErrorType(namedTypeSymbol) && (object)namedTypeSymbol.ContainingAssembly == this))
				{
					return namedTypeSymbol;
				}
				return new MissingMetadataTypeSymbol.TopLevel(Modules[0], ref emittedName);
			}
			ImmutableArray<ModuleSymbol> modules = Modules;
			int length = modules.Length;
			int i = 0;
			namedTypeSymbol = modules[i].LookupTopLevelMetadataType(ref emittedName);
			if (namedTypeSymbol is MissingMetadataTypeSymbol)
			{
				int num = length - 1;
				for (i = 1; i <= num; i++)
				{
					NamedTypeSymbol namedTypeSymbol2 = modules[i].LookupTopLevelMetadataType(ref emittedName);
					if (!(namedTypeSymbol2 is MissingMetadataTypeSymbol))
					{
						namedTypeSymbol = namedTypeSymbol2;
						break;
					}
				}
			}
			bool flag = i < length;
			if (!flag && digThroughForwardedTypes)
			{
				NamedTypeSymbol namedTypeSymbol3 = TryLookupForwardedMetadataTypeWithCycleDetection(ref emittedName, visitedAssemblies, ignoreCase: false);
				if ((object)namedTypeSymbol3 != null)
				{
					namedTypeSymbol = namedTypeSymbol3;
				}
			}
			if (digThroughForwardedTypes || flag)
			{
				CacheTopLevelMetadataType(ref emittedName, namedTypeSymbol);
			}
			return namedTypeSymbol;
		}

		internal abstract override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool ignoreCase);

		internal NamedTypeSymbol CachedTypeByEmittedName(string emittedname)
		{
			MetadataTypeName metadataTypeName = MetadataTypeName.FromFullName(emittedname);
			return _emittedNameToTypeMap[metadataTypeName.ToKey()];
		}

		private NamedTypeSymbol LookupTopLevelMetadataTypeInCache(ref MetadataTypeName emittedName)
		{
			NamedTypeSymbol value = null;
			if (_emittedNameToTypeMap.TryGetValue(emittedName.ToKey(), out value))
			{
				return value;
			}
			return null;
		}

		private void CacheTopLevelMetadataType(ref MetadataTypeName emittedName, NamedTypeSymbol result)
		{
			_emittedNameToTypeMap.GetOrAdd(emittedName.ToKey(), result);
		}
	}
}
