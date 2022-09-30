using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LocationSpecificBinder : Binder
	{
		private readonly BindingLocation _location;

		private readonly Symbol _owner;

		public override BindingLocation BindingLocation => _location;

		public override Symbol ContainingMember => _owner ?? base.ContainingMember;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public LocationSpecificBinder(BindingLocation location, Binder containingBinder)
			: this(location, null, containingBinder)
		{
		}

		public LocationSpecificBinder(BindingLocation location, Symbol owner, Binder containingBinder)
			: base(containingBinder)
		{
			_location = location;
			_owner = owner;
		}
	}
}
