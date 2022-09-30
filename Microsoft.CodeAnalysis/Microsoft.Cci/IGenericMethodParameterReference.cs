namespace Microsoft.Cci
{
    public interface IGenericMethodParameterReference : IGenericParameterReference, ITypeReference, IReference, INamedEntity, IParameterListEntry
    {
        IMethodReference DefiningMethod { get; }
    }
}
