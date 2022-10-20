// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public struct AttributeDescription
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly byte[][] Signatures;

        // VB matches ExtensionAttribute name and namespace ignoring case (it's the only attribute that matches its name case-insensitively)
        public readonly bool MatchIgnoringCase;

        public AttributeDescription(string @namespace, string name, byte[][] signatures, bool matchIgnoringCase = false)
        {
            RoslynDebug.Assert(@namespace != null);
            RoslynDebug.Assert(name != null);
            RoslynDebug.Assert(signatures != null);

            this.Namespace = @namespace;
            this.Name = name;
            this.Signatures = signatures;
            this.MatchIgnoringCase = matchIgnoringCase;
        }

        public string FullName
        {
            get { return Namespace + "." + Name; }
        }

        public override string ToString()
        {
            return FullName + "(" + Signatures.Length + ")";
        }

        internal int GetParameterCount(int signatureIndex)
        {
            var signature = this.Signatures[signatureIndex];

            // only instance ctors are allowed:
            Debug.Assert(signature[0] == (byte)SignatureAttributes.Instance);

            // parameter count is the second element of the signature:
            return signature[1];
        }

        // shortcuts for signature elements supported by our signature comparer:
        private const byte Void = (byte)SignatureTypeCode.Void;
        private const byte Boolean = (byte)SignatureTypeCode.Boolean;
        private const byte Char = (byte)SignatureTypeCode.Char;
        private const byte SByte = (byte)SignatureTypeCode.SByte;
        private const byte Byte = (byte)SignatureTypeCode.Byte;
        private const byte Int16 = (byte)SignatureTypeCode.Int16;
        private const byte UInt16 = (byte)SignatureTypeCode.UInt16;
        private const byte Int32 = (byte)SignatureTypeCode.Int32;
        private const byte UInt32 = (byte)SignatureTypeCode.UInt32;
        private const byte Int64 = (byte)SignatureTypeCode.Int64;
        private const byte UInt64 = (byte)SignatureTypeCode.UInt64;
        private const byte Single = (byte)SignatureTypeCode.Single;
        private const byte Double = (byte)SignatureTypeCode.Double;
        private const byte String = (byte)SignatureTypeCode.String;
        private const byte Object = (byte)SignatureTypeCode.Object;
        private const byte SzArray = (byte)SignatureTypeCode.SZArray;
        private const byte TypeHandle = (byte)SignatureTypeCode.TypeHandle;

        internal enum TypeHandleTarget : byte
        {
            AttributeTargets,
            AssemblyNameFlags,
            MethodImplOptions,
            CharSet,
            LayoutKind,
            UnmanagedType,
            TypeLibTypeFlags,
            ClassInterfaceType,
            ComInterfaceType,
            CompilationRelaxations,
            DebuggingModes,
            SecurityCriticalScope,
            CallingConvention,
            AssemblyHashAlgorithm,
            TransactionOption,
            SecurityAction,
            SystemType,
            DeprecationType,
            Platform
        }

        public struct TypeHandleTargetInfo
        {
            public readonly string Namespace;
            public readonly string Name;
            public readonly SerializationTypeCode Underlying;

            public TypeHandleTargetInfo(string @namespace, string name, SerializationTypeCode underlying)
            {
                Namespace = @namespace;
                Name = name;
                Underlying = underlying;
            }
        }

        public static ImmutableArray<TypeHandleTargetInfo> TypeHandleTargets;

        static AttributeDescription()
        {
            const string system = "System";
            const string compilerServices = "System.Runtime.CompilerServices";
            const string interopServices = "System.Runtime.InteropServices";

            TypeHandleTargets = (new[] {
                 new TypeHandleTargetInfo(system,"AttributeTargets", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.Reflection","AssemblyNameFlags", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(compilerServices,"MethodImplOptions", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"CharSet", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"LayoutKind", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"UnmanagedType", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"TypeLibTypeFlags", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"ClassInterfaceType", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"ComInterfaceType", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(compilerServices,"CompilationRelaxations", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.Diagnostics.DebuggableAttribute","DebuggingModes", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.Security","SecurityCriticalScope", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(interopServices,"CallingConvention", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.Configuration.Assemblies","AssemblyHashAlgorithm", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.EnterpriseServices","TransactionOption", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("System.Security.Permissions","SecurityAction", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo(system,"Type", SerializationTypeCode.Type)
                ,new TypeHandleTargetInfo("Windows.Foundation.Metadata","DeprecationType", SerializationTypeCode.Int32)
                ,new TypeHandleTargetInfo("Windows.Foundation.Metadata","Platform", SerializationTypeCode.Int32)
            }).AsImmutable();
        }

        private static readonly byte[] s_signature_HasThis_Void = new byte[] { (byte)SignatureAttributes.Instance, 0, Void };
        private static readonly byte[] s_signature_HasThis_Void_Byte = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Byte };
        private static readonly byte[] s_signature_HasThis_Void_Int16 = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Int16 };
        private static readonly byte[] s_signature_HasThis_Void_Int32 = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Int32 };
        private static readonly byte[] s_signature_HasThis_Void_UInt32 = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, UInt32 };
        private static readonly byte[] s_signature_HasThis_Void_Int32_Int32 = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, Int32, Int32 };
        private static readonly byte[] s_signature_HasThis_Void_Int32_Int32_Int32_Int32 = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, Int32, Int32, Int32, Int32 };
        private static readonly byte[] s_signature_HasThis_Void_String = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, String };
        private static readonly byte[] s_signature_HasThis_Void_Object = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Object };
        private static readonly byte[] s_signature_HasThis_Void_String_String = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, String, String };
        private static readonly byte[] s_signature_HasThis_Void_String_Boolean = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, String, Boolean };
        private static readonly byte[] s_signature_HasThis_Void_String_String_String = new byte[] { (byte)SignatureAttributes.Instance, 3, Void, String, String, String };
        private static readonly byte[] s_signature_HasThis_Void_String_String_String_String = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, String, String, String, String };
        private static readonly byte[] s_signature_HasThis_Void_AttributeTargets = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.AttributeTargets };
        private static readonly byte[] s_signature_HasThis_Void_AssemblyNameFlags = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.AssemblyNameFlags };
        private static readonly byte[] s_signature_HasThis_Void_MethodImplOptions = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.MethodImplOptions };
        private static readonly byte[] s_signature_HasThis_Void_CharSet = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.CharSet };
        private static readonly byte[] s_signature_HasThis_Void_LayoutKind = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.LayoutKind };
        private static readonly byte[] s_signature_HasThis_Void_UnmanagedType = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.UnmanagedType };
        private static readonly byte[] s_signature_HasThis_Void_TypeLibTypeFlags = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.TypeLibTypeFlags };
        private static readonly byte[] s_signature_HasThis_Void_ClassInterfaceType = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.ClassInterfaceType };
        private static readonly byte[] s_signature_HasThis_Void_ComInterfaceType = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.ComInterfaceType };
        private static readonly byte[] s_signature_HasThis_Void_CompilationRelaxations = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.CompilationRelaxations };
        private static readonly byte[] s_signature_HasThis_Void_DebuggingModes = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.DebuggingModes };
        private static readonly byte[] s_signature_HasThis_Void_SecurityCriticalScope = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.SecurityCriticalScope };
        private static readonly byte[] s_signature_HasThis_Void_CallingConvention = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.CallingConvention };
        private static readonly byte[] s_signature_HasThis_Void_AssemblyHashAlgorithm = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.AssemblyHashAlgorithm };
        private static readonly byte[] s_signature_HasThis_Void_Int64 = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Int64 };
        private static readonly byte[] s_signature_HasThis_Void_UInt8_UInt8_UInt32_UInt32_UInt32 = new byte[] {
            (byte)SignatureAttributes.Instance, 5, Void, Byte, Byte, UInt32, UInt32, UInt32 };
        private static readonly byte[] s_signature_HasThis_Void_UIn8_UInt8_Int32_Int32_Int32 = new byte[] {
            (byte)SignatureAttributes.Instance, 5, Void, Byte, Byte, Int32, Int32, Int32 };


        private static readonly byte[] s_signature_HasThis_Void_Boolean = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, Boolean };
        private static readonly byte[] s_signature_HasThis_Void_Boolean_Boolean = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, Boolean, Boolean };

        private static readonly byte[] s_signature_HasThis_Void_Boolean_TransactionOption = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, Boolean, TypeHandle, (byte)TypeHandleTarget.TransactionOption };
        private static readonly byte[] s_signature_HasThis_Void_Boolean_TransactionOption_Int32 = new byte[] { (byte)SignatureAttributes.Instance, 3, Void, Boolean, TypeHandle, (byte)TypeHandleTarget.TransactionOption, Int32 };
        private static readonly byte[] s_signature_HasThis_Void_Boolean_TransactionOption_Int32_Boolean = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, Boolean, TypeHandle, (byte)TypeHandleTarget.TransactionOption, Int32, Boolean };

        private static readonly byte[] s_signature_HasThis_Void_SecurityAction = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.SecurityAction };
        private static readonly byte[] s_signature_HasThis_Void_Type = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, TypeHandle, (byte)TypeHandleTarget.SystemType };
        private static readonly byte[] s_signature_HasThis_Void_Type_Type = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType };
        private static readonly byte[] s_signature_HasThis_Void_Type_Type_Type = new byte[] { (byte)SignatureAttributes.Instance, 3, Void, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType };
        private static readonly byte[] s_signature_HasThis_Void_Type_Type_Type_Type = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType, TypeHandle, (byte)TypeHandleTarget.SystemType };
        private static readonly byte[] s_signature_HasThis_Void_Type_Int32 = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, TypeHandle, (byte)TypeHandleTarget.SystemType, Int32 };

        private static readonly byte[] s_signature_HasThis_Void_SzArray_Boolean = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, SzArray, Boolean };
        private static readonly byte[] s_signature_HasThis_Void_SzArray_Byte = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, SzArray, Byte };
        private static readonly byte[] s_signature_HasThis_Void_SzArray_String = new byte[] { (byte)SignatureAttributes.Instance, 1, Void, SzArray, String };
        private static readonly byte[] s_signature_HasThis_Void_Boolean_SzArray_String = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, Boolean, SzArray, String };
        private static readonly byte[] s_signature_HasThis_Void_Boolean_String = new byte[] { (byte)SignatureAttributes.Instance, 2, Void, Boolean, String };

        private static readonly byte[] s_signature_HasThis_Void_String_DeprecationType_UInt32 = new byte[] { (byte)SignatureAttributes.Instance, 3, Void, String, TypeHandle, (byte)TypeHandleTarget.DeprecationType, UInt32 };
        private static readonly byte[] s_signature_HasThis_Void_String_DeprecationType_UInt32_Platform = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, String, TypeHandle, (byte)TypeHandleTarget.DeprecationType, UInt32, TypeHandle, (byte)TypeHandleTarget.Platform };
        private static readonly byte[] s_signature_HasThis_Void_String_DeprecationType_UInt32_Type = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, String, TypeHandle, (byte)TypeHandleTarget.DeprecationType, UInt32, TypeHandle, (byte)TypeHandleTarget.SystemType };
        private static readonly byte[] s_signature_HasThis_Void_String_DeprecationType_UInt32_String = new byte[] { (byte)SignatureAttributes.Instance, 4, Void, String, TypeHandle, (byte)TypeHandleTarget.DeprecationType, UInt32, String };

        private static readonly byte[][] s_signatures_HasThis_Void_Only = { s_signature_HasThis_Void };
        private static readonly byte[][] s_signatures_HasThis_Void_String_Only = { s_signature_HasThis_Void_String };
        private static readonly byte[][] s_signatures_HasThis_Void_Type_Only = { s_signature_HasThis_Void_Type };
        private static readonly byte[][] s_signatures_HasThis_Void_Boolean_Only = { s_signature_HasThis_Void_Boolean };

        private static readonly byte[][] s_signaturesOfTypeIdentifierAttribute = { s_signature_HasThis_Void, s_signature_HasThis_Void_String_String };
        private static readonly byte[][] s_signaturesOfAttributeUsage = { s_signature_HasThis_Void_AttributeTargets };
        private static readonly byte[][] s_signaturesOfAssemblySignatureKeyAttribute = { s_signature_HasThis_Void_String_String };
        private static readonly byte[][] s_signaturesOfAssemblyFlagsAttribute =
        {
            s_signature_HasThis_Void_AssemblyNameFlags,
            s_signature_HasThis_Void_Int32,
            s_signature_HasThis_Void_UInt32
        };
        private static readonly byte[][] s_signaturesOfDefaultParameterValueAttribute = { s_signature_HasThis_Void_Object };
        private static readonly byte[][] s_signaturesOfDateTimeConstantAttribute = { s_signature_HasThis_Void_Int64 };
        private static readonly byte[][] s_signaturesOfDecimalConstantAttribute = { s_signature_HasThis_Void_UInt8_UInt8_UInt32_UInt32_UInt32, s_signature_HasThis_Void_UIn8_UInt8_Int32_Int32_Int32 };
        private static readonly byte[][] s_signaturesOfSecurityPermissionAttribute = { s_signature_HasThis_Void_SecurityAction };

        private static readonly byte[][] s_signaturesOfMethodImplAttribute =
        {
            s_signature_HasThis_Void,
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_MethodImplOptions,
        };

        private static readonly byte[][] s_signaturesOfDefaultCharSetAttribute = { s_signature_HasThis_Void_CharSet };

        private static readonly byte[][] s_signaturesOfFieldOffsetAttribute = { s_signature_HasThis_Void_Int32 };
        private static readonly byte[][] s_signaturesOfMemberNotNullAttribute = { s_signature_HasThis_Void_String, s_signature_HasThis_Void_SzArray_String };
        private static readonly byte[][] s_signaturesOfMemberNotNullWhenAttribute = { s_signature_HasThis_Void_Boolean_String, s_signature_HasThis_Void_Boolean_SzArray_String };
        private static readonly byte[][] s_signaturesOfFixedBufferAttribute = { s_signature_HasThis_Void_Type_Int32 };
        private static readonly byte[][] s_signaturesOfPrincipalPermissionAttribute = { s_signature_HasThis_Void_SecurityAction };
        private static readonly byte[][] s_signaturesOfPermissionSetAttribute = { s_signature_HasThis_Void_SecurityAction };

        private static readonly byte[][] s_signaturesOfStructLayoutAttribute =
        {
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_LayoutKind,
        };

        private static readonly byte[][] s_signaturesOfMarshalAsAttribute =
        {
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_UnmanagedType,
        };

        private static readonly byte[][] s_signaturesOfTypeLibTypeAttribute =
        {
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_TypeLibTypeFlags,
        };

        private static readonly byte[][] s_signaturesOfWebMethodAttribute =
        {
            s_signature_HasThis_Void,
            s_signature_HasThis_Void_Boolean,
            s_signature_HasThis_Void_Boolean_TransactionOption,
            s_signature_HasThis_Void_Boolean_TransactionOption_Int32,
            s_signature_HasThis_Void_Boolean_TransactionOption_Int32_Boolean
        };

        private static readonly byte[][] s_signaturesOfHostProtectionAttribute =
        {
            s_signature_HasThis_Void,
            s_signature_HasThis_Void_SecurityAction
        };

        private static readonly byte[][] s_signaturesOfVisualBasicComClassAttribute =
        {
            s_signature_HasThis_Void,
            s_signature_HasThis_Void_String,
            s_signature_HasThis_Void_String_String,
            s_signature_HasThis_Void_String_String_String
        };

        private static readonly byte[][] s_signaturesOfClassInterfaceAttribute =
        {
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_ClassInterfaceType
        };

        private static readonly byte[][] s_signaturesOfInterfaceTypeAttribute =
        {
            s_signature_HasThis_Void_Int16,
            s_signature_HasThis_Void_ComInterfaceType
        };

        private static readonly byte[][] s_signaturesOfCompilationRelaxationsAttribute =
        {
            s_signature_HasThis_Void_Int32,
            s_signature_HasThis_Void_CompilationRelaxations
        };

        private static readonly byte[][] s_signaturesOfDebuggableAttribute =
        {
            s_signature_HasThis_Void_Boolean_Boolean,
            s_signature_HasThis_Void_DebuggingModes
        };

        private static readonly byte[][] s_signaturesOfComSourceInterfacesAttribute =
        {
            s_signature_HasThis_Void_String,
            s_signature_HasThis_Void_Type,
            s_signature_HasThis_Void_Type_Type,
            s_signature_HasThis_Void_Type_Type_Type,
            s_signature_HasThis_Void_Type_Type_Type_Type
        };

        private static readonly byte[][] s_signaturesOfTypeLibVersionAttribute = { s_signature_HasThis_Void_Int32_Int32 };
        private static readonly byte[][] s_signaturesOfComCompatibleVersionAttribute = { s_signature_HasThis_Void_Int32_Int32_Int32_Int32 };
        private static readonly byte[][] s_signaturesOfObsoleteAttribute = { s_signature_HasThis_Void, s_signature_HasThis_Void_String, s_signature_HasThis_Void_String_Boolean };
        private static readonly byte[][] s_signaturesOfDynamicAttribute = { s_signature_HasThis_Void, s_signature_HasThis_Void_SzArray_Boolean };
        private static readonly byte[][] s_signaturesOfTupleElementNamesAttribute = { s_signature_HasThis_Void, s_signature_HasThis_Void_SzArray_String };

        private static readonly byte[][] s_signaturesOfSecurityCriticalAttribute =
        {
            s_signature_HasThis_Void,
            s_signature_HasThis_Void_SecurityCriticalScope
        };

        private static readonly byte[][] s_signaturesOfMyGroupCollectionAttribute = { s_signature_HasThis_Void_String_String_String_String };
        private static readonly byte[][] s_signaturesOfComEventInterfaceAttribute = { s_signature_HasThis_Void_Type_Type };
        private static readonly byte[][] s_signaturesOfLCIDConversionAttribute = { s_signature_HasThis_Void_Int32 };
        private static readonly byte[][] s_signaturesOfUnmanagedFunctionPointerAttribute = { s_signature_HasThis_Void_CallingConvention };
        private static readonly byte[][] s_signaturesOfPrimaryInteropAssemblyAttribute = { s_signature_HasThis_Void_Int32_Int32 };
        private static readonly byte[][] s_signaturesOfAssemblyAlgorithmIdAttribute =
        {
            s_signature_HasThis_Void_AssemblyHashAlgorithm,
            s_signature_HasThis_Void_UInt32
        };

        private static readonly byte[][] s_signaturesOfDeprecatedAttribute =
        {
            s_signature_HasThis_Void_String_DeprecationType_UInt32,
            s_signature_HasThis_Void_String_DeprecationType_UInt32_Platform,
            s_signature_HasThis_Void_String_DeprecationType_UInt32_Type,
            s_signature_HasThis_Void_String_DeprecationType_UInt32_String,
        };

        private static readonly byte[][] s_signaturesOfNullableAttribute = { s_signature_HasThis_Void_Byte, s_signature_HasThis_Void_SzArray_Byte };
        private static readonly byte[][] s_signaturesOfNullableContextAttribute = { s_signature_HasThis_Void_Byte };
        private static readonly byte[][] s_signaturesOfNativeIntegerAttribute = { s_signature_HasThis_Void, s_signature_HasThis_Void_SzArray_Boolean };

        // early decoded attributes:
        public static readonly AttributeDescription OptionalAttribute = new("System.Runtime.InteropServices", "OptionalAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription ComImportAttribute = new("System.Runtime.InteropServices", "ComImportAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription AttributeUsageAttribute = new("System", "AttributeUsageAttribute", s_signaturesOfAttributeUsage);
        public static readonly AttributeDescription ConditionalAttribute = new("System.Diagnostics", "ConditionalAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription CaseInsensitiveExtensionAttribute = new("System.Runtime.CompilerServices", "ExtensionAttribute", s_signatures_HasThis_Void_Only, matchIgnoringCase: true);
        public static readonly AttributeDescription CaseSensitiveExtensionAttribute = new("System.Runtime.CompilerServices", "ExtensionAttribute", s_signatures_HasThis_Void_Only, matchIgnoringCase: false);

        public static readonly AttributeDescription InternalsVisibleToAttribute = new("System.Runtime.CompilerServices", "InternalsVisibleToAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblySignatureKeyAttribute = new("System.Reflection", "AssemblySignatureKeyAttribute", s_signaturesOfAssemblySignatureKeyAttribute);
        public static readonly AttributeDescription AssemblyKeyFileAttribute = new("System.Reflection", "AssemblyKeyFileAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyKeyNameAttribute = new("System.Reflection", "AssemblyKeyNameAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription ParamArrayAttribute = new("System", "ParamArrayAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DefaultMemberAttribute = new("System.Reflection", "DefaultMemberAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription IndexerNameAttribute = new("System.Runtime.CompilerServices", "IndexerNameAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyDelaySignAttribute = new("System.Reflection", "AssemblyDelaySignAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription AssemblyVersionAttribute = new("System.Reflection", "AssemblyVersionAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyFileVersionAttribute = new("System.Reflection", "AssemblyFileVersionAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyTitleAttribute = new("System.Reflection", "AssemblyTitleAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyDescriptionAttribute = new("System.Reflection", "AssemblyDescriptionAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyCultureAttribute = new("System.Reflection", "AssemblyCultureAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyCompanyAttribute = new("System.Reflection", "AssemblyCompanyAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyProductAttribute = new("System.Reflection", "AssemblyProductAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyInformationalVersionAttribute = new("System.Reflection", "AssemblyInformationalVersionAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyCopyrightAttribute = new("System.Reflection", "AssemblyCopyrightAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription SatelliteContractVersionAttribute = new("System.Resources", "SatelliteContractVersionAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyTrademarkAttribute = new("System.Reflection", "AssemblyTrademarkAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyFlagsAttribute = new("System.Reflection", "AssemblyFlagsAttribute", s_signaturesOfAssemblyFlagsAttribute);
        public static readonly AttributeDescription DecimalConstantAttribute = new("System.Runtime.CompilerServices", "DecimalConstantAttribute", s_signaturesOfDecimalConstantAttribute);
        public static readonly AttributeDescription IUnknownConstantAttribute = new("System.Runtime.CompilerServices", "IUnknownConstantAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription CallerFilePathAttribute = new("System.Runtime.CompilerServices", "CallerFilePathAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription CallerLineNumberAttribute = new("System.Runtime.CompilerServices", "CallerLineNumberAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription CallerMemberNameAttribute = new("System.Runtime.CompilerServices", "CallerMemberNameAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription IDispatchConstantAttribute = new("System.Runtime.CompilerServices", "IDispatchConstantAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DefaultParameterValueAttribute = new("System.Runtime.InteropServices", "DefaultParameterValueAttribute", s_signaturesOfDefaultParameterValueAttribute);
        public static readonly AttributeDescription UnverifiableCodeAttribute = new("System.Runtime.InteropServices", "UnverifiableCodeAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription SecurityPermissionAttribute = new("System.Runtime.InteropServices", "SecurityPermissionAttribute", s_signaturesOfSecurityPermissionAttribute);
        public static readonly AttributeDescription DllImportAttribute = new("System.Runtime.InteropServices", "DllImportAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription MethodImplAttribute = new("System.Runtime.CompilerServices", "MethodImplAttribute", s_signaturesOfMethodImplAttribute);
        public static readonly AttributeDescription PreserveSigAttribute = new("System.Runtime.InteropServices", "PreserveSigAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DefaultCharSetAttribute = new("System.Runtime.InteropServices", "DefaultCharSetAttribute", s_signaturesOfDefaultCharSetAttribute);
        public static readonly AttributeDescription SpecialNameAttribute = new("System.Runtime.CompilerServices", "SpecialNameAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription SerializableAttribute = new("System", "SerializableAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription NonSerializedAttribute = new("System", "NonSerializedAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription StructLayoutAttribute = new("System.Runtime.InteropServices", "StructLayoutAttribute", s_signaturesOfStructLayoutAttribute);
        public static readonly AttributeDescription FieldOffsetAttribute = new("System.Runtime.InteropServices", "FieldOffsetAttribute", s_signaturesOfFieldOffsetAttribute);
        public static readonly AttributeDescription FixedBufferAttribute = new("System.Runtime.CompilerServices", "FixedBufferAttribute", s_signaturesOfFixedBufferAttribute);
        public static readonly AttributeDescription AllowNullAttribute = new("System.Diagnostics.CodeAnalysis", "AllowNullAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DisallowNullAttribute = new("System.Diagnostics.CodeAnalysis", "DisallowNullAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MaybeNullAttribute = new("System.Diagnostics.CodeAnalysis", "MaybeNullAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MaybeNullWhenAttribute = new("System.Diagnostics.CodeAnalysis", "MaybeNullWhenAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription NotNullAttribute = new("System.Diagnostics.CodeAnalysis", "NotNullAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MemberNotNullAttribute = new("System.Diagnostics.CodeAnalysis", "MemberNotNullAttribute", s_signaturesOfMemberNotNullAttribute);
        public static readonly AttributeDescription MemberNotNullWhenAttribute = new("System.Diagnostics.CodeAnalysis", "MemberNotNullWhenAttribute", s_signaturesOfMemberNotNullWhenAttribute);
        public static readonly AttributeDescription NotNullIfNotNullAttribute = new("System.Diagnostics.CodeAnalysis", "NotNullIfNotNullAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription NotNullWhenAttribute = new("System.Diagnostics.CodeAnalysis", "NotNullWhenAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription DoesNotReturnIfAttribute = new("System.Diagnostics.CodeAnalysis", "DoesNotReturnIfAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription DoesNotReturnAttribute = new("System.Diagnostics.CodeAnalysis", "DoesNotReturnAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MarshalAsAttribute = new("System.Runtime.InteropServices", "MarshalAsAttribute", s_signaturesOfMarshalAsAttribute);
        public static readonly AttributeDescription InAttribute = new("System.Runtime.InteropServices", "InAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription OutAttribute = new("System.Runtime.InteropServices", "OutAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription IsReadOnlyAttribute = new("System.Runtime.CompilerServices", "IsReadOnlyAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription IsUnmanagedAttribute = new("System.Runtime.CompilerServices", "IsUnmanagedAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription CoClassAttribute = new("System.Runtime.InteropServices", "CoClassAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription GuidAttribute = new("System.Runtime.InteropServices", "GuidAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription CLSCompliantAttribute = new("System", "CLSCompliantAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription HostProtectionAttribute = new("System.Security.Permissions", "HostProtectionAttribute", s_signaturesOfHostProtectionAttribute);
        public static readonly AttributeDescription SuppressUnmanagedCodeSecurityAttribute = new("System.Security", "SuppressUnmanagedCodeSecurityAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription PrincipalPermissionAttribute = new("System.Security.Permissions", "PrincipalPermissionAttribute", s_signaturesOfPrincipalPermissionAttribute);
        public static readonly AttributeDescription PermissionSetAttribute = new("System.Security.Permissions", "PermissionSetAttribute", s_signaturesOfPermissionSetAttribute);
        public static readonly AttributeDescription TypeIdentifierAttribute = new("System.Runtime.InteropServices", "TypeIdentifierAttribute", s_signaturesOfTypeIdentifierAttribute);
        public static readonly AttributeDescription VisualBasicEmbeddedAttribute = new("Microsoft.VisualBasic", "Embedded", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription CodeAnalysisEmbeddedAttribute = new("Microsoft.CodeAnalysis", "EmbeddedAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription VisualBasicComClassAttribute = new("Microsoft.VisualBasic", "ComClassAttribute", s_signaturesOfVisualBasicComClassAttribute);
        public static readonly AttributeDescription StandardModuleAttribute = new("Microsoft.VisualBasic.CompilerServices", "StandardModuleAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription OptionCompareAttribute = new("Microsoft.VisualBasic.CompilerServices", "OptionCompareAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription AccessedThroughPropertyAttribute = new("System.Runtime.CompilerServices", "AccessedThroughPropertyAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription WebMethodAttribute = new("System.Web.Services", "WebMethodAttribute", s_signaturesOfWebMethodAttribute);
        public static readonly AttributeDescription DateTimeConstantAttribute = new("System.Runtime.CompilerServices", "DateTimeConstantAttribute", s_signaturesOfDateTimeConstantAttribute);
        public static readonly AttributeDescription ClassInterfaceAttribute = new("System.Runtime.InteropServices", "ClassInterfaceAttribute", s_signaturesOfClassInterfaceAttribute);
        public static readonly AttributeDescription ComSourceInterfacesAttribute = new("System.Runtime.InteropServices", "ComSourceInterfacesAttribute", s_signaturesOfComSourceInterfacesAttribute);
        public static readonly AttributeDescription ComVisibleAttribute = new("System.Runtime.InteropServices", "ComVisibleAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription DispIdAttribute = new("System.Runtime.InteropServices", "DispIdAttribute", new byte[][] { s_signature_HasThis_Void_Int32 });
        public static readonly AttributeDescription TypeLibVersionAttribute = new("System.Runtime.InteropServices", "TypeLibVersionAttribute", s_signaturesOfTypeLibVersionAttribute);
        public static readonly AttributeDescription ComCompatibleVersionAttribute = new("System.Runtime.InteropServices", "ComCompatibleVersionAttribute", s_signaturesOfComCompatibleVersionAttribute);
        public static readonly AttributeDescription InterfaceTypeAttribute = new("System.Runtime.InteropServices", "InterfaceTypeAttribute", s_signaturesOfInterfaceTypeAttribute);
        public static readonly AttributeDescription WindowsRuntimeImportAttribute = new("System.Runtime.InteropServices.WindowsRuntime", "WindowsRuntimeImportAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DynamicSecurityMethodAttribute = new("System.Security", "DynamicSecurityMethodAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription RequiredAttributeAttribute = new("System.Runtime.CompilerServices", "RequiredAttributeAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription AsyncMethodBuilderAttribute = new("System.Runtime.CompilerServices", "AsyncMethodBuilderAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription AsyncStateMachineAttribute = new("System.Runtime.CompilerServices", "AsyncStateMachineAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription IteratorStateMachineAttribute = new("System.Runtime.CompilerServices", "IteratorStateMachineAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription CompilationRelaxationsAttribute = new("System.Runtime.CompilerServices", "CompilationRelaxationsAttribute", s_signaturesOfCompilationRelaxationsAttribute);
        public static readonly AttributeDescription ReferenceAssemblyAttribute = new("System.Runtime.CompilerServices", "ReferenceAssemblyAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription RuntimeCompatibilityAttribute = new("System.Runtime.CompilerServices", "RuntimeCompatibilityAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DebuggableAttribute = new("System.Diagnostics", "DebuggableAttribute", s_signaturesOfDebuggableAttribute);
        public static readonly AttributeDescription TypeForwardedToAttribute = new("System.Runtime.CompilerServices", "TypeForwardedToAttribute", s_signatures_HasThis_Void_Type_Only);
        public static readonly AttributeDescription STAThreadAttribute = new("System", "STAThreadAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MTAThreadAttribute = new("System", "MTAThreadAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription ObsoleteAttribute = new("System", "ObsoleteAttribute", s_signaturesOfObsoleteAttribute);
        public static readonly AttributeDescription TypeLibTypeAttribute = new("System.Runtime.InteropServices", "TypeLibTypeAttribute", s_signaturesOfTypeLibTypeAttribute);
        public static readonly AttributeDescription DynamicAttribute = new("System.Runtime.CompilerServices", "DynamicAttribute", s_signaturesOfDynamicAttribute);
        public static readonly AttributeDescription TupleElementNamesAttribute = new("System.Runtime.CompilerServices", "TupleElementNamesAttribute", s_signaturesOfTupleElementNamesAttribute);
        public static readonly AttributeDescription IsByRefLikeAttribute = new("System.Runtime.CompilerServices", "IsByRefLikeAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DebuggerHiddenAttribute = new("System.Diagnostics", "DebuggerHiddenAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DebuggerNonUserCodeAttribute = new("System.Diagnostics", "DebuggerNonUserCodeAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DebuggerStepperBoundaryAttribute = new("System.Diagnostics", "DebuggerStepperBoundaryAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DebuggerStepThroughAttribute = new("System.Diagnostics", "DebuggerStepThroughAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription SecurityCriticalAttribute = new("System.Security", "SecurityCriticalAttribute", s_signaturesOfSecurityCriticalAttribute);
        public static readonly AttributeDescription SecuritySafeCriticalAttribute = new("System.Security", "SecuritySafeCriticalAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription DesignerGeneratedAttribute = new("Microsoft.VisualBasic.CompilerServices", "DesignerGeneratedAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription MyGroupCollectionAttribute = new("Microsoft.VisualBasic", "MyGroupCollectionAttribute", s_signaturesOfMyGroupCollectionAttribute);
        public static readonly AttributeDescription ComEventInterfaceAttribute = new("System.Runtime.InteropServices", "ComEventInterfaceAttribute", s_signaturesOfComEventInterfaceAttribute);
        public static readonly AttributeDescription BestFitMappingAttribute = new("System.Runtime.InteropServices", "BestFitMappingAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription FlagsAttribute = new("System", "FlagsAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription LCIDConversionAttribute = new("System.Runtime.InteropServices", "LCIDConversionAttribute", s_signaturesOfLCIDConversionAttribute);
        public static readonly AttributeDescription UnmanagedFunctionPointerAttribute = new("System.Runtime.InteropServices", "UnmanagedFunctionPointerAttribute", s_signaturesOfUnmanagedFunctionPointerAttribute);
        public static readonly AttributeDescription PrimaryInteropAssemblyAttribute = new("System.Runtime.InteropServices", "PrimaryInteropAssemblyAttribute", s_signaturesOfPrimaryInteropAssemblyAttribute);
        public static readonly AttributeDescription ImportedFromTypeLibAttribute = new("System.Runtime.InteropServices", "ImportedFromTypeLibAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription DefaultEventAttribute = new("System.ComponentModel", "DefaultEventAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyConfigurationAttribute = new("System.Reflection", "AssemblyConfigurationAttribute", s_signatures_HasThis_Void_String_Only);
        public static readonly AttributeDescription AssemblyAlgorithmIdAttribute = new("System.Reflection", "AssemblyAlgorithmIdAttribute", s_signaturesOfAssemblyAlgorithmIdAttribute);
        public static readonly AttributeDescription DeprecatedAttribute = new("Windows.Foundation.Metadata", "DeprecatedAttribute", s_signaturesOfDeprecatedAttribute);
        public static readonly AttributeDescription NullableAttribute = new("System.Runtime.CompilerServices", "NullableAttribute", s_signaturesOfNullableAttribute);
        public static readonly AttributeDescription NullableContextAttribute = new("System.Runtime.CompilerServices", "NullableContextAttribute", s_signaturesOfNullableContextAttribute);
        public static readonly AttributeDescription NullablePublicOnlyAttribute = new("System.Runtime.CompilerServices", "NullablePublicOnlyAttribute", s_signatures_HasThis_Void_Boolean_Only);
        public static readonly AttributeDescription ExperimentalAttribute = new("Windows.Foundation.Metadata", "ExperimentalAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription ExcludeFromCodeCoverageAttribute = new("System.Diagnostics.CodeAnalysis", "ExcludeFromCodeCoverageAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription EnumeratorCancellationAttribute = new("System.Runtime.CompilerServices", "EnumeratorCancellationAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription SkipLocalsInitAttribute = new("System.Runtime.CompilerServices", "SkipLocalsInitAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription NativeIntegerAttribute = new("System.Runtime.CompilerServices", "NativeIntegerAttribute", s_signaturesOfNativeIntegerAttribute);
        public static readonly AttributeDescription ModuleInitializerAttribute = new("System.Runtime.CompilerServices", "ModuleInitializerAttribute", s_signatures_HasThis_Void_Only);
        public static readonly AttributeDescription UnmanagedCallersOnlyAttribute = new("System.Runtime.InteropServices", "UnmanagedCallersOnlyAttribute", s_signatures_HasThis_Void_Only);
    }
}
