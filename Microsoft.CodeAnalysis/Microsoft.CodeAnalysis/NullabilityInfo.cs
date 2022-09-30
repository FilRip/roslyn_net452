using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public readonly struct NullabilityInfo : IEquatable<NullabilityInfo>
    {
        public NullableAnnotation Annotation { get; }

        public NullableFlowState FlowState { get; }

        public NullabilityInfo(NullableAnnotation annotation, NullableFlowState flowState)
        {
            Annotation = annotation;
            FlowState = flowState;
        }

        private string GetDebuggerDisplay()
        {
            return $"{{Annotation: {Annotation}, Flow State: {FlowState}}}";
        }

        public override bool Equals(object? other)
        {
            if (other is NullabilityInfo other2)
            {
                return Equals(other2);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Annotation.GetHashCode(), FlowState.GetHashCode());
        }

        public bool Equals(NullabilityInfo other)
        {
            if (Annotation == other.Annotation)
            {
                return FlowState == other.FlowState;
            }
            return false;
        }
    }
}
