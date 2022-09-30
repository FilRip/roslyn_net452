namespace Microsoft.CodeAnalysis.CSharp
{
    public enum UserDefinedConversionResultKind : byte
    {
        NoApplicableOperators,
        NoBestSourceType,
        NoBestTargetType,
        Ambiguous,
        Valid
    }
}
