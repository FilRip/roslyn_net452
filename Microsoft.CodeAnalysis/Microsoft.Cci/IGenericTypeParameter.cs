namespace Microsoft.Cci
{
    public interface IGenericTypeParameter : IGenericParameter, IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry, IGenericTypeParameterReference
    {
        new ITypeDefinition DefiningType { get; }
    }
}
