using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal struct NamespaceExtent
	{
		private readonly NamespaceKind _kind;

		private readonly object _symbolOrCompilation;

		public NamespaceKind Kind => _kind;

		public ModuleSymbol Module
		{
			get
			{
				if (Kind == NamespaceKind.Module)
				{
					return (ModuleSymbol)_symbolOrCompilation;
				}
				throw new InvalidOperationException();
			}
		}

		public AssemblySymbol Assembly
		{
			get
			{
				if (Kind == NamespaceKind.Assembly)
				{
					return (AssemblySymbol)_symbolOrCompilation;
				}
				throw new InvalidOperationException();
			}
		}

		public VisualBasicCompilation Compilation
		{
			get
			{
				if (Kind == NamespaceKind.Compilation)
				{
					return (VisualBasicCompilation)_symbolOrCompilation;
				}
				throw new InvalidOperationException();
			}
		}

		public override string ToString()
		{
			return $"{Kind.ToString()}: {_symbolOrCompilation.ToString()}";
		}

		internal NamespaceExtent(ModuleSymbol module)
		{
			this = default(NamespaceExtent);
			_kind = NamespaceKind.Module;
			_symbolOrCompilation = module;
		}

		internal NamespaceExtent(AssemblySymbol assembly)
		{
			this = default(NamespaceExtent);
			_kind = NamespaceKind.Assembly;
			_symbolOrCompilation = assembly;
		}

		internal NamespaceExtent(VisualBasicCompilation compilation)
		{
			this = default(NamespaceExtent);
			_kind = NamespaceKind.Compilation;
			_symbolOrCompilation = compilation;
		}
	}
}
