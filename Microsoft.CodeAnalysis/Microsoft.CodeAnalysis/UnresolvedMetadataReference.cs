namespace Microsoft.CodeAnalysis
{
    public sealed class UnresolvedMetadataReference : MetadataReference
    {
        public string Reference { get; }

        public override string Display => CodeAnalysisResources.Unresolved + Reference;

        public override bool IsUnresolved => true;

        public UnresolvedMetadataReference(string reference, MetadataReferenceProperties properties)
            : base(properties)
        {
            Reference = reference;
        }

        internal override MetadataReference WithPropertiesImplReturningMetadataReference(MetadataReferenceProperties properties)
        {
            return new UnresolvedMetadataReference(Reference, properties);
        }
    }
}
