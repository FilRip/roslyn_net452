using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct TypeInfo : IEquatable<TypeInfo>
    {
        internal static readonly TypeInfo None = new TypeInfo(null, null, default(NullabilityInfo), default(NullabilityInfo));

        public ITypeSymbol? Type { get; }

        public NullabilityInfo Nullability { get; }

        public ITypeSymbol? ConvertedType { get; }

        public NullabilityInfo ConvertedNullability { get; }

        public TypeInfo(ITypeSymbol? type, ITypeSymbol? convertedType, NullabilityInfo nullability, NullabilityInfo convertedNullability)
        {
            this = default(TypeInfo);
            Type = type;
            Nullability = nullability;
            ConvertedType = convertedType;
            ConvertedNullability = convertedNullability;
        }

        public bool Equals(TypeInfo other)
        {
            if (object.Equals(Type, other.Type) && object.Equals(ConvertedType, other.ConvertedType) && Nullability.Equals(other.Nullability))
            {
                return ConvertedNullability.Equals(other.ConvertedNullability);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypeInfo)
            {
                return Equals((TypeInfo)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(ConvertedType, Hash.Combine(Type, Hash.Combine(Nullability.GetHashCode(), ConvertedNullability.GetHashCode())));
        }
    }
}
