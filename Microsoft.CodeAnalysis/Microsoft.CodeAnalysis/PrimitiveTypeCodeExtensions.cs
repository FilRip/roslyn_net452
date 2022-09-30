using Microsoft.Cci;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public static class PrimitiveTypeCodeExtensions
    {
        public static bool Is64BitIntegral(this PrimitiveTypeCode kind)
        {
            if (kind == PrimitiveTypeCode.Int64 || kind == PrimitiveTypeCode.UInt64)
            {
                return true;
            }
            return false;
        }

        public static bool IsSigned(this PrimitiveTypeCode kind)
        {
            if ((uint)(kind - 2) <= 6u)
            {
                return true;
            }
            return false;
        }

        public static bool IsUnsigned(this PrimitiveTypeCode kind)
        {
            switch (kind)
            {
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.Pointer:
                case PrimitiveTypeCode.UInt8:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.UInt32:
                case PrimitiveTypeCode.UInt64:
                case PrimitiveTypeCode.UIntPtr:
                case PrimitiveTypeCode.FunctionPointer:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFloatingPoint(this PrimitiveTypeCode kind)
        {
            if ((uint)(kind - 3) <= 1u)
            {
                return true;
            }
            return false;
        }

        public static ConstantValueTypeDiscriminator GetConstantValueTypeDiscriminator(this PrimitiveTypeCode type)
        {
            return type switch
            {
                PrimitiveTypeCode.Int8 => ConstantValueTypeDiscriminator.SByte,
                PrimitiveTypeCode.UInt8 => ConstantValueTypeDiscriminator.Byte,
                PrimitiveTypeCode.Int16 => ConstantValueTypeDiscriminator.Int16,
                PrimitiveTypeCode.UInt16 => ConstantValueTypeDiscriminator.UInt16,
                PrimitiveTypeCode.Int32 => ConstantValueTypeDiscriminator.Int32,
                PrimitiveTypeCode.UInt32 => ConstantValueTypeDiscriminator.UInt32,
                PrimitiveTypeCode.Int64 => ConstantValueTypeDiscriminator.Int64,
                PrimitiveTypeCode.UInt64 => ConstantValueTypeDiscriminator.UInt64,
                PrimitiveTypeCode.Char => ConstantValueTypeDiscriminator.Char,
                PrimitiveTypeCode.Boolean => ConstantValueTypeDiscriminator.Boolean,
                PrimitiveTypeCode.Float32 => ConstantValueTypeDiscriminator.Single,
                PrimitiveTypeCode.Float64 => ConstantValueTypeDiscriminator.Double,
                PrimitiveTypeCode.String => ConstantValueTypeDiscriminator.String,
                _ => throw ExceptionUtilities.UnexpectedValue(type),
            };
        }
    }
}
