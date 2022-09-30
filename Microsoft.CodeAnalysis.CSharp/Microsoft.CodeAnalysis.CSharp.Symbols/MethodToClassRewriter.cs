using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class MethodToClassRewriter : BoundTreeRewriterWithStackGuard
    {
        private sealed class BaseMethodWrapperSymbol : SynthesizedMethodBaseSymbol
        {
            internal sealed override bool GenerateDebugInfo => false;

            internal override bool SynthesizesLoweredBoundBody => true;

            internal override TypeWithAnnotations IteratorElementTypeWithAnnotations => default(TypeWithAnnotations);

            internal override bool IsIterator => false;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, this.GetNonNullSyntaxNode(), compilationState, diagnostics);
                syntheticBoundNodeFactory.CurrentFunction = OriginalDefinition;
                try
                {
                    MethodSymbol methodSymbol = BaseMethod;
                    if (Arity > 0)
                    {
                        methodSymbol = methodSymbol.ConstructedFrom.Construct(StaticCast<TypeSymbol>.From(TypeParameters));
                    }
                    BoundBlock boundBlock = MethodBodySynthesizer.ConstructSingleInvocationMethodBody(syntheticBoundNodeFactory, methodSymbol, useBaseReference: true);
                    if (boundBlock.Kind != BoundKind.Block)
                    {
                        boundBlock = syntheticBoundNodeFactory.Block(boundBlock);
                    }
                    syntheticBoundNodeFactory.CompilationState.AddMethodWrapper(methodSymbol, this, boundBlock);
                }
                catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
                {
                    diagnostics.Add(missingPredefinedMember.Diagnostic);
                }
            }

            internal BaseMethodWrapperSymbol(NamedTypeSymbol containingType, MethodSymbol methodBeingWrapped, SyntaxNode syntax, string name)
                : base(containingType, methodBeingWrapped, syntax.SyntaxTree.GetReference(syntax), syntax.GetLocation(), name, DeclarationModifiers.Private)
            {
                TypeMap typeMap = null;
                typeMap = ((!(methodBeingWrapped.ContainingType is SubstitutedNamedTypeSymbol substitutedNamedTypeSymbol)) ? TypeMap.Empty : substitutedNamedTypeSymbol.TypeSubstitution);
                ImmutableArray<TypeParameterSymbol> newTypeParameters;
                if (!methodBeingWrapped.IsGenericMethod)
                {
                    newTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
                }
                else
                {
                    typeMap = typeMap.WithAlphaRename(methodBeingWrapped, this, out newTypeParameters);
                }
                AssignTypeMapAndTypeParameters(typeMap, newTypeParameters);
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
                Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor));
            }
        }

        protected Dictionary<Symbol, CapturedSymbolReplacement> proxies = new Dictionary<Symbol, CapturedSymbolReplacement>();

        protected readonly Dictionary<LocalSymbol, LocalSymbol> localMap = new Dictionary<LocalSymbol, LocalSymbol>();

        protected readonly TypeCompilationState CompilationState;

        protected readonly BindingDiagnosticBag Diagnostics;

        protected readonly VariableSlotAllocator slotAllocatorOpt;

        private readonly Dictionary<BoundValuePlaceholderBase, BoundExpression> _placeholderMap;

        protected abstract TypeMap TypeMap { get; }

        protected abstract MethodSymbol CurrentMethod { get; }

        protected abstract NamedTypeSymbol ContainingType { get; }

        protected abstract BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass);

        protected MethodToClassRewriter(VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            CompilationState = compilationState;
            Diagnostics = diagnostics;
            this.slotAllocatorOpt = slotAllocatorOpt;
            _placeholderMap = new Dictionary<BoundValuePlaceholderBase, BoundExpression>();
        }

        protected abstract bool NeedsProxy(Symbol localOrParameter);

        protected void RewriteLocals(ImmutableArray<LocalSymbol> locals, ArrayBuilder<LocalSymbol> newLocals)
        {
            ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                if (TryRewriteLocal(current, out var newLocal))
                {
                    newLocals.Add(newLocal);
                }
            }
        }

        protected bool TryRewriteLocal(LocalSymbol local, out LocalSymbol newLocal)
        {
            if (NeedsProxy(local))
            {
                newLocal = null;
                return false;
            }
            if (localMap.TryGetValue(local, out newLocal))
            {
                return true;
            }
            TypeSymbol typeSymbol = VisitType(local.Type);
            if (TypeSymbol.Equals(typeSymbol, local.Type, TypeCompareKind.ConsiderEverything))
            {
                newLocal = local;
            }
            else
            {
                newLocal = new TypeSubstitutedLocalSymbol(local, TypeWithAnnotations.Create(typeSymbol), CurrentMethod);
                localMap.Add(local, newLocal);
            }
            return true;
        }

        private ImmutableArray<LocalSymbol> RewriteLocals(ImmutableArray<LocalSymbol> locals)
        {
            if (locals.IsEmpty)
            {
                return locals;
            }
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            RewriteLocals(locals, instance);
            return instance.ToImmutableAndFree();
        }

        public override BoundNode VisitCatchBlock(BoundCatchBlock node)
        {
            if (!node.Locals.IsDefaultOrEmpty)
            {
                ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
                return node.Update(locals, (BoundExpression)Visit(node.ExceptionSourceOpt), VisitType(node.ExceptionTypeOpt), (BoundStatementList)Visit(node.ExceptionFilterPrologueOpt), (BoundExpression)Visit(node.ExceptionFilterOpt), (BoundBlock)Visit(node.Body), node.IsSynthesizedAsyncCatchAll);
            }
            return base.VisitCatchBlock(node);
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
            ImmutableArray<LocalFunctionSymbol> localFunctions = node.LocalFunctions;
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            return node.Update(locals, localFunctions, statements);
        }

        public abstract override BoundNode VisitScope(BoundScope node);

        public override BoundNode VisitSequence(BoundSequence node)
        {
            ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
            ImmutableArray<BoundExpression> sideEffects = VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            TypeSymbol type = VisitType(node.Type);
            return node.Update(locals, sideEffects, value, type);
        }

        public override BoundNode VisitForStatement(BoundForStatement node)
        {
            ImmutableArray<LocalSymbol> outerLocals = RewriteLocals(node.OuterLocals);
            BoundStatement initializer = (BoundStatement)Visit(node.Initializer);
            ImmutableArray<LocalSymbol> innerLocals = RewriteLocals(node.InnerLocals);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement increment = (BoundStatement)Visit(node.Increment);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(outerLocals, initializer, innerLocals, condition, increment, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode VisitDoStatement(BoundDoStatement node)
        {
            ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode VisitWhileStatement(BoundWhileStatement node)
        {
            ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(locals, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode VisitUsingStatement(BoundUsingStatement node)
        {
            ImmutableArray<LocalSymbol> locals = RewriteLocals(node.Locals);
            BoundMultipleLocalDeclarations declarationsOpt = (BoundMultipleLocalDeclarations)Visit(node.DeclarationsOpt);
            BoundExpression expressionOpt = (BoundExpression)Visit(node.ExpressionOpt);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            Conversion iDisposableConversion = RewriteConversion(node.IDisposableConversion);
            return node.Update(locals, declarationsOpt, expressionOpt, iDisposableConversion, body, node.AwaitOpt, node.PatternDisposeInfoOpt);
        }

        private Conversion RewriteConversion(Conversion conversion)
        {
            switch (conversion.Kind)
            {
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return new Conversion(conversion.Kind, VisitMethodSymbol(conversion.Method), conversion.IsExtensionMethod);
                case ConversionKind.MethodGroup:
                    throw ExceptionUtilities.UnexpectedValue(conversion.Kind);
                default:
                    return conversion;
            }
        }

        public sealed override TypeSymbol VisitType(TypeSymbol type)
        {
            return TypeMap.SubstituteType(type).Type;
        }

        public override BoundNode VisitMethodInfo(BoundMethodInfo node)
        {
            MethodSymbol method = VisitMethodSymbol(node.Method);
            return node.Update(method, node.GetMethodFromHandle, node.Type);
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol propertySymbol = VisitPropertySymbol(node.PropertySymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            return node.Update(receiverOpt, propertySymbol, node.ResultKind, VisitType(node.Type));
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            MethodSymbol methodSymbol = VisitMethodSymbol(node.Method);
            BoundExpression boundExpression = (BoundExpression)Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            TypeSymbol type = VisitType(node.Type);
            if (BaseReferenceInReceiverWasRewritten(node.ReceiverOpt, boundExpression) && node.Method.IsMetadataVirtual())
            {
                methodSymbol = GetMethodWrapperForBaseNonVirtualCall(methodSymbol, node.Syntax);
            }
            return node.Update(boundExpression, methodSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, type);
        }

        private MethodSymbol GetMethodWrapperForBaseNonVirtualCall(MethodSymbol methodBeingCalled, SyntaxNode syntax)
        {
            MethodSymbol orCreateBaseFunctionWrapper = GetOrCreateBaseFunctionWrapper(methodBeingCalled, syntax);
            if (!orCreateBaseFunctionWrapper.IsGenericMethod)
            {
                return orCreateBaseFunctionWrapper;
            }
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotations = methodBeingCalled.TypeArgumentsWithAnnotations;
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(typeArgumentsWithAnnotations.Length);
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeArgumentsWithAnnotations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations current = enumerator.Current;
                instance.Add(current.WithTypeAndModifiers(VisitType(current.Type), current.CustomModifiers));
            }
            return orCreateBaseFunctionWrapper.Construct(instance.ToImmutableAndFree());
        }

        private MethodSymbol GetOrCreateBaseFunctionWrapper(MethodSymbol methodBeingWrapped, SyntaxNode syntax)
        {
            methodBeingWrapped = methodBeingWrapped.ConstructedFrom;
            MethodSymbol methodWrapper = CompilationState.GetMethodWrapper(methodBeingWrapped);
            if ((object)methodWrapper != null)
            {
                return methodWrapper;
            }
            NamedTypeSymbol containingType = ContainingType;
            string name = GeneratedNames.MakeBaseMethodWrapperName(CompilationState.NextWrapperMethodIndex);
            methodWrapper = new BaseMethodWrapperSymbol(containingType, methodBeingWrapped, syntax, name);
            if (CompilationState.Emitting)
            {
                CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(containingType, methodWrapper.GetCciAdapter());
            }
            methodWrapper.GenerateMethodBody(CompilationState, Diagnostics);
            return methodWrapper;
        }

        private bool TryReplaceWithProxy(Symbol parameterOrLocal, SyntaxNode syntax, out BoundNode replacement)
        {
            if (proxies.TryGetValue(parameterOrLocal, out var value))
            {
                replacement = value.Replacement(syntax, (NamedTypeSymbol frameType) => FramePointer(syntax, frameType));
                return true;
            }
            replacement = null;
            return false;
        }

        public sealed override BoundNode VisitParameter(BoundParameter node)
        {
            if (TryReplaceWithProxy(node.ParameterSymbol, node.Syntax, out var replacement))
            {
                return replacement;
            }
            return VisitUnhoistedParameter(node);
        }

        protected virtual BoundNode VisitUnhoistedParameter(BoundParameter node)
        {
            return base.VisitParameter(node);
        }

        public sealed override BoundNode VisitLocal(BoundLocal node)
        {
            if (TryReplaceWithProxy(node.LocalSymbol, node.Syntax, out var replacement))
            {
                return replacement;
            }
            return VisitUnhoistedLocal(node);
        }

        private BoundNode VisitUnhoistedLocal(BoundLocal node)
        {
            if (localMap.TryGetValue(node.LocalSymbol, out var value))
            {
                return new BoundLocal(node.Syntax, value, node.ConstantValueOpt, value.Type, node.HasErrors);
            }
            return base.VisitLocal(node);
        }

        public override BoundNode VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            BoundAwaitableValuePlaceholder awaitableInstancePlaceholder = node.AwaitableInstancePlaceholder;
            if (awaitableInstancePlaceholder == null)
            {
                return node;
            }
            BoundAwaitableValuePlaceholder boundAwaitableValuePlaceholder = awaitableInstancePlaceholder.Update(awaitableInstancePlaceholder.ValEscape, VisitType(awaitableInstancePlaceholder.Type));
            _placeholderMap.Add(awaitableInstancePlaceholder, boundAwaitableValuePlaceholder);
            BoundExpression getAwaiter = (BoundExpression)Visit(node.GetAwaiter);
            PropertySymbol isCompleted = VisitPropertySymbol(node.IsCompleted);
            MethodSymbol getResult = VisitMethodSymbol(node.GetResult);
            _placeholderMap.Remove(awaitableInstancePlaceholder);
            return node.Update(boundAwaitableValuePlaceholder, node.IsDynamic, getAwaiter, isCompleted, getResult);
        }

        public override BoundNode VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            return _placeholderMap[node];
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            BoundExpression left = node.Left;
            if (left.Kind != BoundKind.Local)
            {
                return base.VisitAssignmentOperator(node);
            }
            BoundLocal boundLocal = (BoundLocal)left;
            BoundExpression right = node.Right;
            if (boundLocal.LocalSymbol.RefKind != 0 && node.IsRef && NeedsProxy(boundLocal.LocalSymbol))
            {
                throw ExceptionUtilities.Unreachable;
            }
            if (NeedsProxy(boundLocal.LocalSymbol) && !proxies.ContainsKey(boundLocal.LocalSymbol))
            {
                throw ExceptionUtilities.Unreachable;
            }
            BoundExpression boundExpression = (BoundExpression)Visit(boundLocal);
            BoundExpression boundExpression2 = (BoundExpression)Visit(right);
            TypeSymbol type = VisitType(node.Type);
            if (boundExpression.Kind != BoundKind.Local && right.Kind == BoundKind.ConvertedStackAllocExpression)
            {
                BoundLocal boundLocal2 = new SyntheticBoundNodeFactory(CurrentMethod, boundExpression.Syntax, CompilationState, Diagnostics).StoreToTemp(boundExpression2, out BoundAssignmentOperator store);
                BoundAssignmentOperator value = node.Update(boundExpression, boundLocal2, node.IsRef, type);
                return new BoundSequence(node.Syntax, ImmutableArray.Create(boundLocal2.LocalSymbol), ImmutableArray.Create((BoundExpression)store), value, type);
            }
            return node.Update(boundExpression, boundExpression2, node.IsRef, type);
        }

        public override BoundNode VisitFieldInfo(BoundFieldInfo node)
        {
            FieldSymbol field = node.Field.OriginalDefinition.AsMember((NamedTypeSymbol)VisitType(node.Field.ContainingType));
            return node.Update(field, node.GetFieldFromHandle, node.Type);
        }

        public override BoundNode VisitFieldAccess(BoundFieldAccess node)
        {
            BoundExpression receiver = (BoundExpression)Visit(node.ReceiverOpt);
            TypeSymbol typeSymbol = VisitType(node.Type);
            FieldSymbol fieldSymbol = node.FieldSymbol.OriginalDefinition.AsMember((NamedTypeSymbol)VisitType(node.FieldSymbol.ContainingType));
            return node.Update(receiver, fieldSymbol, node.ConstantValueOpt, node.ResultKind, typeSymbol);
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)base.VisitObjectCreationExpression(node);
            if (!TypeSymbol.Equals(boundObjectCreationExpression.Type, node.Type, TypeCompareKind.ConsiderEverything) && (object)node.Constructor != null)
            {
                MethodSymbol constructor = VisitMethodSymbol(node.Constructor);
                boundObjectCreationExpression = boundObjectCreationExpression.Update(constructor, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.ArgumentNamesOpt, boundObjectCreationExpression.ArgumentRefKindsOpt, boundObjectCreationExpression.Expanded, boundObjectCreationExpression.ArgsToParamsOpt, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.ConstantValueOpt, boundObjectCreationExpression.InitializerExpressionOpt, boundObjectCreationExpression.Type);
            }
            return boundObjectCreationExpression;
        }

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            BoundExpression argument = node.Argument;
            BoundExpression boundExpression = (BoundExpression)Visit(argument);
            MethodSymbol methodSymbol = node.MethodOpt;
            if (BaseReferenceInReceiverWasRewritten(argument, boundExpression) && methodSymbol.IsMetadataVirtual())
            {
                methodSymbol = GetMethodWrapperForBaseNonVirtualCall(methodSymbol, argument.Syntax);
            }
            methodSymbol = VisitMethodSymbol(methodSymbol);
            TypeSymbol type = VisitType(node.Type);
            return node.Update(boundExpression, methodSymbol, node.IsExtensionMethod, type);
        }

        public override BoundNode VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            return node.Update(VisitMethodSymbol(node.TargetMethod), VisitType(node.Type));
        }

        public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundExpression whenNotNull = (BoundExpression)Visit(node.WhenNotNull);
            BoundExpression whenNullOpt = (BoundExpression)Visit(node.WhenNullOpt);
            TypeSymbol type = VisitType(node.Type);
            return node.Update(receiver, VisitMethodSymbol(node.HasValueMethodOpt), whenNotNull, whenNullOpt, node.Id, type);
        }

        protected MethodSymbol VisitMethodSymbol(MethodSymbol method)
        {
            if ((object)method == null)
            {
                return null;
            }
            if (method.ContainingType.IsAnonymousType)
            {
                NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeMap.SubstituteType(method.ContainingType).AsTypeSymbolOnly();
                if ((object)namedTypeSymbol == method.ContainingType)
                {
                    return method;
                }
                ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(method.Name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.Method)
                    {
                        return (MethodSymbol)current;
                    }
                }
                throw ExceptionUtilities.Unreachable;
            }
            return method.OriginalDefinition.AsMember((NamedTypeSymbol)TypeMap.SubstituteType(method.ContainingType).AsTypeSymbolOnly()).ConstructIfGeneric(TypeMap.SubstituteTypes(method.TypeArgumentsWithAnnotations));
        }

        private PropertySymbol VisitPropertySymbol(PropertySymbol property)
        {
            if ((object)property == null)
            {
                return null;
            }
            if (!property.ContainingType.IsAnonymousType)
            {
                return property.OriginalDefinition.AsMember((NamedTypeSymbol)TypeMap.SubstituteType(property.ContainingType).AsTypeSymbolOnly());
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeMap.SubstituteType(property.ContainingType).AsTypeSymbolOnly();
            if ((object)namedTypeSymbol == property.ContainingType)
            {
                return property;
            }
            ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(property.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Property)
                {
                    return (PropertySymbol)current;
                }
            }
            throw ExceptionUtilities.Unreachable;
        }

        private FieldSymbol VisitFieldSymbol(FieldSymbol field)
        {
            return field.OriginalDefinition.AsMember((NamedTypeSymbol)TypeMap.SubstituteType(field.ContainingType).AsTypeSymbolOnly());
        }

        public override BoundNode VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            TypeSymbol type = VisitType(node.Type);
            TypeSymbol receiverType = VisitType(node.ReceiverType);
            Symbol symbol = node.MemberSymbol;
            switch (symbol.Kind)
            {
                case SymbolKind.Field:
                    symbol = VisitFieldSymbol((FieldSymbol)symbol);
                    break;
                case SymbolKind.Property:
                    symbol = VisitPropertySymbol((PropertySymbol)symbol);
                    break;
            }
            return node.Update(symbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, receiverType, type);
        }

        public override BoundNode VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            MethodSymbol conversionMethod = VisitMethodSymbol(node.ConversionMethod);
            TypeSymbol type = VisitType(node.Type);
            return node.Update(operand, conversionMethod, type);
        }

        private static bool BaseReferenceInReceiverWasRewritten(BoundExpression originalReceiver, BoundExpression rewrittenReceiver)
        {
            if (originalReceiver != null && originalReceiver.Kind == BoundKind.BaseReference && rewrittenReceiver != null)
            {
                return rewrittenReceiver.Kind != BoundKind.BaseReference;
            }
            return false;
        }
    }
}
