// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Globalization;
using System.Reflection;

using Microsoft.CodeAnalysis;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Scripting.Hosting
{
    using static ObjectFormatterHelpers;

    public abstract partial class CommonPrimitiveFormatter
    {
        /// <summary>
        /// String that describes "null" literal in the language.
        /// </summary>
        protected abstract string NullLiteral { get; }

        protected abstract string FormatLiteral(bool value);
        protected abstract string FormatLiteral(string value, bool quote, bool escapeNonPrintable, int numberRadix = NumberRadixDecimal);
        protected abstract string FormatLiteral(char value, bool quote, bool escapeNonPrintable, bool includeCodePoints = false, int numberRadix = NumberRadixDecimal);
        protected abstract string FormatLiteral(sbyte value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(byte value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(short value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(ushort value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(int value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(uint value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(long value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(ulong value, int numberRadix = NumberRadixDecimal, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(double value, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(float value, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(decimal value, CultureInfo cultureInfo = null);
        protected abstract string FormatLiteral(DateTime value, CultureInfo cultureInfo = null);

        /// <summary>
        /// Returns null if the type is not considered primitive in the target language.
        /// </summary>
        public string FormatPrimitive(object obj, CommonPrimitiveFormatterOptions options)
        {
            if (ReferenceEquals(obj, VoidValue))
            {
                return string.Empty;
            }

            if (obj == null)
            {
                return NullLiteral;
            }

            var type = obj.GetType();

            if (type.GetTypeInfo().IsEnum)
            {
                return obj.ToString();
            }

            return GetPrimitiveSpecialType(type) switch
            {
                SpecialType.System_Int32 => FormatLiteral((int)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_String => FormatLiteral((string)obj, options.QuoteStringsAndCharacters, options.EscapeNonPrintableCharacters, options.NumberRadix),
                SpecialType.System_Boolean => FormatLiteral((bool)obj),
                SpecialType.System_Char => FormatLiteral((char)obj, options.QuoteStringsAndCharacters, options.EscapeNonPrintableCharacters, options.IncludeCharacterCodePoints, options.NumberRadix),
                SpecialType.System_Int64 => FormatLiteral((long)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_Double => FormatLiteral((double)obj, options.CultureInfo),
                SpecialType.System_Byte => FormatLiteral((byte)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_Decimal => FormatLiteral((decimal)obj, options.CultureInfo),
                SpecialType.System_UInt32 => FormatLiteral((uint)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_UInt64 => FormatLiteral((ulong)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_Single => FormatLiteral((float)obj, options.CultureInfo),
                SpecialType.System_Int16 => FormatLiteral((short)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_UInt16 => FormatLiteral((ushort)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_DateTime => FormatLiteral((DateTime)obj, options.CultureInfo),
                SpecialType.System_SByte => FormatLiteral((sbyte)obj, options.NumberRadix, options.CultureInfo),
                SpecialType.System_Object or SpecialType.System_Void or SpecialType.None => null,
                _ => throw ExceptionUtilities.UnexpectedValue(GetPrimitiveSpecialType(type)),
            };
        }
    }
}
