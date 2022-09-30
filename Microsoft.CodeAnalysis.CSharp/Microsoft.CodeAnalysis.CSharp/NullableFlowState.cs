namespace Microsoft.CodeAnalysis.CSharp
{
    public enum NullableFlowState : byte
    {
        NotNull = 0,
        MaybeNull = 1,
        MaybeDefault = 3
    }
}
