using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class MethodSymbol : Symbol, IMethodSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol _underlying;

        private ITypeSymbol _lazyReturnType;

        private ImmutableArray<ITypeSymbol> _lazyTypeArguments;

        private ITypeSymbol _lazyReceiverType;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol UnderlyingMethodSymbol => _underlying;

        MethodKind IMethodSymbol.MethodKind => _underlying.MethodKind switch
        {
            MethodKind.AnonymousFunction => MethodKind.AnonymousFunction,
            MethodKind.Constructor => MethodKind.Constructor,
            MethodKind.Conversion => MethodKind.Conversion,
            MethodKind.DelegateInvoke => MethodKind.DelegateInvoke,
            MethodKind.Destructor => MethodKind.Destructor,
            MethodKind.EventAdd => MethodKind.EventAdd,
            MethodKind.EventRemove => MethodKind.EventRemove,
            MethodKind.ExplicitInterfaceImplementation => MethodKind.ExplicitInterfaceImplementation,
            MethodKind.UserDefinedOperator => MethodKind.UserDefinedOperator,
            MethodKind.BuiltinOperator => MethodKind.BuiltinOperator,
            MethodKind.Ordinary => MethodKind.Ordinary,
            MethodKind.PropertyGet => MethodKind.PropertyGet,
            MethodKind.PropertySet => MethodKind.PropertySet,
            MethodKind.ReducedExtension => MethodKind.ReducedExtension,
            MethodKind.StaticConstructor => MethodKind.StaticConstructor,
            MethodKind.LocalFunction => MethodKind.LocalFunction,
            MethodKind.FunctionPointerSignature => MethodKind.FunctionPointerSignature,
            _ => throw ExceptionUtilities.UnexpectedValue(_underlying.MethodKind),
        };

        ITypeSymbol IMethodSymbol.ReturnType
        {
            get
            {
                if (_lazyReturnType == null)
                {
                    Interlocked.CompareExchange(ref _lazyReturnType, _underlying.ReturnTypeWithAnnotations.GetPublicSymbol(), null);
                }
                return _lazyReturnType;
            }
        }

        Microsoft.CodeAnalysis.NullableAnnotation IMethodSymbol.ReturnNullableAnnotation => _underlying.ReturnTypeWithAnnotations.ToPublicAnnotation();

        ImmutableArray<ITypeSymbol> IMethodSymbol.TypeArguments
        {
            get
            {
                if (_lazyTypeArguments.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeArguments, _underlying.TypeArgumentsWithAnnotations.GetPublicSymbols(), default(ImmutableArray<ITypeSymbol>));
                }
                return _lazyTypeArguments;
            }
        }

        ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> IMethodSymbol.TypeArgumentNullableAnnotations => _underlying.TypeArgumentsWithAnnotations.ToPublicAnnotations();

        ImmutableArray<ITypeParameterSymbol> IMethodSymbol.TypeParameters => _underlying.TypeParameters.GetPublicSymbols();

        ImmutableArray<IParameterSymbol> IMethodSymbol.Parameters => _underlying.Parameters.GetPublicSymbols();

        IMethodSymbol IMethodSymbol.ConstructedFrom => _underlying.ConstructedFrom.GetPublicSymbol();

        bool IMethodSymbol.IsReadOnly => _underlying.IsEffectivelyReadOnly;

        bool IMethodSymbol.IsInitOnly => _underlying.IsInitOnly;

        IMethodSymbol IMethodSymbol.OriginalDefinition => _underlying.OriginalDefinition.GetPublicSymbol();

        IMethodSymbol IMethodSymbol.OverriddenMethod => _underlying.OverriddenMethod.GetPublicSymbol();

        ITypeSymbol IMethodSymbol.ReceiverType
        {
            get
            {
                if (_lazyReceiverType == null)
                {
                    Interlocked.CompareExchange(ref _lazyReceiverType, _underlying.ReceiverType?.GetITypeSymbol(_underlying.ReceiverNullableAnnotation), null);
                }
                return _lazyReceiverType;
            }
        }

        Microsoft.CodeAnalysis.NullableAnnotation IMethodSymbol.ReceiverNullableAnnotation => _underlying.ReceiverNullableAnnotation;

        IMethodSymbol IMethodSymbol.ReducedFrom => _underlying.ReducedFrom.GetPublicSymbol();

        ImmutableArray<IMethodSymbol> IMethodSymbol.ExplicitInterfaceImplementations => _underlying.ExplicitInterfaceImplementations.GetPublicSymbols();

        ISymbol IMethodSymbol.AssociatedSymbol => _underlying.AssociatedSymbol.GetPublicSymbol();

        bool IMethodSymbol.IsGenericMethod => _underlying.IsGenericMethod;

        bool IMethodSymbol.IsAsync => _underlying.IsAsync;

        bool IMethodSymbol.HidesBaseMethodsByName => _underlying.HidesBaseMethodsByName;

        ImmutableArray<CustomModifier> IMethodSymbol.ReturnTypeCustomModifiers => _underlying.ReturnTypeWithAnnotations.CustomModifiers;

        ImmutableArray<CustomModifier> IMethodSymbol.RefCustomModifiers => _underlying.RefCustomModifiers;

        SignatureCallingConvention IMethodSymbol.CallingConvention => _underlying.CallingConvention.ToSignatureConvention();

        ImmutableArray<INamedTypeSymbol> IMethodSymbol.UnmanagedCallingConventionTypes => _underlying.UnmanagedCallingConventionTypes.SelectAsArray((Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol t) => t.GetPublicSymbol());

        IMethodSymbol IMethodSymbol.PartialImplementationPart => _underlying.PartialImplementationPart.GetPublicSymbol();

        IMethodSymbol IMethodSymbol.PartialDefinitionPart => _underlying.PartialDefinitionPart.GetPublicSymbol();

        bool IMethodSymbol.IsPartialDefinition => _underlying.IsPartialDefinition();

        INamedTypeSymbol IMethodSymbol.AssociatedAnonymousDelegate => null;

        int IMethodSymbol.Arity => _underlying.Arity;

        bool IMethodSymbol.IsExtensionMethod => _underlying.IsExtensionMethod;

        MethodImplAttributes IMethodSymbol.MethodImplementationFlags => _underlying.ImplementationAttributes;

        bool IMethodSymbol.IsVararg => _underlying.IsVararg;

        bool IMethodSymbol.IsCheckedBuiltin => _underlying.IsCheckedBuiltin;

        bool IMethodSymbol.ReturnsVoid => _underlying.ReturnsVoid;

        bool IMethodSymbol.ReturnsByRef => _underlying.ReturnsByRef;

        bool IMethodSymbol.ReturnsByRefReadonly => _underlying.ReturnsByRefReadonly;

        RefKind IMethodSymbol.RefKind => _underlying.RefKind;

        bool IMethodSymbol.IsConditional => _underlying.IsConditional;

        public MethodSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol underlying)
        {
            _underlying = underlying;
        }

        ITypeSymbol IMethodSymbol.GetTypeInferredDuringReduction(ITypeParameterSymbol reducedFromTypeParameter)
        {
            return _underlying.GetTypeInferredDuringReduction(reducedFromTypeParameter.EnsureCSharpSymbolOrNull("reducedFromTypeParameter")).GetPublicSymbol();
        }

        IMethodSymbol IMethodSymbol.ReduceExtensionMethod(ITypeSymbol receiverType)
        {
            return _underlying.ReduceExtensionMethod(receiverType.EnsureCSharpSymbolOrNull("receiverType"), null).GetPublicSymbol();
        }

        ImmutableArray<AttributeData> IMethodSymbol.GetReturnTypeAttributes()
        {
            return _underlying.GetReturnTypeAttributes().Cast<CSharpAttributeData, AttributeData>();
        }

        IMethodSymbol IMethodSymbol.Construct(params ITypeSymbol[] typeArguments)
        {
            return _underlying.Construct(Symbol.ConstructTypeArguments(typeArguments)).GetPublicSymbol();
        }

        IMethodSymbol IMethodSymbol.Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> typeArgumentNullableAnnotations)
        {
            return _underlying.Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations)).GetPublicSymbol();
        }

        DllImportData IMethodSymbol.GetDllImportData()
        {
            return _underlying.GetDllImportData();
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitMethod(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitMethod(this);
        }
    }
}
