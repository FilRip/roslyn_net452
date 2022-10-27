// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public static class SpecialTypeExtensions
    {
        /// <summary>
        /// Checks if a type is considered a "built-in integral" by CLR.
        /// </summary>
        public static bool IsClrInteger(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr => true,
                _ => false,
            };
        }

        /// <summary>
        /// Checks if a type is a primitive of a fixed size.
        /// </summary>
        public static bool IsBlittable(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double => true,
                _ => false,
            };
        }

        public static bool IsValueType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Void or SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal or SpecialType.System_IntPtr or SpecialType.System_UIntPtr or SpecialType.System_Nullable_T or SpecialType.System_DateTime or SpecialType.System_TypedReference or SpecialType.System_ArgIterator or SpecialType.System_RuntimeArgumentHandle or SpecialType.System_RuntimeFieldHandle or SpecialType.System_RuntimeMethodHandle or SpecialType.System_RuntimeTypeHandle => true,
                _ => false,
            };
        }

        public static int SizeInBytes(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_SByte => sizeof(sbyte),
                SpecialType.System_Byte => sizeof(byte),
                SpecialType.System_Int16 => sizeof(short),
                SpecialType.System_UInt16 => sizeof(ushort),
                SpecialType.System_Int32 => sizeof(int),
                SpecialType.System_UInt32 => sizeof(uint),
                SpecialType.System_Int64 => sizeof(long),
                SpecialType.System_UInt64 => sizeof(ulong),
                SpecialType.System_Char => sizeof(char),
                SpecialType.System_Single => sizeof(float),
                SpecialType.System_Double => sizeof(double),
                SpecialType.System_Boolean => sizeof(bool),
                SpecialType.System_Decimal => sizeof(decimal),//This isn't in the spec, but it is handled by dev10
                _ => 0,// invalid
            };
        }

        /// <summary>
        /// These special types are structs that contain fields of the same type
        /// (e.g. System.Int32 contains a field of type System.Int32).
        /// </summary>
        public static bool IsPrimitiveRecursiveStruct(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Boolean or SpecialType.System_Byte or SpecialType.System_Char or SpecialType.System_Double or SpecialType.System_Int16 or SpecialType.System_Int32 or SpecialType.System_Int64 or SpecialType.System_UInt16 or SpecialType.System_UInt32 or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr or SpecialType.System_SByte or SpecialType.System_Single => true,
                _ => false,
            };
        }

        public static bool IsValidEnumUnderlyingType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 => true,
                _ => false,
            };
        }

        public static bool IsNumericType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal => true,
                _ => false,
            };
        }

        /// <summary>
        /// Checks if a type is considered a "built-in integral" by CLR.
        /// </summary>
        public static bool IsIntegralType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 => true,
                _ => false,
            };
        }

        public static bool IsUnsignedIntegralType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Byte or SpecialType.System_UInt16 or SpecialType.System_UInt32 or SpecialType.System_UInt64 => true,
                _ => false,
            };
        }

        public static bool IsSignedIntegralType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_Int32 or SpecialType.System_Int64 => true,
                _ => false,
            };
        }

        /// <summary>
        /// For signed integer types return number of bits for their representation minus 1. 
        /// I.e. 7 for Int8, 31 for Int32, etc.
        /// Used for checking loop end condition for VB for loop.
        /// </summary>
        public static int VBForToShiftBits(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_SByte => 7,
                SpecialType.System_Int16 => 15,
                SpecialType.System_Int32 => 31,
                SpecialType.System_Int64 => 63,
                _ => throw ExceptionUtilities.UnexpectedValue(specialType),
            };
        }

        public static SpecialType FromRuntimeTypeOfLiteralValue(object value)
        {
            RoslynDebug.Assert(value != null);

            // Perf: Note that JIT optimizes each expression val.GetType() == typeof(T) to a single register comparison.
            // Also the checks are sorted by commonality of the checked types.

            if (value.GetType() == typeof(int))
            {
                return SpecialType.System_Int32;
            }

            if (value.GetType() == typeof(string))
            {
                return SpecialType.System_String;
            }

            if (value.GetType() == typeof(bool))
            {
                return SpecialType.System_Boolean;
            }

            if (value.GetType() == typeof(char))
            {
                return SpecialType.System_Char;
            }

            if (value.GetType() == typeof(long))
            {
                return SpecialType.System_Int64;
            }

            if (value.GetType() == typeof(double))
            {
                return SpecialType.System_Double;
            }

            if (value.GetType() == typeof(uint))
            {
                return SpecialType.System_UInt32;
            }

            if (value.GetType() == typeof(ulong))
            {
                return SpecialType.System_UInt64;
            }

            if (value.GetType() == typeof(float))
            {
                return SpecialType.System_Single;
            }

            if (value.GetType() == typeof(decimal))
            {
                return SpecialType.System_Decimal;
            }

            if (value.GetType() == typeof(short))
            {
                return SpecialType.System_Int16;
            }

            if (value.GetType() == typeof(ushort))
            {
                return SpecialType.System_UInt16;
            }

            if (value.GetType() == typeof(DateTime))
            {
                return SpecialType.System_DateTime;
            }

            if (value.GetType() == typeof(byte))
            {
                return SpecialType.System_Byte;
            }

            if (value.GetType() == typeof(sbyte))
            {
                return SpecialType.System_SByte;
            }

            return SpecialType.None;
        }
    }
}
