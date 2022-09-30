namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct TypeParameterDiagnosticInfo
	{
		public readonly TypeParameterSymbol TypeParameter;

		public readonly TypeParameterConstraint Constraint;

		public readonly UseSiteInfo<AssemblySymbol> UseSiteInfo;

		public TypeParameterDiagnosticInfo(TypeParameterSymbol typeParameter, UseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			this = default(TypeParameterDiagnosticInfo);
			TypeParameter = typeParameter;
			UseSiteInfo = useSiteInfo;
		}

		public TypeParameterDiagnosticInfo(TypeParameterSymbol typeParameter, DiagnosticInfo diagnostic)
			: this(typeParameter, new UseSiteInfo<AssemblySymbol>(diagnostic))
		{
		}

		public TypeParameterDiagnosticInfo(TypeParameterSymbol typeParameter, TypeParameterConstraint constraint, DiagnosticInfo diagnostic)
			: this(typeParameter, diagnostic)
		{
			Constraint = constraint;
		}
	}
}
