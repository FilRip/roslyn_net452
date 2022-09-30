using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    public sealed class EmitDifferenceResult : EmitResult
    {
        public EmitBaseline? Baseline { get; }

        public EmitDifferenceResult(bool success, ImmutableArray<Diagnostic> diagnostics, EmitBaseline? baseline)
            : base(success, diagnostics)
        {
            Baseline = baseline;
        }
    }
}
