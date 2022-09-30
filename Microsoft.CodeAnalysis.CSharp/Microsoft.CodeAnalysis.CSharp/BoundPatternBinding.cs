using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct BoundPatternBinding
    {
        public readonly BoundExpression VariableAccess;

        public readonly BoundDagTemp TempContainingValue;

        public BoundPatternBinding(BoundExpression variableAccess, BoundDagTemp tempContainingValue)
        {
            VariableAccess = variableAccess;
            TempContainingValue = tempContainingValue;
        }

        public override string ToString()
        {
            return GetDebuggerDisplay();
        }

        internal string GetDebuggerDisplay()
        {
            return "(" + VariableAccess.GetDebuggerDisplay() + " = " + TempContainingValue.GetDebuggerDisplay() + ")";
        }
    }
}
