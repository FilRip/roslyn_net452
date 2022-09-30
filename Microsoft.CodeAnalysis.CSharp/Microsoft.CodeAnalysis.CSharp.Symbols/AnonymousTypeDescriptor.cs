using System;
using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal readonly struct AnonymousTypeDescriptor : IEquatable<AnonymousTypeDescriptor>
    {
        public readonly Location Location;

        public readonly ImmutableArray<AnonymousTypeField> Fields;

        public readonly string Key;

        public AnonymousTypeDescriptor(ImmutableArray<AnonymousTypeField> fields, Location location)
        {
            Fields = fields;
            Location = location;
            Key = ComputeKey(fields, (AnonymousTypeField f) => f.Name);
        }

        internal static string ComputeKey<T>(ImmutableArray<T> fields, Func<T, string> getName)
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            ImmutableArray<T>.Enumerator enumerator = fields.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                instance.Builder.Append('|');
                instance.Builder.Append(getName(current));
            }
            return instance.ToStringAndFree();
        }

        [Conditional("DEBUG")]
        internal void AssertIsGood()
        {
            ImmutableArray<AnonymousTypeField>.Enumerator enumerator = Fields.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _ = enumerator.Current;
            }
        }

        public bool Equals(AnonymousTypeDescriptor desc)
        {
            return Equals(desc, TypeCompareKind.ConsiderEverything);
        }

        internal bool Equals(AnonymousTypeDescriptor other, TypeCompareKind comparison)
        {
            if (Key != other.Key)
            {
                return false;
            }
            ImmutableArray<AnonymousTypeField> fields = Fields;
            int length = fields.Length;
            ImmutableArray<AnonymousTypeField> fields2 = other.Fields;
            for (int i = 0; i < length; i++)
            {
                if (!fields[i].TypeWithAnnotations.Equals(fields2[i].TypeWithAnnotations, comparison))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is AnonymousTypeDescriptor)
            {
                return Equals((AnonymousTypeDescriptor)obj, TypeCompareKind.ConsiderEverything);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        internal AnonymousTypeDescriptor WithNewFieldsTypes(ImmutableArray<TypeWithAnnotations> newFieldTypes)
        {
            return new AnonymousTypeDescriptor(Fields.SelectAsArray((AnonymousTypeField field, int i, ImmutableArray<TypeWithAnnotations> types) => new AnonymousTypeField(field.Name, field.Location, types[i]), newFieldTypes), Location);
        }
    }
}
