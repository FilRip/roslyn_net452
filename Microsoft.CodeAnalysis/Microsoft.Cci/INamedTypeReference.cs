namespace Microsoft.Cci
{
    public interface INamedTypeReference : ITypeReference, IReference, INamedEntity
    {
        ushort GenericParameterCount { get; }

        bool MangleName { get; }
    }
}
