namespace Microsoft.CodeAnalysis
{
    public static class NullableFlowStateExtensions
    {
        public static NullableAnnotation ToAnnotation(this NullableFlowState nullableFlowState)
        {
            return nullableFlowState switch
            {
                NullableFlowState.MaybeNull => NullableAnnotation.Annotated,
                NullableFlowState.NotNull => NullableAnnotation.NotAnnotated,
                _ => NullableAnnotation.None,
            };
        }
    }
}
