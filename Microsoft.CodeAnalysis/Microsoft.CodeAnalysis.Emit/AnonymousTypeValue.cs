using System.Diagnostics;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.Emit
{
    [DebuggerDisplay("{Name, nq}")]
    public struct AnonymousTypeValue
    {
        public readonly string Name;

        public readonly int UniqueIndex;

        public readonly ITypeDefinition Type;

        public AnonymousTypeValue(string name, int uniqueIndex, ITypeDefinition type)
        {
            Name = name;
            UniqueIndex = uniqueIndex;
            Type = type;
        }
    }
}
