using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface ITypeMemberReference : IReference, INamedEntity
    {
        ITypeReference GetContainingType(EmitContext context);
    }
}
