using System;

using Microsoft.CodeAnalysis.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public sealed class MetadataLocation : Location, IEquatable<MetadataLocation?>
    {
        private readonly IModuleSymbolInternal _module;

        public override LocationKind Kind => LocationKind.MetadataFile;

        internal override IModuleSymbolInternal MetadataModuleInternal => _module;

        public MetadataLocation(IModuleSymbolInternal module)
        {
            _module = module;
        }

        public override int GetHashCode()
        {
            return _module.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MetadataLocation);
        }

        public bool Equals(MetadataLocation? other)
        {
            if ((object)other != null)
            {
                return other!._module == _module;
            }
            return false;
        }
    }
}
