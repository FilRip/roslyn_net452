using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ConstantFieldsInProgress
	{
		internal struct Dependencies
		{
			private readonly HashSet<SourceFieldSymbol> _builder;

			internal Dependencies(HashSet<SourceFieldSymbol> builder)
			{
				this = default(Dependencies);
				_builder = builder;
			}

			internal void Add(SourceFieldSymbol field)
			{
				_builder.Add(field);
			}

			internal bool Any()
			{
				return _builder.Count != 0;
			}

			[Conditional("DEBUG")]
			internal void Freeze()
			{
			}
		}

		private readonly SourceFieldSymbol _fieldOpt;

		private readonly Dependencies _dependencies;

		internal static readonly ConstantFieldsInProgress Empty = new ConstantFieldsInProgress(null, default(Dependencies));

		public bool IsEmpty => (object)_fieldOpt == null;

		internal ConstantFieldsInProgress(SourceFieldSymbol fieldOpt, Dependencies dependencies)
		{
			_fieldOpt = fieldOpt;
			_dependencies = dependencies;
		}

		public bool AnyDependencies()
		{
			return _dependencies.Any();
		}

		internal void AddDependency(SourceFieldSymbol field)
		{
			_dependencies.Add(field);
		}
	}
}
