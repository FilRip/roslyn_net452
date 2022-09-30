using System;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct BinaryOperatorSignature : IEquatable<BinaryOperatorSignature>
    {
        public static BinaryOperatorSignature Error;

        public readonly TypeSymbol LeftType;

        public readonly TypeSymbol RightType;

        public readonly TypeSymbol ReturnType;

        public readonly MethodSymbol Method;

        public readonly BinaryOperatorKind Kind;

        public int? Priority;

        public RefKind LeftRefKind
        {
            get
            {
                if ((object)Method != null && !Method.ParameterRefKinds.IsDefaultOrEmpty)
                {
                    return Method.ParameterRefKinds[0];
                }
                return RefKind.None;
            }
        }

        public RefKind RightRefKind
        {
            get
            {
                if ((object)Method != null && !Method.ParameterRefKinds.IsDefaultOrEmpty)
                {
                    return Method.ParameterRefKinds[1];
                }
                return RefKind.None;
            }
        }

        public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol returnType, MethodSymbol method = null)
        {
            Kind = kind;
            LeftType = leftType;
            RightType = rightType;
            ReturnType = returnType;
            Method = method;
            Priority = null;
        }

        public override string ToString()
        {
            return $"kind: {Kind} leftType: {LeftType} leftRefKind: {LeftRefKind} rightType: {RightType} rightRefKind: {RightRefKind} return: {ReturnType}";
        }

        public bool Equals(BinaryOperatorSignature other)
        {
            if (Kind == other.Kind && TypeSymbol.Equals(LeftType, other.LeftType, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(RightType, other.RightType, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(ReturnType, other.ReturnType, TypeCompareKind.ConsiderEverything))
            {
                return Method == other.Method;
            }
            return false;
        }

        public static bool operator ==(BinaryOperatorSignature x, BinaryOperatorSignature y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(BinaryOperatorSignature x, BinaryOperatorSignature y)
        {
            return !x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (obj is BinaryOperatorSignature)
            {
                return Equals((BinaryOperatorSignature)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ReturnType, Hash.Combine(LeftType, Hash.Combine(RightType, Hash.Combine(Method, (int)Kind))));
        }
    }
}
