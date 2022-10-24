// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class LocalDataFlowPass<TLocalState, TLocalFunctionState>
    {
        public readonly struct VariableIdentifier : IEquatable<VariableIdentifier>
        {
            public readonly Symbol Symbol;
            /// <summary>
            /// Indicates whether this variable is nested inside another tracked variable.
            /// For instance, if a field `x` of a struct is a tracked variable, the symbol is not sufficient
            /// to uniquely determine which field is being tracked. The containing slot(s) would
            /// identify which tracked variable the field `x` is part of.
            /// </summary>
            public readonly int ContainingSlot;

            public VariableIdentifier(Symbol symbol, int containingSlot = 0)
            {
                Symbol = symbol;
                ContainingSlot = containingSlot;
            }

            public bool Exists
            {
                get { return (object)Symbol != null; }
            }

            public override int GetHashCode()
            {

                int currentKey = ContainingSlot;
                // MemberIndexOpt, if available, is a fast approach to comparing relative members,
                // and is necessary in cases such as anonymous types where OriginalDefinition will be distinct.
                int? thisIndex = Symbol.MemberIndexOpt;
                return thisIndex.HasValue ?
                    Hash.Combine(thisIndex.GetValueOrDefault(), currentKey) :
                    Hash.Combine(Symbol.OriginalDefinition, currentKey);
            }

            public bool Equals(VariableIdentifier other)
            {

                if (ContainingSlot != other.ContainingSlot)
                {
                    return false;
                }

                // MemberIndexOpt, if available, is a fast approach to comparing relative members,
                // and is necessary in cases such as anonymous types where OriginalDefinition will be distinct.
                int? thisIndex = Symbol.MemberIndexOpt;
                int? otherIndex = other.Symbol.MemberIndexOpt;
                if (thisIndex != otherIndex)
                {
                    return false;
                }

                if (thisIndex.HasValue)
                {
                    return true;
                }

                return Symbol.Equals(other.Symbol, TypeCompareKind.AllIgnoreOptions);
            }

            public override bool Equals(object? obj)
            {
                throw ExceptionUtilities.Unreachable;
            }

            [Obsolete]
            public static bool operator ==(VariableIdentifier left, VariableIdentifier right)
            {
                throw ExceptionUtilities.Unreachable;
            }

            [Obsolete]
            public static bool operator !=(VariableIdentifier left, VariableIdentifier right)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override string ToString()
            {
                return $"ContainingSlot={ContainingSlot}, Symbol={Symbol.GetDebuggerDisplay()}";
            }
        }
    }
}
