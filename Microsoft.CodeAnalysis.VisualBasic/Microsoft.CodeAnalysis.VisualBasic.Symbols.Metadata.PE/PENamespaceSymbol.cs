using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal abstract class PENamespaceSymbol : PEOrSourceOrMergedNamespaceSymbol
	{
		protected Dictionary<string, ImmutableArray<Symbol>> m_lazyMembers;

		protected Dictionary<string, ImmutableArray<PENamedTypeSymbol>> m_lazyTypes;

		private Dictionary<string, TypeDefinitionHandle> _lazyNoPiaLocalTypes;

		private ImmutableArray<NamedTypeSymbol> _lazyModules;

		private ImmutableArray<NamedTypeSymbol> _lazyFlattenedTypes;

		internal sealed override NamespaceExtent Extent => new NamespaceExtent(ContainingPEModule);

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => EmbeddedSymbolKind.None;

		public sealed override ImmutableArray<Location> Locations => StaticCast<Location>.From(ContainingPEModule.MetadataLocation);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal abstract PEModuleSymbol ContainingPEModule { get; }

		internal bool AreTypesLoaded => m_lazyTypes != null;

		internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods
		{
			get
			{
				if (ContainingPEModule.MightContainExtensionMethods)
				{
					return GetTypeMembers();
				}
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers()
		{
			if (_lazyModules.IsDefault)
			{
				ImmutableArray<NamedTypeSymbol> value = GetTypeMembers().WhereAsArray((NamedTypeSymbol t) => t.TypeKind == TypeKind.Module);
				ImmutableInterlocked.InterlockedCompareExchange(ref _lazyModules, value, default(ImmutableArray<NamedTypeSymbol>));
			}
			return _lazyModules;
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers(string name)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol t) => t.TypeKind == TypeKind.Module);
		}

		public sealed override ImmutableArray<Symbol> GetMembers()
		{
			EnsureAllMembersLoaded();
			return m_lazyMembers.Flatten();
		}

		public sealed override ImmutableArray<Symbol> GetMembers(string name)
		{
			EnsureAllMembersLoaded();
			ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
			if (m_lazyMembers.TryGetValue(name, out value))
			{
				return value;
			}
			return ImmutableArray<Symbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			ImmutableArray<NamedTypeSymbol> lazyFlattenedTypes = _lazyFlattenedTypes;
			if (!lazyFlattenedTypes.IsDefault)
			{
				return lazyFlattenedTypes;
			}
			EnsureAllMembersLoaded();
			return _lazyFlattenedTypes = StaticCast<NamedTypeSymbol>.From(m_lazyTypes.Flatten());
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			EnsureAllMembersLoaded();
			ImmutableArray<PENamedTypeSymbol> value = default(ImmutableArray<PENamedTypeSymbol>);
			if (m_lazyTypes.TryGetValue(name, out value))
			{
				return StaticCast<NamedTypeSymbol>.From(value);
			}
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol type, int arity_) => type.Arity == arity_, arity);
		}

		protected abstract void EnsureAllMembersLoaded();

		protected void LoadAllMembers(IEnumerable<IGrouping<string, TypeDefinitionHandle>> typesByNS)
		{
			IEnumerable<IGrouping<string, TypeDefinitionHandle>> types = null;
			IEnumerable<KeyValuePair<string, IEnumerable<IGrouping<string, TypeDefinitionHandle>>>> namespaces = null;
			bool isGlobalNamespace = IsGlobalNamespace;
			MetadataHelpers.GetInfoForImmediateNamespaceMembers(isGlobalNamespace, (!isGlobalNamespace) ? ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat).Length : 0, typesByNS, CaseInsensitiveComparison.Comparer, out types, out namespaces);
			LazyInitializeTypes(types);
			LazyInitializeNamespaces(namespaces);
		}

		private void LazyInitializeNamespaces(IEnumerable<KeyValuePair<string, IEnumerable<IGrouping<string, TypeDefinitionHandle>>>> childNamespaces)
		{
			if (m_lazyMembers != null)
			{
				return;
			}
			Dictionary<string, ImmutableArray<Symbol>> dictionary = new Dictionary<string, ImmutableArray<Symbol>>(CaseInsensitiveComparison.Comparer);
			foreach (KeyValuePair<string, IEnumerable<IGrouping<string, TypeDefinitionHandle>>> childNamespace in childNamespaces)
			{
				PENestedNamespaceSymbol pENestedNamespaceSymbol = new PENestedNamespaceSymbol(childNamespace.Key, this, childNamespace.Value);
				dictionary.Add(pENestedNamespaceSymbol.Name, ImmutableArray.Create((Symbol)pENestedNamespaceSymbol));
			}
			foreach (ImmutableArray<PENamedTypeSymbol> value2 in m_lazyTypes.Values)
			{
				string name = value2[0].Name;
				ImmutableArray<Symbol> value = default(ImmutableArray<Symbol>);
				if (!dictionary.TryGetValue(name, out value))
				{
					dictionary.Add(name, StaticCast<Symbol>.From(value2));
				}
				else
				{
					dictionary[name] = value.Concat(StaticCast<Symbol>.From(value2));
				}
			}
			Interlocked.CompareExchange(ref m_lazyMembers, dictionary, null);
		}

		private void LazyInitializeTypes(IEnumerable<IGrouping<string, TypeDefinitionHandle>> typeGroups)
		{
			if (m_lazyTypes != null)
			{
				return;
			}
			PEModuleSymbol containingPEModule = ContainingPEModule;
			ArrayBuilder<PENamedTypeSymbol> instance = ArrayBuilder<PENamedTypeSymbol>.GetInstance();
			bool flag = !containingPEModule.Module.ContainsNoPiaLocalTypes();
			Dictionary<string, TypeDefinitionHandle> dictionary = null;
			bool isGlobalNamespace = IsGlobalNamespace;
			foreach (IGrouping<string, TypeDefinitionHandle> typeGroup in typeGroups)
			{
				foreach (TypeDefinitionHandle item2 in typeGroup)
				{
					if (flag || !containingPEModule.Module.IsNoPiaLocalType(item2))
					{
						PENamedTypeSymbol item = (isGlobalNamespace ? new PENamedTypeSymbol(containingPEModule, this, item2) : new PENamedTypeSymbolWithEmittedNamespaceName(containingPEModule, this, item2, typeGroup.Key));
						instance.Add(item);
						continue;
					}
					try
					{
						string typeDefNameOrThrow = containingPEModule.Module.GetTypeDefNameOrThrow(item2);
						if (dictionary == null)
						{
							dictionary = new Dictionary<string, TypeDefinitionHandle>();
						}
						string key = MetadataHelpers.BuildQualifiedName(typeGroup.Key, typeDefNameOrThrow);
						dictionary[key] = item2;
					}
					catch (BadImageFormatException ex)
					{
						ProjectData.SetProjectError(ex);
						BadImageFormatException ex2 = ex;
						ProjectData.ClearProjectError();
					}
				}
			}
			Dictionary<string, ImmutableArray<PENamedTypeSymbol>> dictionary2 = instance.ToDictionary((PENamedTypeSymbol c) => c.Name, CaseInsensitiveComparison.Comparer);
			instance.Free();
			if (_lazyNoPiaLocalTypes == null)
			{
				Interlocked.CompareExchange(ref _lazyNoPiaLocalTypes, dictionary, null);
			}
			if (Interlocked.CompareExchange(ref m_lazyTypes, dictionary2, null) == null)
			{
				containingPEModule.OnNewTypeDeclarationsLoaded(dictionary2);
			}
		}

		internal NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName, out bool isNoPiaLocalType)
		{
			NamedTypeSymbol namedTypeSymbol = LookupMetadataType(ref emittedTypeName);
			isNoPiaLocalType = false;
			if (namedTypeSymbol is MissingMetadataTypeSymbol)
			{
				EnsureAllMembersLoaded();
				TypeDefinitionHandle value = default(TypeDefinitionHandle);
				if (_lazyNoPiaLocalTypes != null && _lazyNoPiaLocalTypes.TryGetValue(emittedTypeName.FullName, out value))
				{
					namedTypeSymbol = (NamedTypeSymbol)new MetadataDecoder(ContainingPEModule).GetTypeOfToken(value, out isNoPiaLocalType);
				}
			}
			return namedTypeSymbol;
		}
	}
}
