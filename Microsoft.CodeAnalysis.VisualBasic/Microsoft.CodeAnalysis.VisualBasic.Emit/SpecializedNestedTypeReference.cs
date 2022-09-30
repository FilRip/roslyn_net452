using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal class SpecializedNestedTypeReference : NamedTypeReference, ISpecializedNestedTypeReference
	{
		public override IGenericTypeInstanceReference AsGenericTypeInstanceReference => null;

		public override INamespaceTypeReference AsNamespaceTypeReference => null;

		public override INestedTypeReference AsNestedTypeReference => this;

		public override ISpecializedNestedTypeReference AsSpecializedNestedTypeReference => this;

		public SpecializedNestedTypeReference(NamedTypeSymbol underlyingNamedType)
			: base(underlyingNamedType)
		{
		}

		private INestedTypeReference ISpecializedNestedTypeReferenceGetUnspecializedVersion(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(m_UnderlyingNamedType.OriginalDefinition, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true).AsNestedTypeReference;
		}

		INestedTypeReference ISpecializedNestedTypeReference.GetUnspecializedVersion(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISpecializedNestedTypeReferenceGetUnspecializedVersion
			return this.ISpecializedNestedTypeReferenceGetUnspecializedVersion(context);
		}

		public override void Dispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
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
