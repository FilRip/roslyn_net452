namespace Microsoft.Cci
{
    public interface ITypeDefinitionMember : ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        ITypeDefinition ContainingTypeDefinition { get; }

        TypeMemberVisibility Visibility { get; }
    }
}
