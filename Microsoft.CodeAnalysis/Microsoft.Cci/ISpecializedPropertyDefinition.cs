namespace Microsoft.Cci
{
    internal interface ISpecializedPropertyDefinition : IPropertyDefinition, ISignature, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        IPropertyDefinition UnspecializedVersion
        {
            [return: System.Diagnostics.CodeAnalysis.NotNull]
            get;
        }
    }
}
