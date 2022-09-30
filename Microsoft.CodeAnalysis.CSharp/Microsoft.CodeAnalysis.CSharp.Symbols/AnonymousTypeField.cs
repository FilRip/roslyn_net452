using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal struct AnonymousTypeField
    {
        public readonly string Name;

        public readonly Location Location;

        public readonly TypeWithAnnotations TypeWithAnnotations;

        public TypeSymbol Type => TypeWithAnnotations.Type;

        public AnonymousTypeField(string name, Location location, TypeWithAnnotations typeWithAnnotations)
        {
            Name = name;
            Location = location;
            TypeWithAnnotations = typeWithAnnotations;
        }

        [Conditional("DEBUG")]
        internal void AssertIsGood()
        {
        }
    }
}
