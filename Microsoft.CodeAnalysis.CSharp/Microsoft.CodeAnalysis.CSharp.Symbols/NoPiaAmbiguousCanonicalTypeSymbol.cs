using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class NoPiaAmbiguousCanonicalTypeSymbol : ErrorTypeSymbol
    {
        private readonly AssemblySymbol _embeddingAssembly;

        private readonly NamedTypeSymbol _firstCandidate;

        private readonly NamedTypeSymbol _secondCandidate;

        internal override bool MangleName => false;

        public AssemblySymbol EmbeddingAssembly => _embeddingAssembly;

        public NamedTypeSymbol FirstCandidate => _firstCandidate;

        public NamedTypeSymbol SecondCandidate => _secondCandidate;

        internal override DiagnosticInfo ErrorInfo => new CSDiagnosticInfo(ErrorCode.ERR_NoCanonicalView, _firstCandidate);

        public NoPiaAmbiguousCanonicalTypeSymbol(AssemblySymbol embeddingAssembly, NamedTypeSymbol firstCandidate, NamedTypeSymbol secondCandidate, TupleExtraData? tupleData = null)
            : base(tupleData)
        {
            _embeddingAssembly = embeddingAssembly;
            _firstCandidate = firstCandidate;
            _secondCandidate = secondCandidate;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new NoPiaAmbiguousCanonicalTypeSymbol(_embeddingAssembly, _firstCandidate, _secondCandidate, newData);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return (object)this == t2;
        }
    }
}
