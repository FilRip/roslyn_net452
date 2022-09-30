using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class ConstraintsHelper
	{
		private enum DirectTypeConstraintKind
		{
			None,
			ReferenceTypeConstraint,
			ValueTypeConstraint,
			ExplicitType
		}

		private class CheckConstraintsDiagnosticsBuilders
		{
			public ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder;

			public ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder;

			public CompoundUseSiteInfo<AssemblySymbol> template;
		}

		private struct TypeParameterAndConstraint
		{
			public readonly TypeParameterSymbol TypeParameter;

			public readonly TypeParameterConstraint Constraint;

			public readonly bool IsBad;

			public TypeParameterAndConstraint(TypeParameterSymbol typeParameter, TypeParameterConstraint constraint, bool isBad = false)
			{
				this = default(TypeParameterAndConstraint);
				TypeParameter = typeParameter;
				Constraint = constraint;
				IsBad = isBad;
			}

			public TypeParameterAndConstraint ToBad()
			{
				return new TypeParameterAndConstraint(TypeParameter, Constraint, isBad: true);
			}

			public override string ToString()
			{
				string text = $"{TypeParameter} : {Constraint}";
				if (IsBad)
				{
					text += " (bad)";
				}
				return text;
			}
		}

		private static readonly Func<TypeSymbol, CheckConstraintsDiagnosticsBuilders, bool> s_checkConstraintsSingleTypeFunc = CheckConstraintsSingleType;

		public static ImmutableArray<TypeParameterConstraint> RemoveDirectConstraintConflicts(this TypeParameterSymbol typeParameter, ImmutableArray<TypeParameterConstraint> constraints, ConsList<TypeParameterSymbol> inProgress, DirectConstraintConflictKind reportConflicts, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder)
		{
			if (constraints.Length > 0)
			{
				ArrayBuilder<TypeParameterConstraint> instance = ArrayBuilder<TypeParameterConstraint>.GetInstance();
				Symbol containingSymbol = typeParameter.ContainingSymbol;
				DirectTypeConstraintKind directTypeConstraintKind = DirectTypeConstraintKind.None;
				bool flag = (reportConflicts & DirectConstraintConflictKind.RedundantConstraint) != 0;
				ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = constraints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeParameterConstraint current = enumerator.Current;
					switch (current.Kind)
					{
					case TypeParameterConstraintKind.ReferenceType:
						if (flag)
						{
							if (directTypeConstraintKind != 0)
							{
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_RefAndClassTypeConstrCombined)));
								continue;
							}
							directTypeConstraintKind = DirectTypeConstraintKind.ReferenceTypeConstraint;
						}
						break;
					case TypeParameterConstraintKind.ValueType:
						if (flag)
						{
							if (directTypeConstraintKind != 0)
							{
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ValueAndClassTypeConstrCombined)));
								continue;
							}
							directTypeConstraintKind = DirectTypeConstraintKind.ValueTypeConstraint;
						}
						break;
					case TypeParameterConstraintKind.None:
					{
						TypeSymbol typeConstraint = current.TypeConstraint;
						bool flag2 = ContainsTypeConstraint(instance, typeConstraint);
						if (flag2 && (reportConflicts & DirectConstraintConflictKind.DuplicateTypeConstraint) != 0)
						{
							diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintAlreadyExists1, typeConstraint)));
						}
						switch (typeConstraint.TypeKind)
						{
						case TypeKind.Class:
							if (!flag)
							{
								break;
							}
							switch (directTypeConstraintKind)
							{
							case DirectTypeConstraintKind.None:
								if (((NamedTypeSymbol)typeConstraint).IsNotInheritable)
								{
									diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ClassConstraintNotInheritable1)));
								}
								else
								{
									SpecialType specialType = typeConstraint.SpecialType;
									if ((uint)(specialType - 1) <= 4u || specialType == SpecialType.System_Array)
									{
										diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintIsRestrictedType1, typeConstraint)));
									}
								}
								directTypeConstraintKind = DirectTypeConstraintKind.ExplicitType;
								break;
							case DirectTypeConstraintKind.ReferenceTypeConstraint:
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_RefAndClassTypeConstrCombined)));
								continue;
							case DirectTypeConstraintKind.ValueTypeConstraint:
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ValueAndClassTypeConstrCombined)));
								continue;
							case DirectTypeConstraintKind.ExplicitType:
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_MultipleClassConstraints1, typeParameter)));
								continue;
							}
							break;
						case TypeKind.TypeParameter:
						{
							TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)typeConstraint;
							if (typeParameterSymbol.ContainingSymbol == containingSymbol)
							{
								if (inProgress.ContainsReference(typeParameterSymbol))
								{
									diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameterSymbol, current, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintCycle2, typeParameterSymbol, GetConstraintCycleInfo(inProgress))));
									continue;
								}
								typeParameterSymbol.ResolveConstraints(inProgress);
							}
							if (flag && typeParameterSymbol.HasValueTypeConstraint)
							{
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameterSymbol, current, ErrorFactory.ErrorInfo(ERRID.ERR_TypeParamWithStructConstAsConst)));
								continue;
							}
							break;
						}
						case TypeKind.Array:
						case TypeKind.Delegate:
						case TypeKind.Enum:
						case TypeKind.Struct:
							if (flag)
							{
								diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, current, ErrorFactory.ErrorInfo(ERRID.ERR_ConstNotClassInterfaceOrTypeParam1, typeConstraint)));
							}
							break;
						default:
							throw ExceptionUtilities.UnexpectedValue(typeConstraint.TypeKind);
						case TypeKind.Error:
						case TypeKind.Interface:
						case TypeKind.Module:
							break;
						}
						if (flag2)
						{
							continue;
						}
						break;
					}
					}
					instance.Add(current);
				}
				if (instance.Count != constraints.Length)
				{
					constraints = instance.ToImmutable();
				}
				instance.Free();
			}
			return constraints;
		}

		public static void ReportIndirectConstraintConflicts(this SourceTypeParameterSymbol typeParameter, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder)
		{
			ArrayBuilder<TypeParameterAndConstraint> instance = ArrayBuilder<TypeParameterAndConstraint>.GetInstance();
			GetAllConstraints(typeParameter, instance, null);
			int count = instance.Count;
			int num = count - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeParameterAndConstraint typeParameterAndConstraint = instance[i];
				if (typeParameterAndConstraint.IsBad)
				{
					continue;
				}
				int num2 = i + 1;
				int num3 = count - 1;
				for (int j = num2; j <= num3; j++)
				{
					TypeParameterAndConstraint typeParameterAndConstraint2 = instance[j];
					if (typeParameterAndConstraint2.IsBad)
					{
						continue;
					}
					bool flag = false;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(typeParameter.ContainingAssembly);
					if ((object)typeParameterAndConstraint.TypeParameter == typeParameter && (object)typeParameterAndConstraint2.TypeParameter == typeParameter)
					{
						if (HasConflict(typeParameterAndConstraint.Constraint, typeParameterAndConstraint2.Constraint, ref useSiteInfo))
						{
							diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint2.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConflictingDirectConstraints3, typeParameterAndConstraint2.Constraint.ToDisplayFormat(), typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameter)));
							flag = true;
						}
					}
					else if ((object)typeParameterAndConstraint.TypeParameter != typeParameterAndConstraint2.TypeParameter && HasConflict(typeParameterAndConstraint.Constraint, typeParameterAndConstraint2.Constraint, ref useSiteInfo))
					{
						if ((object)typeParameterAndConstraint.TypeParameter == typeParameter)
						{
							diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashDirectIndirect3, typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameterAndConstraint2.Constraint.ToDisplayFormat(), typeParameterAndConstraint2.TypeParameter)));
							flag = true;
						}
						else if ((object)typeParameterAndConstraint2.TypeParameter == typeParameter)
						{
							diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashIndirectDirect3, typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameterAndConstraint.TypeParameter, typeParameterAndConstraint2.Constraint.ToDisplayFormat())));
							flag = true;
						}
						else
						{
							diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, typeParameterAndConstraint2.Constraint, ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintClashIndirectIndirect4, typeParameterAndConstraint2.Constraint.ToDisplayFormat(), typeParameterAndConstraint2.TypeParameter, typeParameterAndConstraint.Constraint.ToDisplayFormat(), typeParameterAndConstraint.TypeParameter)));
							flag = true;
						}
					}
					if (AppendUseSiteInfo(useSiteInfo, typeParameter, ref useSiteDiagnosticsBuilder))
					{
						flag = true;
					}
					if (flag)
					{
						instance[j] = typeParameterAndConstraint2.ToBad();
					}
				}
			}
			instance.Free();
		}

		public static void CheckAllConstraints(this TypeSymbol type, Location loc, BindingDiagnosticBag diagnostics, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
			CheckAllConstraints(type, instance, ref useSiteDiagnosticsBuilder, template);
			if (useSiteDiagnosticsBuilder != null)
			{
				instance.AddRange(useSiteDiagnosticsBuilder);
			}
			ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				diagnostics.Add(enumerator.Current.UseSiteInfo, loc);
			}
			instance.Free();
		}

		public static void CheckAllConstraints(this TypeSymbol type, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			CheckConstraintsDiagnosticsBuilders checkConstraintsDiagnosticsBuilders = new CheckConstraintsDiagnosticsBuilders();
			checkConstraintsDiagnosticsBuilders.diagnosticsBuilder = diagnosticsBuilder;
			checkConstraintsDiagnosticsBuilders.useSiteDiagnosticsBuilder = useSiteDiagnosticsBuilder;
			checkConstraintsDiagnosticsBuilders.template = template;
			TypeSymbolExtensions.VisitType(type, s_checkConstraintsSingleTypeFunc, checkConstraintsDiagnosticsBuilders);
			useSiteDiagnosticsBuilder = checkConstraintsDiagnosticsBuilders.useSiteDiagnosticsBuilder;
		}

		private static bool CheckConstraintsSingleType(TypeSymbol type, CheckConstraintsDiagnosticsBuilders diagnostics)
		{
			if (type.Kind == SymbolKind.NamedType)
			{
				CheckConstraints((NamedTypeSymbol)type, diagnostics.diagnosticsBuilder, ref diagnostics.useSiteDiagnosticsBuilder, diagnostics.template);
			}
			return false;
		}

		public static void CheckConstraints(this TupleTypeSymbol tuple, SyntaxNode syntaxNode, ImmutableArray<Location> elementLocations, BindingDiagnosticBag diagnostics, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			NamedTypeSymbol tupleUnderlyingType = tuple.TupleUnderlyingType;
			if (!RequiresChecking(tupleUnderlyingType) || syntaxNode.HasErrors)
			{
				return;
			}
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			ArrayBuilder<NamedTypeSymbol> instance2 = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			TupleTypeSymbol.GetUnderlyingTypeChain(tupleUnderlyingType, instance2);
			int num = 0;
			ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator = instance2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
				CheckTypeConstraints(current, instance, ref useSiteDiagnosticsBuilder, template);
				if (useSiteDiagnosticsBuilder != null)
				{
					instance.AddRange(useSiteDiagnosticsBuilder);
				}
				ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator2 = instance.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeParameterDiagnosticInfo current2 = enumerator2.Current;
					int ordinal = current2.TypeParameter.Ordinal;
					Location location = ((ordinal == 7) ? syntaxNode.Location : elementLocations[ordinal + num]);
					diagnostics.Add(current2.UseSiteInfo, location);
				}
				instance.Clear();
				num += 7;
			}
			instance2.Free();
			instance.Free();
		}

		public static bool CheckConstraintsForNonTuple(this NamedTypeSymbol type, SeparatedSyntaxList<TypeSyntax> typeArgumentsSyntax, BindingDiagnosticBag diagnostics, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			if (!RequiresChecking(type))
			{
				return true;
			}
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
			bool result = CheckTypeConstraints(type, instance, ref useSiteDiagnosticsBuilder, template);
			if (useSiteDiagnosticsBuilder != null)
			{
				instance.AddRange(useSiteDiagnosticsBuilder);
			}
			ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterDiagnosticInfo current = enumerator.Current;
				int ordinal = current.TypeParameter.Ordinal;
				Location location = typeArgumentsSyntax[ordinal].GetLocation();
				diagnostics.Add(current.UseSiteInfo, location);
			}
			instance.Free();
			return result;
		}

		public static bool CheckConstraints(this NamedTypeSymbol type, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			type = (NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(type);
			if (!RequiresChecking(type))
			{
				return true;
			}
			return CheckTypeConstraints(type, diagnosticsBuilder, ref useSiteDiagnosticsBuilder, template);
		}

		public static bool CheckConstraints(this MethodSymbol method, Location diagnosticLocation, BindingDiagnosticBag diagnostics, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			if (!RequiresChecking(method))
			{
				return true;
			}
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
			bool result = CheckMethodConstraints(method, instance, ref useSiteDiagnosticsBuilder, template);
			if (useSiteDiagnosticsBuilder != null)
			{
				instance.AddRange(useSiteDiagnosticsBuilder);
			}
			ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				diagnostics.Add(enumerator.Current.UseSiteInfo, diagnosticLocation);
			}
			instance.Free();
			return result;
		}

		public static bool CheckConstraints(this MethodSymbol method, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			if (!RequiresChecking(method))
			{
				return true;
			}
			return CheckMethodConstraints(method, diagnosticsBuilder, ref useSiteDiagnosticsBuilder, template);
		}

		private static bool CheckTypeConstraints(NamedTypeSymbol type, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			TypeSubstitution typeSubstitution = type.TypeSubstitution;
			return CheckConstraints(type, typeSubstitution, type.OriginalDefinition.TypeParameters, type.TypeArgumentsNoUseSiteDiagnostics, diagnosticsBuilder, ref useSiteDiagnosticsBuilder, template);
		}

		private static bool CheckMethodConstraints(MethodSymbol method, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			TypeSubstitution typeSubstitution = ((SubstitutedMethodSymbol)method).TypeSubstitution;
			return CheckConstraints(method, typeSubstitution, method.OriginalDefinition.TypeParameters, method.TypeArguments, diagnosticsBuilder, ref useSiteDiagnosticsBuilder, template);
		}

		public static bool CheckConstraints(this Symbol constructedSymbol, TypeSubstitution substitution, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeSymbol> typeArguments, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder, CompoundUseSiteInfo<AssemblySymbol> template)
		{
			int length = typeParameters.Length;
			bool result = true;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeArgument = typeArguments[i];
				TypeParameterSymbol typeParameter = typeParameters[i];
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(template);
				if (!CheckConstraints(constructedSymbol, substitution, typeParameter, typeArgument, diagnosticsBuilder, ref useSiteInfo))
				{
					result = false;
				}
				if (AppendUseSiteInfo(useSiteInfo, typeParameter, ref useSiteDiagnosticsBuilder))
				{
					result = false;
				}
			}
			return result;
		}

		public static bool CheckConstraints(Symbol constructedSymbol, TypeSubstitution substitution, TypeParameterSymbol typeParameter, TypeSymbol typeArgument, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsErrorType(typeArgument))
			{
				return true;
			}
			bool result = true;
			if (TypeSymbolExtensions.IsRestrictedType(typeArgument))
			{
				diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_RestrictedType1, typeArgument)));
				result = false;
			}
			if (typeParameter.HasConstructorConstraint && !SatisfiesConstructorConstraint(typeParameter, typeArgument, diagnosticsBuilder))
			{
				result = false;
			}
			if (typeParameter.HasReferenceTypeConstraint && !SatisfiesReferenceTypeConstraint(typeParameter, typeArgument, diagnosticsBuilder))
			{
				result = false;
			}
			if (typeParameter.HasValueTypeConstraint && !SatisfiesValueTypeConstraint(constructedSymbol, typeParameter, typeArgument, diagnosticsBuilder, ref useSiteInfo))
			{
				result = false;
			}
			ImmutableArray<TypeSymbol>.Enumerator enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol type = enumerator.Current.InternalSubstituteTypeParameters(substitution).Type;
				if (!SatisfiesTypeConstraint(typeArgument, type, ref useSiteInfo))
				{
					diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_GenericConstraintNotSatisfied2, typeArgument, type)));
					result = false;
				}
			}
			return result;
		}

		private static bool AppendUseSiteInfo(CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeParameterSymbol typeParameter, [In][Out] ref ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder)
		{
			bool flag = useSiteInfo.AccumulatesDiagnostics && !useSiteInfo.Diagnostics.IsNullOrEmpty();
			if (!flag && (!useSiteInfo.AccumulatesDependencies || useSiteInfo.Dependencies.IsNullOrEmpty()))
			{
				return false;
			}
			if (useSiteDiagnosticsBuilder == null)
			{
				useSiteDiagnosticsBuilder = new ArrayBuilder<TypeParameterDiagnosticInfo>();
			}
			if (!flag)
			{
				if (useSiteInfo.AccumulatesDependencies && !useSiteInfo.Dependencies.IsNullOrEmpty())
				{
					useSiteDiagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, (useSiteInfo.Dependencies!.Count == 1) ? new UseSiteInfo<AssemblySymbol>(useSiteInfo.Dependencies.Single()) : new UseSiteInfo<AssemblySymbol>(useSiteInfo.Dependencies.ToImmutableHashSet())));
				}
				return false;
			}
			foreach (DiagnosticInfo item in useSiteInfo.Diagnostics!)
			{
				useSiteDiagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, item));
			}
			return true;
		}

		public static TypeSymbol GetNonInterfaceConstraint(this TypeParameterSymbol typeParameter, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = null;
			ImmutableArray<TypeSymbol>.Enumerator enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol current = enumerator.Current;
				TypeSymbol typeSymbol2 = null;
				SymbolKind kind = current.Kind;
				if (kind != SymbolKind.ErrorType)
				{
					if (kind == SymbolKind.TypeParameter)
					{
						typeSymbol2 = GetNonInterfaceConstraint((TypeParameterSymbol)current, ref useSiteInfo);
					}
					else if (!TypeSymbolExtensions.IsInterfaceType(current))
					{
						typeSymbol2 = current;
					}
					if ((object)typeSymbol == null)
					{
						typeSymbol = typeSymbol2;
					}
					else if ((object)typeSymbol2 != null && TypeSymbolExtensions.IsClassType(typeSymbol) && Conversions.IsDerivedFrom(typeSymbol2, typeSymbol, ref useSiteInfo))
					{
						typeSymbol = typeSymbol2;
					}
				}
			}
			return typeSymbol;
		}

		public static NamedTypeSymbol GetClassConstraint(this TypeParameterSymbol typeParameter, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol nonInterfaceConstraint = GetNonInterfaceConstraint(typeParameter, ref useSiteInfo);
			if ((object)nonInterfaceConstraint == null)
			{
				return null;
			}
			TypeKind typeKind = nonInterfaceConstraint.TypeKind;
			if (typeKind == TypeKind.Array || typeKind == TypeKind.Enum || typeKind == TypeKind.Struct)
			{
				return nonInterfaceConstraint.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
			}
			return (NamedTypeSymbol)nonInterfaceConstraint;
		}

		private static void GetAllConstraints(this TypeParameterSymbol typeParameter, ArrayBuilder<TypeParameterAndConstraint> constraintsBuilder, TypeParameterConstraint? fromConstraintOpt)
		{
			ArrayBuilder<TypeParameterConstraint> instance = ArrayBuilder<TypeParameterConstraint>.GetInstance();
			typeParameter.GetConstraints(instance);
			ArrayBuilder<TypeParameterConstraint>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterConstraint current = enumerator.Current;
				TypeSymbol typeConstraint = current.TypeConstraint;
				if ((object)typeConstraint != null)
				{
					switch (typeConstraint.TypeKind)
					{
					case TypeKind.TypeParameter:
						GetAllConstraints((TypeParameterSymbol)typeConstraint, constraintsBuilder, fromConstraintOpt.HasValue ? fromConstraintOpt.Value : current);
						continue;
					case TypeKind.Error:
						continue;
					}
				}
				constraintsBuilder.Add(fromConstraintOpt.HasValue ? new TypeParameterAndConstraint((TypeParameterSymbol)fromConstraintOpt.Value.TypeConstraint, current.AtLocation(fromConstraintOpt.Value.LocationOpt)) : new TypeParameterAndConstraint(typeParameter, current));
			}
			instance.Free();
		}

		private static bool SatisfiesTypeConstraint(TypeSymbol typeArgument, TypeSymbol constraintType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsErrorType(constraintType))
			{
				TypeSymbolExtensions.AddUseSiteInfo(constraintType, ref useSiteInfo);
				return false;
			}
			return Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(typeArgument, constraintType, ref useSiteInfo);
		}

		private static bool SatisfiesConstructorConstraint(TypeParameterSymbol typeParameter, TypeSymbol typeArgument, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder)
		{
			switch (typeArgument.TypeKind)
			{
			case TypeKind.Enum:
			case TypeKind.Struct:
				return true;
			case TypeKind.TypeParameter:
				if (((TypeParameterSymbol)typeArgument).HasConstructorConstraint || typeArgument.IsValueType)
				{
					return true;
				}
				diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadGenericParamForNewConstraint2, typeArgument, typeParameter)));
				return false;
			default:
				if (typeArgument.TypeKind == TypeKind.Class)
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)typeArgument;
					if (HasPublicParameterlessConstructor(namedTypeSymbol))
					{
						if (namedTypeSymbol.IsMustInherit)
						{
							diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_MustInheritForNewConstraint2, typeArgument, typeParameter)));
							return false;
						}
						return true;
					}
				}
				diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_NoSuitableNewForNewConstraint2, typeArgument, typeParameter)));
				return false;
			}
		}

		private static bool SatisfiesReferenceTypeConstraint(TypeParameterSymbol typeParameter, TypeSymbol typeArgument, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder)
		{
			if (!typeArgument.IsReferenceType)
			{
				diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForRefConstraint2, typeArgument, typeParameter)));
				return false;
			}
			return true;
		}

		private static bool SatisfiesValueTypeConstraint(Symbol constructedSymbol, TypeParameterSymbol typeParameter, TypeSymbol typeArgument, ArrayBuilder<TypeParameterDiagnosticInfo> diagnosticsBuilder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!typeArgument.IsValueType)
			{
				if (diagnosticsBuilder != null)
				{
					if (constructedSymbol is TypeSymbol @this && TypeSymbolExtensions.IsNullableType(@this))
					{
						diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForStructConstraintNull, typeArgument)));
					}
					else
					{
						diagnosticsBuilder.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_BadTypeArgForStructConstraint2, typeArgument, typeParameter)));
					}
				}
				return false;
			}
			if (IsNullableTypeOrTypeParameter(typeArgument, ref useSiteInfo))
			{
				diagnosticsBuilder?.Add(new TypeParameterDiagnosticInfo(typeParameter, ErrorFactory.ErrorInfo(ERRID.ERR_NullableDisallowedForStructConstr1, typeParameter)));
				return false;
			}
			return true;
		}

		private static bool HasConflict(TypeParameterConstraint constraint1, TypeParameterConstraint constraint2, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeConstraint = constraint1.TypeConstraint;
			TypeSymbol typeConstraint2 = constraint2.TypeConstraint;
			if ((object)typeConstraint != null && TypeSymbolExtensions.IsInterfaceType(typeConstraint))
			{
				return false;
			}
			if ((object)typeConstraint2 != null && TypeSymbolExtensions.IsInterfaceType(typeConstraint2))
			{
				return false;
			}
			if (constraint1.IsValueTypeConstraint)
			{
				if (HasValueTypeConstraintConflict(constraint2, ref useSiteInfo))
				{
					return true;
				}
			}
			else if (constraint2.IsValueTypeConstraint && HasValueTypeConstraintConflict(constraint1, ref useSiteInfo))
			{
				return true;
			}
			if (constraint1.IsReferenceTypeConstraint)
			{
				if (HasReferenceTypeConstraintConflict(constraint2))
				{
					return true;
				}
			}
			else if (constraint2.IsReferenceTypeConstraint && HasReferenceTypeConstraintConflict(constraint1))
			{
				return true;
			}
			if ((object)typeConstraint != null && (object)typeConstraint2 != null && !SatisfiesTypeConstraint(typeConstraint, typeConstraint2, ref useSiteInfo) && !SatisfiesTypeConstraint(typeConstraint2, typeConstraint, ref useSiteInfo))
			{
				return true;
			}
			return false;
		}

		private static bool HasValueTypeConstraintConflict(TypeParameterConstraint constraint, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeConstraint = constraint.TypeConstraint;
			if ((object)typeConstraint == null)
			{
				return false;
			}
			if (SatisfiesValueTypeConstraint(null, null, typeConstraint, null, ref useSiteInfo))
			{
				return false;
			}
			SpecialType specialType = typeConstraint.SpecialType;
			if (specialType == SpecialType.System_Object || specialType == SpecialType.System_ValueType)
			{
				return false;
			}
			return true;
		}

		private static bool HasReferenceTypeConstraintConflict(TypeParameterConstraint constraint)
		{
			TypeSymbol typeConstraint = constraint.TypeConstraint;
			if ((object)typeConstraint == null)
			{
				return false;
			}
			if (SatisfiesReferenceTypeConstraint(null, typeConstraint, null))
			{
				return false;
			}
			return true;
		}

		private static bool IsNullableTypeOrTypeParameter(TypeSymbol type, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (type.TypeKind == TypeKind.TypeParameter)
			{
				ImmutableArray<TypeSymbol>.Enumerator enumerator = ((TypeParameterSymbol)type).ConstraintTypesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (IsNullableTypeOrTypeParameter(enumerator.Current, ref useSiteInfo))
					{
						return true;
					}
				}
				return false;
			}
			return TypeSymbolExtensions.IsNullableType(type);
		}

		private static CompoundDiagnosticInfo GetConstraintCycleInfo(ConsList<TypeParameterSymbol> cycle)
		{
			TypeParameterSymbol typeParameterSymbol = null;
			ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
			instance.Add(null);
			foreach (TypeParameterSymbol item in cycle)
			{
				if ((object)typeParameterSymbol != null)
				{
					instance.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintCycleLink2, item, typeParameterSymbol));
				}
				typeParameterSymbol = item;
			}
			instance[0] = ErrorFactory.ErrorInfo(ERRID.ERR_ConstraintCycleLink2, cycle.Head, typeParameterSymbol);
			DiagnosticInfo[] array = instance.ToArrayAndFree();
			Array.Reverse((Array)array);
			return new CompoundDiagnosticInfo(array);
		}

		public static bool HasPublicParameterlessConstructor(NamedTypeSymbol type)
		{
			type = type.OriginalDefinition;
			if (type is SourceNamedTypeSymbol sourceNamedTypeSymbol && !sourceNamedTypeSymbol.MembersHaveBeenCreated)
			{
				return sourceNamedTypeSymbol.InferFromSyntaxIfClassWillHavePublicParameterlessConstructor();
			}
			ImmutableArray<MethodSymbol>.Enumerator enumerator = type.InstanceConstructors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (current.ParameterCount == 0)
				{
					return current.DeclaredAccessibility == Accessibility.Public;
				}
			}
			return false;
		}

		private static bool ContainsTypeConstraint(ArrayBuilder<TypeParameterConstraint> constraints, TypeSymbol constraintType)
		{
			ArrayBuilder<TypeParameterConstraint>.Enumerator enumerator = constraints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol typeConstraint = enumerator.Current.TypeConstraint;
				if ((object)typeConstraint != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(constraintType, typeConstraint))
				{
					return true;
				}
			}
			return false;
		}

		private static bool RequiresChecking(NamedTypeSymbol type)
		{
			if (type.Arity == 0)
			{
				return false;
			}
			if ((object)type.OriginalDefinition == type)
			{
				return false;
			}
			return true;
		}

		private static bool RequiresChecking(MethodSymbol method)
		{
			if (!method.IsGenericMethod)
			{
				return false;
			}
			if ((object)method.OriginalDefinition == method)
			{
				return false;
			}
			return true;
		}
	}
}
