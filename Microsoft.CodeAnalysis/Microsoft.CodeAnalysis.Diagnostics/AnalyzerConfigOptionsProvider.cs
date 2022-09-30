namespace Microsoft.CodeAnalysis.Diagnostics
{
    public abstract class AnalyzerConfigOptionsProvider
    {
        public abstract AnalyzerConfigOptions GlobalOptions { get; }

        public abstract AnalyzerConfigOptions GetOptions(SyntaxTree tree);

        public abstract AnalyzerConfigOptions GetOptions(AdditionalText textFile);
    }
}
