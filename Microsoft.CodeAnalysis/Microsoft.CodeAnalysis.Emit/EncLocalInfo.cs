using System;
using System.Diagnostics;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.Emit
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public readonly struct EncLocalInfo : IEquatable<EncLocalInfo>
    {
        public readonly LocalSlotDebugInfo SlotInfo;

        public readonly ITypeReference? Type;

        public readonly LocalSlotConstraints Constraints;

        public readonly byte[]? Signature;

        public readonly bool IsUnused;

        public bool IsDefault
        {
            get
            {
                if (Type == null)
                {
                    return Signature == null;
                }
                return false;
            }
        }

        public EncLocalInfo(byte[] signature)
        {
            SlotInfo = new LocalSlotDebugInfo(SynthesizedLocalKind.EmitterTemp, LocalDebugId.None);
            Type = null;
            Constraints = LocalSlotConstraints.None;
            Signature = signature;
            IsUnused = true;
        }

        public EncLocalInfo(LocalSlotDebugInfo slotInfo, ITypeReference type, LocalSlotConstraints constraints, byte[]? signature)
        {
            SlotInfo = slotInfo;
            Type = type;
            Constraints = constraints;
            Signature = signature;
            IsUnused = false;
        }

        public bool Equals(EncLocalInfo other)
        {
            if (SlotInfo.Equals(other.SlotInfo) && SymbolEquivalentEqualityComparer.Instance.Equals(Type, other.Type) && Constraints == other.Constraints)
            {
                return IsUnused == other.IsUnused;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EncLocalInfo other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            LocalSlotDebugInfo slotInfo = SlotInfo;
            return Hash.Combine(slotInfo.GetHashCode(), Hash.Combine(SymbolEquivalentEqualityComparer.Instance.GetHashCode(Type), Hash.Combine((int)Constraints, Hash.Combine(IsUnused, 0))));
        }

        private string GetDebuggerDisplay()
        {
            if (IsDefault)
            {
                return "[default]";
            }
            if (IsUnused)
            {
                return "[invalid]";
            }
            return string.Format("[Id={0}, SynthesizedKind={1}, Type={2}, Constraints={3}, Sig={4}]", SlotInfo.Id, SlotInfo.SynthesizedKind, Type, Constraints, (Signature != null) ? BitConverter.ToString(Signature) : "null");
        }
    }
}
