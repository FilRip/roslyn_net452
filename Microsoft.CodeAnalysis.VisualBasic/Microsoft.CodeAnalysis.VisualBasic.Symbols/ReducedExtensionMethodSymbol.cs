using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class ReducedExtensionMethodSymbol : MethodSymbol
	{
		private sealed class ReducedTypeParameterSymbol : TypeParameterSymbol
		{
			private readonly ReducedExtensionMethodSymbol _curriedMethod;

			private readonly TypeParameterSymbol _curriedFromTypeParameter;

			private readonly int _ordinal;

			public override TypeParameterKind TypeParameterKind => TypeParameterKind.Method;

			public override string Name => _curriedFromTypeParameter.Name;

			public override string MetadataName => _curriedFromTypeParameter.MetadataName;

			public override TypeParameterSymbol ReducedFrom => _curriedFromTypeParameter;

			internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics
			{
				get
				{
					ImmutableArray<TypeSymbol> immutableArray = _curriedFromTypeParameter.ConstraintTypesNoUseSiteDiagnostics;
					TypeSubstitution curryTypeSubstitution = _curriedMethod._curryTypeSubstitution;
					if (curryTypeSubstitution != null)
					{
						immutableArray = TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(curryTypeSubstitution, immutableArray);
					}
					return immutableArray;
				}
			}

			public override Symbol ContainingSymbol => _curriedMethod;

			public override bool HasConstructorConstraint => _curriedFromTypeParameter.HasConstructorConstraint;

			public override bool HasReferenceTypeConstraint => _curriedFromTypeParameter.HasReferenceTypeConstraint;

			public override bool HasValueTypeConstraint => _curriedFromTypeParameter.HasValueTypeConstraint;

			public override ImmutableArray<Location> Locations => _curriedFromTypeParameter.Locations;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _curriedFromTypeParameter.DeclaringSyntaxReferences;

			public override int Ordinal => _ordinal;

			public override VarianceKind Variance => _curriedFromTypeParameter.Variance;

			public override bool IsImplicitlyDeclared => _curriedFromTypeParameter.IsImplicitlyDeclared;

			public ReducedTypeParameterSymbol(ReducedExtensionMethodSymbol curriedMethod, TypeParameterSymbol curriedFromTypeParameter, int ordinal)
			{
				_curriedMethod = curriedMethod;
				_curriedFromTypeParameter = curriedFromTypeParameter;
				_ordinal = ordinal;
			}

			public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
			{
				return _curriedFromTypeParameter.GetAttributes();
			}

			public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
			{
				return _curriedFromTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
			}

			internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
			{
				return _curriedFromTypeParameter.GetUseSiteInfo();
			}

			internal override void EnsureAllConstraintsAreResolved()
			{
				_curriedFromTypeParameter.EnsureAllConstraintsAreResolved();
			}

			public override int GetHashCode()
			{
				return Hash.Combine(_ordinal.GetHashCode(), ContainingSymbol.GetHashCode());
			}

			public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
			{
				return Equals(other as ReducedTypeParameterSymbol, comparison);
			}

			public bool Equals(ReducedTypeParameterSymbol other, TypeCompareKind comparison)
			{
				if ((object)this == other)
				{
					return true;
				}
				return (object)other != null && _ordinal == other._ordinal && ContainingSymbol.Equals(other.ContainingSymbol, comparison);
			}
		}

		private class ReducedParameterSymbol : ReducedParameterSymbolBase
		{
			private readonly ReducedExtensionMethodSymbol _curriedMethod;

			private TypeSymbol _lazyType;

			public override Symbol ContainingSymbol => _curriedMethod;

			public override TypeSymbol Type
			{
				get
				{
					if ((object)_lazyType == null)
					{
						TypeSymbol type = m_CurriedFromParameter.Type;
						if (_curriedMethod._curryTypeSubstitution != null)
						{
							type = type.InternalSubstituteTypeParameters(_curriedMethod._curryTypeSubstitution).Type;
						}
						Interlocked.CompareExchange(ref _lazyType, type, null);
					}
					return _lazyType;
				}
			}

			public ReducedParameterSymbol(ReducedExtensionMethodSymbol curriedMethod, ParameterSymbol curriedFromParameter)
				: base(curriedFromParameter)
			{
				_curriedMethod = curriedMethod;
			}
		}

		private readonly TypeSymbol _receiverType;

		private readonly MethodSymbol _curriedFromMethod;

		private readonly ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> _fixedTypeParameters;

		private readonly int _proximity;

		private readonly TypeSubstitution _curryTypeSubstitution;

		private readonly ImmutableArray<ReducedTypeParameterSymbol> _curriedTypeParameters;

		private TypeSymbol _lazyReturnType;

		private ImmutableArray<ReducedParameterSymbol> _lazyParameters;

		public override TypeSymbol ReceiverType => _receiverType;

		internal override ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> FixedTypeParameters => _fixedTypeParameters;

		public override MethodSymbol ReducedFrom => _curriedFromMethod;

		internal override MethodSymbol CallsiteReducedFromMethod
		{
			get
			{
				if (_curryTypeSubstitution == null)
				{
					return _curriedFromMethod;
				}
				if (_curriedFromMethod.Arity == Arity)
				{
					return new SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(_curryTypeSubstitution, TypeArguments);
				}
				TypeSymbol[] array = new TypeSymbol[_curriedFromMethod.Arity - 1 + 1];
				ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Enumerator enumerator = _fixedTypeParameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<TypeParameterSymbol, TypeSymbol> current = enumerator.Current;
					array[current.Key.Ordinal] = current.Value;
				}
				ImmutableArray<ReducedTypeParameterSymbol>.Enumerator enumerator2 = _curriedTypeParameters.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ReducedTypeParameterSymbol current2 = enumerator2.Current;
					array[current2.ReducedFrom.Ordinal] = current2;
				}
				return new SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(_curryTypeSubstitution, array.AsImmutableOrNull());
			}
		}

		internal override bool MayBeReducibleExtensionMethod => false;

		internal override int Proximity => _proximity;

		public override Symbol ContainingSymbol => _curriedFromMethod.ContainingSymbol;

		public override NamedTypeSymbol ContainingType => _curriedFromMethod.ContainingType;

		public override MethodKind MethodKind => MethodKind.ReducedExtension;

		internal override bool IsMethodKindBasedOnSyntax => false;

		public override int Arity => _curriedTypeParameters.Length;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => StaticCast<TypeParameterSymbol>.From(_curriedTypeParameters);

		public override ImmutableArray<TypeSymbol> TypeArguments => StaticCast<TypeSymbol>.From(_curriedTypeParameters);

		public override bool ReturnsByRef => _curriedFromMethod.ReturnsByRef;

		public override TypeSymbol ReturnType
		{
			get
			{
				if ((object)_lazyReturnType == null)
				{
					TypeSymbol typeSymbol = _curriedFromMethod.ReturnType;
					if (_curryTypeSubstitution != null)
					{
						typeSymbol = typeSymbol.InternalSubstituteTypeParameters(_curryTypeSubstitution).Type;
					}
					Interlocked.CompareExchange(ref _lazyReturnType, typeSymbol, null);
				}
				return _lazyReturnType;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					ImmutableArray<ParameterSymbol> parameters = _curriedFromMethod.Parameters;
					if (parameters.Length == 1)
					{
						_lazyParameters = ImmutableArray<ReducedParameterSymbol>.Empty;
					}
					else
					{
						ReducedParameterSymbol[] array = new ReducedParameterSymbol[parameters.Length - 2 + 1];
						int num = parameters.Length - 1;
						for (int i = 1; i <= num; i++)
						{
							array[i - 1] = new ReducedParameterSymbol(this, parameters[i]);
						}
						ImmutableInterlocked.InterlockedCompareExchange(ref _lazyParameters, array.AsImmutableOrNull(), default(ImmutableArray<ReducedParameterSymbol>));
					}
				}
				return StaticCast<ParameterSymbol>.From(_lazyParameters);
			}
		}

		internal override int ParameterCount => _curriedFromMethod.ParameterCount - 1;

		public override bool IsExtensionMethod => true;

		public override bool IsOverloads => false;

		public override bool IsShared => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override MethodSymbol OverriddenMethod => null;

		internal override bool ShadowsExplicitly => false;

		public override bool IsSub => _curriedFromMethod.IsSub;

		public override bool IsAsync => _curriedFromMethod.IsAsync;

		public override bool IsIterator => _curriedFromMethod.IsIterator;

		public override bool IsInitOnly => _curriedFromMethod.IsInitOnly;

		public override bool IsVararg => _curriedFromMethod.IsVararg;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => _curriedFromMethod.ReturnTypeCustomModifiers;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => _curriedFromMethod.RefCustomModifiers;

		internal override SyntaxNode Syntax => _curriedFromMethod.Syntax;

		public override Symbol AssociatedSymbol => null;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override bool IsExternalMethod => _curriedFromMethod.IsExternalMethod;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => _curriedFromMethod.ReturnTypeMarshallingInformation;

		internal override MethodImplAttributes ImplementationAttributes => _curriedFromMethod.ImplementationAttributes;

		internal override bool HasDeclarativeSecurity => _curriedFromMethod.HasDeclarativeSecurity;

		internal override CallingConvention CallingConvention => _curriedFromMethod.CallingConvention;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _curriedFromMethod.ObsoleteAttributeData;

		public override ImmutableArray<Location> Locations => _curriedFromMethod.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _curriedFromMethod.DeclaringSyntaxReferences;

		public override Accessibility DeclaredAccessibility => _curriedFromMethod.DeclaredAccessibility;

		public override bool IsImplicitlyDeclared => _curriedFromMethod.IsImplicitlyDeclared;

		public override string Name => _curriedFromMethod.Name;

		internal override bool HasSpecialName => _curriedFromMethod.HasSpecialName;

		public override string MetadataName => _curriedFromMethod.MetadataName;

		internal override bool GenerateDebugInfoImpl => _curriedFromMethod.GenerateDebugInfo;

		public static MethodSymbol Create(TypeSymbol instanceType, MethodSymbol possiblyExtensionMethod, int proximity, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (!possiblyExtensionMethod.IsDefinition || !possiblyExtensionMethod.MayBeReducibleExtensionMethod || possiblyExtensionMethod.MethodKind == MethodKind.ReducedExtension)
			{
				return null;
			}
			if (possiblyExtensionMethod.ParameterCount == 0)
			{
				return null;
			}
			TypeSymbol type = possiblyExtensionMethod.Parameters[0].Type;
			HashSet<TypeParameterSymbol> hashSet = new HashSet<TypeParameterSymbol>();
			TypeSymbolExtensions.CollectReferencedTypeParameters(type, hashSet);
			ImmutableArray<TypeParameterSymbol> immutableArray = default(ImmutableArray<TypeParameterSymbol>);
			ImmutableArray<TypeSymbol> immutableArray2 = default(ImmutableArray<TypeSymbol>);
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = (useSiteInfo.AccumulatesDependencies ? new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo.AssemblyBeingBuilt) : CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies);
			if (hashSet.Count > 0)
			{
				ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance(possiblyExtensionMethod.ParameterCount, -1);
				instance[0] = 0;
				ImmutableArray<TypeSymbol> typeArguments = default(ImmutableArray<TypeSymbol>);
				TypeArgumentInference.InferenceLevel inferenceLevel = TypeArgumentInference.InferenceLevel.None;
				bool allFailedInferenceIsDueToObject = false;
				bool someInferenceFailed = false;
				InferenceErrorReasons inferenceErrorReasons = InferenceErrorReasons.Other;
				BitVector inferTheseTypeParameters = BitVector.Create(possiblyExtensionMethod.Arity);
				foreach (TypeParameterSymbol item in hashSet)
				{
					inferTheseTypeParameters[item.Ordinal] = true;
				}
				BindingDiagnosticBag diagnostic = (useSiteInfo2.AccumulatesDependencies ? BindingDiagnosticBag.GetInstance(withDiagnostics: false, withDependencies: true) : BindingDiagnosticBag.Discarded);
				ImmutableArray<BoundExpression> arguments = ImmutableArray.Create((BoundExpression)new BoundRValuePlaceholder(VisualBasicSyntaxTree.Dummy.GetRoot(), instanceType));
				BitVector inferredTypeByAssumption = default(BitVector);
				ImmutableArray<SyntaxNodeOrToken> typeArgumentsLocation = default(ImmutableArray<SyntaxNodeOrToken>);
				HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch = null;
				bool num = TypeArgumentInference.Infer(possiblyExtensionMethod, arguments, instance, null, null, null, ref typeArguments, ref inferenceLevel, ref allFailedInferenceIsDueToObject, ref someInferenceFailed, ref inferenceErrorReasons, out inferredTypeByAssumption, out typeArgumentsLocation, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo2, ref diagnostic, inferTheseTypeParameters);
				instance.Free();
				if (!num || !useSiteInfo2.Diagnostics.IsNullOrEmpty())
				{
					diagnostic.Free();
					return null;
				}
				useSiteInfo2.AddDependencies(diagnostic.DependenciesBag);
				diagnostic.Free();
				int count = hashSet.Count;
				ArrayBuilder<TypeParameterSymbol> instance2 = ArrayBuilder<TypeParameterSymbol>.GetInstance(count);
				ArrayBuilder<TypeSymbol> instance3 = ArrayBuilder<TypeSymbol>.GetInstance(count);
				int num2 = possiblyExtensionMethod.Arity - 1;
				for (int i = 0; i <= num2; i++)
				{
					if (inferTheseTypeParameters[i])
					{
						instance2.Add(possiblyExtensionMethod.TypeParameters[i]);
						instance3.Add(typeArguments[i]);
						if (instance2.Count == count)
						{
							break;
						}
					}
				}
				immutableArray = instance2.ToImmutableAndFree();
				immutableArray2 = instance3.ToImmutableAndFree();
				TypeSubstitution typeSubstitution = TypeSubstitution.Create(possiblyExtensionMethod, immutableArray, immutableArray2);
				if (typeSubstitution != null)
				{
					ArrayBuilder<TypeParameterDiagnosticInfo> instance4 = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
					ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
					if (!ConstraintsHelper.CheckConstraints(possiblyExtensionMethod, typeSubstitution, immutableArray, immutableArray2, instance4, ref useSiteDiagnosticsBuilder, new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo2)))
					{
						instance4.Free();
						return null;
					}
					if (useSiteDiagnosticsBuilder != null)
					{
						instance4.AddRange(useSiteDiagnosticsBuilder);
					}
					ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator2 = instance4.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						useSiteInfo2.AddDependencies(enumerator2.Current.UseSiteInfo);
					}
					instance4.Free();
					type = type.InternalSubstituteTypeParameters(typeSubstitution).Type;
				}
			}
			if (!OverloadResolution.DoesReceiverMatchInstance(instanceType, type, ref useSiteInfo2) || !useSiteInfo2.Diagnostics.IsNullOrEmpty())
			{
				return null;
			}
			if (!possiblyExtensionMethod.IsExtensionMethod || possiblyExtensionMethod.MethodKind == MethodKind.ReducedExtension)
			{
				return null;
			}
			ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> fixedTypeParameters = ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Empty;
			if (!immutableArray.IsDefault)
			{
				ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeSymbol>> instance5 = ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.GetInstance(immutableArray.Length);
				int num3 = immutableArray.Length - 1;
				for (int j = 0; j <= num3; j++)
				{
					instance5.Add(new KeyValuePair<TypeParameterSymbol, TypeSymbol>(immutableArray[j], immutableArray2[j]));
				}
				fixedTypeParameters = instance5.ToImmutableAndFree();
			}
			useSiteInfo.AddDependencies(useSiteInfo2);
			return new ReducedExtensionMethodSymbol(type, possiblyExtensionMethod, fixedTypeParameters, proximity);
		}

		private ReducedExtensionMethodSymbol(TypeSymbol receiverType, MethodSymbol curriedFromMethod, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> fixedTypeParameters, int proximity)
		{
			_curriedFromMethod = curriedFromMethod;
			_receiverType = receiverType;
			_fixedTypeParameters = fixedTypeParameters;
			_proximity = proximity;
			if (_curriedFromMethod.Arity == 0)
			{
				_curryTypeSubstitution = null;
				_curriedTypeParameters = ImmutableArray<ReducedTypeParameterSymbol>.Empty;
				return;
			}
			ReducedTypeParameterSymbol[] array = null;
			if (fixedTypeParameters.Length < curriedFromMethod.Arity)
			{
				array = new ReducedTypeParameterSymbol[curriedFromMethod.Arity - fixedTypeParameters.Length - 1 + 1];
			}
			TypeSymbol[] array2 = new TypeSymbol[curriedFromMethod.Arity - 1 + 1];
			int num = fixedTypeParameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				KeyValuePair<TypeParameterSymbol, TypeSymbol> keyValuePair = fixedTypeParameters[i];
				array2[keyValuePair.Key.Ordinal] = keyValuePair.Value;
			}
			if (array == null)
			{
				_curriedTypeParameters = ImmutableArray<ReducedTypeParameterSymbol>.Empty;
			}
			else
			{
				int num2 = 0;
				int num3 = array2.Length - 1;
				for (int i = 0; i <= num3; i++)
				{
					if ((object)array2[i] == null)
					{
						array2[i] = (array[num2] = new ReducedTypeParameterSymbol(this, curriedFromMethod.TypeParameters[i], num2));
						num2++;
						if (num2 == array.Length)
						{
							break;
						}
					}
				}
				_curriedTypeParameters = array.AsImmutableOrNull();
			}
			_curryTypeSubstitution = TypeSubstitution.Create(curriedFromMethod, curriedFromMethod.TypeParameters, array2.AsImmutableOrNull());
		}

		public override TypeSymbol GetTypeInferredDuringReduction(TypeParameterSymbol reducedFromTypeParameter)
		{
			if ((object)reducedFromTypeParameter == null)
			{
				throw new ArgumentNullException();
			}
			if (reducedFromTypeParameter.ContainingSymbol != _curriedFromMethod)
			{
				throw new ArgumentException();
			}
			ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Enumerator enumerator = _fixedTypeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TypeParameterSymbol, TypeSymbol> current = enumerator.Current;
				if (TypeSymbol.Equals(current.Key, reducedFromTypeParameter, TypeCompareKind.ConsiderEverything))
				{
					return current.Value;
				}
			}
			return null;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return _curriedFromMethod.GetUseSiteInfo();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return _curriedFromMethod.GetReturnTypeAttributes();
		}

		public override DllImportData GetDllImportData()
		{
			return _curriedFromMethod.GetDllImportData();
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return _curriedFromMethod.GetSecurityInformation();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _curriedFromMethod.GetAttributes();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _curriedFromMethod.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return _curriedFromMethod.GetAppliedConditionalSymbols();
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_receiverType.GetHashCode(), _curriedFromMethod.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			return obj is ReducedExtensionMethodSymbol reducedExtensionMethodSymbol && reducedExtensionMethodSymbol._curriedFromMethod.Equals(_curriedFromMethod) && reducedExtensionMethodSymbol._receiverType.Equals(_receiverType);
		}
	}
}
