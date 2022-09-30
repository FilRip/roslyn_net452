using System.Linq;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class CSharpCompilationExtensions
    {
        internal static bool IsFeatureEnabled(this CSharpCompilation compilation, MessageID feature)
        {
            return ((CSharpParseOptions)(compilation.SyntaxTrees.FirstOrDefault()?.Options))?.IsFeatureEnabled(feature) ?? false;
        }

        internal static bool IsFeatureEnabled(this SyntaxNode? syntax, MessageID feature)
        {
            return ((CSharpParseOptions)(syntax?.SyntaxTree.Options))?.IsFeatureEnabled(feature) ?? false;
        }
    }
}
