namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedAddAccessorSymbol : SynthesizedEventAccessorSymbol
	{
		public override MethodKind MethodKind => MethodKind.EventAdd;

		public SynthesizedAddAccessorSymbol(SourceMemberContainerTypeSymbol container, SourceEventSymbol @event)
			: base(container, @event)
		{
		}
	}
}
