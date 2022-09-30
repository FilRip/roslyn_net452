using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratedSourceResult
    {
        public SyntaxTree SyntaxTree { get; }

        public SourceText SourceText { get; }

        public string HintName { get; }

        internal GeneratedSourceResult(SyntaxTree tree, SourceText text, string hintName)
        {
            SyntaxTree = tree;
            SourceText = text;
            HintName = hintName;
        }
    }
}
