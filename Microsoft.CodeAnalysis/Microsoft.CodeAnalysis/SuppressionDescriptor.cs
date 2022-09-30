using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class SuppressionDescriptor : IEquatable<SuppressionDescriptor?>
    {
        public string Id { get; }

        public string SuppressedDiagnosticId { get; }

        public LocalizableString Justification { get; }

        public SuppressionDescriptor(string id, string suppressedDiagnosticId, string justification)
            : this(id, suppressedDiagnosticId, (LocalizableString)justification)
        {
        }

        public SuppressionDescriptor(string id, string suppressedDiagnosticId, LocalizableString justification)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(CodeAnalysisResources.SuppressionIdCantBeNullOrWhitespace, "id");
            }
            if (string.IsNullOrWhiteSpace(suppressedDiagnosticId))
            {
                throw new ArgumentException(CodeAnalysisResources.DiagnosticIdCantBeNullOrWhitespace, "suppressedDiagnosticId");
            }
            Id = id;
            SuppressedDiagnosticId = suppressedDiagnosticId;
            Justification = justification ?? throw new ArgumentNullException("justification");
        }

        public bool Equals(SuppressionDescriptor? other)
        {
            if (this == other)
            {
                return true;
            }
            if (other != null && Id == other!.Id && SuppressedDiagnosticId == other!.SuppressedDiagnosticId)
            {
                return Justification.Equals(other!.Justification);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SuppressionDescriptor);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Id.GetHashCode(), Hash.Combine(SuppressedDiagnosticId.GetHashCode(), Justification.GetHashCode()));
        }

        internal bool IsDisabled(CompilationOptions compilationOptions)
        {
            if (compilationOptions == null)
            {
                throw new ArgumentNullException("compilationOptions");
            }
            if (compilationOptions.SpecificDiagnosticOptions.TryGetValue(Id, out var value))
            {
                return value == ReportDiagnostic.Suppress;
            }
            return false;
        }
    }
}
