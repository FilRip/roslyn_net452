namespace Microsoft.Cci
{
    internal interface ISpecializedEventDefinition : IEventDefinition, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        IEventDefinition UnspecializedVersion
        {
            [return: System.Diagnostics.CodeAnalysis.NotNull]
            get;
        }
    }
}
