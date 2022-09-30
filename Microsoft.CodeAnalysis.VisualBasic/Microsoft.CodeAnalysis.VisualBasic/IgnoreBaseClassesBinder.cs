namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class IgnoreBaseClassesBinder : Binder
	{
		public IgnoreBaseClassesBinder(Binder containingBinder)
			: base(containingBinder, null, true)
		{
		}
	}
}
