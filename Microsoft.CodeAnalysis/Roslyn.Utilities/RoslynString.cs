#nullable enable

#nullable enable

namespace Roslyn.Utilities
{
    public static class RoslynString
    {
        public static bool IsNullOrEmpty([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}
