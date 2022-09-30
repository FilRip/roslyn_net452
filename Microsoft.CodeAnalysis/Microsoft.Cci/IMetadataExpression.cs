namespace Microsoft.Cci
{
    public interface IMetadataExpression
    {
        ITypeReference Type { get; }

        void Dispatch(MetadataVisitor visitor);
    }
}
