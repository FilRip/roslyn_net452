using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class NoPiaAmbiguousCanonicalTypeSymbol : ErrorTypeSymbol
	{
		private readonly AssemblySymbol _embeddingAssembly;

		private readonly NamedTypeSymbol _firstCandidate;

		private readonly NamedTypeSymbol _secondCandidate;

		internal override bool MangleName => false;

		public AssemblySymbol EmbeddingAssembly => _embeddingAssembly;

		public NamedTypeSymbol FirstCandidate => _firstCandidate;

		public NamedTypeSymbol SecondCandidate => _secondCandidate;

		internal override DiagnosticInfo ErrorInfo => ErrorFactory.ErrorInfo(ERRID.ERR_AbsentReferenceToPIA1, CustomSymbolDisplayFormatter.QualifiedName(_firstCandidate));

		public NoPiaAmbiguousCanonicalTypeSymbol(AssemblySymbol embeddingAssembly, NamedTypeSymbol firstCandidate, NamedTypeSymbol secondCandidate)
		{
			_embeddingAssembly = embeddingAssembly;
			_firstCandidate = firstCandidate;
			_secondCandidate = secondCandidate;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			return (object)obj == this;
		}
	}
}
