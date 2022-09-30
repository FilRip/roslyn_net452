using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedWithEventsGetAccessorSymbol : SynthesizedWithEventsAccessorSymbol
	{
		public override bool IsSub => false;

		public override MethodKind MethodKind => MethodKind.PropertyGet;

		public override TypeSymbol ReturnType => m_propertyOrEvent.Type;

		public SynthesizedWithEventsGetAccessorSymbol(SourceMemberContainerTypeSymbol container, PropertySymbol propertySymbol)
			: base(container, propertySymbol)
		{
		}

		protected override ImmutableArray<ParameterSymbol> GetParameters()
		{
			PropertySymbol containingProperty = base.ContainingProperty;
			if (containingProperty.ParameterCount > 0)
			{
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(containingProperty.ParameterCount);
				containingProperty.CloneParameters(this, instance);
				return instance.ToImmutableAndFree();
			}
			return ImmutableArray<ParameterSymbol>.Empty;
		}
	}
}
