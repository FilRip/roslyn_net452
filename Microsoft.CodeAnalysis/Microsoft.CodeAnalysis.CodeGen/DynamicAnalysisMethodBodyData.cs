using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public sealed class DynamicAnalysisMethodBodyData
    {
        public readonly ImmutableArray<SourceSpan> Spans;

        public DynamicAnalysisMethodBodyData(ImmutableArray<SourceSpan> spans)
        {
            Spans = spans;
        }
    }
}
