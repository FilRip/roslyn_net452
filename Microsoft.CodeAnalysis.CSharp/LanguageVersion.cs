// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// Specifies the language version.
    /// </summary>
    public enum LanguageVersion
    {
        /// <summary>
        /// C# language version 1
        /// </summary>
        CSharp1 = 1,

        /// <summary>
        /// C# language version 2
        /// </summary>
        CSharp2 = 2,

        /// <summary>
        /// C# language version 3
        /// </summary>
        /// <remarks> 
        /// Features: LINQ.
        /// </remarks>
        CSharp3 = 3,

        /// <summary>
        /// C# language version 4
        /// </summary>
        /// <remarks> 
        /// Features: dynamic.
        /// </remarks>
        CSharp4 = 4,

        /// <summary>
        /// C# language version 5
        /// </summary>
        /// <remarks> 
        /// Features: async, caller info attributes.
        /// </remarks>
        CSharp5 = 5,

        /// <summary>
        /// C# language version 6
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Using of a static class</description></item>
        /// <item><description>Exception filters</description></item>
        /// <item><description>Await in catch/finally blocks</description></item>
        /// <item><description>Auto-property initializers</description></item>
        /// <item><description>Expression-bodied methods and properties</description></item>
        /// <item><description>Null-propagating operator ?.</description></item>
        /// <item><description>String interpolation</description></item>
        /// <item><description>nameof operator</description></item>
        /// <item><description>Dictionary initializer</description></item>
        /// </list>
        /// </remarks>
        CSharp6 = 6,

        /// <summary>
        /// C# language version 7.0
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Out variables</description></item>
        /// <item><description>Pattern-matching</description></item>
        /// <item><description>Tuples</description></item>
        /// <item><description>Deconstruction</description></item>
        /// <item><description>Discards</description></item>
        /// <item><description>Local functions</description></item>
        /// <item><description>Digit separators</description></item>
        /// <item><description>Ref returns and locals</description></item>
        /// <item><description>Generalized async return types</description></item>
        /// <item><description>More expression-bodied members</description></item>
        /// <item><description>Throw expressions</description></item>
        /// </list>
        /// </remarks>
        CSharp7 = 7,

        /// <summary>
        /// C# language version 7.1
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Async Main</description></item>
        /// <item><description>Default literal</description></item>
        /// <item><description>Inferred tuple element names</description></item>
        /// <item><description>Pattern-matching with generics</description></item>
        /// </list>
        /// </remarks>
        CSharp7_1 = 701,

        /// <summary>
        /// C# language version 7.2
        /// </summary>
        /// <remarks>
        /// <para>Features:</para>
        /// <list type="bullet">
        /// <item><description>Ref readonly</description></item>
        /// <item><description>Ref and readonly structs</description></item>
        /// <item><description>Ref extensions</description></item>
        /// <item><description>Conditional ref operator</description></item>
        /// <item><description>Private protected</description></item>
        /// <item><description>Digit separators after base specifier</description></item>
        /// <item><description>Non-trailing named arguments</description></item>
        /// </list>
        /// </remarks>
        CSharp7_2 = 702,

        /// <summary>
        /// C# language version 7.3
        /// </summary>
        CSharp7_3 = 703,

        /// <summary>
        /// C# language version 8.0
        /// </summary>
        CSharp8 = 800,

        // When this value is available in the released NuGet package, update LanguageVersionExtensions in the IDE layer to point to it.
        // https://github.com/dotnet/roslyn/issues/43348
        //
        /// <summary>
        /// C# language version 9.0
        /// </summary>
        CSharp9 = 900,

        /// <summary>
        /// The latest major supported version.
        /// </summary>
        LatestMajor = int.MaxValue - 2,

        /// <summary>
        /// Preview of the next language version.
        /// </summary>
        Preview = int.MaxValue - 1,

        /// <summary>
        /// The latest supported version of the language.
        /// </summary>
        Latest = int.MaxValue,

        /// <summary>
        /// The default language version, which is the latest supported version.
        /// </summary>
        Default = 0,
    }

    internal static class LanguageVersionExtensionsInternal
    {
        internal static bool IsValid(this LanguageVersion value)
        {
            return value switch
            {
                LanguageVersion.CSharp1 or LanguageVersion.CSharp2 or LanguageVersion.CSharp3 or LanguageVersion.CSharp4 or LanguageVersion.CSharp5 or LanguageVersion.CSharp6 or LanguageVersion.CSharp7 or LanguageVersion.CSharp7_1 or LanguageVersion.CSharp7_2 or LanguageVersion.CSharp7_3 or LanguageVersion.CSharp8 or LanguageVersion.CSharp9 or LanguageVersion.Preview => true,
                _ => false,
            };
        }

        internal static ErrorCode GetErrorCode(this LanguageVersion version)
        {
            return version switch
            {
                LanguageVersion.CSharp1 => ErrorCode.ERR_FeatureNotAvailableInVersion1,
                LanguageVersion.CSharp2 => ErrorCode.ERR_FeatureNotAvailableInVersion2,
                LanguageVersion.CSharp3 => ErrorCode.ERR_FeatureNotAvailableInVersion3,
                LanguageVersion.CSharp4 => ErrorCode.ERR_FeatureNotAvailableInVersion4,
                LanguageVersion.CSharp5 => ErrorCode.ERR_FeatureNotAvailableInVersion5,
                LanguageVersion.CSharp6 => ErrorCode.ERR_FeatureNotAvailableInVersion6,
                LanguageVersion.CSharp7 => ErrorCode.ERR_FeatureNotAvailableInVersion7,
                LanguageVersion.CSharp7_1 => ErrorCode.ERR_FeatureNotAvailableInVersion7_1,
                LanguageVersion.CSharp7_2 => ErrorCode.ERR_FeatureNotAvailableInVersion7_2,
                LanguageVersion.CSharp7_3 => ErrorCode.ERR_FeatureNotAvailableInVersion7_3,
                LanguageVersion.CSharp8 => ErrorCode.ERR_FeatureNotAvailableInVersion8,
                LanguageVersion.CSharp9 => ErrorCode.ERR_FeatureNotAvailableInVersion9,
                _ => throw ExceptionUtilities.UnexpectedValue(version),
            };
        }
    }

    internal class CSharpRequiredLanguageVersion : RequiredLanguageVersion
    {
        internal LanguageVersion Version { get; }

        internal CSharpRequiredLanguageVersion(LanguageVersion version)
        {
            Version = version;
        }

        public override string ToString() => Version.ToDisplayString();
    }

    public static class LanguageVersionFacts
    {
        /// <summary>
        /// Displays the version number in the format expected on the command-line (/langver flag).
        /// For instance, "6", "7.0", "7.1", "latest".
        /// </summary>
        public static string ToDisplayString(this LanguageVersion version)
        {
            return version switch
            {
                LanguageVersion.CSharp1 => "1",
                LanguageVersion.CSharp2 => "2",
                LanguageVersion.CSharp3 => "3",
                LanguageVersion.CSharp4 => "4",
                LanguageVersion.CSharp5 => "5",
                LanguageVersion.CSharp6 => "6",
                LanguageVersion.CSharp7 => "7.0",
                LanguageVersion.CSharp7_1 => "7.1",
                LanguageVersion.CSharp7_2 => "7.2",
                LanguageVersion.CSharp7_3 => "7.3",
                LanguageVersion.CSharp8 => "8.0",
                LanguageVersion.CSharp9 => "9.0",
                LanguageVersion.Default => "default",
                LanguageVersion.Latest => "latest",
                LanguageVersion.LatestMajor => "latestmajor",
                LanguageVersion.Preview => "preview",
                _ => throw ExceptionUtilities.UnexpectedValue(version),
            };
        }

        /// <summary>
        /// Try parse a <see cref="LanguageVersion"/> from a string input, returning default if input was null.
        /// </summary>
        public static bool TryParse(string? version, out LanguageVersion result)
        {
            if (version == null)
            {
                result = LanguageVersion.Default;
                return true;
            }

            switch (CaseInsensitiveComparison.ToLower(version))
            {
                case "default":
                    result = LanguageVersion.Default;
                    return true;

                case "latest":
                    result = LanguageVersion.Latest;
                    return true;

                case "latestmajor":
                    result = LanguageVersion.LatestMajor;
                    return true;

                case "preview":
                    result = LanguageVersion.Preview;
                    return true;

                case "1":
                case "1.0":
                case "iso-1":
                    result = LanguageVersion.CSharp1;
                    return true;

                case "2":
                case "2.0":
                case "iso-2":
                    result = LanguageVersion.CSharp2;
                    return true;

                case "3":
                case "3.0":
                    result = LanguageVersion.CSharp3;
                    return true;

                case "4":
                case "4.0":
                    result = LanguageVersion.CSharp4;
                    return true;

                case "5":
                case "5.0":
                    result = LanguageVersion.CSharp5;
                    return true;

                case "6":
                case "6.0":
                    result = LanguageVersion.CSharp6;
                    return true;

                case "7":
                case "7.0":
                    result = LanguageVersion.CSharp7;
                    return true;

                case "7.1":
                    result = LanguageVersion.CSharp7_1;
                    return true;

                case "7.2":
                    result = LanguageVersion.CSharp7_2;
                    return true;

                case "7.3":
                    result = LanguageVersion.CSharp7_3;
                    return true;

                case "8":
                case "8.0":
                    result = LanguageVersion.CSharp8;
                    return true;

                case "9":
                case "9.0":
                    result = LanguageVersion.CSharp9;
                    return true;

                default:
                    result = LanguageVersion.Default;
                    return false;
            }
        }

        /// <summary>
        /// Map a language version (such as Default, Latest, or CSharpN) to a specific version (CSharpM).
        /// </summary>
        public static LanguageVersion MapSpecifiedToEffectiveVersion(this LanguageVersion version)
        {
            return version switch
            {
                LanguageVersion.Latest or LanguageVersion.Default or LanguageVersion.LatestMajor => LanguageVersion.CSharp9,
                _ => version,
            };
        }

        internal static LanguageVersion CurrentVersion => LanguageVersion.CSharp9;

        /// <summary>Inference of tuple element names was added in C# 7.1</summary>
        internal static bool DisallowInferredTupleElementNames(this LanguageVersion self)
        {
            return self < MessageID.IDS_FeatureInferredTupleNames.RequiredVersion();
        }

        internal static bool AllowNonTrailingNamedArguments(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureNonTrailingNamedArguments.RequiredVersion();
        }

        internal static bool AllowAttributesOnBackingFields(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureAttributesOnBackingFields.RequiredVersion();
        }

        internal static bool AllowImprovedOverloadCandidates(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureImprovedOverloadCandidates.RequiredVersion();
        }
    }
}
