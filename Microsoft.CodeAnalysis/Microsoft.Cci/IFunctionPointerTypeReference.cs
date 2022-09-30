namespace Microsoft.Cci
{
    public interface IFunctionPointerTypeReference : ITypeReference, IReference
    {
        ISignature Signature { get; }
    }
}
