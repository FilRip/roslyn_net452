namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal enum NullableContextKind : byte
    {
        Unknown,
        None,
        Oblivious,
        NotAnnotated,
        Annotated
    }
}
