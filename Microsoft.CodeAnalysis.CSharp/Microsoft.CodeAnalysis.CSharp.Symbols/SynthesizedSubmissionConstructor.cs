using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedSubmissionConstructor : SynthesizedInstanceConstructor
    {
        private readonly ImmutableArray<ParameterSymbol> _parameters;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        internal SynthesizedSubmissionConstructor(NamedTypeSymbol containingType, BindingDiagnosticBag diagnostics)
            : base(containingType)
        {
            CSharpCompilation declaringCompilation = containingType.DeclaringCompilation;
            ArrayTypeSymbol arrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(declaringCompilation.GetSpecialType(SpecialType.System_Object));
            UseSiteInfo<AssemblySymbol> useSiteInfo = arrayTypeSymbol.GetUseSiteInfo();
            diagnostics.Add(useSiteInfo, NoLocation.Singleton);
            _parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(arrayTypeSymbol), 0, RefKind.None, "submissionArray"));
        }
    }
}
