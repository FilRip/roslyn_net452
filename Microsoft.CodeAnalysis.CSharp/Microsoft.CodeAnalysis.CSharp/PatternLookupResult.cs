namespace Microsoft.CodeAnalysis.CSharp
{
    internal enum PatternLookupResult
    {
        Success,
        NotAMethod,
        NotCallable,
        NoResults,
        ResultHasErrors
    }
}
