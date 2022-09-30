using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class CSDiagnostic : DiagnosticWithInfo
    {
        internal CSDiagnostic(DiagnosticInfo info, Location location, bool isSuppressed = false)
            : base(info, location, isSuppressed)
        {
        }

        public override string ToString()
        {
            return CSharpDiagnosticFormatter.Instance.Format(this);
        }

        public override Diagnostic WithLocation(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if (location != Location)
            {
                return new CSDiagnostic(base.Info, location, IsSuppressed);
            }
            return this;
        }

        public override Diagnostic WithSeverity(DiagnosticSeverity severity)
        {
            if (Severity != severity)
            {
                return new CSDiagnostic(base.Info.GetInstanceWithSeverity(severity), Location, IsSuppressed);
            }
            return this;
        }

        public override Diagnostic WithIsSuppressed(bool isSuppressed)
        {
            if (IsSuppressed != isSuppressed)
            {
                return new CSDiagnostic(base.Info, Location, isSuppressed);
            }
            return this;
        }
    }
}
