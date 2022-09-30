using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class GenericNestedTypeInstanceReference : GenericTypeInstanceReference, INestedTypeReference
	{
		public override IGenericTypeInstanceReference AsGenericTypeInstanceReference => this;

		public override INamespaceTypeReference AsNamespaceTypeReference => null;

		public override INestedTypeReference AsNestedTypeReference => this;

		public override ISpecializedNestedTypeReference AsSpecializedNestedTypeReference => null;

		public GenericNestedTypeInstanceReference(NamedTypeSymbol underlyingNamedType)
			: base(underlyingNamedType)
		{
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(m_UnderlyingNamedType.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}
	}
}
