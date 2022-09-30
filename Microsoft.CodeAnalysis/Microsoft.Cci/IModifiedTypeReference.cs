using System.Collections.Immutable;

namespace Microsoft.Cci
{
    public interface IModifiedTypeReference : ITypeReference, IReference
    {
        ImmutableArray<ICustomModifier> CustomModifiers { get; }

        ITypeReference UnmodifiedType { get; }
    }
}
