using System;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal readonly struct CSharpTypeInfo : IEquatable<CSharpTypeInfo>
    {
        internal static readonly CSharpTypeInfo None = new CSharpTypeInfo(null, null, default(NullabilityInfo), default(NullabilityInfo), Conversion.Identity);

        public readonly TypeSymbol Type;

        public readonly NullabilityInfo Nullability;

        public readonly TypeSymbol ConvertedType;

        public readonly NullabilityInfo ConvertedNullability;

        public readonly Conversion ImplicitConversion;

        internal CSharpTypeInfo(TypeSymbol type, TypeSymbol convertedType, NullabilityInfo nullability, NullabilityInfo convertedNullability, Conversion implicitConversion)
        {
            Type = type.GetNonErrorGuess() ?? type;
            ConvertedType = convertedType.GetNonErrorGuess() ?? convertedType;
            Nullability = nullability;
            ConvertedNullability = convertedNullability;
            ImplicitConversion = implicitConversion;
        }

        public static implicit operator TypeInfo(CSharpTypeInfo info)
        {
            return new TypeInfo(info.Type?.GetITypeSymbol(info.Nullability.FlowState.ToAnnotation()), info.ConvertedType?.GetITypeSymbol(info.ConvertedNullability.FlowState.ToAnnotation()), info.Nullability, info.ConvertedNullability);
        }

        public override bool Equals(object obj)
        {
            if (obj is CSharpTypeInfo)
            {
                return Equals((CSharpTypeInfo)obj);
            }
            return false;
        }

        public bool Equals(CSharpTypeInfo other)
        {
            if (ImplicitConversion.Equals(other.ImplicitConversion) && TypeSymbol.Equals(Type, other.Type, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(ConvertedType, other.ConvertedType, TypeCompareKind.ConsiderEverything) && Nullability.Equals(other.Nullability))
            {
                return ConvertedNullability.Equals(other.ConvertedNullability);
            }
            return false;
        }

        public override int GetHashCode()
        {
            TypeSymbol convertedType = ConvertedType;
            TypeSymbol type = Type;
            int hashCode = Nullability.GetHashCode();
            int hashCode2 = ConvertedNullability.GetHashCode();
            Conversion implicitConversion = ImplicitConversion;
            return Hash.Combine(convertedType, Hash.Combine(type, Hash.Combine(hashCode, Hash.Combine(hashCode2, implicitConversion.GetHashCode()))));
        }
    }
}
