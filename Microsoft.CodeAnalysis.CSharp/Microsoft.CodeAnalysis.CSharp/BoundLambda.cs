using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLambda : BoundExpression, IBoundLambdaOrFunction
    {
        internal sealed class BlockReturns : BoundTreeWalker
        {
            private readonly ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> _builder;

            private BlockReturns(ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> builder)
            {
                _builder = builder;
            }

            public static void GetReturnTypes(ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> builder, BoundBlock block)
            {
                new BlockReturns(builder).Visit(block);
            }

            public override BoundNode? Visit(BoundNode node)
            {
                if (!(node is BoundExpression))
                {
                    return base.Visit(node);
                }
                return null;
            }

            protected override BoundExpression VisitExpressionWithoutStackGuard(BoundExpression node)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
            {
                return null;
            }

            public override BoundNode? VisitReturnStatement(BoundReturnStatement node)
            {
                BoundExpression expressionOpt = node.ExpressionOpt;
                TypeSymbol typeSymbol = ((expressionOpt == null) ? NoReturnExpression : expressionOpt.Type?.SetUnknownNullabilityForReferenceTypes());
                _builder.Add((node, TypeWithAnnotations.Create(typeSymbol)));
                return null;
            }
        }

        internal static readonly TypeSymbol NoReturnExpression = new UnsupportedMetadataTypeSymbol();

        public override Symbol ExpressionSymbol => Symbol;

        public override object Display => MessageID.Localize();

        public MessageID MessageID
        {
            get
            {
                if (Syntax.Kind() != SyntaxKind.AnonymousMethodExpression)
                {
                    return MessageID.IDS_Lambda;
                }
                return MessageID.IDS_AnonMethod;
            }
        }

        internal InferredLambdaReturnType InferredReturnType { get; private set; }

        MethodSymbol IBoundLambdaOrFunction.Symbol => Symbol;

        SyntaxNode IBoundLambdaOrFunction.Syntax => Syntax;

        public UnboundLambda UnboundLambda { get; }

        public LambdaSymbol Symbol { get; }

        public new TypeSymbol? Type => base.Type;

        public BoundBlock Body { get; }

        public ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics { get; }

        public Binder Binder { get; }

        public BoundLambda(SyntaxNode syntax, UnboundLambda unboundLambda, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, Binder binder, TypeSymbol? delegateType, InferredLambdaReturnType inferredReturnType)
            : this(syntax, unboundLambda.WithNoCache(), (LambdaSymbol)binder.ContainingMemberOrLambda, body, diagnostics, binder, delegateType)
        {
            InferredReturnType = inferredReturnType;
        }

        public TypeWithAnnotations GetInferredReturnType(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return GetInferredReturnType(null, null, ref useSiteInfo);
        }

        public TypeWithAnnotations GetInferredReturnType(ConversionsBase? conversions, NullableWalker.VariableState? nullableState, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            if (!InferredReturnType.UseSiteDiagnostics.IsEmpty)
            {
                useSiteInfo.AddDiagnostics(InferredReturnType.UseSiteDiagnostics);
            }
            if (!InferredReturnType.Dependencies.IsEmpty)
            {
                useSiteInfo.AddDependencies(InferredReturnType.Dependencies);
            }
            if (nullableState == null)
            {
                return InferredReturnType.TypeWithAnnotations;
            }
            ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> instance = ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>.GetInstance();
            DiagnosticBag instance2 = DiagnosticBag.GetInstance();
            NamedTypeSymbol delegateType = Type.GetDelegateType();
            NullableWalker.Analyze(Binder.Compilation, this, (Conversions)conversions, instance2, delegateType?.DelegateInvokeMethod, nullableState, instance);
            instance2.Free();
            InferredLambdaReturnType inferredLambdaReturnType = InferReturnType(instance, this, Binder, delegateType, Symbol.IsAsync, conversions);
            instance.Free();
            return inferredLambdaReturnType.TypeWithAnnotations;
        }

        internal LambdaSymbol CreateLambdaSymbol(NamedTypeSymbol delegateType, Symbol containingSymbol)
        {
            return UnboundLambda.Data.CreateLambdaSymbol(delegateType, containingSymbol);
        }

        internal LambdaSymbol CreateLambdaSymbol(Symbol containingSymbol, TypeWithAnnotations returnType, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, RefKind refKind)
        {
            return UnboundLambda.Data.CreateLambdaSymbol(containingSymbol, returnType, parameterTypes, parameterRefKinds.IsDefault ? Enumerable.Repeat(RefKind.None, parameterTypes.Length).ToImmutableArray() : parameterRefKinds, refKind);
        }

        internal static InferredLambdaReturnType InferReturnType(ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> returnTypes, BoundLambda node, Binder binder, TypeSymbol? delegateType, bool isAsync, ConversionsBase conversions)
        {
            return InferReturnTypeImpl(returnTypes, node, binder, delegateType, isAsync, conversions, node.UnboundLambda.WithDependencies);
        }

        internal static InferredLambdaReturnType InferReturnType(ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> returnTypes, UnboundLambda node, Binder binder, TypeSymbol? delegateType, bool isAsync, ConversionsBase conversions)
        {
            return InferReturnTypeImpl(returnTypes, node, binder, delegateType, isAsync, conversions, node.WithDependencies);
        }

        private static InferredLambdaReturnType InferReturnTypeImpl(ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)> returnTypes, BoundNode node, Binder binder, TypeSymbol? delegateType, bool isAsync, ConversionsBase conversions, bool withDependencies)
        {
            ArrayBuilder<(BoundExpression, TypeWithAnnotations)> instance = ArrayBuilder<(BoundExpression, TypeWithAnnotations)>.GetInstance();
            bool hadExpressionlessReturn = false;
            RefKind refKind = RefKind.None;
            ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>.Enumerator enumerator = returnTypes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                (BoundReturnStatement, TypeWithAnnotations) current = enumerator.Current;
                BoundReturnStatement item = current.Item1;
                TypeWithAnnotations item2 = current.Item2;
                RefKind refKind2 = item.RefKind;
                if (refKind2 != 0)
                {
                    refKind = refKind2;
                }
                if ((object)item2.Type == NoReturnExpression)
                {
                    hadExpressionlessReturn = true;
                }
                else
                {
                    instance.Add((item.ExpressionOpt, item2));
                }
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = (withDependencies ? new CompoundUseSiteInfo<AssemblySymbol>(binder.Compilation.Assembly) : CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies);
            TypeWithAnnotations typeWithAnnotations = CalculateReturnType(binder, conversions, delegateType, instance, isAsync, node, ref useSiteInfo);
            int count = instance.Count;
            instance.Free();
            return new InferredLambdaReturnType(count, hadExpressionlessReturn, refKind, typeWithAnnotations, useSiteInfo.Diagnostics.AsImmutableOrEmpty(), useSiteInfo.AccumulatesDependencies ? useSiteInfo.Dependencies.AsImmutableOrEmpty() : ImmutableArray<AssemblySymbol>.Empty);
        }

        private static TypeWithAnnotations CalculateReturnType(Binder binder, ConversionsBase conversions, TypeSymbol? delegateType, ArrayBuilder<(BoundExpression, TypeWithAnnotations resultType)> returns, bool isAsync, BoundNode node, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            int count = returns.Count;
            TypeWithAnnotations typeWithAnnotations;
            switch (count)
            {
                case 0:
                    typeWithAnnotations = default(TypeWithAnnotations);
                    break;
                case 1:
                    typeWithAnnotations = returns[0].resultType;
                    break;
                default:
                    {
                        if (conversions.IncludeNullability)
                        {
                            typeWithAnnotations = NullableWalker.BestTypeForLambdaReturns(returns, binder, node, (Conversions)conversions);
                            break;
                        }
                        ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(count);
                        ArrayBuilder<(BoundExpression, TypeWithAnnotations)>.Enumerator enumerator = returns.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            TypeWithAnnotations item = enumerator.Current.Item2;
                            instance.Add(item.Type);
                        }
                        TypeSymbol bestType = BestTypeInferrer.GetBestType(instance, conversions, ref useSiteInfo);
                        typeWithAnnotations = (((object)bestType == null) ? default(TypeWithAnnotations) : TypeWithAnnotations.Create(bestType));
                        instance.Free();
                        break;
                    }
            }
            if (!isAsync)
            {
                return typeWithAnnotations;
            }
            NamedTypeSymbol namedTypeSymbol = null;
            if (delegateType?.GetDelegateType()?.DelegateInvokeMethod?.ReturnType is NamedTypeSymbol namedTypeSymbol2 && !namedTypeSymbol2.IsVoidType() && namedTypeSymbol2.IsCustomTaskType(out var _))
            {
                namedTypeSymbol = namedTypeSymbol2.ConstructedFrom;
            }
            if (count == 0)
            {
                return TypeWithAnnotations.Create(((object)namedTypeSymbol != null && namedTypeSymbol.Arity == 0) ? namedTypeSymbol : binder.Compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task));
            }
            if (!typeWithAnnotations.HasType || typeWithAnnotations.IsVoidType())
            {
                return default(TypeWithAnnotations);
            }
            return TypeWithAnnotations.Create((((object)namedTypeSymbol != null && namedTypeSymbol.Arity == 1) ? namedTypeSymbol : binder.Compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T)).Construct(ImmutableArray.Create(typeWithAnnotations)));
        }

        public BoundLambda(SyntaxNode syntax, UnboundLambda unboundLambda, LambdaSymbol symbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, Binder binder, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.Lambda, syntax, type, hasErrors || unboundLambda.HasErrors() || body.HasErrors())
        {
            UnboundLambda = unboundLambda;
            Symbol = symbol;
            Body = body;
            Diagnostics = diagnostics;
            Binder = binder;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLambda(this);
        }

        public BoundLambda Update(UnboundLambda unboundLambda, LambdaSymbol symbol, BoundBlock body, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics, Binder binder, TypeSymbol? type)
        {
            if (unboundLambda != UnboundLambda || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(symbol, Symbol) || body != Body || diagnostics != Diagnostics || binder != Binder || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundLambda boundLambda = new BoundLambda(Syntax, unboundLambda, symbol, body, diagnostics, binder, type, base.HasErrors);
                boundLambda.CopyAttributes(this);
                return boundLambda;
            }
            return this;
        }
    }
}
