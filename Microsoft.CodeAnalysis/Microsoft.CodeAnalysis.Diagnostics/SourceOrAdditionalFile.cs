using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    public readonly struct SourceOrAdditionalFile : IEquatable<SourceOrAdditionalFile>
    {
        public SyntaxTree? SourceTree { get; }

        public AdditionalText? AdditionalFile { get; }

        public SourceOrAdditionalFile(SyntaxTree tree)
        {
            SourceTree = tree;
            AdditionalFile = null;
        }

        public SourceOrAdditionalFile(AdditionalText file)
        {
            AdditionalFile = file;
            SourceTree = null;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SourceOrAdditionalFile other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(SourceOrAdditionalFile other)
        {
            if (SourceTree == other.SourceTree)
            {
                return AdditionalFile == other.AdditionalFile;
            }
            return false;
        }

        public static bool operator ==(SourceOrAdditionalFile left, SourceOrAdditionalFile right)
        {
            return object.Equals(left, right);
        }

        public static bool operator !=(SourceOrAdditionalFile left, SourceOrAdditionalFile right)
        {
            return !object.Equals(left, right);
        }

        public override int GetHashCode()
        {
            if (SourceTree != null)
            {
                return Hash.Combine(newKeyPart: true, SourceTree!.GetHashCode());
            }
            return Hash.Combine(newKeyPart: false, AdditionalFile!.GetHashCode());
        }
    }
}
