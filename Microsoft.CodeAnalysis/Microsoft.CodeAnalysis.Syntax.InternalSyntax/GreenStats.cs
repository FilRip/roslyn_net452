using System.Diagnostics;

namespace Microsoft.CodeAnalysis.Syntax.InternalSyntax
{
    public class GreenStats
    {
        public static void NoteGreen(GreenNode node)
        {
        }

        [Conditional("DEBUG")]
        public static void ItemAdded()
        {
        }

        [Conditional("DEBUG")]
        public static void ItemCacheable()
        {
        }

        [Conditional("DEBUG")]
        public static void CacheHit()
        {
        }
    }
}
