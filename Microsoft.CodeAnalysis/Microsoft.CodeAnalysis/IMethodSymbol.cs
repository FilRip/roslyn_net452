using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public interface IMethodSymbol : ISymbol, IEquatable<ISymbol?>
    {
        MethodKind MethodKind { get; }

        int Arity { get; }

        bool IsGenericMethod { get; }

        bool IsExtensionMethod { get; }

        bool IsAsync { get; }

        bool IsVararg { get; }

        bool IsCheckedBuiltin { get; }

        bool HidesBaseMethodsByName { get; }

        bool ReturnsVoid { get; }

        bool ReturnsByRef { get; }

        bool ReturnsByRefReadonly { get; }

        RefKind RefKind { get; }

        ITypeSymbol ReturnType { get; }

        NullableAnnotation ReturnNullableAnnotation { get; }

        ImmutableArray<ITypeSymbol> TypeArguments { get; }

        ImmutableArray<NullableAnnotation> TypeArgumentNullableAnnotations { get; }

        ImmutableArray<ITypeParameterSymbol> TypeParameters { get; }

        ImmutableArray<IParameterSymbol> Parameters { get; }

        IMethodSymbol ConstructedFrom { get; }

        bool IsReadOnly { get; }

        bool IsInitOnly { get; }

        new IMethodSymbol OriginalDefinition { get; }

        IMethodSymbol? OverriddenMethod { get; }

        ITypeSymbol? ReceiverType { get; }

        NullableAnnotation ReceiverNullableAnnotation { get; }

        IMethodSymbol? ReducedFrom { get; }

        ImmutableArray<IMethodSymbol> ExplicitInterfaceImplementations { get; }

        ImmutableArray<CustomModifier> ReturnTypeCustomModifiers { get; }

        ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        SignatureCallingConvention CallingConvention { get; }

        ImmutableArray<INamedTypeSymbol> UnmanagedCallingConventionTypes { get; }

        ISymbol? AssociatedSymbol { get; }

        IMethodSymbol? PartialDefinitionPart { get; }

        IMethodSymbol? PartialImplementationPart { get; }

        MethodImplAttributes MethodImplementationFlags { get; }

        bool IsPartialDefinition { get; }

        INamedTypeSymbol? AssociatedAnonymousDelegate { get; }

        bool IsConditional { get; }

        ITypeSymbol? GetTypeInferredDuringReduction(ITypeParameterSymbol reducedFromTypeParameter);

        IMethodSymbol? ReduceExtensionMethod(ITypeSymbol receiverType);

        ImmutableArray<AttributeData> GetReturnTypeAttributes();

        IMethodSymbol Construct(params ITypeSymbol[] typeArguments);

        IMethodSymbol Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations);

        DllImportData? GetDllImportData();
    }
}
