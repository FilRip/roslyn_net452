using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class PropertySignatureComparer : IEqualityComparer<PropertySymbol>
	{
		public static readonly PropertySignatureComparer RuntimePropertySignatureComparer = new PropertySignatureComparer(considerName: true, considerType: true, considerReadWriteModifiers: false, considerOptionalParameters: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly PropertySignatureComparer RetargetedExplicitPropertyImplementationComparer = new PropertySignatureComparer(considerName: true, considerType: true, considerReadWriteModifiers: true, considerOptionalParameters: true, considerCustomModifiers: true, considerTupleNames: false);

		public static readonly PropertySignatureComparer WinRTConflictComparer = new PropertySignatureComparer(considerName: true, considerType: false, considerReadWriteModifiers: false, considerOptionalParameters: false, considerCustomModifiers: false, considerTupleNames: false);

		private readonly bool _considerName;

		private readonly bool _considerType;

		private readonly bool _considerReadWriteModifiers;

		private readonly bool _considerOptionalParameters;

		private readonly bool _considerCustomModifiers;

		private readonly bool _considerTupleNames;

		private PropertySignatureComparer(bool considerName, bool considerType, bool considerReadWriteModifiers, bool considerOptionalParameters, bool considerCustomModifiers, bool considerTupleNames)
		{
			_considerName = considerName;
			_considerType = considerType;
			_considerReadWriteModifiers = considerReadWriteModifiers;
			_considerOptionalParameters = considerOptionalParameters;
			_considerCustomModifiers = considerCustomModifiers;
			_considerTupleNames = considerTupleNames;
		}

		public bool Equals(PropertySymbol prop1, PropertySymbol prop2)
		{
			if (prop1 == prop2)
			{
				return true;
			}
			if ((object)prop1 == null || (object)prop2 == null)
			{
				return false;
			}
			if (_considerName && !CaseInsensitiveComparison.Equals(prop1.Name, prop2.Name))
			{
				return false;
			}
			if (_considerReadWriteModifiers && (prop1.IsReadOnly != prop2.IsReadOnly || prop1.IsWriteOnly != prop2.IsWriteOnly))
			{
				return false;
			}
			if (_considerType)
			{
				TypeCompareKind comparison = MethodSignatureComparer.MakeTypeCompareKind(_considerCustomModifiers, _considerTupleNames);
				if (!HaveSameTypes(prop1, prop2, comparison))
				{
					return false;
				}
			}
			if ((prop1.ParameterCount > 0 || prop2.ParameterCount > 0) && !MethodSignatureComparer.HaveSameParameterTypes(prop1.Parameters, null, prop2.Parameters, null, considerByRef: false, _considerCustomModifiers, _considerTupleNames))
			{
				return false;
			}
			return true;
		}

		bool IEqualityComparer<PropertySymbol>.Equals(PropertySymbol prop1, PropertySymbol prop2)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(prop1, prop2);
		}

		public int GetHashCode(PropertySymbol prop)
		{
			int num = 1;
			if ((object)prop != null)
			{
				if (_considerName)
				{
					num = Hash.Combine(prop.Name, num);
				}
				if (_considerType && !_considerCustomModifiers)
				{
					num = Hash.Combine(prop.Type, num);
				}
				num = Hash.Combine(num, prop.ParameterCount);
			}
			return num;
		}

		int IEqualityComparer<PropertySymbol>.GetHashCode(PropertySymbol prop)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
			return this.GetHashCode(prop);
		}

		public static SymbolComparisonResults DetailedCompare(PropertySymbol prop1, PropertySymbol prop2, SymbolComparisonResults comparisons, SymbolComparisonResults stopIfAny = (SymbolComparisonResults)0)
		{
			SymbolComparisonResults symbolComparisonResults = (SymbolComparisonResults)0;
			if (prop1 == prop2)
			{
				return (SymbolComparisonResults)0;
			}
			if ((comparisons & SymbolComparisonResults.PropertyAccessorMismatch) != 0)
			{
				if (prop1.IsReadOnly != prop2.IsReadOnly || prop1.IsWriteOnly != prop2.IsWriteOnly)
				{
					symbolComparisonResults |= SymbolComparisonResults.PropertyAccessorMismatch;
					if ((stopIfAny & SymbolComparisonResults.PropertyAccessorMismatch) != 0)
					{
						goto IL_019b;
					}
				}
				if ((comparisons & SymbolComparisonResults.PropertyInitOnlyMismatch) != 0)
				{
					bool? flag = prop1.SetMethod?.IsInitOnly;
					bool? flag2 = prop2.SetMethod?.IsInitOnly;
					if (((flag.HasValue & flag2.HasValue) ? new bool?(flag.GetValueOrDefault() != flag2.GetValueOrDefault()) : null).GetValueOrDefault())
					{
						symbolComparisonResults |= SymbolComparisonResults.PropertyInitOnlyMismatch;
						if ((stopIfAny & SymbolComparisonResults.PropertyInitOnlyMismatch) != 0)
						{
							goto IL_019b;
						}
					}
				}
			}
			if ((comparisons & (SymbolComparisonResults)131106) != 0)
			{
				symbolComparisonResults |= MethodSignatureComparer.DetailedReturnTypeCompare(prop1.ReturnsByRef, new TypeWithModifiers(prop1.Type, prop1.TypeCustomModifiers), prop1.RefCustomModifiers, null, prop2.ReturnsByRef, new TypeWithModifiers(prop2.Type, prop2.TypeCustomModifiers), prop2.RefCustomModifiers, null, comparisons, stopIfAny);
				if ((stopIfAny & symbolComparisonResults) != 0)
				{
					goto IL_019b;
				}
			}
			if ((comparisons & SymbolComparisonResults.AllParameterMismatches) != 0)
			{
				SymbolComparisonResults num = symbolComparisonResults;
				ImmutableArray<ParameterSymbol> parameters = prop1.Parameters;
				MethodSignatureComparer.LazyTypeSubstitution lazyTypeSubstitution = default(MethodSignatureComparer.LazyTypeSubstitution);
				ImmutableArray<ParameterSymbol> parameters2 = prop2.Parameters;
				MethodSignatureComparer.LazyTypeSubstitution lazyTypeSubstitution2 = default(MethodSignatureComparer.LazyTypeSubstitution);
				symbolComparisonResults = num | MethodSignatureComparer.DetailedParameterCompare(parameters, ref lazyTypeSubstitution, parameters2, ref lazyTypeSubstitution2, comparisons, stopIfAny);
				if ((stopIfAny & symbolComparisonResults) != 0)
				{
					goto IL_019b;
				}
			}
			if ((comparisons & SymbolComparisonResults.NameMismatch) != 0 && !CaseInsensitiveComparison.Equals(prop1.Name, prop2.Name))
			{
				symbolComparisonResults |= SymbolComparisonResults.NameMismatch;
				_ = stopIfAny & SymbolComparisonResults.NameMismatch;
			}
			goto IL_019b;
			IL_019b:
			return symbolComparisonResults & comparisons;
		}

		private static bool HaveSameTypes(PropertySymbol prop1, PropertySymbol prop2, TypeCompareKind comparison)
		{
			if (prop1.ReturnsByRef != prop2.ReturnsByRef)
			{
				return false;
			}
			TypeSymbol type = prop1.Type;
			TypeSymbol type2 = prop2.Type;
			if ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && (!prop1.TypeCustomModifiers.SequenceEqual(prop2.TypeCustomModifiers) || !prop1.RefCustomModifiers.SequenceEqual(prop2.RefCustomModifiers)))
			{
				return false;
			}
			return TypeSymbolExtensions.IsSameType(type, type2, comparison);
		}
	}
}
