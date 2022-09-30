using System.Collections.Generic;
using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public static class DiagnosticBagExtensions
    {
        public static CSDiagnosticInfo Add(this DiagnosticBag diagnostics, ErrorCode code, Location location)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new CSDiagnosticInfo(code);
            CSDiagnostic diag = new CSDiagnostic(cSDiagnosticInfo, location);
            diagnostics.Add(diag);
            return cSDiagnosticInfo;
        }

        public static CSDiagnosticInfo Add(this DiagnosticBag diagnostics, ErrorCode code, Location location, params object[] args)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new CSDiagnosticInfo(code, args);
            CSDiagnostic diag = new CSDiagnostic(cSDiagnosticInfo, location);
            diagnostics.Add(diag);
            return cSDiagnosticInfo;
        }

        public static CSDiagnosticInfo Add(this DiagnosticBag diagnostics, ErrorCode code, Location location, ImmutableArray<Symbol> symbols, params object[] args)
        {
            CSDiagnosticInfo cSDiagnosticInfo = new CSDiagnosticInfo(code, args, symbols, ImmutableArray<Location>.Empty);
            CSDiagnostic diag = new CSDiagnostic(cSDiagnosticInfo, location);
            diagnostics.Add(diag);
            return cSDiagnosticInfo;
        }

        public static void Add(this DiagnosticBag diagnostics, DiagnosticInfo info, Location location)
        {
            CSDiagnostic diag = new CSDiagnostic(info, location);
            diagnostics.Add(diag);
        }

        public static bool Add(this DiagnosticBag diagnostics, SyntaxNode node, HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            return !useSiteDiagnostics.IsNullOrEmpty() && diagnostics.Add(node.Location, useSiteDiagnostics.ToReadOnlyCollection());
        }

        public static bool Add(this DiagnosticBag diagnostics, SyntaxToken token, HashSet<DiagnosticInfo> useSiteDiagnostics)
        {
            return !useSiteDiagnostics.IsNullOrEmpty() && diagnostics.Add(token.GetLocation(), useSiteDiagnostics.ToReadOnlyCollection());
        }

        public static bool Add(this DiagnosticBag diagnostics, Location location, IReadOnlyCollection<DiagnosticInfo> useSiteDiagnostics)
        {
            if (useSiteDiagnostics.IsNullOrEmpty())
            {
                return false;
            }
            bool result = false;
            foreach (DiagnosticInfo useSiteDiagnostic in useSiteDiagnostics)
            {
                if (useSiteDiagnostic.Severity == DiagnosticSeverity.Error)
                {
                    result = true;
                }
                diagnostics.Add(new CSDiagnostic(useSiteDiagnostic, location));
            }
            return result;
        }
    }
}
