using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public sealed class LocalDefinition : ILocalDefinition, INamedEntity
    {
        private readonly ILocalSymbolInternal? _symbolOpt;

        private readonly string? _nameOpt;

        private readonly ITypeReference _type;

        private readonly LocalSlotConstraints _constraints;

        private readonly int _slot;

        private readonly LocalSlotDebugInfo _slotInfo;

        private readonly LocalVariableAttributes _pdbAttributes;

        private readonly ImmutableArray<bool> _dynamicTransformFlags;

        private readonly ImmutableArray<string> _tupleElementNames;

        public ILocalSymbolInternal? SymbolOpt => _symbolOpt;

        public Location Location
        {
            get
            {
                if (_symbolOpt != null)
                {
                    ImmutableArray<Location> locations = _symbolOpt!.Locations;
                    if (!locations.IsDefaultOrEmpty)
                    {
                        return locations[0];
                    }
                }
                return Microsoft.CodeAnalysis.Location.None;
            }
        }

        public int SlotIndex => _slot;

        public MetadataConstant CompileTimeValue
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public ImmutableArray<ICustomModifier> CustomModifiers => ImmutableArray<ICustomModifier>.Empty;

        public bool IsConstant
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public bool IsModified => false;

        public LocalSlotConstraints Constraints => _constraints;

        public bool IsPinned => (_constraints & LocalSlotConstraints.Pinned) != 0;

        public bool IsReference => (_constraints & LocalSlotConstraints.ByRef) != 0;

        public LocalVariableAttributes PdbAttributes => _pdbAttributes;

        public ImmutableArray<bool> DynamicTransformFlags => _dynamicTransformFlags;

        public ImmutableArray<string> TupleElementNames => _tupleElementNames;

        public ITypeReference Type => _type;

        public string? Name => _nameOpt;

        public byte[]? Signature => null;

        public LocalSlotDebugInfo SlotInfo => _slotInfo;

        public LocalDefinition(ILocalSymbolInternal? symbolOpt, string? nameOpt, ITypeReference type, int slot, SynthesizedLocalKind synthesizedKind, LocalDebugId id, LocalVariableAttributes pdbAttributes, LocalSlotConstraints constraints, ImmutableArray<bool> dynamicTransformFlags, ImmutableArray<string> tupleElementNames)
        {
            _symbolOpt = symbolOpt;
            _nameOpt = nameOpt;
            _type = type;
            _slot = slot;
            _slotInfo = new LocalSlotDebugInfo(synthesizedKind, id);
            _pdbAttributes = pdbAttributes;
            _dynamicTransformFlags = dynamicTransformFlags.NullToEmpty();
            _tupleElementNames = tupleElementNames.NullToEmpty();
            _constraints = constraints;
        }

        internal string GetDebuggerDisplay()
        {
            return string.Format("{0}: {1} ({2})", _slot, _nameOpt ?? "<unnamed>", _type);
        }
    }
}
