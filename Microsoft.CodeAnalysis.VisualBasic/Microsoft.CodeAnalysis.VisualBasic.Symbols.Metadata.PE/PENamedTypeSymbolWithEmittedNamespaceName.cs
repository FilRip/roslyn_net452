using System.Reflection.Metadata;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PENamedTypeSymbolWithEmittedNamespaceName : PENamedTypeSymbol
	{
		private readonly string _emittedNamespaceName;

		private readonly SpecialType _corTypeId;

		public override SpecialType SpecialType => _corTypeId;

		internal PENamedTypeSymbolWithEmittedNamespaceName(PEModuleSymbol moduleSymbol, PENamespaceSymbol containingNamespace, TypeDefinitionHandle typeDef, string emittedNamespaceName)
			: base(moduleSymbol, containingNamespace, typeDef)
		{
			_emittedNamespaceName = emittedNamespaceName;
			if ((Arity == 0 || MangleName) && moduleSymbol.ContainingAssembly.KeepLookingForDeclaredSpecialTypes && DeclaredAccessibility == Accessibility.Public)
			{
				_corTypeId = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(emittedNamespaceName, MetadataName));
			}
			else
			{
				_corTypeId = SpecialType.None;
			}
		}

		internal override string GetEmittedNamespaceName()
		{
			return _emittedNamespaceName;
		}
	}
}
