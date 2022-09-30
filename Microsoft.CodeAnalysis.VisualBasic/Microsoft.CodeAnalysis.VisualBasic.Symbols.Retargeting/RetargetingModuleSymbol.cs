using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingModuleSymbol : NonMissingModuleSymbol
	{
		private struct DestinationData
		{
			public AssemblySymbol To;

			public ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> SymbolMap;
		}

		internal class RetargetingSymbolTranslator : VisualBasicSymbolVisitor<RetargetOptions, Symbol>
		{
			private class RetargetedTypeMethodFinder : RetargetingSymbolTranslator
			{
				private RetargetedTypeMethodFinder(RetargetingModuleSymbol retargetingModule)
					: base(retargetingModule)
				{
				}

				public static MethodSymbol Find(RetargetingSymbolTranslator translator, MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
				{
					if (TypeSymbolExtensions.IsErrorType(retargetedType))
					{
						return null;
					}
					if (!method.IsGenericMethod)
					{
						return FindWorker(translator, method, retargetedType, retargetedMethodComparer);
					}
					return FindWorker(new RetargetedTypeMethodFinder(translator._retargetingModule), method, retargetedType, retargetedMethodComparer);
				}

				private static MethodSymbol FindWorker(RetargetingSymbolTranslator translator, MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
				{
					ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(method.Parameters.Length);
					ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
					bool modifiersHaveChanged = default(bool);
					while (enumerator.MoveNext())
					{
						ParameterSymbol current = enumerator.Current;
						instance.Add(new SignatureOnlyParameterSymbol(translator.Retarget(current.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(current.CustomModifiers, ref modifiersHaveChanged), translator.RetargetModifiers(current.RefCustomModifiers, ref modifiersHaveChanged), current.ExplicitDefaultConstantValue, current.IsParamArray, current.IsByRef, current.IsOut, current.IsOptional));
					}
					SignatureOnlyMethodSymbol y = new SignatureOnlyMethodSymbol(method.Name, retargetedType, method.MethodKind, method.CallingConvention, IndexedTypeParameterSymbol.Take(method.Arity), instance.ToImmutableAndFree(), method.ReturnsByRef, translator.Retarget(method.ReturnType, RetargetOptions.RetargetPrimitiveTypesByTypeCode), translator.RetargetModifiers(method.ReturnTypeCustomModifiers, ref modifiersHaveChanged), translator.RetargetModifiers(method.RefCustomModifiers, ref modifiersHaveChanged), ImmutableArray<MethodSymbol>.Empty);
					ImmutableArray<Symbol>.Enumerator enumerator2 = retargetedType.GetMembers(method.Name).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current2 = enumerator2.Current;
						if (current2.Kind == SymbolKind.Method)
						{
							MethodSymbol methodSymbol = (MethodSymbol)current2;
							if (retargetedMethodComparer.Equals(methodSymbol, y))
							{
								return methodSymbol;
							}
						}
					}
					return null;
				}

				public override TypeParameterSymbol Retarget(TypeParameterSymbol typeParameter)
				{
					if ((object)typeParameter.ContainingModule == base.UnderlyingModule)
					{
						return base.Retarget(typeParameter);
					}
					return IndexedTypeParameterSymbol.GetTypeParameter(typeParameter.Ordinal);
				}
			}

			private readonly RetargetingModuleSymbol _retargetingModule;

			private ConcurrentDictionary<Symbol, Symbol> SymbolMap => _retargetingModule._symbolMap;

			private RetargetingAssemblySymbol RetargetingAssembly => _retargetingModule._retargetingAssembly;

			private Dictionary<AssemblySymbol, DestinationData> RetargetingAssemblyMap => _retargetingModule._retargetingAssemblyMap;

			private SourceModuleSymbol UnderlyingModule => _retargetingModule._underlyingModule;

			public RetargetingSymbolTranslator(RetargetingModuleSymbol retargetingModule)
			{
				_retargetingModule = retargetingModule;
			}

			public Symbol Retarget(Symbol symbol)
			{
				return symbol.Accept(this, RetargetOptions.RetargetPrimitiveTypesByName);
			}

			public MarshalPseudoCustomAttributeData Retarget(MarshalPseudoCustomAttributeData marshallingInfo)
			{
				return marshallingInfo?.WithTranslatedTypes((TypeSymbol type, RetargetingSymbolTranslator translator) => translator.Retarget(type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), this);
			}

			public TypeSymbol Retarget(TypeSymbol symbol, RetargetOptions options)
			{
				return (TypeSymbol)symbol.Accept(this, options);
			}

			public NamespaceSymbol Retarget(NamespaceSymbol ns)
			{
				return (NamespaceSymbol)SymbolMap.GetOrAdd(ns, _retargetingModule._createRetargetingNamespace);
			}

			private NamedTypeSymbol RetargetNamedTypeDefinition(NamedTypeSymbol type, RetargetOptions options)
			{
				if (type.IsTupleType)
				{
					NamedTypeSymbol namedTypeSymbol = Retarget(type.TupleUnderlyingType, options);
					if (namedTypeSymbol.IsTupleOrCompatibleWithTupleOfCardinality(type.TupleElementTypes.Length))
					{
						return ((TupleTypeSymbol)type).WithUnderlyingType(namedTypeSymbol);
					}
					return namedTypeSymbol;
				}
				if (options == RetargetOptions.RetargetPrimitiveTypesByTypeCode)
				{
					PrimitiveTypeCode primitiveTypeCode = type.PrimitiveTypeCode;
					if (primitiveTypeCode != PrimitiveTypeCode.NotPrimitive)
					{
						return RetargetingAssembly.GetPrimitiveType(primitiveTypeCode);
					}
				}
				if (type.Kind == SymbolKind.ErrorType)
				{
					return Retarget((ErrorTypeSymbol)type);
				}
				AssemblySymbol containingAssembly = type.ContainingAssembly;
				if (((object)containingAssembly != RetargetingAssembly.UnderlyingAssembly) ? containingAssembly.IsLinked : type.IsExplicitDefinitionOfNoPiaLocalType)
				{
					return RetargetNoPiaLocalType(type);
				}
				if ((object)containingAssembly == RetargetingAssembly.UnderlyingAssembly)
				{
					return RetargetNamedTypeDefinitionFromUnderlyingAssembly(type);
				}
				DestinationData value = default(DestinationData);
				if (!RetargetingAssemblyMap.TryGetValue(containingAssembly, out value))
				{
					return type;
				}
				return PerformTypeRetargeting(ref value, type);
			}

			private NamedTypeSymbol RetargetNamedTypeDefinitionFromUnderlyingAssembly(NamedTypeSymbol type)
			{
				ModuleSymbol containingModule = type.ContainingModule;
				if ((object)containingModule == UnderlyingModule)
				{
					NamedTypeSymbol containingType = type.ContainingType;
					while ((object)containingType != null)
					{
						if (containingType.IsExplicitDefinitionOfNoPiaLocalType)
						{
							return (NamedTypeSymbol)SymbolMap.GetOrAdd(type, new UnsupportedMetadataTypeSymbol());
						}
						containingType = containingType.ContainingType;
					}
					return (NamedTypeSymbol)SymbolMap.GetOrAdd(type, _retargetingModule._createRetargetingNamedType);
				}
				PEModuleSymbol addedModule = (PEModuleSymbol)RetargetingAssembly.Modules[containingModule.Ordinal];
				return RetargetNamedTypeDefinition((PENamedTypeSymbol)type, addedModule);
			}

			private NamedTypeSymbol RetargetNoPiaLocalType(NamedTypeSymbol type)
			{
				NamedTypeSymbol value = null;
				if (RetargetingAssembly.m_NoPiaUnificationMap.TryGetValue(type, out value))
				{
					return value;
				}
				NamedTypeSymbol value2;
				if (type.ContainingSymbol.Kind != SymbolKind.NamedType && type.Arity == 0)
				{
					bool isInterface = type.IsInterface;
					bool flag = false;
					string guidString = null;
					string guidString2 = null;
					if (isInterface)
					{
						flag = type.GetGuidString(ref guidString);
					}
					MetadataTypeName fullEmittedName = MetadataTypeName.FromFullName(type.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), useCLSCompliantNameArityEncoding: false, type.Arity);
					string identifier = null;
					if ((object)type.ContainingModule == _retargetingModule.UnderlyingModule)
					{
						ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = type.GetAttributes().GetEnumerator();
						while (enumerator.MoveNext())
						{
							VisualBasicAttributeData current = enumerator.Current;
							int targetAttributeSignatureIndex = current.GetTargetAttributeSignatureIndex(type, AttributeDescription.TypeIdentifierAttribute);
							if (targetAttributeSignatureIndex != -1)
							{
								if (targetAttributeSignatureIndex == 1 && current.CommonConstructorArguments.Length == 2)
								{
									guidString2 = current.CommonConstructorArguments[0].ValueInternal as string;
									identifier = current.CommonConstructorArguments[1].ValueInternal as string;
								}
								break;
							}
						}
					}
					else if (!flag && !isInterface)
					{
						type.ContainingAssembly.GetGuidString(ref guidString2);
						identifier = fullEmittedName.FullName;
					}
					value2 = MetadataDecoder.SubstituteNoPiaLocalType(ref fullEmittedName, isInterface, type.BaseTypeNoUseSiteDiagnostics, guidString, guidString2, identifier, RetargetingAssembly);
				}
				else
				{
					value2 = new UnsupportedMetadataTypeSymbol();
				}
				return RetargetingAssembly.m_NoPiaUnificationMap.GetOrAdd(type, value2);
			}

			private static NamedTypeSymbol RetargetNamedTypeDefinition(PENamedTypeSymbol type, PEModuleSymbol addedModule)
			{
				TypeSymbol value = null;
				if (addedModule.TypeHandleToTypeMap.TryGetValue(type.Handle, out value))
				{
					return (NamedTypeSymbol)value;
				}
				NamedTypeSymbol containingType = type.ContainingType;
				NamedTypeSymbol result;
				if ((object)containingType != null)
				{
					NamedTypeSymbol namedTypeSymbol = RetargetNamedTypeDefinition((PENamedTypeSymbol)containingType, addedModule);
					MetadataTypeName emittedTypeName = MetadataTypeName.FromTypeName(type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
					result = namedTypeSymbol.LookupMetadataType(ref emittedTypeName);
				}
				else
				{
					MetadataTypeName emittedTypeName = MetadataTypeName.FromNamespaceAndTypeName(type.GetEmittedNamespaceName() ?? type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
					result = addedModule.LookupTopLevelMetadataType(ref emittedTypeName);
				}
				return result;
			}

			private static NamedTypeSymbol PerformTypeRetargeting(ref DestinationData destination, NamedTypeSymbol type)
			{
				NamedTypeSymbol value = null;
				if (!destination.SymbolMap.TryGetValue(type, out value))
				{
					NamedTypeSymbol containingType = type.ContainingType;
					NamedTypeSymbol value2;
					if ((object)containingType != null)
					{
						NamedTypeSymbol namedTypeSymbol = PerformTypeRetargeting(ref destination, containingType);
						MetadataTypeName emittedTypeName = MetadataTypeName.FromTypeName(type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
						value2 = namedTypeSymbol.LookupMetadataType(ref emittedTypeName);
					}
					else
					{
						MetadataTypeName emittedTypeName = MetadataTypeName.FromNamespaceAndTypeName(type.GetEmittedNamespaceName() ?? type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), type.MetadataName, useCLSCompliantNameArityEncoding: false, type.Arity);
						value2 = destination.To.LookupTopLevelMetadataType(ref emittedTypeName, digThroughForwardedTypes: true);
					}
					value = destination.SymbolMap.GetOrAdd(type, value2);
				}
				return value;
			}

			public NamedTypeSymbol Retarget(NamedTypeSymbol type, RetargetOptions options)
			{
				NamedTypeSymbol originalDefinition = type.OriginalDefinition;
				NamedTypeSymbol namedTypeSymbol = RetargetNamedTypeDefinition(originalDefinition, options);
				if ((object)type == originalDefinition)
				{
					return namedTypeSymbol;
				}
				if (namedTypeSymbol.Kind == SymbolKind.ErrorType && !namedTypeSymbol.IsGenericType)
				{
					return namedTypeSymbol;
				}
				if (type.IsUnboundGenericType)
				{
					if ((object)namedTypeSymbol == originalDefinition)
					{
						return type;
					}
					return NamedTypeSymbolExtensions.AsUnboundGenericType(namedTypeSymbol);
				}
				NamedTypeSymbol namedTypeSymbol2 = type;
				ArrayBuilder<TypeWithModifiers> instance = ArrayBuilder<TypeWithModifiers>.GetInstance();
				int num = int.MaxValue;
				while ((object)namedTypeSymbol2 != null)
				{
					if (num == int.MaxValue && !namedTypeSymbol2.IsInterface)
					{
						num = instance.Count;
					}
					int arity = namedTypeSymbol2.Arity;
					if (arity > 0)
					{
						ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = namedTypeSymbol2.TypeArgumentsNoUseSiteDiagnostics;
						if (namedTypeSymbol2.HasTypeArgumentsCustomModifiers)
						{
							int num2 = arity - 1;
							for (int i = 0; i <= num2; i++)
							{
								instance.Add(new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics[i], namedTypeSymbol2.GetTypeArgumentCustomModifiers(i)));
							}
						}
						else
						{
							int num3 = arity - 1;
							for (int j = 0; j <= num3; j++)
							{
								instance.Add(new TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics[j]));
							}
						}
					}
					namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
				}
				bool flag = !originalDefinition.Equals(namedTypeSymbol);
				ArrayBuilder<TypeWithModifiers> instance2 = ArrayBuilder<TypeWithModifiers>.GetInstance(instance.Count);
				ArrayBuilder<TypeWithModifiers>.Enumerator enumerator = instance.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeWithModifiers current = enumerator.Current;
					bool modifiersHaveChanged = false;
					TypeWithModifiers item = new TypeWithModifiers((TypeSymbol)current.Type.Accept(this, RetargetOptions.RetargetPrimitiveTypesByTypeCode), RetargetModifiers(current.CustomModifiers, ref modifiersHaveChanged));
					if (!flag && (modifiersHaveChanged || !TypeSymbol.Equals(item.Type, current.Type, TypeCompareKind.ConsiderEverything)))
					{
						flag = true;
					}
					instance2.Add(item);
				}
				bool flag2 = IsNoPiaIllegalGenericInstantiation(instance, instance2, num);
				instance.Free();
				NamedTypeSymbol namedTypeSymbol3;
				if (!flag)
				{
					namedTypeSymbol3 = type;
				}
				else
				{
					namedTypeSymbol2 = namedTypeSymbol;
					ArrayBuilder<TypeParameterSymbol> instance3 = ArrayBuilder<TypeParameterSymbol>.GetInstance(instance2.Count);
					while ((object)namedTypeSymbol2 != null)
					{
						if (namedTypeSymbol2.Arity > 0)
						{
							instance3.AddRange(namedTypeSymbol2.TypeParameters);
						}
						namedTypeSymbol2 = namedTypeSymbol2.ContainingType;
					}
					instance3.ReverseContents();
					instance2.ReverseContents();
					TypeSubstitution substitution = TypeSubstitution.Create(namedTypeSymbol, instance3.ToImmutableAndFree(), instance2.ToImmutable());
					namedTypeSymbol3 = namedTypeSymbol.Construct(substitution);
				}
				instance2.Free();
				if (flag2)
				{
					namedTypeSymbol3 = new NoPiaIllegalGenericInstantiationSymbol(namedTypeSymbol3);
				}
				return namedTypeSymbol3;
			}

			private bool IsNoPiaIllegalGenericInstantiation(ArrayBuilder<TypeWithModifiers> oldArguments, ArrayBuilder<TypeWithModifiers> newArguments, int startOfNonInterfaceArguments)
			{
				if (UnderlyingModule.ContainsExplicitDefinitionOfNoPiaLocalTypes)
				{
					int num = oldArguments.Count - 1;
					for (int i = startOfNonInterfaceArguments; i <= num; i++)
					{
						if (IsOrClosedOverAnExplicitLocalType(oldArguments[i].Type))
						{
							return true;
						}
					}
				}
				ImmutableArray<AssemblySymbol> assembliesToEmbedTypesFrom = UnderlyingModule.GetAssembliesToEmbedTypesFrom();
				if (assembliesToEmbedTypesFrom.Length > 0)
				{
					int num2 = oldArguments.Count - 1;
					for (int j = startOfNonInterfaceArguments; j <= num2; j++)
					{
						if (MetadataDecoder.IsOrClosedOverATypeFromAssemblies(oldArguments[j].Type, assembliesToEmbedTypesFrom))
						{
							return true;
						}
					}
				}
				ImmutableArray<AssemblySymbol> linkedReferencedAssemblies = RetargetingAssembly.GetLinkedReferencedAssemblies();
				if (!linkedReferencedAssemblies.IsDefaultOrEmpty)
				{
					int num3 = newArguments.Count - 1;
					for (int k = startOfNonInterfaceArguments; k <= num3; k++)
					{
						if (MetadataDecoder.IsOrClosedOverATypeFromAssemblies(newArguments[k].Type, linkedReferencedAssemblies))
						{
							return true;
						}
					}
				}
				return false;
			}

			private bool IsOrClosedOverAnExplicitLocalType(TypeSymbol symbol)
			{
				switch (symbol.Kind)
				{
				case SymbolKind.TypeParameter:
					return false;
				case SymbolKind.ArrayType:
					return IsOrClosedOverAnExplicitLocalType(((ArrayTypeSymbol)symbol).ElementType);
				case SymbolKind.ErrorType:
				case SymbolKind.NamedType:
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)symbol;
					if (namedTypeSymbol.IsTupleType)
					{
						namedTypeSymbol = namedTypeSymbol.TupleUnderlyingType;
					}
					if ((object)symbol.OriginalDefinition.ContainingModule == _retargetingModule.UnderlyingModule && namedTypeSymbol.IsExplicitDefinitionOfNoPiaLocalType)
					{
						return true;
					}
					do
					{
						ImmutableArray<TypeSymbol>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
						while (enumerator.MoveNext())
						{
							TypeSymbol current = enumerator.Current;
							if (IsOrClosedOverAnExplicitLocalType(current))
							{
								return true;
							}
						}
						namedTypeSymbol = namedTypeSymbol.ContainingType;
					}
					while ((object)namedTypeSymbol != null);
					return false;
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
				}
			}

			public virtual TypeParameterSymbol Retarget(TypeParameterSymbol typeParameter)
			{
				return (TypeParameterSymbol)SymbolMap.GetOrAdd(typeParameter, _retargetingModule._createRetargetingTypeParameter);
			}

			public ArrayTypeSymbol Retarget(ArrayTypeSymbol type)
			{
				TypeSymbol elementType = type.ElementType;
				TypeSymbol typeSymbol = Retarget(elementType, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
				bool modifiersHaveChanged = false;
				ImmutableArray<CustomModifier> customModifiers = RetargetModifiers(type.CustomModifiers, ref modifiersHaveChanged);
				if (!modifiersHaveChanged && elementType.Equals(typeSymbol))
				{
					return type;
				}
				if (type.IsSZArray)
				{
					return ArrayTypeSymbol.CreateSZArray(typeSymbol, customModifiers, RetargetingAssembly);
				}
				return ArrayTypeSymbol.CreateMDArray(typeSymbol, customModifiers, type.Rank, type.Sizes, type.LowerBounds, RetargetingAssembly);
			}

			internal ImmutableArray<CustomModifier> RetargetModifiers(ImmutableArray<CustomModifier> oldModifiers, ref bool modifiersHaveChanged)
			{
				ArrayBuilder<CustomModifier> arrayBuilder = null;
				int num = oldModifiers.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					NamedTypeSymbol namedTypeSymbol = Retarget((NamedTypeSymbol)oldModifiers[i].Modifier, RetargetOptions.RetargetPrimitiveTypesByName);
					if (!namedTypeSymbol.Equals(oldModifiers[i].Modifier))
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<CustomModifier>.GetInstance(oldModifiers.Length);
							arrayBuilder.AddRange(oldModifiers, i);
						}
						arrayBuilder.Add(oldModifiers[i].IsOptional ? VisualBasicCustomModifier.CreateOptional(namedTypeSymbol) : VisualBasicCustomModifier.CreateRequired(namedTypeSymbol));
					}
					else
					{
						arrayBuilder?.Add(oldModifiers[i]);
					}
				}
				modifiersHaveChanged = arrayBuilder != null;
				if (!modifiersHaveChanged)
				{
					return oldModifiers;
				}
				return arrayBuilder.ToImmutableAndFree();
			}

			internal ImmutableArray<CustomModifier> RetargetModifiers(ImmutableArray<CustomModifier> oldModifiers, ref ImmutableArray<CustomModifier> lazyCustomModifiers)
			{
				if (lazyCustomModifiers.IsDefault)
				{
					bool modifiersHaveChanged = default(bool);
					ImmutableArray<CustomModifier> value = RetargetModifiers(oldModifiers, ref modifiersHaveChanged);
					ImmutableInterlocked.InterlockedCompareExchange(ref lazyCustomModifiers, value, default(ImmutableArray<CustomModifier>));
				}
				return lazyCustomModifiers;
			}

			internal CustomModifiersTuple RetargetModifiers(ImmutableArray<CustomModifier> oldTypeModifiers, ImmutableArray<CustomModifier> oldRefModifiers, ref CustomModifiersTuple lazyCustomModifiers)
			{
				if (lazyCustomModifiers == null)
				{
					bool modifiersHaveChanged = default(bool);
					ImmutableArray<CustomModifier> typeCustomModifiers = RetargetModifiers(oldTypeModifiers, ref modifiersHaveChanged);
					ImmutableArray<CustomModifier> refCustomModifiers = RetargetModifiers(oldRefModifiers, ref modifiersHaveChanged);
					Interlocked.CompareExchange(ref lazyCustomModifiers, CustomModifiersTuple.Create(typeCustomModifiers, refCustomModifiers), null);
				}
				return lazyCustomModifiers;
			}

			private ImmutableArray<VisualBasicAttributeData> RetargetAttributes(ImmutableArray<VisualBasicAttributeData> oldAttributes)
			{
				return oldAttributes.SelectAsArray((VisualBasicAttributeData a, RetargetingSymbolTranslator t) => t.RetargetAttributeData(a), this);
			}

			[IteratorStateMachine(typeof(VB_0024StateMachine_28_RetargetAttributes))]
			internal IEnumerable<VisualBasicAttributeData> RetargetAttributes(IEnumerable<VisualBasicAttributeData> attributes)
			{
				//yield-return decompiler failed: Method not found
				return new VB_0024StateMachine_28_RetargetAttributes(-2)
				{
					_0024VB_0024Me = this,
					_0024P_attributes = attributes
				};
			}

			private VisualBasicAttributeData RetargetAttributeData(VisualBasicAttributeData oldAttribute)
			{
				MethodSymbol attributeConstructor = oldAttribute.AttributeConstructor;
				MethodSymbol methodSymbol = (((object)attributeConstructor == null) ? null : Retarget(attributeConstructor, MethodSignatureComparer.RetargetedExplicitMethodImplementationComparer));
				NamedTypeSymbol attributeClass = oldAttribute.AttributeClass;
				NamedTypeSymbol attributeClass2 = (((object)methodSymbol != null) ? methodSymbol.ContainingType : (((object)attributeClass == null) ? null : Retarget(attributeClass, RetargetOptions.RetargetPrimitiveTypesByTypeCode)));
				ImmutableArray<TypedConstant> commonConstructorArguments = oldAttribute.CommonConstructorArguments;
				ImmutableArray<TypedConstant> constructorArguments = RetargetAttributeConstructorArguments(commonConstructorArguments);
				ImmutableArray<KeyValuePair<string, TypedConstant>> commonNamedArguments = oldAttribute.CommonNamedArguments;
				ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = RetargetAttributeNamedArguments(commonNamedArguments);
				return new RetargetingAttributeData(oldAttribute.ApplicationSyntaxReference, attributeClass2, methodSymbol, constructorArguments, namedArguments, oldAttribute.IsConditionallyOmitted, oldAttribute.HasErrors);
			}

			private ImmutableArray<TypedConstant> RetargetAttributeConstructorArguments(ImmutableArray<TypedConstant> constructorArguments)
			{
				ImmutableArray<TypedConstant> result = constructorArguments;
				bool typedConstantChanged = false;
				if (!constructorArguments.IsDefault && constructorArguments.Any())
				{
					ArrayBuilder<TypedConstant> instance = ArrayBuilder<TypedConstant>.GetInstance(constructorArguments.Length);
					ImmutableArray<TypedConstant>.Enumerator enumerator = constructorArguments.GetEnumerator();
					while (enumerator.MoveNext())
					{
						TypedConstant current = enumerator.Current;
						TypedConstant item = RetargetTypedConstant(current, ref typedConstantChanged);
						instance.Add(item);
					}
					if (typedConstantChanged)
					{
						result = instance.ToImmutable();
					}
					instance.Free();
				}
				return result;
			}

			private TypedConstant RetargetTypedConstant(TypedConstant oldConstant, ref bool typedConstantChanged)
			{
				TypeSymbol typeSymbol = (TypeSymbol)oldConstant.TypeInternal;
				TypeSymbol typeSymbol2 = (((object)typeSymbol == null) ? null : Retarget(typeSymbol, RetargetOptions.RetargetPrimitiveTypesByTypeCode));
				TypedConstant result;
				if (oldConstant.Kind == TypedConstantKind.Array)
				{
					ImmutableArray<TypedConstant> immutableArray = RetargetAttributeConstructorArguments(oldConstant.Values);
					if ((object)typeSymbol2 == typeSymbol && !(immutableArray != oldConstant.Values))
					{
						return oldConstant;
					}
					typedConstantChanged = true;
					result = new TypedConstant(typeSymbol2, immutableArray);
				}
				else
				{
					object objectValue = RuntimeHelpers.GetObjectValue(oldConstant.ValueInternal);
					object obj = ((oldConstant.Kind != TypedConstantKind.Type || objectValue == null) ? RuntimeHelpers.GetObjectValue(objectValue) : Retarget((TypeSymbol)objectValue, RetargetOptions.RetargetPrimitiveTypesByTypeCode));
					if ((object)typeSymbol2 == typeSymbol && obj == objectValue)
					{
						return oldConstant;
					}
					typedConstantChanged = true;
					result = new TypedConstant(typeSymbol2, oldConstant.Kind, RuntimeHelpers.GetObjectValue(obj));
				}
				return result;
			}

			private ImmutableArray<KeyValuePair<string, TypedConstant>> RetargetAttributeNamedArguments(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
			{
				ImmutableArray<KeyValuePair<string, TypedConstant>> result = namedArguments;
				bool flag = false;
				if (namedArguments.Any())
				{
					ArrayBuilder<KeyValuePair<string, TypedConstant>> instance = ArrayBuilder<KeyValuePair<string, TypedConstant>>.GetInstance(namedArguments.Length);
					ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = namedArguments.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, TypedConstant> current = enumerator.Current;
						TypedConstant value = current.Value;
						bool typedConstantChanged = false;
						TypedConstant value2 = RetargetTypedConstant(value, ref typedConstantChanged);
						if (typedConstantChanged)
						{
							instance.Add(new KeyValuePair<string, TypedConstant>(current.Key, value2));
							flag = true;
						}
						else
						{
							instance.Add(current);
						}
					}
					if (flag)
					{
						result = instance.ToImmutable();
					}
					instance.Free();
				}
				return result;
			}

			internal ImmutableArray<VisualBasicAttributeData> GetRetargetedAttributes(Symbol underlyingSymbol, ref ImmutableArray<VisualBasicAttributeData> lazyCustomAttributes, bool getReturnTypeAttributes = false)
			{
				if (lazyCustomAttributes.IsDefault)
				{
					ImmutableArray<VisualBasicAttributeData> oldAttributes;
					if (!getReturnTypeAttributes)
					{
						oldAttributes = underlyingSymbol.GetAttributes();
						if (underlyingSymbol.Kind == SymbolKind.Method)
						{
							((MethodSymbol)underlyingSymbol).GetReturnTypeAttributes();
						}
					}
					else
					{
						oldAttributes = ((MethodSymbol)underlyingSymbol).GetReturnTypeAttributes();
					}
					ImmutableArray<VisualBasicAttributeData> value = RetargetAttributes(oldAttributes);
					ImmutableInterlocked.InterlockedCompareExchange(ref lazyCustomAttributes, value, default(ImmutableArray<VisualBasicAttributeData>));
				}
				return lazyCustomAttributes;
			}

			public ErrorTypeSymbol Retarget(ErrorTypeSymbol type)
			{
				if (type.GetUseSiteInfo().DiagnosticInfo != null)
				{
					return type;
				}
				return new ExtendedErrorTypeSymbol(type.ErrorInfo ?? ErrorFactory.ErrorInfo(ERRID.ERR_InReferencedAssembly, type.ContainingAssembly?.Identity.GetDisplayName() ?? ""), type.Name, type.Arity, type.CandidateSymbols, type.ResultKind, reportErrorWhenReferenced: true);
			}

			public IEnumerable<NamedTypeSymbol> Retarget(IEnumerable<NamedTypeSymbol> sequence)
			{
				return sequence.Select((NamedTypeSymbol s) => Retarget(s, RetargetOptions.RetargetPrimitiveTypesByName));
			}

			public ImmutableArray<Symbol> Retarget(ImmutableArray<Symbol> arr)
			{
				ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(arr.Length);
				ImmutableArray<Symbol>.Enumerator enumerator = arr.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					instance.Add(Retarget(current));
				}
				return instance.ToImmutableAndFree();
			}

			public ImmutableArray<NamedTypeSymbol> Retarget(ImmutableArray<NamedTypeSymbol> sequence)
			{
				ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance(sequence.Length);
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = sequence.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					instance.Add(Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
				}
				return instance.ToImmutableAndFree();
			}

			public ImmutableArray<TypeSymbol> Retarget(ImmutableArray<TypeSymbol> sequence)
			{
				ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(sequence.Length);
				ImmutableArray<TypeSymbol>.Enumerator enumerator = sequence.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeSymbol current = enumerator.Current;
					instance.Add(Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName));
				}
				return instance.ToImmutableAndFree();
			}

			public ImmutableArray<TypeParameterSymbol> Retarget(ImmutableArray<TypeParameterSymbol> list)
			{
				ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance(list.Length);
				ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = list.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeParameterSymbol current = enumerator.Current;
					instance.Add(Retarget(current));
				}
				return instance.ToImmutableAndFree();
			}

			public RetargetingMethodSymbol Retarget(MethodSymbol method)
			{
				return (RetargetingMethodSymbol)SymbolMap.GetOrAdd(method, _retargetingModule._createRetargetingMethod);
			}

			public MethodSymbol Retarget(MethodSymbol method, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
			{
				if ((object)method.ContainingModule == UnderlyingModule && method.IsDefinition)
				{
					return (RetargetingMethodSymbol)SymbolMap.GetOrAdd(method, _retargetingModule._createRetargetingMethod);
				}
				NamedTypeSymbol containingType = method.ContainingType;
				NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
				return ((object)namedTypeSymbol == containingType) ? method : FindMethodInRetargetedType(method, namedTypeSymbol, retargetedMethodComparer);
			}

			private MethodSymbol FindMethodInRetargetedType(MethodSymbol method, NamedTypeSymbol retargetedType, IEqualityComparer<MethodSymbol> retargetedMethodComparer)
			{
				return RetargetedTypeMethodFinder.Find(this, method, retargetedType, retargetedMethodComparer);
			}

			public RetargetingFieldSymbol Retarget(FieldSymbol field)
			{
				return (RetargetingFieldSymbol)SymbolMap.GetOrAdd(field, _retargetingModule._createRetargetingField);
			}

			public RetargetingPropertySymbol Retarget(PropertySymbol property)
			{
				return (RetargetingPropertySymbol)SymbolMap.GetOrAdd(property, _retargetingModule._createRetargetingProperty);
			}

			public RetargetingEventSymbol Retarget(EventSymbol @event)
			{
				return (RetargetingEventSymbol)SymbolMap.GetOrAdd(@event, _retargetingModule._createRetargetingEvent);
			}

			public EventSymbol RetargetImplementedEvent(EventSymbol @event)
			{
				if ((object)@event.ContainingModule == UnderlyingModule && @event.IsDefinition)
				{
					return (RetargetingEventSymbol)SymbolMap.GetOrAdd(@event, _retargetingModule._createRetargetingEvent);
				}
				NamedTypeSymbol containingType = @event.ContainingType;
				NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
				return ((object)namedTypeSymbol == containingType) ? @event : FindEventInRetargetedType(@event, namedTypeSymbol);
			}

			private EventSymbol FindEventInRetargetedType(EventSymbol @event, NamedTypeSymbol retargetedType)
			{
				TypeSymbol right = Retarget(@event.Type, RetargetOptions.RetargetPrimitiveTypesByName);
				ImmutableArray<Symbol>.Enumerator enumerator = retargetedType.GetMembers(@event.Name).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Event)
					{
						EventSymbol eventSymbol = (EventSymbol)current;
						if (TypeSymbol.Equals(eventSymbol.Type, right, TypeCompareKind.ConsiderEverything))
						{
							return eventSymbol;
						}
					}
				}
				return null;
			}

			public PropertySymbol Retarget(PropertySymbol property, IEqualityComparer<PropertySymbol> retargetedPropertyComparer)
			{
				if ((object)property.ContainingModule == UnderlyingModule && property.IsDefinition)
				{
					return (RetargetingPropertySymbol)SymbolMap.GetOrAdd(property, _retargetingModule._createRetargetingProperty);
				}
				NamedTypeSymbol containingType = property.ContainingType;
				NamedTypeSymbol namedTypeSymbol = Retarget(containingType, RetargetOptions.RetargetPrimitiveTypesByName);
				return ((object)namedTypeSymbol == containingType) ? property : FindPropertyInRetargetedType(property, namedTypeSymbol, retargetedPropertyComparer);
			}

			private PropertySymbol FindPropertyInRetargetedType(PropertySymbol property, NamedTypeSymbol retargetedType, IEqualityComparer<PropertySymbol> retargetedPropertyComparer)
			{
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = property.Parameters.GetEnumerator();
				bool modifiersHaveChanged = default(bool);
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					instance.Add(new SignatureOnlyParameterSymbol(Retarget(current.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), RetargetModifiers(current.CustomModifiers, ref modifiersHaveChanged), RetargetModifiers(current.RefCustomModifiers, ref modifiersHaveChanged), current.HasExplicitDefaultValue ? current.ExplicitDefaultConstantValue : null, current.IsParamArray, current.IsByRef, current.IsOut, current.IsOptional));
				}
				SignatureOnlyPropertySymbol y = new SignatureOnlyPropertySymbol(property.Name, retargetedType, property.IsReadOnly, property.IsWriteOnly, instance.ToImmutableAndFree(), property.ReturnsByRef, Retarget(property.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode), RetargetModifiers(property.TypeCustomModifiers, ref modifiersHaveChanged), RetargetModifiers(property.RefCustomModifiers, ref modifiersHaveChanged));
				ImmutableArray<Symbol>.Enumerator enumerator2 = retargetedType.GetMembers(property.Name).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Symbol current2 = enumerator2.Current;
					if (current2.Kind == SymbolKind.Property)
					{
						PropertySymbol propertySymbol = (PropertySymbol)current2;
						if (retargetedPropertyComparer.Equals(propertySymbol, y))
						{
							return propertySymbol;
						}
					}
				}
				return null;
			}

			public override Symbol VisitModule(ModuleSymbol symbol, RetargetOptions options)
			{
				return _retargetingModule;
			}

			public override Symbol VisitNamespace(NamespaceSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitNamedType(NamedTypeSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol, options);
			}

			public override Symbol VisitArrayType(ArrayTypeSymbol symbol, RetargetOptions arg)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitMethod(MethodSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitField(FieldSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitProperty(PropertySymbol symbol, RetargetOptions arg)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitEvent(EventSymbol symbol, RetargetOptions arg)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitTypeParameter(TypeParameterSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol);
			}

			public override Symbol VisitErrorType(ErrorTypeSymbol symbol, RetargetOptions options)
			{
				return Retarget(symbol);
			}
		}

		private readonly RetargetingAssemblySymbol _retargetingAssembly;

		private readonly SourceModuleSymbol _underlyingModule;

		private readonly Dictionary<AssemblySymbol, DestinationData> _retargetingAssemblyMap;

		internal readonly RetargetingSymbolTranslator RetargetingTranslator;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private readonly ConcurrentDictionary<Symbol, Symbol> _symbolMap;

		private readonly Func<Symbol, RetargetingMethodSymbol> _createRetargetingMethod;

		private readonly Func<Symbol, RetargetingNamespaceSymbol> _createRetargetingNamespace;

		private readonly Func<Symbol, RetargetingTypeParameterSymbol> _createRetargetingTypeParameter;

		private readonly Func<Symbol, RetargetingNamedTypeSymbol> _createRetargetingNamedType;

		private readonly Func<Symbol, RetargetingFieldSymbol> _createRetargetingField;

		private readonly Func<Symbol, RetargetingPropertySymbol> _createRetargetingProperty;

		private readonly Func<Symbol, RetargetingEventSymbol> _createRetargetingEvent;

		internal override int Ordinal => 0;

		internal override Machine Machine => _underlyingModule.Machine;

		internal override bool Bit32Required => _underlyingModule.Bit32Required;

		public SourceModuleSymbol UnderlyingModule => _underlyingModule;

		public override Symbol ContainingSymbol => _retargetingAssembly;

		public override AssemblySymbol ContainingAssembly => _retargetingAssembly;

		public override string Name => _underlyingModule.Name;

		public override string MetadataName => _underlyingModule.MetadataName;

		public override NamespaceSymbol GlobalNamespace => RetargetingTranslator.Retarget(_underlyingModule.GlobalNamespace);

		public override ImmutableArray<Location> Locations => _underlyingModule.Locations;

		internal override ICollection<string> TypeNames => _underlyingModule.TypeNames;

		internal override ICollection<string> NamespaceNames => _underlyingModule.NamespaceNames;

		internal override bool MightContainExtensionMethods => _underlyingModule.MightContainExtensionMethods;

		internal override bool HasAssemblyCompilationRelaxationsAttribute => _underlyingModule.HasAssemblyCompilationRelaxationsAttribute;

		internal override bool HasAssemblyRuntimeCompatibilityAttribute => _underlyingModule.HasAssemblyRuntimeCompatibilityAttribute;

		internal override CharSet? DefaultMarshallingCharSet => _underlyingModule.DefaultMarshallingCharSet;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingModuleSymbol(RetargetingAssemblySymbol retargetingAssembly, SourceModuleSymbol underlyingModule)
		{
			_retargetingAssemblyMap = new Dictionary<AssemblySymbol, DestinationData>();
			_symbolMap = new ConcurrentDictionary<Symbol, Symbol>();
			_retargetingAssembly = retargetingAssembly;
			_underlyingModule = underlyingModule;
			RetargetingTranslator = new RetargetingSymbolTranslator(this);
			_createRetargetingMethod = CreateRetargetingMethod;
			_createRetargetingNamespace = CreateRetargetingNamespace;
			_createRetargetingNamedType = CreateRetargetingNamedType;
			_createRetargetingField = CreateRetargetingField;
			_createRetargetingTypeParameter = CreateRetargetingTypeParameter;
			_createRetargetingProperty = CreateRetargetingProperty;
			_createRetargetingEvent = CreateRetargetingEvent;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingModule, ref _lazyCustomAttributes);
		}

		internal override void SetReferences(ModuleReferences<AssemblySymbol> moduleReferences, SourceAssemblySymbol originatingSourceAssemblyDebugOnly = null)
		{
			base.SetReferences(moduleReferences, originatingSourceAssemblyDebugOnly);
			_retargetingAssemblyMap.Clear();
			ImmutableArray<AssemblySymbol> referencedAssemblySymbols = _underlyingModule.GetReferencedAssemblySymbols();
			ImmutableArray<AssemblySymbol> symbols = moduleReferences.Symbols;
			_ = moduleReferences.Identities;
			int num = -1;
			int i = -1;
			while (true)
			{
				num++;
				i++;
				if (num >= symbols.Length)
				{
					break;
				}
				for (; referencedAssemblySymbols[i].IsLinked; i++)
				{
				}
				if ((object)symbols[num] != referencedAssemblySymbols[i])
				{
					DestinationData value = default(DestinationData);
					if (!_retargetingAssemblyMap.TryGetValue(referencedAssemblySymbols[i], out value))
					{
						ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol> symbolMap = new ConcurrentDictionary<NamedTypeSymbol, NamedTypeSymbol>();
						_retargetingAssemblyMap.Add(referencedAssemblySymbols[i], new DestinationData
						{
							To = symbols[num],
							SymbolMap = symbolMap
						});
					}
				}
			}
		}

		internal bool RetargetingDefinitions(AssemblySymbol from, out AssemblySymbol to)
		{
			DestinationData value = default(DestinationData);
			if (!_retargetingAssemblyMap.TryGetValue(from, out value))
			{
				to = null;
				return false;
			}
			to = value.To;
			return true;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingModule.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		public override ModuleMetadata GetMetadata()
		{
			return _underlyingModule.GetMetadata();
		}

		private RetargetingMethodSymbol CreateRetargetingMethod(Symbol symbol)
		{
			return new RetargetingMethodSymbol(this, (MethodSymbol)symbol);
		}

		private RetargetingNamespaceSymbol CreateRetargetingNamespace(Symbol symbol)
		{
			return new RetargetingNamespaceSymbol(this, (NamespaceSymbol)symbol);
		}

		private RetargetingNamedTypeSymbol CreateRetargetingNamedType(Symbol symbol)
		{
			return new RetargetingNamedTypeSymbol(this, (NamedTypeSymbol)symbol);
		}

		private RetargetingFieldSymbol CreateRetargetingField(Symbol symbol)
		{
			return new RetargetingFieldSymbol(this, (FieldSymbol)symbol);
		}

		private RetargetingPropertySymbol CreateRetargetingProperty(Symbol symbol)
		{
			return new RetargetingPropertySymbol(this, (PropertySymbol)symbol);
		}

		private RetargetingEventSymbol CreateRetargetingEvent(Symbol symbol)
		{
			return new RetargetingEventSymbol(this, (EventSymbol)symbol);
		}

		private RetargetingTypeParameterSymbol CreateRetargetingTypeParameter(Symbol symbol)
		{
			TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)symbol;
			Symbol containingSymbol = typeParameterSymbol.ContainingSymbol;
			if (containingSymbol.Kind != SymbolKind.Method)
			{
				_ = (NamedTypeSymbol)containingSymbol;
			}
			else
			{
				_ = containingSymbol.ContainingType;
			}
			return new RetargetingTypeParameterSymbol(this, typeParameterSymbol);
		}
	}
}
