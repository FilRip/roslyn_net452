using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct MethodTypeInferenceResult
    {
        public readonly ImmutableArray<TypeWithAnnotations> InferredTypeArguments;

        public readonly bool Success;

        public MethodTypeInferenceResult(bool success, ImmutableArray<TypeWithAnnotations> inferredTypeArguments)
        {
            Success = success;
            InferredTypeArguments = inferredTypeArguments;
        }
    }
}
