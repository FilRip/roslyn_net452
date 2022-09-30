using System.Reflection;

namespace Microsoft.CodeAnalysis
{
    public struct EmbeddedResource
    {
        public readonly uint Offset;

        public readonly ManifestResourceAttributes Attributes;

        public readonly string Name;

        public EmbeddedResource(uint offset, ManifestResourceAttributes attributes, string name)
        {
            Offset = offset;
            Attributes = attributes;
            Name = name;
        }
    }
}
