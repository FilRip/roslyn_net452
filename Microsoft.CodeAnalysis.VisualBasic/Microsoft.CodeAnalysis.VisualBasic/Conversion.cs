using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct Conversion : IEquatable<Conversion>, IConvertibleConversion
	{
		private readonly ConversionKind _convKind;

		private readonly MethodSymbol _method;

		internal ConversionKind Kind => _convKind;

		public bool Exists => !Conversions.NoConversion(_convKind);

		public bool IsNarrowing => Conversions.IsNarrowingConversion(_convKind);

		public bool IsWidening => Conversions.IsWideningConversion(_convKind);

		public bool IsIdentity => Conversions.IsIdentityConversion(_convKind);

		public bool IsDefault => (_convKind & ConversionKind.WideningNothingLiteral) == ConversionKind.WideningNothingLiteral;

		public bool IsNumeric => (_convKind & ConversionKind.Numeric) != 0;

		public bool IsBoolean => (_convKind & ConversionKind.Boolean) != 0;

		public bool IsReference => (_convKind & ConversionKind.Reference) != 0;

		public bool IsAnonymousDelegate => (_convKind & ConversionKind.AnonymousDelegate) != 0;

		public bool IsLambda => (_convKind & ConversionKind.Lambda) != 0;

		public bool IsArray => (_convKind & ConversionKind.Array) != 0;

		public bool IsValueType => (_convKind & ConversionKind.Value) != 0;

		public bool IsNullableValueType => (_convKind & ConversionKind.Nullable) != 0;

		public bool IsString => (_convKind & ConversionKind.String) != 0;

		public bool IsTypeParameter => (_convKind & ConversionKind.TypeParameter) != 0;

		public bool IsUserDefined => (_convKind & ConversionKind.UserDefined) != 0;

		internal MethodSymbol Method => _method;

		public IMethodSymbol MethodSymbol => _method;

		internal Conversion(KeyValuePair<ConversionKind, MethodSymbol> conv)
		{
			this = default(Conversion);
			_convKind = conv.Key;
			_method = conv.Value;
		}

		public static bool operator ==(Conversion left, Conversion right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Conversion left, Conversion right)
		{
			return !(left == right);
		}

		public CommonConversion ToCommonConversion()
		{
			return new CommonConversion(Exists, IsIdentity, IsNumeric, IsReference, IsWidening, IsNullableValueType, MethodSymbol);
		}

		CommonConversion IConvertibleConversion.ToCommonConversion()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ToCommonConversion
			return this.ToCommonConversion();
		}

		public override bool Equals(object obj)
		{
			if (obj is Conversion)
			{
				return this == (Conversion)obj;
			}
			return false;
		}

		public bool Equals(Conversion other)
		{
			if (_convKind == other._convKind)
			{
				return Method == other.Method;
			}
			return false;
		}

		bool IEquatable<Conversion>.Equals(Conversion other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(Method, (int)_convKind);
		}

		public override string ToString()
		{
			ConversionKind convKind = _convKind;
			return convKind.ToString();
		}
	}
}
