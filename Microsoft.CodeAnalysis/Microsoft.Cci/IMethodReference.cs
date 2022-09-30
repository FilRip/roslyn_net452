using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Emit;

#nullable enable

namespace Microsoft.Cci
{
    public interface IMethodReference : ISignature, ITypeMemberReference, IReference, INamedEntity
    {
        bool AcceptsExtraArguments { get; }

        ushort GenericParameterCount { get; }

        bool IsGeneric { get; }

        ImmutableArray<IParameterTypeInformation> ExtraParameters { get; }

        IGenericMethodInstanceReference? AsGenericMethodInstanceReference { get; }

        ISpecializedMethodReference? AsSpecializedMethodReference { get; }

        IMethodDefinition? GetResolvedMethod(EmitContext context);
    }
}
