namespace Microsoft.CodeAnalysis.Operations
{
    internal class ForToLoopOperationUserDefinedInfo
    {
        public readonly IBinaryOperation Addition;

        public readonly IBinaryOperation Subtraction;

        public readonly IOperation LessThanOrEqual;

        public readonly IOperation GreaterThanOrEqual;

        public ForToLoopOperationUserDefinedInfo(IBinaryOperation addition, IBinaryOperation subtraction, IOperation lessThanOrEqual, IOperation greaterThanOrEqual)
        {
            Addition = addition;
            Subtraction = subtraction;
            LessThanOrEqual = lessThanOrEqual;
            GreaterThanOrEqual = greaterThanOrEqual;
        }
    }
}
