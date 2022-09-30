namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public interface IAttributeTargetSymbol
    {
        IAttributeTargetSymbol AttributesOwner { get; }

        AttributeLocation AllowedAttributeLocations { get; }

        AttributeLocation DefaultAttributeLocation { get; }
    }
}
