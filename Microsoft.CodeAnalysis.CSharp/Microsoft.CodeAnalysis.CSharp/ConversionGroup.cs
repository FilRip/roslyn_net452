using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public sealed class ConversionGroup
    {
        internal readonly Conversion Conversion;

        internal readonly TypeWithAnnotations ExplicitType;

        internal bool IsExplicitConversion => ExplicitType.HasType;

        internal ConversionGroup(Conversion conversion, TypeWithAnnotations explicitType = default(TypeWithAnnotations))
        {
            Conversion = conversion;
            ExplicitType = explicitType;
        }
    }
}
