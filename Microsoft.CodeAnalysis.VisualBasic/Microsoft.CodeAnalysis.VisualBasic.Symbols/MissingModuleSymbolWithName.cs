using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MissingModuleSymbolWithName : MissingModuleSymbol
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
			return Hash.Combine(m_Assembly.GetHashCode(), StringComparer.OrdinalIgnoreCase.GetHashCode(_name));
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			return obj is MissingModuleSymbolWithName missingModuleSymbolWithName && m_Assembly.Equals(missingModuleSymbolWithName.m_Assembly) && string.Equals(_name, missingModuleSymbolWithName._name, StringComparison.OrdinalIgnoreCase);
		}
	}
}
