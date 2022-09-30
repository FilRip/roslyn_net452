using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal static class NullableAnnotationExtensions
    {
        public const byte NotAnnotatedAttributeValue = 1;

        public const byte AnnotatedAttributeValue = 2;

        public const byte ObliviousAttributeValue = 0;

        public static bool IsAnnotated(this NullableAnnotation annotation)
        {
            return annotation == NullableAnnotation.Annotated;
        }

        public static bool IsNotAnnotated(this NullableAnnotation annotation)
        {
            return annotation == NullableAnnotation.NotAnnotated;
        }

        public static bool IsOblivious(this NullableAnnotation annotation)
        {
            return annotation == NullableAnnotation.Oblivious;
        }

        public static NullableAnnotation Join(this NullableAnnotation a, NullableAnnotation b)
        {
            if ((int)a >= (int)b)
            {
                return a;
            }
            return b;
        }

        public static NullableAnnotation Meet(this NullableAnnotation a, NullableAnnotation b)
        {
            if ((int)a >= (int)b)
            {
                return b;
            }
            return a;
        }

        public static NullableAnnotation EnsureCompatible(this NullableAnnotation a, NullableAnnotation b)
        {
            if (a != NullableAnnotation.Oblivious)
            {
                if (b == NullableAnnotation.Oblivious)
                {
                    return a;
                }
                return ((int)a < (int)b) ? a : b;
            }
            return b;
        }

        public static NullableAnnotation MergeNullableAnnotation(this NullableAnnotation a, NullableAnnotation b, VarianceKind variance)
        {
            return variance switch
            {
                VarianceKind.In => a.Meet(b),
                VarianceKind.Out => a.Join(b),
                VarianceKind.None => a.EnsureCompatible(b),
                _ => throw ExceptionUtilities.UnexpectedValue(variance),
            };
        }

        internal static NullabilityInfo ToNullabilityInfo(this Microsoft.CodeAnalysis.NullableAnnotation annotation, TypeSymbol type)
        {
            if (annotation == Microsoft.CodeAnalysis.NullableAnnotation.None)
            {
                return default(NullabilityInfo);
            }
            return annotation.ToInternalAnnotation().ToNullabilityInfo(type);
        }

        internal static NullabilityInfo ToNullabilityInfo(this NullableAnnotation annotation, TypeSymbol type)
        {
            NullableFlowState state = TypeWithAnnotations.Create(type, annotation).ToTypeWithState().State;
            return new NullabilityInfo(ToPublicAnnotation(type, annotation), state.ToPublicFlowState());
        }

        internal static ITypeSymbol GetPublicSymbol(this TypeWithAnnotations type)
        {
            return type.Type?.GetITypeSymbol(type.ToPublicAnnotation());
        }

        internal static ImmutableArray<ITypeSymbol> GetPublicSymbols(this ImmutableArray<TypeWithAnnotations> types)
        {
            return types.SelectAsArray((TypeWithAnnotations t) => t.GetPublicSymbol());
        }

        internal static Microsoft.CodeAnalysis.NullableAnnotation ToPublicAnnotation(this TypeWithAnnotations type)
        {
            return ToPublicAnnotation(type.Type, type.NullableAnnotation);
        }

        internal static ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> ToPublicAnnotations(this ImmutableArray<TypeWithAnnotations> types)
        {
            return types.SelectAsArray((TypeWithAnnotations t) => t.ToPublicAnnotation());
        }

        internal static Microsoft.CodeAnalysis.NullableAnnotation ToPublicAnnotation(TypeSymbol? type, NullableAnnotation annotation)
        {
            switch (annotation)
            {
                case NullableAnnotation.Annotated:
                    return Microsoft.CodeAnalysis.NullableAnnotation.Annotated;
                case NullableAnnotation.NotAnnotated:
                    return Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated;
                case NullableAnnotation.Oblivious:
                    if ((object)type != null && type!.IsValueType)
                    {
                        return Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated;
                    }
                    return Microsoft.CodeAnalysis.NullableAnnotation.None;
                case NullableAnnotation.Ignored:
                    return Microsoft.CodeAnalysis.NullableAnnotation.None;
                default:
                    throw ExceptionUtilities.UnexpectedValue(annotation);
            }
        }

        internal static NullableAnnotation ToInternalAnnotation(this Microsoft.CodeAnalysis.NullableAnnotation annotation)
        {
            return annotation switch
            {
                Microsoft.CodeAnalysis.NullableAnnotation.None => NullableAnnotation.Oblivious,
                Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated => NullableAnnotation.NotAnnotated,
                Microsoft.CodeAnalysis.NullableAnnotation.Annotated => NullableAnnotation.Annotated,
                _ => throw ExceptionUtilities.UnexpectedValue(annotation),
            };
        }
    }
}
