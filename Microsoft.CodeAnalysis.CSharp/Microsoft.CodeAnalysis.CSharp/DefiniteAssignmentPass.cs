using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class DefiniteAssignmentPass : LocalDataFlowPass<DefiniteAssignmentPass.LocalState, DefiniteAssignmentPass.LocalFunctionState>
    {
        private sealed class SameDiagnosticComparer : EqualityComparer<Diagnostic>
        {
            public static readonly SameDiagnosticComparer Instance = new SameDiagnosticComparer();

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(Diagnostic obj)
            {
                return Hash.Combine(Hash.CombineValues(obj.Arguments), Hash.Combine(obj.Location.GetHashCode(), obj.Code));
            }
        }

        internal struct LocalState : ILocalDataFlowState, AbstractFlowPass<LocalState, LocalFunctionState>.ILocalState
        {
            internal BitVector Assigned;

            public bool NormalizeToBottom { get; }

            public bool Reachable
            {
                get
                {
                    if (Assigned.Capacity > 0)
                    {
                        return !IsAssigned(0);
                    }
                    return true;
                }
            }

            internal LocalState(BitVector assigned, bool normalizeToBottom = false)
            {
                Assigned = assigned;
                NormalizeToBottom = normalizeToBottom;
            }

            public LocalState Clone()
            {
                return new LocalState(Assigned.Clone());
            }

            public bool IsAssigned(int slot)
            {
                return Assigned[slot];
            }

            public void Assign(int slot)
            {
                if (slot != -1)
                {
                    Assigned[slot] = true;
                }
            }

            public void Unassign(int slot)
            {
                if (slot != -1)
                {
                    Assigned[slot] = false;
                }
            }
        }

        internal sealed class LocalFunctionState : AbstractLocalFunctionState
        {
            public BitVector ReadVars = BitVector.Empty;

            public BitVector CapturedMask = BitVector.Null;

            public BitVector InvertedCapturedMask = BitVector.Null;

            public LocalFunctionState(LocalState stateFromBottom, LocalState stateFromTop)
                : base(stateFromBottom, stateFromTop)
            {
            }
        }

        private readonly PooledDictionary<VariableIdentifier, int> _variableSlot = PooledDictionary<VariableIdentifier, int>.GetInstance();

        protected readonly ArrayBuilder<VariableIdentifier> variableBySlot = ArrayBuilder<VariableIdentifier>.GetInstance(1, default(VariableIdentifier));

        private readonly HashSet<Symbol> initiallyAssignedVariables;

        private readonly PooledHashSet<LocalSymbol> _usedVariables = PooledHashSet<LocalSymbol>.GetInstance();

        private PooledHashSet<ParameterSymbol>? _readParameters;

        private readonly PooledHashSet<LocalFunctionSymbol> _usedLocalFunctions = PooledHashSet<LocalFunctionSymbol>.GetInstance();

        private readonly PooledHashSet<Symbol> _writtenVariables = PooledHashSet<Symbol>.GetInstance();

        private readonly PooledDictionary<Symbol, Location> _unsafeAddressTakenVariables = PooledDictionary<Symbol, Location>.GetInstance();

        private readonly PooledHashSet<Symbol> _capturedVariables = PooledHashSet<Symbol>.GetInstance();

        private readonly PooledHashSet<Symbol> _capturedInside = PooledHashSet<Symbol>.GetInstance();

        private readonly PooledHashSet<Symbol> _capturedOutside = PooledHashSet<Symbol>.GetInstance();

        private readonly SourceAssemblySymbol _sourceAssembly;

        private readonly HashSet<PrefixUnaryExpressionSyntax> _unassignedVariableAddressOfSyntaxes;

        private BitVector _alreadyReported;

        private readonly bool _requireOutParamsAssigned;

        private readonly bool _trackClassFields;

        private readonly bool _trackStaticMembers;

        protected MethodSymbol topLevelMethod;

        protected bool _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;

        private readonly bool _shouldCheckConverted;

        public sealed override bool AwaitUsingAndForeachAddsPendingBranch => true;

        internal DefiniteAssignmentPass(CSharpCompilation compilation, Symbol member, BoundNode node, bool strictAnalysis, bool trackUnassignments = false, HashSet<PrefixUnaryExpressionSyntax> unassignedVariableAddressOfSyntaxes = null, bool requireOutParamsAssigned = true, bool trackClassFields = false, bool trackStaticMembers = false)
            : base(compilation, member, node, strictAnalysis ? EmptyStructTypeCache.CreatePrecise() : EmptyStructTypeCache.CreateForDev12Compatibility(compilation), trackUnassignments)
        {
            initiallyAssignedVariables = null;
            _sourceAssembly = (((object)member == null) ? null : ((SourceAssemblySymbol)member.ContainingAssembly));
            _unassignedVariableAddressOfSyntaxes = unassignedVariableAddressOfSyntaxes;
            _requireOutParamsAssigned = requireOutParamsAssigned;
            _trackClassFields = trackClassFields;
            _trackStaticMembers = trackStaticMembers;
            topLevelMethod = member as MethodSymbol;
            _shouldCheckConverted = GetType() == typeof(DefiniteAssignmentPass);
        }

        internal DefiniteAssignmentPass(CSharpCompilation compilation, Symbol member, BoundNode node, EmptyStructTypeCache emptyStructs, bool trackUnassignments = false, HashSet<Symbol> initiallyAssignedVariables = null)
            : base(compilation, member, node, emptyStructs, trackUnassignments)
        {
            this.initiallyAssignedVariables = initiallyAssignedVariables;
            _sourceAssembly = (((object)member == null) ? null : ((SourceAssemblySymbol)member.ContainingAssembly));
            CurrentSymbol = member;
            _unassignedVariableAddressOfSyntaxes = null;
            _requireOutParamsAssigned = true;
            topLevelMethod = member as MethodSymbol;
            _shouldCheckConverted = GetType() == typeof(DefiniteAssignmentPass);
        }

        internal DefiniteAssignmentPass(CSharpCompilation compilation, Symbol member, BoundNode node, BoundNode firstInRegion, BoundNode lastInRegion, HashSet<Symbol> initiallyAssignedVariables, HashSet<PrefixUnaryExpressionSyntax> unassignedVariableAddressOfSyntaxes, bool trackUnassignments)
            : base(compilation, member, node, EmptyStructTypeCache.CreateNeverEmpty(), firstInRegion, lastInRegion, trackRegions: true, trackUnassignments)
        {
            this.initiallyAssignedVariables = initiallyAssignedVariables;
            _sourceAssembly = null;
            CurrentSymbol = member;
            _unassignedVariableAddressOfSyntaxes = unassignedVariableAddressOfSyntaxes;
            _shouldCheckConverted = GetType() == typeof(DefiniteAssignmentPass);
        }

        protected override void Free()
        {
            variableBySlot.Free();
            _variableSlot.Free();
            _usedVariables.Free();
            _readParameters?.Free();
            _usedLocalFunctions.Free();
            _writtenVariables.Free();
            _capturedVariables.Free();
            _capturedInside.Free();
            _capturedOutside.Free();
            _unsafeAddressTakenVariables.Free();
            base.Free();
        }

        protected override bool TryGetVariable(VariableIdentifier identifier, out int slot)
        {
            return _variableSlot.TryGetValue(identifier, out slot);
        }

        protected override int AddVariable(VariableIdentifier identifier)
        {
            int count = variableBySlot.Count;
            _variableSlot.Add(identifier, count);
            variableBySlot.Add(identifier);
            return count;
        }

        protected Symbol GetNonMemberSymbol(int slot)
        {
            VariableIdentifier variableIdentifier = variableBySlot[slot];
            while (variableIdentifier.ContainingSlot > 0)
            {
                variableIdentifier = variableBySlot[variableIdentifier.ContainingSlot];
            }
            return variableIdentifier.Symbol;
        }

        private int RootSlot(int slot)
        {
            while (true)
            {
                int containingSlot = variableBySlot[slot].ContainingSlot;
                if (containingSlot == 0)
                {
                    break;
                }
                slot = containingSlot;
            }
            return slot;
        }

        protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
        {
            return _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException;
        }

        protected override ImmutableArray<AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch> Scan(ref bool badRegion)
        {
            base.Diagnostics.Clear();
            ImmutableArray<ParameterSymbol> methodParameters = base.MethodParameters;
            ParameterSymbol methodThisParameter = base.MethodThisParameter;
            _alreadyReported = BitVector.Empty;
            regionPlace = RegionPlace.Before;
            EnterParameters(methodParameters);
            if ((object)methodThisParameter != null)
            {
                EnterParameter(methodThisParameter);
                if (methodThisParameter.Type.SpecialType != 0)
                {
                    int orCreateSlot = GetOrCreateSlot(methodThisParameter);
                    SetSlotState(orCreateSlot, assigned: true);
                }
            }
            ImmutableArray<PendingBranch> result = base.Scan(ref badRegion);
            if (ShouldAnalyzeOutParameters(out var location))
            {
                LeaveParameters(methodParameters, null, location);
                if ((object)methodThisParameter != null)
                {
                    LeaveParameter(methodThisParameter, null, location);
                }
                LocalState self = State;
                ImmutableArray<PendingBranch>.Enumerator enumerator = result.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PendingBranch current = enumerator.Current;
                    State = current.State;
                    LeaveParameters(methodParameters, current.Branch.Syntax, null);
                    if ((object)methodThisParameter != null)
                    {
                        LeaveParameter(methodThisParameter, current.Branch.Syntax, null);
                    }
                    Join(ref self, ref State);
                }
                State = self;
            }
            return result;
        }

        protected override ImmutableArray<AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch> RemoveReturns()
        {
            ImmutableArray<PendingBranch> immutableArray = base.RemoveReturns();
            if (CurrentSymbol is MethodSymbol methodSymbol && methodSymbol.IsAsync && !methodSymbol.IsImplicitlyDeclared && !immutableArray.Any((PendingBranch pending) => HasAwait(pending)))
            {
                Location location = ((CurrentSymbol is LambdaSymbol lambdaSymbol) ? lambdaSymbol.DiagnosticLocation : CurrentSymbol.Locations.FirstOrNone());
                base.Diagnostics.Add(ErrorCode.WRN_AsyncLacksAwaits, location);
            }
            return immutableArray;
        }

        private static bool HasAwait(AbstractFlowPass<LocalState, LocalFunctionState>.PendingBranch pending)
        {
            BoundNode branch = pending.Branch;
            if (branch == null)
            {
                return false;
            }
            return branch.Kind switch
            {
                BoundKind.AwaitExpression => true,
                BoundKind.UsingStatement => ((BoundUsingStatement)branch).AwaitOpt != null,
                BoundKind.ForEachStatement => ((BoundForEachStatement)branch).AwaitOpt != null,
                BoundKind.UsingLocalDeclarations => ((BoundUsingLocalDeclarations)branch).AwaitOpt != null,
                _ => false,
            };
        }

        protected virtual void ReportUnassignedOutParameter(ParameterSymbol parameter, SyntaxNode node, Location location)
        {
            if ((!_requireOutParamsAssigned && (object)topLevelMethod == CurrentSymbol) || base.Diagnostics == null || !State.Reachable)
            {
                return;
            }
            if (location == null)
            {
                location = new SourceLocation(node);
            }
            bool flag = false;
            if (parameter.IsThis)
            {
                int num = VariableSlot(parameter);
                if (!State.IsAssigned(num))
                {
                    foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(parameter.Type))
                    {
                        if (_emptyStructTypeCache.IsEmptyStructType(structInstanceField.Type) || LocalDataFlowPass<LocalState, LocalFunctionState>.HasInitializer(structInstanceField))
                        {
                            continue;
                        }
                        int num2 = VariableSlot(structInstanceField, num);
                        if (num2 == -1 || !State.IsAssigned(num2))
                        {
                            Symbol associatedSymbol = structInstanceField.AssociatedSymbol;
                            if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Property)
                            {
                                base.Diagnostics.Add(ErrorCode.ERR_UnassignedThisAutoProperty, location, associatedSymbol);
                            }
                            else
                            {
                                base.Diagnostics.Add(ErrorCode.ERR_UnassignedThis, location, structInstanceField);
                            }
                        }
                    }
                    flag = true;
                }
            }
            if (!flag)
            {
                base.Diagnostics.Add(ErrorCode.ERR_ParamUnassigned, location, parameter.Name);
            }
        }

        public static void Analyze(CSharpCompilation compilation, MethodSymbol member, BoundNode node, DiagnosticBag diagnostics, bool requireOutParamsAssigned = true)
        {
            DiagnosticBag diagnosticBag = analyze(strictAnalysis: true);
            if (diagnosticBag.IsEmptyWithoutResolution)
            {
                diagnosticBag.Free();
                return;
            }
            DiagnosticBag diagnosticBag2 = analyze(strictAnalysis: false);
            if (diagnosticBag2.AsEnumerable().Any((Diagnostic d) => d.Code == 8078))
            {
                diagnostics.AddRangeAndFree(diagnosticBag2);
                diagnosticBag.Free();
                return;
            }
            if (diagnosticBag.Count == diagnosticBag2.Count)
            {
                diagnostics.AddRangeAndFree(diagnosticBag);
                diagnosticBag2.Free();
                return;
            }
            HashSet<Diagnostic> hashSet = new HashSet<Diagnostic>(diagnosticBag2.AsEnumerable(), SameDiagnosticComparer.Instance);
            diagnosticBag2.Free();
            foreach (Diagnostic item in diagnosticBag.AsEnumerable())
            {
                if (item.Severity != DiagnosticSeverity.Error || hashSet.Contains(item))
                {
                    diagnostics.Add(item);
                    continue;
                }
                ErrorCode code = (ErrorCode)item.Code;
                ErrorCode code2 = code switch
                {
                    ErrorCode.ERR_UnassignedThisAutoProperty => ErrorCode.WRN_UnassignedThisAutoProperty,
                    ErrorCode.ERR_UnassignedThis => ErrorCode.WRN_UnassignedThis,
                    ErrorCode.ERR_ParamUnassigned => ErrorCode.WRN_ParamUnassigned,
                    ErrorCode.ERR_UseDefViolationProperty => ErrorCode.WRN_UseDefViolationProperty,
                    ErrorCode.ERR_UseDefViolationField => ErrorCode.WRN_UseDefViolationField,
                    ErrorCode.ERR_UseDefViolationThis => ErrorCode.WRN_UseDefViolationThis,
                    ErrorCode.ERR_UseDefViolationOut => ErrorCode.WRN_UseDefViolationOut,
                    ErrorCode.ERR_UseDefViolation => ErrorCode.WRN_UseDefViolation,
                    _ => code,
                };
                object?[] array;
                if (item is DiagnosticWithInfo diagnosticWithInfo)
                {
                    DiagnosticInfo info = diagnosticWithInfo.Info;
                    if (info != null)
                    {
                        object[] arguments = info.Arguments;
                        array = arguments;
                        goto IL_0201;
                    }
                }
                array = item.Arguments.ToArray();
                goto IL_0201;
            IL_0201:
                object[] args = array;
                diagnostics.Add(code2, item.Location, args);
            }
            diagnosticBag.Free();
            DiagnosticBag analyze(bool strictAnalysis)
            {
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                DefiniteAssignmentPass definiteAssignmentPass = new DefiniteAssignmentPass(compilation, member, node, strictAnalysis, trackUnassignments: false, null, requireOutParamsAssigned)
                {
                    _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true
                };
                try
                {
                    bool badRegion = false;
                    definiteAssignmentPass.Analyze(ref badRegion, instance);
                    return instance;
                }
                catch (CancelledByStackGuardException ex) when (diagnostics != null)
                {
                    ex.AddAnError(instance);
                    return instance;
                }
                finally
                {
                    definiteAssignmentPass.Free();
                }
            }
        }

        protected void Analyze(ref bool badRegion, DiagnosticBag diagnostics)
        {
            Analyze(ref badRegion);
            if (diagnostics == null)
            {
                return;
            }
            foreach (Symbol capturedVariable in _capturedVariables)
            {
                if (_unsafeAddressTakenVariables.TryGetValue(capturedVariable, out var value))
                {
                    diagnostics.Add(ErrorCode.ERR_LocalCantBeFixedAndHoisted, value, capturedVariable.Name);
                }
            }
            diagnostics.AddRange(base.Diagnostics);
            if (!(CurrentSymbol is SynthesizedRecordConstructor))
            {
                return;
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = base.MethodParameters.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                ParameterSymbol current2 = enumerator2.Current;
                PooledHashSet<ParameterSymbol>? readParameters = _readParameters;
                if (readParameters == null || !readParameters!.Contains(current2))
                {
                    diagnostics.Add(ErrorCode.WRN_UnreadRecordParameter, current2.Locations.FirstOrNone(), current2.Name);
                }
            }
        }

        private void CheckCaptured(Symbol variable, ParameterSymbol rangeVariableUnderlyingParameter = null)
        {
            if (CurrentSymbol is SourceMethodSymbol containingSymbol && Symbol.IsCaptured(rangeVariableUnderlyingParameter ?? variable, containingSymbol))
            {
                NoteCaptured(variable);
            }
        }

        private void NoteCaptured(Symbol variable)
        {
            if (regionPlace == RegionPlace.Inside)
            {
                _capturedInside.Add(variable);
                _capturedVariables.Add(variable);
            }
            else if (variable.Kind != SymbolKind.RangeVariable)
            {
                _capturedOutside.Add(variable);
                _capturedVariables.Add(variable);
            }
        }

        protected IEnumerable<Symbol> GetCapturedInside()
        {
            return _capturedInside.ToArray();
        }

        protected IEnumerable<Symbol> GetCapturedOutside()
        {
            return _capturedOutside.ToArray();
        }

        protected IEnumerable<Symbol> GetCaptured()
        {
            return _capturedVariables.ToArray();
        }

        protected IEnumerable<Symbol> GetUnsafeAddressTaken()
        {
            return _unsafeAddressTakenVariables.Keys.ToArray();
        }

        protected IEnumerable<MethodSymbol> GetUsedLocalFunctions()
        {
            return _usedLocalFunctions.ToArray();
        }

        private void NoteRecordParameterReadIfNeeded(Symbol symbol)
        {
            if (symbol is ParameterSymbol item && symbol.ContainingSymbol is SynthesizedRecordConstructor)
            {
                if (_readParameters == null)
                {
                    _readParameters = PooledHashSet<ParameterSymbol>.GetInstance();
                }
                _readParameters!.Add(item);
            }
        }

        protected virtual void NoteRead(Symbol variable, ParameterSymbol rangeVariableUnderlyingParameter = null)
        {
            if (variable is LocalSymbol item)
            {
                _usedVariables.Add(item);
            }
            NoteRecordParameterReadIfNeeded(variable);
            if (variable is LocalFunctionSymbol item2)
            {
                _usedLocalFunctions.Add(item2);
            }
            if ((object)variable != null)
            {
                if ((object)_sourceAssembly != null && variable.Kind == SymbolKind.Field)
                {
                    _sourceAssembly.NoteFieldAccess((FieldSymbol)variable.OriginalDefinition, read: true, write: false);
                }
                CheckCaptured(variable, rangeVariableUnderlyingParameter);
            }
        }

        private void NoteRead(BoundNode fieldOrEventAccess)
        {
            BoundNode boundNode = fieldOrEventAccess;
            while (boundNode != null)
            {
                switch (boundNode.Kind)
                {
                    default:
                        return;
                    case BoundKind.FieldAccess:
                        {
                            BoundFieldAccess boundFieldAccess = (BoundFieldAccess)boundNode;
                            NoteRead(boundFieldAccess.FieldSymbol);
                            if (MayRequireTracking(boundFieldAccess.ReceiverOpt, boundFieldAccess.FieldSymbol))
                            {
                                boundNode = boundFieldAccess.ReceiverOpt;
                                break;
                            }
                            return;
                        }
                    case BoundKind.EventAccess:
                        {
                            BoundEventAccess boundEventAccess = (BoundEventAccess)boundNode;
                            FieldSymbol associatedField = boundEventAccess.EventSymbol.AssociatedField;
                            if ((object)associatedField != null)
                            {
                                NoteRead(associatedField);
                                if (MayRequireTracking(boundEventAccess.ReceiverOpt, associatedField))
                                {
                                    boundNode = boundEventAccess.ReceiverOpt;
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case BoundKind.ThisReference:
                        NoteRead(base.MethodThisParameter);
                        return;
                    case BoundKind.Local:
                        NoteRead(((BoundLocal)boundNode).LocalSymbol);
                        return;
                    case BoundKind.Parameter:
                        NoteRead(((BoundParameter)boundNode).ParameterSymbol);
                        return;
                }
            }
        }

        protected virtual void NoteWrite(Symbol variable, BoundExpression value, bool read)
        {
            if ((object)variable != null)
            {
                _writtenVariables.Add(variable);
                if ((object)_sourceAssembly != null && variable.Kind == SymbolKind.Field)
                {
                    FieldSymbol fieldSymbol = (FieldSymbol)variable.OriginalDefinition;
                    _sourceAssembly.NoteFieldAccess(fieldSymbol, read && WriteConsideredUse(fieldSymbol.Type, value), write: true);
                }
                LocalSymbol localSymbol = variable as LocalSymbol;
                if ((object)localSymbol != null && read && WriteConsideredUse(localSymbol.Type, value))
                {
                    _usedVariables.Add(localSymbol);
                }
                CheckCaptured(variable);
            }
        }

        internal static bool WriteConsideredUse(TypeSymbol type, BoundExpression value)
        {
            if (value == null || value.HasAnyErrors)
            {
                return true;
            }
            if ((object)type != null && type.IsReferenceType && type.SpecialType != SpecialType.System_String)
            {
                return value.ConstantValue != ConstantValue.Null;
            }
            if ((object)type != null && type.IsPointerOrFunctionPointer())
            {
                return true;
            }
            if (value != null && (object)value.ConstantValue != null && value.Kind != BoundKind.InterpolatedString)
            {
                return false;
            }
            switch (value.Kind)
            {
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)value;
                        if (boundConversion.ConversionKind.IsUserDefinedConversion() || boundConversion.ConversionKind == ConversionKind.IntPtr)
                        {
                            return true;
                        }
                        return WriteConsideredUse(null, boundConversion.Operand);
                    }
                case BoundKind.DefaultLiteral:
                case BoundKind.DefaultExpression:
                    return false;
                case BoundKind.ObjectCreationExpression:
                    {
                        BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)value;
                        if (boundObjectCreationExpression.Constructor.IsImplicitlyDeclared)
                        {
                            return boundObjectCreationExpression.InitializerExpressionOpt != null;
                        }
                        return true;
                    }
                case BoundKind.TupleLiteral:
                case BoundKind.ConvertedTupleLiteral:
                    return false;
                default:
                    return true;
            }
        }

        private void NoteWrite(BoundExpression n, BoundExpression value, bool read)
        {
            while (n != null)
            {
                switch (n.Kind)
                {
                    case BoundKind.FieldAccess:
                        {
                            BoundFieldAccess boundFieldAccess = (BoundFieldAccess)n;
                            if ((object)_sourceAssembly != null)
                            {
                                FieldSymbol originalDefinition2 = boundFieldAccess.FieldSymbol.OriginalDefinition;
                                _sourceAssembly.NoteFieldAccess(originalDefinition2, value == null || WriteConsideredUse(boundFieldAccess.FieldSymbol.Type, value), write: true);
                            }
                            if (MayRequireTracking(boundFieldAccess.ReceiverOpt, boundFieldAccess.FieldSymbol))
                            {
                                n = boundFieldAccess.ReceiverOpt;
                                if (n.Kind == BoundKind.Local)
                                {
                                    _usedVariables.Add(((BoundLocal)n).LocalSymbol);
                                }
                                break;
                            }
                            return;
                        }
                    case BoundKind.EventAccess:
                        {
                            BoundEventAccess boundEventAccess = (BoundEventAccess)n;
                            FieldSymbol associatedField = boundEventAccess.EventSymbol.AssociatedField;
                            if ((object)associatedField != null)
                            {
                                if ((object)_sourceAssembly != null)
                                {
                                    FieldSymbol originalDefinition = associatedField.OriginalDefinition;
                                    _sourceAssembly.NoteFieldAccess(originalDefinition, value == null || WriteConsideredUse(associatedField.Type, value), write: true);
                                }
                                if (MayRequireTracking(boundEventAccess.ReceiverOpt, associatedField))
                                {
                                    n = boundEventAccess.ReceiverOpt;
                                    break;
                                }
                                return;
                            }
                            return;
                        }
                    case BoundKind.ThisReference:
                        NoteWrite(base.MethodThisParameter, value, read);
                        return;
                    case BoundKind.Local:
                        NoteWrite(((BoundLocal)n).LocalSymbol, value, read);
                        return;
                    case BoundKind.Parameter:
                        NoteWrite(((BoundParameter)n).ParameterSymbol, value, read);
                        return;
                    case BoundKind.RangeVariable:
                        NoteWrite(((BoundRangeVariable)n).Value, value, read);
                        return;
                    default:
                        return;
                }
            }
        }

        protected override void Normalize(ref LocalState state)
        {
            int capacity = state.Assigned.Capacity;
            int count = variableBySlot.Count;
            state.Assigned.EnsureCapacity(count);
            for (int i = capacity; i < count; i++)
            {
                int containingSlot = variableBySlot[i].ContainingSlot;
                bool value = containingSlot > 0 && state.Assigned[containingSlot] && variableBySlot[containingSlot].Symbol.GetTypeOrReturnType().TypeKind == TypeKind.Struct;
                if (state.NormalizeToBottom && containingSlot == 0)
                {
                    value = true;
                }
                state.Assigned[i] = value;
            }
        }

        protected override bool TryGetReceiverAndMember(BoundExpression expr, out BoundExpression receiver, out Symbol member)
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
                            return _trackStaticMembers;
                        }
                        receiver = boundFieldAccess.ReceiverOpt;
                        break;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)expr;
                        EventSymbol eventSymbol = boundEventAccess.EventSymbol;
                        member = eventSymbol.AssociatedField;
                        if (eventSymbol.IsStatic)
                        {
                            return _trackStaticMembers;
                        }
                        receiver = boundEventAccess.ReceiverOpt;
                        break;
                    }
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)expr;
                        if (Binder.AccessingAutoPropertyFromConstructor(boundPropertyAccess, CurrentSymbol))
                        {
                            PropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
                            member = (propertySymbol as SourcePropertySymbolBase)?.BackingField;
                            if ((object)member == null)
                            {
                                return false;
                            }
                            if (propertySymbol.IsStatic)
                            {
                                return _trackStaticMembers;
                            }
                            receiver = boundPropertyAccess.ReceiverOpt;
                        }
                        break;
                    }
            }
            if ((object)member != null && receiver != null && receiver.Kind != BoundKind.TypeExpression)
            {
                return MayRequireTrackingReceiverType(receiver.Type);
            }
            return false;
        }

        private bool MayRequireTrackingReceiverType(TypeSymbol type)
        {
            if ((object)type != null)
            {
                if (!_trackClassFields)
                {
                    return type.TypeKind == TypeKind.Struct;
                }
                return true;
            }
            return false;
        }

        protected bool MayRequireTracking(BoundExpression receiverOpt, FieldSymbol fieldSymbol)
        {
            if ((object)fieldSymbol != null && receiverOpt != null && !fieldSymbol.IsStatic && !fieldSymbol.IsFixedSizeBuffer && receiverOpt.Kind != BoundKind.TypeExpression && MayRequireTrackingReceiverType(receiverOpt.Type))
            {
                return !receiverOpt.Type.IsPrimitiveRecursiveStruct();
            }
            return false;
        }

        protected void CheckAssigned(Symbol symbol, SyntaxNode node)
        {
            if ((object)symbol == null)
            {
                return;
            }
            NoteRead(symbol);
            if (State.Reachable)
            {
                int num = VariableSlot(symbol);
                if (num >= State.Assigned.Capacity)
                {
                    Normalize(ref State);
                }
                if (num > 0 && !State.IsAssigned(num))
                {
                    ReportUnassignedIfNotCapturedInLocalFunction(symbol, node, num);
                }
            }
        }

        private void ReportUnassignedIfNotCapturedInLocalFunction(Symbol symbol, SyntaxNode node, int slot, bool skipIfUseBeforeDeclaration = true)
        {
            if (IsCapturedInLocalFunction(slot))
            {
                RecordReadInLocalFunction(slot);
            }
            else
            {
                ReportUnassigned(symbol, node, slot, skipIfUseBeforeDeclaration);
            }
        }

        protected virtual void ReportUnassigned(Symbol symbol, SyntaxNode node, int slot, bool skipIfUseBeforeDeclaration)
        {
            if (slot <= 0 || (symbol is LocalSymbol localSymbol && localSymbol.IsConst))
            {
                return;
            }
            if (slot >= _alreadyReported.Capacity)
            {
                _alreadyReported.EnsureCapacity(variableBySlot.Count);
            }
            if ((!skipIfUseBeforeDeclaration || symbol.Kind != SymbolKind.Local || (symbol.Locations.Length != 0 && node.Span.End >= symbol.Locations.FirstOrNone().SourceSpan.Start)) && !_alreadyReported[slot] && !symbol.GetTypeOrReturnType().Type.IsErrorType())
            {
                string name = symbol.Name;
                ErrorCode code;
                if (symbol.Kind != SymbolKind.Field)
                {
                    code = ((symbol.Kind != SymbolKind.Parameter || ((ParameterSymbol)symbol).RefKind != RefKind.Out) ? ErrorCode.ERR_UseDefViolation : ((!((ParameterSymbol)symbol).IsThis) ? ErrorCode.ERR_UseDefViolationOut : ErrorCode.ERR_UseDefViolationThis));
                }
                else
                {
                    Symbol associatedSymbol = ((FieldSymbol)symbol).AssociatedSymbol;
                    if ((object)associatedSymbol != null && associatedSymbol.Kind == SymbolKind.Property)
                    {
                        code = ErrorCode.ERR_UseDefViolationProperty;
                        name = associatedSymbol.Name;
                    }
                    else
                    {
                        code = ErrorCode.ERR_UseDefViolationField;
                    }
                }
                base.Diagnostics.Add(code, new SourceLocation(node), name);
            }
            _alreadyReported[slot] = true;
        }

        protected virtual void CheckAssigned(BoundExpression expr, FieldSymbol fieldSymbol, SyntaxNode node)
        {
            if (State.Reachable && !IsAssigned(expr, out var unassignedSlot))
            {
                ReportUnassignedIfNotCapturedInLocalFunction(fieldSymbol, node, unassignedSlot);
            }
            NoteRead(expr);
        }

        private bool IsAssigned(BoundExpression node, out int unassignedSlot)
        {
            unassignedSlot = -1;
            if (_emptyStructTypeCache.IsEmptyStructType(node.Type))
            {
                return true;
            }
            switch (node.Kind)
            {
                case BoundKind.ThisReference:
                    if ((object)base.MethodThisParameter == null)
                    {
                        unassignedSlot = -1;
                        return true;
                    }
                    unassignedSlot = GetOrCreateSlot(base.MethodThisParameter);
                    break;
                case BoundKind.Local:
                    unassignedSlot = GetOrCreateSlot(((BoundLocal)node).LocalSymbol);
                    break;
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)node;
                        if (!MayRequireTracking(boundFieldAccess.ReceiverOpt, boundFieldAccess.FieldSymbol) || IsAssigned(boundFieldAccess.ReceiverOpt, out unassignedSlot))
                        {
                            return true;
                        }
                        unassignedSlot = GetOrCreateSlot(boundFieldAccess.FieldSymbol, unassignedSlot);
                        break;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)node;
                        if (!MayRequireTracking(boundEventAccess.ReceiverOpt, boundEventAccess.EventSymbol.AssociatedField) || IsAssigned(boundEventAccess.ReceiverOpt, out unassignedSlot))
                        {
                            return true;
                        }
                        unassignedSlot = GetOrCreateSlot(boundEventAccess.EventSymbol.AssociatedField, unassignedSlot);
                        break;
                    }
                case BoundKind.PropertyAccess:
                    {
                        BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)node;
                        if (Binder.AccessingAutoPropertyFromConstructor(boundPropertyAccess, CurrentSymbol))
                        {
                            SynthesizedBackingFieldSymbol synthesizedBackingFieldSymbol = (boundPropertyAccess.PropertySymbol as SourcePropertySymbolBase)?.BackingField;
                            if (synthesizedBackingFieldSymbol != null)
                            {
                                if (!MayRequireTracking(boundPropertyAccess.ReceiverOpt, synthesizedBackingFieldSymbol) || IsAssigned(boundPropertyAccess.ReceiverOpt, out unassignedSlot))
                                {
                                    return true;
                                }
                                unassignedSlot = GetOrCreateSlot(synthesizedBackingFieldSymbol, unassignedSlot);
                                break;
                            }
                        }
                        goto default;
                    }
                case BoundKind.Parameter:
                    {
                        BoundParameter boundParameter = (BoundParameter)node;
                        unassignedSlot = GetOrCreateSlot(boundParameter.ParameterSymbol);
                        break;
                    }
                default:
                    unassignedSlot = -1;
                    return true;
            }
            if (unassignedSlot > 0)
            {
                return State.IsAssigned(unassignedSlot);
            }
            return true;
        }

        private Symbol UseNonFieldSymbolUnsafely(BoundExpression expression)
        {
            while (expression != null)
            {
                BoundFieldAccess boundFieldAccess;
                switch (expression.Kind)
                {
                    case BoundKind.FieldAccess:
                        {
                            boundFieldAccess = (BoundFieldAccess)expression;
                            FieldSymbol fieldSymbol = boundFieldAccess.FieldSymbol;
                            if ((object)_sourceAssembly != null)
                            {
                                _sourceAssembly.NoteFieldAccess(fieldSymbol, read: true, write: true);
                            }
                            if (fieldSymbol.ContainingType.IsReferenceType || fieldSymbol.IsStatic)
                            {
                                return null;
                            }
                            break;
                        }
                    case BoundKind.Local:
                        {
                            LocalSymbol localSymbol = ((BoundLocal)expression).LocalSymbol;
                            _usedVariables.Add(localSymbol);
                            return localSymbol;
                        }
                    case BoundKind.RangeVariable:
                        return ((BoundRangeVariable)expression).RangeVariableSymbol;
                    case BoundKind.Parameter:
                        return ((BoundParameter)expression).ParameterSymbol;
                    case BoundKind.ThisReference:
                        return base.MethodThisParameter;
                    case BoundKind.BaseReference:
                        return base.MethodThisParameter;
                    default:
                        return null;
                }
                expression = boundFieldAccess.ReceiverOpt;
            }
            return null;
        }

        protected void Assign(BoundNode node, BoundExpression value, bool isRef = false, bool read = true)
        {
            AssignImpl(node, value, isRef, written: true, read);
        }

        protected virtual void AssignImpl(BoundNode node, BoundExpression value, bool isRef, bool written, bool read)
        {
            switch (node.Kind)
            {
                case BoundKind.DeclarationPattern:
                    {
                        BoundDeclarationPattern boundDeclarationPattern = (BoundDeclarationPattern)node;
                        if (boundDeclarationPattern.Variable is LocalSymbol symbol)
                        {
                            int orCreateSlot2 = GetOrCreateSlot(symbol);
                            SetSlotState(orCreateSlot2, written || !State.Reachable);
                        }
                        if (written)
                        {
                            NoteWrite(boundDeclarationPattern.VariableAccess, value, read);
                        }
                        break;
                    }
                case BoundKind.RecursivePattern:
                    {
                        BoundRecursivePattern boundRecursivePattern = (BoundRecursivePattern)node;
                        if (boundRecursivePattern.Variable is LocalSymbol symbol2)
                        {
                            int orCreateSlot3 = GetOrCreateSlot(symbol2);
                            SetSlotState(orCreateSlot3, written || !State.Reachable);
                        }
                        if (written)
                        {
                            NoteWrite(boundRecursivePattern.VariableAccess, value, read);
                        }
                        break;
                    }
                case BoundKind.LocalDeclaration:
                    {
                        LocalSymbol localSymbol = ((BoundLocalDeclaration)node).LocalSymbol;
                        int orCreateSlot = GetOrCreateSlot(localSymbol);
                        SetSlotState(orCreateSlot, written || !State.Reachable);
                        if (written)
                        {
                            NoteWrite(localSymbol, value, read);
                        }
                        break;
                    }
                case BoundKind.Local:
                    {
                        BoundLocal boundLocal = (BoundLocal)node;
                        if (boundLocal.LocalSymbol.RefKind != 0 && !isRef)
                        {
                            if (written)
                            {
                                VisitRvalue(boundLocal, isKnownToBeAnLvalue: true);
                            }
                            break;
                        }
                        int slot = MakeSlot(boundLocal);
                        SetSlotState(slot, written);
                        if (written)
                        {
                            NoteWrite(boundLocal, value, read);
                        }
                        break;
                    }
                case BoundKind.Parameter:
                    {
                        BoundParameter boundParameter = (BoundParameter)node;
                        ParameterSymbol parameterSymbol = boundParameter.ParameterSymbol;
                        if (isRef && parameterSymbol.RefKind == RefKind.Out)
                        {
                            LeaveParameter(parameterSymbol, node.Syntax, boundParameter.Syntax.Location);
                        }
                        int slot3 = MakeSlot(boundParameter);
                        SetSlotState(slot3, written);
                        if (written)
                        {
                            NoteWrite(boundParameter, value, read);
                        }
                        break;
                    }
                case BoundKind.ThisReference:
                case BoundKind.FieldAccess:
                case BoundKind.PropertyAccess:
                case BoundKind.EventAccess:
                    {
                        BoundExpression boundExpression = (BoundExpression)node;
                        int slot2 = MakeSlot(boundExpression);
                        SetSlotState(slot2, written);
                        if (written)
                        {
                            NoteWrite(boundExpression, value, read);
                        }
                        break;
                    }
                case BoundKind.RangeVariable:
                    AssignImpl(((BoundRangeVariable)node).Value, value, isRef, written, read);
                    break;
                case BoundKind.BadExpression:
                    {
                        BoundBadExpression boundBadExpression = (BoundBadExpression)node;
                        if (!boundBadExpression.ChildBoundNodes.IsDefault && boundBadExpression.ChildBoundNodes.Length == 1)
                        {
                            AssignImpl(boundBadExpression.ChildBoundNodes[0], value, isRef, written, read);
                        }
                        break;
                    }
                case BoundKind.TupleLiteral:
                case BoundKind.ConvertedTupleLiteral:
                    ((BoundTupleExpression)node).VisitAllElements(delegate (BoundExpression x, DefiniteAssignmentPass self)
                    {
                        self.Assign(x, null, isRef);
                    }, this);
                    break;
            }
        }

        private bool FieldsAllSet(int containingSlot, LocalState state)
        {
            TypeSymbol type = variableBySlot[containingSlot].Symbol.GetTypeOrReturnType().Type;
            foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(type))
            {
                if (!_emptyStructTypeCache.IsEmptyStructType(structInstanceField.Type) && !(structInstanceField is TupleErrorFieldSymbol))
                {
                    int num = VariableSlot(structInstanceField, containingSlot);
                    if (num == -1 || !state.IsAssigned(num))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected void SetSlotState(int slot, bool assigned)
        {
            if (slot > 0)
            {
                if (assigned)
                {
                    SetSlotAssigned(slot);
                }
                else
                {
                    SetSlotUnassigned(slot);
                }
            }
        }

        protected void SetSlotAssigned(int slot, ref LocalState state)
        {
            if (slot < 0)
            {
                return;
            }
            VariableIdentifier variableIdentifier = variableBySlot[slot];
            TypeSymbol type = variableIdentifier.Symbol.GetTypeOrReturnType().Type;
            if (slot >= state.Assigned.Capacity)
            {
                Normalize(ref state);
            }
            if (state.IsAssigned(slot))
            {
                return;
            }
            state.Assign(slot);
            if (EmptyStructTypeCache.IsTrackableStructType(type))
            {
                foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(type))
                {
                    int num = VariableSlot(structInstanceField, slot);
                    if (num > 0)
                    {
                        SetSlotAssigned(num, ref state);
                    }
                }
            }
            while (variableIdentifier.ContainingSlot > 0)
            {
                slot = variableIdentifier.ContainingSlot;
                if (!state.IsAssigned(slot) && FieldsAllSet(slot, state))
                {
                    state.Assign(slot);
                    variableIdentifier = variableBySlot[slot];
                    continue;
                }
                break;
            }
        }

        private void SetSlotAssigned(int slot)
        {
            SetSlotAssigned(slot, ref State);
        }

        private void SetSlotUnassigned(int slot, ref LocalState state)
        {
            if (slot < 0)
            {
                return;
            }
            VariableIdentifier variableIdentifier = variableBySlot[slot];
            TypeSymbol type = variableIdentifier.Symbol.GetTypeOrReturnType().Type;
            if (!state.IsAssigned(slot))
            {
                return;
            }
            state.Unassign(slot);
            if (EmptyStructTypeCache.IsTrackableStructType(type))
            {
                foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(type))
                {
                    int num = VariableSlot(structInstanceField, slot);
                    if (num > 0)
                    {
                        SetSlotUnassigned(num, ref state);
                    }
                }
            }
            while (variableIdentifier.ContainingSlot > 0)
            {
                slot = variableIdentifier.ContainingSlot;
                state.Unassign(slot);
                variableIdentifier = variableBySlot[slot];
            }
        }

        private void SetSlotUnassigned(int slot)
        {
            if (NonMonotonicState.HasValue)
            {
                LocalState state = NonMonotonicState.Value;
                SetSlotUnassigned(slot, ref state);
                NonMonotonicState = state;
            }
            SetSlotUnassigned(slot, ref State);
        }

        protected override LocalState TopState()
        {
            return new LocalState(BitVector.Empty);
        }

        protected override LocalState ReachableBottomState()
        {
            LocalState result = new LocalState(BitVector.AllSet(variableBySlot.Count));
            result.Assigned[0] = false;
            return result;
        }

        protected override void EnterParameter(ParameterSymbol parameter)
        {
            int orCreateSlot = GetOrCreateSlot(parameter);
            if (parameter.RefKind == RefKind.Out && (!(CurrentSymbol is MethodSymbol methodSymbol) || !methodSymbol.IsAsync))
            {
                if (orCreateSlot > 0)
                {
                    SetSlotState(orCreateSlot, initiallyAssignedVariables?.Contains(parameter) ?? false);
                }
                return;
            }
            if (orCreateSlot > 0)
            {
                SetSlotState(orCreateSlot, assigned: true);
            }
            NoteWrite(parameter, null, read: true);
        }

        protected override void LeaveParameters(ImmutableArray<ParameterSymbol> parameters, SyntaxNode syntax, Location location)
        {
            if (State.Reachable)
            {
                base.LeaveParameters(parameters, syntax, location);
            }
        }

        protected override void LeaveParameter(ParameterSymbol parameter, SyntaxNode syntax, Location location)
        {
            if (parameter.RefKind != 0)
            {
                int num = VariableSlot(parameter);
                if (num > 0 && !State.IsAssigned(num))
                {
                    ReportUnassignedOutParameter(parameter, syntax, location);
                }
                NoteRead(parameter);
            }
        }

        protected override LocalState UnreachableState()
        {
            LocalState result = State.Clone();
            result.Assigned.EnsureCapacity(1);
            result.Assign(0);
            return result;
        }

        public override void VisitPattern(BoundPattern pattern)
        {
            base.VisitPattern(pattern);
            LocalState stateWhenFalse = StateWhenFalse;
            SetState(StateWhenTrue);
            AssignPatternVariables(pattern);
            SetConditionalState(State, stateWhenFalse);
        }

        private void AssignPatternVariables(BoundPattern pattern, bool definitely = true)
        {
            switch (pattern.Kind)
            {
                case BoundKind.DeclarationPattern:
                    {
                        BoundDeclarationPattern node = (BoundDeclarationPattern)pattern;
                        if (definitely)
                        {
                            Assign(node, null, isRef: false, read: false);
                        }
                        break;
                    }
                case BoundKind.ConstantPattern:
                    {
                        BoundConstantPattern boundConstantPattern = (BoundConstantPattern)pattern;
                        VisitRvalue(boundConstantPattern.Value);
                        break;
                    }
                case BoundKind.RecursivePattern:
                    {
                        BoundRecursivePattern boundRecursivePattern = (BoundRecursivePattern)pattern;
                        if (!boundRecursivePattern.Deconstruction.IsDefaultOrEmpty)
                        {
                            ImmutableArray<BoundSubpattern>.Enumerator enumerator = boundRecursivePattern.Deconstruction.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                BoundSubpattern current = enumerator.Current;
                                AssignPatternVariables(current.Pattern, definitely);
                            }
                        }
                        if (!boundRecursivePattern.Properties.IsDefaultOrEmpty)
                        {
                            ImmutableArray<BoundSubpattern>.Enumerator enumerator = boundRecursivePattern.Properties.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                BoundSubpattern current2 = enumerator.Current;
                                AssignPatternVariables(current2.Pattern, definitely);
                            }
                        }
                        if (definitely)
                        {
                            Assign(boundRecursivePattern, null, isRef: false, read: false);
                        }
                        break;
                    }
                case BoundKind.ITuplePattern:
                    {
                        ImmutableArray<BoundSubpattern>.Enumerator enumerator = ((BoundITuplePattern)pattern).Subpatterns.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            BoundSubpattern current3 = enumerator.Current;
                            AssignPatternVariables(current3.Pattern, definitely);
                        }
                        break;
                    }
                case BoundKind.RelationalPattern:
                    {
                        BoundRelationalPattern boundRelationalPattern = (BoundRelationalPattern)pattern;
                        VisitRvalue(boundRelationalPattern.Value);
                        break;
                    }
                case BoundKind.NegatedPattern:
                    {
                        BoundNegatedPattern boundNegatedPattern = (BoundNegatedPattern)pattern;
                        AssignPatternVariables(boundNegatedPattern.Negated, definitely: false);
                        break;
                    }
                case BoundKind.BinaryPattern:
                    {
                        BoundBinaryPattern boundBinaryPattern = (BoundBinaryPattern)pattern;
                        bool definitely2 = definitely && !boundBinaryPattern.Disjunction;
                        AssignPatternVariables(boundBinaryPattern.Left, definitely2);
                        AssignPatternVariables(boundBinaryPattern.Right, definitely2);
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(pattern.Kind);
                case BoundKind.DiscardPattern:
                case BoundKind.TypePattern:
                    break;
            }
        }

        public override BoundNode VisitBlock(BoundBlock node)
        {
            DeclareVariables(node.Locals);
            VisitStatementsWithLocalFunctions(node);
            ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                if (current.IsUsing)
                {
                    NoteRead(current);
                }
            }
            ReportUnusedVariables(node.Locals);
            ReportUnusedVariables(node.LocalFunctions);
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
                    if (current.Kind == BoundKind.LocalFunctionStatement)
                    {
                        VisitAlways(current);
                    }
                }
                enumerator = block.Statements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundStatement current2 = enumerator.Current;
                    if (current2.Kind != BoundKind.LocalFunctionStatement)
                    {
                        VisitStatement(current2);
                    }
                }
            }
            else
            {
                ImmutableArray<BoundStatement>.Enumerator enumerator = block.Statements.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    BoundStatement current3 = enumerator.Current;
                    VisitStatement(current3);
                }
            }
        }

        public override BoundNode VisitSwitchStatement(BoundSwitchStatement node)
        {
            DeclareVariables(node.InnerLocals);
            BoundNode result = base.VisitSwitchStatement(node);
            ReportUnusedVariables(node.InnerLocals);
            ReportUnusedVariables(node.InnerLocalFunctions);
            return result;
        }

        protected override void VisitSwitchSection(BoundSwitchSection node, bool isLastSection)
        {
            DeclareVariables(node.Locals);
            base.VisitSwitchSection(node, isLastSection);
        }

        public override BoundNode VisitForStatement(BoundForStatement node)
        {
            DeclareVariables(node.OuterLocals);
            DeclareVariables(node.InnerLocals);
            BoundNode result = base.VisitForStatement(node);
            ReportUnusedVariables(node.InnerLocals);
            ReportUnusedVariables(node.OuterLocals);
            return result;
        }

        public override BoundNode VisitDoStatement(BoundDoStatement node)
        {
            DeclareVariables(node.Locals);
            BoundNode result = base.VisitDoStatement(node);
            ReportUnusedVariables(node.Locals);
            return result;
        }

        public override BoundNode VisitWhileStatement(BoundWhileStatement node)
        {
            DeclareVariables(node.Locals);
            BoundNode result = base.VisitWhileStatement(node);
            ReportUnusedVariables(node.Locals);
            return result;
        }

        public override BoundNode VisitUsingStatement(BoundUsingStatement node)
        {
            ImmutableArray<LocalSymbol> locals = node.Locals;
            DeclareVariables(locals);
            BoundNode result = base.VisitUsingStatement(node);
            if (!locals.IsDefaultOrEmpty)
            {
                ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalSymbol current = enumerator.Current;
                    if (current.DeclarationKind == LocalDeclarationKind.UsingVariable)
                    {
                        NoteRead(current);
                    }
                }
            }
            return result;
        }

        public override BoundNode VisitFixedStatement(BoundFixedStatement node)
        {
            DeclareVariables(node.Locals);
            return base.VisitFixedStatement(node);
        }

        public override BoundNode VisitSequence(BoundSequence node)
        {
            DeclareVariables(node.Locals);
            BoundNode result = base.VisitSequence(node);
            ReportUnusedVariables(node.Locals);
            return result;
        }

        private void DeclareVariables(ImmutableArray<LocalSymbol> locals)
        {
            ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                DeclareVariable(current);
            }
        }

        private void DeclareVariable(LocalSymbol symbol)
        {
            bool assigned = symbol.IsConst || (initiallyAssignedVariables?.Contains(symbol) ?? false);
            SetSlotState(GetOrCreateSlot(symbol), assigned);
        }

        private void ReportUnusedVariables(ImmutableArray<LocalSymbol> locals)
        {
            ImmutableArray<LocalSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                ReportIfUnused(current, assigned: true);
            }
        }

        private void ReportIfUnused(LocalSymbol symbol, bool assigned)
        {
            if (!_usedVariables.Contains(symbol) && symbol.DeclarationKind != LocalDeclarationKind.PatternVariable && !string.IsNullOrEmpty(symbol.Name))
            {
                base.Diagnostics.Add((assigned && _writtenVariables.Contains(symbol)) ? ErrorCode.WRN_UnreferencedVarAssg : ErrorCode.WRN_UnreferencedVar, symbol.Locations.FirstOrNone(), symbol.Name);
            }
        }

        private void ReportUnusedVariables(ImmutableArray<LocalFunctionSymbol> locals)
        {
            ImmutableArray<LocalFunctionSymbol>.Enumerator enumerator = locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalFunctionSymbol current = enumerator.Current;
                ReportIfUnused(current);
            }
        }

        private void ReportIfUnused(LocalFunctionSymbol symbol)
        {
            if (!_usedLocalFunctions.Contains(symbol) && !string.IsNullOrEmpty(symbol.Name))
            {
                base.Diagnostics.Add(ErrorCode.WRN_UnreferencedLocalFunction, symbol.Locations.FirstOrNone(), symbol.Name);
            }
        }

        public override BoundNode VisitLocal(BoundLocal node)
        {
            LocalSymbol localSymbol = node.LocalSymbol;
            SourceLocalSymbol obj = localSymbol as SourceLocalSymbol;
            if ((object)obj != null && obj.IsVar)
            {
                SyntaxNode forbiddenZone = localSymbol.ForbiddenZone;
                if (forbiddenZone != null && forbiddenZone.Contains(node.Syntax))
                {
                    int orCreateSlot = GetOrCreateSlot(node.LocalSymbol);
                    if (orCreateSlot > 0)
                    {
                        _alreadyReported[orCreateSlot] = true;
                    }
                }
            }
            CheckAssigned(localSymbol, node.Syntax);
            if (localSymbol.IsFixed && CurrentSymbol is MethodSymbol methodSymbol && (methodSymbol.MethodKind == MethodKind.AnonymousFunction || methodSymbol.MethodKind == MethodKind.LocalFunction) && _capturedVariables.Contains(localSymbol))
            {
                base.Diagnostics.Add(ErrorCode.ERR_FixedLocalInLambda, new SourceLocation(node.Syntax), localSymbol);
            }
            return null;
        }

        public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            GetOrCreateSlot(node.LocalSymbol);
            HashSet<Symbol> hashSet = initiallyAssignedVariables;
            if (hashSet != null && hashSet.Contains(node.LocalSymbol))
            {
                Assign(node, null);
            }
            BoundNode result = base.VisitLocalDeclaration(node);
            if (node.InitializerOpt != null)
            {
                Assign(node, node.InitializerOpt);
            }
            return result;
        }

        public override BoundNode VisitMethodGroup(BoundMethodGroup node)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = node.Methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                if (current.MethodKind == MethodKind.LocalFunction)
                {
                    _usedLocalFunctions.Add((LocalFunctionSymbol)current);
                }
            }
            return base.VisitMethodGroup(node);
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            Symbol currentSymbol = CurrentSymbol;
            CurrentSymbol = node.Symbol;
            SavedPending oldPending = SavePending();
            LocalState self = State;
            State = (State.Reachable ? State.Clone() : ReachableBottomState());
            if (!node.WasCompilerGenerated)
            {
                EnterParameters(node.Symbol.Parameters);
            }
            SavedPending oldPending2 = SavePending();
            VisitAlways(node.Body);
            RestorePending(oldPending2);
            ImmutableArray<PendingBranch> immutableArray = RemoveReturns();
            RestorePending(oldPending);
            LeaveParameters(node.Symbol.Parameters, node.Syntax, null);
            Join(ref self, ref State);
            ImmutableArray<PendingBranch>.Enumerator enumerator = immutableArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PendingBranch current = enumerator.Current;
                State = current.State;
                if (current.Branch.Kind == BoundKind.ReturnStatement)
                {
                    LeaveParameters(node.Symbol.Parameters, current.Branch.Syntax, null);
                }
                Join(ref self, ref State);
            }
            State = self;
            CurrentSymbol = currentSymbol;
            return null;
        }

        public override BoundNode VisitThisReference(BoundThisReference node)
        {
            CheckAssigned(base.MethodThisParameter, node.Syntax);
            return null;
        }

        public override BoundNode VisitParameter(BoundParameter node)
        {
            if (!node.WasCompilerGenerated)
            {
                CheckAssigned(node.ParameterSymbol, node.Syntax);
            }
            else
            {
                NoteRecordParameterReadIfNeeded(node.ParameterSymbol);
            }
            return null;
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            base.VisitAssignmentOperator(node);
            Assign(node.Left, node.Right, node.IsRef);
            return null;
        }

        public override BoundNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            base.VisitDeconstructionAssignmentOperator(node);
            Assign(node.Left, node.Right);
            return null;
        }

        public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
        {
            base.VisitIncrementOperator(node);
            Assign(node.Operand, node);
            return null;
        }

        public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            VisitCompoundAssignmentTarget(node);
            VisitRvalue(node.Right);
            AfterRightHasBeenVisited(node);
            Assign(node.Left, node);
            return null;
        }

        public override BoundNode VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            BoundExpression boundExpression = node.Expression;
            if (boundExpression.Kind == BoundKind.AddressOfOperator)
            {
                boundExpression = ((BoundAddressOfOperator)boundExpression).Operand;
            }
            VisitAddressOfOperand(boundExpression, shouldReadOperand: false);
            return null;
        }

        public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            BoundExpression operand = node.Operand;
            bool shouldReadOperand = false;
            Symbol symbol = UseNonFieldSymbolUnsafely(operand);
            if ((object)symbol != null)
            {
                HashSet<PrefixUnaryExpressionSyntax> unassignedVariableAddressOfSyntaxes = _unassignedVariableAddressOfSyntaxes;
                if (unassignedVariableAddressOfSyntaxes != null && !unassignedVariableAddressOfSyntaxes.Contains(node.Syntax as PrefixUnaryExpressionSyntax))
                {
                    shouldReadOperand = true;
                }
                if (!_unsafeAddressTakenVariables.ContainsKey(symbol))
                {
                    _unsafeAddressTakenVariables.Add(symbol, node.Syntax.Location);
                }
            }
            VisitAddressOfOperand(node.Operand, shouldReadOperand);
            return null;
        }

        protected override void WriteArgument(BoundExpression arg, RefKind refKind, MethodSymbol method)
        {
            if (refKind == RefKind.Ref)
            {
                CheckAssigned(arg, arg.Syntax);
            }
            Assign(arg, null);
            if (refKind != 0 && ((object)method == null || method.IsExtern))
            {
                TypeSymbol type = arg.Type;
                if ((object)type != null)
                {
                    MarkFieldsUsed(type);
                }
            }
        }

        protected void CheckAssigned(BoundExpression expr, SyntaxNode node)
        {
            if (!State.Reachable)
            {
                return;
            }
            MakeSlot(expr);
            switch (expr.Kind)
            {
                case BoundKind.Local:
                    CheckAssigned(((BoundLocal)expr).LocalSymbol, node);
                    break;
                case BoundKind.Parameter:
                    CheckAssigned(((BoundParameter)expr).ParameterSymbol, node);
                    break;
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr;
                        FieldSymbol fieldSymbol = boundFieldAccess.FieldSymbol;
                        if (!fieldSymbol.IsFixedSizeBuffer && MayRequireTracking(boundFieldAccess.ReceiverOpt, fieldSymbol))
                        {
                            CheckAssigned(expr, fieldSymbol, node);
                        }
                        break;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)expr;
                        FieldSymbol associatedField = boundEventAccess.EventSymbol.AssociatedField;
                        if ((object)associatedField != null && MayRequireTracking(boundEventAccess.ReceiverOpt, associatedField))
                        {
                            CheckAssigned(boundEventAccess, associatedField, node);
                        }
                        break;
                    }
                case BoundKind.ThisReference:
                case BoundKind.BaseReference:
                    CheckAssigned(base.MethodThisParameter, node);
                    break;
            }
        }

        private void MarkFieldsUsed(TypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Array:
                    MarkFieldsUsed(((ArrayTypeSymbol)type).ElementType);
                    break;
                case TypeKind.Class:
                case TypeKind.Struct:
                    {
                        if (!type.IsFromCompilation(compilation) || !(type.ContainingAssembly is SourceAssemblySymbol sourceAssemblySymbol) || !sourceAssemblySymbol.TypesReferencedInExternalMethods.Add(type))
                        {
                            break;
                        }
                        ImmutableArray<Symbol>.Enumerator enumerator = ((NamedTypeSymbol)type).GetMembersUnordered().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Symbol current = enumerator.Current;
                            if (current.Kind == SymbolKind.Field)
                            {
                                FieldSymbol fieldSymbol = (FieldSymbol)current;
                                sourceAssemblySymbol.NoteFieldAccess(fieldSymbol, read: true, write: true);
                                MarkFieldsUsed(fieldSymbol.Type);
                            }
                        }
                        break;
                    }
            }
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            CheckAssigned(base.MethodThisParameter, node.Syntax);
            return null;
        }

        protected override void VisitCatchBlock(BoundCatchBlock catchBlock, ref LocalState finallyState)
        {
            DeclareVariables(catchBlock.Locals);
            BoundExpression exceptionSourceOpt = catchBlock.ExceptionSourceOpt;
            if (exceptionSourceOpt != null)
            {
                Assign(exceptionSourceOpt, null, isRef: false, read: false);
            }
            base.VisitCatchBlock(catchBlock, ref finallyState);
            ImmutableArray<LocalSymbol>.Enumerator enumerator = catchBlock.Locals.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                ReportIfUnused(current, current.DeclarationKind != LocalDeclarationKind.CatchVariable);
            }
        }

        public override BoundNode VisitFieldAccess(BoundFieldAccess node)
        {
            BoundNode result = base.VisitFieldAccess(node);
            NoteRead(node.FieldSymbol);
            if (node.FieldSymbol.IsFixedSizeBuffer && node.Syntax != null && !SyntaxFacts.IsFixedStatementExpression(node.Syntax))
            {
                Symbol symbol = UseNonFieldSymbolUnsafely(node.ReceiverOpt);
                if ((object)symbol != null)
                {
                    CheckCaptured(symbol);
                    if (!_unsafeAddressTakenVariables.ContainsKey(symbol))
                    {
                        _unsafeAddressTakenVariables.Add(symbol, node.Syntax.Location);
                        return result;
                    }
                }
            }
            else if (MayRequireTracking(node.ReceiverOpt, node.FieldSymbol))
            {
                CheckAssigned(node, node.FieldSymbol, node.Syntax);
            }
            return result;
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            BoundNode result = base.VisitPropertyAccess(node);
            if (Binder.AccessingAutoPropertyFromConstructor(node, CurrentSymbol))
            {
                SynthesizedBackingFieldSymbol synthesizedBackingFieldSymbol = (node.PropertySymbol as SourcePropertySymbolBase)?.BackingField;
                if (synthesizedBackingFieldSymbol != null && MayRequireTracking(node.ReceiverOpt, synthesizedBackingFieldSymbol) && State.Reachable && !IsAssigned(node, out var unassignedSlot))
                {
                    ReportUnassignedIfNotCapturedInLocalFunction(synthesizedBackingFieldSymbol, node.Syntax, unassignedSlot);
                }
            }
            return result;
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            BoundNode result = base.VisitEventAccess(node);
            FieldSymbol associatedField = node.EventSymbol.AssociatedField;
            if ((object)associatedField != null)
            {
                NoteRead(associatedField);
                if (MayRequireTracking(node.ReceiverOpt, associatedField))
                {
                    CheckAssigned(node, associatedField, node.Syntax);
                }
            }
            return result;
        }

        public override void VisitForEachIterationVariables(BoundForEachStatement node)
        {
            ImmutableArray<LocalSymbol>.Enumerator enumerator = node.IterationVariables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LocalSymbol current = enumerator.Current;
                int orCreateSlot = GetOrCreateSlot(current);
                if (orCreateSlot > 0)
                {
                    SetSlotAssigned(orCreateSlot);
                }
                NoteWrite(current, null, read: true);
            }
        }

        public override BoundNode VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            BoundNode result = base.VisitObjectInitializerMember(node);
            if ((object)_sourceAssembly != null && node.MemberSymbol != null && node.MemberSymbol!.Kind == SymbolKind.Field)
            {
                _sourceAssembly.NoteFieldAccess((FieldSymbol)node.MemberSymbol!.OriginalDefinition, read: false, write: true);
            }
            return result;
        }

        public override BoundNode VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            return null;
        }

        protected override void VisitAssignmentOfNullCoalescingAssignment(BoundNullCoalescingAssignmentOperator node, BoundPropertyAccess propertyAccessOpt)
        {
            base.VisitAssignmentOfNullCoalescingAssignment(node, propertyAccessOpt);
            Assign(node.LeftOperand, node.RightOperand);
        }

        protected override void AdjustStateForNullCoalescingAssignmentNonNullCase(BoundNullCoalescingAssignmentOperator node)
        {
            Assign(node.LeftOperand, node.LeftOperand);
        }

        protected override string Dump(LocalState state)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[assigned ");
            AppendBitNames(state.Assigned, stringBuilder);
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        protected void AppendBitNames(BitVector a, StringBuilder builder)
        {
            bool flag = false;
            foreach (int item in a.TrueBits())
            {
                if (flag)
                {
                    builder.Append(", ");
                }
                flag = true;
                AppendBitName(item, builder);
            }
        }

        protected void AppendBitName(int bit, StringBuilder builder)
        {
            VariableIdentifier variableIdentifier = variableBySlot[bit];
            if (variableIdentifier.ContainingSlot > 0)
            {
                AppendBitName(variableIdentifier.ContainingSlot, builder);
                builder.Append(".");
            }
            builder.Append((bit == 0) ? "<unreachable>" : (string.IsNullOrEmpty(variableIdentifier.Symbol.Name) ? ("<anon>" + variableIdentifier.Symbol.GetHashCode()) : variableIdentifier.Symbol.Name));
        }

        protected override bool Meet(ref LocalState self, ref LocalState other)
        {
            if (self.Assigned.Capacity != other.Assigned.Capacity)
            {
                Normalize(ref self);
                Normalize(ref other);
            }
            if (!other.Reachable)
            {
                self.Assigned[0] = true;
                return true;
            }
            bool result = false;
            for (int i = 1; i < self.Assigned.Capacity; i++)
            {
                if (other.Assigned[i] && !self.Assigned[i])
                {
                    SetSlotAssigned(i, ref self);
                    result = true;
                }
            }
            return result;
        }

        protected override bool Join(ref LocalState self, ref LocalState other)
        {
            if (self.Reachable == other.Reachable)
            {
                if (self.Assigned.Capacity != other.Assigned.Capacity)
                {
                    Normalize(ref self);
                    Normalize(ref other);
                }
                return self.Assigned.IntersectWith(in other.Assigned);
            }
            if (!self.Reachable)
            {
                self.Assigned = other.Assigned.Clone();
                return true;
            }
            return false;
        }

        protected override LocalFunctionState CreateLocalFunctionState(LocalFunctionSymbol symbol)
        {
            return CreateLocalFunctionState();
        }

        private LocalFunctionState CreateLocalFunctionState()
        {
            return new LocalFunctionState(new LocalState(BitVector.AllSet(variableBySlot.Count), normalizeToBottom: true), UnreachableState());
        }

        protected override void VisitLocalFunctionUse(LocalFunctionSymbol localFunc, LocalFunctionState localFunctionState, SyntaxNode syntax, bool isCall)
        {
            _usedLocalFunctions.Add(localFunc);
            BitVector readVars = localFunctionState.ReadVars;
            for (int i = 1; i < readVars.Capacity; i++)
            {
                if (readVars[i])
                {
                    Symbol symbol = variableBySlot[i].Symbol;
                    CheckIfAssignedDuringLocalFunctionReplay(symbol, syntax, i);
                }
            }
            base.VisitLocalFunctionUse(localFunc, localFunctionState, syntax, isCall);
        }

        private void CheckIfAssignedDuringLocalFunctionReplay(Symbol symbol, SyntaxNode node, int slot)
        {
            if ((object)symbol == null)
            {
                return;
            }
            NoteRead(symbol);
            if (State.Reachable)
            {
                if (slot >= State.Assigned.Capacity)
                {
                    Normalize(ref State);
                }
                if (slot > 0 && !State.IsAssigned(slot))
                {
                    ReportUnassignedIfNotCapturedInLocalFunction(symbol, node, slot, skipIfUseBeforeDeclaration: false);
                }
            }
        }

        private void RecordReadInLocalFunction(int slot)
        {
            LocalFunctionSymbol nearestLocalFunctionOpt = GetNearestLocalFunctionOpt(CurrentSymbol);
            LocalFunctionState orCreateLocalFuncUsages = GetOrCreateLocalFuncUsages(nearestLocalFunctionOpt);
            TypeSymbol type = variableBySlot[slot].Symbol.GetTypeOrReturnType().Type;
            if (EmptyStructTypeCache.IsTrackableStructType(type))
            {
                foreach (FieldSymbol structInstanceField in _emptyStructTypeCache.GetStructInstanceFields(type))
                {
                    int orCreateSlot = GetOrCreateSlot(structInstanceField, slot);
                    if (orCreateSlot > 0 && !State.IsAssigned(orCreateSlot))
                    {
                        RecordReadInLocalFunction(orCreateSlot);
                    }
                }
                return;
            }
            orCreateLocalFuncUsages.ReadVars[slot] = true;
        }

        private BitVector GetCapturedBitmask()
        {
            int count = variableBySlot.Count;
            BitVector result = BitVector.AllSet(count);
            for (int i = 1; i < count; i++)
            {
                result[i] = IsCapturedInLocalFunction(i);
            }
            return result;
        }

        private bool IsCapturedInLocalFunction(int slot)
        {
            if (slot <= 0)
            {
                return false;
            }
            Symbol symbol = variableBySlot[RootSlot(slot)].Symbol;
            LocalFunctionSymbol nearestLocalFunctionOpt = GetNearestLocalFunctionOpt(CurrentSymbol);
            if ((object)nearestLocalFunctionOpt != null)
            {
                return Symbol.IsCaptured(symbol, nearestLocalFunctionOpt);
            }
            return false;
        }

        private static LocalFunctionSymbol GetNearestLocalFunctionOpt(Symbol symbol)
        {
            while (symbol != null)
            {
                if (symbol.Kind == SymbolKind.Method && ((MethodSymbol)symbol).MethodKind == MethodKind.LocalFunction)
                {
                    return (LocalFunctionSymbol)symbol;
                }
                symbol = symbol.ContainingSymbol;
            }
            return null;
        }

        protected override LocalFunctionState LocalFunctionStart(LocalFunctionState startState)
        {
            LocalFunctionState localFunctionState = CreateLocalFunctionState();
            localFunctionState.ReadVars = startState.ReadVars.Clone();
            startState.ReadVars.Clear();
            return localFunctionState;
        }

        protected override bool LocalFunctionEnd(LocalFunctionState savedState, LocalFunctionState currentState, ref LocalState stateAtReturn)
        {
            if (currentState.CapturedMask.IsNull)
            {
                currentState.CapturedMask = GetCapturedBitmask();
                currentState.InvertedCapturedMask = currentState.CapturedMask.Clone();
                currentState.InvertedCapturedMask.Invert();
            }
            stateAtReturn.Assigned.IntersectWith(in currentState.CapturedMask);
            if (NonMonotonicState.HasValue)
            {
                LocalState value = NonMonotonicState.Value;
                value.Assigned.UnionWith(in currentState.InvertedCapturedMask);
                NonMonotonicState = value;
            }
            BitVector other = currentState.ReadVars;
            other.IntersectWith(in currentState.CapturedMask);
            return savedState.ReadVars.UnionWith(in other);
        }
    }
}
