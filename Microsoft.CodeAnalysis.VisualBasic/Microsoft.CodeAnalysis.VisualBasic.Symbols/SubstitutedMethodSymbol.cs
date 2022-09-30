using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SubstitutedMethodSymbol : MethodSymbol
	{
		public abstract class SpecializedMethod : SubstitutedMethodSymbol
		{
			protected readonly SubstitutedNamedType _container;

			public abstract override MethodSymbol OriginalDefinition { get; }

			public override MethodSymbol ConstructedFrom => this;

			public override Symbol ContainingSymbol => _container;

			protected SpecializedMethod(SubstitutedNamedType container)
			{
				_container = container;
			}
		}

		public class SpecializedNonGenericMethod : SpecializedMethod
		{
			private readonly MethodSymbol _originalDefinition;

			private readonly ImmutableArray<ParameterSymbol> _parameters;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override TypeSubstitution TypeSubstitution => _container.TypeSubstitution;

			public override MethodSymbol OriginalDefinition => _originalDefinition;

			internal override bool CanConstruct => false;

			public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

			public SpecializedNonGenericMethod(SubstitutedNamedType container, MethodSymbol originalDefinition)
				: base(container)
			{
				_originalDefinition = originalDefinition;
				_parameters = SubstituteParameters();
			}

			public override MethodSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				throw new InvalidOperationException();
			}

			public override bool Equals(object obj)
			{
				if (obj != this)
				{
					return EqualsWithNoRegardToTypeArguments(obj as SpecializedNonGenericMethod);
				}
				return true;
			}
		}

		public sealed class SpecializedGenericMethod : SpecializedMethod
		{
			private readonly TypeSubstitution _substitution;

			private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

			private readonly ImmutableArray<ParameterSymbol> _parameters;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override TypeSubstitution TypeSubstitution => _substitution;

			public override MethodSymbol OriginalDefinition => (MethodSymbol)_substitution.TargetGenericDefinition;

			internal override bool CanConstruct
			{
				get
				{
					NamedTypeSymbol namedTypeSymbol = _container;
					do
					{
						if (namedTypeSymbol.Arity > 0)
						{
							if ((object)namedTypeSymbol.ConstructedFrom == namedTypeSymbol)
							{
								return false;
							}
							return true;
						}
						namedTypeSymbol = namedTypeSymbol.ContainingType;
					}
					while ((object)namedTypeSymbol != null && !namedTypeSymbol.IsDefinition);
					return true;
				}
			}

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

			public override ImmutableArray<TypeSymbol> TypeArguments => StaticCast<TypeSymbol>.From(_typeParameters);

			public static SpecializedGenericMethod Create(SubstitutedNamedType container, MethodSymbol originalDefinition)
			{
				ImmutableArray<TypeParameterSymbol> typeParameters = originalDefinition.TypeParameters;
				SubstitutedTypeParameterSymbol[] array = new SubstitutedTypeParameterSymbol[typeParameters.Length - 1 + 1];
				int num = typeParameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new SubstitutedTypeParameterSymbol(typeParameters[i]);
				}
				ImmutableArray<SubstitutedTypeParameterSymbol> immutableArray = array.AsImmutableOrNull();
				TypeSubstitution substitution = TypeSubstitution.CreateForAlphaRename(container.TypeSubstitution, immutableArray);
				return new SpecializedGenericMethod(container, substitution, immutableArray);
			}

			private SpecializedGenericMethod(SubstitutedNamedType container, TypeSubstitution substitution, ImmutableArray<SubstitutedTypeParameterSymbol> typeParameters)
				: base(container)
			{
				_substitution = substitution;
				_typeParameters = StaticCast<TypeParameterSymbol>.From(typeParameters);
				ImmutableArray<SubstitutedTypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.SetContainingSymbol(this);
				}
				_parameters = SubstituteParameters();
			}

			public override MethodSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				CheckCanConstructAndTypeArguments(typeArguments);
				typeArguments = TypeSymbolExtensions.TransformToCanonicalFormFor(typeArguments, this);
				if (typeArguments.IsDefault)
				{
					return this;
				}
				TypeSubstitution substitution = TypeSubstitution.Create(_substitution.Parent, _substitution.TargetGenericDefinition, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
				return new ConstructedSpecializedGenericMethod(this, substitution, typeArguments);
			}

			public override bool Equals(object obj)
			{
				if (obj != this)
				{
					return EqualsWithNoRegardToTypeArguments(obj as SpecializedGenericMethod);
				}
				return true;
			}
		}

		public abstract class ConstructedMethod : SubstitutedMethodSymbol
		{
			protected readonly TypeSubstitution _substitution;

			protected readonly ImmutableArray<TypeSymbol> _typeArguments;

			public override TypeSubstitution TypeSubstitution => _substitution;

			public sealed override MethodSymbol OriginalDefinition => (MethodSymbol)_substitution.TargetGenericDefinition;

			internal override bool CanConstruct => false;

			public override ImmutableArray<TypeSymbol> TypeArguments => _typeArguments;

			protected ConstructedMethod(TypeSubstitution substitution, ImmutableArray<TypeSymbol> typeArguments)
			{
				_substitution = substitution;
				_typeArguments = typeArguments;
			}

			public override MethodSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				throw new InvalidOperationException();
			}

			public override int GetHashCode()
			{
				int num = base.GetHashCode();
				ImmutableArray<TypeSymbol>.Enumerator enumerator = TypeArguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					num = Hash.Combine(enumerator.Current, num);
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				ConstructedMethod constructedMethod = obj as ConstructedMethod;
				if (!EqualsWithNoRegardToTypeArguments(constructedMethod))
				{
					return false;
				}
				ImmutableArray<TypeSymbol> typeArguments = TypeArguments;
				ImmutableArray<TypeSymbol> typeArguments2 = constructedMethod.TypeArguments;
				int num = typeArguments.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!typeArguments[i].Equals(typeArguments2[i]))
					{
						return false;
					}
				}
				return true;
			}
		}

		public sealed class ConstructedSpecializedGenericMethod : ConstructedMethod
		{
			private readonly SpecializedGenericMethod _constructedFrom;

			private readonly ImmutableArray<ParameterSymbol> _parameters;

			public override MethodSymbol ConstructedFrom => _constructedFrom;

			public override Symbol ContainingSymbol => _constructedFrom.ContainingSymbol;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _constructedFrom.TypeParameters;

			public ConstructedSpecializedGenericMethod(SpecializedGenericMethod constructedFrom, TypeSubstitution substitution, ImmutableArray<TypeSymbol> typeArguments)
				: base(substitution, typeArguments)
			{
				_constructedFrom = constructedFrom;
				_parameters = SubstituteParameters();
			}
		}

		public sealed class ConstructedNotSpecializedGenericMethod : ConstructedMethod
		{
			private readonly ImmutableArray<ParameterSymbol> _parameters;

			public override MethodSymbol ConstructedFrom => base.OriginalDefinition;

			public override Symbol ContainingSymbol => base.OriginalDefinition.ContainingSymbol;

			public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => base.OriginalDefinition.TypeParameters;

			internal override MethodSymbol CallsiteReducedFromMethod
			{
				get
				{
					MethodSymbol reducedFrom = ReducedFrom;
					if ((object)reducedFrom == null)
					{
						return null;
					}
					if (Arity == reducedFrom.Arity)
					{
						return reducedFrom.Construct(TypeArguments);
					}
					TypeSymbol[] array = new TypeSymbol[reducedFrom.Arity - 1 + 1];
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Enumerator enumerator = FixedTypeParameters.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<TypeParameterSymbol, TypeSymbol> current = enumerator.Current;
						array[current.Key.Ordinal] = current.Value;
					}
					ImmutableArray<TypeParameterSymbol> typeParameters = TypeParameters;
					ImmutableArray<TypeSymbol> typeArguments = TypeArguments;
					int num = typeArguments.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[typeParameters[i].ReducedFrom.Ordinal] = typeArguments[i];
					}
					return reducedFrom.Construct(array.AsImmutableOrNull());
				}
			}

			public ConstructedNotSpecializedGenericMethod(TypeSubstitution substitution, ImmutableArray<TypeSymbol> typeArguments)
				: base(substitution, typeArguments)
			{
				_parameters = SubstituteParameters();
			}
		}

		private Symbol _propertyOrEventSymbolOpt;

		private readonly OverriddenMembersResult<MethodSymbol> _lazyOverriddenMethods;

		public abstract override MethodSymbol OriginalDefinition { get; }

		public sealed override string Name => OriginalDefinition.Name;

		public sealed override string MetadataName => OriginalDefinition.MetadataName;

		public sealed override bool IsImplicitlyDeclared => OriginalDefinition.IsImplicitlyDeclared;

		internal sealed override bool HasSpecialName => OriginalDefinition.HasSpecialName;

		internal sealed override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => OriginalDefinition.ReturnTypeMarshallingInformation;

		internal sealed override MethodImplAttributes ImplementationAttributes => OriginalDefinition.ImplementationAttributes;

		internal sealed override bool HasDeclarativeSecurity => OriginalDefinition.HasDeclarativeSecurity;

		public override Symbol AssociatedSymbol => _propertyOrEventSymbolOpt;

		public override MethodSymbol ReducedFrom => OriginalDefinition.ReducedFrom;

		public override TypeSymbol ReceiverType
		{
			get
			{
				if (OriginalDefinition.IsReducedExtensionMethod)
				{
					return OriginalDefinition.ReceiverType;
				}
				return ContainingType;
			}
		}

		internal override ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> FixedTypeParameters => OriginalDefinition.FixedTypeParameters;

		internal override int Proximity => OriginalDefinition.Proximity;

		internal override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		public abstract override MethodSymbol ConstructedFrom { get; }

		public abstract override Symbol ContainingSymbol { get; }

		public override Accessibility DeclaredAccessibility => OriginalDefinition.DeclaredAccessibility;

		public override bool IsExtensionMethod => OriginalDefinition.IsExtensionMethod;

		internal override bool MayBeReducibleExtensionMethod => OriginalDefinition.MayBeReducibleExtensionMethod;

		public override bool IsExternalMethod => OriginalDefinition.IsExternalMethod;

		public override bool IsGenericMethod => OriginalDefinition.IsGenericMethod;

		public abstract TypeSubstitution TypeSubstitution { get; }

		public override int Arity => OriginalDefinition.Arity;

		public override bool IsMustOverride => OriginalDefinition.IsMustOverride;

		public override bool IsNotOverridable => OriginalDefinition.IsNotOverridable;

		public override bool IsOverloads => OriginalDefinition.IsOverloads;

		public override bool IsOverridable => OriginalDefinition.IsOverridable;

		public override bool IsOverrides => OriginalDefinition.IsOverrides;

		public override bool IsShared => OriginalDefinition.IsShared;

		public override bool IsSub => OriginalDefinition.IsSub;

		public override bool IsAsync => OriginalDefinition.IsAsync;

		public override bool IsIterator => OriginalDefinition.IsIterator;

		public sealed override bool IsInitOnly => OriginalDefinition.IsInitOnly;

		public override bool IsVararg => OriginalDefinition.IsVararg;

		public override ImmutableArray<Location> Locations => OriginalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => OriginalDefinition.DeclaringSyntaxReferences;

		public override MethodKind MethodKind => OriginalDefinition.MethodKind;

		internal sealed override bool IsMethodKindBasedOnSyntax => OriginalDefinition.IsMethodKindBasedOnSyntax;

		internal sealed override int ParameterCount => OriginalDefinition.ParameterCount;

		public abstract override ImmutableArray<ParameterSymbol> Parameters { get; }

		public sealed override bool ReturnsByRef => OriginalDefinition.ReturnsByRef;

		public override TypeSymbol ReturnType => OriginalDefinition.ReturnType.InternalSubstituteTypeParameters(TypeSubstitution).Type;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => TypeSubstitution.SubstituteCustomModifiers(OriginalDefinition.ReturnType, OriginalDefinition.ReturnTypeCustomModifiers);

		public override ImmutableArray<CustomModifier> RefCustomModifiers => TypeSubstitution.SubstituteCustomModifiers(OriginalDefinition.RefCustomModifiers);

		public abstract override ImmutableArray<TypeSymbol> TypeArguments { get; }

		public abstract override ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImplementsHelper.SubstituteExplicitInterfaceImplementations(OriginalDefinition.ExplicitInterfaceImplementations, TypeSubstitution);

		internal override CallingConvention CallingConvention => OriginalDefinition.CallingConvention;

		internal abstract override bool CanConstruct { get; }

		internal override SyntaxNode Syntax => null;

		internal sealed override bool GenerateDebugInfoImpl => OriginalDefinition.GenerateDebugInfo;

		protected virtual ImmutableArray<ParameterSymbol> SubstituteParameters()
		{
			ImmutableArray<ParameterSymbol> parameters = OriginalDefinition.Parameters;
			int length = parameters.Length;
			if (length == 0)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = SubstitutedParameterSymbol.CreateMethodParameter(this, parameters[i]);
			}
			return array.AsImmutableOrNull();
		}

		public sealed override DllImportData GetDllImportData()
		{
			return OriginalDefinition.GetDllImportData();
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return OriginalDefinition.GetSecurityInformation();
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return OriginalDefinition.GetAppliedConditionalSymbols();
		}

		public override TypeSymbol GetTypeInferredDuringReduction(TypeParameterSymbol reducedFromTypeParameter)
		{
			return OriginalDefinition.GetTypeInferredDuringReduction(reducedFromTypeParameter);
		}

		internal override bool CallsAreOmitted(SyntaxNodeOrToken atNode, SyntaxTree syntaxTree)
		{
			return OriginalDefinition.CallsAreOmitted(atNode, syntaxTree);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return OriginalDefinition.GetAttributes();
		}

		internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return OriginalDefinition.IsMetadataNewSlot(ignoreInterfaceImplementationChanges);
		}

		internal sealed override bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			ParameterSymbol meParameter2 = null;
			if (!OriginalDefinition.TryGetMeParameter(out meParameter2))
			{
				meParameter = null;
				return false;
			}
			meParameter = (((object)meParameter2 != null) ? new MeParameterSymbol(this) : null);
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = OriginalDefinition.GetHashCode();
			return Hash.Combine(ContainingType, hashCode);
		}

		public abstract override bool Equals(object obj);

		private bool EqualsWithNoRegardToTypeArguments<T>(T other) where T : SubstitutedMethodSymbol
		{
			if (other == null)
			{
				return false;
			}
			if (!OriginalDefinition.Equals(other.OriginalDefinition))
			{
				return false;
			}
			_ = ContainingType;
			if (!ContainingType.Equals(other.ContainingType))
			{
				return false;
			}
			return true;
		}

		public abstract override MethodSymbol Construct(ImmutableArray<TypeSymbol> typeArguments);

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		internal bool SetAssociatedPropertyOrEvent(Symbol propertyOrEventSymbol)
		{
			if ((object)_propertyOrEventSymbolOpt == null)
			{
				_propertyOrEventSymbolOpt = propertyOrEventSymbol;
				return true;
			}
			return false;
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
