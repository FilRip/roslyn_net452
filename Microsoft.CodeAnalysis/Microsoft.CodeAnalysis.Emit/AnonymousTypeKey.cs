using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Emit
{
    public struct AnonymousTypeKey : IEquatable<AnonymousTypeKey>
    {
        public readonly bool IsDelegate;

        public readonly ImmutableArray<AnonymousTypeKeyField> Fields;

        public AnonymousTypeKey(ImmutableArray<AnonymousTypeKeyField> fields, bool isDelegate = false)
        {
            IsDelegate = isDelegate;
            Fields = fields;
        }

        public bool Equals(AnonymousTypeKey other)
        {
            if (IsDelegate == other.IsDelegate)
            {
                return Fields.SequenceEqual(other.Fields);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals((AnonymousTypeKey)obj);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(IsDelegate.GetHashCode(), Hash.CombineValues(Fields));
        }

        private string GetDebuggerDisplay()
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            StringBuilder builder = instance.Builder;
            for (int i = 0; i < Fields.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append("|");
                }
                builder.Append(Fields[i]);
            }
            return instance.ToStringAndFree();
        }
    }
}
