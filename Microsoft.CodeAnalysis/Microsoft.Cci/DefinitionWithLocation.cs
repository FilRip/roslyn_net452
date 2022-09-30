using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.Cci
{
    [DebuggerDisplay("{GetDebuggerDisplay(),nq}")]
    public struct DefinitionWithLocation : IEquatable<DefinitionWithLocation>
    {
        public readonly IDefinition Definition;

        public readonly int StartLine;

        public readonly int StartColumn;

        public readonly int EndLine;

        public readonly int EndColumn;

        public DefinitionWithLocation(IDefinition definition, int startLine, int startColumn, int endLine, int endColumn)
        {
            Definition = definition;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        private string GetDebuggerDisplay()
        {
            return $"{Definition} => ({StartLine},{StartColumn}) - ({EndLine}, {EndColumn})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is DefinitionWithLocation other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(DefinitionWithLocation other)
        {
            if (Definition == other.Definition && StartLine == other.StartLine && StartColumn == other.StartColumn && EndLine == other.EndLine)
            {
                return EndColumn == other.EndColumn;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(RuntimeHelpers.GetHashCode(Definition), StartLine.GetHashCode());
        }
    }
}
