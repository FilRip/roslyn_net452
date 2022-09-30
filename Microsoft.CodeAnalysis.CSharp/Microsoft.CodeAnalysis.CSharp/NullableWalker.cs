using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class NullableWalker : LocalDataFlowPass<NullableWalker.LocalState, NullableWalker.LocalFunctionState>
    {
        public sealed class VariableState
        {
            internal readonly VariablesSnapshot Variables;

            internal readonly LocalStateSnapshot VariableNullableStates;

            public VariableState(VariablesSnapshot variables, LocalStateSnapshot variableNullableStates)
            {
                Variables = variables;
                VariableNullableStates = variableNullableStates;
            }
        }

        internal readonly struct Data
        {
            internal readonly int TrackedEntries;

            internal readonly bool RequiredAnalysis;

            internal Data(int trackedEntries, bool requiredAnalysis)
            {
                TrackedEntries = trackedEntries;
                RequiredAnalysis = requiredAnalysis;
            }
        }private readonly struct VisitResult
        {
            public readonly TypeWithState RValueType;

            public readonly TypeWithAnnotations LValueType;

            public VisitResult(TypeWithState rValueType, TypeWithAnnotations lValueType)
            {
                RValueType = rValueType;
                LValueType = lValueType;
            }

            public VisitResult(TypeSymbol? type, NullableAnnotation annotation, NullableFlowState state)
            {
                RValueType = TypeWithState.Create(type, state);
                LValueType = TypeWithAnnotations.Create(type, annotation);
            }

            internal string GetDebuggerDisplay()
            {
                return "{LValue: " + LValueType.GetDebuggerDisplay() + ", RValue: " + RValueType.GetDebuggerDisplay() + "}";
            }
        }

        [DebuggerDisplay("{VisitResult.GetDebuggerDisplay(), nq}")]
        private readonly struct VisitArgumentResult
        {
            public readonly VisitResult VisitResult;

            public readonly Optional<LocalState> StateForLambda;

            public TypeWithState RValueType => VisitResult.RValueType;

            public TypeWithAnnotations LValueType => VisitResult.LValueType;

            public VisitArgumentResult(VisitResult visitResult, Optional<LocalState> stateForLambda)
            {
                VisitResult = visitResult;
                StateForLambda = stateForLambda;
            }
        }

        private enum AssignmentKind
        {
            Assignment,
            Return,
            Argument,
            ForEachIterationVariable
        }

        private readonly struct CompareExchangeInfo
        {
            public readonly ImmutableArray<BoundExpression> Arguments;

            public readonly ImmutableArray<VisitArgumentResult> Results;

            public readonly ImmutableArray<int> ArgsToParamsOpt;

            public bool IsDefault
            {
                get
                {
                    if (!Arguments.IsDefault)
                    {
                        return Results.IsDefault;
                    }
                    return true;
                }
            }

            public CompareExchangeInfo(ImmutableArray<BoundExpression> arguments, ImmutableArray<VisitArgumentResult> results, ImmutableArray<int> argsToParamsOpt)
            {
                Arguments = arguments;
                Results = results;
                ArgsToParamsOpt = argsToParamsOpt;
            }
        }

        private sealed class MethodInferenceExtensions : MethodTypeInferrer.Extensions
        {
            private readonly NullableWalker _walker;

            internal MethodInferenceExtensions(NullableWalker walker)
            {
                _walker = walker;
            }

            internal override TypeWithAnnotations GetTypeWithAnnotations(BoundExpression expr)
            {
                return TypeWithAnnotations.Create(expr.Type, GetNullableAnnotation(expr));
            }

            private static NullableAnnotation GetNullableAnnotation(BoundExpression expr)
            {
                switch (expr.Kind)
                {
                    case BoundKind.DefaultLiteral:
                    case BoundKind.DefaultExpression:
                    case BoundKind.Literal:
                        {
                            ConstantValue? constantValue = expr.ConstantValue;
                            if ((object)constantValue != null && !constantValue!.IsNull)
                            {
                                return NullableAnnotation.Annotated;
                            }
                            return NullableAnnotation.NotAnnotated;
                        }
                    case BoundKind.ExpressionWithNullability:
                        return ((BoundExpressionWithNullability)expr).NullableAnnotation;
                    case BoundKind.MethodGroup:
                    case BoundKind.UnconvertedObjectCreationExpression:
                    case BoundKind.UnboundLambda:
                        return NullableAnnotation.NotAnnotated;
                    default:
                        return NullableAnnotation.Oblivious;
                }
            }

            internal override TypeWithAnnotations GetMethodGroupResultType(BoundMethodGroup group, MethodSymbol method)
            {
                if (_walker.TryGetMethodGroupReceiverNullability(group.ReceiverOpt, out var type) && !method.IsStatic)
                {
                    method = (MethodSymbol)AsMemberOfType(type.Type, method);
                }
                return method.ReturnTypeWithAnnotations;
            }
        }

        private readonly struct DeconstructionVariable
        {
            internal readonly BoundExpression Expression;

            internal readonly TypeWithAnnotations Type;

            internal readonly ArrayBuilder<DeconstructionVariable>? NestedVariables;

            internal DeconstructionVariable(BoundExpression expression, TypeWithAnnotations type)
            {
                Expression = expression;
                Type = type;
                NestedVariables = null;
            }

            internal DeconstructionVariable(BoundExpression expression, ArrayBuilder<DeconstructionVariable> nestedVariables)
            {
                Expression = expression;
                Type = default(TypeWithAnnotations);
                NestedVariables = nestedVariables;
            }
        }

        public sealed class LocalStateSnapshot
        {
            internal readonly int Id;

            internal readonly LocalStateSnapshot? Container;

            internal readonly BitVector State;

            public LocalStateSnapshot(int id, LocalStateSnapshot? container, BitVector state)
            {
                Id = id;
                Container = container;
                State = state;
            }
        }public struct LocalState : LocalDataFlowPass<LocalState, LocalFunctionState>.ILocalDataFlowState, AbstractFlowPass<LocalState, LocalFunctionState>.ILocalState
        {
            private sealed class Boxed
            {
                internal LocalState Value;

                internal Boxed(LocalState value)
                {
                    Value = value;
                }
            }

            internal readonly int Id;

            private readonly Boxed? _container;

            private BitVector _state;

            public bool Reachable => _state[0];

            public bool NormalizeToBottom => false;

            private int Capacity => _state.Capacity / 2;

            public NullableFlowState this[int slot]
            {
                get
                {
                    var (id, index) = Variables.DeconstructSlot(slot);
                    return GetValue(id, index);
                }
                set
                {
                    var (id, index) = Variables.DeconstructSlot(slot);
                    SetValue(id, index, value);
                }
            }

            private LocalState(int id, Boxed? container, BitVector state)
            {
                Id = id;
                _container = container;
                _state = state;
            }

            internal static LocalState Create(LocalStateSnapshot snapshot)
            {
                Boxed? container = ((snapshot.Container == null) ? null : new Boxed(Create(snapshot.Container)));
                return new LocalState(snapshot.Id, container, snapshot.State.Clone());
            }

            internal LocalStateSnapshot CreateSnapshot()
            {
                return new LocalStateSnapshot(Id, _container?.Value.CreateSnapshot(), _state.Clone());
            }

            public static LocalState ReachableState(Variables variables)
            {
                return CreateReachableOrUnreachableState(variables, reachable: true);
            }

            public static LocalState UnreachableState(Variables variables)
            {
                return CreateReachableOrUnreachableState(variables, reachable: false);
            }

            private static LocalState CreateReachableOrUnreachableState(Variables variables, bool reachable)
            {
                Boxed? container = ((variables.Container == null) ? null : new Boxed(CreateReachableOrUnreachableState(variables.Container, reachable)));
                int capacity = ((!reachable) ? 1 : variables.NextAvailableIndex);
                return new LocalState(variables.Id, container, CreateBitVector(capacity, reachable));
            }

            public LocalState CreateNestedMethodState(Variables variables)
            {
                return new LocalState(variables.Id, new Boxed(this), CreateBitVector(variables.NextAvailableIndex, reachable: true));
            }

            private static BitVector CreateBitVector(int capacity, bool reachable)
            {
                if (capacity < 1)
                {
                    capacity = 1;
                }
                BitVector result = BitVector.Create(capacity * 2);
                result[0] = reachable;
                return result;
            }

            private void EnsureCapacity(int capacity)
            {
                _state.EnsureCapacity(capacity * 2);
            }

            public bool HasVariable(int slot)
            {
                if (slot <= 0)
                {
                    return false;
                }
                var (id, index) = Variables.DeconstructSlot(slot);
                return HasVariable(id, index);
            }

            private bool HasVariable(int id, int index)
            {
                if (Id > id)
                {
                    return _container!.Value.HasValue(id, index);
                }
                return Id == id;
            }

            public bool HasValue(int slot)
            {
                if (slot <= 0)
                {
                    return false;
                }
                var (id, index) = Variables.DeconstructSlot(slot);
                return HasValue(id, index);
            }

            private bool HasValue(int id, int index)
            {
                if (Id != id)
                {
                    return _container!.Value.HasValue(id, index);
                }
                return index < Capacity;
            }

            public void Normalize(NullableWalker walker, Variables variables)
            {
                if (Id != variables.Id)
                {
                    Normalize(walker, variables.Container);
                    return;
                }
                _container?.Value.Normalize(walker, variables.Container);
                int capacity = Capacity;
                EnsureCapacity(variables.NextAvailableIndex);
                Populate(walker, capacity);
            }

            public void PopulateAll(NullableWalker walker)
            {
                _container?.Value.PopulateAll(walker);
                Populate(walker, 1);
            }

            private void Populate(NullableWalker walker, int start)
            {
                int capacity = Capacity;
                for (int i = start; i < capacity; i++)
                {
                    int slot = Variables.ConstructSlot(Id, i);
                    SetValue(Id, i, walker.GetDefaultState(ref this, slot));
                }
            }

            private NullableFlowState GetValue(int id, int index)
            {
                if (Id != id)
                {
                    return _container!.Value.GetValue(id, index);
                }
                if (index < Capacity && Reachable)
                {
                    index *= 2;
                    bool num = _state[index + 1];
                    bool flag = _state[index];
                    if (!num)
                    {
                        if (!flag)
                        {
                            return NullableFlowState.NotNull;
                        }
                        return NullableFlowState.MaybeNull;
                    }
                    if (!flag)
                    {
                        throw ExceptionUtilities.UnexpectedValue(index);
                    }
                    return NullableFlowState.MaybeDefault;
                }
                return NullableFlowState.NotNull;
            }

            private void SetValue(int id, int index, NullableFlowState value)
            {
                if (Id != id)
                {
                    _container!.Value.SetValue(id, index, value);
                }
                else if (Reachable)
                {
                    index *= 2;
                    _state[index] = value != NullableFlowState.NotNull;
                    _state[index + 1] = value == NullableFlowState.MaybeDefault;
                }
            }

            internal void ForEach<TArg>(Action<int, TArg> action, TArg arg)
            {
                _container?.Value.ForEach(action, arg);
                for (int i = 1; i < Capacity; i++)
                {
                    action(Variables.ConstructSlot(Id, i), arg);
                }
            }

            internal LocalState GetStateForVariables(int id)
            {
                LocalState result = this;
                while (result.Id != id)
                {
                    result = result._container!.Value;
                }
                return result;
            }

            public LocalState Clone()
            {
                Boxed container = ((_container == null) ? null : new Boxed(_container!.Value.Clone()));
                return new LocalState(Id, container, _state.Clone());
            }

            public bool Join(in LocalState other)
            {
                bool result = false;
                if (_container != null && _container!.Value.Join(in other._container!.Value))
                {
                    result = true;
                }
                if (_state.UnionWith(in other._state))
                {
                    result = true;
                }
                return result;
            }

            public bool Meet(in LocalState other)
            {
                bool result = false;
                if (_container != null && _container!.Value.Meet(in other._container!.Value))
                {
                    result = true;
                }
                if (_state.IntersectWith(in other._state))
                {
                    result = true;
                }
                return result;
            }

            internal string GetDebuggerDisplay()
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                StringBuilder builder = instance.Builder;
                builder.Append(" ");
                for (int num = Math.Min(Capacity, 8) - 1; num >= 0; num--)
                {
                    builder.Append(_state[num * 2] ? '?' : '!');
                }
                return instance.ToStringAndFree();
            }

            internal string Dump(Variables variables)
            {
                if (!Reachable)
                {
                    return "unreachable";
                }
                if (Id != variables.Id)
                {
                    return "invalid";
                }
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                Dump(instance, variables);
                return instance.ToStringAndFree();
            }

            private void Dump(StringBuilder builder, Variables variables)
            {
                Variables variables2 = variables;
                _container?.Value.Dump(builder, variables2.Container);
                for (int i = 1; i < Capacity; i++)
                {
                    string text = getName(Variables.ConstructSlot(Id, i));
                    if (text != null)
                    {
                        builder.Append(text);
                        builder.Append(GetValue(Id, i) switch
                        {
                            NullableFlowState.MaybeNull => "?",
                            NullableFlowState.MaybeDefault => "??",
                            _ => "!",
                        });
                    }
                }
                string? getName(int slot)
                {
                    LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier variableIdentifier = variables2[slot];
                    string name = variableIdentifier.Symbol.Name;
                    int containingSlot = variableIdentifier.ContainingSlot;
                    if (containingSlot <= 0)
                    {
                        return name;
                    }
                    return getName(containingSlot) + "." + name;
                }
            }
        }

        public sealed class LocalFunctionState : AbstractLocalFunctionState
        {
            public LocalState StartingState;

            public LocalFunctionState(LocalState unreachableState)
                : base(unreachableState.Clone(), unreachableState.Clone())
            {
                StartingState = unreachableState;
            }
        }

        private sealed class NullabilityInfoTypeComparer : IEqualityComparer<(NullabilityInfo info, TypeSymbol? type)>
        {
            public static readonly NullabilityInfoTypeComparer Instance = new NullabilityInfoTypeComparer();

            public bool Equals((NullabilityInfo info, TypeSymbol? type) x, (NullabilityInfo info, TypeSymbol? type) y)
            {
                if (x.info.Equals(y.info))
                {
                    return Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(x.type, y.type);
                }
                return false;
            }

            public int GetHashCode((NullabilityInfo info, TypeSymbol? type) obj)
            {
                return obj.GetHashCode();
            }
        }

        private sealed class ExpressionAndSymbolEqualityComparer : IEqualityComparer<(BoundNode? expr, Symbol symbol)>
        {
            internal static readonly ExpressionAndSymbolEqualityComparer Instance = new ExpressionAndSymbolEqualityComparer();

            private ExpressionAndSymbolEqualityComparer()
            {
            }

            public bool Equals((BoundNode? expr, Symbol symbol) x, (BoundNode? expr, Symbol symbol) y)
            {
                if (x.expr == y.expr)
                {
                    return (object)x.symbol == y.symbol;
                }
                return false;
            }

            public int GetHashCode((BoundNode? expr, Symbol symbol) obj)
            {
                return Hash.Combine(obj.expr, obj.symbol.GetHashCode());
            }
        }

        private sealed class PlaceholderLocal : LocalSymbol
        {
            private readonly Symbol _containingSymbol;

            private readonly TypeWithAnnotations _type;

            private readonly object _identifier;

            internal override SyntaxNode ScopeDesignatorOpt => null;

            public override Symbol ContainingSymbol => _containingSymbol;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public override TypeWithAnnotations TypeWithAnnotations => _type;

            internal override LocalDeclarationKind DeclarationKind => LocalDeclarationKind.None;

            internal override SyntaxToken IdentifierToken
            {
                get
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }

            internal override bool IsCompilerGenerated => true;

            internal override bool IsImportedFromMetadata => false;

            internal override bool IsPinned => false;

            public override RefKind RefKind => RefKind.None;

            internal override SynthesizedLocalKind SynthesizedKind
            {
                get
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }

            internal override uint ValEscapeScope
            {
                get
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }

            internal override uint RefEscapeScope
            {
                get
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }

            public PlaceholderLocal(Symbol containingSymbol, object identifier, TypeWithAnnotations type)
            {
                _containingSymbol = containingSymbol;
                _type = type;
                _identifier = identifier;
            }

            public override bool Equals(Symbol obj, TypeCompareKind compareKind)
            {
                if ((object)this == obj)
                {
                    return true;
                }
                if (obj is PlaceholderLocal placeholderLocal)
                {
                    return _identifier.Equals(placeholderLocal._identifier);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return _identifier.GetHashCode();
            }

            internal override ConstantValue GetConstantValue(SyntaxNode node, LocalSymbol inProgress, BindingDiagnosticBag diagnostics = null)
            {
                return null;
            }

            internal override ImmutableBindingDiagnostic<AssemblySymbol> GetConstantValueDiagnostics(BoundExpression boundInitValue)
            {
                return ImmutableBindingDiagnostic<AssemblySymbol>.Empty;
            }

            internal override SyntaxNode GetDeclaratorSyntax()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override LocalSymbol WithSynthesizedLocalKindAndSyntax(SynthesizedLocalKind kind, SyntaxNode syntax)
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal sealed class SnapshotManager
        {
            internal sealed class Builder
            {
                private readonly ImmutableDictionary<(BoundNode?, Symbol), Symbol>.Builder _updatedSymbolMap = ImmutableDictionary.CreateBuilder(ExpressionAndSymbolEqualityComparer.Instance, Symbols.SymbolEqualityComparer.ConsiderEverything);

                private readonly ArrayBuilder<SharedWalkerState> _walkerStates = ArrayBuilder<SharedWalkerState>.GetInstance();

                private readonly SortedDictionary<int, Snapshot> _incrementalSnapshots = new SortedDictionary<int, Snapshot>();

                private readonly PooledDictionary<Symbol, int> _symbolToSlot = PooledDictionary<Symbol, int>.GetInstance();

                private int _currentWalkerSlot = -1;

                internal SnapshotManager ToManagerAndFree()
                {
                    _symbolToSlot.Free();
                    ImmutableArray<(int, Snapshot)> incrementalSnapshots = _incrementalSnapshots.SelectAsArray((KeyValuePair<int, Snapshot> kvp) => (kvp.Key, kvp.Value));
                    ImmutableDictionary<(BoundNode, Symbol), Symbol> updatedSymbolsMap = _updatedSymbolMap.ToImmutable();
                    return new SnapshotManager(_walkerStates.ToImmutableAndFree(), incrementalSnapshots, updatedSymbolsMap);
                }

                internal int EnterNewWalker(Symbol symbol)
                {
                    int currentWalkerSlot = _currentWalkerSlot;
                    if (_symbolToSlot.TryGetValue(symbol, out var value))
                    {
                        _currentWalkerSlot = value;
                        return currentWalkerSlot;
                    }
                    _currentWalkerSlot = _symbolToSlot.Count;
                    _symbolToSlot.Add(symbol, _currentWalkerSlot);
                    return currentWalkerSlot;
                }

                internal void ExitWalker(SharedWalkerState stableState, int previousSlot)
                {
                    _walkerStates.SetItem(_currentWalkerSlot, stableState);
                    _currentWalkerSlot = previousSlot;
                }

                internal void TakeIncrementalSnapshot(BoundNode? node, LocalState currentState)
                {
                    if (node != null && !node!.WasCompilerGenerated)
                    {
                        _incrementalSnapshots[node!.Syntax.SpanStart] = new Snapshot(currentState.CreateSnapshot(), _currentWalkerSlot);
                    }
                }

                internal void SetUpdatedSymbol(BoundNode node, Symbol originalSymbol, Symbol updatedSymbol)
                {
                    _updatedSymbolMap[GetKey(node, originalSymbol)] = updatedSymbol;
                }

                internal void RemoveSymbolIfPresent(BoundNode node, Symbol symbol)
                {
                    _updatedSymbolMap.Remove(GetKey(node, symbol));
                }

                private static (BoundNode?, Symbol) GetKey(BoundNode node, Symbol symbol)
                {
                    if (node is BoundLambda && symbol is LambdaSymbol)
                    {
                        return (null, symbol);
                    }
                    return (node, symbol);
                }
            }

            private readonly ImmutableArray<SharedWalkerState> _walkerSharedStates;

            private readonly ImmutableArray<(int position, Snapshot snapshot)> _incrementalSnapshots;

            private readonly ImmutableDictionary<(BoundNode?, Symbol), Symbol> _updatedSymbolsMap;

            private static readonly Func<(int position, Snapshot snapshot), int, int> BinarySearchComparer = ((int position, Snapshot snapshot) current, int target) => current.position.CompareTo(target);

            private SnapshotManager(ImmutableArray<SharedWalkerState> walkerSharedStates, ImmutableArray<(int position, Snapshot snapshot)> incrementalSnapshots, ImmutableDictionary<(BoundNode?, Symbol), Symbol> updatedSymbolsMap)
            {
                _walkerSharedStates = walkerSharedStates;
                _incrementalSnapshots = incrementalSnapshots;
                _updatedSymbolsMap = updatedSymbolsMap;
            }

            internal (VariablesSnapshot, LocalStateSnapshot) GetSnapshot(int position)
            {
                Snapshot snapshotForPosition = GetSnapshotForPosition(position);
                return (_walkerSharedStates[snapshotForPosition.SharedStateIndex].Variables, snapshotForPosition.VariableState);
            }

            internal TypeWithAnnotations? GetUpdatedTypeForLocalSymbol(SourceLocalSymbol symbol)
            {
                Snapshot snapshotForPosition = GetSnapshotForPosition(symbol.IdentifierToken.SpanStart);
                if (_walkerSharedStates[snapshotForPosition.SharedStateIndex].Variables.TryGetType(symbol, out var type))
                {
                    return type;
                }
                return null;
            }

            internal NamedTypeSymbol? GetUpdatedDelegateTypeForLambda(LambdaSymbol lambda)
            {
                if (_updatedSymbolsMap.TryGetValue((null, lambda), out var value))
                {
                    return (NamedTypeSymbol)value;
                }
                return null;
            }

            internal bool TryGetUpdatedSymbol(BoundNode node, Symbol symbol, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Symbol? updatedSymbol)
            {
                return _updatedSymbolsMap.TryGetValue((node, symbol), out updatedSymbol);
            }

            private Snapshot GetSnapshotForPosition(int position)
            {
                int num = _incrementalSnapshots.BinarySearch(position, BinarySearchComparer);
                if (num < 0)
                {
                    num = ~num - 1;
                    if (num < 0)
                    {
                        num = 0;
                    }
                }
                return _incrementalSnapshots[num].snapshot;
            }
        }

        internal struct SharedWalkerState
        {
            internal readonly VariablesSnapshot Variables;

            internal SharedWalkerState(VariablesSnapshot variables)
            {
                Variables = variables;
            }
        }

        private readonly struct Snapshot
        {
            internal readonly LocalStateSnapshot VariableState;

            internal readonly int SharedStateIndex;

            internal Snapshot(LocalStateSnapshot variableState, int sharedStateIndex)
            {
                VariableState = variableState;
                SharedStateIndex = sharedStateIndex;
            }
        }public sealed class VariablesSnapshot
        {
            internal readonly int Id;

            internal readonly VariablesSnapshot? Container;

            internal readonly Symbol? Symbol;

            internal readonly ImmutableArray<KeyValuePair<LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier, int>> VariableSlot;

            internal readonly ImmutableDictionary<Symbol, TypeWithAnnotations> VariableTypes;

            public VariablesSnapshot(int id, VariablesSnapshot? container, Symbol? symbol, ImmutableArray<KeyValuePair<LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier, int>> variableSlot, ImmutableDictionary<Symbol, TypeWithAnnotations> variableTypes)
            {
                Id = id;
                Container = container;
                Symbol = symbol;
                VariableSlot = variableSlot;
                VariableTypes = variableTypes;
            }

            internal bool TryGetType(Symbol symbol, out TypeWithAnnotations type)
            {
                return VariableTypes.TryGetValue(symbol, out type);
            }

            private string GetDebuggerDisplay()
            {
                object arg = Symbol ?? ((object)"<null>");
                return $"Id={Id}, Symbol={arg}, Count={VariableSlot.Length}";
            }
        }public sealed class Variables
        {
            private const int MaxSlotDepth = 5;

            private const int IdOffset = 16;

            private const int IdMask = 32767;

            private const int IndexMask = 65535;

            internal readonly int Id;

            internal readonly Variables? Container;

            internal readonly Symbol? Symbol;

            private readonly PooledDictionary<LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier, int> _variableSlot = PooledDictionary<VariableIdentifier, int>.GetInstance();

            private readonly PooledDictionary<Symbol, TypeWithAnnotations> _variableTypes = SpecializedSymbolCollections.GetPooledSymbolDictionaryInstance<Symbol, TypeWithAnnotations>();

            private readonly ArrayBuilder<LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier> _variableBySlot = ArrayBuilder<VariableIdentifier>.GetInstance(1, default(VariableIdentifier));

            internal LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier this[int slot]
            {
                get
                {
                    var (id, index) = DeconstructSlot(slot);
                    return GetVariablesForId(id)!._variableBySlot[index];
                }
            }

            internal int NextAvailableIndex => _variableBySlot.Count;

            internal static Variables Create(Symbol? symbol)
            {
                return new Variables(0, null, symbol);
            }

            internal static Variables Create(VariablesSnapshot snapshot)
            {
                Variables container = ((snapshot.Container == null) ? null : Create(snapshot.Container));
                Variables variables = new Variables(snapshot.Id, container, snapshot.Symbol);
                variables.Populate(snapshot);
                return variables;
            }

            private int GetNextId()
            {
                return Id + 1;
            }

            private void Populate(VariablesSnapshot snapshot)
            {
                _variableBySlot.AddMany(default(VariableIdentifier), snapshot.VariableSlot.Length);
                ImmutableArray<KeyValuePair<VariableIdentifier, int>>.Enumerator enumerator = snapshot.VariableSlot.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<VariableIdentifier, int> current = enumerator.Current;
                    VariableIdentifier key = current.Key;
                    int value = current.Value;
                    _variableSlot.Add(key, value);
                    _variableBySlot[value] = key;
                }
                foreach (KeyValuePair<Symbol, TypeWithAnnotations> variableType in snapshot.VariableTypes)
                {
                    _variableTypes.Add(variableType.Key, variableType.Value);
                }
            }

            private Variables(int id, Variables? container, Symbol? symbol)
            {
                Id = id;
                Container = container;
                Symbol = symbol;
            }

            internal void Free()
            {
                Container?.Free();
                _variableBySlot.Free();
                _variableTypes.Free();
                _variableSlot.Free();
            }

            internal VariablesSnapshot CreateSnapshot()
            {
                return new VariablesSnapshot(Id, Container?.CreateSnapshot(), Symbol, ImmutableArray.CreateRange(_variableSlot), ImmutableDictionary.CreateRange(_variableTypes));
            }

            internal Variables CreateNestedMethodScope(MethodSymbol method)
            {
                return new Variables(GetNextId(), this, method);
            }

            internal int RootSlot(int slot)
            {
                while (true)
                {
                    int containingSlot = this[slot].ContainingSlot;
                    if (containingSlot == 0)
                    {
                        break;
                    }
                    slot = containingSlot;
                }
                return slot;
            }

            internal bool TryGetValue(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier, out int slot)
            {
                return GetVariablesForVariable(identifier).TryGetValueInternal(identifier, out slot);
            }

            private bool TryGetValueInternal(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier, out int slot)
            {
                if (_variableSlot.TryGetValue(identifier, out var value))
                {
                    slot = ConstructSlot(Id, value);
                    return true;
                }
                slot = -1;
                return false;
            }

            internal int Add(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier)
            {
                return GetVariablesForVariable(identifier).AddInternal(identifier);
            }

            private int AddInternal(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier)
            {
                if (getSlotDepth(identifier.ContainingSlot) >= 5)
                {
                    return -1;
                }
                int nextAvailableIndex = NextAvailableIndex;
                if (nextAvailableIndex > 65535)
                {
                    return -1;
                }
                _variableSlot.Add(identifier, nextAvailableIndex);
                _variableBySlot.Add(identifier);
                return ConstructSlot(Id, nextAvailableIndex);
                int getSlotDepth(int slot)
                {
                    int num = 0;
                    while (slot > 0)
                    {
                        num++;
                        int item = DeconstructSlot(slot).Index;
                        slot = _variableBySlot[item].ContainingSlot;
                    }
                    return num;
                }
            }

            internal bool TryGetType(Symbol symbol, out TypeWithAnnotations type)
            {
                return GetVariablesContainingSymbol(symbol)._variableTypes.TryGetValue(symbol, out type);
            }

            internal void SetType(Symbol symbol, TypeWithAnnotations type)
            {
                GetVariablesContainingSymbol(symbol)._variableTypes[symbol] = type;
            }

            internal int GetTotalVariableCount()
            {
                return (Container?.GetTotalVariableCount() ?? 0) + _variableSlot.Count;
            }

            internal void GetMembers(ArrayBuilder<(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier, int)> builder, int containingSlot)
            {
                (int Id, int Index) tuple = DeconstructSlot(containingSlot);
                int item = tuple.Id;
                int item2 = tuple.Index;
                ArrayBuilder<VariableIdentifier> variableBySlot = GetVariablesForId(item)!._variableBySlot;
                for (item2++; item2 < variableBySlot.Count; item2++)
                {
                    VariableIdentifier item3 = variableBySlot[item2];
                    if (item3.ContainingSlot == containingSlot)
                    {
                        builder.Add((item3, ConstructSlot(item, item2)));
                    }
                }
            }

            private Variables GetVariablesForVariable(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier)
            {
                int containingSlot = identifier.ContainingSlot;
                if (containingSlot > 0)
                {
                    return GetVariablesForId(DeconstructSlot(containingSlot).Id);
                }
                return GetVariablesContainingSymbol(identifier.Symbol);
            }

            private Variables GetVariablesContainingSymbol(Symbol symbol)
            {
                if ((symbol is LocalSymbol || symbol is ParameterSymbol) && symbol.ContainingSymbol is MethodSymbol method)
                {
                    Variables variablesForMethodScope = GetVariablesForMethodScope(method);
                    if (variablesForMethodScope != null)
                    {
                        return variablesForMethodScope;
                    }
                }
                return GetRootScope();
            }

            internal Variables GetRootScope()
            {
                Variables variables = this;
                while (true)
                {
                    Variables container = variables.Container;
                    if (container == null)
                    {
                        break;
                    }
                    variables = container;
                }
                return variables;
            }

            private Variables? GetVariablesForId(int id)
            {
                Variables variables = this;
                do
                {
                    if (variables.Id == id)
                    {
                        return variables;
                    }
                    variables = variables.Container;
                }
                while (variables != null);
                return null;
            }

            internal Variables? GetVariablesForMethodScope(MethodSymbol method)
            {
                method = method.PartialImplementationPart ?? method;
                Variables variables = this;
                do
                {
                    if ((object)method == variables.Symbol)
                    {
                        return variables;
                    }
                    variables = variables.Container;
                }
                while (variables != null);
                return null;
            }

            internal static int ConstructSlot(int id, int index)
            {
                if (index >= 0)
                {
                    return (id << 16) | index;
                }
                return index;
            }

            internal static (int Id, int Index) DeconstructSlot(int slot)
            {
                if (slot >= 0)
                {
                    return ((slot >> 16) & 0x7FFF, slot & 0xFFFF);
                }
                return (0, slot);
            }

            private string GetDebuggerDisplay()
            {
                object arg = Symbol ?? ((object)"<null>");
                return $"Id={Id}, Symbol={arg}, Count={_variableSlot.Count}";
            }
        }

        private struct PossiblyConditionalState
        {
            public LocalState State;

            public LocalState StateWhenTrue;

            public LocalState StateWhenFalse;

            public bool IsConditionalState;

            public PossiblyConditionalState(LocalState stateWhenTrue, LocalState stateWhenFalse)
            {
                StateWhenTrue = stateWhenTrue.Clone();
                StateWhenFalse = stateWhenFalse.Clone();
                IsConditionalState = true;
                State = default(LocalState);
            }

            public PossiblyConditionalState(LocalState state)
            {
                StateWhenTrue = (StateWhenFalse = default(LocalState));
                IsConditionalState = false;
                State = state.Clone();
            }

            public static PossiblyConditionalState Create(NullableWalker nullableWalker)
            {
                if (!nullableWalker.IsConditionalState)
                {
                    return new PossiblyConditionalState(nullableWalker.State);
                }
                return new PossiblyConditionalState(nullableWalker.StateWhenTrue, nullableWalker.StateWhenFalse);
            }

            public PossiblyConditionalState Clone()
            {
                if (!IsConditionalState)
                {
                    return new PossiblyConditionalState(State);
                }
                return new PossiblyConditionalState(StateWhenTrue, StateWhenFalse);
            }
        }

        private Variables _variables;

        private readonly Binder _binder;

        private readonly Conversions _conversions;

        private readonly bool _useConstructorExitWarnings;

        private bool _useDelegateInvokeParameterTypes;

        private MethodSymbol? _delegateInvokeMethod;

        private ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>? _returnTypesOpt;

        private static readonly TypeWithState _invalidType = TypeWithState.Create(ErrorTypeSymbol.UnknownResultType, NullableFlowState.NotNull);

        private readonly ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)>.Builder? _analyzedNullabilityMapOpt;

        private readonly SnapshotManager.Builder? _snapshotBuilderOpt;

        private bool _disableNullabilityAnalysis;

        private PooledDictionary<BoundExpression, TypeWithState>? _methodGroupReceiverMapOpt;

        private PooledDictionary<BoundAwaitableValuePlaceholder, (BoundExpression AwaitableExpression, VisitResult Result)>? _awaitablePlaceholdersOpt;

        private PooledDictionary<MethodSymbol, Variables>? _nestedFunctionVariables;

        private PooledDictionary<BoundExpression, ImmutableArray<(LocalState State, TypeWithState ResultType, bool EndReachable)>>? _conditionalInfoForConversionOpt;

        private readonly bool _isSpeculative;

        private readonly bool _hasInitialState;

        private VisitResult _visitResult;

        private VisitResult _currentConditionalReceiverVisitResult;

        private PooledDictionary<object, PlaceholderLocal>? _placeholderLocalsOpt;

        private bool _disableDiagnostics;

        private bool _expressionIsRead = true;

        private int _lastConditionalAccessSlot = -1;

        private PooledDictionary<BoundExpression, ImmutableArray<(LocalState State, TypeWithState ResultType, bool EndReachable)>> ConditionalInfoForConversion => _conditionalInfoForConversionOpt ?? (_conditionalInfoForConversionOpt = PooledDictionary<BoundExpression, ImmutableArray<(LocalState, TypeWithState, bool)>>.GetInstance());

        private TypeWithState ResultType => _visitResult.RValueType;

        private TypeWithAnnotations LvalueResultType => _visitResult.LValueType;

        private bool IsAnalyzingAttribute => methodMainNode.Kind == BoundKind.Attribute;

        public sealed override bool AwaitUsingAndForeachAddsPendingBranch => true;

        private void SetResultType(BoundExpression? expression, TypeWithState type, bool updateAnalyzedNullability = true)
        {
            SetResult(expression, type, type.ToTypeWithAnnotations(compilation), updateAnalyzedNullability);
        }

        private void UseRvalueOnly(BoundExpression? expression)
        {
            SetResult(expression, ResultType, ResultType.ToTypeWithAnnotations(compilation), updateAnalyzedNullability: true, false);
        }

        private void SetLvalueResultType(BoundExpression? expression, TypeWithAnnotations type)
        {
            SetResult(expression, type.ToTypeWithState(), type);
        }

        private void UseLvalueOnly(BoundExpression? expression)
        {
            SetResult(expression, LvalueResultType.ToTypeWithState(), LvalueResultType, updateAnalyzedNullability: true, true);
        }

        private void SetInvalidResult()
        {
            SetResult(null, _invalidType, _invalidType.ToTypeWithAnnotations(compilation), updateAnalyzedNullability: false);
        }

        private void SetResult(BoundExpression? expression, TypeWithState resultType, TypeWithAnnotations lvalueType, bool updateAnalyzedNullability = true, bool? isLvalue = null)
        {
            _visitResult = new VisitResult(resultType, lvalueType);
            if (updateAnalyzedNullability)
            {
                SetAnalyzedNullability(expression, _visitResult, isLvalue);
            }
        }

        private bool ShouldMakeNotNullRvalue(BoundExpression node)
        {
            if (!node.IsSuppressed && !node.HasAnyErrors)
            {
                return !IsReachable();
            }
            return true;
        }

        private void SetAnalyzedNullability(BoundExpression? expr, VisitResult result, bool? isLvalue = null)
        {
            if (expr != null && !_disableNullabilityAnalysis && _analyzedNullabilityMapOpt != null)
            {
                ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)>.Builder? analyzedNullabilityMapOpt = _analyzedNullabilityMapOpt;
                NullabilityInfo item = new NullabilityInfo(result.LValueType.ToPublicAnnotation(), result.RValueType.State.ToPublicFlowState());
                TypeSymbol? type = expr!.Type;
                analyzedNullabilityMapOpt![expr] = (item, ((object)type != null && type!.Equals(result.RValueType.Type, TypeCompareKind.AllIgnoreOptions)) ? result.RValueType.Type : expr!.Type);
            }
        }

        protected override void Free()
        {
            _nestedFunctionVariables?.Free();
            _awaitablePlaceholdersOpt?.Free();
            _methodGroupReceiverMapOpt?.Free();
            _placeholderLocalsOpt?.Free();
            _variables.Free();
            _conditionalInfoForConversionOpt?.Free();
            base.Free();
        }

        private NullableWalker(CSharpCompilation compilation, Symbol? symbol, bool useConstructorExitWarnings, bool useDelegateInvokeParameterTypes, MethodSymbol? delegateInvokeMethodOpt, BoundNode node, Binder binder, Conversions conversions, Variables? variables, ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>? returnTypesOpt, ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>.Builder? analyzedNullabilityMapOpt, SnapshotManager.Builder? snapshotBuilderOpt, bool isSpeculative = false)
            : base(compilation, symbol, node, EmptyStructTypeCache.CreatePrecise(), trackUnassignments: true)
        {
            _variables = variables ?? Variables.Create(symbol);
            _binder = binder;
            _conversions = (Conversions)conversions.WithNullability(includeNullability: true);
            _useConstructorExitWarnings = useConstructorExitWarnings;
            _useDelegateInvokeParameterTypes = useDelegateInvokeParameterTypes;
            _delegateInvokeMethod = delegateInvokeMethodOpt;
            _analyzedNullabilityMapOpt = analyzedNullabilityMapOpt;
            _returnTypesOpt = returnTypesOpt;
            _snapshotBuilderOpt = snapshotBuilderOpt;
            _isSpeculative = isSpeculative;
            _hasInitialState = variables != null;
        }

        public string GetDebuggerDisplay()
        {
            if (IsConditionalState)
            {
                return "{" + GetType().Name + " WhenTrue:" + Dump(StateWhenTrue) + " WhenFalse:" + Dump(StateWhenFalse) + "}";
            }
            return "{" + GetType().Name + " " + Dump(State) + "}";
        }

        protected override void EnsureSufficientExecutionStack(int recursionDepth)
        {
            if (recursionDepth > 20)
            {
                CSharpCompilation.NullableData nullableAnalysisData = compilation.NullableAnalysisData;
                if (nullableAnalysisData != null)
                {
                    int maxRecursionDepth = nullableAnalysisData.MaxRecursionDepth;
                    if (maxRecursionDepth > 0 && recursionDepth > maxRecursionDepth)
                    {
                        throw new InsufficientExecutionStackException();
                    }
                }
            }
            base.EnsureSufficientExecutionStack(recursionDepth);
        }

        protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
        {
            return true;
        }

        protected override bool TryGetVariable(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier, out int slot)
        {
            return _variables.TryGetValue(identifier, out slot);
        }

        protected override int AddVariable(LocalDataFlowPass<LocalState, LocalFunctionState>.VariableIdentifier identifier)
        {
            return _variables.Add(identifier);
        }

        protected override ImmutableArray<AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch> Scan(ref bool badRegion)
        {
            if (_returnTypesOpt != null)
            {
                _returnTypesOpt!.Clear();
            }
            base.Diagnostics.Clear();
            regionPlace = RegionPlace.Before;
            if (!_isSpeculative)
            {
                ParameterSymbol methodThisParameter = base.MethodThisParameter;
                EnterParameters();
                if ((object)methodThisParameter != null)
                {
                    EnterParameter(methodThisParameter, methodThisParameter.TypeWithAnnotations);
                }
                makeNotNullMembersMaybeNull();
                _snapshotBuilderOpt?.TakeIncrementalSnapshot(methodMainNode, State);
            }
            ImmutableArray<PendingBranch> result = base.Scan(ref badRegion);
            MethodSymbol obj = _symbol as MethodSymbol;
            if ((object)obj == null || !obj.IsConstructor() || _useConstructorExitWarnings)
            {
                EnforceDoesNotReturn(null);
                enforceMemberNotNull(null, State);
                enforceNotNull(null, State);
                ImmutableArray<PendingBranch>.Enumerator enumerator = result.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PendingBranch current = enumerator.Current;
                    enforceMemberNotNull(current.Branch.Syntax, current.State);
                    if (current.Branch is BoundReturnStatement boundReturnStatement)
                    {
                        enforceNotNull(boundReturnStatement.Syntax, current.State);
                        enforceNotNullWhenForPendingReturn(current, boundReturnStatement);
                        enforceMemberNotNullWhenForPendingReturn(current, boundReturnStatement);
                    }
                }
            }
            return result;
            void checkMemberStateOnConstructorExit(MethodSymbol constructor, Symbol member, LocalState state, int thisSlot, Location? exitLocation)
            {
                bool flag3 = !constructor.RequiresInstanceReceiver();
                if (member.IsStatic == flag3 && !LocalDataFlowPass<LocalState, LocalFunctionState>.HasInitializer(member))
                {
                    FieldSymbol fieldSymbol2;
                    Symbol symbol;
                    TypeWithAnnotations typeWithAnnotations;
                    if (!(member is FieldSymbol fieldSymbol))
                    {
                        if (!(member is EventSymbol eventSymbol))
                        {
                            return;
                        }
                        typeWithAnnotations = eventSymbol.TypeWithAnnotations;
                        fieldSymbol2 = eventSymbol.AssociatedField;
                        symbol = eventSymbol;
                        if ((object)fieldSymbol2 == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        typeWithAnnotations = fieldSymbol.TypeWithAnnotations;
                        fieldSymbol2 = fieldSymbol;
                        symbol = (Symbol)(fieldSymbol.AssociatedSymbol as PropertySymbol ?? ((object)fieldSymbol));
                    }
                    if (!fieldSymbol2.IsConst && !typeWithAnnotations.Type.IsValueType && !typeWithAnnotations.Type.IsErrorType())
                    {
                        FlowAnalysisAnnotations flowAnalysisAnnotations2 = symbol.GetFlowAnalysisAnnotations();
                        if ((flowAnalysisAnnotations2 & FlowAnalysisAnnotations.AllowNull) == 0)
                        {
                            typeWithAnnotations = ApplyUnconditionalAnnotations(typeWithAnnotations, flowAnalysisAnnotations2);
                            if (typeWithAnnotations.NullableAnnotation.IsNotAnnotated())
                            {
                                int orCreateSlot4 = GetOrCreateSlot(symbol, thisSlot);
                                if (orCreateSlot4 >= 0)
                                {
                                    NullableFlowState num4 = state[orCreateSlot4];
                                    NullableFlowState nullableFlowState2 = ((!typeWithAnnotations.Type.IsPossiblyNullableReferenceTypeTypeParameter() || (flowAnalysisAnnotations2 & FlowAnalysisAnnotations.NotNull) != 0) ? NullableFlowState.MaybeNull : NullableFlowState.MaybeDefault);
                                    if ((int)num4 >= (int)nullableFlowState2)
                                    {
                                        base.Diagnostics.Add(ErrorCode.WRN_UninitializedNonNullableField, exitLocation ?? symbol.Locations.FirstOrNone(), symbol.Kind.Localize(), symbol.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            void enforceMemberNotNull(SyntaxNode? syntaxOpt, LocalState state)
            {
                if (state.Reachable)
                {
                    MethodSymbol methodSymbol3 = _symbol as MethodSymbol;
                    if ((object)methodSymbol3 != null)
                    {
                        if (methodSymbol3.IsConstructor())
                        {
                            int thisSlot2 = 0;
                            if (methodSymbol3.RequiresInstanceReceiver)
                            {
                                methodSymbol3.TryGetThisParameter(out var thisParameter);
                                thisSlot2 = GetOrCreateSlot(thisParameter);
                            }
                            Location exitLocation2 = (methodSymbol3.DeclaringSyntaxReferences.IsEmpty ? null : methodSymbol3.Locations.FirstOrDefault());
                            ImmutableArray<Symbol>.Enumerator enumerator10 = methodSymbol3.ContainingType.GetMembersUnordered().GetEnumerator();
                            while (enumerator10.MoveNext())
                            {
                                Symbol current11 = enumerator10.Current;
                                checkMemberStateOnConstructorExit(methodSymbol3, current11, state, thisSlot2, exitLocation2);
                            }
                        }
                        else
                        {
                            do
                            {
                                ImmutableArray<string>.Enumerator enumerator11 = methodSymbol3.NotNullMembers.GetEnumerator();
                                while (enumerator11.MoveNext())
                                {
                                    string current12 = enumerator11.Current;
                                    enforceMemberNotNullOnMember(syntaxOpt, state, methodSymbol3, current12);
                                }
                                methodSymbol3 = methodSymbol3.OverriddenMethod;
                            }
                            while (methodSymbol3 != null);
                        }
                    }
                }
            }
            void enforceMemberNotNullOnMember(SyntaxNode? syntaxOpt, LocalState state, MethodSymbol method, string memberName)
            {
                ImmutableArray<Symbol>.Enumerator enumerator9 = method.ContainingType.GetMembers(memberName).GetEnumerator();
                while (enumerator9.MoveNext())
                {
                    Symbol current10 = enumerator9.Current;
                    if (memberHasBadState(current10, state))
                    {
                        base.Diagnostics.Add(ErrorCode.WRN_MemberNotNull, syntaxOpt?.GetLocation() ?? methodMainNode.Syntax.GetLastToken().GetLocation(), current10.Name);
                    }
                }
            }
            void enforceMemberNotNullWhen(SyntaxNode? syntaxOpt, bool sense, LocalState state)
            {
                if (_symbol is MethodSymbol methodSymbol)
                {
                    ImmutableArray<string>.Enumerator enumerator3 = (sense ? methodSymbol.NotNullWhenTrueMembers : methodSymbol.NotNullWhenFalseMembers).GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        string current3 = enumerator3.Current;
                        ImmutableArray<Symbol>.Enumerator enumerator4 = methodSymbol.ContainingType.GetMembers(current3).GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            Symbol current4 = enumerator4.Current;
                            reportMemberIfBadConditionalState(syntaxOpt, sense, current4, state);
                        }
                    }
                }
            }
            void enforceMemberNotNullWhenForPendingReturn(AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch pendingReturn, BoundReturnStatement returnStatement)
            {
                if (pendingReturn.IsConditionalState)
                {
                    BoundExpression expressionOpt = returnStatement.ExpressionOpt;
                    if (expressionOpt != null)
                    {
                        ConstantValue constantValue = expressionOpt.ConstantValue;
                        if ((object)constantValue != null && constantValue.IsBoolean)
                        {
                            bool booleanValue = constantValue.BooleanValue;
                            enforceMemberNotNullWhen(returnStatement.Syntax, booleanValue, pendingReturn.State);
                            return;
                        }
                    }
                    if (pendingReturn.StateWhenTrue.Reachable && pendingReturn.StateWhenFalse.Reachable && _symbol is MethodSymbol methodSymbol2)
                    {
                        ImmutableArray<string>.Enumerator enumerator5 = methodSymbol2.NotNullWhenTrueMembers.GetEnumerator();
                        while (enumerator5.MoveNext())
                        {
                            string current5 = enumerator5.Current;
                            enforceMemberNotNullWhenIfAffected(returnStatement.Syntax, sense: true, methodSymbol2.ContainingType.GetMembers(current5), pendingReturn.StateWhenTrue, pendingReturn.StateWhenFalse);
                        }
                        enumerator5 = methodSymbol2.NotNullWhenFalseMembers.GetEnumerator();
                        while (enumerator5.MoveNext())
                        {
                            string current6 = enumerator5.Current;
                            enforceMemberNotNullWhenIfAffected(returnStatement.Syntax, sense: false, methodSymbol2.ContainingType.GetMembers(current6), pendingReturn.StateWhenFalse, pendingReturn.StateWhenTrue);
                        }
                    }
                }
                else
                {
                    BoundExpression expressionOpt = returnStatement.ExpressionOpt;
                    if (expressionOpt != null)
                    {
                        ConstantValue constantValue = expressionOpt.ConstantValue;
                        if ((object)constantValue != null && constantValue.IsBoolean)
                        {
                            bool booleanValue2 = constantValue.BooleanValue;
                            enforceMemberNotNullWhen(returnStatement.Syntax, booleanValue2, pendingReturn.State);
                        }
                    }
                }
            }
            void enforceMemberNotNullWhenIfAffected(SyntaxNode? syntaxOpt, bool sense, ImmutableArray<Symbol> members, LocalState state, LocalState otherState)
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = members.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (memberHasBadState(current2, state) != memberHasBadState(current2, otherState))
                    {
                        reportMemberIfBadConditionalState(syntaxOpt, sense, current2, state);
                    }
                }
            }
            void enforceNotNull(SyntaxNode? syntaxOpt, LocalState state)
            {
                if (state.Reachable)
                {
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator8 = base.MethodParameters.GetEnumerator();
                    while (enumerator8.MoveNext())
                    {
                        ParameterSymbol current9 = enumerator8.Current;
                        int orCreateSlot3 = GetOrCreateSlot(current9);
                        if (orCreateSlot3 > 0)
                        {
                            bool num3 = (current9.FlowAnalysisAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull;
                            NullableFlowState nullableFlowState = state[orCreateSlot3];
                            if (num3 && nullableFlowState.MayBeNull())
                            {
                                base.Diagnostics.Add(ErrorCode.WRN_ParameterDisallowsNull, syntaxOpt?.GetLocation() ?? methodMainNode.Syntax.GetLastToken().GetLocation(), current9.Name);
                            }
                            else
                            {
                                EnforceNotNullIfNotNull(syntaxOpt, state, base.MethodParameters, current9.NotNullIfParameterNotNull, nullableFlowState, current9);
                            }
                        }
                    }
                }
            }
            void enforceNotNullWhenForPendingReturn(AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch pendingReturn, BoundReturnStatement returnStatement)
            {
                ImmutableArray<ParameterSymbol> methodParameters = base.MethodParameters;
                if (!methodParameters.IsEmpty)
                {
                    if (pendingReturn.IsConditionalState)
                    {
                        BoundExpression expressionOpt2 = returnStatement.ExpressionOpt;
                        if (expressionOpt2 != null)
                        {
                            ConstantValue constantValue2 = expressionOpt2.ConstantValue;
                            if ((object)constantValue2 != null && constantValue2.IsBoolean)
                            {
                                bool booleanValue3 = constantValue2.BooleanValue;
                                enforceParameterNotNullWhen(returnStatement.Syntax, methodParameters, booleanValue3, pendingReturn.State);
                                return;
                            }
                        }
                        if (pendingReturn.StateWhenTrue.Reachable && pendingReturn.StateWhenFalse.Reachable)
                        {
                            ImmutableArray<ParameterSymbol>.Enumerator enumerator7 = methodParameters.GetEnumerator();
                            while (enumerator7.MoveNext())
                            {
                                ParameterSymbol current8 = enumerator7.Current;
                                int orCreateSlot2 = GetOrCreateSlot(current8);
                                if (orCreateSlot2 > 0 && pendingReturn.StateWhenTrue[orCreateSlot2] != pendingReturn.StateWhenFalse[orCreateSlot2])
                                {
                                    reportParameterIfBadConditionalState(returnStatement.Syntax, current8, sense: true, pendingReturn.StateWhenTrue);
                                    reportParameterIfBadConditionalState(returnStatement.Syntax, current8, sense: false, pendingReturn.StateWhenFalse);
                                }
                            }
                        }
                    }
                    else
                    {
                        BoundExpression expressionOpt2 = returnStatement.ExpressionOpt;
                        if (expressionOpt2 != null)
                        {
                            ConstantValue constantValue2 = expressionOpt2.ConstantValue;
                            if ((object)constantValue2 != null && constantValue2.IsBoolean)
                            {
                                bool booleanValue4 = constantValue2.BooleanValue;
                                enforceParameterNotNullWhen(returnStatement.Syntax, methodParameters, booleanValue4, pendingReturn.State);
                            }
                        }
                    }
                }
            }
            void enforceParameterNotNullWhen(SyntaxNode syntax, ImmutableArray<ParameterSymbol> parameters, bool sense, LocalState stateWhen)
            {
                if (stateWhen.Reachable)
                {
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator6 = parameters.GetEnumerator();
                    while (enumerator6.MoveNext())
                    {
                        ParameterSymbol current7 = enumerator6.Current;
                        reportParameterIfBadConditionalState(syntax, current7, sense, stateWhen);
                    }
                }
            }
            int getSlotForFieldOrPropertyOrEvent(Symbol member)
            {
                if (member.Kind != SymbolKind.Field && member.Kind != SymbolKind.Property && member.Kind != SymbolKind.Event)
                {
                    return -1;
                }
                int num5 = 0;
                if (!member.IsStatic)
                {
                    if ((object)base.MethodThisParameter == null)
                    {
                        return -1;
                    }
                    num5 = GetOrCreateSlot(base.MethodThisParameter);
                    if (num5 < 0)
                    {
                        return -1;
                    }
                }
                return GetOrCreateSlot(member, num5);
            }
            void makeMemberMaybeNull(MethodSymbol method, string memberName)
            {
                ImmutableArray<Symbol>.Enumerator enumerator12 = method.ContainingType.GetMembers(memberName).GetEnumerator();
                while (enumerator12.MoveNext())
                {
                    Symbol current13 = enumerator12.Current;
                    int slotForFieldOrPropertyOrEvent2 = getSlotForFieldOrPropertyOrEvent(current13);
                    if (slotForFieldOrPropertyOrEvent2 > 0)
                    {
                        State[slotForFieldOrPropertyOrEvent2] = NullableFlowState.MaybeNull;
                    }
                }
            }
            void makeMembersMaybeNull(MethodSymbol method, ImmutableArray<string> members)
            {
                ImmutableArray<string>.Enumerator enumerator13 = members.GetEnumerator();
                while (enumerator13.MoveNext())
                {
                    string current14 = enumerator13.Current;
                    makeMemberMaybeNull(method, current14);
                }
            }
            void makeNotNullMembersMaybeNull()
            {
                Symbol symbol2 = _symbol;
                MethodSymbol method2 = symbol2 as MethodSymbol;
                if ((object)method2 != null)
                {
                    if (method2.IsConstructor())
                    {
                        if (needsDefaultInitialStateForMembers())
                        {
                            ImmutableArray<Symbol>.Enumerator enumerator14 = method2.ContainingType.GetMembersUnordered().GetEnumerator();
                            while (enumerator14.MoveNext())
                            {
                                Symbol current15 = enumerator14.Current;
                                if (current15.IsStatic == method2.IsStatic)
                                {
                                    Symbol symbol3 = current15;
                                    if (!(current15 is PropertySymbol))
                                    {
                                        if (current15 is FieldSymbol fieldSymbol3)
                                        {
                                            if (fieldSymbol3.IsConst)
                                            {
                                                continue;
                                            }
                                            if (fieldSymbol3.AssociatedSymbol is PropertySymbol propertySymbol)
                                            {
                                                if (IsPropertyOutputMoreStrictThanInput(propertySymbol))
                                                {
                                                    continue;
                                                }
                                                symbol3 = propertySymbol;
                                            }
                                        }
                                        int slotForFieldOrPropertyOrEvent3 = getSlotForFieldOrPropertyOrEvent(symbol3);
                                        if (slotForFieldOrPropertyOrEvent3 > 0)
                                        {
                                            TypeWithAnnotations typeOrReturnType = symbol3.GetTypeOrReturnType();
                                            if (!typeOrReturnType.NullableAnnotation.IsOblivious())
                                            {
                                                State[slotForFieldOrPropertyOrEvent3] = ((!typeOrReturnType.Type.IsPossiblyNullableReferenceTypeTypeParameter()) ? NullableFlowState.MaybeNull : NullableFlowState.MaybeDefault);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        do
                        {
                            makeMembersMaybeNull(method2, method2.NotNullMembers);
                            makeMembersMaybeNull(method2, method2.NotNullWhenTrueMembers);
                            makeMembersMaybeNull(method2, method2.NotNullWhenFalseMembers);
                            method2 = method2.OverriddenMethod;
                        }
                        while (method2 != null);
                    }
                }
                bool needsDefaultInitialStateForMembers()
                {
                    if (_hasInitialState)
                    {
                        return false;
                    }
                    if (!method2.HasThisConstructorInitializer() && (!method2.ContainingType.IsValueType || method2.IsStatic))
                    {
                        return true;
                    }
                    if (methodMainNode is BoundConstructorMethodBody boundConstructorMethodBody && boundConstructorMethodBody.Initializer?.Expression.ExpressionSymbol is MethodSymbol method3)
                    {
                        return method3.IsDefaultValueTypeConstructor();
                    }
                    return false;
                }
            }
            bool memberHasBadState(Symbol member, LocalState state)
            {
                switch (member.Kind)
                {
                    case SymbolKind.Field:
                    case SymbolKind.Property:
                        {
                            int slotForFieldOrPropertyOrEvent = getSlotForFieldOrPropertyOrEvent(member);
                            if (slotForFieldOrPropertyOrEvent > 0)
                            {
                                return !state[slotForFieldOrPropertyOrEvent].IsNotNull();
                            }
                            return false;
                        }
                    default:
                        return false;
                }
            }
            bool parameterHasBadConditionalState(ParameterSymbol parameter, bool sense, LocalState stateWhen)
            {
                RefKind refKind = parameter.RefKind;
                if (refKind != RefKind.Out && refKind != RefKind.Ref)
                {
                    return false;
                }
                int orCreateSlot = GetOrCreateSlot(parameter);
                if (orCreateSlot > 0)
                {
                    NullableFlowState state2 = stateWhen[orCreateSlot];
                    FlowAnalysisAnnotations flowAnalysisAnnotations = parameter.FlowAnalysisAnnotations;
                    if (sense)
                    {
                        bool num = (flowAnalysisAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNullWhenTrue;
                        bool flag = (flowAnalysisAnnotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNullWhenFalse;
                        if (!num || !state2.MayBeNull())
                        {
                            if (flag)
                            {
                                return ShouldReportNullableAssignment(parameter.TypeWithAnnotations, state2);
                            }
                            return false;
                        }
                        return true;
                    }
                    bool num2 = (flowAnalysisAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNullWhenFalse;
                    bool flag2 = (flowAnalysisAnnotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNullWhenTrue;
                    if (!num2 || !state2.MayBeNull())
                    {
                        if (flag2)
                        {
                            return ShouldReportNullableAssignment(parameter.TypeWithAnnotations, state2);
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
            void reportMemberIfBadConditionalState(SyntaxNode? syntaxOpt, bool sense, Symbol member, LocalState state)
            {
                if (memberHasBadState(member, state))
                {
                    base.Diagnostics.Add(ErrorCode.WRN_MemberNotNullWhen, syntaxOpt?.GetLocation() ?? methodMainNode.Syntax.GetLastToken().GetLocation(), member.Name, sense ? "true" : "false");
                }
            }
            void reportParameterIfBadConditionalState(SyntaxNode syntax, ParameterSymbol parameter, bool sense, LocalState stateWhen)
            {
                if (parameterHasBadConditionalState(parameter, sense, stateWhen))
                {
                    base.Diagnostics.Add(ErrorCode.WRN_ParameterConditionallyDisallowsNull, syntax.Location, parameter.Name, sense ? "true" : "false");
                }
            }
        }

        private void EnforceNotNullIfNotNull(SyntaxNode? syntaxOpt, LocalState state, ImmutableArray<ParameterSymbol> parameters, ImmutableHashSet<string> inputParamNames, NullableFlowState outputState, ParameterSymbol? outputParam)
        {
            if (inputParamNames.IsEmpty || outputState.IsNotNull())
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (!inputParamNames.Contains(current.Name))
                {
                    continue;
                }
                int orCreateSlot = GetOrCreateSlot(current);
                if (orCreateSlot > 0 && state[orCreateSlot].IsNotNull())
                {
                    Location location = syntaxOpt?.GetLocation() ?? methodMainNode.Syntax.GetLastToken().GetLocation();
                    if ((object)outputParam != null)
                    {
                        base.Diagnostics.Add(ErrorCode.WRN_ParameterNotNullIfNotNull, location, outputParam!.Name, current.Name);
                    }
                    else if (CurrentSymbol is MethodSymbol methodSymbol && !methodSymbol.IsAsync)
                    {
                        base.Diagnostics.Add(ErrorCode.WRN_ReturnNotNullIfNotNull, location, current.Name);
                    }
                    break;
                }
            }
        }

        private void EnforceDoesNotReturn(SyntaxNode? syntaxOpt)
        {
            Symbol currentSymbol = CurrentSymbol;
            if (currentSymbol is MethodSymbol methodSymbol && currentSymbol.ContainingSymbol is TypeSymbol && (methodSymbol.FlowAnalysisAnnotations & FlowAnalysisAnnotations.DoesNotReturn) == FlowAnalysisAnnotations.DoesNotReturn && IsReachable())
            {
                ReportDiagnostic(ErrorCode.WRN_ShouldNotReturn, syntaxOpt?.GetLocation() ?? methodMainNode.Syntax.GetLastToken().GetLocation());
            }
        }

        internal static void AnalyzeIfNeeded(CSharpCompilation compilation, MethodSymbol method, BoundNode node, DiagnosticBag diagnostics, bool useConstructorExitWarnings, VariableState? initialNullableState, bool getFinalNullableState, out VariableState? finalNullableState)
        {
            if (!HasRequiredLanguageVersion(compilation) || !compilation.IsNullableAnalysisEnabledIn(method))
            {
                if (compilation.IsNullableAnalysisEnabledAlways)
                {
                    Analyze(compilation, method, node, new DiagnosticBag(), useConstructorExitWarnings: false, null, getFinalNullableState: false, out var _, requiresAnalysis: false);
                }
                finalNullableState = null;
            }
            else
            {
                Analyze(compilation, method, node, diagnostics, useConstructorExitWarnings, initialNullableState, getFinalNullableState, out finalNullableState);
            }
        }

        private static void Analyze(CSharpCompilation compilation, MethodSymbol method, BoundNode node, DiagnosticBag diagnostics, bool useConstructorExitWarnings, VariableState? initialNullableState, bool getFinalNullableState, out VariableState? finalNullableState, bool requiresAnalysis = true)
        {
            if (method.IsImplicitlyDeclared && !method.IsImplicitConstructor && !method.IsScriptInitializer)
            {
                finalNullableState = null;
                return;
            }
            Binder binder = ((method is SynthesizedSimpleProgramEntryPointSymbol synthesizedSimpleProgramEntryPointSymbol) ? synthesizedSimpleProgramEntryPointSymbol.GetBodyBinder(ignoreAccessibility: false) : compilation.GetBinderFactory(node.SyntaxTree).GetBinder(node.Syntax));
            Conversions conversions = binder.Conversions;
            Analyze(compilation, method, node, binder, conversions, diagnostics, useConstructorExitWarnings, useDelegateInvokeParameterTypes: false, null, initialNullableState, null, null, null, getFinalNullableState, out finalNullableState, requiresAnalysis);
        }

        internal static VariableState? GetAfterInitializersState(CSharpCompilation compilation, Symbol? symbol)
        {
            if (symbol is MethodSymbol methodSymbol && methodSymbol.IncludeFieldInitializersInBody() && methodSymbol.ContainingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
            {
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                Binder.ProcessedFieldInitializers processedInitializers = default(Binder.ProcessedFieldInitializers);
                Binder.BindFieldInitializers(compilation, null, methodSymbol.IsStatic ? sourceMemberContainerTypeSymbol.StaticInitializers : sourceMemberContainerTypeSymbol.InstanceInitializers, BindingDiagnosticBag.Discarded, ref processedInitializers);
                AnalyzeIfNeeded(compilation, methodSymbol, InitializerRewriter.RewriteConstructor(processedInitializers.BoundInitializers, methodSymbol), instance, useConstructorExitWarnings: false, null, getFinalNullableState: true, out var finalNullableState);
                instance.Free();
                return finalNullableState;
            }
            return null;
        }

        internal static void AnalyzeWithoutRewrite(CSharpCompilation compilation, Symbol? symbol, BoundNode node, Binder binder, DiagnosticBag diagnostics, bool createSnapshots)
        {
            AnalyzeWithSemanticInfo(compilation, symbol, node, binder, GetAfterInitializersState(compilation, symbol), diagnostics, createSnapshots, requiresAnalysis: false);
        }

        internal static BoundNode AnalyzeAndRewrite(CSharpCompilation compilation, Symbol? symbol, BoundNode node, Binder binder, VariableState? initialState, DiagnosticBag diagnostics, bool createSnapshots, out SnapshotManager? snapshotManager, ref ImmutableDictionary<Symbol, Symbol>? remappedSymbols)
        {
            (SnapshotManager?, ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>) tuple = AnalyzeWithSemanticInfo(compilation, symbol, node, binder, initialState, diagnostics, createSnapshots, requiresAnalysis: true);
            (snapshotManager, _) = tuple;
            return Rewrite(tuple.Item2, snapshotManager, node, ref remappedSymbols);
        }

        private static (SnapshotManager?, ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>) AnalyzeWithSemanticInfo(CSharpCompilation compilation, Symbol? symbol, BoundNode node, Binder binder, VariableState? initialState, DiagnosticBag diagnostics, bool createSnapshots, bool requiresAnalysis)
        {
            ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>.Builder builder = ImmutableDictionary.CreateBuilder(EqualityComparer<BoundExpression>.Default, NullabilityInfoTypeComparer.Instance);
            SnapshotManager.Builder? builder2 = ((createSnapshots && symbol != null) ? new SnapshotManager.Builder() : null);
            Analyze(compilation, symbol, node, binder, binder.Conversions, diagnostics, useConstructorExitWarnings: true, useDelegateInvokeParameterTypes: false, null, initialState, builder, builder2, null, getFinalNullableState: false, out var _, requiresAnalysis);
            ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)> item = builder.ToImmutable();
            return (builder2?.ToManagerAndFree(), item);
        }

        internal static BoundNode AnalyzeAndRewriteSpeculation(int position, BoundNode node, Binder binder, SnapshotManager originalSnapshots, out SnapshotManager newSnapshots, ref ImmutableDictionary<Symbol, Symbol>? remappedSymbols)
        {
            ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>.Builder builder = ImmutableDictionary.CreateBuilder(EqualityComparer<BoundExpression>.Default, NullabilityInfoTypeComparer.Instance);
            SnapshotManager.Builder builder2 = new SnapshotManager.Builder();
            (VariablesSnapshot, LocalStateSnapshot) snapshot = originalSnapshots.GetSnapshot(position);
            VariablesSnapshot item = snapshot.Item1;
            LocalStateSnapshot item2 = snapshot.Item2;
            Symbol symbol = item.Symbol;
            NullableWalker nullableWalker = new NullableWalker(binder.Compilation, symbol, useConstructorExitWarnings: false, useDelegateInvokeParameterTypes: false, null, node, binder, binder.Conversions, Variables.Create(item), null, builder, builder2, isSpeculative: true);
            try
            {
                Analyze(nullableWalker, symbol, null, LocalState.Create(item2), builder2);
            }
            finally
            {
                nullableWalker.Free();
            }
            ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol)> updatedNullabilities = builder.ToImmutable();
            newSnapshots = builder2.ToManagerAndFree();
            return Rewrite(updatedNullabilities, newSnapshots, node, ref remappedSymbols);
        }

        private static BoundNode Rewrite(ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)> updatedNullabilities, SnapshotManager? snapshotManager, BoundNode node, ref ImmutableDictionary<Symbol, Symbol>? remappedSymbols)
        {
            ImmutableDictionary<Symbol, Symbol>.Builder builder = ImmutableDictionary.CreateBuilder(Symbols.SymbolEqualityComparer.ConsiderEverything, Symbols.SymbolEqualityComparer.ConsiderEverything);
            if (remappedSymbols != null)
            {
                builder.AddRange(remappedSymbols);
            }
            BoundNode result = new NullabilityRewriter(updatedNullabilities, snapshotManager, builder).Visit(node);
            remappedSymbols = builder.ToImmutable();
            return result;
        }

        private static bool HasRequiredLanguageVersion(CSharpCompilation compilation)
        {
            return compilation.LanguageVersion >= MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion();
        }

        internal static bool NeedsAnalysis(CSharpCompilation compilation, SyntaxNode syntaxNode)
        {
            if (HasRequiredLanguageVersion(compilation))
            {
                if (!compilation.IsNullableAnalysisEnabledIn(syntaxNode))
                {
                    return compilation.IsNullableAnalysisEnabledAlways;
                }
                return true;
            }
            return false;
        }

        internal static void AnalyzeIfNeeded(Binder binder, BoundNode node, SyntaxNode syntax, DiagnosticBag diagnostics)
        {
            bool requiresAnalysis = true;
            CSharpCompilation cSharpCompilation = binder.Compilation;
            if (!HasRequiredLanguageVersion(cSharpCompilation) || !cSharpCompilation.IsNullableAnalysisEnabledIn(syntax))
            {
                if (!cSharpCompilation.IsNullableAnalysisEnabledAlways)
                {
                    return;
                }
                diagnostics = new DiagnosticBag();
                requiresAnalysis = false;
            }
            Analyze(cSharpCompilation, null, node, binder, binder.Conversions, diagnostics, useConstructorExitWarnings: false, useDelegateInvokeParameterTypes: false, null, null, null, null, null, getFinalNullableState: false, out var _, requiresAnalysis);
        }

        internal static void Analyze(CSharpCompilation compilation, BoundLambda lambda, Conversions conversions, DiagnosticBag diagnostics, MethodSymbol? delegateInvokeMethodOpt, VariableState initialState, ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>? returnTypesOpt)
        {
            LambdaSymbol symbol = lambda.Symbol;
            Variables variables = Variables.Create(initialState.Variables).CreateNestedMethodScope(symbol);
            NullableWalker nullableWalker = new NullableWalker(compilation, symbol, useConstructorExitWarnings: false, UseDelegateInvokeParameterTypes(lambda, delegateInvokeMethodOpt), delegateInvokeMethodOpt, lambda.Body, lambda.Binder, conversions, variables, returnTypesOpt, null, null);
            try
            {
                LocalState localState = LocalState.Create(initialState.VariableNullableStates).CreateNestedMethodState(variables);
                Analyze(nullableWalker, symbol, diagnostics, localState, null);
            }
            finally
            {
                nullableWalker.Free();
            }
        }

        private static void Analyze(CSharpCompilation compilation, Symbol? symbol, BoundNode node, Binder binder, Conversions conversions, DiagnosticBag diagnostics, bool useConstructorExitWarnings, bool useDelegateInvokeParameterTypes, MethodSymbol? delegateInvokeMethodOpt, VariableState? initialState, ImmutableDictionary<BoundExpression, (NullabilityInfo, TypeSymbol?)>.Builder? analyzedNullabilityMapOpt, SnapshotManager.Builder? snapshotBuilderOpt, ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>? returnTypesOpt, bool getFinalNullableState, out VariableState? finalNullableState, bool requiresAnalysis = true)
        {
            NullableWalker nullableWalker = new NullableWalker(compilation, symbol, useConstructorExitWarnings, useDelegateInvokeParameterTypes, delegateInvokeMethodOpt, node, binder, conversions, (initialState == null) ? null : Variables.Create(initialState!.Variables), returnTypesOpt, analyzedNullabilityMapOpt, snapshotBuilderOpt);
            finalNullableState = null;
            try
            {
                Analyze(nullableWalker, symbol, diagnostics, (initialState == null) ? default(Optional<LocalState>) : LocalState.Create(initialState!.VariableNullableStates), snapshotBuilderOpt, requiresAnalysis);
                if (getFinalNullableState)
                {
                    finalNullableState = GetVariableState(nullableWalker._variables, nullableWalker.State);
                }
            }
            finally
            {
                nullableWalker.Free();
            }
        }

        private static void Analyze(NullableWalker walker, Symbol? symbol, DiagnosticBag? diagnostics, Optional<LocalState> initialState, SnapshotManager.Builder? snapshotBuilderOpt, bool requiresAnalysis = true)
        {
            int previousSlot = snapshotBuilderOpt?.EnterNewWalker(symbol) ?? (-1);
            try
            {
                bool badRegion = false;
                walker.Analyze(ref badRegion, initialState);
                diagnostics?.AddRange(walker.Diagnostics);
            }
            catch (CancelledByStackGuardException ex) when (diagnostics != null)
            {
                ex.AddAnError(diagnostics);
            }
            finally
            {
                snapshotBuilderOpt?.ExitWalker(walker.SaveSharedState(), previousSlot);
            }
            walker.RecordNullableAnalysisData(symbol, requiresAnalysis);
        }

        private void RecordNullableAnalysisData(Symbol? symbol, bool requiredAnalysis)
        {
            ConcurrentDictionary<object, Data> concurrentDictionary = compilation.NullableAnalysisData?.Data;
            if (concurrentDictionary != null)
            {
                object key = symbol ?? ((object)methodMainNode.Syntax);
                if (!concurrentDictionary.TryGetValue(key, out var _))
                {
                    concurrentDictionary.TryAdd(key, new Data(_variables.GetTotalVariableCount(), requiredAnalysis));
                }
            }
        }

        private SharedWalkerState SaveSharedState()
        {
            return new SharedWalkerState(_variables.CreateSnapshot());
        }

        private void TakeIncrementalSnapshot(BoundNode? node)
        {
            _snapshotBuilderOpt?.TakeIncrementalSnapshot(node, State);
        }

        private void SetUpdatedSymbol(BoundNode node, Symbol originalSymbol, Symbol updatedSymbol)
        {
            if (_snapshotBuilderOpt == null)
            {
                return;
            }
            bool flag = false;
            if (node is BoundLambda boundLambda && originalSymbol is LambdaSymbol l && updatedSymbol is NamedTypeSymbol n)
            {
                if (!AreLambdaAndNewDelegateSimilar(l, n))
                {
                    return;
                }
                flag = updatedSymbol.Equals(boundLambda.Type.GetDelegateType(), TypeCompareKind.ConsiderEverything);
            }
            if (flag || Symbol.Equals(originalSymbol, updatedSymbol, TypeCompareKind.ConsiderEverything))
            {
                _snapshotBuilderOpt!.RemoveSymbolIfPresent(node, originalSymbol);
            }
            else
            {
                _snapshotBuilderOpt!.SetUpdatedSymbol(node, originalSymbol, updatedSymbol);
            }
        }

        protected override void Normalize(ref LocalState state)
        {
            if (state.Reachable)
            {
                state.Normalize(this, _variables);
            }
        }

        private NullableFlowState GetDefaultState(ref LocalState state, int slot)
        {
            if (!state.Reachable)
            {
                return NullableFlowState.NotNull;
            }
            Symbol symbol = _variables[slot].Symbol;
            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    {
                        LocalSymbol localSymbol = (LocalSymbol)symbol;
                        if (!_variables.TryGetType(localSymbol, out var type2))
                        {
                            type2 = localSymbol.TypeWithAnnotations;
                        }
                        return type2.ToTypeWithState().State;
                    }
                case SymbolKind.Parameter:
                    {
                        ParameterSymbol parameterSymbol = (ParameterSymbol)symbol;
                        if (!_variables.TryGetType(parameterSymbol, out var type))
                        {
                            type = parameterSymbol.TypeWithAnnotations;
                        }
                        return GetParameterState(type, parameterSymbol.FlowAnalysisAnnotations).State;
                    }
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                    return GetDefaultState(symbol);
                case SymbolKind.ErrorType:
                    return NullableFlowState.NotNull;
                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
            }
        }

        protected override bool TryGetReceiverAndMember(BoundExpression expr, out BoundExpression? receiver, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Symbol? member)
        {
            receiver = null;
            member = null;
            switch (expr.Kind)
            {
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
                        FieldSymbol fieldSymbol = (FieldSymbol)(member = boundFieldAccess.FieldSymbol);
                        if (fieldSymbol.IsFixedSizeBuffer)
                        {
                            return false;
                        }
                        if (fieldSymbol.IsStatic)
                        {
                            return true;
                        }
                        receiver = boundFieldAccess.ReceiverOpt;
                        break;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)expr;
                        if ((member = boundEventAccess.EventSymbol)!.IsStatic)
                        {
                            return true;
                        }
                        receiver = boundEventAccess.ReceiverOpt;
                        break;
                    }
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)expr;
                        if ((member = boundPropertyAccess.PropertySymbol)!.IsStatic)
                        {
                            return true;
                        }
                        receiver = boundPropertyAccess.ReceiverOpt;
                        break;
                    }
            }
            if ((object)member != null && receiver != null && receiver!.Kind != BoundKind.TypeExpression)
            {
                return (object)receiver!.Type != null;
            }
            return false;
        }

        protected override int MakeSlot(BoundExpression node)
        {
            return makeSlot(node);
            int getPlaceholderSlot(BoundExpression expr)
            {
                if (_placeholderLocalsOpt != null && _placeholderLocalsOpt!.TryGetValue(expr, out var value))
                {
                    return GetOrCreateSlot(value);
                }
                return -1;
            }
            static MethodSymbol? getTopLevelMethod(MethodSymbol? method)
            {
                while ((object)method != null)
                {
                    Symbol containingSymbol = method!.ContainingSymbol;
                    if (containingSymbol.Kind == SymbolKind.NamedType)
                    {
                        return method;
                    }
                    method = containingSymbol as MethodSymbol;
                }
                return null;
            }
            int makeSlot(BoundExpression node)
            {
                switch (node.Kind)
                {
                    case BoundKind.ThisReference:
                    case BoundKind.BaseReference:
                        {
                            ParameterSymbol parameterSymbol = getTopLevelMethod(_symbol as MethodSymbol)?.ThisParameter;
                            if ((object)parameterSymbol == null)
                            {
                                return -1;
                            }
                            return GetOrCreateSlot(parameterSymbol);
                        }
                    case BoundKind.Conversion:
                        {
                            int placeholderSlot2 = getPlaceholderSlot(node);
                            if (placeholderSlot2 > 0)
                            {
                                return placeholderSlot2;
                            }
                            BoundConversion boundConversion = (BoundConversion)node;
                            switch (boundConversion.Conversion.Kind)
                            {
                                case ConversionKind.ExplicitNullable:
                                    {
                                        BoundExpression operand = boundConversion.Operand;
                                        TypeSymbol type = operand.Type;
                                        TypeSymbol type2 = boundConversion.Type;
                                        if (AreNullableAndUnderlyingTypes(type, type2, out var _))
                                        {
                                            int num = MakeSlot(operand);
                                            if (num >= 0)
                                            {
                                                return GetNullableOfTValueSlot(type, num, out Symbol valueProperty);
                                            }
                                            return -1;
                                        }
                                        break;
                                    }
                                case ConversionKind.Identity:
                                case ConversionKind.ImplicitTupleLiteral:
                                case ConversionKind.ImplicitReference:
                                case ConversionKind.Boxing:
                                case ConversionKind.DefaultLiteral:
                                    return MakeSlot(boundConversion.Operand);
                            }
                            return -1;
                        }
                    case BoundKind.DefaultLiteral:
                    case BoundKind.DefaultExpression:
                    case BoundKind.ObjectCreationExpression:
                    case BoundKind.TupleLiteral:
                    case BoundKind.ConvertedTupleLiteral:
                    case BoundKind.DynamicObjectCreationExpression:
                    case BoundKind.AnonymousObjectCreationExpression:
                    case BoundKind.NewT:
                        return getPlaceholderSlot(node);
                    case BoundKind.ConditionalAccess:
                        return getPlaceholderSlot(node);
                    case BoundKind.ConditionalReceiver:
                        return _lastConditionalAccessSlot;
                    default:
                        {
                            int placeholderSlot = getPlaceholderSlot(node);
                            if (placeholderSlot <= 0)
                            {
                                return base.MakeSlot(node);
                            }
                            return placeholderSlot;
                        }
                }
            }
        }

        protected override int GetOrCreateSlot(Symbol symbol, int containingSlot = 0, bool forceSlotEvenIfEmpty = false, bool createIfMissing = true)
        {
            if (containingSlot > 0 && !IsSlotMember(containingSlot, symbol))
            {
                return -1;
            }
            return base.GetOrCreateSlot(symbol, containingSlot, forceSlotEvenIfEmpty, createIfMissing);
        }

        private void VisitAndUnsplitAll<T>(ImmutableArray<T> nodes) where T : BoundNode
        {
            if (!nodes.IsDefault)
            {
                ImmutableArray<T>.Enumerator enumerator = nodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    Visit(current);
                    Unsplit();
                }
            }
        }

        private void VisitWithoutDiagnostics(BoundNode? node)
        {
            bool disableDiagnostics = _disableDiagnostics;
            _disableDiagnostics = true;
            Visit(node);
            _disableDiagnostics = disableDiagnostics;
        }

        protected override void VisitRvalue(BoundExpression? node, bool isKnownToBeAnLvalue = false)
        {
            Visit(node);
            VisitRvalueEpilogue(node);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void VisitRvalueEpilogue(BoundExpression? node)
        {
            Unsplit();
            UseRvalueOnly(node);
        }

        private TypeWithState VisitRvalueWithState(BoundExpression? node)
        {
            VisitRvalue(node);
            return ResultType;
        }

        private TypeWithAnnotations VisitLvalueWithAnnotations(BoundExpression node)
        {
            VisitLValue(node);
            Unsplit();
            return LvalueResultType;
        }

        private static object GetTypeAsDiagnosticArgument(TypeSymbol? typeOpt)
        {
            return typeOpt ?? ((object)"<null>");
        }

        private static object GetParameterAsDiagnosticArgument(ParameterSymbol? parameterOpt)
        {
            if ((object)parameterOpt != null)
            {
                return new FormattedSymbol(parameterOpt, SymbolDisplayFormat.ShortFormat);
            }
            return "";
        }

        private static object GetContainingSymbolAsDiagnosticArgument(ParameterSymbol? parameterOpt)
        {
            Symbol symbol = parameterOpt?.ContainingSymbol;
            if ((object)symbol != null)
            {
                return new FormattedSymbol(symbol, SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
            return "";
        }

        private static bool ShouldReportNullableAssignment(TypeWithAnnotations type, NullableFlowState state)
        {
            if (!type.HasType || type.Type.IsValueType)
            {
                return false;
            }
            NullableAnnotation nullableAnnotation = type.NullableAnnotation;
            if (nullableAnnotation - 1 <= NullableAnnotation.Oblivious)
            {
                return false;
            }
            switch (state)
            {
                case NullableFlowState.NotNull:
                    return false;
                case NullableFlowState.MaybeNull:
                    if (type.Type.IsTypeParameterDisallowingAnnotationInCSharp8() && (!(type.Type is TypeParameterSymbol typeParameterSymbol) || !(typeParameterSymbol.IsNotNullable ?? false)))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        private void ReportNullableAssignmentIfNecessary(BoundExpression? value, TypeWithAnnotations targetType, TypeWithState valueType, bool useLegacyWarnings, AssignmentKind assignmentKind = AssignmentKind.Assignment, ParameterSymbol? parameterOpt = null, Location? location = null)
        {
            if ((targetType.HasType && !targetType.Type.Equals(valueType.Type, TypeCompareKind.AllIgnoreOptions)) || value == null || (value!.WasCompilerGenerated && assignmentKind == AssignmentKind.Argument) || !ShouldReportNullableAssignment(targetType, valueType.State))
            {
                return;
            }
            if ((object)location == null)
            {
                location = value!.Syntax.GetLocation();
            }
            if (SkipReferenceConversions(value).IsSuppressed)
            {
                return;
            }
            ConstantValue? constantValue = value!.ConstantValue;
            if ((object)constantValue != null && constantValue!.IsNull && !useLegacyWarnings)
            {
                ReportDiagnostic((assignmentKind == AssignmentKind.Return) ? ErrorCode.WRN_NullReferenceReturn : ErrorCode.WRN_NullAsNonNullable, location);
            }
            else if (assignmentKind == AssignmentKind.Argument)
            {
                ReportDiagnostic(ErrorCode.WRN_NullReferenceArgument, location, GetParameterAsDiagnosticArgument(parameterOpt), GetContainingSymbolAsDiagnosticArgument(parameterOpt));
                LearnFromNonNullTest(value, ref State);
            }
            else if (useLegacyWarnings)
            {
                if (!isMaybeDefaultValue(valueType) || allowUnconstrainedTypeParameterAnnotations(compilation))
                {
                    ReportNonSafetyDiagnostic(location);
                }
            }
            else
            {
                ReportDiagnostic((assignmentKind == AssignmentKind.Return) ? ErrorCode.WRN_NullReferenceReturn : ErrorCode.WRN_NullReferenceAssignment, location);
            }
            static bool allowUnconstrainedTypeParameterAnnotations(CSharpCompilation compilation)
            {
                return MessageID.IDS_FeatureDefaultTypeParameterConstraint.RequiredVersion() <= compilation.LanguageVersion;
            }
            static bool isMaybeDefaultValue(TypeWithState valueType)
            {
                TypeSymbol? type = valueType.Type;
                if ((object)type != null && type!.TypeKind == TypeKind.TypeParameter)
                {
                    return valueType.State == NullableFlowState.MaybeDefault;
                }
                return false;
            }
        }

        internal static bool AreParameterAnnotationsCompatible(RefKind refKind, TypeWithAnnotations overriddenType, FlowAnalysisAnnotations overriddenAnnotations, TypeWithAnnotations overridingType, FlowAnalysisAnnotations overridingAnnotations, bool forRef = false)
        {
            switch (refKind)
            {
                case RefKind.Ref:
                    if (AreParameterAnnotationsCompatible(RefKind.None, overriddenType, overriddenAnnotations, overridingType, overridingAnnotations, forRef: true))
                    {
                        return AreParameterAnnotationsCompatible(RefKind.Out, overriddenType, overriddenAnnotations, overridingType, overridingAnnotations);
                    }
                    return false;
                case RefKind.None:
                case RefKind.In:
                    {
                        if (isBadAssignment(GetParameterState(overriddenType, overriddenAnnotations), overridingType, overridingAnnotations))
                        {
                            return false;
                        }
                        bool flag = (overridingAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull;
                        if ((overriddenAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull && !flag && !forRef)
                        {
                            return false;
                        }
                        bool flag2 = (overridingAnnotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNull;
                        if ((overriddenAnnotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNull && !flag2 && !forRef)
                        {
                            return false;
                        }
                        break;
                    }
            }
            if (refKind == RefKind.Out && (!canAssignOutputValueWhen(sense: true) || !canAssignOutputValueWhen(sense: false)))
            {
                return false;
            }
            return true;
            bool canAssignOutputValueWhen(bool sense)
            {
                if (isBadAssignment(ApplyUnconditionalAnnotations(overridingType.ToTypeWithState(), makeUnconditionalAnnotation(overridingAnnotations, sense)), destinationAnnotations: ToInwardAnnotations(makeUnconditionalAnnotation(overriddenAnnotations, sense)), destinationType: overriddenType))
                {
                    return false;
                }
                return true;
            }
            static bool isBadAssignment(TypeWithState valueState, TypeWithAnnotations destinationType, FlowAnalysisAnnotations destinationAnnotations)
            {
                if (ShouldReportNullableAssignment(ApplyLValueAnnotations(destinationType, destinationAnnotations), valueState.State))
                {
                    return true;
                }
                if (IsDisallowedNullAssignment(valueState, destinationAnnotations))
                {
                    return true;
                }
                return false;
            }
            static FlowAnalysisAnnotations makeUnconditionalAnnotation(FlowAnalysisAnnotations annotations, bool sense)
            {
                if (sense)
                {
                    return makeUnconditionalAnnotationCore(makeUnconditionalAnnotationCore(annotations, FlowAnalysisAnnotations.NotNullWhenTrue, FlowAnalysisAnnotations.NotNull), FlowAnalysisAnnotations.MaybeNullWhenTrue, FlowAnalysisAnnotations.MaybeNull);
                }
                return makeUnconditionalAnnotationCore(makeUnconditionalAnnotationCore(annotations, FlowAnalysisAnnotations.NotNullWhenFalse, FlowAnalysisAnnotations.NotNull), FlowAnalysisAnnotations.MaybeNullWhenFalse, FlowAnalysisAnnotations.MaybeNull);
            }
            static FlowAnalysisAnnotations makeUnconditionalAnnotationCore(FlowAnalysisAnnotations annotations, FlowAnalysisAnnotations conditionalAnnotation, FlowAnalysisAnnotations replacementAnnotation)
            {
                if ((annotations & conditionalAnnotation) != 0)
                {
                    return annotations | replacementAnnotation;
                }
                return annotations & ~replacementAnnotation;
            }
        }

        private static bool IsDefaultValue(BoundExpression expr)
        {
            switch (expr.Kind)
            {
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)expr;
                        ConversionKind kind = boundConversion.Conversion.Kind;
                        if (kind == ConversionKind.DefaultLiteral || kind == ConversionKind.NullLiteral)
                        {
                            return IsDefaultValue(boundConversion.Operand);
                        }
                        return false;
                    }
                case BoundKind.DefaultLiteral:
                case BoundKind.DefaultExpression:
                    return true;
                default:
                    return false;
            }
        }

        private void ReportNullabilityMismatchInAssignment(SyntaxNode syntaxNode, object sourceType, object destinationType)
        {
            ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInAssignment, syntaxNode, sourceType, destinationType);
        }

        private void ReportNullabilityMismatchInAssignment(Location location, object sourceType, object destinationType)
        {
            ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInAssignment, location, sourceType, destinationType);
        }

        private void TrackNullableStateForAssignment(BoundExpression? valueOpt, TypeWithAnnotations targetType, int targetSlot, TypeWithState valueType, int valueSlot = -1)
        {
            if (!State.Reachable || !targetType.HasType || targetSlot <= 0 || targetSlot == valueSlot)
            {
                return;
            }
            if (!State.HasValue(targetSlot))
            {
                Normalize(ref State);
            }
            NullableFlowState state = valueType.State;
            SetStateAndTrackForFinally(ref State, targetSlot, state);
            InheritDefaultState(targetType.Type, targetSlot);
            if (!areEquivalentTypes(targetType, valueType))
            {
                return;
            }
            if (targetType.Type.IsReferenceType || targetType.TypeKind == TypeKind.TypeParameter || targetType.IsNullableType())
            {
                if (valueSlot > 0)
                {
                    InheritNullableStateOfTrackableType(targetSlot, valueSlot, targetSlot);
                }
            }
            else if (EmptyStructTypeCache.IsTrackableStructType(targetType.Type))
            {
                InheritNullableStateOfTrackableStruct(targetType.Type, targetSlot, valueSlot, valueOpt != null && IsDefaultValue(valueOpt), targetSlot);
            }
            static bool areEquivalentTypes(TypeWithAnnotations target, TypeWithState assignedValue)
            {
                return target.Type.Equals(assignedValue.Type, TypeCompareKind.AllIgnoreOptions);
            }
        }

        private void ReportNonSafetyDiagnostic(Location location)
        {
            ReportDiagnostic(ErrorCode.WRN_ConvertingNullableToNonNullable, location);
        }

        private void ReportDiagnostic(ErrorCode errorCode, SyntaxNode syntaxNode, params object[] arguments)
        {
            ReportDiagnostic(errorCode, syntaxNode.GetLocation(), arguments);
        }

        private void ReportDiagnostic(ErrorCode errorCode, Location location, params object[] arguments)
        {
            if (IsReachable() && !_disableDiagnostics)
            {
                base.Diagnostics.Add(errorCode, location, arguments);
            }
        }

        private void InheritNullableStateOfTrackableStruct(TypeSymbol targetType, int targetSlot, int valueSlot, bool isDefaultValue, int skipSlot = -1)
        {
            if (skipSlot < 0)
            {
                skipSlot = targetSlot;
            }
            if (!isDefaultValue && valueSlot > 0)
            {
                InheritNullableStateOfTrackableType(targetSlot, valueSlot, skipSlot);
                return;
            }
            foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(targetType))
            {
                InheritNullableStateOfMember(targetSlot, valueSlot, structInstanceField, isDefaultValue, skipSlot);
            }
        }

        private bool IsSlotMember(int slot, Symbol possibleMember)
        {
            TypeSymbol containingType = possibleMember.ContainingType;
            TypeSymbol source = NominalSlotType(slot);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            ConversionsBase conversionsBase = _conversions.WithNullability(includeNullability: false);
            if (!conversionsBase.HasIdentityOrImplicitReferenceConversion(source, containingType, ref useSiteInfo))
            {
                return conversionsBase.HasBoxingConversion(source, containingType, ref useSiteInfo);
            }
            return true;
        }

        private void InheritNullableStateOfMember(int targetContainerSlot, int valueContainerSlot, Symbol member, bool isDefaultValue, int skipSlot)
        {
            if (!IsSlotMember(targetContainerSlot, member))
            {
                return;
            }
            TypeWithAnnotations typeOrReturnType = member.GetTypeOrReturnType();
            if (typeOrReturnType.Type.IsReferenceType || typeOrReturnType.TypeKind == TypeKind.TypeParameter || typeOrReturnType.IsNullableType())
            {
                int orCreateSlot = GetOrCreateSlot(member, targetContainerSlot);
                if (orCreateSlot <= 0)
                {
                    return;
                }
                NullableFlowState newState = (isDefaultValue ? NullableFlowState.MaybeNull : typeOrReturnType.ToTypeWithState().State);
                int num = -1;
                if (valueContainerSlot > 0)
                {
                    num = VariableSlot(member, valueContainerSlot);
                    if (num == skipSlot)
                    {
                        return;
                    }
                    newState = (State.HasValue(num) ? State[num] : NullableFlowState.NotNull);
                }
                SetStateAndTrackForFinally(ref State, orCreateSlot, newState);
                if (num > 0)
                {
                    InheritNullableStateOfTrackableType(orCreateSlot, num, skipSlot);
                }
            }
            else
            {
                if (!EmptyStructTypeCache.IsTrackableStructType(typeOrReturnType.Type))
                {
                    return;
                }
                int orCreateSlot2 = GetOrCreateSlot(member, targetContainerSlot);
                if (orCreateSlot2 > 0)
                {
                    int num2 = ((valueContainerSlot > 0) ? GetOrCreateSlot(member, valueContainerSlot) : (-1));
                    if (num2 != skipSlot)
                    {
                        InheritNullableStateOfTrackableStruct(typeOrReturnType.Type, orCreateSlot2, num2, isDefaultValue, skipSlot);
                    }
                }
            }
        }

        private TypeSymbol NominalSlotType(int slot)
        {
            return _variables[slot].Symbol.GetTypeOrReturnType().Type;
        }

        private void SetStateAndTrackForFinally(ref LocalState state, int slot, NullableFlowState newState)
        {
            state[slot] = newState;
            if (newState != 0 && NonMonotonicState.HasValue)
            {
                LocalState value = NonMonotonicState.Value;
                if (value.HasVariable(slot))
                {
                    value[slot] = newState.Join(value[slot]);
                    NonMonotonicState = value;
                }
            }
        }

        protected override void JoinTryBlockState(ref LocalState self, ref LocalState other)
        {
            LocalState other2 = other.GetStateForVariables(self.Id);
            Join(ref self, ref other2);
        }

        private void InheritDefaultState(TypeSymbol targetType, int targetSlot)
        {
            ArrayBuilder<(VariableIdentifier, int)> instance = ArrayBuilder<(VariableIdentifier, int)>.GetInstance();
            _variables.GetMembers(instance, targetSlot);
            ArrayBuilder<(VariableIdentifier, int)>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                (VariableIdentifier, int) current = enumerator.Current;
                VariableIdentifier item = current.Item1;
                int item2 = current.Item2;
                Symbol symbol = AsMemberOfType(targetType, item.Symbol);
                SetStateAndTrackForFinally(ref State, item2, GetDefaultState(symbol));
                InheritDefaultState(symbol.GetTypeOrReturnType().Type, item2);
            }
            instance.Free();
        }

        private NullableFlowState GetDefaultState(Symbol symbol)
        {
            return ApplyUnconditionalAnnotations(symbol.GetTypeOrReturnType().ToTypeWithState(), GetRValueAnnotations(symbol)).State;
        }

        private void InheritNullableStateOfTrackableType(int targetSlot, int valueSlot, int skipSlot)
        {
            ArrayBuilder<(VariableIdentifier, int)> instance = ArrayBuilder<(VariableIdentifier, int)>.GetInstance();
            _variables.GetMembers(instance, valueSlot);
            ArrayBuilder<(VariableIdentifier, int)>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VariableIdentifier item = enumerator.Current.Item1;
                Symbol symbol = item.Symbol;
                InheritNullableStateOfMember(targetSlot, valueSlot, symbol, isDefaultValue: false, skipSlot);
            }
            instance.Free();
        }

        protected override LocalState TopState()
        {
            LocalState result = LocalState.ReachableState(_variables);
            result.PopulateAll(this);
            return result;
        }

        protected override LocalState UnreachableState()
        {
            return LocalState.UnreachableState(_variables);
        }

        protected override LocalState ReachableBottomState()
        {
            return LocalState.ReachableState(_variables);
        }

        private void EnterParameters()
        {
            if (!(CurrentSymbol is MethodSymbol methodSymbol))
            {
                return;
            }
            if (methodSymbol is SynthesizedRecordConstructor)
            {
                if (_hasInitialState)
                {
                    return;
                }
            }
            else if (methodSymbol.IsConstructor() && !_hasInitialState)
            {
                return;
            }
            MethodSymbol methodSymbol2 = methodSymbol.PartialDefinitionPart ?? methodSymbol;
            ImmutableArray<ParameterSymbol> parameters = methodSymbol2.Parameters;
            ImmutableArray<ParameterSymbol> parameters2 = (_useDelegateInvokeParameterTypes ? _delegateInvokeMethod : methodSymbol2).Parameters;
            LocalState other = State.Clone();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterSymbol parameterSymbol = parameters[i];
                TypeWithAnnotations parameterType = ((i >= parameters2.Length) ? parameterSymbol.TypeWithAnnotations : parameters2[i].TypeWithAnnotations);
                EnterParameter(parameterSymbol, parameterType);
            }
            Join(ref State, ref other);
        }

        private void EnterParameter(ParameterSymbol parameter, TypeWithAnnotations parameterType)
        {
            _variables.SetType(parameter, parameterType);
            int orCreateSlot = GetOrCreateSlot(parameter);
            if (orCreateSlot > 0)
            {
                NullableFlowState state = GetParameterState(parameterType, parameter.FlowAnalysisAnnotations).State;
                State[orCreateSlot] = state;
                if (EmptyStructTypeCache.IsTrackableStructType(parameterType.Type))
                {
                    InheritNullableStateOfTrackableStruct(parameterType.Type, orCreateSlot, -1, parameter.ExplicitDefaultConstantValue?.IsNull ?? false);
                }
            }
        }

        public override BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue equalsValue)
        {
            ParameterSymbol parameter = equalsValue.Parameter;
            FlowAnalysisAnnotations parameterAnnotations = GetParameterAnnotations(parameter);
            TypeWithAnnotations targetTypeOpt = ApplyLValueAnnotations(parameter.TypeWithAnnotations, parameterAnnotations);
            TypeWithState state = VisitOptionalImplicitConversion(equalsValue.Value, targetTypeOpt, useLegacyWarnings: false, trackMembers: false, AssignmentKind.Assignment);
            CheckDisallowedNullAssignment(state, parameterAnnotations, equalsValue.Value.Syntax.Location);
            return null;
        }

        private static TypeWithState GetParameterState(TypeWithAnnotations parameterType, FlowAnalysisAnnotations parameterAnnotations)
        {
            if ((parameterAnnotations & FlowAnalysisAnnotations.AllowNull) != 0)
            {
                return TypeWithState.Create(parameterType.Type, NullableFlowState.MaybeDefault);
            }
            if ((parameterAnnotations & FlowAnalysisAnnotations.DisallowNull) != 0)
            {
                return TypeWithState.Create(parameterType.Type, NullableFlowState.NotNull);
            }
            return parameterType.ToTypeWithState();
        }

        public sealed override BoundNode? VisitReturnStatement(BoundReturnStatement node)
        {
            BoundExpression expressionOpt = node.ExpressionOpt;
            if (expressionOpt == null)
            {
                EnforceDoesNotReturn(node.Syntax);
                base.PendingBranches.Add(new PendingBranch(node, State, null));
                SetUnreachable();
                return null;
            }
            if (_returnTypesOpt == null && TryGetReturnType(out var type, out var annotations))
            {
                if (node.RefKind == RefKind.None && type.Type.SpecialType == SpecialType.System_Boolean)
                {
                    Visit(expressionOpt);
                }
                else
                {
                    TypeWithState state = ((node.RefKind != 0) ? VisitRefExpression(expressionOpt, type) : VisitOptionalImplicitConversion(expressionOpt, type, useLegacyWarnings: false, trackMembers: false, AssignmentKind.Return));
                    CheckDisallowedNullAssignment(state, ToInwardAnnotations(annotations), node.Syntax.Location, expressionOpt);
                }
            }
            else
            {
                TypeWithState typeWithState = VisitRvalueWithState(expressionOpt);
                if (_returnTypesOpt != null)
                {
                    _returnTypesOpt!.Add((node, typeWithState.ToTypeWithAnnotations(compilation)));
                }
            }
            EnforceDoesNotReturn(node.Syntax);
            if (IsConditionalState)
            {
                LocalState self = StateWhenTrue.Clone();
                Join(ref self, ref StateWhenFalse);
                base.PendingBranches.Add(new PendingBranch(node, self, null, IsConditionalState, StateWhenTrue, StateWhenFalse));
            }
            else
            {
                base.PendingBranches.Add(new PendingBranch(node, State, null));
            }
            Unsplit();
            if (CurrentSymbol is MethodSymbol methodSymbol)
            {
                EnforceNotNullIfNotNull(node.Syntax, State, methodSymbol.Parameters, methodSymbol.ReturnNotNullIfParameterNotNull, ResultType.State, null);
            }
            SetUnreachable();
            return null;
        }

        private TypeWithState VisitRefExpression(BoundExpression expr, TypeWithAnnotations destinationType)
        {
            Visit(expr);
            TypeWithState resultType = ResultType;
            if (!expr.IsSuppressed && RemoveConversion(expr, includeExplicitConversions: false).expression.Kind != BoundKind.ThrowExpression)
            {
                TypeWithAnnotations lvalueResultType = LvalueResultType;
                if (IsNullabilityMismatch(lvalueResultType, destinationType))
                {
                    ReportNullabilityMismatchInAssignment(expr.Syntax, lvalueResultType, destinationType);
                }
                else
                {
                    ReportNullableAssignmentIfNecessary(expr, destinationType, resultType, useLegacyWarnings: false);
                }
            }
            return resultType;
        }

        private bool TryGetReturnType(out TypeWithAnnotations type, out FlowAnalysisAnnotations annotations)
        {
            if (!(CurrentSymbol is MethodSymbol methodSymbol))
            {
                type = default(TypeWithAnnotations);
                annotations = FlowAnalysisAnnotations.None;
                return false;
            }
            MethodSymbol methodSymbol2 = _delegateInvokeMethod ?? methodSymbol;
            TypeWithAnnotations returnTypeWithAnnotations = methodSymbol2.ReturnTypeWithAnnotations;
            if (returnTypeWithAnnotations.IsVoidType())
            {
                type = default(TypeWithAnnotations);
                annotations = FlowAnalysisAnnotations.None;
                return false;
            }
            if (!methodSymbol.IsAsync)
            {
                annotations = methodSymbol2.ReturnTypeFlowAnalysisAnnotations;
                type = ApplyUnconditionalAnnotations(returnTypeWithAnnotations, annotations);
                return true;
            }
            if (returnTypeWithAnnotations.Type.IsGenericTaskType(compilation))
            {
                type = ((NamedTypeSymbol)returnTypeWithAnnotations.Type).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.Single();
                annotations = FlowAnalysisAnnotations.None;
                return true;
            }
            type = default(TypeWithAnnotations);
            annotations = FlowAnalysisAnnotations.None;
            return false;
        }

        public override BoundNode? VisitLocal(BoundLocal node)
        {
            LocalSymbol localSymbol = node.LocalSymbol;
            int orCreateSlot = GetOrCreateSlot(localSymbol);
            TypeWithAnnotations lvalueType = GetDeclaredLocalResult(localSymbol);
            if (!node.Type.Equals(lvalueType.Type, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreNullableModifiersForReferenceTypes))
            {
                lvalueType = TypeWithAnnotations.Create(node.Type, lvalueType.NullableAnnotation);
            }
            SetResult(node, GetAdjustedResult(lvalueType.ToTypeWithState(), orCreateSlot), lvalueType);
            SplitIfBooleanConstant(node);
            return null;
        }

        public override BoundNode? VisitBlock(BoundBlock node)
        {
            DeclareLocals(node.Locals);
            VisitStatementsWithLocalFunctions(node);
            return null;
        }

        private void VisitStatementsWithLocalFunctions(BoundBlock block)
        {
            if (!TrackingRegions && !block.LocalFunctions.IsDefaultOrEmpty)
            {
                ImmutableArray<BoundStatement>.Enumerator enumerator = block.Statements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundStatement current = enumerator.Current;
                    if (current.Kind != BoundKind.LocalFunctionStatement)
                    {
                        VisitStatement(current);
                    }
                }
                enumerator = block.Statements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is BoundLocalFunctionStatement node)
                    {
                        VisitLocalFunctionStatement(node);
                    }
                }
            }
            else
            {
                ImmutableArray<BoundStatement>.Enumerator enumerator = block.Statements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundStatement current2 = enumerator.Current;
                    VisitStatement(current2);
                }
            }
        }

        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            LocalFunctionSymbol localFunc = node.Symbol;
            LocalFunctionState orCreateLocalFuncUsages = GetOrCreateLocalFuncUsages(localFunc);
            LocalState state = TopState();
            LocalState startingState = orCreateLocalFuncUsages.StartingState;
            startingState.ForEach(delegate (int slot, Variables variables)
            {
                if (Symbol.IsCaptured(variables[variables.RootSlot(slot)].Symbol, localFunc))
                {
                    state[slot] = startingState[slot];
                }
            }, _variables);
            orCreateLocalFuncUsages.Visited = true;
            AnalyzeLocalFunctionOrLambda(node, localFunc, state, null, useDelegateInvokeParameterTypes: false);
            SetInvalidResult();
            return null;
        }

        private Variables GetOrCreateNestedFunctionVariables(Variables container, MethodSymbol lambdaOrLocalFunction)
        {
            if (_nestedFunctionVariables == null)
            {
                _nestedFunctionVariables = PooledDictionary<MethodSymbol, Variables>.GetInstance();
            }
            if (!_nestedFunctionVariables!.TryGetValue(lambdaOrLocalFunction, out var value))
            {
                value = container.CreateNestedMethodScope(lambdaOrLocalFunction);
                _nestedFunctionVariables!.Add(lambdaOrLocalFunction, value);
            }
            return value;
        }

        private void AnalyzeLocalFunctionOrLambda(IBoundLambdaOrFunction lambdaOrFunction, MethodSymbol lambdaOrFunctionSymbol, LocalState state, MethodSymbol? delegateInvokeMethod, bool useDelegateInvokeParameterTypes)
        {
            Symbol currentSymbol = CurrentSymbol;
            CurrentSymbol = lambdaOrFunctionSymbol;
            MethodSymbol? delegateInvokeMethod2 = _delegateInvokeMethod;
            _delegateInvokeMethod = delegateInvokeMethod;
            bool useDelegateInvokeParameterTypes2 = _useDelegateInvokeParameterTypes;
            _useDelegateInvokeParameterTypes = useDelegateInvokeParameterTypes;
            ArrayBuilder<(BoundReturnStatement, TypeWithAnnotations)>? returnTypesOpt = _returnTypesOpt;
            _returnTypesOpt = null;
            LocalState state2 = State;
            _variables = GetOrCreateNestedFunctionVariables(_variables, lambdaOrFunctionSymbol);
            State = state.CreateNestedMethodState(_variables);
            int previousSlot = _snapshotBuilderOpt?.EnterNewWalker(lambdaOrFunctionSymbol) ?? (-1);
            try
            {
                SavedPending oldPending = SavePending();
                EnterParameters();
                SavedPending oldPending2 = SavePending();
                if (lambdaOrFunctionSymbol.IsIterator)
                {
                    base.PendingBranches.Add(new PendingBranch(null, State, null));
                }
                VisitAlways(lambdaOrFunction.Body);
                RestorePending(oldPending2);
                RemoveReturns();
                RestorePending(oldPending);
            }
            finally
            {
                _snapshotBuilderOpt?.ExitWalker(SaveSharedState(), previousSlot);
            }
            _variables = _variables.Container;
            State = state2;
            _returnTypesOpt = returnTypesOpt;
            _useDelegateInvokeParameterTypes = useDelegateInvokeParameterTypes2;
            _delegateInvokeMethod = delegateInvokeMethod2;
            CurrentSymbol = currentSymbol;
        }

        protected override void VisitLocalFunctionUse(LocalFunctionSymbol symbol, LocalFunctionState localFunctionState, SyntaxNode syntax, bool isCall)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private void VisitLocalFunctionUse(LocalFunctionSymbol symbol)
        {
            LocalFunctionState orCreateLocalFuncUsages = GetOrCreateLocalFuncUsages(symbol);
            LocalState other = State.GetStateForVariables(orCreateLocalFuncUsages.StartingState.Id);
            if (Join(ref orCreateLocalFuncUsages.StartingState, ref other) && orCreateLocalFuncUsages.Visited)
            {
                stateChangedAfterUse = true;
            }
        }

        public override BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            DeclareLocals(node.Locals);
            return base.VisitDoStatement(node);
        }

        public override BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            DeclareLocals(node.Locals);
            return base.VisitWhileStatement(node);
        }

        public override BoundNode? VisitWithExpression(BoundWithExpression withExpr)
        {
            BoundExpression receiver = withExpr.Receiver;
            VisitRvalue(receiver);
            CheckPossibleNullReceiver(receiver);
            TypeWithAnnotations typeWithAnnotations = withExpr.CloneMethod?.ReturnTypeWithAnnotations ?? ResultType.ToTypeWithAnnotations(compilation);
            TypeWithState typeWithState = ApplyUnconditionalAnnotations(typeWithAnnotations.ToTypeWithState(), GetRValueAnnotations(withExpr.CloneMethod));
            int orCreatePlaceholderSlot = GetOrCreatePlaceholderSlot(withExpr);
            TrackNullableStateForAssignment(receiver, typeWithAnnotations, orCreatePlaceholderSlot, typeWithState, MakeSlot(receiver));
            SetResult(withExpr, typeWithState, typeWithAnnotations);
            VisitObjectCreationInitializer(null, orCreatePlaceholderSlot, withExpr.InitializerExpression, FlowAnalysisAnnotations.None);
            return null;
        }

        public override BoundNode? VisitForStatement(BoundForStatement node)
        {
            DeclareLocals(node.OuterLocals);
            DeclareLocals(node.InnerLocals);
            return base.VisitForStatement(node);
        }

        public override BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            DeclareLocals(node.IterationVariables);
            return base.VisitForEachStatement(node);
        }

        public override BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            DeclareLocals(node.Locals);
            Visit(node.AwaitOpt);
            return base.VisitUsingStatement(node);
        }

        public override BoundNode? VisitUsingLocalDeclarations(BoundUsingLocalDeclarations node)
        {
            Visit(node.AwaitOpt);
            return base.VisitUsingLocalDeclarations(node);
        }

        public override BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            DeclareLocals(node.Locals);
            return base.VisitFixedStatement(node);
        }

        public override BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            DeclareLocals(node.Locals);
            return base.VisitConstructorMethodBody(node);
        }

        private void DeclareLocal(LocalSymbol local)
        {
            if (local.DeclarationKind != 0)
            {
                int orCreateSlot = GetOrCreateSlot(local);
                if (orCreateSlot > 0)
                {
                    State[orCreateSlot] = GetDefaultState(ref State, orCreateSlot);
                    InheritDefaultState(GetDeclaredLocalResult(local).Type, orCreateSlot);
                }
            }
        }

        private void DeclareLocals(ImmutableArray<LocalSymbol> locals)
        {
            ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                DeclareLocal(current);
            }
        }

        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            LocalSymbol localSymbol = node.LocalSymbol;
            int orCreateSlot = GetOrCreateSlot(localSymbol);
            bool disableDiagnostics = _disableDiagnostics;
            _disableDiagnostics = true;
            LocalState state = State;
            VisitAndUnsplitAll(node.ArgumentsOpt);
            _disableDiagnostics = disableDiagnostics;
            SetState(state);
            if (node.DeclaredTypeOpt != null)
            {
                VisitTypeExpression(node.DeclaredTypeOpt);
            }
            BoundExpression? initializerOpt = node.InitializerOpt;
            if (initializerOpt == null)
            {
                return null;
            }
            TypeWithAnnotations typeWithAnnotations = localSymbol.TypeWithAnnotations;
            bool inferredType = node.InferredType;
            TypeWithState valueType = ((!localSymbol.IsRef) ? VisitOptionalImplicitConversion(initializerOpt, inferredType ? default(TypeWithAnnotations) : typeWithAnnotations, useLegacyWarnings: true, trackMembers: true, AssignmentKind.Assignment) : VisitRefExpression(initializerOpt, typeWithAnnotations));
            if (inferredType)
            {
                if (valueType.HasNullType)
                {
                    valueType = typeWithAnnotations.ToTypeWithState();
                }
                typeWithAnnotations = valueType.ToAnnotatedTypeWithAnnotations(compilation);
                _variables.SetType(localSymbol, typeWithAnnotations);
                if (node.DeclaredTypeOpt != null)
                {
                    SetAnalyzedNullability(node.DeclaredTypeOpt, new VisitResult(typeWithAnnotations.ToTypeWithState(), typeWithAnnotations), true);
                }
            }
            TrackNullableStateForAssignment(initializerOpt, typeWithAnnotations, orCreateSlot, valueType, MakeSlot(initializerOpt));
            return null;
        }

        protected override BoundExpression? VisitExpressionWithoutStackGuard(BoundExpression node)
        {
            SetInvalidResult();
            base.VisitExpressionWithoutStackGuard(node);
            TypeWithState resultType = ResultType;
            if (ShouldMakeNotNullRvalue(node))
            {
                TypeWithState resultType2 = resultType.WithNotNullState();
                SetResult(node, resultType2, LvalueResultType);
            }
            return null;
        }

        private static bool AreLambdaAndNewDelegateSimilar(LambdaSymbol l, NamedTypeSymbol n)
        {
            MethodSymbol delegateInvokeMethod = n.DelegateInvokeMethod;
            if (delegateInvokeMethod.Parameters.SequenceEqual(l.Parameters, (ParameterSymbol p1, ParameterSymbol p2) => p1.Type.Equals(p2.Type, TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.IgnoreTupleNames)))
            {
                return delegateInvokeMethod.ReturnType.Equals(l.ReturnType, TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.IgnoreTupleNames);
            }
            return false;
        }

        public override BoundNode? Visit(BoundNode? node)
        {
            return Visit(node, expressionIsRead: true);
        }

        private BoundNode VisitLValue(BoundNode node)
        {
            return Visit(node, expressionIsRead: false);
        }

        private BoundNode Visit(BoundNode? node, bool expressionIsRead)
        {
            bool expressionIsRead2 = _expressionIsRead;
            _expressionIsRead = expressionIsRead;
            TakeIncrementalSnapshot(node);
            BoundNode result = base.Visit(node);
            _expressionIsRead = expressionIsRead2;
            return result;
        }

        protected override void VisitStatement(BoundStatement statement)
        {
            SetInvalidResult();
            base.VisitStatement(statement);
            SetInvalidResult();
        }

        public override BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = node.Arguments;
            ImmutableArray<VisitArgumentResult> item = VisitArguments(node, arguments, node.ArgumentRefKindsOpt, node.Constructor, node.ArgsToParamsOpt, node.DefaultArguments, node.Expanded, invokedAsExtensionMethod: false).results;
            VisitObjectOrDynamicObjectCreation(node, arguments, item, node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            SetResultType(node, TypeWithState.Create(null, NullableFlowState.NotNull));
            return null;
        }

        private void VisitObjectOrDynamicObjectCreation(BoundExpression node, ImmutableArray<BoundExpression> arguments, ImmutableArray<VisitArgumentResult> argumentResults, BoundExpression? initializerOpt)
        {
            ImmutableArray<TypeWithState> types = argumentResults.SelectAsArray((VisitArgumentResult ar) => ar.RValueType);
            int num = -1;
            TypeSymbol type = node.Type;
            NullableFlowState nullableFlowState = NullableFlowState.NotNull;
            if ((object)type != null)
            {
                num = GetOrCreatePlaceholderSlot(node);
                if (num > 0)
                {
                    BoundObjectCreationExpression? boundObjectCreationExpression = node as BoundObjectCreationExpression;
                    MethodSymbol? methodSymbol = boundObjectCreationExpression?.Constructor;
                    bool flag = methodSymbol?.IsDefaultValueTypeConstructor() ?? false;
                    if (EmptyStructTypeCache.IsTrackableStructType(type))
                    {
                        NamedTypeSymbol? namedTypeSymbol = methodSymbol?.ContainingType;
                        if (namedTypeSymbol is object && namedTypeSymbol.IsTupleType && !flag)
                        {
                            TrackNullableStateOfTupleElements(num, namedTypeSymbol, arguments, types, boundObjectCreationExpression.ArgsToParamsOpt, useRestField: true);
                        }
                        else
                        {
                            InheritNullableStateOfTrackableStruct(type, num, -1, flag);
                        }
                    }
                    else if (type.IsNullableType())
                    {
                        if (flag)
                        {
                            nullableFlowState = NullableFlowState.MaybeNull;
                        }
                        else if (methodSymbol is object && methodSymbol.ParameterCount == 1 && AreNullableAndUnderlyingTypes(type, methodSymbol.ParameterTypesWithAnnotations[0].Type, out TypeWithAnnotations underlyingTypeWithAnnotations))
                        {
                            BoundExpression boundExpression = arguments[0];
                            int num2 = MakeSlot(boundExpression);
                            if (num2 > 0)
                            {
                                TrackNullableStateOfNullableValue(num, type, boundExpression, underlyingTypeWithAnnotations.ToTypeWithState(), num2);
                            }
                        }
                    }
                    State[num] = nullableFlowState;
                }
            }
            if (initializerOpt != null)
            {
                VisitObjectCreationInitializer(null, num, initializerOpt, FlowAnalysisAnnotations.None);
            }
            SetResultType(node, TypeWithState.Create(type, nullableFlowState));
        }

        private void VisitObjectCreationInitializer(Symbol? containingSymbol, int containingSlot, BoundExpression node, FlowAnalysisAnnotations leftAnnotations)
        {
            Symbol? containingSymbol2 = containingSymbol;
            TakeIncrementalSnapshot(node);
            ImmutableArray<BoundExpression>.Enumerator enumerator;
            if (!(node is BoundObjectInitializerExpression boundObjectInitializerExpression))
            {
                if (node is BoundCollectionInitializerExpression boundCollectionInitializerExpression)
                {
                    checkImplicitReceiver(boundCollectionInitializerExpression);
                    enumerator = boundCollectionInitializerExpression.Initializers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        BoundExpression current = enumerator.Current;
                        if (current.Kind == BoundKind.CollectionElementInitializer)
                        {
                            VisitCollectionElementInitializer((BoundCollectionElementInitializer)current);
                        }
                        else
                        {
                            VisitRvalue(current);
                        }
                    }
                    SetNotNullResult(boundCollectionInitializerExpression.Placeholder);
                }
                else if (containingSymbol2 is object)
                {
                    TypeWithAnnotations typeWithAnnotations = ApplyLValueAnnotations(containingSymbol2.GetTypeOrReturnType(), leftAnnotations);
                    TypeWithState valueType = VisitOptionalImplicitConversion(node, typeWithAnnotations, useLegacyWarnings: false, trackMembers: true, AssignmentKind.Assignment);
                    TrackNullableStateForAssignment(node, typeWithAnnotations, containingSlot, valueType, MakeSlot(node));
                }
                return;
            }
            checkImplicitReceiver(boundObjectInitializerExpression);
            enumerator = boundObjectInitializerExpression.Initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current2 = enumerator.Current;
                if (current2.Kind == BoundKind.AssignmentOperator)
                {
                    VisitObjectElementInitializer(containingSlot, (BoundAssignmentOperator)current2);
                }
                else
                {
                    VisitRvalue(current2);
                }
            }
            SetNotNullResult(boundObjectInitializerExpression.Placeholder);
            void checkImplicitReceiver(BoundObjectInitializerExpressionBase node)
            {
                if (containingSlot >= 0 && !node.Initializers.IsEmpty && !node.Type.IsValueType && State[containingSlot].MayBeNull())
                {
                    if (containingSymbol2 is null)
                    {
                        ReportDiagnostic(ErrorCode.WRN_NullReferenceReceiver, node.Syntax);
                    }
                    else
                    {
                        ReportDiagnostic(ErrorCode.WRN_NullReferenceInitializer, node.Syntax, containingSymbol2);
                    }
                }
            }
        }

        private void VisitObjectElementInitializer(int containingSlot, BoundAssignmentOperator node)
        {
            TakeIncrementalSnapshot(node);
            BoundExpression left = node.Left;
            if (left.Kind == BoundKind.ObjectInitializerMember)
            {
                BoundObjectInitializerMember boundObjectInitializerMember = (BoundObjectInitializerMember)left;
                TakeIncrementalSnapshot(left);
                Symbol? memberSymbol = boundObjectInitializerMember.MemberSymbol;
                if (!boundObjectInitializerMember.Arguments.IsDefaultOrEmpty)
                {
                    VisitArguments(boundObjectInitializerMember, boundObjectInitializerMember.Arguments, boundObjectInitializerMember.ArgumentRefKindsOpt, (PropertySymbol)memberSymbol, boundObjectInitializerMember.ArgsToParamsOpt, boundObjectInitializerMember.DefaultArguments, boundObjectInitializerMember.Expanded);
                }
                if (memberSymbol is object)
                {
                    int containingSlot2 = ((containingSlot < 0 || !IsSlotMember(containingSlot, memberSymbol)) ? (-1) : GetOrCreateSlot(memberSymbol, containingSlot));
                    VisitObjectCreationInitializer(memberSymbol, containingSlot2, node.Right, GetLValueAnnotations(node.Left));
                }
                VisitResult result = new VisitResult(boundObjectInitializerMember.Type, NullableAnnotation.NotAnnotated, NullableFlowState.NotNull);
                SetAnalyzedNullability(boundObjectInitializerMember, result);
                SetAnalyzedNullability(node, result);
            }
            else
            {
                Visit(node);
            }
        }

        private new void VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            MethodSymbol? item = VisitArguments(node, node.Arguments, default(ImmutableArray<RefKind>), node.AddMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.Expanded, node.InvokedAsExtensionMethod).method;
            if (node.ImplicitReceiverOpt != null)
            {
                SetAnalyzedNullability(node.ImplicitReceiverOpt, new VisitResult(node.ImplicitReceiverOpt!.Type, NullableAnnotation.NotAnnotated, NullableFlowState.NotNull));
            }
            SetUnknownResultNullability(node);
            SetUpdatedSymbol(node, node.AddMethod, item);
        }

        private void SetNotNullResult(BoundExpression node)
        {
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
        }

        protected override bool IsEmptyStructType(TypeSymbol type)
        {
            if (type.TypeKind != TypeKind.Struct)
            {
                return false;
            }
            if (!_emptyStructTypeCache.IsEmptyStructType(type))
            {
                return false;
            }
            if (type.SpecialType != 0)
            {
                return true;
            }
            ImmutableArray<Symbol> membersUnordered = ((NamedTypeSymbol)type).GetMembersUnordered();
            if (membersUnordered.Any((Symbol m) => m.Kind == SymbolKind.Field))
            {
                return true;
            }
            if (membersUnordered.Any((Symbol m) => m.Kind == SymbolKind.Property))
            {
                return false;
            }
            return true;
        }

        private int GetOrCreatePlaceholderSlot(BoundExpression node)
        {
            if (IsEmptyStructType(node.Type))
            {
                return -1;
            }
            return GetOrCreatePlaceholderSlot(node, TypeWithAnnotations.Create(node.Type, NullableAnnotation.NotAnnotated));
        }

        private int GetOrCreatePlaceholderSlot(object identifier, TypeWithAnnotations type)
        {
            if (_placeholderLocalsOpt == null)
            {
                _placeholderLocalsOpt = PooledDictionary<object, PlaceholderLocal>.GetInstance();
            }
            if (!_placeholderLocalsOpt!.TryGetValue(identifier, out var value))
            {
                value = new PlaceholderLocal(CurrentSymbol, identifier, type);
                _placeholderLocalsOpt!.Add(identifier, value);
            }
            return GetOrCreateSlot(value, 0, forceSlotEvenIfEmpty: true);
        }

        public override BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            NamedTypeSymbol type = (NamedTypeSymbol)node.Type;
            ImmutableArray<BoundExpression> arguments = node.Arguments;
            ImmutableArray<TypeWithState> items = arguments.SelectAsArray((BoundExpression arg, NullableWalker self) => self.VisitRvalueWithState(arg), this);
            ImmutableArray<TypeWithAnnotations> immutableArray = items.SelectAsArray((TypeWithState arg) => arg.ToTypeWithAnnotations(compilation));
            if (immutableArray.All((TypeWithAnnotations argType) => argType.HasType))
            {
                type = AnonymousTypeManager.ConstructAnonymousTypeSymbol(type, immutableArray);
                int orCreatePlaceholderSlot = GetOrCreatePlaceholderSlot(node);
                int currentDeclarationIndex2 = 0;
                for (int i = 0; i < arguments.Length; i++)
                {
                    BoundExpression boundExpression = arguments[i];
                    TypeWithState typeWithState = items[i];
                    PropertySymbol anonymousTypeProperty = AnonymousTypeManager.GetAnonymousTypeProperty(type, i);
                    if (anonymousTypeProperty.Type.SpecialType != SpecialType.System_Void)
                    {
                        int orCreateSlot = GetOrCreateSlot(anonymousTypeProperty, orCreatePlaceholderSlot);
                        TrackNullableStateForAssignment(boundExpression, anonymousTypeProperty.TypeWithAnnotations, orCreateSlot, typeWithState, MakeSlot(boundExpression));
                        BoundAnonymousPropertyDeclaration? boundAnonymousPropertyDeclaration = getDeclaration(node, anonymousTypeProperty, ref currentDeclarationIndex2);
                        if (boundAnonymousPropertyDeclaration != null)
                        {
                            TakeIncrementalSnapshot(boundAnonymousPropertyDeclaration);
                            SetAnalyzedNullability(boundAnonymousPropertyDeclaration, new VisitResult(typeWithState, anonymousTypeProperty.TypeWithAnnotations));
                        }
                    }
                }
            }
            SetResultType(node, TypeWithState.Create(type, NullableFlowState.NotNull));
            return null;
            static BoundAnonymousPropertyDeclaration? getDeclaration(BoundAnonymousObjectCreationExpression node, PropertySymbol currentProperty, ref int currentDeclarationIndex)
            {
                if (currentDeclarationIndex >= node.Declarations.Length)
                {
                    return null;
                }
                BoundAnonymousPropertyDeclaration boundAnonymousPropertyDeclaration2 = node.Declarations[currentDeclarationIndex];
                if (boundAnonymousPropertyDeclaration2.Property.MemberIndexOpt == currentProperty.MemberIndexOpt)
                {
                    currentDeclarationIndex++;
                    return boundAnonymousPropertyDeclaration2;
                }
                return null;
            }
        }

        public override BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Bounds.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                VisitRvalue(current);
            }
            BoundArrayInitialization? initializerOpt = node.InitializerOpt;
            if (initializerOpt == null)
            {
                SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
                return null;
            }
            TakeIncrementalSnapshot(initializerOpt);
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(initializerOpt.Initializers.Length);
            GetArrayElements(initializerOpt, instance);
            int count = instance.Count;
            bool num = node.Syntax.Kind() == SyntaxKind.ImplicitArrayCreationExpression;
            ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)node.Type;
            TypeWithAnnotations elementTypeWithAnnotations = arrayTypeSymbol.ElementTypeWithAnnotations;
            if (!num)
            {
                ArrayBuilder<BoundExpression>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundExpression current2 = enumerator2.Current;
                    VisitOptionalImplicitConversion(current2, elementTypeWithAnnotations, useLegacyWarnings: false, trackMembers: false, AssignmentKind.Assignment);
                }
            }
            else
            {
                ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance(count);
                ArrayBuilder<Conversion> instance3 = ArrayBuilder<Conversion>.GetInstance(count);
                ArrayBuilder<TypeWithState> instance4 = ArrayBuilder<TypeWithState>.GetInstance(count);
                ArrayBuilder<BoundExpression> instance5 = ArrayBuilder<BoundExpression>.GetInstance(count);
                ArrayBuilder<BoundExpression>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundExpression current3 = enumerator2.Current;
                    var (boundExpression, item) = RemoveConversion(current3, includeExplicitConversions: false);
                    instance2.Add(boundExpression);
                    instance3.Add(item);
                    SnapshotWalkerThroughConversionGroup(current3, boundExpression);
                    TypeWithState item2 = VisitRvalueWithState(boundExpression);
                    instance4.Add(item2);
                    instance5.Add(CreatePlaceholderIfNecessary(boundExpression, item2.ToTypeWithAnnotations(compilation)));
                }
                ImmutableArray<BoundExpression> exprs = instance5.ToImmutableAndFree();
                TypeSymbol? typeSymbol = null;
                if (!node.HasErrors)
                {
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    typeSymbol = BestTypeInferrer.InferBestType(exprs, _conversions, ref useSiteInfo);
                }
                TypeWithAnnotations typeWithAnnotations = ((typeSymbol is null) ? elementTypeWithAnnotations.SetUnknownNullabilityForReferenceTypes() : TypeWithAnnotations.Create(typeSymbol));
                if (typeSymbol is object)
                {
                    for (int i = 0; i < count; i++)
                    {
                        BoundExpression boundExpression2 = instance2[i];
                        BoundConversion? conversionIfApplicable = GetConversionIfApplicable(instance[i], boundExpression2);
                        instance4[i] = VisitConversion(conversionIfApplicable, boundExpression2, instance3[i], typeWithAnnotations, instance4[i], checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment, null, reportTopLevelWarnings: false);
                    }
                    NullableFlowState nullableState = BestTypeInferrer.GetNullableState(instance4);
                    typeWithAnnotations = TypeWithState.Create(typeWithAnnotations.Type, nullableState).ToTypeWithAnnotations(compilation);
                    for (int j = 0; j < count; j++)
                    {
                        VisitConversion(null, instance2[j], Conversion.Identity, typeWithAnnotations, instance4[j], checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment, null, reportTopLevelWarnings: true, reportRemainingWarnings: false);
                    }
                }
                else
                {
                    for (int k = 0; k < count; k++)
                    {
                        TrackAnalyzedNullabilityThroughConversionGroup(typeWithAnnotations.ToTypeWithState(), instance[k] as BoundConversion, instance2[k]);
                    }
                }
                instance2.Free();
                instance3.Free();
                instance4.Free();
                arrayTypeSymbol = arrayTypeSymbol.WithElementType(typeWithAnnotations);
            }
            instance.Free();
            SetResultType(node, TypeWithState.Create(arrayTypeSymbol, NullableFlowState.NotNull));
            return null;
        }

        internal static TypeWithAnnotations BestTypeForLambdaReturns(ArrayBuilder<(BoundExpression, TypeWithAnnotations)> returns, Binder binder, BoundNode node, Conversions conversions)
        {
            NullableWalker nullableWalker = new NullableWalker(binder.Compilation, null, useConstructorExitWarnings: false, useDelegateInvokeParameterTypes: false, null, node, binder, conversions, null, null, null, null);
            int count = returns.Count;
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(count);
            ArrayBuilder<BoundExpression> instance2 = ArrayBuilder<BoundExpression>.GetInstance(count);
            for (int i = 0; i < count; i++)
            {
                var (expr, typeWithAnnotations) = returns[i];
                instance.Add(typeWithAnnotations);
                instance2.Add(CreatePlaceholderIfNecessary(expr, typeWithAnnotations));
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            ImmutableArray<BoundExpression> exprs = instance2.ToImmutableAndFree();
            TypeSymbol? typeSymbol = BestTypeInferrer.InferBestType(exprs, nullableWalker._conversions, ref useSiteInfo);
            TypeWithAnnotations result;
            if ((object)typeSymbol != null)
            {
                TypeWithAnnotations targetTypeWithNullability = TypeWithAnnotations.Create(typeSymbol);
                ConversionsBase conversionsBase = nullableWalker._conversions.WithNullability(includeNullability: false);
                for (int j = 0; j < count; j++)
                {
                    BoundExpression boundExpression = exprs[j];
                    Conversion conversion = conversionsBase.ClassifyConversionFromExpression(boundExpression, typeSymbol, ref useSiteInfo);
                    instance[j] = nullableWalker.VisitConversion(null, boundExpression, conversion, targetTypeWithNullability, instance[j].ToTypeWithState(), checkConversion: false, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Return, null, reportTopLevelWarnings: false, reportRemainingWarnings: false).ToTypeWithAnnotations(binder.Compilation);
                }
                result = TypeWithAnnotations.Create(typeSymbol, BestTypeInferrer.GetNullableAnnotation(instance));
            }
            else
            {
                result = default(TypeWithAnnotations);
            }
            instance.Free();
            nullableWalker.Free();
            return result;
        }

        private static void GetArrayElements(BoundArrayInitialization node, ArrayBuilder<BoundExpression> builder)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current.Kind == BoundKind.ArrayInitialization)
                {
                    GetArrayElements((BoundArrayInitialization)current, builder);
                }
                else
                {
                    builder.Add(current);
                }
            }
        }

        public override BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            Visit(node.Expression);
            CheckPossibleNullReceiver(node.Expression);
            ArrayTypeSymbol arrayTypeSymbol = ResultType.Type as ArrayTypeSymbol;
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.Indices.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                VisitRvalue(current);
            }
            TypeWithAnnotations type = ((node.Indices.Length != 1 || !TypeSymbol.Equals(node.Indices[0].Type, compilation.GetWellKnownType(WellKnownType.System_Range), TypeCompareKind.ConsiderEverything)) ? (arrayTypeSymbol?.ElementTypeWithAnnotations ?? default(TypeWithAnnotations)) : TypeWithAnnotations.Create(arrayTypeSymbol));
            SetLvalueResultType(node, type);
            return null;
        }

        private TypeWithState InferResultNullability(BinaryOperatorKind operatorKind, MethodSymbol? methodOpt, TypeSymbol resultType, TypeWithState leftType, TypeWithState rightType)
        {
            NullableFlowState defaultState = NullableFlowState.NotNull;
            if (operatorKind.IsUserDefined())
            {
                if ((object)methodOpt != null && methodOpt!.ParameterCount == 2)
                {
                    if (operatorKind.IsLifted() && !operatorKind.IsComparison())
                    {
                        return GetLiftedReturnType(methodOpt!.ReturnTypeWithAnnotations, leftType.State.Join(rightType.State));
                    }
                    TypeWithState result = GetReturnTypeWithState(methodOpt);
                    if ((leftType.IsNotNull && methodOpt!.ReturnNotNullIfParameterNotNull.Contains(methodOpt!.Parameters[0].Name)) || (rightType.IsNotNull && methodOpt!.ReturnNotNullIfParameterNotNull.Contains(methodOpt!.Parameters[1].Name)))
                    {
                        result = result.WithNotNullState();
                    }
                    return result;
                }
            }
            else if (!operatorKind.IsDynamic() && !resultType.IsValueType)
            {
                defaultState = (operatorKind.Operator() | operatorKind.OperandTypes()) switch
                {
                    BinaryOperatorKind.DelegateCombination => leftType.State.Meet(rightType.State),
                    BinaryOperatorKind.DelegateRemoval => NullableFlowState.MaybeNull,
                    _ => NullableFlowState.NotNull,
                };
            }
            if (operatorKind.IsLifted() && !operatorKind.IsComparison())
            {
                defaultState = leftType.State.Join(rightType.State);
            }
            return TypeWithState.Create(resultType, defaultState);
        }

        protected override void VisitBinaryOperatorChildren(ArrayBuilder<BoundBinaryOperator> stack)
        {
            BoundBinaryOperator binary = stack.Pop();
            var (leftOperand, leftConversion) = RemoveConversion(binary.Left, includeExplicitConversions: false);
            Visit(leftOperand);
            while (true)
            {
                if (!learnFromBooleanConstantTest())
                {
                    Unsplit();
                    UseRvalueOnly(leftOperand);
                    AfterLeftChildHasBeenVisited(leftOperand, leftConversion, binary);
                }
                if (stack.Count != 0)
                {
                    leftOperand = binary;
                    leftConversion = Conversion.Identity;
                    binary = stack.Pop();
                    continue;
                }
                break;
            }
            bool learnFromBooleanConstantTest()
            {
                if (!IsConditionalState)
                {
                    return false;
                }
                if (!leftConversion.IsIdentity)
                {
                    return false;
                }
                BinaryOperatorKind binaryOperatorKind = binary.OperatorKind.Operator();
                if (binaryOperatorKind != BinaryOperatorKind.Equal && binaryOperatorKind != BinaryOperatorKind.NotEqual)
                {
                    return false;
                }
                ConstantValue? constantValue = binary.Right.ConstantValue;
                bool flag;
                if ((object)constantValue != null && constantValue!.IsBoolean)
                {
                    UseRvalueOnly(leftOperand);
                    LocalState whenTrue = StateWhenTrue.Clone();
                    LocalState whenFalse = StateWhenFalse.Clone();
                    Unsplit();
                    Visit(binary.Right);
                    UseRvalueOnly(binary.Right);
                    SetConditionalState(whenTrue, whenFalse);
                    flag = binaryOperatorKind == BinaryOperatorKind.Equal == binary.Right.ConstantValue!.BooleanValue;
                }
                else
                {
                    ConstantValue? constantValue2 = binary.Left.ConstantValue;
                    if ((object)constantValue2 == null || !constantValue2!.IsBoolean)
                    {
                        return false;
                    }
                    Unsplit();
                    UseRvalueOnly(leftOperand);
                    Visit(binary.Right);
                    UseRvalueOnly(binary.Right);
                    flag = binaryOperatorKind == BinaryOperatorKind.Equal == binary.Left.ConstantValue!.BooleanValue;
                }
                if (!flag && IsConditionalState)
                {
                    SetConditionalState(StateWhenFalse, StateWhenTrue);
                }
                SetResult(binary, TypeWithState.ForType(binary.Type), TypeWithAnnotations.Create(binary.Type));
                return true;
            }
        }

        private void AfterLeftChildHasBeenVisited(BoundExpression leftOperand, Conversion leftConversion, BoundBinaryOperator binary)
        {
            TypeWithState leftType = ResultType;

            var (rightOperand, rightConversion) = RemoveConversion(binary.Right, includeExplicitConversions: false);
            var rightType = VisitRvalueWithState(rightOperand);

            // At this point, State.Reachable may be false for
            // invalid code such as `s + throw new Exception()`.

            var method = binary.MethodOpt;

            if (binary.OperatorKind.IsUserDefined() &&
                method?.ParameterCount == 2)
            {
                // Update method based on inferred operand type.
                TypeSymbol methodContainer = method.ContainingType;
                bool isLifted = binary.OperatorKind.IsLifted();
                TypeWithState leftUnderlyingType = GetNullableUnderlyingTypeIfNecessary(isLifted, leftType);
                TypeWithState rightUnderlyingType = GetNullableUnderlyingTypeIfNecessary(isLifted, rightType);
                TypeSymbol? asMemberOfType = getTypeIfContainingType(methodContainer, leftUnderlyingType.Type) ??
                    getTypeIfContainingType(methodContainer, rightUnderlyingType.Type);
                if (asMemberOfType is object)
                {
                    method = (MethodSymbol)AsMemberOfType(asMemberOfType, method);
                }

                // Analyze operator call properly (honoring [Disallow|Allow|Maybe|NotNull] attribute annotations) https://github.com/dotnet/roslyn/issues/32671
                var parameters = method.Parameters;
                visitOperandConversion(binary.Left, leftOperand, leftConversion, parameters[0], leftUnderlyingType);
                visitOperandConversion(binary.Right, rightOperand, rightConversion, parameters[1], rightUnderlyingType);
                SetUpdatedSymbol(binary, binary.MethodOpt!, method);

                void visitOperandConversion(
                    BoundExpression expr,
                    BoundExpression operand,
                    Conversion conversion,
                    ParameterSymbol parameter,
                    TypeWithState operandType)
                {
                    TypeWithAnnotations targetTypeWithNullability = parameter.TypeWithAnnotations;

                    if (isLifted && targetTypeWithNullability.Type.IsNonNullableValueType())
                    {
                        targetTypeWithNullability = TypeWithAnnotations.Create(MakeNullableOf(targetTypeWithNullability));
                    }

                    _ = VisitConversion(
                        expr as BoundConversion,
                        operand,
                        conversion,
                        targetTypeWithNullability,
                        operandType,
                        checkConversion: true,
                        fromExplicitCast: false,
                        useLegacyWarnings: false,
                        AssignmentKind.Argument,
                        parameter);
                }
            }
            else
            {
                // Assume this is a built-in operator in which case the parameter types are unannotated.
                visitOperandConversion(binary.Left, leftOperand, leftConversion, leftType);
                visitOperandConversion(binary.Right, rightOperand, rightConversion, rightType);

                void visitOperandConversion(
                    BoundExpression expr,
                    BoundExpression operand,
                    Conversion conversion,
                    TypeWithState operandType)
                {
                    if (expr.Type is object)
                    {
                        _ = VisitConversion(
                            expr as BoundConversion,
                            operand,
                            conversion,
                            TypeWithAnnotations.Create(expr.Type),
                            operandType,
                            checkConversion: true,
                            fromExplicitCast: false,
                            useLegacyWarnings: false,
                            AssignmentKind.Argument);
                    }
                }
            }

            // For nested binary operators, this can be the only time they're visited due to explicit stack used in AbstractFlowPass.VisitBinaryOperator,
            // so we need to set the flow-analyzed type here.
            var inferredResult = InferResultNullability(binary.OperatorKind, method, binary.Type, leftType, rightType);
            SetResult(binary, inferredResult, inferredResult.ToTypeWithAnnotations(compilation));

            BinaryOperatorKind op = binary.OperatorKind.Operator();

            if (op == BinaryOperatorKind.Equal || op == BinaryOperatorKind.NotEqual)
            {
                // learn from null constant
                BoundExpression? operandComparedToNull = null;
                if (binary.Right.ConstantValue?.IsNull == true)
                {
                    operandComparedToNull = binary.Left;
                }
                else if (binary.Left.ConstantValue?.IsNull == true)
                {
                    operandComparedToNull = binary.Right;
                }

                if (operandComparedToNull != null)
                {
                    // Set all nested conditional slots. For example in a?.b?.c we'll set a, b, and c.
                    bool nonNullCase = op != BinaryOperatorKind.Equal; // true represents WhenTrue
                    splitAndLearnFromNonNullTest(operandComparedToNull, whenTrue: nonNullCase);

                    // `x == null` and `x != null` are pure null tests so update the null-state in the alternative branch too
                    LearnFromNullTest(operandComparedToNull, ref nonNullCase ? ref StateWhenFalse : ref StateWhenTrue);
                    return;
                }
            }

            // learn from comparison between non-null and maybe-null, possibly updating maybe-null to non-null
            BoundExpression? operandComparedToNonNull = null;
            if (leftType.IsNotNull && rightType.MayBeNull)
            {
                operandComparedToNonNull = binary.Right;
            }
            else if (rightType.IsNotNull && leftType.MayBeNull)
            {
                operandComparedToNonNull = binary.Left;
            }

            if (operandComparedToNonNull != null)
            {
                switch (op)
                {
                    case BinaryOperatorKind.Equal:
                    case BinaryOperatorKind.GreaterThan:
                    case BinaryOperatorKind.LessThan:
                    case BinaryOperatorKind.GreaterThanOrEqual:
                    case BinaryOperatorKind.LessThanOrEqual:
                        operandComparedToNonNull = SkipReferenceConversions(operandComparedToNonNull);
                        splitAndLearnFromNonNullTest(operandComparedToNonNull, whenTrue: true);
                        return;
                    case BinaryOperatorKind.NotEqual:
                        operandComparedToNonNull = SkipReferenceConversions(operandComparedToNonNull);
                        splitAndLearnFromNonNullTest(operandComparedToNonNull, whenTrue: false);
                        return;
                };
            }

            void splitAndLearnFromNonNullTest(BoundExpression operandComparedToNonNull, bool whenTrue)
            {
                var slotBuilder = ArrayBuilder<int>.GetInstance();
                GetSlotsToMarkAsNotNullable(operandComparedToNonNull, slotBuilder);
                if (slotBuilder.Count != 0)
                {
                    Split();
                    ref LocalState stateToUpdate = ref whenTrue ? ref this.StateWhenTrue : ref this.StateWhenFalse;
                    MarkSlotsAsNotNull(slotBuilder, ref stateToUpdate);
                }
                slotBuilder.Free();
            }

            TypeSymbol? getTypeIfContainingType(TypeSymbol baseType, TypeSymbol? derivedType)
            {
                if (derivedType is null)
                {
                    return null;
                }
                derivedType = derivedType.StrippedType();
                var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                var conversion = _conversions.ClassifyBuiltInConversion(derivedType, baseType, ref discardedUseSiteInfo);
                if (conversion.Exists && !conversion.IsExplicit)
                {
                    return derivedType;
                }
                return null;
            }
        }

        private void GetSlotsToMarkAsNotNullable(BoundExpression operand, ArrayBuilder<int> slotBuilder)
        {
            int lastConditionalAccessSlot = _lastConditionalAccessSlot;
            try
            {
                while (true)
                {
                    switch (operand.Kind)
                    {
                        case BoundKind.Conversion:
                            operand = ((BoundConversion)operand).Operand;
                            break;
                        case BoundKind.AsOperator:
                            operand = ((BoundAsOperator)operand).Operand;
                            break;
                        case BoundKind.ConditionalAccess:
                            {
                                BoundConditionalAccess boundConditionalAccess = (BoundConditionalAccess)operand;
                                GetSlotsToMarkAsNotNullable(boundConditionalAccess.Receiver, slotBuilder);
                                int num = MakeSlot(boundConditionalAccess.Receiver);
                                if (num > 0)
                                {
                                    TypeSymbol type = boundConditionalAccess.Receiver.Type;
                                    if (type.IsNullableType())
                                    {
                                        num = GetNullableOfTValueSlot(type, num, out var _);
                                    }
                                }
                                if (num > 0)
                                {
                                    _lastConditionalAccessSlot = num;
                                    operand = boundConditionalAccess.AccessExpression;
                                    break;
                                }
                                return;
                            }
                        default:
                            {
                                int num = MakeSlot(operand);
                                if (num > 0 && PossiblyNullableType(operand.Type))
                                {
                                    slotBuilder.Add(num);
                                }
                                return;
                            }
                    }
                }
            }
            finally
            {
                _lastConditionalAccessSlot = lastConditionalAccessSlot;
            }
        }

        private static bool PossiblyNullableType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] TypeSymbol? operandType)
        {
            return operandType?.CanContainNull() ?? false;
        }

        private static void MarkSlotsAsNotNull(ArrayBuilder<int> slots, ref LocalState stateToUpdate)
        {
            ArrayBuilder<int>.Enumerator enumerator = slots.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                stateToUpdate[current] = NullableFlowState.NotNull;
            }
        }

        private void LearnFromNonNullTest(BoundExpression expression, ref LocalState state)
        {
            if (expression.Kind == BoundKind.AwaitableValuePlaceholder)
            {
                if (_awaitablePlaceholdersOpt == null || !_awaitablePlaceholdersOpt!.TryGetValue((BoundAwaitableValuePlaceholder)expression, out var value))
                {
                    return;
                }
                (expression, _) = value;
            }
            ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance();
            GetSlotsToMarkAsNotNullable(expression, instance);
            MarkSlotsAsNotNull(instance, ref state);
            instance.Free();
        }

        private void LearnFromNonNullTest(int slot, ref LocalState state)
        {
            state[slot] = NullableFlowState.NotNull;
        }

        private void LearnFromNullTest(BoundExpression expression, ref LocalState state)
        {
            if (!(expression.ConstantValue != null))
            {
                BoundExpression item = RemoveConversion(expression, includeExplicitConversions: true).expression;
                int slot = MakeSlot(item);
                LearnFromNullTest(slot, item.Type, ref state, markDependentSlotsNotNull: false);
            }
        }

        private void LearnFromNullTest(int slot, TypeSymbol? expressionType, ref LocalState state, bool markDependentSlotsNotNull)
        {
            if (slot > 0 && PossiblyNullableType(expressionType))
            {
                if (state[slot] == NullableFlowState.NotNull)
                {
                    state[slot] = NullableFlowState.MaybeNull;
                }
                if (markDependentSlotsNotNull)
                {
                    MarkDependentSlotsNotNull(slot, expressionType, ref state);
                }
            }
        }

        private void MarkDependentSlotsNotNull(int slot, TypeSymbol expressionType, ref LocalState state, int depth = 2)
        {
            if (depth <= 0)
            {
                return;
            }
            foreach (Symbol member in getMembers(expressionType))
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                NamedTypeSymbol namedTypeSymbol = _symbol?.ContainingType;
                if (((member is PropertySymbol propertySymbol && !propertySymbol.IsIndexedProperty) || member.Kind == SymbolKind.Field) && member.RequiresInstanceReceiver() && ((object)namedTypeSymbol == null || AccessCheck.IsSymbolAccessible(member, namedTypeSymbol, ref useSiteInfo)))
                {
                    int orCreateSlot = GetOrCreateSlot(member, slot, forceSlotEvenIfEmpty: true, createIfMissing: false);
                    if (orCreateSlot > 0)
                    {
                        state[orCreateSlot] = NullableFlowState.NotNull;
                        MarkDependentSlotsNotNull(orCreateSlot, member.GetTypeOrReturnType().Type, ref state, depth - 1);
                    }
                }
            }
            static NamedTypeSymbol effectiveBase(TypeSymbol type)
            {
                if (type is TypeParameterSymbol typeParameterSymbol2)
                {
                    return typeParameterSymbol2.EffectiveBaseClassNoUseSiteDiagnostics;
                }
                return type.BaseTypeNoUseSiteDiagnostics;
            }
            static IEnumerable<Symbol> getMembers(TypeSymbol type)
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = type.GetMembers().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    yield return enumerator2.Current;
                }
                NamedTypeSymbol baseType = effectiveBase(type);
                while ((object)baseType != null)
                {
                    enumerator2 = baseType.GetMembers().GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        yield return enumerator2.Current;
                    }
                    baseType = baseType.BaseTypeNoUseSiteDiagnostics;
                }
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator3 = inheritedInterfaces(type).GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    NamedTypeSymbol current2 = enumerator3.Current;
                    enumerator2 = current2.GetMembers().GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        yield return enumerator2.Current;
                    }
                }
            }
            static ImmutableArray<NamedTypeSymbol> inheritedInterfaces(TypeSymbol type)
            {
                if (type is TypeParameterSymbol typeParameterSymbol)
                {
                    return typeParameterSymbol.AllEffectiveInterfacesNoUseSiteDiagnostics;
                }
                if ((object)type != null && type.TypeKind == TypeKind.Interface)
                {
                    return type.AllInterfacesNoUseSiteDiagnostics;
                }
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }
        }

        private static BoundExpression SkipReferenceConversions(BoundExpression possiblyConversion)
        {
            while (possiblyConversion.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)possiblyConversion;
                ConversionKind conversionKind = boundConversion.ConversionKind;
                if (conversionKind == ConversionKind.ImplicitReference || conversionKind == ConversionKind.ExplicitReference)
                {
                    possiblyConversion = boundConversion.Operand;
                    continue;
                }
                return possiblyConversion;
            }
            return possiblyConversion;
        }

        public override BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            BoundExpression leftOperand = node.LeftOperand;
            BoundExpression rightOperand = node.RightOperand;
            int targetSlot = MakeSlot(leftOperand);
            TypeWithAnnotations typeWithAnnotations = VisitLvalueWithAnnotations(leftOperand);
            LocalState state = State.Clone();
            LearnFromNonNullTest(leftOperand, ref state);
            LearnFromNullTest(leftOperand, ref State);
            if (node.IsNullableValueTypeAssignment)
            {
                typeWithAnnotations = TypeWithAnnotations.Create(node.Type, NullableAnnotation.NotAnnotated);
            }
            TypeWithState valueType = VisitOptionalImplicitConversion(rightOperand, typeWithAnnotations, UseLegacyWarnings(leftOperand, typeWithAnnotations), trackMembers: false, AssignmentKind.Assignment);
            TrackNullableStateForAssignment(rightOperand, typeWithAnnotations, targetSlot, valueType, MakeSlot(rightOperand));
            Join(ref State, ref state);
            TypeWithState type = TypeWithState.Create(typeWithAnnotations.Type, valueType.State);
            SetResultType(node, type);
            return null;
        }

        public override BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundNullCoalescingOperator node2 = node;
            BoundExpression leftOperand = node2.LeftOperand;
            BoundExpression rightOperand = node2.RightOperand;
            TypeWithState typeWithState = VisitRvalueWithState(leftOperand);
            TypeWithState typeWithState2;
            if (AbstractFlowPass<LocalState, LocalFunctionState>.IsConstantNull(leftOperand))
            {
                typeWithState2 = VisitRvalueWithState(rightOperand);
                SetResultType(node2, TypeWithState.Create(node2.Type, typeWithState2.State));
                return null;
            }
            LocalState state = State.Clone();
            LearnFromNonNullTest(leftOperand, ref state);
            LearnFromNullTest(leftOperand, ref State);
            if (leftOperand.ConstantValue != null)
            {
                SetUnreachable();
            }
            typeWithState2 = VisitRvalueWithState(rightOperand);
            Join(ref State, ref state);
            ConstantValue? constantValue = rightOperand.ConstantValue;
            if ((object)constantValue != null && constantValue!.IsBoolean)
            {
                Split();
                if (rightOperand.ConstantValue!.BooleanValue)
                {
                    StateWhenFalse = state;
                }
                else
                {
                    StateWhenTrue = state;
                }
            }
            TypeSymbol type = typeWithState.Type;
            TypeSymbol type2 = typeWithState2.Type;
            var (type3, b) = node2.OperatorResultKind switch
            {
                BoundNullCoalescingOperatorResultKind.NoCommonType => (node2.Type, NullableFlowState.NotNull),
                BoundNullCoalescingOperatorResultKind.LeftType => getLeftResultType(type, type2),
                BoundNullCoalescingOperatorResultKind.LeftUnwrappedType => getLeftResultType(type.StrippedType(), type2),
                BoundNullCoalescingOperatorResultKind.RightType => getResultStateWithRightType(type, type2),
                BoundNullCoalescingOperatorResultKind.LeftUnwrappedRightType => getResultStateWithRightType(type.StrippedType(), type2),
                BoundNullCoalescingOperatorResultKind.RightDynamicType => (type2, NullableFlowState.NotNull),
                _ => throw ExceptionUtilities.UnexpectedValue(node2.OperatorResultKind),
            };
            SetResultType(node2, TypeWithState.Create(type3, typeWithState2.State.Join(b)));
            return null;
            (TypeSymbol ResultType, NullableFlowState LeftState) getLeftResultType(TypeSymbol leftType, TypeSymbol rightType)
            {
                BoundConversion obj = node2.RightOperand as BoundConversion;
                if ((obj == null || obj.ExplicitCastInCode) && GenerateConversionForConditionalOperator(node2.LeftOperand, leftType, rightType, reportMismatch: false).Exists)
                {
                    return (rightType, NullableFlowState.NotNull);
                }
                Conversion conversion2 = GenerateConversionForConditionalOperator(node2.RightOperand, rightType, leftType, reportMismatch: true);
                return (leftType, NullableFlowState.NotNull);
            }
            (TypeSymbol ResultType, NullableFlowState LeftState) getResultStateWithRightType(TypeSymbol leftType, TypeSymbol rightType)
            {
                Conversion conversion = GenerateConversionForConditionalOperator(node2.LeftOperand, leftType, rightType, reportMismatch: true);
                if (conversion.IsUserDefined)
                {
                    TypeWithState typeWithState3 = VisitConversion(null, node2.LeftOperand, conversion, TypeWithAnnotations.Create(rightType), TypeWithState.Create(leftType, NullableFlowState.NotNull), checkConversion: false, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment, null, reportTopLevelWarnings: false, reportRemainingWarnings: false);
                    return (typeWithState3.Type, typeWithState3.State);
                }
                return (rightType, NullableFlowState.NotNull);
            }
        }

        public override BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            BoundExpression receiver = node.Receiver;
            VisitRvalueWithState(receiver);
            _currentConditionalReceiverVisitResult = _visitResult;
            int lastConditionalAccessSlot = _lastConditionalAccessSlot;
            LocalState other = State.Clone();
            if (AbstractFlowPass<LocalState, LocalFunctionState>.IsConstantNull(node.Receiver))
            {
                SetUnreachable();
                _lastConditionalAccessSlot = -1;
            }
            else
            {
                LearnFromNullTest(receiver, ref other);
                LearnFromNonNullTest(receiver, ref State);
                int num = MakeSlot(receiver);
                if (num > 0)
                {
                    TypeSymbol? type = receiver.Type;
                    if ((object)type != null && type.IsNullableType())
                    {
                        num = GetNullableOfTValueSlot(receiver.Type, num, out var _);
                    }
                }
                _lastConditionalAccessSlot = num;
            }
            TypeWithAnnotations underlying = VisitLvalueWithAnnotations(node.AccessExpression);
            TypeSymbol type2 = underlying.Type;
            Join(ref State, ref other);
            TypeSymbol type3 = node.Type;
            TypeSymbol type4 = ((type3.IsVoidType() || type3.IsErrorType()) ? type3 : ((type3.IsNullableType() && !type2.IsNullableType()) ? MakeNullableOf(underlying) : type2));
            SetResultType(node, TypeWithState.Create(type4, NullableFlowState.MaybeDefault));
            _currentConditionalReceiverVisitResult = default(VisitResult);
            _lastConditionalAccessSlot = lastConditionalAccessSlot;
            return null;
        }

        protected override BoundNode? VisitConditionalOperatorCore(BoundExpression node, bool isRef, BoundExpression condition, BoundExpression originalConsequence, BoundExpression originalAlternative)
        {
            VisitCondition(condition);
            LocalState other = StateWhenTrue;
            LocalState stateWhenFalse = StateWhenFalse;
            TypeWithState typeWithState;
            TypeWithState item2;
            if (isRef)
            {
                (TypeWithAnnotations LValueType, TypeWithState RValueType) tuple = visitConditionalRefOperand(other, originalConsequence);
                TypeWithAnnotations item = tuple.LValueType;
                item2 = tuple.RValueType;
                other = State;
                TypeWithAnnotations typeWithAnnotations;
                (typeWithAnnotations, typeWithState) = visitConditionalRefOperand(stateWhenFalse, originalAlternative);
                Join(ref State, ref other);
                TypeSymbol typeSymbol = node.Type?.SetUnknownNullabilityForReferenceTypes();
                if (IsNullabilityMismatch(item, typeWithAnnotations))
                {
                    ReportNullabilityMismatchInAssignment(node.Syntax, item, typeWithAnnotations);
                }
                else if (!node.HasErrors)
                {
                    typeSymbol = item2.Type!.MergeEquivalentTypes(typeWithState.Type, VarianceKind.None);
                }
                NullableAnnotation nullableAnnotation = item.NullableAnnotation.EnsureCompatible(typeWithAnnotations.NullableAnnotation);
                NullableFlowState defaultState = item2.State.Join(typeWithState.State);
                SetResult(node, TypeWithState.Create(typeSymbol, defaultState), TypeWithAnnotations.Create(typeSymbol, nullableAnnotation));
                return null;
            }
            BoundExpression boundExpression;
            Conversion conversion;
            BoundExpression boundExpression2;
            Conversion conversion2;
            bool flag;
            bool flag2;
            if (!stateWhenFalse.Reachable)
            {
                (BoundExpression, Conversion, TypeWithState) tuple3 = visitConditionalOperand(stateWhenFalse, originalAlternative);
                boundExpression = tuple3.Item1;
                conversion = tuple3.Item2;
                typeWithState = tuple3.Item3;
                (BoundExpression, Conversion, TypeWithState) tuple4 = visitConditionalOperand(other, originalConsequence);
                boundExpression2 = tuple4.Item1;
                conversion2 = tuple4.Item2;
                item2 = tuple4.Item3;
                flag = false;
                flag2 = IsReachable();
            }
            else if (!other.Reachable)
            {
                (BoundExpression, Conversion, TypeWithState) tuple5 = visitConditionalOperand(other, originalConsequence);
                boundExpression2 = tuple5.Item1;
                conversion2 = tuple5.Item2;
                item2 = tuple5.Item3;
                (BoundExpression, Conversion, TypeWithState) tuple6 = visitConditionalOperand(stateWhenFalse, originalAlternative);
                boundExpression = tuple6.Item1;
                conversion = tuple6.Item2;
                typeWithState = tuple6.Item3;
                flag2 = false;
                flag = IsReachable();
            }
            else
            {
                (boundExpression2, conversion2, item2) = visitConditionalOperand(other, originalConsequence);
                Unsplit();
                other = State;
                flag2 = other.Reachable;
                (boundExpression, conversion, typeWithState) = visitConditionalOperand(stateWhenFalse, originalAlternative);
                Unsplit();
                flag = State.Reachable;
                Join(ref State, ref other);
            }
            bool flag3 = node is BoundConditionalOperator boundConditionalOperator && boundConditionalOperator.WasTargetTyped;
            TypeSymbol typeSymbol2;
            if (node.HasErrors || flag3)
            {
                typeSymbol2 = null;
            }
            else
            {
                BoundExpression expr = CreatePlaceholderIfNecessary(boundExpression2, item2.ToTypeWithAnnotations(compilation));
                BoundExpression expr2 = CreatePlaceholderIfNecessary(boundExpression, typeWithState.ToTypeWithAnnotations(compilation));
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                typeSymbol2 = BestTypeInferrer.InferBestTypeForConditionalOperator(expr, expr2, _conversions, out var _, ref useSiteInfo);
            }
            if ((object)typeSymbol2 == null)
            {
                typeSymbol2 = node.Type?.SetUnknownNullabilityForReferenceTypes();
            }
            NullableFlowState defaultState2;
            if (!flag3)
            {
                if ((object)typeSymbol2 == null)
                {
                    defaultState2 = NullableFlowState.NotNull;
                }
                else
                {
                    TypeWithAnnotations targetType = TypeWithAnnotations.Create(typeSymbol2);
                    TypeWithState typeWithState2 = ConvertConditionalOperandOrSwitchExpressionArmResult(originalConsequence, boundExpression2, conversion2, targetType, item2, other, flag2);
                    defaultState2 = NullableFlowStateExtensions.Join(b: ConvertConditionalOperandOrSwitchExpressionArmResult(originalAlternative, boundExpression, conversion, targetType, typeWithState, stateWhenFalse, flag).State, a: typeWithState2.State);
                }
            }
            else
            {
                defaultState2 = item2.State.Join(typeWithState.State);
                ConditionalInfoForConversion.Add(node, ImmutableArray.Create((other, item2, flag2), (stateWhenFalse, typeWithState, flag)));
            }
            SetResultType(node, TypeWithState.Create(typeSymbol2, defaultState2));
            return null;
            (BoundExpression, Conversion, TypeWithState) visitConditionalOperand(LocalState state, BoundExpression operand)
            {
                SetState(state);
                var (boundExpression3, item3) = RemoveConversion(operand, includeExplicitConversions: false);
                SnapshotWalkerThroughConversionGroup(operand, boundExpression3);
                Visit(boundExpression3);
                return (boundExpression3, item3, ResultType);
            }
            (TypeWithAnnotations LValueType, TypeWithState RValueType) visitConditionalRefOperand(LocalState state, BoundExpression operand)
            {
                SetState(state);
                return (VisitLvalueWithAnnotations(operand), ResultType);
            }
        }

        private TypeWithState ConvertConditionalOperandOrSwitchExpressionArmResult(BoundExpression node, BoundExpression operand, Conversion conversion, TypeWithAnnotations targetType, TypeWithState operandType, LocalState state, bool isReachable)
        {
            LocalState state2 = State;
            State = state;
            bool disableDiagnostics = _disableDiagnostics;
            if (!isReachable)
            {
                _disableDiagnostics = true;
            }
            TypeWithState result = VisitConversion(GetConversionIfApplicable(node, operand), operand, conversion, targetType, operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment, null, reportTopLevelWarnings: false);
            if (!isReachable)
            {
                result = default(TypeWithState);
                _disableDiagnostics = disableDiagnostics;
            }
            State = state2;
            return result;
        }

        private bool IsReachable()
        {
            if (!IsConditionalState)
            {
                return State.Reachable;
            }
            if (!StateWhenTrue.Reachable)
            {
                return StateWhenFalse.Reachable;
            }
            return true;
        }

        private static BoundExpression CreatePlaceholderIfNecessary(BoundExpression expr, TypeWithAnnotations type)
        {
            if (type.HasType)
            {
                return new BoundExpressionWithNullability(expr.Syntax, expr, type.NullableAnnotation, type.Type);
            }
            return expr;
        }

        public override BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            TypeSymbol typeSymbol = _currentConditionalReceiverVisitResult.RValueType.Type;
            if ((object)typeSymbol != null && typeSymbol.IsNullableType())
            {
                typeSymbol = typeSymbol.GetNullableUnderlyingType();
            }
            SetResultType(node, TypeWithState.Create(typeSymbol, NullableFlowState.NotNull));
            return null;
        }

        public override BoundNode? VisitCall(BoundCall node)
        {
            TypeWithState receiverType = VisitCallReceiver(node);
            ReinferMethodAndVisitArguments(node, receiverType);
            return null;
        }

        private void ReinferMethodAndVisitArguments(BoundCall node, TypeWithState receiverType)
        {
            MethodSymbol methodSymbol = node.Method;
            ImmutableArray<RefKind> argumentRefKindsOpt = node.ArgumentRefKindsOpt;
            if (!receiverType.HasNullType)
            {
                methodSymbol = (MethodSymbol)AsMemberOfType(receiverType.Type, methodSymbol);
            }
            ImmutableArray<VisitArgumentResult> results;
            bool flag;
            (methodSymbol, results, flag) = VisitArguments(node, node.Arguments, argumentRefKindsOpt, methodSymbol.Parameters, node.ArgsToParamsOpt, node.DefaultArguments, node.Expanded, node.InvokedAsExtensionMethod, methodSymbol);
            ApplyMemberPostConditions(node.ReceiverOpt, methodSymbol);
            LearnFromEqualsMethod(methodSymbol, node, receiverType, results);
            TypeWithState resultType = GetReturnTypeWithState(methodSymbol);
            if (flag)
            {
                resultType = resultType.WithNotNullState();
            }
            SetResult(node, resultType, methodSymbol.ReturnTypeWithAnnotations);
            SetUpdatedSymbol(node, node.Method, methodSymbol);
        }

        private void LearnFromEqualsMethod(MethodSymbol method, BoundCall node, TypeWithState receiverType, ImmutableArray<VisitArgumentResult> results)
        {
            int parameterCount = method.ParameterCount;
            ImmutableArray<BoundExpression> arguments = node.Arguments;
            if (node.HasErrors || (parameterCount != 1 && parameterCount != 2) || parameterCount != arguments.Length || method.MethodKind != MethodKind.Ordinary || method.ReturnType.SpecialType != SpecialType.System_Boolean || (method.Name != SpecialMembers.GetDescriptor(SpecialMember.System_Object__Equals).Name && method.Name != SpecialMembers.GetDescriptor(SpecialMember.System_Object__ReferenceEquals).Name && !anyOverriddenMethodHasExplicitImplementation(method)))
            {
                return;
            }
            if (method.Equals(compilation.GetSpecialTypeMember(SpecialMember.System_Object__EqualsObjectObject)) || method.Equals(compilation.GetSpecialTypeMember(SpecialMember.System_Object__ReferenceEquals)) || isWellKnownEqualityMethodOrImplementation(compilation, method, receiverType.Type, WellKnownMember.System_Collections_Generic_IEqualityComparer_T__Equals))
            {
                learnFromEqualsMethodArguments(arguments[0], results[0].RValueType, arguments[1], results[1].RValueType);
                return;
            }
            bool flag = method.GetLeastOverriddenMethod(null).Equals(compilation.GetSpecialTypeMember(SpecialMember.System_Object__Equals));
            BoundExpression receiverOpt = node.ReceiverOpt;
            if (receiverOpt != null && (flag || isWellKnownEqualityMethodOrImplementation(compilation, method, receiverType.Type, WellKnownMember.System_IEquatable_T__Equals)))
            {
                learnFromEqualsMethodArguments(receiverOpt, receiverType, arguments[0], results[0].RValueType);
            }
            static bool anyOverriddenMethodHasExplicitImplementation(MethodSymbol method)
            {
                MethodSymbol methodSymbol4 = method;
                while ((object)methodSymbol4 != null)
                {
                    if (methodSymbol4.IsExplicitInterfaceImplementation)
                    {
                        return true;
                    }
                    methodSymbol4 = methodSymbol4.OverriddenMethod;
                }
                return false;
            }
            static bool isWellKnownEqualityMethodOrImplementation(CSharpCompilation compilation, MethodSymbol method, TypeSymbol? receiverType, WellKnownMember wellKnownMember)
            {
                MethodSymbol methodSymbol = (MethodSymbol)compilation.GetWellKnownTypeMember(wellKnownMember);
                if ((object)methodSymbol == null || (object)receiverType == null)
                {
                    return false;
                }
                NamedTypeSymbol containingType = methodSymbol.ContainingType;
                TypeWithAnnotations typeWithAnnotations = method.Parameters[0].TypeWithAnnotations;
                NamedTypeSymbol newOwner = containingType.Construct(ImmutableArray.Create(typeWithAnnotations));
                MethodSymbol methodSymbol2 = methodSymbol.AsMember(newOwner);
                if (methodSymbol2.Equals(method))
                {
                    return true;
                }
                TypeSymbol typeSymbol = receiverType;
                while ((object)typeSymbol != null && (object)method != null)
                {
                    Symbol symbol = typeSymbol.FindImplementationForInterfaceMember(methodSymbol2);
                    if ((object)symbol == null)
                    {
                        return false;
                    }
                    if (symbol.ContainingType.IsInterface)
                    {
                        return false;
                    }
                    MethodSymbol methodSymbol3 = method;
                    while ((object)methodSymbol3 != null)
                    {
                        if (methodSymbol3.Equals(symbol))
                        {
                            return true;
                        }
                        methodSymbol3 = methodSymbol3.OverriddenMethod;
                    }
                    while (!typeSymbol.Equals(symbol.ContainingType) && (object)method != null)
                    {
                        if (typeSymbol.Equals(method.ContainingType))
                        {
                            method = method.OverriddenMethod;
                        }
                        typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
                    }
                    if ((object)method != null && typeSymbol.Equals(method.ContainingType))
                    {
                        method = method.OverriddenMethod;
                    }
                    typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
                }
                return false;
            }
            void learnFromEqualsMethodArguments(BoundExpression left, TypeWithState leftType, BoundExpression right, TypeWithState rightType)
            {
                ConstantValue? constantValue = left.ConstantValue;
                if ((object)constantValue != null && constantValue!.IsNull)
                {
                    Split();
                    LearnFromNullTest(right, ref StateWhenTrue);
                    LearnFromNonNullTest(right, ref StateWhenFalse);
                }
                else
                {
                    ConstantValue? constantValue2 = right.ConstantValue;
                    if ((object)constantValue2 != null && constantValue2!.IsNull)
                    {
                        Split();
                        LearnFromNullTest(left, ref StateWhenTrue);
                        LearnFromNonNullTest(left, ref StateWhenFalse);
                    }
                    else if (leftType.MayBeNull && rightType.IsNotNull)
                    {
                        Split();
                        LearnFromNonNullTest(left, ref StateWhenTrue);
                    }
                    else if (rightType.MayBeNull && leftType.IsNotNull)
                    {
                        Split();
                        LearnFromNonNullTest(right, ref StateWhenTrue);
                    }
                }
            }
        }

        private bool IsCompareExchangeMethod(MethodSymbol? method)
        {
            if ((object)method == null)
            {
                return false;
            }
            if (!method!.Equals(compilation.GetWellKnownTypeMember(WellKnownMember.System_Threading_Interlocked__CompareExchange), SymbolEqualityComparer.ConsiderEverything.CompareKind))
            {
                return method!.OriginalDefinition.Equals(compilation.GetWellKnownTypeMember(WellKnownMember.System_Threading_Interlocked__CompareExchange_T), SymbolEqualityComparer.ConsiderEverything.CompareKind);
            }
            return true;
        }

        private NullableFlowState LearnFromCompareExchangeMethod(in CompareExchangeInfo compareExchangeInfo)
        {
            if (compareExchangeInfo.Arguments.Length != 3)
            {
                return NullableFlowState.NotNull;
            }
            ImmutableArray<int> argsToParamsOpt = compareExchangeInfo.ArgsToParamsOpt;
            int index;
            int index2;
            int index3;
            if (!argsToParamsOpt.IsDefault)
            {
                int num = argsToParamsOpt.IndexOf(2);
                int num2 = argsToParamsOpt.IndexOf(1);
                int num3 = argsToParamsOpt.IndexOf(0);
                index = num;
                index2 = num2;
                index3 = num3;
            }
            else
            {
                index = 2;
                index2 = 1;
                index3 = 0;
            }
            BoundExpression boundExpression = compareExchangeInfo.Arguments[index];
            NullableFlowState nullableFlowState = compareExchangeInfo.Results[index2].RValueType.State;
            ConstantValue? constantValue = boundExpression.ConstantValue;
            if ((object)constantValue == null || !constantValue!.IsNull)
            {
                NullableFlowState state = compareExchangeInfo.Results[index3].RValueType.State;
                nullableFlowState = nullableFlowState.Join(state);
            }
            return nullableFlowState;
        }

        private TypeWithState VisitCallReceiver(BoundCall node)
        {
            BoundExpression receiverOpt = node.ReceiverOpt;
            TypeWithState result = default(TypeWithState);
            if (receiverOpt != null)
            {
                result = VisitRvalueWithState(receiverOpt);
                bool checkNullableValueType = false;
                TypeSymbol type = result.Type;
                MethodSymbol method = node.Method;
                if (method.RequiresInstanceReceiver && (object)type != null && type.IsNullableType() && method.ContainingType.IsReferenceType)
                {
                    checkNullableValueType = true;
                }
                else if (method.OriginalDefinition == compilation.GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_Value))
                {
                    checkNullableValueType = true;
                }
                CheckPossibleNullReceiver(receiverOpt, checkNullableValueType);
            }
            return result;
        }

        private TypeWithState GetReturnTypeWithState(MethodSymbol method)
        {
            return TypeWithState.Create(method.ReturnTypeWithAnnotations, GetRValueAnnotations(method));
        }

        private FlowAnalysisAnnotations GetRValueAnnotations(Symbol? symbol)
        {
            if (IsAnalyzingAttribute)
            {
                return FlowAnalysisAnnotations.None;
            }
            return symbol.GetFlowAnalysisAnnotations() & (FlowAnalysisAnnotations.MaybeNull | FlowAnalysisAnnotations.NotNull);
        }

        private FlowAnalysisAnnotations GetParameterAnnotations(ParameterSymbol parameter)
        {
            if (!IsAnalyzingAttribute)
            {
                return parameter.FlowAnalysisAnnotations;
            }
            return FlowAnalysisAnnotations.None;
        }

        private static TypeWithAnnotations ApplyLValueAnnotations(TypeWithAnnotations declaredType, FlowAnalysisAnnotations flowAnalysisAnnotations)
        {
            if ((flowAnalysisAnnotations & FlowAnalysisAnnotations.DisallowNull) == FlowAnalysisAnnotations.DisallowNull)
            {
                return declaredType.AsNotAnnotated();
            }
            if ((flowAnalysisAnnotations & FlowAnalysisAnnotations.AllowNull) == FlowAnalysisAnnotations.AllowNull)
            {
                return declaredType.AsAnnotated();
            }
            return declaredType;
        }

        private static TypeWithState ApplyUnconditionalAnnotations(TypeWithState typeWithState, FlowAnalysisAnnotations annotations)
        {
            if ((annotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull)
            {
                return TypeWithState.Create(typeWithState.Type, NullableFlowState.NotNull);
            }
            if ((annotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNull)
            {
                return TypeWithState.Create(typeWithState.Type, NullableFlowState.MaybeDefault);
            }
            return typeWithState;
        }

        private static TypeWithAnnotations ApplyUnconditionalAnnotations(TypeWithAnnotations declaredType, FlowAnalysisAnnotations annotations)
        {
            if ((annotations & FlowAnalysisAnnotations.MaybeNull) == FlowAnalysisAnnotations.MaybeNull)
            {
                return declaredType.AsAnnotated();
            }
            if ((annotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull)
            {
                return declaredType.AsNotAnnotated();
            }
            return declaredType;
        }

        private static bool HasImplicitTypeArguments(BoundNode node)
        {
            if (node is BoundCollectionElementInitializer boundCollectionElementInitializer)
            {
                MethodSymbol addMethod = boundCollectionElementInitializer.AddMethod;
                if ((object)addMethod != null && !addMethod.TypeArgumentsWithAnnotations.IsEmpty)
                {
                    return true;
                }
            }
            if (node is BoundForEachStatement boundForEachStatement)
            {
                ForEachEnumeratorInfo enumeratorInfoOpt = boundForEachStatement.EnumeratorInfoOpt;
                if (enumeratorInfoOpt != null)
                {
                    MethodArgumentInfo getEnumeratorInfo = enumeratorInfoOpt.GetEnumeratorInfo;
                    if ((object)getEnumeratorInfo != null)
                    {
                        MethodSymbol addMethod = getEnumeratorInfo.Method;
                        if ((object)addMethod != null && !addMethod.TypeArgumentsWithAnnotations.IsEmpty)
                        {
                            return true;
                        }
                    }
                }
            }
            SyntaxNode syntax = node.Syntax;
            if (syntax.Kind() != SyntaxKind.InvocationExpression)
            {
                return false;
            }
            return HasImplicitTypeArguments(((InvocationExpressionSyntax)syntax).Expression);
        }

        private static bool HasImplicitTypeArguments(SyntaxNode syntax)
        {
            NameSyntax nameSyntax = Binder.GetNameSyntax(syntax, out var _);
            if (nameSyntax == null)
            {
                return false;
            }
            nameSyntax = nameSyntax.GetUnqualifiedName();
            return nameSyntax.Kind() != SyntaxKind.GenericName;
        }

        protected override void VisitArguments(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, MethodSymbol method)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private (MethodSymbol? method, ImmutableArray<VisitArgumentResult> results, bool returnNotNull) VisitArguments(BoundExpression node, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, MethodSymbol? method, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool expanded, bool invokedAsExtensionMethod)
        {
            return VisitArguments(node, arguments, refKindsOpt, method?.Parameters ?? default(ImmutableArray<ParameterSymbol>), argsToParamsOpt, defaultArguments, expanded, invokedAsExtensionMethod, method);
        }

        private ImmutableArray<VisitArgumentResult> VisitArguments(BoundExpression node, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, PropertySymbol? property, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool expanded)
        {
            return VisitArguments(node, arguments, refKindsOpt, property?.Parameters ?? default(ImmutableArray<ParameterSymbol>), argsToParamsOpt, defaultArguments, expanded, invokedAsExtensionMethod: false).results;
        }

        private (MethodSymbol? method, ImmutableArray<VisitArgumentResult> results, bool returnNotNull) VisitArguments(BoundNode node, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, ImmutableArray<ParameterSymbol> parametersOpt, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool expanded, bool invokedAsExtensionMethod, MethodSymbol? method = null)
        {
            bool item = false;
            (ImmutableArray<BoundExpression> arguments, ImmutableArray<Conversion> conversions) tuple = RemoveArgumentConversions(arguments, refKindsOpt);
            ImmutableArray<BoundExpression> item2 = tuple.arguments;
            ImmutableArray<Conversion> item3 = tuple.conversions;
            ImmutableArray<VisitArgumentResult> immutableArray = VisitArgumentsEvaluate(node.Syntax, item2, refKindsOpt, parametersOpt, argsToParamsOpt, defaultArguments, expanded);
            if ((object)method != null && method!.IsGenericMethod)
            {
                if (HasImplicitTypeArguments(node))
                {
                    method = InferMethodTypeArguments(method, GetArgumentsForMethodTypeInference(immutableArray, item2), refKindsOpt, argsToParamsOpt, expanded);
                    parametersOpt = method!.Parameters;
                }
                if (ConstraintsHelper.RequiresChecking(method))
                {
                    SyntaxNode syntax = node.Syntax;
                    SyntaxNode syntax2;
                    if (syntax is InvocationExpressionSyntax invocationExpressionSyntax)
                    {
                        ExpressionSyntax expression = invocationExpressionSyntax.Expression;
                        syntax2 = expression;
                    }
                    else if (syntax is ForEachStatementSyntax forEachStatementSyntax)
                    {
                        ExpressionSyntax expression2 = forEachStatementSyntax.Expression;
                        syntax2 = expression2;
                    }
                    else
                    {
                        syntax2 = syntax;
                    }
                    CheckMethodConstraints(syntax2, method);
                }
            }
            ArrayBuilder<ParameterSymbol> arrayBuilder = ((!IsAnalyzingAttribute && !parametersOpt.IsDefault && parametersOpt.Any((ParameterSymbol p) => !p.NotNullIfParameterNotNull.IsEmpty)) ? ArrayBuilder<ParameterSymbol>.GetInstance() : null);
            if (!node.HasErrors && !parametersOpt.IsDefault)
            {
                ImmutableHashSet<string> immutableHashSet = (IsAnalyzingAttribute ? null : method?.ReturnNotNullIfParameterNotNull);
                for (int i = 0; i < immutableArray.Length; i++)
                {
                    var (parameterSymbol, parameterType, parameterAnnotations, flag) = GetCorrespondingParameter(i, parametersOpt, argsToParamsOpt, expanded);
                    if ((object)parameterSymbol == null)
                    {
                        continue;
                    }
                    BoundExpression boundExpression = item2[i];
                    BoundExpression conversionOpt = ((i < arguments.Length) ? arguments[i] : item2[i]);
                    bool disableDiagnostics = _disableDiagnostics;
                    _disableDiagnostics |= defaultArguments[i];
                    VisitArgumentConversionAndInboundAssignmentsAndPreConditions(GetConversionIfApplicable(conversionOpt, boundExpression), boundExpression, (item3.IsDefault || i >= item3.Length) ? Conversion.Identity : item3[i], AbstractFlowPass<LocalState, LocalFunctionState>.GetRefKind(refKindsOpt, i), parameterSymbol, parameterType, parameterAnnotations, immutableArray[i], invokedAsExtensionMethod && i == 0);
                    _disableDiagnostics = disableDiagnostics;
                    if (immutableArray[i].RValueType.IsNotNull || flag)
                    {
                        arrayBuilder?.Add(parameterSymbol);
                        if (immutableHashSet != null && immutableHashSet.Contains(parameterSymbol.Name))
                        {
                            item = true;
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < immutableArray.Length; j++)
                {
                    if (item2[j] is BoundLambda node2)
                    {
                        VisitLambda(node2, null, immutableArray[j].StateForLambda);
                    }
                }
            }
            if (node is BoundCall boundCall)
            {
                MethodSymbol method2 = boundCall.Method;
                if ((object)method2 != null && method2.OriginalDefinition is LocalFunctionSymbol symbol)
                {
                    VisitLocalFunctionUse(symbol);
                }
            }
            if (!node.HasErrors && !parametersOpt.IsDefault)
            {
                CompareExchangeInfo compareExchangeInfo = (IsCompareExchangeMethod(method) ? new CompareExchangeInfo(arguments, immutableArray, argsToParamsOpt) : default(CompareExchangeInfo));
                for (int k = 0; k < arguments.Length; k++)
                {
                    var (parameterSymbol2, parameterType2, parameterAnnotations2, _) = GetCorrespondingParameter(k, parametersOpt, argsToParamsOpt, expanded);
                    if ((object)parameterSymbol2 != null)
                    {
                        VisitArgumentOutboundAssignmentsAndPostConditions(arguments[k], AbstractFlowPass<LocalState, LocalFunctionState>.GetRefKind(refKindsOpt, k), parameterSymbol2, parameterType2, parameterAnnotations2, immutableArray[k], arrayBuilder, (!compareExchangeInfo.IsDefault && parameterSymbol2.Ordinal == 0) ? compareExchangeInfo : default(CompareExchangeInfo));
                    }
                }
            }
            else
            {
                for (int l = 0; l < arguments.Length; l++)
                {
                    BoundExpression boundExpression2 = arguments[l];
                    VisitArgumentResult visitArgumentResult = immutableArray[l];
                    BoundExpression convertedNode = item2[l];
                    TrackAnalyzedNullabilityThroughConversionGroup(TypeWithState.Create(boundExpression2.Type, visitArgumentResult.RValueType.State), boundExpression2 as BoundConversion, convertedNode);
                }
            }
            if (!IsAnalyzingAttribute && (object)method != null && (method!.FlowAnalysisAnnotations & FlowAnalysisAnnotations.DoesNotReturn) == FlowAnalysisAnnotations.DoesNotReturn)
            {
                SetUnreachable();
            }
            arrayBuilder?.Free();
            return (method, immutableArray, item);
        }

        private void ApplyMemberPostConditions(BoundExpression? receiverOpt, MethodSymbol? method)
        {
            if ((object)method != null)
            {
                int num = ((!method!.IsStatic) ? ((receiverOpt == null) ? (-1) : MakeSlot(receiverOpt)) : 0);
                if (num >= 0)
                {
                    ApplyMemberPostConditions(num, method);
                }
            }
        }

        private void ApplyMemberPostConditions(int receiverSlot, MethodSymbol method)
        {
            do
            {
                NamedTypeSymbol containingType = method.ContainingType;
                ImmutableArray<string> notNullMembers = method.NotNullMembers;
                ImmutableArray<string> notNullWhenTrueMembers = method.NotNullWhenTrueMembers;
                ImmutableArray<string> notNullWhenFalseMembers = method.NotNullWhenFalseMembers;
                if (IsConditionalState)
                {
                    applyMemberPostConditions(receiverSlot, containingType, notNullMembers, ref StateWhenTrue);
                    applyMemberPostConditions(receiverSlot, containingType, notNullMembers, ref StateWhenFalse);
                }
                else
                {
                    applyMemberPostConditions(receiverSlot, containingType, notNullMembers, ref State);
                }
                if (!notNullWhenTrueMembers.IsEmpty || !notNullWhenFalseMembers.IsEmpty)
                {
                    Split();
                    applyMemberPostConditions(receiverSlot, containingType, notNullWhenTrueMembers, ref StateWhenTrue);
                    applyMemberPostConditions(receiverSlot, containingType, notNullWhenFalseMembers, ref StateWhenFalse);
                }
                method = method.OverriddenMethod;
            }
            while (method != null);
            void applyMemberPostConditions(int receiverSlot, TypeSymbol type, ImmutableArray<string> members, ref LocalState state)
            {
                if (!members.IsEmpty)
                {
                    ImmutableArray<string>.Enumerator enumerator2 = members.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        string current2 = enumerator2.Current;
                        markMembersAsNotNull(receiverSlot, type, current2, ref state);
                    }
                }
            }
            void markMembersAsNotNull(int receiverSlot, TypeSymbol type, string memberName, ref LocalState state)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembers(memberName).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.IsStatic)
                    {
                        receiverSlot = 0;
                    }
                    switch (current.Kind)
                    {
                        case SymbolKind.Field:
                        case SymbolKind.Property:
                            {
                                int orCreateSlot = GetOrCreateSlot(current, receiverSlot);
                                if (orCreateSlot > 0)
                                {
                                    state[orCreateSlot] = NullableFlowState.NotNull;
                                }
                                break;
                            }
                    }
                }
            }
        }

        private ImmutableArray<VisitArgumentResult> VisitArgumentsEvaluate(SyntaxNode syntax, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt, ImmutableArray<ParameterSymbol> parametersOpt, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, bool expanded)
        {
            int length = arguments.Length;
            if (length == 0 && parametersOpt.IsDefaultOrEmpty)
            {
                return ImmutableArray<VisitArgumentResult>.Empty;
            }
            PooledHashSet<ParameterSymbol> instance = PooledHashSet<ParameterSymbol>.GetInstance();
            ArrayBuilder<VisitArgumentResult> instance2 = ArrayBuilder<VisitArgumentResult>.GetInstance(length);
            bool disableDiagnostics = _disableDiagnostics;
            for (int i = 0; i < length; i++)
            {
                (ParameterSymbol? Parameter, TypeWithAnnotations Type, FlowAnalysisAnnotations Annotations, bool isExpandedParamsArgument) correspondingParameter = GetCorrespondingParameter(i, parametersOpt, argsToParamsOpt, expanded);
                ParameterSymbol item = correspondingParameter.Parameter;
                FlowAnalysisAnnotations item2 = correspondingParameter.Annotations;
                _disableDiagnostics = defaultArguments[i] || disableDiagnostics;
                instance2.Add(VisitArgumentEvaluate(arguments[i], AbstractFlowPass<LocalState, LocalFunctionState>.GetRefKind(refKindsOpt, i), item2));
                instance.Add(item);
            }
            _disableDiagnostics = disableDiagnostics;
            SetInvalidResult();
            instance.Free();
            return instance2.ToImmutableAndFree();
        }

        private VisitArgumentResult VisitArgumentEvaluate(BoundExpression argument, RefKind refKind, FlowAnalysisAnnotations annotations)
        {
            Optional<LocalState> stateForLambda = ((argument.Kind == BoundKind.Lambda) ? State.Clone() : default(Optional<LocalState>));
            switch (refKind)
            {
                case RefKind.Ref:
                    Visit(argument);
                    Unsplit();
                    break;
                case RefKind.None:
                case RefKind.In:
                    switch (annotations & FlowAnalysisAnnotations.DoesNotReturn)
                    {
                        case FlowAnalysisAnnotations.DoesNotReturnIfTrue:
                            Visit(argument);
                            if (IsConditionalState)
                            {
                                SetState(StateWhenFalse);
                            }
                            break;
                        case FlowAnalysisAnnotations.DoesNotReturnIfFalse:
                            Visit(argument);
                            if (IsConditionalState)
                            {
                                SetState(StateWhenTrue);
                            }
                            break;
                        default:
                            VisitRvalue(argument);
                            break;
                    }
                    break;
                case RefKind.Out:
                    Visit(argument);
                    UseLvalueOnly(argument);
                    break;
            }
            return new VisitArgumentResult(_visitResult, stateForLambda);
        }

        private void VisitArgumentConversionAndInboundAssignmentsAndPreConditions(BoundConversion? conversionOpt, BoundExpression argumentNoConversion, Conversion conversion, RefKind refKind, ParameterSymbol parameter, TypeWithAnnotations parameterType, FlowAnalysisAnnotations parameterAnnotations, VisitArgumentResult result, bool extensionMethodThisArgument)
        {
            TypeWithState rValueType = result.RValueType;
            switch (refKind)
            {
                case RefKind.None:
                case RefKind.In:
                    {
                        if (conversion.Kind == ConversionKind.ImplicitUserDefined)
                        {
                            TypeSymbol type = rValueType.Type;
                            conversion = GenerateConversion(_conversions, argumentNoConversion, type, parameterType.Type, fromExplicitCast: false, extensionMethodThisArgument: false);
                            if (!conversion.Exists && !argumentNoConversion.IsSuppressed)
                            {
                                ReportNullabilityMismatchInArgument(argumentNoConversion.Syntax, type, parameter, parameterType.Type, forOutput: false);
                            }
                        }
                        TypeWithState typeWithState = VisitConversion(conversionOpt, argumentNoConversion, conversion, ApplyLValueAnnotations(parameterType, parameterAnnotations), rValueType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Argument, parameter, reportTopLevelWarnings: true, reportRemainingWarnings: true, extensionMethodThisArgument, result.StateForLambda);
                        if (CheckDisallowedNullAssignment(typeWithState, parameterAnnotations, argumentNoConversion.Syntax.Location))
                        {
                            LearnFromNonNullTest(argumentNoConversion, ref State);
                        }
                        SetResultType(argumentNoConversion, typeWithState, updateAnalyzedNullability: false);
                        break;
                    }
                case RefKind.Ref:
                    if (!argumentNoConversion.IsSuppressed)
                    {
                        TypeWithAnnotations lValueType = result.LValueType;
                        if (IsNullabilityMismatch(lValueType.Type, parameterType.Type))
                        {
                            ReportNullabilityMismatchInRefArgument(argumentNoConversion, lValueType.Type, parameter, parameterType.Type);
                            break;
                        }
                        ReportNullableAssignmentIfNecessary(argumentNoConversion, ApplyLValueAnnotations(parameterType, parameterAnnotations), rValueType, useLegacyWarnings: false);
                        CheckDisallowedNullAssignment(rValueType, parameterAnnotations, argumentNoConversion.Syntax.Location);
                    }
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(refKind);
                case RefKind.Out:
                    break;
            }
        }

        private bool CheckDisallowedNullAssignment(TypeWithState state, FlowAnalysisAnnotations annotations, Location location, BoundExpression? boundValueOpt = null)
        {
            if (boundValueOpt != null && boundValueOpt!.WasCompilerGenerated)
            {
                return false;
            }
            if (IsDisallowedNullAssignment(state, annotations))
            {
                ReportDiagnostic(ErrorCode.WRN_DisallowNullAttributeForbidsMaybeNullAssignment, location);
                return true;
            }
            return false;
        }

        private static bool IsDisallowedNullAssignment(TypeWithState valueState, FlowAnalysisAnnotations targetAnnotations)
        {
            if ((targetAnnotations & FlowAnalysisAnnotations.DisallowNull) != 0 && hasNoNonNullableCounterpart(valueState.Type))
            {
                return valueState.MayBeNull;
            }
            return false;
            static bool hasNoNonNullableCounterpart(TypeSymbol? type)
            {
                if ((object)type == null)
                {
                    return false;
                }
                if (type!.Kind != SymbolKind.TypeParameter || type!.IsReferenceType)
                {
                    return type.IsNullableTypeOrTypeParameter();
                }
                return true;
            }
        }

        private void VisitArgumentOutboundAssignmentsAndPostConditions(BoundExpression argument, RefKind refKind, ParameterSymbol parameter, TypeWithAnnotations parameterType, FlowAnalysisAnnotations parameterAnnotations, VisitArgumentResult result, ArrayBuilder<ParameterSymbol>? notNullParametersOpt, CompareExchangeInfo compareExchangeInfoOpt)
        {
            switch (refKind)
            {
                case RefKind.None:
                case RefKind.In:
                    learnFromPostConditions(argument, parameterType, parameterAnnotations);
                    break;
                case RefKind.Ref:
                    {
                        parameterAnnotations = notNullBasedOnParameters(parameterAnnotations, notNullParametersOpt, parameter);
                        TypeWithState typeWithState2 = TypeWithState.Create(parameterType, parameterAnnotations);
                        if (!compareExchangeInfoOpt.IsDefault)
                        {
                            NullableFlowState defaultState = LearnFromCompareExchangeMethod(in compareExchangeInfoOpt);
                            typeWithState2 = TypeWithState.Create(parameterType.Type, defaultState);
                        }
                        BoundParameter boundParameter2 = new BoundParameter(argument.Syntax, parameter);
                        TypeWithAnnotations lValueType3 = result.LValueType;
                        trackNullableStateForAssignment(boundParameter2, lValueType3, MakeSlot(argument), typeWithState2, argument.IsSuppressed, parameterAnnotations);
                        if (!argument.IsSuppressed)
                        {
                            FlowAnalysisAnnotations lValueAnnotations2 = GetLValueAnnotations(argument);
                            ReportNullableAssignmentIfNecessary(boundParameter2, ApplyLValueAnnotations(lValueType3, lValueAnnotations2), applyPostConditionsUnconditionally(typeWithState2, parameterAnnotations), UseLegacyWarnings(argument, result.LValueType));
                        }
                        break;
                    }
                case RefKind.Out:
                    {
                        parameterAnnotations = notNullBasedOnParameters(parameterAnnotations, notNullParametersOpt, parameter);
                        TypeWithState rightState = TypeWithState.Create(parameterType, parameterAnnotations);
                        TypeWithState valueType = applyPostConditionsUnconditionally(rightState, parameterAnnotations);
                        TypeWithAnnotations lValueType2 = result.LValueType;
                        FlowAnalysisAnnotations lValueAnnotations = GetLValueAnnotations(argument);
                        TypeWithAnnotations typeWithAnnotations = ApplyLValueAnnotations(lValueType2, lValueAnnotations);
                        if (argument is BoundLocal boundLocal && boundLocal.DeclarationKind == BoundLocalDeclarationKind.WithInferredType)
                        {
                            TypeWithAnnotations typeWithAnnotations2 = valueType.ToAnnotatedTypeWithAnnotations(compilation);
                            _variables.SetType(boundLocal.LocalSymbol, typeWithAnnotations2);
                            typeWithAnnotations = typeWithAnnotations2;
                        }
                        else if (argument is BoundDiscardExpression expr)
                        {
                            SetAnalyzedNullability(expr, new VisitResult(rightState, rightState.ToTypeWithAnnotations(compilation)), true);
                        }
                        BoundParameter boundParameter = new BoundParameter(argument.Syntax, parameter);
                        CheckDisallowedNullAssignment(rightState, lValueAnnotations, argument.Syntax.Location);
                        AdjustSetValue(argument, ref rightState);
                        trackNullableStateForAssignment(boundParameter, typeWithAnnotations, MakeSlot(argument), rightState, argument.IsSuppressed, parameterAnnotations);
                        if (!argument.IsSuppressed)
                        {
                            ReportNullableAssignmentIfNecessary(boundParameter, typeWithAnnotations, valueType, UseLegacyWarnings(argument, result.LValueType));
                            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                            if (!_conversions.HasIdentityOrImplicitReferenceConversion(parameterType.Type, typeWithAnnotations.Type, ref useSiteInfo))
                            {
                                ReportNullabilityMismatchInArgument(argument.Syntax, typeWithAnnotations.Type, parameter, parameterType.Type, forOutput: true);
                            }
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(refKind);
            }
            static TypeWithState applyPostConditionsUnconditionally(TypeWithState typeWithState, FlowAnalysisAnnotations annotations)
            {
                if ((annotations & FlowAnalysisAnnotations.MaybeNull) != 0)
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.MaybeDefault);
                }
                if ((annotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull)
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.NotNull);
                }
                return typeWithState;
            }
            static TypeWithState applyPostConditionsWhenFalse(TypeWithState typeWithState, FlowAnalysisAnnotations annotations)
            {
                bool flag5 = (annotations & FlowAnalysisAnnotations.NotNullWhenFalse) != 0;
                bool flag6 = (annotations & FlowAnalysisAnnotations.MaybeNullWhenTrue) != 0;
                if ((annotations & FlowAnalysisAnnotations.MaybeNullWhenFalse) != 0 && !(flag6 && flag5))
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.MaybeDefault);
                }
                if (flag5)
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.NotNull);
                }
                return typeWithState;
            }
            static TypeWithState applyPostConditionsWhenTrue(TypeWithState typeWithState, FlowAnalysisAnnotations annotations)
            {
                bool flag7 = (annotations & FlowAnalysisAnnotations.NotNullWhenTrue) != 0;
                bool num = (annotations & FlowAnalysisAnnotations.MaybeNullWhenTrue) != 0;
                bool flag8 = (annotations & FlowAnalysisAnnotations.MaybeNullWhenFalse) != 0;
                if (num && !(flag8 && flag7))
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.MaybeDefault);
                }
                if (flag7)
                {
                    return TypeWithState.Create(typeWithState.Type, NullableFlowState.NotNull);
                }
                return typeWithState;
            }
            static bool hasConditionalPostCondition(FlowAnalysisAnnotations annotations)
            {
                if (!(((annotations & FlowAnalysisAnnotations.MaybeNullWhenTrue) != 0) ^ ((annotations & FlowAnalysisAnnotations.MaybeNullWhenFalse) != 0)))
                {
                    return ((annotations & FlowAnalysisAnnotations.NotNullWhenTrue) != 0) ^ ((annotations & FlowAnalysisAnnotations.NotNullWhenFalse) != 0);
                }
                return true;
            }
            void learnFromPostConditions(BoundExpression argument, TypeWithAnnotations parameterType, FlowAnalysisAnnotations parameterAnnotations)
            {
                bool flag = (parameterAnnotations & FlowAnalysisAnnotations.NotNullWhenTrue) != 0;
                bool flag2 = (parameterAnnotations & FlowAnalysisAnnotations.NotNullWhenFalse) != 0;
                bool flag3 = (parameterAnnotations & FlowAnalysisAnnotations.MaybeNullWhenTrue) != 0;
                bool flag4 = (parameterAnnotations & FlowAnalysisAnnotations.MaybeNullWhenFalse) != 0;
                if (flag3 && flag4 && !IsConditionalState && !(flag && flag2))
                {
                    LearnFromNullTest(argument, ref State);
                }
                else if (flag && flag2 && !IsConditionalState && !(flag3 || flag4))
                {
                    LearnFromNonNullTest(argument, ref State);
                }
                else if (flag || flag2 || flag3 || flag4)
                {
                    Split();
                    if (flag)
                    {
                        LearnFromNonNullTest(argument, ref StateWhenTrue);
                    }
                    if (flag2)
                    {
                        LearnFromNonNullTest(argument, ref StateWhenFalse);
                    }
                    if (flag3)
                    {
                        LearnFromNullTest(argument, ref StateWhenTrue);
                    }
                    if (flag4)
                    {
                        LearnFromNullTest(argument, ref StateWhenFalse);
                    }
                }
            }
            FlowAnalysisAnnotations notNullBasedOnParameters(FlowAnalysisAnnotations parameterAnnotations, ArrayBuilder<ParameterSymbol>? notNullParametersOpt, ParameterSymbol parameter)
            {
                if (!IsAnalyzingAttribute && notNullParametersOpt != null)
                {
                    ImmutableHashSet<string> notNullIfParameterNotNull = parameter.NotNullIfParameterNotNull;
                    if (!notNullIfParameterNotNull.IsEmpty)
                    {
                        ArrayBuilder<ParameterSymbol>.Enumerator enumerator = notNullParametersOpt!.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ParameterSymbol current = enumerator.Current;
                            if (notNullIfParameterNotNull.Contains(current.Name))
                            {
                                return FlowAnalysisAnnotations.NotNull;
                            }
                        }
                    }
                }
                return parameterAnnotations;
            }
            void trackNullableStateForAssignment(BoundExpression parameterValue, TypeWithAnnotations lValueType, int targetSlot, TypeWithState parameterWithState, bool isSuppressed, FlowAnalysisAnnotations parameterAnnotations)
            {
                if (!IsConditionalState && !hasConditionalPostCondition(parameterAnnotations))
                {
                    TrackNullableStateForAssignment(parameterValue, lValueType, targetSlot, parameterWithState.WithSuppression(isSuppressed));
                }
                else
                {
                    Split();
                    LocalState state = StateWhenFalse.Clone();
                    SetState(StateWhenTrue);
                    TrackNullableStateForAssignment(parameterValue, lValueType, targetSlot, applyPostConditionsWhenTrue(parameterWithState, parameterAnnotations).WithSuppression(isSuppressed));
                    LocalState whenTrue = State.Clone();
                    SetState(state);
                    TrackNullableStateForAssignment(parameterValue, lValueType, targetSlot, applyPostConditionsWhenFalse(parameterWithState, parameterAnnotations).WithSuppression(isSuppressed));
                    SetConditionalState(whenTrue, State);
                }
            }
        }

        private (ImmutableArray<BoundExpression> arguments, ImmutableArray<Conversion> conversions) RemoveArgumentConversions(ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> refKindsOpt)
        {
            int length = arguments.Length;
            ImmutableArray<Conversion> item = default(ImmutableArray<Conversion>);
            if (length > 0)
            {
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
                ArrayBuilder<Conversion> instance2 = ArrayBuilder<Conversion>.GetInstance(length);
                bool flag = false;
                for (int i = 0; i < length; i++)
                {
                    RefKind refKind = AbstractFlowPass<LocalState, LocalFunctionState>.GetRefKind(refKindsOpt, i);
                    BoundExpression boundExpression = arguments[i];
                    Conversion item2 = Conversion.Identity;
                    if (refKind == RefKind.None)
                    {
                        BoundExpression boundExpression2 = boundExpression;
                        (boundExpression, item2) = RemoveConversion(boundExpression, includeExplicitConversions: false);
                        if (boundExpression != boundExpression2)
                        {
                            SnapshotWalkerThroughConversionGroup(boundExpression2, boundExpression);
                            flag = true;
                        }
                    }
                    instance.Add(boundExpression);
                    instance2.Add(item2);
                }
                if (flag)
                {
                    arguments = instance.ToImmutable();
                    item = instance2.ToImmutable();
                }
                instance.Free();
                instance2.Free();
            }
            return (arguments, item);
        }

        private static VariableState GetVariableState(Variables variables, LocalState localState)
        {
            return new VariableState(variables.CreateSnapshot(), localState.CreateSnapshot());
        }

        private (ParameterSymbol? Parameter, TypeWithAnnotations Type, FlowAnalysisAnnotations Annotations, bool isExpandedParamsArgument) GetCorrespondingParameter(int argumentOrdinal, ImmutableArray<ParameterSymbol> parametersOpt, ImmutableArray<int> argsToParamsOpt, bool expanded)
        {
            if (parametersOpt.IsDefault)
            {
                return default((ParameterSymbol, TypeWithAnnotations, FlowAnalysisAnnotations, bool));
            }
            ParameterSymbol correspondingParameter = Binder.GetCorrespondingParameter(argumentOrdinal, parametersOpt, argsToParamsOpt, expanded);
            if ((object)correspondingParameter == null)
            {
                return default((ParameterSymbol, TypeWithAnnotations, FlowAnalysisAnnotations, bool));
            }
            TypeWithAnnotations typeWithAnnotations = correspondingParameter.TypeWithAnnotations;
            if (expanded && correspondingParameter.Ordinal == parametersOpt.Length - 1 && typeWithAnnotations.IsSZArray())
            {
                typeWithAnnotations = ((ArrayTypeSymbol)typeWithAnnotations.Type).ElementTypeWithAnnotations;
                return (correspondingParameter, typeWithAnnotations, FlowAnalysisAnnotations.None, true);
            }
            return (correspondingParameter, typeWithAnnotations, GetParameterAnnotations(correspondingParameter), false);
        }

        private MethodSymbol InferMethodTypeArguments(MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<int> argsToParamsOpt, bool expanded)
        {
            MethodSymbol constructedFrom = method.ConstructedFrom;
            ArrayBuilder<RefKind> instance = ArrayBuilder<RefKind>.GetInstance();
            if (argumentRefKindsOpt != null)
            {
                instance.AddRange(argumentRefKindsOpt);
            }
            OverloadResolution.GetEffectiveParameterTypes(constructedFrom, arguments.Length, argsToParamsOpt, instance, isMethodGroupConversion: false, allowRefOmittedArguments: true, _binder, expanded, out var parameterTypes, out var parameterRefKinds);
            instance.Free();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            MethodTypeInferenceResult methodTypeInferenceResult = MethodTypeInferrer.Infer(_binder, _conversions, constructedFrom.TypeParameters, constructedFrom.ContainingType, parameterTypes, parameterRefKinds, arguments, ref useSiteInfo, new MethodInferenceExtensions(this));
            if (!methodTypeInferenceResult.Success)
            {
                return method;
            }
            return constructedFrom.Construct(methodTypeInferenceResult.InferredTypeArguments);
        }

        private ImmutableArray<BoundExpression> GetArgumentsForMethodTypeInference(ImmutableArray<VisitArgumentResult> argumentResults, ImmutableArray<BoundExpression> arguments)
        {
            int length = argumentResults.Length;
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(length);
            for (int i = 0; i < length; i++)
            {
                VisitArgumentResult visitArgumentResult = argumentResults[i];
                Optional<LocalState> stateForLambda = visitArgumentResult.StateForLambda;
                TypeWithAnnotations argumentType2 = visitArgumentResult.RValueType.ToTypeWithAnnotations(compilation);
                instance.Add(getArgumentForMethodTypeInference(arguments[i], argumentType2, stateForLambda));
            }
            return instance.ToImmutableAndFree();
            BoundExpression getArgumentForMethodTypeInference(BoundExpression argument, TypeWithAnnotations argumentType, Optional<LocalState> lambdaState)
            {
                if (argument.Kind == BoundKind.Lambda)
                {
                    return getUnboundLambda((BoundLambda)argument, GetVariableState(_variables, lambdaState.Value));
                }
                if (!argumentType.HasType)
                {
                    return argument;
                }
                if (argument is BoundLocal boundLocal)
                {
                    if (boundLocal.DeclarationKind == BoundLocalDeclarationKind.WithInferredType)
                    {
                        goto IL_0071;
                    }
                }
                else if (argument is BoundConditionalOperator boundConditionalOperator)
                {
                    if (boundConditionalOperator.WasTargetTyped)
                    {
                        goto IL_0071;
                    }
                }
                else if (argument is BoundConvertedSwitchExpression boundConvertedSwitchExpression && boundConvertedSwitchExpression.WasTargetTyped)
                {
                    goto IL_0071;
                }
                bool flag = false;
                goto IL_0077;
            IL_0077:
                if (flag)
                {
                    return new BoundExpressionWithNullability(argument.Syntax, argument, NullableAnnotation.Oblivious, null);
                }
                return new BoundExpressionWithNullability(argument.Syntax, argument, argumentType.NullableAnnotation, argumentType.Type);
            IL_0071:
                flag = true;
                goto IL_0077;
            }
            static UnboundLambda getUnboundLambda(BoundLambda expr, VariableState variableState)
            {
                return expr.UnboundLambda.WithNullableState(variableState);
            }
        }

        private void CheckMethodConstraints(SyntaxNode syntax, MethodSymbol method)
        {
            if (_disableDiagnostics)
            {
                return;
            }
            ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
            ArrayBuilder<TypeParameterDiagnosticInfo> instance2 = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
            ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
            ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(compilation, _conversions, includeNullability: true, NoLocation.Singleton, null, CompoundUseSiteInfo<AssemblySymbol>.Discarded);
            ConstraintsHelper.CheckMethodConstraints(method, in args, instance, instance2, ref useSiteDiagnosticsBuilder);
            ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterDiagnosticInfo current = enumerator.Current;
                if (current.UseSiteInfo.DiagnosticInfo != null)
                {
                    base.Diagnostics.Add(current.UseSiteInfo.DiagnosticInfo, syntax.Location);
                }
            }
            useSiteDiagnosticsBuilder?.Free();
            instance2.Free();
            instance.Free();
        }

        private static (BoundExpression expression, Conversion conversion) RemoveConversion(BoundExpression expr, bool includeExplicitConversions)
        {
            ConversionGroup conversionGroup = null;
            while (expr.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)expr;
                if (conversionGroup != boundConversion.ConversionGroupOpt && conversionGroup != null)
                {
                    break;
                }
                conversionGroup = boundConversion.ConversionGroupOpt;
                if (!includeExplicitConversions && conversionGroup != null && conversionGroup.IsExplicitConversion)
                {
                    return (expr, Conversion.Identity);
                }
                expr = boundConversion.Operand;
                if (conversionGroup == null)
                {
                    return (expr, boundConversion.Conversion);
                }
            }
            return (expr, conversionGroup?.Conversion ?? Conversion.Identity);
        }

        private Conversion GenerateConversionForConditionalOperator(BoundExpression sourceExpression, TypeSymbol? sourceType, TypeSymbol destinationType, bool reportMismatch)
        {
            Conversion result = GenerateConversion(_conversions, sourceExpression, sourceType, destinationType, fromExplicitCast: false, extensionMethodThisArgument: false);
            if (!result.Exists && reportMismatch && !sourceExpression.IsSuppressed)
            {
                ReportNullabilityMismatchInAssignment(sourceExpression.Syntax, GetTypeAsDiagnosticArgument(sourceType), destinationType);
            }
            return result;
        }

        private static Conversion GenerateConversion(Conversions conversions, BoundExpression? sourceExpression, TypeSymbol? sourceType, TypeSymbol destinationType, bool fromExplicitCast, bool extensionMethodThisArgument)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            bool flag = (object)sourceType == null || UseExpressionForConversion(sourceExpression);
            if (extensionMethodThisArgument)
            {
                return conversions.ClassifyImplicitExtensionMethodThisArgConversion(flag ? sourceExpression : null, sourceType, destinationType, ref useSiteInfo);
            }
            if (!flag)
            {
                if (!fromExplicitCast)
                {
                    return conversions.ClassifyImplicitConversionFromType(sourceType, destinationType, ref useSiteInfo);
                }
                return conversions.ClassifyConversionFromType(sourceType, destinationType, ref useSiteInfo, forCast: true);
            }
            if (!fromExplicitCast)
            {
                return conversions.ClassifyImplicitConversionFromExpression(sourceExpression, destinationType, ref useSiteInfo);
            }
            return conversions.ClassifyConversionFromExpression(sourceExpression, destinationType, ref useSiteInfo, forCast: true);
        }

        private static bool UseExpressionForConversion([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] BoundExpression? value)
        {
            if (value == null)
            {
                return false;
            }
            if ((object)value!.Type == null || value!.Type.IsDynamic() || value!.ConstantValue != null)
            {
                return true;
            }
            if (value!.Kind == BoundKind.InterpolatedString)
            {
                return true;
            }
            return false;
        }

        private TypeWithState GetAdjustedResult(TypeWithState type, int slot)
        {
            if (State.HasValue(slot))
            {
                NullableFlowState defaultState = State[slot];
                return TypeWithState.Create(type.Type, defaultState);
            }
            return type;
        }

        private static Symbol AsMemberOfType(TypeSymbol? type, Symbol symbol)
        {
            Symbol symbol2 = symbol;
            NamedTypeSymbol namedTypeSymbol = type as NamedTypeSymbol;
            if ((object)namedTypeSymbol == null || namedTypeSymbol.IsErrorType() || symbol2 is ErrorMethodSymbol)
            {
                return symbol2;
            }
            if (symbol2.Kind == SymbolKind.Method && ((MethodSymbol)symbol2).MethodKind == MethodKind.LocalFunction)
            {
                return symbol2;
            }
            if (symbol2 is TupleElementFieldSymbol || symbol2 is TupleErrorFieldSymbol)
            {
                return symbol2.SymbolAsMember(namedTypeSymbol);
            }
            NamedTypeSymbol symbolContainer = symbol2.ContainingType;
            if (symbolContainer.IsAnonymousType)
            {
                int? num = ((symbol2.Kind == SymbolKind.Property) ? symbol2.MemberIndexOpt : null);
                if (!num.HasValue)
                {
                    return symbol2;
                }
                return AnonymousTypeManager.GetAnonymousTypeProperty(namedTypeSymbol, num.GetValueOrDefault());
            }
            if (!symbolContainer.IsGenericType)
            {
                return symbol2;
            }
            if (!namedTypeSymbol.IsGenericType)
            {
                return symbol2;
            }
            if (symbolContainer.IsInterface)
            {
                if (tryAsMemberOfSingleType(namedTypeSymbol, out var result2))
                {
                    return result2;
                }
                ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedTypeSymbol.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (tryAsMemberOfSingleType(enumerator.Current, out result2))
                    {
                        return result2;
                    }
                }
            }
            else
            {
                do
                {
                    if (tryAsMemberOfSingleType(namedTypeSymbol, out var result3))
                    {
                        return result3;
                    }
                    namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
                }
                while ((object)namedTypeSymbol != null);
            }
            return symbol2;
            bool tryAsMemberOfSingleType(NamedTypeSymbol singleType, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Symbol? result)
            {
                if (!singleType.Equals(symbolContainer, TypeCompareKind.AllIgnoreOptions))
                {
                    result = null;
                    return false;
                }
                Symbol originalDefinition = symbol2.OriginalDefinition;
                result = originalDefinition.SymbolAsMember(singleType);
                if (result is MethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
                {
                    result = methodSymbol.Construct(((MethodSymbol)symbol2).TypeArgumentsWithAnnotations);
                }
                return true;
            }
        }

        public override BoundNode? VisitConversion(BoundConversion node)
        {
            TypeWithAnnotations typeWithAnnotations = node.ConversionGroupOpt?.ExplicitType ?? default(TypeWithAnnotations);
            bool hasType = typeWithAnnotations.HasType;
            TypeWithAnnotations targetTypeWithNullability = (hasType ? typeWithAnnotations : TypeWithAnnotations.Create(node.Type));
            var (boundExpression, conversion) = RemoveConversion(node, includeExplicitConversions: true);
            SnapshotWalkerThroughConversionGroup(node, boundExpression);
            TypeWithState operandType = VisitRvalueWithState(boundExpression);
            SetResultType(node, VisitConversion(node, boundExpression, conversion, targetTypeWithNullability, operandType, checkConversion: true, hasType, hasType, AssignmentKind.Assignment, null, hasType, reportRemainingWarnings: true, extensionMethodThisArgument: false, default(Optional<LocalState>), trackMembers: true));
            return null;
        }

        private TypeWithState VisitOptionalImplicitConversion(BoundExpression expr, TypeWithAnnotations targetTypeOpt, bool useLegacyWarnings, bool trackMembers, AssignmentKind assignmentKind)
        {
            if (!targetTypeOpt.HasType)
            {
                return VisitRvalueWithState(expr);
            }
            var (boundExpression, conversion) = RemoveConversion(expr, includeExplicitConversions: false);
            SnapshotWalkerThroughConversionGroup(expr, boundExpression);
            TypeWithState operandType = VisitRvalueWithState(boundExpression);
            bool reportRemainingWarnings = !conversion.IsExplicit;
            BoundConversion? conversionIfApplicable = GetConversionIfApplicable(expr, boundExpression);
            Conversion conversion2 = conversion;
            TypeWithAnnotations targetTypeWithNullability = targetTypeOpt;
            bool trackMembers2 = trackMembers;
            return VisitConversion(conversionIfApplicable, boundExpression, conversion2, targetTypeWithNullability, operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings, assignmentKind, null, reportTopLevelWarnings: true, reportRemainingWarnings, extensionMethodThisArgument: false, default(Optional<LocalState>), trackMembers2);
        }

        private static bool AreNullableAndUnderlyingTypes([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] TypeSymbol? nullableTypeOpt, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] TypeSymbol? underlyingTypeOpt, out TypeWithAnnotations underlyingTypeWithAnnotations)
        {
            if ((object)nullableTypeOpt != null && nullableTypeOpt.IsNullableType() && (object)underlyingTypeOpt != null && !underlyingTypeOpt.IsNullableType())
            {
                TypeWithAnnotations nullableUnderlyingTypeWithAnnotations = nullableTypeOpt.GetNullableUnderlyingTypeWithAnnotations();
                if (nullableUnderlyingTypeWithAnnotations.Type.Equals(underlyingTypeOpt, TypeCompareKind.AllIgnoreOptions))
                {
                    underlyingTypeWithAnnotations = nullableUnderlyingTypeWithAnnotations;
                    return true;
                }
            }
            underlyingTypeWithAnnotations = default(TypeWithAnnotations);
            return false;
        }

        public override BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            VisitTupleExpression(node);
            return null;
        }

        public override BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            LocalState state = State.Clone();
            VisitWithoutDiagnostics(node.SourceTuple);
            SetState(state);
            VisitTupleExpression(node);
            return null;
        }

        private void VisitTupleExpression(BoundTupleExpression node)
        {
            ImmutableArray<BoundExpression> arguments = node.Arguments;
            ImmutableArray<TypeWithState> immutableArray = arguments.SelectAsArray((BoundExpression a, NullableWalker w) => w.VisitRvalueWithState(a), this);
            ImmutableArray<TypeWithAnnotations> newElementTypes = immutableArray.SelectAsArray((TypeWithState a) => a.ToTypeWithAnnotations(compilation));
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)node.Type;
            if ((object)namedTypeSymbol == null)
            {
                SetResultType(node, TypeWithState.Create(null, NullableFlowState.NotNull));
                return;
            }
            int orCreatePlaceholderSlot = GetOrCreatePlaceholderSlot(node);
            if (orCreatePlaceholderSlot > 0)
            {
                State[orCreatePlaceholderSlot] = NullableFlowState.NotNull;
                TrackNullableStateOfTupleElements(orCreatePlaceholderSlot, namedTypeSymbol, arguments, immutableArray, default(ImmutableArray<int>), useRestField: false);
            }
            namedTypeSymbol = namedTypeSymbol.WithElementTypes(newElementTypes);
            if (!_disableDiagnostics)
            {
                ImmutableArray<Location> elementLocations = namedTypeSymbol.TupleElements.SelectAsArray((FieldSymbol element, Location location) => element.Locations.FirstOrDefault() ?? location, node.Syntax.Location);
                NamedTypeSymbol tuple = namedTypeSymbol;
                ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(compilation, _conversions, includeNullability: true, node.Syntax.Location, null);
                tuple.CheckConstraints(in args, node.Syntax, elementLocations, new BindingDiagnosticBag(base.Diagnostics));
            }
            SetResultType(node, TypeWithState.Create(namedTypeSymbol, NullableFlowState.NotNull));
        }

        private void TrackNullableStateOfTupleElements(int slot, NamedTypeSymbol tupleType, ImmutableArray<BoundExpression> values, ImmutableArray<TypeWithState> types, ImmutableArray<int> argsToParamsOpt, bool useRestField)
        {
            if (slot > 0)
            {
                ImmutableArray<FieldSymbol> tupleElements = tupleType.TupleElements;
                int num = values.Length;
                if (useRestField)
                {
                    num = Math.Min(num, 7);
                }
                for (int i = 0; i < num; i++)
                {
                    int index = GetArgumentOrdinalFromParameterOrdinal(i);
                    trackState(values[index], tupleElements[i], types[index]);
                }
                if (useRestField && values.Length == 8 && tupleType.GetMembers("Rest").FirstOrDefault() is FieldSymbol field2)
                {
                    int index2 = GetArgumentOrdinalFromParameterOrdinal(7);
                    trackState(values[index2], field2, types[index2]);
                }
            }
            int GetArgumentOrdinalFromParameterOrdinal(int parameterOrdinal)
            {
                if (!argsToParamsOpt.IsDefault)
                {
                    return argsToParamsOpt.IndexOf(parameterOrdinal);
                }
                return parameterOrdinal;
            }
            void trackState(BoundExpression value, FieldSymbol field, TypeWithState valueType)
            {
                int orCreateSlot = GetOrCreateSlot(field, slot);
                TrackNullableStateForAssignment(value, field.TypeWithAnnotations, orCreateSlot, valueType, MakeSlot(value));
            }
        }

        private void TrackNullableStateOfNullableValue(int containingSlot, TypeSymbol containingType, BoundExpression? value, TypeWithState valueType, int valueSlot)
        {
            int nullableOfTValueSlot = GetNullableOfTValueSlot(containingType, containingSlot, out Symbol valueProperty);
            if (nullableOfTValueSlot > 0)
            {
                TrackNullableStateForAssignment(value, valueProperty.GetTypeOrReturnType(), nullableOfTValueSlot, valueType, valueSlot);
            }
        }

        private void TrackNullableStateOfTupleConversion(BoundConversion? conversionOpt, BoundExpression convertedNode, Conversion conversion, TypeSymbol targetType, TypeSymbol operandType, int slot, int valueSlot, AssignmentKind assignmentKind, ParameterSymbol? parameterOpt, bool reportWarnings)
        {
            BoundConversion conversionOpt2 = conversionOpt;
            BoundExpression convertedNode2 = convertedNode;
            ParameterSymbol parameterOpt2 = parameterOpt;
            if (operandType is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsTupleType)
            {
                ImmutableArray<Conversion> underlyingConversions = conversion.UnderlyingConversions;
                ImmutableArray<FieldSymbol> tupleElements = ((NamedTypeSymbol)targetType).TupleElements;
                ImmutableArray<FieldSymbol> tupleElements2 = namedTypeSymbol.TupleElements;
                int length = tupleElements2.Length;
                for (int i = 0; i < length; i++)
                {
                    trackConvertedValue(tupleElements[i], underlyingConversions[i], tupleElements2[i]);
                }
            }
            void trackConvertedValue(FieldSymbol targetField, Conversion conversion, FieldSymbol valueField)
            {
                switch (conversion.Kind)
                {
                    case ConversionKind.Identity:
                    case ConversionKind.NullLiteral:
                    case ConversionKind.ImplicitReference:
                    case ConversionKind.Boxing:
                    case ConversionKind.ExplicitReference:
                    case ConversionKind.Unboxing:
                    case ConversionKind.DefaultLiteral:
                        InheritNullableStateOfMember(slot, valueSlot, valueField, isDefaultValue: false, slot);
                        break;
                    case ConversionKind.ImplicitTupleLiteral:
                    case ConversionKind.ImplicitTuple:
                    case ConversionKind.ExplicitTupleLiteral:
                    case ConversionKind.ExplicitTuple:
                        {
                            int orCreateSlot4 = GetOrCreateSlot(targetField, slot);
                            if (orCreateSlot4 > 0)
                            {
                                State[orCreateSlot4] = NullableFlowState.NotNull;
                                int orCreateSlot5 = GetOrCreateSlot(valueField, valueSlot);
                                if (orCreateSlot5 > 0)
                                {
                                    TrackNullableStateOfTupleConversion(conversionOpt2, convertedNode2, conversion, targetField.Type, valueField.Type, orCreateSlot4, orCreateSlot5, assignmentKind, parameterOpt2, reportWarnings);
                                }
                            }
                            break;
                        }
                    case ConversionKind.ImplicitNullable:
                    case ConversionKind.ExplicitNullable:
                        {
                            if (AreNullableAndUnderlyingTypes(targetField.Type, valueField.Type, out var _))
                            {
                                int orCreateSlot2 = GetOrCreateSlot(targetField, slot);
                                if (orCreateSlot2 > 0)
                                {
                                    State[orCreateSlot2] = NullableFlowState.NotNull;
                                    int orCreateSlot3 = GetOrCreateSlot(valueField, valueSlot);
                                    if (orCreateSlot3 > 0)
                                    {
                                        TrackNullableStateOfNullableValue(orCreateSlot2, targetField.Type, null, valueField.TypeWithAnnotations.ToTypeWithState(), orCreateSlot3);
                                    }
                                }
                            }
                            break;
                        }
                    case ConversionKind.ImplicitUserDefined:
                    case ConversionKind.ExplicitUserDefined:
                        {
                            TypeWithState typeWithState = VisitUserDefinedConversion(conversionOpt2, convertedNode2, conversion, targetField.TypeWithAnnotations, valueField.TypeWithAnnotations.ToTypeWithState(), useLegacyWarnings: false, assignmentKind, parameterOpt2, reportWarnings, reportWarnings, (conversionOpt2 ?? convertedNode2).Syntax.GetLocation());
                            int orCreateSlot = GetOrCreateSlot(targetField, slot);
                            if (orCreateSlot > 0)
                            {
                                State[orCreateSlot] = typeWithState.State;
                            }
                            break;
                        }
                }
            }
        }

        public override BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            base.VisitTupleBinaryOperator(node);
            SetNotNullResult(node);
            return null;
        }

        private void ReportNullabilityMismatchWithTargetDelegate(Location location, TypeSymbol targetType, MethodSymbol targetInvokeMethod, MethodSymbol sourceInvokeMethod, bool invokedAsExtensionMethod)
        {
            SourceMemberContainerTypeSymbol.CheckValidNullableMethodOverride(compilation, targetInvokeMethod, sourceInvokeMethod, new BindingDiagnosticBag(base.Diagnostics), reportBadDelegateReturn, reportBadDelegateParameter, (targetType, location), invokedAsExtensionMethod);
            void reportBadDelegateParameter(BindingDiagnosticBag bag, MethodSymbol sourceInvokeMethod, MethodSymbol targetInvokeMethod, ParameterSymbol parameter, bool topLevel, (TypeSymbol targetType, Location location) arg)
            {
                ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate, arg.location, GetParameterAsDiagnosticArgument(parameter), GetContainingSymbolAsDiagnosticArgument(parameter), arg.targetType);
            }
            void reportBadDelegateReturn(BindingDiagnosticBag bag, MethodSymbol targetInvokeMethod, MethodSymbol sourceInvokeMethod, bool topLevel, (TypeSymbol targetType, Location location) arg)
            {
                ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInReturnTypeOfTargetDelegate, arg.location, new FormattedSymbol(sourceInvokeMethod, SymbolDisplayFormat.MinimallyQualifiedFormat), arg.targetType);
            }
        }

        private void ReportNullabilityMismatchWithTargetDelegate(Location location, NamedTypeSymbol delegateType, UnboundLambda unboundLambda)
        {
            if (!unboundLambda.HasExplicitlyTypedParameterList)
            {
                return;
            }
            MethodSymbol methodSymbol = delegateType?.DelegateInvokeMethod;
            if ((object)methodSymbol == null)
            {
                return;
            }
            int num = Math.Min(methodSymbol.ParameterCount, unboundLambda.ParameterCount);
            for (int i = 0; i < num; i++)
            {
                ParameterSymbol parameterSymbol = methodSymbol.Parameters[i];
                if (IsNullabilityMismatch(parameterSymbol.TypeWithAnnotations, unboundLambda.ParameterTypeWithAnnotations(i), requireIdentity: true))
                {
                    ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInParameterTypeOfTargetDelegate, location, unboundLambda.ParameterName(i), unboundLambda.MessageID.Localize(), delegateType);
                }
            }
        }

        private bool IsNullabilityMismatch(TypeWithAnnotations source, TypeWithAnnotations destination, bool requireIdentity)
        {
            if (!HasTopLevelNullabilityConversion(source, destination, requireIdentity))
            {
                return true;
            }
            if (requireIdentity)
            {
                return IsNullabilityMismatch(source, destination);
            }
            TypeSymbol type = source.Type;
            TypeSymbol type2 = destination.Type;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return !_conversions.ClassifyImplicitConversionFromType(type, type2, ref useSiteInfo).Exists;
        }

        private bool HasTopLevelNullabilityConversion(TypeWithAnnotations source, TypeWithAnnotations destination, bool requireIdentity)
        {
            if (!requireIdentity)
            {
                return _conversions.HasTopLevelNullabilityImplicitConversion(source, destination);
            }
            return _conversions.HasTopLevelNullabilityIdentityConversion(source, destination);
        }

        private static BoundConversion? GetConversionIfApplicable(BoundExpression? conversionOpt, BoundExpression convertedNode)
        {
            if (conversionOpt != convertedNode)
            {
                return (BoundConversion)conversionOpt;
            }
            return null;
        }

        private TypeWithState VisitConversion(BoundConversion? conversionOpt, BoundExpression conversionOperand, Conversion conversion, TypeWithAnnotations targetTypeWithNullability, TypeWithState operandType, bool checkConversion, bool fromExplicitCast, bool useLegacyWarnings, AssignmentKind assignmentKind, ParameterSymbol? parameterOpt = null, bool reportTopLevelWarnings = true, bool reportRemainingWarnings = true, bool extensionMethodThisArgument = false, Optional<LocalState> stateForLambda = default(Optional<LocalState>), bool trackMembers = false, Location? diagnosticLocationOpt = null)
        {
            BoundExpression conversionOperand2 = conversionOperand;
            NullableFlowState resultState2 = NullableFlowState.NotNull;
            bool flag = true;
            bool isSuppressed2 = false;
            if ((object)diagnosticLocationOpt == null)
            {
                diagnosticLocationOpt = (conversionOpt ?? conversionOperand2).Syntax.GetLocation();
            }
            if (conversionOperand2.IsSuppressed)
            {
                reportTopLevelWarnings = false;
                reportRemainingWarnings = false;
                isSuppressed2 = true;
            }
            TypeSymbol type = targetTypeWithNullability.Type;
            switch (conversion.Kind)
            {
                case ConversionKind.MethodGroup:
                    {
                        BoundMethodGroup boundMethodGroup = conversionOperand2 as BoundMethodGroup;
                        (MethodSymbol invokeSignature, ImmutableArray<ParameterSymbol>) tuple = getDelegateOrFunctionPointerInfo(type);
                        MethodSymbol item = tuple.invokeSignature;
                        ImmutableArray<ParameterSymbol> item2 = tuple.Item2;
                        MethodSymbol methodSymbol = conversion.Method;
                        if (boundMethodGroup != null)
                        {
                            if (methodSymbol?.OriginalDefinition is LocalFunctionSymbol symbol)
                            {
                                VisitLocalFunctionUse(symbol);
                            }
                            methodSymbol = CheckMethodGroupReceiverNullability(boundMethodGroup, item2, methodSymbol, conversion.IsExtensionMethod);
                        }
                        if (reportRemainingWarnings && item != null)
                        {
                            ReportNullabilityMismatchWithTargetDelegate(diagnosticLocationOpt, type, item, methodSymbol, conversion.IsExtensionMethod);
                        }
                        resultState2 = NullableFlowState.NotNull;
                        break;
                    }
                case ConversionKind.AnonymousFunction:
                    if (conversionOperand2 is BoundLambda boundLambda)
                    {
                        NamedTypeSymbol delegateType = type.GetDelegateType();
                        VisitLambda(boundLambda, delegateType, stateForLambda);
                        if (reportRemainingWarnings)
                        {
                            ReportNullabilityMismatchWithTargetDelegate(diagnosticLocationOpt, delegateType, boundLambda.UnboundLambda);
                        }
                        TrackAnalyzedNullabilityThroughConversionGroup(targetTypeWithNullability.ToTypeWithState(), conversionOpt, conversionOperand2);
                        return TypeWithState.Create(type, NullableFlowState.NotNull);
                    }
                    break;
                case ConversionKind.InterpolatedString:
                    resultState2 = NullableFlowState.NotNull;
                    break;
                case ConversionKind.SwitchExpression:
                case ConversionKind.ConditionalExpression:
                case ConversionKind.ObjectCreation:
                    resultState2 = visitNestedTargetTypedConstructs();
                    TrackAnalyzedNullabilityThroughConversionGroup(targetTypeWithNullability.ToTypeWithState(), conversionOpt, conversionOperand2);
                    break;
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return VisitUserDefinedConversion(conversionOpt, conversionOperand2, conversion, targetTypeWithNullability, operandType, useLegacyWarnings, assignmentKind, parameterOpt, reportTopLevelWarnings, reportRemainingWarnings, diagnosticLocationOpt);
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitDynamic:
                    resultState2 = getConversionResultState(operandType);
                    break;
                case ConversionKind.Boxing:
                    resultState2 = getBoxingConversionResultState(targetTypeWithNullability, operandType);
                    break;
                case ConversionKind.Unboxing:
                    if (type.IsNonNullableValueType())
                    {
                        if (!operandType.IsNotNull && reportRemainingWarnings)
                        {
                            ReportDiagnostic(ErrorCode.WRN_UnboxPossibleNull, diagnosticLocationOpt);
                        }
                        LearnFromNonNullTest(conversionOperand2, ref State);
                    }
                    else
                    {
                        resultState2 = getUnboxingConversionResultState(operandType);
                    }
                    break;
                case ConversionKind.ImplicitThrow:
                    resultState2 = NullableFlowState.NotNull;
                    break;
                case ConversionKind.NoConversion:
                    resultState2 = getConversionResultState(operandType);
                    break;
                case ConversionKind.NullLiteral:
                case ConversionKind.DefaultLiteral:
                    checkConversion = false;
                    goto case ConversionKind.Identity;
                case ConversionKind.Identity:
                    {
                        if (useLegacyWarnings && conversionOperand2 is BoundConversion boundConversion && !boundConversion.ConversionKind.IsUserDefinedConversion())
                        {
                            TypeWithAnnotations? typeWithAnnotations = boundConversion.ConversionGroupOpt?.ExplicitType;
                            if (typeWithAnnotations.HasValue && typeWithAnnotations.GetValueOrDefault().Equals(targetTypeWithNullability, TypeCompareKind.ConsiderEverything))
                            {
                                TrackAnalyzedNullabilityThroughConversionGroup(calculateResultType(targetTypeWithNullability, fromExplicitCast, operandType.State, isSuppressed2, type), conversionOpt, conversionOperand2);
                                return operandType;
                            }
                        }
                        TypeSymbol? type3 = operandType.Type;
                        if (((object)type3 == null || !type3!.IsTupleType) && conversionOperand2.Kind != BoundKind.TupleLiteral)
                        {
                            goto case ConversionKind.ImplicitReference;
                        }
                        goto case ConversionKind.ImplicitTupleLiteral;
                    }
                case ConversionKind.ImplicitReference:
                case ConversionKind.ExplicitReference:
                    if (checkConversion)
                    {
                        conversion = GenerateConversion(_conversions, conversionOperand2, operandType.Type, type, fromExplicitCast, extensionMethodThisArgument);
                        flag = conversion.Exists;
                    }
                    resultState2 = (conversion.IsReference ? getReferenceConversionResultState(targetTypeWithNullability, operandType) : operandType.State);
                    break;
                case ConversionKind.ImplicitNullable:
                    {
                        if (trackMembers && AreNullableAndUnderlyingTypes(type, operandType.Type, out var underlyingTypeWithAnnotations))
                        {
                            int num2 = MakeSlot(conversionOperand2);
                            if (num2 > 0)
                            {
                                int orCreatePlaceholderSlot2 = GetOrCreatePlaceholderSlot(conversionOpt);
                                TrackNullableStateOfNullableValue(orCreatePlaceholderSlot2, type, conversionOperand2, underlyingTypeWithAnnotations.ToTypeWithState(), num2);
                            }
                        }
                        if (checkConversion)
                        {
                            conversion = GenerateConversion(_conversions, conversionOperand2, operandType.Type, type, fromExplicitCast, extensionMethodThisArgument);
                            flag = conversion.Exists;
                        }
                        resultState2 = operandType.State;
                        break;
                    }
                case ConversionKind.ExplicitNullable:
                    {
                        TypeSymbol? type2 = operandType.Type;
                        if ((object)type2 != null && type2.IsNullableType() && !type.IsNullableType())
                        {
                            if (reportTopLevelWarnings && operandType.MayBeNull)
                            {
                                ReportDiagnostic(ErrorCode.WRN_NullableValueTypeMayBeNull, diagnosticLocationOpt);
                            }
                            if (conversionOperand2 != null)
                            {
                                LearnFromNonNullTest(conversionOperand2, ref State);
                            }
                        }
                        goto case ConversionKind.ImplicitNullable;
                    }
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitTuple:
                    if (trackMembers)
                    {
                        ConversionKind kind = conversion.Kind;
                        if (kind == ConversionKind.ImplicitTuple || kind == ConversionKind.ExplicitTuple)
                        {
                            int num = MakeSlot(conversionOperand2);
                            if (num > 0)
                            {
                                int orCreatePlaceholderSlot = GetOrCreatePlaceholderSlot(conversionOpt);
                                if (orCreatePlaceholderSlot > 0)
                                {
                                    TrackNullableStateOfTupleConversion(conversionOpt, conversionOperand2, conversion, type, operandType.Type, orCreatePlaceholderSlot, num, assignmentKind, parameterOpt, reportRemainingWarnings);
                                }
                            }
                        }
                    }
                    if (checkConversion && !type.IsErrorType())
                    {
                        conversion = GenerateConversion(_conversions, conversionOperand2, operandType.Type, type, fromExplicitCast, extensionMethodThisArgument);
                        flag = conversion.Exists;
                    }
                    resultState2 = NullableFlowState.NotNull;
                    break;
            }
            TypeWithState typeWithState = calculateResultType(targetTypeWithNullability, fromExplicitCast, resultState2, isSuppressed2, type);
            TypeSymbol? type4 = operandType.Type;
            if (((object)type4 == null || !type4.IsErrorType()) && !type.IsErrorType())
            {
                if (reportTopLevelWarnings)
                {
                    ReportNullableAssignmentIfNecessary(conversionOperand2, targetTypeWithNullability, typeWithState, useLegacyWarnings, assignmentKind, parameterOpt, diagnosticLocationOpt);
                }
                if (reportRemainingWarnings && !flag)
                {
                    if (assignmentKind == AssignmentKind.Argument)
                    {
                        ReportNullabilityMismatchInArgument(diagnosticLocationOpt, operandType.Type, parameterOpt, type, forOutput: false);
                    }
                    else
                    {
                        ReportNullabilityMismatchInAssignment(diagnosticLocationOpt, GetTypeAsDiagnosticArgument(operandType.Type), type);
                    }
                }
            }
            TrackAnalyzedNullabilityThroughConversionGroup(typeWithState, conversionOpt, conversionOperand2);
            return typeWithState;
            static TypeWithState calculateResultType(TypeWithAnnotations targetTypeWithNullability, bool fromExplicitCast, NullableFlowState resultState, bool isSuppressed, TypeSymbol targetType)
            {
                if (isSuppressed)
                {
                    resultState = NullableFlowState.NotNull;
                }
                else if (fromExplicitCast && targetTypeWithNullability.NullableAnnotation.IsAnnotated() && !targetType.IsNullableType())
                {
                    resultState = (((object)targetType == null || !targetType.IsTypeParameterDisallowingAnnotationInCSharp8()) ? NullableFlowState.MaybeNull : NullableFlowState.MaybeDefault);
                }
                return TypeWithState.Create(targetType, resultState);
            }
            static bool dependsOnTypeParameter(TypeParameterSymbol typeParameter1, TypeParameterSymbol typeParameter2, NullableAnnotation typeParameter1Annotation, out NullableAnnotation annotation)
            {
                if (typeParameter1.Equals(typeParameter2, TypeCompareKind.AllIgnoreOptions))
                {
                    annotation = typeParameter1Annotation;
                    return true;
                }
                bool flag2 = false;
                NullableAnnotation a = NullableAnnotation.Annotated;
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeParameter1.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations current = enumerator.Current;
                    if (current.Type is TypeParameterSymbol typeParameter4 && dependsOnTypeParameter(typeParameter4, typeParameter2, current.NullableAnnotation, out var annotation3))
                    {
                        flag2 = true;
                        a = a.Meet(annotation3);
                    }
                }
                if (flag2)
                {
                    annotation = a.Join(typeParameter1Annotation);
                    return true;
                }
                annotation = NullableAnnotation.NotAnnotated;
                return false;
            }
            static NullableFlowState getBoxingConversionResultState(TypeWithAnnotations targetType, TypeWithState operandType)
            {
                NullableFlowState state4 = operandType.State;
                if (state4 == NullableFlowState.MaybeNull)
                {
                    TypeSymbol type8 = operandType.Type;
                    if ((object)type8 == null || !type8.IsTypeParameterDisallowingAnnotationInCSharp8())
                    {
                        return NullableFlowState.MaybeDefault;
                    }
                    if (targetType.NullableAnnotation.IsNotAnnotated() && type8 is TypeParameterSymbol typeParameter5 && targetType.Type is TypeParameterSymbol typeParameter6 && dependsOnTypeParameter(typeParameter5, typeParameter6, NullableAnnotation.NotAnnotated, out var annotation4))
                    {
                        if (annotation4 != NullableAnnotation.Annotated)
                        {
                            return NullableFlowState.MaybeNull;
                        }
                        return NullableFlowState.MaybeDefault;
                    }
                }
                return state4;
            }
            static NullableFlowState getConversionResultState(TypeWithState operandType)
            {
                NullableFlowState state2 = operandType.State;
                if (state2 == NullableFlowState.MaybeNull)
                {
                    return NullableFlowState.MaybeDefault;
                }
                return state2;
            }
            static (MethodSymbol invokeSignature, ImmutableArray<ParameterSymbol>) getDelegateOrFunctionPointerInfo(TypeSymbol targetType)
            {
                if (targetType is NamedTypeSymbol namedTypeSymbol)
                {
                    if (targetType.TypeKind == TypeKind.Delegate)
                    {
                        MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
                        if ((object)delegateInvokeMethod != null)
                        {
                            ImmutableArray<ParameterSymbol> parameters = delegateInvokeMethod.Parameters;
                            return (delegateInvokeMethod, parameters);
                        }
                    }
                    if (targetType.SpecialType == SpecialType.System_Delegate)
                    {
                        goto IL_0080;
                    }
                    if (targetType is ErrorTypeSymbol)
                    {
                        return (null, ImmutableArray<ParameterSymbol>.Empty);
                    }
                }
                else
                {
                    if (targetType is FunctionPointerTypeSymbol functionPointerTypeSymbol)
                    {
                        FunctionPointerMethodSymbol signature = functionPointerTypeSymbol.Signature;
                        if ((object)signature != null)
                        {
                            ImmutableArray<ParameterSymbol> parameters2 = signature.Parameters;
                            return (signature, parameters2);
                        }
                    }
                    else if ((object)targetType == null)
                    {
                        goto IL_009e;
                    }
                    if (targetType.SpecialType == SpecialType.System_Delegate)
                    {
                        goto IL_0080;
                    }
                }
                goto IL_009e;
            IL_0080:
                return (null, ImmutableArray<ParameterSymbol>.Empty);
            IL_009e:
                throw ExceptionUtilities.UnexpectedValue(targetType);
            }
            static NullableFlowState getReferenceConversionResultState(TypeWithAnnotations targetType, TypeWithState operandType)
            {
                NullableFlowState state = operandType.State;
                switch (state)
                {
                    case NullableFlowState.MaybeNull:
                        {
                            TypeSymbol type6 = targetType.Type;
                            if ((object)type6 != null && type6.IsTypeParameterDisallowingAnnotationInCSharp8())
                            {
                                TypeSymbol type7 = operandType.Type;
                                if ((object)type7 == null || !type7.IsTypeParameterDisallowingAnnotationInCSharp8())
                                {
                                    return NullableFlowState.MaybeDefault;
                                }
                                if (targetType.NullableAnnotation.IsNotAnnotated() && type7 is TypeParameterSymbol typeParameter3 && dependsOnTypeParameter(typeParameter3, (TypeParameterSymbol)targetType.Type, NullableAnnotation.NotAnnotated, out var annotation2))
                                {
                                    if (annotation2 != NullableAnnotation.Annotated)
                                    {
                                        return NullableFlowState.MaybeNull;
                                    }
                                    return NullableFlowState.MaybeDefault;
                                }
                            }
                            break;
                        }
                    case NullableFlowState.MaybeDefault:
                        {
                            TypeSymbol type5 = targetType.Type;
                            if ((object)type5 != null && !type5.IsTypeParameterDisallowingAnnotationInCSharp8())
                            {
                                return NullableFlowState.MaybeNull;
                            }
                            break;
                        }
                }
                return state;
            }
            static NullableFlowState getUnboxingConversionResultState(TypeWithState operandType)
            {
                NullableFlowState state3 = operandType.State;
                if (state3 == NullableFlowState.MaybeNull)
                {
                    return NullableFlowState.MaybeDefault;
                }
                return state3;
            }
            NullableFlowState visitNestedTargetTypedConstructs()
            {
                if (conversionOperand2 is BoundConditionalOperator boundConditionalOperator)
                {
                    if (boundConditionalOperator.WasTargetTyped)
                    {
                        ImmutableArray<(LocalState, TypeWithState, bool)> immutableArray = ConditionalInfoForConversion[boundConditionalOperator];
                        BoundExpression consequence = boundConditionalOperator.Consequence;
                        (BoundExpression expression, Conversion conversion) tuple2 = RemoveConversion(consequence, includeExplicitConversions: false);
                        BoundExpression item3 = tuple2.expression;
                        Conversion item4 = tuple2.conversion;
                        TypeWithState typeWithState2 = ConvertConditionalOperandOrSwitchExpressionArmResult(consequence, item3, item4, targetTypeWithNullability, immutableArray[0].Item2, immutableArray[0].Item1, immutableArray[0].Item3);
                        BoundExpression alternative = boundConditionalOperator.Alternative;
                        (BoundExpression expression, Conversion conversion) tuple3 = RemoveConversion(alternative, includeExplicitConversions: false);
                        BoundExpression item5 = tuple3.expression;
                        Conversion item6 = tuple3.conversion;
                        TypeWithState typeWithState3 = ConvertConditionalOperandOrSwitchExpressionArmResult(alternative, item5, item6, targetTypeWithNullability, immutableArray[1].Item2, immutableArray[1].Item1, immutableArray[1].Item3);
                        ConditionalInfoForConversion.Remove(boundConditionalOperator);
                        return typeWithState2.State.Join(typeWithState3.State);
                    }
                }
                else if (conversionOperand2 is BoundConvertedSwitchExpression boundConvertedSwitchExpression)
                {
                    if (boundConvertedSwitchExpression.WasTargetTyped)
                    {
                        ImmutableArray<(LocalState, TypeWithState, bool)> immutableArray2 = ConditionalInfoForConversion[boundConvertedSwitchExpression];
                        ArrayBuilder<TypeWithState> instance = ArrayBuilder<TypeWithState>.GetInstance(immutableArray2.Length);
                        for (int i = 0; i < immutableArray2.Length; i++)
                        {
                            (LocalState, TypeWithState, bool) tuple4 = immutableArray2[i];
                            LocalState item7 = tuple4.Item1;
                            TypeWithState item8 = tuple4.Item2;
                            bool item9 = tuple4.Item3;
                            BoundExpression value = boundConvertedSwitchExpression.SwitchArms[i].Value;
                            var (operand, conversion2) = RemoveConversion(value, includeExplicitConversions: false);
                            instance.Add(ConvertConditionalOperandOrSwitchExpressionArmResult(value, operand, conversion2, targetTypeWithNullability, item8, item7, item9));
                        }
                        NullableFlowState nullableState = BestTypeInferrer.GetNullableState(instance);
                        instance.Free();
                        ConditionalInfoForConversion.Remove(boundConvertedSwitchExpression);
                        return nullableState;
                    }
                }
                else if (conversionOperand2 is BoundObjectCreationExpression boundObjectCreationExpression)
                {
                    if (boundObjectCreationExpression.WasTargetTyped)
                    {
                        goto IL_022d;
                    }
                }
                else
                {
                    if (conversionOperand2 is BoundUnconvertedObjectCreationExpression)
                    {
                        goto IL_022d;
                    }
                    if (conversionOperand2 is BoundUnconvertedConditionalOperator || conversionOperand2 is BoundUnconvertedSwitchExpression)
                    {
                        return operandType.State;
                    }
                }
                throw ExceptionUtilities.UnexpectedValue(conversionOperand2.Kind);
            IL_022d:
                return NullableFlowState.NotNull;
            }
        }

        private TypeWithState VisitUserDefinedConversion(BoundConversion? conversionOpt, BoundExpression conversionOperand, Conversion conversion, TypeWithAnnotations targetTypeWithNullability, TypeWithState operandType, bool useLegacyWarnings, AssignmentKind assignmentKind, ParameterSymbol? parameterOpt, bool reportTopLevelWarnings, bool reportRemainingWarnings, Location diagnosticLocation)
        {
            TypeSymbol type = targetTypeWithNullability.Type;
            if (!conversion.IsValid)
            {
                TypeWithState typeWithState = TypeWithState.Create(type, NullableFlowState.NotNull);
                TrackAnalyzedNullabilityThroughConversionGroup(typeWithState, conversionOpt, conversionOperand);
                return typeWithState;
            }
            operandType = VisitConversion(conversionOpt, conversionOperand, conversion.UserDefinedFromConversion, TypeWithAnnotations.Create(conversion.BestUserDefinedConversionAnalysis!.FromType), operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings, assignmentKind, parameterOpt, reportTopLevelWarnings, reportRemainingWarnings, extensionMethodThisArgument: false, default(Optional<LocalState>), trackMembers: false, diagnosticLocation);
            MethodSymbol method = conversion.Method;
            ParameterSymbol parameterSymbol = method.Parameters[0];
            FlowAnalysisAnnotations parameterAnnotations = GetParameterAnnotations(parameterSymbol);
            TypeWithAnnotations targetType = ApplyLValueAnnotations(parameterSymbol.TypeWithAnnotations, parameterAnnotations);
            TypeWithState typeWithState2 = default(TypeWithState);
            bool flag = false;
            if (operandType.Type.IsNullableType() && !targetType.IsNullableType())
            {
                TypeWithAnnotations nullableUnderlyingTypeWithAnnotations = operandType.Type.GetNullableUnderlyingTypeWithAnnotations();
                typeWithState2 = nullableUnderlyingTypeWithAnnotations.ToTypeWithState();
                flag = targetType.Equals(nullableUnderlyingTypeWithAnnotations, TypeCompareKind.AllIgnoreOptions);
            }
            NullableFlowState state = operandType.State;
            Location location = conversionOperand.Syntax.GetLocation();
            ClassifyAndVisitConversion(conversionOperand, targetType, flag ? typeWithState2 : operandType, useLegacyWarnings, AssignmentKind.Argument, parameterSymbol, reportRemainingWarnings, fromExplicitCast: false, location);
            if (!flag && CheckDisallowedNullAssignment(operandType, parameterAnnotations, conversionOperand.Syntax.Location))
            {
                LearnFromNonNullTest(conversionOperand, ref State);
            }
            TypeWithAnnotations returnTypeWithAnnotations = method.ReturnTypeWithAnnotations;
            operandType = GetLiftedReturnTypeIfNecessary(flag, returnTypeWithAnnotations, state);
            if (!flag || state.IsNotNull())
            {
                operandType = ((!state.IsNotNull() || !method.ReturnNotNullIfParameterNotNull.Contains(parameterSymbol.Name)) ? ApplyUnconditionalAnnotations(operandType, GetRValueAnnotations(method)) : operandType.WithNotNullState());
            }
            operandType = ClassifyAndVisitConversion(conversionOperand, TypeWithAnnotations.Create(conversion.BestUserDefinedConversionAnalysis!.ToType), operandType, useLegacyWarnings, assignmentKind, parameterOpt, reportRemainingWarnings, fromExplicitCast: false, location);
            operandType = ClassifyAndVisitConversion(conversionOpt ?? conversionOperand, targetTypeWithNullability, operandType, useLegacyWarnings, assignmentKind, parameterOpt, reportRemainingWarnings, conversionOpt?.ExplicitCastInCode ?? false, diagnosticLocation);
            TrackAnalyzedNullabilityThroughConversionGroup(operandType, conversionOpt, conversionOperand);
            return operandType;
        }

        private void SnapshotWalkerThroughConversionGroup(BoundExpression conversionExpression, BoundExpression convertedNode)
        {
            if (_snapshotBuilderOpt != null)
            {
                BoundConversion boundConversion = conversionExpression as BoundConversion;
                _ = boundConversion?.ConversionGroupOpt;
                while (boundConversion != null && boundConversion != convertedNode && boundConversion.Syntax.SpanStart != convertedNode.Syntax.SpanStart)
                {
                    TakeIncrementalSnapshot(boundConversion);
                    boundConversion = boundConversion.Operand as BoundConversion;
                }
            }
        }

        private void TrackAnalyzedNullabilityThroughConversionGroup(TypeWithState resultType, BoundConversion? conversionOpt, BoundExpression convertedNode)
        {
            VisitResult visitResult2 = new VisitResult(resultType, resultType.ToTypeWithAnnotations(compilation));
            _ = conversionOpt?.ConversionGroupOpt;
            while (conversionOpt != null && conversionOpt != convertedNode)
            {
                visitResult2 = withType(visitResult2, conversionOpt!.Type);
                SetAnalyzedNullability(conversionOpt, visitResult2);
                conversionOpt = conversionOpt!.Operand as BoundConversion;
            }
            static VisitResult withType(VisitResult visitResult, TypeSymbol newType)
            {
                return new VisitResult(TypeWithState.Create(newType, visitResult.RValueType.State), TypeWithAnnotations.Create(newType, visitResult.LValueType.NullableAnnotation));
            }
        }

        private TypeWithState GetLiftedReturnType(TypeWithAnnotations returnType, NullableFlowState operandState)
        {
            TypeSymbol type = (returnType.Type.IsNonNullableValueType() ? MakeNullableOf(returnType) : returnType.Type);
            NullableFlowState defaultState = returnType.ToTypeWithState().State.Join(operandState);
            return TypeWithState.Create(type, defaultState);
        }

        private static TypeWithState GetNullableUnderlyingTypeIfNecessary(bool isLifted, TypeWithState typeWithState)
        {
            if (isLifted)
            {
                TypeSymbol type = typeWithState.Type;
                if ((object)type != null && type.IsNullableType())
                {
                    return type.GetNullableUnderlyingTypeWithAnnotations().ToTypeWithState();
                }
            }
            return typeWithState;
        }

        private TypeWithState GetLiftedReturnTypeIfNecessary(bool isLifted, TypeWithAnnotations returnType, NullableFlowState operandState)
        {
            if (!isLifted)
            {
                return returnType.ToTypeWithState();
            }
            return GetLiftedReturnType(returnType, operandState);
        }

        private TypeSymbol MakeNullableOf(TypeWithAnnotations underlying)
        {
            return compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(ImmutableArray.Create(underlying));
        }

        private TypeWithState ClassifyAndVisitConversion(BoundExpression node, TypeWithAnnotations targetType, TypeWithState operandType, bool useLegacyWarnings, AssignmentKind assignmentKind, ParameterSymbol? parameterOpt, bool reportWarnings, bool fromExplicitCast, Location diagnosticLocation)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            Conversion conversion = _conversions.ClassifyStandardConversion(null, operandType.Type, targetType.Type, ref useSiteInfo);
            if (reportWarnings && !conversion.Exists)
            {
                if (assignmentKind == AssignmentKind.Argument)
                {
                    ReportNullabilityMismatchInArgument(diagnosticLocation, operandType.Type, parameterOpt, targetType.Type, forOutput: false);
                }
                else
                {
                    ReportNullabilityMismatchInAssignment(diagnosticLocation, operandType.Type, targetType.Type);
                }
            }
            return VisitConversion(null, node, conversion, targetType, operandType, checkConversion: false, fromExplicitCast, useLegacyWarnings, assignmentKind, parameterOpt, reportWarnings, !fromExplicitCast && reportWarnings, extensionMethodThisArgument: false, default(Optional<LocalState>), trackMembers: false, diagnosticLocation);
        }

        public override BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.MethodOpt?.OriginalDefinition is LocalFunctionSymbol symbol)
            {
                VisitLocalFunctionUse(symbol);
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)node.Type;
            BoundExpression argument = node.Argument;
            if (!(argument is BoundMethodGroup boundMethodGroup))
            {
                if (!(argument is BoundLambda boundLambda))
                {
                    if (argument != null)
                    {
                        TypeSymbol type = argument.Type;
                        if ((object)type != null && type.TypeKind == TypeKind.Delegate)
                        {
                            TypeWithAnnotations targetType = TypeWithAnnotations.Create(type, NullableAnnotation.NotAnnotated);
                            TypeWithState valueType = VisitRvalueWithState(argument);
                            ReportNullableAssignmentIfNecessary(argument, targetType, valueType, useLegacyWarnings: false);
                            if (!argument.IsSuppressed)
                            {
                                ReportNullabilityMismatchWithTargetDelegate(argument.Syntax.Location, namedTypeSymbol, namedTypeSymbol.DelegateInvokeMethod, type.DelegateInvokeMethod(), invokedAsExtensionMethod: false);
                            }
                            LearnFromNonNullTest(argument, ref State);
                            goto IL_01a0;
                        }
                    }
                    VisitRvalue(node.Argument);
                }
                else
                {
                    VisitLambda(boundLambda, namedTypeSymbol);
                    SetNotNullResult(boundLambda);
                    if (!boundLambda.IsSuppressed)
                    {
                        ReportNullabilityMismatchWithTargetDelegate(boundLambda.Symbol.DiagnosticLocation, namedTypeSymbol, boundLambda.UnboundLambda);
                    }
                }
            }
            else
            {
                VisitMethodGroup(boundMethodGroup);
                MethodSymbol methodOpt = node.MethodOpt;
                if ((object)methodOpt != null)
                {
                    methodOpt = CheckMethodGroupReceiverNullability(boundMethodGroup, namedTypeSymbol.DelegateInvokeMethod.Parameters, methodOpt, node.IsExtensionMethod);
                    if (!boundMethodGroup.IsSuppressed)
                    {
                        ReportNullabilityMismatchWithTargetDelegate(boundMethodGroup.Syntax.Location, namedTypeSymbol, namedTypeSymbol.DelegateInvokeMethod, methodOpt, node.IsExtensionMethod);
                    }
                }
                SetAnalyzedNullability(boundMethodGroup, default(VisitResult));
            }
            goto IL_01a0;
        IL_01a0:
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            BoundExpression receiverOpt = node.ReceiverOpt;
            if (receiverOpt != null)
            {
                VisitRvalue(receiverOpt);
                SetMethodGroupReceiverNullability(receiverOpt, ResultType);
            }
            SetNotNullResult(node);
            return null;
        }

        private bool TryGetMethodGroupReceiverNullability([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] BoundExpression? receiverOpt, out TypeWithState type)
        {
            if (receiverOpt != null && _methodGroupReceiverMapOpt != null && _methodGroupReceiverMapOpt!.TryGetValue(receiverOpt, out type))
            {
                return true;
            }
            type = default(TypeWithState);
            return false;
        }

        private void SetMethodGroupReceiverNullability(BoundExpression receiver, TypeWithState type)
        {
            if (_methodGroupReceiverMapOpt == null)
            {
                _methodGroupReceiverMapOpt = PooledDictionary<BoundExpression, TypeWithState>.GetInstance();
            }
            _methodGroupReceiverMapOpt![receiver] = type;
        }

        private MethodSymbol CheckMethodGroupReceiverNullability(BoundMethodGroup group, ImmutableArray<ParameterSymbol> parameters, MethodSymbol method, bool invokedAsExtensionMethod)
        {
            BoundExpression receiverOpt = group.ReceiverOpt;
            if (TryGetMethodGroupReceiverNullability(receiverOpt, out var type))
            {
                SyntaxNode syntax = group.Syntax;
                if (!invokedAsExtensionMethod)
                {
                    method = (MethodSymbol)AsMemberOfType(type.Type, method);
                }
                if (method.IsGenericMethod && HasImplicitTypeArguments(group.Syntax))
                {
                    ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance();
                    if (invokedAsExtensionMethod)
                    {
                        instance.Add(CreatePlaceholderIfNecessary(receiverOpt, type.ToTypeWithAnnotations(compilation)));
                    }
                    ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ParameterSymbol current = enumerator.Current;
                        TypeWithAnnotations typeWithAnnotations = current.TypeWithAnnotations;
                        instance.Add(new BoundExpressionWithNullability(syntax, new BoundParameter(syntax, current), typeWithAnnotations.NullableAnnotation, typeWithAnnotations.Type));
                    }
                    method = InferMethodTypeArguments(method, instance.ToImmutableAndFree(), default(ImmutableArray<RefKind>), default(ImmutableArray<int>), expanded: false);
                }
                if (invokedAsExtensionMethod)
                {
                    CheckExtensionMethodThisNullability(receiverOpt, Conversion.Identity, method.Parameters[0], type);
                }
                else
                {
                    CheckPossibleNullReceiver(receiverOpt, type, checkNullableValueType: false);
                }
                if (ConstraintsHelper.RequiresChecking(method))
                {
                    CheckMethodConstraints(syntax, method);
                }
            }
            return method;
        }

        public override BoundNode? VisitLambda(BoundLambda node)
        {
            SetNotNullResult(node);
            return null;
        }

        private void VisitLambda(BoundLambda node, NamedTypeSymbol? delegateTypeOpt, Optional<LocalState> initialState = default(Optional<LocalState>))
        {
            MethodSymbol delegateInvokeMethod = delegateTypeOpt?.DelegateInvokeMethod;
            bool flag = UseDelegateInvokeParameterTypes(node, delegateInvokeMethod);
            if (flag && _snapshotBuilderOpt != null)
            {
                SetUpdatedSymbol(node, node.Symbol, delegateTypeOpt);
            }
            AnalyzeLocalFunctionOrLambda(node, node.Symbol, initialState.HasValue ? initialState.Value : State.Clone(), delegateInvokeMethod, flag);
        }

        private static bool UseDelegateInvokeParameterTypes(BoundLambda lambda, MethodSymbol? delegateInvokeMethod)
        {
            if ((object)delegateInvokeMethod != null)
            {
                return !lambda.UnboundLambda.HasExplicitlyTypedParameterList;
            }
            return false;
        }

        public override BoundNode? VisitUnboundLambda(UnboundLambda node)
        {
            BoundLambda node2 = node.BindForErrorRecovery();
            VisitLambda(node2, null);
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitThisReference(BoundThisReference node)
        {
            VisitThisOrBaseReference(node);
            return null;
        }

        private void VisitThisOrBaseReference(BoundExpression node)
        {
            TypeWithState resultType = TypeWithState.Create(node.Type, NullableFlowState.NotNull);
            TypeWithAnnotations lvalueType = TypeWithAnnotations.Create(node.Type, NullableAnnotation.NotAnnotated);
            SetResult(node, resultType, lvalueType);
        }

        public override BoundNode? VisitParameter(BoundParameter node)
        {
            ParameterSymbol parameterSymbol = node.ParameterSymbol;
            int orCreateSlot = GetOrCreateSlot(parameterSymbol);
            TypeWithAnnotations declaredParameterResult = GetDeclaredParameterResult(parameterSymbol);
            TypeWithState parameterState = GetParameterState(declaredParameterResult, parameterSymbol.FlowAnalysisAnnotations);
            SetResult(node, GetAdjustedResult(parameterState, orCreateSlot), declaredParameterResult);
            return null;
        }

        public override BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            BoundExpression boundExpression = node.Left;
            if (boundExpression is BoundFieldAccess boundFieldAccess && boundFieldAccess.ExpressionSymbol is FieldSymbol fieldSymbol)
            {
                Symbol associatedSymbol = fieldSymbol.AssociatedSymbol;
                if (!(associatedSymbol is PropertySymbol propertySymbol))
                {
                    if (associatedSymbol is EventSymbol eventSymbol)
                    {
                        BoundFieldAccess boundFieldAccess2 = boundFieldAccess;
                        boundExpression = new BoundEventAccess(boundFieldAccess2.Syntax, boundFieldAccess2.ReceiverOpt, eventSymbol, isUsableAsField: true, LookupResultKind.Viable, eventSymbol.Type, boundFieldAccess2.HasErrors);
                    }
                }
                else
                {
                    boundExpression = new BoundPropertyAccess(boundFieldAccess.Syntax, boundFieldAccess.ReceiverOpt, propertySymbol, LookupResultKind.Viable, propertySymbol.Type, boundFieldAccess.HasErrors);
                }
            }
            BoundExpression right = node.Right;
            VisitLValue(boundExpression);
            Unsplit();
            FlowAnalysisAnnotations lValueAnnotations = GetLValueAnnotations(boundExpression);
            TypeWithAnnotations typeWithAnnotations = ApplyLValueAnnotations(LvalueResultType, lValueAnnotations);
            if (boundExpression.Kind == BoundKind.EventAccess && ((BoundEventAccess)boundExpression).EventSymbol.IsWindowsRuntimeEvent)
            {
                VisitRvalue(right);
                SetNotNullResult(node);
            }
            else
            {
                TypeWithState rightState;
                if (!node.IsRef)
                {
                    bool flag = boundExpression is BoundDiscardExpression;
                    rightState = VisitOptionalImplicitConversion(right, flag ? default(TypeWithAnnotations) : typeWithAnnotations, UseLegacyWarnings(boundExpression, typeWithAnnotations), trackMembers: true, AssignmentKind.Assignment);
                }
                else
                {
                    rightState = VisitRefExpression(right, typeWithAnnotations);
                }
                CheckDisallowedNullAssignment(rightState, lValueAnnotations, right.Syntax.Location);
                AdjustSetValue(boundExpression, ref rightState);
                TrackNullableStateForAssignment(right, typeWithAnnotations, MakeSlot(boundExpression), rightState, MakeSlot(right));
                if (boundExpression is BoundDiscardExpression)
                {
                    TypeWithAnnotations lvalueType = rightState.ToTypeWithAnnotations(compilation);
                    SetResult(boundExpression, rightState, lvalueType, updateAnalyzedNullability: true, true);
                    SetResult(node, rightState, lvalueType);
                }
                else
                {
                    SetResult(node, TypeWithState.Create(typeWithAnnotations.Type, rightState.State), typeWithAnnotations);
                }
            }
            return null;
        }

        private bool IsPropertyOutputMoreStrictThanInput(PropertySymbol property)
        {
            TypeWithAnnotations typeWithAnnotations = property.TypeWithAnnotations;
            FlowAnalysisAnnotations flowAnalysisAnnotations = ((!IsAnalyzingAttribute) ? property.GetFlowAnalysisAnnotations() : FlowAnalysisAnnotations.None);
            TypeWithAnnotations typeWithAnnotations2 = ApplyLValueAnnotations(typeWithAnnotations, flowAnalysisAnnotations);
            if (typeWithAnnotations2.NullableAnnotation.IsOblivious() || !typeWithAnnotations2.CanBeAssignedNull)
            {
                return false;
            }
            return ApplyUnconditionalAnnotations(typeWithAnnotations.ToTypeWithState(), flowAnalysisAnnotations).IsNotNull;
        }

        private void AdjustSetValue(BoundExpression left, ref TypeWithState rightState)
        {
            PropertySymbol propertySymbol = ((left is BoundPropertyAccess boundPropertyAccess) ? boundPropertyAccess.PropertySymbol : ((!(left is BoundIndexerAccess boundIndexerAccess)) ? null : boundIndexerAccess.Indexer));
            PropertySymbol propertySymbol2 = propertySymbol;
            if ((object)propertySymbol2 != null && IsPropertyOutputMoreStrictThanInput(propertySymbol2))
            {
                rightState = rightState.WithNotNullState();
            }
        }

        private FlowAnalysisAnnotations GetLValueAnnotations(BoundExpression expr)
        {
            if (IsAnalyzingAttribute)
            {
                return FlowAnalysisAnnotations.None;
            }
            FlowAnalysisAnnotations flowAnalysisAnnotations;
            if (!(expr is BoundPropertyAccess boundPropertyAccess))
            {
                if (!(expr is BoundIndexerAccess boundIndexerAccess))
                {
                    if (!(expr is BoundFieldAccess boundFieldAccess))
                    {
                        if (!(expr is BoundObjectInitializerMember boundObjectInitializerMember))
                        {
                            if (expr is BoundParameter boundParameter)
                            {
                                ParameterSymbol parameterSymbol = boundParameter.ParameterSymbol;
                                if ((object)parameterSymbol != null)
                                {
                                    flowAnalysisAnnotations = ToInwardAnnotations(GetParameterAnnotations(parameterSymbol) & ~FlowAnalysisAnnotations.NotNull);
                                    goto IL_00cb;
                                }
                            }
                            goto IL_00c8;
                        }
                        Symbol memberSymbol = boundObjectInitializerMember.MemberSymbol;
                        if (!(memberSymbol is PropertySymbol property))
                        {
                            if (!(memberSymbol is FieldSymbol field2))
                            {
                                goto IL_00c8;
                            }
                            flowAnalysisAnnotations = getFieldAnnotations(field2);
                        }
                        else
                        {
                            flowAnalysisAnnotations = property.GetFlowAnalysisAnnotations();
                        }
                    }
                    else
                    {
                        flowAnalysisAnnotations = getFieldAnnotations(boundFieldAccess.FieldSymbol);
                    }
                }
                else
                {
                    flowAnalysisAnnotations = boundIndexerAccess.Indexer.GetFlowAnalysisAnnotations();
                }
            }
            else
            {
                flowAnalysisAnnotations = boundPropertyAccess.PropertySymbol.GetFlowAnalysisAnnotations();
            }
            goto IL_00cb;
        IL_00c8:
            flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            goto IL_00cb;
        IL_00cb:
            return flowAnalysisAnnotations & (FlowAnalysisAnnotations.AllowNull | FlowAnalysisAnnotations.DisallowNull);
            static FlowAnalysisAnnotations getFieldAnnotations(FieldSymbol field)
            {
                if (!(field.AssociatedSymbol is PropertySymbol property2))
                {
                    return field.FlowAnalysisAnnotations;
                }
                return property2.GetFlowAnalysisAnnotations();
            }
        }

        private static FlowAnalysisAnnotations ToInwardAnnotations(FlowAnalysisAnnotations outwardAnnotations)
        {
            FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
            if ((outwardAnnotations & FlowAnalysisAnnotations.MaybeNull) != 0)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
            }
            if ((outwardAnnotations & FlowAnalysisAnnotations.NotNull) == FlowAnalysisAnnotations.NotNull)
            {
                flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
            }
            return flowAnalysisAnnotations;
        }

        private static bool UseLegacyWarnings(BoundExpression expr, TypeWithAnnotations exprType)
        {
            return expr.Kind switch
            {
                BoundKind.Local => expr.GetRefKind() == RefKind.None,
                BoundKind.Parameter => ((BoundParameter)expr).ParameterSymbol.RefKind == RefKind.None,
                _ => false,
            };
        }

        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            return VisitDeconstructionAssignmentOperator(node, null);
        }

        private BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, TypeWithState? rightResultOpt)
        {
            bool disableNullabilityAnalysis = _disableNullabilityAnalysis;
            _disableNullabilityAnalysis = true;
            BoundTupleExpression left = node.Left;
            BoundConversion right = node.Right;
            ArrayBuilder<DeconstructionVariable> deconstructionAssignmentVariables = GetDeconstructionAssignmentVariables(left);
            if (node.HasErrors)
            {
                VisitRvalue(right.Operand);
            }
            else
            {
                VisitDeconstructionArguments(deconstructionAssignmentVariables, right.Conversion, right.Operand, rightResultOpt);
            }
            deconstructionAssignmentVariables.FreeAll((DeconstructionVariable v) => v.NestedVariables);
            SetNotNullResult(node);
            _disableNullabilityAnalysis = disableNullabilityAnalysis;
            return null;
        }

        private void VisitDeconstructionArguments(ArrayBuilder<DeconstructionVariable> variables, Conversion conversion, BoundExpression right, TypeWithState? rightResultOpt = null)
        {
            if (!conversion.DeconstructionInfo.IsDefault)
            {
                VisitDeconstructMethodArguments(variables, conversion, right, rightResultOpt);
            }
            else
            {
                VisitTupleDeconstructionArguments(variables, conversion.UnderlyingConversions, right);
            }
        }

        private void VisitDeconstructMethodArguments(ArrayBuilder<DeconstructionVariable> variables, Conversion conversion, BoundExpression right, TypeWithState? rightResultOpt)
        {
            VisitRvalue(right);
            if (rightResultOpt.HasValue)
            {
                SetResultType(right, rightResultOpt.Value);
            }
            TypeWithState resultType = ResultType;
            BoundCall boundCall = conversion.DeconstructionInfo.Invocation as BoundCall;
            MethodSymbol methodSymbol = boundCall?.Method;
            if ((object)methodSymbol == null)
            {
                return;
            }
            int count = variables.Count;
            if (!boundCall.InvokedAsExtensionMethod)
            {
                CheckPossibleNullReceiver(right);
                if (methodSymbol.OriginalDefinition != methodSymbol)
                {
                    methodSymbol = methodSymbol.OriginalDefinition.AsMember((NamedTypeSymbol)resultType.Type);
                }
            }
            else if (methodSymbol.IsGenericMethod)
            {
                ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(count + 1);
                instance.Add(CreatePlaceholderIfNecessary(right, resultType.ToTypeWithAnnotations(compilation)));
                for (int i = 0; i < count; i++)
                {
                    instance.Add(new BoundExpressionWithNullability(variables[i].Expression.Syntax, variables[i].Expression, NullableAnnotation.Oblivious, conversion.DeconstructionInfo.OutputPlaceholders[i].Type));
                }
                methodSymbol = InferMethodTypeArguments(methodSymbol, instance.ToImmutableAndFree(), boundCall.ArgumentRefKindsOpt, boundCall.ArgsToParamsOpt, boundCall.Expanded);
                if (ConstraintsHelper.RequiresChecking(methodSymbol))
                {
                    CheckMethodConstraints(boundCall.Syntax, methodSymbol);
                }
            }
            ImmutableArray<ParameterSymbol> parameters = methodSymbol.Parameters;
            int num = (boundCall.InvokedAsExtensionMethod ? 1 : 0);
            if (boundCall.InvokedAsExtensionMethod)
            {
                Conversion item = RemoveConversion(boundCall.Arguments[0], includeExplicitConversions: false).conversion;
                CheckExtensionMethodThisNullability(right, item, methodSymbol.Parameters[0], resultType);
            }
            for (int j = 0; j < count; j++)
            {
                DeconstructionVariable deconstructionVariable = variables[j];
                ParameterSymbol parameterSymbol = parameters[j + num];
                Conversion conversion2 = conversion.UnderlyingConversions[j];
                ArrayBuilder<DeconstructionVariable> nestedVariables = deconstructionVariable.NestedVariables;
                if (nestedVariables != null)
                {
                    BoundExpression right2 = CreatePlaceholderIfNecessary(boundCall.Arguments[j + num], parameterSymbol.TypeWithAnnotations);
                    VisitDeconstructionArguments(nestedVariables, conversion2, right2);
                }
                else
                {
                    VisitArgumentConversionAndInboundAssignmentsAndPreConditions(null, deconstructionVariable.Expression, conversion2, parameterSymbol.RefKind, parameterSymbol, parameterSymbol.TypeWithAnnotations, GetParameterAnnotations(parameterSymbol), new VisitArgumentResult(new VisitResult(deconstructionVariable.Type.ToTypeWithState(), deconstructionVariable.Type), default(Optional<LocalState>)), extensionMethodThisArgument: false);
                }
            }
            for (int k = 0; k < count; k++)
            {
                DeconstructionVariable deconstructionVariable2 = variables[k];
                ParameterSymbol parameterSymbol2 = parameters[k + num];
                if (deconstructionVariable2.NestedVariables == null)
                {
                    VisitArgumentOutboundAssignmentsAndPostConditions(deconstructionVariable2.Expression, parameterSymbol2.RefKind, parameterSymbol2, parameterSymbol2.TypeWithAnnotations, GetRValueAnnotations(parameterSymbol2), new VisitArgumentResult(new VisitResult(deconstructionVariable2.Type.ToTypeWithState(), deconstructionVariable2.Type), default(Optional<LocalState>)), null, default(CompareExchangeInfo));
                }
            }
        }

        private void VisitTupleDeconstructionArguments(ArrayBuilder<DeconstructionVariable> variables, ImmutableArray<Conversion> conversions, BoundExpression right)
        {
            int count = variables.Count;
            ImmutableArray<BoundExpression> deconstructionRightParts = GetDeconstructionRightParts(right);
            for (int i = 0; i < count; i++)
            {
                DeconstructionVariable deconstructionVariable = variables[i];
                Conversion conversion = conversions[i];
                BoundExpression boundExpression = deconstructionRightParts[i];
                ArrayBuilder<DeconstructionVariable> nestedVariables = deconstructionVariable.NestedVariables;
                if (nestedVariables != null)
                {
                    VisitDeconstructionArguments(nestedVariables, conversion, boundExpression);
                    continue;
                }
                TypeWithAnnotations type = deconstructionVariable.Type;
                FlowAnalysisAnnotations lValueAnnotations = GetLValueAnnotations(deconstructionVariable.Expression);
                type = ApplyLValueAnnotations(type, lValueAnnotations);
                TypeWithState rightState;
                TypeWithState operandType;
                int valueSlot;
                if (conversion.IsIdentity)
                {
                    if (deconstructionVariable.Expression is BoundLocal boundLocal && boundLocal.DeclarationKind == BoundLocalDeclarationKind.WithInferredType)
                    {
                        rightState = (operandType = VisitRvalueWithState(boundExpression));
                        _variables.SetType(boundLocal.LocalSymbol, operandType.ToAnnotatedTypeWithAnnotations(compilation));
                    }
                    else
                    {
                        operandType = default(TypeWithState);
                        rightState = VisitOptionalImplicitConversion(boundExpression, type, useLegacyWarnings: true, trackMembers: true, AssignmentKind.Assignment);
                    }
                    valueSlot = MakeSlot(boundExpression);
                }
                else
                {
                    operandType = VisitRvalueWithState(boundExpression);
                    rightState = VisitConversion(null, boundExpression, conversion, type, operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: true, AssignmentKind.Assignment);
                    valueSlot = -1;
                }
                CheckDisallowedNullAssignment(rightState, lValueAnnotations, right.Syntax.Location);
                int num = MakeSlot(deconstructionVariable.Expression);
                AdjustSetValue(deconstructionVariable.Expression, ref rightState);
                TrackNullableStateForAssignment(boundExpression, type, num, rightState, valueSlot);
                if (num > 0 && conversion.Kind == ConversionKind.ImplicitNullable && AreNullableAndUnderlyingTypes(type.Type, operandType.Type, out var underlyingTypeWithAnnotations))
                {
                    valueSlot = MakeSlot(boundExpression);
                    if (valueSlot > 0)
                    {
                        TypeWithState valueType = TypeWithState.Create(underlyingTypeWithAnnotations.Type, NullableFlowState.NotNull);
                        TrackNullableStateOfNullableValue(num, type.Type, boundExpression, valueType, valueSlot);
                    }
                }
            }
        }

        private ArrayBuilder<DeconstructionVariable> GetDeconstructionAssignmentVariables(BoundTupleExpression tuple)
        {
            ImmutableArray<BoundExpression> arguments = tuple.Arguments;
            ArrayBuilder<DeconstructionVariable> instance = ArrayBuilder<DeconstructionVariable>.GetInstance(arguments.Length);
            ImmutableArray<BoundExpression>.Enumerator enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                instance.Add(getDeconstructionAssignmentVariable(current));
            }
            return instance;
            DeconstructionVariable getDeconstructionAssignmentVariable(BoundExpression expr)
            {
                BoundKind kind = expr.Kind;
                if (kind - 148 <= BoundKind.PropertyEqualsValue)
                {
                    return new DeconstructionVariable(expr, GetDeconstructionAssignmentVariables((BoundTupleExpression)expr));
                }
                VisitLValue(expr);
                return new DeconstructionVariable(expr, LvalueResultType);
            }
        }

        private ImmutableArray<BoundExpression> GetDeconstructionRightParts(BoundExpression expr)
        {
            switch (expr.Kind)
            {
                case BoundKind.TupleLiteral:
                case BoundKind.ConvertedTupleLiteral:
                    return ((BoundTupleExpression)expr).Arguments;
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)expr;
                        ConversionKind conversionKind = boundConversion.ConversionKind;
                        if (conversionKind == ConversionKind.Identity || conversionKind == ConversionKind.ImplicitTupleLiteral)
                        {
                            return GetDeconstructionRightParts(boundConversion.Operand);
                        }
                        break;
                    }
            }
            if (expr.Type is NamedTypeSymbol namedTypeSymbol && namedTypeSymbol.IsTupleType)
            {
                return namedTypeSymbol.TupleElements.SelectAsArray((Func<FieldSymbol, BoundExpression, BoundExpression>)((FieldSymbol f, BoundExpression e) => new BoundFieldAccess(e.Syntax, e, f, null)), expr);
            }
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            TypeWithState typeWithState = VisitRvalueWithState(node.Operand);
            TypeWithAnnotations lvalueResultType = LvalueResultType;
            bool flag = false;
            object obj;
            if (State.Reachable)
            {
                if (node.OperatorKind.IsUserDefined())
                {
                    MethodSymbol? methodOpt = node.MethodOpt;
                    if ((object)methodOpt != null && methodOpt!.ParameterCount == 1)
                    {
                        obj = node.MethodOpt;
                        goto IL_0053;
                    }
                }
                obj = null;
                goto IL_0053;
            }
            goto IL_01bb;
        IL_00dc:
            TypeWithAnnotations targetTypeWithNullability;
            AssignmentKind assignmentKind;
            ParameterSymbol parameterOpt;
            TypeWithState typeWithState2 = ((!targetTypeWithNullability.HasType) ? typeWithState : VisitConversion(null, node.Operand, node.OperandConversion, targetTypeWithNullability, typeWithState, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, assignmentKind, parameterOpt));
            MethodSymbol methodSymbol;
            TypeWithState operandType = methodSymbol?.ReturnTypeWithAnnotations.ToTypeWithState() ?? typeWithState2;
            TypeWithAnnotations targetTypeWithNullability2 = typeWithState.ToTypeWithAnnotations(compilation);
            operandType = VisitConversion(null, node, node.ResultConversion, targetTypeWithNullability2, operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment);
            if (!node.HasErrors)
            {
                UnaryOperatorKind unaryOperatorKind = node.OperatorKind.Operator();
                TypeWithState type = ((unaryOperatorKind == UnaryOperatorKind.PrefixIncrement || unaryOperatorKind == UnaryOperatorKind.PrefixDecrement) ? operandType : typeWithState);
                SetResultType(node, type);
                flag = true;
                TrackNullableStateForAssignment(node, lvalueResultType, MakeSlot(node.Operand), operandType);
            }
            goto IL_01bb;
        IL_0053:
            methodSymbol = (MethodSymbol)obj;
            assignmentKind = AssignmentKind.Assignment;
            parameterOpt = null;
            if (node.OperandConversion.IsUserDefined)
            {
                MethodSymbol? method = node.OperandConversion.Method;
                if ((object)method != null && method!.ParameterCount == 1)
                {
                    targetTypeWithNullability = node.OperandConversion.Method!.ReturnTypeWithAnnotations;
                    goto IL_00dc;
                }
            }
            if ((object)methodSymbol != null)
            {
                targetTypeWithNullability = methodSymbol.Parameters[0].TypeWithAnnotations;
                assignmentKind = AssignmentKind.Argument;
                parameterOpt = methodSymbol.Parameters[0];
            }
            else
            {
                targetTypeWithNullability = default(TypeWithAnnotations);
            }
            goto IL_00dc;
        IL_01bb:
            if (!flag)
            {
                SetNotNullResult(node);
            }
            return null;
        }

        public override BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            BoundExpression left = node.Left;
            BoundExpression right = node.Right;
            Visit(left);
            TypeWithAnnotations typeWithAnnotations = LvalueResultType;
            TypeWithState resultType = ResultType;
            TypeWithState adjustedResult = GetAdjustedResult(resultType, MakeSlot(node.Left));
            adjustedResult = (((object)node.Operator.LeftType == null) ? default(TypeWithState) : VisitConversion(null, node.Left, node.LeftConversion, TypeWithAnnotations.Create(node.Operator.LeftType), adjustedResult, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment, null, reportTopLevelWarnings: false));
            TypeWithState rightType = VisitRvalueWithState(right);
            TypeWithState operandType;
            if ((object)node.Operator.ReturnType != null)
            {
                if (node.Operator.Kind.IsUserDefined() && (object)node.Operator.Method != null && node.Operator.Method.ParameterCount == 2)
                {
                    MethodSymbol method = node.Operator.Method;
                    VisitArguments(node, ImmutableArray.Create(node.Left, right), method.ParameterRefKinds, method.Parameters, default(ImmutableArray<int>), default(BitVector), expanded: true, invokedAsExtensionMethod: false, method);
                }
                operandType = InferResultNullability(node.Operator.Kind, node.Operator.Method, node.Operator.ReturnType, adjustedResult, rightType);
                FlowAnalysisAnnotations lValueAnnotations = GetLValueAnnotations(node.Left);
                typeWithAnnotations = ApplyLValueAnnotations(typeWithAnnotations, lValueAnnotations);
                operandType = VisitConversion(null, node, node.FinalConversion, typeWithAnnotations, operandType, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Assignment);
                CheckDisallowedNullAssignment(operandType, lValueAnnotations, node.Syntax.Location);
            }
            else
            {
                operandType = TypeWithState.Create(node.Type, NullableFlowState.NotNull);
            }
            AdjustSetValue(left, ref operandType);
            TrackNullableStateForAssignment(node, typeWithAnnotations, MakeSlot(node.Left), operandType);
            SetResultType(node, operandType);
            return null;
        }

        public override BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            BoundExpression boundExpression = node.Expression;
            if (boundExpression.Kind == BoundKind.AddressOfOperator)
            {
                boundExpression = ((BoundAddressOfOperator)boundExpression).Operand;
            }
            VisitRvalue(boundExpression);
            if (node.Expression.Kind == BoundKind.AddressOfOperator)
            {
                SetResultType(node.Expression, TypeWithState.Create(node.Expression.Type, ResultType.State));
            }
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            Visit(node.Operand);
            SetNotNullResult(node);
            return null;
        }

        private void ReportArgumentWarnings(BoundExpression argument, TypeWithState argumentType, ParameterSymbol parameter)
        {
            TypeWithAnnotations typeWithAnnotations = parameter.TypeWithAnnotations;
            ReportNullableAssignmentIfNecessary(argument, typeWithAnnotations, argumentType, useLegacyWarnings: false, AssignmentKind.Argument, parameter);
            TypeSymbol type = argumentType.Type;
            if ((object)type != null && IsNullabilityMismatch(typeWithAnnotations.Type, type))
            {
                ReportNullabilityMismatchInArgument(argument.Syntax, type, parameter, typeWithAnnotations.Type, forOutput: false);
            }
        }

        private void ReportNullabilityMismatchInRefArgument(BoundExpression argument, TypeSymbol argumentType, ParameterSymbol parameter, TypeSymbol parameterType)
        {
            ReportDiagnostic(ErrorCode.WRN_NullabilityMismatchInArgument, argument.Syntax, argumentType, parameterType, GetParameterAsDiagnosticArgument(parameter), GetContainingSymbolAsDiagnosticArgument(parameter));
        }

        private void ReportNullabilityMismatchInArgument(SyntaxNode argument, TypeSymbol argumentType, ParameterSymbol parameter, TypeSymbol parameterType, bool forOutput)
        {
            ReportNullabilityMismatchInArgument(argument.GetLocation(), argumentType, parameter, parameterType, forOutput);
        }

        private void ReportNullabilityMismatchInArgument(Location argumentLocation, TypeSymbol argumentType, ParameterSymbol? parameterOpt, TypeSymbol parameterType, bool forOutput)
        {
            ReportDiagnostic(forOutput ? ErrorCode.WRN_NullabilityMismatchInArgumentForOutput : ErrorCode.WRN_NullabilityMismatchInArgument, argumentLocation, argumentType, ((object)parameterOpt != null && parameterOpt!.Type.IsNonNullableValueType() && parameterType.IsNullableType()) ? parameterOpt!.Type : parameterType, GetParameterAsDiagnosticArgument(parameterOpt), GetContainingSymbolAsDiagnosticArgument(parameterOpt));
        }

        private TypeWithAnnotations GetDeclaredLocalResult(LocalSymbol local)
        {
            if (!_variables.TryGetType(local, out var type))
            {
                return local.TypeWithAnnotations;
            }
            return type;
        }

        private TypeWithAnnotations GetDeclaredParameterResult(ParameterSymbol parameter)
        {
            if (!_variables.TryGetType(parameter, out var type))
            {
                return parameter.TypeWithAnnotations;
            }
            return type;
        }

        public override BoundNode? VisitBaseReference(BoundBaseReference node)
        {
            VisitThisOrBaseReference(node);
            return null;
        }

        private void SplitIfBooleanConstant(BoundExpression node)
        {
            ConstantValue constantValue = node.ConstantValue;
            if ((object)constantValue != null && constantValue.IsBoolean)
            {
                bool booleanValue = constantValue.BooleanValue;
                Split();
                if (booleanValue)
                {
                    StateWhenFalse = UnreachableState();
                }
                else
                {
                    StateWhenTrue = UnreachableState();
                }
            }
        }

        public override BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            Symbol updatedSymbol = VisitMemberAccess(node, node.ReceiverOpt, node.FieldSymbol);
            SplitIfBooleanConstant(node);
            SetUpdatedSymbol(node, node.FieldSymbol, updatedSymbol);
            return null;
        }

        public override BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol propertySymbol = node.PropertySymbol;
            Symbol updatedSymbol = VisitMemberAccess(node, node.ReceiverOpt, propertySymbol);
            if (!IsAnalyzingAttribute)
            {
                if (_expressionIsRead)
                {
                    ApplyMemberPostConditions(node.ReceiverOpt, propertySymbol.GetMethod);
                }
                else
                {
                    ApplyMemberPostConditions(node.ReceiverOpt, propertySymbol.SetMethod);
                }
            }
            SetUpdatedSymbol(node, propertySymbol, updatedSymbol);
            return null;
        }

        public override BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            BoundExpression receiverOpt = node.ReceiverOpt;
            TypeSymbol type = VisitRvalueWithState(receiverOpt).Type;
            CheckPossibleNullReceiver(receiverOpt);
            PropertySymbol propertySymbol = node.Indexer;
            if ((object)type != null)
            {
                propertySymbol = (PropertySymbol)AsMemberOfType(type, propertySymbol);
            }
            VisitArguments(node, node.Arguments, node.ArgumentRefKindsOpt, propertySymbol, node.ArgsToParamsOpt, node.DefaultArguments, node.Expanded);
            TypeWithState resultType = ApplyUnconditionalAnnotations(propertySymbol.TypeWithAnnotations.ToTypeWithState(), GetRValueAnnotations(propertySymbol));
            SetResult(node, resultType, propertySymbol.TypeWithAnnotations);
            SetUpdatedSymbol(node, node.Indexer, propertySymbol);
            return null;
        }

        public override BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            BoundExpression receiver = node.Receiver;
            TypeSymbol type = VisitRvalueWithState(receiver).Type;
            CheckPossibleNullReceiver(receiver);
            VisitRvalue(node.Argument);
            Symbol symbol = node.PatternSymbol;
            if ((object)type != null)
            {
                symbol = AsMemberOfType(type, symbol);
            }
            SetLvalueResultType(node, symbol.GetTypeOrReturnType());
            SetUpdatedSymbol(node, node.PatternSymbol, symbol);
            return null;
        }

        public override BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            Symbol updatedSymbol = VisitMemberAccess(node, node.ReceiverOpt, node.EventSymbol);
            SetUpdatedSymbol(node, node.EventSymbol, updatedSymbol);
            return null;
        }

        private Symbol VisitMemberAccess(BoundExpression node, BoundExpression? receiverOpt, Symbol member)
        {
            TypeWithState typeWithState = ((receiverOpt != null) ? VisitRvalueWithState(receiverOpt) : default(TypeWithState));
            SpecialMember? specialMember = null;
            if (member.RequiresInstanceReceiver())
            {
                member = AsMemberOfType(typeWithState.Type, member);
                specialMember = GetNullableOfTMember(member);
                bool flag = specialMember != SpecialMember.System_Nullable_T_get_Value;
                CheckPossibleNullReceiver(receiverOpt, !flag);
            }
            TypeWithAnnotations typeOrReturnType = member.GetTypeOrReturnType();
            FlowAnalysisAnnotations rValueAnnotations = GetRValueAnnotations(member);
            TypeWithState resultType = ApplyUnconditionalAnnotations(typeOrReturnType.ToTypeWithState(), rValueAnnotations);
            if (PossiblyNullableType(resultType.Type))
            {
                int slot = MakeMemberSlot(receiverOpt, member);
                if (State.HasValue(slot))
                {
                    NullableFlowState defaultState = State[slot];
                    resultType = TypeWithState.Create(resultType.Type, defaultState);
                }
            }
            if (specialMember == SpecialMember.System_Nullable_T_get_HasValue && receiverOpt != null)
            {
                int num = MakeSlot(receiverOpt);
                if (num > 0)
                {
                    Split();
                    StateWhenTrue[num] = NullableFlowState.NotNull;
                }
            }
            SetResult(node, resultType, typeOrReturnType);
            return member;
        }

        private SpecialMember? GetNullableOfTMember(Symbol member)
        {
            if (member.Kind == SymbolKind.Property)
            {
                MethodSymbol getMethod = ((PropertySymbol)member.OriginalDefinition).GetMethod;
                if ((object)getMethod != null && getMethod.ContainingType.SpecialType == SpecialType.System_Nullable_T)
                {
                    if (getMethod == compilation.GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_Value))
                    {
                        return SpecialMember.System_Nullable_T_get_Value;
                    }
                    if (getMethod == compilation.GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_HasValue))
                    {
                        return SpecialMember.System_Nullable_T_get_HasValue;
                    }
                }
            }
            return null;
        }

        private int GetNullableOfTValueSlot(TypeSymbol containingType, int containingSlot, out Symbol? valueProperty, bool forceSlotEvenIfEmpty = false)
        {
            valueProperty = ((MethodSymbol)compilation.GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_Value))?.AsMember((NamedTypeSymbol)containingType)?.AssociatedSymbol;
            if ((object)valueProperty != null)
            {
                return GetOrCreateSlot(valueProperty, containingSlot, forceSlotEvenIfEmpty);
            }
            return -1;
        }

        protected unsafe override void VisitForEachExpression(BoundForEachStatement node)
        {
            if (node.Expression.Kind != BoundKind.Conversion)
            {
                // If we're in this scenario, there was a binding error, and we should suppress any further warnings.
                VisitRvalue(node.Expression);
                return;
            }

            var (expr, conversion) = RemoveConversion(node.Expression, includeExplicitConversions: false);
            SnapshotWalkerThroughConversionGroup(node.Expression, expr);

            // There are 7 ways that a foreach can be created:
            //    1. The collection type is an array type. For this, initial binding will generate an implicit reference conversion to
            //       IEnumerable, and we do not need to do any reinferring of enumerators here.
            //    2. The collection type is dynamic. For this we do the same as 1.
            //    3. The collection type implements the GetEnumerator pattern. For this, there is an identity conversion. Because
            //       this identity conversion uses nested types from initial binding, we cannot trust them and must instead use
            //       the type of the expression returned from VisitResult to reinfer the enumerator information.
            //    4. The collection type implements IEnumerable<T>. Only a few cases can hit this without being caught by number 3,
            //       such as a type with a private implementation of IEnumerable<T>, or a type parameter constrained to that type.
            //       In these cases, there will be an implicit conversion to IEnumerable<T>, but this will use types from
            //       initial binding. For this scenario, we need to look through the list of implemented interfaces on the type and
            //       find the version of IEnumerable<T> that it has after nullable analysis, as type substitution could have changed
            //       nested nullability of type parameters. See ForEach_22 for a concrete example of this.
            //    5. The collection type implements IEnumerable (non-generic). Because this version isn't generic, we don't need to
            //       do any reinference, and the existing conversion can stand as is.
            //    6. The target framework's System.String doesn't implement IEnumerable. This is a compat case: System.String normally
            //       does implement IEnumerable, but there are certain target frameworks where this isn't the case. The compiler will
            //       still emit code for foreach in these scenarios.
            //    7. The collection type implements the GetEnumerator pattern via an extension GetEnumerator. For this, there will be 
            //       conversion to the parameter of the extension method.
            //    8. Some binding error occurred, and some other error has already been reported. Usually this doesn't have any kind
            //       of conversion on top, but if there was an explicit conversion in code then we could get past the initial check
            //       for a BoundConversion node.

            var resultTypeWithState = VisitRvalueWithState(expr);
            var resultType = resultTypeWithState.Type;

            SetAnalyzedNullability(expr, _visitResult);
            TypeWithAnnotations targetTypeWithAnnotations;

            MethodSymbol? reinferredGetEnumeratorMethod = null;

            if (node.EnumeratorInfoOpt?.GetEnumeratorInfo is { Method: { IsExtensionMethod: true, Parameters: var parameters } } enumeratorMethodInfo)
            {
                // this is case 7
                // We do not need to do this same analysis for non-extension methods because they do not have generic parameters that
                // can be inferred from usage like extension methods can. We don't warn about default arguments at the call site, so
                // there's nothing that can be learned from the non-extension case.
                var (method, results, _) = VisitArguments(
                    node,
                    enumeratorMethodInfo.Arguments,
                    refKindsOpt: default,
                    parameters,
                    argsToParamsOpt: enumeratorMethodInfo.ArgsToParamsOpt,
                    defaultArguments: enumeratorMethodInfo.DefaultArguments,
                    expanded: false,
                    invokedAsExtensionMethod: true,
                    enumeratorMethodInfo.Method);

                targetTypeWithAnnotations = results[0].LValueType;
                reinferredGetEnumeratorMethod = method;
            }
            else if (conversion.IsIdentity ||
                (conversion.Kind == ConversionKind.ExplicitReference && resultType.SpecialType == SpecialType.System_String))
            {
                // This is case 3 or 6.
                targetTypeWithAnnotations = resultTypeWithState.ToTypeWithAnnotations(compilation);
            }
            else if (conversion.IsImplicit)
            {
                bool isAsync = node.AwaitOpt != null;
                if (node.Expression.Type!.SpecialType == SpecialType.System_Collections_IEnumerable)
                {
                    // If this is a conversion to IEnumerable (non-generic), nothing to do. This is cases 1, 2, and 5.
                    targetTypeWithAnnotations = TypeWithAnnotations.Create(node.Expression.Type);
                }
                else if (ForEachLoopBinder.IsIEnumerableT(node.Expression.Type.OriginalDefinition, isAsync, compilation))
                {
                    // This is case 4. We need to look for the IEnumerable<T> that this reinferred expression implements,
                    // so that we pick up any nested type substitutions that could have occurred.
                    var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    targetTypeWithAnnotations = TypeWithAnnotations.Create(ForEachLoopBinder.GetIEnumerableOfT(resultType, isAsync, compilation, ref discardedUseSiteInfo, out bool foundMultiple));
                }
                else
                {
                    // This is case 8. There was not a successful binding, as a successful binding will _always_ generate one of the
                    // above conversions. Just return, as we want to suppress further errors.
                    return;
                }
            }
            else
            {
                // This is also case 8.
                return;
            }

            var convertedResult = VisitConversion(
                GetConversionIfApplicable(node.Expression, expr),
                expr,
                conversion,
                targetTypeWithAnnotations,
                resultTypeWithState,
                checkConversion: true,
                fromExplicitCast: false,
                useLegacyWarnings: false,
                AssignmentKind.Assignment);

            bool reportedDiagnostic = node.EnumeratorInfoOpt?.GetEnumeratorInfo.Method is { IsExtensionMethod: true }
                ? false
                : CheckPossibleNullReceiver(expr);

            SetAnalyzedNullability(node.Expression, new VisitResult(convertedResult, convertedResult.ToTypeWithAnnotations(compilation)));

            TypeWithState currentPropertyGetterTypeWithState;

            if (node.EnumeratorInfoOpt is null)
            {
                currentPropertyGetterTypeWithState = default;
            }
            else if (resultType is ArrayTypeSymbol arrayType)
            {
                // Even though arrays use the IEnumerator pattern, we use the array element type as the foreach target type, so
                // directly get our source type from there instead of doing method reinference.
                currentPropertyGetterTypeWithState = arrayType.ElementTypeWithAnnotations.ToTypeWithState();
            }
            else if (resultType.SpecialType == SpecialType.System_String)
            {
                // There are frameworks where System.String does not implement IEnumerable, but we still lower it to a for loop
                // using the indexer over the individual characters anyway. So the type must be not annotated char.
                currentPropertyGetterTypeWithState =
                    TypeWithAnnotations.Create(node.EnumeratorInfoOpt.ElementType, NullableAnnotation.NotAnnotated).ToTypeWithState();
            }
            else
            {
                // Reinfer the return type of the node.Expression.GetEnumerator().Current property, so that if
                // the collection changed nested generic types we pick up those changes.
                reinferredGetEnumeratorMethod ??= (MethodSymbol)AsMemberOfType(convertedResult.Type, node.EnumeratorInfoOpt.GetEnumeratorInfo.Method);
                var enumeratorReturnType = GetReturnTypeWithState(reinferredGetEnumeratorMethod);

                if (enumeratorReturnType.State != NullableFlowState.NotNull)
                {
                    if (!reportedDiagnostic && !(node.Expression is BoundConversion { Operand: { IsSuppressed: true } }))
                    {
                        ReportDiagnostic(ErrorCode.WRN_NullReferenceReceiver, expr.Syntax.GetLocation());
                    }
                }

                var currentPropertyGetter = (MethodSymbol)AsMemberOfType(enumeratorReturnType.Type, node.EnumeratorInfoOpt.CurrentPropertyGetter);

                currentPropertyGetterTypeWithState = ApplyUnconditionalAnnotations(
                    currentPropertyGetter.ReturnTypeWithAnnotations.ToTypeWithState(),
                    currentPropertyGetter.ReturnTypeFlowAnalysisAnnotations);

                // Analyze `await MoveNextAsync()`
                if (node.AwaitOpt is { AwaitableInstancePlaceholder: BoundAwaitableValuePlaceholder moveNextPlaceholder } awaitMoveNextInfo)
                {
                    var moveNextAsyncMethod = (MethodSymbol)AsMemberOfType(reinferredGetEnumeratorMethod.ReturnType, node.EnumeratorInfoOpt.MoveNextInfo.Method);

                    EnsureAwaitablePlaceholdersInitialized();
                    var result = new VisitResult(GetReturnTypeWithState(moveNextAsyncMethod), moveNextAsyncMethod.ReturnTypeWithAnnotations);
                    _awaitablePlaceholdersOpt.Add(moveNextPlaceholder, (moveNextPlaceholder, result));
                    Visit(awaitMoveNextInfo);
                    _awaitablePlaceholdersOpt.Remove(moveNextPlaceholder);
                }

                // Analyze `await DisposeAsync()`
                if (node.EnumeratorInfoOpt is { NeedsDisposal: true, DisposeAwaitableInfo: BoundAwaitableInfo awaitDisposalInfo })
                {
                    var disposalPlaceholder = awaitDisposalInfo.AwaitableInstancePlaceholder;
                    bool addedPlaceholder = false;
                    if (node.EnumeratorInfoOpt.PatternDisposeInfo is { Method: var originalDisposeMethod }) // no statically known Dispose method if doing a runtime check
                    {
                        var disposeAsyncMethod = (MethodSymbol)AsMemberOfType(reinferredGetEnumeratorMethod.ReturnType, originalDisposeMethod);
                        EnsureAwaitablePlaceholdersInitialized();
                        var result = new VisitResult(GetReturnTypeWithState(disposeAsyncMethod), disposeAsyncMethod.ReturnTypeWithAnnotations);
                        _awaitablePlaceholdersOpt.Add(disposalPlaceholder, (disposalPlaceholder, result));
                        addedPlaceholder = true;
                    }

                    Visit(awaitDisposalInfo);

                    if (addedPlaceholder)
                    {
                        _awaitablePlaceholdersOpt!.Remove(disposalPlaceholder!);
                    }
                }
            }

            SetResultType(expression: null, currentPropertyGetterTypeWithState);
        }

        public override void VisitForEachIterationVariables(BoundForEachStatement node)
        {
            TypeWithState typeWithState = ((node.EnumeratorInfoOpt == null) ? default(TypeWithState) : ResultType);
            TypeWithAnnotations typeWithAnnotations = typeWithState.ToTypeWithAnnotations(compilation);
            SyntaxNode syntax = node.Syntax;
            Location location;
            if (!(syntax is ForEachStatementSyntax forEachStatementSyntax))
            {
                if (!(syntax is ForEachVariableStatementSyntax forEachVariableStatementSyntax))
                {
                    throw ExceptionUtilities.UnexpectedValue(node.Syntax);
                }
                location = forEachVariableStatementSyntax.Variable.GetLocation();
            }
            else
            {
                location = forEachStatementSyntax.Identifier.GetLocation();
            }
            Location location2 = location;
            if (node.DeconstructionOpt != null)
            {
                BoundDeconstructionAssignmentOperator deconstructionAssignment = node.DeconstructionOpt!.DeconstructionAssignment;
                VisitDeconstructionAssignmentOperator(deconstructionAssignment, typeWithState.HasNullType ? null : new TypeWithState?(typeWithState));
                Visit(node.IterationVariableType);
                return;
            }
            Visit(node.IterationVariableType);
            ImmutableArray<LocalSymbol>.Enumerator enumerator = node.IterationVariables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                NullableFlowState value = NullableFlowState.NotNull;
                if (!typeWithState.HasNullType)
                {
                    TypeWithAnnotations typeWithAnnotations2 = current.TypeWithAnnotations;
                    TypeWithState typeWithState2 = typeWithState;
                    TypeWithState rValueType = typeWithState;
                    if (current.IsRef)
                    {
                        if (IsNullabilityMismatch(typeWithAnnotations, typeWithAnnotations2))
                        {
                            ForEachStatementSyntax forEachStatementSyntax2 = (ForEachStatementSyntax)node.Syntax;
                            ReportNullabilityMismatchInAssignment(forEachStatementSyntax2.Type, typeWithAnnotations, typeWithAnnotations2);
                        }
                    }
                    else if (current is SourceLocalSymbol sourceLocalSymbol && sourceLocalSymbol.IsVar)
                    {
                        typeWithAnnotations2 = typeWithState.ToAnnotatedTypeWithAnnotations(compilation);
                        _variables.SetType(current, typeWithAnnotations2);
                        rValueType = typeWithAnnotations2.ToTypeWithState();
                    }
                    else
                    {
                        CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                        Conversion conversion = ((node.ElementConversion.Kind == ConversionKind.UnsetConversionKind) ? _conversions.ClassifyImplicitConversionFromType(typeWithAnnotations.Type, typeWithAnnotations2.Type, ref useSiteInfo) : node.ElementConversion);
                        BoundTypeExpression iterationVariableType = node.IterationVariableType;
                        Conversion conversion2 = conversion;
                        TypeWithAnnotations targetTypeWithNullability = typeWithAnnotations2;
                        TypeWithState operandType = typeWithState;
                        bool fromExplicitCast = !conversion.IsImplicit;
                        location = location2;
                        typeWithState2 = VisitConversion(null, iterationVariableType, conversion2, targetTypeWithNullability, operandType, checkConversion: true, fromExplicitCast, useLegacyWarnings: true, AssignmentKind.ForEachIterationVariable, null, reportTopLevelWarnings: true, reportRemainingWarnings: true, extensionMethodThisArgument: false, default(Optional<LocalState>), trackMembers: false, location);
                    }
                    SetAnalyzedNullability(node.IterationVariableType, new VisitResult(rValueType, typeWithAnnotations2), true);
                    value = typeWithState2.State;
                }
                int orCreateSlot = GetOrCreateSlot(current);
                if (orCreateSlot > 0)
                {
                    State[orCreateSlot] = value;
                }
            }
        }

        public override BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            BoundNode result = base.VisitFromEndIndexExpression(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            ImmutableArray<BoundExpression>.Enumerator enumerator = node.ChildBoundNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current is BoundLambda node2)
                {
                    TakeIncrementalSnapshot(node2);
                    VisitLambda(node2, null);
                    VisitRvalueEpilogue(node2);
                }
                else
                {
                    VisitRvalue(current);
                }
            }
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type);
            return null;
        }

        public override BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            BoundNode result = base.VisitTypeExpression(node);
            if (node.BoundContainingTypeOpt != null)
            {
                VisitTypeExpression(node.BoundContainingTypeOpt);
            }
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            BoundNode result = base.VisitTypeOrValueExpression(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            BoundUnaryOperator node2 = node;
            TypeWithState type;
            switch (node2.OperatorKind)
            {
                case UnaryOperatorKind.BoolLogicalNegation:
                    Visit(node2.Operand);
                    if (IsConditionalState)
                    {
                        SetConditionalState(StateWhenFalse, StateWhenTrue);
                    }
                    type = adjustForLifting(ResultType);
                    break;
                case UnaryOperatorKind.DynamicTrue:
                    Visit(node2.Operand);
                    type = adjustForLifting(ResultType);
                    break;
                case UnaryOperatorKind.DynamicLogicalNegation:
                    Visit(node2.Operand);
                    if (IsConditionalState)
                    {
                        SetConditionalState(StateWhenFalse, StateWhenTrue);
                    }
                    type = adjustForLifting(ResultType);
                    break;
                default:
                    if (node2.OperatorKind.IsUserDefined())
                    {
                        MethodSymbol methodOpt = node2.MethodOpt;
                        if ((object)methodOpt != null && methodOpt.ParameterCount == 1)
                        {
                            var (boundExpression, conversion) = RemoveConversion(node2.Operand, includeExplicitConversions: false);
                            VisitRvalue(boundExpression);
                            TypeWithState resultType = ResultType;
                            bool isLifted = node2.OperatorKind.IsLifted();
                            TypeWithState nullableUnderlyingTypeIfNecessary = GetNullableUnderlyingTypeIfNecessary(isLifted, resultType);
                            methodOpt = (MethodSymbol)AsMemberOfType(nullableUnderlyingTypeIfNecessary.Type.StrippedType(), methodOpt);
                            ParameterSymbol parameterSymbol = methodOpt.Parameters[0];
                            VisitConversion(node2.Operand as BoundConversion, boundExpression, conversion, parameterSymbol.TypeWithAnnotations, nullableUnderlyingTypeIfNecessary, checkConversion: true, fromExplicitCast: false, useLegacyWarnings: false, AssignmentKind.Argument, parameterSymbol);
                            type = GetLiftedReturnTypeIfNecessary(isLifted, methodOpt.ReturnTypeWithAnnotations, resultType.State);
                            SetUpdatedSymbol(node2, node2.MethodOpt, methodOpt);
                            break;
                        }
                    }
                    VisitRvalue(node2.Operand);
                    type = adjustForLifting(ResultType);
                    break;
            }
            SetResultType(node2, type);
            return null;
            TypeWithState adjustForLifting(TypeWithState argumentResult)
            {
                return TypeWithState.Create(node2.Type, node2.OperatorKind.IsLifted() ? argumentResult.State : NullableFlowState.NotNull);
            }
        }

        public override BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            BoundNode result = base.VisitPointerIndirectionOperator(node);
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type);
            return result;
        }

        public override BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            BoundNode result = base.VisitPointerElementAccess(node);
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type);
            return result;
        }

        public override BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            VisitRvalue(node.Operand);
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            BoundNode result = base.VisitMakeRefOperator(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            BoundNode result = base.VisitRefValueOperator(node);
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type, node.NullableAnnotation);
            SetLvalueResultType(node, type);
            return result;
        }

        private TypeWithState InferResultNullability(BoundUserDefinedConditionalLogicalOperator node)
        {
            if (node.OperatorKind.IsLifted())
            {
                return TypeWithState.Create(node.Type, NullableFlowState.NotNull);
            }
            if ((object)node.LogicalOperator != null && node.LogicalOperator.ParameterCount == 2)
            {
                return GetReturnTypeWithState(node.LogicalOperator);
            }
            return default(TypeWithState);
        }

        protected override void AfterLeftChildOfBinaryLogicalOperatorHasBeenVisited(BoundExpression node, BoundExpression right, bool isAnd, bool isBool, ref LocalState leftTrue, ref LocalState leftFalse)
        {
            TypeWithState resultType = ResultType;
            MethodSymbol methodSymbol = null;
            MethodSymbol methodSymbol2 = null;
            BoundExpression argument = null;
            switch (node.Kind)
            {
                case BoundKind.UserDefinedConditionalLogicalOperator:
                    {
                        BoundUserDefinedConditionalLogicalOperator boundUserDefinedConditionalLogicalOperator = (BoundUserDefinedConditionalLogicalOperator)node;
                        if (boundUserDefinedConditionalLogicalOperator.LogicalOperator != null && boundUserDefinedConditionalLogicalOperator.LogicalOperator.ParameterCount == 2)
                        {
                            methodSymbol = boundUserDefinedConditionalLogicalOperator.LogicalOperator;
                            argument = boundUserDefinedConditionalLogicalOperator.Left;
                            methodSymbol2 = (isAnd ? boundUserDefinedConditionalLogicalOperator.FalseOperator : boundUserDefinedConditionalLogicalOperator.TrueOperator);
                            if ((object)methodSymbol2 != null && methodSymbol2.ParameterCount != 1)
                            {
                                methodSymbol2 = null;
                            }
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(node.Kind);
                case BoundKind.BinaryOperator:
                    break;
            }
            if ((object)methodSymbol2 != null)
            {
                ReportArgumentWarnings(argument, resultType, methodSymbol2.Parameters[0]);
            }
            if ((object)methodSymbol != null)
            {
                ReportArgumentWarnings(argument, resultType, methodSymbol.Parameters[0]);
            }
            Visit(right);
            TypeWithState resultType2 = ResultType;
            SetResultType(node, InferResultNullabilityOfBinaryLogicalOperator(node, resultType, resultType2));
            if ((object)methodSymbol != null)
            {
                ReportArgumentWarnings(right, resultType2, methodSymbol.Parameters[1]);
            }
            AfterRightChildOfBinaryLogicalOperatorHasBeenVisited(node, right, isAnd, isBool, ref leftTrue, ref leftFalse);
        }

        private TypeWithState InferResultNullabilityOfBinaryLogicalOperator(BoundExpression node, TypeWithState leftType, TypeWithState rightType)
        {
            if (!(node is BoundBinaryOperator boundBinaryOperator))
            {
                if (node is BoundUserDefinedConditionalLogicalOperator node2)
                {
                    return InferResultNullability(node2);
                }
                throw ExceptionUtilities.UnexpectedValue(node);
            }
            return InferResultNullability(boundBinaryOperator.OperatorKind, boundBinaryOperator.MethodOpt, boundBinaryOperator.Type, leftType, rightType);
        }

        public override BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            BoundNode result = base.VisitAwaitExpression(node);
            BoundAwaitableInfo awaitableInfo = node.AwaitableInfo;
            BoundAwaitableValuePlaceholder awaitableInstancePlaceholder = awaitableInfo.AwaitableInstancePlaceholder;
            EnsureAwaitablePlaceholdersInitialized();
            _awaitablePlaceholdersOpt!.Add(awaitableInstancePlaceholder, (node.Expression, _visitResult));
            Visit(awaitableInfo);
            _awaitablePlaceholdersOpt!.Remove(awaitableInstancePlaceholder);
            if (node.Type.IsValueType || node.HasErrors || (object)awaitableInfo.GetResult == null)
            {
                SetNotNullResult(node);
                return result;
            }
            MethodSymbol getResult = awaitableInfo.GetResult;
            MethodSymbol methodSymbol = ((_visitResult.RValueType.Type is NamedTypeSymbol newOwner) ? getResult.OriginalDefinition.AsMember(newOwner) : getResult);
            SetResultType(node, methodSymbol.ReturnTypeWithAnnotations.ToTypeWithState());
            return result;
        }

        public override BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            BoundNode result = base.VisitTypeOfOperator(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
            return result;
        }

        public override BoundNode? VisitMethodInfo(BoundMethodInfo node)
        {
            BoundNode result = base.VisitMethodInfo(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitFieldInfo(BoundFieldInfo node)
        {
            BoundNode result = base.VisitFieldInfo(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            BoundNode result = base.VisitDefaultLiteral(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.MaybeDefault));
            return result;
        }

        public override BoundNode? VisitDefaultExpression(BoundDefaultExpression node)
        {
            BoundNode result = base.VisitDefaultExpression(node);
            TypeSymbol type = node.Type;
            if (EmptyStructTypeCache.IsTrackableStructType(type))
            {
                int orCreatePlaceholderSlot = GetOrCreatePlaceholderSlot(node);
                if (orCreatePlaceholderSlot > 0)
                {
                    State[orCreatePlaceholderSlot] = NullableFlowState.NotNull;
                    InheritNullableStateOfTrackableStruct(type, orCreatePlaceholderSlot, -1, isDefaultValue: true);
                }
            }
            SetResultType(node, TypeWithState.ForType(type));
            return result;
        }

        public override BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            BoundExpression operand = node.Operand;
            BoundTypeExpression targetType = node.TargetType;
            BoundNode result = base.VisitIsOperator(node);
            Split();
            LearnFromNonNullTest(operand, ref StateWhenTrue);
            TypeSymbol type = targetType.Type;
            if ((object)type != null && type.SpecialType == SpecialType.System_Object)
            {
                LearnFromNullTest(operand, ref StateWhenFalse);
            }
            VisitTypeExpression(targetType);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            TypeWithState typeWithState = VisitRvalueWithState(node.Operand);
            NullableFlowState defaultState = NullableFlowState.NotNull;
            TypeSymbol type = node.Type;
            if (type.CanContainNull())
            {
                ConversionKind kind = node.Conversion.Kind;
                defaultState = ((kind != ConversionKind.Identity && kind != ConversionKind.ImplicitNullable && kind - 12 > ConversionKind.NoConversion) ? NullableFlowState.MaybeDefault : typeWithState.State);
            }
            VisitTypeExpression(node.TargetType);
            SetResultType(node, TypeWithState.Create(type, defaultState));
            return null;
        }

        public override BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            BoundNode result = base.VisitSizeOfOperator(node);
            VisitTypeExpression(node.SourceType);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitArgList(BoundArgList node)
        {
            BoundNode result = base.VisitArgList(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            VisitArgumentsEvaluate(node.Syntax, node.Arguments, node.ArgumentRefKindsOpt, default(ImmutableArray<ParameterSymbol>), default(ImmutableArray<int>), default(BitVector), expanded: false);
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitLiteral(BoundLiteral node)
        {
            BoundNode result = base.VisitLiteral(node);
            TypeSymbol? type = node.Type;
            TypeSymbol? type2 = node.Type;
            int defaultState;
            if ((object)type2 == null || type2.CanContainNull())
            {
                ConstantValue? constantValue = node.ConstantValue;
                if ((object)constantValue != null && constantValue!.IsNull)
                {
                    defaultState = 3;
                    goto IL_003b;
                }
            }
            defaultState = 0;
            goto IL_003b;
        IL_003b:
            SetResultType(node, TypeWithState.Create(type, (NullableFlowState)defaultState));
            SplitIfBooleanConstant(node);
            return result;
        }

        public override BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            BoundNode result = base.VisitPreviousSubmissionReference(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            BoundNode result = base.VisitHostObjectMemberReference(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitPseudoVariable(BoundPseudoVariable node)
        {
            BoundNode? result = base.VisitPseudoVariable(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            BoundNode result = base.VisitRangeExpression(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            VisitWithoutDiagnostics(node.Value);
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitLabel(BoundLabel node)
        {
            BoundNode? result = base.VisitLabel(node);
            SetUnknownResultNullability(node);
            return result;
        }

        public override BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            BoundExpression receiver = node.Receiver;
            VisitRvalue(receiver);
            CheckPossibleNullReceiver(receiver);
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type);
            return null;
        }

        public override BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            BoundExpression expression = node.Expression;
            VisitRvalue(expression);
            BoundExpression receiverOpt = (expression as BoundMethodGroup)?.ReceiverOpt;
            if (TryGetMethodGroupReceiverNullability(receiverOpt, out var type))
            {
                CheckPossibleNullReceiver(receiverOpt, type, checkNullableValueType: false);
            }
            VisitArgumentsEvaluate(node.Syntax, node.Arguments, node.ArgumentRefKindsOpt, default(ImmutableArray<ParameterSymbol>), default(ImmutableArray<int>), default(BitVector), expanded: false);
            TypeWithAnnotations type2 = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type2);
            return null;
        }

        public override BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            BoundExpression receiverOpt = node.ReceiverOpt;
            VisitRvalue(receiverOpt);
            EventSymbol eventSymbol = node.Event;
            if (!eventSymbol.IsStatic)
            {
                eventSymbol = (EventSymbol)AsMemberOfType(ResultType.Type, eventSymbol);
                CheckPossibleNullReceiver(receiverOpt);
                SetUpdatedSymbol(node, node.Event, eventSymbol);
            }
            VisitRvalue(node.Argument);
            ConstantValue? constantValue = node.Argument.ConstantValue;
            if ((object)constantValue == null || !constantValue!.IsNull)
            {
                int num = MakeMemberSlot(receiverOpt, eventSymbol);
                if (num > 0)
                {
                    State[num] = ((!node.IsAddition) ? NullableFlowState.MaybeNull : State[num].Meet(ResultType.State));
                }
            }
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = node.Arguments;
            ImmutableArray<VisitArgumentResult> argumentResults = VisitArgumentsEvaluate(node.Syntax, arguments, node.ArgumentRefKindsOpt, default(ImmutableArray<ParameterSymbol>), default(ImmutableArray<int>), default(BitVector), expanded: false);
            VisitObjectOrDynamicObjectCreation(node, arguments, argumentResults, node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            BoundNode result = base.VisitImplicitReceiver(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            BoundNode result = base.VisitNoPiaObjectCreationExpression(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
            return result;
        }

        public override BoundNode? VisitNewT(BoundNewT node)
        {
            VisitObjectOrDynamicObjectCreation(node, ImmutableArray<BoundExpression>.Empty, ImmutableArray<VisitArgumentResult>.Empty, node.InitializerExpressionOpt);
            return null;
        }

        public override BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            BoundNode result = base.VisitArrayInitialization(node);
            SetNotNullResult(node);
            return result;
        }

        private void SetUnknownResultNullability(BoundExpression expression)
        {
            SetResultType(expression, TypeWithState.Create(expression.Type, NullableFlowState.NotNull));
        }

        public override BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            BoundNode result = base.VisitStackAllocArrayCreation(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            BoundExpression receiver = node.Receiver;
            VisitRvalue(receiver);
            CheckPossibleNullReceiver(receiver);
            VisitArgumentsEvaluate(node.Syntax, node.Arguments, node.ArgumentRefKindsOpt, default(ImmutableArray<ParameterSymbol>), default(ImmutableArray<int>), default(BitVector), expanded: false);
            TypeWithAnnotations type = TypeWithAnnotations.Create(node.Type);
            SetLvalueResultType(node, type);
            return null;
        }

        private bool CheckPossibleNullReceiver(BoundExpression? receiverOpt, bool checkNullableValueType = false)
        {
            return CheckPossibleNullReceiver(receiverOpt, ResultType, checkNullableValueType);
        }

        private bool CheckPossibleNullReceiver(BoundExpression? receiverOpt, TypeWithState resultType, bool checkNullableValueType)
        {
            bool reportedDiagnostic = false;
            if (receiverOpt != null && State.Reachable)
            {
                TypeSymbol type = resultType.Type;
                if ((object)type == null)
                {
                    return false;
                }
                if (!ReportPossibleNullReceiverIfNeeded(type, resultType.State, checkNullableValueType, receiverOpt!.Syntax, out reportedDiagnostic))
                {
                    return reportedDiagnostic;
                }
                LearnFromNonNullTest(receiverOpt, ref State);
            }
            return reportedDiagnostic;
        }

        private bool ReportPossibleNullReceiverIfNeeded(TypeSymbol type, NullableFlowState state, bool checkNullableValueType, SyntaxNode syntax, out bool reportedDiagnostic)
        {
            reportedDiagnostic = false;
            if (state.MayBeNull())
            {
                bool isValueType = type.IsValueType;
                if (isValueType && (!checkNullableValueType || !type.IsNullableTypeOrTypeParameter() || type.GetNullableUnderlyingType().IsErrorType()))
                {
                    return false;
                }
                ReportDiagnostic(isValueType ? ErrorCode.WRN_NullableValueTypeMayBeNull : ErrorCode.WRN_NullReferenceReceiver, syntax);
                reportedDiagnostic = true;
            }
            return true;
        }

        private void CheckExtensionMethodThisNullability(BoundExpression expr, Conversion conversion, ParameterSymbol parameter, TypeWithState result)
        {
            VisitArgumentConversionAndInboundAssignmentsAndPreConditions(null, expr, conversion, parameter.RefKind, parameter, parameter.TypeWithAnnotations, GetParameterAnnotations(parameter), new VisitArgumentResult(new VisitResult(result, result.ToTypeWithAnnotations(compilation)), default(Optional<LocalState>)), extensionMethodThisArgument: true);
        }

        private static bool IsNullabilityMismatch(TypeWithAnnotations type1, TypeWithAnnotations type2)
        {
            if (type1.Equals(type2, TypeCompareKind.AllIgnoreOptions))
            {
                return !type1.Equals(type2, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.ObliviousNullableModifierMatchesAny | TypeCompareKind.IgnoreNativeIntegers);
            }
            return false;
        }

        private static bool IsNullabilityMismatch(TypeSymbol type1, TypeSymbol type2)
        {
            if (type1.Equals(type2, TypeCompareKind.AllIgnoreOptions))
            {
                return !type1.Equals(type2, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.ObliviousNullableModifierMatchesAny | TypeCompareKind.IgnoreNativeIntegers);
            }
            return false;
        }

        public override BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            BoundNode result = base.VisitQueryClause(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            BoundNode result = base.VisitNameOfOperator(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
            return result;
        }

        public override BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node)
        {
            BoundNode result = base.VisitNamespaceExpression(node);
            SetUnknownResultNullability(node);
            return result;
        }

        public override BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            BoundNode result = base.VisitInterpolatedString(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
            return result;
        }

        public override BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            BoundNode result = base.VisitUnconvertedInterpolatedString(node);
            SetResultType(node, TypeWithState.Create(node.Type, NullableFlowState.NotNull));
            return result;
        }

        public override BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            BoundNode result = base.VisitStringInsert(node);
            SetUnknownResultNullability(node);
            return result;
        }

        public override BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            BoundNode result = base.VisitConvertedStackAllocExpression(node);
            SetNotNullResult(node);
            return result;
        }

        public override BoundNode? VisitDiscardExpression(BoundDiscardExpression node)
        {
            TypeWithAnnotations lvalueType = TypeWithAnnotations.Create(node.Type);
            TypeWithState resultType = TypeWithState.ForType(node.Type);
            SetResult(node, resultType, lvalueType);
            return null;
        }

        public override BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            VisitThrow(node.Expression);
            SetResultType(node, default(TypeWithState));
            return null;
        }

        public override BoundNode? VisitThrowStatement(BoundThrowStatement node)
        {
            VisitThrow(node.ExpressionOpt);
            return null;
        }

        private void VisitThrow(BoundExpression? expr)
        {
            if (expr != null && VisitRvalueWithState(expr).MayBeNull)
            {
                ReportDiagnostic(ErrorCode.WRN_ThrowPossibleNull, expr!.Syntax);
            }
            SetUnreachable();
        }

        public override BoundNode? VisitYieldReturnStatement(BoundYieldReturnStatement node)
        {
            BoundExpression expression = node.Expression;
            if (expression == null)
            {
                return null;
            }
            MethodSymbol methodSymbol = (MethodSymbol)CurrentSymbol;
            TypeWithAnnotations iteratorElementTypeFromReturnType = InMethodBinder.GetIteratorElementTypeFromReturnType(compilation, RefKind.None, methodSymbol.ReturnType, null, null);
            VisitOptionalImplicitConversion(expression, iteratorElementTypeFromReturnType, useLegacyWarnings: false, trackMembers: false, AssignmentKind.Return);
            return null;
        }

        protected override void VisitCatchBlock(BoundCatchBlock node, ref LocalState finallyState)
        {
            TakeIncrementalSnapshot(node);
            if (node.Locals.Length > 0)
            {
                LocalSymbol localSymbol = node.Locals[0];
                if (localSymbol.DeclarationKind == LocalDeclarationKind.CatchVariable)
                {
                    int orCreateSlot = GetOrCreateSlot(localSymbol);
                    if (orCreateSlot > 0)
                    {
                        State[orCreateSlot] = NullableFlowState.NotNull;
                    }
                }
            }
            if (node.ExceptionSourceOpt != null)
            {
                VisitWithoutDiagnostics(node.ExceptionSourceOpt);
            }
            base.VisitCatchBlock(node, ref finallyState);
        }

        public override BoundNode? VisitLockStatement(BoundLockStatement node)
        {
            VisitRvalue(node.Argument);
            CheckPossibleNullReceiver(node.Argument);
            VisitStatement(node.Body);
            return null;
        }

        public override BoundNode? VisitAttribute(BoundAttribute node)
        {
            VisitArguments(node, node.ConstructorArguments, ImmutableArray<RefKind>.Empty, node.Constructor, node.ConstructorArgumentsToParamsOpt, default(BitVector), node.ConstructorExpanded, invokedAsExtensionMethod: false);
            ImmutableArray<BoundAssignmentOperator>.Enumerator enumerator = node.NamedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundAssignmentOperator current = enumerator.Current;
                Visit(current);
            }
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            TypeWithAnnotations lvalueType = TypeWithAnnotations.Create(node.Type, node.NullableAnnotation);
            SetResult(node.Expression, lvalueType.ToTypeWithState(), lvalueType);
            return null;
        }

        public override BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            SetNotNullResult(node);
            return null;
        }

        public override BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            SetNotNullResult(node);
            return null;
        }

        [System.Diagnostics.CodeAnalysis.MemberNotNull("_awaitablePlaceholdersOpt")]
        private void EnsureAwaitablePlaceholdersInitialized()
        {
            if (_awaitablePlaceholdersOpt == null)
            {
                _awaitablePlaceholdersOpt = PooledDictionary<BoundAwaitableValuePlaceholder, (BoundExpression, VisitResult)>.GetInstance();
            }
        }

        public override BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            if (_awaitablePlaceholdersOpt != null && _awaitablePlaceholdersOpt!.TryGetValue(node, out var value))
            {
                VisitResult item = value.Item2;
                SetResult(node, item.RValueType, item.LValueType);
            }
            else
            {
                SetNotNullResult(node);
            }
            return null;
        }

        public override BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            Visit(node.AwaitableInstancePlaceholder);
            Visit(node.GetAwaiter);
            return null;
        }

        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            Visit(node.InvokedExpression);
            VisitArguments(node, node.Arguments, node.ArgumentRefKindsOpt, node.FunctionPointer.Signature, default(ImmutableArray<int>), default(BitVector), expanded: false, invokedAsExtensionMethod: false);
            TypeWithAnnotations returnTypeWithAnnotations = node.FunctionPointer.Signature.ReturnTypeWithAnnotations;
            SetResult(node, returnTypeWithAnnotations.ToTypeWithState(), returnTypeWithAnnotations);
            return null;
        }

        protected override string Dump(LocalState state)
        {
            return state.Dump(_variables);
        }

        protected override bool Meet(ref LocalState self, ref LocalState other)
        {
            if (!self.Reachable)
            {
                return false;
            }
            if (!other.Reachable)
            {
                self = other.Clone();
                return true;
            }
            Normalize(ref self);
            Normalize(ref other);
            return self.Meet(in other);
        }

        protected override bool Join(ref LocalState self, ref LocalState other)
        {
            if (!other.Reachable)
            {
                return false;
            }
            if (!self.Reachable)
            {
                self = other.Clone();
                return true;
            }
            Normalize(ref self);
            Normalize(ref other);
            return self.Join(in other);
        }

        protected override LocalFunctionState CreateLocalFunctionState(LocalFunctionSymbol symbol)
        {
            return new LocalFunctionState(LocalState.UnreachableState(((symbol.ContainingSymbol is MethodSymbol method) ? _variables.GetVariablesForMethodScope(method) : null) ?? _variables.GetRootScope()));
        }

        private void LearnFromAnyNullPatterns(BoundExpression expression, BoundPattern pattern)
        {
            int inputSlot = MakeSlot(expression);
            LearnFromAnyNullPatterns(inputSlot, expression.Type, pattern);
        }

        private void VisitPatternForRewriting(BoundPattern pattern)
        {
            LocalState state = State;
            VisitWithoutDiagnostics(pattern);
            SetState(state);
        }

        public override BoundNode VisitSubpattern(BoundSubpattern node)
        {
            Visit(node.Pattern);
            return null;
        }

        public override BoundNode VisitRecursivePattern(BoundRecursivePattern node)
        {
            Visit(node.DeclaredType);
            VisitAndUnsplitAll(node.Deconstruction);
            VisitAndUnsplitAll(node.Properties);
            Visit(node.VariableAccess);
            return null;
        }

        public override BoundNode VisitConstantPattern(BoundConstantPattern node)
        {
            Visit(node.Value);
            return null;
        }

        public override BoundNode VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            Visit(node.VariableAccess);
            Visit(node.DeclaredType);
            return null;
        }

        public override BoundNode VisitDiscardPattern(BoundDiscardPattern node)
        {
            return null;
        }

        public override BoundNode VisitTypePattern(BoundTypePattern node)
        {
            Visit(node.DeclaredType);
            return null;
        }

        public override BoundNode VisitRelationalPattern(BoundRelationalPattern node)
        {
            Visit(node.Value);
            return null;
        }

        public override BoundNode VisitNegatedPattern(BoundNegatedPattern node)
        {
            Visit(node.Negated);
            return null;
        }

        public override BoundNode VisitBinaryPattern(BoundBinaryPattern node)
        {
            Visit(node.Left);
            Visit(node.Right);
            return null;
        }

        public override BoundNode VisitITuplePattern(BoundITuplePattern node)
        {
            VisitAndUnsplitAll(node.Subpatterns);
            return null;
        }

        private void LearnFromAnyNullPatterns(int inputSlot, TypeSymbol inputType, BoundPattern pattern)
        {
            if (inputSlot <= 0)
            {
                return;
            }
            VisitPatternForRewriting(pattern);
            if (!(pattern is BoundConstantPattern boundConstantPattern))
            {
                if (pattern is BoundDeclarationPattern || pattern is BoundDiscardPattern || pattern is BoundITuplePattern || pattern is BoundRelationalPattern)
                {
                    return;
                }
                if (!(pattern is BoundTypePattern boundTypePattern))
                {
                    if (!(pattern is BoundRecursivePattern boundRecursivePattern))
                    {
                        if (!(pattern is BoundNegatedPattern boundNegatedPattern))
                        {
                            if (!(pattern is BoundBinaryPattern boundBinaryPattern))
                            {
                                throw ExceptionUtilities.UnexpectedValue(pattern);
                            }
                            LearnFromAnyNullPatterns(inputSlot, inputType, boundBinaryPattern.Left);
                            LearnFromAnyNullPatterns(inputSlot, inputType, boundBinaryPattern.Right);
                        }
                        else
                        {
                            LearnFromAnyNullPatterns(inputSlot, inputType, boundNegatedPattern.Negated);
                        }
                        return;
                    }
                    if (boundRecursivePattern.IsExplicitNotNullTest)
                    {
                        LearnFromNullTest(inputSlot, inputType, ref State, markDependentSlotsNotNull: false);
                    }
                    if ((object)boundRecursivePattern.DeconstructMethod == null && !boundRecursivePattern.Deconstruction.IsDefault)
                    {
                        ImmutableArray<FieldSymbol> tupleElements = inputType.TupleElements;
                        int i = 0;
                        for (int num = Math.Min(boundRecursivePattern.Deconstruction.Length, (!tupleElements.IsDefault) ? tupleElements.Length : 0); i < num; i++)
                        {
                            BoundSubpattern boundSubpattern = boundRecursivePattern.Deconstruction[i];
                            FieldSymbol fieldSymbol = tupleElements[i];
                            LearnFromAnyNullPatterns(GetOrCreateSlot(fieldSymbol, inputSlot), fieldSymbol.Type, boundSubpattern.Pattern);
                        }
                    }
                    if (boundRecursivePattern.Properties.IsDefault)
                    {
                        return;
                    }
                    int j = 0;
                    for (int length = boundRecursivePattern.Properties.Length; j < length; j++)
                    {
                        BoundSubpattern boundSubpattern2 = boundRecursivePattern.Properties[j];
                        Symbol symbol = boundSubpattern2.Symbol;
                        if ((object)symbol != null && symbol.ContainingType.Equals(inputType, TypeCompareKind.AllIgnoreOptions))
                        {
                            LearnFromAnyNullPatterns(GetOrCreateSlot(symbol, inputSlot), symbol.GetTypeOrReturnType().Type, boundSubpattern2.Pattern);
                        }
                    }
                }
                else if (boundTypePattern.IsExplicitNotNullTest)
                {
                    LearnFromNullTest(inputSlot, inputType, ref State, markDependentSlotsNotNull: false);
                }
            }
            else if (boundConstantPattern.Value.ConstantValue == ConstantValue.Null)
            {
                LearnFromNullTest(inputSlot, inputType, ref State, markDependentSlotsNotNull: false);
            }
        }

        protected override LocalState VisitSwitchStatementDispatch(BoundSwitchStatement node)
        {
            int num = (node.Expression.IsSuppressed ? GetOrCreatePlaceholderSlot(node.Expression) : MakeSlot(node.Expression));
            ImmutableArray<BoundSwitchSection>.Enumerator enumerator;
            if (num > 0)
            {
                TypeSymbol type = node.Expression.Type;
                enumerator = node.SwitchSections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = enumerator.Current.SwitchLabels.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        BoundSwitchLabel current = enumerator2.Current;
                        LearnFromAnyNullPatterns(num, type, current.Pattern);
                    }
                }
            }
            Visit(node.Expression);
            TypeWithState resultType = ResultType;
            DeclareLocals(node.InnerLocals);
            enumerator = node.SwitchSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchSection current2 = enumerator.Current;
                DeclareLocals(current2.Locals);
            }
            PooledDictionary<LabelSymbol, (LocalState, bool)> pooledDictionary = LearnFromDecisionDag(node.Syntax, node.DecisionDag, node.Expression, resultType);
            enumerator = node.SwitchSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ImmutableArray<BoundSwitchLabel>.Enumerator enumerator2 = enumerator.Current.SwitchLabels.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BoundSwitchLabel current3 = enumerator2.Current;
                    SetState((pooledDictionary.TryGetValue(current3.Label, out var value) ? value : (UnreachableState(), false)).Item1);
                    base.PendingBranches.Add(new PendingBranch(current3, State, current3.Label));
                }
            }
            LocalState result = (pooledDictionary.TryGetValue(node.BreakLabel, out (LocalState, bool) value2) ? value2.Item1 : UnreachableState());
            pooledDictionary.Free();
            return result;
        }

        protected override void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            TakeIncrementalSnapshot(node);
            SetState(UnreachableState());
            ImmutableArray<BoundSwitchLabel>.Enumerator enumerator = node.SwitchLabels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchLabel current = enumerator.Current;
                TakeIncrementalSnapshot(current);
                VisitPatternForRewriting(current.Pattern);
                VisitLabel(current.Label, node);
            }
            VisitStatementList(node);
        }

        private PooledDictionary<LabelSymbol, (LocalState state, bool believedReachable)> LearnFromDecisionDag(SyntaxNode node, BoundDecisionDag decisionDag, BoundExpression expression, TypeWithState expressionType)
        {
            // We reuse the slot at the beginning of a switch (or is-pattern expression), pretending that we are
            // not copying the input to evaluate the patterns.  In this way we infer non-nullability of the original
            // variable's parts based on matched pattern parts.  Mutations in `when` clauses can show the inaccuracy
            // of analysis based on this choice.
            var rootTemp = BoundDagTemp.ForOriginalInput(expression);
            int originalInputSlot = MakeSlot(expression);
            if (originalInputSlot <= 0)
            {
                originalInputSlot = makeDagTempSlot(expressionType.ToTypeWithAnnotations(compilation), rootTemp);
            }

            // If the input of the switch (or is-pattern expression) is a tuple literal, we reuse the slots of
            // those expressions (when possible), pretending that we are not copying them into a temporary ValueTuple instance
            // to evaluate the patterns.  In this way we infer non-nullability of the original element's parts.
            // We do not extend such courtesy to nested tuple literals.
            var originalInputElementSlots = expression is BoundTupleExpression tuple
                ? tuple.Arguments.SelectAsArray(static (a, w) => w.MakeSlot(a), this)
                : default;
            var originalInputMap = PooledDictionary<int, BoundExpression>.GetInstance();
            originalInputMap.Add(originalInputSlot, expression);

            var tempMap = PooledDictionary<BoundDagTemp, (int slot, TypeSymbol type)>.GetInstance();
            tempMap.Add(rootTemp, (originalInputSlot, expressionType.Type));

            var nodeStateMap = PooledDictionary<BoundDecisionDagNode, (PossiblyConditionalState state, bool believedReachable)>.GetInstance();
            nodeStateMap.Add(decisionDag.RootNode, (state: PossiblyConditionalState.Create(this), believedReachable: true));

            var labelStateMap = PooledDictionary<LabelSymbol, (LocalState state, bool believedReachable)>.GetInstance();

            foreach (var dagNode in decisionDag.TopologicallySortedNodes)
            {
                bool found = nodeStateMap.TryGetValue(dagNode, out var nodeStateAndBelievedReachable);
                (PossiblyConditionalState nodeState, bool nodeBelievedReachable) = nodeStateAndBelievedReachable;
                if (nodeState.IsConditionalState)
                {
                    SetConditionalState(nodeState.StateWhenTrue, nodeState.StateWhenFalse);
                }
                else
                {
                    SetState(nodeState.State);
                }

                switch (dagNode)
                {
                    case BoundEvaluationDecisionDagNode p:
                        {
                            var evaluation = p.Evaluation;
                            (int inputSlot, TypeSymbol inputType) = tempMap.TryGetValue(evaluation.Input, out var slotAndType) ? slotAndType : throw ExceptionUtilities.Unreachable;

                            switch (evaluation)
                            {
                                case BoundDagDeconstructEvaluation e:
                                    {
                                        // https://github.com/dotnet/roslyn/issues/34232
                                        // We may need to recompute the Deconstruct method for a deconstruction if
                                        // the receiver type has changed (e.g. its nested nullability).
                                        var method = e.DeconstructMethod;
                                        int extensionExtra = method.RequiresInstanceReceiver ? 0 : 1;
                                        for (int i = 0; i < method.ParameterCount - extensionExtra; i++)
                                        {
                                            var parameterType = method.Parameters[i + extensionExtra].TypeWithAnnotations;
                                            var output = new BoundDagTemp(e.Syntax, parameterType.Type, e, i);
                                            int outputSlot = makeDagTempSlot(parameterType, output);
                                            addToTempMap(output, outputSlot, parameterType.Type);
                                        }
                                        break;
                                    }
                                case BoundDagTypeEvaluation e:
                                    {
                                        var output = new BoundDagTemp(e.Syntax, e.Type, e);
                                        var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                                        int outputSlot;
                                        switch (_conversions.WithNullability(false).ClassifyConversionFromType(inputType, e.Type, ref discardedUseSiteInfo).Kind)
                                        {
                                            case ConversionKind.Identity:
                                            case ConversionKind.ImplicitReference:
                                                outputSlot = inputSlot;
                                                break;
                                            case ConversionKind.ExplicitNullable when AreNullableAndUnderlyingTypes(inputType, e.Type, out _):
                                                outputSlot = GetNullableOfTValueSlot(inputType, inputSlot, out _, forceSlotEvenIfEmpty: true);
                                                if (outputSlot < 0)
                                                    goto default;
                                                break;
                                            default:
                                                outputSlot = makeDagTempSlot(TypeWithAnnotations.Create(e.Type, NullableAnnotation.NotAnnotated), output);
                                                break;
                                        }
                                        Unsplit();
                                        State[outputSlot] = NullableFlowState.NotNull;
                                        addToTempMap(output, outputSlot, e.Type);
                                        break;
                                    }
                                case BoundDagFieldEvaluation e:
                                    {
                                        var field = (FieldSymbol)AsMemberOfType(inputType, e.Field);
                                        var type = field.TypeWithAnnotations;
                                        var output = new BoundDagTemp(e.Syntax, type.Type, e);
                                        int outputSlot = -1;
                                        var originalTupleElement = e.Input.IsOriginalInput && !originalInputElementSlots.IsDefault
                                            ? field
                                            : null;
                                        if (originalTupleElement is not null)
                                        {
                                            // Re-use the slot of the element/expression if possible
                                            outputSlot = originalInputElementSlots[originalTupleElement.TupleElementIndex];
                                        }
                                        if (outputSlot <= 0)
                                        {
                                            outputSlot = GetOrCreateSlot(field, inputSlot, forceSlotEvenIfEmpty: true);

                                            if (originalTupleElement is not null && outputSlot > 0)
                                            {
                                                // The expression in the tuple could not be assigned a slot (for example, `a?.b`),
                                                // so we had to create a slot for the tuple element instead.
                                                // We'll remember that so that we can apply any learnings to the expression.
                                                if (!originalInputMap.ContainsKey(outputSlot))
                                                {
                                                    originalInputMap.Add(outputSlot,
                                                        ((BoundTupleExpression)expression).Arguments[originalTupleElement.TupleElementIndex]);
                                                }
                                            }
                                        }
                                        if (outputSlot <= 0)
                                        {
                                            outputSlot = makeDagTempSlot(type, output);
                                        }

                                        addToTempMap(output, outputSlot, type.Type);
                                        break;
                                    }
                                case BoundDagPropertyEvaluation e:
                                    {
                                        var property = (PropertySymbol)AsMemberOfType(inputType, e.Property);
                                        var type = property.TypeWithAnnotations;
                                        var output = new BoundDagTemp(e.Syntax, type.Type, e);
                                        int outputSlot = GetOrCreateSlot(property, inputSlot, forceSlotEvenIfEmpty: true);
                                        if (outputSlot <= 0)
                                        {
                                            outputSlot = makeDagTempSlot(type, output);
                                        }
                                        addToTempMap(output, outputSlot, type.Type);

                                        if (property.GetMethod is not null)
                                        {
                                            // A property evaluation splits the state if MemberNotNullWhen is used
                                            ApplyMemberPostConditions(inputSlot, property.GetMethod);
                                        }

                                        break;
                                    }
                                case BoundDagIndexEvaluation e:
                                    {
                                        var type = TypeWithAnnotations.Create(e.Property.Type, NullableAnnotation.Annotated);
                                        var output = new BoundDagTemp(e.Syntax, type.Type, e);
                                        int outputSlot = makeDagTempSlot(type, output);
                                        addToTempMap(output, outputSlot, type.Type);
                                        break;
                                    }
                                default:
                                    throw ExceptionUtilities.UnexpectedValue(p.Evaluation.Kind);
                            }
                            gotoNodeWithCurrentState(p.Next, nodeBelievedReachable);
                            break;
                        }
                    case BoundTestDecisionDagNode p:
                        {
                            var test = p.Test;
                            bool foundTemp = tempMap.TryGetValue(test.Input, out var slotAndType);

                            (int inputSlot, TypeSymbol inputType) = slotAndType;
                            Split();
                            switch (test)
                            {
                                case BoundDagTypeTest:
                                    if (inputSlot > 0)
                                    {
                                        learnFromNonNullTest(inputSlot, ref this.StateWhenTrue);
                                    }
                                    gotoNode(p.WhenTrue, this.StateWhenTrue, nodeBelievedReachable);
                                    gotoNode(p.WhenFalse, this.StateWhenFalse, nodeBelievedReachable);
                                    break;
                                case BoundDagNonNullTest t:
                                    var inputMaybeNull = this.StateWhenTrue[inputSlot].MayBeNull();
                                    if (inputSlot > 0)
                                    {
                                        MarkDependentSlotsNotNull(inputSlot, inputType, ref this.StateWhenFalse);
                                        if (t.IsExplicitTest)
                                        {
                                            LearnFromNullTest(inputSlot, inputType, ref this.StateWhenFalse, markDependentSlotsNotNull: false);
                                        }
                                        learnFromNonNullTest(inputSlot, ref this.StateWhenTrue);
                                    }
                                    gotoNode(p.WhenTrue, this.StateWhenTrue, nodeBelievedReachable);
                                    gotoNode(p.WhenFalse, this.StateWhenFalse, nodeBelievedReachable & inputMaybeNull);
                                    break;
                                case BoundDagExplicitNullTest _:
                                    if (inputSlot > 0)
                                    {
                                        LearnFromNullTest(inputSlot, inputType, ref this.StateWhenTrue, markDependentSlotsNotNull: true);
                                        learnFromNonNullTest(inputSlot, ref this.StateWhenFalse);
                                    }
                                    gotoNode(p.WhenTrue, this.StateWhenTrue, nodeBelievedReachable);
                                    gotoNode(p.WhenFalse, this.StateWhenFalse, nodeBelievedReachable);
                                    break;
                                case BoundDagValueTest t:
                                    if (inputSlot > 0)
                                    {
                                        learnFromNonNullTest(inputSlot, ref this.StateWhenTrue);
                                    }
                                    bool isFalseTest = t.Value == ConstantValue.False;
                                    gotoNode(p.WhenTrue, isFalseTest ? this.StateWhenFalse : this.StateWhenTrue, nodeBelievedReachable);
                                    gotoNode(p.WhenFalse, isFalseTest ? this.StateWhenTrue : this.StateWhenFalse, nodeBelievedReachable);
                                    break;
                                case BoundDagRelationalTest _:
                                    if (inputSlot > 0)
                                    {
                                        learnFromNonNullTest(inputSlot, ref this.StateWhenTrue);
                                    }
                                    gotoNode(p.WhenTrue, this.StateWhenTrue, nodeBelievedReachable);
                                    gotoNode(p.WhenFalse, this.StateWhenFalse, nodeBelievedReachable);
                                    break;
                                default:
                                    throw ExceptionUtilities.UnexpectedValue(test.Kind);
                            }
                            break;
                        }
                    case BoundLeafDecisionDagNode d:
                        // We have one leaf decision dag node per reachable label
                        Unsplit(); // Could be split in pathological cases like `false switch { ... }`
                        labelStateMap.Add(d.Label, (this.State, nodeBelievedReachable));
                        break;
                    case BoundWhenDecisionDagNode w:
                        // bind the pattern variables, inferring their types as well
                        Unsplit();
                        foreach (var binding in w.Bindings)
                        {
                            var variableAccess = binding.VariableAccess;
                            var tempSource = binding.TempContainingValue;
                            var foundTemp = tempMap.TryGetValue(tempSource, out var tempSlotAndType);
                            if (foundTemp) // in erroneous programs, we might not have seen a temp defined.
                            {
                                var (tempSlot, tempType) = tempSlotAndType;
                                var tempState = this.State[tempSlot];
                                if (variableAccess is BoundLocal { LocalSymbol: SourceLocalSymbol local } boundLocal)
                                {
                                    var value = TypeWithState.Create(tempType, tempState);
                                    var inferredType = value.ToTypeWithAnnotations(compilation, asAnnotatedType: boundLocal.DeclarationKind == BoundLocalDeclarationKind.WithInferredType);
                                    if (_variables.TryGetType(local, out var existingType))
                                    {
                                        // merge inferred nullable annotation from different branches of the decision tree
                                        inferredType = TypeWithAnnotations.Create(inferredType.Type, existingType.NullableAnnotation.Join(inferredType.NullableAnnotation));
                                    }
                                    _variables.SetType(local, inferredType);

                                    int localSlot = GetOrCreateSlot(local, forceSlotEvenIfEmpty: true);
                                    if (localSlot > 0)
                                    {
                                        TrackNullableStateForAssignment(valueOpt: null, inferredType, localSlot, TypeWithState.Create(tempType, tempState), tempSlot);
                                    }
                                }
                                else
                                {
                                    // https://github.com/dotnet/roslyn/issues/34144 perform inference for top-level var-declared fields in scripts
                                }
                            }
                        }

                        if (w.WhenExpression != null && w.WhenExpression.ConstantValue != ConstantValue.True)
                        {
                            VisitCondition(w.WhenExpression);
                            gotoNode(w.WhenTrue, this.StateWhenTrue, nodeBelievedReachable);
                            gotoNode(w.WhenFalse, this.StateWhenFalse, nodeBelievedReachable);
                        }
                        else
                        {
                            gotoNode(w.WhenTrue, this.State, nodeBelievedReachable);
                        }
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(dagNode.Kind);
                }
            }

            SetUnreachable(); // the decision dag is always complete (no fall-through)
            originalInputMap.Free();
            tempMap.Free();
            nodeStateMap.Free();
            return labelStateMap;

            void learnFromNonNullTest(int inputSlot, ref LocalState state)
            {
                LearnFromNonNullTest(inputSlot, ref state);
                if (originalInputMap.TryGetValue(inputSlot, out var expression))
                    LearnFromNonNullTest(expression, ref state);
            }

            void addToTempMap(BoundDagTemp output, int slot, TypeSymbol type)
            {
                // We need to track all dag temps, so there should be a slot
                if (!tempMap.TryGetValue(output, out var outputSlotAndType))
                {
                    tempMap.Add(output, (slot, type));
                }
            }

            bool isDerivedType(TypeSymbol derivedType, TypeSymbol baseType)
            {
                if (derivedType.IsErrorType() || baseType.IsErrorType())
                    return true;

                var discardedUseSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                return _conversions.WithNullability(false).ClassifyConversionFromType(derivedType, baseType, ref discardedUseSiteInfo).Kind switch
                {
                    ConversionKind.Identity => true,
                    ConversionKind.ImplicitReference => true,
                    ConversionKind.Boxing => true,
                    _ => false,
                };
            }

            void gotoNodeWithCurrentState(BoundDecisionDagNode node, bool believedReachable)
            {
                if (nodeStateMap.TryGetValue(node, out var stateAndReachable))
                {
                    switch (IsConditionalState, stateAndReachable.state.IsConditionalState)
                    {
                        case (true, true):
                            Join(ref this.StateWhenTrue, ref stateAndReachable.state.StateWhenTrue);
                            Join(ref this.StateWhenFalse, ref stateAndReachable.state.StateWhenFalse);
                            break;
                        case (true, false):
                            Join(ref this.StateWhenTrue, ref stateAndReachable.state.State);
                            Join(ref this.StateWhenFalse, ref stateAndReachable.state.State);
                            break;
                        case (false, true):
                            Split();
                            Join(ref this.StateWhenTrue, ref stateAndReachable.state.StateWhenTrue);
                            Join(ref this.StateWhenFalse, ref stateAndReachable.state.StateWhenFalse);
                            break;
                        case (false, false):
                            Join(ref this.State, ref stateAndReachable.state.State);
                            break;
                    }
                    believedReachable |= stateAndReachable.believedReachable;
                }

                nodeStateMap[node] = (PossiblyConditionalState.Create(this), believedReachable);
            }

            void gotoNode(BoundDecisionDagNode node, LocalState state, bool believedReachable)
            {
                PossiblyConditionalState result;
                if (nodeStateMap.TryGetValue(node, out var stateAndReachable))
                {
                    result = stateAndReachable.state;
                    switch (result.IsConditionalState)
                    {
                        case true:
                            Join(ref result.StateWhenTrue, ref state);
                            Join(ref result.StateWhenFalse, ref state);
                            break;
                        case false:
                            Join(ref result.State, ref state);
                            break;
                    }
                    believedReachable |= stateAndReachable.believedReachable;
                }
                else
                {
                    result = new PossiblyConditionalState(state);
                }

                nodeStateMap[node] = (result, believedReachable);
            }

            int makeDagTempSlot(TypeWithAnnotations type, BoundDagTemp temp)
            {
                object slotKey = (node, temp);
                return GetOrCreatePlaceholderSlot(slotKey, type);
            }
        }

        public override BoundNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            bool inferType = !node.WasTargetTyped;
            VisitSwitchExpressionCore(node, inferType);
            return null;
        }

        public override BoundNode VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            VisitSwitchExpressionCore(node, inferType: true);
            return null;
        }

        private void VisitSwitchExpressionCore(BoundSwitchExpression node, bool inferType)
        {
            int num = (node.Expression.IsSuppressed ? GetOrCreatePlaceholderSlot(node.Expression) : MakeSlot(node.Expression));
            ImmutableArray<BoundSwitchExpressionArm>.Enumerator enumerator;
            if (num > 0)
            {
                TypeSymbol type = node.Expression.Type;
                enumerator = node.SwitchArms.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundSwitchExpressionArm current = enumerator.Current;
                    LearnFromAnyNullPatterns(num, type, current.Pattern);
                }
            }
            Visit(node.Expression);
            TypeWithState resultType = ResultType;
            PooledDictionary<LabelSymbol, (LocalState state, bool believedReachable)> labelStateMap = LearnFromDecisionDag(node.Syntax, node.DecisionDag, node.Expression, resultType);
            LocalState self = UnreachableState();
            if (!node.ReportedNotExhaustive && node.DefaultLabel != null && labelStateMap.TryGetValue(node.DefaultLabel, out var value) && value.Item2)
            {
                SetState(value.Item1);
                ImmutableArray<BoundDecisionDagNode> topologicallySortedNodes = node.DecisionDag.TopologicallySortedNodes;
                BoundDecisionDagNode targetNode = topologicallySortedNodes.Where((BoundDecisionDagNode n) => n is BoundLeafDecisionDagNode boundLeafDecisionDagNode && boundLeafDecisionDagNode.Label == node.DefaultLabel).First();
                string text = PatternExplainer.SamplePatternForPathToDagNode(BoundDagTemp.ForOriginalInput(node.Expression), topologicallySortedNodes, targetNode, nullPaths: true, out bool requiresFalseWhenClause, out bool unnamedEnumValue);
                ErrorCode errorCode = (requiresFalseWhenClause ? ErrorCode.WRN_SwitchExpressionNotExhaustiveForNullWithWhen : ErrorCode.WRN_SwitchExpressionNotExhaustiveForNull);
                ReportDiagnostic(errorCode, ((SwitchExpressionSyntax)node.Syntax).SwitchKeyword.GetLocation(), text);
            }
            int length = node.SwitchArms.Length;
            ArrayBuilder<Conversion> instance = ArrayBuilder<Conversion>.GetInstance(length);
            ArrayBuilder<TypeWithState> instance2 = ArrayBuilder<TypeWithState>.GetInstance(length);
            ArrayBuilder<BoundExpression> instance3 = ArrayBuilder<BoundExpression>.GetInstance(length);
            ArrayBuilder<BoundExpression> instance4 = ArrayBuilder<BoundExpression>.GetInstance(length);
            enumerator = node.SwitchArms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundSwitchExpressionArm current2 = enumerator.Current;
                SetState(getStateForArm(current2));
                TakeIncrementalSnapshot(current2);
                VisitPatternForRewriting(current2.Pattern);
                var (boundExpression, item) = RemoveConversion(current2.Value, includeExplicitConversions: false);
                SnapshotWalkerThroughConversionGroup(current2.Value, boundExpression);
                instance3.Add(boundExpression);
                instance.Add(item);
                TypeWithState item2 = VisitRvalueWithState(boundExpression);
                instance2.Add(item2);
                Join(ref self, ref State);
                instance4.Add(CreatePlaceholderIfNecessary(boundExpression, item2.ToTypeWithAnnotations(compilation)));
            }
            ImmutableArray<BoundExpression> exprs = instance4.ToImmutableAndFree();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            TypeSymbol typeSymbol = (inferType ? BestTypeInferrer.InferBestType(exprs, _conversions, ref useSiteInfo) : null) ?? node.Type?.SetUnknownNullabilityForReferenceTypes();
            TypeWithAnnotations typeWithAnnotations = TypeWithAnnotations.Create(typeSymbol);
            NullableFlowState defaultState;
            if (inferType)
            {
                if ((object)typeSymbol == null)
                {
                    defaultState = NullableFlowState.NotNull;
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        BoundExpression operand = instance3[i];
                        BoundSwitchExpressionArm boundSwitchExpressionArm = node.SwitchArms[i];
                        LocalState state = getStateForArm(boundSwitchExpressionArm);
                        instance2[i] = ConvertConditionalOperandOrSwitchExpressionArmResult(boundSwitchExpressionArm.Value, operand, instance[i], typeWithAnnotations, instance2[i], state, state.Reachable);
                    }
                    defaultState = BestTypeInferrer.GetNullableState(instance2);
                }
            }
            else
            {
                ArrayBuilder<(LocalState, TypeWithState, bool)> instance5 = ArrayBuilder<(LocalState, TypeWithState, bool)>.GetInstance(length);
                for (int j = 0; j < length; j++)
                {
                    _ = instance3[j];
                    LocalState item3 = getStateForArm(node.SwitchArms[j]);
                    instance5.Add((item3, instance2[j], item3.Reachable));
                }
                ConditionalInfoForConversion.Add(node, instance5.ToImmutableAndFree());
                defaultState = BestTypeInferrer.GetNullableState(instance2);
            }
            TypeWithState resultType2 = TypeWithState.Create(typeSymbol, defaultState);
            instance.Free();
            instance2.Free();
            instance3.Free();
            labelStateMap.Free();
            SetState(self);
            SetResult(node, resultType2, typeWithAnnotations);
            LocalState getStateForArm(BoundSwitchExpressionArm arm)
            {
                if (arm.Pattern.HasErrors || !labelStateMap.TryGetValue(arm.Label, out var value2))
                {
                    return UnreachableState();
                }
                return value2.Item1;
            }
        }

        public override BoundNode VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            LearnFromAnyNullPatterns(node.Expression, node.Pattern);
            VisitPatternForRewriting(node.Pattern);
            Visit(node.Expression);
            TypeWithState resultType = ResultType;
            PooledDictionary<LabelSymbol, (LocalState state, bool believedReachable)> pooledDictionary = LearnFromDecisionDag(node.Syntax, node.DecisionDag, node.Expression, resultType);
            LocalState whenTrue = (pooledDictionary.TryGetValue(node.IsNegated ? node.WhenFalseLabel : node.WhenTrueLabel, out (LocalState, bool) value) ? value.Item1 : UnreachableState());
            LocalState whenFalse = (pooledDictionary.TryGetValue(node.IsNegated ? node.WhenTrueLabel : node.WhenFalseLabel, out (LocalState, bool) value2) ? value2.Item1 : UnreachableState());
            pooledDictionary.Free();
            SetConditionalState(whenTrue, whenFalse);
            SetNotNullResult(node);
            return null;
        }
    }
}
