using System;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class CompileTimeCalculations
	{
		internal static long UncheckedCLng(ulong v)
		{
			return (long)v;
		}

		internal static long UncheckedCLng(double v)
		{
			return (long)Math.Round(v);
		}

		internal static ulong UncheckedCULng(long v)
		{
			return (ulong)v;
		}

		internal static ulong UncheckedCULng(int v)
		{
			return (ulong)v;
		}

		internal static ulong UncheckedCULng(double v)
		{
			return (ulong)Math.Round(v);
		}

		internal static int UncheckedCInt(ulong v)
		{
			return (int)v;
		}

		internal static int UncheckedCInt(long v)
		{
			return (int)v;
		}

		internal static uint UncheckedCUInt(ulong v)
		{
			return (uint)v;
		}

		internal static uint UncheckedCUInt(long v)
		{
			return (uint)v;
		}

		internal static uint UncheckedCUInt(int v)
		{
			return (uint)v;
		}

		internal static short UncheckedCShort(ulong v)
		{
			return (short)v;
		}

		internal static short UncheckedCShort(long v)
		{
			return (short)v;
		}

		internal static short UncheckedCShort(int v)
		{
			return (short)v;
		}

		internal static short UncheckedCShort(ushort v)
		{
			return (short)v;
		}

		internal static int UncheckedCInt(uint v)
		{
			return (int)v;
		}

		internal static short UncheckedCShort(uint v)
		{
			return (short)v;
		}

		internal static ushort UncheckedCUShort(short v)
		{
			return (ushort)v;
		}

		internal static ushort UncheckedCUShort(int v)
		{
			return (ushort)v;
		}

		internal static ushort UncheckedCUShort(long v)
		{
			return (ushort)v;
		}

		internal static byte UncheckedCByte(sbyte v)
		{
			return (byte)v;
		}

		internal static byte UncheckedCByte(int v)
		{
			return (byte)v;
		}

		internal static byte UncheckedCByte(long v)
		{
			return (byte)v;
		}

		internal static byte UncheckedCByte(ushort v)
		{
			return (byte)v;
		}

		internal static sbyte UncheckedCSByte(byte v)
		{
			return (sbyte)v;
		}

		internal static sbyte UncheckedCSByte(int v)
		{
			return (sbyte)v;
		}

		internal static sbyte UncheckedCSByte(long v)
		{
			return (sbyte)v;
		}

		internal static int UncheckedMul(int x, int y)
		{
			return x * y;
		}

		internal static long UncheckedMul(long x, long y)
		{
			return x * y;
		}

		internal static long UncheckedIntegralDiv(long x, long y)
		{
			if (y == -1)
			{
				return UncheckedNegate(x);
			}
			return x / y;
		}

		private static int UncheckedAdd(int x, int y)
		{
			return x + y;
		}

		private static long UncheckedAdd(long x, long y)
		{
			return x + y;
		}

		private static ulong UncheckedAdd(ulong x, ulong y)
		{
			return x + y;
		}

		private static long UncheckedSub(long x, long y)
		{
			return x - y;
		}

		private static uint UncheckedSub(uint x, uint y)
		{
			return x - y;
		}

		private static long UncheckedNegate(long x)
		{
			return -x;
		}

		internal static long GetConstantValueAsInt64(ref ConstantValue value)
		{
			return value.Discriminator switch
			{
				ConstantValueTypeDiscriminator.SByte => value.SByteValue, 
				ConstantValueTypeDiscriminator.Byte => value.ByteValue, 
				ConstantValueTypeDiscriminator.Int16 => value.Int16Value, 
				ConstantValueTypeDiscriminator.UInt16 => value.UInt16Value, 
				ConstantValueTypeDiscriminator.Int32 => value.Int32Value, 
				ConstantValueTypeDiscriminator.UInt32 => value.UInt32Value, 
				ConstantValueTypeDiscriminator.Int64 => value.Int64Value, 
				ConstantValueTypeDiscriminator.UInt64 => UncheckedCLng(value.UInt64Value), 
				ConstantValueTypeDiscriminator.Char => (int)value.CharValue, 
				ConstantValueTypeDiscriminator.Boolean => value.BooleanValue ? 1 : 0, 
				ConstantValueTypeDiscriminator.DateTime => value.DateTimeValue.Ticks, 
				_ => throw ExceptionUtilities.UnexpectedValue(value.Discriminator), 
			};
		}

		internal static ConstantValue GetConstantValue(ConstantValueTypeDiscriminator type, long value)
		{
			return type switch
			{
				ConstantValueTypeDiscriminator.SByte => ConstantValue.Create(UncheckedCSByte(value)), 
				ConstantValueTypeDiscriminator.Byte => ConstantValue.Create(UncheckedCByte(value)), 
				ConstantValueTypeDiscriminator.Int16 => ConstantValue.Create(UncheckedCShort(value)), 
				ConstantValueTypeDiscriminator.UInt16 => ConstantValue.Create(UncheckedCUShort(value)), 
				ConstantValueTypeDiscriminator.Int32 => ConstantValue.Create(UncheckedCInt(value)), 
				ConstantValueTypeDiscriminator.UInt32 => ConstantValue.Create(UncheckedCUInt(value)), 
				ConstantValueTypeDiscriminator.Int64 => ConstantValue.Create(value), 
				ConstantValueTypeDiscriminator.UInt64 => ConstantValue.Create(UncheckedCULng(value)), 
				ConstantValueTypeDiscriminator.Char => ConstantValue.Create(Strings.ChrW(UncheckedCInt(value))), 
				ConstantValueTypeDiscriminator.Boolean => ConstantValue.Create(value != 0L), 
				ConstantValueTypeDiscriminator.DateTime => ConstantValue.Create(new DateTime(value)), 
				_ => throw ExceptionUtilities.UnexpectedValue(type), 
			};
		}

		internal static long NarrowIntegralResult(long sourceValue, ConstantValueTypeDiscriminator sourceType, ConstantValueTypeDiscriminator resultType, ref bool overflow)
		{
			long num = 0L;
			switch (resultType)
			{
			case ConstantValueTypeDiscriminator.Boolean:
				return (sourceValue != 0L) ? 1 : 0;
			case ConstantValueTypeDiscriminator.SByte:
				num = UncheckedCSByte(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.Byte:
				num = UncheckedCByte(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.Int16:
				num = UncheckedCShort(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.UInt16:
				num = UncheckedCUShort(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.Int32:
				num = UncheckedCInt(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.UInt32:
				num = UncheckedCUInt(sourceValue);
				break;
			case ConstantValueTypeDiscriminator.Int64:
				num = sourceValue;
				break;
			case ConstantValueTypeDiscriminator.UInt64:
				num = sourceValue;
				break;
			case ConstantValueTypeDiscriminator.Char:
				num = UncheckedCUShort(sourceValue);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(resultType);
			}
			if (!ConstantValue.IsBooleanType(sourceType) && (ConstantValue.IsUnsignedIntegralType(sourceType) ^ ConstantValue.IsUnsignedIntegralType(resultType)))
			{
				if (!ConstantValue.IsUnsignedIntegralType(sourceType))
				{
					if (sourceValue < 0)
					{
						overflow = true;
					}
				}
				else if (num < 0)
				{
					overflow = true;
				}
			}
			if (num != sourceValue)
			{
				overflow = true;
			}
			return num;
		}

		internal static long NarrowIntegralResult(long sourceValue, SpecialType sourceType, SpecialType resultType, ref bool overflow)
		{
			return NarrowIntegralResult(sourceValue, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToConstantValueDiscriminator(sourceType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToConstantValueDiscriminator(resultType), ref overflow);
		}

		internal static long NarrowIntegralResult(long sourceValue, TypeSymbol sourceType, TypeSymbol resultType, ref bool overflow)
		{
			return NarrowIntegralResult(sourceValue, TypeSymbolExtensions.GetConstantValueTypeDiscriminator(sourceType), TypeSymbolExtensions.GetConstantValueTypeDiscriminator(resultType), ref overflow);
		}

		internal static ConstantValue ConvertIntegralValue(long sourceValue, ConstantValueTypeDiscriminator sourceType, ConstantValueTypeDiscriminator targetType, ref bool integerOverflow)
		{
			if (ConstantValue.IsIntegralType(targetType) || ConstantValue.IsBooleanType(targetType) || ConstantValue.IsCharType(targetType))
			{
				return GetConstantValue(targetType, NarrowIntegralResult(sourceValue, sourceType, targetType, ref integerOverflow));
			}
			if (ConstantValue.IsStringType(targetType) && ConstantValue.IsCharType(sourceType))
			{
				return ConstantValue.Create(new string(Strings.ChrW(UncheckedCInt(sourceValue)), 1));
			}
			if (ConstantValue.IsFloatingType(targetType))
			{
				return ConvertFloatingValue(ConstantValue.IsUnsignedIntegralType(sourceType) ? ((double)UncheckedCULng(sourceValue)) : ((double)sourceValue), targetType, ref integerOverflow);
			}
			if (ConstantValue.IsDecimalType(targetType))
			{
				bool isNegative;
				if (!ConstantValue.IsUnsignedIntegralType(sourceType) && sourceValue < 0)
				{
					isNegative = true;
					sourceValue = -sourceValue;
				}
				else
				{
					isNegative = false;
				}
				int lo = UncheckedCInt(sourceValue & -1);
				int mid = UncheckedCInt((sourceValue & -4294967296L) >> 32);
				int hi = 0;
				byte scale = 0;
				return ConvertDecimalValue(new decimal(lo, mid, hi, isNegative, scale), targetType, ref integerOverflow);
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal static ConstantValue ConvertFloatingValue(double sourceValue, ConstantValueTypeDiscriminator targetType, ref bool integerOverflow)
		{
			bool overflow = false;
			if (ConstantValue.IsBooleanType(targetType))
			{
				return ConvertIntegralValue((sourceValue != 0.0) ? 1 : 0, ConstantValueTypeDiscriminator.Int64, targetType, ref integerOverflow);
			}
			if (ConstantValue.IsIntegralType(targetType) || ConstantValue.IsCharType(targetType))
			{
				overflow = DetectFloatingToIntegralOverflow(sourceValue, ConstantValue.IsUnsignedIntegralType(targetType));
				if (!overflow)
				{
					double num = sourceValue + 0.5;
					double num2 = Math.Floor(num);
					long sourceValue2 = ((num2 == num && Math.IEEERemainder(num, 2.0) != 0.0) ? (IsUnsignedLongType(targetType) ? ConvertFloatingToUI64(num2 - 1.0) : UncheckedCLng(num2 - 1.0)) : (IsUnsignedLongType(targetType) ? ConvertFloatingToUI64(num2) : UncheckedCLng(num2)));
					ConstantValueTypeDiscriminator sourceType = ((!(sourceValue < 0.0)) ? ConstantValueTypeDiscriminator.UInt64 : ConstantValueTypeDiscriminator.Int64);
					return ConvertIntegralValue(sourceValue2, sourceType, targetType, ref integerOverflow);
				}
			}
			if (ConstantValue.IsFloatingType(targetType))
			{
				double num3 = NarrowFloatingResult(sourceValue, targetType, ref overflow);
				if (targetType == ConstantValueTypeDiscriminator.Single)
				{
					return ConstantValue.Create((float)num3);
				}
				return ConstantValue.Create(num3);
			}
			if (ConstantValue.IsDecimalType(targetType))
			{
				decimal sourceValue3 = default(decimal);
				try
				{
					sourceValue3 = Convert.ToDecimal(sourceValue);
				}
				catch (OverflowException ex)
				{
					ProjectData.SetProjectError(ex);
					OverflowException ex2 = ex;
					overflow = true;
					ProjectData.ClearProjectError();
				}
				if (!overflow)
				{
					return ConvertDecimalValue(sourceValue3, targetType, ref integerOverflow);
				}
			}
			if (overflow)
			{
				return ConstantValue.Bad;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal static ConstantValue ConvertDecimalValue(decimal sourceValue, ConstantValueTypeDiscriminator targetType, ref bool integerOverflow)
		{
			bool flag = false;
			if (ConstantValue.IsIntegralType(targetType) || ConstantValue.IsCharType(targetType))
			{
				sourceValue.GetBits(out var isNegative, out var scale, out var low, out var mid, out var high);
				if (scale != 0)
				{
					return ConvertFloatingValue(decimal.ToDouble(sourceValue), targetType, ref integerOverflow);
				}
				flag = (ulong)high != 0;
				if (!flag)
				{
					long num = (long)(((ulong)mid << 32) | low);
					ConstantValueTypeDiscriminator sourceType = ConstantValueTypeDiscriminator.Nothing;
					if (isNegative)
					{
						if (ConstantValue.IsUnsignedIntegralType(targetType) || UncheckedCULng(num) > 9223372036854775808uL)
						{
							flag = true;
						}
						else
						{
							num = UncheckedNegate(num);
							sourceType = ConstantValueTypeDiscriminator.Int64;
						}
					}
					else
					{
						sourceType = ConstantValueTypeDiscriminator.UInt64;
					}
					if (!flag)
					{
						return ConvertIntegralValue(num, sourceType, targetType, ref integerOverflow);
					}
				}
			}
			if (ConstantValue.IsFloatingType(targetType) || ConstantValue.IsBooleanType(targetType))
			{
				return ConvertFloatingValue(decimal.ToDouble(sourceValue), targetType, ref integerOverflow);
			}
			if (ConstantValue.IsDecimalType(targetType))
			{
				return ConstantValue.Create(sourceValue);
			}
			if (flag)
			{
				return ConstantValue.Bad;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal static double NarrowFloatingResult(double value, ConstantValueTypeDiscriminator resultType, ref bool overflow)
		{
			if (double.IsNaN(value))
			{
				overflow = true;
			}
			switch (resultType)
			{
			case ConstantValueTypeDiscriminator.Double:
				return value;
			case ConstantValueTypeDiscriminator.Single:
				if (value > 3.4028234663852886E+38 || value < -3.4028234663852886E+38)
				{
					overflow = true;
				}
				return (float)value;
			default:
				throw ExceptionUtilities.UnexpectedValue(resultType);
			}
		}

		internal static double NarrowFloatingResult(double value, SpecialType resultType, ref bool overflow)
		{
			if (double.IsNaN(value))
			{
				overflow = true;
			}
			switch (resultType)
			{
			case SpecialType.System_Double:
				return value;
			case SpecialType.System_Single:
				if (value > 3.4028234663852886E+38 || value < -3.4028234663852886E+38)
				{
					overflow = true;
				}
				return (float)value;
			default:
				throw ExceptionUtilities.UnexpectedValue(resultType);
			}
		}

		private static bool DetectFloatingToIntegralOverflow(double sourceValue, bool isUnsigned)
		{
			if (isUnsigned)
			{
				if (sourceValue < 1.7293822569102705E+19)
				{
					if (sourceValue > -1.0)
					{
						return false;
					}
				}
				else
				{
					double num = sourceValue - 1.7293822569102705E+19;
					if (num < 8.0704505322479288E+18 && UncheckedCLng(num) < 1152921504606846976L)
					{
						return false;
					}
				}
			}
			else if (sourceValue < -8.0704505322479288E+18)
			{
				double num2 = sourceValue - -8.0704505322479288E+18;
				if (num2 > -8.0704505322479288E+18 && UncheckedCLng(num2) > -1152921504606846977L)
				{
					return false;
				}
			}
			else
			{
				if (!(sourceValue > 8.0704505322479288E+18))
				{
					return false;
				}
				double num3 = sourceValue - 8.0704505322479288E+18;
				if (num3 < 8.0704505322479288E+18 && UncheckedCLng(num3) > 1152921504606846976L)
				{
					return false;
				}
			}
			return true;
		}

		private static long ConvertFloatingToUI64(double sourceValue)
		{
			double num = 9.2233720368547758E+18;
			if (sourceValue < num)
			{
				return UncheckedCLng(sourceValue);
			}
			return UncheckedAdd(UncheckedCLng(sourceValue - num), long.MinValue);
		}

		private static bool IsUnsignedLongType(ConstantValueTypeDiscriminator type)
		{
			return type == ConstantValueTypeDiscriminator.UInt64;
		}

		internal static bool TypeAllowsCompileTimeConversions(ConstantValueTypeDiscriminator type)
		{
			return TypeAllowsCompileTimeOperations(type);
		}

		internal static bool TypeAllowsCompileTimeOperations(ConstantValueTypeDiscriminator type)
		{
			ConstantValueTypeDiscriminator constantValueTypeDiscriminator = type;
			if (constantValueTypeDiscriminator - 2 <= ConstantValueTypeDiscriminator.UInt32 || constantValueTypeDiscriminator - 12 <= ConstantValueTypeDiscriminator.Int32)
			{
				return true;
			}
			return false;
		}

		internal static ConstantValue AdjustConstantValueFromMetadata(ConstantValue value, TypeSymbol targetType, bool isByRefParamValue)
		{
			if ((object)targetType == null || TypeSymbolExtensions.IsErrorType(targetType))
			{
				return value;
			}
			switch (value.Discriminator)
			{
			case ConstantValueTypeDiscriminator.Int32:
				if (!TypeSymbolExtensions.IsIntrinsicType(targetType) && !TypeSymbolExtensions.IsEnumType(targetType) && !TypeSymbolExtensions.IsObjectType(targetType) && (!TypeSymbolExtensions.IsNullableType(targetType) || !TypeSymbolExtensions.IsIntrinsicType(TypeSymbolExtensions.GetNullableUnderlyingType(targetType))) && (isByRefParamValue || (!TypeSymbolExtensions.IsTypeParameter(targetType) && !TypeSymbolExtensions.IsArrayType(targetType) && targetType.IsReferenceType)))
				{
					value = ((value.Int32Value != 0) ? ConstantValue.Bad : ConstantValue.Nothing);
				}
				break;
			case ConstantValueTypeDiscriminator.Int64:
				if (TypeSymbolExtensions.IsDateTimeType(targetType))
				{
					value = ConstantValue.Create(new DateTime(value.Int64Value));
				}
				break;
			}
			return value;
		}

		internal static long Multiply(long leftValue, long rightValue, SpecialType sourceType, SpecialType resultType, ref bool integerOverflow)
		{
			return Multiply(leftValue, rightValue, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToConstantValueDiscriminator(sourceType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToConstantValueDiscriminator(resultType), ref integerOverflow);
		}

		internal static long Multiply(long leftValue, long rightValue, ConstantValueTypeDiscriminator sourceType, ConstantValueTypeDiscriminator resultType, ref bool integerOverflow)
		{
			long num = NarrowIntegralResult(UncheckedMul(leftValue, rightValue), sourceType, resultType, ref integerOverflow);
			if (ConstantValue.IsUnsignedIntegralType(resultType))
			{
				if (rightValue != 0L && (double)UncheckedCULng(num) / (double)UncheckedCULng(rightValue) != (double)UncheckedCULng(leftValue))
				{
					integerOverflow = true;
				}
			}
			else if ((leftValue > 0 && rightValue > 0 && num <= 0) || (leftValue < 0 && rightValue < 0 && num <= 0) || (leftValue > 0 && rightValue < 0 && num >= 0) || (leftValue < 0 && rightValue > 0 && num >= 0) || (rightValue != 0L && (double)num / (double)rightValue != (double)leftValue))
			{
				integerOverflow = true;
			}
			return num;
		}
	}
}
