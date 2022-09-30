namespace Microsoft.CodeAnalysis.CSharp
{
    public enum LambdaConversionResult
    {
        Success,
        BadTargetType,
        BadParameterCount,
        MissingSignatureWithOutParameter,
        MismatchedParameterType,
        RefInImplicitlyTypedLambda,
        StaticTypeInImplicitlyTypedLambda,
        ExpressionTreeMustHaveDelegateTypeArgument,
        ExpressionTreeFromAnonymousMethod,
        BindingFailed
    }
}
