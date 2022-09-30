using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface ISpecializedNestedTypeReference : INestedTypeReference, INamedTypeReference, ITypeReference, IReference, INamedEntity, ITypeMemberReference
    {
        [return: System.Diagnostics.CodeAnalysis.NotNull]
        INestedTypeReference GetUnspecializedVersion(EmitContext context);
    }
}
