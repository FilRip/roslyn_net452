// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public static class PrimitiveTypeCodeExtensions
    {
        public static bool Is64BitIntegral(this Cci.PrimitiveTypeCode kind)
        {
            return kind switch
            {
                Cci.PrimitiveTypeCode.Int64 or Cci.PrimitiveTypeCode.UInt64 => true,
                _ => false,
            };
        }

        public static bool IsSigned(this Cci.PrimitiveTypeCode kind)
        {
            return kind switch
            {
                Cci.PrimitiveTypeCode.Int8 or Cci.PrimitiveTypeCode.Int16 or Cci.PrimitiveTypeCode.Int32 or Cci.PrimitiveTypeCode.Int64 or Cci.PrimitiveTypeCode.IntPtr or Cci.PrimitiveTypeCode.Float32 or Cci.PrimitiveTypeCode.Float64 => true,
                _ => false,
            };
        }

        public static bool IsUnsigned(this Cci.PrimitiveTypeCode kind)
        {
            return kind switch
            {
                Cci.PrimitiveTypeCode.UInt8 or Cci.PrimitiveTypeCode.UInt16 or Cci.PrimitiveTypeCode.UInt32 or Cci.PrimitiveTypeCode.UInt64 or Cci.PrimitiveTypeCode.UIntPtr or Cci.PrimitiveTypeCode.Char or Cci.PrimitiveTypeCode.Pointer or Cci.PrimitiveTypeCode.FunctionPointer => true,
                _ => false,
            };
        }

        public static bool IsFloatingPoint(this Cci.PrimitiveTypeCode kind)
        {
            return kind switch
            {
                Cci.PrimitiveTypeCode.Float32 or Cci.PrimitiveTypeCode.Float64 => true,
                _ => false,
            };
        }


        public static ConstantValueTypeDiscriminator GetConstantValueTypeDiscriminator(this Cci.PrimitiveTypeCode type)
        {
            return type switch
            {
                Cci.PrimitiveTypeCode.Int8 => ConstantValueTypeDiscriminator.SByte,
                Cci.PrimitiveTypeCode.UInt8 => ConstantValueTypeDiscriminator.Byte,
                Cci.PrimitiveTypeCode.Int16 => ConstantValueTypeDiscriminator.Int16,
                Cci.PrimitiveTypeCode.UInt16 => ConstantValueTypeDiscriminator.UInt16,
                Cci.PrimitiveTypeCode.Int32 => ConstantValueTypeDiscriminator.Int32,
                Cci.PrimitiveTypeCode.UInt32 => ConstantValueTypeDiscriminator.UInt32,
                Cci.PrimitiveTypeCode.Int64 => ConstantValueTypeDiscriminator.Int64,
                Cci.PrimitiveTypeCode.UInt64 => ConstantValueTypeDiscriminator.UInt64,
                Cci.PrimitiveTypeCode.Char => ConstantValueTypeDiscriminator.Char,
                Cci.PrimitiveTypeCode.Boolean => ConstantValueTypeDiscriminator.Boolean,
                Cci.PrimitiveTypeCode.Float32 => ConstantValueTypeDiscriminator.Single,
                Cci.PrimitiveTypeCode.Float64 => ConstantValueTypeDiscriminator.Double,
                Cci.PrimitiveTypeCode.String => ConstantValueTypeDiscriminator.String,
                _ => throw ExceptionUtilities.UnexpectedValue(type),
            };
        }
    }
}
