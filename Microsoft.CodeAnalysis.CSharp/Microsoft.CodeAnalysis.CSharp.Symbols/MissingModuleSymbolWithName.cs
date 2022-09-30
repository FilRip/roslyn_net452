using System;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class MissingModuleSymbolWithName : MissingModuleSymbol
    {
        private readonly string _name;

        public override string Name => _name;

        public MissingModuleSymbolWithName(AssemblySymbol assembly, string name)
            : base(assembly, -1)
        {
            _name = name;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(assembly.GetHashCode(), StringComparer.OrdinalIgnoreCase.GetHashCode(_name));
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if ((object)this == obj)
            {
                return true;
            }
            if (obj is MissingModuleSymbolWithName missingModuleSymbolWithName && assembly.Equals(missingModuleSymbolWithName.assembly, compareKind))
            {
                return string.Equals(_name, missingModuleSymbolWithName._name, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
