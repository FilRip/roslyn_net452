using System;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TuplePropertySymbol : WrappedPropertySymbol
	{
		private readonly TupleTypeSymbol _containingType;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		public override bool IsTupleProperty => true;

		public override PropertySymbol TupleUnderlyingProperty => _underlyingProperty;

		public override TypeSymbol Type => _underlyingProperty.Type;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => _underlyingProperty.TypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _underlyingProperty.RefCustomModifiers;

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					InterlockedOperations.Initialize(ref _lazyParameters, CreateParameters());
				}
				return _lazyParameters;
			}
		}

		public override MethodSymbol GetMethod => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingProperty.GetMethod);

		public override MethodSymbol SetMethod => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingProperty.SetMethod);

		internal override FieldSymbol AssociatedField => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingProperty.AssociatedField);

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => _underlyingProperty.ExplicitInterfaceImplementations;

		public override Symbol ContainingSymbol => _containingType;

		public override bool IsOverloads => _underlyingProperty.IsOverloads;

		internal override bool IsMyGroupCollectionProperty => _underlyingProperty.IsMyGroupCollectionProperty;

		public TuplePropertySymbol(TupleTypeSymbol container, PropertySymbol underlyingProperty)
			: base(underlyingProperty)
		{
			_containingType = container;
		}

		private ImmutableArray<ParameterSymbol> CreateParameters()
		{
			return _underlyingProperty.Parameters.SelectAsArray((Func<ParameterSymbol, ParameterSymbol>)((ParameterSymbol p) => new TupleParameterSymbol(this, p)));
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = base.GetUseSiteInfo();
			MergeUseSiteInfo(useSiteInfo, _underlyingProperty.GetUseSiteInfo());
			return useSiteInfo;
		}

		public override int GetHashCode()
		{
			return _underlyingProperty.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TuplePropertySymbol);
		}

		public bool Equals(TuplePropertySymbol other)
		{
			if ((object)other != this)
			{
				if ((object)other != null && TypeSymbol.Equals(_containingType, other._containingType, TypeCompareKind.ConsiderEverything))
				{
					return _underlyingProperty == other._underlyingProperty;
				}
				return false;
			}
			return true;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingProperty.GetAttributes();
		}
	}
}
