using System;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class RequiredLanguageVersion : IFormattable
    {
        public abstract override string ToString();

        string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
        {
            return ToString();
        }
    }
}
