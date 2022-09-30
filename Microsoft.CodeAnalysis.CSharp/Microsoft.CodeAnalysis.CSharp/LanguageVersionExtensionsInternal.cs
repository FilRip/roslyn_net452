using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class LanguageVersionExtensionsInternal
    {
        internal static bool IsValid(this LanguageVersion value)
        {
            switch (value)
            {
                case LanguageVersion.CSharp1:
                case LanguageVersion.CSharp2:
                case LanguageVersion.CSharp3:
                case LanguageVersion.CSharp4:
                case LanguageVersion.CSharp5:
                case LanguageVersion.CSharp6:
                case LanguageVersion.CSharp7:
                case LanguageVersion.CSharp7_1:
                case LanguageVersion.CSharp7_2:
                case LanguageVersion.CSharp7_3:
                case LanguageVersion.CSharp8:
                case LanguageVersion.CSharp9:
                case LanguageVersion.Preview:
                    return true;
                default:
                    return false;
            }
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
}
