using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class ErrorLogger
    {
        public abstract void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo);
    }
}
