namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    internal readonly struct NullableContextState
    {
        internal enum State : byte
        {
            Unknown,
            Disabled,
            Enabled,
            ExplicitlyRestored
        }

        internal int Position { get; }

        internal State WarningsState { get; }

        internal State AnnotationsState { get; }

        internal NullableContextState(int position, State warningsState, State annotationsState)
        {
            Position = position;
            WarningsState = warningsState;
            AnnotationsState = annotationsState;
        }
    }
}
