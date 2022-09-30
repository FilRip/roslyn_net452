using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public sealed class CompilerAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly ImmutableDictionary<object, AnalyzerConfigOptions> _treeDict;

        public static CompilerAnalyzerConfigOptionsProvider Empty { get; } = new CompilerAnalyzerConfigOptionsProvider(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, CompilerAnalyzerConfigOptions.Empty);


        public override AnalyzerConfigOptions GlobalOptions { get; }

        public CompilerAnalyzerConfigOptionsProvider(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict, AnalyzerConfigOptions globalOptions)
        {
            _treeDict = treeDict;
            GlobalOptions = globalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            if (!_treeDict.TryGetValue(tree, out var value))
            {
                return CompilerAnalyzerConfigOptions.Empty;
            }
            return value;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            if (!_treeDict.TryGetValue(textFile, out var value))
            {
                return CompilerAnalyzerConfigOptions.Empty;
            }
            return value;
        }

        internal CompilerAnalyzerConfigOptionsProvider WithAdditionalTreeOptions(ImmutableDictionary<object, AnalyzerConfigOptions> treeDict)
        {
            return new CompilerAnalyzerConfigOptionsProvider(_treeDict.AddRange(treeDict), GlobalOptions);
        }

        internal CompilerAnalyzerConfigOptionsProvider WithGlobalOptions(AnalyzerConfigOptions globalOptions)
        {
            return new CompilerAnalyzerConfigOptionsProvider(_treeDict, globalOptions);
        }
    }
}
