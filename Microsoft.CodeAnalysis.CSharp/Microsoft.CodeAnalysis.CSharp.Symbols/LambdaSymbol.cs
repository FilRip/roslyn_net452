using System.Collections.Immutable;
using System.Reflection;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class LambdaSymbol : SourceMethodSymbolWithAttributes
    {
        private readonly Binder _binder;

        private readonly Symbol _containingSymbol;

        private readonly MessageID _messageID;

        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private RefKind _refKind;

        private TypeWithAnnotations _returnType;

        private readonly bool _isSynthesized;

        private readonly bool _isAsync;

        private readonly bool _isStatic;

        private readonly BindingDiagnosticBag _declarationDiagnostics;

        internal static readonly TypeSymbol ReturnTypeIsBeingInferred = new UnsupportedMetadataTypeSymbol();

        internal static readonly TypeSymbol InferenceFailureReturnType = new UnsupportedMetadataTypeSymbol();

        public MessageID MessageID => _messageID;

        public override MethodKind MethodKind => MethodKind.AnonymousFunction;

        public override bool IsExtern => false;

        public override bool IsSealed => false;

        public override bool IsAbstract => false;

        public override bool IsVirtual => false;

        public override bool IsOverride => false;

        public override bool IsStatic => _isStatic;

        public override bool IsAsync => _isAsync;

        internal override bool IsMetadataFinal => false;

        public override bool IsVararg => false;

        internal override bool HasSpecialName => false;

        internal override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

        public override bool ReturnsVoid
        {
            get
            {
                if (ReturnTypeWithAnnotations.HasType)
                {
                    return base.ReturnType.IsVoidType();
                }
                return false;
            }
        }

        public override RefKind RefKind => _refKind;

        public override TypeWithAnnotations ReturnTypeWithAnnotations => _returnType;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override bool IsExplicitInterfaceImplementation => false;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        public override Symbol? AssociatedSymbol => null;

        public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

        public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

        public override int Arity => 0;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        public override Accessibility DeclaredAccessibility => Accessibility.Private;

        public override ImmutableArray<Location> Locations => ImmutableArray.Create(Syntax.Location);

        internal Location DiagnosticLocation
        {
            get
            {
                SyntaxNode syntax = Syntax;
                if (!(syntax is AnonymousMethodExpressionSyntax anonymousMethodExpressionSyntax))
                {
                    if (syntax is LambdaExpressionSyntax lambdaExpressionSyntax)
                    {
                        return lambdaExpressionSyntax.ArrowToken.GetLocation();
                    }
                    return Locations[0];
                }
                return anonymousMethodExpressionSyntax.DelegateKeyword.GetLocation();
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(syntaxReferenceOpt);

        public override Symbol ContainingSymbol => _containingSymbol;

        internal override CallingConvention CallingConvention => CallingConvention.Default;

        public override bool IsExtensionMethod => false;

        internal SyntaxNode Syntax => syntaxReferenceOpt.GetSyntax();

        internal override Binder SignatureBinder => _binder;

        internal override Binder ParameterBinder => new WithLambdaParametersBinder(this, _binder);

        public override bool IsImplicitlyDeclared => _isSynthesized;

        internal override bool GenerateDebugInfo => true;

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public LambdaSymbol(Binder binder, CSharpCompilation compilation, Symbol containingSymbol, UnboundLambda unboundLambda, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, RefKind refKind, TypeWithAnnotations returnType)
            : base(unboundLambda.Syntax.GetReference())
        {
            _binder = binder;
            _containingSymbol = containingSymbol;
            _messageID = unboundLambda.Data.MessageID;
            _refKind = refKind;
            _returnType = ((!returnType.HasType) ? TypeWithAnnotations.Create(ReturnTypeIsBeingInferred) : returnType);
            _isSynthesized = unboundLambda.WasCompilerGenerated;
            _isAsync = unboundLambda.IsAsync;
            _isStatic = unboundLambda.IsStatic;
            _parameters = MakeParameters(compilation, unboundLambda, parameterTypes, parameterRefKinds);
            _declarationDiagnostics = new BindingDiagnosticBag();
        }

        internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        internal void SetInferredReturnType(RefKind refKind, TypeWithAnnotations inferredReturnType)
        {
            _refKind = refKind;
            _returnType = inferredReturnType;
        }

        internal override bool TryGetThisParameter(out ParameterSymbol? thisParameter)
        {
            thisParameter = null;
            return true;
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            if (!(Syntax is LambdaExpressionSyntax lambdaExpressionSyntax))
            {
                return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
            }
            return OneOrMany.Create(lambdaExpressionSyntax.AttributeLists);
        }

        internal void GetDeclarationDiagnostics(BindingDiagnosticBag addTo)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = _parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.ForceComplete(null, default(CancellationToken));
            }
            GetAttributes();
            GetReturnTypeAttributes();
            addTo.AddRange(_declarationDiagnostics, allowMismatchInDependencyAccumulation: true);
        }

        internal override void AddDeclarationDiagnostics(BindingDiagnosticBag diagnostics)
        {
            _declarationDiagnostics.AddRange(diagnostics);
        }

        private ImmutableArray<ParameterSymbol> MakeParameters(CSharpCompilation compilation, UnboundLambda unboundLambda, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds)
        {
            if (!unboundLambda.HasSignature || unboundLambda.ParameterCount == 0)
            {
                return parameterTypes.SelectAsArray((TypeWithAnnotations type, int ordinal, (LambdaSymbol owner, ImmutableArray<RefKind> refKinds) arg) => SynthesizedParameterSymbol.Create(arg.owner, type, ordinal, arg.refKinds[ordinal], GeneratedNames.LambdaCopyParameterName(ordinal)), (this, parameterRefKinds));
            }
            ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(unboundLambda.ParameterCount);
            bool hasExplicitlyTypedParameterList = unboundLambda.HasExplicitlyTypedParameterList;
            int length = parameterTypes.Length;
            for (int i = 0; i < unboundLambda.ParameterCount; i++)
            {
                TypeWithAnnotations parameterType;
                RefKind refKind;
                if (hasExplicitlyTypedParameterList)
                {
                    parameterType = unboundLambda.ParameterTypeWithAnnotations(i);
                    refKind = unboundLambda.RefKind(i);
                }
                else if (i < length)
                {
                    parameterType = parameterTypes[i];
                    refKind = parameterRefKinds[i];
                }
                else
                {
                    parameterType = TypeWithAnnotations.Create(new ExtendedErrorTypeSymbol(compilation, string.Empty, 0, null));
                    refKind = RefKind.None;
                }
                SyntaxList<AttributeListSyntax> attributeLists = unboundLambda.ParameterAttributes(i);
                string name = unboundLambda.ParameterName(i);
                Location location = unboundLambda.ParameterLocation(i);
                ImmutableArray<Location> locations = ((location == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(location));
                LambdaParameterSymbol item = new LambdaParameterSymbol(this, attributeLists, parameterType, i, refKind, name, unboundLambda.ParameterIsDiscard(i), locations);
                instance.Add(item);
            }
            return instance.ToImmutableAndFree();
        }

        public sealed override bool Equals(Symbol symbol, TypeCompareKind compareKind)
        {
            if ((object)this == symbol)
            {
                return true;
            }
            if (symbol is LambdaSymbol lambdaSymbol)
            {
                if (areEqual(lambdaSymbol.syntaxReferenceOpt, syntaxReferenceOpt) && lambdaSymbol._refKind == _refKind && TypeSymbol.Equals(lambdaSymbol.ReturnType, base.ReturnType, compareKind) && base.ParameterTypesWithAnnotations.SequenceEqual(lambdaSymbol.ParameterTypesWithAnnotations, compareKind, (TypeWithAnnotations p1, TypeWithAnnotations p2, TypeCompareKind compareKind) => p1.Equals(p2, compareKind)))
                {
                    return lambdaSymbol.ContainingSymbol.Equals(ContainingSymbol, compareKind);
                }
            }
            return false;
            static bool areEqual(SyntaxReference a, SyntaxReference b)
            {
                if (a.SyntaxTree == b.SyntaxTree)
                {
                    return a.Span == b.Span;
                }
                return false;
            }
        }

        public override int GetHashCode()
        {
            return syntaxReferenceOpt.GetHashCode();
        }

        public override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            throw ExceptionUtilities.Unreachable;
        }

        protected override void NoteAttributesComplete(bool forReturnType)
        {
        }
    }
}
