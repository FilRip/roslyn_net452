// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public struct BinaryOperatorSignature : IEquatable<BinaryOperatorSignature>
    {
        public static BinaryOperatorSignature Error { get; set; } = default;

        public readonly TypeSymbol LeftType;
        public readonly TypeSymbol RightType;
        public readonly TypeSymbol ReturnType;
        public readonly MethodSymbol Method;
        public readonly BinaryOperatorKind Kind;

        /// <summary>
        /// To duplicate native compiler behavior for some scenarios we force a priority among
        /// operators. If two operators are both applicable and both have a non-null Priority,
        /// the one with the numerically lower Priority value is preferred.
        /// </summary>
        public int? Priority { get; set; }

        public BinaryOperatorSignature(BinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol returnType, MethodSymbol method = null)
        {
            this.Kind = kind;
            this.LeftType = leftType;
            this.RightType = rightType;
            this.ReturnType = returnType;
            this.Method = method;
            this.Priority = null;
        }

        public override string ToString()
        {
            return $"kind: {this.Kind} leftType: {this.LeftType} leftRefKind: {this.LeftRefKind} rightType: {this.RightType} rightRefKind: {this.RightRefKind} return: {this.ReturnType}";
        }

        public bool Equals(BinaryOperatorSignature other)
        {
            return
                this.Kind == other.Kind &&
                TypeSymbol.Equals(this.LeftType, other.LeftType, TypeCompareKind.ConsiderEverything2) &&
                TypeSymbol.Equals(this.RightType, other.RightType, TypeCompareKind.ConsiderEverything2) &&
                TypeSymbol.Equals(this.ReturnType, other.ReturnType, TypeCompareKind.ConsiderEverything2) &&
                this.Method == other.Method;
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryOperatorSignature signature && Equals(signature);
        }

        public static bool operator ==(BinaryOperatorSignature x, BinaryOperatorSignature y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(BinaryOperatorSignature x, BinaryOperatorSignature y)
        {
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ReturnType,
                   Hash.Combine(LeftType,
                   Hash.Combine(RightType,
                   Hash.Combine(Method, (int)Kind))));
        }

        public RefKind LeftRefKind
        {
            get
            {
                if (Method is not null && !Method.ParameterRefKinds.IsDefaultOrEmpty)
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
                if (Method is not null && !Method.ParameterRefKinds.IsDefaultOrEmpty)
                {
                    return Method.ParameterRefKinds[1];
                }

                return RefKind.None;
            }
        }
    }
}
