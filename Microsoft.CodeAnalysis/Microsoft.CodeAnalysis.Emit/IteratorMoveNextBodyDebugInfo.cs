using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.Emit
{
    public sealed class IteratorMoveNextBodyDebugInfo : StateMachineMoveNextBodyDebugInfo
    {
        public IteratorMoveNextBodyDebugInfo(IMethodDefinition kickoffMethod)
            : base(kickoffMethod)
        {
        }
    }
}
