using System.Collections.Generic;

using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.Cci
{
    public interface IGenericMethodInstanceReference : IMethodReference, ISignature, ITypeMemberReference, IReference, INamedEntity
    {
        IEnumerable<ITypeReference> GetGenericArguments(EmitContext context);

        IMethodReference GetGenericMethod(EmitContext context);
    }
}
