using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class ClosureConversion : MethodToClassRewriter
    {
        internal sealed class Analysis : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
        {
            [DebuggerDisplay("{ToString(), nq}")]
            public sealed class Scope
            {
                public readonly Scope Parent;

                public readonly ArrayBuilder<Scope> NestedScopes = ArrayBuilder<Scope>.GetInstance();

                public readonly ArrayBuilder<NestedFunction> NestedFunctions = ArrayBuilder<NestedFunction>.GetInstance();

                public readonly SetWithInsertionOrder<Symbol> DeclaredVariables = new SetWithInsertionOrder<Symbol>();

                public readonly BoundNode BoundNode;

                public readonly NestedFunction ContainingFunctionOpt;

                public ClosureEnvironment? DeclaredEnvironment;

                public bool CanMergeWithParent { get; internal set; } = true;


                public Scope(Scope parent, BoundNode boundNode, NestedFunction containingFunction)
                {
                    Parent = parent;
                    BoundNode = boundNode;
                    ContainingFunctionOpt = containingFunction;
                }

                public void Free()
                {
                    ArrayBuilder<Scope>.Enumerator enumerator = NestedScopes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Free();
                    }
                    NestedScopes.Free();
                    ArrayBuilder<NestedFunction>.Enumerator enumerator2 = NestedFunctions.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.Free();
                    }
                    NestedFunctions.Free();
                }

                public override string ToString()
                {
                    return BoundNode.Syntax.GetText().ToString();
                }
            }

            public sealed class NestedFunction
            {
                public readonly MethodSymbol OriginalMethodSymbol;

                public readonly SyntaxReference BlockSyntax;

                public readonly PooledHashSet<Symbol> CapturedVariables = PooledHashSet<Symbol>.GetInstance();

                public readonly ArrayBuilder<ClosureEnvironment> CapturedEnvironments = ArrayBuilder<ClosureEnvironment>.GetInstance();

                public ClosureEnvironment ContainingEnvironmentOpt;

                private bool _capturesThis;

                public SynthesizedClosureMethod SynthesizedLoweredMethod;

                public bool CapturesThis
                {
                    get
                    {
                        return _capturesThis;
                    }
                    set
                    {
                        _capturesThis = value;
                    }
                }

                public NestedFunction(MethodSymbol symbol, SyntaxReference blockSyntax)
                {
                    OriginalMethodSymbol = symbol;
                    BlockSyntax = blockSyntax;
                }

                public void Free()
                {
                    CapturedVariables.Free();
                    CapturedEnvironments.Free();
                }
            }

            public sealed class ClosureEnvironment
            {
                public readonly SetWithInsertionOrder<Symbol> CapturedVariables;

                public bool CapturesParent;

                public readonly bool IsStruct;

                internal SynthesizedClosureEnvironment SynthesizedEnvironment;

                public ClosureEnvironment(IEnumerable<Symbol> capturedVariables, bool isStruct)
                {
                    CapturedVariables = new SetWithInsertionOrder<Symbol>();
                    foreach (Symbol capturedVariable in capturedVariables)
                    {
                        CapturedVariables.Add(capturedVariable);
                    }
                    IsStruct = isStruct;
                }
            }

            private class ScopeTreeBuilder : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
            {
                private Scope _currentScope;

                private NestedFunction _currentFunction;

                private bool _inExpressionTree;

                private readonly SmallDictionary<Symbol, Scope> _localToScope = new SmallDictionary<Symbol, Scope>();

                private readonly MethodSymbol _topLevelMethod;

                private readonly HashSet<MethodSymbol> _methodsConvertedToDelegates;

                private readonly DiagnosticBag _diagnostics;

                private readonly PooledDictionary<LabelSymbol, ArrayBuilder<Scope>> _scopesAfterLabel = PooledDictionary<LabelSymbol, ArrayBuilder<Scope>>.GetInstance();

                private readonly ArrayBuilder<ArrayBuilder<LabelSymbol>> _labelsInScope = ArrayBuilder<ArrayBuilder<LabelSymbol>>.GetInstance();

                private ScopeTreeBuilder(Scope rootScope, MethodSymbol topLevelMethod, HashSet<MethodSymbol> methodsConvertedToDelegates, DiagnosticBag diagnostics)
                {
                    _currentScope = rootScope;
                    _labelsInScope.Push(ArrayBuilder<LabelSymbol>.GetInstance());
                    _topLevelMethod = topLevelMethod;
                    _methodsConvertedToDelegates = methodsConvertedToDelegates;
                    _diagnostics = diagnostics;
                }

                public static Scope Build(BoundNode node, MethodSymbol topLevelMethod, HashSet<MethodSymbol> methodsConvertedToDelegates, DiagnosticBag diagnostics)
                {
                    Scope scope = new Scope(null, node, null);
                    new ScopeTreeBuilder(scope, topLevelMethod, methodsConvertedToDelegates, diagnostics).Build();
                    return scope;
                }

                private void Build()
                {
                    DeclareLocals(_currentScope, _topLevelMethod.Parameters);
                    if (_topLevelMethod.TryGetThisParameter(out var thisParameter) && (object)thisParameter != null)
                    {
                        DeclareLocals(_currentScope, ImmutableArray.Create((Symbol)thisParameter));
                    }
                    Visit(_currentScope.BoundNode);
                    foreach (ArrayBuilder<Scope> value in _scopesAfterLabel.Values)
                    {
                        value.Free();
                    }
                    _scopesAfterLabel.Free();
                    _labelsInScope.Pop().Free();
                    _labelsInScope.Free();
                }

                public override BoundNode VisitMethodGroup(BoundMethodGroup node)
                {
                    throw ExceptionUtilities.Unreachable;
                }

                public override BoundNode VisitBlock(BoundBlock node)
                {
                    Scope currentScope = _currentScope;
                    PushOrReuseScope(node, node.Locals);
                    BoundNode? result = base.VisitBlock(node);
                    PopScope(currentScope);
                    return result;
                }

                public override BoundNode VisitCatchBlock(BoundCatchBlock node)
                {
                    Scope currentScope = _currentScope;
                    PushOrReuseScope(node, node.Locals);
                    BoundNode? result = base.VisitCatchBlock(node);
                    PopScope(currentScope);
                    return result;
                }

                public override BoundNode VisitSequence(BoundSequence node)
                {
                    Scope currentScope = _currentScope;
                    PushOrReuseScope(node, node.Locals);
                    BoundNode? result = base.VisitSequence(node);
                    PopScope(currentScope);
                    return result;
                }

                public override BoundNode VisitLambda(BoundLambda node)
                {
                    bool inExpressionTree = _inExpressionTree;
                    _inExpressionTree |= node.Type.IsExpressionTree();
                    _methodsConvertedToDelegates.Add(node.Symbol.OriginalDefinition);
                    BoundNode? result = VisitNestedFunction(node.Symbol, node.Body);
                    _inExpressionTree = inExpressionTree;
                    return result;
                }

                public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
                {
                    return VisitNestedFunction(node.Symbol.OriginalDefinition, node.Body);
                }

                public override BoundNode VisitCall(BoundCall node)
                {
                    if (node.Method.MethodKind == MethodKind.LocalFunction)
                    {
                        AddIfCaptured(node.Method.OriginalDefinition, node.Syntax);
                    }
                    return base.VisitCall(node);
                }

                public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
                {
                    MethodSymbol? methodOpt = node.MethodOpt;
                    if ((object)methodOpt != null && methodOpt!.MethodKind == MethodKind.LocalFunction)
                    {
                        MethodSymbol originalDefinition = node.MethodOpt!.OriginalDefinition;
                        AddIfCaptured(originalDefinition, node.Syntax);
                        _methodsConvertedToDelegates.Add(originalDefinition);
                    }
                    return base.VisitDelegateCreationExpression(node);
                }

                public override BoundNode VisitParameter(BoundParameter node)
                {
                    AddIfCaptured(node.ParameterSymbol, node.Syntax);
                    return base.VisitParameter(node);
                }

                public override BoundNode VisitLocal(BoundLocal node)
                {
                    AddIfCaptured(node.LocalSymbol, node.Syntax);
                    return base.VisitLocal(node);
                }

                public override BoundNode VisitBaseReference(BoundBaseReference node)
                {
                    AddIfCaptured(_topLevelMethod.ThisParameter, node.Syntax);
                    return base.VisitBaseReference(node);
                }

                public override BoundNode VisitThisReference(BoundThisReference node)
                {
                    ParameterSymbol thisParameter = _topLevelMethod.ThisParameter;
                    if (thisParameter != null)
                    {
                        AddIfCaptured(thisParameter, node.Syntax);
                    }
                    return base.VisitThisReference(node);
                }

                public override BoundNode VisitLabelStatement(BoundLabelStatement node)
                {
                    _labelsInScope.Peek().Add(node.Label);
                    _scopesAfterLabel.Add(node.Label, ArrayBuilder<Scope>.GetInstance());
                    return base.VisitLabelStatement(node);
                }

                public override BoundNode VisitGotoStatement(BoundGotoStatement node)
                {
                    CheckCanMergeWithParent(node.Label);
                    return base.VisitGotoStatement(node);
                }

                public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
                {
                    CheckCanMergeWithParent(node.Label);
                    return base.VisitConditionalGoto(node);
                }

                private void CheckCanMergeWithParent(LabelSymbol jumpTarget)
                {
                    if (_scopesAfterLabel.TryGetValue(jumpTarget, out var value))
                    {
                        ArrayBuilder<Scope>.Enumerator enumerator = value.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.CanMergeWithParent = false;
                        }
                        value.Clear();
                    }
                }

                private BoundNode? VisitNestedFunction(MethodSymbol functionSymbol, BoundBlock? body)
                {
                    if (body == null)
                    {
                        _currentScope.NestedFunctions.Add(new NestedFunction(functionSymbol, null));
                        return null;
                    }
                    NestedFunction nestedFunction = new NestedFunction(functionSymbol, body!.Syntax.GetReference());
                    _currentScope.NestedFunctions.Add(nestedFunction);
                    NestedFunction currentFunction = _currentFunction;
                    _currentFunction = nestedFunction;
                    Scope currentScope = _currentScope;
                    CreateAndPushScope(body);
                    DeclareLocals(_currentScope, functionSymbol.Parameters, _inExpressionTree);
                    BoundNode result = (_inExpressionTree ? base.VisitBlock(body) : VisitBlock(body));
                    PopScope(currentScope);
                    _currentFunction = currentFunction;
                    return result;
                }

                private void AddIfCaptured(Symbol symbol, SyntaxNode syntax)
                {
                    if (_currentFunction == null || (symbol is LocalSymbol localSymbol && localSymbol.IsConst) || (symbol is MethodSymbol methodSymbol && _currentFunction.OriginalMethodSymbol == methodSymbol) || !(symbol.ContainingSymbol != _currentFunction.OriginalMethodSymbol))
                    {
                        return;
                    }
                    AddDiagnosticIfRestrictedType(symbol, syntax);
                    Scope scope = _currentScope;
                    NestedFunction nestedFunction = _currentFunction;
                    while (nestedFunction != null && symbol.ContainingSymbol != nestedFunction.OriginalMethodSymbol)
                    {
                        nestedFunction.CapturedVariables.Add(symbol);
                        while (scope.ContainingFunctionOpt == nestedFunction)
                        {
                            scope = scope.Parent;
                        }
                        nestedFunction = scope.ContainingFunctionOpt;
                    }
                    if (symbol.Kind != SymbolKind.Method && _localToScope.TryGetValue(symbol, out var value))
                    {
                        value.DeclaredVariables.Add(symbol);
                    }
                }

                private void AddDiagnosticIfRestrictedType(Symbol capturedVariable, SyntaxNode syntax)
                {
                    TypeSymbol type;
                    switch (capturedVariable.Kind)
                    {
                        default:
                            return;
                        case SymbolKind.Local:
                            type = ((LocalSymbol)capturedVariable).Type;
                            break;
                        case SymbolKind.Parameter:
                            type = ((ParameterSymbol)capturedVariable).Type;
                            break;
                    }
                    if (type.IsRestrictedType())
                    {
                        _diagnostics.Add(ErrorCode.ERR_SpecialByRefInLambda, syntax.Location, type);
                    }
                }

                private void PushOrReuseScope<TSymbol>(BoundNode node, ImmutableArray<TSymbol> locals) where TSymbol : Symbol
                {
                    if (!locals.IsEmpty && _currentScope.BoundNode != node)
                    {
                        CreateAndPushScope(node);
                    }
                    DeclareLocals(_currentScope, locals);
                }

                private void CreateAndPushScope(BoundNode node)
                {
                    Scope scope = CreateNestedScope(_currentScope, _currentFunction);
                    ArrayBuilder<LabelSymbol>.Enumerator enumerator = _labelsInScope.Peek().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        LabelSymbol current = enumerator.Current;
                        _scopesAfterLabel[current].Add(scope);
                    }
                    _labelsInScope.Push(ArrayBuilder<LabelSymbol>.GetInstance());
                    _currentScope = scope;
                    Scope CreateNestedScope(Scope parentScope, NestedFunction currentFunction)
                    {
                        Scope scope2 = new Scope(parentScope, node, currentFunction);
                        parentScope.NestedScopes.Add(scope2);
                        return scope2;
                    }
                }

                private void PopScope(Scope scope)
                {
                    if (scope != _currentScope)
                    {
                        ArrayBuilder<LabelSymbol> arrayBuilder = _labelsInScope.Pop();
                        ArrayBuilder<LabelSymbol>.Enumerator enumerator = arrayBuilder.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            LabelSymbol current = enumerator.Current;
                            _scopesAfterLabel[current].Free();
                            _scopesAfterLabel.Remove(current);
                        }
                        arrayBuilder.Free();
                        _currentScope = _currentScope.Parent;
                    }
                }

                private void DeclareLocals<TSymbol>(Scope scope, ImmutableArray<TSymbol> locals, bool declareAsFree = false) where TSymbol : Symbol
                {
                    ImmutableArray<TSymbol>.Enumerator enumerator = locals.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        TSymbol current = enumerator.Current;
                        if (!declareAsFree)
                        {
                            _localToScope.Add(current, scope);
                        }
                    }
                }
            }

            public readonly PooledHashSet<MethodSymbol> MethodsConvertedToDelegates;

            public readonly Scope ScopeTree;

            private readonly MethodSymbol _topLevelMethod;

            private readonly int _topLevelMethodOrdinal;

            private readonly VariableSlotAllocator _slotAllocatorOpt;

            private readonly TypeCompilationState _compilationState;

            public bool CanTakeRefParameters(MethodSymbol function)
            {
                if (!function.IsAsync && !function.IsIterator)
                {
                    return !MethodsConvertedToDelegates.Contains(function);
                }
                return false;
            }

            private Analysis(Scope scopeTree, PooledHashSet<MethodSymbol> methodsConvertedToDelegates, MethodSymbol topLevelMethod, int topLevelMethodOrdinal, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState)
            {
                ScopeTree = scopeTree;
                MethodsConvertedToDelegates = methodsConvertedToDelegates;
                _topLevelMethod = topLevelMethod;
                _topLevelMethodOrdinal = topLevelMethodOrdinal;
                _slotAllocatorOpt = slotAllocatorOpt;
                _compilationState = compilationState;
            }

            public static Analysis Analyze(BoundNode node, MethodSymbol method, int topLevelMethodOrdinal, MethodSymbol substitutedSourceMethod, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, ArrayBuilder<ClosureDebugInfo> closureDebugInfo, DiagnosticBag diagnostics)
            {
                PooledHashSet<MethodSymbol> instance = PooledHashSet<MethodSymbol>.GetInstance();
                Analysis analysis = new Analysis(ScopeTreeBuilder.Build(node, method, instance, diagnostics), instance, method, topLevelMethodOrdinal, slotAllocatorOpt, compilationState);
                analysis.MakeAndAssignEnvironments();
                analysis.ComputeLambdaScopesAndFrameCaptures();
                if (compilationState.Compilation.Options.OptimizationLevel == OptimizationLevel.Release)
                {
                    analysis.MergeEnvironments();
                }
                analysis.InlineThisOnlyEnvironments();
                return analysis;
            }

            private static BoundNode FindNodeToAnalyze(BoundNode node)
            {
                while (true)
                {
                    switch (node.Kind)
                    {
                        case BoundKind.SequencePoint:
                            node = ((BoundSequencePoint)node).StatementOpt;
                            break;
                        case BoundKind.SequencePointWithSpan:
                            node = ((BoundSequencePointWithSpan)node).StatementOpt;
                            break;
                        case BoundKind.FieldEqualsValue:
                        case BoundKind.Block:
                        case BoundKind.StatementList:
                            return node;
                        case BoundKind.GlobalStatementInitializer:
                            return ((BoundGlobalStatementInitializer)node).Statement;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(node.Kind);
                    }
                }
            }

            private void ComputeLambdaScopesAndFrameCaptures()
            {
                VisitNestedFunctions(ScopeTree, delegate (Scope scope, NestedFunction function)
                {
                    if (function.CapturedEnvironments.Count > 0)
                    {
                        PooledHashSet<ClosureEnvironment> instance = PooledHashSet<ClosureEnvironment>.GetInstance();
                        instance.AddAll(function.CapturedEnvironments);
                        Scope scope2;
                        for (scope2 = scope; scope2 != null; scope2 = scope2.Parent)
                        {
                            ClosureEnvironment declaredEnvironment = scope2.DeclaredEnvironment;
                            if (declaredEnvironment != null && instance.Remove(declaredEnvironment) && !declaredEnvironment.IsStruct)
                            {
                                function.ContainingEnvironmentOpt = declaredEnvironment;
                                break;
                            }
                        }
                        ClosureEnvironment closureEnvironment = scope2?.DeclaredEnvironment;
                        scope2 = scope2?.Parent;
                        while (scope2 != null && instance.Count != 0)
                        {
                            ClosureEnvironment declaredEnvironment2 = scope2.DeclaredEnvironment;
                            if (declaredEnvironment2 != null)
                            {
                                if (!declaredEnvironment2.IsStruct)
                                {
                                    closureEnvironment.CapturesParent = true;
                                    closureEnvironment = declaredEnvironment2;
                                }
                                instance.Remove(declaredEnvironment2);
                            }
                            scope2 = scope2.Parent;
                        }
                        if (instance.Count > 0)
                        {
                            throw ExceptionUtilities.Unreachable;
                        }
                        instance.Free();
                    }
                });
            }

            private void InlineThisOnlyEnvironments()
            {
                if (!_topLevelMethod.TryGetThisParameter(out var thisParameter) || thisParameter == null)
                {
                    return;
                }
                ClosureEnvironment env = ScopeTree.DeclaredEnvironment;
                if (env == null || env.CapturedVariables.Count > 1 || !env.CapturedVariables.Contains(thisParameter))
                {
                    return;
                }
                if (env.IsStruct)
                {
                    if (!CheckNestedFunctions(ScopeTree, (Scope scope, NestedFunction closure) => closure.CapturedEnvironments.Contains(env) && closure.ContainingEnvironmentOpt != null))
                    {
                        RemoveEnv();
                    }
                }
                else
                {
                    if ((object)VarianceSafety.GetEnclosingVariantInterface(_topLevelMethod) != null)
                    {
                        return;
                    }
                    RemoveEnv();
                    VisitNestedFunctions(ScopeTree, delegate (Scope scope, NestedFunction closure)
                    {
                        if (closure.ContainingEnvironmentOpt == env)
                        {
                            closure.ContainingEnvironmentOpt = null;
                        }
                    });
                }
                void RemoveEnv()
                {
                    ScopeTree.DeclaredEnvironment = null;
                    VisitNestedFunctions(ScopeTree, delegate (Scope scope, NestedFunction nested)
                    {
                        int num = nested.CapturedEnvironments.IndexOf(env);
                        if (num >= 0)
                        {
                            nested.CapturedEnvironments.RemoveAt(num);
                        }
                    });
                }
            }

            private void MakeAndAssignEnvironments()
            {
                VisitScopeTree(ScopeTree, delegate (Scope scope)
                {
                    SetWithInsertionOrder<Symbol> declaredVariables = scope.DeclaredVariables;
                    if (declaredVariables.Count == 0)
                    {
                        return;
                    }
                    bool isStruct = (object)VarianceSafety.GetEnclosingVariantInterface(_topLevelMethod) == null;
                    SetWithInsertionOrder<NestedFunction> closures = new SetWithInsertionOrder<NestedFunction>();
                    bool addedItem;
                    do
                    {
                        addedItem = false;
                        VisitNestedFunctions(scope, delegate (Scope closureScope, NestedFunction closure)
                        {
                            if (!closures.Contains(closure) && (closure.CapturedVariables.Overlaps(scope.DeclaredVariables) || closure.CapturedVariables.Overlaps(closures.Select((NestedFunction c) => c.OriginalMethodSymbol))))
                            {
                                closures.Add(closure);
                                addedItem = true;
                                isStruct &= CanTakeRefParameters(closure.OriginalMethodSymbol);
                            }
                        });
                    }
                    while (addedItem);
                    ClosureEnvironment closureEnvironment = new ClosureEnvironment(declaredVariables, isStruct);
                    scope.DeclaredEnvironment = closureEnvironment;
                    _topLevelMethod.TryGetThisParameter(out var thisParameter);
                    foreach (NestedFunction item in closures)
                    {
                        item.CapturedEnvironments.Add(closureEnvironment);
                        if (thisParameter != null && closureEnvironment.CapturedVariables.Contains(thisParameter))
                        {
                            item.CapturesThis = true;
                        }
                    }
                });
            }

            private PooledDictionary<Scope, PooledHashSet<NestedFunction>> CalculateFunctionsCapturingScopeVariables()
            {
                PooledDictionary<Scope, PooledHashSet<NestedFunction>> closuresCapturingScopeVariables = PooledDictionary<Scope, PooledHashSet<NestedFunction>>.GetInstance();
                PooledDictionary<ClosureEnvironment, Scope> environmentsToScopes = PooledDictionary<ClosureEnvironment, Scope>.GetInstance();
                VisitScopeTree(ScopeTree, delegate (Scope scope)
                {
                    if (scope.DeclaredEnvironment != null)
                    {
                        closuresCapturingScopeVariables[scope] = PooledHashSet<NestedFunction>.GetInstance();
                        environmentsToScopes[scope.DeclaredEnvironment] = scope;
                    }
                    ArrayBuilder<NestedFunction>.Enumerator enumerator2 = scope.NestedFunctions.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NestedFunction current = enumerator2.Current;
                        ArrayBuilder<ClosureEnvironment>.Enumerator enumerator3 = current.CapturedEnvironments.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            ClosureEnvironment current2 = enumerator3.Current;
                            closuresCapturingScopeVariables[environmentsToScopes[current2]].Add(current);
                        }
                    }
                });
                environmentsToScopes.Free();
                foreach (var (scope3, values) in closuresCapturingScopeVariables)
                {
                    if (scope3.DeclaredEnvironment == null)
                    {
                        continue;
                    }
                    Scope scope4 = scope3;
                    while (scope4.DeclaredEnvironment == null || scope4.DeclaredEnvironment!.CapturesParent)
                    {
                        scope4 = scope4.Parent;
                        if (scope4 == null)
                        {
                            throw ExceptionUtilities.Unreachable;
                        }
                        if (scope4.DeclaredEnvironment != null && !scope4.DeclaredEnvironment!.IsStruct)
                        {
                            closuresCapturingScopeVariables[scope4].AddAll(values);
                        }
                    }
                }
                return closuresCapturingScopeVariables;
            }

            private void MergeEnvironments()
            {
                PooledDictionary<Scope, PooledHashSet<NestedFunction>> pooledDictionary = CalculateFunctionsCapturingScopeVariables();
                foreach (var (scope2, pooledHashSet2) in pooledDictionary)
                {
                    if (pooledHashSet2.Count == 0)
                    {
                        continue;
                    }
                    ClosureEnvironment declaredEnvironment = scope2.DeclaredEnvironment;
                    if (declaredEnvironment.IsStruct)
                    {
                        continue;
                    }
                    Scope scope3 = scope2;
                    Scope scope4 = scope2;
                    while (scope4.Parent != null && scope4.CanMergeWithParent)
                    {
                        Scope parent = scope4.Parent;
                        ClosureEnvironment declaredEnvironment2 = parent.DeclaredEnvironment;
                        if (declaredEnvironment2 == null || declaredEnvironment2.IsStruct)
                        {
                            scope4 = parent;
                            continue;
                        }
                        if (!pooledDictionary[parent].SetEquals(pooledHashSet2))
                        {
                            break;
                        }
                        scope3 = parent;
                        scope4 = parent;
                    }
                    if (scope3 == scope2)
                    {
                        continue;
                    }
                    ClosureEnvironment declaredEnvironment3 = scope3.DeclaredEnvironment;
                    foreach (Symbol capturedVariable in declaredEnvironment.CapturedVariables)
                    {
                        declaredEnvironment3.CapturedVariables.Add(capturedVariable);
                    }
                    scope2.DeclaredEnvironment = null;
                    foreach (NestedFunction item in pooledHashSet2)
                    {
                        item.CapturedEnvironments.Remove(declaredEnvironment);
                        if (!item.CapturedEnvironments.Contains(declaredEnvironment3))
                        {
                            item.CapturedEnvironments.Add(declaredEnvironment3);
                        }
                        if (item.ContainingEnvironmentOpt == declaredEnvironment)
                        {
                            item.ContainingEnvironmentOpt = declaredEnvironment3;
                        }
                    }
                }
                foreach (PooledHashSet<NestedFunction> value in pooledDictionary.Values)
                {
                    value.Free();
                }
                pooledDictionary.Free();
            }

            internal DebugId GetTopLevelMethodId()
            {
                return _slotAllocatorOpt?.MethodId ?? new DebugId(_topLevelMethodOrdinal, _compilationState.ModuleBuilderOpt!.CurrentGenerationOrdinal);
            }

            internal DebugId GetClosureId(SyntaxNode syntax, ArrayBuilder<ClosureDebugInfo> closureDebugInfo)
            {
                DebugId debugId = ((_slotAllocatorOpt == null || !_slotAllocatorOpt.TryGetPreviousClosure(syntax, out DebugId closureId)) ? new DebugId(closureDebugInfo.Count, _compilationState.ModuleBuilderOpt!.CurrentGenerationOrdinal) : closureId);
                int syntaxOffset = _topLevelMethod.CalculateLocalSyntaxOffset(LambdaUtilities.GetDeclaratorPosition(syntax), syntax.SyntaxTree);
                closureDebugInfo.Add(new ClosureDebugInfo(syntaxOffset, debugId));
                return debugId;
            }

            public static Scope GetVariableDeclarationScope(Scope startingScope, Symbol variable)
            {
                if (variable is ParameterSymbol parameterSymbol && parameterSymbol.IsThis)
                {
                    return null;
                }
                for (Scope scope = startingScope; scope != null; scope = scope.Parent)
                {
                    switch (variable.Kind)
                    {
                        case SymbolKind.Local:
                        case SymbolKind.Parameter:
                            if (scope.DeclaredVariables.Contains(variable))
                            {
                                return scope;
                            }
                            break;
                        case SymbolKind.Method:
                            {
                                ArrayBuilder<NestedFunction>.Enumerator enumerator = scope.NestedFunctions.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    if (enumerator.Current.OriginalMethodSymbol == variable)
                                    {
                                        return scope;
                                    }
                                }
                                break;
                            }
                        default:
                            throw ExceptionUtilities.UnexpectedValue(variable.Kind);
                    }
                }
                return null;
            }

            public static Scope GetScopeParent(Scope treeRoot, BoundNode scopeNode)
            {
                return GetScopeWithMatchingBoundNode(treeRoot, scopeNode).Parent;
            }

            public static Scope GetScopeWithMatchingBoundNode(Scope treeRoot, BoundNode node)
            {
                return Helper(treeRoot) ?? throw ExceptionUtilities.Unreachable;
                Scope Helper(Scope currentScope)
                {
                    if (currentScope.BoundNode == node)
                    {
                        return currentScope;
                    }
                    ArrayBuilder<Scope>.Enumerator enumerator = currentScope.NestedScopes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Scope scope = Helper(enumerator.Current);
                        if (scope != null)
                        {
                            return scope;
                        }
                    }
                    return null;
                }
            }

            public static (NestedFunction, Scope) GetVisibleNestedFunction(Scope startingScope, MethodSymbol functionSymbol)
            {
                for (Scope scope = startingScope; scope != null; scope = scope.Parent)
                {
                    ArrayBuilder<NestedFunction>.Enumerator enumerator = scope.NestedFunctions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        NestedFunction current = enumerator.Current;
                        if (current.OriginalMethodSymbol == functionSymbol)
                        {
                            return (current, scope);
                        }
                    }
                }
                throw ExceptionUtilities.Unreachable;
            }

            public static NestedFunction GetNestedFunctionInTree(Scope treeRoot, MethodSymbol functionSymbol)
            {
                return helper(treeRoot) ?? throw ExceptionUtilities.Unreachable;
                NestedFunction helper(Scope scope)
                {
                    ArrayBuilder<NestedFunction>.Enumerator enumerator = scope.NestedFunctions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        NestedFunction current = enumerator.Current;
                        if (current.OriginalMethodSymbol == functionSymbol)
                        {
                            return current;
                        }
                    }
                    ArrayBuilder<Scope>.Enumerator enumerator2 = scope.NestedScopes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NestedFunction nestedFunction = helper(enumerator2.Current);
                        if (nestedFunction != null)
                        {
                            return nestedFunction;
                        }
                    }
                    return null;
                }
            }

            public void Free()
            {
                MethodsConvertedToDelegates.Free();
                ScopeTree.Free();
            }

            public static void VisitNestedFunctions(Scope scope, Action<Scope, NestedFunction> action)
            {
                ArrayBuilder<NestedFunction>.Enumerator enumerator = scope.NestedFunctions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NestedFunction current = enumerator.Current;
                    action(scope, current);
                }
                ArrayBuilder<Scope>.Enumerator enumerator2 = scope.NestedScopes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    VisitNestedFunctions(enumerator2.Current, action);
                }
            }

            public static bool CheckNestedFunctions(Scope scope, Func<Scope, NestedFunction, bool> func)
            {
                ArrayBuilder<NestedFunction>.Enumerator enumerator = scope.NestedFunctions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NestedFunction current = enumerator.Current;
                    if (func(scope, current))
                    {
                        return true;
                    }
                }
                ArrayBuilder<Scope>.Enumerator enumerator2 = scope.NestedScopes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (CheckNestedFunctions(enumerator2.Current, func))
                    {
                        return true;
                    }
                }
                return false;
            }

            public static void VisitScopeTree(Scope treeRoot, Action<Scope> action)
            {
                action(treeRoot);
                ArrayBuilder<Scope>.Enumerator enumerator = treeRoot.NestedScopes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    VisitScopeTree(enumerator.Current, action);
                }
            }
        }

        private readonly Analysis _analysis;

        private readonly MethodSymbol _topLevelMethod;

        private readonly MethodSymbol _substitutedSourceMethod;

        private readonly int _topLevelMethodOrdinal;

        private SynthesizedClosureEnvironment _lazyStaticLambdaFrame;

        private readonly Dictionary<ParameterSymbol, ParameterSymbol> _parameterMap = new Dictionary<ParameterSymbol, ParameterSymbol>();

        private readonly Dictionary<BoundNode, Analysis.ClosureEnvironment> _frames = new Dictionary<BoundNode, Analysis.ClosureEnvironment>();

        private readonly Dictionary<NamedTypeSymbol, Symbol> _framePointers = new Dictionary<NamedTypeSymbol, Symbol>();

        private readonly HashSet<LocalSymbol> _assignLocals;

        private MethodSymbol _currentMethod;

        private ParameterSymbol _currentFrameThis;

        private readonly ArrayBuilder<LambdaDebugInfo> _lambdaDebugInfoBuilder;

        private int _synthesizedFieldNameIdDispenser;

        private Symbol _innermostFramePointer;

        private TypeMap _currentLambdaBodyTypeMap;

        private ImmutableArray<TypeParameterSymbol> _currentTypeParameters;

        private BoundExpression _thisProxyInitDeferred;

        private bool _seenBaseCall;

        private bool _inExpressionLambda;

        private ArrayBuilder<LocalSymbol> _addedLocals;

        private ArrayBuilder<BoundStatement> _addedStatements;

        private ArrayBuilder<TypeCompilationState.MethodWithBody> _synthesizedMethods;

        private readonly ImmutableHashSet<Symbol> _allCapturedVariables;

        protected override TypeMap TypeMap => _currentLambdaBodyTypeMap;

        protected override MethodSymbol CurrentMethod => _currentMethod;

        protected override NamedTypeSymbol ContainingType => _topLevelMethod.ContainingType;

        private ClosureConversion(Analysis analysis, NamedTypeSymbol thisType, ParameterSymbol thisParameterOpt, MethodSymbol method, int methodOrdinal, MethodSymbol substitutedSourceMethod, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, HashSet<LocalSymbol> assignLocals)
            : base(slotAllocatorOpt, compilationState, diagnostics)
        {
            _topLevelMethod = method;
            _substitutedSourceMethod = substitutedSourceMethod;
            _topLevelMethodOrdinal = methodOrdinal;
            _lambdaDebugInfoBuilder = lambdaDebugInfoBuilder;
            _currentMethod = method;
            _analysis = analysis;
            _assignLocals = assignLocals;
            _currentTypeParameters = method.TypeParameters;
            _currentLambdaBodyTypeMap = TypeMap.Empty;
            _innermostFramePointer = (_currentFrameThis = thisParameterOpt);
            _framePointers[thisType] = thisParameterOpt;
            _seenBaseCall = method.MethodKind != MethodKind.Constructor;
            _synthesizedFieldNameIdDispenser = 1;
            ImmutableHashSet<Symbol>.Builder allCapturedVars = ImmutableHashSet.CreateBuilder<Symbol>();
            Analysis.VisitNestedFunctions(analysis.ScopeTree, delegate (Analysis.Scope scope, Analysis.NestedFunction function)
            {
                allCapturedVars.UnionWith(function.CapturedVariables);
            });
            _allCapturedVariables = allCapturedVars.ToImmutable();
        }

        protected override bool NeedsProxy(Symbol localOrParameter)
        {
            return _allCapturedVariables.Contains(localOrParameter);
        }

        public static BoundStatement Rewrite(BoundStatement loweredBody, NamedTypeSymbol thisType, ParameterSymbol thisParameter, MethodSymbol method, int methodOrdinal, MethodSymbol substitutedSourceMethod, ArrayBuilder<LambdaDebugInfo> lambdaDebugInfoBuilder, ArrayBuilder<ClosureDebugInfo> closureDebugInfoBuilder, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, HashSet<LocalSymbol> assignLocals)
        {
            Analysis analysis = Analysis.Analyze(loweredBody, method, methodOrdinal, substitutedSourceMethod, slotAllocatorOpt, compilationState, closureDebugInfoBuilder, diagnostics.DiagnosticBag);
            ClosureConversion closureConversion = new ClosureConversion(analysis, thisType, thisParameter, method, methodOrdinal, substitutedSourceMethod, lambdaDebugInfoBuilder, slotAllocatorOpt, compilationState, diagnostics, assignLocals);
            closureConversion.SynthesizeClosureEnvironments(closureDebugInfoBuilder);
            closureConversion.SynthesizeClosureMethods();
            BoundStatement result = closureConversion.AddStatementsIfNeeded((BoundStatement)closureConversion.Visit(loweredBody));
            if (closureConversion._synthesizedMethods != null)
            {
                if (compilationState.SynthesizedMethods == null)
                {
                    compilationState.SynthesizedMethods = closureConversion._synthesizedMethods;
                }
                else
                {
                    compilationState.SynthesizedMethods!.AddRange(closureConversion._synthesizedMethods);
                    closureConversion._synthesizedMethods.Free();
                }
            }
            analysis.Free();
            return result;
        }

        private BoundStatement AddStatementsIfNeeded(BoundStatement body)
        {
            if (_addedLocals != null)
            {
                _addedStatements.Add(body);
                body = new BoundBlock(body.Syntax, _addedLocals.ToImmutableAndFree(), _addedStatements.ToImmutableAndFree())
                {
                    WasCompilerGenerated = true
                };
                _addedLocals = null;
                _addedStatements = null;
            }
            return body;
        }

        private void SynthesizeClosureEnvironments(ArrayBuilder<ClosureDebugInfo> closureDebugInfo)
        {
            Analysis.VisitScopeTree(_analysis.ScopeTree, delegate (Analysis.Scope scope)
            {
                Analysis.ClosureEnvironment declaredEnvironment = scope.DeclaredEnvironment;
                if (declaredEnvironment != null)
                {
                    SynthesizedClosureEnvironment synthesizedClosureEnvironment2 = (declaredEnvironment.SynthesizedEnvironment = MakeFrame(scope, declaredEnvironment));
                    CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(ContainingType, synthesizedClosureEnvironment2.GetCciAdapter());
                    if (synthesizedClosureEnvironment2.Constructor != null)
                    {
                        AddSynthesizedMethod(synthesizedClosureEnvironment2.Constructor, FlowAnalysisPass.AppendImplicitReturn(MethodCompiler.BindMethodBody(synthesizedClosureEnvironment2.Constructor, CompilationState, Diagnostics), synthesizedClosureEnvironment2.Constructor));
                    }
                    _frames.Add(scope.BoundNode, declaredEnvironment);
                }
            });
            SynthesizedClosureEnvironment MakeFrame(Analysis.Scope scope, Analysis.ClosureEnvironment env)
            {
                SyntaxNode syntax = scope.BoundNode.Syntax;
                DebugId topLevelMethodId = _analysis.GetTopLevelMethodId();
                DebugId closureId = _analysis.GetClosureId(syntax, closureDebugInfo);
                MethodSymbol methodSymbol = scope.ContainingFunctionOpt?.OriginalMethodSymbol ?? _topLevelMethod;
                if ((object)_substitutedSourceMethod != null && methodSymbol == _topLevelMethod)
                {
                    methodSymbol = _substitutedSourceMethod;
                }
                SynthesizedClosureEnvironment synthesizedClosureEnvironment = new SynthesizedClosureEnvironment(_topLevelMethod, methodSymbol, env.IsStruct, syntax, topLevelMethodId, closureId);
                foreach (Symbol capturedVariable in env.CapturedVariables)
                {
                    LambdaCapturedVariable lambdaCapturedVariable = LambdaCapturedVariable.Create(synthesizedClosureEnvironment, capturedVariable, ref _synthesizedFieldNameIdDispenser);
                    proxies.Add(capturedVariable, new CapturedToFrameSymbolReplacement(lambdaCapturedVariable, isReusable: false));
                    synthesizedClosureEnvironment.AddHoistedField(lambdaCapturedVariable);
                    CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(synthesizedClosureEnvironment, lambdaCapturedVariable.GetCciAdapter());
                }
                return synthesizedClosureEnvironment;
            }
        }

        private void SynthesizeClosureMethods()
        {
            Analysis.VisitNestedFunctions(_analysis.ScopeTree, delegate (Analysis.Scope scope, Analysis.NestedFunction nestedFunction)
            {
                MethodSymbol originalMethodSymbol = nestedFunction.OriginalMethodSymbol;
                SyntaxNode syntax = originalMethodSymbol.DeclaringSyntaxReferences[0].GetSyntax();
                ClosureKind closureKind;
                NamedTypeSymbol containingType;
                int closureOrdinal;
                if (nestedFunction.ContainingEnvironmentOpt != null)
                {
                    SynthesizedClosureEnvironment synthesizedEnvironment = nestedFunction.ContainingEnvironmentOpt.SynthesizedEnvironment;
                    closureKind = ClosureKind.General;
                    containingType = synthesizedEnvironment;
                    closureOrdinal = synthesizedEnvironment.ClosureOrdinal;
                }
                else if (nestedFunction.CapturesThis)
                {
                    containingType = _topLevelMethod.ContainingType;
                    closureKind = ClosureKind.ThisOnly;
                    closureOrdinal = -2;
                }
                else if ((nestedFunction.CapturedEnvironments.Count == 0 && originalMethodSymbol.MethodKind == MethodKind.AnonymousFunction && _analysis.MethodsConvertedToDelegates.Contains(originalMethodSymbol)) || (object)VarianceSafety.GetEnclosingVariantInterface(_topLevelMethod) != null)
                {
                    containingType = GetStaticFrame(Diagnostics, syntax);
                    closureKind = ClosureKind.Singleton;
                    closureOrdinal = -1;
                }
                else
                {
                    containingType = _topLevelMethod.ContainingType;
                    closureKind = ClosureKind.Static;
                    closureOrdinal = -1;
                }
                DebugId topLevelMethodId = _analysis.GetTopLevelMethodId();
                DebugId lambdaId = GetLambdaId(syntax, closureKind, closureOrdinal);
                SynthesizedClosureMethod synthesizedClosureMethod = (nestedFunction.SynthesizedLoweredMethod = new SynthesizedClosureMethod(containingType, getStructEnvironments(nestedFunction), closureKind, _topLevelMethod, topLevelMethodId, originalMethodSymbol, nestedFunction.BlockSyntax, lambdaId));
            });
            static ImmutableArray<SynthesizedClosureEnvironment> getStructEnvironments(Analysis.NestedFunction function)
            {
                ArrayBuilder<SynthesizedClosureEnvironment> instance = ArrayBuilder<SynthesizedClosureEnvironment>.GetInstance();
                ArrayBuilder<Analysis.ClosureEnvironment>.Enumerator enumerator = function.CapturedEnvironments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Analysis.ClosureEnvironment current = enumerator.Current;
                    if (current.IsStruct)
                    {
                        instance.Add(current.SynthesizedEnvironment);
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        private SynthesizedClosureEnvironment GetStaticFrame(BindingDiagnosticBag diagnostics, SyntaxNode syntax)
        {
            if ((object)_lazyStaticLambdaFrame == null)
            {
                bool flag = !_topLevelMethod.IsGenericMethod;
                if (flag)
                {
                    _lazyStaticLambdaFrame = CompilationState.StaticLambdaFrame;
                }
                if ((object)_lazyStaticLambdaFrame == null)
                {
                    DebugId methodId = ((!flag) ? _analysis.GetTopLevelMethodId() : new DebugId(-1, CompilationState.ModuleBuilderOpt!.CurrentGenerationOrdinal));
                    DebugId closureId = default(DebugId);
                    MethodSymbol containingMethod = (flag ? null : (_substitutedSourceMethod ?? _topLevelMethod));
                    _lazyStaticLambdaFrame = new SynthesizedClosureEnvironment(_topLevelMethod, containingMethod, isStruct: false, null, methodId, closureId);
                    if (flag)
                    {
                        CompilationState.StaticLambdaFrame = _lazyStaticLambdaFrame;
                    }
                    SynthesizedClosureEnvironment lazyStaticLambdaFrame = _lazyStaticLambdaFrame;
                    CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(ContainingType, lazyStaticLambdaFrame.GetCciAdapter());
                    AddSynthesizedMethod(lazyStaticLambdaFrame.Constructor, FlowAnalysisPass.AppendImplicitReturn(MethodCompiler.BindMethodBody(lazyStaticLambdaFrame.Constructor, CompilationState, diagnostics), lazyStaticLambdaFrame.Constructor));
                    SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(lazyStaticLambdaFrame.StaticConstructor, syntax, CompilationState, diagnostics);
                    BoundBlock body = syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Field(null, lazyStaticLambdaFrame.SingletonCache), syntheticBoundNodeFactory.New(lazyStaticLambdaFrame.Constructor)), new BoundReturnStatement(syntax, RefKind.None, null));
                    AddSynthesizedMethod(lazyStaticLambdaFrame.StaticConstructor, body);
                }
            }
            return _lazyStaticLambdaFrame;
        }

        private BoundExpression FrameOfType(SyntaxNode syntax, NamedTypeSymbol frameType)
        {
            return FramePointer(syntax, frameType.OriginalDefinition);
        }

        protected override BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass)
        {
            if ((object)_currentFrameThis != null && TypeSymbol.Equals(_currentFrameThis.Type, frameClass, TypeCompareKind.ConsiderEverything))
            {
                return new BoundThisReference(syntax, frameClass);
            }
            SynthesizedClosureMethod synthesizedClosureMethod = _currentMethod as SynthesizedClosureMethod;
            if (synthesizedClosureMethod != null)
            {
                for (int i = synthesizedClosureMethod.ParameterCount - synthesizedClosureMethod.ExtraSynthesizedParameterCount; i < synthesizedClosureMethod.ParameterCount; i++)
                {
                    ParameterSymbol parameterSymbol = synthesizedClosureMethod.Parameters[i];
                    if (TypeSymbol.Equals(parameterSymbol.Type.OriginalDefinition, frameClass, TypeCompareKind.ConsiderEverything))
                    {
                        return new BoundParameter(syntax, parameterSymbol);
                    }
                }
            }
            Symbol symbol = _framePointers[frameClass];
            if (proxies.TryGetValue(symbol, out var value))
            {
                return value.Replacement(syntax, (NamedTypeSymbol frameType) => FramePointer(syntax, frameType));
            }
            LocalSymbol localSymbol = (LocalSymbol)symbol;
            return new BoundLocal(syntax, localSymbol, null, localSymbol.Type);
        }

        private static void InsertAndFreePrologue<T>(ArrayBuilder<BoundStatement> result, ArrayBuilder<T> prologue) where T : BoundNode
        {
            ArrayBuilder<T>.Enumerator enumerator = prologue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (current is BoundStatement item)
                {
                    result.Add(item);
                }
                else
                {
                    result.Add(new BoundExpressionStatement(current.Syntax, (BoundExpression)(object)current));
                }
            }
            prologue.Free();
        }

        private BoundNode IntroduceFrame(BoundNode node, Analysis.ClosureEnvironment env, Func<ArrayBuilder<BoundExpression>, ArrayBuilder<LocalSymbol>, BoundNode> F)
        {
            SynthesizedClosureEnvironment synthesizedEnvironment = env.SynthesizedEnvironment;
            ImmutableArray<TypeWithAnnotations> typeArguments = ImmutableArray.Create(_currentTypeParameters.SelectAsArray((TypeParameterSymbol t) => TypeWithAnnotations.Create(t)), 0, synthesizedEnvironment.Arity);
            NamedTypeSymbol namedTypeSymbol = synthesizedEnvironment.ConstructIfGeneric(typeArguments);
            LocalSymbol localSymbol = new SynthesizedLocal(_topLevelMethod, TypeWithAnnotations.Create(namedTypeSymbol), SynthesizedLocalKind.LambdaDisplayClass, synthesizedEnvironment.ScopeSyntaxOpt);
            SyntaxNode syntax = node.Syntax;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            if ((object)synthesizedEnvironment.Constructor != null)
            {
                MethodSymbol constructor = synthesizedEnvironment.Constructor.AsMember(namedTypeSymbol);
                instance.Add(new BoundAssignmentOperator(syntax, new BoundLocal(syntax, localSymbol, null, namedTypeSymbol), new BoundObjectCreationExpression(syntax, constructor), namedTypeSymbol));
            }
            CapturedSymbolReplacement value = null;
            if ((object)_innermostFramePointer != null)
            {
                proxies.TryGetValue(_innermostFramePointer, out value);
                if (env.CapturesParent)
                {
                    LambdaCapturedVariable lambdaCapturedVariable = LambdaCapturedVariable.Create(synthesizedEnvironment, _innermostFramePointer, ref _synthesizedFieldNameIdDispenser);
                    FieldSymbol fieldSymbol = lambdaCapturedVariable.AsMember(namedTypeSymbol);
                    BoundExpression boundExpression = new BoundFieldAccess(syntax, new BoundLocal(syntax, localSymbol, null, namedTypeSymbol), fieldSymbol, null);
                    BoundExpression right = FrameOfType(syntax, fieldSymbol.Type as NamedTypeSymbol);
                    BoundExpression item = new BoundAssignmentOperator(syntax, boundExpression, right, boundExpression.Type);
                    instance.Add(item);
                    if (CompilationState.Emitting)
                    {
                        synthesizedEnvironment.AddHoistedField(lambdaCapturedVariable);
                        CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(synthesizedEnvironment, lambdaCapturedVariable.GetCciAdapter());
                    }
                    proxies[_innermostFramePointer] = new CapturedToFrameSymbolReplacement(lambdaCapturedVariable, isReusable: false);
                }
            }
            foreach (Symbol capturedVariable in env.CapturedVariables)
            {
                InitVariableProxy(syntax, capturedVariable, localSymbol, instance);
            }
            Symbol innermostFramePointer = _innermostFramePointer;
            if (!localSymbol.Type.IsValueType)
            {
                _innermostFramePointer = localSymbol;
            }
            ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance();
            instance2.Add(localSymbol);
            _framePointers.Add(synthesizedEnvironment, localSymbol);
            BoundNode result = F(instance, instance2);
            _innermostFramePointer = innermostFramePointer;
            if ((object)_innermostFramePointer != null)
            {
                if (value != null)
                {
                    proxies[_innermostFramePointer] = value;
                    return result;
                }
                proxies.Remove(_innermostFramePointer);
            }
            return result;
        }

        private void InitVariableProxy(SyntaxNode syntax, Symbol symbol, LocalSymbol framePointer, ArrayBuilder<BoundExpression> prologue)
        {
            if (!proxies.TryGetValue(symbol, out var value))
            {
                return;
            }
            BoundExpression boundExpression;
            switch (symbol.Kind)
            {
                case SymbolKind.Parameter:
                    {
                        ParameterSymbol parameterSymbol = (ParameterSymbol)symbol;
                        if (!_parameterMap.TryGetValue(parameterSymbol, out var value3))
                        {
                            value3 = parameterSymbol;
                        }
                        boundExpression = new BoundParameter(syntax, value3);
                        break;
                    }
                case SymbolKind.Local:
                    {
                        LocalSymbol localSymbol = (LocalSymbol)symbol;
                        if (_assignLocals == null || !_assignLocals.Contains(localSymbol))
                        {
                            return;
                        }
                        if (!localMap.TryGetValue(localSymbol, out var value2))
                        {
                            value2 = localSymbol;
                        }
                        boundExpression = new BoundLocal(syntax, value2, null, value2.Type);
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
            }
            BoundExpression left = value.Replacement(syntax, (NamedTypeSymbol frameType1) => new BoundLocal(syntax, framePointer, null, framePointer.Type));
            BoundAssignmentOperator boundAssignmentOperator = new BoundAssignmentOperator(syntax, left, boundExpression, boundExpression.Type);
            if (_currentMethod.MethodKind == MethodKind.Constructor && symbol == _currentMethod.ThisParameter && !_seenBaseCall)
            {
                _thisProxyInitDeferred = boundAssignmentOperator;
            }
            else
            {
                prologue.Add(boundAssignmentOperator);
            }
        }

        protected override BoundNode VisitUnhoistedParameter(BoundParameter node)
        {
            if (_parameterMap.TryGetValue(node.ParameterSymbol, out var value))
            {
                return new BoundParameter(node.Syntax, value, node.HasErrors);
            }
            return base.VisitUnhoistedParameter(node);
        }

        public override BoundNode VisitThisReference(BoundThisReference node)
        {
            if (!(_currentMethod == _topLevelMethod) && !(_topLevelMethod.ThisParameter == null))
            {
                return FramePointer(node.Syntax, (NamedTypeSymbol)node.Type);
            }
            return node;
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            if (_currentMethod.IsStatic || !TypeSymbol.Equals(_currentMethod.ContainingType, _topLevelMethod.ContainingType, TypeCompareKind.ConsiderEverything))
            {
                return FramePointer(node.Syntax, _topLevelMethod.ContainingType);
            }
            return node;
        }

        private void RemapLocalFunction(SyntaxNode syntax, MethodSymbol localFunc, out BoundExpression receiver, out MethodSymbol method, ref ImmutableArray<BoundExpression> arguments, ref ImmutableArray<RefKind> argRefKinds)
        {
            SynthesizedClosureMethod synthesizedLoweredMethod = Analysis.GetNestedFunctionInTree(_analysis.ScopeTree, localFunc.OriginalDefinition).SynthesizedLoweredMethod;
            int extraSynthesizedParameterCount = synthesizedLoweredMethod.ExtraSynthesizedParameterCount;
            if (extraSynthesizedParameterCount != 0)
            {
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(synthesizedLoweredMethod.ParameterCount);
                instance.AddRange(arguments);
                for (int i = synthesizedLoweredMethod.ParameterCount - extraSynthesizedParameterCount; i < synthesizedLoweredMethod.ParameterCount; i++)
                {
                    NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)synthesizedLoweredMethod.Parameters[i].Type.OriginalDefinition;
                    if (namedTypeSymbol.Arity > 0)
                    {
                        ImmutableArray<TypeParameterSymbol> constructedFromTypeParameters = ((SynthesizedClosureEnvironment)namedTypeSymbol).ConstructedFromTypeParameters;
                        ImmutableArray<TypeParameterSymbol> immutableArray = TypeMap.SubstituteTypeParameters(constructedFromTypeParameters);
                        namedTypeSymbol = namedTypeSymbol.Construct(immutableArray);
                    }
                    BoundExpression item = FrameOfType(syntax, namedTypeSymbol);
                    instance.Add(item);
                }
                ArrayBuilder<RefKind> instance2 = ArrayBuilder<RefKind>.GetInstance(instance.Count);
                if (!argRefKinds.IsDefault)
                {
                    instance2.AddRange(argRefKinds);
                }
                else
                {
                    instance2.AddMany(RefKind.None, arguments.Length);
                }
                instance2.AddMany(RefKind.Ref, extraSynthesizedParameterCount);
                arguments = instance.ToImmutableAndFree();
                argRefKinds = instance2.ToImmutableAndFree();
            }
            method = synthesizedLoweredMethod;
            RemapLambdaOrLocalFunction(syntax, localFunc, SubstituteTypeArguments(localFunc.TypeArgumentsWithAnnotations), synthesizedLoweredMethod.ClosureKind, ref method, out receiver, out var _);
        }

        private ImmutableArray<TypeWithAnnotations> SubstituteTypeArguments(ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            if (typeArguments.IsEmpty)
            {
                return typeArguments;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(typeArguments.Length);
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeWithAnnotations typeWithAnnotations = enumerator.Current;
                TypeWithAnnotations previous;
                do
                {
                    previous = typeWithAnnotations;
                    typeWithAnnotations = TypeMap.SubstituteType(previous);
                }
                while (!TypeSymbol.Equals(previous.Type, typeWithAnnotations.Type, TypeCompareKind.ConsiderEverything));
                instance.Add(typeWithAnnotations);
            }
            return instance.ToImmutableAndFree();
        }

        private void RemapLambdaOrLocalFunction(SyntaxNode syntax, MethodSymbol originalMethod, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, ClosureKind closureKind, ref MethodSymbol synthesizedMethod, out BoundExpression receiver, out NamedTypeSymbol constructedFrame)
        {
            NamedTypeSymbol containingType = synthesizedMethod.ContainingType;
            SynthesizedClosureEnvironment synthesizedClosureEnvironment = containingType as SynthesizedClosureEnvironment;
            int num = (synthesizedClosureEnvironment?.Arity ?? 0) + synthesizedMethod.Arity;
            ImmutableArray<TypeWithAnnotations> immutableArray = ImmutableArray.Create(_currentTypeParameters.SelectAsArray((TypeParameterSymbol t) => TypeWithAnnotations.Create(t)), 0, num - originalMethod.Arity);
            if (!typeArgumentsOpt.IsDefault)
            {
                immutableArray = immutableArray.Concat(typeArgumentsOpt);
            }
            if ((object)synthesizedClosureEnvironment != null && synthesizedClosureEnvironment.Arity != 0)
            {
                ImmutableArray<TypeWithAnnotations> typeArguments = ImmutableArray.Create(immutableArray, 0, synthesizedClosureEnvironment.Arity);
                immutableArray = ImmutableArray.Create(immutableArray, synthesizedClosureEnvironment.Arity, immutableArray.Length - synthesizedClosureEnvironment.Arity);
                constructedFrame = synthesizedClosureEnvironment.Construct(typeArguments);
            }
            else
            {
                constructedFrame = containingType;
            }
            synthesizedMethod = synthesizedMethod.AsMember(constructedFrame);
            if (synthesizedMethod.IsGenericMethod)
            {
                synthesizedMethod = synthesizedMethod.Construct(immutableArray);
            }
            switch (closureKind)
            {
                case ClosureKind.Singleton:
                    {
                        FieldSymbol fieldSymbol = synthesizedClosureEnvironment.SingletonCache.AsMember(constructedFrame);
                        receiver = new BoundFieldAccess(syntax, null, fieldSymbol, null);
                        break;
                    }
                case ClosureKind.Static:
                    receiver = new BoundTypeExpression(syntax, null, synthesizedMethod.ContainingType);
                    break;
                default:
                    receiver = FrameOfType(syntax, constructedFrame);
                    break;
            }
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            if (node.Method.MethodKind == MethodKind.LocalFunction)
            {
                ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
                ImmutableArray<RefKind> argRefKinds = node.ArgumentRefKindsOpt;
                TypeSymbol type = VisitType(node.Type);
                RemapLocalFunction(node.Syntax, node.Method, out var receiver, out var method, ref arguments, ref argRefKinds);
                return node.Update(receiver, method, arguments, node.ArgumentNamesOpt, argRefKinds, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, type);
            }
            BoundNode boundNode = base.VisitCall(node);
            if (boundNode.Kind != BoundKind.Call)
            {
                return boundNode;
            }
            BoundCall boundCall = (BoundCall)boundNode;
            if (!_seenBaseCall && _currentMethod == _topLevelMethod && node.IsConstructorInitializer())
            {
                _seenBaseCall = true;
                if (_thisProxyInitDeferred != null)
                {
                    return new BoundSequence(node.Syntax, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create((BoundExpression)boundCall), _thisProxyInitDeferred, boundCall.Type);
                }
            }
            return boundCall;
        }

        private BoundSequence RewriteSequence(BoundSequence node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
        {
            RewriteLocals(node.Locals, newLocals);
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.SideEffects.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                BoundExpression boundExpression = (BoundExpression)Visit(current);
                if (boundExpression != null)
                {
                    prologue.Add(boundExpression);
                }
            }
            BoundExpression value = (BoundExpression)Visit(node.Value);
            TypeSymbol type = VisitType(node.Type);
            return node.Update(newLocals.ToImmutableAndFree(), prologue.ToImmutableAndFree(), value, type);
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            if (_frames.TryGetValue(node, out var value))
            {
                return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteBlock(node, prologue, newLocals));
            }
            return RewriteBlock(node, ArrayBuilder<BoundExpression>.GetInstance(), ArrayBuilder<LocalSymbol>.GetInstance());
        }

        private BoundBlock RewriteBlock(BoundBlock node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
        {
            RewriteLocals(node.Locals, newLocals);
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            if (prologue.Count > 0)
            {
                instance.Add(BoundSequencePoint.CreateHidden());
            }
            InsertAndFreePrologue(instance, prologue);
            ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundStatement current = enumerator.Current;
                BoundStatement boundStatement = (BoundStatement)Visit(current);
                if (boundStatement != null)
                {
                    instance.Add(boundStatement);
                }
            }
            return node.Update(newLocals.ToImmutableAndFree(), node.LocalFunctions, instance.ToImmutableAndFree());
        }

        public override BoundNode VisitScope(BoundScope node)
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            RewriteLocals(node.Locals, instance);
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            if (instance.Count == 0)
            {
                instance.Free();
                return new BoundStatementList(node.Syntax, statements);
            }
            return node.Update(instance.ToImmutableAndFree(), statements);
        }

        public override BoundNode VisitCatchBlock(BoundCatchBlock node)
        {
            if (_frames.TryGetValue(node, out var value))
            {
                return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteCatch(node, prologue, newLocals));
            }
            return RewriteCatch(node, ArrayBuilder<BoundExpression>.GetInstance(), ArrayBuilder<LocalSymbol>.GetInstance());
        }

        private BoundNode RewriteCatch(BoundCatchBlock node, ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
        {
            RewriteLocals(node.Locals, newLocals);
            ImmutableArray<LocalSymbol> locals = newLocals.ToImmutableAndFree();
            BoundExpression boundExpression = null;
            BoundStatementList boundStatementList = (BoundStatementList)Visit(node.ExceptionFilterPrologueOpt);
            BoundExpression boundExpression2 = (BoundExpression)Visit(node.ExceptionFilterOpt);
            if (node.ExceptionSourceOpt != null)
            {
                boundExpression = (BoundExpression)Visit(node.ExceptionSourceOpt);
                if (prologue.Count > 0)
                {
                    boundExpression = new BoundSequence(boundExpression.Syntax, ImmutableArray.Create<LocalSymbol>(), prologue.ToImmutable(), boundExpression, boundExpression.Type);
                }
            }
            else if (prologue.Count > 0)
            {
                ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance(prologue.Count);
                ArrayBuilder<BoundExpression>.Enumerator enumerator = prologue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundExpression current = enumerator.Current;
                    instance.Add(new BoundExpressionStatement(current.Syntax, current)
                    {
                        WasCompilerGenerated = true
                    });
                }
                if (boundStatementList != null)
                {
                    instance.AddRange(boundStatementList.Statements);
                }
                boundStatementList = new BoundStatementList(boundExpression2.Syntax, instance.ToImmutableAndFree());
            }
            prologue.Free();
            TypeSymbol exceptionTypeOpt = VisitType(node.ExceptionTypeOpt);
            BoundBlock body = (BoundBlock)Visit(node.Body);
            return node.Update(locals, boundExpression, exceptionTypeOpt, boundStatementList, boundExpression2, body, node.IsSynthesizedAsyncCatchAll);
        }

        public override BoundNode VisitSequence(BoundSequence node)
        {
            if (_frames.TryGetValue(node, out var value))
            {
                return IntroduceFrame(node, value, (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals) => RewriteSequence(node, prologue, newLocals));
            }
            return RewriteSequence(node, ArrayBuilder<BoundExpression>.GetInstance(), ArrayBuilder<LocalSymbol>.GetInstance());
        }

        public override BoundNode VisitStatementList(BoundStatementList node)
        {
            if (_frames.TryGetValue(node, out var value))
            {
                return IntroduceFrame(node, value, delegate (ArrayBuilder<BoundExpression> prologue, ArrayBuilder<LocalSymbol> newLocals)
                {
                    ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
                    InsertAndFreePrologue(instance, prologue);
                    ImmutableArray<BoundStatement>.Enumerator enumerator = node.Statements.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        BoundStatement current = enumerator.Current;
                        instance.Add((BoundStatement)Visit(current));
                    }
                    return new BoundBlock(node.Syntax, newLocals.ToImmutableAndFree(), instance.ToImmutableAndFree(), node.HasErrors);
                });
            }
            return base.VisitStatementList(node);
        }

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument.Kind == BoundKind.Lambda)
            {
                return RewriteLambdaConversion((BoundLambda)node.Argument);
            }
            MethodSymbol? methodOpt = node.MethodOpt;
            if ((object)methodOpt != null && methodOpt!.MethodKind == MethodKind.LocalFunction)
            {
                ImmutableArray<BoundExpression> arguments = default(ImmutableArray<BoundExpression>);
                ImmutableArray<RefKind> argRefKinds = default(ImmutableArray<RefKind>);
                RemapLocalFunction(node.Syntax, node.MethodOpt, out var receiver, out var method, ref arguments, ref argRefKinds);
                return new BoundDelegateCreationExpression(node.Syntax, receiver, method, node.IsExtensionMethod, VisitType(node.Type));
            }
            return base.VisitDelegateCreationExpression(node);
        }

        public override BoundNode VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            if (node.TargetMethod.MethodKind == MethodKind.LocalFunction)
            {
                ImmutableArray<BoundExpression> arguments = default(ImmutableArray<BoundExpression>);
                ImmutableArray<RefKind> argRefKinds = default(ImmutableArray<RefKind>);
                RemapLocalFunction(node.Syntax, node.TargetMethod, out var _, out var method, ref arguments, ref argRefKinds);
                return node.Update(method, node.Type);
            }
            return base.VisitFunctionPointerLoad(node);
        }

        public override BoundNode VisitConversion(BoundConversion conversion)
        {
            if (conversion.ConversionKind == ConversionKind.AnonymousFunction)
            {
                BoundExpression boundExpression = (BoundExpression)RewriteLambdaConversion((BoundLambda)conversion.Operand);
                if (_inExpressionLambda && conversion.ExplicitCastInCode)
                {
                    boundExpression = new BoundConversion(conversion.Syntax, boundExpression, conversion.Conversion, isBaseConversion: false, @checked: false, explicitCastInCode: true, conversionGroupOpt: conversion.ConversionGroupOpt, constantValueOpt: conversion.ConstantValueOpt, type: conversion.Type);
                }
                return boundExpression;
            }
            return base.VisitConversion(conversion);
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            RewriteLambdaOrLocalFunction(node, out var _, out var _, out var _, out var _, out var _, out var _);
            return new BoundNoOpStatement(node.Syntax, NoOpStatementFlavor.Default);
        }

        private DebugId GetLambdaId(SyntaxNode syntax, ClosureKind closureKind, int closureOrdinal)
        {
            SyntaxNode syntaxNode;
            bool isLambdaBody;
            if (syntax is AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax)
            {
                syntaxNode = anonymousFunctionExpressionSyntax.Body;
                isLambdaBody = true;
            }
            else if (syntax is LocalFunctionStatementSyntax localFunctionStatementSyntax)
            {
                syntaxNode = (SyntaxNode)(localFunctionStatementSyntax.Body ?? ((object)localFunctionStatementSyntax.ExpressionBody?.Expression));
                if (syntaxNode == null)
                {
                    syntaxNode = localFunctionStatementSyntax;
                    isLambdaBody = false;
                }
                else
                {
                    isLambdaBody = true;
                }
            }
            else if (LambdaUtilities.IsQueryPairLambda(syntax))
            {
                syntaxNode = syntax;
                isLambdaBody = false;
            }
            else
            {
                syntaxNode = syntax;
                isLambdaBody = true;
            }
            DebugId debugId = ((slotAllocatorOpt == null || !slotAllocatorOpt.TryGetPreviousLambda(syntaxNode, isLambdaBody, out DebugId lambdaId)) ? new DebugId(_lambdaDebugInfoBuilder.Count, CompilationState.ModuleBuilderOpt!.CurrentGenerationOrdinal) : lambdaId);
            int syntaxOffset = _topLevelMethod.CalculateLocalSyntaxOffset(LambdaUtilities.GetDeclaratorPosition(syntaxNode), syntaxNode.SyntaxTree);
            _lambdaDebugInfoBuilder.Add(new LambdaDebugInfo(syntaxOffset, debugId, closureOrdinal));
            return debugId;
        }

        private SynthesizedClosureMethod RewriteLambdaOrLocalFunction(IBoundLambdaOrFunction node, out ClosureKind closureKind, out NamedTypeSymbol translatedLambdaContainer, out SynthesizedClosureEnvironment containerAsFrame, out BoundNode lambdaScope, out DebugId topLevelMethodId, out DebugId lambdaId)
        {
            Analysis.NestedFunction function = Analysis.GetNestedFunctionInTree(_analysis.ScopeTree, node.Symbol);
            SynthesizedClosureMethod synthesizedLoweredMethod = function.SynthesizedLoweredMethod;
            closureKind = synthesizedLoweredMethod.ClosureKind;
            translatedLambdaContainer = synthesizedLoweredMethod.ContainingType;
            containerAsFrame = translatedLambdaContainer as SynthesizedClosureEnvironment;
            topLevelMethodId = _analysis.GetTopLevelMethodId();
            lambdaId = synthesizedLoweredMethod.LambdaId;
            if (function.ContainingEnvironmentOpt != null)
            {
                BoundNode tmpScope = null;
                Analysis.VisitScopeTree(_analysis.ScopeTree, delegate (Analysis.Scope scope)
                {
                    if (scope.DeclaredEnvironment == function.ContainingEnvironmentOpt)
                    {
                        tmpScope = scope.BoundNode;
                    }
                });
                lambdaScope = tmpScope;
            }
            else
            {
                lambdaScope = null;
            }
            CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(translatedLambdaContainer, synthesizedLoweredMethod.GetCciAdapter());
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.Symbol.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                _parameterMap.Add(current, synthesizedLoweredMethod.Parameters[current.Ordinal]);
            }
            MethodSymbol currentMethod = _currentMethod;
            ParameterSymbol currentFrameThis = _currentFrameThis;
            ImmutableArray<TypeParameterSymbol> currentTypeParameters = _currentTypeParameters;
            Symbol innermostFramePointer = _innermostFramePointer;
            TypeMap currentLambdaBodyTypeMap = _currentLambdaBodyTypeMap;
            ArrayBuilder<BoundStatement> addedStatements = _addedStatements;
            ArrayBuilder<LocalSymbol> addedLocals = _addedLocals;
            _addedStatements = null;
            _addedLocals = null;
            _currentMethod = synthesizedLoweredMethod;
            if (closureKind == ClosureKind.Static || closureKind == ClosureKind.Singleton)
            {
                _innermostFramePointer = (_currentFrameThis = null);
            }
            else
            {
                _currentFrameThis = synthesizedLoweredMethod.ThisParameter;
                _framePointers.TryGetValue(translatedLambdaContainer, out _innermostFramePointer);
            }
            _currentTypeParameters = containerAsFrame?.TypeParameters.Concat(synthesizedLoweredMethod.TypeParameters) ?? synthesizedLoweredMethod.TypeParameters;
            _currentLambdaBodyTypeMap = synthesizedLoweredMethod.TypeMap;
            BoundBlock body = node.Body;
            if (body != null)
            {
                BoundStatement body2 = AddStatementsIfNeeded((BoundStatement)VisitBlock(body));
                AddSynthesizedMethod(synthesizedLoweredMethod, body2);
            }
            _currentMethod = currentMethod;
            _currentFrameThis = currentFrameThis;
            _currentTypeParameters = currentTypeParameters;
            _innermostFramePointer = innermostFramePointer;
            _currentLambdaBodyTypeMap = currentLambdaBodyTypeMap;
            _addedLocals = addedLocals;
            _addedStatements = addedStatements;
            return synthesizedLoweredMethod;
        }

        private void AddSynthesizedMethod(MethodSymbol method, BoundStatement body)
        {
            if (_synthesizedMethods == null)
            {
                _synthesizedMethods = ArrayBuilder<TypeCompilationState.MethodWithBody>.GetInstance();
            }
            _synthesizedMethods.Add(new TypeCompilationState.MethodWithBody(method, body, CompilationState.CurrentImportChain));
        }

        private BoundNode RewriteLambdaConversion(BoundLambda node)
        {
            bool inExpressionLambda = _inExpressionLambda;
            _inExpressionLambda = _inExpressionLambda || node.Type.IsExpressionTree();
            if (_inExpressionLambda)
            {
                TypeSymbol type = VisitType(node.Type);
                BoundBlock body = (BoundBlock)Visit(node.Body);
                node = node.Update(node.UnboundLambda, node.Symbol, body, node.Diagnostics, node.Binder, type);
                BoundNode result = (inExpressionLambda ? node : ExpressionLambdaRewriter.RewriteLambda(node, CompilationState, TypeMap, base.RecursionDepth, Diagnostics));
                _inExpressionLambda = inExpressionLambda;
                return result;
            }
            MethodSymbol synthesizedMethod = RewriteLambdaOrLocalFunction(node, out ClosureKind closureKind, out NamedTypeSymbol translatedLambdaContainer, out SynthesizedClosureEnvironment containerAsFrame, out BoundNode lambdaScope, out DebugId topLevelMethodId, out DebugId lambdaId);
            RemapLambdaOrLocalFunction(node.Syntax, node.Symbol, default(ImmutableArray<TypeWithAnnotations>), closureKind, ref synthesizedMethod, out var receiver, out var constructedFrame);
            TypeSymbol type2 = VisitType(node.Type);
            BoundExpression boundExpression = new BoundDelegateCreationExpression(node.Syntax, receiver, synthesizedMethod, isExtensionMethod: false, type2);
            bool flag = closureKind == ClosureKind.Singleton && _currentMethod.MethodKind != MethodKind.StaticConstructor && !synthesizedMethod.IsGenericMethod;
            bool flag2 = lambdaScope != null && lambdaScope != Analysis.GetScopeParent(_analysis.ScopeTree, node.Body).BoundNode && InLoopOrLambda(node.Syntax, lambdaScope.Syntax);
            if (flag || flag2)
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(_currentMethod, node.Syntax, CompilationState, Diagnostics);
                try
                {
                    BoundExpression left;
                    if (flag || (flag2 && (object)containerAsFrame != null))
                    {
                        TypeSymbol type3 = containerAsFrame.TypeMap.SubstituteType(node.Type).Type;
                        string name = GeneratedNames.MakeLambdaCacheFieldName((closureKind == ClosureKind.General) ? (-1) : topLevelMethodId.Ordinal, topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation);
                        SynthesizedLambdaCacheFieldSymbol synthesizedLambdaCacheFieldSymbol = new SynthesizedLambdaCacheFieldSymbol(translatedLambdaContainer, type3, name, _topLevelMethod, isReadOnly: false, closureKind == ClosureKind.Singleton);
                        CompilationState.ModuleBuilderOpt!.AddSynthesizedDefinition(translatedLambdaContainer, synthesizedLambdaCacheFieldSymbol.GetCciAdapter());
                        left = syntheticBoundNodeFactory.Field(receiver, synthesizedLambdaCacheFieldSymbol.AsMember(constructedFrame));
                    }
                    else
                    {
                        LocalSymbol localSymbol = syntheticBoundNodeFactory.SynthesizedLocal(type2, null, isPinned: false, RefKind.None, SynthesizedLocalKind.CachedAnonymousMethodDelegate);
                        if (_addedLocals == null)
                        {
                            _addedLocals = ArrayBuilder<LocalSymbol>.GetInstance();
                        }
                        _addedLocals.Add(localSymbol);
                        if (_addedStatements == null)
                        {
                            _addedStatements = ArrayBuilder<BoundStatement>.GetInstance();
                        }
                        left = syntheticBoundNodeFactory.Local(localSymbol);
                        _addedStatements.Add(syntheticBoundNodeFactory.Assignment(left, syntheticBoundNodeFactory.Null(type2)));
                    }
                    return syntheticBoundNodeFactory.Coalesce(left, syntheticBoundNodeFactory.AssignmentExpression(left, boundExpression));
                }
                catch (SyntheticBoundNodeFactory.MissingPredefinedMember missingPredefinedMember)
                {
                    Diagnostics.Add(missingPredefinedMember.Diagnostic);
                    return new BoundBadExpression(syntheticBoundNodeFactory.Syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, ImmutableArray.Create((BoundExpression)node), node.Type);
                }
            }
            return boundExpression;
        }

        private static bool InLoopOrLambda(SyntaxNode lambdaSyntax, SyntaxNode scopeSyntax)
        {
            SyntaxNode parent = lambdaSyntax.Parent;
            while (parent != null && parent != scopeSyntax)
            {
                SyntaxKind syntaxKind = parent.Kind();
                if (syntaxKind - 8642 <= SyntaxKind.List || syntaxKind - 8809 <= (SyntaxKind)3 || syntaxKind == SyntaxKind.ForEachVariableStatement)
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
