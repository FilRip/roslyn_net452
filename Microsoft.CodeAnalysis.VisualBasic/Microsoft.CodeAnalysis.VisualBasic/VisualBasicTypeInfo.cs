using System;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct VisualBasicTypeInfo : IEquatable<VisualBasicTypeInfo>
	{
		private readonly TypeSymbol _type;

		private readonly TypeSymbol _convertedType;

		private readonly Conversion _implicitConversion;

		internal static VisualBasicTypeInfo None = new VisualBasicTypeInfo(null, null, new Conversion(Conversions.Identity));

		public TypeSymbol Type => _type;

		public TypeSymbol ConvertedType => _convertedType;

		public Conversion ImplicitConversion => _implicitConversion;

		public static implicit operator TypeInfo(VisualBasicTypeInfo info)
		{
			return new TypeInfo(info.Type, info.ConvertedType, default(NullabilityInfo), default(NullabilityInfo));
		}

		internal VisualBasicTypeInfo(TypeSymbol type, TypeSymbol convertedType, Conversion implicitConversion)
		{
			this = default(VisualBasicTypeInfo);
			_type = GetPossibleGuessForErrorType(type);
			_convertedType = GetPossibleGuessForErrorType(convertedType);
			_implicitConversion = implicitConversion;
		}

		public bool Equals(VisualBasicTypeInfo other)
		{
			if (_implicitConversion.Equals(other._implicitConversion) && TypeSymbol.Equals(_type, other._type, TypeCompareKind.ConsiderEverything))
			{
				return TypeSymbol.Equals(_convertedType, other._convertedType, TypeCompareKind.ConsiderEverything);
			}
			return false;
		}

		bool IEquatable<VisualBasicTypeInfo>.Equals(VisualBasicTypeInfo other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override bool Equals(object obj)
		{
			if (obj is VisualBasicTypeInfo)
			{
				return Equals((VisualBasicTypeInfo)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			TypeSymbol convertedType = _convertedType;
			TypeSymbol type = _type;
			Conversion implicitConversion = _implicitConversion;
			return Hash.Combine(convertedType, Hash.Combine(type, implicitConversion.GetHashCode()));
		}

		private static TypeSymbol GetPossibleGuessForErrorType(TypeSymbol type)
		{
			if (!(type is ErrorTypeSymbol errorTypeSymbol))
			{
				return type;
			}
			NamedTypeSymbol nonErrorGuessType = errorTypeSymbol.NonErrorGuessType;
			if ((object)nonErrorGuessType == null)
			{
				return type;
			}
			return nonErrorGuessType;
		}
	}
}
