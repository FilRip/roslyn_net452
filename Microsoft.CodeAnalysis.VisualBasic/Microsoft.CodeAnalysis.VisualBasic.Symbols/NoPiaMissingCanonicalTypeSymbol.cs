using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class NoPiaMissingCanonicalTypeSymbol : ErrorTypeSymbol
	{
		private readonly AssemblySymbol _embeddingAssembly;

		private readonly string _guid;

		private readonly string _scope;

		private readonly string _identifier;

		private readonly string _fullTypeName;

		public AssemblySymbol EmbeddingAssembly => _embeddingAssembly;

		public string FullTypeName => _fullTypeName;

		public override string Name => _fullTypeName;

		internal override bool MangleName => false;

		public string Guid => _guid;

		public string Scope => _scope;

		public string Identifier => _identifier;

		internal override DiagnosticInfo ErrorInfo => ErrorFactory.ErrorInfo(ERRID.ERR_AbsentReferenceToPIA1, _fullTypeName);

		public NoPiaMissingCanonicalTypeSymbol(AssemblySymbol embeddingAssembly, string fullTypeName, string guid, string scope, string identifier)
		{
			_fullTypeName = fullTypeName;
			_embeddingAssembly = embeddingAssembly;
			_guid = guid;
			_scope = scope;
			_identifier = identifier;
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
