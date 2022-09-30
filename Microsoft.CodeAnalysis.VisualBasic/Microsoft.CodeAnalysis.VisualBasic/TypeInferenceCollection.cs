using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class TypeInferenceCollection<TDominantTypeData> where TDominantTypeData : DominantTypeData
	{
		public enum HintSatisfaction
		{
			ThroughIdentity,
			ThroughWidening,
			ThroughNarrowing,
			Unsatisfied,
			Count
		}

		private readonly ArrayBuilder<TDominantTypeData> _dominantTypeDataList;

		public TypeInferenceCollection()
		{
			_dominantTypeDataList = new ArrayBuilder<TDominantTypeData>();
		}

		public ArrayBuilder<TDominantTypeData> GetTypeDataList()
		{
			return _dominantTypeDataList;
		}

		internal void FindDominantType(ArrayBuilder<TDominantTypeData> resultList, ref InferenceErrorReasons inferenceErrorReasons, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			resultList.Clear();
			TDominantTypeData item = null;
			TDominantTypeData item2 = null;
			int num = 0;
			int num2 = 0;
			ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance(4, 0);
			ArrayBuilder<TDominantTypeData>.Enumerator enumerator = _dominantTypeDataList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TDominantTypeData current = enumerator.Current;
				if ((object)current.ResultType == null)
				{
					current.IsStrictCandidate = false;
					current.IsUnstrictCandidate = false;
					continue;
				}
				instance.ZeroInit(4);
				ArrayBuilder<TDominantTypeData>.Enumerator enumerator2 = _dominantTypeDataList.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TDominantTypeData current2 = enumerator2.Current;
					instance[(int)CheckHintSatisfaction(current, current2, current2.InferenceRestrictions, ref useSiteInfo)]++;
				}
				current.IsStrictCandidate = instance[3] == 0 && instance[2] == 0;
				current.IsUnstrictCandidate = instance[3] == 0;
				if (current.IsStrictCandidate)
				{
					num++;
					item = current;
				}
				if (current.IsUnstrictCandidate)
				{
					num2++;
					item2 = current;
				}
			}
			instance.Free();
			switch (num)
			{
			case 1:
				resultList.Add(item);
				return;
			case 0:
				if (num2 == 1)
				{
					resultList.Add(item2);
					return;
				}
				break;
			}
			if (num2 == 0)
			{
				inferenceErrorReasons |= InferenceErrorReasons.NoBest;
				return;
			}
			if (num == 0)
			{
				ArrayBuilder<TDominantTypeData>.Enumerator enumerator3 = _dominantTypeDataList.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					TDominantTypeData current3 = enumerator3.Current;
					if (current3.IsUnstrictCandidate)
					{
						resultList.Add(current3);
					}
				}
				inferenceErrorReasons |= InferenceErrorReasons.Ambiguous;
				return;
			}
			ArrayBuilder<TDominantTypeData>.Enumerator enumerator4 = _dominantTypeDataList.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				TDominantTypeData current4 = enumerator4.Current;
				if (!current4.IsStrictCandidate)
				{
					continue;
				}
				bool flag = true;
				ArrayBuilder<TDominantTypeData>.Enumerator enumerator5 = _dominantTypeDataList.GetEnumerator();
				while (enumerator5.MoveNext())
				{
					TDominantTypeData current5 = enumerator5.Current;
					if (!current5.IsStrictCandidate || current4 == current5 || (object)current4.ResultType == null || (object)current5.ResultType == null)
					{
						continue;
					}
					ConversionKind conv;
					if (!(current5.ResultType is ArrayLiteralTypeSymbol arrayLiteralTypeSymbol))
					{
						conv = Conversions.ClassifyConversion(current5.ResultType, current4.ResultType, ref useSiteInfo).Key;
					}
					else
					{
						BoundArrayLiteral arrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral;
						conv = Conversions.ClassifyConversion(arrayLiteral, current4.ResultType, arrayLiteral.Binder, ref useSiteInfo).Key;
						if (Conversions.IsWideningConversion(conv) && TypeSymbolExtensions.IsSameTypeIgnoringAll(arrayLiteralTypeSymbol, current4.ResultType))
						{
							conv = ConversionKind.Identity;
						}
					}
					if (!Conversions.IsWideningConversion(conv))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					resultList.Add(current4);
				}
			}
			if (resultList.Count > 1)
			{
				int num3 = -1;
				int num4 = resultList.Count - 1;
				for (int i = 0; i <= num4; i++)
				{
					if (!(resultList[i].ResultType is ArrayLiteralTypeSymbol))
					{
						num3++;
						if (num3 != i)
						{
							resultList[num3] = resultList[i];
						}
					}
				}
				if (num3 > -1)
				{
					resultList.Clip(num3 + 1);
				}
				else
				{
					TypeSymbol typeSymbol = resultList[0].ResultType;
					int num5 = resultList.Count - 1;
					for (int j = 1; j <= num5; j++)
					{
						if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(resultList[j].ResultType, typeSymbol))
						{
							typeSymbol = null;
							break;
						}
					}
					if ((object)typeSymbol != null)
					{
						resultList.Clip(1);
					}
					else
					{
						int num6 = ((ArrayLiteralTypeSymbol)resultList[0].ResultType).Rank;
						int num7 = resultList.Count - 1;
						for (int k = 1; k <= num7; k++)
						{
							if (((ArrayLiteralTypeSymbol)resultList[k].ResultType).Rank != num6)
							{
								num6 = -1;
								break;
							}
						}
						if (num6 != -1)
						{
							ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance();
							ArrayBuilder<TDominantTypeData>.Enumerator enumerator6 = resultList.GetEnumerator();
							while (enumerator6.MoveNext())
							{
								AppendArrayElements(((ArrayLiteralTypeSymbol)enumerator6.Current.ResultType).ArrayLiteral.Initializer, instance2);
							}
							BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(null, PooledHashSet<AssemblySymbol>.GetInstance());
							Binder binder = ((ArrayLiteralTypeSymbol)resultList[0].ResultType).ArrayLiteral.Binder;
							VisualBasicSyntaxNode root = VisualBasicSyntaxTree.Dummy.GetRoot();
							int numCandidates = 0;
							InferenceErrorReasons errorReasons = InferenceErrorReasons.Other;
							TypeSymbol typeSymbol2 = binder.InferDominantTypeOfExpressions(root, instance2, bindingDiagnosticBag, ref numCandidates, ref errorReasons);
							if ((object)typeSymbol2 != null)
							{
								useSiteInfo.AddDependencies(bindingDiagnosticBag.DependenciesBag);
								TDominantTypeData val = null;
								BoundArrayLiteral boundArrayLiteral = null;
								ArrayBuilder<TDominantTypeData>.Enumerator enumerator7 = resultList.GetEnumerator();
								while (enumerator7.MoveNext())
								{
									TDominantTypeData current6 = enumerator7.Current;
									ArrayLiteralTypeSymbol arrayLiteralTypeSymbol2 = (ArrayLiteralTypeSymbol)current6.ResultType;
									if (TypeSymbolExtensions.IsSameTypeIgnoringAll(arrayLiteralTypeSymbol2.ElementType, typeSymbol2))
									{
										BoundArrayLiteral arrayLiteral2 = arrayLiteralTypeSymbol2.ArrayLiteral;
										if (val == null || (arrayLiteral2.HasDominantType && (!boundArrayLiteral.HasDominantType || boundArrayLiteral.NumberOfCandidates < arrayLiteral2.NumberOfCandidates)))
										{
											val = current6;
											boundArrayLiteral = arrayLiteralTypeSymbol2.ArrayLiteral;
										}
									}
								}
								if (val != null)
								{
									resultList.Clear();
									resultList.Add(val);
								}
							}
							bindingDiagnosticBag.Free();
						}
					}
				}
			}
			if (resultList.Count == 1)
			{
				return;
			}
			if (resultList.Count > 1)
			{
				inferenceErrorReasons |= InferenceErrorReasons.Ambiguous;
				return;
			}
			ArrayBuilder<TDominantTypeData>.Enumerator enumerator8 = _dominantTypeDataList.GetEnumerator();
			while (enumerator8.MoveNext())
			{
				TDominantTypeData current7 = enumerator8.Current;
				if (current7.IsStrictCandidate)
				{
					resultList.Add(current7);
				}
			}
			inferenceErrorReasons |= InferenceErrorReasons.Ambiguous;
		}

		private static void AppendArrayElements(BoundArrayInitialization source, ArrayBuilder<BoundExpression> elements)
		{
			ImmutableArray<BoundExpression>.Enumerator enumerator = source.Initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				if (current.Kind == BoundKind.ArrayInitialization)
				{
					AppendArrayElements((BoundArrayInitialization)current, elements);
				}
				else
				{
					elements.Add(current);
				}
			}
		}

		private HintSatisfaction CheckHintSatisfaction(DominantTypeData candidateData, DominantTypeData hintData, RequiredConversion hintRestrictions, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol resultType = candidateData.ResultType;
			TypeSymbol resultType2 = hintData.ResultType;
			ConversionKind conversionKind;
			if ((object)resultType == null)
			{
				conversionKind = ConversionKind.DelegateRelaxationLevelNone;
			}
			else
			{
				switch (hintRestrictions)
				{
				case RequiredConversion.None:
					conversionKind = ConversionKind.Identity;
					break;
				case RequiredConversion.Identity:
					conversionKind = Conversions.ClassifyDirectCastConversion(resultType2, resultType, ref useSiteInfo);
					if (!Conversions.IsIdentityConversion(conversionKind))
					{
						conversionKind = ConversionKind.DelegateRelaxationLevelNone;
					}
					break;
				case RequiredConversion.Any:
				{
					if (!(resultType2 is ArrayLiteralTypeSymbol arrayLiteralTypeSymbol))
					{
						conversionKind = Conversions.ClassifyConversion(resultType2, resultType, ref useSiteInfo).Key;
						break;
					}
					BoundArrayLiteral arrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral;
					conversionKind = Conversions.ClassifyConversion(arrayLiteral, resultType, arrayLiteral.Binder, ref useSiteInfo).Key;
					if (Conversions.IsWideningConversion(conversionKind))
					{
						if (TypeSymbolExtensions.IsSameTypeIgnoringAll(arrayLiteralTypeSymbol, resultType))
						{
							conversionKind = ConversionKind.Identity;
						}
						else if ((conversionKind & ConversionKind.InvolvesNarrowingFromNumericConstant) != 0)
						{
							conversionKind = ConversionKind.Narrowing;
						}
					}
					break;
				}
				case RequiredConversion.AnyReverse:
					conversionKind = Conversions.ClassifyConversion(resultType, resultType2, ref useSiteInfo).Key;
					break;
				case RequiredConversion.AnyAndReverse:
				{
					ConversionKind key = Conversions.ClassifyConversion(resultType2, resultType, ref useSiteInfo).Key;
					ConversionKind key2 = Conversions.ClassifyConversion(resultType, resultType2, ref useSiteInfo).Key;
					conversionKind = ((!Conversions.NoConversion(key) && !Conversions.NoConversion(key2)) ? ((!Conversions.IsNarrowingConversion(key) && !Conversions.IsNarrowingConversion(key2)) ? ((!Conversions.IsIdentityConversion(key) || !Conversions.IsIdentityConversion(key2)) ? ConversionKind.Widening : ConversionKind.Identity) : ConversionKind.Narrowing) : ConversionKind.DelegateRelaxationLevelNone);
					break;
				}
				case RequiredConversion.ArrayElement:
					conversionKind = Conversions.ClassifyArrayElementConversion(resultType2, resultType, ref useSiteInfo);
					break;
				case RequiredConversion.Reference:
					if (!resultType2.IsReferenceType || !resultType.IsReferenceType)
					{
						conversionKind = (TypeSymbolExtensions.IsSameTypeIgnoringAll(resultType2, resultType) ? ConversionKind.Identity : ConversionKind.DelegateRelaxationLevelNone);
						break;
					}
					conversionKind = Conversions.ClassifyDirectCastConversion(resultType2, resultType, ref useSiteInfo);
					if (Conversions.IsNarrowingConversion(conversionKind))
					{
						conversionKind = ConversionKind.DelegateRelaxationLevelNone;
					}
					break;
				case RequiredConversion.ReverseReference:
					if (!resultType2.IsReferenceType || !resultType.IsReferenceType)
					{
						conversionKind = (TypeSymbolExtensions.IsSameTypeIgnoringAll(resultType, resultType2) ? ConversionKind.Identity : ConversionKind.DelegateRelaxationLevelNone);
						break;
					}
					conversionKind = Conversions.ClassifyDirectCastConversion(resultType, resultType2, ref useSiteInfo);
					if (Conversions.IsNarrowingConversion(conversionKind))
					{
						conversionKind = ConversionKind.DelegateRelaxationLevelNone;
					}
					break;
				default:
					conversionKind = ConversionKind.DelegateRelaxationLevelNone;
					break;
				}
			}
			if (Conversions.NoConversion(conversionKind))
			{
				return HintSatisfaction.Unsatisfied;
			}
			if (Conversions.IsNarrowingConversion(conversionKind))
			{
				return HintSatisfaction.ThroughNarrowing;
			}
			if (Conversions.IsIdentityConversion(conversionKind))
			{
				return HintSatisfaction.ThroughIdentity;
			}
			if (Conversions.IsWideningConversion(conversionKind))
			{
				return HintSatisfaction.ThroughWidening;
			}
			return HintSatisfaction.Unsatisfied;
		}
	}
	internal class TypeInferenceCollection : TypeInferenceCollection<DominantTypeData>
	{
		public void AddType(TypeSymbol type, RequiredConversion conversion, BoundExpression sourceExpression)
		{
			if (TypeSymbolExtensions.IsVoidType(type) || TypeSymbolExtensions.IsErrorType(type))
			{
				return;
			}
			bool flag = false;
			if (!(type is ArrayLiteralTypeSymbol))
			{
				ArrayBuilder<DominantTypeData>.Enumerator enumerator = GetTypeDataList().GetEnumerator();
				while (enumerator.MoveNext())
				{
					DominantTypeData current = enumerator.Current;
					if (!(current.ResultType is ArrayLiteralTypeSymbol) && TypeSymbolExtensions.IsSameTypeIgnoringAll(type, current.ResultType))
					{
						current.ResultType = MergeTupleNames(type, current.ResultType);
						current.InferenceRestrictions = Conversions.CombineConversionRequirements(current.InferenceRestrictions, conversion);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				DominantTypeData dominantTypeData = new DominantTypeData();
				dominantTypeData.ResultType = type;
				dominantTypeData.InferenceRestrictions = conversion;
				GetTypeDataList().Add(dominantTypeData);
			}
		}

		internal static TypeSymbol MergeTupleNames(TypeSymbol first, TypeSymbol second)
		{
			if (TypeSymbolExtensions.IsSameType(first, second, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) || !TypeSymbolExtensions.ContainsTupleNames(first))
			{
				return first;
			}
			if (!TypeSymbolExtensions.ContainsTupleNames(second))
			{
				return second;
			}
			ImmutableArray<string> self = VisualBasicCompilation.TupleNamesEncoder.Encode(first);
			ImmutableArray<string> other = VisualBasicCompilation.TupleNamesEncoder.Encode(second);
			ImmutableArray<string> immutableArray;
			if (self.IsDefault || other.IsDefault)
			{
				immutableArray = default(ImmutableArray<string>);
			}
			else
			{
				immutableArray = self.ZipAsArray(other, (string n1, string n2) => (!CaseInsensitiveComparison.Equals(n1, n2)) ? null : n1);
				if (immutableArray.All((string n) => n == null))
				{
					immutableArray = default(ImmutableArray<string>);
				}
			}
			return TupleTypeDecoder.DecodeTupleTypesIfApplicable(first, immutableArray);
		}
	}
}
