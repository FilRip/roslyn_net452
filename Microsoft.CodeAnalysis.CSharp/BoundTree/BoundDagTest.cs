// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class BoundDagTest
    {
        public override bool Equals([NotNullWhen(true)] object? obj) => this.Equals(obj as BoundDagTest);

        private bool Equals(BoundDagTest? other)
        {
            if (other is null || this.Kind != other.Kind)
                return false;
            if (this == other)
                return true;

            return (this, other)
switch
            {
                (BoundDagTypeTest x, BoundDagTypeTest y) => x.Type.Equals(y.Type, TypeCompareKind.AllIgnoreOptions),
                (BoundDagNonNullTest x, BoundDagNonNullTest y) => x.IsExplicitTest == y.IsExplicitTest,
                (BoundDagExplicitNullTest x, BoundDagExplicitNullTest y) => true,
                (BoundDagValueTest x, BoundDagValueTest y) => x.Value.Equals(y.Value),
                (BoundDagRelationalTest x, BoundDagRelationalTest y) => x.Relation == y.Relation && x.Value.Equals(y.Value),
                _ => throw ExceptionUtilities.UnexpectedValue(this),
            };
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Kind.GetHashCode(), Input.GetHashCode());
        }
    }
}
