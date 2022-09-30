namespace Microsoft.CodeAnalysis.CSharp
{
    public enum OperatorAnalysisResultKind : byte
    {
        Undefined,
        Inapplicable,
        Worse,
        Applicable
    }
}
