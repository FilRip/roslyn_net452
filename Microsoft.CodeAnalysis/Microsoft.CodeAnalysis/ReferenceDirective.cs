#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct ReferenceDirective
    {
        public readonly string? File;

        public readonly Location? Location;

        public ReferenceDirective(string file, Location location)
        {
            File = file;
            Location = location;
        }
    }
}
