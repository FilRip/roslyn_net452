using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MissingAssemblySymbol : AssemblySymbol
	{
		protected readonly AssemblyIdentity m_Identity;

		protected readonly MissingModuleSymbol m_ModuleSymbol;

		private ImmutableArray<ModuleSymbol> _lazyModules;

		internal sealed override bool IsMissing => true;

		internal override bool IsLinked => false;

		public override AssemblyIdentity Identity => m_Identity;

		public override Version AssemblyVersionPattern => null;

		internal override ImmutableArray<byte> PublicKey => Identity.PublicKey;

		public override ImmutableArray<ModuleSymbol> Modules
		{
			get
			{
				if (_lazyModules.IsDefault)
				{
					_lazyModules = ImmutableArray.Create((ModuleSymbol)m_ModuleSymbol);
				}
				return _lazyModules;
			}
		}

		public sealed override NamespaceSymbol GlobalNamespace => m_ModuleSymbol.GlobalNamespace;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ICollection<string> TypeNames => SpecializedCollections.EmptyCollection<string>();

		public override ICollection<string> NamespaceNames => SpecializedCollections.EmptyCollection<string>();

		public sealed override bool MightContainExtensionMethods => false;

		public MissingAssemblySymbol(AssemblyIdentity identity)
		{
			m_Identity = identity;
			m_ModuleSymbol = new MissingModuleSymbol(this, 0);
		}

		internal override Symbol GetDeclaredSpecialTypeMember(SpecialMember member)
		{
			return null;
		}

		public override int GetHashCode()
		{
			return m_Identity.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MissingAssemblySymbol);
		}

		public bool Equals(MissingAssemblySymbol other)
		{
			if ((object)other != null)
			{
				if ((object)this != other)
				{
					return m_Identity.Equals(other.m_Identity);
				}
				return true;
			}
			return false;
		}

		internal override void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies()
		{
			return ImmutableArray<AssemblySymbol>.Empty;
		}

		internal override void SetNoPiaResolutionAssemblies(ImmutableArray<AssemblySymbol> assemblies)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<AssemblySymbol> GetNoPiaResolutionAssemblies()
		{
			return ImmutableArray<AssemblySymbol>.Empty;
		}

		internal override IEnumerable<ImmutableArray<byte>> GetInternalsVisibleToPublicKeys(string simpleName)
		{
			return SpecializedCollections.EmptyEnumerable<ImmutableArray<byte>>();
		}

		internal override bool AreInternalsVisibleToThisAssembly(AssemblySymbol other)
		{
			return false;
		}

		internal override NamedTypeSymbol LookupTopLevelMetadataTypeWithCycleDetection(ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies, bool digThroughForwardedTypes)
		{
			return m_ModuleSymbol.LookupTopLevelMetadataType(ref emittedName);
		}

		internal sealed override IEnumerable<NamedTypeSymbol> GetAllTopLevelForwardedTypes()
		{
			return SpecializedCollections.EmptyEnumerable<NamedTypeSymbol>();
		}

		internal override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override AssemblyMetadata GetMetadata()
		{
			return null;
		}
	}
}
