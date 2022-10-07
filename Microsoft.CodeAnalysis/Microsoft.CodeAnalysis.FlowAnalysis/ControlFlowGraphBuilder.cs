using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.FlowAnalysis
{
    internal sealed class ControlFlowGraphBuilder : OperationVisitor<int?, IOperation>
    {
        internal sealed class BasicBlockBuilder
        {
            internal struct Branch
            {
                public ControlFlowBranchSemantics Kind { get; set; }

                public BasicBlockBuilder? Destination { get; set; }
            }

            public int Ordinal;

            public readonly BasicBlockKind Kind;

            private ArrayBuilder<IOperation>? _statements;

            private BasicBlockBuilder? _predecessor1;

            private BasicBlockBuilder? _predecessor2;

            private PooledHashSet<BasicBlockBuilder>? _predecessors;

            public IOperation? BranchValue;

            public ControlFlowConditionKind ConditionKind;

            public Branch Conditional;

            public Branch FallThrough;

            public bool IsReachable;

            public ControlFlowRegion? Region;

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "StatementsOpt")]
            public bool HasStatements
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "StatementsOpt")]
                get
                {
                    ArrayBuilder<IOperation>? statements = _statements;
                    if (statements == null)
                    {
                        return false;
                    }
                    return statements!.Count > 0;
                }
            }

            public ArrayBuilder<IOperation>? StatementsOpt => _statements;

            public bool HasPredecessors
            {
                get
                {
                    if (_predecessors != null)
                    {
                        return _predecessors!.Count > 0;
                    }
                    if (_predecessor1 == null)
                    {
                        return _predecessor2 != null;
                    }
                    return true;
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "BranchValue")]
            public bool HasCondition
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "BranchValue")]
                get
                {
                    return ConditionKind != ControlFlowConditionKind.None;
                }
            }

            public BasicBlockBuilder(BasicBlockKind kind)
            {
                Kind = kind;
                Ordinal = -1;
                IsReachable = false;
            }

            public void AddStatement(IOperation operation)
            {
                if (_statements == null)
                {
                    _statements = ArrayBuilder<IOperation>.GetInstance();
                }
                _statements!.Add(operation);
            }

            public void MoveStatementsFrom(BasicBlockBuilder other)
            {
                if (other._statements != null)
                {
                    if (_statements == null)
                    {
                        _statements = other._statements;
                        other._statements = null;
                    }
                    else
                    {
                        _statements!.AddRange(other._statements);
                        other._statements!.Clear();
                    }
                }
            }

            public BasicBlock ToImmutable()
            {
                BasicBlock result = new BasicBlock(Kind, _statements?.ToImmutableAndFree() ?? ImmutableArray<IOperation>.Empty, BranchValue, ConditionKind, Ordinal, IsReachable, Region);
                _statements = null;
                return result;
            }

            public BasicBlockBuilder? GetSingletonPredecessorOrDefault()
            {
                if (_predecessors != null)
                {
                    return _predecessors.AsSingleton();
                }
                if (_predecessor2 == null)
                {
                    return _predecessor1;
                }
                if (_predecessor1 == null)
                {
                    return _predecessor2;
                }
                return null;
            }

            public void AddPredecessor(BasicBlockBuilder predecessor)
            {
                if (_predecessors != null)
                {
                    _predecessors!.Add(predecessor);
                }
                else if (_predecessor1 != predecessor && _predecessor2 != predecessor)
                {
                    if (_predecessor1 == null)
                    {
                        _predecessor1 = predecessor;
                        return;
                    }
                    if (_predecessor2 == null)
                    {
                        _predecessor2 = predecessor;
                        return;
                    }
                    _predecessors = PooledHashSet<BasicBlockBuilder>.GetInstance();
                    _predecessors!.Add(_predecessor1);
                    _predecessors!.Add(_predecessor2);
                    _predecessors!.Add(predecessor);
                    _predecessor1 = null;
                    _predecessor2 = null;
                }
            }

            public void RemovePredecessor(BasicBlockBuilder predecessor)
            {
                if (_predecessors != null)
                {
                    _predecessors!.Remove(predecessor);
                }
                else if (_predecessor1 == predecessor)
                {
                    _predecessor1 = null;
                }
                else if (_predecessor2 == predecessor)
                {
                    _predecessor2 = null;
                }
            }

            public void GetPredecessors(ArrayBuilder<BasicBlockBuilder> builder)
            {
                if (_predecessors != null)
                {
                    foreach (BasicBlockBuilder item in _predecessors!)
                    {
                        builder.Add(item);
                    }
                    return;
                }
                if (_predecessor1 != null)
                {
                    builder.Add(_predecessor1);
                }
                if (_predecessor2 != null)
                {
                    builder.Add(_predecessor2);
                }
            }

            public ImmutableArray<ControlFlowBranch> ConvertPredecessorsToBranches(ArrayBuilder<BasicBlock> blocks)
            {
                ArrayBuilder<BasicBlock> blocks2 = blocks;
                if (!HasPredecessors)
                {
                    _predecessors?.Free();
                    _predecessors = null;
                    return ImmutableArray<ControlFlowBranch>.Empty;
                }
                BasicBlock block = blocks2[Ordinal];
                ArrayBuilder<ControlFlowBranch> branches = ArrayBuilder<ControlFlowBranch>.GetInstance(_predecessors?.Count ?? 2);
                if (_predecessors != null)
                {
                    foreach (BasicBlockBuilder item in _predecessors!)
                    {
                        addBranches(item);
                    }
                    _predecessors!.Free();
                    _predecessors = null;
                }
                else
                {
                    if (_predecessor1 != null)
                    {
                        addBranches(_predecessor1);
                        _predecessor1 = null;
                    }
                    if (_predecessor2 != null)
                    {
                        addBranches(_predecessor2);
                        _predecessor2 = null;
                    }
                }
                branches.Sort(delegate (ControlFlowBranch x, ControlFlowBranch y)
                {
                    int num = x.Source.Ordinal - y.Source.Ordinal;
                    if (num == 0 && x.IsConditionalSuccessor != y.IsConditionalSuccessor)
                    {
                        num = ((!x.IsConditionalSuccessor) ? 1 : (-1));
                    }
                    return num;
                });
                return branches.ToImmutableAndFree();
                void addBranches(BasicBlockBuilder predecessorBlockBuilder)
                {
                    BasicBlock basicBlock = blocks2[predecessorBlockBuilder.Ordinal];
                    if (basicBlock.FallThroughSuccessor!.Destination == block)
                    {
                        branches.Add(basicBlock.FallThroughSuccessor);
                    }
                    if (basicBlock.ConditionalSuccessor?.Destination == block)
                    {
                        branches.Add(basicBlock.ConditionalSuccessor);
                    }
                }
            }

            public void Free()
            {
                Ordinal = -1;
                _statements?.Free();
                _statements = null;
                _predecessors?.Free();
                _predecessors = null;
                _predecessor1 = null;
                _predecessor2 = null;
            }
        }

        internal class CaptureIdDispenser
        {
            private int _captureId = -1;

            public int GetNextId()
            {
                return Interlocked.Increment(ref _captureId);
            }

            public int GetCurrentId()
            {
                return _captureId;
            }
        }

        private readonly struct ConditionalAccessOperationTracker
        {
            public readonly ArrayBuilder<IOperation>? Operations;

            public readonly BasicBlockBuilder? WhenNull;

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, new string[] { "Operations", "WhenNull" })]
            public bool IsDefault
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, new string[] { "Operations", "WhenNull" })]
                get
                {
                    return Operations == null;
                }
            }

            public ConditionalAccessOperationTracker(ArrayBuilder<IOperation> operations, BasicBlockBuilder whenNull)
            {
                Operations = operations;
                WhenNull = whenNull;
            }

            public void Free()
            {
                Operations?.Free();
            }
        }

        internal struct Context
        {
            public readonly IOperation? ImplicitInstance;

            public readonly INamedTypeSymbol? AnonymousType;

            public readonly ImmutableArray<KeyValuePair<IPropertySymbol, IOperation>> AnonymousTypePropertyValues;

            internal Context(IOperation? implicitInstance, INamedTypeSymbol? anonymousType, ImmutableArray<KeyValuePair<IPropertySymbol, IOperation>> anonymousTypePropertyValues)
            {
                ImplicitInstance = implicitInstance;
                AnonymousType = anonymousType;
                AnonymousTypePropertyValues = anonymousTypePropertyValues;
            }
        }

        private class EvalStackFrame
        {
            private RegionBuilder? _lazyRegionBuilder;

            public RegionBuilder? RegionBuilderOpt
            {
                get
                {
                    return _lazyRegionBuilder;
                }
                set
                {
                    _lazyRegionBuilder = value;
                }
            }
        }

        private struct ImplicitInstanceInfo
        {
            public IOperation? ImplicitInstance { get; }

            public INamedTypeSymbol? AnonymousType { get; }

            public PooledDictionary<IPropertySymbol, IOperation>? AnonymousTypePropertyValues { get; }

            public ImplicitInstanceInfo(IOperation currentImplicitInstance)
            {
                ImplicitInstance = currentImplicitInstance;
                AnonymousType = null;
                AnonymousTypePropertyValues = null;
            }

            public ImplicitInstanceInfo(INamedTypeSymbol currentInitializedAnonymousType)
            {
                ImplicitInstance = null;
                AnonymousType = currentInitializedAnonymousType;
                AnonymousTypePropertyValues = PooledDictionary<IPropertySymbol, IOperation>.GetInstance();
            }

            public ImplicitInstanceInfo(in Context context)
            {
                if (context.ImplicitInstance != null)
                {
                    ImplicitInstance = context.ImplicitInstance;
                    AnonymousType = null;
                    AnonymousTypePropertyValues = null;
                }
                else if (context.AnonymousType != null)
                {
                    ImplicitInstance = null;
                    AnonymousType = context.AnonymousType;
                    AnonymousTypePropertyValues = PooledDictionary<IPropertySymbol, IOperation>.GetInstance();
                    ImmutableArray<KeyValuePair<IPropertySymbol, IOperation>>.Enumerator enumerator = context.AnonymousTypePropertyValues.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<IPropertySymbol, IOperation> current = enumerator.Current;
                        AnonymousTypePropertyValues!.Add(current.Key, current.Value);
                    }
                }
                else
                {
                    ImplicitInstance = null;
                    AnonymousType = null;
                    AnonymousTypePropertyValues = null;
                }
            }

            public void Free()
            {
                AnonymousTypePropertyValues?.Free();
            }
        }

        private class RegionBuilder
        {
            private sealed class AnonymousFunctionsMapBuilder : OperationVisitor<(ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder map, ControlFlowRegion region), IOperation>
            {
                public static readonly AnonymousFunctionsMapBuilder Instance = new AnonymousFunctionsMapBuilder();

                public override IOperation? VisitFlowAnonymousFunction(IFlowAnonymousFunctionOperation operation, (ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder map, ControlFlowRegion region) argument)
                {
                    argument.map.Add(operation, (argument.region, argument.map.Count));
                    return base.VisitFlowAnonymousFunction(operation, argument);
                }

                internal override IOperation? VisitNoneOperation(IOperation operation, (ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder map, ControlFlowRegion region) argument)
                {
                    return DefaultVisit(operation, argument);
                }

                public override IOperation? DefaultVisit(IOperation operation, (ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder map, ControlFlowRegion region) argument)
                {
                    foreach (IOperation childOperation in ((Operation)operation).ChildOperations)
                    {
                        Visit(childOperation, argument);
                    }
                    return null;
                }
            }

            public ControlFlowRegionKind Kind;

            public readonly ITypeSymbol? ExceptionType;

            public BasicBlockBuilder? FirstBlock;

            public BasicBlockBuilder? LastBlock;

            public ArrayBuilder<RegionBuilder>? Regions;

            public ImmutableArray<ILocalSymbol> Locals;

            public ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>? LocalFunctions;

            public ArrayBuilder<CaptureId>? CaptureIds;

            public readonly bool IsStackSpillRegion;

            public RegionBuilder? Enclosing { get; private set; }

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, new string[] { "FirstBlock", "LastBlock" })]
            public bool IsEmpty
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, new string[] { "FirstBlock", "LastBlock" })]
                get
                {
                    return FirstBlock == null;
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "Regions")]
            public bool HasRegions
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "Regions")]
                get
                {
                    ArrayBuilder<RegionBuilder>? regions = Regions;
                    if (regions == null)
                    {
                        return false;
                    }
                    return regions!.Count > 0;
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "LocalFunctions")]
            public bool HasLocalFunctions
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "LocalFunctions")]
                get
                {
                    ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>? localFunctions = LocalFunctions;
                    if (localFunctions == null)
                    {
                        return false;
                    }
                    return localFunctions!.Count > 0;
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "CaptureIds")]
            public bool HasCaptureIds
            {
                [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "CaptureIds")]
                get
                {
                    ArrayBuilder<CaptureId>? captureIds = CaptureIds;
                    if (captureIds == null)
                    {
                        return false;
                    }
                    return captureIds!.Count > 0;
                }
            }

            public RegionBuilder(ControlFlowRegionKind kind, ITypeSymbol? exceptionType = null, ImmutableArray<ILocalSymbol> locals = default(ImmutableArray<ILocalSymbol>), bool isStackSpillRegion = false)
            {
                Kind = kind;
                ExceptionType = exceptionType;
                Locals = locals.NullToEmpty();
                IsStackSpillRegion = isStackSpillRegion;
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNull("CaptureIds")]
            public void AddCaptureId(int captureId)
            {
                if (CaptureIds == null)
                {
                    CaptureIds = ArrayBuilder<CaptureId>.GetInstance();
                }
                CaptureIds!.Add(new CaptureId(captureId));
            }

            public void AddCaptureIds(ArrayBuilder<CaptureId>? others)
            {
                if (others != null)
                {
                    if (CaptureIds == null)
                    {
                        CaptureIds = ArrayBuilder<CaptureId>.GetInstance();
                    }
                    CaptureIds!.AddRange(others);
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNull("LocalFunctions")]
            public void Add(IMethodSymbol symbol, ILocalFunctionOperation operation)
            {
                if (LocalFunctions == null)
                {
                    LocalFunctions = ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>.GetInstance();
                }
                LocalFunctions!.Add((symbol, operation));
            }

            public void AddRange(ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>? others)
            {
                if (others != null)
                {
                    if (LocalFunctions == null)
                    {
                        LocalFunctions = ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>.GetInstance();
                    }
                    LocalFunctions!.AddRange(others);
                }
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNull("Regions")]
            public void Add(RegionBuilder region)
            {
                if (Regions == null)
                {
                    Regions = ArrayBuilder<RegionBuilder>.GetInstance();
                }
                region.Enclosing = this;
                Regions!.Add(region);
            }

            public void Remove(RegionBuilder region)
            {
                if (Regions!.Count == 1)
                {
                    Regions!.Clear();
                }
                else
                {
                    Regions!.RemoveAt(Regions!.IndexOf(region));
                }
                region.Enclosing = null;
            }

            public void ReplaceRegion(RegionBuilder toReplace, ArrayBuilder<RegionBuilder> replaceWith)
            {
                int num = ((Regions!.Count != 1) ? Regions!.IndexOf(toReplace) : 0);
                int count = replaceWith.Count;
                if (count == 1)
                {
                    RegionBuilder regionBuilder = replaceWith[0];
                    regionBuilder.Enclosing = this;
                    Regions![num] = regionBuilder;
                }
                else
                {
                    int count2 = Regions!.Count;
                    Regions!.Count = count - 1 + count2;
                    int num2 = count2 - 1;
                    int num3 = Regions!.Count - 1;
                    while (num2 > num)
                    {
                        Regions![num3] = Regions![num2];
                        num2--;
                        num3--;
                    }
                    ArrayBuilder<RegionBuilder>.Enumerator enumerator = replaceWith.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        RegionBuilder current = enumerator.Current;
                        current.Enclosing = this;
                        Regions![num++] = current;
                    }
                }
                toReplace.Enclosing = null;
            }

            [System.Diagnostics.CodeAnalysis.MemberNotNull(new string[] { "FirstBlock", "LastBlock" })]
            public void ExtendToInclude(BasicBlockBuilder block)
            {
                if (FirstBlock == null)
                {
                    if (!HasRegions)
                    {
                        FirstBlock = block;
                        LastBlock = block;
                        return;
                    }
                    FirstBlock = Regions!.First().FirstBlock;
                }
                LastBlock = block;
            }

            public void Free()
            {
                Enclosing = null;
                FirstBlock = null;
                LastBlock = null;
                Regions?.Free();
                Regions = null;
                LocalFunctions?.Free();
                LocalFunctions = null;
                CaptureIds?.Free();
                CaptureIds = null;
            }

            public ControlFlowRegion ToImmutableRegionAndFree(ArrayBuilder<BasicBlockBuilder> blocks, ArrayBuilder<IMethodSymbol> localFunctions, ImmutableDictionary<IMethodSymbol, (ControlFlowRegion region, ILocalFunctionOperation operation, int ordinal)>.Builder localFunctionsMap, ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder? anonymousFunctionsMapOpt, ControlFlowRegion? enclosing)
            {
                ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion region, int ordinal)>.Builder anonymousFunctionsMapOpt2 = anonymousFunctionsMapOpt;
                int count = localFunctions.Count;
                if (HasLocalFunctions)
                {
                    ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>.Enumerator enumerator = LocalFunctions!.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IMethodSymbol item = enumerator.Current.Item1;
                        localFunctions.Add(item);
                    }
                }
                ImmutableArray<ControlFlowRegion> nestedRegions;
                if (HasRegions)
                {
                    ArrayBuilder<ControlFlowRegion> instance = ArrayBuilder<ControlFlowRegion>.GetInstance(Regions!.Count);
                    ArrayBuilder<RegionBuilder>.Enumerator enumerator2 = Regions!.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        RegionBuilder current = enumerator2.Current;
                        instance.Add(current.ToImmutableRegionAndFree(blocks, localFunctions, localFunctionsMap, anonymousFunctionsMapOpt2, null));
                    }
                    nestedRegions = instance.ToImmutableAndFree();
                }
                else
                {
                    nestedRegions = ImmutableArray<ControlFlowRegion>.Empty;
                }
                CaptureIds?.Sort((CaptureId x, CaptureId y) => x.Value.CompareTo(y.Value));
                ControlFlowRegion result = new ControlFlowRegion(Kind, FirstBlock!.Ordinal, LastBlock!.Ordinal, nestedRegions, Locals, LocalFunctions?.SelectAsArray(((IMethodSymbol, ILocalFunctionOperation) tuple) => tuple.Item1) ?? default(ImmutableArray<IMethodSymbol>), CaptureIds?.ToImmutable() ?? default(ImmutableArray<CaptureId>), ExceptionType, enclosing);
                if (HasLocalFunctions)
                {
                    ArrayBuilder<(IMethodSymbol, ILocalFunctionOperation)>.Enumerator enumerator = LocalFunctions!.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var (key, item2) = enumerator.Current;
                        localFunctionsMap.Add(key, (result, item2, count++));
                    }
                }
                int num = FirstBlock!.Ordinal;
                ImmutableArray<ControlFlowRegion>.Enumerator enumerator3 = nestedRegions.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    ControlFlowRegion current2 = enumerator3.Current;
                    for (int i = num; i < current2.FirstBlockOrdinal; i++)
                    {
                        setRegion(blocks[i]);
                    }
                    num = current2.LastBlockOrdinal + 1;
                }
                for (int j = num; j <= LastBlock!.Ordinal; j++)
                {
                    setRegion(blocks[j]);
                }
                Free();
                return result;
                void setRegion(BasicBlockBuilder block)
                {
                    block.Region = result;
                    if (anonymousFunctionsMapOpt2 != null)
                    {
                        (ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion, int)>.Builder, ControlFlowRegion) argument = (anonymousFunctionsMapOpt2, result);
                        if (block.HasStatements)
                        {
                            ArrayBuilder<IOperation>.Enumerator enumerator4 = block.StatementsOpt!.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                IOperation current3 = enumerator4.Current;
                                AnonymousFunctionsMapBuilder.Instance.Visit(current3, argument);
                            }
                        }
                        AnonymousFunctionsMapBuilder.Instance.Visit(block.BranchValue, argument);
                    }
                }
            }
        }

        private readonly Compilation _compilation;

        private readonly BasicBlockBuilder _entry = new BasicBlockBuilder(BasicBlockKind.Entry);

        private readonly BasicBlockBuilder _exit = new BasicBlockBuilder(BasicBlockKind.Exit);

        private readonly ArrayBuilder<BasicBlockBuilder> _blocks;

        private readonly PooledDictionary<BasicBlockBuilder, RegionBuilder> _regionMap;

        private BasicBlockBuilder? _currentBasicBlock;

        private RegionBuilder? _currentRegion;

        private PooledDictionary<ILabelSymbol, BasicBlockBuilder>? _labeledBlocks;

        private bool _haveAnonymousFunction;

        private IOperation? _currentStatement;

        private readonly ArrayBuilder<(EvalStackFrame? frameOpt, IOperation? operationOpt)> _evalStack;

        private int _startSpillingAt;

        private ConditionalAccessOperationTracker _currentConditionalAccessTracker;

        private IOperation? _currentSwitchOperationExpression;

        private IOperation? _forToLoopBinaryOperatorLeftOperand;

        private IOperation? _forToLoopBinaryOperatorRightOperand;

        private IOperation? _currentAggregationGroup;

        private bool _forceImplicit;

        private readonly CaptureIdDispenser _captureIdDispenser;

        private ImplicitInstanceInfo _currentImplicitInstance;

        private RegionBuilder CurrentRegionRequired => _currentRegion;

        private BasicBlockBuilder CurrentBasicBlock
        {
            get
            {
                if (_currentBasicBlock == null)
                {
                    AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
                }
                return _currentBasicBlock;
            }
        }

        private Context GetCurrentContext()
        {
            return new Context(_currentImplicitInstance.ImplicitInstance, _currentImplicitInstance.AnonymousType, _currentImplicitInstance.AnonymousTypePropertyValues?.ToImmutableArray() ?? ImmutableArray<KeyValuePair<IPropertySymbol, IOperation>>.Empty);
        }

        private void SetCurrentContext(in Context context)
        {
            _currentImplicitInstance = new ImplicitInstanceInfo(in context);
        }

        private ControlFlowGraphBuilder(Compilation compilation, CaptureIdDispenser? captureIdDispenser, ArrayBuilder<BasicBlockBuilder> blocks)
        {
            _compilation = compilation;
            _captureIdDispenser = captureIdDispenser ?? new CaptureIdDispenser();
            _blocks = blocks;
            _regionMap = PooledDictionary<BasicBlockBuilder, RegionBuilder>.GetInstance();
            _evalStack = ArrayBuilder<(EvalStackFrame, IOperation)>.GetInstance();
        }

        private bool IsImplicit(IOperation operation)
        {
            if (!_forceImplicit)
            {
                return operation.IsImplicit;
            }
            return true;
        }

        public static ControlFlowGraph Create(IOperation body, ControlFlowGraph? parent = null, ControlFlowRegion? enclosing = null, CaptureIdDispenser? captureIdDispenser = null, in Context context = default(Context))
        {
            ArrayBuilder<BasicBlockBuilder> instance = ArrayBuilder<BasicBlockBuilder>.GetInstance();
            ControlFlowGraphBuilder controlFlowGraphBuilder = new ControlFlowGraphBuilder(((Operation)body).OwningSemanticModel!.Compilation, captureIdDispenser, instance);
            RegionBuilder regionBuilder = new RegionBuilder(ControlFlowRegionKind.Root);
            controlFlowGraphBuilder.EnterRegion(regionBuilder);
            controlFlowGraphBuilder.AppendNewBlock(controlFlowGraphBuilder._entry, linkToPrevious: false);
            controlFlowGraphBuilder._currentBasicBlock = null;
            controlFlowGraphBuilder.SetCurrentContext(in context);
            controlFlowGraphBuilder.EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime));
            switch (body.Kind)
            {
                case OperationKind.LocalFunction:
                    controlFlowGraphBuilder.VisitLocalFunctionAsRoot((ILocalFunctionOperation)body);
                    break;
                case OperationKind.AnonymousFunction:
                    {
                        IAnonymousFunctionOperation anonymousFunctionOperation = (IAnonymousFunctionOperation)body;
                        controlFlowGraphBuilder.VisitStatement(anonymousFunctionOperation.Body);
                        break;
                    }
                default:
                    controlFlowGraphBuilder.VisitStatement(body);
                    break;
            }
            controlFlowGraphBuilder.LeaveRegion();
            controlFlowGraphBuilder.AppendNewBlock(controlFlowGraphBuilder._exit);
            controlFlowGraphBuilder.LeaveRegion();
            controlFlowGraphBuilder._currentImplicitInstance.Free();
            CheckUnresolvedBranches(instance, controlFlowGraphBuilder._labeledBlocks);
            Pack(instance, regionBuilder, controlFlowGraphBuilder._regionMap);
            ArrayBuilder<IMethodSymbol> instance2 = ArrayBuilder<IMethodSymbol>.GetInstance();
            ImmutableDictionary<IMethodSymbol, (ControlFlowRegion, ILocalFunctionOperation, int)>.Builder builder = ImmutableDictionary.CreateBuilder<IMethodSymbol, (ControlFlowRegion, ILocalFunctionOperation, int)>();
            ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion, int)>.Builder builder2 = null;
            if (controlFlowGraphBuilder._haveAnonymousFunction)
            {
                builder2 = ImmutableDictionary.CreateBuilder<IFlowAnonymousFunctionOperation, (ControlFlowRegion, int)>();
            }
            ControlFlowRegion root = regionBuilder.ToImmutableRegionAndFree(instance, instance2, builder, builder2, enclosing);
            regionBuilder = null;
            MarkReachableBlocks(instance);
            controlFlowGraphBuilder._evalStack.Free();
            controlFlowGraphBuilder._regionMap.Free();
            controlFlowGraphBuilder._labeledBlocks?.Free();
            return new ControlFlowGraph(body, parent, controlFlowGraphBuilder._captureIdDispenser, ToImmutableBlocks(instance), root, instance2.ToImmutableAndFree(), builder.ToImmutable(), builder2?.ToImmutable() ?? ImmutableDictionary<IFlowAnonymousFunctionOperation, (ControlFlowRegion, int)>.Empty);
        }

        private static ImmutableArray<BasicBlock> ToImmutableBlocks(ArrayBuilder<BasicBlockBuilder> blockBuilders)
        {
            ArrayBuilder<BasicBlock> builder = ArrayBuilder<BasicBlock>.GetInstance(blockBuilders.Count);
            ArrayBuilder<BasicBlockBuilder>.Enumerator enumerator = blockBuilders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BasicBlockBuilder current = enumerator.Current;
                builder.Add(current.ToImmutable());
            }
            enumerator = blockBuilders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BasicBlockBuilder current2 = enumerator.Current;
                ControlFlowBranch successor = getFallThroughSuccessor(current2);
                ControlFlowBranch conditionalSuccessor = getConditionalSuccessor(current2);
                builder[current2.Ordinal].SetSuccessors(successor, conditionalSuccessor);
            }
            enumerator = blockBuilders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BasicBlockBuilder current3 = enumerator.Current;
                builder[current3.Ordinal].SetPredecessors(current3.ConvertPredecessorsToBranches(builder));
            }
            return builder.ToImmutableAndFree();
            ControlFlowBranch getBranch(in BasicBlockBuilder.Branch branch, BasicBlockBuilder source, bool isConditionalSuccessor)
            {
                return new ControlFlowBranch(builder[source.Ordinal], (branch.Destination != null) ? builder[branch.Destination!.Ordinal] : null, branch.Kind, isConditionalSuccessor);
            }
            ControlFlowBranch? getConditionalSuccessor(BasicBlockBuilder blockBuilder)
            {
                if (!blockBuilder.HasCondition)
                {
                    return null;
                }
                return getBranch(in blockBuilder.Conditional, blockBuilder, isConditionalSuccessor: true);
            }
            ControlFlowBranch? getFallThroughSuccessor(BasicBlockBuilder blockBuilder)
            {
                if (blockBuilder.Kind == BasicBlockKind.Exit)
                {
                    return null;
                }
                return getBranch(in blockBuilder.FallThrough, blockBuilder, isConditionalSuccessor: false);
            }
        }

        private static void MarkReachableBlocks(ArrayBuilder<BasicBlockBuilder> blocks)
        {
            PooledDictionary<ControlFlowRegion, bool> instance = PooledDictionary<ControlFlowRegion, bool>.GetInstance();
            PooledHashSet<ControlFlowRegion> instance2 = PooledHashSet<ControlFlowRegion>.GetInstance();
            MarkReachableBlocks(blocks, 0, blocks.Count - 1, null, instance, instance2, out var _);
            instance.Free();
            instance2.Free();
        }

        private static BitVector MarkReachableBlocks(
            ArrayBuilder<BasicBlockBuilder> blocks,
            int firstBlockOrdinal,
            int lastBlockOrdinal,
            ArrayBuilder<BasicBlockBuilder>? outOfRangeBlocksToVisit,
            PooledDictionary<ControlFlowRegion, bool> continueDispatchAfterFinally,
            PooledHashSet<ControlFlowRegion> dispatchedExceptionsFromRegions,
            out bool fellThrough)
        {
            var visited = BitVector.Empty;
            var toVisit = ArrayBuilder<BasicBlockBuilder>.GetInstance();

            fellThrough = false;
            toVisit.Push(blocks[firstBlockOrdinal]);

            do
            {
                BasicBlockBuilder current = toVisit.Pop();

                if (current.Ordinal < firstBlockOrdinal || current.Ordinal > lastBlockOrdinal)
                {
                    outOfRangeBlocksToVisit.Push(current);
                    continue;
                }

                if (visited[current.Ordinal])
                {
                    continue;
                }

                visited[current.Ordinal] = true;
                current.IsReachable = true;
                bool fallThrough = true;

                if (current.HasCondition)
                {
                    if (current.BranchValue.GetConstantValue() is { IsBoolean: true, BooleanValue: bool constant })
                    {
                        if (constant == (current.ConditionKind == ControlFlowConditionKind.WhenTrue))
                        {
                            followBranch(current, in current.Conditional);
                            fallThrough = false;
                        }
                    }
                    else
                    {
                        followBranch(current, in current.Conditional);
                    }
                }

                if (fallThrough)
                {
                    BasicBlockBuilder.Branch branch = current.FallThrough;
                    followBranch(current, in branch);

                    if (current.Ordinal == lastBlockOrdinal && branch.Kind != ControlFlowBranchSemantics.Throw && branch.Kind != ControlFlowBranchSemantics.Rethrow)
                    {
                        fellThrough = true;
                    }
                }

                // We are using very simple approach:
                // If try block is reachable, we should dispatch an exception from it, even if it is empty.
                // To simplify implementation, we dispatch exception from every reachable basic block and rely
                // on dispatchedExceptionsFromRegions cache to avoid doing duplicate work.
                dispatchException(current.Region);
            }
            while (toVisit.Count != 0);

            toVisit.Free();
            return visited;

            void followBranch(BasicBlockBuilder current, in BasicBlockBuilder.Branch branch)
            {
                switch (branch.Kind)
                {
                    case ControlFlowBranchSemantics.None:
                    case ControlFlowBranchSemantics.ProgramTermination:
                    case ControlFlowBranchSemantics.StructuredExceptionHandling:
                    case ControlFlowBranchSemantics.Throw:
                    case ControlFlowBranchSemantics.Rethrow:
                    case ControlFlowBranchSemantics.Error:
                        return;

                    case ControlFlowBranchSemantics.Regular:
                    case ControlFlowBranchSemantics.Return:

                        if (stepThroughFinally(current.Region, branch.Destination))
                        {
                            toVisit.Add(branch.Destination);
                        }

                        return;

                    default:
                        throw ExceptionUtilities.UnexpectedValue(branch.Kind);
                }
            }

            // Returns whether we should proceed to the destination after finallies were taken care of.
            bool stepThroughFinally(ControlFlowRegion region, BasicBlockBuilder destination)
            {
                int destinationOrdinal = destination.Ordinal;
                while (!region.ContainsBlock(destinationOrdinal))
                {
                    ControlFlowRegion enclosing = region.EnclosingRegion;
                    if (region.Kind == ControlFlowRegionKind.Try && enclosing.Kind == ControlFlowRegionKind.TryAndFinally)
                    {
                        if (!stepThroughSingleFinally(enclosing.NestedRegions[1]))
                        {
                            // The point that continues dispatch is not reachable. Cancel the dispatch.
                            return false;
                        }
                    }

                    region = enclosing;
                }

                return true;
            }

            // Returns whether we should proceed with dispatch after finally was taken care of.
            bool stepThroughSingleFinally(ControlFlowRegion @finally)
            {
                if (!continueDispatchAfterFinally.TryGetValue(@finally, out bool continueDispatch))
                {
                    // For simplicity, we do a complete walk of the finally/filter region in isolation
                    // to make sure that the resume dispatch point is reachable from its beginning.
                    // It could also be reachable through invalid branches into the finally and we don't want to consider
                    // these cases for regular finally handling.
                    BitVector isolated = MarkReachableBlocks(blocks,
                                                             @finally.FirstBlockOrdinal,
                                                             @finally.LastBlockOrdinal,
                                                             outOfRangeBlocksToVisit: toVisit,
                                                             continueDispatchAfterFinally,
                                                             dispatchedExceptionsFromRegions,
                                                             out bool isolatedFellThrough);
                    visited.UnionWith(isolated);

                    continueDispatch = isolatedFellThrough &&
                                       blocks[@finally.LastBlockOrdinal].FallThrough.Kind == ControlFlowBranchSemantics.StructuredExceptionHandling;

                    continueDispatchAfterFinally.Add(@finally, continueDispatch);
                }

                return continueDispatch;
            }

            void dispatchException([DisallowNull()] ControlFlowRegion? fromRegion)
            {
                do
                {
                    if (!dispatchedExceptionsFromRegions.Add(fromRegion))
                    {
                        return;
                    }

                    ControlFlowRegion? enclosing = fromRegion.Kind == ControlFlowRegionKind.Root ? null : fromRegion.EnclosingRegion;
                    if (fromRegion.Kind == ControlFlowRegionKind.Try)
                    {
                        switch (enclosing!.Kind)
                        {
                            case ControlFlowRegionKind.TryAndFinally:
                                if (!stepThroughSingleFinally(enclosing.NestedRegions[1]))
                                {
                                    // The point that continues dispatch is not reachable. Cancel the dispatch.
                                    return;
                                }
                                break;

                            case ControlFlowRegionKind.TryAndCatch:
                                dispatchExceptionThroughCatches(enclosing, startAt: 1);
                                break;

                            default:
                                throw ExceptionUtilities.UnexpectedValue(enclosing.Kind);
                        }
                    }
                    else if (fromRegion.Kind == ControlFlowRegionKind.Filter)
                    {
                        // If filter throws, dispatch is resumed at the next catch with an original exception
                        ControlFlowRegion tryAndCatch = enclosing.EnclosingRegion;

                        int index = tryAndCatch.NestedRegions.IndexOf(enclosing, startIndex: 1);

                        if (index > 0)
                        {
                            dispatchExceptionThroughCatches(tryAndCatch, startAt: index + 1);
                            fromRegion = tryAndCatch;
                            continue;
                        }

                        throw ExceptionUtilities.Unreachable;
                    }

                    fromRegion = enclosing;
                }
                while (fromRegion != null);
            }

            void dispatchExceptionThroughCatches(ControlFlowRegion tryAndCatch, int startAt)
            {
                // For simplicity, we do not try to figure out whether a catch clause definitely
                // handles all exceptions.

                for (int i = startAt; i < tryAndCatch.NestedRegions.Length; i++)
                {
                    ControlFlowRegion @catch = tryAndCatch.NestedRegions[i];

                    switch (@catch.Kind)
                    {
                        case ControlFlowRegionKind.Catch:
                            toVisit.Add(blocks[@catch.FirstBlockOrdinal]);
                            break;

                        case ControlFlowRegionKind.FilterAndHandler:
                            BasicBlockBuilder entryBlock = blocks[@catch.FirstBlockOrdinal];

                            toVisit.Add(entryBlock);
                            break;

                        default:
                            throw ExceptionUtilities.UnexpectedValue(@catch.Kind);
                    }
                }
            }
        }

        private static void Pack(ArrayBuilder<BasicBlockBuilder> blocks, RegionBuilder root, PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap)
        {
            bool flag = true;
            while ((flag | PackRegions(root, blocks, regionMap)) && PackBlocks(blocks, regionMap))
            {
                flag = false;
            }
        }

        private static bool PackRegions(RegionBuilder root, ArrayBuilder<BasicBlockBuilder> blocks, PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap)
        {
            ArrayBuilder<BasicBlockBuilder> blocks2 = blocks;
            PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap2 = regionMap;
            return PackRegion(root);
            bool PackRegion(RegionBuilder region)
            {
                bool result = false;
                if (region.HasRegions)
                {
                    for (int num = region.Regions!.Count - 1; num >= 0; num--)
                    {
                        RegionBuilder regionBuilder = region.Regions![num];
                        if (PackRegion(regionBuilder))
                        {
                            result = true;
                        }
                        if (regionBuilder.Kind == ControlFlowRegionKind.LocalLifetime && regionBuilder.Locals.IsEmpty && !regionBuilder.HasLocalFunctions && !regionBuilder.HasCaptureIds)
                        {
                            MergeSubRegionAndFree(regionBuilder, blocks2, regionMap2);
                            result = true;
                        }
                    }
                }
                switch (region.Kind)
                {
                    case ControlFlowRegionKind.Root:
                    case ControlFlowRegionKind.LocalLifetime:
                    case ControlFlowRegionKind.Try:
                    case ControlFlowRegionKind.Filter:
                    case ControlFlowRegionKind.Catch:
                    case ControlFlowRegionKind.Finally:
                    case ControlFlowRegionKind.StaticLocalInitializer:
                    case ControlFlowRegionKind.ErroneousBody:
                        {
                            ArrayBuilder<RegionBuilder>? regions = region.Regions;
                            if (regions != null && regions!.Count == 1)
                            {
                                RegionBuilder regionBuilder2 = region.Regions![0];
                                if (regionBuilder2.Kind == ControlFlowRegionKind.LocalLifetime && regionBuilder2.FirstBlock == region.FirstBlock && regionBuilder2.LastBlock == region.LastBlock)
                                {
                                    region.Locals = region.Locals.Concat(regionBuilder2.Locals);
                                    region.AddRange(regionBuilder2.LocalFunctions);
                                    region.AddCaptureIds(regionBuilder2.CaptureIds);
                                    MergeSubRegionAndFree(regionBuilder2, blocks2, regionMap2);
                                    result = true;
                                    break;
                                }
                            }
                            if (region.HasRegions)
                            {
                                for (int num2 = region.Regions!.Count - 1; num2 >= 0; num2--)
                                {
                                    RegionBuilder regionBuilder3 = region.Regions![num2];
                                    if (regionBuilder3.Kind == ControlFlowRegionKind.LocalLifetime && !regionBuilder3.HasLocalFunctions && !regionBuilder3.HasRegions && regionBuilder3.FirstBlock == regionBuilder3.LastBlock)
                                    {
                                        BasicBlockBuilder firstBlock = regionBuilder3.FirstBlock;
                                        if (!firstBlock.HasStatements && firstBlock.BranchValue == null)
                                        {
                                            regionMap2[firstBlock] = region;
                                            regionBuilder3.Free();
                                            region.Regions!.RemoveAt(num2);
                                            result = true;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(region.Kind);
                    case ControlFlowRegionKind.FilterAndHandler:
                    case ControlFlowRegionKind.TryAndCatch:
                    case ControlFlowRegionKind.TryAndFinally:
                        break;
                }
                return result;
            }
        }

        private static void MergeSubRegionAndFree(RegionBuilder subRegion, ArrayBuilder<BasicBlockBuilder> blocks, PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap, bool canHaveEmptyRegion = false)
        {
            RegionBuilder enclosing = subRegion.Enclosing;
            if (subRegion.IsEmpty)
            {
                enclosing.Remove(subRegion);
                subRegion.Free();
                return;
            }
            int num = subRegion.FirstBlock!.Ordinal;
            if (subRegion.HasRegions)
            {
                ArrayBuilder<RegionBuilder>.Enumerator enumerator = subRegion.Regions!.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    RegionBuilder current = enumerator.Current;
                    for (int i = num; i < current.FirstBlock!.Ordinal; i++)
                    {
                        regionMap[blocks[i]] = enclosing;
                    }
                    num = current.LastBlock!.Ordinal + 1;
                }
                enclosing.ReplaceRegion(subRegion, subRegion.Regions);
            }
            else
            {
                enclosing.Remove(subRegion);
            }
            for (int j = num; j <= subRegion.LastBlock!.Ordinal; j++)
            {
                regionMap[blocks[j]] = enclosing;
            }
            subRegion.Free();
        }

        private static bool PackBlocks(ArrayBuilder<BasicBlockBuilder> blocks, PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap)
        {
            ArrayBuilder<BasicBlockBuilder> blocks2 = blocks;
            PooledDictionary<BasicBlockBuilder, RegionBuilder> regionMap2 = regionMap;
            ArrayBuilder<RegionBuilder> fromCurrent = null;
            ArrayBuilder<RegionBuilder> fromDestination = null;
            ArrayBuilder<RegionBuilder> fromPredecessor = null;
            ArrayBuilder<BasicBlockBuilder> arrayBuilder = null;
            bool result = false;
            bool flag;
            do
            {
                flag = false;
                int num = blocks2.Count - 1;
                for (int i = 1; i < num; i++)
                {
                    BasicBlockBuilder basicBlockBuilder = blocks2[i];
                    basicBlockBuilder.Ordinal = i;
                    if (basicBlockBuilder.HasStatements)
                    {
                        BasicBlockBuilder singletonPredecessorOrDefault = basicBlockBuilder.GetSingletonPredecessorOrDefault();
                        if (singletonPredecessorOrDefault == null || singletonPredecessorOrDefault.HasCondition || singletonPredecessorOrDefault.Ordinal >= basicBlockBuilder.Ordinal || singletonPredecessorOrDefault.Kind == BasicBlockKind.Entry || singletonPredecessorOrDefault.FallThrough.Destination != basicBlockBuilder || regionMap2[singletonPredecessorOrDefault] != regionMap2[basicBlockBuilder])
                        {
                            continue;
                        }
                        singletonPredecessorOrDefault.MoveStatementsFrom(basicBlockBuilder);
                        flag = true;
                    }
                    ref BasicBlockBuilder.Branch fallThrough = ref basicBlockBuilder.FallThrough;
                    if (!basicBlockBuilder.HasCondition)
                    {
                        if (fallThrough.Destination == basicBlockBuilder)
                        {
                            continue;
                        }
                        RegionBuilder regionBuilder = regionMap2[basicBlockBuilder];
                        if (regionBuilder.FirstBlock == regionBuilder.LastBlock)
                        {
                            if (regionBuilder.Kind == ControlFlowRegionKind.Finally && fallThrough.Destination == null && fallThrough.Kind == ControlFlowBranchSemantics.StructuredExceptionHandling && !basicBlockBuilder.HasPredecessors)
                            {
                                RegionBuilder? enclosing = regionBuilder.Enclosing;
                                RegionBuilder regionBuilder2 = enclosing!.Regions!.First();
                                if (regionBuilder2.Locals.IsEmpty && !regionBuilder2.HasLocalFunctions && !regionBuilder2.HasCaptureIds)
                                {
                                    i = regionBuilder2.FirstBlock!.Ordinal - 1;
                                    MergeSubRegionAndFree(regionBuilder2, blocks2, regionMap2);
                                }
                                else
                                {
                                    regionBuilder2.Kind = ControlFlowRegionKind.LocalLifetime;
                                    i--;
                                }
                                MergeSubRegionAndFree(regionBuilder, blocks2, regionMap2);
                                RegionBuilder enclosing2 = enclosing!.Enclosing;
                                MergeSubRegionAndFree(enclosing, blocks2, regionMap2);
                                num--;
                                removeBlock(basicBlockBuilder, enclosing2);
                                result = true;
                                flag = true;
                            }
                            continue;
                        }
                        if (fallThrough.Kind == ControlFlowBranchSemantics.StructuredExceptionHandling)
                        {
                            if (basicBlockBuilder.HasPredecessors)
                            {
                                BasicBlockBuilder singletonPredecessorOrDefault2 = basicBlockBuilder.GetSingletonPredecessorOrDefault();
                                if (singletonPredecessorOrDefault2 == null || singletonPredecessorOrDefault2.Ordinal != i - 1 || singletonPredecessorOrDefault2.FallThrough.Destination != basicBlockBuilder || singletonPredecessorOrDefault2.Conditional.Destination == basicBlockBuilder || regionMap2[singletonPredecessorOrDefault2] != regionBuilder)
                                {
                                    continue;
                                }
                                singletonPredecessorOrDefault2.FallThrough = basicBlockBuilder.FallThrough;
                            }
                        }
                        else
                        {
                            IOperation branchValue = basicBlockBuilder.BranchValue;
                            if (tryGetImplicitEntryRegion(basicBlockBuilder, regionBuilder) != null && (branchValue != null || fallThrough.Destination != blocks2[i + 1]))
                            {
                                continue;
                            }
                            if (branchValue != null)
                            {
                                if (!basicBlockBuilder.HasPredecessors && fallThrough.Kind == ControlFlowBranchSemantics.Return)
                                {
                                    if (fallThrough.Destination!.Kind != BasicBlockKind.Exit || !branchValue.IsImplicit || branchValue.Kind != OperationKind.LocalReference || !((ILocalReferenceOperation)branchValue).Local.IsFunctionValue)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    BasicBlockBuilder singletonPredecessorOrDefault3 = basicBlockBuilder.GetSingletonPredecessorOrDefault();
                                    if (singletonPredecessorOrDefault3 == null || singletonPredecessorOrDefault3.BranchValue != null || singletonPredecessorOrDefault3.Kind == BasicBlockKind.Entry || regionMap2[singletonPredecessorOrDefault3] != regionBuilder)
                                    {
                                        continue;
                                    }
                                }
                            }
                            RegionBuilder regionBuilder3 = ((fallThrough.Destination == null) ? null : regionMap2[fallThrough.Destination]);
                            if (basicBlockBuilder.HasPredecessors)
                            {
                                if (arrayBuilder == null)
                                {
                                    arrayBuilder = ArrayBuilder<BasicBlockBuilder>.GetInstance();
                                }
                                else
                                {
                                    arrayBuilder.Clear();
                                }
                                basicBlockBuilder.GetPredecessors(arrayBuilder);
                                if (regionBuilder != regionBuilder3)
                                {
                                    fromCurrent?.Clear();
                                    fromDestination?.Clear();
                                    if (!checkBranchesFromPredecessors(arrayBuilder, regionBuilder, regionBuilder3))
                                    {
                                        continue;
                                    }
                                }
                                ArrayBuilder<BasicBlockBuilder>.Enumerator enumerator = arrayBuilder.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    BasicBlockBuilder current = enumerator.Current;
                                    if (tryMergeBranch(current, ref current.FallThrough, basicBlockBuilder) && branchValue != null)
                                    {
                                        current.BranchValue = branchValue;
                                    }
                                    tryMergeBranch(current, ref current.Conditional, basicBlockBuilder);
                                }
                            }
                            fallThrough.Destination?.RemovePredecessor(basicBlockBuilder);
                        }
                        i--;
                        num--;
                        removeBlock(basicBlockBuilder, regionBuilder);
                        result = true;
                        flag = true;
                    }
                    else
                    {
                        if (fallThrough.Kind == ControlFlowBranchSemantics.StructuredExceptionHandling)
                        {
                            continue;
                        }
                        BasicBlockBuilder singletonPredecessorOrDefault4 = basicBlockBuilder.GetSingletonPredecessorOrDefault();
                        if (singletonPredecessorOrDefault4 == null)
                        {
                            continue;
                        }
                        RegionBuilder regionBuilder4 = regionMap2[basicBlockBuilder];
                        if (tryGetImplicitEntryRegion(basicBlockBuilder, regionBuilder4) == null && singletonPredecessorOrDefault4.Kind != 0 && singletonPredecessorOrDefault4.FallThrough.Destination == basicBlockBuilder && !singletonPredecessorOrDefault4.HasCondition && regionMap2[singletonPredecessorOrDefault4] == regionBuilder4)
                        {
                            mergeBranch(singletonPredecessorOrDefault4, ref singletonPredecessorOrDefault4.FallThrough, ref fallThrough);
                            fallThrough.Destination?.RemovePredecessor(basicBlockBuilder);
                            singletonPredecessorOrDefault4.BranchValue = basicBlockBuilder.BranchValue;
                            singletonPredecessorOrDefault4.ConditionKind = basicBlockBuilder.ConditionKind;
                            singletonPredecessorOrDefault4.Conditional = basicBlockBuilder.Conditional;
                            BasicBlockBuilder destination = basicBlockBuilder.Conditional.Destination;
                            if (destination != null)
                            {
                                destination.AddPredecessor(singletonPredecessorOrDefault4);
                                destination.RemovePredecessor(basicBlockBuilder);
                            }
                            i--;
                            num--;
                            removeBlock(basicBlockBuilder, regionBuilder4);
                            result = true;
                            flag = true;
                        }
                    }
                }
                blocks2[0].Ordinal = 0;
                blocks2[num].Ordinal = num;
            }
            while (flag);
            fromCurrent?.Free();
            fromDestination?.Free();
            fromPredecessor?.Free();
            arrayBuilder?.Free();
            return result;
            bool checkBranchesFromPredecessors(ArrayBuilder<BasicBlockBuilder> predecessors, RegionBuilder currentRegion, RegionBuilder? destinationRegionOpt)
            {
                ArrayBuilder<BasicBlockBuilder>.Enumerator enumerator2 = predecessors.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BasicBlockBuilder current2 = enumerator2.Current;
                    RegionBuilder regionBuilder5 = regionMap2[current2];
                    if (regionBuilder5 != currentRegion)
                    {
                        if (destinationRegionOpt == null)
                        {
                            return false;
                        }
                        fromPredecessor?.Clear();
                        collectAncestorsAndSelf(currentRegion, ref fromCurrent);
                        collectAncestorsAndSelf(destinationRegionOpt, ref fromDestination);
                        collectAncestorsAndSelf(regionBuilder5, ref fromPredecessor);
                        int indexOfLastLeftRegion = getIndexOfLastLeftRegion(fromCurrent, fromDestination);
                        int indexOfLastLeftRegion2 = getIndexOfLastLeftRegion(fromPredecessor, fromDestination);
                        int indexOfLastLeftRegion3 = getIndexOfLastLeftRegion(fromPredecessor, fromCurrent);
                        if (fromPredecessor.Count - indexOfLastLeftRegion3 + fromCurrent.Count - indexOfLastLeftRegion != fromPredecessor.Count - indexOfLastLeftRegion2)
                        {
                            return false;
                        }
                    }
                    else if (current2.Kind == BasicBlockKind.Entry && destinationRegionOpt == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            static void collectAncestorsAndSelf([System.Diagnostics.CodeAnalysis.DisallowNull] RegionBuilder? from, [System.Diagnostics.CodeAnalysis.NotNull] ref ArrayBuilder<RegionBuilder>? builder)
            {
                if (builder == null)
                {
                    builder = ArrayBuilder<RegionBuilder>.GetInstance();
                }
                else if (builder!.Count != 0)
                {
                    return;
                }
                do
                {
                    builder!.Add(from);
                    from = from!.Enclosing;
                }
                while (from != null);
                builder!.ReverseContents();
            }
            static int getIndexOfLastLeftRegion(ArrayBuilder<RegionBuilder> from, ArrayBuilder<RegionBuilder> to)
            {
                int j;
                for (j = 0; j < from.Count && j < to.Count && from[j] == to[j]; j++)
                {
                }
                return j;
            }
            static void mergeBranch(BasicBlockBuilder predecessor, ref BasicBlockBuilder.Branch predecessorBranch, ref BasicBlockBuilder.Branch successorBranch)
            {
                predecessorBranch.Destination = successorBranch.Destination;
                successorBranch.Destination?.AddPredecessor(predecessor);
                predecessorBranch.Kind = successorBranch.Kind;
            }
            void removeBlock(BasicBlockBuilder block, RegionBuilder region)
            {
                if (region.FirstBlock == block)
                {
                    BasicBlockBuilder firstBlock = (region.FirstBlock = blocks2[block.Ordinal + 1]);
                    RegionBuilder enclosing3 = region.Enclosing;
                    while (enclosing3 != null && enclosing3.FirstBlock == block)
                    {
                        enclosing3.FirstBlock = firstBlock;
                        enclosing3 = enclosing3.Enclosing;
                    }
                }
                else if (region.LastBlock == block)
                {
                    BasicBlockBuilder lastBlock = (region.LastBlock = blocks2[block.Ordinal - 1]);
                    RegionBuilder enclosing4 = region.Enclosing;
                    while (enclosing4 != null && enclosing4.LastBlock == block)
                    {
                        enclosing4.LastBlock = lastBlock;
                        enclosing4 = enclosing4.Enclosing;
                    }
                }
                regionMap2.Remove(block);
                blocks2.RemoveAt(block.Ordinal);
                block.Free();
            }
            static RegionBuilder? tryGetImplicitEntryRegion(BasicBlockBuilder block, [System.Diagnostics.CodeAnalysis.DisallowNull] RegionBuilder? currentRegion)
            {
                do
                {
                    if (currentRegion!.FirstBlock != block)
                    {
                        return null;
                    }
                    ControlFlowRegionKind kind = currentRegion!.Kind;
                    if ((uint)(kind - 3) <= 1u || kind == ControlFlowRegionKind.Finally)
                    {
                        return currentRegion;
                    }
                    currentRegion = currentRegion!.Enclosing;
                }
                while (currentRegion != null);
                return null;
            }
            static bool tryMergeBranch(BasicBlockBuilder predecessor, ref BasicBlockBuilder.Branch predecessorBranch, BasicBlockBuilder successor)
            {
                if (predecessorBranch.Destination == successor)
                {
                    mergeBranch(predecessor, ref predecessorBranch, ref successor.FallThrough);
                    return true;
                }
                return false;
            }
        }

        private static void CheckUnresolvedBranches(ArrayBuilder<BasicBlockBuilder> blocks, PooledDictionary<ILabelSymbol, BasicBlockBuilder>? labeledBlocks)
        {
            if (labeledBlocks == null)
            {
                return;
            }
            PooledHashSet<BasicBlockBuilder> unresolved = null;
            foreach (BasicBlockBuilder value in labeledBlocks!.Values)
            {
                if (value.Ordinal == -1)
                {
                    if (unresolved == null)
                    {
                        unresolved = PooledHashSet<BasicBlockBuilder>.GetInstance();
                    }
                    unresolved.Add(value);
                }
            }
            if (unresolved != null)
            {
                ArrayBuilder<BasicBlockBuilder>.Enumerator enumerator2 = blocks.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    BasicBlockBuilder current2 = enumerator2.Current;
                    fixupBranch(ref current2.Conditional);
                    fixupBranch(ref current2.FallThrough);
                }
                unresolved.Free();
            }
            void fixupBranch(ref BasicBlockBuilder.Branch branch)
            {
                if (branch.Destination != null && unresolved.Contains(branch.Destination))
                {
                    branch.Destination = null;
                    branch.Kind = ControlFlowBranchSemantics.Error;
                }
            }
        }

        private void VisitStatement(IOperation? operation)
        {
            if (operation != null)
            {
                IOperation currentStatement = _currentStatement;
                _currentStatement = operation;
                EvalStackFrame frame = PushStackFrame();
                AddStatement(base.Visit(operation, null));
                PopStackFrameAndLeaveRegion(frame);
                _currentStatement = currentStatement;
            }
        }

        private void AddStatement(IOperation? statement)
        {
            if (statement != null)
            {
                Operation.SetParentOperation(statement, null);
                CurrentBasicBlock.AddStatement(statement);
            }
        }

        [System.Diagnostics.CodeAnalysis.MemberNotNull("_currentBasicBlock")]
        private void AppendNewBlock(BasicBlockBuilder block, bool linkToPrevious = true)
        {
            if (linkToPrevious)
            {
                BasicBlockBuilder basicBlockBuilder = _blocks.Last();
                if (basicBlockBuilder.FallThrough.Destination == null)
                {
                    LinkBlocks(basicBlockBuilder, block);
                }
            }
            if (block.Ordinal != -1)
            {
                throw ExceptionUtilities.Unreachable;
            }
            block.Ordinal = _blocks.Count;
            _blocks.Add(block);
            _currentBasicBlock = block;
            _currentRegion!.ExtendToInclude(block);
            _regionMap.Add(block, _currentRegion);
        }

        private void EnterRegion(RegionBuilder region, bool spillingStack = false)
        {
            if (!spillingStack)
            {
                SpillEvalStack();
            }
            _currentRegion?.Add(region);
            _currentRegion = region;
            _currentBasicBlock = null;
        }

        private void LeaveRegion()
        {
            if (_currentRegion!.IsEmpty)
            {
                AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
            }
            RegionBuilder currentRegion = _currentRegion;
            _currentRegion = _currentRegion!.Enclosing;
            _currentRegion?.ExtendToInclude(currentRegion.LastBlock);
            _currentBasicBlock = null;
        }

        private static void LinkBlocks(BasicBlockBuilder prevBlock, BasicBlockBuilder nextBlock, ControlFlowBranchSemantics branchKind = ControlFlowBranchSemantics.Regular)
        {
            prevBlock.FallThrough.Destination = nextBlock;
            prevBlock.FallThrough.Kind = branchKind;
            nextBlock.AddPredecessor(prevBlock);
        }

        private void UnconditionalBranch(BasicBlockBuilder nextBlock)
        {
            LinkBlocks(CurrentBasicBlock, nextBlock);
            _currentBasicBlock = null;
        }

        public override IOperation? VisitBlock(IBlockOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.Locals));
            VisitStatements(operation.Operations);
            LeaveRegion();
            return FinishVisitingStatement(operation);
        }

        private void StartVisitingStatement(IOperation operation)
        {
            SpillEvalStack();
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("result")]
        private IOperation? FinishVisitingStatement(IOperation originalOperation, IOperation? result = null)
        {
            if (_currentStatement == originalOperation)
            {
                return result;
            }
            return result ?? MakeInvalidOperation(originalOperation.Syntax, originalOperation.Type, ImmutableArray<IOperation>.Empty);
        }

        private void VisitStatements(ImmutableArray<IOperation> statements)
        {
            for (int i = 0; i < statements.Length && !VisitStatementsOneOrAll(statements[i], statements, i); i++)
            {
            }
        }

        private bool VisitStatementsOneOrAll(IOperation? operation, ImmutableArray<IOperation> statements, int startIndex)
        {
            if (!(operation is IUsingDeclarationOperation operation2))
            {
                if (operation is ILabeledOperation labeledOperation && labeledOperation.Operation != null)
                {
                    return visitPossibleUsingDeclarationInLabel(labeledOperation);
                }
                VisitStatement(operation);
                return false;
            }
            VisitUsingVariableDeclarationOperation(operation2, statements.AsSpan()[(startIndex + 1)..]);
            return true;
            bool visitPossibleUsingDeclarationInLabel(ILabeledOperation labelOperation)
            {
                IOperation currentStatement = _currentStatement;
                _currentStatement = labelOperation;
                StartVisitingStatement(labelOperation);
                VisitLabel(labelOperation.Label);
                bool result = VisitStatementsOneOrAll(labelOperation.Operation, statements, startIndex);
                FinishVisitingStatement(labelOperation);
                _currentStatement = currentStatement;
                return result;
            }
        }

        internal override IOperation? VisitWithStatement(IWithStatementOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            ImplicitInstanceInfo currentImplicitInstance = _currentImplicitInstance;
            _currentImplicitInstance = new ImplicitInstanceInfo(VisitAndCapture(operation.Value));
            VisitStatement(operation.Body);
            _currentImplicitInstance = currentImplicitInstance;
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitConstructorBodyOperation(IConstructorBodyOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.Locals));
            if (operation.Initializer != null)
            {
                VisitStatement(operation.Initializer);
            }
            VisitMethodBodyBaseOperation(operation);
            LeaveRegion();
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitMethodBodyOperation(IMethodBodyOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            VisitMethodBodyBaseOperation(operation);
            return FinishVisitingStatement(operation);
        }

        private void VisitMethodBodyBaseOperation(IMethodBodyBaseOperation operation)
        {
            VisitMethodBodies(operation.BlockBody, operation.ExpressionBody);
        }

        private void VisitMethodBodies(IBlockOperation? blockBody, IBlockOperation? expressionBody)
        {
            if (blockBody != null)
            {
                VisitStatement(blockBody);
                if (expressionBody != null)
                {
                    UnconditionalBranch(_exit);
                    EnterRegion(new RegionBuilder(ControlFlowRegionKind.ErroneousBody));
                    VisitStatement(expressionBody);
                    LeaveRegion();
                }
            }
            else if (expressionBody != null)
            {
                VisitStatement(expressionBody);
            }
        }

        public override IOperation? VisitConditional(IConditionalOperation operation, int? captureIdForResult)
        {
            if (operation == _currentStatement)
            {
                if (operation.WhenFalse == null)
                {
                    BasicBlockBuilder dest = null;
                    VisitConditionalBranch(operation.Condition, ref dest, jumpIfTrue: false);
                    VisitStatement(operation.WhenTrue);
                    AppendNewBlock(dest);
                }
                else
                {
                    BasicBlockBuilder dest2 = null;
                    VisitConditionalBranch(operation.Condition, ref dest2, jumpIfTrue: false);
                    VisitStatement(operation.WhenTrue);
                    BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                    UnconditionalBranch(basicBlockBuilder);
                    AppendNewBlock(dest2);
                    VisitStatement(operation.WhenFalse);
                    AppendNewBlock(basicBlockBuilder);
                }
                return null;
            }
            SpillEvalStack();
            BasicBlockBuilder dest3 = null;
            VisitConditionalBranch(operation.Condition, ref dest3, jumpIfTrue: false);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation result;
            if (operation.WhenTrue is IConversionOperation conversionOperation && conversionOperation.Operand.Kind == OperationKind.Throw)
            {
                base.Visit(conversionOperation.Operand, null);
                UnconditionalBranch(basicBlockBuilder2);
                AppendNewBlock(dest3);
                result = VisitRequired(operation.WhenFalse);
            }
            else if (operation.WhenFalse is IConversionOperation conversionOperation2 && conversionOperation2.Operand.Kind == OperationKind.Throw)
            {
                result = VisitRequired(operation.WhenTrue);
                UnconditionalBranch(basicBlockBuilder2);
                AppendNewBlock(dest3);
                BaseVisitRequired(conversionOperation2.Operand, null);
            }
            else
            {
                RegionBuilder regionBuilder = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, default(ImmutableArray<ILocalSymbol>), isStackSpillRegion: true);
                EnterRegion(regionBuilder);
                int num = captureIdForResult ?? GetNextCaptureId(regionBuilder);
                VisitAndCapture(operation.WhenTrue, num);
                UnconditionalBranch(basicBlockBuilder2);
                AppendNewBlock(dest3);
                VisitAndCapture(operation.WhenFalse, num);
                result = GetCaptureReference(num, operation);
            }
            AppendNewBlock(basicBlockBuilder2);
            return result;
        }

        private void VisitAndCapture(IOperation operation, int captureId)
        {
            EvalStackFrame frame = PushStackFrame();
            IOperation result = BaseVisitRequired(operation, captureId);
            PopStackFrame(frame);
            CaptureResultIfNotAlready(operation.Syntax, captureId, result);
            LeaveRegionIfAny(frame);
        }

        private IOperation VisitAndCapture(IOperation operation)
        {
            EvalStackFrame frame = PushStackFrame();
            PushOperand(BaseVisitRequired(operation, null));
            SpillEvalStack();
            return PopStackFrame(frame, PopOperand());
        }

        private void CaptureResultIfNotAlready(SyntaxNode syntax, int captureId, IOperation result)
        {
            if (result.Kind != OperationKind.FlowCaptureReference || captureId != ((IFlowCaptureReferenceOperation)result).Id.Value)
            {
                SpillEvalStack();
                AddStatement(new FlowCaptureOperation(captureId, syntax, result));
            }
        }

        private EvalStackFrame PushStackFrame()
        {
            EvalStackFrame evalStackFrame = new EvalStackFrame();
            _evalStack.Push((evalStackFrame, null));
            return evalStackFrame;
        }

        private void PopStackFrame(EvalStackFrame frame, bool mergeNestedRegions = true)
        {
            int count = _evalStack.Count;
            if (_startSpillingAt == count)
            {
                _startSpillingAt--;
            }
            _evalStack.Pop();
            if (!(frame.RegionBuilderOpt != null && mergeNestedRegions))
            {
                return;
            }
            while (_currentRegion != frame.RegionBuilderOpt)
            {
                RegionBuilder currentRegion = _currentRegion;
                _currentRegion = currentRegion.Enclosing;
                _currentRegion!.AddCaptureIds(currentRegion.CaptureIds);
                if (!currentRegion.IsEmpty)
                {
                    _currentRegion!.ExtendToInclude(currentRegion.LastBlock);
                }
                MergeSubRegionAndFree(currentRegion, _blocks, _regionMap, canHaveEmptyRegion: true);
            }
        }

        private void PopStackFrameAndLeaveRegion(EvalStackFrame frame)
        {
            PopStackFrame(frame);
            LeaveRegionIfAny(frame);
        }

        private void LeaveRegionIfAny(EvalStackFrame frame)
        {
            RegionBuilder regionBuilderOpt = frame.RegionBuilderOpt;
            if (regionBuilderOpt != null)
            {
                while (_currentRegion != regionBuilderOpt)
                {
                    LeaveRegion();
                }
                LeaveRegion();
            }
        }

        private T PopStackFrame<T>(EvalStackFrame frame, T value)
        {
            PopStackFrame(frame);
            return value;
        }

        private void LeaveRegionsUpTo(RegionBuilder resultCaptureRegion)
        {
            while (_currentRegion != resultCaptureRegion)
            {
                LeaveRegion();
            }
        }

        private int GetNextCaptureId(RegionBuilder owner)
        {
            int nextId = _captureIdDispenser.GetNextId();
            owner.AddCaptureId(nextId);
            return nextId;
        }

        private void SpillEvalStack()
        {
            int num = -1;
            for (int num2 = _startSpillingAt - 1; num2 >= 0; num2--)
            {
                if (_evalStack[num2].frameOpt != null)
                {
                    num = num2;
                    break;
                }
            }
            for (int i = _startSpillingAt; i < _evalStack.Count; i++)
            {
                var (evalStackFrame, operation) = _evalStack[i];
                if (evalStackFrame != null)
                {
                    num = i;
                    evalStackFrame.RegionBuilderOpt = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, default(ImmutableArray<ILocalSymbol>), isStackSpillRegion: true);
                    EnterRegion(evalStackFrame.RegionBuilderOpt, spillingStack: true);
                }
                else
                {
                    if (operation.Kind == OperationKind.FlowCaptureReference || operation.Kind == OperationKind.DeclarationExpression || operation.Kind == OperationKind.Discard || operation.Kind == OperationKind.OmittedArgument)
                    {
                        continue;
                    }
                    RegionBuilder regionBuilder = _evalStack[num].frameOpt!.RegionBuilderOpt;
                    if (_currentRegion != regionBuilder)
                    {
                        PooledHashSet<CaptureId> instance = PooledHashSet<CaptureId>.GetInstance();
                        for (int j = num + 1; j < _evalStack.Count; j++)
                        {
                            IOperation item = _evalStack[j].operationOpt;
                            if (item == null)
                            {
                                continue;
                            }
                            if (j < i)
                            {
                                if (item is IFlowCaptureReferenceOperation flowCaptureReferenceOperation)
                                {
                                    instance.Add(flowCaptureReferenceOperation.Id);
                                }
                            }
                            else
                            {
                                if (j <= i)
                                {
                                    continue;
                                }
                                foreach (IFlowCaptureReferenceOperation item2 in item.DescendantsAndSelf().OfType<IFlowCaptureReferenceOperation>())
                                {
                                    instance.Add(item2.Id);
                                }
                            }
                        }
                        RegionBuilder regionBuilder2 = CurrentRegionRequired;
                        do
                        {
                            if (regionBuilder2.HasCaptureIds && regionBuilder2.CaptureIds.Any((CaptureId id, PooledHashSet<CaptureId> set) => set.Contains(id), instance))
                            {
                                regionBuilder = regionBuilder2;
                                break;
                            }
                            regionBuilder2 = regionBuilder2.Enclosing;
                        }
                        while (regionBuilder2 != regionBuilder);
                        instance.Free();
                    }
                    int nextCaptureId = GetNextCaptureId(regionBuilder);
                    AddStatement(new FlowCaptureOperation(nextCaptureId, operation.Syntax, operation));
                    _evalStack[i] = (null, GetCaptureReference(nextCaptureId, operation));
                    while (_currentRegion != regionBuilder)
                    {
                        LeaveRegion();
                    }
                }
            }
            _startSpillingAt = _evalStack.Count;
        }

        private void PushOperand(IOperation operation)
        {
            _evalStack.Push((null, operation));
        }

        private IOperation PopOperand()
        {
            int count = _evalStack.Count;
            if (_startSpillingAt == count)
            {
                _startSpillingAt--;
            }
            return _evalStack.Pop().operationOpt;
        }

        private IOperation PeekOperand()
        {
            return _evalStack.Peek().operationOpt;
        }

        private void VisitAndPushArray<T>(ImmutableArray<T> array, Func<T, IOperation>? unwrapper = null) where T : IOperation
        {
            ImmutableArray<T>.Enumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                IOperation operation;
                if (unwrapper != null)
                {
                    operation = unwrapper!(current);
                }
                else
                {
                    IOperation operation2 = current;
                    operation = operation2;
                }
                PushOperand(VisitRequired(operation));
            }
        }

        private ImmutableArray<T> PopArray<T>(ImmutableArray<T> originalArray, Func<IOperation, int, ImmutableArray<T>, T>? wrapper = null) where T : IOperation
        {
            int length = originalArray.Length;
            if (length == 0)
            {
                return ImmutableArray<T>.Empty;
            }
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance(length);
            for (int num = length - 1; num >= 0; num--)
            {
                IOperation operation = PopOperand();
                instance.Add((wrapper != null) ? wrapper!(operation, num, originalArray) : ((T)operation));
            }
            instance.ReverseContents();
            return instance.ToImmutableAndFree();
        }

        private ImmutableArray<T> VisitArray<T>(ImmutableArray<T> originalArray, Func<T, IOperation>? unwrapper = null, Func<IOperation, int, ImmutableArray<T>, T>? wrapper = null) where T : IOperation
        {
            VisitAndPushArray(originalArray, unwrapper);
            return PopArray(originalArray, wrapper);
        }

        private ImmutableArray<IArgumentOperation> VisitArguments(ImmutableArray<IArgumentOperation> arguments)
        {
            return VisitArray(arguments, UnwrapArgument, RewriteArgumentFromArray);
        }

        private static IOperation UnwrapArgument(IArgumentOperation argument)
        {
            return argument.Value;
        }

        private IArgumentOperation RewriteArgumentFromArray(IOperation visitedArgument, int index, ImmutableArray<IArgumentOperation> args)
        {
            ArgumentOperation argumentOperation = (ArgumentOperation)args[index];
            return new ArgumentOperation(argumentOperation.ArgumentKind, argumentOperation.Parameter, visitedArgument, argumentOperation.InConversionConvertible, argumentOperation.OutConversionConvertible, null, argumentOperation.Syntax, IsImplicit(argumentOperation));
        }

        public override IOperation VisitSimpleAssignment(ISimpleAssignmentOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(operation.Target));
            IOperation value = VisitRequired(operation.Value);
            return PopStackFrame(frame, new SimpleAssignmentOperation(operation.IsRef, PopOperand(), value, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation)));
        }

        public override IOperation VisitCompoundAssignment(ICompoundAssignmentOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            CompoundAssignmentOperation compoundAssignmentOperation = (CompoundAssignmentOperation)operation;
            PushOperand(VisitRequired(compoundAssignmentOperation.Target));
            IOperation value = VisitRequired(compoundAssignmentOperation.Value);
            return PopStackFrame(frame, new CompoundAssignmentOperation(compoundAssignmentOperation.InConversionConvertible, compoundAssignmentOperation.OutConversionConvertible, operation.OperatorKind, operation.IsLifted, operation.IsChecked, operation.OperatorMethod, PopOperand(), value, null, operation.Syntax, operation.Type, IsImplicit(operation)));
        }

        public override IOperation VisitArrayElementReference(IArrayElementReferenceOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(operation.ArrayReference));
            ImmutableArray<IOperation> indices = VisitArray(operation.Indices);
            IOperation arrayReference = PopOperand();
            PopStackFrame(frame);
            return new ArrayElementReferenceOperation(arrayReference, indices, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        private static bool IsConditional(IBinaryOperation operation)
        {
            BinaryOperatorKind operatorKind = operation.OperatorKind;
            if ((uint)(operatorKind - 13) <= 1u)
            {
                return true;
            }
            return false;
        }

        public override IOperation VisitBinaryOperator(IBinaryOperation operation, int? captureIdForResult)
        {
            if (IsConditional(operation))
            {
                if (operation.OperatorMethod != null)
                {
                    return VisitUserDefinedBinaryConditionalOperator(operation, captureIdForResult);
                }
                if (ITypeSymbolHelpers.IsBooleanType(operation.Type) && ITypeSymbolHelpers.IsBooleanType(operation.LeftOperand.Type) && ITypeSymbolHelpers.IsBooleanType(operation.RightOperand.Type))
                {
                    return VisitBinaryConditionalOperator(operation, sense: true, captureIdForResult, null, null);
                }
                if (operation.IsLifted && ITypeSymbolHelpers.IsNullableOfBoolean(operation.Type) && ITypeSymbolHelpers.IsNullableOfBoolean(operation.LeftOperand.Type) && ITypeSymbolHelpers.IsNullableOfBoolean(operation.RightOperand.Type))
                {
                    return VisitNullableBinaryConditionalOperator(operation, captureIdForResult);
                }
                if (ITypeSymbolHelpers.IsObjectType(operation.Type) && ITypeSymbolHelpers.IsObjectType(operation.LeftOperand.Type) && ITypeSymbolHelpers.IsObjectType(operation.RightOperand.Type))
                {
                    return VisitObjectBinaryConditionalOperator(operation, captureIdForResult);
                }
                if (ITypeSymbolHelpers.IsDynamicType(operation.Type) && (ITypeSymbolHelpers.IsDynamicType(operation.LeftOperand.Type) || ITypeSymbolHelpers.IsDynamicType(operation.RightOperand.Type)))
                {
                    return VisitDynamicBinaryConditionalOperator(operation, captureIdForResult);
                }
            }
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(operation.LeftOperand));
            IOperation rightOperand = VisitRequired(operation.RightOperand);
            return PopStackFrame(frame, new BinaryOperation(operation.OperatorKind, PopOperand(), rightOperand, operation.IsLifted, operation.IsChecked, operation.IsCompareText, operation.OperatorMethod, ((BinaryOperation)operation).UnaryOperatorMethod, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation)));
        }

        public override IOperation VisitTupleBinaryOperator(ITupleBinaryOperation operation, int? captureIdForResult)
        {
            var (leftOperand, rightOperand) = VisitPreservingTupleOperations(operation.LeftOperand, operation.RightOperand);
            return new TupleBinaryOperation(operation.OperatorKind, leftOperand, rightOperand, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitUnaryOperator(IUnaryOperation operation, int? captureIdForResult)
        {
            if (IsBooleanLogicalNot(operation))
            {
                return VisitConditionalExpression(operation, sense: true, captureIdForResult, null, null);
            }
            return new UnaryOperation(operation.OperatorKind, VisitRequired(operation.Operand), operation.IsLifted, operation.IsChecked, operation.OperatorMethod, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        private static bool IsBooleanLogicalNot(IUnaryOperation operation)
        {
            if (operation.OperatorKind == UnaryOperatorKind.Not && operation.OperatorMethod == null && ITypeSymbolHelpers.IsBooleanType(operation.Type))
            {
                return ITypeSymbolHelpers.IsBooleanType(operation.Operand.Type);
            }
            return false;
        }

        private static bool CalculateAndOrSense(IBinaryOperation binOp, bool sense)
        {
            return binOp.OperatorKind switch
            {
                BinaryOperatorKind.ConditionalOr => !sense,
                BinaryOperatorKind.ConditionalAnd => sense,
                _ => throw ExceptionUtilities.UnexpectedValue(binOp.OperatorKind),
            };
        }

        private IOperation VisitBinaryConditionalOperator(IBinaryOperation binOp, bool sense, int? captureIdForResult, BasicBlockBuilder? fallToTrueOpt, BasicBlockBuilder? fallToFalseOpt)
        {
            if (!CalculateAndOrSense(binOp, sense))
            {
                return VisitShortCircuitingOperator(binOp, sense, sense, stopValue: true, captureIdForResult, fallToTrueOpt, fallToFalseOpt);
            }
            return VisitShortCircuitingOperator(binOp, sense, !sense, stopValue: false, captureIdForResult, fallToTrueOpt, fallToFalseOpt);
        }

        private IOperation VisitNullableBinaryConditionalOperator(IBinaryOperation binOp, int? captureIdForResult)
        {
            SpillEvalStack();
            IOperation leftOperand = binOp.LeftOperand;
            IOperation rightOperand = binOp.RightOperand;
            bool num = CalculateAndOrSense(binOp, sense: true);
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder basicBlockBuilder3 = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation operation = VisitAndCapture(leftOperand);
            IOperation operation2 = operation;
            if (num)
            {
                operation2 = negateNullable(operation2);
            }
            operation2 = CallNullableMember(operation2, SpecialMember.System_Nullable_T_GetValueOrDefault);
            ConditionalBranch(operation2, jumpIfTrue: true, basicBlockBuilder3);
            UnconditionalBranch(basicBlockBuilder2);
            int id = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            AppendNewBlock(basicBlockBuilder2);
            EvalStackFrame frame = PushStackFrame();
            IOperation operation3 = VisitAndCapture(rightOperand);
            operation2 = operation3;
            if (!num)
            {
                operation2 = negateNullable(operation2);
            }
            operation2 = CallNullableMember(operation2, SpecialMember.System_Nullable_T_GetValueOrDefault);
            ConditionalBranch(operation2, jumpIfTrue: true, basicBlockBuilder3);
            _currentBasicBlock = null;
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, OperationCloner.CloneOperation(operation3)));
            UnconditionalBranch(basicBlockBuilder);
            PopStackFrameAndLeaveRegion(frame);
            AppendNewBlock(basicBlockBuilder3);
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, OperationCloner.CloneOperation(operation)));
            LeaveRegionsUpTo(currentRegionRequired);
            AppendNewBlock(basicBlockBuilder);
            return GetCaptureReference(id, binOp);
            static IOperation negateNullable(IOperation operand)
            {
                return new UnaryOperation(UnaryOperatorKind.Not, operand, isLifted: true, isChecked: false, null, null, operand.Syntax, operand.Type, null, isImplicit: true);
            }
        }

        private IOperation VisitObjectBinaryConditionalOperator(IBinaryOperation binOp, int? captureIdForResult)
        {
            SpillEvalStack();
            INamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            IOperation leftOperand = binOp.LeftOperand;
            IOperation rightOperand = binOp.RightOperand;
            bool flag = CalculateAndOrSense(binOp, sense: true);
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            EvalStackFrame frame = PushStackFrame();
            IOperation condition = CreateConversion(VisitRequired(leftOperand), specialType);
            ConditionalBranch(condition, flag, basicBlockBuilder2);
            _currentBasicBlock = null;
            PopStackFrameAndLeaveRegion(frame);
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            int nextCaptureId = GetNextCaptureId(currentRegionRequired);
            ConstantValue constantValue = ConstantValue.Create(!flag);
            AddStatement(new FlowCaptureOperation(nextCaptureId, binOp.Syntax, new LiteralOperation(null, leftOperand.Syntax, specialType, constantValue, isImplicit: true)));
            UnconditionalBranch(basicBlockBuilder);
            AppendNewBlock(basicBlockBuilder2);
            frame = PushStackFrame();
            condition = CreateConversion(VisitRequired(rightOperand), specialType);
            AddStatement(new FlowCaptureOperation(nextCaptureId, binOp.Syntax, condition));
            PopStackFrame(frame);
            LeaveRegionsUpTo(currentRegionRequired);
            AppendNewBlock(basicBlockBuilder);
            condition = new FlowCaptureReferenceOperation(nextCaptureId, binOp.Syntax, specialType, null);
            return new ConversionOperation(condition, _compilation.ClassifyConvertibleConversion(condition, binOp.Type, out ConstantValue constantValue2), isTryCast: false, isChecked: false, null, binOp.Syntax, binOp.Type, binOp.GetConstantValue(), isImplicit: true);
        }

        private IOperation CreateConversion(IOperation operand, ITypeSymbol type)
        {
            return new ConversionOperation(operand, _compilation.ClassifyConvertibleConversion(operand, type, out ConstantValue constantValue), isTryCast: false, isChecked: false, null, operand.Syntax, type, constantValue, isImplicit: true);
        }

        private IOperation VisitDynamicBinaryConditionalOperator(IBinaryOperation binOp, int? captureIdForResult)
        {
            SpillEvalStack();
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            INamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            IOperation leftOperand = binOp.LeftOperand;
            IOperation rightOperand = binOp.RightOperand;
            IMethodSymbol unaryOperatorMethod = ((BinaryOperation)binOp).UnaryOperatorMethod;
            bool flag = CalculateAndOrSense(binOp, sense: true);
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation operation = VisitAndCapture(leftOperand);
            IOperation operation2 = operation;
            bool jumpIfTrue;
            if (ITypeSymbolHelpers.IsBooleanType(leftOperand.Type))
            {
                jumpIfTrue = flag;
            }
            else if (ITypeSymbolHelpers.IsDynamicType(leftOperand.Type) || unaryOperatorMethod != null)
            {
                jumpIfTrue = false;
                operation2 = ((unaryOperatorMethod != null && (!ITypeSymbolHelpers.IsBooleanType(unaryOperatorMethod.ReturnType) || (!ITypeSymbolHelpers.IsNullableType(leftOperand.Type) && ITypeSymbolHelpers.IsNullableType(unaryOperatorMethod.Parameters[0].Type)))) ? MakeInvalidOperation(specialType, operation2) : new UnaryOperation(flag ? UnaryOperatorKind.False : UnaryOperatorKind.True, operation2, isLifted: false, isChecked: false, unaryOperatorMethod, null, operation2.Syntax, specialType, null, isImplicit: true));
            }
            else
            {
                operation2 = CreateConversion(operation2, specialType);
                jumpIfTrue = flag;
            }
            ConditionalBranch(operation2, jumpIfTrue, basicBlockBuilder2);
            _currentBasicBlock = null;
            int id = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            IOperation operation3 = OperationCloner.CloneOperation(operation);
            if (!ITypeSymbolHelpers.IsDynamicType(leftOperand.Type))
            {
                operation3 = CreateConversion(operation3, binOp.Type);
            }
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, operation3));
            UnconditionalBranch(basicBlockBuilder);
            AppendNewBlock(basicBlockBuilder2);
            EvalStackFrame frame = PushStackFrame();
            PushOperand(OperationCloner.CloneOperation(operation));
            IOperation rightOperand2 = VisitRequired(rightOperand);
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, new BinaryOperation(flag ? BinaryOperatorKind.And : BinaryOperatorKind.Or, PopOperand(), rightOperand2, isLifted: false, binOp.IsChecked, binOp.IsCompareText, binOp.OperatorMethod, null, null, binOp.Syntax, binOp.Type, binOp.GetConstantValue(), IsImplicit(binOp))));
            PopStackFrameAndLeaveRegion(frame);
            LeaveRegionsUpTo(currentRegionRequired);
            AppendNewBlock(basicBlockBuilder);
            return GetCaptureReference(id, binOp);
        }

        private IOperation VisitUserDefinedBinaryConditionalOperator(IBinaryOperation binOp, int? captureIdForResult)
        {
            SpillEvalStack();
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            INamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            bool isLifted = binOp.IsLifted;
            IOperation leftOperand = binOp.LeftOperand;
            IOperation rightOperand = binOp.RightOperand;
            IMethodSymbol unaryOperatorMethod = ((BinaryOperation)binOp).UnaryOperatorMethod;
            bool flag = CalculateAndOrSense(binOp, sense: true);
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation operation = VisitAndCapture(leftOperand);
            IOperation operation2 = operation;
            if (ITypeSymbolHelpers.IsNullableType(leftOperand.Type))
            {
                if ((unaryOperatorMethod == null) ? isLifted : (!ITypeSymbolHelpers.IsNullableType(unaryOperatorMethod.Parameters[0].Type)))
                {
                    operation2 = MakeIsNullOperation(operation2, specialType);
                    ConditionalBranch(operation2, jumpIfTrue: true, basicBlockBuilder2);
                    _currentBasicBlock = null;
                    operation2 = CallNullableMember(OperationCloner.CloneOperation(operation), SpecialMember.System_Nullable_T_GetValueOrDefault);
                }
            }
            else if (unaryOperatorMethod != null && ITypeSymbolHelpers.IsNullableType(unaryOperatorMethod.Parameters[0].Type))
            {
                operation2 = MakeInvalidOperation(unaryOperatorMethod.Parameters[0].Type, operation2);
            }
            operation2 = ((unaryOperatorMethod == null || !ITypeSymbolHelpers.IsBooleanType(unaryOperatorMethod.ReturnType)) ? MakeInvalidOperation(specialType, operation2) : new UnaryOperation(flag ? UnaryOperatorKind.False : UnaryOperatorKind.True, operation2, isLifted: false, isChecked: false, unaryOperatorMethod, null, operation2.Syntax, unaryOperatorMethod.ReturnType, null, isImplicit: true));
            ConditionalBranch(operation2, jumpIfTrue: false, basicBlockBuilder2);
            _currentBasicBlock = null;
            int id = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, OperationCloner.CloneOperation(operation)));
            UnconditionalBranch(basicBlockBuilder);
            AppendNewBlock(basicBlockBuilder2);
            EvalStackFrame frame = PushStackFrame();
            PushOperand(OperationCloner.CloneOperation(operation));
            IOperation rightOperand2 = VisitRequired(rightOperand);
            AddStatement(new FlowCaptureOperation(id, binOp.Syntax, new BinaryOperation(flag ? BinaryOperatorKind.And : BinaryOperatorKind.Or, PopOperand(), rightOperand2, isLifted, binOp.IsChecked, binOp.IsCompareText, binOp.OperatorMethod, null, null, binOp.Syntax, binOp.Type, binOp.GetConstantValue(), IsImplicit(binOp))));
            PopStackFrameAndLeaveRegion(frame);
            LeaveRegionsUpTo(currentRegionRequired);
            AppendNewBlock(basicBlockBuilder);
            return GetCaptureReference(id, binOp);
        }

        private IOperation VisitShortCircuitingOperator(IBinaryOperation condition, bool sense, bool stopSense, bool stopValue, int? captureIdForResult, BasicBlockBuilder? fallToTrueOpt, BasicBlockBuilder? fallToFalseOpt)
        {
            SpillEvalStack();
            ref BasicBlockBuilder reference = ref stopValue ? ref fallToTrueOpt : ref fallToFalseOpt;
            bool num = reference == null;
            VisitConditionalBranch(condition.LeftOperand, ref reference, stopSense);
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            int num2 = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            CaptureResultIfNotAlready(result: VisitConditionalExpression(condition.RightOperand, sense, num2, fallToTrueOpt, fallToFalseOpt), syntax: condition.RightOperand.Syntax, captureId: num2);
            LeaveRegionsUpTo(currentRegionRequired);
            if (num)
            {
                BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                UnconditionalBranch(basicBlockBuilder);
                AppendNewBlock(reference);
                ConstantValue constantValue = ConstantValue.Create(stopValue);
                object obj;
                if (reference.GetSingletonPredecessorOrDefault() == null)
                {
                    obj = condition;
                }
                else
                {
                    obj = condition.LeftOperand;
                }
                SyntaxNode syntax = ((IOperation)obj).Syntax;
                AddStatement(new FlowCaptureOperation(num2, syntax, new LiteralOperation(null, syntax, condition.Type, constantValue, isImplicit: true)));
                AppendNewBlock(basicBlockBuilder);
            }
            return GetCaptureReference(num2, condition);
        }

        private IOperation VisitConditionalExpression(IOperation condition, bool sense, int? captureIdForResult, BasicBlockBuilder? fallToTrueOpt, BasicBlockBuilder? fallToFalseOpt)
        {
            IUnaryOperation unaryOperation = null;
            while (true)
            {
                IOperation operation = condition;
                if (!(operation is IParenthesizedOperation parenthesizedOperation))
                {
                    if (!(operation is IUnaryOperation unaryOperation2) || !IsBooleanLogicalNot(unaryOperation2))
                    {
                        break;
                    }
                    unaryOperation = unaryOperation2;
                    condition = unaryOperation2.Operand;
                    sense = !sense;
                }
                else
                {
                    condition = parenthesizedOperation.Operand;
                }
            }
            if (condition.Kind == OperationKind.Binary)
            {
                IBinaryOperation binOp = (IBinaryOperation)condition;
                if (IsBooleanConditionalOperator(binOp))
                {
                    return VisitBinaryConditionalOperator(binOp, sense, captureIdForResult, fallToTrueOpt, fallToFalseOpt);
                }
            }
            condition = VisitRequired(condition);
            if (!sense)
            {
                if (unaryOperation == null)
                {
                    return new UnaryOperation(UnaryOperatorKind.Not, condition, isLifted: false, isChecked: false, null, null, condition.Syntax, condition.Type, null, isImplicit: true);
                }
                return new UnaryOperation(unaryOperation.OperatorKind, condition, unaryOperation.IsLifted, unaryOperation.IsChecked, unaryOperation.OperatorMethod, null, unaryOperation.Syntax, unaryOperation.Type, unaryOperation.GetConstantValue(), IsImplicit(unaryOperation));
            }
            return condition;
        }

        private static bool IsBooleanConditionalOperator(IBinaryOperation binOp)
        {
            if (IsConditional(binOp) && binOp.OperatorMethod == null && ITypeSymbolHelpers.IsBooleanType(binOp.Type) && ITypeSymbolHelpers.IsBooleanType(binOp.LeftOperand.Type))
            {
                return ITypeSymbolHelpers.IsBooleanType(binOp.RightOperand.Type);
            }
            return false;
        }

        private void VisitConditionalBranch(IOperation condition, [System.Diagnostics.CodeAnalysis.NotNull] ref BasicBlockBuilder? dest, bool jumpIfTrue)
        {
            SpillEvalStack();
            VisitConditionalBranchCore(condition, ref dest, jumpIfTrue);
        }

        private void VisitConditionalBranchCore(IOperation condition, [System.Diagnostics.CodeAnalysis.NotNull] ref BasicBlockBuilder? dest, bool jumpIfTrue)
        {
            while (true)
            {
                if (condition.Kind == OperationKind.Parenthesized)
                {
                    condition = ((IParenthesizedOperation)condition).Operand;
                    continue;
                }
                switch (condition.Kind)
                {
                    case OperationKind.Binary:
                        {
                            IBinaryOperation binaryOperation = (IBinaryOperation)condition;
                            if (IsBooleanConditionalOperator(binaryOperation))
                            {
                                if (CalculateAndOrSense(binaryOperation, jumpIfTrue))
                                {
                                    BasicBlockBuilder dest2 = null;
                                    VisitConditionalBranchCore(binaryOperation.LeftOperand, ref dest2, !jumpIfTrue);
                                    VisitConditionalBranchCore(binaryOperation.RightOperand, ref dest, jumpIfTrue);
                                    AppendNewBlock(dest2);
                                    return;
                                }
                                VisitConditionalBranchCore(binaryOperation.LeftOperand, ref dest, jumpIfTrue);
                                condition = binaryOperation.RightOperand;
                                continue;
                            }
                            break;
                        }
                    case OperationKind.Unary:
                        {
                            IUnaryOperation unaryOperation = (IUnaryOperation)condition;
                            if (IsBooleanLogicalNot(unaryOperation))
                            {
                                jumpIfTrue = !jumpIfTrue;
                                condition = unaryOperation.Operand;
                                continue;
                            }
                            break;
                        }
                    case OperationKind.Conditional:
                        if (ITypeSymbolHelpers.IsBooleanType(condition.Type))
                        {
                            IConditionalOperation conditionalOperation = (IConditionalOperation)condition;
                            if (ITypeSymbolHelpers.IsBooleanType(conditionalOperation.WhenTrue.Type) && ITypeSymbolHelpers.IsBooleanType(conditionalOperation.WhenFalse!.Type))
                            {
                                BasicBlockBuilder dest3 = null;
                                VisitConditionalBranchCore(conditionalOperation.Condition, ref dest3, jumpIfTrue: false);
                                VisitConditionalBranchCore(conditionalOperation.WhenTrue, ref dest, jumpIfTrue);
                                BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                                UnconditionalBranch(basicBlockBuilder);
                                AppendNewBlock(dest3);
                                VisitConditionalBranchCore(conditionalOperation.WhenFalse, ref dest, jumpIfTrue);
                                AppendNewBlock(basicBlockBuilder);
                                return;
                            }
                        }
                        break;
                    case OperationKind.Coalesce:
                        if (ITypeSymbolHelpers.IsBooleanType(condition.Type))
                        {
                            ICoalesceOperation coalesceOperation = (ICoalesceOperation)condition;
                            if (ITypeSymbolHelpers.IsBooleanType(coalesceOperation.WhenNull.Type))
                            {
                                BasicBlockBuilder basicBlockBuilder2 = new(BasicBlockKind.Block);
                                EvalStackFrame frame = PushStackFrame();
                                IOperation condition2 = NullCheckAndConvertCoalesceValue(coalesceOperation, basicBlockBuilder2);
                                dest ??= new BasicBlockBuilder(BasicBlockKind.Block);
                                ConditionalBranch(condition2, jumpIfTrue, dest);
                                _currentBasicBlock = null;
                                BasicBlockBuilder basicBlockBuilder3 = new(BasicBlockKind.Block);
                                UnconditionalBranch(basicBlockBuilder3);
                                PopStackFrameAndLeaveRegion(frame);
                                AppendNewBlock(basicBlockBuilder2);
                                VisitConditionalBranchCore(coalesceOperation.WhenNull, ref dest, jumpIfTrue);
                                AppendNewBlock(basicBlockBuilder3);
                                return;
                            }
                        }
                        break;
                    case OperationKind.Conversion:
                        {
                            IConversionOperation conversionOperation = (IConversionOperation)condition;
                            if (conversionOperation.Operand.Kind == OperationKind.Throw)
                            {
                                base.Visit(conversionOperation.Operand, null);
                                dest = dest ?? new BasicBlockBuilder(BasicBlockKind.Block);
                                return;
                            }
                            break;
                        }
                }
                break;
            }
            EvalStackFrame frame2 = PushStackFrame();
            condition = VisitRequired(condition);
            dest = dest ?? new BasicBlockBuilder(BasicBlockKind.Block);
            ConditionalBranch(condition, jumpIfTrue, dest);
            _currentBasicBlock = null;
            PopStackFrameAndLeaveRegion(frame2);
        }

        private void ConditionalBranch(IOperation condition, bool jumpIfTrue, BasicBlockBuilder destination)
        {
            BasicBlockBuilder currentBasicBlock = CurrentBasicBlock;
            BasicBlockBuilder.Branch conditional = RegularBranch(destination);
            Operation.SetParentOperation(condition, null);
            conditional.Destination!.AddPredecessor(currentBasicBlock);
            currentBasicBlock.BranchValue = condition;
            currentBasicBlock.ConditionKind = ((!jumpIfTrue) ? ControlFlowConditionKind.WhenFalse : ControlFlowConditionKind.WhenTrue);
            currentBasicBlock.Conditional = conditional;
        }

        private IOperation NullCheckAndConvertCoalesceValue(ICoalesceOperation operation, BasicBlockBuilder whenNull)
        {
            IOperation value = operation.Value;
            SyntaxNode syntax = value.Syntax;
            ITypeSymbol type = value.Type;
            PushOperand(VisitRequired(value));
            SpillEvalStack();
            IOperation operation2 = PopOperand();
            ConditionalBranch(MakeIsNullOperation(operation2), jumpIfTrue: true, whenNull);
            _currentBasicBlock = null;
            CommonConversion valueConversion = operation.ValueConversion;
            IOperation operation3 = OperationCloner.CloneOperation(operation2);
            IOperation operation4 = null;
            if (valueConversion.Exists)
            {
                IOperation operation5 = ((!ITypeSymbolHelpers.IsNullableType(type) || (valueConversion.IsIdentity && ITypeSymbolHelpers.IsNullableType(operation.Type))) ? operation3 : TryCallNullableMember(operation3, SpecialMember.System_Nullable_T_GetValueOrDefault));
                if (operation5 != null)
                {
                    operation4 = ((!valueConversion.IsIdentity) ? new ConversionOperation(operation5, ((CoalesceOperation)operation).ValueConversionConvertible, isTryCast: false, isChecked: false, null, syntax, operation.Type, null, isImplicit: true) : operation5);
                }
            }
            if (operation4 == null)
            {
                operation4 = MakeInvalidOperation(operation.Type, operation3);
            }
            return operation4;
        }

        public override IOperation VisitCoalesce(ICoalesceOperation operation, int? captureIdForResult)
        {
            SpillEvalStack();
            IConversionOperation conversionOperation = operation.WhenNull as IConversionOperation;
            bool num = conversionOperation != null && conversionOperation.Operand.Kind == OperationKind.Throw;
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            EvalStackFrame frame = PushStackFrame();
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation operation2 = NullCheckAndConvertCoalesceValue(operation, basicBlockBuilder);
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            IOperation result;
            if (num)
            {
                result = operation2;
                UnconditionalBranch(basicBlockBuilder2);
                PopStackFrame(frame);
                AppendNewBlock(basicBlockBuilder);
                base.Visit(conversionOperation.Operand, null);
            }
            else
            {
                int num2 = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
                AddStatement(new FlowCaptureOperation(num2, operation.Value.Syntax, operation2));
                result = GetCaptureReference(num2, operation);
                UnconditionalBranch(basicBlockBuilder2);
                PopStackFrameAndLeaveRegion(frame);
                AppendNewBlock(basicBlockBuilder);
                VisitAndCapture(operation.WhenNull, num2);
                LeaveRegionsUpTo(currentRegionRequired);
            }
            AppendNewBlock(basicBlockBuilder2);
            return result;
        }

        public override IOperation? VisitCoalesceAssignment(ICoalesceAssignmentOperation operation, int? captureIdForResult)
        {
            ICoalesceAssignmentOperation operation2 = operation;
            SpillEvalStack();
            bool isStatement = _currentStatement == operation2 || operation2.Parent!.Kind == OperationKind.ExpressionStatement;
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(operation2.Target));
            SpillEvalStack();
            IOperation locationCapture = PopOperand();
            EvalStackFrame valueFrame = PushStackFrame();
            SpillEvalStack();
            int nextCaptureId = GetNextCaptureId(valueFrame.RegionBuilderOpt);
            AddStatement(new FlowCaptureOperation(nextCaptureId, locationCapture.Syntax, locationCapture));
            IOperation valueCapture = GetCaptureReference(nextCaptureId, locationCapture);
            BasicBlockBuilder whenNull = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder afterCoalesce = new BasicBlockBuilder(BasicBlockKind.Block);
            int resultCaptureId = (isStatement ? (-1) : (captureIdForResult ?? GetNextCaptureId(currentRegionRequired)));
            IOperation target = operation2.Target;
            if (target != null && target.Type?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && ((INamedTypeSymbol)operation2.Target.Type).TypeArguments[0].Equals(operation2.Type))
            {
                nullableValueTypeReturn();
            }
            else
            {
                standardReturn();
            }
            PopStackFrame(frame);
            LeaveRegionsUpTo(currentRegionRequired);
            AppendNewBlock(afterCoalesce);
            if (!isStatement)
            {
                return GetCaptureReference(resultCaptureId, operation2);
            }
            return null;
            void nullableValueTypeReturn()
            {
                int id = -1;
                EvalStackFrame evalStackFrame = null;
                if (!isStatement)
                {
                    evalStackFrame = PushStackFrame();
                    SpillEvalStack();
                    id = GetNextCaptureId(evalStackFrame.RegionBuilderOpt);
                    AddStatement(new FlowCaptureOperation(id, operation2.Target.Syntax, CallNullableMember(valueCapture, SpecialMember.System_Nullable_T_GetValueOrDefault)));
                }
                ConditionalBranch(CallNullableMember(OperationCloner.CloneOperation(valueCapture), SpecialMember.System_Nullable_T_get_HasValue), jumpIfTrue: false, whenNull);
                if (!isStatement)
                {
                    _currentBasicBlock = null;
                    AddStatement(new FlowCaptureOperation(resultCaptureId, operation2.Syntax, GetCaptureReference(id, operation2.Target)));
                    PopStackFrame(evalStackFrame);
                }
                PopStackFrame(valueFrame);
                UnconditionalBranch(afterCoalesce);
                AppendNewBlock(whenNull);
                EvalStackFrame evalStackFrame2 = PushStackFrame();
                SpillEvalStack();
                IOperation operation4 = VisitRequired(operation2.Value);
                if (!isStatement)
                {
                    int nextCaptureId2 = GetNextCaptureId(evalStackFrame2.RegionBuilderOpt);
                    AddStatement(new FlowCaptureOperation(nextCaptureId2, operation4.Syntax, operation4));
                    operation4 = GetCaptureReference(nextCaptureId2, operation4);
                    AddStatement(new FlowCaptureOperation(resultCaptureId, operation2.Syntax, GetCaptureReference(nextCaptureId2, operation4)));
                }
                AddStatement(new SimpleAssignmentOperation(isRef: false, OperationCloner.CloneOperation(locationCapture), CreateConversion(operation4, operation2.Target.Type), null, operation2.Syntax, operation2.Target.Type, operation2.GetConstantValue(), isImplicit: true));
                PopStackFrameAndLeaveRegion(evalStackFrame2);
            }
            void standardReturn()
            {
                ConditionalBranch(MakeIsNullOperation(valueCapture), jumpIfTrue: true, whenNull);
                if (!isStatement)
                {
                    _currentBasicBlock = null;
                    AddStatement(new FlowCaptureOperation(resultCaptureId, operation2.Syntax, OperationCloner.CloneOperation(valueCapture)));
                }
                PopStackFrameAndLeaveRegion(valueFrame);
                UnconditionalBranch(afterCoalesce);
                AppendNewBlock(whenNull);
                EvalStackFrame frame2 = PushStackFrame();
                IOperation value = VisitRequired(operation2.Value);
                IOperation operation3 = new SimpleAssignmentOperation(isRef: false, OperationCloner.CloneOperation(locationCapture), value, null, operation2.Syntax, operation2.Type, operation2.GetConstantValue(), isImplicit: true);
                if (isStatement)
                {
                    AddStatement(operation3);
                }
                else
                {
                    AddStatement(new FlowCaptureOperation(resultCaptureId, operation2.Syntax, operation3));
                }
                PopStackFrameAndLeaveRegion(frame2);
            }
        }

        private static BasicBlockBuilder.Branch RegularBranch(BasicBlockBuilder destination)
        {
            BasicBlockBuilder.Branch result = default(BasicBlockBuilder.Branch);
            result.Destination = destination;
            result.Kind = ControlFlowBranchSemantics.Regular;
            return result;
        }

        private static IOperation MakeInvalidOperation(ITypeSymbol? type, IOperation child)
        {
            return new InvalidOperation(ImmutableArray.Create(child), null, child.Syntax, type, null, isImplicit: true);
        }

        private static IOperation MakeInvalidOperation(SyntaxNode syntax, ITypeSymbol? type, IOperation child1, IOperation child2)
        {
            return MakeInvalidOperation(syntax, type, ImmutableArray.Create(child1, child2));
        }

        private static IOperation MakeInvalidOperation(SyntaxNode syntax, ITypeSymbol? type, ImmutableArray<IOperation> children)
        {
            return new InvalidOperation(children, null, syntax, type, null, isImplicit: true);
        }

        private IsNullOperation MakeIsNullOperation(IOperation operand)
        {
            return MakeIsNullOperation(operand, _compilation.GetSpecialType(SpecialType.System_Boolean));
        }

        private static IsNullOperation MakeIsNullOperation(IOperation operand, ITypeSymbol booleanType)
        {
            ConstantValue constantValue = operand.GetConstantValue();
            object obj;
            if ((object)constantValue != null)
            {
                bool isNull = constantValue.IsNull;
                obj = ConstantValue.Create(isNull);
            }
            else
            {
                obj = null;
            }
            ConstantValue constantValue2 = (ConstantValue)obj;
            return new IsNullOperation(operand.Syntax, operand, booleanType, constantValue2);
        }

        private IOperation? TryCallNullableMember(IOperation value, SpecialMember nullableMember)
        {
            ITypeSymbol type = value.Type;
            IMethodSymbol methodSymbol = (IMethodSymbol)(_compilation.CommonGetSpecialTypeMember(nullableMember)?.GetISymbol());
            if (methodSymbol != null)
            {
                ImmutableArray<ISymbol>.Enumerator enumerator = type.GetMembers(methodSymbol.Name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ISymbol current = enumerator.Current;
                    if (current.OriginalDefinition.Equals(methodSymbol))
                    {
                        methodSymbol = (IMethodSymbol)current;
                        return new InvocationOperation(methodSymbol, value, isVirtual: false, ImmutableArray<IArgumentOperation>.Empty, null, value.Syntax, methodSymbol.ReturnType, isImplicit: true);
                    }
                }
            }
            return null;
        }

        private IOperation CallNullableMember(IOperation value, SpecialMember nullableMember)
        {
            return TryCallNullableMember(value, nullableMember) ?? MakeInvalidOperation(ITypeSymbolHelpers.GetNullableUnderlyingType(value.Type), value);
        }

        public override IOperation? VisitConditionalAccess(IConditionalAccessOperation operation, int? captureIdForResult)
        {
            SpillEvalStack();
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            int num;
            if (_currentStatement != operation)
            {
                if (_currentStatement == operation.Parent)
                {
                    IOperation? currentStatement = _currentStatement;
                    num = ((currentStatement != null && currentStatement!.Kind == OperationKind.ExpressionStatement) ? 1 : 0);
                }
                else
                {
                    num = 0;
                }
            }
            else
            {
                num = 1;
            }
            bool flag = (byte)num != 0;
            EvalStackFrame frame = null;
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance();
            if (!flag)
            {
                frame = PushStackFrame();
            }
            IConditionalAccessOperation conditionalAccessOperation = operation;
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            ConditionalAccessOperationTracker previousTracker = _currentConditionalAccessTracker;
            _currentConditionalAccessTracker = new ConditionalAccessOperationTracker(instance, basicBlockBuilder);
            IOperation operation2;
            while (true)
            {
                operation2 = conditionalAccessOperation.Operation;
                if (!isConditionalAccessInstancePresentInChildren(conditionalAccessOperation.WhenNotNull))
                {
                    VisitConditionalAccessTestExpression(operation2);
                    break;
                }
                instance.Push(operation2);
                if (!(conditionalAccessOperation.WhenNotNull is IConditionalAccessOperation conditionalAccessOperation2))
                {
                    break;
                }
                conditionalAccessOperation = conditionalAccessOperation2;
            }
            if (flag)
            {
                IOperation operation3 = VisitRequired(conditionalAccessOperation.WhenNotNull);
                resetConditionalAccessTracker();
                if (_currentStatement != operation)
                {
                    IExpressionStatementOperation expressionStatementOperation = (IExpressionStatementOperation)_currentStatement;
                    operation3 = new ExpressionStatementOperation(operation3, null, expressionStatementOperation.Syntax, IsImplicit(expressionStatementOperation));
                }
                AddStatement(operation3);
                AppendNewBlock(basicBlockBuilder);
                return null;
            }
            int num2 = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            if (ITypeSymbolHelpers.IsNullableType(operation.Type) && !ITypeSymbolHelpers.IsNullableType(conditionalAccessOperation.WhenNotNull.Type))
            {
                IOperation operand = VisitRequired(conditionalAccessOperation.WhenNotNull);
                AddStatement(new FlowCaptureOperation(num2, conditionalAccessOperation.WhenNotNull.Syntax, MakeNullable(operand, operation.Type)));
            }
            else
            {
                CaptureResultIfNotAlready(conditionalAccessOperation.WhenNotNull.Syntax, num2, VisitRequired(conditionalAccessOperation.WhenNotNull, num2));
            }
            PopStackFrame(frame);
            LeaveRegionsUpTo(currentRegionRequired);
            resetConditionalAccessTracker();
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
            UnconditionalBranch(basicBlockBuilder2);
            AppendNewBlock(basicBlockBuilder);
            object obj;
            if (operation.Operation != operation2)
            {
                obj = operation;
            }
            else
            {
                obj = operation2;
            }
            SyntaxNode syntax = ((IOperation)obj).Syntax;
            AddStatement(new FlowCaptureOperation(num2, syntax, new DefaultValueOperation(null, syntax, operation.Type, (operation.Type!.IsReferenceType && !ITypeSymbolHelpers.IsNullableType(operation.Type)) ? ConstantValue.Null : null, isImplicit: true)));
            AppendNewBlock(basicBlockBuilder2);
            return GetCaptureReference(num2, operation);
            static bool checkInvalidChildren(InvalidOperation operation)
            {
                foreach (IOperation childOperation in operation.ChildOperations)
                {
                    if (childOperation is IConditionalAccessInstanceOperation || isConditionalAccessInstancePresentInChildren(childOperation))
                    {
                        return true;
                    }
                }
                return false;
            }
            static bool isConditionalAccessInstancePresentInChildren(IOperation operation)
            {
                if (operation is InvalidOperation operation4)
                {
                    return checkInvalidChildren(operation4);
                }
                Operation operation5 = (Operation)operation;
                while (true)
                {
                    Operation.Enumerator enumerator2 = operation5.ChildOperations.GetEnumerator();
                    if (!enumerator2.MoveNext())
                    {
                        break;
                    }
                    if (enumerator2.Current is IConditionalAccessInstanceOperation)
                    {
                        return true;
                    }
                    if (enumerator2.Current is InvalidOperation operation6)
                    {
                        return checkInvalidChildren(operation6);
                    }
                    operation5 = (Operation)enumerator2.Current;
                }
                return false;
            }
            void resetConditionalAccessTracker()
            {
                _currentConditionalAccessTracker.Free();
                _currentConditionalAccessTracker = previousTracker;
            }
        }

        public override IOperation VisitConditionalAccessInstance(IConditionalAccessInstanceOperation operation, int? captureIdForResult)
        {
            IOperation testExpression = _currentConditionalAccessTracker.Operations.Pop();
            return VisitConditionalAccessTestExpression(testExpression);
        }

        private IOperation VisitConditionalAccessTestExpression(IOperation testExpression)
        {
            _ = testExpression.Syntax;
            ITypeSymbol? type = testExpression.Type;
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(testExpression));
            SpillEvalStack();
            IOperation operation = PopOperand();
            PopStackFrame(frame);
            ConditionalBranch(MakeIsNullOperation(operation), jumpIfTrue: true, _currentConditionalAccessTracker.WhenNull);
            _currentBasicBlock = null;
            IOperation operation2 = OperationCloner.CloneOperation(operation);
            if (ITypeSymbolHelpers.IsNullableType(type))
            {
                operation2 = CallNullableMember(operation2, SpecialMember.System_Nullable_T_GetValueOrDefault);
            }
            return operation2;
        }

        public override IOperation? VisitExpressionStatement(IExpressionStatementOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            IOperation operation2 = Visit(operation.Operation);
            if (operation2 == null)
            {
                return FinishVisitingStatement(operation);
            }
            if (operation.Operation.Kind == OperationKind.Throw)
            {
                return FinishVisitingStatement(operation);
            }
            return FinishVisitingStatement(operation, new ExpressionStatementOperation(operation2, null, operation.Syntax, IsImplicit(operation)));
        }

        public override IOperation? VisitWhileLoop(IWhileLoopOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.Locals);
            BasicBlockBuilder labeledOrNewBlock = GetLabeledOrNewBlock(operation.ContinueLabel);
            BasicBlockBuilder dest = GetLabeledOrNewBlock(operation.ExitLabel);
            if (operation.ConditionIsTop)
            {
                AppendNewBlock(labeledOrNewBlock);
                EnterRegion(region);
                VisitConditionalBranch(operation.Condition, ref dest, operation.ConditionIsUntil);
                VisitStatement(operation.Body);
                UnconditionalBranch(labeledOrNewBlock);
            }
            else
            {
                BasicBlockBuilder dest2 = new BasicBlockBuilder(BasicBlockKind.Block);
                AppendNewBlock(dest2);
                EnterRegion(region);
                VisitStatement(operation.Body);
                AppendNewBlock(labeledOrNewBlock);
                if (operation.Condition != null)
                {
                    VisitConditionalBranch(operation.Condition, ref dest2, !operation.ConditionIsUntil);
                }
                else
                {
                    UnconditionalBranch(dest2);
                }
            }
            LeaveRegion();
            AppendNewBlock(dest);
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitTry(ITryOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            BasicBlockBuilder labeledOrNewBlock = GetLabeledOrNewBlock(operation.ExitLabel);
            if (operation.Catches.IsEmpty && operation.Finally == null)
            {
                VisitStatement(operation.Body);
                AppendNewBlock(labeledOrNewBlock);
                return FinishVisitingStatement(operation);
            }
            RegionBuilder regionBuilder = null;
            bool flag = operation.Finally != null;
            if (flag)
            {
                regionBuilder = new RegionBuilder(ControlFlowRegionKind.TryAndFinally);
                EnterRegion(regionBuilder);
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.Try));
            }
            bool num = !operation.Catches.IsEmpty;
            if (num)
            {
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.TryAndCatch));
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.Try));
            }
            VisitStatement(operation.Body);
            UnconditionalBranch(labeledOrNewBlock);
            if (num)
            {
                LeaveRegion();
                ImmutableArray<ICatchClauseOperation>.Enumerator enumerator = operation.Catches.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ICatchClauseOperation current = enumerator.Current;
                    RegionBuilder regionBuilder2 = null;
                    IOperation exceptionDeclarationOrExpression = current.ExceptionDeclarationOrExpression;
                    IOperation filter = current.Filter;
                    bool flag2 = filter != null;
                    BasicBlockBuilder dest = new BasicBlockBuilder(BasicBlockKind.Block);
                    if (flag2)
                    {
                        regionBuilder2 = new RegionBuilder(ControlFlowRegionKind.FilterAndHandler, current.ExceptionType, current.Locals);
                        EnterRegion(regionBuilder2);
                        RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.Filter, current.ExceptionType);
                        EnterRegion(region);
                        AddExceptionStore(current.ExceptionType, exceptionDeclarationOrExpression);
                        VisitConditionalBranch(filter, ref dest, jumpIfTrue: true);
                        BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                        AppendNewBlock(basicBlockBuilder);
                        basicBlockBuilder.FallThrough.Kind = ControlFlowBranchSemantics.StructuredExceptionHandling;
                        LeaveRegion();
                    }
                    RegionBuilder region2 = new RegionBuilder(ControlFlowRegionKind.Catch, current.ExceptionType, flag2 ? default(ImmutableArray<ILocalSymbol>) : current.Locals);
                    EnterRegion(region2);
                    AppendNewBlock(dest, linkToPrevious: false);
                    if (!flag2)
                    {
                        AddExceptionStore(current.ExceptionType, exceptionDeclarationOrExpression);
                    }
                    VisitStatement(current.Handler);
                    UnconditionalBranch(labeledOrNewBlock);
                    LeaveRegion();
                    if (flag2)
                    {
                        LeaveRegion();
                    }
                }
                LeaveRegion();
            }
            if (flag)
            {
                LeaveRegion();
                RegionBuilder region3 = new RegionBuilder(ControlFlowRegionKind.Finally);
                EnterRegion(region3);
                AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
                VisitStatement(operation.Finally);
                BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
                AppendNewBlock(basicBlockBuilder2);
                basicBlockBuilder2.FallThrough.Kind = ControlFlowBranchSemantics.StructuredExceptionHandling;
                LeaveRegion();
                LeaveRegion();
            }
            AppendNewBlock(labeledOrNewBlock, linkToPrevious: false);
            return FinishVisitingStatement(operation);
        }

        private void AddExceptionStore(ITypeSymbol exceptionType, IOperation? exceptionDeclarationOrExpression)
        {
            if (exceptionDeclarationOrExpression != null)
            {
                SyntaxNode syntax = exceptionDeclarationOrExpression!.Syntax;
                IOperation operation;
                if (exceptionDeclarationOrExpression!.Kind == OperationKind.VariableDeclarator)
                {
                    ILocalSymbol symbol = ((IVariableDeclaratorOperation)exceptionDeclarationOrExpression).Symbol;
                    operation = new LocalReferenceOperation(symbol, isDeclaration: true, null, syntax, symbol.Type, null, isImplicit: true);
                }
                else
                {
                    operation = VisitRequired(exceptionDeclarationOrExpression);
                }
                if (operation != null)
                {
                    AddStatement(new SimpleAssignmentOperation(isRef: false, operation, new CaughtExceptionOperation(syntax, exceptionType), null, syntax, null, null, isImplicit: true));
                }
            }
        }

        public override IOperation VisitCatchClause(ICatchClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation? VisitReturn(IReturnOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            IOperation operation2 = Visit(operation.ReturnedValue);
            switch (operation.Kind)
            {
                case OperationKind.YieldReturn:
                    AddStatement(new ReturnOperation(operation2, OperationKind.YieldReturn, null, operation.Syntax, IsImplicit(operation)));
                    break;
                case OperationKind.Return:
                case OperationKind.YieldBreak:
                    {
                        BasicBlockBuilder currentBasicBlock = CurrentBasicBlock;
                        LinkBlocks(CurrentBasicBlock, _exit, (operation2 == null) ? ControlFlowBranchSemantics.Regular : ControlFlowBranchSemantics.Return);
                        currentBasicBlock.BranchValue = Operation.SetParentOperation(operation2, null);
                        _currentBasicBlock = null;
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(operation.Kind);
            }
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitLabeled(ILabeledOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            VisitLabel(operation.Label);
            VisitStatement(operation.Operation);
            return FinishVisitingStatement(operation);
        }

        public void VisitLabel(ILabelSymbol operation)
        {
            BasicBlockBuilder basicBlockBuilder = GetLabeledOrNewBlock(operation);
            if (basicBlockBuilder.Ordinal != -1)
            {
                basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            }
            AppendNewBlock(basicBlockBuilder);
        }

        private BasicBlockBuilder GetLabeledOrNewBlock(ILabelSymbol? labelOpt)
        {
            if (labelOpt == null)
            {
                return new BasicBlockBuilder(BasicBlockKind.Block);
            }
            BasicBlockBuilder value;
            if (_labeledBlocks == null)
            {
                _labeledBlocks = PooledDictionary<ILabelSymbol, BasicBlockBuilder>.GetInstance();
            }
            else if (_labeledBlocks!.TryGetValue(labelOpt, out value))
            {
                return value;
            }
            value = new BasicBlockBuilder(BasicBlockKind.Block);
            _labeledBlocks!.Add(labelOpt, value);
            return value;
        }

        public override IOperation? VisitBranch(IBranchOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            UnconditionalBranch(GetLabeledOrNewBlock(operation.Target));
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitEmpty(IEmptyOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitThrow(IThrowOperation operation, int? captureIdForResult)
        {
            bool num = _currentStatement == operation;
            if (!num)
            {
                SpillEvalStack();
            }
            EvalStackFrame frame = PushStackFrame();
            LinkThrowStatement(Visit(operation.Exception));
            PopStackFrameAndLeaveRegion(frame);
            AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block), linkToPrevious: false);
            if (num)
            {
                return null;
            }
            return new NoneOperation(ImmutableArray<IOperation>.Empty, null, operation.Syntax, null, null, isImplicit: true);
        }

        private void LinkThrowStatement(IOperation? exception)
        {
            BasicBlockBuilder currentBasicBlock = CurrentBasicBlock;
            currentBasicBlock.BranchValue = Operation.SetParentOperation(exception, null);
            currentBasicBlock.FallThrough.Kind = ((exception == null) ? ControlFlowBranchSemantics.Rethrow : ControlFlowBranchSemantics.Throw);
        }

        public override IOperation? VisitUsing(IUsingOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            DisposeOperationInfo disposeInfo = ((UsingOperation)operation).DisposeInfo;
            HandleUsingOperationParts(operation.Resources, operation.Body, disposeInfo.DisposeMethod, disposeInfo.DisposeArguments, operation.Locals, operation.IsAsynchronous);
            return FinishVisitingStatement(operation);
        }

        private void HandleUsingOperationParts(IOperation resources, IOperation body, IMethodSymbol? disposeMethod, ImmutableArray<IArgumentOperation> disposeArguments, ImmutableArray<ILocalSymbol> locals, bool isAsynchronous)
        {
            IOperation body2 = body;
            IMethodSymbol disposeMethod2 = disposeMethod;
            RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, locals);
            EnterRegion(region);
            ITypeSymbol typeSymbol;
            if (!isAsynchronous)
            {
                ITypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_IDisposable);
                typeSymbol = specialType;
            }
            else
            {
                typeSymbol = _compilation.CommonGetWellKnownType(WellKnownType.System_IAsyncDisposable).GetITypeSymbol();
            }
            ITypeSymbol iDisposable = typeSymbol;
            if (resources is IVariableDeclarationGroupOperation variableDeclarationGroupOperation)
            {
                ArrayBuilder<(IVariableDeclarationOperation, IVariableDeclaratorOperation)> instance = ArrayBuilder<(IVariableDeclarationOperation, IVariableDeclaratorOperation)>.GetInstance(variableDeclarationGroupOperation.Declarations.Length);
                ImmutableArray<IVariableDeclarationOperation>.Enumerator enumerator = variableDeclarationGroupOperation.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IVariableDeclarationOperation current = enumerator.Current;
                    ImmutableArray<IVariableDeclaratorOperation>.Enumerator enumerator2 = current.Declarators.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        IVariableDeclaratorOperation current2 = enumerator2.Current;
                        instance.Add((current, current2));
                    }
                }
                instance.ReverseContents();
                processQueue(instance);
            }
            else
            {
                EvalStackFrame frame = PushStackFrame();
                IOperation operation = VisitRequired(resources);
                if (shouldConvertToIDisposableBeforeTry(operation))
                {
                    operation = ConvertToIDisposable(operation, iDisposable);
                }
                PushOperand(operation);
                SpillEvalStack();
                operation = PopOperand();
                PopStackFrame(frame);
                processResource(operation, null);
                LeaveRegionIfAny(frame);
            }
            LeaveRegion();
            void processQueue(ArrayBuilder<(IVariableDeclarationOperation, IVariableDeclaratorOperation)>? resourceQueueOpt)
            {
                if (resourceQueueOpt == null || resourceQueueOpt!.Count == 0)
                {
                    VisitStatement(body2);
                }
                else
                {
                    var (declaration, variableDeclaratorOperation) = resourceQueueOpt.Pop();
                    HandleVariableDeclarator(declaration, variableDeclaratorOperation);
                    ILocalSymbol symbol = variableDeclaratorOperation.Symbol;
                    processResource(new LocalReferenceOperation(symbol, isDeclaration: false, null, variableDeclaratorOperation.Syntax, symbol.Type, null, isImplicit: true), resourceQueueOpt);
                }
            }
            void processResource(IOperation resource, ArrayBuilder<(IVariableDeclarationOperation, IVariableDeclaratorOperation)>? resourceQueueOpt)
            {
                RegionBuilder regionBuilder = null;
                if (shouldConvertToIDisposableBeforeTry(resource))
                {
                    regionBuilder = new RegionBuilder(ControlFlowRegionKind.LocalLifetime);
                    EnterRegion(regionBuilder);
                    resource = ConvertToIDisposable(resource, iDisposable);
                    int nextCaptureId = GetNextCaptureId(regionBuilder);
                    AddStatement(new FlowCaptureOperation(nextCaptureId, resource.Syntax, resource));
                    resource = GetCaptureReference(nextCaptureId, resource);
                }
                BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.TryAndFinally));
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.Try));
                processQueue(resourceQueueOpt);
                UnconditionalBranch(basicBlockBuilder);
                LeaveRegion();
                AddDisposingFinally(resource, requiresRuntimeConversion: false, iDisposable, disposeMethod2, disposeArguments, isAsynchronous);
                LeaveRegion();
                if (regionBuilder != null)
                {
                    LeaveRegion();
                }
                AppendNewBlock(basicBlockBuilder, linkToPrevious: false);
            }
            static bool shouldConvertToIDisposableBeforeTry(IOperation resource)
            {
                if (resource.Type != null)
                {
                    return resource.Type!.Kind == SymbolKind.DynamicType;
                }
                return true;
            }
        }

        private void AddDisposingFinally(IOperation resource, bool requiresRuntimeConversion, ITypeSymbol iDisposable, IMethodSymbol? disposeMethod, ImmutableArray<IArgumentOperation> disposeArguments, bool isAsynchronous)
        {
            IMethodSymbol disposeMethod2 = disposeMethod;
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            basicBlockBuilder.FallThrough.Kind = ControlFlowBranchSemantics.StructuredExceptionHandling;
            RegionBuilder regionBuilder = new RegionBuilder(ControlFlowRegionKind.Finally);
            EnterRegion(regionBuilder);
            AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
            if (requiresRuntimeConversion)
            {
                resource = ConvertToIDisposable(resource, iDisposable, isTryCast: true);
                int nextCaptureId = GetNextCaptureId(regionBuilder);
                AddStatement(new FlowCaptureOperation(nextCaptureId, resource.Syntax, resource));
                resource = GetCaptureReference(nextCaptureId, resource);
            }
            if (requiresRuntimeConversion || !isNotNullableValueType(resource.Type))
            {
                IOperation condition = MakeIsNullOperation(OperationCloner.CloneOperation(resource));
                ConditionalBranch(condition, jumpIfTrue: true, basicBlockBuilder);
                _currentBasicBlock = null;
            }
            if (!iDisposable.Equals(resource.Type) && disposeMethod2 == null)
            {
                resource = ConvertToIDisposable(resource, iDisposable);
            }
            EvalStackFrame frame = PushStackFrame();
            AddStatement(tryDispose(resource) ?? MakeInvalidOperation(null, resource));
            PopStackFrameAndLeaveRegion(frame);
            AppendNewBlock(basicBlockBuilder);
            LeaveRegion();
            static bool isNotNullableValueType([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ITypeSymbol? type)
            {
                if (type != null && type!.IsValueType)
                {
                    return !ITypeSymbolHelpers.IsNullableType(type);
                }
                return false;
            }
            IOperation? tryDispose(IOperation value)
            {
                IMethodSymbol methodSymbol = disposeMethod2 ?? (isAsynchronous ? ((IMethodSymbol)(_compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_IAsyncDisposable__DisposeAsync)?.GetISymbol())) : ((IMethodSymbol)(_compilation.CommonGetSpecialTypeMember(SpecialMember.System_IDisposable__Dispose)?.GetISymbol())));
                if (methodSymbol != null)
                {
                    ImmutableArray<IArgumentOperation> arguments = ((disposeMethod2 != null) ? VisitArguments(disposeArguments) : ImmutableArray<IArgumentOperation>.Empty);
                    InvocationOperation invocationOperation = new InvocationOperation(methodSymbol, value, disposeMethod2?.IsVirtual ?? true, arguments, null, value.Syntax, methodSymbol.ReturnType, isImplicit: true);
                    if (isAsynchronous)
                    {
                        return new AwaitOperation(invocationOperation, null, value.Syntax, _compilation.GetSpecialType(SpecialType.System_Void), isImplicit: true);
                    }
                    return invocationOperation;
                }
                return null;
            }
        }

        private IOperation ConvertToIDisposable(IOperation operand, ITypeSymbol iDisposable, bool isTryCast = false)
        {
            return new ConversionOperation(operand, _compilation.ClassifyConvertibleConversion(operand, iDisposable, out ConstantValue constantValue), isTryCast, isChecked: false, null, operand.Syntax, iDisposable, constantValue, isImplicit: true);
        }

        public override IOperation? VisitLock(ILockOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            ITypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Object);
            LockOperation lockOperation = (LockOperation)operation;
            RegionBuilder regionBuilder = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, (lockOperation.LockTakenSymbol != null) ? ImmutableArray.Create(lockOperation.LockTakenSymbol) : ImmutableArray<ILocalSymbol>.Empty);
            EnterRegion(regionBuilder);
            EvalStackFrame frame = PushStackFrame();
            IOperation operation2 = VisitRequired(operation.LockedValue);
            if (!specialType.Equals(operation2.Type))
            {
                operation2 = CreateConversion(operation2, specialType);
            }
            PushOperand(operation2);
            SpillEvalStack();
            operation2 = PopOperand();
            PopStackFrame(frame);
            IMethodSymbol methodSymbol = (IMethodSymbol)(_compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Enter2)?.GetISymbol());
            bool num = methodSymbol == null;
            if (num)
            {
                methodSymbol = (IMethodSymbol)(_compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Enter)?.GetISymbol());
                if (methodSymbol == null)
                {
                    AddStatement(MakeInvalidOperation(null, operation2));
                }
                else
                {
                    AddStatement(new InvocationOperation(methodSymbol, null, isVirtual: false, ImmutableArray.Create((IArgumentOperation)new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[0], operation2, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, operation2.Syntax, isImplicit: true)), null, operation2.Syntax, methodSymbol.ReturnType, isImplicit: true));
                }
            }
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.TryAndFinally));
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.Try));
            IOperation operation3 = null;
            if (!num)
            {
                operation3 = new LocalReferenceOperation(lockOperation.LockTakenSymbol, isDeclaration: true, null, operation2.Syntax, lockOperation.LockTakenSymbol!.Type, null, isImplicit: true);
                AddStatement(new InvocationOperation(methodSymbol, null, isVirtual: false, ImmutableArray.Create(new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[0], operation2, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, operation2.Syntax, isImplicit: true), (IArgumentOperation)new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[1], operation3, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, operation2.Syntax, isImplicit: true)), null, operation2.Syntax, methodSymbol.ReturnType, isImplicit: true));
            }
            VisitStatement(operation.Body);
            UnconditionalBranch(basicBlockBuilder);
            LeaveRegion();
            BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block)
            {
                FallThrough =
                {
                    Kind = ControlFlowBranchSemantics.StructuredExceptionHandling
                }
            };
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.Finally));
            AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
            if (!num)
            {
                IOperation condition = new LocalReferenceOperation(lockOperation.LockTakenSymbol, isDeclaration: false, null, operation2.Syntax, lockOperation.LockTakenSymbol!.Type, null, isImplicit: true);
                ConditionalBranch(condition, jumpIfTrue: false, basicBlockBuilder2);
                _currentBasicBlock = null;
            }
            IMethodSymbol methodSymbol2 = (IMethodSymbol)(_compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Exit)?.GetISymbol());
            operation2 = OperationCloner.CloneOperation(operation2);
            if (methodSymbol2 == null)
            {
                AddStatement(MakeInvalidOperation(null, operation2));
            }
            else
            {
                AddStatement(new InvocationOperation(methodSymbol2, null, isVirtual: false, ImmutableArray.Create((IArgumentOperation)new ArgumentOperation(ArgumentKind.Explicit, methodSymbol2.Parameters[0], operation2, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, operation2.Syntax, isImplicit: true)), null, operation2.Syntax, methodSymbol2.ReturnType, isImplicit: true));
            }
            AppendNewBlock(basicBlockBuilder2);
            LeaveRegion();
            LeaveRegion();
            LeaveRegionsUpTo(regionBuilder);
            LeaveRegion();
            AppendNewBlock(basicBlockBuilder, linkToPrevious: false);
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitForEachLoop(IForEachLoopOperation operation, int? captureIdForResult)
        {
            IForEachLoopOperation operation2 = operation;
            StartVisitingStatement(operation2);
            RegionBuilder enumeratorCaptureRegion = new RegionBuilder(ControlFlowRegionKind.LocalLifetime);
            EnterRegion(enumeratorCaptureRegion);
            ForEachLoopOperationInfo info = ((ForEachLoopOperation)operation2).Info;
            RegionBuilder regionBuilder = null;
            if (!operation2.Locals.IsEmpty && operation2.LoopControlVariable.Kind == OperationKind.VariableDeclarator)
            {
                ILocalSymbol symbol = ((IVariableDeclaratorOperation)operation2.LoopControlVariable).Symbol;
                foreach (IOperation item in operation2.Collection.DescendantsAndSelf())
                {
                    if (item is ILocalReferenceOperation localReferenceOperation && localReferenceOperation.Local.Equals(symbol))
                    {
                        regionBuilder = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, ImmutableArray.Create(symbol));
                        EnterRegion(regionBuilder);
                        break;
                    }
                }
            }
            IOperation operation3 = getEnumerator();
            if (regionBuilder != null)
            {
                LeaveRegion();
            }
            ForEachLoopOperationInfo forEachLoopOperationInfo = info;
            if (forEachLoopOperationInfo != null && forEachLoopOperationInfo.NeedsDispose)
            {
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.TryAndFinally));
                EnterRegion(new RegionBuilder(ControlFlowRegionKind.Try));
            }
            BasicBlockBuilder labeledOrNewBlock = GetLabeledOrNewBlock(operation2.ContinueLabel);
            BasicBlockBuilder labeledOrNewBlock2 = GetLabeledOrNewBlock(operation2.ExitLabel);
            AppendNewBlock(labeledOrNewBlock);
            EvalStackFrame frame = PushStackFrame();
            ConditionalBranch(getCondition(operation3), jumpIfTrue: false, labeledOrNewBlock2);
            _currentBasicBlock = null;
            PopStackFrameAndLeaveRegion(frame);
            RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation2.Locals);
            EnterRegion(region);
            frame = PushStackFrame();
            AddStatement(getLoopControlVariableAssignment(applyConversion(info?.CurrentConversion, getCurrent(OperationCloner.CloneOperation(operation3)), info?.ElementType)));
            PopStackFrameAndLeaveRegion(frame);
            VisitStatement(operation2.Body);
            UnconditionalBranch(labeledOrNewBlock);
            LeaveRegion();
            AppendNewBlock(labeledOrNewBlock2);
            ForEachLoopOperationInfo forEachLoopOperationInfo2 = info;
            if (forEachLoopOperationInfo2 != null && forEachLoopOperationInfo2.NeedsDispose)
            {
                BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                UnconditionalBranch(basicBlockBuilder);
                LeaveRegion();
                bool isAsynchronous = info.IsAsynchronous;
                ITypeSymbol typeSymbol;
                if (!isAsynchronous)
                {
                    ITypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_IDisposable);
                    typeSymbol = specialType;
                }
                else
                {
                    typeSymbol = _compilation.CommonGetWellKnownType(WellKnownType.System_IAsyncDisposable).GetITypeSymbol();
                }
                ITypeSymbol iDisposable = typeSymbol;
                AddDisposingFinally(OperationCloner.CloneOperation(operation3), !info.KnownToImplementIDisposable && info.PatternDisposeMethod == null, iDisposable, info.PatternDisposeMethod, info.DisposeArguments, isAsynchronous);
                LeaveRegion();
                AppendNewBlock(basicBlockBuilder, linkToPrevious: false);
            }
            LeaveRegion();
            return FinishVisitingStatement(operation2);
            static IOperation applyConversion(IConvertibleConversion? conversionOpt, IOperation operand, ITypeSymbol? targetType)
            {
                if (conversionOpt != null && !conversionOpt!.ToCommonConversion().IsIdentity)
                {
                    operand = new ConversionOperation(operand, conversionOpt, isTryCast: false, isChecked: false, null, operand.Syntax, targetType, null, isImplicit: true);
                }
                return operand;
            }
            IOperation getCondition(IOperation enumeratorRef)
            {
                if (info?.MoveNextMethod != null)
                {
                    InvocationOperation invocationOperation = makeInvocationDroppingInstanceForStaticMethods(info.MoveNextMethod, enumeratorRef, info.MoveNextArguments);
                    if (operation2.IsAsynchronous)
                    {
                        return new AwaitOperation(invocationOperation, null, operation2.Syntax, _compilation.GetSpecialType(SpecialType.System_Boolean), isImplicit: true);
                    }
                    return invocationOperation;
                }
                return MakeInvalidOperation(_compilation.GetSpecialType(SpecialType.System_Boolean), enumeratorRef);
            }
            IOperation getCurrent(IOperation enumeratorRef)
            {
                if (info?.CurrentProperty != null)
                {
                    return new PropertyReferenceOperation(info.CurrentProperty, makeArguments(info.CurrentArguments), info.CurrentProperty.IsStatic ? null : enumeratorRef, null, operation2.LoopControlVariable.Syntax, info.CurrentProperty.Type, isImplicit: true);
                }
                return MakeInvalidOperation(null, enumeratorRef);
            }
            IOperation getEnumerator()
            {
                EvalStackFrame frame2 = PushStackFrame();
                IOperation result;
                if (info?.GetEnumeratorMethod != null)
                {
                    IOperation value = makeInvocation(operation2.Collection.Syntax, info.GetEnumeratorMethod, info.GetEnumeratorMethod.IsStatic ? null : Visit(operation2.Collection), info.GetEnumeratorArguments);
                    int nextCaptureId = GetNextCaptureId(enumeratorCaptureRegion);
                    AddStatement(new FlowCaptureOperation(nextCaptureId, operation2.Collection.Syntax, value));
                    result = new FlowCaptureReferenceOperation(nextCaptureId, operation2.Collection.Syntax, info.GetEnumeratorMethod.ReturnType, null);
                }
                else
                {
                    AddStatement(MakeInvalidOperation(null, VisitRequired(operation2.Collection)));
                    result = new InvalidOperation(ImmutableArray<IOperation>.Empty, null, operation2.Collection.Syntax, null, null, isImplicit: true);
                }
                PopStackFrameAndLeaveRegion(frame2);
                return result;
            }
            IOperation getLoopControlVariableAssignment(IOperation current)
            {
                switch (operation2.LoopControlVariable.Kind)
                {
                    case OperationKind.VariableDeclarator:
                        {
                            IVariableDeclaratorOperation variableDeclaratorOperation = (IVariableDeclaratorOperation)operation2.LoopControlVariable;
                            ILocalSymbol symbol2 = variableDeclaratorOperation.Symbol;
                            current = applyConversion(info?.ElementConversion, current, symbol2.Type);
                            return new SimpleAssignmentOperation(symbol2.RefKind != RefKind.None, new LocalReferenceOperation(symbol2, isDeclaration: true, null, variableDeclaratorOperation.Syntax, symbol2.Type, null, isImplicit: true), current, null, variableDeclaratorOperation.Syntax, null, null, isImplicit: true);
                        }
                    case OperationKind.Tuple:
                    case OperationKind.DeclarationExpression:
                        return new DeconstructionAssignmentOperation(VisitPreservingTupleOperations(operation2.LoopControlVariable), current, null, operation2.LoopControlVariable.Syntax, operation2.LoopControlVariable.Type, isImplicit: true);
                    default:
                        return new SimpleAssignmentOperation(isRef: false, VisitRequired(operation2.LoopControlVariable), current, null, operation2.LoopControlVariable.Syntax, operation2.LoopControlVariable.Type, null, isImplicit: true);
                }
            }
            ImmutableArray<IArgumentOperation> makeArguments(ImmutableArray<IArgumentOperation> arguments)
            {
                if (arguments != null)
                {
                    return VisitArguments(arguments);
                }
                return ImmutableArray<IArgumentOperation>.Empty;
            }
            InvocationOperation makeInvocation(SyntaxNode syntax, IMethodSymbol method, IOperation? instanceOpt, ImmutableArray<IArgumentOperation> arguments)
            {
                return new InvocationOperation(method, instanceOpt, method.IsVirtual || method.IsAbstract || method.IsOverride, makeArguments(arguments), null, syntax, method.ReturnType, isImplicit: true);
            }
            InvocationOperation makeInvocationDroppingInstanceForStaticMethods(IMethodSymbol method, IOperation instance, ImmutableArray<IArgumentOperation> arguments)
            {
                return makeInvocation(instance.Syntax, method, method.IsStatic ? null : instance, arguments);
            }
        }

        public override IOperation? VisitForToLoop(IForToLoopOperation operation, int? captureIdForResult)
        {
            IForToLoopOperation operation2 = operation;
            StartVisitingStatement(operation2);
            (ILocalSymbol, ForToLoopOperationUserDefinedInfo) info = ((ForToLoopOperation)operation2).Info;
            ILocalSymbol loopObject = info.Item1;
            ForToLoopOperationUserDefinedInfo userDefinedInfo = info.Item2;
            bool isObjectLoop = loopObject != null;
            ImmutableArray<ILocalSymbol> locals = operation2.Locals;
            if (isObjectLoop)
            {
                locals = locals.Insert(0, loopObject);
            }
            ITypeSymbol booleanType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            BasicBlockBuilder labeledOrNewBlock = GetLabeledOrNewBlock(operation2.ContinueLabel);
            BasicBlockBuilder @break = GetLabeledOrNewBlock(operation2.ExitLabel);
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            BasicBlockBuilder bodyBlock = new BasicBlockBuilder(BasicBlockKind.Block);
            RegionBuilder loopRegion = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, locals);
            EnterRegion(loopRegion);
            int limitValueId = -1;
            int stepValueId = -1;
            IFlowCaptureReferenceOperation positiveFlag = null;
            ITypeSymbol stepEnumUnderlyingTypeOrSelf = ITypeSymbolHelpers.GetEnumUnderlyingTypeOrSelf(operation2.StepValue.Type);
            initializeLoop();
            AppendNewBlock(basicBlockBuilder);
            checkLoopCondition();
            AppendNewBlock(bodyBlock);
            VisitStatement(operation2.Body);
            AppendNewBlock(labeledOrNewBlock);
            incrementLoopControlVariable();
            UnconditionalBranch(basicBlockBuilder);
            LeaveRegion();
            AppendNewBlock(@break);
            return FinishVisitingStatement(operation2);
            void checkLoopCondition()
            {
                if (isObjectLoop)
                {
                    EvalStackFrame frame4 = PushStackFrame();
                    PushOperand(visitLoopControlVariableReference(forceImplicit: true));
                    IOperation condition2 = tryCallObjectForLoopControlHelper(operation2.LimitValue.Syntax, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForNextCheckObj);
                    ConditionalBranch(condition2, jumpIfTrue: false, @break);
                    UnconditionalBranch(bodyBlock);
                    PopStackFrameAndLeaveRegion(frame4);
                }
                else if (userDefinedInfo != null)
                {
                    EvalStackFrame frame5 = PushStackFrame();
                    PushOperand(visitLoopControlVariableReference(forceImplicit: true));
                    SpillEvalStack();
                    IOperation forToLoopBinaryOperatorLeftOperand = PopOperand();
                    BasicBlockBuilder basicBlockBuilder4 = new BasicBlockBuilder(BasicBlockKind.Block);
                    ConditionalBranch(positiveFlag, jumpIfTrue: false, basicBlockBuilder4);
                    _currentBasicBlock = null;
                    _forToLoopBinaryOperatorLeftOperand = forToLoopBinaryOperatorLeftOperand;
                    _forToLoopBinaryOperatorRightOperand = GetCaptureReference(limitValueId, operation2.LimitValue);
                    VisitConditionalBranch(userDefinedInfo.LessThanOrEqual, ref @break, jumpIfTrue: false);
                    UnconditionalBranch(bodyBlock);
                    AppendNewBlock(basicBlockBuilder4);
                    _forToLoopBinaryOperatorLeftOperand = OperationCloner.CloneOperation(_forToLoopBinaryOperatorLeftOperand);
                    _forToLoopBinaryOperatorRightOperand = OperationCloner.CloneOperation(_forToLoopBinaryOperatorRightOperand);
                    VisitConditionalBranch(userDefinedInfo.GreaterThanOrEqual, ref @break, jumpIfTrue: false);
                    UnconditionalBranch(bodyBlock);
                    PopStackFrameAndLeaveRegion(frame5);
                    _forToLoopBinaryOperatorLeftOperand = null;
                    _forToLoopBinaryOperatorRightOperand = null;
                }
                else
                {
                    EvalStackFrame frame6 = PushStackFrame();
                    PushOperand(visitLoopControlVariableReference(forceImplicit: true));
                    IOperation operation8 = GetCaptureReference(limitValueId, operation2.LimitValue);
                    BinaryOperatorKind binaryOperatorKind = BinaryOperatorKind.None;
                    if (ITypeSymbolHelpers.IsUnsignedIntegralType(stepEnumUnderlyingTypeOrSelf))
                    {
                        binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
                    }
                    else
                    {
                        ConstantValue constantValue = operation2.StepValue.GetConstantValue();
                        if ((object)constantValue != null && !constantValue.IsBad)
                        {
                            if (constantValue.IsNegativeNumeric)
                            {
                                binaryOperatorKind = BinaryOperatorKind.GreaterThanOrEqual;
                            }
                            else if (constantValue.IsNumeric)
                            {
                                binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
                            }
                        }
                    }
                    if (binaryOperatorKind == BinaryOperatorKind.None && ITypeSymbolHelpers.IsSignedIntegralType(stepEnumUnderlyingTypeOrSelf))
                    {
                        binaryOperatorKind = BinaryOperatorKind.LessThanOrEqual;
                        PushOperand(negateIfStepNegative(PopOperand()));
                        operation8 = negateIfStepNegative(operation8);
                    }
                    if (binaryOperatorKind != 0)
                    {
                        IOperation condition3 = new BinaryOperation(binaryOperatorKind, PopOperand(), operation8, isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation2.LimitValue.Syntax, booleanType, null, isImplicit: true);
                        ConditionalBranch(condition3, jumpIfTrue: false, @break);
                        UnconditionalBranch(bodyBlock);
                        PopStackFrameAndLeaveRegion(frame6);
                    }
                    else if (positiveFlag == null)
                    {
                        IOperation condition3 = MakeInvalidOperation(operation2.LimitValue.Syntax, booleanType, PopOperand(), operation8);
                        ConditionalBranch(condition3, jumpIfTrue: false, @break);
                        UnconditionalBranch(bodyBlock);
                        PopStackFrameAndLeaveRegion(frame6);
                    }
                    else
                    {
                        IOperation operation9 = null;
                        if (ITypeSymbolHelpers.IsNullableType(operation2.LimitValue.Type))
                        {
                            operation9 = new BinaryOperation(BinaryOperatorKind.Or, MakeIsNullOperation(operation8, booleanType), MakeIsNullOperation(PopOperand(), booleanType), isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation2.StepValue.Syntax, _compilation.GetSpecialType(SpecialType.System_Boolean), null, isImplicit: true);
                            BasicBlockBuilder basicBlockBuilder5 = new BasicBlockBuilder(BasicBlockKind.Block);
                            ConditionalBranch(operation9, jumpIfTrue: false, basicBlockBuilder5);
                            UnconditionalBranch(@break);
                            PopStackFrameAndLeaveRegion(frame6);
                            AppendNewBlock(basicBlockBuilder5);
                            frame6 = PushStackFrame();
                            PushOperand(CallNullableMember(visitLoopControlVariableReference(forceImplicit: true), SpecialMember.System_Nullable_T_GetValueOrDefault));
                            operation8 = CallNullableMember(GetCaptureReference(limitValueId, operation2.LimitValue), SpecialMember.System_Nullable_T_GetValueOrDefault);
                        }
                        SpillEvalStack();
                        IOperation operation10 = PopOperand();
                        BasicBlockBuilder basicBlockBuilder6 = new BasicBlockBuilder(BasicBlockKind.Block);
                        ConditionalBranch(positiveFlag, jumpIfTrue: false, basicBlockBuilder6);
                        _currentBasicBlock = null;
                        IOperation condition3 = new BinaryOperation(BinaryOperatorKind.LessThanOrEqual, operation10, operation8, isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation2.LimitValue.Syntax, booleanType, null, isImplicit: true);
                        ConditionalBranch(condition3, jumpIfTrue: false, @break);
                        UnconditionalBranch(bodyBlock);
                        AppendNewBlock(basicBlockBuilder6);
                        condition3 = new BinaryOperation(BinaryOperatorKind.GreaterThanOrEqual, OperationCloner.CloneOperation(operation10), OperationCloner.CloneOperation(operation8), isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation2.LimitValue.Syntax, booleanType, null, isImplicit: true);
                        ConditionalBranch(condition3, jumpIfTrue: false, @break);
                        UnconditionalBranch(bodyBlock);
                        PopStackFrameAndLeaveRegion(frame6);
                    }
                }
            }
            void incrementLoopControlVariable()
            {
                if (!isObjectLoop)
                {
                    if (userDefinedInfo != null)
                    {
                        EvalStackFrame frame = PushStackFrame();
                        IOperation operation3 = visitLoopControlVariableReference(forceImplicit: true);
                        PushOperand(operation3);
                        _forToLoopBinaryOperatorLeftOperand = visitLoopControlVariableReference(forceImplicit: true);
                        _forToLoopBinaryOperatorRightOperand = GetCaptureReference(stepValueId, operation2.StepValue);
                        IOperation value = VisitRequired(userDefinedInfo.Addition);
                        _forToLoopBinaryOperatorLeftOperand = null;
                        _forToLoopBinaryOperatorRightOperand = null;
                        operation3 = PopOperand();
                        AddStatement(new SimpleAssignmentOperation(isRef: false, operation3, value, null, operation3.Syntax, null, null, isImplicit: true));
                        PopStackFrameAndLeaveRegion(frame);
                    }
                    else
                    {
                        BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
                        bool flag = ITypeSymbolHelpers.IsNullableType(operation2.StepValue.Type);
                        EvalStackFrame frame2 = PushStackFrame();
                        PushOperand(visitLoopControlVariableReference(forceImplicit: true));
                        IOperation operation4;
                        if (flag)
                        {
                            SpillEvalStack();
                            BasicBlockBuilder basicBlockBuilder3 = new BasicBlockBuilder(BasicBlockKind.Block);
                            EvalStackFrame frame3 = PushStackFrame();
                            IOperation condition = new BinaryOperation(BinaryOperatorKind.Or, MakeIsNullOperation(GetCaptureReference(stepValueId, operation2.StepValue), booleanType), MakeIsNullOperation(visitLoopControlVariableReference(forceImplicit: true), booleanType), isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation2.StepValue.Syntax, _compilation.GetSpecialType(SpecialType.System_Boolean), null, isImplicit: true);
                            ConditionalBranch(condition, jumpIfTrue: false, basicBlockBuilder3);
                            _currentBasicBlock = null;
                            PopStackFrameAndLeaveRegion(frame3);
                            operation4 = OperationCloner.CloneOperation(PeekOperand());
                            AddStatement(new SimpleAssignmentOperation(isRef: false, operation4, new DefaultValueOperation(null, operation4.Syntax, operation4.Type, null, isImplicit: true), null, operation4.Syntax, null, null, isImplicit: true));
                            UnconditionalBranch(basicBlockBuilder2);
                            AppendNewBlock(basicBlockBuilder3);
                        }
                        IOperation operation5 = visitLoopControlVariableReference(forceImplicit: true);
                        IOperation operation6 = GetCaptureReference(stepValueId, operation2.StepValue);
                        if (flag)
                        {
                            operation5 = CallNullableMember(operation5, SpecialMember.System_Nullable_T_GetValueOrDefault);
                            operation6 = CallNullableMember(operation6, SpecialMember.System_Nullable_T_GetValueOrDefault);
                        }
                        IOperation operation7 = new BinaryOperation(BinaryOperatorKind.Add, operation5, operation6, isLifted: false, operation2.IsChecked, isCompareText: false, null, null, null, operation2.StepValue.Syntax, operation5.Type, null, isImplicit: true);
                        operation4 = PopOperand();
                        if (flag)
                        {
                            operation7 = MakeNullable(operation7, operation4.Type);
                        }
                        AddStatement(new SimpleAssignmentOperation(isRef: false, operation4, operation7, null, operation4.Syntax, null, null, isImplicit: true));
                        PopStackFrame(frame2, !flag);
                        LeaveRegionIfAny(frame2);
                        AppendNewBlock(basicBlockBuilder2);
                    }
                }
            }
            void initializeLoop()
            {
                EvalStackFrame frame7 = PushStackFrame();
                PushOperand(visitLoopControlVariableReference(forceImplicit: false));
                PushOperand(VisitRequired(operation2.InitialValue));
                if (isObjectLoop)
                {
                    PushOperand(VisitRequired(operation2.LimitValue));
                    PushOperand(VisitRequired(operation2.StepValue));
                    IOperation condition4 = tryCallObjectForLoopControlHelper(operation2.LoopControlVariable.Syntax, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForLoopInitObj);
                    ConditionalBranch(condition4, jumpIfTrue: false, @break);
                    UnconditionalBranch(bodyBlock);
                }
                else
                {
                    SpillEvalStack();
                    _ = CurrentRegionRequired;
                    limitValueId = GetNextCaptureId(loopRegion);
                    VisitAndCapture(operation2.LimitValue, limitValueId);
                    stepValueId = GetNextCaptureId(loopRegion);
                    VisitAndCapture(operation2.StepValue, stepValueId);
                    IOperation operation12 = GetCaptureReference(stepValueId, operation2.StepValue);
                    if (userDefinedInfo != null)
                    {
                        _forToLoopBinaryOperatorLeftOperand = GetCaptureReference(stepValueId, operation2.StepValue);
                        _forToLoopBinaryOperatorRightOperand = GetCaptureReference(stepValueId, operation2.StepValue);
                        IOperation forToLoopBinaryOperatorRightOperand = VisitRequired(userDefinedInfo.Subtraction);
                        _forToLoopBinaryOperatorLeftOperand = operation12;
                        _forToLoopBinaryOperatorRightOperand = forToLoopBinaryOperatorRightOperand;
                        int nextCaptureId = GetNextCaptureId(loopRegion);
                        VisitAndCapture(userDefinedInfo.GreaterThanOrEqual, nextCaptureId);
                        positiveFlag = GetCaptureReference(nextCaptureId, userDefinedInfo.GreaterThanOrEqual);
                        _forToLoopBinaryOperatorLeftOperand = null;
                        _forToLoopBinaryOperatorRightOperand = null;
                    }
                    else
                    {
                        ConstantValue constantValue2 = operation2.StepValue.GetConstantValue();
                        if (((object)constantValue2 == null || constantValue2.IsBad) && !ITypeSymbolHelpers.IsSignedIntegralType(stepEnumUnderlyingTypeOrSelf) && !ITypeSymbolHelpers.IsUnsignedIntegralType(stepEnumUnderlyingTypeOrSelf))
                        {
                            IOperation operation13 = null;
                            if (ITypeSymbolHelpers.IsNullableType(operation12.Type))
                            {
                                operation13 = MakeIsNullOperation(GetCaptureReference(stepValueId, operation2.StepValue), booleanType);
                                operation12 = CallNullableMember(operation12, SpecialMember.System_Nullable_T_GetValueOrDefault);
                            }
                            ITypeSymbol enumUnderlyingTypeOrSelf = ITypeSymbolHelpers.GetEnumUnderlyingTypeOrSelf(operation12.Type);
                            if (ITypeSymbolHelpers.IsNumericType(enumUnderlyingTypeOrSelf))
                            {
                                int nextCaptureId2 = GetNextCaptureId(loopRegion);
                                BasicBlockBuilder basicBlockBuilder7 = new BasicBlockBuilder(BasicBlockKind.Block);
                                IOperation operation14;
                                if (operation13 != null)
                                {
                                    BasicBlockBuilder basicBlockBuilder8 = new BasicBlockBuilder(BasicBlockKind.Block);
                                    ConditionalBranch(operation13, jumpIfTrue: false, basicBlockBuilder8);
                                    _currentBasicBlock = null;
                                    operation14 = new LiteralOperation(null, operation12.Syntax, booleanType, ConstantValue.Create(value: false), isImplicit: true);
                                    AddStatement(new FlowCaptureOperation(nextCaptureId2, operation14.Syntax, operation14));
                                    UnconditionalBranch(basicBlockBuilder7);
                                    AppendNewBlock(basicBlockBuilder8);
                                }
                                IOperation rightOperand2 = new LiteralOperation(null, operation12.Syntax, operation12.Type, ConstantValue.Default(enumUnderlyingTypeOrSelf.SpecialType), isImplicit: true);
                                operation14 = new BinaryOperation(BinaryOperatorKind.GreaterThanOrEqual, operation12, rightOperand2, isLifted: false, isChecked: false, isCompareText: false, null, null, null, operation12.Syntax, booleanType, null, isImplicit: true);
                                AddStatement(new FlowCaptureOperation(nextCaptureId2, operation14.Syntax, operation14));
                                AppendNewBlock(basicBlockBuilder7);
                                positiveFlag = GetCaptureReference(nextCaptureId2, operation14);
                            }
                        }
                    }
                    IOperation value4 = PopOperand();
                    AddStatement(new SimpleAssignmentOperation(isRef: false, PopOperand(), value4, null, operation2.InitialValue.Syntax, null, null, isImplicit: true));
                }
                PopStackFrameAndLeaveRegion(frame7);
            }
            IOperation negateIfStepNegative(IOperation operand)
            {
                int value2 = stepEnumUnderlyingTypeOrSelf.SpecialType.VBForToShiftBits();
                LiteralOperation rightOperand = new LiteralOperation(null, operand.Syntax, _compilation.GetSpecialType(SpecialType.System_Int32), ConstantValue.Create(value2), isImplicit: true);
                BinaryOperation leftOperand = new BinaryOperation(BinaryOperatorKind.RightShift, GetCaptureReference(stepValueId, operation2.StepValue), rightOperand, isLifted: false, isChecked: false, isCompareText: false, null, null, null, operand.Syntax, operation2.StepValue.Type, null, isImplicit: true);
                return new BinaryOperation(BinaryOperatorKind.ExclusiveOr, leftOperand, operand, isLifted: false, isChecked: false, isCompareText: false, null, null, null, operand.Syntax, operand.Type, null, isImplicit: true);
            }
            IOperation tryCallObjectForLoopControlHelper(SyntaxNode syntax, WellKnownMember helper)
            {
                bool flag2 = helper == WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForLoopInitObj;
                LocalReferenceOperation value3 = new LocalReferenceOperation(loopObject, flag2, null, operation2.LoopControlVariable.Syntax, loopObject.Type, null, isImplicit: true);
                IMethodSymbol methodSymbol = (IMethodSymbol)(_compilation.CommonGetWellKnownTypeMember(helper)?.GetISymbol());
                int parametersCount = WellKnownMembers.GetDescriptor(helper).ParametersCount;
                if (methodSymbol == null)
                {
                    ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(--parametersCount, null);
                    instance[--parametersCount] = value3;
                    do
                    {
                        instance[--parametersCount] = PopOperand();
                    }
                    while (parametersCount != 0);
                    return MakeInvalidOperation(operation2.LimitValue.Syntax, booleanType, instance.ToImmutableAndFree());
                }
                ArrayBuilder<IArgumentOperation> instance2 = ArrayBuilder<IArgumentOperation>.GetInstance(parametersCount, null);
                instance2[--parametersCount] = new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[parametersCount], visitLoopControlVariableReference(forceImplicit: true), OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, syntax, isImplicit: true);
                instance2[--parametersCount] = new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[parametersCount], value3, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, syntax, isImplicit: true);
                do
                {
                    IOperation operation11 = PopOperand();
                    instance2[--parametersCount] = new ArgumentOperation(ArgumentKind.Explicit, methodSymbol.Parameters[parametersCount], operation11, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, flag2 ? operation11.Syntax : syntax, isImplicit: true);
                }
                while (parametersCount != 0);
                return new InvocationOperation(methodSymbol, null, isVirtual: false, instance2.ToImmutableAndFree(), null, operation2.LimitValue.Syntax, methodSymbol.ReturnType, isImplicit: true);
            }
            IOperation visitLoopControlVariableReference(bool forceImplicit)
            {
                if (operation2.LoopControlVariable.Kind != OperationKind.VariableDeclarator)
                {
                    _forceImplicit = forceImplicit;
                    IOperation? result = VisitRequired(operation2.LoopControlVariable);
                    _forceImplicit = false;
                    return result;
                }
                IVariableDeclaratorOperation variableDeclaratorOperation = (IVariableDeclaratorOperation)operation2.LoopControlVariable;
                ILocalSymbol symbol = variableDeclaratorOperation.Symbol;
                return new LocalReferenceOperation(symbol, isDeclaration: true, null, variableDeclaratorOperation.Syntax, symbol.Type, null, isImplicit: true);
            }
        }

        private static FlowCaptureReferenceOperation GetCaptureReference(int id, IOperation underlying)
        {
            return new FlowCaptureReferenceOperation(id, underlying.Syntax, underlying.Type, underlying.GetConstantValue());
        }

        internal override IOperation VisitAggregateQuery(IAggregateQueryOperation operation, int? captureIdForResult)
        {
            SpillEvalStack();
            IOperation currentAggregationGroup = _currentAggregationGroup;
            _currentAggregationGroup = VisitAndCapture(operation.Group);
            IOperation? result = VisitRequired(operation.Aggregation);
            _currentAggregationGroup = currentAggregationGroup;
            return result;
        }

        public override IOperation? VisitSwitch(ISwitchOperation operation, int? captureIdForResult)
        {
            ISwitchOperation operation2 = operation;
            StartVisitingStatement(operation2);
            INamedTypeSymbol booleanType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            IOperation switchValue = VisitAndCapture(operation2.Value);
            ImmutableArray<ILocalSymbol> locals = getLocals();
            RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, locals);
            EnterRegion(region);
            BasicBlockBuilder defaultBody = null;
            BasicBlockBuilder @break = GetLabeledOrNewBlock(operation2.ExitLabel);
            ImmutableArray<ISwitchCaseOperation>.Enumerator enumerator = operation2.Cases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ISwitchCaseOperation current = enumerator.Current;
                handleSection(current);
            }
            if (defaultBody != null)
            {
                UnconditionalBranch(defaultBody);
            }
            LeaveRegion();
            AppendNewBlock(@break);
            return FinishVisitingStatement(operation2);
            ImmutableArray<ILocalSymbol> getLocals()
            {
                ImmutableArray<ILocalSymbol> immutableArray = operation2.Locals;
                ImmutableArray<ISwitchCaseOperation>.Enumerator enumerator3 = operation2.Cases.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    ISwitchCaseOperation current3 = enumerator3.Current;
                    immutableArray = immutableArray.Concat(current3.Locals);
                }
                return immutableArray;
            }
            void handleCase(ICaseClauseOperation caseClause, BasicBlockBuilder body, [System.Diagnostics.CodeAnalysis.DisallowNull] BasicBlockBuilder? nextCase)
            {
                BasicBlockBuilder nextCase2 = nextCase;
                BasicBlockBuilder labeled = GetLabeledOrNewBlock(caseClause.Label);
                LinkBlocks(labeled, body);
                IOperation condition;
                switch (caseClause.CaseKind)
                {
                    case CaseKind.SingleValue:
                        handleEqualityCheck(((ISingleValueCaseClauseOperation)caseClause).Value);
                        break;
                    case CaseKind.Pattern:
                        {
                            IPatternCaseClauseOperation patternCaseClauseOperation = (IPatternCaseClauseOperation)caseClause;
                            EvalStackFrame frame = PushStackFrame();
                            PushOperand(OperationCloner.CloneOperation(switchValue));
                            IPatternOperation pattern = (IPatternOperation)VisitRequired(patternCaseClauseOperation.Pattern);
                            condition = new IsPatternOperation(PopOperand(), pattern, null, patternCaseClauseOperation.Pattern.Syntax, booleanType, isImplicit: true);
                            ConditionalBranch(condition, jumpIfTrue: false, nextCase2);
                            PopStackFrameAndLeaveRegion(frame);
                            if (patternCaseClauseOperation.Guard != null)
                            {
                                AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block));
                                VisitConditionalBranch(patternCaseClauseOperation.Guard, ref nextCase2, jumpIfTrue: false);
                            }
                            AppendNewBlock(labeled);
                            _currentBasicBlock = null;
                            break;
                        }
                    case CaseKind.Relational:
                        {
                            IRelationalCaseClauseOperation relationalCaseClauseOperation = (IRelationalCaseClauseOperation)caseClause;
                            if (relationalCaseClauseOperation.Relation != BinaryOperatorKind.Equals)
                            {
                                throw ExceptionUtilities.UnexpectedValue(relationalCaseClauseOperation.Relation);
                            }
                            handleEqualityCheck(relationalCaseClauseOperation.Value);
                            break;
                        }
                    case CaseKind.Default:
                        _ = (IDefaultCaseClauseOperation)caseClause;
                        if (defaultBody == null)
                        {
                            defaultBody = labeled;
                        }
                        UnconditionalBranch(nextCase2);
                        AppendNewBlock(labeled);
                        _currentBasicBlock = null;
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(caseClause.CaseKind);
                }
                void handleEqualityCheck(IOperation compareWith)
                {
                    bool flag = ITypeSymbolHelpers.IsNullableType(operation2.Value.Type);
                    bool flag2 = ITypeSymbolHelpers.IsNullableType(compareWith.Type);
                    bool flag3 = flag || flag2;
                    EvalStackFrame frame2 = PushStackFrame();
                    PushOperand(OperationCloner.CloneOperation(switchValue));
                    IOperation operation3 = VisitRequired(compareWith);
                    IOperation operation4 = PopOperand();
                    if (flag3)
                    {
                        if (!flag)
                        {
                            if (operation4.Type != null)
                            {
                                operation4 = MakeNullable(operation4, compareWith.Type);
                            }
                        }
                        else if (!flag2 && operation3.Type != null)
                        {
                            operation3 = MakeNullable(operation3, operation2.Value.Type);
                        }
                    }
                    condition = new BinaryOperation(BinaryOperatorKind.Equals, operation4, operation3, flag3, isChecked: false, isCompareText: false, null, null, null, compareWith.Syntax, booleanType, null, isImplicit: true);
                    ConditionalBranch(condition, jumpIfTrue: false, nextCase2);
                    PopStackFrameAndLeaveRegion(frame2);
                    AppendNewBlock(labeled);
                    _currentBasicBlock = null;
                }
            }
            void handleSection(ISwitchCaseOperation section)
            {
                BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                BasicBlockBuilder dest = new BasicBlockBuilder(BasicBlockKind.Block);
                IOperation condition2 = ((SwitchCaseOperation)section).Condition;
                if (condition2 != null)
                {
                    _currentSwitchOperationExpression = switchValue;
                    VisitConditionalBranch(condition2, ref dest, jumpIfTrue: false);
                    _currentSwitchOperationExpression = null;
                }
                else
                {
                    ImmutableArray<ICaseClauseOperation>.Enumerator enumerator2 = section.Clauses.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        ICaseClauseOperation current2 = enumerator2.Current;
                        BasicBlockBuilder basicBlockBuilder2 = new BasicBlockBuilder(BasicBlockKind.Block);
                        handleCase(current2, basicBlockBuilder, basicBlockBuilder2);
                        AppendNewBlock(basicBlockBuilder2);
                    }
                    UnconditionalBranch(dest);
                }
                AppendNewBlock(basicBlockBuilder);
                VisitStatements(section.Body);
                UnconditionalBranch(@break);
                AppendNewBlock(dest);
            }
        }

        private IOperation MakeNullable(IOperation operand, ITypeSymbol type)
        {
            return CreateConversion(operand, type);
        }

        public override IOperation VisitSwitchCase(ISwitchCaseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitSingleValueCaseClause(ISingleValueCaseClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitDefaultCaseClause(IDefaultCaseClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitRelationalCaseClause(IRelationalCaseClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitRangeCaseClause(IRangeCaseClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitPatternCaseClause(IPatternCaseClauseOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation? VisitEnd(IEndOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            BasicBlockBuilder currentBasicBlock = CurrentBasicBlock;
            AppendNewBlock(new BasicBlockBuilder(BasicBlockKind.Block), linkToPrevious: false);
            currentBasicBlock.FallThrough.Kind = ControlFlowBranchSemantics.ProgramTermination;
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitForLoop(IForLoopOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.Locals));
            ImmutableArray<IOperation> before = operation.Before;
            if (before.Length == 1 && before[0].Kind == OperationKind.VariableDeclarationGroup)
            {
                HandleVariableDeclarations((VariableDeclarationGroupOperation)before.Single());
            }
            else
            {
                VisitStatements(before);
            }
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            AppendNewBlock(basicBlockBuilder);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.ConditionLocals));
            BasicBlockBuilder dest = GetLabeledOrNewBlock(operation.ExitLabel);
            if (operation.Condition != null)
            {
                VisitConditionalBranch(operation.Condition, ref dest, jumpIfTrue: false);
            }
            VisitStatement(operation.Body);
            BasicBlockBuilder labeledOrNewBlock = GetLabeledOrNewBlock(operation.ContinueLabel);
            AppendNewBlock(labeledOrNewBlock);
            VisitStatements(operation.AtLoopBottom);
            UnconditionalBranch(basicBlockBuilder);
            LeaveRegion();
            LeaveRegion();
            AppendNewBlock(dest);
            return FinishVisitingStatement(operation);
        }

        internal override IOperation? VisitFixed(IFixedOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, operation.Locals));
            HandleVariableDeclarations(operation.Variables);
            VisitStatement(operation.Body);
            LeaveRegion();
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitVariableDeclarationGroup(IVariableDeclarationGroupOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            HandleVariableDeclarations(operation);
            return FinishVisitingStatement(operation);
        }

        private void HandleVariableDeclarations(IVariableDeclarationGroupOperation operation)
        {
            ImmutableArray<IVariableDeclarationOperation>.Enumerator enumerator = operation.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IVariableDeclarationOperation current = enumerator.Current;
                HandleVariableDeclaration(current);
            }
        }

        private void HandleVariableDeclaration(IVariableDeclarationOperation operation)
        {
            ImmutableArray<IVariableDeclaratorOperation>.Enumerator enumerator = operation.Declarators.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IVariableDeclaratorOperation current = enumerator.Current;
                HandleVariableDeclarator(operation, current);
            }
        }

        private void HandleVariableDeclarator(IVariableDeclarationOperation declaration, IVariableDeclaratorOperation declarator)
        {
            if (declarator.Initializer != null || declaration.Initializer != null)
            {
                ILocalSymbol symbol = declarator.Symbol;
                BasicBlockBuilder basicBlockBuilder = null;
                if (symbol.IsStatic)
                {
                    basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
                    ITypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
                    StaticLocalInitializationSemaphoreOperation condition = new StaticLocalInitializationSemaphoreOperation(symbol, declarator.Syntax, specialType);
                    ConditionalBranch(condition, jumpIfTrue: false, basicBlockBuilder);
                    _currentBasicBlock = null;
                    EnterRegion(new RegionBuilder(ControlFlowRegionKind.StaticLocalInitializer));
                }
                EvalStackFrame frame = PushStackFrame();
                IOperation operation = null;
                SyntaxNode syntax = null;
                if (declarator.Initializer != null)
                {
                    operation = Visit(declarator.Initializer!.Value);
                    syntax = declarator.Syntax;
                }
                if (declaration.Initializer != null)
                {
                    IOperation operation2 = VisitRequired(declaration.Initializer!.Value);
                    syntax = declaration.Syntax;
                    operation = ((operation == null) ? operation2 : new InvalidOperation(ImmutableArray.Create(operation, operation2), null, declaration.Syntax, symbol.Type, null, isImplicit: true));
                }
                LocalReferenceOperation localReferenceOperation = new LocalReferenceOperation(symbol, isDeclaration: true, null, declarator.Syntax, symbol.Type, null, isImplicit: true);
                SimpleAssignmentOperation statement = new SimpleAssignmentOperation(symbol.IsRef, localReferenceOperation, operation, null, syntax, localReferenceOperation.Type, null, isImplicit: true);
                AddStatement(statement);
                PopStackFrameAndLeaveRegion(frame);
                if (symbol.IsStatic)
                {
                    LeaveRegion();
                    AppendNewBlock(basicBlockBuilder);
                }
            }
        }

        public override IOperation VisitVariableDeclaration(IVariableDeclarationOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitVariableDeclarator(IVariableDeclaratorOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitVariableInitializer(IVariableInitializerOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitFlowCapture(IFlowCaptureOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitFlowCaptureReference(IFlowCaptureReferenceOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitIsNull(IIsNullOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitCaughtException(ICaughtExceptionOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitInvocation(IInvocationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            IOperation instance = (operation.TargetMethod.IsStatic ? null : operation.Instance);
            var (instance2, arguments) = VisitInstanceWithArguments(instance, operation.Arguments);
            PopStackFrame(frame);
            return new InvocationOperation(operation.TargetMethod, instance2, operation.IsVirtual, arguments, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        private (IOperation? visitedInstance, ImmutableArray<IArgumentOperation> visitedArguments) VisitInstanceWithArguments(IOperation? instance, ImmutableArray<IArgumentOperation> arguments)
        {
            if (instance != null)
            {
                PushOperand(VisitRequired(instance));
            }
            ImmutableArray<IArgumentOperation> item = VisitArguments(arguments);
            return ((instance == null) ? null : PopOperand(), item);
        }

        internal override IOperation VisitNoPiaObjectCreation(INoPiaObjectCreationOperation operation, int? argument)
        {
            EvalStackFrame frame = PushStackFrame();
            IOperation objectCreation = new NoPiaObjectCreationOperation(null, null, operation.Syntax, operation.Type, IsImplicit(operation));
            return PopStackFrame(frame, HandleObjectOrCollectionInitializer(operation.Initializer, objectCreation));
        }

        public override IOperation VisitObjectCreation(IObjectCreationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            EvalStackFrame frame2 = PushStackFrame();
            ImmutableArray<IArgumentOperation> arguments = VisitArguments(operation.Arguments);
            PopStackFrame(frame2);
            IOperation objectCreation = new ObjectCreationOperation(operation.Constructor, null, arguments, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
            return PopStackFrame(frame, HandleObjectOrCollectionInitializer(operation.Initializer, objectCreation));
        }

        public override IOperation VisitTypeParameterObjectCreation(ITypeParameterObjectCreationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            TypeParameterObjectCreationOperation objectCreation = new TypeParameterObjectCreationOperation(null, null, operation.Syntax, operation.Type, IsImplicit(operation));
            return PopStackFrame(frame, HandleObjectOrCollectionInitializer(operation.Initializer, objectCreation));
        }

        public override IOperation VisitDynamicObjectCreation(IDynamicObjectCreationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            EvalStackFrame frame2 = PushStackFrame();
            ImmutableArray<IOperation> arguments = VisitArray(operation.Arguments);
            PopStackFrame(frame2);
            HasDynamicArgumentsExpression hasDynamicArgumentsExpression = (HasDynamicArgumentsExpression)operation;
            IOperation objectCreation = new DynamicObjectCreationOperation(null, arguments, hasDynamicArgumentsExpression.ArgumentNames, hasDynamicArgumentsExpression.ArgumentRefKinds, null, operation.Syntax, operation.Type, IsImplicit(operation));
            return PopStackFrame(frame, HandleObjectOrCollectionInitializer(operation.Initializer, objectCreation));
        }

        private IOperation HandleObjectOrCollectionInitializer(IObjectOrCollectionInitializerOperation? initializer, IOperation objectCreation)
        {
            if (initializer == null || initializer!.Initializers.IsEmpty)
            {
                return objectCreation;
            }
            PushOperand(objectCreation);
            SpillEvalStack();
            objectCreation = PopOperand();
            visitInitializer(initializer, objectCreation);
            return objectCreation;
            void handleInitializer(IOperation innerInitializer)
            {
                switch (innerInitializer.Kind)
                {
                    case OperationKind.MemberInitializer:
                        handleMemberInitializer((IMemberInitializerOperation)innerInitializer);
                        break;
                    case OperationKind.SimpleAssignment:
                        handleSimpleAssignment((ISimpleAssignmentOperation)innerInitializer);
                        break;
                    default:
                        {
                            EvalStackFrame frame3 = PushStackFrame();
                            AddStatement(Visit(innerInitializer));
                            PopStackFrameAndLeaveRegion(frame3);
                            break;
                        }
                }
            }
            void handleMemberInitializer(IMemberInitializerOperation memberInitializer)
            {
                EvalStackFrame frame = PushStackFrame();
                IOperation initializedInstance2 = (tryPushTarget(memberInitializer.InitializedMember) ? popTarget(memberInitializer.InitializedMember) : VisitRequired(memberInitializer.InitializedMember));
                visitInitializer(memberInitializer.Initializer, initializedInstance2);
                PopStackFrameAndLeaveRegion(frame);
            }
            void handleSimpleAssignment(ISimpleAssignmentOperation assignmentOperation)
            {
                EvalStackFrame frame2 = PushStackFrame();
                IOperation statement;
                if (!tryPushTarget(assignmentOperation.Target))
                {
                    statement = VisitRequired(assignmentOperation);
                }
                else
                {
                    IOperation value = VisitRequired(assignmentOperation.Value);
                    IOperation target = popTarget(assignmentOperation.Target);
                    statement = new SimpleAssignmentOperation(assignmentOperation.IsRef, target, value, null, assignmentOperation.Syntax, assignmentOperation.Type, assignmentOperation.GetConstantValue(), IsImplicit(assignmentOperation));
                }
                AddStatement(statement);
                PopStackFrameAndLeaveRegion(frame2);
            }
            IOperation popTarget(IOperation originalTarget)
            {
                switch (originalTarget.Kind)
                {
                    case OperationKind.FieldReference:
                        {
                            IFieldReferenceOperation fieldReferenceOperation = (IFieldReferenceOperation)originalTarget;
                            IOperation instance2 = ((!fieldReferenceOperation.Member.IsStatic && fieldReferenceOperation.Instance != null) ? PopOperand() : null);
                            return new FieldReferenceOperation(fieldReferenceOperation.Field, fieldReferenceOperation.IsDeclaration, instance2, null, fieldReferenceOperation.Syntax, fieldReferenceOperation.Type, fieldReferenceOperation.GetConstantValue(), IsImplicit(fieldReferenceOperation));
                        }
                    case OperationKind.EventReference:
                        {
                            IEventReferenceOperation eventReferenceOperation = (IEventReferenceOperation)originalTarget;
                            IOperation instance2 = ((!eventReferenceOperation.Member.IsStatic && eventReferenceOperation.Instance != null) ? PopOperand() : null);
                            return new EventReferenceOperation(eventReferenceOperation.Event, instance2, null, eventReferenceOperation.Syntax, eventReferenceOperation.Type, IsImplicit(eventReferenceOperation));
                        }
                    case OperationKind.PropertyReference:
                        {
                            IPropertyReferenceOperation propertyReferenceOperation = (IPropertyReferenceOperation)originalTarget;
                            IOperation instance2 = ((!propertyReferenceOperation.Member.IsStatic && propertyReferenceOperation.Instance != null) ? PopOperand() : null);
                            ImmutableArray<IArgumentOperation> arguments = PopArray(propertyReferenceOperation.Arguments, RewriteArgumentFromArray);
                            return new PropertyReferenceOperation(propertyReferenceOperation.Property, arguments, instance2, null, propertyReferenceOperation.Syntax, propertyReferenceOperation.Type, IsImplicit(propertyReferenceOperation));
                        }
                    case OperationKind.ArrayElementReference:
                        {
                            IArrayElementReferenceOperation arrayElementReferenceOperation = (IArrayElementReferenceOperation)originalTarget;
                            IOperation instance2 = PopOperand();
                            ImmutableArray<IOperation> indices = PopArray(arrayElementReferenceOperation.Indices);
                            return new ArrayElementReferenceOperation(instance2, indices, null, originalTarget.Syntax, originalTarget.Type, IsImplicit(originalTarget));
                        }
                    case OperationKind.DynamicIndexerAccess:
                        {
                            DynamicIndexerAccessOperation dynamicIndexerAccessOperation = (DynamicIndexerAccessOperation)originalTarget;
                            IOperation instance2 = PopOperand();
                            ImmutableArray<IOperation> arguments2 = PopArray(dynamicIndexerAccessOperation.Arguments);
                            return new DynamicIndexerAccessOperation(instance2, arguments2, dynamicIndexerAccessOperation.ArgumentNames, dynamicIndexerAccessOperation.ArgumentRefKinds, null, dynamicIndexerAccessOperation.Syntax, dynamicIndexerAccessOperation.Type, IsImplicit(dynamicIndexerAccessOperation));
                        }
                    case OperationKind.DynamicMemberReference:
                        {
                            IDynamicMemberReferenceOperation dynamicMemberReferenceOperation = (IDynamicMemberReferenceOperation)originalTarget;
                            IOperation instance2 = ((dynamicMemberReferenceOperation.Instance != null) ? PopOperand() : null);
                            return new DynamicMemberReferenceOperation(instance2, dynamicMemberReferenceOperation.MemberName, dynamicMemberReferenceOperation.TypeArguments, dynamicMemberReferenceOperation.ContainingType, null, dynamicMemberReferenceOperation.Syntax, dynamicMemberReferenceOperation.Type, IsImplicit(dynamicMemberReferenceOperation));
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(originalTarget.Kind);
                }
            }
            bool tryPushTarget(IOperation instance)
            {
                switch (instance.Kind)
                {
                    case OperationKind.FieldReference:
                    case OperationKind.PropertyReference:
                    case OperationKind.EventReference:
                        {
                            IMemberReferenceOperation memberReferenceOperation = (IMemberReferenceOperation)instance;
                            if (memberReferenceOperation.Kind == OperationKind.PropertyReference)
                            {
                                VisitAndPushArray(((IPropertyReferenceOperation)memberReferenceOperation).Arguments, UnwrapArgument);
                                SpillEvalStack();
                            }
                            if (!memberReferenceOperation.Member.IsStatic && memberReferenceOperation.Instance != null)
                            {
                                PushOperand(VisitRequired(memberReferenceOperation.Instance));
                            }
                            return true;
                        }
                    case OperationKind.ArrayElementReference:
                        {
                            IArrayElementReferenceOperation arrayElementReferenceOperation2 = (IArrayElementReferenceOperation)instance;
                            VisitAndPushArray(arrayElementReferenceOperation2.Indices);
                            SpillEvalStack();
                            PushOperand(VisitRequired(arrayElementReferenceOperation2.ArrayReference));
                            return true;
                        }
                    case OperationKind.DynamicIndexerAccess:
                        {
                            IDynamicIndexerAccessOperation dynamicIndexerAccessOperation2 = (IDynamicIndexerAccessOperation)instance;
                            VisitAndPushArray(dynamicIndexerAccessOperation2.Arguments);
                            SpillEvalStack();
                            PushOperand(VisitRequired(dynamicIndexerAccessOperation2.Operation));
                            return true;
                        }
                    case OperationKind.DynamicMemberReference:
                        {
                            IDynamicMemberReferenceOperation dynamicMemberReferenceOperation2 = (IDynamicMemberReferenceOperation)instance;
                            if (dynamicMemberReferenceOperation2.Instance != null)
                            {
                                PushOperand(VisitRequired(dynamicMemberReferenceOperation2.Instance));
                            }
                            return true;
                        }
                    default:
                        return false;
                }
            }
            void visitInitializer(IObjectOrCollectionInitializerOperation initializerOperation, IOperation initializedInstance)
            {
                ImplicitInstanceInfo currentImplicitInstance = _currentImplicitInstance;
                _currentImplicitInstance = new ImplicitInstanceInfo(initializedInstance);
                ImmutableArray<IOperation>.Enumerator enumerator = initializerOperation.Initializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IOperation current = enumerator.Current;
                    handleInitializer(current);
                }
                _currentImplicitInstance = currentImplicitInstance;
            }
        }

        public override IOperation VisitObjectOrCollectionInitializer(IObjectOrCollectionInitializerOperation operation, int? captureIdForResult)
        {
            return MakeInvalidOperation(operation.Syntax, operation.Type, ImmutableArray<IOperation>.Empty);
        }

        public override IOperation VisitMemberInitializer(IMemberInitializerOperation operation, int? captureIdForResult)
        {
            return MakeInvalidOperation(operation.Syntax, operation.Type, ImmutableArray<IOperation>.Empty);
        }

        public override IOperation VisitAnonymousObjectCreation(IAnonymousObjectCreationOperation operation, int? captureIdForResult)
        {
            if (operation.Initializers.IsEmpty)
            {
                return new AnonymousObjectCreationOperation(ImmutableArray<IOperation>.Empty, null, operation.Syntax, operation.Type, IsImplicit(operation));
            }
            ImplicitInstanceInfo currentImplicitInstance = _currentImplicitInstance;
            _currentImplicitInstance = new ImplicitInstanceInfo((INamedTypeSymbol)operation.Type);
            SpillEvalStack();
            EvalStackFrame frame = PushStackFrame();
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(operation.Initializers.Length);
            for (int i = 0; i < operation.Initializers.Length; i++)
            {
                ISimpleAssignmentOperation simpleAssignmentOperation = (ISimpleAssignmentOperation)operation.Initializers[i];
                IPropertyReferenceOperation propertyReferenceOperation = (IPropertyReferenceOperation)simpleAssignmentOperation.Target;
                InstanceReferenceOperation instance2 = new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, null, propertyReferenceOperation.Instance!.Syntax, propertyReferenceOperation.Instance!.Type, IsImplicit(propertyReferenceOperation.Instance));
                IOperation target = new PropertyReferenceOperation(propertyReferenceOperation.Property, ImmutableArray<IArgumentOperation>.Empty, instance2, null, propertyReferenceOperation.Syntax, propertyReferenceOperation.Type, IsImplicit(propertyReferenceOperation));
                IOperation value = visitAndCaptureInitializer(propertyReferenceOperation.Property, simpleAssignmentOperation.Value);
                SimpleAssignmentOperation item = new SimpleAssignmentOperation(simpleAssignmentOperation.IsRef, target, value, null, simpleAssignmentOperation.Syntax, simpleAssignmentOperation.Type, simpleAssignmentOperation.GetConstantValue(), IsImplicit(simpleAssignmentOperation));
                instance.Add(item);
            }
            _currentImplicitInstance.Free();
            _currentImplicitInstance = currentImplicitInstance;
            for (int j = 0; j < instance.Count; j++)
            {
                PopOperand();
            }
            PopStackFrame(frame);
            return new AnonymousObjectCreationOperation(instance.ToImmutableAndFree(), null, operation.Syntax, operation.Type, IsImplicit(operation));
            IOperation visitAndCaptureInitializer(IPropertySymbol initializedProperty, IOperation initializer)
            {
                PushOperand(VisitRequired(initializer));
                SpillEvalStack();
                IOperation operation2 = PeekOperand();
                _currentImplicitInstance.AnonymousTypePropertyValues![initializedProperty] = operation2;
                return operation2;
            }
        }

        public override IOperation? VisitLocalFunction(ILocalFunctionOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            RegionBuilder regionBuilder = CurrentRegionRequired;
            while (regionBuilder.IsStackSpillRegion)
            {
                regionBuilder = regionBuilder.Enclosing;
            }
            regionBuilder.Add(operation.Symbol, operation);
            return FinishVisitingStatement(operation);
        }

        private IOperation? VisitLocalFunctionAsRoot(ILocalFunctionOperation operation)
        {
            VisitMethodBodies(operation.Body, operation.IgnoredBody);
            return null;
        }

        public override IOperation VisitAnonymousFunction(IAnonymousFunctionOperation operation, int? captureIdForResult)
        {
            _haveAnonymousFunction = true;
            Context context = GetCurrentContext();
            return new FlowAnonymousFunctionOperation(in context, operation, IsImplicit(operation));
        }

        public override IOperation VisitFlowAnonymousFunction(IFlowAnonymousFunctionOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitArrayCreation(IArrayCreationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            VisitAndPushArray(operation.DimensionSizes);
            IArrayInitializerOperation initializer = (IArrayInitializerOperation)Visit(operation.Initializer);
            ImmutableArray<IOperation> dimensionSizes = PopArray(operation.DimensionSizes);
            PopStackFrame(frame);
            return new ArrayCreationOperation(dimensionSizes, initializer, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitArrayInitializer(IArrayInitializerOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            visitAndPushArrayInitializerValues(operation);
            return PopStackFrame(frame, popAndAssembleArrayInitializerValues(operation));
            IArrayInitializerOperation popAndAssembleArrayInitializerValues(IArrayInitializerOperation initializer)
            {
                ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(initializer.ElementValues.Length);
                for (int num = initializer.ElementValues.Length - 1; num >= 0; num--)
                {
                    IOperation operation2 = initializer.ElementValues[num];
                    IOperation item = ((operation2.Kind != OperationKind.ArrayInitializer) ? PopOperand() : popAndAssembleArrayInitializerValues((IArrayInitializerOperation)operation2));
                    instance.Add(item);
                }
                instance.ReverseContents();
                return new ArrayInitializerOperation(instance.ToImmutableAndFree(), null, initializer.Syntax, IsImplicit(initializer));
            }
            void visitAndPushArrayInitializerValues(IArrayInitializerOperation initializer)
            {
                ImmutableArray<IOperation>.Enumerator enumerator = initializer.ElementValues.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IOperation current = enumerator.Current;
                    if (current.Kind == OperationKind.ArrayInitializer)
                    {
                        visitAndPushArrayInitializerValues((IArrayInitializerOperation)current);
                    }
                    else
                    {
                        PushOperand(VisitRequired(current));
                    }
                }
            }
        }

        public override IOperation VisitInstanceReference(IInstanceReferenceOperation operation, int? captureIdForResult)
        {
            if (operation.ReferenceKind == InstanceReferenceKind.ImplicitReceiver)
            {
                if (_currentImplicitInstance.ImplicitInstance != null)
                {
                    return OperationCloner.CloneOperation(_currentImplicitInstance.ImplicitInstance);
                }
                return MakeInvalidOperation(operation.Syntax, operation.Type, ImmutableArray<IOperation>.Empty);
            }
            return new InstanceReferenceOperation(operation.ReferenceKind, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDynamicInvocation(IDynamicInvocationOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            if (operation.Operation.Kind == OperationKind.DynamicMemberReference)
            {
                IOperation instance = ((IDynamicMemberReferenceOperation)operation.Operation).Instance;
                if (instance != null)
                {
                    PushOperand(VisitRequired(instance));
                }
            }
            else
            {
                PushOperand(VisitRequired(operation.Operation));
            }
            ImmutableArray<IOperation> arguments = VisitArray(operation.Arguments);
            IOperation operation2;
            if (operation.Operation.Kind == OperationKind.DynamicMemberReference)
            {
                IDynamicMemberReferenceOperation dynamicMemberReferenceOperation = (IDynamicMemberReferenceOperation)operation.Operation;
                operation2 = new DynamicMemberReferenceOperation((dynamicMemberReferenceOperation.Instance != null) ? PopOperand() : null, dynamicMemberReferenceOperation.MemberName, dynamicMemberReferenceOperation.TypeArguments, dynamicMemberReferenceOperation.ContainingType, null, dynamicMemberReferenceOperation.Syntax, dynamicMemberReferenceOperation.Type, IsImplicit(dynamicMemberReferenceOperation));
            }
            else
            {
                operation2 = PopOperand();
            }
            PopStackFrame(frame);
            return new DynamicInvocationOperation(operation2, arguments, ((HasDynamicArgumentsExpression)operation).ArgumentNames, ((HasDynamicArgumentsExpression)operation).ArgumentRefKinds, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDynamicIndexerAccess(IDynamicIndexerAccessOperation operation, int? captureIdForResult)
        {
            PushOperand(VisitRequired(operation.Operation));
            ImmutableArray<IOperation> arguments = VisitArray(operation.Arguments);
            return new DynamicIndexerAccessOperation(PopOperand(), arguments, ((HasDynamicArgumentsExpression)operation).ArgumentNames, ((HasDynamicArgumentsExpression)operation).ArgumentRefKinds, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDynamicMemberReference(IDynamicMemberReferenceOperation operation, int? captureIdForResult)
        {
            return new DynamicMemberReferenceOperation(Visit(operation.Instance), operation.MemberName, operation.TypeArguments, operation.ContainingType, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDeconstructionAssignment(IDeconstructionAssignmentOperation operation, int? captureIdForResult)
        {
            var (target, value) = VisitPreservingTupleOperations(operation.Target, operation.Value);
            return new DeconstructionAssignmentOperation(target, value, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        private void PushTargetAndUnwrapTupleIfNecessary(IOperation value)
        {
            if (value.Kind == OperationKind.Tuple)
            {
                ImmutableArray<IOperation>.Enumerator enumerator = ((ITupleOperation)value).Elements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IOperation current = enumerator.Current;
                    PushTargetAndUnwrapTupleIfNecessary(current);
                }
            }
            else
            {
                PushOperand(VisitRequired(value));
            }
        }

        private IOperation PopTargetAndWrapTupleIfNecessary(IOperation value)
        {
            if (value.Kind == OperationKind.Tuple)
            {
                ITupleOperation tupleOperation = (ITupleOperation)value;
                int length = tupleOperation.Elements.Length;
                ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(length);
                for (int num = length - 1; num >= 0; num--)
                {
                    instance.Add(PopTargetAndWrapTupleIfNecessary(tupleOperation.Elements[num]));
                }
                instance.ReverseContents();
                return new TupleOperation(instance.ToImmutableAndFree(), tupleOperation.NaturalType, null, tupleOperation.Syntax, tupleOperation.Type, IsImplicit(tupleOperation));
            }
            return PopOperand();
        }

        public override IOperation VisitDeclarationExpression(IDeclarationExpressionOperation operation, int? captureIdForResult)
        {
            return new DeclarationExpressionOperation(VisitPreservingTupleOperations(operation.Expression), null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        private IOperation VisitPreservingTupleOperations(IOperation operation)
        {
            EvalStackFrame frame = PushStackFrame();
            PushTargetAndUnwrapTupleIfNecessary(operation);
            return PopStackFrame(frame, PopTargetAndWrapTupleIfNecessary(operation));
        }

        private (IOperation visitedLeft, IOperation visitedRight) VisitPreservingTupleOperations(IOperation left, IOperation right)
        {
            EvalStackFrame frame = PushStackFrame();
            PushTargetAndUnwrapTupleIfNecessary(left);
            IOperation item = VisitRequired(right);
            IOperation item2 = PopTargetAndWrapTupleIfNecessary(left);
            PopStackFrame(frame);
            return (item2, item);
        }

        public override IOperation VisitTuple(ITupleOperation operation, int? captureIdForResult)
        {
            return VisitPreservingTupleOperations(operation);
        }

        internal override IOperation VisitNoneOperation(IOperation operation, int? captureIdForResult)
        {
            if (_currentStatement == operation)
            {
                return VisitNoneOperationStatement(operation);
            }
            return VisitNoneOperationExpression(operation);
        }

        private IOperation VisitNoneOperationStatement(IOperation operation)
        {
            VisitStatements(((Operation)operation).ChildOperations.ToImmutableArray());
            return new NoneOperation(ImmutableArray<IOperation>.Empty, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        private IOperation VisitNoneOperationExpression(IOperation operation)
        {
            return PopStackFrame(PushStackFrame(), new NoneOperation(VisitArray(((Operation)operation).ChildOperations.ToImmutableArray()), null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation)));
        }

        public override IOperation VisitInterpolatedString(IInterpolatedStringOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            ImmutableArray<IInterpolatedStringContentOperation>.Enumerator enumerator = operation.Parts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IInterpolatedStringContentOperation current = enumerator.Current;
                if (current.Kind == OperationKind.Interpolation)
                {
                    IInterpolationOperation interpolationOperation = (IInterpolationOperation)current;
                    PushOperand(VisitRequired(interpolationOperation.Expression));
                    if (interpolationOperation.Alignment != null)
                    {
                        PushOperand(VisitRequired(interpolationOperation.Alignment));
                    }
                }
            }
            ArrayBuilder<IInterpolatedStringContentOperation> instance = ArrayBuilder<IInterpolatedStringContentOperation>.GetInstance(operation.Parts.Length);
            for (int num = operation.Parts.Length - 1; num >= 0; num--)
            {
                IInterpolatedStringContentOperation interpolatedStringContentOperation = operation.Parts[num];
                IInterpolatedStringContentOperation item;
                if (interpolatedStringContentOperation.Kind == OperationKind.Interpolation)
                {
                    IInterpolationOperation interpolationOperation2 = (IInterpolationOperation)interpolatedStringContentOperation;
                    IOperation formatString = ((interpolationOperation2.FormatString == null) ? null : VisitLiteral((ILiteralOperation)interpolationOperation2.FormatString, null));
                    IOperation alignment = ((interpolationOperation2.Alignment != null) ? PopOperand() : null);
                    item = new InterpolationOperation(PopOperand(), alignment, formatString, null, interpolatedStringContentOperation.Syntax, IsImplicit(interpolatedStringContentOperation));
                }
                else
                {
                    IInterpolatedStringTextOperation interpolatedStringTextOperation = (IInterpolatedStringTextOperation)interpolatedStringContentOperation;
                    item = new InterpolatedStringTextOperation(VisitLiteral((ILiteralOperation)interpolatedStringTextOperation.Text, null), null, interpolatedStringContentOperation.Syntax, IsImplicit(interpolatedStringContentOperation));
                }
                instance.Add(item);
            }
            instance.ReverseContents();
            PopStackFrame(frame);
            return new InterpolatedStringOperation(instance.ToImmutableAndFree(), null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitInterpolatedStringText(IInterpolatedStringTextOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitInterpolation(IInterpolationOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitNameOf(INameOfOperation operation, int? captureIdForResult)
        {
            return new LiteralOperation(null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitLiteral(ILiteralOperation operation, int? captureIdForResult)
        {
            return new LiteralOperation(null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitLocalReference(ILocalReferenceOperation operation, int? captureIdForResult)
        {
            return new LocalReferenceOperation(operation.Local, operation.IsDeclaration, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitParameterReference(IParameterReferenceOperation operation, int? captureIdForResult)
        {
            return new ParameterReferenceOperation(operation.Parameter, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitFieldReference(IFieldReferenceOperation operation, int? captureIdForResult)
        {
            IOperation instance = (operation.Field.IsStatic ? null : Visit(operation.Instance));
            return new FieldReferenceOperation(operation.Field, operation.IsDeclaration, instance, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitMethodReference(IMethodReferenceOperation operation, int? captureIdForResult)
        {
            IOperation instance = (operation.Method.IsStatic ? null : Visit(operation.Instance));
            return new MethodReferenceOperation(operation.Method, operation.IsVirtual, instance, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitPropertyReference(IPropertyReferenceOperation operation, int? captureIdForResult)
        {
            if (operation.Instance is IInstanceReferenceOperation instanceReferenceOperation && instanceReferenceOperation.ReferenceKind == InstanceReferenceKind.ImplicitReceiver && operation.Property.ContainingType.IsAnonymousType && operation.Property.ContainingType == _currentImplicitInstance.AnonymousType)
            {
                if (_currentImplicitInstance.AnonymousTypePropertyValues!.TryGetValue(operation.Property, out var value))
                {
                    if (!(value is IFlowCaptureReferenceOperation flowCaptureReferenceOperation))
                    {
                        return OperationCloner.CloneOperation(value);
                    }
                    return GetCaptureReference(flowCaptureReferenceOperation.Id.Value, operation);
                }
                return MakeInvalidOperation(operation.Syntax, operation.Type, ImmutableArray<IOperation>.Empty);
            }
            EvalStackFrame frame = PushStackFrame();
            IOperation instance = (operation.Property.IsStatic ? null : operation.Instance);
            var (instance2, arguments) = VisitInstanceWithArguments(instance, operation.Arguments);
            PopStackFrame(frame);
            return new PropertyReferenceOperation(operation.Property, arguments, instance2, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitEventReference(IEventReferenceOperation operation, int? captureIdForResult)
        {
            IOperation instance = (operation.Event.IsStatic ? null : Visit(operation.Instance));
            return new EventReferenceOperation(operation.Event, instance, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitTypeOf(ITypeOfOperation operation, int? captureIdForResult)
        {
            return new TypeOfOperation(operation.TypeOperand, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitParenthesized(IParenthesizedOperation operation, int? captureIdForResult)
        {
            return new ParenthesizedOperation(VisitRequired(operation.Operand), null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitAwait(IAwaitOperation operation, int? captureIdForResult)
        {
            return new AwaitOperation(VisitRequired(operation.Operation), null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitSizeOf(ISizeOfOperation operation, int? captureIdForResult)
        {
            return new SizeOfOperation(operation.TypeOperand, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitStop(IStopOperation operation, int? captureIdForResult)
        {
            return new StopOperation(null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitIsType(IIsTypeOperation operation, int? captureIdForResult)
        {
            return new IsTypeOperation(VisitRequired(operation.ValueOperand), operation.TypeOperand, operation.IsNegated, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation? VisitParameterInitializer(IParameterInitializerOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            ParameterReferenceOperation rewrittenTarget = new ParameterReferenceOperation(operation.Parameter, null, operation.Syntax, operation.Parameter.Type, isImplicit: true);
            VisitInitializer(rewrittenTarget, operation);
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitFieldInitializer(IFieldInitializerOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            ImmutableArray<IFieldSymbol>.Enumerator enumerator = operation.InitializedFields.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IFieldSymbol current = enumerator.Current;
                IInstanceReferenceOperation instance = (current.IsStatic ? null : new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, null, operation.Syntax, current.ContainingType, isImplicit: true));
                FieldReferenceOperation rewrittenTarget = new FieldReferenceOperation(current, isDeclaration: false, instance, null, operation.Syntax, current.Type, null, isImplicit: true);
                VisitInitializer(rewrittenTarget, operation);
            }
            return FinishVisitingStatement(operation);
        }

        public override IOperation? VisitPropertyInitializer(IPropertyInitializerOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            ImmutableArray<IPropertySymbol>.Enumerator enumerator = operation.InitializedProperties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IPropertySymbol current = enumerator.Current;
                InstanceReferenceOperation instance = (current.IsStatic ? null : new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, null, operation.Syntax, current.ContainingType, isImplicit: true));
                ImmutableArray<IArgumentOperation> arguments;
                if (!current.Parameters.IsEmpty)
                {
                    ArrayBuilder<IArgumentOperation> instance2 = ArrayBuilder<IArgumentOperation>.GetInstance(current.Parameters.Length);
                    ImmutableArray<IParameterSymbol>.Enumerator enumerator2 = current.Parameters.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        IParameterSymbol current2 = enumerator2.Current;
                        InvalidOperation value = new InvalidOperation(ImmutableArray<IOperation>.Empty, null, operation.Syntax, current2.Type, null, isImplicit: true);
                        ArgumentOperation item = new ArgumentOperation(ArgumentKind.Explicit, current2, value, OperationFactory.IdentityConversion, OperationFactory.IdentityConversion, null, operation.Syntax, isImplicit: true);
                        instance2.Add(item);
                    }
                    arguments = instance2.ToImmutableAndFree();
                }
                else
                {
                    arguments = ImmutableArray<IArgumentOperation>.Empty;
                }
                IOperation rewrittenTarget = new PropertyReferenceOperation(current, arguments, instance, null, operation.Syntax, current.Type, isImplicit: true);
                VisitInitializer(rewrittenTarget, operation);
            }
            return FinishVisitingStatement(operation);
        }

        private void VisitInitializer(IOperation rewrittenTarget, ISymbolInitializerOperation initializer)
        {
            EnterRegion(new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, initializer.Locals));
            EvalStackFrame frame = PushStackFrame();
            SimpleAssignmentOperation statement = new SimpleAssignmentOperation(isRef: false, rewrittenTarget, VisitRequired(initializer.Value), null, initializer.Syntax, rewrittenTarget.Type, null, isImplicit: true);
            AddStatement(statement);
            PopStackFrameAndLeaveRegion(frame);
            LeaveRegion();
        }

        public override IOperation VisitEventAssignment(IEventAssignmentOperation operation, int? captureIdForResult)
        {
            IEventAssignmentOperation operation2 = operation;
            EvalStackFrame frame = PushStackFrame();
            IEventReferenceOperation eventReferenceOperation = getEventReference();
            IOperation handlerValue;
            IOperation eventReference;
            if (eventReferenceOperation != null)
            {
                IOperation operation3 = (eventReferenceOperation.Event.IsStatic ? null : eventReferenceOperation.Instance);
                if (operation3 != null)
                {
                    PushOperand(VisitRequired(operation3));
                }
                handlerValue = VisitRequired(operation2.HandlerValue);
                IOperation instance = ((operation3 == null) ? null : PopOperand());
                eventReference = new EventReferenceOperation(eventReferenceOperation.Event, instance, null, operation2.EventReference.Syntax, operation2.EventReference.Type, IsImplicit(operation2.EventReference));
            }
            else
            {
                PushOperand(VisitRequired(operation2.EventReference));
                handlerValue = VisitRequired(operation2.HandlerValue);
                eventReference = PopOperand();
            }
            PopStackFrame(frame);
            return new EventAssignmentOperation(eventReference, handlerValue, operation2.Adds, null, operation2.Syntax, operation2.Type, IsImplicit(operation2));
            IEventReferenceOperation? getEventReference()
            {
                IOperation operation4 = operation2.EventReference;
                while (true)
                {
                    switch (operation4.Kind)
                    {
                        case OperationKind.EventReference:
                            return (IEventReferenceOperation)operation4;
                        case OperationKind.Parenthesized:
                            break;
                        default:
                            return null;
                    }
                    operation4 = ((IParenthesizedOperation)operation4).Operand;
                }
            }
        }

        public override IOperation VisitRaiseEvent(IRaiseEventOperation operation, int? captureIdForResult)
        {
            StartVisitingStatement(operation);
            EvalStackFrame frame = PushStackFrame();
            IOperation operation2 = (operation.EventReference.Event.IsStatic ? null : operation.EventReference.Instance);
            if (operation2 != null)
            {
                PushOperand(VisitRequired(operation2));
            }
            ImmutableArray<IArgumentOperation> arguments = VisitArguments(operation.Arguments);
            IOperation instance = ((operation2 == null) ? null : PopOperand());
            EventReferenceOperation eventReference = new EventReferenceOperation(operation.EventReference.Event, instance, null, operation.EventReference.Syntax, operation.EventReference.Type, IsImplicit(operation.EventReference));
            PopStackFrame(frame);
            return FinishVisitingStatement(operation, new RaiseEventOperation(eventReference, arguments, null, operation.Syntax, IsImplicit(operation)));
        }

        public override IOperation VisitAddressOf(IAddressOfOperation operation, int? captureIdForResult)
        {
            return new AddressOfOperation(VisitRequired(operation.Reference), null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitIncrementOrDecrement(IIncrementOrDecrementOperation operation, int? captureIdForResult)
        {
            return new IncrementOrDecrementOperation(operation.IsPostfix, operation.IsLifted, operation.IsChecked, VisitRequired(operation.Target), operation.OperatorMethod, operation.Kind, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDiscardOperation(IDiscardOperation operation, int? captureIdForResult)
        {
            return new DiscardOperation(operation.DiscardSymbol, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitDiscardPattern(IDiscardPatternOperation pat, int? captureIdForResult)
        {
            return new DiscardPatternOperation(pat.InputType, pat.NarrowedType, null, pat.Syntax, IsImplicit(pat));
        }

        public override IOperation VisitOmittedArgument(IOmittedArgumentOperation operation, int? captureIdForResult)
        {
            return new OmittedArgumentOperation(null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        internal override IOperation VisitPlaceholder(IPlaceholderOperation operation, int? captureIdForResult)
        {
            switch (operation.PlaceholderKind)
            {
                case PlaceholderKind.SwitchOperationExpression:
                    if (_currentSwitchOperationExpression != null)
                    {
                        return OperationCloner.CloneOperation(_currentSwitchOperationExpression);
                    }
                    break;
                case PlaceholderKind.ForToLoopBinaryOperatorLeftOperand:
                    if (_forToLoopBinaryOperatorLeftOperand != null)
                    {
                        return _forToLoopBinaryOperatorLeftOperand;
                    }
                    break;
                case PlaceholderKind.ForToLoopBinaryOperatorRightOperand:
                    if (_forToLoopBinaryOperatorRightOperand != null)
                    {
                        return _forToLoopBinaryOperatorRightOperand;
                    }
                    break;
                case PlaceholderKind.AggregationGroup:
                    if (_currentAggregationGroup != null)
                    {
                        return OperationCloner.CloneOperation(_currentAggregationGroup);
                    }
                    break;
            }
            return new PlaceholderOperation(operation.PlaceholderKind, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitConversion(IConversionOperation operation, int? captureIdForResult)
        {
            return new ConversionOperation(VisitRequired(operation.Operand), ((ConversionOperation)operation).ConversionConvertible, operation.IsTryCast, operation.IsChecked, null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitDefaultValue(IDefaultValueOperation operation, int? captureIdForResult)
        {
            return new DefaultValueOperation(null, operation.Syntax, operation.Type, operation.GetConstantValue(), IsImplicit(operation));
        }

        public override IOperation VisitIsPattern(IIsPatternOperation operation, int? captureIdForResult)
        {
            EvalStackFrame frame = PushStackFrame();
            PushOperand(VisitRequired(operation.Value));
            IPatternOperation pattern = (IPatternOperation)VisitRequired(operation.Pattern);
            IOperation value = PopOperand();
            PopStackFrame(frame);
            return new IsPatternOperation(value, pattern, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitInvalid(IInvalidOperation operation, int? captureIdForResult)
        {
            IInvalidOperation operation2 = operation;
            ArrayBuilder<IOperation> children = ArrayBuilder<IOperation>.GetInstance();
            children.AddRange(((InvalidOperation)operation2).Children);
            if (children.Count != 0 && children.Last().Kind == OperationKind.ObjectOrCollectionInitializer)
            {
                SpillEvalStack();
                EvalStackFrame frame = PushStackFrame();
                IObjectOrCollectionInitializerOperation initializer = (IObjectOrCollectionInitializerOperation)children.Last();
                children.RemoveLast();
                EvalStackFrame frame2 = PushStackFrame();
                ArrayBuilder<IOperation>.Enumerator enumerator = children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IOperation current = enumerator.Current;
                    PushOperand(VisitRequired(current));
                }
                for (int num = children.Count - 1; num >= 0; num--)
                {
                    children[num] = PopOperand();
                }
                PopStackFrame(frame2);
                IOperation objectCreation = new InvalidOperation(children.ToImmutableAndFree(), null, operation2.Syntax, operation2.Type, operation2.GetConstantValue(), IsImplicit(operation2));
                objectCreation = HandleObjectOrCollectionInitializer(initializer, objectCreation);
                PopStackFrame(frame);
                return objectCreation;
            }
            if (_currentStatement == operation2)
            {
                return visitInvalidOperationStatement(operation2);
            }
            return visitInvalidOperationExpression(operation2);
            IOperation visitInvalidOperationExpression(IInvalidOperation invalidOperation)
            {
                return PopStackFrame(PushStackFrame(), new InvalidOperation(VisitArray(children.ToImmutableAndFree()), null, invalidOperation.Syntax, invalidOperation.Type, invalidOperation.GetConstantValue(), IsImplicit(operation2)));
            }
            IOperation visitInvalidOperationStatement(IInvalidOperation invalidOperation)
            {
                VisitStatements(children.ToImmutableAndFree());
                return new InvalidOperation(ImmutableArray<IOperation>.Empty, null, invalidOperation.Syntax, invalidOperation.Type, invalidOperation.GetConstantValue(), IsImplicit(invalidOperation));
            }
        }

        public override IOperation? VisitReDim(IReDimOperation operation, int? argument)
        {
            StartVisitingStatement(operation);
            bool isImplicit = operation.Clauses.Length > 1 || IsImplicit(operation);
            ImmutableArray<IReDimClauseOperation>.Enumerator enumerator = operation.Clauses.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IReDimClauseOperation current = enumerator.Current;
                EvalStackFrame frame = PushStackFrame();
                ReDimOperation statement = new ReDimOperation(ImmutableArray.Create(visitReDimClause(current)), operation.Preserve, null, operation.Syntax, isImplicit);
                AddStatement(statement);
                PopStackFrameAndLeaveRegion(frame);
            }
            return FinishVisitingStatement(operation);
            IReDimClauseOperation visitReDimClause(IReDimClauseOperation clause)
            {
                PushOperand(VisitRequired(clause.Operand));
                ImmutableArray<IOperation> dimensionSizes = VisitArray(clause.DimensionSizes);
                return new ReDimClauseOperation(PopOperand(), dimensionSizes, null, clause.Syntax, IsImplicit(clause));
            }
        }

        public override IOperation VisitReDimClause(IReDimClauseOperation operation, int? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitTranslatedQuery(ITranslatedQueryOperation operation, int? captureIdForResult)
        {
            return new TranslatedQueryOperation(VisitRequired(operation.Operation), null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitConstantPattern(IConstantPatternOperation operation, int? captureIdForResult)
        {
            return new ConstantPatternOperation(VisitRequired(operation.Value), operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitRelationalPattern(IRelationalPatternOperation operation, int? argument)
        {
            return new RelationalPatternOperation(operation.OperatorKind, VisitRequired(operation.Value), operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitBinaryPattern(IBinaryPatternOperation operation, int? argument)
        {
            return new BinaryPatternOperation(operation.OperatorKind, (IPatternOperation)VisitRequired(operation.LeftPattern), (IPatternOperation)VisitRequired(operation.RightPattern), operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitNegatedPattern(INegatedPatternOperation operation, int? argument)
        {
            return new NegatedPatternOperation((IPatternOperation)VisitRequired(operation.Pattern), operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitTypePattern(ITypePatternOperation operation, int? argument)
        {
            return new TypePatternOperation(operation.MatchedType, operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitDeclarationPattern(IDeclarationPatternOperation operation, int? captureIdForResult)
        {
            return new DeclarationPatternOperation(operation.MatchedType, operation.MatchesNull, operation.DeclaredSymbol, operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitRecursivePattern(IRecursivePatternOperation operation, int? argument)
        {
            return new RecursivePatternOperation(operation.MatchedType, operation.DeconstructSymbol, operation.DeconstructionSubpatterns.SelectAsArray((IPatternOperation p, ControlFlowGraphBuilder @this) => (IPatternOperation)@this.VisitRequired(p), this), operation.PropertySubpatterns.SelectAsArray((IPropertySubpatternOperation p, ControlFlowGraphBuilder @this) => (IPropertySubpatternOperation)@this.VisitRequired(p), this), operation.DeclaredSymbol, operation.InputType, operation.NarrowedType, null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitPropertySubpattern(IPropertySubpatternOperation operation, int? argument)
        {
            return new PropertySubpatternOperation(VisitRequired(operation.Member), (IPatternOperation)VisitRequired(operation.Pattern), null, operation.Syntax, IsImplicit(operation));
        }

        public override IOperation VisitDelegateCreation(IDelegateCreationOperation operation, int? captureIdForResult)
        {
            return new DelegateCreationOperation(VisitRequired(operation.Target), null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitRangeOperation(IRangeOperation operation, int? argument)
        {
            if (operation.LeftOperand != null)
            {
                PushOperand(VisitRequired(operation.LeftOperand));
            }
            IOperation rightOperand = null;
            if (operation.RightOperand != null)
            {
                rightOperand = Visit(operation.RightOperand);
            }
            return new RangeOperation((operation.LeftOperand == null) ? null : PopOperand(), rightOperand, operation.IsLifted, operation.Method, null, operation.Syntax, operation.Type, IsImplicit(operation));
        }

        public override IOperation VisitSwitchExpression(ISwitchExpressionOperation operation, int? captureIdForResult)
        {
            INamedTypeSymbol specialType = _compilation.GetSpecialType(SpecialType.System_Boolean);
            SpillEvalStack();
            RegionBuilder currentRegionRequired = CurrentRegionRequired;
            int num = captureIdForResult ?? GetNextCaptureId(currentRegionRequired);
            IOperation operation2 = VisitAndCapture(operation.Value);
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder(BasicBlockKind.Block);
            ImmutableArray<ISwitchExpressionArmOperation>.Enumerator enumerator = operation.Arms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ISwitchExpressionArmOperation current = enumerator.Current;
                RegionBuilder region = new RegionBuilder(ControlFlowRegionKind.LocalLifetime, null, current.Locals);
                EnterRegion(region);
                BasicBlockBuilder dest = new BasicBlockBuilder(BasicBlockKind.Block);
                EvalStackFrame frame = PushStackFrame();
                IPatternOperation pattern = (IPatternOperation)VisitRequired(current.Pattern);
                IsPatternOperation condition = new IsPatternOperation(OperationCloner.CloneOperation(operation2), pattern, null, current.Syntax, specialType, IsImplicit(current));
                ConditionalBranch(condition, jumpIfTrue: false, dest);
                _currentBasicBlock = null;
                PopStackFrameAndLeaveRegion(frame);
                if (current.Guard != null)
                {
                    EvalStackFrame frame2 = PushStackFrame();
                    VisitConditionalBranch(current.Guard, ref dest, jumpIfTrue: false);
                    _currentBasicBlock = null;
                    PopStackFrameAndLeaveRegion(frame2);
                }
                VisitAndCapture(current.Value, num);
                UnconditionalBranch(basicBlockBuilder);
                AppendNewBlock(dest);
                LeaveRegion();
            }
            LeaveRegionsUpTo(currentRegionRequired);
            IMethodSymbol methodSymbol = (IMethodSymbol)((_compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Runtime_CompilerServices_SwitchExpressionException__ctor) ?? _compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_InvalidOperationException__ctor))?.GetISymbol());
            IOperation operation4;
            if (methodSymbol != null)
            {
                IOperation operation3 = new ObjectCreationOperation(methodSymbol, null, ImmutableArray<IArgumentOperation>.Empty, null, operation.Syntax, methodSymbol.ContainingType, null, isImplicit: true);
                operation4 = operation3;
            }
            else
            {
                operation4 = MakeInvalidOperation(operation.Syntax, _compilation.GetSpecialType(SpecialType.System_Object), ImmutableArray<IOperation>.Empty);
            }
            IOperation exception = operation4;
            LinkThrowStatement(exception);
            _currentBasicBlock = null;
            AppendNewBlock(basicBlockBuilder, linkToPrevious: false);
            return GetCaptureReference(num, operation);
        }

        private void VisitUsingVariableDeclarationOperation(IUsingDeclarationOperation operation, ReadOnlySpan<IOperation> statements)
        {
            IOperation currentStatement = _currentStatement;
            _currentStatement = operation;
            StartVisitingStatement(operation);
            ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(statements.Length);
            ArrayBuilder<IOperation> arrayBuilder = null;
            ReadOnlySpan<IOperation> readOnlySpan = statements;
            for (int i = 0; i < readOnlySpan.Length; i++)
            {
                IOperation operation2 = readOnlySpan[i];
                if (operation2.Kind == OperationKind.LocalFunction)
                {
                    (arrayBuilder ??= ArrayBuilder<IOperation>.GetInstance()).Add(operation2);
                }
                else
                {
                    instance.Add(operation2);
                }
            }
            BlockOperation body = BlockOperation.CreateTemporaryBlock(instance.ToImmutableAndFree(), ((Operation)operation).OwningSemanticModel, operation.Syntax);
            DisposeOperationInfo disposeInfo = ((UsingDeclarationOperation)operation).DisposeInfo;
            HandleUsingOperationParts(operation.DeclarationGroup, body, disposeInfo.DisposeMethod, disposeInfo.DisposeArguments, ImmutableArray<ILocalSymbol>.Empty, operation.IsAsynchronous);
            FinishVisitingStatement(operation);
            _currentStatement = currentStatement;
            if (arrayBuilder != null)
            {
                VisitStatements(arrayBuilder.ToImmutableAndFree());
            }
        }

        public IOperation? Visit(IOperation? operation)
        {
            return Visit(operation, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("operation")]
        public IOperation? VisitRequired(IOperation? operation, int? argument = null)
        {
            return Visit(operation, argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("operation")]
        public IOperation? BaseVisitRequired(IOperation? operation, int? argument)
        {
            return base.Visit(operation, argument);
        }

        public override IOperation? Visit(IOperation? operation, int? argument)
        {
            if (operation == null)
            {
                return null;
            }
            return PopStackFrame(PushStackFrame(), base.Visit(operation, argument));
        }

        public override IOperation DefaultVisit(IOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitArgument(IArgumentOperation operation, int? captureIdForResult)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitUsingDeclaration(IUsingDeclarationOperation operation, int? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitWith(IWithOperation operation, int? argument)
        {
            EvalStackFrame frame = PushStackFrame();
            IOperation operation2 = VisitRequired(operation.Operand);
            IOperation operation4;
            if (operation.CloneMethod != null)
            {
                IOperation operation3 = new InvocationOperation(operation.CloneMethod, operation2, isVirtual: true, ImmutableArray<IArgumentOperation>.Empty, null, operation.Syntax, operation.Type, isImplicit: true);
                operation4 = operation3;
            }
            else
            {
                operation4 = MakeInvalidOperation(operation2.Type, operation2);
            }
            IOperation objectCreation = operation4;
            return PopStackFrame(frame, HandleObjectOrCollectionInitializer(operation.Initializer, objectCreation));
        }
    }
}
