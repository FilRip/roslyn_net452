using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PENestedNamespaceSymbol : PENamespaceSymbol
	{
		internal readonly PENamespaceSymbol m_ContainingNamespaceSymbol;

		protected readonly string m_Name;

		private IEnumerable<IGrouping<string, TypeDefinitionHandle>> _typesByNS;

		public override Symbol ContainingSymbol => m_ContainingNamespaceSymbol;

		internal override PEModuleSymbol ContainingPEModule => m_ContainingNamespaceSymbol.ContainingPEModule;

		public override string Name => m_Name;

		public override bool IsGlobalNamespace => false;

		public override AssemblySymbol ContainingAssembly => ContainingPEModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => m_ContainingNamespaceSymbol.ContainingPEModule;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PENestedNamespaceSymbol(string name, PENamespaceSymbol containingNamespace, IEnumerable<IGrouping<string, TypeDefinitionHandle>> typesByNS)
		{
			m_ContainingNamespaceSymbol = containingNamespace;
			m_Name = name;
			_typesByNS = typesByNS;
		}

		protected override void EnsureAllMembersLoaded()
		{
			IEnumerable<IGrouping<string, TypeDefinitionHandle>> typesByNS = _typesByNS;
			if (m_lazyTypes == null || m_lazyMembers == null)
			{
				LoadAllMembers(typesByNS);
				Interlocked.Exchange(ref _typesByNS, null);
			}
		}

		protected override Accessibility GetDeclaredAccessibilityOfMostAccessibleDescendantType()
		{
			IEnumerable<IGrouping<string, TypeDefinitionHandle>> typesByNS = _typesByNS;
			if (typesByNS != null && m_lazyTypes == null)
			{
				PEModule module = ContainingPEModule.Module;
				Accessibility result = Accessibility.NotApplicable;
				TypeAttributes typeDefFlagsOrThrow = default(TypeAttributes);
				foreach (IGrouping<string, TypeDefinitionHandle> item in typesByNS)
				{
					foreach (TypeDefinitionHandle item2 in item)
					{
						try
						{
							typeDefFlagsOrThrow = module.GetTypeDefFlagsOrThrow(item2);
						}
						catch (BadImageFormatException ex)
						{
							ProjectData.SetProjectError(ex);
							BadImageFormatException ex2 = ex;
							ProjectData.ClearProjectError();
						}
						switch (typeDefFlagsOrThrow & TypeAttributes.VisibilityMask)
						{
						case TypeAttributes.Public:
							return Accessibility.Public;
						case TypeAttributes.NotPublic:
							result = Accessibility.Internal;
							break;
						}
					}
				}
				return result;
			}
			return base.GetDeclaredAccessibilityOfMostAccessibleDescendantType();
		}
	}
}
