// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A diagnostic (such as a compiler error or a warning), along with the location where it occurred.
    /// </summary>
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public class DiagnosticWithInfo : Diagnostic
    {
        private readonly DiagnosticInfo _info;
        private readonly Location _location;
        private readonly bool _isSuppressed;

        public DiagnosticWithInfo(DiagnosticInfo info, Location location, bool isSuppressed = false)
        {
            RoslynDebug.Assert(info != null);
            RoslynDebug.Assert(location != null);
            _info = info;
            _location = location;
            _isSuppressed = isSuppressed;
        }

        public override Location Location
        {
            get { return _location; }
        }

        public override IReadOnlyList<Location> AdditionalLocations
        {
            get { return this.Info.AdditionalLocations; }
        }

        internal override IReadOnlyList<string> CustomTags
        {
            get
            {
                return this.Info.CustomTags;
            }
        }

        public override DiagnosticDescriptor Descriptor
        {
            get
            {
                return this.Info.Descriptor;
            }
        }

        public override string Id
        {
            get { return this.Info.MessageIdentifier; }
        }

        public override string Category
        {
            get { return this.Info.Category; }
        }


        public sealed override int Code
        {
            get { return this.Info.Code; }
        }

        public sealed override DiagnosticSeverity Severity
        {
            get { return this.Info.Severity; }
        }

        public sealed override DiagnosticSeverity DefaultSeverity
        {
            get { return this.Info.DefaultSeverity; }
        }

        public sealed override bool IsEnabledByDefault
        {
            // All compiler errors and warnings are enabled by default.
            get { return true; }
        }

        public override bool IsSuppressed
        {
            get { return _isSuppressed; }
        }

        public sealed override int WarningLevel
        {
            get { return this.Info.WarningLevel; }
        }

        public override string GetMessage(IFormatProvider? formatProvider = null)
        {
            return this.Info.GetMessage(formatProvider);
        }

        public override IReadOnlyList<object?> Arguments
        {
            get { return this.Info.Arguments; }
        }

        /// <summary>
        /// Get the information about the diagnostic: the code, severity, message, etc.
        /// </summary>
        public DiagnosticInfo Info
        {
            get
            {
                if (_info.Severity == InternalDiagnosticSeverity.Unknown)
                {
                    return _info.GetResolvedInfo();
                }

                return _info;
            }
        }

        /// <summary>
        /// True if the DiagnosticInfo for this diagnostic requires (or required - this property
        /// is immutable) resolution.
        /// </summary>
        public bool HasLazyInfo
        {
            get
            {
                return _info.Severity == InternalDiagnosticSeverity.Unknown ||
                    _info.Severity == InternalDiagnosticSeverity.Void;
            }
        }

        public override int GetHashCode()
        {
            return Hash.Combine(this.Location.GetHashCode(), this.Info.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Diagnostic);
        }

        public override bool Equals(Diagnostic? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }


            if (obj is not DiagnosticWithInfo other || this.GetType() != other.GetType())
            {
                return false;
            }

            return
                this.Location.Equals(other._location) &&
                this.Info.Equals(other.Info) &&
                this.AdditionalLocations.SequenceEqual(other.AdditionalLocations);
        }

        private string GetDebuggerDisplay()
        {
            return _info.Severity switch
            {
                InternalDiagnosticSeverity.Unknown => "Unresolved diagnostic at " + this.Location,// If we called ToString before the diagnostic was resolved,
                                                                                                  // we would risk infinite recursion (e.g. if we were still computing
                                                                                                  // member lists).
                InternalDiagnosticSeverity.Void => "Void diagnostic at " + this.Location,// If we called ToString on a void diagnostic, the MessageProvider
                                                                                         // would complain about the code.
                _ => ToString(),
            };
        }

        public override Diagnostic WithLocation(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (location != _location)
            {
                return new DiagnosticWithInfo(_info, location, _isSuppressed);
            }

            return this;
        }

        public override Diagnostic WithSeverity(DiagnosticSeverity severity)
        {
            if (this.Severity != severity)
            {
                return new DiagnosticWithInfo(this.Info.GetInstanceWithSeverity(severity), _location, _isSuppressed);
            }

            return this;
        }

        public override Diagnostic WithIsSuppressed(bool isSuppressed)
        {
            if (this.IsSuppressed != isSuppressed)
            {
                return new DiagnosticWithInfo(this.Info, _location, isSuppressed);
            }

            return this;
        }

        public sealed override bool IsNotConfigurable()
        {
            return this.Info.IsNotConfigurable();
        }
    }
}
