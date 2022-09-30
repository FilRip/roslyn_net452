using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class UnboundLambda : BoundExpression
    {
        private readonly NullableWalker.VariableState? _nullableState;

        public override object Display => MessageID.Localize();

        public MessageID MessageID => Data.MessageID;

        public bool HasSignature => Data.HasSignature;

        public bool HasExplicitlyTypedParameterList => Data.HasExplicitlyTypedParameterList;

        public int ParameterCount => Data.ParameterCount;

        public bool IsAsync => Data.IsAsync;

        public bool IsStatic => Data.IsStatic;

        public new TypeSymbol? Type => base.Type;

        public UnboundLambdaState Data { get; }

        public bool WithDependencies { get; }

        public UnboundLambda(CSharpSyntaxNode syntax, Binder binder, bool withDependencies, ImmutableArray<SyntaxList<AttributeListSyntax>> attributes, ImmutableArray<RefKind> refKinds, ImmutableArray<TypeWithAnnotations> types, ImmutableArray<string> names, ImmutableArray<bool> discardsOpt, bool isAsync, bool isStatic)
            : this(syntax, new PlainUnboundLambdaState(binder, attributes, names, discardsOpt, types, refKinds, isAsync, isStatic, includeCache: true), withDependencies, !types.IsDefault && types.Any(delegate (TypeWithAnnotations t)
            {
                TypeSymbol type = t.Type;
                return (object)type != null && type.Kind == SymbolKind.ErrorType;
            }))
        {
            Data.SetUnboundLambda(this);
        }

        private UnboundLambda(SyntaxNode syntax, UnboundLambdaState state, bool withDependencies, NullableWalker.VariableState? nullableState, bool hasErrors)
            : this(syntax, state, withDependencies, hasErrors)
        {
            _nullableState = nullableState;
        }

        internal UnboundLambda WithNullableState(NullableWalker.VariableState nullableState)
        {
            UnboundLambdaState unboundLambdaState = Data.WithCaching(includeCache: true);
            UnboundLambda unboundLambda = new UnboundLambda(Syntax, unboundLambdaState, WithDependencies, nullableState, base.HasErrors);
            unboundLambdaState.SetUnboundLambda(unboundLambda);
            return unboundLambda;
        }

        internal UnboundLambda WithNoCache()
        {
            UnboundLambdaState unboundLambdaState = Data.WithCaching(includeCache: false);
            if (unboundLambdaState == Data)
            {
                return this;
            }
            UnboundLambda unboundLambda = new UnboundLambda(Syntax, unboundLambdaState, WithDependencies, _nullableState, base.HasErrors);
            unboundLambdaState.SetUnboundLambda(unboundLambda);
            return unboundLambda;
        }

        public NamedTypeSymbol? InferDelegateType(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return Data.InferDelegateType(ref useSiteInfo);
        }

        public BoundLambda Bind(NamedTypeSymbol delegateType)
        {
            return SuppressIfNeeded(Data.Bind(delegateType));
        }

        public BoundLambda BindForErrorRecovery()
        {
            return SuppressIfNeeded(Data.BindForErrorRecovery());
        }

        public BoundLambda BindForReturnTypeInference(NamedTypeSymbol delegateType)
        {
            return SuppressIfNeeded(Data.BindForReturnTypeInference(delegateType));
        }

        private BoundLambda SuppressIfNeeded(BoundLambda lambda)
        {
            if (!base.IsSuppressed)
            {
                return lambda;
            }
            return (BoundLambda)lambda.WithSuppression();
        }

        public TypeWithAnnotations InferReturnType(ConversionsBase conversions, NamedTypeSymbol delegateType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return BindForReturnTypeInference(delegateType).GetInferredReturnType(conversions, _nullableState, ref useSiteInfo);
        }

        public RefKind RefKind(int index)
        {
            return Data.RefKind(index);
        }

        public void GenerateAnonymousFunctionConversionError(BindingDiagnosticBag diagnostics, TypeSymbol targetType)
        {
            Data.GenerateAnonymousFunctionConversionError(diagnostics, targetType);
        }

        public bool GenerateSummaryErrors(BindingDiagnosticBag diagnostics)
        {
            return Data.GenerateSummaryErrors(diagnostics);
        }

        public SyntaxList<AttributeListSyntax> ParameterAttributes(int index)
        {
            return Data.ParameterAttributes(index);
        }

        public TypeWithAnnotations ParameterTypeWithAnnotations(int index)
        {
            return Data.ParameterTypeWithAnnotations(index);
        }

        public TypeSymbol ParameterType(int index)
        {
            return ParameterTypeWithAnnotations(index).Type;
        }

        public Location ParameterLocation(int index)
        {
            return Data.ParameterLocation(index);
        }

        public string ParameterName(int index)
        {
            return Data.ParameterName(index);
        }

        public bool ParameterIsDiscard(int index)
        {
            return Data.ParameterIsDiscard(index);
        }

        public UnboundLambda(SyntaxNode syntax, UnboundLambdaState data, bool withDependencies, bool hasErrors)
            : base(BoundKind.UnboundLambda, syntax, null, hasErrors)
        {
            Data = data;
            WithDependencies = withDependencies;
        }

        public UnboundLambda(SyntaxNode syntax, UnboundLambdaState data, bool withDependencies)
            : base(BoundKind.UnboundLambda, syntax, null)
        {
            Data = data;
            WithDependencies = withDependencies;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnboundLambda(this);
        }

        public UnboundLambda Update(UnboundLambdaState data, bool withDependencies)
        {
            if (data != Data || withDependencies != WithDependencies)
            {
                UnboundLambda unboundLambda = new UnboundLambda(Syntax, data, withDependencies, base.HasErrors);
                unboundLambda.CopyAttributes(this);
                return unboundLambda;
            }
            return this;
        }
    }
}
