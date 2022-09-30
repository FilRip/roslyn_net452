using System;
using System.Collections.Generic;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public struct Suppression : IEquatable<Suppression>
    {
        public SuppressionDescriptor Descriptor { get; }

        public Diagnostic SuppressedDiagnostic { get; }

        private Suppression(SuppressionDescriptor descriptor, Diagnostic suppressedDiagnostic)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException("descriptor");
            SuppressedDiagnostic = suppressedDiagnostic ?? throw new ArgumentNullException("suppressedDiagnostic");
            if (descriptor.SuppressedDiagnosticId != suppressedDiagnostic.Id)
            {
                throw new ArgumentException(string.Format(CodeAnalysisResources.InvalidDiagnosticSuppressionReported, suppressedDiagnostic.Id, descriptor.SuppressedDiagnosticId));
            }
        }

        public static Suppression Create(SuppressionDescriptor descriptor, Diagnostic suppressedDiagnostic)
        {
            return new Suppression(descriptor, suppressedDiagnostic);
        }

        public static bool operator ==(Suppression left, Suppression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Suppression left, Suppression right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Suppression other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(Suppression other)
        {
            if (EqualityComparer<SuppressionDescriptor>.Default.Equals(Descriptor, other.Descriptor))
            {
                return EqualityComparer<Diagnostic>.Default.Equals(SuppressedDiagnostic, other.SuppressedDiagnostic);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(EqualityComparer<SuppressionDescriptor>.Default.GetHashCode(Descriptor), EqualityComparer<Diagnostic>.Default.GetHashCode(SuppressedDiagnostic));
        }
    }
}
