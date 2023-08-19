// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

using Microsoft.CodeAnalysis.Emit;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    /// <summary>
    /// Some features of the compiler (such as anonymous types, pay-as-you-go, NoPIA, ...)
    /// rely on all referenced symbols to go through translate mechanism. Because by default
    /// symbol translator does not translate some of indirectly referenced symbols, such as 
    /// type argument, we have to force translation here
    /// 
    /// This class provides unified implementation for this functionality.
    /// </summary>
    internal static class ReferenceDependencyWalker
    {
        public static void VisitReference(Cci.IReference reference, EmitContext context)
        {
            if (reference is Cci.ITypeReference typeReference)
            {
                VisitTypeReference(typeReference, context);
                return;
            }

            if (reference is Cci.IMethodReference methodReference)
            {
                VisitMethodReference(methodReference, context);
                return;
            }

            if (reference is Cci.IFieldReference fieldReference)
            {
                VisitFieldReference(fieldReference, context);
            }
        }

        private static void VisitTypeReference(Cci.ITypeReference typeReference, EmitContext context)
        {
            RoslynDebug.Assert(typeReference != null);

            if (typeReference is Cci.IArrayTypeReference arrayType)
            {
                VisitTypeReference(arrayType.GetElementType(context), context);
                return;
            }

            if (typeReference is Cci.IPointerTypeReference pointerType)
            {
                VisitTypeReference(pointerType.GetTargetType(context), context);
                return;
            }

            if (typeReference is Cci.IModifiedTypeReference modifiedType)
            {
                foreach (var custModifier in modifiedType.CustomModifiers)
                {
                    VisitTypeReference(custModifier.GetModifier(context), context);
                }
                VisitTypeReference(modifiedType.UnmodifiedType, context);
                return;
            }

            // Visit containing type
            Cci.INestedTypeReference? nestedType = typeReference.AsNestedTypeReference;
            if (nestedType != null)
            {
                VisitTypeReference(nestedType.GetContainingType(context), context);
            }

            // Visit generic arguments
            Cci.IGenericTypeInstanceReference? genericInstance = typeReference.AsGenericTypeInstanceReference;
            if (genericInstance != null)
            {
                foreach (var arg in genericInstance.GetGenericArguments(context))
                {
                    VisitTypeReference(arg, context);
                }
            }

            if (typeReference is Cci.IFunctionPointerTypeReference functionPointer)
            {
                VisitSignature(functionPointer.Signature, context);
            }
        }

        private static void VisitMethodReference(Cci.IMethodReference methodReference, EmitContext context)
        {
            RoslynDebug.Assert(methodReference != null);

            // Visit containing type
            VisitTypeReference(methodReference.GetContainingType(context), context);

            // Visit generic arguments if any
            Cci.IGenericMethodInstanceReference? genericInstance = methodReference.AsGenericMethodInstanceReference;
            if (genericInstance != null)
            {
                foreach (var arg in genericInstance.GetGenericArguments(context))
                {
                    VisitTypeReference(arg, context);
                }
                methodReference = genericInstance.GetGenericMethod(context);
            }

            // Translate substituted method to original definition
            Cci.ISpecializedMethodReference? specializedMethod = methodReference.AsSpecializedMethodReference;
            if (specializedMethod != null)
            {
                methodReference = specializedMethod.UnspecializedVersion;
            }

            VisitSignature(methodReference, context);

            if (methodReference.AcceptsExtraArguments)
            {
                VisitParameters(methodReference.ExtraParameters, context);
            }
        }

        internal static void VisitSignature(Cci.ISignature signature, EmitContext context)
        {
            // Visit parameter types
            VisitParameters(signature.GetParameters(context), context);

            // Visit return value type
            VisitTypeReference(signature.GetType(context), context);

            foreach (var typeModifier in signature.RefCustomModifiers)
            {
                VisitTypeReference(typeModifier.GetModifier(context), context);
            }

            foreach (var typeModifier in signature.ReturnValueCustomModifiers)
            {
                VisitTypeReference(typeModifier.GetModifier(context), context);
            }
        }

        private static void VisitParameters(ImmutableArray<Cci.IParameterTypeInformation> parameters, EmitContext context)
        {
            foreach (var param in parameters)
            {
                VisitTypeReference(param.GetType(context), context);

                foreach (var typeModifier in param.RefCustomModifiers)
                {
                    VisitTypeReference(typeModifier.GetModifier(context), context);
                }

                foreach (var typeModifier in param.CustomModifiers)
                {
                    VisitTypeReference(typeModifier.GetModifier(context), context);
                }
            }
        }

        private static void VisitFieldReference(Cci.IFieldReference fieldReference, EmitContext context)
        {
            RoslynDebug.Assert(fieldReference != null);

            // Visit containing type
            VisitTypeReference(fieldReference.GetContainingType(context), context);

            // Translate substituted field to original definition
            Cci.ISpecializedFieldReference? specializedField = fieldReference.AsSpecializedFieldReference;
            if (specializedField != null)
            {
                fieldReference = specializedField.UnspecializedVersion;
            }

            // Visit field type
            VisitTypeReference(fieldReference.GetType(context), context);
        }
    }
}
