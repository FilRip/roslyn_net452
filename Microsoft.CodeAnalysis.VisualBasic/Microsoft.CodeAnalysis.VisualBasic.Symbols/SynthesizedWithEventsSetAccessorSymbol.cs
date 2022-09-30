using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedWithEventsSetAccessorSymbol : SynthesizedWithEventsAccessorSymbol
	{
		private readonly TypeSymbol _returnType;

		private readonly string _valueParameterName;

		public override bool IsSub => true;

		public override MethodKind MethodKind => MethodKind.PropertySet;

		public override TypeSymbol ReturnType => _returnType;

		internal override MethodImplAttributes ImplementationAttributes
		{
			get
			{
				MethodImplAttributes methodImplAttributes = base.ImplementationAttributes;
				if (((PropertySymbol)base.AssociatedSymbol).IsWithEvents)
				{
					methodImplAttributes |= MethodImplAttributes.Synchronized;
				}
				return methodImplAttributes;
			}
		}

		public SynthesizedWithEventsSetAccessorSymbol(SourceMemberContainerTypeSymbol container, PropertySymbol propertySymbol, TypeSymbol returnType, string valueParameterName)
			: base(container, propertySymbol)
		{
			_returnType = returnType;
			_valueParameterName = valueParameterName;
		}

		protected override ImmutableArray<ParameterSymbol> GetParameters()
		{
			PropertySymbol containingProperty = base.ContainingProperty;
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(containingProperty.ParameterCount + 1);
			containingProperty.CloneParameters(this, instance);
			instance.Add(SynthesizedParameterSymbol.CreateSetAccessorValueParameter(this, containingProperty, _valueParameterName));
			return instance.ToImmutableAndFree();
		}
	}
}
