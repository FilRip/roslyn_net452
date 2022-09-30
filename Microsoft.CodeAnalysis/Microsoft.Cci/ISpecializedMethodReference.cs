namespace Microsoft.Cci
{
    public interface ISpecializedMethodReference : IMethodReference, ISignature, ITypeMemberReference, IReference, INamedEntity
    {
        IMethodReference UnspecializedVersion { get; }
    }
}
