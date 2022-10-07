using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public struct LocalSlotDebugInfo : IEquatable<LocalSlotDebugInfo>
    {
        public readonly SynthesizedLocalKind SynthesizedKind;

        public readonly LocalDebugId Id;

        public LocalSlotDebugInfo(SynthesizedLocalKind synthesizedKind, LocalDebugId id)
        {
            SynthesizedKind = synthesizedKind;
            Id = id;
        }

        public bool Equals(LocalSlotDebugInfo other)
        {
            if (SynthesizedKind == other.SynthesizedKind)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LocalSlotDebugInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public override int GetHashCode()
        {
            SynthesizedLocalKind synthesizedKind = SynthesizedKind;
            LocalDebugId id = Id;
            return Hash.Combine((int)synthesizedKind, id.GetHashCode());
        }

        public override string ToString()
        {
            string text = SynthesizedKind.ToString();
            LocalDebugId id = Id;
            return text + " " + id.ToString();
        }
    }
}
