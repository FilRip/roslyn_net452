using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace Microsoft.CodeAnalysis
{
    [StructLayout(LayoutKind.Auto)]
    public struct ParamInfo<TypeSymbol> where TypeSymbol : class
    {
        public bool IsByRef;

        public TypeSymbol Type;

        public ParameterHandle Handle;

        public ImmutableArray<ModifierInfo<TypeSymbol>> RefCustomModifiers;

        public ImmutableArray<ModifierInfo<TypeSymbol>> CustomModifiers;
    }
}
