using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class SpecialTypeExtensions
	{
		public static bool IsFloatingType(this SpecialType @this)
		{
			SpecialType specialType = @this;
			if ((uint)(specialType - 18) <= 1u)
			{
				return true;
			}
			return false;
		}

		public static bool IsIntrinsicType(this SpecialType @this)
		{
			if (@this != SpecialType.System_String)
			{
				return IsIntrinsicValueType(@this);
			}
			return true;
		}

		public static bool IsIntrinsicValueType(this SpecialType @this)
		{
			SpecialType specialType = @this;
			if ((uint)(specialType - 7) <= 12u || specialType == SpecialType.System_DateTime)
			{
				return true;
			}
			return false;
		}

		public static bool IsPrimitiveType(this SpecialType @this)
		{
			switch (@this)
			{
			case SpecialType.System_Boolean:
			case SpecialType.System_Char:
			case SpecialType.System_SByte:
			case SpecialType.System_Byte:
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
			case SpecialType.System_Single:
			case SpecialType.System_Double:
			case SpecialType.System_IntPtr:
			case SpecialType.System_UIntPtr:
				return true;
			default:
				return false;
			}
		}

		public static bool IsStrictSupertypeOfConcreteDelegate(this SpecialType @this)
		{
			SpecialType specialType = @this;
			if (specialType == SpecialType.System_Object || (uint)(specialType - 3) <= 1u)
			{
				return true;
			}
			return false;
		}

		public static bool IsRestrictedType(this SpecialType @this)
		{
			SpecialType specialType = @this;
			if ((uint)(specialType - 36) <= 2u)
			{
				return true;
			}
			return false;
		}

		public static bool IsValidTypeForAttributeArgument(this SpecialType @this)
		{
			switch (@this)
			{
			case SpecialType.System_Object:
			case SpecialType.System_Boolean:
			case SpecialType.System_Char:
			case SpecialType.System_SByte:
			case SpecialType.System_Byte:
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
			case SpecialType.System_Single:
			case SpecialType.System_Double:
			case SpecialType.System_String:
				return true;
			default:
				return false;
			}
		}

		public static bool IsValidTypeForSwitchTable(this SpecialType @this)
		{
			SpecialType specialType = @this;
			if ((uint)(specialType - 7) <= 9u || specialType == SpecialType.System_String)
			{
				return true;
			}
			return false;
		}

		public static int? TypeToIndex(this SpecialType type)
		{
			int value;
			int? result;
			switch (type)
			{
			case SpecialType.System_Object:
				value = 0;
				goto IL_00b2;
			case SpecialType.System_String:
				value = 1;
				goto IL_00b2;
			case SpecialType.System_Boolean:
				value = 2;
				goto IL_00b2;
			case SpecialType.System_Char:
				value = 3;
				goto IL_00b2;
			case SpecialType.System_SByte:
				value = 4;
				goto IL_00b2;
			case SpecialType.System_Int16:
				value = 5;
				goto IL_00b2;
			case SpecialType.System_Int32:
				value = 6;
				goto IL_00b2;
			case SpecialType.System_Int64:
				value = 7;
				goto IL_00b2;
			case SpecialType.System_Byte:
				value = 8;
				goto IL_00b2;
			case SpecialType.System_UInt16:
				value = 9;
				goto IL_00b2;
			case SpecialType.System_UInt32:
				value = 10;
				goto IL_00b2;
			case SpecialType.System_UInt64:
				value = 11;
				goto IL_00b2;
			case SpecialType.System_Single:
				value = 12;
				goto IL_00b2;
			case SpecialType.System_Double:
				value = 13;
				goto IL_00b2;
			case SpecialType.System_Decimal:
				value = 14;
				goto IL_00b2;
			case SpecialType.System_DateTime:
				value = 15;
				goto IL_00b2;
			default:
				{
					result = null;
					break;
				}
				IL_00b2:
				result = value;
				break;
			}
			return result;
		}

		public static string GetNativeCompilerVType(this SpecialType @this)
		{
			switch (@this)
			{
			case SpecialType.System_Void:
				return "t_void";
			case SpecialType.System_Boolean:
				return "t_bool";
			case SpecialType.System_Char:
				return "t_char";
			case SpecialType.System_SByte:
				return "t_i1";
			case SpecialType.System_Byte:
				return "t_ui1";
			case SpecialType.System_Int16:
				return "t_i2";
			case SpecialType.System_UInt16:
				return "t_ui2";
			case SpecialType.System_Int32:
				return "t_i4";
			case SpecialType.System_UInt32:
				return "t_ui4";
			case SpecialType.System_Int64:
				return "t_i8";
			case SpecialType.System_UInt64:
				return "t_ui8";
			case SpecialType.System_Decimal:
				return "t_decimal";
			case SpecialType.System_Single:
				return "t_single";
			case SpecialType.System_Double:
				return "t_double";
			case SpecialType.System_String:
				return "t_string";
			case SpecialType.System_IntPtr:
			case SpecialType.System_UIntPtr:
				return "t_ptr";
			case SpecialType.System_Array:
				return "t_array";
			case SpecialType.System_DateTime:
				return "t_date";
			default:
				return null;
			}
		}

		public static string GetDisplayName(this SpecialType @this)
		{
			return TryGetKeywordText(@this);
		}

		public static string TryGetKeywordText(this SpecialType @this)
		{
			return @this switch
			{
				SpecialType.System_SByte => "SByte", 
				SpecialType.System_Int16 => "Short", 
				SpecialType.System_Int32 => "Integer", 
				SpecialType.System_Int64 => "Long", 
				SpecialType.System_Byte => "Byte", 
				SpecialType.System_UInt16 => "UShort", 
				SpecialType.System_UInt32 => "UInteger", 
				SpecialType.System_UInt64 => "ULong", 
				SpecialType.System_Single => "Single", 
				SpecialType.System_Double => "Double", 
				SpecialType.System_Decimal => "Decimal", 
				SpecialType.System_Char => "Char", 
				SpecialType.System_Boolean => "Boolean", 
				SpecialType.System_String => "String", 
				SpecialType.System_Object => "Object", 
				SpecialType.System_DateTime => "Date", 
				SpecialType.System_Void => "Void", 
				_ => null, 
			};
		}

		internal static ConstantValueTypeDiscriminator ToConstantValueDiscriminator(this SpecialType @this)
		{
			return @this switch
			{
				SpecialType.System_SByte => ConstantValueTypeDiscriminator.SByte, 
				SpecialType.System_Byte => ConstantValueTypeDiscriminator.Byte, 
				SpecialType.System_Int16 => ConstantValueTypeDiscriminator.Int16, 
				SpecialType.System_UInt16 => ConstantValueTypeDiscriminator.UInt16, 
				SpecialType.System_Int32 => ConstantValueTypeDiscriminator.Int32, 
				SpecialType.System_UInt32 => ConstantValueTypeDiscriminator.UInt32, 
				SpecialType.System_Int64 => ConstantValueTypeDiscriminator.Int64, 
				SpecialType.System_UInt64 => ConstantValueTypeDiscriminator.UInt64, 
				SpecialType.System_Char => ConstantValueTypeDiscriminator.Char, 
				SpecialType.System_Boolean => ConstantValueTypeDiscriminator.Boolean, 
				SpecialType.System_Single => ConstantValueTypeDiscriminator.Single, 
				SpecialType.System_Double => ConstantValueTypeDiscriminator.Double, 
				SpecialType.System_Decimal => ConstantValueTypeDiscriminator.Decimal, 
				SpecialType.System_DateTime => ConstantValueTypeDiscriminator.DateTime, 
				SpecialType.System_String => ConstantValueTypeDiscriminator.String, 
				_ => throw ExceptionUtilities.UnexpectedValue(@this), 
			};
		}

		internal static int GetShiftSizeMask(this SpecialType @this)
		{
			switch (@this)
			{
			case SpecialType.System_SByte:
			case SpecialType.System_Byte:
				return 7;
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
				return 15;
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
				return 31;
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
				return 63;
			default:
				throw ExceptionUtilities.UnexpectedValue(@this);
			}
		}

		public static Type ToRuntimeType(this SpecialType @this)
		{
			return @this switch
			{
				SpecialType.System_SByte => typeof(sbyte), 
				SpecialType.System_Int16 => typeof(short), 
				SpecialType.System_Int32 => typeof(int), 
				SpecialType.System_Int64 => typeof(long), 
				SpecialType.System_Byte => typeof(byte), 
				SpecialType.System_UInt16 => typeof(ushort), 
				SpecialType.System_UInt32 => typeof(uint), 
				SpecialType.System_UInt64 => typeof(ulong), 
				SpecialType.System_Single => typeof(float), 
				SpecialType.System_Double => typeof(double), 
				SpecialType.System_Decimal => typeof(decimal), 
				SpecialType.System_Char => typeof(char), 
				SpecialType.System_Boolean => typeof(bool), 
				SpecialType.System_String => typeof(string), 
				SpecialType.System_Object => typeof(object), 
				SpecialType.System_DateTime => typeof(DateTime), 
				SpecialType.System_Void => typeof(void), 
				_ => throw ExceptionUtilities.UnexpectedValue(@this), 
			};
		}
	}
}
