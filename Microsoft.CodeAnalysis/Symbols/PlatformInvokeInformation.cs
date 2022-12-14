// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Runtime.InteropServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Information that describes how a method from the underlying Platform is to be invoked.
    /// </summary>
    public sealed class DllImportData : Cci.IPlatformInvokeInformation
    {
        private readonly string? _moduleName;
        private readonly string? _entryPointName;            // null if unspecified, the name of the target method should be used
        private readonly MethodImportAttributes _flags;

        public DllImportData(string? moduleName, string? entryPointName, MethodImportAttributes flags)
        {
            _moduleName = moduleName;
            _entryPointName = entryPointName;
            _flags = flags;
        }

        /// <summary>
        /// Module name. Null if value specified in the attribute is not valid.
        /// </summary>
        public string? ModuleName
        {
            get { return _moduleName; }
        }

        /// <summary>
        /// Name of the native entry point or null if not specified (the effective name is the same as the name of the target method).
        /// </summary>
        public string? EntryPointName
        {
            get { return _entryPointName; }
        }

        MethodImportAttributes Cci.IPlatformInvokeInformation.Flags
        {
            get { return _flags; }
        }

        /// <summary>
        /// Controls whether the <see cref="CharacterSet"/> field causes the common language runtime 
        /// to search an unmanaged DLL for entry-point names other than the one specified.
        /// </summary>
        public bool ExactSpelling
        {
            get
            {
                return (_flags & MethodImportAttributes.ExactSpelling) != 0;
            }
        }

        /// <summary>
        /// Indicates how to marshal string parameters and controls name mangling.
        /// </summary>
        public CharSet CharacterSet
        {
            get
            {
                return (_flags & MethodImportAttributes.CharSetMask) switch
                {
                    MethodImportAttributes.CharSetAnsi => CharSet.Ansi,
                    MethodImportAttributes.CharSetUnicode => CharSet.Unicode,
                    MethodImportAttributes.CharSetAuto => Cci.Constants.CharSet_Auto,
                    0 => Cci.Constants.CharSet_None,
                    _ => throw ExceptionUtilities.UnexpectedValue(_flags),
                };
            }
        }

        /// <summary>
        /// Indicates whether the callee calls the SetLastError Win32 API function before returning from the attributed method.
        /// </summary>
        public bool SetLastError
        {
            get
            {
                return (_flags & MethodImportAttributes.SetLastError) != 0;
            }
        }

        /// <summary>
        /// Indicates the calling convention of an entry point.
        /// </summary>
        public CallingConvention CallingConvention
        {
            get
            {
                return (_flags & MethodImportAttributes.CallingConventionMask) switch
                {
                    MethodImportAttributes.CallingConventionCDecl => CallingConvention.Cdecl,
                    MethodImportAttributes.CallingConventionStdCall => CallingConvention.StdCall,
                    MethodImportAttributes.CallingConventionThisCall => CallingConvention.ThisCall,
                    MethodImportAttributes.CallingConventionFastCall => Cci.Constants.CallingConvention_FastCall,
                    _ => CallingConvention.Winapi,
                };
            }
        }

        /// <summary>
        /// Enables or disables best-fit mapping behavior when converting Unicode characters to ANSI characters.
        /// Null if not specified (the setting for the containing type or assembly should be used, <see cref="BestFitMappingAttribute"/>).
        /// </summary>
        public bool? BestFitMapping
        {
            get
            {
                return (_flags & MethodImportAttributes.BestFitMappingMask) switch
                {
                    MethodImportAttributes.BestFitMappingEnable => true,
                    MethodImportAttributes.BestFitMappingDisable => false,
                    _ => null,
                };
            }
        }

        /// <summary>
        /// Enables or disables the throwing of an exception on an unmappable Unicode character that is converted to an ANSI "?" character.
        /// Null if not specified.
        /// </summary>
        public bool? ThrowOnUnmappableCharacter
        {
            get
            {
                return (_flags & MethodImportAttributes.ThrowOnUnmappableCharMask) switch
                {
                    MethodImportAttributes.ThrowOnUnmappableCharEnable => true,
                    MethodImportAttributes.ThrowOnUnmappableCharDisable => false,
                    _ => null,
                };
            }
        }

        public static MethodImportAttributes MakeFlags(bool exactSpelling, CharSet charSet, bool setLastError, CallingConvention callingConvention, bool? useBestFit, bool? throwOnUnmappable)
        {
            MethodImportAttributes result = 0;
            if (exactSpelling)
            {
                result |= MethodImportAttributes.ExactSpelling;
            }

            switch (charSet)
            {
                case CharSet.Ansi:
                    result |= MethodImportAttributes.CharSetAnsi;
                    break;

                case CharSet.Unicode:
                    result |= MethodImportAttributes.CharSetUnicode;
                    break;

                case Cci.Constants.CharSet_Auto:
                    result |= MethodImportAttributes.CharSetAuto;
                    break;

                    // Dev10: use default without reporting an error
            }

            if (setLastError)
            {
                result |= MethodImportAttributes.SetLastError;
            }

            result |= callingConvention switch
            {
                CallingConvention.Cdecl => MethodImportAttributes.CallingConventionCDecl,
                CallingConvention.StdCall => MethodImportAttributes.CallingConventionStdCall,
                CallingConvention.ThisCall => MethodImportAttributes.CallingConventionThisCall,
                Cci.Constants.CallingConvention_FastCall => MethodImportAttributes.CallingConventionFastCall,
                // Dev10: uses default without reporting an error
                _ => MethodImportAttributes.CallingConventionWinApi,
            };
            if (throwOnUnmappable.HasValue)
            {
                if (throwOnUnmappable.Value)
                {
                    result |= MethodImportAttributes.ThrowOnUnmappableCharEnable;
                }
                else
                {
                    result |= MethodImportAttributes.ThrowOnUnmappableCharDisable;
                }
            }

            if (useBestFit.HasValue)
            {
                if (useBestFit.Value)
                {
                    result |= MethodImportAttributes.BestFitMappingEnable;
                }
                else
                {
                    result |= MethodImportAttributes.BestFitMappingDisable;
                }
            }

            return result;
        }
    }
}
