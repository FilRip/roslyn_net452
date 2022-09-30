namespace Microsoft.CodeAnalysis.Debugging
{
    public struct StateMachineHoistedLocalScope
    {
        public readonly int StartOffset;

        public readonly int EndOffset;

        public int Length => EndOffset - StartOffset;

        public bool IsDefault
        {
            get
            {
                if (StartOffset == 0)
                {
                    return EndOffset == 0;
                }
                return false;
            }
        }

        public StateMachineHoistedLocalScope(int startOffset, int endOffset)
        {
            StartOffset = startOffset;
            EndOffset = endOffset;
        }
    }
}
