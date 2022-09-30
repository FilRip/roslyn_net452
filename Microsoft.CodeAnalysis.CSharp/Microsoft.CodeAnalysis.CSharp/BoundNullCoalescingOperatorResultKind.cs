namespace Microsoft.CodeAnalysis.CSharp
{
    public enum BoundNullCoalescingOperatorResultKind
    {
        NoCommonType,
        LeftType,
        LeftUnwrappedType,
        RightType,
        LeftUnwrappedRightType,
        RightDynamicType
    }
}
