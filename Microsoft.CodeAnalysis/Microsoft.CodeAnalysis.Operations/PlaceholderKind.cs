namespace Microsoft.CodeAnalysis.Operations
{
    public enum PlaceholderKind
    {
        Unspecified,
        SwitchOperationExpression,
        ForToLoopBinaryOperatorLeftOperand,
        ForToLoopBinaryOperatorRightOperand,
        AggregationGroup
    }
}
