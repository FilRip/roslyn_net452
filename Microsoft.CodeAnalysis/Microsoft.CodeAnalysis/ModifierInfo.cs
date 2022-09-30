using System.Runtime.InteropServices;

namespace Microsoft.CodeAnalysis
{
    [StructLayout(LayoutKind.Auto)]
    public struct ModifierInfo<TypeSymbol> where TypeSymbol : class
    {
        public readonly bool IsOptional;

        public readonly TypeSymbol Modifier;

        public ModifierInfo(bool isOptional, TypeSymbol modifier)
        {
            IsOptional = isOptional;
            Modifier = modifier;
        }
    }
}
