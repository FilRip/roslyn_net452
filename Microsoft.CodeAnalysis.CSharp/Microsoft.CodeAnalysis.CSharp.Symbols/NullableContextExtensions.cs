using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class NullableContextExtensions
    {
        internal static bool TryGetByte(this NullableContextKind kind, out byte? value)
        {
            switch (kind)
            {
                case NullableContextKind.Unknown:
                    value = null;
                    return false;
                case NullableContextKind.None:
                    value = null;
                    return true;
                case NullableContextKind.Oblivious:
                    value = 0;
                    return true;
                case NullableContextKind.NotAnnotated:
                    value = 1;
                    return true;
                case NullableContextKind.Annotated:
                    value = 2;
                    return true;
                default:
                    throw ExceptionUtilities.UnexpectedValue(kind);
            }
        }

        internal static NullableContextKind ToNullableContextFlags(this byte? value)
        {
            return value switch
            {
                null => NullableContextKind.None,
                0 => NullableContextKind.Oblivious,
                1 => NullableContextKind.NotAnnotated,
                2 => NullableContextKind.Annotated,
                _ => throw ExceptionUtilities.UnexpectedValue(value),
            };
        }
    }
}
