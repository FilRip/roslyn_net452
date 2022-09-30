namespace Microsoft.CodeAnalysis
{
    public static class NullableContextExtensions
    {
        private static bool IsFlagSet(NullableContext context, NullableContext flag)
        {
            return (context & flag) == flag;
        }

        public static bool WarningsEnabled(this NullableContext context)
        {
            return IsFlagSet(context, NullableContext.WarningsEnabled);
        }

        public static bool AnnotationsEnabled(this NullableContext context)
        {
            return IsFlagSet(context, NullableContext.AnnotationsEnabled);
        }

        public static bool WarningsInherited(this NullableContext context)
        {
            return IsFlagSet(context, NullableContext.WarningsContextInherited);
        }

        public static bool AnnotationsInherited(this NullableContext context)
        {
            return IsFlagSet(context, NullableContext.AnnotationsContextInherited);
        }
    }
}
