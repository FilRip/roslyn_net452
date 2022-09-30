using System.Collections.Generic;

using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IGenericParameter : IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry
    {
        bool MustBeReferenceType { get; }

        bool MustBeValueType { get; }

        bool MustHaveDefaultConstructor { get; }

        TypeParameterVariance Variance { get; }

        IGenericMethodParameter? AsGenericMethodParameter { get; }

        IGenericTypeParameter? AsGenericTypeParameter { get; }

        IEnumerable<TypeReferenceWithAttributes> GetConstraints(EmitContext context);
    }
}
