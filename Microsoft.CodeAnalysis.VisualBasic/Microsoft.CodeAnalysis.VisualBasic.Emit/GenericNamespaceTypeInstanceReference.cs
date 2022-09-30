using Microsoft.Cci;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class GenericNamespaceTypeInstanceReference : GenericTypeInstanceReference
	{
		public override IGenericTypeInstanceReference AsGenericTypeInstanceReference => this;

		public override INamespaceTypeReference AsNamespaceTypeReference => null;

		public override INestedTypeReference AsNestedTypeReference => null;

		public override ISpecializedNestedTypeReference AsSpecializedNestedTypeReference => null;

		public GenericNamespaceTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
			: base(underlyingNamedType)
		{
		}
	}
}
