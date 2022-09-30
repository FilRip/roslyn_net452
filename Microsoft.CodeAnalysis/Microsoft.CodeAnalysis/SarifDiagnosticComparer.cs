using System.Collections.Generic;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal sealed class SarifDiagnosticComparer : IEqualityComparer<DiagnosticDescriptor>
    {
        public static readonly SarifDiagnosticComparer Instance = new SarifDiagnosticComparer();

        private SarifDiagnosticComparer()
        {
        }

        public bool Equals(DiagnosticDescriptor? x, DiagnosticDescriptor? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            if (x!.Category == y!.Category && x!.DefaultSeverity == y!.DefaultSeverity && x!.Description.Equals(y!.Description) && x!.HelpLinkUri == y!.HelpLinkUri && x!.Id == y!.Id && x!.IsEnabledByDefault == y!.IsEnabledByDefault && x!.Title.Equals(y!.Title))
            {
                return x!.CustomTags.SequenceEqual(y!.CustomTags);
            }
            return false;
        }

        public int GetHashCode(DiagnosticDescriptor obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return Hash.Combine(obj.Category.GetHashCode(), Hash.Combine(obj.DefaultSeverity.GetHashCode(), Hash.Combine(obj.Description.GetHashCode(), Hash.Combine(obj.HelpLinkUri.GetHashCode(), Hash.Combine(obj.Id.GetHashCode(), Hash.Combine(obj.IsEnabledByDefault.GetHashCode(), Hash.Combine(obj.Title.GetHashCode(), Hash.CombineValues(obj.CustomTags))))))));
        }
    }
}
