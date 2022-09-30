using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class TypeSubstitution
	{
		private readonly ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> _pairs;

		private readonly Symbol _targetGenericDefinition;

		private readonly TypeSubstitution _parent;

		private static readonly Func<TypeSymbol, TypeWithModifiers> s_withoutModifiers = (TypeSymbol arg) => new TypeWithModifiers(arg);

		public ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> Pairs => _pairs;

		public ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> PairsIncludingParent
		{
			get
			{
				if (_parent == null)
				{
					return Pairs;
				}
				ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> instance = ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.GetInstance();
				AddPairsIncludingParentToBuilder(instance);
				return instance.ToImmutableAndFree();
			}
		}

		public TypeSubstitution Parent => _parent;

		public Symbol TargetGenericDefinition => _targetGenericDefinition;

		private void AddPairsIncludingParentToBuilder(ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> pairBuilder)
		{
			if (_parent != null)
			{
				_parent.AddPairsIncludingParentToBuilder(pairBuilder);
			}
			pairBuilder.AddRange(_pairs);
		}

		public TypeWithModifiers GetSubstitutionFor(TypeParameterSymbol tp)
		{
			Symbol containingSymbol = tp.ContainingSymbol;
			TypeSubstitution typeSubstitution = this;
			TypeWithModifiers result;
			while (true)
			{
				if ((object)typeSubstitution.TargetGenericDefinition == containingSymbol)
				{
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = typeSubstitution.Pairs.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<TypeParameterSymbol, TypeWithModifiers> current = enumerator.Current;
						if (current.Key.Equals(tp))
						{
							return current.Value;
						}
					}
					result = new TypeWithModifiers(tp, ImmutableArray<CustomModifier>.Empty);
					break;
				}
				typeSubstitution = typeSubstitution.Parent;
				if (typeSubstitution == null)
				{
					result = new TypeWithModifiers(tp, ImmutableArray<CustomModifier>.Empty);
					break;
				}
			}
			return result;
		}

		public ImmutableArray<TypeSymbol> GetTypeArgumentsFor(NamedTypeSymbol originalDefinition, out bool hasTypeArgumentsCustomModifiers)
		{
			TypeSubstitution typeSubstitution = this;
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(originalDefinition.Arity, null);
			hasTypeArgumentsCustomModifiers = false;
			do
			{
				if ((object)typeSubstitution.TargetGenericDefinition == originalDefinition)
				{
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = typeSubstitution.Pairs.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<TypeParameterSymbol, TypeWithModifiers> current = enumerator.Current;
						instance[current.Key.Ordinal] = current.Value.Type;
						if (!current.Value.CustomModifiers.IsDefaultOrEmpty)
						{
							hasTypeArgumentsCustomModifiers = true;
						}
					}
					break;
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
			int num = instance.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if ((object)instance[i] == null)
				{
					instance[i] = originalDefinition.TypeParameters[i];
				}
			}
			return instance.ToImmutableAndFree();
		}

		public ImmutableArray<CustomModifier> GetTypeArgumentsCustomModifiersFor(TypeParameterSymbol originalDefinition)
		{
			TypeSubstitution typeSubstitution = this;
			do
			{
				if ((object)typeSubstitution.TargetGenericDefinition == originalDefinition.ContainingSymbol)
				{
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = typeSubstitution.Pairs.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<TypeParameterSymbol, TypeWithModifiers> current = enumerator.Current;
						if (current.Key.Ordinal == originalDefinition.Ordinal)
						{
							return current.Value.CustomModifiers;
						}
					}
					break;
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
			return ImmutableArray<CustomModifier>.Empty;
		}

		public bool HasTypeArgumentsCustomModifiersFor(NamedTypeSymbol originalDefinition)
		{
			TypeSubstitution typeSubstitution = this;
			do
			{
				if ((object)typeSubstitution.TargetGenericDefinition == originalDefinition)
				{
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = typeSubstitution.Pairs.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.Value.CustomModifiers.IsDefaultOrEmpty)
						{
							return true;
						}
					}
					break;
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
			return false;
		}

		public void ThrowIfSubstitutingToAlphaRenamedTypeParameter()
		{
			TypeSubstitution typeSubstitution = this;
			do
			{
				ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = typeSubstitution.Pairs.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeSymbol type = enumerator.Current.Value.Type;
					if (TypeSymbolExtensions.IsTypeParameter(type) && !type.IsDefinition)
					{
						throw new ArgumentException();
					}
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
		}

		public TypeSubstitution GetSubstitutionForGenericDefinition(Symbol targetGenericDefinition)
		{
			TypeSubstitution typeSubstitution = this;
			do
			{
				if ((object)typeSubstitution.TargetGenericDefinition == targetGenericDefinition)
				{
					return typeSubstitution;
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
			return null;
		}

		public TypeSubstitution GetSubstitutionForGenericDefinitionOrContainers(Symbol targetGenericDefinition)
		{
			TypeSubstitution typeSubstitution = this;
			do
			{
				if (typeSubstitution.IsValidToApplyTo(targetGenericDefinition))
				{
					return typeSubstitution;
				}
				typeSubstitution = typeSubstitution.Parent;
			}
			while (typeSubstitution != null);
			return null;
		}

		public bool IsValidToApplyTo(Symbol genericDefinition)
		{
			Symbol symbol = genericDefinition;
			do
			{
				if ((object)symbol == TargetGenericDefinition)
				{
					return true;
				}
				symbol = symbol.ContainingType;
			}
			while ((object)symbol != null);
			return false;
		}

		public static TypeSubstitution Concat(Symbol targetGenericDefinition, TypeSubstitution sub1, TypeSubstitution sub2)
		{
			if (sub1 == null)
			{
				return sub2;
			}
			if (sub2 == null)
			{
				if ((object)targetGenericDefinition == sub1.TargetGenericDefinition)
				{
					return sub1;
				}
				return Concat(sub1, targetGenericDefinition, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Empty);
			}
			return ConcatNotNulls(sub1, sub2);
		}

		private static TypeSubstitution ConcatNotNulls(TypeSubstitution sub1, TypeSubstitution sub2)
		{
			if (sub2.Parent == null)
			{
				return Concat(sub1, sub2.TargetGenericDefinition, sub2.Pairs);
			}
			return Concat(ConcatNotNulls(sub1, sub2.Parent), sub2.TargetGenericDefinition, sub2.Pairs);
		}

		public static TypeSubstitution Create(Symbol targetGenericDefinition, TypeParameterSymbol[] @params, TypeWithModifiers[] args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			return Create(targetGenericDefinition, @params.AsImmutableOrNull(), args.AsImmutableOrNull(), allowAlphaRenamedTypeParametersAsArguments);
		}

		public static TypeSubstitution Create(Symbol targetGenericDefinition, TypeParameterSymbol[] @params, TypeSymbol[] args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			return Create(targetGenericDefinition, @params.AsImmutableOrNull(), args.AsImmutableOrNull(), allowAlphaRenamedTypeParametersAsArguments);
		}

		public static TypeSubstitution Create(Symbol targetGenericDefinition, ImmutableArray<TypeParameterSymbol> @params, ImmutableArray<TypeWithModifiers> args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			if (@params.Length != args.Length)
			{
				throw new ArgumentException(VBResources.NumberOfTypeParametersAndArgumentsMustMatch);
			}
			TypeSubstitution typeSubstitution = null;
			Symbol symbol = null;
			ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> instance = ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.GetInstance();
			try
			{
				int num = @params.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					TypeParameterSymbol typeParameterSymbol = @params[i];
					TypeWithModifiers value = args[i];
					if ((object)symbol != typeParameterSymbol.ContainingSymbol)
					{
						if (instance.Count > 0)
						{
							typeSubstitution = Concat(typeSubstitution, symbol, instance.ToImmutable());
							instance.Clear();
						}
						symbol = typeParameterSymbol.ContainingSymbol;
					}
					if (!value.Is(typeParameterSymbol))
					{
						if (!allowAlphaRenamedTypeParametersAsArguments && TypeSymbolExtensions.IsTypeParameter(value.Type) && !value.Type.IsDefinition)
						{
							throw new ArgumentException();
						}
						instance.Add(new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(typeParameterSymbol, value));
					}
				}
				if (instance.Count > 0)
				{
					typeSubstitution = Concat(typeSubstitution, symbol, instance.ToImmutable());
				}
			}
			finally
			{
				instance.Free();
			}
			if (typeSubstitution != null && (object)typeSubstitution.TargetGenericDefinition != targetGenericDefinition)
			{
				typeSubstitution = Concat(typeSubstitution, targetGenericDefinition, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Empty);
			}
			return typeSubstitution;
		}

		public static TypeSubstitution Create(Symbol targetGenericDefinition, ImmutableArray<TypeParameterSymbol> @params, ImmutableArray<TypeSymbol> args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			return Create(targetGenericDefinition, @params, args.SelectAsArray(s_withoutModifiers), allowAlphaRenamedTypeParametersAsArguments);
		}

		public static TypeSubstitution Create(TypeSubstitution parent, Symbol targetGenericDefinition, ImmutableArray<TypeSymbol> args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			return Create(parent, targetGenericDefinition, args.SelectAsArray(s_withoutModifiers), allowAlphaRenamedTypeParametersAsArguments);
		}

		private static TypeSubstitution Concat(TypeSubstitution parent, Symbol targetGenericDefinition, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> pairs)
		{
			if (parent == null || (object)parent.TargetGenericDefinition == targetGenericDefinition.ContainingType)
			{
				return new TypeSubstitution(targetGenericDefinition, pairs, parent);
			}
			NamedTypeSymbol containingType = targetGenericDefinition.ContainingType;
			return new TypeSubstitution(targetGenericDefinition, pairs, Concat(parent, containingType, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Empty));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} : ", TargetGenericDefinition);
			ToString(stringBuilder);
			return stringBuilder.ToString();
		}

		private void ToString(StringBuilder builder)
		{
			if (_parent != null)
			{
				_parent.ToString(builder);
				builder.Append(", ");
			}
			builder.Append('{');
			int num = _pairs.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (i != 0)
				{
					builder.Append(", ");
				}
				builder.AppendFormat("{0}->{1}", _pairs[i].Key.ToString(), _pairs[i].Value.Type.ToString());
			}
			builder.Append('}');
		}

		private TypeSubstitution(Symbol targetGenericDefinition, ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> pairs, TypeSubstitution parent)
		{
			_pairs = pairs;
			_parent = parent;
			_targetGenericDefinition = targetGenericDefinition;
		}

		public static TypeSubstitution CreateForAlphaRename(TypeSubstitution parent, ImmutableArray<SubstitutedTypeParameterSymbol> alphaRenamedTypeParameters)
		{
			Symbol containingSymbol = alphaRenamedTypeParameters[0].OriginalDefinition.ContainingSymbol;
			ImmutableArray<TypeParameterSymbol> immutableArray = ((containingSymbol.Kind != SymbolKind.Method) ? ((NamedTypeSymbol)containingSymbol).TypeParameters : ((MethodSymbol)containingSymbol).TypeParameters);
			KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[] array = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[immutableArray.Length - 1 + 1];
			int num = immutableArray.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(immutableArray[i], new TypeWithModifiers(alphaRenamedTypeParameters[i]));
			}
			return Concat(parent, containingSymbol, array.AsImmutableOrNull());
		}

		public static TypeSubstitution CreateAdditionalMethodTypeParameterSubstitution(MethodSymbol targetMethod, ImmutableArray<TypeWithModifiers> typeArguments)
		{
			ImmutableArray<TypeParameterSymbol> typeParameters = targetMethod.TypeParameters;
			int num = 0;
			int num2 = typeArguments.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				TypeWithModifiers typeWithModifiers = typeArguments[i];
				if (TypeSymbolExtensions.IsTypeParameter(typeWithModifiers.Type))
				{
					TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)typeWithModifiers.Type;
					if (typeParameterSymbol.Ordinal == i && (object)typeParameterSymbol.ContainingSymbol == targetMethod && typeWithModifiers.CustomModifiers.IsDefaultOrEmpty)
					{
						continue;
					}
				}
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[] array = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[num - 1 + 1];
			num = 0;
			int num3 = typeArguments.Length - 1;
			for (int j = 0; j <= num3; j++)
			{
				TypeWithModifiers typeWithModifiers = typeArguments[j];
				if (TypeSymbolExtensions.IsTypeParameter(typeWithModifiers.Type))
				{
					TypeParameterSymbol typeParameterSymbol2 = (TypeParameterSymbol)typeWithModifiers.Type;
					if (typeParameterSymbol2.Ordinal == j && (object)typeParameterSymbol2.ContainingSymbol == targetMethod && typeWithModifiers.CustomModifiers.IsDefaultOrEmpty)
					{
						continue;
					}
				}
				array[num] = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(typeParameters[j], typeWithModifiers);
				num++;
			}
			return new TypeSubstitution(targetMethod, array.AsImmutableOrNull(), null);
		}

		public static TypeSubstitution AdjustForConstruct(TypeSubstitution adjustedParent, TypeSubstitution oldConstructSubstitution, TypeSubstitution additionalSubstitution)
		{
			ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> instance = ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.GetInstance();
			bool flag = PrivateAdjustForConstruct(instance, oldConstructSubstitution, additionalSubstitution);
			TypeSubstitution result = ((!flag && oldConstructSubstitution.Parent == adjustedParent) ? oldConstructSubstitution : ((instance.Count != 0 || adjustedParent != null) ? Concat(adjustedParent, oldConstructSubstitution.TargetGenericDefinition, flag ? instance.ToImmutable() : oldConstructSubstitution.Pairs) : null));
			instance.Free();
			return result;
		}

		private static bool PrivateAdjustForConstruct(ArrayBuilder<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> pairs, TypeSubstitution oldConstructSubstitution, TypeSubstitution additionalSubstitution)
		{
			bool result = false;
			ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>> pairs2 = oldConstructSubstitution.Pairs;
			BitVector bitVector = default(BitVector);
			Symbol targetGenericDefinition = oldConstructSubstitution.TargetGenericDefinition;
			if (pairs2.Length > 0)
			{
				int capacity = ((targetGenericDefinition.Kind != SymbolKind.Method) ? ((NamedTypeSymbol)targetGenericDefinition).Arity : ((MethodSymbol)targetGenericDefinition).Arity);
				bitVector = BitVector.Create(capacity);
			}
			int num = pairs2.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeWithModifiers value = pairs2[i].Value.InternalSubstituteTypeParameters(additionalSubstitution);
				bitVector[pairs2[i].Key.Ordinal] = true;
				if (!value.Equals(pairs2[i].Value))
				{
					result = true;
				}
				if (!value.Is(pairs2[i].Key))
				{
					pairs.Add(new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(pairs2[i].Key, value));
				}
			}
			TypeSubstitution substitutionForGenericDefinition = additionalSubstitution.GetSubstitutionForGenericDefinition(targetGenericDefinition);
			if (substitutionForGenericDefinition != null)
			{
				ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = substitutionForGenericDefinition.Pairs.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<TypeParameterSymbol, TypeWithModifiers> current = enumerator.Current;
					if (bitVector.IsNull || !bitVector[current.Key.Ordinal])
					{
						result = true;
						pairs.Add(current);
					}
				}
			}
			return result;
		}

		public static TypeSubstitution Create(TypeSubstitution parent, Symbol targetGenericDefinition, ImmutableArray<TypeWithModifiers> args, bool allowAlphaRenamedTypeParametersAsArguments = false)
		{
			ImmutableArray<TypeParameterSymbol> immutableArray = ((targetGenericDefinition.Kind != SymbolKind.Method) ? ((NamedTypeSymbol)targetGenericDefinition).TypeParameters : ((MethodSymbol)targetGenericDefinition).TypeParameters);
			int length = immutableArray.Length;
			if (args.Length != length)
			{
				throw new ArgumentException(VBResources.NumberOfTypeParametersAndArgumentsMustMatch);
			}
			int num = 0;
			int num2 = length - 1;
			for (int i = 0; i <= num2; i++)
			{
				TypeWithModifiers typeWithModifiers = args[i];
				if (!typeWithModifiers.Is(immutableArray[i]))
				{
					num++;
				}
				if (!allowAlphaRenamedTypeParametersAsArguments && TypeSymbolExtensions.IsTypeParameter(typeWithModifiers.Type) && !typeWithModifiers.Type.IsDefinition)
				{
					throw new ArgumentException();
				}
			}
			if (num == 0)
			{
				return Concat(targetGenericDefinition, parent, null);
			}
			int num3 = 0;
			KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[] array = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>[num - 1 + 1];
			int num4 = length - 1;
			for (int j = 0; j <= num4; j++)
			{
				if (!args[j].Is(immutableArray[j]))
				{
					array[num3] = new KeyValuePair<TypeParameterSymbol, TypeWithModifiers>(immutableArray[j], args[j]);
					num3++;
				}
			}
			return Concat(parent, targetGenericDefinition, array.AsImmutableOrNull());
		}

		public ImmutableArray<CustomModifier> SubstituteCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers)
		{
			if (TypeSymbolExtensions.IsTypeParameter(type))
			{
				return new TypeWithModifiers(type, customModifiers).InternalSubstituteTypeParameters(this).CustomModifiers;
			}
			return SubstituteCustomModifiers(customModifiers);
		}

		public ImmutableArray<CustomModifier> SubstituteCustomModifiers(ImmutableArray<CustomModifier> customModifiers)
		{
			if (customModifiers.IsDefaultOrEmpty)
			{
				return customModifiers;
			}
			int num = customModifiers.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				NamedTypeSymbol obj = (NamedTypeSymbol)customModifiers[i].Modifier;
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)obj.InternalSubstituteTypeParameters(this).AsTypeSymbolOnly();
				if (TypeSymbol.Equals(obj, namedTypeSymbol, TypeCompareKind.ConsiderEverything))
				{
					continue;
				}
				ArrayBuilder<CustomModifier> instance = ArrayBuilder<CustomModifier>.GetInstance(customModifiers.Length);
				instance.AddRange(customModifiers, i);
				instance.Add(customModifiers[i].IsOptional ? VisualBasicCustomModifier.CreateOptional(namedTypeSymbol) : VisualBasicCustomModifier.CreateRequired(namedTypeSymbol));
				int num2 = i + 1;
				int num3 = customModifiers.Length - 1;
				for (int j = num2; j <= num3; j++)
				{
					NamedTypeSymbol obj2 = (NamedTypeSymbol)customModifiers[j].Modifier;
					namedTypeSymbol = (NamedTypeSymbol)obj2.InternalSubstituteTypeParameters(this).AsTypeSymbolOnly();
					if (!TypeSymbol.Equals(obj2, namedTypeSymbol, TypeCompareKind.ConsiderEverything))
					{
						instance.Add(customModifiers[j].IsOptional ? VisualBasicCustomModifier.CreateOptional(namedTypeSymbol) : VisualBasicCustomModifier.CreateRequired(namedTypeSymbol));
					}
					else
					{
						instance.Add(customModifiers[j]);
					}
				}
				return instance.ToImmutableAndFree();
			}
			return customModifiers;
		}

		public bool WasConstructedForModifiers()
		{
			ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeWithModifiers>>.Enumerator enumerator = _pairs.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TypeParameterSymbol, TypeWithModifiers> current = enumerator.Current;
				if (!current.Key.Equals(current.Value.Type.OriginalDefinition))
				{
					return false;
				}
			}
			return _parent == null || _parent.WasConstructedForModifiers();
		}
	}
}
