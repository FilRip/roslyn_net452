namespace Microsoft.Cci
{
    public interface ISpecializedFieldReference : IFieldReference, ITypeMemberReference, IReference, INamedEntity
    {
        IFieldReference UnspecializedVersion { get; }
    }
}
