using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class MethodToStateMachineRewriter : MethodToClassRewriter
    {
        internal readonly MethodSymbol OriginalMethod;

        private readonly bool _useFinalizerBookkeeping;

        protected readonly SyntheticBoundNodeFactory F;

        protected readonly FieldSymbol stateField;

        protected readonly LocalSymbol cachedState;

        protected readonly LocalSymbol cachedThis;

        private int _nextState;

        private Dictionary<LabelSymbol, List<int>> _dispatches = new Dictionary<LabelSymbol, List<int>>();

        private bool _hasFinalizerState = true;

        private int _currentFinalizerState = -1;

        private Dictionary<TypeSymbol, ArrayBuilder<StateMachineFieldSymbol>> _lazyAvailableReusableHoistedFields;

        private int _nextHoistedFieldId = 1;

        private readonly EmptyStructTypeCache _emptyStructTypeCache = EmptyStructTypeCache.CreateNeverEmpty();

        private readonly IReadOnlySet<Symbol> _hoistedVariables;

        private readonly SynthesizedLocalOrdinalsDispenser _synthesizedLocalOrdinals;

        private int _nextFreeHoistedLocalSlot;

        protected override TypeMap TypeMap => ((SynthesizedContainer)F.CurrentType).TypeMap;

        protected override MethodSymbol CurrentMethod => F.CurrentFunction;

        protected override NamedTypeSymbol ContainingType => OriginalMethod.ContainingType;

        internal IReadOnlySet<Symbol> HoistedVariables => _hoistedVariables;

        protected abstract BoundStatement GenerateReturn(bool finished);

        public MethodToStateMachineRewriter(SyntheticBoundNodeFactory F, MethodSymbol originalMethod, FieldSymbol state, IReadOnlySet<Symbol> hoistedVariables, IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> nonReusableLocalProxies, SynthesizedLocalOrdinalsDispenser synthesizedLocalOrdinals, VariableSlotAllocator slotAllocatorOpt, int nextFreeHoistedLocalSlot, BindingDiagnosticBag diagnostics, bool useFinalizerBookkeeping)
            : base(slotAllocatorOpt, F.CompilationState, diagnostics)
        {
            this.F = F;
            stateField = state;
            cachedState = F.SynthesizedLocal(F.SpecialType(SpecialType.System_Int32), F.Syntax, isPinned: false, RefKind.None, SynthesizedLocalKind.StateMachineCachedState);
            _useFinalizerBookkeeping = useFinalizerBookkeeping;
            _hasFinalizerState = useFinalizerBookkeeping;
            OriginalMethod = originalMethod;
            _hoistedVariables = hoistedVariables;
            _synthesizedLocalOrdinals = synthesizedLocalOrdinals;
            _nextFreeHoistedLocalSlot = nextFreeHoistedLocalSlot;
            foreach (KeyValuePair<Symbol, CapturedSymbolReplacement> nonReusableLocalProxy in nonReusableLocalProxies)
            {
                proxies.Add(nonReusableLocalProxy.Key, nonReusableLocalProxy.Value);
            }
            ParameterSymbol thisParameter = originalMethod.ThisParameter;
            if ((object)thisParameter != null && thisParameter.Type.IsReferenceType && proxies.TryGetValue(thisParameter, out var value) && F.Compilation.Options.OptimizationLevel == OptimizationLevel.Release)
            {
                BoundExpression boundExpression = value.Replacement(F.Syntax, (NamedTypeSymbol frameType) => F.This());
                cachedThis = F.SynthesizedLocal(boundExpression.Type, F.Syntax, isPinned: false, RefKind.None, SynthesizedLocalKind.FrameCache);
            }
        }

        protected override bool NeedsProxy(Symbol localOrParameter)
        {
            return _hoistedVariables.Contains(localOrParameter);
        }

        protected override BoundExpression FramePointer(SyntaxNode syntax, NamedTypeSymbol frameClass)
        {
            SyntaxNode syntax2 = F.Syntax;
            F.Syntax = syntax;
            BoundThisReference result = F.This();
            F.Syntax = syntax2;
            return result;
        }

        protected void AddState(out int stateNumber, out GeneratedLabelSymbol resumeLabel)
        {
            stateNumber = _nextState++;
            if (_useFinalizerBookkeeping && !_hasFinalizerState)
            {
                _currentFinalizerState = _nextState++;
                _hasFinalizerState = true;
            }
            AddState(stateNumber, out resumeLabel);
        }

        protected void AddState(int stateNumber, out GeneratedLabelSymbol resumeLabel)
        {
            if (_dispatches == null)
            {
                _dispatches = new Dictionary<LabelSymbol, List<int>>();
            }
            resumeLabel = F.GenerateLabel("stateMachine");
            List<int> list = new List<int>();
            list.Add(stateNumber);
            _dispatches.Add(resumeLabel, list);
        }

        protected BoundStatement Dispatch()
        {
            return F.Switch(F.Local(cachedState), _dispatches.OrderBy(delegate (KeyValuePair<LabelSymbol, List<int>> kv)
            {
                KeyValuePair<LabelSymbol, List<int>> keyValuePair2 = kv;
                return keyValuePair2.Value[0];
            }).Select(delegate (KeyValuePair<LabelSymbol, List<int>> kv)
            {
                SyntheticBoundNodeFactory f = F;
                KeyValuePair<LabelSymbol, List<int>> keyValuePair = kv;
                List<int> value = keyValuePair.Value;
                BoundStatement[] array = new BoundStatement[1];
                SyntheticBoundNodeFactory f2 = F;
                keyValuePair = kv;
                array[0] = f2.Goto(keyValuePair.Key);
                return f.SwitchSection(value, array);
            }).ToImmutableArray());
        }

        private BoundStatement PossibleIteratorScope(ImmutableArray<LocalSymbol> locals, Func<BoundStatement> wrapped)
        {
            if (locals.IsDefaultOrEmpty)
            {
                return wrapped();
            }
            ArrayBuilder<StateMachineFieldSymbol> instance = ArrayBuilder<StateMachineFieldSymbol>.GetInstance();
            ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                if (!NeedsProxy(current) || current.RefKind != 0)
                {
                    continue;
                }
                bool reused = false;
                if (!proxies.TryGetValue(current, out var value))
                {
                    value = new CapturedToStateMachineFieldReplacement(GetOrAllocateReusableHoistedField(TypeMap.SubstituteType(current.Type).Type, out reused, current), isReusable: true);
                    proxies.Add(current, value);
                }
                if (current.SynthesizedKind == SynthesizedLocalKind.UserDefined)
                {
                    SyntaxNode scopeDesignatorOpt = current.ScopeDesignatorOpt;
                    if (scopeDesignatorOpt == null || scopeDesignatorOpt.Kind() != SyntaxKind.SwitchSection)
                    {
                        goto IL_00c8;
                    }
                }
                if (current.SynthesizedKind != SynthesizedLocalKind.LambdaDisplayClass)
                {
                    continue;
                }
                goto IL_00c8;
            IL_00c8:
                if (!reused)
                {
                    instance.Add(((CapturedToStateMachineFieldReplacement)value).HoistedField);
                }
            }
            BoundStatement boundStatement = wrapped();
            ArrayBuilder<BoundAssignmentOperator> instance2 = ArrayBuilder<BoundAssignmentOperator>.GetInstance();
            enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current2 = enumerator.Current;
                if (!proxies.TryGetValue(current2, out var value2))
                {
                    continue;
                }
                if (value2 is CapturedToStateMachineFieldReplacement capturedToStateMachineFieldReplacement)
                {
                    AddVariableCleanup(instance2, capturedToStateMachineFieldReplacement.HoistedField);
                    if (value2.IsReusable)
                    {
                        FreeReusableHoistedField(capturedToStateMachineFieldReplacement.HoistedField);
                    }
                    continue;
                }
                ImmutableArray<StateMachineFieldSymbol>.Enumerator enumerator2 = ((CapturedToExpressionSymbolReplacement)value2).HoistedFields.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    StateMachineFieldSymbol current3 = enumerator2.Current;
                    AddVariableCleanup(instance2, current3);
                    if (value2.IsReusable)
                    {
                        FreeReusableHoistedField(current3);
                    }
                }
            }
            if (instance2.Count != 0)
            {
                boundStatement = F.Block(boundStatement, F.Block(instance2.SelectAsArray((Func<BoundAssignmentOperator, SyntheticBoundNodeFactory, BoundStatement>)((BoundAssignmentOperator e, SyntheticBoundNodeFactory f) => f.ExpressionStatement(e)), F)));
            }
            instance2.Free();
            if (instance.Count != 0)
            {
                boundStatement = MakeStateMachineScope(instance.ToImmutable(), boundStatement);
            }
            instance.Free();
            return boundStatement;
        }

        internal BoundBlock MakeStateMachineScope(ImmutableArray<StateMachineFieldSymbol> hoistedLocals, BoundStatement statement)
        {
            return F.Block(new BoundStateMachineScope(F.Syntax, hoistedLocals, statement));
        }

        internal static bool TryUnwrapBoundStateMachineScope(ref BoundStatement statement, out ImmutableArray<StateMachineFieldSymbol> hoistedLocals)
        {
            if (statement.Kind == BoundKind.Block)
            {
                ImmutableArray<BoundStatement> statements = ((BoundBlock)statement).Statements;
                if (statements.Length == 1 && statements[0].Kind == BoundKind.StateMachineScope)
                {
                    BoundStateMachineScope boundStateMachineScope = (BoundStateMachineScope)statements[0];
                    statement = boundStateMachineScope.Statement;
                    hoistedLocals = boundStateMachineScope.Fields;
                    return true;
                }
            }
            hoistedLocals = ImmutableArray<StateMachineFieldSymbol>.Empty;
            return false;
        }

        private void AddVariableCleanup(ArrayBuilder<BoundAssignmentOperator> cleanup, FieldSymbol field)
        {
            if (MightContainReferences(field.Type))
            {
                cleanup.Add(F.AssignmentExpression(F.Field(F.This(), field), F.NullOrDefault(field.Type)));
            }
        }

        private bool MightContainReferences(TypeSymbol type)
        {
            if (type.IsReferenceType || type.TypeKind == TypeKind.TypeParameter)
            {
                return true;
            }
            if (type.TypeKind != TypeKind.Struct)
            {
                return false;
            }
            if (type.SpecialType == SpecialType.System_TypedReference)
            {
                return true;
            }
            if (type.SpecialType != 0)
            {
                return false;
            }
            if (!type.IsFromCompilation(CompilationState.ModuleBuilderOpt!.Compilation))
            {
                return true;
            }
            foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(type))
            {
                if (MightContainReferences(structInstanceField.Type))
                {
                    return true;
                }
            }
            return false;
        }

        private StateMachineFieldSymbol GetOrAllocateReusableHoistedField(TypeSymbol type, out bool reused, LocalSymbol local = null)
        {
            if (_lazyAvailableReusableHoistedFields != null && _lazyAvailableReusableHoistedFields.TryGetValue(type, out var value) && value.Count > 0)
            {
                StateMachineFieldSymbol result = value.Last();
                value.RemoveLast();
                reused = true;
                return result;
            }
            reused = false;
            int num = _nextHoistedFieldId++;
            if ((object)local != null && local.SynthesizedKind == SynthesizedLocalKind.UserDefined)
            {
                string name = GeneratedNames.MakeHoistedLocalFieldName(SynthesizedLocalKind.UserDefined, num, local.Name);
                return F.StateMachineField(type, name, SynthesizedLocalKind.UserDefined, num);
            }
            return F.StateMachineField(type, GeneratedNames.ReusableHoistedLocalFieldName(num));
        }

        private void FreeReusableHoistedField(StateMachineFieldSymbol field)
        {
            if (_lazyAvailableReusableHoistedFields == null || !_lazyAvailableReusableHoistedFields.TryGetValue(field.Type, out var value))
            {
                if (_lazyAvailableReusableHoistedFields == null)
                {
                    _lazyAvailableReusableHoistedFields = new Dictionary<TypeSymbol, ArrayBuilder<StateMachineFieldSymbol>>(Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.IgnoringDynamicTupleNamesAndNullability);
                }
                _lazyAvailableReusableHoistedFields.Add(field.Type, value = new ArrayBuilder<StateMachineFieldSymbol>());
            }
            value.Add(field);
        }

        private BoundExpression HoistRefInitialization(SynthesizedLocal local, BoundAssignmentOperator node)
        {
            BoundExpression expr = (BoundExpression)Visit(node.Right);
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
            bool needsSacrificialEvaluation = false;
            ArrayBuilder<StateMachineFieldSymbol> instance2 = ArrayBuilder<StateMachineFieldSymbol>.GetInstance();
            AwaitExpressionSyntax awaitExpressionSyntax;
            int syntaxOffset;
            if (F.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug)
            {
                awaitExpressionSyntax = (AwaitExpressionSyntax)local.GetDeclaratorSyntax();
                syntaxOffset = OriginalMethod.CalculateLocalSyntaxOffset(LambdaUtilities.GetDeclaratorPosition(awaitExpressionSyntax), awaitExpressionSyntax.SyntaxTree);
            }
            else
            {
                awaitExpressionSyntax = null;
                syntaxOffset = -1;
            }
            BoundExpression boundExpression = HoistExpression(expr, awaitExpressionSyntax, syntaxOffset, local.RefKind, instance, instance2, ref needsSacrificialEvaluation);
            proxies.Add(local, new CapturedToExpressionSymbolReplacement(boundExpression, instance2.ToImmutableAndFree(), isReusable: true));
            if (needsSacrificialEvaluation)
            {
                TypeSymbol type = TypeMap.SubstituteType(local.Type).Type;
                LocalSymbol localSymbol = F.SynthesizedLocal(type, null, isPinned: false, RefKind.Ref);
                return F.Sequence(ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree(), F.AssignmentExpression(F.Local(localSymbol), boundExpression, isRef: true));
            }
            if (instance.Count == 0)
            {
                instance.Free();
                return null;
            }
            BoundExpression result = instance.Last();
            instance.RemoveLast();
            return F.Sequence(ImmutableArray<LocalSymbol>.Empty, instance.ToImmutableAndFree(), result);
        }

        private BoundExpression HoistExpression(BoundExpression expr, AwaitExpressionSyntax awaitSyntaxOpt, int syntaxOffset, RefKind refKind, ArrayBuilder<BoundExpression> sideEffects, ArrayBuilder<StateMachineFieldSymbol> hoistedFields, ref bool needsSacrificialEvaluation)
        {
            switch (expr.Kind)
            {
                case BoundKind.ArrayAccess:
                    {
                        BoundArrayAccess boundArrayAccess = (BoundArrayAccess)expr;
                        BoundExpression expression = HoistExpression(boundArrayAccess.Expression, awaitSyntaxOpt, syntaxOffset, RefKind.None, sideEffects, hoistedFields, ref needsSacrificialEvaluation);
                        ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
                        ImmutableArray<BoundExpression>.Enumerator enumerator = boundArrayAccess.Indices.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            BoundExpression current = enumerator.Current;
                            instance.Add(HoistExpression(current, awaitSyntaxOpt, syntaxOffset, RefKind.None, sideEffects, hoistedFields, ref needsSacrificialEvaluation));
                        }
                        needsSacrificialEvaluation = true;
                        return boundArrayAccess.Update(expression, instance.ToImmutableAndFree(), boundArrayAccess.Type);
                    }
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
                        if (boundFieldAccess.FieldSymbol.IsStatic)
                        {
                            if (refKind != 0 || boundFieldAccess.FieldSymbol.IsReadOnly)
                            {
                                return expr;
                            }
                        }
                        else if (refKind != 0)
                        {
                            bool flag = !boundFieldAccess.FieldSymbol.ContainingType.IsReferenceType;
                            BoundExpression boundExpression = HoistExpression(boundFieldAccess.ReceiverOpt, awaitSyntaxOpt, syntaxOffset, flag ? refKind : RefKind.None, sideEffects, hoistedFields, ref needsSacrificialEvaluation);
                            if (boundExpression.Kind != BoundKind.ThisReference && !flag)
                            {
                                needsSacrificialEvaluation = true;
                            }
                            return F.Field(boundExpression, boundFieldAccess.FieldSymbol);
                        }
                        break;
                    }
                case BoundKind.DefaultExpression:
                case BoundKind.ThisReference:
                case BoundKind.BaseReference:
                    return expr;
                case BoundKind.Call:
                    {
                        BoundCall boundCall = (BoundCall)expr;
                        if (refKind != 0 && refKind != RefKind.In)
                        {
                            F.Diagnostics.Add(ErrorCode.ERR_RefReturningCallAndAwait, F.Syntax.Location, boundCall.Method);
                        }
                        refKind = RefKind.None;
                        break;
                    }
                case BoundKind.ConditionalOperator:
                    _ = (BoundConditionalOperator)expr;
                    if (refKind != 0 && refKind != RefKind.In)
                    {
                        F.Diagnostics.Add(ErrorCode.ERR_RefConditionalAndAwait, F.Syntax.Location);
                    }
                    refKind = RefKind.None;
                    break;
            }
            if (expr.ConstantValue != null)
            {
                return expr;
            }
            if (refKind != 0)
            {
                throw ExceptionUtilities.UnexpectedValue(expr.Kind);
            }
            TypeSymbol type = expr.Type;
            StateMachineFieldSymbol stateMachineFieldSymbol;
            if (F.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug)
            {
                int ordinal = _synthesizedLocalOrdinals.AssignLocalOrdinal(SynthesizedLocalKind.AwaitByRefSpill, syntaxOffset);
                LocalDebugId localDebugId = new LocalDebugId(syntaxOffset, ordinal);
                if (slotAllocatorOpt == null || !slotAllocatorOpt.TryGetPreviousHoistedLocalSlotIndex(awaitSyntaxOpt, F.ModuleBuilderOpt!.Translate(type, awaitSyntaxOpt, Diagnostics.DiagnosticBag), SynthesizedLocalKind.AwaitByRefSpill, localDebugId, Diagnostics.DiagnosticBag, out var slotIndex))
                {
                    slotIndex = _nextFreeHoistedLocalSlot++;
                }
                string name = GeneratedNames.MakeHoistedLocalFieldName(SynthesizedLocalKind.AwaitByRefSpill, slotIndex);
                stateMachineFieldSymbol = F.StateMachineField(expr.Type, name, new LocalSlotDebugInfo(SynthesizedLocalKind.AwaitByRefSpill, localDebugId), slotIndex);
            }
            else
            {
                stateMachineFieldSymbol = GetOrAllocateReusableHoistedField(type, out var _);
            }
            hoistedFields.Add(stateMachineFieldSymbol);
            BoundFieldAccess boundFieldAccess2 = F.Field(F.This(), stateMachineFieldSymbol);
            sideEffects.Add(F.AssignmentExpression(boundFieldAccess2, expr));
            return boundFieldAccess2;
        }

        public override BoundNode Visit(BoundNode node)
        {
            if (node == null)
            {
                return node;
            }
            SyntaxNode syntax = F.Syntax;
            F.Syntax = node.Syntax;
            BoundNode? result = base.Visit(node);
            F.Syntax = syntax;
            return result;
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            return PossibleIteratorScope(node.Locals, () => (BoundStatement)base.VisitBlock(node));
        }

        public override BoundNode VisitScope(BoundScope node)
        {
            ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
            ArrayBuilder<StateMachineFieldSymbol> instance2 = ArrayBuilder<StateMachineFieldSymbol>.GetInstance();
            bool flag = false;
            ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                if (TryRewriteLocal(current, out var newLocal))
                {
                    instance.Add(newLocal);
                    flag = flag || (object)current != newLocal;
                }
                else
                {
                    instance2.Add(((CapturedToStateMachineFieldReplacement)proxies[current]).HoistedField);
                }
            }
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            if (instance2.Count != 0)
            {
                BoundStatement statement;
                if (instance.Count == 0)
                {
                    instance.Free();
                    statement = new BoundStatementList(node.Syntax, statements);
                }
                else
                {
                    statement = node.Update(instance.ToImmutableAndFree(), statements);
                }
                return MakeStateMachineScope(instance2.ToImmutable(), statement);
            }
            instance2.Free();
            ImmutableArray<LocalSymbol> locals;
            if (flag)
            {
                locals = instance.ToImmutableAndFree();
            }
            else
            {
                instance.Free();
                locals = node.Locals;
            }
            return node.Update(locals, statements);
        }

        public override BoundNode VisitForStatement(BoundForStatement node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitUsingStatement(BoundUsingStatement node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
        {
            BoundExpression boundExpression = (BoundExpression)Visit(node.Expression);
            if (boundExpression != null)
            {
                return node.Update(boundExpression);
            }
            return null;
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            if (node.Left.Kind != BoundKind.Local)
            {
                return base.VisitAssignmentOperator(node);
            }
            LocalSymbol localSymbol = ((BoundLocal)node.Left).LocalSymbol;
            if (!NeedsProxy(localSymbol))
            {
                return base.VisitAssignmentOperator(node);
            }
            if (proxies.ContainsKey(localSymbol))
            {
                return base.VisitAssignmentOperator(node);
            }
            return HoistRefInitialization((SynthesizedLocal)localSymbol, node);
        }

        public override BoundNode VisitTryStatement(BoundTryStatement node)
        {
            Dictionary<LabelSymbol, List<int>> dictionary = _dispatches;
            int currentFinalizerState = _currentFinalizerState;
            bool hasFinalizerState = _hasFinalizerState;
            _dispatches = null;
            _currentFinalizerState = -1;
            _hasFinalizerState = false;
            BoundBlock boundBlock = F.Block((BoundStatement)Visit(node.TryBlock));
            GeneratedLabelSymbol generatedLabelSymbol = null;
            if (_dispatches != null)
            {
                generatedLabelSymbol = F.GenerateLabel("tryDispatch");
                if (_hasFinalizerState)
                {
                    GeneratedLabelSymbol generatedLabelSymbol2 = F.GenerateLabel("finalizer");
                    _dispatches.Add(generatedLabelSymbol2, new List<int> { _currentFinalizerState });
                    GeneratedLabelSymbol label = F.GenerateLabel("skipFinalizer");
                    boundBlock = F.Block(F.HiddenSequencePoint(), Dispatch(), F.Goto(label), F.Label(generatedLabelSymbol2), GenerateSetBothStates(-1), GenerateReturn(finished: false), F.Label(label), boundBlock);
                }
                else
                {
                    boundBlock = F.Block(F.HiddenSequencePoint(), Dispatch(), boundBlock);
                }
                if (dictionary == null)
                {
                    dictionary = new Dictionary<LabelSymbol, List<int>>();
                }
                dictionary.Add(generatedLabelSymbol, new List<int>(from kv in _dispatches.Values
                                                                   from n in kv
                                                                   orderby n
                                                                   select n));
            }
            _hasFinalizerState = hasFinalizerState;
            _currentFinalizerState = currentFinalizerState;
            _dispatches = dictionary;
            ImmutableArray<BoundCatchBlock> catchBlocks = VisitList(node.CatchBlocks);
            BoundBlock finallyBlockOpt = ((node.FinallyBlockOpt == null) ? null : F.Block(F.HiddenSequencePoint(), F.If(ShouldEnterFinallyBlock(), VisitFinally(node.FinallyBlockOpt)), F.HiddenSequencePoint()));
            BoundStatement boundStatement = node.Update(boundBlock, catchBlocks, finallyBlockOpt, node.FinallyLabelOpt, node.PreferFaultHandler);
            if ((object)generatedLabelSymbol != null)
            {
                boundStatement = F.Block(F.HiddenSequencePoint(), F.Label(generatedLabelSymbol), boundStatement);
            }
            return boundStatement;
        }

        protected virtual BoundBlock VisitFinally(BoundBlock finallyBlock)
        {
            return (BoundBlock)Visit(finallyBlock);
        }

        protected virtual BoundBinaryOperator ShouldEnterFinallyBlock()
        {
            return F.IntLessThan(F.Local(cachedState), F.Literal(0));
        }

        protected BoundExpressionStatement GenerateSetBothStates(int stateNumber)
        {
            return F.Assignment(F.Field(F.This(), stateField), F.AssignmentExpression(F.Local(cachedState), F.Literal(stateNumber)));
        }

        protected BoundStatement CacheThisIfNeeded()
        {
            if ((object)cachedThis != null)
            {
                BoundExpression right = proxies[OriginalMethod.ThisParameter].Replacement(F.Syntax, (NamedTypeSymbol frameType) => F.This());
                return F.Assignment(F.Local(cachedThis), right);
            }
            return F.StatementList();
        }

        public sealed override BoundNode VisitThisReference(BoundThisReference node)
        {
            if ((object)cachedThis != null)
            {
                return F.Local(cachedThis);
            }
            ParameterSymbol thisParameter = OriginalMethod.ThisParameter;
            if ((object)thisParameter == null || !proxies.TryGetValue(thisParameter, out var value))
            {
                return node.Update(VisitType(node.Type));
            }
            return value.Replacement(F.Syntax, (NamedTypeSymbol frameType) => F.This());
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            if ((object)cachedThis != null)
            {
                return F.Local(cachedThis);
            }
            return proxies[OriginalMethod.ThisParameter].Replacement(F.Syntax, (NamedTypeSymbol frameType) => F.This());
        }
    }
}
