#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class NoLocation : Location
    {
        public static readonly Location Singleton = new NoLocation();

        public override LocationKind Kind => LocationKind.None;

        private NoLocation()
        {
        }

        public override bool Equals(object? obj)
        {
            return (object)this == obj;
        }

        public override int GetHashCode()
        {
            return 0x16487756;
        }
    }
}
