namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedPropertyAccessorBase<T> : SynthesizedAccessor<T> where T : PropertySymbol
	{
		internal abstract FieldSymbol BackingFieldSymbol { get; }

		protected SynthesizedPropertyAccessorBase(NamedTypeSymbol container, T property)
			: base(container, property)
		{
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			return SynthesizedPropertyAccessorHelper.GetBoundMethodBody(this, BackingFieldSymbol, ref methodBodyBinder);
		}
	}
}
