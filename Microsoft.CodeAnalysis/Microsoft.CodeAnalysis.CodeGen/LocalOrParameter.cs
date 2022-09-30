using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public struct LocalOrParameter
    {
        public readonly LocalDefinition? Local;

        public readonly int ParameterIndex;

        private LocalOrParameter(LocalDefinition? local, int parameterIndex)
        {
            Local = local;
            ParameterIndex = parameterIndex;
        }

        public static implicit operator LocalOrParameter(LocalDefinition? local)
        {
            return new LocalOrParameter(local, -1);
        }

        public static implicit operator LocalOrParameter(int parameterIndex)
        {
            return new LocalOrParameter(null, parameterIndex);
        }

        private string GetDebuggerDisplay()
        {
            if (Local == null)
            {
                return ParameterIndex.ToString();
            }
            return Local!.GetDebuggerDisplay();
        }
    }
}
