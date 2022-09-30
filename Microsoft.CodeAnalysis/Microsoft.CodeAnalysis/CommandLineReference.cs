using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{Reference,nq}")]
    public struct CommandLineReference : IEquatable<CommandLineReference>
    {
        private readonly string _reference;

        private readonly MetadataReferenceProperties _properties;

        public string Reference => _reference;

        public MetadataReferenceProperties Properties => _properties;

        public CommandLineReference(string reference, MetadataReferenceProperties properties)
        {
            _reference = reference;
            _properties = properties;
        }

        public override bool Equals(object? obj)
        {
            if (obj is CommandLineReference)
            {
                return base.Equals((CommandLineReference)obj);
            }
            return false;
        }

        public bool Equals(CommandLineReference other)
        {
            if (_reference == other._reference)
            {
                return _properties.Equals(other._properties);
            }
            return false;
        }

        public override int GetHashCode()
        {
            string reference = _reference;
            MetadataReferenceProperties properties = _properties;
            return Hash.Combine(reference, properties.GetHashCode());
        }
    }
}
