using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEGlobalNamespaceSymbol : PENamespaceSymbol
	{
		private readonly PEModuleSymbol _moduleSymbol;

		public override Symbol ContainingSymbol => _moduleSymbol;

		internal override PEModuleSymbol ContainingPEModule => _moduleSymbol;

		public override string Name => string.Empty;

		public override bool IsGlobalNamespace => true;

		public override AssemblySymbol ContainingAssembly => _moduleSymbol.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _moduleSymbol;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PEGlobalNamespaceSymbol(PEModuleSymbol moduleSymbol)
		{
			_moduleSymbol = moduleSymbol;
		}

		protected override void EnsureAllMembersLoaded()
		{
			if (m_lazyTypes == null || m_lazyMembers == null)
			{
				IEnumerable<IGrouping<string, TypeDefinitionHandle>> typesByNS;
				try
				{
					typesByNS = _moduleSymbol.Module.GroupTypesByNamespaceOrThrow(CaseInsensitiveComparison.Comparer);
				}
				catch (BadImageFormatException ex)
				{
					ProjectData.SetProjectError(ex);
					BadImageFormatException ex2 = ex;
					typesByNS = SpecializedCollections.EmptyEnumerable<IGrouping<string, TypeDefinitionHandle>>();
					ProjectData.ClearProjectError();
				}
				LoadAllMembers(typesByNS);
			}
		}
	}
}
