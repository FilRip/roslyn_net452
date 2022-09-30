namespace Microsoft.CodeAnalysis
{
    public static class NullableContextOptionsExtensions
    {
        private static bool IsFlagSet(NullableContextOptions context, NullableContextOptions flag)
        {
            return (context & flag) == flag;
        }

        public static bool WarningsEnabled(this NullableContextOptions context)
        {
            return IsFlagSet(context, NullableContextOptions.Warnings);
        }

        public static bool AnnotationsEnabled(this NullableContextOptions context)
        {
            return IsFlagSet(context, NullableContextOptions.Annotations);
        }
    }
}
