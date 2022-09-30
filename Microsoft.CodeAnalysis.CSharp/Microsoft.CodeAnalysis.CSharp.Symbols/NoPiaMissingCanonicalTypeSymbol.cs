using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal class NoPiaMissingCanonicalTypeSymbol : ErrorTypeSymbol
    {
        private readonly AssemblySymbol _embeddingAssembly;

        private readonly string _fullTypeName;

        private readonly string _guid;

        private readonly string _scope;

        private readonly string _identifier;

        public AssemblySymbol EmbeddingAssembly => _embeddingAssembly;

        public string FullTypeName => _fullTypeName;

        internal override bool MangleName => false;

        public string Guid => _guid;

        public string Scope => _scope;

        public string Identifier => _identifier;

        internal override DiagnosticInfo ErrorInfo => new CSDiagnosticInfo(ErrorCode.ERR_NoCanonicalView, _fullTypeName);

        public NoPiaMissingCanonicalTypeSymbol(AssemblySymbol embeddingAssembly, string fullTypeName, string guid, string scope, string identifier, TupleExtraData? tupleData = null)
            : base(tupleData)
        {
            _embeddingAssembly = embeddingAssembly;
            _fullTypeName = fullTypeName;
            _guid = guid;
            _scope = scope;
            _identifier = identifier;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new NoPiaMissingCanonicalTypeSymbol(_embeddingAssembly, _fullTypeName, _guid, _scope, _identifier, newData);
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
