namespace Microsoft.Cci
{
    public interface IGenericMethodParameter : IGenericParameter, IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry, IGenericMethodParameterReference
    {
        new IMethodDefinition DefiningMethod { get; }
    }
}
