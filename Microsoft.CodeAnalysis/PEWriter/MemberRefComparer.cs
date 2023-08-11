// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.Cci
{
    internal sealed class MemberRefComparer : IEqualityComparer<ITypeMemberReference>
    {
        private readonly MetadataWriter _metadataWriter;

        internal MemberRefComparer(MetadataWriter metadataWriter)
        {
            _metadataWriter = metadataWriter;
        }

        public bool Equals(ITypeMemberReference? x, ITypeMemberReference? y)
        {
            if (x == y)
            {
                return true;
            }
            RoslynDebug.Assert(x is object && y is object);

            if (x.GetContainingType(_metadataWriter.Context) != y.GetContainingType(_metadataWriter.Context) &&
                _metadataWriter.GetMemberReferenceParent(x) != _metadataWriter.GetMemberReferenceParent(y))
            {
                return false;
            }

            if (x.Name != y.Name)
            {
                return false;
            }
            if (x is IFieldReference xf && y is IFieldReference yf)
            {
                return _metadataWriter.GetFieldSignatureIndex(xf) == _metadataWriter.GetFieldSignatureIndex(yf);
            }
            if (x is IMethodReference xm && y is IMethodReference ym)
            {
                return _metadataWriter.GetMethodSignatureHandle(xm) == _metadataWriter.GetMethodSignatureHandle(ym);
            }

            return false;
        }

        public int GetHashCode(ITypeMemberReference memberRef)
        {
            int hash = Hash.Combine(memberRef.Name, _metadataWriter.GetMemberReferenceParent(memberRef).GetHashCode());

            if (memberRef is IFieldReference fieldRef)
            {
                hash = Hash.Combine(hash, _metadataWriter.GetFieldSignatureIndex(fieldRef).GetHashCode());
            }
            else
            {
                if (memberRef is IMethodReference methodRef)
                {
                    hash = Hash.Combine(hash, _metadataWriter.GetMethodSignatureHandle(methodRef).GetHashCode());
                }
            }

            return hash;
        }
    }
}
