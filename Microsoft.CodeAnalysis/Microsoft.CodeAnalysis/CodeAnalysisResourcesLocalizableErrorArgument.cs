using System;
using System.Globalization;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct CodeAnalysisResourcesLocalizableErrorArgument : IFormattable
    {
        private readonly string _targetResourceId;

        public CodeAnalysisResourcesLocalizableErrorArgument(string targetResourceId)
        {
            _targetResourceId = targetResourceId;
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            if (_targetResourceId != null)
            {
                return CodeAnalysisResources.ResourceManager.GetString(_targetResourceId, formatProvider as CultureInfo) ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
