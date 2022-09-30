using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class UnboundArgumentErrorTypeSymbol : ErrorTypeSymbol
    {
        public static readonly ErrorTypeSymbol Instance = new UnboundArgumentErrorTypeSymbol(string.Empty, new CSDiagnosticInfo(ErrorCode.ERR_UnexpectedUnboundGenericName));

        private readonly string _name;

        private readonly DiagnosticInfo _errorInfo;

        public override string Name => _name;

        internal override bool MangleName => false;

        internal override DiagnosticInfo ErrorInfo => _errorInfo;

        public static ImmutableArray<TypeWithAnnotations> CreateTypeArguments(ImmutableArray<TypeParameterSymbol> typeParameters, int n, DiagnosticInfo errorInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            for (int i = 0; i < n; i++)
            {
                string name = ((i < typeParameters.Length) ? typeParameters[i].Name : string.Empty);
                instance.Add(TypeWithAnnotations.Create(new UnboundArgumentErrorTypeSymbol(name, errorInfo)));
            }
            return instance.ToImmutableAndFree();
        }

        private UnboundArgumentErrorTypeSymbol(string name, DiagnosticInfo errorInfo, TupleExtraData? tupleData = null)
            : base(tupleData)
        {
            _name = name;
            _errorInfo = errorInfo;
        }

        protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
        {
            return new UnboundArgumentErrorTypeSymbol(_name, _errorInfo, newData);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if ((object)t2 == this)
            {
                return true;
            }
            if (t2 is UnboundArgumentErrorTypeSymbol unboundArgumentErrorTypeSymbol && string.Equals(unboundArgumentErrorTypeSymbol._name, _name, StringComparison.Ordinal))
            {
                return object.Equals(unboundArgumentErrorTypeSymbol._errorInfo, _errorInfo);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (_errorInfo != null)
            {
                return Hash.Combine(_name, _errorInfo.Code);
            }
            return _name.GetHashCode();
        }
    }
}
