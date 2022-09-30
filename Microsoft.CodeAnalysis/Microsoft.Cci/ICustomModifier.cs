using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface ICustomModifier
    {
        bool IsOptional { get; }

        ITypeReference GetModifier(EmitContext context);
    }
}
