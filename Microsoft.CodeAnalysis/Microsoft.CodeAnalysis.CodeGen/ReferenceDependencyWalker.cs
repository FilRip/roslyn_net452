using System.Collections.Immutable;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;

namespace Microsoft.CodeAnalysis.CodeGen
{
    internal static class ReferenceDependencyWalker
    {
        public static void VisitReference(IReference reference, EmitContext context)
        {
            if (reference is ITypeReference typeReference)
            {
                VisitTypeReference(typeReference, context);
            }
            else if (reference is IMethodReference methodReference)
            {
                VisitMethodReference(methodReference, context);
            }
            else if (reference is IFieldReference fieldReference)
            {
                VisitFieldReference(fieldReference, context);
            }
        }

        private static void VisitTypeReference(ITypeReference typeReference, EmitContext context)
        {
            if (typeReference is IArrayTypeReference arrayTypeReference)
            {
                VisitTypeReference(arrayTypeReference.GetElementType(context), context);
                return;
            }
            if (typeReference is IPointerTypeReference pointerTypeReference)
            {
                VisitTypeReference(pointerTypeReference.GetTargetType(context), context);
                return;
            }
            if (typeReference is IModifiedTypeReference modifiedTypeReference)
            {
                ImmutableArray<ICustomModifier>.Enumerator enumerator = modifiedTypeReference.CustomModifiers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VisitTypeReference(enumerator.Current.GetModifier(context), context);
                }
                VisitTypeReference(modifiedTypeReference.UnmodifiedType, context);
                return;
            }
            INestedTypeReference asNestedTypeReference = typeReference.AsNestedTypeReference;
            if (asNestedTypeReference != null)
            {
                VisitTypeReference(asNestedTypeReference.GetContainingType(context), context);
            }
            IGenericTypeInstanceReference asGenericTypeInstanceReference = typeReference.AsGenericTypeInstanceReference;
            if (asGenericTypeInstanceReference != null)
            {
                ImmutableArray<ITypeReference>.Enumerator enumerator2 = asGenericTypeInstanceReference.GetGenericArguments(context).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    VisitTypeReference(enumerator2.Current, context);
                }
            }
            if (typeReference is IFunctionPointerTypeReference functionPointerTypeReference)
            {
                VisitSignature(functionPointerTypeReference.Signature, context);
            }
        }

        private static void VisitMethodReference(IMethodReference methodReference, EmitContext context)
        {
            VisitTypeReference(methodReference.GetContainingType(context), context);
            IGenericMethodInstanceReference asGenericMethodInstanceReference = methodReference.AsGenericMethodInstanceReference;
            if (asGenericMethodInstanceReference != null)
            {
                foreach (ITypeReference genericArgument in asGenericMethodInstanceReference.GetGenericArguments(context))
                {
                    VisitTypeReference(genericArgument, context);
                }
                methodReference = asGenericMethodInstanceReference.GetGenericMethod(context);
            }
            ISpecializedMethodReference asSpecializedMethodReference = methodReference.AsSpecializedMethodReference;
            if (asSpecializedMethodReference != null)
            {
                methodReference = asSpecializedMethodReference.UnspecializedVersion;
            }
            VisitSignature(methodReference, context);
            if (methodReference.AcceptsExtraArguments)
            {
                VisitParameters(methodReference.ExtraParameters, context);
            }
        }

        internal static void VisitSignature(ISignature signature, EmitContext context)
        {
            VisitParameters(signature.GetParameters(context), context);
            VisitTypeReference(signature.GetType(context), context);
            ImmutableArray<ICustomModifier>.Enumerator enumerator = signature.RefCustomModifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VisitTypeReference(enumerator.Current.GetModifier(context), context);
            }
            enumerator = signature.ReturnValueCustomModifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VisitTypeReference(enumerator.Current.GetModifier(context), context);
            }
        }

        private static void VisitParameters(ImmutableArray<IParameterTypeInformation> parameters, EmitContext context)
        {
            ImmutableArray<IParameterTypeInformation>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IParameterTypeInformation current = enumerator.Current;
                VisitTypeReference(current.GetType(context), context);
                ImmutableArray<ICustomModifier>.Enumerator enumerator2 = current.RefCustomModifiers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    VisitTypeReference(enumerator2.Current.GetModifier(context), context);
                }
                enumerator2 = current.CustomModifiers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    VisitTypeReference(enumerator2.Current.GetModifier(context), context);
                }
            }
        }

        private static void VisitFieldReference(IFieldReference fieldReference, EmitContext context)
        {
            VisitTypeReference(fieldReference.GetContainingType(context), context);
            ISpecializedFieldReference asSpecializedFieldReference = fieldReference.AsSpecializedFieldReference;
            if (asSpecializedFieldReference != null)
            {
                fieldReference = asSpecializedFieldReference.UnspecializedVersion;
            }
            VisitTypeReference(fieldReference.GetType(context), context);
        }
    }
}
