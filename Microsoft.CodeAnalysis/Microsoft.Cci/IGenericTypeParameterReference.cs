namespace Microsoft.Cci
{
    public interface IGenericTypeParameterReference : IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry
    {
        ITypeReference DefiningType { get; }
    }
}
