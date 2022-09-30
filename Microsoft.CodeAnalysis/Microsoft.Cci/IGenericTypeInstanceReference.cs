using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface IGenericTypeInstanceReference : ITypeReference, IReference
    {
        ImmutableArray<ITypeReference> GetGenericArguments(EmitContext context);

        INamedTypeReference GetGenericType(EmitContext context);
    }
}
