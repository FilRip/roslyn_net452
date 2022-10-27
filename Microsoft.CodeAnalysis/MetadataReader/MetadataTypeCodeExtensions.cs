// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Reflection.Metadata;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    internal static class MetadataTypeCodeExtensions
    {
        internal static SpecialType ToSpecialType(this SignatureTypeCode typeCode)
        {
            return typeCode switch
            {
                SignatureTypeCode.TypedReference => SpecialType.System_TypedReference,
                SignatureTypeCode.Void => SpecialType.System_Void,
                SignatureTypeCode.Boolean => SpecialType.System_Boolean,
                SignatureTypeCode.SByte => SpecialType.System_SByte,
                SignatureTypeCode.Byte => SpecialType.System_Byte,
                SignatureTypeCode.Int16 => SpecialType.System_Int16,
                SignatureTypeCode.UInt16 => SpecialType.System_UInt16,
                SignatureTypeCode.Int32 => SpecialType.System_Int32,
                SignatureTypeCode.UInt32 => SpecialType.System_UInt32,
                SignatureTypeCode.Int64 => SpecialType.System_Int64,
                SignatureTypeCode.UInt64 => SpecialType.System_UInt64,
                SignatureTypeCode.Single => SpecialType.System_Single,
                SignatureTypeCode.Double => SpecialType.System_Double,
                SignatureTypeCode.Char => SpecialType.System_Char,
                SignatureTypeCode.String => SpecialType.System_String,
                SignatureTypeCode.IntPtr => SpecialType.System_IntPtr,
                SignatureTypeCode.UIntPtr => SpecialType.System_UIntPtr,
                SignatureTypeCode.Object => SpecialType.System_Object,
                _ => throw ExceptionUtilities.UnexpectedValue(typeCode),
            };
        }

        internal static bool HasShortFormSignatureEncoding(this SpecialType type)
        {
            // Spec II.23.2.16: Short form signatures:
            // The following table shows which short-forms should be used in place of each long-form item. 
            // Long Form                             Short Form                     
            //   CLASS     System.String               ELEMENT_TYPE_STRING
            //   CLASS     System.Object               ELEMENT_TYPE_OBJECT
            //   VALUETYPE System.Void                 ELEMENT_TYPE_VOID
            //   VALUETYPE System.Boolean              ELEMENT_TYPE_BOOLEAN
            //   VALUETYPE System.Char                 ELEMENT_TYPE_CHAR
            //   VALUETYPE System.Byte                 ELEMENT_TYPE_U1
            //   VALUETYPE System.Sbyte                ELEMENT_TYPE_I1
            //   VALUETYPE System.Int16                ELEMENT_TYPE_I2
            //   VALUETYPE System.UInt16               ELEMENT_TYPE_U2
            //   VALUETYPE System.Int32                ELEMENT_TYPE_I4
            //   VALUETYPE System.UInt32               ELEMENT_TYPE_U4
            //   VALUETYPE System.Int64                ELEMENT_TYPE_I8
            //   VALUETYPE System.UInt64               ELEMENT_TYPE_U8
            //   VALUETYPE System.IntPtr               ELEMENT_TYPE_I
            //   VALUETYPE System.UIntPtr              ELEMENT_TYPE_U
            //   VALUETYPE System.TypedReference       ELEMENT_TYPE_TYPEDBYREF

            // The spec is missing:
            //   VALUETYPE System.Single               ELEMENT_TYPE_R4
            //   VALUETYPE System.Double               ELEMENT_TYPE_R8

            return type switch
            {
                SpecialType.System_String or SpecialType.System_Object or SpecialType.System_Void or SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_Byte or SpecialType.System_SByte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_IntPtr or SpecialType.System_UIntPtr or SpecialType.System_TypedReference or SpecialType.System_Single or SpecialType.System_Double => true,
                _ => false,
            };
        }

        internal static SerializationTypeCode ToSerializationType(this SpecialType specialType)
        {
            return specialType switch
            {
                SpecialType.System_Boolean => SerializationTypeCode.Boolean,
                SpecialType.System_SByte => SerializationTypeCode.SByte,
                SpecialType.System_Byte => SerializationTypeCode.Byte,
                SpecialType.System_Int16 => SerializationTypeCode.Int16,
                SpecialType.System_Int32 => SerializationTypeCode.Int32,
                SpecialType.System_Int64 => SerializationTypeCode.Int64,
                SpecialType.System_UInt16 => SerializationTypeCode.UInt16,
                SpecialType.System_UInt32 => SerializationTypeCode.UInt32,
                SpecialType.System_UInt64 => SerializationTypeCode.UInt64,
                SpecialType.System_Single => SerializationTypeCode.Single,
                SpecialType.System_Double => SerializationTypeCode.Double,
                SpecialType.System_Char => SerializationTypeCode.Char,
                SpecialType.System_String => SerializationTypeCode.String,
                SpecialType.System_Object => SerializationTypeCode.TaggedObject,
                _ => throw ExceptionUtilities.UnexpectedValue(specialType),
            };
        }
    }
}
