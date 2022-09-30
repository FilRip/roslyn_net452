namespace Microsoft.CodeAnalysis
{
    public static class ObjectDisplayExtensions
    {
        public static bool IncludesOption(this ObjectDisplayOptions options, ObjectDisplayOptions flag)
        {
            return (options & flag) == flag;
        }
    }
}
