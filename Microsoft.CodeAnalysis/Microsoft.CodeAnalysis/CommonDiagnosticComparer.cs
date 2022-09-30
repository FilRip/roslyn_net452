using System.Collections.Generic;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class CommonDiagnosticComparer : IEqualityComparer<Diagnostic>
    {
        public static readonly CommonDiagnosticComparer Instance = new CommonDiagnosticComparer();

        private CommonDiagnosticComparer()
        {
        }

        public bool Equals(Diagnostic? x, Diagnostic? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            if (x!.Location == y!.Location)
            {
                return x!.Id == y!.Id;
            }
            return false;
        }

        public int GetHashCode(Diagnostic obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return Hash.Combine(obj.Location, obj.Id.GetHashCode());
        }
    }
}
