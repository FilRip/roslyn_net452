using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class TypeMap : AbstractTypeParameterMap
    {
        public static readonly Func<TypeWithAnnotations, TypeSymbol> AsTypeSymbol = (TypeWithAnnotations t) => t.Type;

        private static readonly SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> s_emptyDictionary = new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);

        private static readonly TypeMap s_emptyTypeMap = new TypeMap();

        public static TypeMap Empty => s_emptyTypeMap;

        internal static ImmutableArray<TypeWithAnnotations> TypeParametersAsTypeSymbolsWithAnnotations(ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            return typeParameters.SelectAsArray((TypeParameterSymbol tp) => TypeWithAnnotations.Create(tp));
        }

        internal static ImmutableArray<TypeWithAnnotations> TypeParametersAsTypeSymbolsWithIgnoredAnnotations(ImmutableArray<TypeParameterSymbol> typeParameters)
        {
            return typeParameters.SelectAsArray((TypeParameterSymbol tp) => TypeWithAnnotations.Create(tp, NullableAnnotation.Ignored));
        }

        internal static ImmutableArray<TypeSymbol> AsTypeSymbols(ImmutableArray<TypeWithAnnotations> typesOpt)
        {
            if (!typesOpt.IsDefault)
            {
                return typesOpt.SelectAsArray(AsTypeSymbol);
            }
            return default(ImmutableArray<TypeSymbol>);
        }

        internal TypeMap(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeWithAnnotations> to, bool allowAlpha = false)
            : base(ConstructMapping(from, to))
        {
        }

        internal TypeMap(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeParameterSymbol> to, bool allowAlpha = false)
            : this(from, TypeParametersAsTypeSymbolsWithAnnotations(to), allowAlpha)
        {
        }

        private TypeMap(SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> mapping)
            : base(new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(mapping, ReferenceEqualityComparer.Instance))
        {
        }

        private static SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> ForType(NamedTypeSymbol containingType)
        {
            if (!(containingType is SubstitutedNamedTypeSymbol substitutedNamedTypeSymbol))
            {
                return new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);
            }
            return new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(substitutedNamedTypeSymbol.TypeSubstitution.Mapping, ReferenceEqualityComparer.Instance);
        }

        internal TypeMap(NamedTypeSymbol containingType, ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeWithAnnotations> typeArguments)
            : base(ForType(containingType))
        {
            for (int i = 0; i < typeParameters.Length; i++)
            {
                TypeParameterSymbol typeParameterSymbol = typeParameters[i];
                TypeWithAnnotations value = typeArguments[i];
                if (!value.Is(typeParameterSymbol))
                {
                    Mapping.Add(typeParameterSymbol, value);
                }
            }
        }

        private TypeMap()
            : base(s_emptyDictionary)
        {
        }

        private TypeMap WithAlphaRename(ImmutableArray<TypeParameterSymbol> oldTypeParameters, Symbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            if (oldTypeParameters.Length == 0)
            {
                newTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
                return this;
            }
            TypeMap typeMap = new TypeMap(Mapping);
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            bool flag = (object)oldTypeParameters[0].ContainingSymbol.OriginalDefinition != newOwner.OriginalDefinition;
            int num = 0;
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = oldTypeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                SubstitutedTypeParameterSymbol substitutedTypeParameterSymbol = (flag ? new SynthesizedSubstitutedTypeParameterSymbol(newOwner, typeMap, current, num) : new SubstitutedTypeParameterSymbol(newOwner, typeMap, current, num));
                typeMap.Mapping.Add(current, TypeWithAnnotations.Create(substitutedTypeParameterSymbol));
                instance.Add(substitutedTypeParameterSymbol);
                num++;
            }
            newTypeParameters = instance.ToImmutableAndFree();
            return typeMap;
        }

        internal TypeMap WithAlphaRename(NamedTypeSymbol oldOwner, NamedTypeSymbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            return WithAlphaRename(oldOwner.OriginalDefinition.TypeParameters, newOwner, out newTypeParameters);
        }

        internal TypeMap WithAlphaRename(MethodSymbol oldOwner, Symbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters)
        {
            return WithAlphaRename(oldOwner.OriginalDefinition.TypeParameters, newOwner, out newTypeParameters);
        }

        internal TypeMap WithConcatAlphaRename(MethodSymbol oldOwner, Symbol newOwner, out ImmutableArray<TypeParameterSymbol> newTypeParameters, out ImmutableArray<TypeParameterSymbol> oldTypeParameters, MethodSymbol stopAt = null)
        {
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            while (oldOwner != null && oldOwner != stopAt)
            {
                ImmutableArray<TypeParameterSymbol> typeParameters = oldOwner.OriginalDefinition.TypeParameters;
                for (int num = typeParameters.Length - 1; num >= 0; num--)
                {
                    instance.Add(typeParameters[num]);
                }
                oldOwner = oldOwner.ContainingSymbol.OriginalDefinition as MethodSymbol;
            }
            instance.ReverseContents();
            oldTypeParameters = instance.ToImmutableAndFree();
            return WithAlphaRename(oldTypeParameters, newOwner, out newTypeParameters);
        }

        private static SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> ConstructMapping(ImmutableArray<TypeParameterSymbol> from, ImmutableArray<TypeWithAnnotations> to)
        {
            SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> smallDictionary = new SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>(ReferenceEqualityComparer.Instance);
            for (int i = 0; i < from.Length; i++)
            {
                TypeParameterSymbol typeParameterSymbol = from[i];
                TypeWithAnnotations value = to[i];
                if (!value.Is(typeParameterSymbol))
                {
                    smallDictionary.Add(typeParameterSymbol, value);
                }
            }
            return smallDictionary;
        }
    }
}
