using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Syntax
{
    public abstract class AbstractWarningStateMap<TWarningState> where TWarningState : struct
    {
        protected readonly struct WarningStateMapEntry : IComparable<WarningStateMapEntry>
        {
            public readonly int Position;

            public readonly TWarningState GeneralWarningOption;

            public readonly ImmutableDictionary<string, TWarningState> SpecificWarningOption;

            public WarningStateMapEntry(int position)
            {
                Position = position;
                GeneralWarningOption = default;
                SpecificWarningOption = ImmutableDictionary.Create<string, TWarningState>();
            }

            public WarningStateMapEntry(int position, TWarningState general, ImmutableDictionary<string, TWarningState> specific)
            {
                Position = position;
                GeneralWarningOption = general;
                SpecificWarningOption = specific ?? ImmutableDictionary.Create<string, TWarningState>();
            }

            public int CompareTo(WarningStateMapEntry other)
            {
                return Position - other.Position;
            }
        }

        private readonly WarningStateMapEntry[] _warningStateMapEntries;

        protected AbstractWarningStateMap(SyntaxTree syntaxTree)
        {
            _warningStateMapEntries = CreateWarningStateMapEntries(syntaxTree);
        }

        protected abstract WarningStateMapEntry[] CreateWarningStateMapEntries(SyntaxTree syntaxTree);

        public TWarningState GetWarningState(string id, int position)
        {
            WarningStateMapEntry entryAtOrBeforePosition = GetEntryAtOrBeforePosition(position);
            if (entryAtOrBeforePosition.SpecificWarningOption.TryGetValue(id, out var value))
            {
                return value;
            }
            return entryAtOrBeforePosition.GeneralWarningOption;
        }

        private WarningStateMapEntry GetEntryAtOrBeforePosition(int position)
        {
            int num = Array.BinarySearch(_warningStateMapEntries, new WarningStateMapEntry(position));
            return _warningStateMapEntries[(num >= 0) ? num : (~num - 1)];
        }
    }
}
