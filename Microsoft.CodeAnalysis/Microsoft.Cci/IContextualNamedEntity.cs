namespace Microsoft.Cci
{
    public interface IContextualNamedEntity : INamedEntity
    {
        void AssociateWithMetadataWriter(MetadataWriter metadataWriter);
    }
}
