namespace Microsoft.Cci
{
    public interface IGlobalMethodDefinition : IMethodDefinition, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition, IMethodReference, ISignature
    {
        new string Name { get; }
    }
}
