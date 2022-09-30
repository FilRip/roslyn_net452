using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MissingModuleSymbol : ModuleSymbol
	{
		protected readonly AssemblySymbol m_Assembly;

		protected readonly int m_Ordinal;

		protected readonly MissingNamespaceSymbol m_GlobalNamespace;

		internal override int Ordinal => m_Ordinal;

		internal override Machine Machine => Machine.I386;

		internal override bool Bit32Required => false;

		public override string Name => "<Missing Module>";

		public override AssemblySymbol ContainingAssembly => m_Assembly;

		public override Symbol ContainingSymbol => m_Assembly;

		public override NamespaceSymbol GlobalNamespace => m_GlobalNamespace;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		internal override ICollection<string> NamespaceNames => SpecializedCollections.EmptyCollection<string>();

		internal override ICollection<string> TypeNames => SpecializedCollections.EmptyCollection<string>();

		internal sealed override bool IsMissing => true;

		internal override bool HasUnifiedReferences => false;

		internal sealed override bool MightContainExtensionMethods => false;

		internal override bool HasAssemblyCompilationRelaxationsAttribute => false;

		internal override bool HasAssemblyRuntimeCompatibilityAttribute => false;

		internal override CharSet? DefaultMarshallingCharSet => null;

		public MissingModuleSymbol(AssemblySymbol assembly, int ordinal)
		{
			m_Assembly = assembly;
			m_Ordinal = ordinal;
			m_GlobalNamespace = new MissingNamespaceSymbol(this);
		}

		public override int GetHashCode()
		{
			return m_Assembly.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			return obj is MissingModuleSymbol missingModuleSymbol && m_Assembly.Equals(missingModuleSymbol.m_Assembly);
		}

		internal override NamedTypeSymbol LookupTopLevelMetadataType(ref MetadataTypeName emittedName)
		{
			return new MissingMetadataTypeSymbol.TopLevel(this, ref emittedName);
		}

		internal override ImmutableArray<AssemblyIdentity> GetReferencedAssemblies()
		{
			return ImmutableArray<AssemblyIdentity>.Empty;
		}

		internal override ImmutableArray<AssemblySymbol> GetReferencedAssemblySymbols()
		{
			return ImmutableArray<AssemblySymbol>.Empty;
		}

		internal override void SetReferences(ModuleReferences<AssemblySymbol> moduleReferences, SourceAssemblySymbol originatingSourceAssemblyDebugOnly = null)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override DiagnosticInfo GetUnificationUseSiteErrorInfo(TypeSymbol dependentType)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override ModuleMetadata GetMetadata()
		{
			return null;
		}
	}
}
