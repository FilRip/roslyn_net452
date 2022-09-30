using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratedSyntaxTree
    {
        public SourceText Text { get; }

        public string HintName { get; }

        public SyntaxTree Tree { get; }

        public GeneratedSyntaxTree(string hintName, SourceText text, SyntaxTree tree)
        {
            Text = text;
            HintName = hintName;
            Tree = tree;
        }
    }
}
