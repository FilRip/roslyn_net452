using System;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class VBDiagnostic : DiagnosticWithInfo
	{
		internal VBDiagnostic(DiagnosticInfo info, Location location, bool isSuppressed = false)
			: base(info, location, isSuppressed)
		{
		}

		public override string ToString()
		{
			return VisualBasicDiagnosticFormatter.Instance.Format(this);
		}

		internal override Diagnostic WithLocation(Location location)
		{
			if ((object)location == null)
			{
				throw new ArgumentNullException("location");
			}
			if ((object)location != Location)
			{
				return new VBDiagnostic(base.Info, location, IsSuppressed);
			}
			return this;
		}

		internal override Diagnostic WithSeverity(DiagnosticSeverity severity)
		{
			if (Severity != severity)
			{
				return new VBDiagnostic(base.Info.GetInstanceWithSeverity(severity), Location, IsSuppressed);
			}
			return this;
		}

		internal override Diagnostic WithIsSuppressed(bool isSuppressed)
		{
			if (IsSuppressed != isSuppressed)
			{
				return new VBDiagnostic(base.Info, Location, isSuppressed);
			}
			return this;
		}
	}
}
