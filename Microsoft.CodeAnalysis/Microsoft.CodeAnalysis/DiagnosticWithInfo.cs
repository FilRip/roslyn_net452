using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DiagnosticWithInfo : Diagnostic
    {
        private readonly DiagnosticInfo _info;

        private readonly Location _location;

        private readonly bool _isSuppressed;

        public override Location Location => _location;

        public override IReadOnlyList<Location> AdditionalLocations => Info.AdditionalLocations;

        internal override IReadOnlyList<string> CustomTags => Info.CustomTags;

        public override DiagnosticDescriptor Descriptor => Info.Descriptor;

        public override string Id => Info.MessageIdentifier;

        public override string Category => Info.Category;

        public sealed override int Code => Info.Code;

        public sealed override DiagnosticSeverity Severity => Info.Severity;

        public sealed override DiagnosticSeverity DefaultSeverity => Info.DefaultSeverity;

        public sealed override bool IsEnabledByDefault => true;

        public override bool IsSuppressed => _isSuppressed;

        public sealed override int WarningLevel => Info.WarningLevel;

        public override IReadOnlyList<object?> Arguments => Info.Arguments;

        public DiagnosticInfo Info
        {
            get
            {
                if (_info.Severity == (DiagnosticSeverity)(-1))
                {
                    return _info.GetResolvedInfo();
                }
                return _info;
            }
        }

        internal bool HasLazyInfo
        {
            get
            {
                if (_info.Severity != (DiagnosticSeverity)(-1))
                {
                    return _info.Severity == (DiagnosticSeverity)(-2);
                }
                return true;
            }
        }

        public DiagnosticWithInfo(DiagnosticInfo info, Location location, bool isSuppressed = false)
        {
            _info = info;
            _location = location;
            _isSuppressed = isSuppressed;
        }

        public override string GetMessage(IFormatProvider? formatProvider = null)
        {
            return Info.GetMessage(formatProvider);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Location.GetHashCode(), Info.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Diagnostic);
        }

        public override bool Equals(Diagnostic? obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!(obj is DiagnosticWithInfo diagnosticWithInfo) || GetType() != diagnosticWithInfo.GetType())
            {
                return false;
            }
            if (Location.Equals(diagnosticWithInfo._location) && Info.Equals(diagnosticWithInfo.Info))
            {
                return AdditionalLocations.SequenceEqual(diagnosticWithInfo.AdditionalLocations);
            }
            return false;
        }

        private string GetDebuggerDisplay()
        {
            return _info.Severity switch
            {
                (DiagnosticSeverity)(-1) => "Unresolved diagnostic at " + Location,
                (DiagnosticSeverity)(-2) => "Void diagnostic at " + Location,
                _ => ToString(),
            };
        }

        public override Diagnostic WithLocation(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }
            if (location != _location)
            {
                return new DiagnosticWithInfo(_info, location, _isSuppressed);
            }
            return this;
        }

        public override Diagnostic WithSeverity(DiagnosticSeverity severity)
        {
            if (Severity != severity)
            {
                return new DiagnosticWithInfo(Info.GetInstanceWithSeverity(severity), _location, _isSuppressed);
            }
            return this;
        }

        public override Diagnostic WithIsSuppressed(bool isSuppressed)
        {
            if (IsSuppressed != isSuppressed)
            {
                return new DiagnosticWithInfo(Info, _location, isSuppressed);
            }
            return this;
        }

        public sealed override bool IsNotConfigurable()
        {
            return Info.IsNotConfigurable();
        }
    }
}
