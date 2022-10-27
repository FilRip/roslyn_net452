#nullable enable

namespace Microsoft.CodeAnalysis.CompilerServer
{
    internal sealed class CachingMetadataReference : PortableExecutableReference
    {
        private static readonly MetadataAndSymbolCache s_mdCache = new();

        public new string FilePath { get; }

        public CachingMetadataReference(string fullPath, MetadataReferenceProperties properties)
            : base(properties, fullPath)
        {
            this.FilePath = fullPath;
        }

        protected override DocumentationProvider CreateDocumentationProvider() => DocumentationProvider.Default;

        protected override Metadata GetMetadataImpl() => s_mdCache.GetMetadata(this.FilePath, this.Properties);

        protected override PortableExecutableReference WithPropertiesImpl(
            MetadataReferenceProperties properties)
        {
            return new CachingMetadataReference(this.FilePath, properties);
        }
    }
}
