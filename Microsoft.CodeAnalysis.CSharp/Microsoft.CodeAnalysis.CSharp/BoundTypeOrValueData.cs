using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct BoundTypeOrValueData : IEquatable<BoundTypeOrValueData>
    {
        public Symbol ValueSymbol { get; }

        public BoundExpression ValueExpression { get; }

        public BindingDiagnosticBag ValueDiagnostics { get; }

        public BoundExpression TypeExpression { get; }

        public BindingDiagnosticBag TypeDiagnostics { get; }

        public BoundTypeOrValueData(Symbol valueSymbol, BoundExpression valueExpression, BindingDiagnosticBag valueDiagnostics, BoundExpression typeExpression, BindingDiagnosticBag typeDiagnostics)
        {
            ValueSymbol = valueSymbol;
            ValueExpression = valueExpression;
            ValueDiagnostics = valueDiagnostics;
            TypeExpression = typeExpression;
            TypeDiagnostics = typeDiagnostics;
        }

        public static bool operator ==(BoundTypeOrValueData a, BoundTypeOrValueData b)
        {
            if ((object)a.ValueSymbol == b.ValueSymbol && a.ValueExpression == b.ValueExpression && a.ValueDiagnostics == b.ValueDiagnostics && a.TypeExpression == b.TypeExpression)
            {
                return a.TypeDiagnostics == b.TypeDiagnostics;
            }
            return false;
        }

        public static bool operator !=(BoundTypeOrValueData a, BoundTypeOrValueData b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is BoundTypeOrValueData)
            {
                return (BoundTypeOrValueData)obj == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ValueSymbol.GetHashCode(), Hash.Combine(ValueExpression.GetHashCode(), Hash.Combine(ValueDiagnostics.GetHashCode(), Hash.Combine(TypeExpression.GetHashCode(), TypeDiagnostics.GetHashCode()))));
        }

        bool IEquatable<BoundTypeOrValueData>.Equals(BoundTypeOrValueData b)
        {
            return b == this;
        }
    }
}
