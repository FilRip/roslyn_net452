namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SemanticModelBinder : Binder
	{
		private readonly bool _ignoreAccessibility;

		public sealed override bool IsSemanticModelBinder => true;

		protected SemanticModelBinder(Binder containingBinder, bool ignoreAccessibility = false)
			: base(containingBinder)
		{
			_ignoreAccessibility = false;
			_ignoreAccessibility = ignoreAccessibility;
		}

		public static Binder Mark(Binder binder, bool ignoreAccessibility = false)
		{
			if (!binder.IsSemanticModelBinder || binder.IgnoresAccessibility != ignoreAccessibility)
			{
				return new SemanticModelBinder(binder, ignoreAccessibility);
			}
			return binder;
		}

		internal override LookupOptions BinderSpecificLookupOptions(LookupOptions options)
		{
			if (_ignoreAccessibility)
			{
				return base.BinderSpecificLookupOptions(options) | LookupOptions.IgnoreAccessibility;
			}
			return base.BinderSpecificLookupOptions(options);
		}
	}
}
