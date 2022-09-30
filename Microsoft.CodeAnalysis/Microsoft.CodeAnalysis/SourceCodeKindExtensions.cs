namespace Microsoft.CodeAnalysis
{
    public static class SourceCodeKindExtensions
    {
        public static SourceCodeKind MapSpecifiedToEffectiveKind(this SourceCodeKind kind)
        {
            if (kind != 0 && (uint)(kind - 1) <= 1u)
            {
                return SourceCodeKind.Script;
            }
            return SourceCodeKind.Regular;
        }

        public static bool IsValid(this SourceCodeKind value)
        {
            if (value >= SourceCodeKind.Regular)
            {
                return value <= SourceCodeKind.Script;
            }
            return false;
        }
    }
}
