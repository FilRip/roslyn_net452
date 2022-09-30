namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class MutableTypeMap : AbstractTypeParameterMap
    {
        internal MutableTypeMap()
            : base(new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>())
        {
        }

        internal void Add(TypeParameterSymbol key, TypeWithAnnotations value)
        {
            Mapping.Add(key, value);
        }
    }
}
