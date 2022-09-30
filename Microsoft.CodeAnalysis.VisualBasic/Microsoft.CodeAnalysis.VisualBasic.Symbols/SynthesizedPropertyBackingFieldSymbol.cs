namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SynthesizedPropertyBackingFieldSymbol : SynthesizedBackingFieldBase<SourcePropertySymbol>
	{
		public override TypeSymbol Type => _propertyOrEvent.Type;

		public SynthesizedPropertyBackingFieldSymbol(SourcePropertySymbol property, string name, bool isShared)
			: base(property, name, isShared)
		{
		}
	}
}
