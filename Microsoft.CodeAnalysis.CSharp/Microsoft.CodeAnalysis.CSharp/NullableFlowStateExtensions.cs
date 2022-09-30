using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class NullableFlowStateExtensions
    {
        public static bool MayBeNull(this NullableFlowState state)
        {
            return state != NullableFlowState.NotNull;
        }

        public static bool IsNotNull(this NullableFlowState state)
        {
            return state == NullableFlowState.NotNull;
        }

        public static NullableFlowState Join(this NullableFlowState a, NullableFlowState b)
        {
            if ((int)a <= (int)b)
            {
                return b;
            }
            return a;
        }

        public static NullableFlowState Meet(this NullableFlowState a, NullableFlowState b)
        {
            if ((int)a >= (int)b)
            {
                return b;
            }
            return a;
        }

        internal static Microsoft.CodeAnalysis.NullableFlowState ToPublicFlowState(this NullableFlowState nullableFlowState)
        {
            return nullableFlowState switch
            {
                NullableFlowState.NotNull => Microsoft.CodeAnalysis.NullableFlowState.NotNull,
                NullableFlowState.MaybeNull => Microsoft.CodeAnalysis.NullableFlowState.MaybeNull,
                NullableFlowState.MaybeDefault => Microsoft.CodeAnalysis.NullableFlowState.MaybeNull,
                _ => throw ExceptionUtilities.UnexpectedValue(nullableFlowState),
            };
        }

        public static NullableFlowState ToInternalFlowState(this Microsoft.CodeAnalysis.NullableFlowState flowState)
        {
            return flowState switch
            {
                Microsoft.CodeAnalysis.NullableFlowState.None => NullableFlowState.NotNull,
                Microsoft.CodeAnalysis.NullableFlowState.NotNull => NullableFlowState.NotNull,
                Microsoft.CodeAnalysis.NullableFlowState.MaybeNull => NullableFlowState.MaybeNull,
                _ => throw ExceptionUtilities.UnexpectedValue(flowState),
            };
        }
    }
}
