namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal readonly struct TypeParameterDiagnosticInfo
    {
        public readonly TypeParameterSymbol TypeParameter;

        public readonly UseSiteInfo<AssemblySymbol> UseSiteInfo;

        public TypeParameterDiagnosticInfo(TypeParameterSymbol typeParameter, UseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeParameter = typeParameter;
            UseSiteInfo = useSiteInfo;
        }
    }
}
