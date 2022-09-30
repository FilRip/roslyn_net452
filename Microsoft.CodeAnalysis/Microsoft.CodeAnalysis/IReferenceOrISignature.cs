using System;
using System.Runtime.CompilerServices;

using Microsoft.Cci;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal readonly struct IReferenceOrISignature : IEquatable<IReferenceOrISignature>
    {
        private readonly object _item;

        public IReferenceOrISignature(IReference item)
        {
            _item = item;
        }

        public IReferenceOrISignature(ISignature item)
        {
            _item = item;
        }

        public IReferenceOrISignature(IMethodReference item)
        {
            _item = item;
        }

        public bool Equals(IReferenceOrISignature other)
        {
            return _item == other._item;
        }

        public override bool Equals(object? obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(_item);
        }

        public override string ToString()
        {
            return _item.ToString() ?? "null";
        }

        internal object AsObject()
        {
            return _item;
        }
    }
}
