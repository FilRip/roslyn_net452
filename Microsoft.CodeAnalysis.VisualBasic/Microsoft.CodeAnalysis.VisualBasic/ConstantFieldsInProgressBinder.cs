using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ConstantFieldsInProgressBinder : Binder
	{
		private readonly ConstantFieldsInProgress _inProgress;

		private readonly FieldSymbol _field;

		internal override ConstantFieldsInProgress ConstantFieldsInProgress => _inProgress;

		public override Symbol ContainingMember => _field;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		internal ConstantFieldsInProgressBinder(ConstantFieldsInProgress inProgress, Binder next, FieldSymbol field)
			: base(next)
		{
			_inProgress = inProgress;
			_field = field;
		}
	}
}
