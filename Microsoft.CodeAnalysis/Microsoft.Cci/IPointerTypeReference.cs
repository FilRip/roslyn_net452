using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface IPointerTypeReference : ITypeReference, IReference
    {
        ITypeReference GetTargetType(EmitContext context);
    }
}
