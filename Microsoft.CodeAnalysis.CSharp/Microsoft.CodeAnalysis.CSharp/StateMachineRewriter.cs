using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal abstract class StateMachineRewriter
    {
        protected readonly BoundStatement body;

        protected readonly MethodSymbol method;

        protected readonly BindingDiagnosticBag diagnostics;

        protected readonly SyntheticBoundNodeFactory F;

        protected readonly SynthesizedContainer stateMachineType;

        protected readonly VariableSlotAllocator slotAllocatorOpt;

        protected readonly SynthesizedLocalOrdinalsDispenser synthesizedLocalOrdinals;

        protected FieldSymbol stateField;

        protected IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> nonReusableLocalProxies;

        protected int nextFreeHoistedLocalSlot;

        protected IOrderedReadOnlySet<Symbol> hoistedVariables;

        protected Dictionary<Symbol, CapturedSymbolReplacement> initialParameters;

        protected FieldSymbol initialThreadIdField;

        protected abstract bool PreserveInitialParameterValuesAndThreadId { get; }

        protected StateMachineRewriter(BoundStatement body, MethodSymbol method, SynthesizedContainer stateMachineType, VariableSlotAllocator slotAllocatorOpt, TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            this.body = body;
            this.method = method;
            this.stateMachineType = stateMachineType;
            this.slotAllocatorOpt = slotAllocatorOpt;
            synthesizedLocalOrdinals = new SynthesizedLocalOrdinalsDispenser();
            this.diagnostics = diagnostics;
            F = new SyntheticBoundNodeFactory(method, body.Syntax, compilationState, diagnostics);
        }

        protected abstract void GenerateControlFields();

        protected abstract void InitializeStateMachine(ArrayBuilder<BoundStatement> bodyBuilder, NamedTypeSymbol frameType, LocalSymbol stateMachineLocal);

        protected abstract BoundStatement GenerateStateMachineCreation(LocalSymbol stateMachineVariable, NamedTypeSymbol frameType, IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies);

        protected abstract void GenerateMethodImplementations();

        protected BoundStatement Rewrite()
        {
            if (body.HasErrors)
            {
                return body;
            }
            F.OpenNestedType(stateMachineType);
            GenerateControlFields();
            if (PreserveInitialParameterValuesAndThreadId && CanGetThreadId())
            {
                initialThreadIdField = F.StateMachineField(F.SpecialType(SpecialType.System_Int32), GeneratedNames.MakeIteratorCurrentThreadIdFieldName());
            }
            if (PreserveInitialParameterValuesAndThreadId)
            {
                initialParameters = new Dictionary<Symbol, CapturedSymbolReplacement>();
            }
            OrderedSet<Symbol> variablesToHoist = IteratorAndAsyncCaptureWalker.Analyze(F.Compilation, method, body, diagnostics.DiagnosticBag);
            if (diagnostics.HasAnyErrors())
            {
                return new BoundBadStatement(F.Syntax, ImmutableArray<BoundNode>.Empty, hasErrors: true);
            }
            CreateNonReusableLocalProxies(variablesToHoist, out nonReusableLocalProxies, out nextFreeHoistedLocalSlot);
            hoistedVariables = variablesToHoist;
            GenerateMethodImplementations();
            return GenerateKickoffMethodBody();
        }

        private void CreateNonReusableLocalProxies(IEnumerable<Symbol> variablesToHoist, out IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies, out int nextFreeHoistedLocalSlot)
        {
            Dictionary<Symbol, CapturedSymbolReplacement> dictionary = new Dictionary<Symbol, CapturedSymbolReplacement>();
            TypeMap typeMap = stateMachineType.TypeMap;
            bool flag = F.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug;
            bool flag2 = flag && slotAllocatorOpt != null;
            nextFreeHoistedLocalSlot = (flag2 ? slotAllocatorOpt.PreviousHoistedLocalSlotCount : 0);
            foreach (Symbol item in variablesToHoist)
            {
                if (item.Kind == SymbolKind.Local)
                {
                    LocalSymbol localSymbol = (LocalSymbol)item;
                    SynthesizedLocalKind synthesizedKind = localSymbol.SynthesizedKind;
                    if (!synthesizedKind.MustSurviveStateMachineSuspension() || localSymbol.IsConst || localSymbol.RefKind != 0)
                    {
                        continue;
                    }
                    StateMachineFieldSymbol stateMachineFieldSymbol = null;
                    if (ShouldPreallocateNonReusableProxy(localSymbol))
                    {
                        TypeSymbol type = typeMap.SubstituteType(localSymbol.Type).Type;
                        int num = -1;
                        LocalDebugId localDebugId;
                        if (flag)
                        {
                            SyntaxNode declaratorSyntax = localSymbol.GetDeclaratorSyntax();
                            int syntaxOffset = method.CalculateLocalSyntaxOffset(LambdaUtilities.GetDeclaratorPosition(declaratorSyntax), declaratorSyntax.SyntaxTree);
                            int ordinal = synthesizedLocalOrdinals.AssignLocalOrdinal(synthesizedKind, syntaxOffset);
                            localDebugId = new LocalDebugId(syntaxOffset, ordinal);
                            if (flag2 && slotAllocatorOpt.TryGetPreviousHoistedLocalSlotIndex(declaratorSyntax, F.ModuleBuilderOpt!.Translate(type, declaratorSyntax, diagnostics.DiagnosticBag), synthesizedKind, localDebugId, diagnostics.DiagnosticBag, out var slotIndex))
                            {
                                num = slotIndex;
                            }
                        }
                        else
                        {
                            localDebugId = LocalDebugId.None;
                        }
                        if (num == -1)
                        {
                            num = nextFreeHoistedLocalSlot++;
                        }
                        string name = GeneratedNames.MakeHoistedLocalFieldName(synthesizedKind, num, localSymbol.Name);
                        stateMachineFieldSymbol = F.StateMachineField(type, name, new LocalSlotDebugInfo(synthesizedKind, localDebugId), num);
                    }
                    if (stateMachineFieldSymbol != null)
                    {
                        dictionary.Add(localSymbol, new CapturedToStateMachineFieldReplacement(stateMachineFieldSymbol, isReusable: false));
                    }
                    continue;
                }
                ParameterSymbol parameterSymbol = (ParameterSymbol)item;
                if (parameterSymbol.IsThis)
                {
                    NamedTypeSymbol containingType = method.ContainingType;
                    StateMachineFieldSymbol stateMachineFieldSymbol2 = F.StateMachineField(containingType, GeneratedNames.ThisProxyFieldName(), isPublic: true, isThis: true);
                    dictionary.Add(parameterSymbol, new CapturedToStateMachineFieldReplacement(stateMachineFieldSymbol2, isReusable: false));
                    if (PreserveInitialParameterValuesAndThreadId)
                    {
                        StateMachineFieldSymbol hoistedField = (containingType.IsStructType() ? F.StateMachineField(containingType, GeneratedNames.StateMachineThisParameterProxyName(), isPublic: true, isThis: true) : stateMachineFieldSymbol2);
                        initialParameters.Add(parameterSymbol, new CapturedToStateMachineFieldReplacement(hoistedField, isReusable: false));
                    }
                }
                else
                {
                    StateMachineFieldSymbol hoistedField2 = F.StateMachineField(typeMap.SubstituteType(parameterSymbol.Type).Type, parameterSymbol.Name, !PreserveInitialParameterValuesAndThreadId);
                    dictionary.Add(parameterSymbol, new CapturedToStateMachineFieldReplacement(hoistedField2, isReusable: false));
                    if (PreserveInitialParameterValuesAndThreadId)
                    {
                        StateMachineFieldSymbol hoistedField3 = F.StateMachineField(typeMap.SubstituteType(parameterSymbol.Type).Type, GeneratedNames.StateMachineParameterProxyFieldName(parameterSymbol.Name), isPublic: true);
                        initialParameters.Add(parameterSymbol, new CapturedToStateMachineFieldReplacement(hoistedField3, isReusable: false));
                    }
                }
            }
            proxies = dictionary;
        }

        private bool ShouldPreallocateNonReusableProxy(LocalSymbol local)
        {
            SynthesizedLocalKind synthesizedKind = local.SynthesizedKind;
            OptimizationLevel optimizationLevel = F.Compilation.Options.OptimizationLevel;
            if (optimizationLevel == OptimizationLevel.Release && synthesizedKind == SynthesizedLocalKind.UserDefined)
            {
                return false;
            }
            return !synthesizedKind.IsSlotReusable(optimizationLevel);
        }

        private BoundStatement GenerateKickoffMethodBody()
        {
            F.CurrentFunction = method;
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            NamedTypeSymbol namedTypeSymbol = (method.IsGenericMethod ? stateMachineType.Construct(method.TypeArgumentsWithAnnotations, unbound: false) : stateMachineType);
            LocalSymbol localSymbol = F.SynthesizedLocal(namedTypeSymbol);
            InitializeStateMachine(instance, namedTypeSymbol, localSymbol);
            IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> readOnlyDictionary;
            if (!PreserveInitialParameterValuesAndThreadId)
            {
                readOnlyDictionary = nonReusableLocalProxies;
            }
            else
            {
                IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> readOnlyDictionary2 = initialParameters;
                readOnlyDictionary = readOnlyDictionary2;
            }
            IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies = readOnlyDictionary;
            instance.Add(GenerateStateMachineCreation(localSymbol, namedTypeSymbol, proxies));
            return F.Block(ImmutableArray.Create(localSymbol), instance.ToImmutableAndFree());
        }

        protected BoundStatement GenerateParameterStorage(LocalSymbol stateMachineVariable, IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> proxies)
        {
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            if (!method.IsStatic && proxies.TryGetValue(method.ThisParameter, out var value))
            {
                instance.Add(F.Assignment(value.Replacement(F.Syntax, (NamedTypeSymbol frameType1) => F.Local(stateMachineVariable)), F.This()));
            }
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (proxies.TryGetValue(current, out var value2))
                {
                    instance.Add(F.Assignment(value2.Replacement(F.Syntax, (NamedTypeSymbol frameType1) => F.Local(stateMachineVariable)), F.Parameter(current)));
                }
            }
            return F.Block(instance.ToImmutableAndFree());
        }

        protected SynthesizedImplementationMethod OpenMethodImplementation(MethodSymbol methodToImplement, string methodName = null, bool hasMethodBodyDependency = false)
        {
            SynthesizedStateMachineDebuggerHiddenMethod synthesizedStateMachineDebuggerHiddenMethod = new SynthesizedStateMachineDebuggerHiddenMethod(methodName, methodToImplement, (StateMachineTypeSymbol)F.CurrentType, null, hasMethodBodyDependency);
            F.ModuleBuilderOpt!.AddSynthesizedDefinition(F.CurrentType, synthesizedStateMachineDebuggerHiddenMethod.GetCciAdapter());
            F.CurrentFunction = synthesizedStateMachineDebuggerHiddenMethod;
            return synthesizedStateMachineDebuggerHiddenMethod;
        }

        protected MethodSymbol OpenPropertyImplementation(MethodSymbol getterToImplement)
        {
            SynthesizedStateMachineProperty synthesizedStateMachineProperty = new SynthesizedStateMachineProperty(getterToImplement, (StateMachineTypeSymbol)F.CurrentType);
            F.ModuleBuilderOpt!.AddSynthesizedDefinition(F.CurrentType, synthesizedStateMachineProperty.GetCciAdapter());
            MethodSymbol getMethod = synthesizedStateMachineProperty.GetMethod;
            F.ModuleBuilderOpt!.AddSynthesizedDefinition(F.CurrentType, getMethod.GetCciAdapter());
            F.CurrentFunction = getMethod;
            return getMethod;
        }

        protected SynthesizedImplementationMethod OpenMoveNextMethodImplementation(MethodSymbol methodToImplement)
        {
            SynthesizedStateMachineMoveNextMethod synthesizedStateMachineMoveNextMethod = new SynthesizedStateMachineMoveNextMethod(methodToImplement, (StateMachineTypeSymbol)F.CurrentType);
            F.ModuleBuilderOpt!.AddSynthesizedDefinition(F.CurrentType, synthesizedStateMachineMoveNextMethod.GetCciAdapter());
            F.CurrentFunction = synthesizedStateMachineMoveNextMethod;
            return synthesizedStateMachineMoveNextMethod;
        }

        protected BoundExpression MakeCurrentThreadId()
        {
            PropertySymbol propertySymbol = (PropertySymbol)F.WellKnownMember(WellKnownMember.System_Environment__CurrentManagedThreadId, isOptional: true);
            if ((object)propertySymbol != null)
            {
                MethodSymbol getMethod = propertySymbol.GetMethod;
                if ((object)getMethod != null)
                {
                    return F.Call(null, getMethod);
                }
            }
            return F.Property(F.Property(WellKnownMember.System_Threading_Thread__CurrentThread), WellKnownMember.System_Threading_Thread__ManagedThreadId);
        }

        protected SynthesizedImplementationMethod GenerateIteratorGetEnumerator(MethodSymbol getEnumeratorMethod, ref BoundExpression managedThreadId, int initialState)
        {
            SynthesizedImplementationMethod result = OpenMethodImplementation(getEnumeratorMethod);
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance();
            LocalSymbol resultVariable = F.SynthesizedLocal(stateMachineType);
            BoundStatement boundStatement = F.Assignment(F.Local(resultVariable), F.New(stateMachineType.Constructor, F.Literal(initialState)));
            GeneratedLabelSymbol label = F.GenerateLabel("thisInitialized");
            if ((object)initialThreadIdField != null)
            {
                managedThreadId = MakeCurrentThreadId();
                ArrayBuilder<BoundStatement> instance2 = ArrayBuilder<BoundStatement>.GetInstance(4);
                GenerateResetInstance(instance2, initialState);
                instance2.Add(F.Assignment(F.Local(resultVariable), F.This()));
                if (method.IsStatic || method.ThisParameter.Type.IsReferenceType)
                {
                    instance2.Add(F.Goto(label));
                }
                boundStatement = F.If(F.LogicalAnd(F.IntEqual(F.Field(F.This(), stateField), F.Literal(-2)), F.IntEqual(F.Field(F.This(), initialThreadIdField), managedThreadId)), F.Block(instance2.ToImmutableAndFree()), boundStatement);
            }
            instance.Add(boundStatement);
            Dictionary<Symbol, CapturedSymbolReplacement> dictionary = initialParameters;
            IReadOnlyDictionary<Symbol, CapturedSymbolReplacement> readOnlyDictionary = nonReusableLocalProxies;
            if (!method.IsStatic && readOnlyDictionary.TryGetValue(method.ThisParameter, out var value))
            {
                instance.Add(F.Assignment(value.Replacement(F.Syntax, (NamedTypeSymbol stateMachineType) => F.Local(resultVariable)), dictionary[method.ThisParameter].Replacement(F.Syntax, (NamedTypeSymbol stateMachineType) => F.This())));
            }
            instance.Add(F.Label(label));
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (readOnlyDictionary.TryGetValue(current, out var value2))
                {
                    BoundExpression resultParameter = value2.Replacement(F.Syntax, (NamedTypeSymbol stateMachineType) => F.Local(resultVariable));
                    BoundExpression parameterProxy = dictionary[current].Replacement(F.Syntax, (NamedTypeSymbol stateMachineType) => F.This());
                    BoundStatement item = InitializeParameterField(getEnumeratorMethod, current, resultParameter, parameterProxy);
                    instance.Add(item);
                }
            }
            instance.Add(F.Return(F.Local(resultVariable)));
            F.CloseMethod(F.Block(ImmutableArray.Create(resultVariable), instance.ToImmutableAndFree()));
            return result;
        }

        protected virtual void GenerateResetInstance(ArrayBuilder<BoundStatement> builder, int initialState)
        {
            builder.Add(F.Assignment(F.Field(F.This(), stateField), F.Literal(initialState)));
        }

        protected virtual BoundStatement InitializeParameterField(MethodSymbol getEnumeratorMethod, ParameterSymbol parameter, BoundExpression resultParameter, BoundExpression parameterProxy)
        {
            return F.Assignment(resultParameter, parameterProxy);
        }

        protected bool CanGetThreadId()
        {
            if ((object)F.WellKnownMember(WellKnownMember.System_Threading_Thread__ManagedThreadId, isOptional: true) == null)
            {
                return (object)F.WellKnownMember(WellKnownMember.System_Environment__CurrentManagedThreadId, isOptional: true) != null;
            }
            return true;
        }
    }
}
