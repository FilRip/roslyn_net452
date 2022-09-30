using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class PlaceholderTypeArgumentSymbol : ErrorTypeSymbol
    {
        private static readonly TypeWithAnnotations s_instance = TypeWithAnnotations.Create(new PlaceholderTypeArgumentSymbol());

        public override string Name => string.Empty;

        internal override bool MangleName => false;

        internal override DiagnosticInfo? ErrorInfo => null;

        public static ImmutableArray<TypeWithAnnotations> CreateTypeArguments(ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            return typeParameters.SelectAsArray((TypeParameterSymbol _) => s_instance);
        }

        private PlaceholderTypeArgumentSymbol()
        {
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            return (object)t2 == this;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
    }
}
