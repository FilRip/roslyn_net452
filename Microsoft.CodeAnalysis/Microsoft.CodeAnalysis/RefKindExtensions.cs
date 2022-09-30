using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public static class RefKindExtensions
    {
        public const RefKind StrictIn = (RefKind)4;

        public static string ToParameterDisplayString(this RefKind kind)
        {
            return kind switch
            {
                RefKind.Out => "out",
                RefKind.Ref => "ref",
                RefKind.In => "in",
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        public static string ToArgumentDisplayString(this RefKind kind)
        {
            return kind switch
            {
                RefKind.Out => "out",
                RefKind.Ref => "ref",
                RefKind.In => "in",
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        public static string ToParameterPrefix(this RefKind kind)
        {
            return kind switch
            {
                RefKind.Out => "out ",
                RefKind.Ref => "ref ",
                RefKind.In => "in ",
                RefKind.None => string.Empty,
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }
    }
}
