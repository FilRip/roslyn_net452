using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class Conversions
	{
		internal class ConversionEasyOut
		{
			private static readonly int[,] s_convkind;

			static ConversionEasyOut()
			{
				s_convkind = new int[16, 16]
				{
					{
						5, 34, 258, 258, 258, 258, 258, 258, 258, 258,
						258, 258, 258, 258, 258, 258
					},
					{
						33, 5, 514, 514, 514, 514, 514, 514, 514, 514,
						514, 514, 514, 514, 514, 514
					},
					{
						257, 514, 5, 0, 1026, 1026, 1026, 1026, 1026, 1026,
						1026, 1026, 1026, 1026, 1026, 0
					},
					{
						257, 513, 0, 5, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 0
					},
					{
						257, 514, 1026, 0, 5, 9, 9, 9, 10, 10,
						10, 10, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 5, 9, 9, 10, 10,
						10, 10, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 5, 9, 10, 10,
						10, 10, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 5, 10, 10,
						10, 10, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 9, 9, 9, 5, 9,
						9, 9, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 9, 9, 10, 5,
						9, 9, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 9, 10, 10,
						5, 9, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 10, 10, 10,
						10, 5, 9, 9, 9, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 10, 10, 10,
						10, 10, 5, 9, 10, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 10, 10, 10,
						10, 10, 10, 5, 10, 0
					},
					{
						257, 514, 1026, 0, 10, 10, 10, 10, 10, 10,
						10, 10, 9, 9, 5, 0
					},
					{
						257, 514, 0, 0, 0, 0, 0, 0, 0, 0,
						0, 0, 0, 0, 0, 5
					}
				};
			}

			public static ConversionKind? ClassifyPredefinedConversion(TypeSymbol source, TypeSymbol target)
			{
				ConversionKind? result;
				TypeSymbol nullableUnderlyingTypeOrSelf;
				bool flag;
				TypeSymbol nullableUnderlyingTypeOrSelf2;
				bool flag2;
				bool flag3;
				bool flag4;
				int? num;
				int? num2;
				if ((object)source == null || (object)target == null)
				{
					result = null;
				}
				else
				{
					nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(source);
					flag = (object)nullableUnderlyingTypeOrSelf != source;
					nullableUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(target);
					flag2 = (object)nullableUnderlyingTypeOrSelf2 != target;
					TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
					flag3 = (object)enumUnderlyingTypeOrSelf != nullableUnderlyingTypeOrSelf;
					TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf2);
					flag4 = (object)enumUnderlyingTypeOrSelf2 != nullableUnderlyingTypeOrSelf2;
					if ((flag3 || flag) && (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf)))
					{
						result = null;
					}
					else if ((flag4 || flag2) && (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf2)))
					{
						result = null;
					}
					else
					{
						num = TypeSymbolExtensions.TypeToIndex(enumUnderlyingTypeOrSelf);
						if (!num.HasValue)
						{
							result = null;
						}
						else
						{
							num2 = TypeSymbolExtensions.TypeToIndex(enumUnderlyingTypeOrSelf2);
							if (!num2.HasValue)
							{
								result = null;
							}
							else if (flag)
							{
								if (!TypeSymbolExtensions.IsObjectType(target))
								{
									goto IL_011a;
								}
								result = ConversionKind.WideningValue;
							}
							else
							{
								if (!flag2 || !TypeSymbolExtensions.IsObjectType(source))
								{
									goto IL_011a;
								}
								result = ConversionKind.NarrowingValue;
							}
						}
					}
				}
				goto IL_022f;
				IL_022f:
				return result;
				IL_011a:
				ConversionKind conversionKind = (ConversionKind)s_convkind[num.Value, num2.Value];
				if (NoConversion(conversionKind))
				{
					result = conversionKind;
				}
				else
				{
					if ((flag3 && !TypeSymbolExtensions.IsObjectType(target)) || (flag4 && !TypeSymbolExtensions.IsObjectType(source)))
					{
						TypeSymbol t = nullableUnderlyingTypeOrSelf;
						TypeSymbol t2 = nullableUnderlyingTypeOrSelf2;
						if (flag3)
						{
							if (flag4)
							{
								if (!IsIdentityConversion(conversionKind) || !TypeSymbolExtensions.IsSameTypeIgnoringAll(t, t2))
								{
									conversionKind = ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
								}
							}
							else if (IsWideningConversion(conversionKind))
							{
								conversionKind |= ConversionKind.InvolvesEnumTypeConversions;
								if ((conversionKind & ConversionKind.Identity) != 0)
								{
									conversionKind = (conversionKind & ~ConversionKind.Identity) | ConversionKind.Widening | ConversionKind.Numeric;
								}
							}
							else
							{
								conversionKind |= ConversionKind.InvolvesEnumTypeConversions;
							}
						}
						else
						{
							conversionKind = (conversionKind & ~ConversionKind.Widening) | ConversionKind.Narrowing | ConversionKind.InvolvesEnumTypeConversions;
							if ((conversionKind & ConversionKind.Identity) != 0)
							{
								conversionKind = (conversionKind & ~ConversionKind.Identity) | ConversionKind.Numeric;
							}
						}
					}
					if (flag || flag2)
					{
						if (!flag)
						{
							conversionKind = ((!IsWideningConversion(conversionKind)) ? ConversionKind.NarrowingNullable : ConversionKind.WideningNullable);
						}
						else if (flag2)
						{
							if (IsNarrowingConversion(conversionKind))
							{
								conversionKind = ConversionKind.NarrowingNullable;
							}
							else if (!IsIdentityConversion(conversionKind))
							{
								conversionKind = ConversionKind.WideningNullable;
							}
						}
						else
						{
							conversionKind = ConversionKind.NarrowingNullable;
						}
					}
					result = conversionKind;
				}
				goto IL_022f;
			}
		}

		private struct ToInterfaceConversionClassifier
		{
			private ConversionKind _conv;

			private NamedTypeSymbol _match;

			public ConversionKind Result
			{
				get
				{
					if (IsIdentityConversion(_conv))
					{
						return ConversionKind.Widening;
					}
					return _conv;
				}
			}

			[Conditional("DEBUG")]
			public void AssertFoundIdentity()
			{
			}

			public static ConversionKind ClassifyConversionToVariantCompatibleInterface(NamedTypeSymbol source, NamedTypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
			{
				ToInterfaceConversionClassifier toInterfaceConversionClassifier = default(ToInterfaceConversionClassifier);
				toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface(source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				return toInterfaceConversionClassifier.Result;
			}

			public bool AccumulateConversionClassificationToVariantCompatibleInterface(NamedTypeSymbol source, NamedTypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
			{
				if (IsIdentityConversion(_conv))
				{
					return true;
				}
				if (IsInterfaceType(source))
				{
					ClassifyInterfaceImmediateVarianceCompatibility(source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				}
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = source.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					if (!TypeSymbolExtensions.IsErrorType(current) && ClassifyInterfaceImmediateVarianceCompatibility(current, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo))
					{
						return true;
					}
				}
				return false;
			}

			private bool ClassifyInterfaceImmediateVarianceCompatibility(NamedTypeSymbol source, NamedTypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
			{
				ConversionKind conversionKind = ClassifyImmediateVarianceCompatibility(source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				if ((conversionKind & ConversionKind.MightSucceedAtRuntime) != 0)
				{
					conversionKind = ConversionKind.Narrowing | (conversionKind & (ConversionKind.InvolvesEnumTypeConversions | ConversionKind.VarianceConversionAmbiguity));
				}
				if (ConversionExists(conversionKind))
				{
					if (IsIdentityConversion(conversionKind))
					{
						_conv = ConversionKind.Identity;
						return true;
					}
					if ((object)_match != null)
					{
						if ((_conv & ConversionKind.VarianceConversionAmbiguity) == 0 && !TypeSymbolExtensions.IsSameTypeIgnoringAll(_match, source))
						{
							_conv = ConversionKind.Narrowing | ConversionKind.VarianceConversionAmbiguity;
						}
					}
					else
					{
						_match = source;
						_conv = conversionKind & (ConversionKind.Widening | ConversionKind.Narrowing | ConversionKind.InvolvesEnumTypeConversions | ConversionKind.VarianceConversionAmbiguity);
					}
				}
				return false;
			}
		}

		public static readonly KeyValuePair<ConversionKind, MethodSymbol> Identity = new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.Identity, null);

		private Conversions()
		{
			throw ExceptionUtilities.Unreachable;
		}

		private static ConversionKind? FastClassifyPredefinedConversion(TypeSymbol source, TypeSymbol target)
		{
			return ConversionEasyOut.ClassifyPredefinedConversion(source, target);
		}

		public static ConstantValue TryFoldConstantConversion(BoundExpression source, TypeSymbol destination, ref bool integerOverflow)
		{
			integerOverflow = false;
			ConstantValue sourceValue = source.ConstantValueOpt;
			if ((object)sourceValue == null || sourceValue.IsBad)
			{
				return null;
			}
			if (!TypeSymbolExtensions.AllowsCompileTimeConversions(destination))
			{
				return null;
			}
			if (BoundExpressionExtensions.IsNothingLiteral(source))
			{
				if (TypeSymbolExtensions.IsStringType(destination))
				{
					return source.ConstantValueOpt;
				}
				return ConstantValue.Default(TypeSymbolExtensions.GetConstantValueTypeDiscriminator(destination));
			}
			TypeSymbol type = source.Type;
			if (!TypeSymbolExtensions.AllowsCompileTimeConversions(type))
			{
				return null;
			}
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type);
			TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(destination);
			if ((object)enumUnderlyingTypeOrSelf == enumUnderlyingTypeOrSelf2)
			{
				return sourceValue;
			}
			if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf))
			{
				if (TypeSymbolExtensions.IsCharType(enumUnderlyingTypeOrSelf2))
				{
					string text = (sourceValue.IsNothing ? null : sourceValue.StringValue);
					char value = ((text != null && text.Length != 0) ? text[0] : '\0');
					return ConstantValue.Create(value);
				}
			}
			else
			{
				if (!TypeSymbolExtensions.IsCharType(enumUnderlyingTypeOrSelf))
				{
					return TryFoldConstantNumericOrBooleanConversion(ref sourceValue, enumUnderlyingTypeOrSelf, enumUnderlyingTypeOrSelf2, ref integerOverflow);
				}
				if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf2))
				{
					return ConstantValue.Create(new string(sourceValue.CharValue, 1));
				}
			}
			return null;
		}

		private static ConstantValue TryFoldConstantNumericOrBooleanConversion(ref ConstantValue sourceValue, TypeSymbol sourceType, TypeSymbol targetType, ref bool integerOverflow)
		{
			integerOverflow = false;
			if (TypeSymbolExtensions.IsIntegralType(sourceType) || TypeSymbolExtensions.IsBooleanType(sourceType))
			{
				if (TypeSymbolExtensions.IsNumericType(targetType) || TypeSymbolExtensions.IsBooleanType(targetType))
				{
					long num = CompileTimeCalculations.GetConstantValueAsInt64(ref sourceValue);
					if (TypeSymbolExtensions.IsBooleanType(sourceType) && num != 0L)
					{
						if (TypeSymbolExtensions.IsUnsignedIntegralType(targetType))
						{
							bool overflow = false;
							num = CompileTimeCalculations.NarrowIntegralResult(-1L, sourceType, targetType, ref overflow);
						}
						else
						{
							num = -1L;
						}
					}
					return CompileTimeCalculations.ConvertIntegralValue(num, sourceValue.Discriminator, TypeSymbolExtensions.GetConstantValueTypeDiscriminator(targetType), ref integerOverflow);
				}
			}
			else if (TypeSymbolExtensions.IsFloatingType(sourceType))
			{
				if (TypeSymbolExtensions.IsNumericType(targetType) || TypeSymbolExtensions.IsBooleanType(targetType))
				{
					ConstantValue constantValue = CompileTimeCalculations.ConvertFloatingValue((sourceValue.Discriminator == ConstantValueTypeDiscriminator.Double) ? sourceValue.DoubleValue : ((double)sourceValue.SingleValue), TypeSymbolExtensions.GetConstantValueTypeDiscriminator(targetType), ref integerOverflow);
					if (constantValue.IsBad)
					{
						integerOverflow = false;
					}
					return constantValue;
				}
			}
			else if (TypeSymbolExtensions.IsDecimalType(sourceType) && (TypeSymbolExtensions.IsNumericType(targetType) || TypeSymbolExtensions.IsBooleanType(targetType)))
			{
				ConstantValue constantValue2 = CompileTimeCalculations.ConvertDecimalValue(sourceValue.DecimalValue, TypeSymbolExtensions.GetConstantValueTypeDiscriminator(targetType), ref integerOverflow);
				if (constantValue2.IsBad)
				{
					integerOverflow = false;
				}
				return constantValue2;
			}
			return null;
		}

		public static ConstantValue TryFoldNothingReferenceConversion(BoundExpression source, ConversionKind conversion, TypeSymbol targetType)
		{
			return TryFoldNothingReferenceConversion(source.ConstantValueOpt, conversion, targetType);
		}

		internal static ConstantValue TryFoldNothingReferenceConversion(ConstantValue sourceValue, ConversionKind conversion, TypeSymbol targetType)
		{
			if ((object)sourceValue == null || !sourceValue.IsNothing || TypeSymbolExtensions.IsTypeParameter(targetType) || !targetType.IsReferenceType)
			{
				return null;
			}
			if (conversion == ConversionKind.WideningNothingLiteral || IsIdentityConversion(conversion) || (conversion & ConversionKind.WideningReference) == ConversionKind.WideningReference || (conversion & ConversionKind.WideningArray) == ConversionKind.WideningArray)
			{
				return sourceValue;
			}
			return null;
		}

		public static KeyValuePair<ConversionKind, MethodSymbol> ClassifyConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyPredefinedConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return new KeyValuePair<ConversionKind, MethodSymbol>(conversionKind, null);
			}
			return ClassifyUserDefinedConversion(source, destination, ref useSiteInfo);
		}

		public static ConversionKind ClassifyPredefinedConversion(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool userDefinedConversionsMightStillBeApplicable = false;
			return ClassifyPredefinedConversion(source, destination, binder, out userDefinedConversionsMightStillBeApplicable, ref useSiteInfo);
		}

		private static ConversionKind ClassifyPredefinedConversion(BoundExpression source, TypeSymbol destination, Binder binder, out bool userDefinedConversionsMightStillBeApplicable, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			userDefinedConversionsMightStillBeApplicable = false;
			bool flag = false;
			if ((source.Kind == BoundKind.FieldAccess) ? ((object)((BoundFieldAccess)source).FieldSymbol.GetConstantValue(binder.ConstantFieldsInProgress) != null && source.IsConstant) : ((source.Kind != BoundKind.Local) ? source.IsConstant : ((object)((BoundLocal)source).LocalSymbol.GetConstantValue(binder) != null && source.IsConstant)))
			{
				ConversionKind conversionKind = ClassifyNothingLiteralConversion(source, destination);
				if (ConversionExists(conversionKind))
				{
					return conversionKind;
				}
				conversionKind = ClassifyNumericConstantConversion(source, destination, binder);
				if (ConversionExists(conversionKind) || FailedDueToNumericOverflow(conversionKind))
				{
					return conversionKind;
				}
			}
			if (!BoundExpressionExtensions.IsValue(source))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol typeSymbol = ((source.Kind == BoundKind.TupleLiteral) ? ((BoundTupleLiteral)source).InferredType : source.Type);
			if ((object)typeSymbol == null)
			{
				BoundKind kind = BoundExpressionExtensions.GetMostEnclosedParenthesizedExpression(source).Kind;
				userDefinedConversionsMightStillBeApplicable = kind == BoundKind.ArrayLiteral || kind == BoundKind.TupleLiteral;
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (typeSymbol.Kind != SymbolKind.ErrorType)
			{
				ConversionKind conversionKind2 = ClassifyPredefinedConversion(typeSymbol, destination, ref useSiteInfo);
				if (ConversionExists(conversionKind2))
				{
					return conversionKind2;
				}
				userDefinedConversionsMightStillBeApplicable = true;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static KeyValuePair<ConversionKind, MethodSymbol> ClassifyConversion(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyExpressionReclassification(source, destination, binder, ref useSiteInfo);
			KeyValuePair<ConversionKind, MethodSymbol> result;
			if (ConversionExists(conversionKind) || FailedDueToQueryLambdaBodyMismatch(conversionKind) || (conversionKind & (ConversionKind.FailedDueToArrayLiteralElementConversion | ConversionKind.Lambda)) != 0)
			{
				result = new KeyValuePair<ConversionKind, MethodSymbol>(conversionKind, null);
			}
			else
			{
				bool userDefinedConversionsMightStillBeApplicable = false;
				conversionKind = ClassifyPredefinedConversion(source, destination, binder, out userDefinedConversionsMightStillBeApplicable, ref useSiteInfo);
				if (!ConversionExists(conversionKind) && userDefinedConversionsMightStillBeApplicable)
				{
					return ClassifyUserDefinedConversion(source, destination, binder, ref useSiteInfo);
				}
				result = new KeyValuePair<ConversionKind, MethodSymbol>(conversionKind, null);
			}
			return result;
		}

		private static ConversionKind ClassifyExpressionReclassification(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			switch (source.Kind)
			{
			case BoundKind.Parenthesized:
				if ((object)source.Type == null)
				{
					return ClassifyExpressionReclassification(((BoundParenthesized)source).Expression, destination, binder, ref useSiteInfo);
				}
				break;
			case BoundKind.UnboundLambda:
				return ClassifyUnboundLambdaConversion((UnboundLambda)source, destination);
			case BoundKind.QueryLambda:
				return ClassifyQueryLambdaConversion((BoundQueryLambda)source, destination, binder, ref useSiteInfo);
			case BoundKind.GroupTypeInferenceLambda:
				return ClassifyGroupTypeInferenceLambdaConversion((GroupTypeInferenceLambda)source, destination);
			case BoundKind.AddressOfOperator:
				return ClassifyAddressOfConversion((BoundAddressOfOperator)source, destination);
			case BoundKind.ArrayLiteral:
				return ClassifyArrayLiteralConversion((BoundArrayLiteral)source, destination, binder, ref useSiteInfo);
			case BoundKind.InterpolatedStringExpression:
				return ClassifyInterpolatedStringConversion((BoundInterpolatedStringExpression)source, destination, binder);
			case BoundKind.TupleLiteral:
				return ClassifyTupleConversion((BoundTupleLiteral)source, destination, binder, ref useSiteInfo);
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyUnboundLambdaConversion(UnboundLambda source, TypeSymbol destination)
		{
			ConversionKind val = ConversionKind.DelegateRelaxationLevelNone;
			ConversionKind conversionKind = ConversionKind.DelegateRelaxationLevelNone;
			MethodSymbol methodSymbol = null;
			if (TypeSymbolExtensions.IsStrictSupertypeOfConcreteDelegate(destination))
			{
				val = ConversionKind.DelegateRelaxationLevelWideningToNonLambda;
				KeyValuePair<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> inferredAnonymousDelegate = source.InferredAnonymousDelegate;
				if (!inferredAnonymousDelegate.Value.Diagnostics.IsDefault && inferredAnonymousDelegate.Value.Diagnostics.HasAnyErrors())
				{
					return ConversionKind.Lambda;
				}
				methodSymbol = inferredAnonymousDelegate.Key.DelegateInvokeMethod;
			}
			else
			{
				bool wasExpression = default(bool);
				methodSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(destination, source.Binder, ref wasExpression)?.DelegateInvokeMethod;
				if (wasExpression)
				{
					conversionKind = ConversionKind.ConvertedToExpressionTree;
				}
				if ((object)methodSymbol == null || methodSymbol.GetUseSiteInfo().DiagnosticInfo != null)
				{
					return ConversionKind.Lambda | conversionKind;
				}
				if (source.IsInferredDelegateForThisLambda(methodSymbol.ContainingType))
				{
					ImmutableBindingDiagnostic<AssemblySymbol> value = source.InferredAnonymousDelegate.Value;
					if (!value.Diagnostics.IsDefault && value.Diagnostics.HasAnyErrors())
					{
						return ConversionKind.Lambda | conversionKind;
					}
				}
			}
			BoundLambda boundLambda = source.Bind(new UnboundLambda.TargetSignature(methodSymbol));
			if (boundLambda.DelegateRelaxation == ConversionKind.DelegateRelaxationLevelInvalid)
			{
				return ConversionKind.DelegateRelaxationLevelInvalid | ConversionKind.Lambda | conversionKind;
			}
			return (ConversionKind)((int)(ConversionKind.Lambda | conversionKind | ((!IsNarrowingMethodConversion(boundLambda.MethodConversionKind, isForAddressOf: false)) ? ConversionKind.Widening : ConversionKind.Narrowing)) | Math.Max((int)boundLambda.DelegateRelaxation, (int)val));
		}

		public static ConversionKind ClassifyArrayLiteralConversion(BoundArrayLiteral source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayTypeSymbol inferredType = source.InferredType;
			NamedTypeSymbol namedTypeSymbol = destination as NamedTypeSymbol;
			NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol?.OriginalDefinition;
			TypeSymbol typeSymbol = null;
			if (destination is ArrayTypeSymbol arrayTypeSymbol && (inferredType.Rank == arrayTypeSymbol.Rank || source.IsEmptyArrayLiteral))
			{
				typeSymbol = arrayTypeSymbol.ElementType;
			}
			else
			{
				if ((inferredType.Rank != 1 && !source.IsEmptyArrayLiteral) || (object)namedTypeSymbol2 == null || (namedTypeSymbol2.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T && namedTypeSymbol2.SpecialType != SpecialType.System_Collections_Generic_IList_T && namedTypeSymbol2.SpecialType != SpecialType.System_Collections_Generic_ICollection_T && namedTypeSymbol2.SpecialType != SpecialType.System_Collections_Generic_IReadOnlyList_T && namedTypeSymbol2.SpecialType != SpecialType.System_Collections_Generic_IReadOnlyCollection_T))
				{
					ConversionKind conv = ClassifyStringConversion(inferredType, destination);
					if (NoConversion(conv))
					{
						conv = ClassifyDirectCastConversion(inferredType, destination, ref useSiteInfo);
					}
					if (NoConversion(conv))
					{
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					ConversionKind conversionKind = ClassifyArrayInitialization(source.Initializer, inferredType.ElementType, binder, ref useSiteInfo);
					if (NoConversion(conversionKind))
					{
						return conversionKind;
					}
					if (IsWideningConversion(conv))
					{
						return conversionKind;
					}
					return ConversionKind.Narrowing;
				}
				typeSymbol = namedTypeSymbol.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo)[0];
			}
			return ClassifyArrayInitialization(source.Initializer, typeSymbol, binder, ref useSiteInfo);
		}

		public static ConversionKind ClassifyInterpolatedStringConversion(BoundInterpolatedStringExpression source, TypeSymbol destination, Binder binder)
		{
			if (destination.Equals(binder.Compilation.GetWellKnownType(WellKnownType.System_FormattableString)) || destination.Equals(binder.Compilation.GetWellKnownType(WellKnownType.System_IFormattable)))
			{
				return ConversionKind.InterpolatedString;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static ConversionKind ClassifyTupleConversion(BoundTupleLiteral source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbol.Equals(source.Type, destination, TypeCompareKind.ConsiderEverything))
			{
				return ConversionKind.Identity;
			}
			ImmutableArray<BoundExpression> arguments = source.Arguments;
			ConversionKind conversionKind = ConversionKind.WideningTuple;
			ConversionKind conversionKind2 = ConversionKind.NarrowingTuple;
			if (TypeSymbolExtensions.IsNullableType(destination))
			{
				destination = TypeSymbolExtensions.GetNullableUnderlyingType(destination);
				conversionKind = ConversionKind.WideningNullableTuple;
				conversionKind2 = ConversionKind.NarrowingNullableTuple;
			}
			TupleTypeSymbol inferredType = source.InferredType;
			if ((object)inferredType != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(inferredType, destination))
			{
				return conversionKind;
			}
			if (!destination.IsTupleOrCompatibleWithTupleOfCardinality(arguments.Length))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<TypeSymbol> elementTypesOfTupleOrCompatible = TypeSymbolExtensions.GetElementTypesOfTupleOrCompatible(destination);
			ConversionKind conversionKind3 = conversionKind;
			ConversionKind conversionKind4 = ConversionKind.DelegateRelaxationLevelNone;
			ConversionKind conversionKind5 = ConversionKind.InvolvesNarrowingFromNumericConstant;
			ConversionKind conversionKind6 = ConversionKind.DelegateRelaxationLevelNone;
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				BoundExpression boundExpression = arguments[i];
				TypeSymbol typeSymbol = elementTypesOfTupleOrCompatible[i];
				if (boundExpression.HasErrors || TypeSymbolExtensions.IsErrorType(typeSymbol))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				ConversionKind key = ClassifyConversion(boundExpression, typeSymbol, binder, ref useSiteInfo).Key;
				if (NoConversion(key))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				ConversionKind conversionKind7 = key & ConversionKind.DelegateRelaxationLevelMask;
				if (conversionKind7 > conversionKind6)
				{
					conversionKind6 = conversionKind7;
				}
				conversionKind4 |= key;
				if (IsNarrowingConversion(key))
				{
					conversionKind5 &= key;
					conversionKind3 = conversionKind2;
				}
			}
			return conversionKind3 | (conversionKind4 & conversionKind5) | conversionKind6;
		}

		private static ConversionKind ClassifyArrayInitialization(BoundArrayInitialization source, TypeSymbol targetElementType, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsErrorType(targetElementType))
			{
				return ConversionKind.FailedDueToArrayLiteralElementConversion;
			}
			ConversionKind conversionKind = ConversionKind.Widening;
			ConversionKind conversionKind2 = ConversionKind.InvolvesNarrowingFromNumericConstant;
			bool flag = false;
			ImmutableArray<BoundExpression>.Enumerator enumerator = source.Initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundExpression current = enumerator.Current;
				ConversionKind conversionKind3 = ((current.Kind != BoundKind.ArrayInitialization) ? ClassifyConversion(current, targetElementType, binder, ref useSiteInfo).Key : ClassifyArrayInitialization((BoundArrayInitialization)current, targetElementType, binder, ref useSiteInfo));
				if (IsNarrowingConversion(conversionKind3))
				{
					conversionKind = ConversionKind.Narrowing;
					conversionKind2 &= conversionKind3;
					flag = true;
				}
				else if (NoConversion(conversionKind3))
				{
					conversionKind = ConversionKind.FailedDueToArrayLiteralElementConversion;
					flag = false;
					break;
				}
				if (IsWideningConversion(conversionKind) && (conversionKind3 & ConversionKind.InvolvesNarrowingFromNumericConstant) != 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				conversionKind |= conversionKind2;
			}
			return conversionKind;
		}

		private static ConversionKind ClassifyAddressOfConversion(BoundAddressOfOperator source, TypeSymbol destination)
		{
			return Binder.ClassifyAddressOfConversion(source, destination);
		}

		private static ConversionKind ClassifyQueryLambdaConversion(BoundQueryLambda source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool wasExpression = default(bool);
			NamedTypeSymbol namedTypeSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(destination, binder, ref wasExpression);
			if ((object)namedTypeSymbol == null)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ConversionKind conversionKind = (wasExpression ? ConversionKind.ConvertedToExpressionTree : ConversionKind.DelegateRelaxationLevelNone);
			MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
			if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null || delegateInvokeMethod.IsSub)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (delegateInvokeMethod.ParameterCount != source.LambdaSymbol.ParameterCount)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<ParameterSymbol> parameters = source.LambdaSymbol.Parameters;
			ImmutableArray<ParameterSymbol> parameters2 = delegateInvokeMethod.Parameters;
			int num = parameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = parameters[i];
				ParameterSymbol parameterSymbol2 = parameters2[i];
				if (parameterSymbol.IsByRef != parameterSymbol2.IsByRef || !TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol.Type, parameterSymbol2.Type))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
			}
			if ((object)source.LambdaSymbol.ReturnType == LambdaSymbol.ReturnTypePendingDelegate)
			{
				if (!TypeSymbolExtensions.IsErrorType(delegateInvokeMethod.ReturnType))
				{
					KeyValuePair<ConversionKind, MethodSymbol> keyValuePair;
					if (source.ExprIsOperandOfConditionalBranch && TypeSymbolExtensions.IsBooleanType(delegateInvokeMethod.ReturnType))
					{
						BoundExpression expression = source.Expression;
						TypeSymbol returnType = delegateInvokeMethod.ReturnType;
						bool applyNullableIsTrueOperator = false;
						OverloadResolution.OverloadResolutionResult isTrueOperator = default(OverloadResolution.OverloadResolutionResult);
						keyValuePair = ClassifyConversionOfOperandOfConditionalBranch(expression, returnType, binder, out applyNullableIsTrueOperator, out isTrueOperator, ref useSiteInfo);
					}
					else
					{
						keyValuePair = ClassifyConversion(source.Expression, delegateInvokeMethod.ReturnType, binder, ref useSiteInfo);
					}
					if (IsIdentityConversion(keyValuePair.Key))
					{
						return (keyValuePair.Key & ~ConversionKind.Identity) | (ConversionKind.Widening | ConversionKind.Lambda) | conversionKind;
					}
					if (NoConversion(keyValuePair.Key))
					{
						return keyValuePair.Key | (ConversionKind.FailedDueToQueryLambdaBodyMismatch | ConversionKind.Lambda) | conversionKind;
					}
					return (keyValuePair.Key & ~(ConversionKind.Nullable | ConversionKind.UserDefined | ConversionKind.Tuple)) | ConversionKind.Lambda | conversionKind;
				}
			}
			else if (TypeSymbolExtensions.IsSameTypeIgnoringAll(delegateInvokeMethod.ReturnType, source.LambdaSymbol.ReturnType))
			{
				return ConversionKind.Widening | ConversionKind.Lambda | conversionKind;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static KeyValuePair<ConversionKind, MethodSymbol> ClassifyConversionOfOperandOfConditionalBranch(BoundExpression operand, TypeSymbol booleanType, Binder binder, out bool applyNullableIsTrueOperator, out OverloadResolution.OverloadResolutionResult isTrueOperator, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			applyNullableIsTrueOperator = false;
			isTrueOperator = default(OverloadResolution.OverloadResolutionResult);
			KeyValuePair<ConversionKind, MethodSymbol> result = ClassifyConversion(operand, booleanType, binder, ref useSiteInfo);
			if (IsWideningConversion(result.Key))
			{
				return result;
			}
			TypeSymbol type = operand.Type;
			if ((object)type != null && !TypeSymbolExtensions.IsErrorType(type) && !TypeSymbolExtensions.IsObjectType(type))
			{
				TypeSymbol typeSymbol = null;
				KeyValuePair<ConversionKind, MethodSymbol> result2 = default(KeyValuePair<ConversionKind, MethodSymbol>);
				if (TypeSymbolExtensions.IsNullableOfBoolean(type))
				{
					result2 = Identity;
					typeSymbol = type;
				}
				else
				{
					NamedTypeSymbol specialType = booleanType.ContainingAssembly.GetSpecialType(SpecialType.System_Nullable_T);
					if (!TypeSymbolExtensions.IsErrorType(specialType) && (TypeSymbolExtensions.IsNullableType(type) || TypeSymbolExtensions.CanContainUserDefinedOperators(type, ref useSiteInfo)))
					{
						typeSymbol = specialType.Construct(ImmutableArray.Create(booleanType));
						result2 = ClassifyConversion(operand, typeSymbol, binder, ref useSiteInfo);
					}
				}
				if (IsWideningConversion(result2.Key))
				{
					applyNullableIsTrueOperator = true;
					return result2;
				}
				OverloadResolution.OverloadResolutionResult overloadResolutionResult = default(OverloadResolution.OverloadResolutionResult);
				if (TypeSymbolExtensions.CanContainUserDefinedOperators(type, ref useSiteInfo))
				{
					overloadResolutionResult = OverloadResolution.ResolveIsTrueOperator(operand, binder, ref useSiteInfo);
				}
				if (overloadResolutionResult.BestResult.HasValue)
				{
					isTrueOperator = overloadResolutionResult;
					if (overloadResolutionResult.BestResult.Value.Candidate.IsLifted)
					{
						applyNullableIsTrueOperator = true;
					}
					return new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.Widening, null);
				}
				if (IsNarrowingConversion(result2.Key) && ((result2.Key & (ConversionKind.Nullable | ConversionKind.UserDefined)) != ConversionKind.UserDefined || !TypeSymbolExtensions.IsBooleanType(result2.Value.ReturnType)))
				{
					applyNullableIsTrueOperator = true;
					return result2;
				}
			}
			return result;
		}

		private static ConversionKind ClassifyGroupTypeInferenceLambdaConversion(GroupTypeInferenceLambda source, TypeSymbol destination)
		{
			NamedTypeSymbol namedTypeSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(destination, source.Binder);
			if ((object)namedTypeSymbol == null)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
			if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null || delegateInvokeMethod.IsSub)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<ParameterSymbol> parameters = source.Parameters;
			if (delegateInvokeMethod.ParameterCount != parameters.Length)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<ParameterSymbol> parameters2 = delegateInvokeMethod.Parameters;
			int num = parameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = parameters[i];
				ParameterSymbol parameterSymbol2 = parameters2[i];
				if (parameterSymbol.IsByRef != parameterSymbol2.IsByRef || ((object)parameterSymbol.Type != null && !TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol.Type, parameterSymbol2.Type)))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
			}
			if (!delegateInvokeMethod.ReturnType.IsAnonymousType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			AnonymousTypeManager.AnonymousTypePublicSymbol anonymousTypePublicSymbol = (AnonymousTypeManager.AnonymousTypePublicSymbol)(NamedTypeSymbol)delegateInvokeMethod.ReturnType;
			if (anonymousTypePublicSymbol.Properties.Length != 1 || (object)anonymousTypePublicSymbol.Properties[0].SetMethod != null || !anonymousTypePublicSymbol.Properties[0].Name.Equals("$VB$ItAnonymous") || !TypeSymbolExtensions.IsSameTypeIgnoringAll(parameters2[1].Type, anonymousTypePublicSymbol.Properties[0].Type))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			return ConversionKind.Widening | ConversionKind.Lambda;
		}

		private static ConversionKind ClassifyNumericConstantConversion(BoundExpression constantExpression, TypeSymbol destination, Binder binder)
		{
			if (constantExpression.ConstantValueOpt.IsBad)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(destination);
			if (BoundExpressionExtensions.IsIntegerZeroLiteral(constantExpression) && TypeSymbolExtensions.IsEnumType(nullableUnderlyingTypeOrSelf) && TypeSymbolExtensions.IsIntegralType(((NamedTypeSymbol)nullableUnderlyingTypeOrSelf).EnumUnderlyingType))
			{
				if ((object)nullableUnderlyingTypeOrSelf == destination)
				{
					return ConversionKind.WideningNumeric | ConversionKind.InvolvesEnumTypeConversions;
				}
				return ConversionKind.NarrowingNullable | ConversionKind.InvolvesNarrowingFromNumericConstant;
			}
			TypeSymbol type = constantExpression.Type;
			if ((object)type == null)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if ((object)type == destination)
			{
				return ConversionKind.Identity;
			}
			ConversionKind conversionKind = ConversionKind.DelegateRelaxationLevelNone;
			bool integerOverflow = false;
			if (TypeSymbolExtensions.IsIntegralType(type))
			{
				if (TypeSymbolExtensions.IsIntegralType(nullableUnderlyingTypeOrSelf))
				{
					conversionKind = FastClassifyPredefinedConversion(type, destination).Value;
					if (IsNarrowingConversion(conversionKind))
					{
						conversionKind |= ConversionKind.InvolvesNarrowingFromNumericConstant;
						ConstantValue sourceValue = constantExpression.ConstantValueOpt;
						TryFoldConstantNumericOrBooleanConversion(ref sourceValue, type, nullableUnderlyingTypeOrSelf, ref integerOverflow);
						if (!integerOverflow)
						{
							if ((object)nullableUnderlyingTypeOrSelf == destination)
							{
								conversionKind = (conversionKind & ~ConversionKind.Narrowing) | ConversionKind.Widening;
							}
						}
						else if (binder.CheckOverflow)
						{
							return ConversionKind.FailedDueToIntegerOverflow;
						}
					}
					return conversionKind;
				}
			}
			else if (TypeSymbolExtensions.IsFloatingType(type) && TypeSymbolExtensions.IsFloatingType(nullableUnderlyingTypeOrSelf))
			{
				conversionKind = FastClassifyPredefinedConversion(type, destination).Value;
				if (IsNarrowingConversion(conversionKind))
				{
					conversionKind |= ConversionKind.InvolvesNarrowingFromNumericConstant;
				}
			}
			if (!IsWideningConversion(conversionKind))
			{
				TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type);
				TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(destination));
				if (TypeSymbolExtensions.IsNumericType(enumUnderlyingTypeOrSelf) && TypeSymbolExtensions.IsNumericType(enumUnderlyingTypeOrSelf2))
				{
					ConstantValue sourceValue = constantExpression.ConstantValueOpt;
					if (TryFoldConstantNumericOrBooleanConversion(ref sourceValue, enumUnderlyingTypeOrSelf, enumUnderlyingTypeOrSelf2, ref integerOverflow).IsBad)
					{
						conversionKind = ConversionKind.FailedDueToNumericOverflow;
					}
					else if (integerOverflow && binder.CheckOverflow)
					{
						return ConversionKind.FailedDueToIntegerOverflow;
					}
				}
			}
			return conversionKind;
		}

		private static ConversionKind ClassifyNothingLiteralConversion(BoundExpression constantExpression, TypeSymbol destination)
		{
			if (BoundExpressionExtensions.IsStrictNothingLiteral(constantExpression))
			{
				if (TypeSymbolExtensions.IsObjectType(destination) && (object)constantExpression.Type != null && TypeSymbolExtensions.IsObjectType(constantExpression.Type))
				{
					return ConversionKind.Identity;
				}
				return ConversionKind.WideningNothingLiteral;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static ConversionKind ClassifyDirectCastConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyIdentityConversion(source, destination);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			if (TypeSymbolExtensions.IsIntegralType(source))
			{
				if (destination.TypeKind == TypeKind.Enum && ((NamedTypeSymbol)destination).EnumUnderlyingType.Equals(source))
				{
					return ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
				}
			}
			else if (TypeSymbolExtensions.IsIntegralType(destination))
			{
				if (source.TypeKind == TypeKind.Enum && ((NamedTypeSymbol)source).EnumUnderlyingType.Equals(destination))
				{
					return ConversionKind.WideningNumeric | ConversionKind.InvolvesEnumTypeConversions;
				}
			}
			else if (source.TypeKind == TypeKind.Enum && destination.TypeKind == TypeKind.Enum)
			{
				NamedTypeSymbol enumUnderlyingType = ((NamedTypeSymbol)source).EnumUnderlyingType;
				if (TypeSymbolExtensions.IsIntegralType(enumUnderlyingType) && enumUnderlyingType.Equals(((NamedTypeSymbol)destination).EnumUnderlyingType))
				{
					return ConversionKind.NarrowingNumeric | ConversionKind.InvolvesEnumTypeConversions;
				}
			}
			conversionKind = ClassifyReferenceConversion(source, destination, 0, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyArrayConversion(source, destination, 0, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyValueTypeConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			return ClassifyTypeParameterConversion(source, destination, 0, ref useSiteInfo);
		}

		public static ConversionKind ClassifyDirectCastConversion(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind;
			if (source.IsConstant)
			{
				conversionKind = ClassifyNothingLiteralConversion(source, destination);
				if (ConversionExists(conversionKind))
				{
					return conversionKind;
				}
			}
			conversionKind = ClassifyExpressionReclassification(source, destination, binder, ref useSiteInfo);
			if (ConversionExists(conversionKind) || (conversionKind & (ConversionKind.FailedDueToArrayLiteralElementConversion | ConversionKind.Lambda)) != 0)
			{
				return conversionKind;
			}
			if (!BoundExpressionExtensions.IsValue(source))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol type = source.Type;
			if ((object)type == null)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (type.Kind != SymbolKind.ErrorType)
			{
				return ClassifyDirectCastConversion(type, destination, ref useSiteInfo);
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static ConversionKind ClassifyTryCastConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyDirectCastConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			return ClassifyTryCastConversionForTypeParameters(source, destination, ref useSiteInfo);
		}

		public static ConversionKind ClassifyTryCastConversion(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind;
			if (source.IsConstant)
			{
				conversionKind = ClassifyNothingLiteralConversion(source, destination);
				if (ConversionExists(conversionKind))
				{
					return conversionKind;
				}
			}
			conversionKind = ClassifyExpressionReclassification(source, destination, binder, ref useSiteInfo);
			if (ConversionExists(conversionKind) || (conversionKind & (ConversionKind.FailedDueToArrayLiteralElementConversion | ConversionKind.Lambda)) != 0)
			{
				return conversionKind;
			}
			if (!BoundExpressionExtensions.IsValue(source))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol type = source.Type;
			if ((object)type == null)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (type.Kind != SymbolKind.ErrorType)
			{
				return ClassifyTryCastConversion(type, destination, ref useSiteInfo);
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyTryCastConversionForTypeParameters(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			SymbolKind kind = source.Kind;
			SymbolKind kind2 = destination.Kind;
			if (kind == SymbolKind.ArrayType && kind2 == SymbolKind.ArrayType)
			{
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)source;
				ArrayTypeSymbol obj = (ArrayTypeSymbol)destination;
				TypeSymbol elementType = arrayTypeSymbol.ElementType;
				TypeSymbol elementType2 = obj.ElementType;
				if (elementType.IsReferenceType)
				{
					if (elementType2.IsValueType)
					{
						return ConversionKind.DelegateRelaxationLevelNone;
					}
				}
				else if (elementType.IsValueType && elementType2.IsReferenceType)
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				return ClassifyTryCastConversionForTypeParameters(elementType, elementType2, ref useSiteInfo);
			}
			if (kind != SymbolKind.TypeParameter && kind2 != SymbolKind.TypeParameter)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol nonInterfaceTypeConstraintOrSelf = GetNonInterfaceTypeConstraintOrSelf(source, ref useSiteInfo);
			TypeSymbol nonInterfaceTypeConstraintOrSelf2 = GetNonInterfaceTypeConstraintOrSelf(destination, ref useSiteInfo);
			if ((object)nonInterfaceTypeConstraintOrSelf == null || (object)nonInterfaceTypeConstraintOrSelf2 == null)
			{
				return ConversionKind.Narrowing;
			}
			ConversionKind conversionKind = ClassifyDirectCastConversion(nonInterfaceTypeConstraintOrSelf, nonInterfaceTypeConstraintOrSelf2, ref useSiteInfo);
			if (IsWideningConversion(conversionKind))
			{
				if (kind2 == SymbolKind.TypeParameter && (nonInterfaceTypeConstraintOrSelf.TypeKind != TypeKind.Class || ((NamedTypeSymbol)nonInterfaceTypeConstraintOrSelf).IsNotInheritable) && !ClassOrBasesSatisfyConstraints(nonInterfaceTypeConstraintOrSelf, (TypeParameterSymbol)destination, ref useSiteInfo))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				return ConversionKind.Narrowing | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
			}
			conversionKind = ClassifyDirectCastConversion(nonInterfaceTypeConstraintOrSelf2, nonInterfaceTypeConstraintOrSelf, ref useSiteInfo);
			if (IsWideningConversion(conversionKind))
			{
				if (kind == SymbolKind.TypeParameter && (nonInterfaceTypeConstraintOrSelf2.TypeKind != TypeKind.Class || ((NamedTypeSymbol)nonInterfaceTypeConstraintOrSelf2).IsNotInheritable) && !ClassOrBasesSatisfyConstraints(nonInterfaceTypeConstraintOrSelf2, (TypeParameterSymbol)source, ref useSiteInfo))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				return ConversionKind.Narrowing | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static bool ClassOrBasesSatisfyConstraints(TypeSymbol @class, TypeParameterSymbol typeParam, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = @class;
			while ((object)typeSymbol != null)
			{
				if (ConstraintsHelper.CheckConstraints(null, null, typeParam, typeSymbol, null, ref useSiteInfo))
				{
					return true;
				}
				typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
			}
			return false;
		}

		private static TypeSymbol GetNonInterfaceTypeConstraintOrSelf(TypeSymbol type, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (type.Kind == SymbolKind.TypeParameter)
			{
				TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
				if (typeParameterSymbol.HasValueTypeConstraint)
				{
					NamedTypeSymbol specialType = typeParameterSymbol.ContainingAssembly.GetSpecialType(SpecialType.System_ValueType);
					return (specialType.Kind == SymbolKind.ErrorType) ? null : specialType;
				}
				return ConstraintsHelper.GetNonInterfaceConstraint(typeParameterSymbol, ref useSiteInfo);
			}
			return type;
		}

		private static KeyValuePair<ConversionKind, MethodSymbol> ClassifyUserDefinedConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (IsInterfaceType(source) || IsInterfaceType(destination) || (!TypeSymbolExtensions.CanContainUserDefinedOperators(source, ref useSiteInfo) && !TypeSymbolExtensions.CanContainUserDefinedOperators(destination, ref useSiteInfo)))
			{
				return default(KeyValuePair<ConversionKind, MethodSymbol>);
			}
			return OverloadResolution.ResolveUserDefinedConversion(source, destination, ref useSiteInfo);
		}

		private static KeyValuePair<ConversionKind, MethodSymbol> ClassifyUserDefinedConversion(BoundExpression source, TypeSymbol destination, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = source.Type;
			KeyValuePair<ConversionKind, MethodSymbol> result;
			if ((object)typeSymbol == null)
			{
				source = BoundExpressionExtensions.GetMostEnclosedParenthesizedExpression(source);
				if (source.Kind == BoundKind.ArrayLiteral)
				{
					typeSymbol = new ArrayLiteralTypeSymbol((BoundArrayLiteral)source);
				}
				else if (source.Kind == BoundKind.TupleLiteral)
				{
					typeSymbol = ((BoundTupleLiteral)source).InferredType;
					if ((object)typeSymbol == null)
					{
						result = default(KeyValuePair<ConversionKind, MethodSymbol>);
						goto IL_016f;
					}
				}
				else
				{
					typeSymbol = source.Type;
				}
			}
			KeyValuePair<ConversionKind, MethodSymbol> result2 = ClassifyUserDefinedConversion(typeSymbol, destination, ref useSiteInfo);
			if (NoConversion(result2.Key))
			{
				return result2;
			}
			if (IsNarrowingConversion(result2.Key))
			{
				TypeSymbol type = result2.Value.Parameters[0].Type;
				ConversionKind conversionKind = ((source.Kind == BoundKind.ArrayLiteral) ? ClassifyArrayLiteralConversion((BoundArrayLiteral)source, type, binder, ref useSiteInfo) : ClassifyPredefinedConversion(source, type, binder, ref useSiteInfo));
				if (NoConversion(conversionKind))
				{
					if (FailedDueToNumericOverflow(conversionKind))
					{
						return new KeyValuePair<ConversionKind, MethodSymbol>((result2.Key & ~ConversionKind.Narrowing) | (conversionKind & ConversionKind.FailedDueToIntegerOverflow), result2.Value);
					}
					result = default(KeyValuePair<ConversionKind, MethodSymbol>);
					goto IL_016f;
				}
				if ((conversionKind & ConversionKind.InvolvesNarrowingFromNumericConstant) != 0 && OverloadResolution.IsWidening(result2.Value) && IsWideningConversion(ClassifyPredefinedConversion(result2.Value.ReturnType, destination, ref useSiteInfo)))
				{
					ConversionKind conversionKind2 = result2.Key | ConversionKind.InvolvesNarrowingFromNumericConstant;
					if (IsWideningConversion(conversionKind))
					{
						conversionKind2 = (conversionKind2 & ~ConversionKind.Narrowing) | ConversionKind.Widening;
					}
					result2 = new KeyValuePair<ConversionKind, MethodSymbol>(conversionKind2, result2.Value);
				}
			}
			return result2;
			IL_016f:
			return result;
		}

		public static ConversionKind ClassifyPredefinedConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind? conversionKind = FastClassifyPredefinedConversion(source, destination);
			if (conversionKind.HasValue)
			{
				return conversionKind.Value;
			}
			return ClassifyPredefinedConversionSlow(source, destination, ref useSiteInfo);
		}

		private static ConversionKind ClassifyPredefinedConversionSlow(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyIdentityConversion(source, destination);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyReferenceConversion(source, destination, 0, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return AddDelegateRelaxationInformationForADelegate(source, destination, conversionKind);
			}
			conversionKind = ClassifyAnonymousDelegateConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyArrayConversion(source, destination, 0, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyTupleConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyValueTypeConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyNullableConversion(source, destination, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyStringConversion(source, destination);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			conversionKind = ClassifyTypeParameterConversion(source, destination, 0, ref useSiteInfo);
			return AddDelegateRelaxationInformationForADelegate(source, destination, conversionKind);
		}

		private static ConversionKind AddDelegateRelaxationInformationForADelegate(TypeSymbol source, TypeSymbol destination, ConversionKind convKind)
		{
			if (TypeSymbolExtensions.IsDelegateType(source))
			{
				convKind &= ~ConversionKind.DelegateRelaxationLevelMask;
				if (!ConversionExists(convKind))
				{
					return convKind | ConversionKind.DelegateRelaxationLevelInvalid;
				}
				if (IsWideningConversion(convKind))
				{
					if (IsIdentityConversion(convKind))
					{
						return convKind;
					}
					if (!TypeSymbolExtensions.IsDelegateType(destination) || TypeSymbolExtensions.IsStrictSupertypeOfConcreteDelegate(destination))
					{
						return convKind | ConversionKind.DelegateRelaxationLevelWideningToNonLambda;
					}
					return convKind | ConversionKind.DelegateRelaxationLevelWidening;
				}
				return convKind | ConversionKind.DelegateRelaxationLevelNarrowing;
			}
			return convKind;
		}

		private static ConversionKind ClassifyIdentityConversion(TypeSymbol source, TypeSymbol destination)
		{
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(source, destination))
			{
				return ConversionKind.Identity;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyReferenceConversion(TypeSymbol source, TypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (source.SpecialType == SpecialType.System_Void || destination.SpecialType == SpecialType.System_Void)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			bool isClassType = default(bool);
			bool isDelegateType = default(bool);
			bool isInterfaceType = default(bool);
			bool isArrayType = default(bool);
			bool isClassType2 = default(bool);
			bool isDelegateType2 = default(bool);
			bool isInterfaceType2 = default(bool);
			bool isArrayType2 = default(bool);
			if (!ClassifyAsReferenceType(source, ref isClassType, ref isDelegateType, ref isInterfaceType, ref isArrayType) || !ClassifyAsReferenceType(destination, ref isClassType2, ref isDelegateType2, ref isInterfaceType2, ref isArrayType2))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (destination.SpecialType == SpecialType.System_Object)
			{
				return ConversionKind.WideningReference;
			}
			if (isInterfaceType)
			{
				if (isClassType2)
				{
					return ConversionKind.NarrowingReference;
				}
				if (isArrayType2)
				{
					ConversionKind conversionKind = ClassifyReferenceConversionFromArrayToAnInterface(destination, source, varianceCompatibilityClassificationDepth, ref useSiteInfo);
					if (NoConversion(conversionKind))
					{
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					return ConversionKind.NarrowingReference | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
				}
			}
			if (isInterfaceType2)
			{
				if (isInterfaceType || isClassType)
				{
					ConversionKind conversionKind2 = ToInterfaceConversionClassifier.ClassifyConversionToVariantCompatibleInterface((NamedTypeSymbol)source, (NamedTypeSymbol)destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
					if (ConversionExists(conversionKind2))
					{
						return conversionKind2 | ConversionKind.Reference;
					}
					return ConversionKind.NarrowingReference;
				}
				if (isArrayType)
				{
					return ClassifyReferenceConversionFromArrayToAnInterface(source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				}
			}
			else if (isClassType || isArrayType)
			{
				if (isClassType2 && IsDerivedFrom(source, destination, ref useSiteInfo))
				{
					return ConversionKind.WideningReference;
				}
				if (isClassType && IsDerivedFrom(destination, source, ref useSiteInfo))
				{
					return ConversionKind.NarrowingReference;
				}
				if (isDelegateType && isDelegateType2)
				{
					ConversionKind conversionKind3 = ClassifyConversionToVariantCompatibleDelegateType((NamedTypeSymbol)source, (NamedTypeSymbol)destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
					if (ConversionExists(conversionKind3))
					{
						return conversionKind3 | ConversionKind.Reference;
					}
					if ((conversionKind3 & ConversionKind.MightSucceedAtRuntime) != 0)
					{
						return ConversionKind.MightSucceedAtRuntime;
					}
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyReferenceConversionFromArrayToAnInterface(TypeSymbol source, TypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol namedTypeSymbol = source.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
			if ((object)namedTypeSymbol != null && !TypeSymbolExtensions.IsErrorType(namedTypeSymbol) && namedTypeSymbol.TypeKind == TypeKind.Class && IsWideningConversion(ClassifyDirectCastConversion(namedTypeSymbol, destination, ref useSiteInfo)))
			{
				return ConversionKind.WideningReference;
			}
			ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)source;
			if (!arrayTypeSymbol.IsSZArray)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)destination.OriginalDefinition;
			if ((object)namedTypeSymbol2 == destination || namedTypeSymbol2.Kind == SymbolKind.ErrorType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			SpecialType specialType = namedTypeSymbol2.SpecialType;
			if (specialType != SpecialType.System_Collections_Generic_IList_T && specialType != SpecialType.System_Collections_Generic_ICollection_T && specialType != SpecialType.System_Collections_Generic_IEnumerable_T && specialType != SpecialType.System_Collections_Generic_IReadOnlyList_T && specialType != SpecialType.System_Collections_Generic_IReadOnlyCollection_T)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol typeSymbol = ((NamedTypeSymbol)destination).TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo)[0];
			if (typeSymbol.Kind == SymbolKind.ErrorType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol elementType = arrayTypeSymbol.ElementType;
			if (elementType.Kind == SymbolKind.ErrorType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(elementType, typeSymbol))
			{
				return ConversionKind.WideningReference;
			}
			ConversionKind conversionKind = ClassifyArrayConversionBasedOnElementTypes(elementType, typeSymbol, varianceCompatibilityClassificationDepth, ref useSiteInfo);
			if (IsWideningConversion(conversionKind))
			{
				return ConversionKind.WideningReference | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
			}
			if (IsNarrowingConversion(conversionKind))
			{
				return ConversionKind.NarrowingReference | (conversionKind & (ConversionKind.InvolvesEnumTypeConversions | ConversionKind.VarianceConversionAmbiguity));
			}
			return ConversionKind.NarrowingReference;
		}

		public static bool HasWideningDirectCastConversionButNotEnumTypeConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsErrorType(source) || TypeSymbolExtensions.IsErrorType(destination))
			{
				return TypeSymbolExtensions.IsSameTypeIgnoringAll(source, destination);
			}
			ConversionKind conversionKind = ClassifyDirectCastConversion(source, destination, ref useSiteInfo);
			if (IsWideningConversion(conversionKind) && (conversionKind & ConversionKind.InvolvesEnumTypeConversions) == 0)
			{
				return true;
			}
			return false;
		}

		private static ConversionKind ClassifyConversionToVariantCompatibleDelegateType(NamedTypeSymbol source, NamedTypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyImmediateVarianceCompatibility(source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			ConversionKind conversionKind2 = ClassifyImmediateVarianceCompatibility(destination, source, varianceCompatibilityClassificationDepth, ref useSiteInfo);
			if (ConversionExists(conversionKind2))
			{
				return (conversionKind2 & ~(ConversionKind.Widening | ConversionKind.NarrowingDueToContraVarianceInDelegate)) | ConversionKind.Narrowing;
			}
			return (conversionKind | conversionKind2) & ConversionKind.MightSucceedAtRuntime;
		}

		private static ConversionKind ClassifyImmediateVarianceCompatibility(NamedTypeSymbol source, NamedTypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(source.OriginalDefinition, destination.OriginalDefinition))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (varianceCompatibilityClassificationDepth > 20)
			{
				return ConversionKind.Narrowing;
			}
			varianceCompatibilityClassificationDepth++;
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			ConversionKind conversionKind = ConversionKind.DelegateRelaxationLevelNone;
			ConversionKind conversionKind2 = ConversionKind.VarianceConversionAmbiguity;
			bool flag4 = IsInterfaceType(source);
			do
			{
				ImmutableArray<TypeParameterSymbol> typeParameters = source.TypeParameters;
				ImmutableArray<TypeSymbol> immutableArray = source.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
				ImmutableArray<TypeSymbol> immutableArray2 = destination.TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
				int num = typeParameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					TypeSymbol typeSymbol = immutableArray[i];
					TypeSymbol typeSymbol2 = immutableArray2[i];
					if (TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, typeSymbol2))
					{
						continue;
					}
					if (TypeSymbolExtensions.IsErrorType(typeSymbol) || TypeSymbolExtensions.IsErrorType(typeSymbol2))
					{
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					if (typeSymbol.IsValueType || typeSymbol2.IsValueType)
					{
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					flag2 = false;
					ConversionKind conversionKind3 = ConversionKind.DelegateRelaxationLevelNone;
					switch (typeParameters[i].Variance)
					{
					case VarianceKind.Out:
						conversionKind3 = Classify_Reference_Array_TypeParameterConversion(typeSymbol, typeSymbol2, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (!flag4 && IsNarrowingConversion(conversionKind3) && (conversionKind3 & ConversionKind.NarrowingDueToContraVarianceInDelegate) != 0)
						{
							flag3 = true;
							conversionKind2 = ConversionKind.DelegateRelaxationLevelNone;
							continue;
						}
						break;
					case VarianceKind.In:
						conversionKind3 = Classify_Reference_Array_TypeParameterConversion(typeSymbol2, typeSymbol, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (!flag4 && !IsWideningConversion(conversionKind3) && typeSymbol2.IsReferenceType && typeSymbol.IsReferenceType)
						{
							flag3 = true;
							conversionKind2 = ConversionKind.DelegateRelaxationLevelNone;
							continue;
						}
						break;
					default:
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					if (NoConversion(conversionKind3))
					{
						if ((conversionKind3 & ConversionKind.MightSucceedAtRuntime) == 0)
						{
							return ConversionKind.DelegateRelaxationLevelNone;
						}
						flag = false;
						conversionKind2 = ConversionKind.DelegateRelaxationLevelNone;
						continue;
					}
					if ((conversionKind3 & ConversionKind.InvolvesEnumTypeConversions) != 0)
					{
						conversionKind = ConversionKind.InvolvesEnumTypeConversions;
					}
					if (IsNarrowingConversion(conversionKind3))
					{
						flag = false;
						conversionKind2 &= conversionKind3;
					}
				}
				source = source.ContainingType;
				destination = destination.ContainingType;
			}
			while ((object)source != null);
			if (flag2)
			{
				return ConversionKind.Identity;
			}
			if (!flag)
			{
				return ConversionKind.MightSucceedAtRuntime | conversionKind2 | conversionKind;
			}
			if (flag3)
			{
				return ConversionKind.Narrowing | ConversionKind.NarrowingDueToContraVarianceInDelegate | conversionKind;
			}
			return ConversionKind.Widening | conversionKind;
		}

		private static bool ClassifyAsReferenceType(TypeSymbol candidate, ref bool isClassType, ref bool isDelegateType, ref bool isInterfaceType, ref bool isArrayType)
		{
			switch (candidate.TypeKind)
			{
			case TypeKind.Class:
			case TypeKind.Module:
				isClassType = true;
				isDelegateType = false;
				isInterfaceType = false;
				isArrayType = false;
				break;
			case TypeKind.Delegate:
				isClassType = true;
				isDelegateType = true;
				isInterfaceType = false;
				isArrayType = false;
				break;
			case TypeKind.Interface:
				isClassType = false;
				isDelegateType = false;
				isInterfaceType = true;
				isArrayType = false;
				break;
			case TypeKind.Array:
				isClassType = false;
				isDelegateType = false;
				isInterfaceType = false;
				isArrayType = true;
				break;
			default:
				isClassType = false;
				isDelegateType = false;
				isInterfaceType = false;
				isArrayType = false;
				return false;
			}
			return true;
		}

		private static bool IsClassType(TypeSymbol type)
		{
			TypeKind typeKind = type.TypeKind;
			if (typeKind != TypeKind.Class && typeKind != TypeKind.Module)
			{
				return typeKind == TypeKind.Delegate;
			}
			return true;
		}

		private static bool IsValueType(TypeSymbol type)
		{
			TypeKind typeKind = type.TypeKind;
			if (typeKind != TypeKind.Enum)
			{
				return typeKind == TypeKind.Struct;
			}
			return true;
		}

		private static bool IsDelegateType(TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Delegate;
		}

		private static bool IsArrayType(TypeSymbol type)
		{
			return type.TypeKind == TypeKind.Array;
		}

		private static bool IsInterfaceType(TypeSymbol type)
		{
			return TypeSymbolExtensions.IsInterfaceType(type);
		}

		public static bool IsDerivedFrom(TypeSymbol derivedType, TypeSymbol baseType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return TypeSymbolExtensions.IsBaseTypeOf(baseType, derivedType, ref useSiteInfo);
		}

		private static ConversionKind ClassifyAnonymousDelegateConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (source.IsAnonymousType && TypeSymbolExtensions.IsDelegateType(source) && TypeSymbolExtensions.IsDelegateType(destination))
			{
				MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)destination).DelegateInvokeMethod;
				if ((object)delegateInvokeMethod == null || delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo != null)
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				MethodConversionKind methodConversionKind = ClassifyMethodConversionForLambdaOrAnonymousDelegate(delegateInvokeMethod, ((NamedTypeSymbol)source).DelegateInvokeMethod, ref useSiteInfo);
				if (!IsDelegateRelaxationSupportedFor(methodConversionKind))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				ConversionKind conversionKind = DetermineDelegateRelaxationLevel(methodConversionKind);
				if (IsStubRequiredForMethodConversion(methodConversionKind))
				{
					conversionKind |= ConversionKind.NeedAStub;
				}
				if (IsNarrowingMethodConversion(methodConversionKind, isForAddressOf: true))
				{
					return ConversionKind.Narrowing | ConversionKind.AnonymousDelegate | conversionKind;
				}
				return ConversionKind.Widening | ConversionKind.AnonymousDelegate | conversionKind;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyArrayConversion(TypeSymbol source, TypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!IsArrayType(source) || !IsArrayType(destination))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)source;
			ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)destination;
			if (!arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol elementType = arrayTypeSymbol.ElementType;
			TypeSymbol elementType2 = arrayTypeSymbol2.ElementType;
			if (elementType.Kind == SymbolKind.ErrorType || elementType2.Kind == SymbolKind.ErrorType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			return ClassifyArrayConversionBasedOnElementTypes(elementType, elementType2, varianceCompatibilityClassificationDepth, ref useSiteInfo);
		}

		public static ConversionKind ClassifyArrayElementConversion(TypeSymbol srcElem, TypeSymbol dstElem, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyIdentityConversion(srcElem, dstElem);
			if (ConversionExists(conversionKind))
			{
				return conversionKind;
			}
			return ClassifyArrayConversionBasedOnElementTypes(srcElem, dstElem, 0, ref useSiteInfo);
		}

		internal static ConversionKind Classify_Reference_Array_TypeParameterConversion(TypeSymbol srcElem, TypeSymbol dstElem, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ConversionKind conversionKind = ClassifyReferenceConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
			if (NoConversion(conversionKind) && (conversionKind & ConversionKind.MightSucceedAtRuntime) == 0)
			{
				conversionKind = ClassifyArrayConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				if (NoConversion(conversionKind) && (conversionKind & ConversionKind.MightSucceedAtRuntime) == 0)
				{
					conversionKind = ClassifyTypeParameterConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				}
			}
			return conversionKind;
		}

		private static ConversionKind ClassifyArrayConversionBasedOnElementTypes(TypeSymbol srcElem, TypeSymbol dstElem, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool isValueType = srcElem.IsValueType;
			bool isValueType2 = dstElem.IsValueType;
			if (!isValueType && !isValueType2)
			{
				ConversionKind conversionKind = Classify_Reference_Array_TypeParameterConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				if (IsWideningConversion(conversionKind))
				{
					if (srcElem.IsReferenceType && dstElem.IsReferenceType)
					{
						return ConversionKind.WideningArray | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
					}
					if (srcElem.Kind == SymbolKind.TypeParameter && dstElem.Kind == SymbolKind.TypeParameter)
					{
						if (srcElem.IsReferenceType)
						{
							return ConversionKind.WideningArray | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
						}
						_ = dstElem.IsReferenceType;
						return ConversionKind.NarrowingArray | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
					}
					if (srcElem.Kind == SymbolKind.TypeParameter || dstElem.Kind == SymbolKind.TypeParameter)
					{
						return ConversionKind.NarrowingArray | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
					}
				}
				else
				{
					if (IsNarrowingConversion(conversionKind))
					{
						return ConversionKind.NarrowingArray | (conversionKind & (ConversionKind.InvolvesEnumTypeConversions | ConversionKind.VarianceConversionAmbiguity));
					}
					if ((conversionKind & ConversionKind.MightSucceedAtRuntime) != 0)
					{
						return ConversionKind.MightSucceedAtRuntime;
					}
				}
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (isValueType)
			{
				if (isValueType2)
				{
					ConversionKind conversionKind2 = ConversionKind.DelegateRelaxationLevelNone;
					if (srcElem.Kind == SymbolKind.TypeParameter || dstElem.Kind == SymbolKind.TypeParameter)
					{
						ConversionKind conversionKind3 = ClassifyTypeParameterConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (IsWideningConversion(conversionKind3))
						{
							return ConversionKind.WideningArray | (conversionKind3 & ConversionKind.InvolvesEnumTypeConversions);
						}
						if (IsNarrowingConversion(conversionKind3))
						{
							return ConversionKind.NarrowingArray | (conversionKind3 & ConversionKind.InvolvesEnumTypeConversions);
						}
						if ((conversionKind3 & ConversionKind.MightSucceedAtRuntime) != 0)
						{
							conversionKind2 = ConversionKind.MightSucceedAtRuntime;
						}
					}
					TypeSymbol typeSymbol = srcElem;
					TypeSymbol typeSymbol2 = dstElem;
					if (srcElem.Kind == SymbolKind.TypeParameter)
					{
						TypeSymbol valueTypeConstraint = GetValueTypeConstraint(srcElem, ref useSiteInfo);
						if ((object)valueTypeConstraint != null)
						{
							typeSymbol = valueTypeConstraint;
						}
					}
					if (dstElem.Kind == SymbolKind.TypeParameter)
					{
						TypeSymbol valueTypeConstraint2 = GetValueTypeConstraint(dstElem, ref useSiteInfo);
						if ((object)valueTypeConstraint2 != null)
						{
							typeSymbol2 = valueTypeConstraint2;
						}
					}
					NamedTypeSymbol nonErrorEnumUnderlyingType = GetNonErrorEnumUnderlyingType(typeSymbol);
					NamedTypeSymbol nonErrorEnumUnderlyingType2 = GetNonErrorEnumUnderlyingType(typeSymbol2);
					if ((object)nonErrorEnumUnderlyingType != null)
					{
						if (TypeSymbolExtensions.IsNumericType(nonErrorEnumUnderlyingType))
						{
							if ((object)nonErrorEnumUnderlyingType2 != null)
							{
								if (nonErrorEnumUnderlyingType.Equals(nonErrorEnumUnderlyingType2))
								{
									return ConversionKind.NarrowingArray | ConversionKind.InvolvesEnumTypeConversions;
								}
							}
							else if (nonErrorEnumUnderlyingType.Equals(typeSymbol2))
							{
								if ((object)dstElem == typeSymbol2)
								{
									return ConversionKind.WideningArray | ConversionKind.InvolvesEnumTypeConversions;
								}
								return ConversionKind.NarrowingArray | ConversionKind.InvolvesEnumTypeConversions;
							}
						}
					}
					else if ((object)nonErrorEnumUnderlyingType2 != null && TypeSymbolExtensions.IsNumericType(nonErrorEnumUnderlyingType2) && nonErrorEnumUnderlyingType2.Equals(typeSymbol))
					{
						return ConversionKind.NarrowingArray | ConversionKind.InvolvesEnumTypeConversions;
					}
					if (conversionKind2 == ConversionKind.DelegateRelaxationLevelNone)
					{
						int num = ArrayElementBitSize(typeSymbol);
						if (num > 0 && num == ArrayElementBitSize(typeSymbol2))
						{
							conversionKind2 = ConversionKind.MightSucceedAtRuntime;
						}
					}
					return conversionKind2;
				}
				if (dstElem.Kind == SymbolKind.TypeParameter && !dstElem.IsReferenceType)
				{
					if (srcElem.Kind == SymbolKind.TypeParameter)
					{
						ConversionKind conversionKind4 = ClassifyTypeParameterConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (IsWideningConversion(conversionKind4))
						{
							return ConversionKind.NarrowingArray | (conversionKind4 & ConversionKind.InvolvesEnumTypeConversions);
						}
						if ((conversionKind4 & ConversionKind.MightSucceedAtRuntime) != 0)
						{
							return ConversionKind.MightSucceedAtRuntime;
						}
						return ConversionKind.DelegateRelaxationLevelNone;
					}
					if (ArrayElementBitSize(srcElem) > 0)
					{
						return ConversionKind.MightSucceedAtRuntime;
					}
				}
			}
			else if (srcElem.Kind == SymbolKind.TypeParameter && !srcElem.IsReferenceType)
			{
				if (dstElem.Kind == SymbolKind.TypeParameter)
				{
					ConversionKind conversionKind5 = ClassifyTypeParameterConversion(srcElem, dstElem, varianceCompatibilityClassificationDepth, ref useSiteInfo);
					if (IsNarrowingConversion(conversionKind5))
					{
						return ConversionKind.NarrowingArray | (conversionKind5 & ConversionKind.InvolvesEnumTypeConversions);
					}
					if ((conversionKind5 & ConversionKind.MightSucceedAtRuntime) != 0)
					{
						return ConversionKind.MightSucceedAtRuntime;
					}
				}
				else if (ArrayElementBitSize(dstElem) > 0)
				{
					return ConversionKind.MightSucceedAtRuntime;
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static int ArrayElementBitSize(TypeSymbol type)
		{
			switch (TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type).SpecialType)
			{
			case SpecialType.System_Boolean:
			case SpecialType.System_SByte:
			case SpecialType.System_Byte:
				return 8;
			case SpecialType.System_Int16:
			case SpecialType.System_UInt16:
				return 16;
			case SpecialType.System_Int32:
			case SpecialType.System_UInt32:
				return 32;
			case SpecialType.System_Int64:
			case SpecialType.System_UInt64:
				return 64;
			default:
				return 0;
			}
		}

		private static TypeSymbol GetValueTypeConstraint(TypeSymbol typeParam, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol nonInterfaceConstraint = ConstraintsHelper.GetNonInterfaceConstraint((TypeParameterSymbol)typeParam, ref useSiteInfo);
			if ((object)nonInterfaceConstraint != null && nonInterfaceConstraint.IsValueType)
			{
				return nonInterfaceConstraint;
			}
			return null;
		}

		private static NamedTypeSymbol GetNonErrorEnumUnderlyingType(TypeSymbol type)
		{
			if (type.TypeKind == TypeKind.Enum)
			{
				NamedTypeSymbol enumUnderlyingType = ((NamedTypeSymbol)type).EnumUnderlyingType;
				if (enumUnderlyingType.Kind != SymbolKind.ErrorType)
				{
					return enumUnderlyingType;
				}
			}
			return null;
		}

		private static ConversionKind ClassifyValueTypeConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (source.SpecialType == SpecialType.System_Void || destination.SpecialType == SpecialType.System_Void)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (IsValueType(source))
			{
				if (!TypeSymbolExtensions.IsRestrictedType(source))
				{
					if (destination.SpecialType == SpecialType.System_Object)
					{
						return ConversionKind.WideningValue;
					}
					if (IsClassType(destination))
					{
						if (IsDerivedFrom(source, destination, ref useSiteInfo))
						{
							return ConversionKind.WideningValue;
						}
					}
					else if (IsInterfaceType(destination))
					{
						ConversionKind conversionKind = ToInterfaceConversionClassifier.ClassifyConversionToVariantCompatibleInterface((NamedTypeSymbol)source, (NamedTypeSymbol)destination, 0, ref useSiteInfo);
						if (ConversionExists(conversionKind))
						{
							return conversionKind | ConversionKind.Value;
						}
					}
				}
			}
			else if (IsValueType(destination))
			{
				if (source.SpecialType == SpecialType.System_Object)
				{
					return ConversionKind.NarrowingValue;
				}
				if (IsClassType(source))
				{
					if (IsDerivedFrom(destination, source, ref useSiteInfo))
					{
						return ConversionKind.NarrowingValue;
					}
				}
				else if (IsInterfaceType(source))
				{
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = destination.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
					while (enumerator.MoveNext())
					{
						NamedTypeSymbol current = enumerator.Current;
						if (!TypeSymbolExtensions.IsErrorType(current) && TypeSymbolExtensions.IsSameTypeIgnoringAll(current, source))
						{
							return ConversionKind.NarrowingValue;
						}
					}
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyNullableConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool flag = TypeSymbolExtensions.IsNullableType(source);
			bool flag2 = TypeSymbolExtensions.IsNullableType(destination);
			if (!flag && !flag2)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol typeSymbol = null;
			TypeSymbol typeSymbol2 = null;
			if (flag)
			{
				typeSymbol = TypeSymbolExtensions.GetNullableUnderlyingType(source);
				if (typeSymbol.Kind == SymbolKind.ErrorType || !typeSymbol.IsValueType || TypeSymbolExtensions.IsNullableType(typeSymbol))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
			}
			if (flag2)
			{
				typeSymbol2 = TypeSymbolExtensions.GetNullableUnderlyingType(destination);
				if (typeSymbol2.Kind == SymbolKind.ErrorType || !typeSymbol2.IsValueType || TypeSymbolExtensions.IsNullableType(typeSymbol2))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
			}
			if (flag)
			{
				if (flag2)
				{
					ConversionKind conversionKind = ClassifyPredefinedConversion(typeSymbol, typeSymbol2, ref useSiteInfo);
					if (IsWideningConversion(conversionKind))
					{
						return ConversionKind.WideningNullable | (conversionKind & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Tuple));
					}
					if (IsNarrowingConversion(conversionKind))
					{
						return ConversionKind.NarrowingNullable | (conversionKind & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Tuple));
					}
				}
				else if (IsInterfaceType(destination))
				{
					ConversionKind conversionKind = ClassifyDirectCastConversion(typeSymbol, destination, ref useSiteInfo);
					if (IsWideningConversion(conversionKind))
					{
						return ConversionKind.WideningNullable;
					}
					if (IsNarrowingConversion(conversionKind))
					{
						return ConversionKind.NarrowingNullable;
					}
				}
				else
				{
					if (TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, destination))
					{
						return ConversionKind.NarrowingNullable;
					}
					ConversionKind conversionKind = ClassifyPredefinedConversion(typeSymbol, destination, ref useSiteInfo);
					if (ConversionExists(conversionKind))
					{
						return ConversionKind.NarrowingNullable | (conversionKind & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Tuple));
					}
				}
			}
			else
			{
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(source, typeSymbol2))
				{
					return ConversionKind.WideningNullable;
				}
				ConversionKind conversionKind2 = ClassifyPredefinedConversion(source, typeSymbol2, ref useSiteInfo);
				if (IsWideningConversion(conversionKind2))
				{
					return ConversionKind.WideningNullable | (conversionKind2 & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Tuple));
				}
				if (IsNarrowingConversion(conversionKind2))
				{
					return ConversionKind.NarrowingNullable | (conversionKind2 & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Tuple));
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyTupleConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!source.IsTupleType)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<TypeSymbol> tupleElementTypes = ((TupleTypeSymbol)source).TupleElementTypes;
			if (!destination.IsTupleOrCompatibleWithTupleOfCardinality(tupleElementTypes.Length))
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			ImmutableArray<TypeSymbol> elementTypesOfTupleOrCompatible = TypeSymbolExtensions.GetElementTypesOfTupleOrCompatible(destination);
			ConversionKind conversionKind = ConversionKind.WideningTuple;
			ConversionKind conversionKind2 = ConversionKind.DelegateRelaxationLevelNone;
			int num = tupleElementTypes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeSymbol = tupleElementTypes[i];
				TypeSymbol typeSymbol2 = elementTypesOfTupleOrCompatible[i];
				if (TypeSymbolExtensions.IsErrorType(typeSymbol) || TypeSymbolExtensions.IsErrorType(typeSymbol2))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				ConversionKind key = ClassifyConversion(typeSymbol, typeSymbol2, ref useSiteInfo).Key;
				if (NoConversion(key))
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				ConversionKind conversionKind3 = key & ConversionKind.DelegateRelaxationLevelMask;
				if (conversionKind3 > conversionKind2)
				{
					conversionKind2 = conversionKind3;
				}
				if (IsNarrowingConversion(key))
				{
					conversionKind = ConversionKind.NarrowingTuple;
				}
			}
			return conversionKind | conversionKind2;
		}

		public static ConversionKind ClassifyStringConversion(TypeSymbol source, TypeSymbol destination)
		{
			TypeSymbol typeSymbol;
			if (source.SpecialType == SpecialType.System_String)
			{
				typeSymbol = destination;
			}
			else
			{
				if (destination.SpecialType != SpecialType.System_String)
				{
					return ConversionKind.DelegateRelaxationLevelNone;
				}
				typeSymbol = source;
			}
			if (typeSymbol.Kind == SymbolKind.ArrayType)
			{
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)typeSymbol;
				if (arrayTypeSymbol.IsSZArray && arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_Char)
				{
					if ((object)arrayTypeSymbol == source)
					{
						return ConversionKind.WideningString;
					}
					return ConversionKind.NarrowingString;
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyTypeParameterConversion(TypeSymbol source, TypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (source.Kind == SymbolKind.TypeParameter)
			{
				ConversionKind conversionKind = ClassifyConversionFromTypeParameter((TypeParameterSymbol)source, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				if (ConversionExists(conversionKind))
				{
					return conversionKind;
				}
			}
			if (destination.Kind == SymbolKind.TypeParameter)
			{
				ConversionKind conversionKind = ClassifyConversionToTypeParameter(source, (TypeParameterSymbol)destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
				if (ConversionExists(conversionKind))
				{
					return conversionKind;
				}
			}
			if (source.Kind == SymbolKind.TypeParameter || destination.Kind == SymbolKind.TypeParameter)
			{
				return ConversionKind.MightSucceedAtRuntime;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyConversionFromTypeParameter(TypeParameterSymbol typeParameter, TypeSymbol destination, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (destination.SpecialType == SpecialType.System_Object)
			{
				return ConversionKind.WideningTypeParameter;
			}
			ArrayBuilder<TypeParameterSymbol> queue = null;
			ConversionKind result = ClassifyConversionFromTypeParameter(typeParameter, destination, ref queue, varianceCompatibilityClassificationDepth, ref useSiteInfo);
			queue?.Free();
			return result;
		}

		private static ConversionKind ClassifyConversionFromTypeParameter(TypeParameterSymbol typeParameter, TypeSymbol destination, [In][Out] ref ArrayBuilder<TypeParameterSymbol> queue, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = 0;
			bool flag = false;
			ToInterfaceConversionClassifier toInterfaceConversionClassifier = default(ToInterfaceConversionClassifier);
			NamedTypeSymbol destination2 = null;
			bool isClassType = default(bool);
			bool isDelegateType = default(bool);
			bool isInterfaceType = default(bool);
			bool isArrayType = default(bool);
			ClassifyAsReferenceType(destination, ref isClassType, ref isDelegateType, ref isInterfaceType, ref isArrayType);
			if (isInterfaceType)
			{
				destination2 = (NamedTypeSymbol)destination;
			}
			bool isClassType2 = default(bool);
			bool isDelegateType2 = default(bool);
			bool isInterfaceType2 = default(bool);
			bool isArrayType2 = default(bool);
			while (true)
			{
				if (!flag && typeParameter.HasValueTypeConstraint)
				{
					if (destination.SpecialType == SpecialType.System_ValueType)
					{
						return ConversionKind.WideningTypeParameter;
					}
					if (isInterfaceType)
					{
						NamedTypeSymbol specialType = typeParameter.ContainingAssembly.GetSpecialType(SpecialType.System_ValueType);
						if (specialType.Kind != SymbolKind.ErrorType && toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface(specialType, destination2, varianceCompatibilityClassificationDepth, ref useSiteInfo))
						{
							return ConversionKind.WideningTypeParameter;
						}
					}
					flag = true;
				}
				ImmutableArray<TypeSymbol>.Enumerator enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeSymbol current = enumerator.Current;
					if (current.Kind == SymbolKind.ErrorType)
					{
						continue;
					}
					if (TypeSymbolExtensions.IsSameTypeIgnoringAll(current, destination))
					{
						return ConversionKind.WideningTypeParameter;
					}
					if (current.TypeKind == TypeKind.Enum && TypeSymbolExtensions.IsSameTypeIgnoringAll(((NamedTypeSymbol)current).EnumUnderlyingType, destination))
					{
						return ConversionKind.WideningTypeParameter | ConversionKind.InvolvesEnumTypeConversions;
					}
					bool flag2 = false;
					if (!ClassifyAsReferenceType(current, ref isClassType2, ref isDelegateType2, ref isInterfaceType2, ref isArrayType2))
					{
						flag2 = IsValueType(current);
					}
					if (isInterfaceType)
					{
						if (isClassType2 || isInterfaceType2 || flag2)
						{
							if (toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface((NamedTypeSymbol)current, destination2, varianceCompatibilityClassificationDepth, ref useSiteInfo))
							{
								return ConversionKind.WideningTypeParameter;
							}
						}
						else if (isArrayType2)
						{
							ConversionKind conversionKind = ClassifyReferenceConversionFromArrayToAnInterface(current, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
							if (IsWideningConversion(conversionKind))
							{
								return ConversionKind.WideningTypeParameter | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
							}
						}
					}
					else if (isClassType)
					{
						if ((isClassType2 || flag2 || isArrayType2) && IsDerivedFrom(current, destination, ref useSiteInfo))
						{
							return ConversionKind.WideningTypeParameter;
						}
					}
					else if (isArrayType && isArrayType2)
					{
						ConversionKind conversionKind2 = ClassifyArrayConversion(current, destination, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (IsWideningConversion(conversionKind2))
						{
							return ConversionKind.WideningTypeParameter | (conversionKind2 & ConversionKind.InvolvesEnumTypeConversions);
						}
					}
					if (current.Kind == SymbolKind.TypeParameter)
					{
						if (queue == null)
						{
							queue = ArrayBuilder<TypeParameterSymbol>.GetInstance();
						}
						queue.Add((TypeParameterSymbol)current);
					}
				}
				if (queue == null || num >= queue.Count)
				{
					break;
				}
				typeParameter = queue[num];
				num++;
			}
			if (isInterfaceType)
			{
				ConversionKind result = toInterfaceConversionClassifier.Result;
				if (ConversionExists(result))
				{
					return ConversionKind.TypeParameter | result;
				}
				return ConversionKind.NarrowingTypeParameter;
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		private static ConversionKind ClassifyConversionToTypeParameter(TypeSymbol source, TypeParameterSymbol typeParameter, int varianceCompatibilityClassificationDepth, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (source.SpecialType == SpecialType.System_Object)
			{
				return ConversionKind.NarrowingTypeParameter;
			}
			if (typeParameter.HasValueTypeConstraint)
			{
				if (source.SpecialType == SpecialType.System_ValueType)
				{
					return ConversionKind.NarrowingTypeParameter;
				}
				if (IsClassType(source))
				{
					NamedTypeSymbol specialType = typeParameter.ContainingAssembly.GetSpecialType(SpecialType.System_ValueType);
					if (specialType.Kind != SymbolKind.ErrorType && IsDerivedFrom(specialType, source, ref useSiteInfo))
					{
						return ConversionKind.NarrowingTypeParameter;
					}
				}
			}
			bool isClassType = default(bool);
			bool isDelegateType = default(bool);
			bool isInterfaceType = default(bool);
			bool isArrayType = default(bool);
			ClassifyAsReferenceType(source, ref isClassType, ref isDelegateType, ref isInterfaceType, ref isArrayType);
			if (isInterfaceType)
			{
				return ConversionKind.NarrowingTypeParameter;
			}
			ImmutableArray<TypeSymbol>.Enumerator enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
			bool isClassType2 = default(bool);
			bool isDelegateType2 = default(bool);
			bool isInterfaceType2 = default(bool);
			bool isArrayType2 = default(bool);
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				if (current.Kind == SymbolKind.ErrorType)
				{
					continue;
				}
				if (TypeSymbolExtensions.IsSameTypeIgnoringAll(current, source))
				{
					return ConversionKind.NarrowingTypeParameter;
				}
				if (current.TypeKind == TypeKind.Enum && TypeSymbolExtensions.IsSameTypeIgnoringAll(((NamedTypeSymbol)current).EnumUnderlyingType, source))
				{
					return ConversionKind.NarrowingTypeParameter | ConversionKind.InvolvesEnumTypeConversions;
				}
				bool flag = false;
				if (!ClassifyAsReferenceType(current, ref isClassType2, ref isDelegateType2, ref isInterfaceType2, ref isArrayType2))
				{
					flag = IsValueType(current);
				}
				if (isClassType2 || flag || isArrayType2)
				{
					if (isClassType)
					{
						if (IsDerivedFrom(current, source, ref useSiteInfo))
						{
							return ConversionKind.NarrowingTypeParameter;
						}
					}
					else if (isArrayType && isArrayType2)
					{
						ConversionKind conversionKind = ClassifyArrayConversion(current, source, varianceCompatibilityClassificationDepth, ref useSiteInfo);
						if (IsWideningConversion(conversionKind))
						{
							return ConversionKind.NarrowingTypeParameter | (conversionKind & ConversionKind.InvolvesEnumTypeConversions);
						}
					}
				}
				else if (current.Kind == SymbolKind.TypeParameter)
				{
					ConversionKind conversionKind2 = ClassifyTypeParameterConversion(source, current, varianceCompatibilityClassificationDepth, ref useSiteInfo);
					if (IsNarrowingConversion(conversionKind2))
					{
						return ConversionKind.NarrowingTypeParameter | (conversionKind2 & ConversionKind.InvolvesEnumTypeConversions);
					}
				}
			}
			return ConversionKind.DelegateRelaxationLevelNone;
		}

		public static MethodConversionKind ClassifyMethodConversionBasedOnReturn(TypeSymbol returnTypeOfConvertFromMethod, bool convertFromMethodIsByRef, TypeSymbol returnTypeOfConvertToMethod, bool convertToMethodIsByRef, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (convertToMethodIsByRef != convertFromMethodIsByRef)
			{
				return MethodConversionKind.Error_ByRefByValMismatch;
			}
			return ClassifyMethodConversionBasedOnReturnType(returnTypeOfConvertFromMethod, returnTypeOfConvertToMethod, convertFromMethodIsByRef, ref useSiteInfo);
		}

		public static MethodConversionKind ClassifyMethodConversionBasedOnReturnType(TypeSymbol returnTypeOfConvertFromMethod, TypeSymbol returnTypeOfConvertToMethod, bool isRefReturning, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsVoidType(returnTypeOfConvertToMethod))
			{
				if (TypeSymbolExtensions.IsVoidType(returnTypeOfConvertFromMethod))
				{
					return MethodConversionKind.Identity;
				}
				return MethodConversionKind.ReturnValueIsDropped;
			}
			if (TypeSymbolExtensions.IsVoidType(returnTypeOfConvertFromMethod))
			{
				return MethodConversionKind.Error_SubToFunction;
			}
			if (TypeSymbolExtensions.IsErrorType(returnTypeOfConvertFromMethod) || TypeSymbolExtensions.IsErrorType(returnTypeOfConvertToMethod))
			{
				if ((object)returnTypeOfConvertFromMethod == returnTypeOfConvertToMethod && (object)returnTypeOfConvertFromMethod == LambdaSymbol.ReturnTypeVoidReplacement)
				{
					return MethodConversionKind.Identity;
				}
				return MethodConversionKind.Error_Unspecified;
			}
			ConversionKind key = ClassifyConversion(returnTypeOfConvertFromMethod, returnTypeOfConvertToMethod, ref useSiteInfo).Key;
			if (isRefReturning && !IsIdentityConversion(key))
			{
				return MethodConversionKind.Error_ReturnTypeMismatch;
			}
			return IsNarrowingConversion(key) ? MethodConversionKind.ReturnIsWidening : ((!IsWideningConversion(key)) ? MethodConversionKind.Error_ReturnTypeMismatch : ((!IsIdentityConversion(key)) ? ((returnTypeOfConvertFromMethod.IsReferenceType && returnTypeOfConvertToMethod.IsReferenceType && (key & ConversionKind.UserDefined) == 0) ? ((!IsWideningConversion(ClassifyDirectCastConversion(returnTypeOfConvertFromMethod, returnTypeOfConvertToMethod, ref useSiteInfo))) ? MethodConversionKind.ReturnIsIsVbOrBoxNarrowing : MethodConversionKind.ReturnIsClrNarrowing) : MethodConversionKind.ReturnIsIsVbOrBoxNarrowing) : MethodConversionKind.Identity));
		}

		public static MethodConversionKind ClassifyMethodConversionBasedOnArgumentConversion(ConversionKind conversion, TypeSymbol delegateParameterType)
		{
			if (NoConversion(conversion))
			{
				return MethodConversionKind.Error_OverloadResolution;
			}
			if (IsNarrowingConversion(conversion))
			{
				return MethodConversionKind.OneArgumentIsNarrowing;
			}
			if (!IsIdentityConversion(conversion))
			{
				if (IsCLRPredefinedConversion(conversion) && delegateParameterType.IsReferenceType)
				{
					return MethodConversionKind.OneArgumentIsClrWidening;
				}
				return MethodConversionKind.OneArgumentIsVbOrBoxWidening;
			}
			return MethodConversionKind.Identity;
		}

		public static MethodConversionKind ClassifyMethodConversionForLambdaOrAnonymousDelegate(MethodSymbol toMethod, MethodSymbol lambdaOrDelegateInvokeSymbol, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ClassifyMethodConversionForLambdaOrAnonymousDelegate(new UnboundLambda.TargetSignature(toMethod), lambdaOrDelegateInvokeSymbol, ref useSiteInfo);
		}

		public static MethodConversionKind ClassifyMethodConversionForLambdaOrAnonymousDelegate(UnboundLambda.TargetSignature toMethodSignature, MethodSymbol lambdaOrDelegateInvokeSymbol, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ClassifyMethodConversionBasedOnReturn(lambdaOrDelegateInvokeSymbol.ReturnType, lambdaOrDelegateInvokeSymbol.ReturnsByRef, toMethodSignature.ReturnType, toMethodSignature.ReturnsByRef, ref useSiteInfo) | ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(toMethodSignature, lambdaOrDelegateInvokeSymbol.Parameters, ref useSiteInfo);
		}

		public static MethodConversionKind ClassifyMethodConversionForEventRaise(MethodSymbol toDelegateInvokeMethod, ImmutableArray<ParameterSymbol> parameters, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(new UnboundLambda.TargetSignature(toDelegateInvokeMethod), parameters, ref useSiteInfo);
		}

		private static MethodConversionKind ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(UnboundLambda.TargetSignature toMethodSignature, ImmutableArray<ParameterSymbol> parameters, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			MethodConversionKind methodConversionKind = default(MethodConversionKind);
			if (parameters.Length == 0 && toMethodSignature.ParameterTypes.Length > 0)
			{
				methodConversionKind |= MethodConversionKind.AllArgumentsIgnored;
			}
			else if (parameters.Length != toMethodSignature.ParameterTypes.Length)
			{
				methodConversionKind |= MethodConversionKind.Error_OverloadResolution;
			}
			else
			{
				int num = parameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (toMethodSignature.ParameterIsByRef[i] != parameters[i].IsByRef)
					{
						methodConversionKind |= MethodConversionKind.Error_ByRefByValMismatch;
					}
					TypeSymbol typeSymbol = toMethodSignature.ParameterTypes[i];
					TypeSymbol type = parameters[i].Type;
					if (!TypeSymbolExtensions.IsErrorType(typeSymbol) && !TypeSymbolExtensions.IsErrorType(type))
					{
						methodConversionKind |= ClassifyMethodConversionBasedOnArgumentConversion(ClassifyConversion(typeSymbol, type, ref useSiteInfo).Key, typeSymbol);
						if (toMethodSignature.ParameterIsByRef[i])
						{
							methodConversionKind |= ClassifyMethodConversionBasedOnArgumentConversion(ClassifyConversion(type, typeSymbol, ref useSiteInfo).Key, type);
						}
					}
				}
			}
			return methodConversionKind;
		}

		public static ConversionKind DetermineDelegateRelaxationLevelForLambdaReturn(BoundExpression expressionOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (expressionOpt == null || expressionOpt.Kind != BoundKind.Conversion || expressionOpt.HasErrors)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			BoundConversion boundConversion = (BoundConversion)expressionOpt;
			if (boundConversion.ExplicitCastInCode)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			TypeSymbol type = boundConversion.Operand.Type;
			MethodConversionKind methodConversion = (((object)type != null) ? ClassifyMethodConversionBasedOnReturnType(type, boundConversion.Type, isRefReturning: false, ref useSiteInfo) : MethodConversionKind.Identity);
			return DetermineDelegateRelaxationLevel(methodConversion);
		}

		public static ConversionKind DetermineDelegateRelaxationLevel(MethodConversionKind methodConversion)
		{
			if (methodConversion == MethodConversionKind.Identity)
			{
				return ConversionKind.DelegateRelaxationLevelNone;
			}
			if (!IsDelegateRelaxationSupportedFor(methodConversion))
			{
				return ConversionKind.DelegateRelaxationLevelInvalid;
			}
			if ((methodConversion & (MethodConversionKind.OneArgumentIsNarrowing | MethodConversionKind.ReturnIsWidening)) != 0)
			{
				return ConversionKind.DelegateRelaxationLevelNarrowing;
			}
			if ((methodConversion & (MethodConversionKind.ReturnValueIsDropped | MethodConversionKind.AllArgumentsIgnored)) == 0)
			{
				return ConversionKind.DelegateRelaxationLevelWidening;
			}
			return ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs;
		}

		public static bool IsDelegateRelaxationSupportedFor(MethodConversionKind methodConversion)
		{
			return (methodConversion & MethodConversionKind.AllErrorReasons) == 0;
		}

		public static bool IsStubRequiredForMethodConversion(MethodConversionKind methodConversions)
		{
			if ((methodConversions & (MethodConversionKind.OneArgumentIsVbOrBoxWidening | MethodConversionKind.OneArgumentIsNarrowing | MethodConversionKind.ReturnIsWidening | MethodConversionKind.ReturnIsIsVbOrBoxNarrowing | MethodConversionKind.ReturnValueIsDropped | MethodConversionKind.AllArgumentsIgnored | MethodConversionKind.ExcessOptionalArgumentsOnTarget)) != 0)
			{
				return (methodConversions & MethodConversionKind.AllErrorReasons) == 0;
			}
			return false;
		}

		public static bool IsNarrowingMethodConversion(MethodConversionKind methodConversion, bool isForAddressOf)
		{
			MethodConversionKind methodConversionKind = ((!isForAddressOf) ? (MethodConversionKind.OneArgumentIsNarrowing | MethodConversionKind.ReturnIsWidening) : (MethodConversionKind.OneArgumentIsNarrowing | MethodConversionKind.ReturnIsWidening | MethodConversionKind.AllArgumentsIgnored));
			return (methodConversion & methodConversionKind) != 0;
		}

		public static RequiredConversion InvertConversionRequirement(RequiredConversion restriction)
		{
			return restriction switch
			{
				RequiredConversion.AnyReverse => RequiredConversion.Any, 
				RequiredConversion.ReverseReference => RequiredConversion.Reference, 
				RequiredConversion.Any => RequiredConversion.AnyReverse, 
				RequiredConversion.ArrayElement => RequiredConversion.ReverseReference, 
				RequiredConversion.Reference => RequiredConversion.ReverseReference, 
				_ => restriction, 
			};
		}

		public static RequiredConversion StrengthenConversionRequirementToReference(RequiredConversion restriction)
		{
			switch (restriction)
			{
			case RequiredConversion.AnyReverse:
				return RequiredConversion.ReverseReference;
			case RequiredConversion.Any:
			case RequiredConversion.ArrayElement:
				return RequiredConversion.Reference;
			case RequiredConversion.AnyAndReverse:
				return RequiredConversion.Identity;
			default:
				return restriction;
			}
		}

		public static RequiredConversion CombineConversionRequirements(RequiredConversion restriction1, RequiredConversion restriction2)
		{
			if (restriction1 == restriction2)
			{
				return restriction1;
			}
			if (restriction1 == RequiredConversion.None)
			{
				return restriction2;
			}
			if (restriction2 == RequiredConversion.None)
			{
				return restriction1;
			}
			if (restriction1 == RequiredConversion.Identity || restriction2 == RequiredConversion.Identity)
			{
				return RequiredConversion.Identity;
			}
			if ((restriction1 == RequiredConversion.AnyReverse || restriction1 == RequiredConversion.ReverseReference) && (restriction2 == RequiredConversion.AnyReverse || restriction2 == RequiredConversion.ReverseReference))
			{
				return RequiredConversion.ReverseReference;
			}
			if ((restriction1 == RequiredConversion.Any || restriction1 == RequiredConversion.AnyReverse || restriction1 == RequiredConversion.AnyAndReverse) && (restriction2 == RequiredConversion.Any || restriction2 == RequiredConversion.AnyReverse || restriction2 == RequiredConversion.AnyAndReverse))
			{
				return RequiredConversion.AnyAndReverse;
			}
			if ((restriction1 == RequiredConversion.Any || restriction1 == RequiredConversion.ArrayElement) && (restriction2 == RequiredConversion.Any || restriction2 == RequiredConversion.ArrayElement))
			{
				return RequiredConversion.ArrayElement;
			}
			if ((restriction1 == RequiredConversion.Any || restriction1 == RequiredConversion.ArrayElement || restriction1 == RequiredConversion.Reference) && (restriction2 == RequiredConversion.Any || restriction2 == RequiredConversion.ArrayElement || restriction2 == RequiredConversion.Reference))
			{
				return RequiredConversion.Reference;
			}
			return RequiredConversion.Identity;
		}

		public static bool IsWideningConversion(ConversionKind conv)
		{
			return (conv & ConversionKind.Widening) != 0;
		}

		public static bool IsNarrowingConversion(ConversionKind conv)
		{
			return (conv & ConversionKind.Narrowing) != 0;
		}

		public static bool NoConversion(ConversionKind conv)
		{
			return (conv & (ConversionKind.Widening | ConversionKind.Narrowing)) == 0;
		}

		public static bool ConversionExists(ConversionKind conv)
		{
			return (conv & (ConversionKind.Widening | ConversionKind.Narrowing)) != 0;
		}

		public static bool IsIdentityConversion(ConversionKind conv)
		{
			return (conv & ConversionKind.Identity) == ConversionKind.Identity;
		}

		public static bool FailedDueToNumericOverflow(ConversionKind conv)
		{
			return (conv & (ConversionKind.FailedDueToNumericOverflow | ConversionKind.Widening | ConversionKind.Narrowing)) == ConversionKind.FailedDueToNumericOverflow;
		}

		public static bool FailedDueToQueryLambdaBodyMismatch(ConversionKind conv)
		{
			return (conv & (ConversionKind.FailedDueToQueryLambdaBodyMismatch | ConversionKind.Widening | ConversionKind.Narrowing)) == ConversionKind.FailedDueToQueryLambdaBodyMismatch;
		}

		public static bool IsCLRPredefinedConversion(ConversionKind conversion)
		{
			if (IsIdentityConversion(conversion))
			{
				return true;
			}
			if ((conversion & (ConversionKind.Reference | ConversionKind.Array | ConversionKind.TypeParameter)) != 0)
			{
				return true;
			}
			return false;
		}
	}
}
