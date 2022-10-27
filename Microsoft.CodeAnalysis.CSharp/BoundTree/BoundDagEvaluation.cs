// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundDagEvaluation
    {
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is BoundDagEvaluation other && this.Equals(other);
        public virtual bool Equals(BoundDagEvaluation other)
        {
            return this == other ||
                this.Kind == other.Kind &&
                this.Input.Equals(other.Input) &&
                this.Symbol.Equals(other.Symbol, TypeCompareKind.AllIgnoreOptions);
        }
        private Symbol Symbol
        {
            get
            {
                return this switch
                {
                    BoundDagFieldEvaluation e => e.Field.CorrespondingTupleField ?? e.Field,
                    BoundDagPropertyEvaluation e => e.Property,
                    BoundDagTypeEvaluation e => e.Type,
                    BoundDagDeconstructEvaluation e => e.DeconstructMethod,
                    BoundDagIndexEvaluation e => e.Property,
                    _ => throw ExceptionUtilities.UnexpectedValue(this.Kind),
                };
            }
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Input.GetHashCode(), this.Symbol?.GetHashCode() ?? 0);
        }
    }

    public partial class BoundDagIndexEvaluation
    {
        public override int GetHashCode() => base.GetHashCode() ^ this.Index;
        public override bool Equals(BoundDagEvaluation obj)
        {
            return this == obj ||
                base.Equals(obj) &&
                // base.Equals checks the kind field, so the following cast is safe
                this.Index == ((BoundDagIndexEvaluation)obj).Index;
        }
    }
}
