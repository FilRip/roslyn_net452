using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis
{
    public readonly struct GeneratedSourceText
    {
        public SourceText Text { get; }

        public string HintName { get; }

        public GeneratedSourceText(string hintName, SourceText text)
        {
            Text = text;
            HintName = hintName;
        }
    }
}
