namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedRemoveAccessorSymbol : SynthesizedEventAccessorSymbol
	{
		public override MethodKind MethodKind => MethodKind.EventRemove;

		public SynthesizedRemoveAccessorSymbol(SourceMemberContainerTypeSymbol container, SourceEventSymbol @event)
			: base(container, @event)
		{
		}
	}
}
